<?php

/**
 * PHP Proxy Client
 *
 * Version 1.1 beta
 * See https://github.com/Esri/resource-proxy for more information.
 *
 */

$version = "1.1 Beta";

error_reporting(0);

class Proxy {

    /**
     * Holds proxy configuration parameters
     *
     * @var ProxyConfig
     */

    public $proxyConfig;

    /**
     * Holds the referer associated with client request
     *
     * @var string
     */

    public $referer;

    /**
     * Holds a collection of server urls listed in configuration (keys same as keys in config, but lowercase).
     *
     * @var array
     */

    public $serverUrls;

    /**
     * Log object used for writing messages
     *
     * @var ProxyLog
     */

    public $proxyLog;

    /**
     * Meter object used for rate metering
     *
     * @var RateMeter
     */

    public $meter;

    /**
     * Holds headers returned by the proxied resource
     *
     * @var array
     */

    public $headers = array();

    /**
     * cURL resource used to send HTTP requests
     *
     * @var resource
     */

    public $ch;

    /**
     * Holds the action associated with the request (post, get)
     *
     * @var string
     */

    public $proxyMethod;

    /**
     * Holds the URL being requested
     *
     * @var string
     */

    public $proxyUrl;

    /**
     * Holds the query string associated with the request
     *
     * @var string
     */

    public $queryString;

    /**
     * Holds the query string with Url and all the Data with key values
     *
     * @var string
     */

    public $proxyUrlWithData;

    /**
     * Holds the data to be sent with a post request
     *
     * @var array
     */

    public $proxyData;

    /**
     * Holds the resource being requested (keys are the same keys in the serverUrl config file, except lowercase)
     *
     * @var array
     */

    public $resource;

    /**
     * Number of allowed attempts to get a new token
     *
     * @var int
     */

    public $allowedAttempts = 3;

    /**
     * Attempt count when getting a new token
     *
     * @var int
     */

    public $attemptsCount = 0;

    /**
     * Property indicating if an attempt has been made to get the token from the application session.
     *
     * @var boolean
     */

    public $sessionAttempt = false;

    /**
     * Holds URL which is used in creating the session key
     *
     * @var string
     */

    public $sessionUrl;

    /**
     * Allowed application urls array is just an array of urls
     *
     * @var array
     */

    public $allowedReferers;

    /**
     * Holds the response
     *
     * @var resource
     */

    public $response;

    /**
     * Holds the response body following curl request
     *
     * @var String
     */

    public $proxyBody;

    /**
     * Holds a field to help debug booleans
     *
     * @var array
     */

    public $bool = array(true=>"True", false=>"False");

    /**
     * Holds staged file path.
     *
     * @var string
     */

    private  $unlinkPath;
    
    /**
     * Holds a cloned copy of the resource response
     *
     * @var string
     */
    
    public $responseClone;
    
    /**
     * Holds headers sent by the client
     *
     * @var array
    */
    
    public $clientRequestHeaders;
    
    /**
     * Holds content length of last request
     *
     * @var int
     */
    
    
    public $contentLength;



    public function __construct($configuration, $log) {


        $this->proxyLog = $log;

        $this->proxyConfig = $configuration->proxyConfig;

        $this->serverUrls = $configuration->serverUrls;

        $this->setupSession();
        
        $this->getIncomingHeaders();

        $this->setupClassProperties();

        $this->checkEmptyParameters();
        
        $this->checkForPing();

        if ($this->proxyConfig['mustmatch'] != null && $this->proxyConfig['mustmatch'] == true || $this->proxyConfig['mustmatch'] == "true") {

            if($this->isAllowedApplication() == false){

                $this->allowedApplicationError();

            }

            $this->verifyConfiguration();

            if ($this->meter->underMeterCap()) {

                $this->runProxy();

            } else {

                $this->rateMeterExceededError();

            }

        } else if($this->proxyConfig['mustmatch'] != null && $this->proxyConfig['mustmatch'] == false) {

            $this->runProxy();

        }else{

            $this->configurationParameterError();

        }

    }

    public function setupSession()
    {
        if (!isset($_SESSION)) {

            session_start();

        }

    }
    
    public function getIncomingHeaders()
    {
        $headers = null;
    
        if (!function_exists('getallheaders'))
        {
            $headers = array();
    
            foreach ($_SERVER as $key => $value)
            {
    
                if (substr($key,0,5)=="HTTP_") {
                     
                    $key = str_replace(" ","-",ucwords(strtolower(str_replace("_"," ",substr($key,5)))));
    
                    $headers[$key] = $value;
    
                }
            }
    
        }else{
    
            $headers = getallheaders();
    
        }
    
        $this->clientRequestHeaders = $headers;
    
    }

    public function makeDirectory($dir, $mode = 0777) //Not implemented.
    {
        if (is_dir($dir) || @mkdir($dir,$mode)) return true;

        if (!$this->makeDirectory(dirname($dir),$mode)) return false;

        return @mkdir($dir,$mode);

    }

    public function verifyConfiguration()
    {
        if (!isset($this->resource['ratelimit']) || !isset($this->resource['ratelimitperiod'])) {

            $this->resource['ratelimit'] = null;

            $this->resource['ratelimitperiod'] = null;
        }

        if ($this->canProcessRequest()) {

            $this->meter = new RateMeter($this->resource['url'], $this->resource['ratelimit'], $this->resource['ratelimitperiod'], $this->proxyLog); //need to pass meter interval and meter cap

        } else {

            $this->canProcessRequestError();

        }
    }

    public function configurationParameterError()
    {

        $this->proxyLog->log("Malformed 'mustMatch' property in configuration file");

        header('Status: 412', true, 412);

        header('Content-Type: application/json');

        $configError = array(
                "error" => array("code" => 412,
                        "details" => array("Detected malformed 'mustMatch' property in the configuration file. The server does not meet one of the preconditions that the requester put on the request."),
                        "message" => "Proxy failed due to configuration error."
                ));

        echo json_encode($configError);

        exit();
    }

    public function rateMeterExceededError()
    {

        $this->proxyLog->log("Rate meter exceeded by " . $_SERVER['REMOTE_ADDR']);

        header('Status: 402', true, 402);

        header('Content-Type: application/json');

        $exceededError = array(
                "error" => array("code" => 402,
                "details" => array("This is a metered resource, number of requests have exceeded the rate limit interval."),
                "message" => "Unable to proxy request for requested resource."
        ));

        echo json_encode($exceededError);

        exit();
    }

    public function canProcessRequestError()
    {

        $this->proxyLog->log("Proxy could not resolve requested url - " . $this->proxyUrl . ".  Possible solution would be to update 'mustMatch', 'matchAll' or 'url' property in the configuration file.");

        header('Status: 403', true, 403);

        header('Content-Type: application/json');

        $configError = array(
                "error" => array("code" => 403,
                        "details" => array("Proxy has not been set up for this URL. Make sure there is a serverUrl in the configuration file that matches: " . $this->proxyUrl),
                        "message" => "Proxy has not been set up for this URL."
                ));

        echo json_encode($configError);

        exit();
    }

    public function allowedApplicationError()
    {

        header('Status: 402', true, 402);

        header('Content-Type: application/json');

        $allowedApplicationError = array(
                "error" => array("code" => 402,
                "details" => array("This is a protected resource.  Application access is restricted."),
                "message" => "Application access is restricted.  Unable to proxy request."
        ));

        echo json_encode($allowedApplicationError);

        exit();
    }

    public function checkEmptyParameters()
    {
        if(empty($this->proxyUrl)) {  // nothing to proxy
            $this->emptyParametersError();
        }
    }

    public function checkForPing()
    {
        if($this->proxyUrl == "ping") {
            $this->proxyLog->log("Pinged");

            header('Status: 200', true, 200);
            header('Content-Type: application/json');
            
            $curl_version = curl_version();
            $pngMsg = array(
                        "Proxy Version"      => $GLOBALS['version'],
                        // "PHP Version"        => phpversion(),
                        // "Curl Version"       => $curl_version[version],
                        "Configuration File" => "OK", // or it would have failed in XmlParser()
                        "Log File"           => "OK"  // or it would have failed in configurationParameterError()
                    );

            echo json_encode($pngMsg);
            exit();
        }
    }

    public function emptyParametersError()
    {
        $message = "This proxy does not support empty parameters.";
        $this->proxyLog->log("$message");

        header('Status: 400', true, 400);

        header('Content-Type: application/json');

        $configError = array(
                "error" => array("code" => 400,
                    "details" => array("$message"),
                    "message" => "$message"
                ));

        echo json_encode($configError);

        exit();
    }

    public function setProxyHeaders()
    {
        
        $header_size = curl_getinfo($this->ch, CURLINFO_HEADER_SIZE); //cURL will go null after this 

        $header_content = trim(substr($this->response,0, $header_size));
        
        $header_array = $this->parse_resource_headers($header_content);
        
        foreach ($header_array as $key => $value) {
        
            if(is_string($key)){
                
                $header = sprintf("%s: %s", $key, $value);
                
                $this->headers[] = $header;
                
            }
        
        }

    }

    public function setResponseBody()
    {

        $this->proxyBody = substr($this->responseClone, $this->contentLength);
        
    }
    
    public function parse_resource_headers($raw_headers) //Takes the cURL response header (from the resource) and parses it into array
    {
        $headers = array();  //Thanks to this http://stackoverflow.com/questions/6368574/how-to-get-the-functionality-of-http-parse-headers-without-pecl
    
        $key = '';
    
        foreach(explode("\n", $raw_headers) as $i => $h) {
    
            $h = explode(':', $h, 2);
    
            if (isset($h[1])) {
    
                if (!isset($headers[$h[0]]))
    
                    $headers[$h[0]] = trim($h[1]);
    
                elseif (is_array($headers[$h[0]])) {
    
                    $headers[$h[0]] = array_merge($headers[$h[0]], array(trim($h[1])));
    
                } else {
    
                    $headers[$h[0]] = array_merge(array($headers[$h[0]]), array(trim($h[1])));
                }
    
                $key = $h[0];
    
            } else {
    
                if (substr($h[0], 0, 1) == "\t"){
    
                    $headers[$key] .= "\r\n\t".trim($h[0]);
    
                } elseif (!$key){
    
                    $headers[0] = trim($h[0]);
    
                }
            }
        }
    
        return $headers;
    }

    public function getResponse()
    {
        //Remove built in PHP headers
        

        foreach(headers_list() as $key => $value)
        {
            $pos = strripos($value, ":");
            
            $header_type = substr($value,0,$pos);
            
            if ($this->contains($value, "Cookie")) { //Don't remove the PHP session cookie
                 
                continue;
            
            }
            
            header_remove($header_type);
            
        }
        
        //Remove scenario causing provisional header error message
        
        foreach ($this->headers as $key => $value) {
            
            if ($this->contains($value, "Transfer-Encoding: chunked")) { //See issue #75
                 
                continue;
            
            }
            
            header($value, false);
  
        }

        header("Content-length: " . strlen($this->proxyBody)); //Issue 190 with truncated response, not sure how to gzip the data (or keep gzip via CURLOPT_ENCODING) without extension.
        
        echo $this->proxyBody;
        
        $this->proxyLog->log("Proxy complete");

        $this->proxyConfig = null;

        $this->meter = null;

        $this->proxyLog = null;

        exit();
    }


    public function setupClassProperties()
    {
        $this->decodeCharacterEncoding(); // Sanitize url being proxied and removing encodings if present

        try {

            if (!empty($_POST) && empty($_FILES)) { // Is it a POST without files?

                $this->proxyLog->log('POST detected');

                $this->proxyUrl = $_SERVER['QUERY_STRING'];

                $this->proxyData = $_POST;

                $this->proxyMethod = "POST";

            } else if (!empty($_POST) && !empty($_FILES)) { // Is it a POST with files?

                $this->proxyLog->log('FILES detected');

                $this->proxyUrl = $_SERVER['QUERY_STRING'];

                $this->proxyData = $_POST;

                $this->proxyMethod = "FILES";

            } else if (empty($_POST) && empty($_FILES)) { // It must be a GET!

                $this->proxyLog->log('GET detected');

                $p = preg_split("/\?/", $_SERVER['QUERY_STRING']); // Finds question marks in query string

                $this->proxyUrl = $p[0];

                $this->proxyUrlWithData = $_SERVER['QUERY_STRING'];

                $this->proxyMethod = "GET";
            }

        } catch (Exception $e) {


            $this->proxyLog->log("Proxy could not detect request method action type (POST, GET, FILES).");
        }

    }
    
    public function decodeCharacterEncoding()
    {
        $hasHttpEncoding = $this->startsWith($_SERVER['QUERY_STRING'], 'http%3a%2f%2f');
        
        $hasHttpsEncoding = $this->startsWith($_SERVER['QUERY_STRING'], 'https%3a%2f%2f');
        
        if($hasHttpEncoding || $hasHttpsEncoding){
            
            $_SERVER['QUERY_STRING'] = urldecode($_SERVER['QUERY_STRING']); //Remove encoding from GET requests
            
            foreach($_POST as $k => $v) {
                
                $_POST[$k] = urldecode($v);  //Remove encoding for each POST value
                
            }
            
        }
    }

    public function formatWithPrefix($url)
    {
        if(substr($url, 0, 4) != "http"){

            if(substr($this->proxyUrl, 0, 5) == "https") {

                $url = "https://" . $url;

            }else{

                $url = "http://" . $url;
            }

        }

        return $url;
    }

    public function removeTrailingSlash($url)
    {
        if (substr($url, -1) == '/')
        {
            $url = rtrim($url, "/");
        }

        return $url;

    }

    public function resolveDoubleSlashCondition($url)
    {
        if (substr($url, 0, strlen("//")) == "//") {

            $url = substr($url, strlen("//"));
        }

        return $url;

    }

    public function sanitizeUrl($url)
    {
        $url = $this->resolveDoubleSlashCondition($url);

        $url = $this->formatWithPrefix($url);

        $url = $this->removeTrailingSlash($url);

        return $url;

    }

    public function canProcessRequest()
    {
        $canProcess = false;

        if ($this->proxyConfig['mustmatch'] == false || $this->proxyConfig['mustmatch'] === "false" || $this->proxyConfig['mustmatch'] == true || $this->proxyConfig['mustmatch'] == "true") {

            //check with listed serverurl regardless if mustMatch is true or false
            foreach ($this->serverUrls as $key => $value) {
                
                $serverUrl = $value['serverurl'][0];
                $serverUrl['url'] = $this->sanitizeUrl($serverUrl['url']); //Do all the URL cleanups and checks at once
                
                if(is_string($serverUrl['matchall'])){
                    $serverUrl['matchAll'] = strtolower($serverUrl['matchall']);
                }

                if ($serverUrl['matchall'] == true || $serverUrl['matchall'] === "true") {

                    $urlStartsWith = $this->startsWith($this->proxyUrl, $serverUrl['url']);

                    if ($urlStartsWith){

                        $this->resource = $serverUrl;

                        $this->sessionUrl = $serverUrl['url'];

                        $canProcess = true;

                        return $canProcess;

                    }

                } else if ($serverUrl['matchall'] == false || $serverUrl['matchall'] === "false"){

                    $isEqual = $this->equals($this->proxyUrl, $serverUrl['url']);

                    if($isEqual){

                        $this->resource = $serverUrl;

                        $this->sessionUrl = $serverUrl['url'];

                        $canProcess = true;

                        return $canProcess;
                    }

                }
            }

            if ($this->proxyConfig['mustmatch'] == false || $this->proxyConfig['mustmatch'] == "false") $canProcess = true; //if not found and mustMatch is false, then canProcess is true

        } else {

            $this->proxyLog->log("Proxy has failed. Review configuration file for errors.");

            $canProcess = false;
        }

        return $canProcess;
    }


    public function getRequestConfig()
    {

        return $this->requestConfig;

    }

    public function useSessionToken()
    {

        $sessonKey = 'token_for_' . $this->sessionUrl;

        $sessonKey = sprintf("'%s'", $sessonKey);

        if(isset($_SESSION[$sessonKey])) //Try to get token from session
        {

            $token = $_SESSION[$sessonKey];

            $this->appendToken($token);

            $this->sessionAttempt = true;

            $this->proxyLog->log("Using session token");

        }
    }

    public function runProxy()
    {

        $this->useSessionToken();

        if($this->proxyMethod == "FILES"){

            $this->proxyFiles();


        }else if($this->proxyMethod == "POST"){

            $this->proxyPost();

        }else if($this->proxyMethod == "GET"){

            $this->proxyGet();

        }

        $isUnauthorized = $this->isUnauthorized();

        if($isUnauthorized === true) {

            if($this->attemptsCount < $this->allowedAttempts) {

                $this->attemptsCount++;

                $this->proxyLog->log("Retry attempt " . $this->attemptsCount . " of " . $this->allowedAttempts);

                $token = $this->getNewTokenIfCredentialsAreSpecified();

                if(!empty($token) || $token != null) {

                    $this->addTokenToSession($token);

                    $this->appendToken($token);
                }

                if($this->attemptsCount == $this->allowedAttempts) {

                    $this->proxyLog->log("Removing session value");

                    $sessonKey = 'token_for_' . $this->sessionUrl;

                    $sessonKey = sprintf("'%s'", $sessonKey);

                    unset($_SESSION[$sessonKey]);  //Remove token from session
                }

                $this->runProxy();
            }

        } else {

            $this->proxyLog->log("Ok to proxy");
        }

        return true;
    }

    public function isUnauthorized()
    {

        $isUnauthorized = false;

        $jsonData = json_decode($this->proxyBody);

        if (strpos($this->proxyBody,'"code":499') !== false) {

            $isUnauthorized = true;

        }

        if (strpos($this->proxyBody,'"code":498') !== false) {

            $isUnauthorized = true;

        }

        if (strpos($this->proxyBody,'"code":403') !== false) {
        
            $isUnauthorized = true;
        
        }

        $errorCode = $jsonData->{'error'}->{'code'};

        if($errorCode == 499 || $errorCode == 498 || $errorCode == 403)
        {
            $isUnauthorized = true;

        }

        if($isUnauthorized){

            $this->proxyLog->log("Authorization failed : " . $this->proxyBody);

        }

        return $isUnauthorized;
    }

    private function appendToken($token)
    {
        if($this->proxyMethod == 'POST' || $this->proxyMethod == 'FILES')
        {

            if(array_key_exists("token", $this->proxyData))
            {
                $this->proxyData["token"] = $token;

            }else{

                $appendedToken = array_merge($this->proxyData, array("token" => $token));

                $this->proxyData = $appendedToken;

            }

        }else{

            $pos = strripos($this->proxyUrlWithData, "&token=");

            if($pos > 0)
            {

                $this->proxyUrlWithData = substr($this->proxyUrlWithData,0,$pos) . "&token=" . $token; //Remove old tokens.

            }else{

                $this->proxyUrlWithData = $this->proxyUrlWithData . "&token=" . $token;

            }

        }
    }

    public function initCurl()
    {
        $headers = array('Expect:', 'Referer: ' . $this->referer);

        $this->ch = curl_init();

        curl_setopt($this->ch, CURLOPT_HTTPHEADER, $headers);

        curl_setopt($this->ch, CURLOPT_SSL_VERIFYPEER , false);

        curl_setopt($this->ch, CURLOPT_SSL_VERIFYHOST , false);

        curl_setopt($this->ch, CURLOPT_HEADER, true);
        
        curl_setopt($this->ch, CURLOPT_FOLLOWLOCATION, true);

    }
    
    public function curlError()
    {
        // see full of cURL error codes at http://curl.haxx.se/libcurl/c/libcurl-errors.html
    
        $message = "cURL error (" . curl_errno($this->ch) . "): "
                . curl_error($this->ch) . ".";
    
        $this->proxyLog->log($message);
    
        header('Status: 502', true, 502);  // 502 Bad Gateway -  The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.
    
        header('Content-Type: application/json');
    
        $configError = array(
                "error" => array("code" => 502,
                        "details" => array($message),
                        "message" => "Proxy failed due to curl error."
                ));
    
        echo json_encode($configError);
    
        curl_close($this->ch);
    
        $this->ch = null;
    
        exit();
    }

    public function proxyGet() {

        $this->response = null;

        try {

            $this->initCurl();

            curl_setopt($this->ch, CURLOPT_HTTPGET, true);

            curl_setopt($this->ch, CURLOPT_RETURNTRANSFER, true);

            curl_setopt($this->ch, CURLOPT_URL, $this->proxyUrlWithData);

            $this->response = curl_exec($this->ch);
            
            $this->responseClone = $this->response;
            
            $this->contentLength = curl_getinfo($this->ch,CURLINFO_HEADER_SIZE);

            if(curl_errno($this->ch) > 0 || empty($this->response))
            {
                $this->curlError();

            }else{

                $this->setProxyHeaders();

                $this->setResponseBody();

            }

            curl_close($this->ch);

            $this->ch = null;


        } catch (Exception $e) {

            $this->proxyLog->log($e->getMessage());
        }

        return;
    }

    public function proxyPost($url, $params)
    {
        $this->response = null;

        $this->headers = null;

        $this->proxyBody = null;

        if(empty($url) || $url == null || empty($params) || $url == $params){ //If no $url or $params passed, default to class property values

            $url = $this->proxyUrl;

            $params = $this->proxyData;

        }

        try {

            $this->initCurl();

            curl_setopt($this->ch, CURLOPT_URL, $url);

            curl_setopt($this->ch, CURLOPT_RETURNTRANSFER, true);

            curl_setopt($this->ch, CURLOPT_POST, true);

            if(is_array($params)){ //If $params is array, convert it to a curl query string like 'image=png&f=json'
                curl_setopt($this->ch, CURLOPT_POSTFIELDS, http_build_query($params));
            } else {
                curl_setopt($this->ch, CURLOPT_POSTFIELDS, $params);
            }

            $this->response = curl_exec($this->ch);
            
            $this->responseClone = $this->response;
            
            $this->contentLength = curl_getinfo($this->ch,CURLINFO_HEADER_SIZE);

        } catch (Exception $e) {

            $this->proxyLog->log($e->getMessage());
        }

        if(curl_errno($this->ch) > 0 || empty($this->response))
        {
            $this->curlError();

        }else{

            $this->setProxyHeaders();

            $this->setResponseBody();
        }

        curl_close($this->ch);

        $this->ch = null;

        return;
    }

    public function proxyFiles()
    {
        try {

            if (count($this->proxyData))
            {
                $query_array = array();

                foreach ($this->proxyData as $pkey => $pvalue)
                {
                    $query_array[$pkey] = $pvalue;

                    foreach ($_FILES as $key => $file)
                    {
                        $parts = pathinfo($file["tmp_name"]);

                        $this->unlinkPath = $parts["dirname"] . DIRECTORY_SEPARATOR . $file["name"];

                        rename($file["tmp_name"], $this->unlinkPath);

                        $this->proxyData[$key] = "@" . $this->unlinkPath;

                        $query_array[$key] = "@" . $this->unlinkPath;

                    }

                }

            }

            $this->initCurl();

            curl_setopt($this->ch, CURLOPT_URL, $this->proxyUrl);

            curl_setopt($this->ch, CURLOPT_RETURNTRANSFER, true);

            curl_setopt($this->ch, CURLOPT_POST, true);

            curl_setopt($this->ch, CURLOPT_POSTFIELDS, $query_array);

            $this->response = curl_exec($this->ch);
            
            $this->responseClone = $this->response;
            
            $this->contentLength = curl_getinfo($this->ch,CURLINFO_HEADER_SIZE);

            if(curl_errno($this->ch) > 0 || empty($this->response))
            {
                $this->curlError();
            }else{

                $this->setProxyHeaders();

                $this->setResponseBody();

                if($this->isUnauthorized() == true) {

                    $this->proxyLog->log("Unlinking: " . $this->unlinkPath);

                    unlink($this->unlinkPath);

                }

            }

            curl_close($this->ch);

            $this->ch = null;

            return;

        } catch (Exception $e) {

            $this->proxyLog->log($e->getMessage());
        }

    }

    function startsWith($requested, $needed)
    {

        return stripos($requested, $needed) === 0;

    }

    function contains($haystack, $needle)
    {

        return stripos($haystack, $needle) !== false ? true : false;

    }

    function equals($string, $anotherstring)
    {

        return $string == $anotherstring;

    }


    public function isUserLogin()
    {

        if (isset($this->resource['username']) && isset($this->resource['password'])) {

            return true;
        }

        return false;
    }

    public function isAppLogin()
    {

        if (isset ( $this->resource['clientid']) && isset ($this->resource['clientsecret'])) {

            return true;

        }

        return false;
    }

    public function exchangePortalTokenForServerToken($portalToken) {

        $this->proxyLog->log("Exchanging portal token for server-specific token for " . $this->resource['url']);

        $pos = strripos($this->resource['oauth2endpoint'], "/oauth2");

        $exchangeUri = substr($this->resource['oauth2endpoint'],0,$pos) . "/generateToken";

        $this->proxyPost($exchangeUri, array(
                'token' => $portalToken,
                'serverURL' => $this->resource['url'],
                'f' => 'json'
        ));

        $tokenResponse = json_decode($this->proxyBody, true);

        $token = $tokenResponse['token'];

        return $token;


    }

    function getNewTokenIfCredentialsAreSpecified() {

        $this->sessionUrl = $this->resource['url']; //Store url in local variable because later we may tweak url

        $token = null;

        $isUserLogin = $this->isUserLogin();

        $isAppLogin = $this->isAppLogin();

        if ($isUserLogin || $isAppLogin) {

            if ($isAppLogin) {

                $token = $this->doAppLogin();

            } else if($isUserLogin) {

                $token = $this->doUserPasswordLogin();
            }

        }else{

            $this->proxyLog->log("Can not determine if OAuth or ArcGIS Server means of authentication.  Check config for errors.");
        }

        return $token;
    }


     public function addTokenToSession($token) {

        $sessonKey = 'token_for_' . $this->sessionUrl;

        $sessonKey = sprintf("'%s'", $sessonKey);

        try {

            $this->proxyLog->log('Adding token to session');

            $_SESSION[$sessonKey] = $token;

        }catch(Exception $e){

            $this->proxyLog->log("Error setting session: " . $e);
        }

     }


    public function doUserPasswordLogin() {

        $this->proxyLog->log("Resource using ArcGIS Server security");

        $tokenServiceUri = $this->getTokenEndpoint();

        $this->proxyPost($tokenServiceUri, array (
                'request' => 'getToken',
                'f' => 'json',
                'referer' => $this->referer,
                'expiration' => 60,
                'username' => $this->resource['username'],
                'password' => $this->resource['password']
        ));

        $tokenResponse = json_decode($this->proxyBody, true);

        $token = $tokenResponse['token'];

        return $token;
    }

    public function getTokenEndpoint()
    {
        if ($this->contains($this->proxyUrl, "/rest/") !== false){
            
            $position = stripos($this->proxyUrl, "/rest/");
            
            $infoUrl = substr($this->proxyUrl,0,$position) . "/rest/info";
        
        } else if ($this->contains($this->proxyUrl, "/sharing/") !== false){

            $position = stripos($this->proxyUrl, "/sharing/");
            
            $infoUrl = substr($this->proxyUrl,0,$position) . "/sharing/rest/info";    

        }else{

            $infoUrl = $this->resource['url'] . "/arcgis/rest/info";
        }

        $this->proxyPost($infoUrl,array('f' => 'json'));

        $infoResponse = json_decode($this->proxyBody, true);

        $tokenServiceUri = $infoResponse['authInfo']['tokenServicesUrl'];

        if(!empty($tokenServiceUri)) {

            $this->proxyLog->log("Got token endpoint");

        }else{

            $this->proxyLog->log("Unable to get token endpoint");
        }

        return $tokenServiceUri;
    }



    public function doAppLogin()
    {
        $this->resource['oauth2endpoint'] = isset($this->resource['oauth2endpoint']) ? $this->resource['oauth2endpoint'] : "https://arcgis.com/sharing/oauth2/";

        if (substr($this->resource['oauth2endpoint'], -1) != '/')
        {
            $this->resource['oauth2endpoint'] = $this->resource['oauth2endpoint'] . "/";
        }

        $this->proxyLog->log("Resource using OAuth");

        $this->proxyPost($this->resource['oauth2endpoint'] . "token", array(
                'client_id' => $this->resource['clientid'],
                'client_secret' => $this->resource['clientsecret'],
                'grant_type' => 'client_credentials',
                'f' => 'json'
        ));

        $tokenResponse = json_decode($this->proxyBody, true);

        $token = $tokenResponse['access_token'];

        if (!empty($token))
        {
            $token = $this->exchangePortalTokenForServerToken($token);
        }

        return $token;
    }


    public function isAllowedApplication()
    {

        if(in_array("*",$this->proxyConfig['allowedreferers'])){

            $this->referer = $_SERVER['SERVER_NAME']; //This is to enable browser testing when * is used

            $isAllowedApplication = true;

            return $isAllowedApplication;

        }else{

            $this->referer = $_SERVER['HTTP_REFERER'];

        }

        $isAllowedApplication = false;

        if (in_array($this->referer, $this->proxyConfig['allowedreferers'])) {

            $isAllowedApplication = true;

        }else{

            $this->proxyLog->log("Attempt made to use this proxy from " . $this->referer . " and " . $_SERVER['REMOTE_ADDR']);

        }

        return $isAllowedApplication;

    }

    function __destruct()
    {

    }
}




class ProxyLog {

    public $timeFormat = 'm-d-y H:i:s';

    public $seperator = ' | ';

    public $eol = "\r\n";

    public $indent = " ";

    public $proxyConfig;


    public function __construct($configuration = null) {

        if($configuration != null){

            $this->proxyConfig = $configuration->proxyConfig;

            $this->addLogLevel();

            if($this->proxyConfig['loglevel'] != 3){

                $this->attemptWriteToLog();
            }

        }else{

            throw new Exception ('Problem creating log.');
        }

    }

     private function addLogLevel()
     {

        if(empty($this->proxyConfig['logfile'])) {

            $this->proxyConfig['logfile'] = null;

            $this->proxyConfig['loglevel'] = 3; //Turns off logging
        }

        if(!empty($this->proxyConfig['logfile']) && empty($this->proxyConfig['loglevel'])) {

            $this->proxyConfig['loglevel'] = 0; //Turns on logging

        }

     }


    public function write($m)
    {

        if (isset($this->proxyConfig['logfile'])) {

            try {

                $fh = null;

                $fh = fopen($this->proxyConfig['logfile'], (file_exists($this->proxyConfig['logfile'])) ? 'a' : 'w');

                if (is_writable($this->proxyConfig['logfile'])) {

                    fwrite($fh, $this->eol);

                    fwrite($fh, $this->getTime());

                    //fwrite($fh, $this->seperator);

                    //fwrite($fh, $_SERVER['HTTP_REFERER']);

                    fwrite($fh, $this->seperator);

                    fwrite($fh, $m);

                }else{

                    header('Status: 200', true, 200);

                    header('Content-Type: application/json');

                    $configError = array(
                            "error" => array("code" => 412,
                                    "details" => array("Detected malformed 'logFile' in the configuration file.  Make sure this app has write permissions to log file specified in the configuration file.  The server does not meet one of the preconditions that the requester put on the request."),
                                    "message" => "Proxy failed due to configuration error."
                            ));

                    echo json_encode($configError);

                    exit();

                }

                fclose($fh);

            } catch (Exception $e) {

                $this->log($e->getMessage());

            }
        } else {

            header('Status: 200', true, 200);

            header('Content-Type: application/json');

            $configError = array(
                    "error" => array("code" => 412,
                            "details" => array("Detected malformed 'logFile' in the configuration file.  Make sure this app has write permissions to log file specified in the configuration file.  The server does not meet one of the preconditions that the requester put on the request."),
                            "message" => "Proxy failed due to configuration error."
                    ));

            echo json_encode($configError);

            exit();

        }
    }

    public function attemptWriteToLog()
    {

        if ($this->proxyConfig['loglevel'] == 0 || $this->proxyConfig['loglevel'] == 2) {

            if (isset($this->proxyConfig['logfile'])) {

                try {

                    $fh = null;

                    $fh = fopen($this->proxyConfig['logfile'], (file_exists($this->proxyConfig['logfile'])) ? 'a' : 'w');

                    if (is_writable($this->proxyConfig['logfile'])) {

                        fwrite($fh, $this->eol);

                        fwrite($fh, ' ');

                    }else{

                        header('Status: 200', true, 200);

                        header('Content-Type: application/json');

                        $configError = array(
                                "error" => array("code" => 412,
                                        "details" => array("Detected malformed 'logFile' in the configuration file.  Make sure this app has write permissions to log file specified in the configuration file.  The server does not meet one of the preconditions that the requester put on the request."),
                                        "message" => "Proxy failed due to configuration error."
                                ));

                        echo json_encode($configError);

                        exit();

                    }

                    fclose($fh);

                } catch (Exception $e) {

                    header('Status: 200', true, 200);

                    header('Content-Type: application/json');

                    $configError = array(
                            "error" => array("code" => 412,
                                    "details" => array("Could not write to log file.  Make sure this app has write permissions to log file specified in the configuration file.  The server does not meet one of the preconditions that the requester put on the request."),
                                    "message" => "Proxy failed due to configuration error."
                            ));

                    echo json_encode($configError);

                    exit();

                }
            } else {

                header('Status: 200', true, 200);

                header('Content-Type: application/json');

                $configError = array(
                        "error" => array("code" => 412,
                                "details" => array("Detected malformed 'logFile' in the configuration file.  Make sure this app has write permissions to log file specified in the configuration file.  The server does not meet one of the preconditions that the requester put on the request."),
                                "message" => "Proxy failed due to configuration error."
                        ));

                echo json_encode($configError);

                exit();

            }

        }

    }


    public function log($message)
    {

        if ($this->proxyConfig['loglevel'] == 0) {

            $this->write($message); //Writes messages and errors to logs

        } elseif ($this->proxyConfig['loglevel'] == 1) {

            echo $message; //Show proxy errors and messages in browser console (should only be used when looking for errors)

        } elseif ($this->proxyConfig['loglevel'] == 2) {

            $this->write($message);  //Writes messages and errors to logs

            echo $message; //Show proxy errors and messages in browser console (should only be used when looking for errors)

        } elseif ($this->proxyConfig['loglevel'] == 3) {

            return; //No logging
        }

    }

    public function getTime() {

        return date($this->timeFormat);
    }

    function __destruct()
    {

    }

}

class RateMeter
{
    /**
     * Holds cleanup threshold value
     *
     * @var string
     * @access public
     */

    const CLEAN_RATEMAP_AFTER = 10000;

    /**
     * Holds proxy log
     *
     * @var string
     * @access public
     */
    public $proxyLog;

    /**
     * Holds serverurl property
     *
     * @var string
     * @access public
     */
    public $serverUrl;

    /**
     * Ip property
     *
     * @var string
     * @access public
     */
    public $ip;

    /**
     * Time property
     *
     * @var string
     * @access public
     */

    public $time;

    /**
     * Stores microtime as property allowing for fractional seconds
     *
     * @var string
     * @access public
     */

    public $microtime;

    /**
     * Sqlite database name property
     *
     * @var string
     * @access public
     */
    public $dbname;

    /**
     * Sqlite connection property
     *
     * @var PDO
     * @access public
     */
    public $con;

    /**
     * Under meter cap property
     *
     * @var bool
     * @access public
     */
    public $underMeterCap;


    /**
     * Meter cap property
     *
     * @var int
     * @access public
     */

    public $countCap;

    /**
     * Meter rate property
     *
     * @var int
     * @access public
     */
    public $rate;

    /**
     * Holds the rate meter count value
     *
     * @var int
     * @access public
     */
    public $count = 0;

    /**
     * Holds rate meter period found in config
     *
     * @var int
     * @access public
     */

    public $ratelimitperiod;

    /**
     * Holds current resource being proxied
     *
     * @var string
     */

    public $resourceUrl;



    public function __construct ($url, $ratelimit, $ratelimitperiod, $log) {

        $this->proxyLog = $log;

        $this->resourceUrl = $url;

        $this->ratelimitperiod = $ratelimitperiod;

        $this->countCap = $ratelimit;

        $this->rate = $ratelimit / $ratelimitperiod / 60;  //ratelimitperiod is designed to be in seconds

        $this->ip = $_SERVER['REMOTE_ADDR'];

        $this->dbname = 'proxy.sqlite'; // This string may need to come config

        $this->getRateMeterDatabase();

    }

    public function getConnection()
    {
        if($this->con != null)
        {
            return $this->con;

        }else{

            $this->proxyLog->log('Cannot get a connection');

            header('Status: 200', true, 200);

            header('Content-Type: application/json');

            $serverError = array(
                    "error" => array("code" => 500,
                            "details" => array("Cannot make a Sqlite database connection.  Check to see if it exists.  If it does, consider backing up and then deleting sqlite database."),
                            "message" => "Proxy failed could not connect to sqlite database."
                    ));

            echo json_encode($serverError);

            exit();
        }
    }

    public function getRateMeterDatabase()
    {
        try {

            if(file_exists($this->dbname))
            {
                $db = new PDO("sqlite:" . $this->dbname);

                $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

                $this->con = $db;

                return;
            }

        }
        catch(ErrorException $e)
        {
            $this->proxyLog->log($e->getMessage());

            return null;
        }

        try {

            $db = new PDO("sqlite:" . $this->dbname);

            $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

            chmod($this->dbname,0777);

            if($db != null)
            {
                $db->beginTransaction();

                $this->createResourceIpTable($db);

                $this->createClicksTable($db);

                $this->insertClickRecord($db);

                $db->commit();

                $this->con = $db;

            }

            return;

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());

            return null;
        }
    }

    public function insertClickRecord($db)
    {

        try {


            $sql = "INSERT INTO clicks (id, total) VALUES (:id,:total)";

            $q = $db->prepare($sql);

            $q->bindValue(':id', null);

            $q->bindValue(':total', 1);

            $q->execute() or die($this->getDatabaseErrorMessage());

        }
        catch(PDOException $e)
        {

            $this->proxyLog->log($e->getMessage());
        }

    }

    public function createClicksTable($db)
    {
        try {

            $db->exec("CREATE TABLE IF NOT EXISTS clicks (
                    id INTEGER PRIMARY KEY,
                    total INTEGER)");

            $this->proxyLog->log("clicks table created!");
        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

        try {

            $db->exec('CREATE INDEX total ON clicks (total)');

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

    }

    public function createResourceIpTable($db)
    {
        try {

            $db->exec("CREATE TABLE IF NOT EXISTS ips (
                    id INTEGER PRIMARY KEY,
                    url VARCHAR(255),
                    ip VARCHAR(50),
                    count INTEGER,
                    rate INTEGER,
                    time INTEGER)");

            $this->proxyLog->log("ips table created!");

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

        try {

            $db->exec('CREATE INDEX url ON ips (url)');

            $db->exec('CREATE INDEX ip ON ips (ip)');

            $db->exec('CREATE INDEX count ON ips (count)');

            $db->exec('CREATE INDEX rate ON ips (rate)');

            $db->exec('CREATE INDEX time ON ips (time)');

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

    }

    public function getClickCount()
    {
        $db = $this->getConnection();

        try {

            $sth = $db->prepare("SELECT total FROM clicks WHERE id = :id");

            $sth->execute(array(':id' => 1)) or die($this->getDatabaseErrorMessage());

            $r = $sth->fetchAll();

            return $r[0]['total'];

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }
    }

    public function selectLastRequest()
    {

        $db = $this->getConnection();

        try {

            $sth = $db->prepare("SELECT time, id, count, rate FROM ips WHERE url = :url AND ip = :ip");

            $sth->execute(array(':url' => $this->resourceUrl, ':ip' => $this->ip)) or die($this->getDatabaseErrorMessage());

            $r = $sth->fetchAll();

            return $r[0];

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }
    }

    public function updateResourceIp($id)
    {
        $db = $this->getConnection();

        try {

            $sth = $db->prepare("UPDATE ips SET id=:id, url=:url, ip=:ip, count=:count, rate=:rate, time=:time WHERE id = :id");

            $sth->bindValue(':id', $id);

            $sth->bindValue(':url', $this->resourceUrl);

            $sth->bindValue(':ip', $this->ip);

            $sth->bindValue(':count', $this->count);

            $sth->bindValue(':rate', $this->rate);

            $sth->bindValue(':time', $this->microtime);

            $sth->execute() or die($this->getDatabaseErrorMessage());
        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

    }

    public function updateClicks($id, $total)
    {

        $db = $this->getConnection();

        try {

            $sth = $db->prepare("UPDATE clicks SET total=:total WHERE id = :id");

            $sth->bindValue(':id', $id);

            $sth->bindValue(':total', $total);

            $sth->execute() or die($this->getDatabaseErrorMessage());
        }
        catch(PDOException $e)
        {

            $this->proxyLog->log($e->getMessage());
        }

    }

    public function insertResourceIp()
    {
        $db = $this->getConnection();

        try {

            $sql = "INSERT INTO ips (id, url, ip, count, rate, time) VALUES (:id,:url,:ip,:count,:rate,:time)";

            $q = $db->prepare($sql);

            $q->bindValue(':id', null);

            $q->bindValue(':url', $this->resourceUrl);

            $q->bindValue(':ip', $this->ip);

            $q->bindValue(':count', $this->count);

            $q->bindValue(':rate', $this->rate);

            $q->bindValue(':time', $this->microtime);

            $q->execute() or die($this->getDatabaseErrorMessage());

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }

    }

    public function canBeCleaned($count, $lastTime, $rate)
    {
        $tsTotalSeconds = $this->getTimeDifferenceInSeconds($lastTime);

        return $count - $tsTotalSeconds * $rate <= 0;
    }

    public function fetchAllIps() {

        $this->microtime = microtime(true);

        $db = $this->getConnection();

        try {

            $sth = $db->prepare("SELECT * FROM ips;");

            $sth->execute() or die($this->getDatabaseErrorMessage());

            $r = $sth->fetchAll();

            return $r;

        }
        catch(PDOException $e)
        {
            $this->proxyLog->log($e->getMessage());
        }
    }


    public function rateMeterCleanup()
    {
        $db = $this->getConnection();

        $r = $this->fetchAllIps();

        $deletes = array();

        foreach ($r as $item => $value)
        {

            if($this->canBeCleaned($value['count'], $value['time'], $value['rate'])){

                $this->proxyLog->log($value['id'] . "::id");

                $deletes[] = $value['id'];

            }else{

                $this->proxyLog->log('Nothing to clean.');

            }

        }

        $hasDeletes = (boolean)$deletes;

        if($hasDeletes){

            $placeholders = implode(',', array_fill(0, count($deletes), '?'));

            $stmt = $db->prepare('DELETE FROM ips WHERE id IN(' . $placeholders . ')'); //Using indexed based binding pattern

            foreach ($deletes as $k => $id){

                $stmt->bindValue(($k+1), $id);

            }

            try {

                $stmt->execute();

                $this->proxyLog->log('Cleanup occured.');

            } catch (Exception $e) {

                $this->proxyLog->log($e->getMessage());

            }

        }

    }


    public function checkRateMeter()
    {
        $this->microtime = microtime(true);

        $lastRequest = $this->selectLastRequest();

        $clickCount = $this->getClickCount();

        $clickCount = $clickCount + 1;

        $this->updateClicks(1, $clickCount); //Updating the click table so we know when to clean up (aka after 10,000 requests)

        if ($lastRequest != null || count($lastRequest) > 0) {

            $count = $lastRequest['count'];

            $tsTotalSeconds = $this->getTimeDifferenceInSeconds($lastRequest['time']);

            // $this->debugMeterAlgorithm($count, $tsTotalSeconds, $this->rate);

            $count = max(0, $count - $tsTotalSeconds * $this->rate);

            if ($count <= $this->countCap) {

                $this->underMeterCap = true;

                $count = $count + 1;

                $this->count = $count;

                $this->updateResourceIp($lastRequest['id']);

                return true;

            }

        }else{

            $this->insertResourceIp(); //Add item to ips table.

            $this->underMeterCap = true;

            return;
        }

        $this->underMeterCap = false;

        if($clickCount >= RateMeter::CLEAN_RATEMAP_AFTER)
        {
            $this->proxyLog->log("Click count: " . $clickCount);

            $this->rateMeterCleanup();

            $this->updateClicks(1, 0); //Set click counter back to zero after cleanup.

        }

        return;

    }
    public function debugMeterAlgorithm($count, $tsTotalSeconds, $rate)
    {
        $debugCountValue = $count - $tsTotalSeconds * $rate;

        $this->proxyLog->log("Count is ( count - timespan x rate ) : " . $count . " - " . $tsTotalSeconds . " x " . $rate . " = " . $debugCountValue);

        $this->proxyLog->log("debugCountValue:::" . $debugCountValue);

    }


    public function getTimeDifferenceInSeconds($firstTime, $secondTime = null)
    {


        if($firstTime == null)
        {
            $this->proxyLog->log("No time value was returned from 'ips' table in Sqlite database!");

            header('Status: 200', true, 200);

            header('Content-Type: application/json');

            $serverError = array(
                    "error" => array("code" => 500,
                            "details" => array("No time value was returned from 'ips' table in Sqlite database.  Consider backing up and then deleting sqlite database."),
                            "message" => "Proxy failed due to missing value in database."
                    ));

            echo json_encode($serverError);

            exit();

        }

        if($secondTime == null)
        {
            $secondTime = microtime(true);
        }

        $time = array($firstTime, $secondTime);

        sort($time);

        $diff = $time[1] - $time[0];

        return $diff;

    }


    public function underMeterCap()
    {
        if(empty($this->countCap) || empty($this->ratelimitperiod)) {

            return true;

        }

        $this->checkRateMeter();

        if($this->underMeterCap)
        {
            return true;

        }else{

            return false;
        }
    }

    public function getDatabaseErrorMessage()
    {
        $this->proxyLog->log("A database error occured.");

        header('Status: 200', true, 200);

        header('Content-Type: application/json');

        $dbError = array(
                "error" => array("code" => 500,
                        "details" => array("A database error occurred.  Consider backing up and then deleting sqlite database."),
                        "message" => "Proxy failed due to database error."
                ));

        return json_encode($dbError);
    }

    function __destruct()
    {
        $this->con = null;

    }

}

class ProxyConfig {

    public $proxyConfig;

    public $serverUrls;

    public function __construct()
    {

    }

    public function useXML()
    {

        $xmlParser = new XmlParser();

        $proxyconfig = $xmlParser->results[0]['proxyconfig'];

        $proxyconfig = $this->lowercaseArrayKeys($proxyconfig, CASE_LOWER);

        $proxyConfig = $this->setProxyConfig($proxyconfig);

        $serverUrls = $xmlParser->results[0]['childrens'][0]['childrens'];

        $serverUrls = $this->lowercaseArrayKeys($serverUrls, CASE_LOWER);

        $normalizeServerUrls = $this->normalizeServerUrls($serverUrls);

        $this->setServerUrls($normalizeServerUrls);

        $allowedReferers = explode(",", $this->proxyConfig['allowedreferers']); //Change XML allowedreferers from string to array

        $this->proxyConfig['allowedreferers'] = $allowedReferers; //Add above array to the proxyconfig property

        $xmlParser = null;

    }

    function lowercaseArrayKeys($array, $case)
    {
        $array = array_change_key_case($array, $case);

        foreach ($array as $key => $value) {

            if ( is_array($value) ) {

                $array[$key] = $this->lowercaseArrayKeys($value, $case);

            }
        }

        return $array;
    }


    public function normalizeServerUrls($serverUrls) {

        $formatedData = array();

        foreach ($serverUrls as $key => $item) {

            if(is_array($item)) {

                foreach ($item as $k => $v) {

                    if(is_array($v)) {

                        $normal = array("serverurl" => array(0=>$v));

                        $formatedData[] = $normal;

                    }

                }
            }
        }

        return $formatedData;
    }


    public function useJSON()
    {
        try {

            $c = file_get_contents("proxy.config");

            $configJson = json_decode($c, true);

            $config = $this->lowercaseArrayKeys($configJson, CASE_LOWER);

            $this->setProxyConfig($config['proxyconfig'][0]);

            $this->setServerUrls($config['serverurls']);

        }catch (Exception $e) {

            $this->proxyLog->log($e->getMessage());

        }

    }

    public function __set($property, $value)
    {
        $method = 'set' . $value;

        if (!method_exists($this, $method))
        {

            throw new Exception('Error in proxy configuration file.');
        }

        $this->$method($value);
    }

    public function __get($property)
    {
        $method = 'get' . $property;

        if (!method_exists($this, $method))
        {
            throw new Exception('Error in proxy configuration file.');
        }

        return $this->$method();
    }

    public function setOptions(array $options)
    {
        $methods = get_class_methods($this);

        foreach ($options as $key => $value) {

            $method = 'set' . ucfirst($key);

            if (in_array($method, $methods))
            {
                $this->$method($value);
            }
        }

        return $this;
    }

    public function getServerUrls()
    {

        return $this->serverUrls;

    }

    public function setServerUrls($item)
    {

        $this->serverUrls = $item;

        return $this;

    }

    public function getProxyConfig()
    {

        return $this->proxyConfig;

    }

    public function setProxyConfig($item)
    {

        $this->proxyConfig = $item;

        return $this;
    }

    function __destruct()
    {

    }

}

class XmlParser
{
    public $results = array();

    public $parser;

    public $xmlString;

    public $file;

    function XmlParser($f = "proxy.config")
    {
        if(trim($f) != "") { $this->loadFile($f);}
    }

    function loadFile($f)
    {
        $data = file($f);

        // Check that configuration file can be read, and that it's not empty
        if (!$data) {
            $message = "Proxy error: problem reading proxy configuration file.";
            // This is before we have the log location, so we cannot log to logfile

            header('Status: 403', true, 403);  // 403 Forbidden - The server understood the request, but is refusing to fulfill it. For example, if a directory or file is unreadable due to file permissions.

            header('Content-Type: application/json');

            $configError = array(
                    "error" => array("code" => 403,
                        "details" => array("$message"),
                        "message" => "$message"
                    ));

            die(json_encode($configError));     
        }
        
        $xml = implode("\n", $data);

        return $this->parse($xml);
    }

    function parse($xml)
    {
        $this->parser = xml_parser_create();

        xml_set_object($this->parser, $this);

        xml_set_element_handler($this->parser, "tagStart", "tagEnd");

        $this->xmlString = xml_parse($this->parser,$xml);

        if(!$this->xmlString)
        {
            die(sprintf("Config XML error: %s at line %d",
                    xml_error_string(xml_get_error_code($this->parser)),
                    xml_get_current_line_number($this->parser))); //This is before we have the log location
        }

        xml_parser_free($this->parser);

        return $this->results;
    }

    // lowercase array keys and properties etc from the config file
    function tagStart($parser, $name, $attrs)
    {
        $attrs = array_change_key_case($attrs, CASE_LOWER);

        $tag = array(strtolower($name) => $attrs);

        array_push($this->results, $tag);
    }


    function tagEnd($parser, $name)
    {

        //http://www.php.net/manual/en/function.xml-parse.php

        $this->results[count($this->results)-2]['childrens'][] = $this->results[count($this->results)-1];

        if(count($this->results[count($this->results)-2]['childrens'] ) == 1)
        {
            $this->results[count($this->results)-2]['firstchild'] =& $this->results[count($this->results)-2]['childrens'][0];
        }

        array_pop($this->results);
    }

    function __destruct()
    {

    }

}


$proxyConfig = new ProxyConfig();

$proxyConfig->useXML();

//$proxyConfig->useJSON();

$proxyLog = new ProxyLog($proxyConfig);

$proxyObject = new Proxy($proxyConfig, $proxyLog);

$proxyObject->getResponse();

?>