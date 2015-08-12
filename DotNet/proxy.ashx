<%@ WebHandler Language="C#" Class="proxy" %>

/*
 * DotNet proxy client.
 *
 * Version 1.1 beta
 * See https://github.com/Esri/resource-proxy for more information.
 *
 */

#define TRACE
using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using System.Web.Caching;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class proxy : IHttpHandler {

    private static String version = "1.1 beta";

    class RateMeter {
        double _rate; //internal rate is stored in requests per second
        int _countCap;
        double _count = 0;
        DateTime _lastUpdate = DateTime.Now;

        public RateMeter(int rate_limit, int rate_limit_period) {
            _rate = (double) rate_limit / rate_limit_period / 60;
            _countCap = rate_limit;
        }

        //called when rate-limited endpoint is invoked
        public bool click() {
            TimeSpan ts = DateTime.Now - _lastUpdate;
            _lastUpdate = DateTime.Now;
            //assuming uniform distribution of requests over time,
            //reducing the counter according to # of seconds passed
            //since last invocation
            _count = Math.Max(0, _count - ts.TotalSeconds * _rate);
            if (_count <= _countCap) {
                //good to proceed
                _count++;
                return true;
            }
            return false;
        }

        public bool canBeCleaned() {
            TimeSpan ts = DateTime.Now - _lastUpdate;
            return _count - ts.TotalSeconds * _rate <= 0;
        }
    }

    private static string PROXY_REFERER = "http://localhost/proxy/proxy.ashx";
    private static string DEFAULT_OAUTH = "https://www.arcgis.com/sharing/oauth2/";
    private static int CLEAN_RATEMAP_AFTER = 10000; //clean the rateMap every xxxx requests
    private static System.Net.IWebProxy SYSTEM_PROXY = System.Net.HttpWebRequest.DefaultWebProxy; // Use the default system proxy
    private static LogTraceListener logTraceListener = null;
    private static Object _rateMapLock = new Object();

    public void ProcessRequest(HttpContext context) {

        
        if (logTraceListener == null)
        {
            logTraceListener = new LogTraceListener();
            Trace.Listeners.Add(logTraceListener);
        }
        
        
        HttpResponse response = context.Response;
        if (context.Request.Url.Query.Length < 1)
        {
            string errorMsg = "This proxy does not support empty parameters.";
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.BadRequest);
            return;
        }

        string uri = context.Request.Url.Query.Substring(1);

        //if uri is ping
        if (uri.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
        {
            ProxyConfig proxyConfig = ProxyConfig.GetCurrentConfig();

            String checkConfig = (proxyConfig == null) ? "Not Readable" : "OK";
            String checkLog = "";
            if (checkConfig != "OK")
            {
                checkLog = "Can not verify";
            }
            else
            {
                String filename = proxyConfig.logFile;
                checkLog = (filename != null && filename != "") ? "OK" : "Not Exist/Readable";

                if (checkLog == "OK")
                    log(TraceLevel.Info, "Log from ping");
                
            }

            sendPingResponse(response, version, checkConfig, checkLog);
            return;
        }

        //if url is encoded, decode it.
        if (uri.StartsWith("http%3a%2f%2f", StringComparison.InvariantCultureIgnoreCase) || uri.StartsWith("https%3a%2f%2f", StringComparison.InvariantCultureIgnoreCase))
            uri = HttpUtility.UrlDecode(uri);
        
        log(TraceLevel.Info, uri);
        ServerUrl serverUrl;
        bool passThrough = false;
        try {
            serverUrl = getConfig().GetConfigServerUrl(uri);
            passThrough = serverUrl == null;
        }
        //if XML couldn't be parsed
        catch (InvalidOperationException ex) {

            string errorMsg = ex.InnerException.Message + " " + uri;
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.InternalServerError);
            return;
        }  
        //if mustMatch was set to true and URL wasn't in the list
        catch (ArgumentException ex) {
            string errorMsg = ex.Message + " " + uri;
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.Forbidden);
            return;
        }
        //use actual request header instead of a placeholder, if present
        if (context.Request.Headers["referer"] != null)
            PROXY_REFERER = context.Request.Headers["referer"];

        //referer
        //check against the list of referers if they have been specified in the proxy.config
        String[] allowedReferersArray = ProxyConfig.GetAllowedReferersArray();
        if (allowedReferersArray != null && allowedReferersArray.Length > 0 && context.Request.Headers["referer"] != null)
        {
            PROXY_REFERER = context.Request.Headers["referer"];
            string requestReferer = context.Request.Headers["referer"];
            try
            {
                String checkValidUri = new UriBuilder(requestReferer.StartsWith("//") ? requestReferer.Substring(requestReferer.IndexOf("//") + 2) : requestReferer).Host;

            }
            catch (Exception e)
            {
                log(TraceLevel.Warning, "Proxy is being used from an invalid referer: " + context.Request.Headers["referer"]);
                sendErrorResponse(context.Response, "Error verifying referer. ", "403 - Forbidden: Access is denied.", System.Net.HttpStatusCode.Forbidden);
                return;
            }

            if (!checkReferer(allowedReferersArray, requestReferer))
            {
                log(TraceLevel.Warning, "Proxy is being used from an unknown referer: " + context.Request.Headers["referer"]);
                sendErrorResponse(context.Response, "Unsupported referer. ", "403 - Forbidden: Access is denied.", System.Net.HttpStatusCode.Forbidden);
            }


        }

        //Check to see if allowed referer list is specified and reject if referer is null
        if (context.Request.Headers["referer"] == null && allowedReferersArray != null && !allowedReferersArray[0].Equals("*"))
        {
            log(TraceLevel.Warning, "Proxy is being called by a null referer.  Access denied.");
            sendErrorResponse(response, "Current proxy configuration settings do not allow requests which do not include a referer header.", "403 - Forbidden: Access is denied.", System.Net.HttpStatusCode.Forbidden);
            return;
        }

        //Throttling: checking the rate limit coming from particular client IP
        if (!passThrough && serverUrl.RateLimit > -1) {
            lock (_rateMapLock)
            {
                ConcurrentDictionary<string, RateMeter> ratemap = (ConcurrentDictionary<string, RateMeter>)context.Application["rateMap"];
                if (ratemap == null)
                {
                    ratemap = new ConcurrentDictionary<string, RateMeter>();
                    context.Application["rateMap"] = ratemap;
                    context.Application["rateMap_cleanup_counter"] = 0;
                }
                string key = "[" + serverUrl.Url + "]x[" + context.Request.UserHostAddress + "]";
                RateMeter rate;
                if (!ratemap.TryGetValue(key, out rate))
                {
                    rate = new RateMeter(serverUrl.RateLimit, serverUrl.RateLimitPeriod);
                    ratemap.TryAdd(key, rate);
                }
                if (!rate.click())
                {
                    log(TraceLevel.Warning, " Pair " + key + " is throttled to " + serverUrl.RateLimit + " requests per " + serverUrl.RateLimitPeriod + " minute(s). Come back later.");
                    sendErrorResponse(context.Response, "This is a metered resource, number of requests have exceeded the rate limit interval.", "Unable to proxy request for requested resource", System.Net.HttpStatusCode.PaymentRequired);
                    return;
                }

                //making sure the rateMap gets periodically cleaned up so it does not grow uncontrollably
                int cnt = (int)context.Application["rateMap_cleanup_counter"];
                cnt++;
                if (cnt >= CLEAN_RATEMAP_AFTER)
                {
                    cnt = 0;
                    cleanUpRatemap(ratemap);
                }
                context.Application["rateMap_cleanup_counter"] = cnt;
            }
        }

        //readying body (if any) of POST request
        byte[] postBody = readRequestPostBody(context);
        string post = System.Text.Encoding.UTF8.GetString(postBody);

        System.Net.NetworkCredential credentials = null;
        string requestUri = uri;
        bool hasClientToken = false;
        string token = string.Empty;
        string tokenParamName = null;

        if (!passThrough && serverUrl.Domain != null)
        {
            credentials = new System.Net.NetworkCredential(serverUrl.Username, serverUrl.Password, serverUrl.Domain);
        }
        else
        {
            //if token comes with client request, it takes precedence over token or credentials stored in configuration
            hasClientToken = uri.Contains("?token=") || uri.Contains("&token=") || post.Contains("?token=") || post.Contains("&token=");

            if (!passThrough && !hasClientToken)
            {
                // Get new token and append to the request.
                // But first, look up in the application scope, maybe it's already there:
                token = (String)context.Application["token_for_" + serverUrl.Url];
                bool tokenIsInApplicationScope = !String.IsNullOrEmpty(token);

                //if still no token, let's see if there is an access token or if are credentials stored in configuration which we can use to obtain new token
                if (!tokenIsInApplicationScope)
                {
                    token = serverUrl.AccessToken;
                    if (String.IsNullOrEmpty(token))
                        token = getNewTokenIfCredentialsAreSpecified(serverUrl, uri);
                }

                if (!String.IsNullOrEmpty(token) && !tokenIsInApplicationScope)
                {
                    //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                    context.Application.Lock();
                    context.Application["token_for_" + serverUrl.Url] = token;
                    context.Application.UnLock();
                }
            }

            //name by which token parameter is passed (if url actually came from the list)
            tokenParamName = serverUrl != null ? serverUrl.TokenParamName : null;

            if (String.IsNullOrEmpty(tokenParamName))
                tokenParamName = "token";

            requestUri = addTokenToUri(uri, token, tokenParamName);
        }
        if ((serverUrl.HostRedirect != null) && (serverUrl.HostRedirect != string.Empty))
        {
            requestUri = serverUrl.HostRedirect + new Uri(requestUri).PathAndQuery;
        }

        
        //forwarding original request
        System.Net.WebResponse serverResponse = null;
        try {
            serverResponse = forwardToServer(context.Request, requestUri, postBody, credentials);
        } catch (System.Net.WebException webExc) {
            
            string errorMsg = webExc.Message + " " + uri;
            log(TraceLevel.Error, errorMsg);

            if (webExc.Response != null)
            {
                copyResponseHeaders(webExc.Response as System.Net.HttpWebResponse, context.Response);
                
                using (Stream responseStream = webExc.Response.GetResponseStream())
                {
                    byte[] bytes = new byte[32768];
                    int bytesRead = 0;

                    while ((bytesRead = responseStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        responseStream.Write(bytes, 0, bytesRead);
                    }

                    context.Response.StatusCode = (int)(webExc.Response as System.Net.HttpWebResponse).StatusCode;
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError;
                sendErrorResponse(context.Response, null, errorMsg, statusCode);
            }
            return;
        }

        if (passThrough || string.IsNullOrEmpty(token) || hasClientToken)
            //if token is not required or provided by the client, just fetch the response as is:
            fetchAndPassBackToClient(serverResponse, response, true);
        else {
            //credentials for secured service have come from configuration file:
            //it means that the proxy is responsible for making sure they were properly applied:

            //first attempt to send the request:
            bool tokenRequired = fetchAndPassBackToClient(serverResponse, response, false);


            //checking if previously used token has expired and needs to be renewed
            if (tokenRequired) {
                log(TraceLevel.Info, "Renewing token and trying again.");
                //server returned error - potential cause: token has expired.
                //we'll do second attempt to call the server with renewed token:
                token = getNewTokenIfCredentialsAreSpecified(serverUrl, uri);
                serverResponse = forwardToServer(context.Request, addTokenToUri(uri, token, tokenParamName), postBody);

                //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                context.Application.Lock();
                context.Application["token_for_" + serverUrl.Url] = token;
                context.Application.UnLock();

                fetchAndPassBackToClient(serverResponse, response, true);
            }
        }
        response.End();
    }

    public bool IsReusable {
        get { return true; }
    }

/**
* Private
*/
    private byte[] readRequestPostBody(HttpContext context) {
        if (context.Request.InputStream.Length > 0) {
            byte[] bytes = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(bytes, 0, (int)context.Request.InputStream.Length);
            return bytes;
        }
        return new byte[0];
    }

    private void writeRequestPostBody(System.Net.HttpWebRequest req, byte[] bytes)
    {
        if (bytes != null && bytes.Length > 0)
        {
            req.ContentLength = bytes.Length;
            using (Stream outputStream = req.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
    
    private System.Net.WebResponse forwardToServer(HttpRequest req, string uri, byte[] postBody, System.Net.NetworkCredential credentials = null)
    {
        string method = postBody.Length > 0 ? "POST" : req.HttpMethod;
        System.Net.HttpWebRequest forwardReq = createHTTPRequest(uri, method, req.ContentType, credentials);
        copyRequestHeaders(req, forwardReq);
        writeRequestPostBody(forwardReq, postBody);
        return forwardReq.GetResponse();
    }

    /// <summary>
    /// Attempts to copy all headers from the fromResponse to the the toResponse.
    /// </summary>
    /// <param name="fromResponse">The response that we are copying the headers from</param>
    /// <param name="toResponse">The response that we are copying the headers to</param>
    private void copyResponseHeaders(System.Net.WebResponse fromResponse, HttpResponse toResponse)
    {
        foreach (var headerKey in fromResponse.Headers.AllKeys)
        {
            switch (headerKey.ToLower())
            {
                case "content-type":
                case "transfer-encoding":
                    continue;
                default:
                    toResponse.AddHeader(headerKey, fromResponse.Headers[headerKey]);
                    break;
            }
        }
        toResponse.ContentType = fromResponse.ContentType;
    }

    private void copyRequestHeaders(HttpRequest fromRequest, System.Net.HttpWebRequest toRequest)
    {
        foreach (var headerKey in fromRequest.Headers.AllKeys)
        {
            string headerValue = fromRequest.Headers[headerKey];
            switch (headerKey.ToLower())
            {
                case "accept-encoding":
                case "proxy-connection":
                    continue;
                case "range":
                    setRangeHeader(toRequest, headerValue);
                    break;
                case "accept":
                    toRequest.Accept = headerValue;
                    break;
                case "if-modified-since":
                    DateTime modDT;
                    if (DateTime.TryParse(headerValue, out modDT))
                        toRequest.IfModifiedSince = modDT;
                    break;
                case "referer":
                    toRequest.Referer = headerValue;
                    break;
                case "user-agent":
                    toRequest.UserAgent = headerValue;
                    break;
                default:
                    // Some headers are restricted and would throw an exception:
                    // http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.headers(v=vs.100).aspx
                    if (!System.Net.WebHeaderCollection.IsRestricted(headerKey) &&
                        toRequest.Headers[headerKey] == null)
                        toRequest.Headers[headerKey] = headerValue;
                    break;
            }
        }
    }

    private void setRangeHeader(System.Net.HttpWebRequest req, string range)
    {
        string[] specifierAndRange = range.Split('=');
        if (specifierAndRange.Length == 2)
        {
            string specifier = specifierAndRange[0];
            string[] fromAndTo = specifierAndRange[1].Split('-');
            if (fromAndTo.Length == 2)
            {
                int from, to;
                if (int.TryParse(fromAndTo[0], out from) && int.TryParse(fromAndTo[1], out to))
                    req.AddRange(specifier, from, to);
            }
        }
    }
        
    private bool fetchAndPassBackToClient(System.Net.WebResponse serverResponse, HttpResponse clientResponse, bool ignoreAuthenticationErrors) {
        if (serverResponse != null) {
            copyResponseHeaders(serverResponse, clientResponse);
            using (Stream byteStream = serverResponse.GetResponseStream()) {
                // Text response
                if (serverResponse.ContentType.Contains("text") ||
                    serverResponse.ContentType.Contains("json") ||
                    serverResponse.ContentType.Contains("xml")) {
                    using (StreamReader sr = new StreamReader(byteStream)) {
                        string strResponse = sr.ReadToEnd();
                        if (
                            !ignoreAuthenticationErrors
                            && strResponse.IndexOf("{\"error\":{") > -1
                            && (strResponse.IndexOf("\"code\":498") > -1 || strResponse.IndexOf("\"code\":499") > -1)
                        )
                            return true;
                        clientResponse.Write(strResponse);
                    }
                } else {
                    // Binary response (image, lyr file, other binary file)

                    // Tell client not to cache the image since it's dynamic
                    clientResponse.CacheControl = "no-cache";
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = byteStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        clientResponse.OutputStream.Write(buffer, 0, read);
                    }
                    clientResponse.OutputStream.Close();
                }
                serverResponse.Close();
            }
            clientResponse.StatusCode = (int)(serverResponse as System.Net.HttpWebResponse).StatusCode;
        }
        return false;
    }

    private System.Net.WebResponse doHTTPRequest(string uri, string method, System.Net.NetworkCredential credentials = null)
    {
        byte[] bytes = null;
        String contentType = null;
        log(TraceLevel.Info, "Sending request!");

        if (method.Equals("POST"))
        {
            String[] uriArray = uri.Split(new char[] { '?' }, 2);
            uri = uriArray[0];
            if (uriArray.Length > 1)
            {
                contentType = "application/x-www-form-urlencoded";
                String queryString = uriArray[1];
                bytes = System.Text.Encoding.UTF8.GetBytes(queryString);
            }
        }

        System.Net.HttpWebRequest req = createHTTPRequest(uri, method, contentType, credentials);
        req.Referer = PROXY_REFERER;
        writeRequestPostBody(req, bytes);
        return req.GetResponse();
    }
    
    private System.Net.HttpWebRequest createHTTPRequest(string uri, string method, string contentType, System.Net.NetworkCredential credentials = null)
    {
        System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
        req.ServicePoint.Expect100Continue = false;
        req.Method = method;
        if (method == "POST")
            req.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;

        // Use the default system proxy
        req.Proxy = SYSTEM_PROXY;

        if (credentials != null)
            req.Credentials = credentials;

        return req;
    }
    
    private string webResponseToString(System.Net.WebResponse serverResponse) {
        using (Stream byteStream = serverResponse.GetResponseStream()) {
            using (StreamReader sr = new StreamReader(byteStream)) {
                string strResponse = sr.ReadToEnd();
                return strResponse;
            }
        }
    }

    private string getNewTokenIfCredentialsAreSpecified(ServerUrl su, string reqUrl) {
        string token = "";
        string infoUrl = "";

        bool isUserLogin = !String.IsNullOrEmpty(su.Username) && !String.IsNullOrEmpty(su.Password);
        bool isAppLogin = !String.IsNullOrEmpty(su.ClientId) && !String.IsNullOrEmpty(su.ClientSecret);
        if (isUserLogin || isAppLogin) {
            log(TraceLevel.Info, "Matching credentials found in configuration file. OAuth 2.0 mode: " + isAppLogin);
            if (isAppLogin) {
                //OAuth 2.0 mode authentication
                //"App Login" - authenticating using client_id and client_secret stored in config
                su.OAuth2Endpoint = string.IsNullOrEmpty(su.OAuth2Endpoint) ? DEFAULT_OAUTH : su.OAuth2Endpoint;
                if (su.OAuth2Endpoint[su.OAuth2Endpoint.Length - 1] != '/')
                    su.OAuth2Endpoint += "/";
                log(TraceLevel.Info, "Service is secured by " + su.OAuth2Endpoint + ": getting new token...");
                string uri = su.OAuth2Endpoint + "token?client_id=" + su.ClientId + "&client_secret=" + su.ClientSecret + "&grant_type=client_credentials&f=json";
                string tokenResponse = webResponseToString(doHTTPRequest(uri, "POST"));
                token = extractToken(tokenResponse, "token");
                if (!string.IsNullOrEmpty(token))
                    token = exchangePortalTokenForServerToken(token, su);
            } else {
                //standalone ArcGIS Server/ArcGIS Online token-based authentication

                //if a request is already being made to generate a token, just let it go
                if (reqUrl.ToLower().Contains("/generatetoken")) {
                    string tokenResponse = webResponseToString(doHTTPRequest(reqUrl, "POST"));
                    token = extractToken(tokenResponse, "token");
                    return token;
                }           
                
                //lets look for '/rest/' in the requested URL (could be 'rest/services', 'rest/community'...)
                if (reqUrl.ToLower().Contains("/rest/"))
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/rest/", StringComparison.OrdinalIgnoreCase));
                
                //if we don't find 'rest', lets look for the portal specific 'sharing' instead
                else if (reqUrl.ToLower().Contains("/sharing/")) {
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/sharing/", StringComparison.OrdinalIgnoreCase));
                    infoUrl = infoUrl + "/sharing";
                }
                else
                    throw new ApplicationException("Unable to determine the correct URL to request a token to access private resources.");
                    
                if (infoUrl != "") {
                    log(TraceLevel.Info," Querying security endpoint...");
                    infoUrl += "/rest/info?f=json";
                    //lets send a request to try and determine the URL of a token generator
                    string infoResponse = webResponseToString(doHTTPRequest(infoUrl, "GET"));
                    String tokenServiceUri = getJsonValue(infoResponse, "tokenServicesUrl");
                    if (string.IsNullOrEmpty(tokenServiceUri))
                        tokenServiceUri = getJsonValue(infoResponse, "tokenServiceUrl");
                    if (tokenServiceUri != "") {
                        log(TraceLevel.Info," Service is secured by " + tokenServiceUri + ": getting new token...");
                        string uri = tokenServiceUri + "?f=json&request=getToken&referer=" + PROXY_REFERER + "&expiration=60&username=" + su.Username + "&password=" + su.Password;
                        string tokenResponse = webResponseToString(doHTTPRequest(uri, "POST"));
                        token = extractToken(tokenResponse, "token");
                    }
                }


            }
        }
        return token;
    }

    private bool checkWildcardSubdomain(String allowedReferer, String requestedReferer, String protocol)
    {
        String[] allowedRefererParts = Regex.Split(allowedReferer, "(\\.)");
        String[] refererParts = Regex.Split(requestedReferer, "(\\.)");

        int allowedIndex = allowedRefererParts.Length - 1;
        int refererIndex = refererParts.Length - 1;
        while (allowedIndex >= 0 && refererIndex >= 0)
        {
            if (allowedRefererParts[allowedIndex].Equals(refererParts[refererIndex], StringComparison.OrdinalIgnoreCase) || allowedRefererParts[allowedIndex].Equals(protocol+refererParts[refererIndex], StringComparison.OrdinalIgnoreCase))
            {
                allowedIndex = allowedIndex - 1;
                refererIndex = refererIndex - 1;
            }
            else
            {
                if (allowedRefererParts[allowedIndex].Equals("*") || allowedRefererParts[allowedIndex].Equals(protocol + "*"))
                {
                    allowedIndex = allowedIndex - 1;
                    refererIndex = refererIndex - 1;
                    continue; //next
                }
                if (refererParts[refererIndex].Contains("/"))
                {
                    if (refererParts[refererIndex].StartsWith(allowedRefererParts[allowedIndex]) && ((refererParts[refererIndex] + "/").StartsWith(allowedRefererParts[allowedIndex] + "/")))
                    {
                        //check folder match. 
                        // if domain/folder compared with domain/folders, is not a match
                        allowedIndex = allowedIndex - 1;
                        refererIndex = refererIndex - 1;
                        continue; //next
                    }
                }

                return false;
            }
        }
        return true;
    }

    private bool checkReferer(String[] allowedReferers, String referer)
    {
        if (allowedReferers != null && allowedReferers.Length > 0)
        {
            if (allowedReferers.Length == 1 && allowedReferers[0].Equals("*")) return true; //speed-up
            foreach (String allowedReferer in allowedReferers)
            {
                //get protocol type
                String protocol = "";
                if (allowedReferer.StartsWith("http://")) protocol = "http://";
                else if (allowedReferer.StartsWith("https://")) protocol = "https://";
                else if (allowedReferer.StartsWith("//")) protocol = "//";

                // allowedReferer = "http://" or "https://", must exact match    
                if (protocol.Equals("http://") || protocol.Equals("https://"))
                {
                    if (referer.ToLower().Equals(allowedReferer.ToLower())) return true;
                }
                else
                {
                    // protocol = "//" or ""
                    // accept "http://" "https://" "//" or ""
                    String allowedRefererAccepted = allowedReferer;
                    if (protocol.Equals("//"))
                    {
                        allowedRefererAccepted = allowedReferer.Substring(allowedReferer.IndexOf("//") + 2);
                    }

                    referer = (referer.Contains("//")) ? referer.Substring(referer.IndexOf("//") + 2) : referer;
                    if (referer.ToLower().Equals(allowedRefererAccepted.ToLower())) return true;
                }

                if (allowedReferer.Contains("*") || referer.Contains("/"))
                {   //try if the allowed referer contains wildcard for subdomain or folder

                    if (checkWildcardSubdomain(allowedReferer, referer, protocol)) return true;

                }

            }
            return false;//no-match
        }
        return true;//when allowedReferer is null, then allow everything
    }

    private string exchangePortalTokenForServerToken(string portalToken, ServerUrl su) {
        //ideally, we should POST the token request
        log(TraceLevel.Info," Exchanging Portal token for Server-specific token for " + su.Url + "...");
        string uri = su.OAuth2Endpoint.Substring(0, su.OAuth2Endpoint.IndexOf("/oauth2/", StringComparison.OrdinalIgnoreCase)) +
             "/generateToken?token=" + portalToken + "&serverURL=" + su.Url + "&f=json";
        string tokenResponse = webResponseToString(doHTTPRequest(uri, "GET"));
        return extractToken(tokenResponse, "token");
    }


    private static void sendPingResponse(HttpResponse response, String version, String config, String log)
    {
        response.AddHeader("Content-Type", "application/json");
        response.AddHeader("Accept-Encoding", "gzip");
        String message = "{ " +
            "\"Proxy Version\": \"" + version + "\"" +
            ", \"Configuration File\": \"" + config + "\"" +
            ", \"Log File\": \"" + log + "\"" +
            "}";
        response.StatusCode = 200;
        response.Write(message);
        response.Flush();
    }

    private static void sendErrorResponse(HttpResponse response, String errorDetails, String errorMessage, System.Net.HttpStatusCode errorCode)
    {
        String message = string.Format("{{error: {{code: {0},message:\"{1}\"", (int)errorCode, errorMessage);
        if (!string.IsNullOrEmpty(errorDetails))
            message += string.Format(",details:[message:\"{0}\"]", errorDetails);
        message += "}}";
        response.StatusCode = (int)errorCode;
        //this displays our customized error messages instead of IIS's custom errors
        response.TrySkipIisCustomErrors = true;
        response.Write(message);
        response.Flush();
    }

    private static string getClientIp(HttpRequest request)
    {
        if (request == null)
            return null;
        string remoteAddr = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrWhiteSpace(remoteAddr))
        {
            remoteAddr = request.ServerVariables["REMOTE_ADDR"];
        }
        else
        {
            // the HTTP_X_FORWARDED_FOR may contain an array of IP, this can happen if you connect through a proxy.
            string[] ipRange = remoteAddr.Split(',');
            remoteAddr = ipRange[ipRange.Length - 1];
        }
        return remoteAddr;
    }

    private string addTokenToUri(string uri, string token, string tokenParamName) {
        if (!String.IsNullOrEmpty(token))
            uri += uri.Contains("?")? "&" + tokenParamName + "=" + token : "?" + tokenParamName + "=" + token;
        return uri;
    }

    private string extractToken(string tokenResponse, string key) {
        string token = getJsonValue(tokenResponse, key);
        if (string.IsNullOrEmpty(token))
            log(TraceLevel.Error," Token cannot be obtained: " + tokenResponse);
        else
            log(TraceLevel.Info," Token obtained: " + token);
        return token;
    }

    private string getJsonValue(string text, string key) {
        int i = text.IndexOf(key);
        String value = "";
        if (i > -1) {
            value = text.Substring(text.IndexOf(':', i) + 1).Trim();
            value = value.Length > 0 && value[0] == '"' ?
                value.Substring(1, value.IndexOf('"', 1) - 1):
                value = value.Substring(0, Math.Max(0, Math.Min(Math.Min(value.IndexOf(","), value.IndexOf("]")), value.IndexOf("}"))));
        }
        return value;
    }

    private void cleanUpRatemap(ConcurrentDictionary<string, RateMeter> ratemap) {
        foreach (string key in ratemap.Keys){
            RateMeter rate = ratemap[key];
            if (rate.canBeCleaned())
                ratemap.TryRemove(key, out rate);
        }
    }

/**
* Static
*/
    private static ProxyConfig getConfig() {
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        if (config != null)
            return config;
        else
            throw new ApplicationException("The proxy configuration file cannot be found, or is not readable.");
    }

    //writing Log file
    private static void log(TraceLevel logLevel, string msg) {
        string logMessage = string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
        
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        TraceSwitch ts = null;

        if (config.logLevel != null)
        {
            ts = new TraceSwitch("TraceLevelSwitch2", "TraceSwitch in the proxy.config file", config.logLevel);
        }
        else
        {
            ts = new TraceSwitch("TraceLevelSwitch2", "TraceSwitch in the proxy.config file", "Error");
            config.logLevel = "Error";
        }

        Trace.WriteLineIf(logLevel <= ts.Level, logMessage);
    }

    private static object _lockobject = new object();

}

class LogTraceListener : TraceListener
{
    private static object _lockobject = new object();
    public override void Write(string message)
    {
        //Only log messages to disk if logFile has value in configuration, otherwise log nothing.   
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        
        if (config.LogFile != null)
        {
            string log = config.LogFile;
            if (!log.Contains("\\") || log.Contains(".\\"))
            {
                if (log.Contains(".\\")) //If this type of relative pathing .\log.txt
                {
                    log = log.Replace(".\\", "");
                }
                string configDirectory = HttpContext.Current.Server.MapPath("proxy.config"); //Cannot use System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath b/ config may be in a child directory
                string path = configDirectory.Replace("proxy.config", "");
                log = path + log;
            }

            lock (_lockobject)
            {
                using (StreamWriter sw = File.AppendText(log))
                {
                    sw.Write(message);
                }
            }
        }
    }


    public override void WriteLine(string message)
    {
        //Only log messages to disk if logFile has value in configuration, otherwise log nothing.   
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        if (config.LogFile != null)
        {
            string log = config.LogFile;
            if (!log.Contains("\\") || log.Contains(".\\"))
            {
                if (log.Contains(".\\")) //If this type of relative pathing .\log.txt
                {
                    log = log.Replace(".\\", "");
                }
                string configDirectory = HttpContext.Current.Server.MapPath("proxy.config"); //Cannot use System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath b/ config may be in a child directory
                string path = configDirectory.Replace("proxy.config", "");
                log = path + log;
            }

            lock (_lockobject)
            {
                using (StreamWriter sw = File.AppendText(log))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }

}


[XmlRoot("ProxyConfig")]
public class ProxyConfig
{
    private static object _lockobject = new object();
    public static ProxyConfig LoadProxyConfig(string fileName) {
        ProxyConfig config = null;
        lock (_lockobject) {
            if (System.IO.File.Exists(fileName)) {
                XmlSerializer reader = new XmlSerializer(typeof(ProxyConfig));
                using (System.IO.StreamReader file = new System.IO.StreamReader(fileName)) {
                    try {
                        config = (ProxyConfig)reader.Deserialize(file);
                    }
                    catch (Exception ex) {
                        throw ex;
                    }
                }
            }
        }
        return config;
    }

    public static ProxyConfig GetCurrentConfig() {
        ProxyConfig config = HttpRuntime.Cache["proxyConfig"] as ProxyConfig;
        if (config == null) {
            string fileName = HttpContext.Current.Server.MapPath("proxy.config");
            config = LoadProxyConfig(fileName);
            if (config != null) {
                CacheDependency dep = new CacheDependency(fileName);
                HttpRuntime.Cache.Insert("proxyConfig", config, dep);
            }
        }
        return config;
    }

    //referer
    //create an array with valid referers using the allowedReferers String that is defined in the proxy.config
    public static String[] GetAllowedReferersArray()
    {
        if (allowedReferers == null)
            return null;

        return allowedReferers.Split(',');
    }

    //referer
    //check if URL starts with prefix...
    public static bool isUrlPrefixMatch(String prefix, String uri)
    {

        return uri.ToLower().StartsWith(prefix.ToLower()) ||
                    uri.ToLower().Replace("https://", "http://").StartsWith(prefix.ToLower()) ||
                    uri.ToLower().Substring(uri.IndexOf("//")).StartsWith(prefix.ToLower());
    }

    ServerUrl[] serverUrls;
    public String logFile;
    public String logLevel;
    bool mustMatch;
    //referer
    static String allowedReferers;

    [XmlArray("serverUrls")]
    [XmlArrayItem("serverUrl")]
    public ServerUrl[] ServerUrls {
        get { return this.serverUrls; }
        set
        {
            this.serverUrls = value;
        }
    }
    [XmlAttribute("mustMatch")]
    public bool MustMatch {
        get { return mustMatch; }
        set
        { mustMatch = value; }
    }
    
    //logFile
    [XmlAttribute("logFile")]
    public String LogFile
    {
        get { return logFile; }
        set
        { logFile = value; }
    }

    //logLevel
    [XmlAttribute("logLevel")]
    public String LogLevel
    {
        get { return logLevel; }
        set
        { logLevel = value; }
    }


    //referer
    [XmlAttribute("allowedReferers")]
    public string AllowedReferers
    {
        get { return allowedReferers; }
        set
        {
            allowedReferers = Regex.Replace(value, @"\s", "");
        }
    }

    public ServerUrl GetConfigServerUrl(string uri) {                       
        //split both request and proxy.config urls and compare them
        string[] uriParts = uri.Split(new char[] {'/','?'}, StringSplitOptions.RemoveEmptyEntries);
        string[] configUriParts = new string[] {};
                
        foreach (ServerUrl su in serverUrls) {
            //if a relative path is specified in the proxy.config, append what's in the request itself
            if (!su.Url.StartsWith("http"))
                su.Url = su.Url.Insert(0, uriParts[0]);

            configUriParts = su.Url.Split(new char[] { '/','?' }, StringSplitOptions.RemoveEmptyEntries);
            
            //if the request has less parts than the config, don't allow
            if (configUriParts.Length > uriParts.Length) continue;
            
            int i = 0;
            for (i = 0; i < configUriParts.Length; i++) {
                
                if (!configUriParts[i].ToLower().Equals(uriParts[i].ToLower())) break;
            }
            if (i == configUriParts.Length) {
                //if the urls don't match exactly, and the individual matchAll tag is 'false', don't allow
                if (configUriParts.Length == uriParts.Length || su.MatchAll)
                    return su;
            }                  
        }       
        
        if (mustMatch)
            throw new ArgumentException("Proxy has not been set up for this URL. Make sure there is a serverUrl in the configuration file that matches: " + uri);
        return null;
    }
}

public class ServerUrl {
    string url;
    string hostRedirect;
    bool matchAll;
    string oauth2Endpoint;
    string domain;
    string username;
    string password;
    string clientId;
    string clientSecret;
    string accessToken;
    string tokenParamName;
    string rateLimit;
    string rateLimitPeriod;
    
    [XmlAttribute("url")]
    public string Url {
        get { return url; }
        set { url = value; }
    }
    [XmlAttribute("hostRedirect")]
    public string HostRedirect
    {
        get { return hostRedirect; }
        set { hostRedirect = value; }
    }
    [XmlAttribute("matchAll")]
    public bool MatchAll {
        get { return matchAll; }
        set { matchAll = value; }
    }
    [XmlAttribute("oauth2Endpoint")]
    public string OAuth2Endpoint {
        get { return oauth2Endpoint; }
        set { oauth2Endpoint = value; }
    }
    [XmlAttribute("domain")]
    public string Domain
    {
        get { return domain; }
        set { domain = value; }
    }
    [XmlAttribute("username")]
    public string Username {
        get { return username; }
        set { username = value; }
    }
    [XmlAttribute("password")]
    public string Password {
        get { return password; }
        set { password = value; }
    }
    [XmlAttribute("clientId")]
    public string ClientId {
        get { return clientId; }
        set { clientId = value; }
    }
    [XmlAttribute("clientSecret")]
    public string ClientSecret {
        get { return clientSecret; }
        set { clientSecret = value; }
    }
    [XmlAttribute("accessToken")]
    public string AccessToken {
        get { return accessToken; }
        set { accessToken = value; }
    }
    [XmlAttribute("tokenParamName")]
    public string TokenParamName {
        get { return tokenParamName; }
        set { tokenParamName = value; }
    }
    [XmlAttribute("rateLimit")]
    public int RateLimit {
        get { return string.IsNullOrEmpty(rateLimit)? -1 : int.Parse(rateLimit); }
        set { rateLimit = value.ToString(); }
    }
    [XmlAttribute("rateLimitPeriod")]
    public int RateLimitPeriod {
        get { return string.IsNullOrEmpty(rateLimitPeriod)? 60 : int.Parse(rateLimitPeriod); }
        set { rateLimitPeriod = value.ToString(); }
    }
}
