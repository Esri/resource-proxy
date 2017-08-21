<%@page session="false"%>
<%@page import="
java.net.HttpURLConnection,
java.net.URL,
java.net.URLEncoder,
java.net.URLDecoder,
java.net.MalformedURLException,
java.io.BufferedReader,
java.io.ByteArrayOutputStream,
java.io.DataInputStream,
java.io.FileNotFoundException,
java.io.IOException,
java.io.InputStream,
java.io.InputStreamReader,
java.io.OutputStream,
java.io.Reader,
java.util.Date,
java.util.concurrent.ConcurrentHashMap,
java.util.Map,
java.util.Set,
java.util.regex.Matcher,
java.util.regex.Pattern,
java.util.ArrayList,
java.util.logging.Logger,
java.util.logging.FileHandler,
java.util.logging.SimpleFormatter,
java.util.logging.Level,
java.util.List,
java.util.Iterator,
java.util.Enumeration,
java.util.HashMap,
java.text.SimpleDateFormat" %>

<!-- ----------------------------------------------------------
*
* JSP proxy client
*
* Version 1.1.2
* See https://github.com/Esri/resource-proxy for more information.
*
----------------------------------------------------------- -->

<%! final String version = "1.1.2";   %>

<%!
    public static final class DataValidUtil {
        public static String removeCRLF(String inputLine) {
            String filteredLine = inputLine;

            if (hasCRLF(inputLine)) {
                filteredLine = filteredLine.replace("\n","").replace("\r","");
            }
            return filteredLine;
        }

        public static String replaceCRLF(String inputLine, String replaceString) {
            String filteredLine = inputLine;

            if (hasCRLF(inputLine)) {
                filteredLine = filteredLine.replace("\n",replaceString).replace("\r",replaceString);
            }
            return filteredLine;
        }

        public static boolean hasCRLF(String inputLine) {
            return inputLine.contains("\n") || inputLine.contains("\r");
        }

    }

    public static class RateMeter {
        double _rate; //internal rate is stored in requests per second
        int _countCap;
        double _count = 0;

        long _lastUpdate = new Date().getTime();

        public RateMeter(int rateLimit, int rateLimitPeriod){
            this._rate = (double) rateLimit / rateLimitPeriod / 60;
            this._countCap = rateLimit;
        }

        //called when rate-limited endpoint is invoked
        public boolean click() {
            long ts = (new Date().getTime() - _lastUpdate) / 1000;
            this._lastUpdate = new Date().getTime();
            //assuming uniform distribution of requests over time,
            //reducing the counter according to # of seconds passed
            //since last invocation
            this._count = Math.max(0, this._count - ts * this._rate);
            if (this._count <= this._countCap) {
                //good to proceed
                this._count++;
                return true;
            }
            return false;
        }

        public boolean canBeCleaned() {
            long ts = (new Date().getTime() - this._lastUpdate) / 1000;
            return this._count - ts * this._rate <= 0;
        }

    }

    //set the default values
    String PROXY_REFERER = "http://localhost/proxy.jsp";
    String DEFAULT_OAUTH = "https://www.arcgis.com/sharing/oauth2/";
    int CLEAN_RATEMAP_AFTER = 10000;

    //setReferer if real referer exist
    private void setReferer(String r){
        PROXY_REFERER = r;
    }

    //process the POST request body sent by the client
    private byte[] readRequestPostBody(HttpServletRequest request) throws IOException{
        int clength = request.getContentLength();
        if(clength > 0) {
            byte[] bytes = new byte[clength];
            DataInputStream dataIs = new DataInputStream(request.getInputStream());

            dataIs.readFully(bytes);
            dataIs.close();
            return bytes;
        }

        return new byte[0];
    }

    //proxy sends the actual request to the server
    private HttpURLConnection forwardToServer(HttpServletRequest request, String uri, byte[] postBody) throws IOException{
        //copy the client's request header to the proxy's request
        Enumeration headerNames = request.getHeaderNames();
        HashMap<String, String> mapHeaderInfo = new HashMap<String, String>();
        while (headerNames.hasMoreElements()) {
            String key = (String) headerNames.nextElement();
            String value = request.getHeader(key);
            if (!key.equalsIgnoreCase("host")) mapHeaderInfo.put(key, value);
        }

        return
                postBody.length > 0 ?
                        doHTTPRequest(uri, postBody, "POST", mapHeaderInfo) :
                        doHTTPRequest(uri, request.getMethod());
    }

    //proxy gets the response back from server
    private boolean fetchAndPassBackToClient(HttpURLConnection con, HttpServletResponse clientResponse, boolean ignoreAuthenticationErrors) throws IOException{
        if (con!=null){
            Map<String, List<String>> headerFields = con.getHeaderFields();
            Set<String> headerFieldsSet = headerFields.keySet();

            //copy the response header to the response to the client
            for (String headerFieldKey : headerFieldsSet){
                //prevent request for partial content
                if (headerFieldKey != null && headerFieldKey.toLowerCase().equals("accept-ranges")){
                    continue;
                }

                List<String> headerFieldValue = headerFields.get(headerFieldKey);
                StringBuilder sb = new StringBuilder();
                for (String value : headerFieldValue) {
                    // Reset the content-type for OGC WMS - issue #367
                    // Note: this might not be what everyone expects, but it helps some users
                    // TODO: make this configurable
                    if (headerFieldKey != null && headerFieldKey.toLowerCase().equals("content-type")){
                        if (value != null && value.toLowerCase().contains("application/vnd.ogc.wms_xml")){
                            _log(Level.FINE, "Adjusting Content-Type for WMS OGC: " + value);
                            value = "text/xml";
                        }
                    }

                    // remove Transfer-Encoding/chunked to the client
                    // StackOverflow http://stackoverflow.com/questions/31452074/how-to-proxy-http-requests-in-spring-mvc
                    if (headerFieldKey != null && headerFieldKey.toLowerCase().equals("transfer-encoding")) {
	                    if (value != null && value.toLowerCase().equals("chunked")) {
		                    continue;
	                    }
                    }

                    sb.append(value);
                    sb.append("");
                }
                if (headerFieldKey != null){
                    clientResponse.addHeader(headerFieldKey, DataValidUtil.removeCRLF(sb.toString()));
                }
            }

            //copy the response content to the response to the client
            InputStream byteStream;
            if (con.getResponseCode() >= 400 && con.getErrorStream() != null){
                if (ignoreAuthenticationErrors && (con.getResponseCode() == 498 || con.getResponseCode() == 499)) return true;
                byteStream = con.getErrorStream();
            }else{
                byteStream = con.getInputStream();
            }

            clientResponse.setStatus(con.getResponseCode());

            ByteArrayOutputStream buffer = new ByteArrayOutputStream();
            final int length = 5000;

            byte[] bytes = new byte[length];
            int bytesRead;
            while ((bytesRead = byteStream.read(bytes, 0, length)) > 0) {
                buffer.write(bytes, 0, bytesRead);
            }
            buffer.flush();

            //if the content of the HttpURLConnection contains error message, it means the token expired, so let proxy try again
            String strResponse = buffer.toString();
            if (!ignoreAuthenticationErrors && strResponse.contains("error") && (strResponse.contains("\"code\": 498") || strResponse.contains("\"code\": 499")
                    || strResponse.contains("\"code\":498") || strResponse.contains("\"code\":499"))) {
                return true;
            }

            byte[] byteResponse = buffer.toByteArray();
            OutputStream ostream = clientResponse.getOutputStream();
            ostream.write(byteResponse);
            ostream.close();
            byteStream.close();
        }
        return false;
    }

    //copy header info to the proxy's request
    private boolean passHeadersInfo(Map mapHeaderInfo, HttpURLConnection con) {
        Iterator headerIterator = mapHeaderInfo.entrySet().iterator();
        while (headerIterator.hasNext()) {
            Map.Entry pair = (Map.Entry)headerIterator.next();
            con.setRequestProperty(pair.getKey().toString(),pair.getValue().toString());
            headerIterator.remove(); // avoids a ConcurrentModificationException
        }
        return true;
    }

    //simplified interface of doHTTPRequest, will eventually call the complete interface of doHTTPRequest
    private HttpURLConnection doHTTPRequest(String uri, String method) throws IOException{
        //build the bytes sent to server
        byte[] bytes = null;

        //build the header sent to server
        HashMap<String, String> headerInfo=new HashMap<String, String>();
        headerInfo.put("Referer", PROXY_REFERER);
        if (method.equals("POST")){
            String[] uriArray = uri.split("\\?", 2);
            uri = uriArray[0];

            headerInfo.put("Content-Type", "application/x-www-form-urlencoded");

            if (uriArray.length > 1){
                String queryString = uriArray[1];
                bytes = queryString.getBytes("UTF-8");
            }
        }
        return doHTTPRequest(uri, bytes, method, headerInfo);
    }

    //complete interface of doHTTPRequest
    private HttpURLConnection doHTTPRequest(String uri, byte[] bytes, String method, Map mapHeaderInfo) throws IOException{
        URL url = new URL(uri);
        HttpURLConnection con = (HttpURLConnection)url.openConnection();

        con.setConnectTimeout(5000);
        con.setReadTimeout(10000);
        con.setRequestMethod(method);

        //pass the header to the proxy's request
        passHeadersInfo(mapHeaderInfo, con);

        //if it is a POST request
        if (bytes != null && bytes.length > 0 || method.equals("POST")) {

            if (bytes == null){
                bytes = new byte[0];
            }

            con.setRequestMethod("POST");
            con.setDoOutput(true);


            OutputStream os = con.getOutputStream();
            os.write(bytes);
        }

        return con;
    }

    //convert response from InputStream format to String format
    private String webResponseToString(HttpURLConnection con) throws IOException{

        InputStream in = con.getInputStream();

        Reader reader = new BufferedReader(new InputStreamReader(in, "UTF-8"));
        StringBuilder content = new StringBuilder();
        char[] buffer = new char[5000];
        int n;

        while ( ( n = reader.read(buffer)) != -1 ) {
            content.append(buffer, 0, n);
        }
        reader.close();

        return content.toString();
    }

    //request token
    private String getNewTokenIfCredentialsAreSpecified(ServerUrl su, String url) throws IOException{
        String token = "";
        boolean isUserLogin = (su.getUsername() != null && !su.getUsername().isEmpty()) && (su.getPassword() != null && !su.getPassword().isEmpty());
        boolean isAppLogin = (su.getClientId() != null && !su.getClientId().isEmpty()) && (su.getClientSecret() != null && !su.getClientSecret().isEmpty());
        if (isUserLogin || isAppLogin) {
            _log(Level.INFO, "Matching credentials found in configuration file. OAuth 2.0 mode: " + isAppLogin);
            if (isAppLogin) {
                //OAuth 2.0 mode authentication
                //"App Login" - authenticating using client_id and client_secret stored in config
                if (su.getOAuth2Endpoint() == null || su.getOAuth2Endpoint().isEmpty()){
                    su.setOAuth2Endpoint(DEFAULT_OAUTH);
                }
                if (su.getOAuth2Endpoint().charAt(su.getOAuth2Endpoint().length() - 1) != '/') {
                    su.setOAuth2Endpoint(su.getOAuth2Endpoint() + "/");
                }
                _log(Level.INFO, "Service is secured by " + su.getOAuth2Endpoint() + ": getting new token...");
                String uri = su.getOAuth2Endpoint() + "token?client_id=" + URLEncoder.encode(su.getClientId(),"UTF-8") + "&client_secret=" + URLEncoder.encode(su.getClientSecret(), "UTF-8") + "&grant_type=client_credentials&f=json";
                String tokenResponse = webResponseToString(doHTTPRequest(uri, "POST"));
                token = extractToken(tokenResponse, "access_token");
                if (token != null && !token.isEmpty()) {
                    token = exchangePortalTokenForServerToken(token, su);
                }
            } else {
                //standalone ArcGIS Server token-based authentication

                //if a request is already being made to generate a token, just let it go
                if (url.toLowerCase().contains("/generatetoken")){
                    String tokenResponse = webResponseToString(doHTTPRequest(url, "POST"));
                    token = extractToken(tokenResponse, "token");
                    return token;
                }

                String infoUrl;
                //lets look for '/rest/' in the request url (could be 'rest/services', 'rest/community'...)
                if (url.toLowerCase().contains("/rest/")){
                    infoUrl = url.substring(0, url.indexOf("/rest/"));
                    infoUrl += "/rest/info?f=json";
                    //if we don't find 'rest', lets look for the portal specific 'sharing' instead
                }else if (url.toLowerCase().contains("/sharing/")){
                    infoUrl = url.substring(0, url.indexOf("sharing"));
                    infoUrl += "/sharing/rest/info?f=json";
                }else
                    return "-1"; //return -1, signaling that infourl can not be found

                if (!infoUrl.isEmpty()) {

                    _log(Level.INFO, "[Info]: Querying security endpoint...");

                    String tokenServiceUri = su.getTokenServiceUri();

                    if (tokenServiceUri == null || tokenServiceUri.isEmpty()){
                        _log(Level.INFO, "Token URL not cached.  Querying rest info page...");
                        String infoResponse = webResponseToString(doHTTPRequest(infoUrl, "GET"));
                        tokenServiceUri = getJsonValue(infoResponse, "tokenServicesUrl");

                        //If the tokenServiceUri does not exist, check the owningSystemUrl for token endpoint
                        if (tokenServiceUri.isEmpty()){
                            String owningSystemUrl = getJsonValue(infoResponse, "owningSystemUrl");
                            if (!owningSystemUrl.isEmpty()){
                                tokenServiceUri = owningSystemUrl + "/sharing/generateToken";
                            }
                        }
                        su.setTokenServiceUri(tokenServiceUri);
                    }

                    if (tokenServiceUri != null && !tokenServiceUri.isEmpty()){
                        _log(Level.INFO, "[Info]: Service is secured by " + tokenServiceUri + ": getting new token...");
                        String uri = tokenServiceUri + "?f=json&request=getToken&referer=" + URLEncoder.encode(PROXY_REFERER,"UTF-8") + "&expiration=60&username=" + URLEncoder.encode(su.getUsername(),"UTF-8") + "&password=" + URLEncoder.encode(su.getPassword(), "UTF-8");
                        String tokenResponse = webResponseToString(doHTTPRequest(uri, "POST"));
                        token = extractToken(tokenResponse, "token");
                    }
                }
            }
        }
        return token;
    }

    private boolean pathMatched(String allowedRefererPath, String refererPath){
        //If equal, return true
        if (refererPath.equals(allowedRefererPath)){
            return true;
        }

        //If the allowedRefererPath contain a ending star and match the begining part of referer, it is proper start with.
        if (allowedRefererPath.endsWith("*")){
            allowedRefererPath = allowedRefererPath.substring(0, allowedRefererPath.length()-1);
            if (refererPath.toLowerCase().startsWith(allowedRefererPath.toLowerCase())){
                return true;
            }
        }
        return false;
    }

    private boolean domainMatched(String allowedRefererDomain, String refererDomain) throws MalformedURLException{
        if (allowedRefererDomain.equals(refererDomain)){
            return true;
        }

        //try if the allowed referer contains wildcard for subdomain
        if (allowedRefererDomain.contains("*")){
            if (checkWildcardSubdomain(allowedRefererDomain, refererDomain)){
                return true;//return true if match wildcard subdomain
            }
        }

        return false;
    }

    private boolean protocolMatch(String allowedRefererProtocol, String refererProtocol){
        return allowedRefererProtocol.equals(refererProtocol);
    }

    private boolean checkReferer(String[] allowedReferers, String referer) throws MalformedURLException{
        if (allowedReferers != null && allowedReferers.length > 0){
            if (allowedReferers.length == 1 && allowedReferers[0].equals("*")) {
                return true; //speed-up
            }

            for (String allowedReferer : allowedReferers){
                allowedReferer = allowedReferer.replaceAll("\\s", "");

                URL refererURL = new URL(referer);
                URL allowedRefererURL;

                //since the allowedReferer can be a malformedURL, we first construct a valid one to be compared with referer
                //if allowedReferer starts with https:// or http://, then exact match is required
                if (allowedReferer.startsWith("https://") || allowedReferer.startsWith("http://")){
                    allowedRefererURL = new URL(allowedReferer);
                } else {

                    String protocol = refererURL.getProtocol();
                    //if allowedReferer starts with "//" or no protocol, we use the one from refererURL to prefix to allowedReferer.
                    if (allowedReferer.startsWith("//")){
                        allowedRefererURL = new URL(protocol+":"+allowedReferer);
                    } else {
                        //if the allowedReferer looks like "example.esri.com"
                        allowedRefererURL = new URL(protocol+"://"+allowedReferer);
                    }
                }

                //Check if both domain and path match
                if (protocolMatch(allowedRefererURL.getProtocol(), refererURL.getProtocol()) &&
                        domainMatched(allowedRefererURL.getHost(), refererURL.getHost()) &&
                        pathMatched(allowedRefererURL.getPath(), refererURL.getPath())){
                    return true;
                }
            }
            return false;//no-match in allowedReferer, does not allow the request
        }
        return true;//when allowedReferer is null, then allow everything
    }

    //check the wildcard in allowedReferer in proxy.config
    private boolean checkWildcardSubdomain(String allowedRefererDomain, String refererDomain) throws MalformedURLException{

        String[] allowedRefererParts = allowedRefererDomain.split("(\\.)");
        String[] refererParts = refererDomain.split("(\\.)");

        if (allowedRefererParts.length != refererParts.length){
            return false;
        }

        int index = allowedRefererParts.length-1;
        while(index >= 0){
            if (allowedRefererParts[index].equalsIgnoreCase(refererParts[index])){
                index = index - 1;
            }else{
                if(allowedRefererParts[index].equals("*")){
                    index = index - 1;
                    continue; //next
                }
                return false;
            }
        }
        return true;
    }

    private String getFullUrl(String url){
        return url.startsWith("//") ? url.replace("//","https://") : url;
    }

    private String exchangePortalTokenForServerToken(String portalToken, ServerUrl su) throws IOException{
        String url = getFullUrl(su.getUrl());
        _log(Level.INFO, "[Info]: Exchanging Portal token for Server-specific token for " + url + "...");
        String uri = su.getOAuth2Endpoint().substring(0, su.getOAuth2Endpoint().toLowerCase().indexOf("/oauth2/")) +
                "/generateToken?token=" + URLEncoder.encode(portalToken,"UTF-8") + "&serverURL=" + URLEncoder.encode(url,"UTF-8") + "&f=json";
        String tokenResponse = webResponseToString(doHTTPRequest(uri, "GET"));
        return extractToken(tokenResponse, "token");
    }

    private String addTokenToUri(String uri, String token) {
        if (token != null && !token.isEmpty())
            uri += uri.contains("?") ? "&token=" + token : "?token=" + token;
        return uri;
    }

    private String extractToken(String tokenResponse, String key) {
        String token = getJsonValue(tokenResponse, key);
        if (token == null || token.isEmpty()) {
            _log(Level.WARNING, "Token cannot be obtained: " + tokenResponse);
        } else {
            _log(Level.INFO, "Token obtained: " + token);
        }
        return token;
    }

    private String getJsonValue(String text, String key) {
        _log(Level.FINE, "JSON Response: " + text);
        int i = text.indexOf(key);
        String value = "";
        if (i > -1) {
            value = text.substring(text.indexOf(':', i) + 1).trim();
            value = (value.length() > 0 && value.charAt(0) == '"') ?
                    value.substring(1, value.indexOf('"', 1)) :
                    value.substring(0, Math.max(0, Math.min(Math.min(value.indexOf(","), value.indexOf("]")), value.indexOf("}"))));
        }
        _log(Level.FINE, "Extracted Value: " + value);
        return value;
    }

    private void cleanUpRatemap(ConcurrentHashMap<String, RateMeter> ratemap) {
        Set<Map.Entry<String, RateMeter>> entrySet = ratemap.entrySet();
        for (Map.Entry<String,RateMeter> entry : entrySet){
            RateMeter rate = entry.getValue();
            if (rate.canBeCleaned())
                ratemap.remove(entry.getKey(), rate);
        }
    }

    /**
     * Static
     */

    private static ProxyConfig getConfig()  throws IOException{
        ProxyConfig config = ProxyConfig.getCurrentConfig();
        if (config != null)
            return config;
        else
            throw new FileNotFoundException("The proxy configuration file");
    }

    //writing Log file
    private static Object _lockobject = new Object();
    private static Logger logger = Logger.getLogger("ESRI_PROXY_LOGGER");

    private boolean okToLog(){
        try{
            ProxyConfig proxyConfig = getConfig();
            String filename = proxyConfig.getLogFile();
            return filename != null && !filename.equals("") && !filename.isEmpty() && logger != null;
        }catch (Exception e) {
            e.printStackTrace();
        }
        return false;
    }

    private static void _log(Level level, String s, Throwable thrown) {
        try {

            ProxyConfig proxyConfig = getConfig();
            String filename = proxyConfig.getLogFile();
            boolean okToLog = filename != null && !filename.isEmpty() && logger != null;
            synchronized (_lockobject) {

                if (okToLog) {

                    if (logger.getUseParentHandlers()){
                        FileHandler fh = new FileHandler(filename, true);
                        logger.addHandler(fh);
                        SimpleFormatter formatter = new SimpleFormatter();
                        fh.setFormatter(formatter);
                        logger.setUseParentHandlers(false);

                        String logLevelStr = proxyConfig.getLogLevel();
                        Level logLevel = Level.SEVERE;

                        if (logLevelStr != null){
                            try {
                                logLevel = Level.parse(logLevelStr);
                            } catch (IllegalArgumentException e) {
                                SimpleDateFormat dt = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                                System.err.println(dt.format(new Date()) + ": " + logLevelStr + " is not a valid logging level.  Defaulting to SEVERE.");
                            }
                        }

                        logger.setLevel(logLevel);

                        logger.info("Log handler configured and initialized.");
                    }

                    if (thrown != null){
                        logger.log(level, DataValidUtil.replaceCRLF(s, "__"), thrown);
                    } else {
                        logger.log(level, DataValidUtil.replaceCRLF(s, "__"));
                    }
                }
            }
        }
        catch (Exception e) {
            SimpleDateFormat dt = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            System.err.println("Error writing to log: ");
            System.err.println(dt.format(new Date()) + " " + s);
            e.printStackTrace();
        }
    }

    private static void _log(String s, Throwable thrown){
        _log(Level.SEVERE, s, thrown);
    }

    private static void _log(Level level, String s){
        _log(level, s, null);
    }

    public static class ProxyConfig
    {
        public boolean canReadProxyConfig(){
            InputStream configFile = ProxyConfig.class.getClassLoader().getResourceAsStream("proxy.config");
            return (configFile != null);
        }

        public synchronized static ProxyConfig loadProxyConfig()  throws IOException{
            ProxyConfig config = null;

            InputStream configFile = ProxyConfig.class.getClassLoader().getResourceAsStream("proxy.config");
            if (configFile != null) {
                BufferedReader reader = new BufferedReader( new InputStreamReader (configFile, "UTF-8"));
                String line;
                StringBuilder stringBuilder = new StringBuilder();

                while( ( line = reader.readLine() ) != null ) {
                    stringBuilder.append( line );
                }
                reader.close();

                String configFileStr = stringBuilder.toString();
                configFileStr = configFileStr.replaceAll("(?ms)<!\\-\\-(.+?)\\-\\->", "");

                Pattern p = Pattern.compile("<\\s*ProxyConfig(.+?)>", Pattern.MULTILINE | Pattern.DOTALL);
                Matcher m = p.matcher(configFileStr);
                boolean found = m.find();

                if (found){

                    String proxyConfigAttributes = m.group(1);

                    config = new ProxyConfig();

                    if (proxyConfigAttributes != null && !proxyConfigAttributes.isEmpty()){
                        String mustMatch = ProxyConfig.getAttributeWithRegex("mustMatch", proxyConfigAttributes);
                        if (mustMatch != null && !mustMatch.isEmpty()){
                            config.setMustMatch(Boolean.parseBoolean(mustMatch));
                        }

                        String allowedReferers = ProxyConfig.getAttributeWithRegex("allowedReferers", proxyConfigAttributes);
                        if (allowedReferers != null && !allowedReferers.isEmpty()){
                            config.setAllowedReferers(allowedReferers.split(","));
                        }

                        String logFile = ProxyConfig.getAttributeWithRegex("logFile", proxyConfigAttributes);
                        if (logFile != null && !logFile.isEmpty()){
                            config.setLogFile(logFile);
                        }

                        String logLevel = ProxyConfig.getAttributeWithRegex("logLevel", proxyConfigAttributes);
                        if (logLevel != null && !logLevel.isEmpty()){
                            config.setLogLevel(logLevel);
                        }


                        p = Pattern.compile("<\\s*serverUrls\\s*>(.+?)<\\s*/\\s*serverUrls\\s*>", Pattern.MULTILINE | Pattern.DOTALL);
                        m = p.matcher(configFileStr);
                        found = m.find();

                        if (found) {
                            String serverUrls = m.group(1);
                            if (serverUrls != null && !serverUrls.isEmpty()) {
                                p = Pattern.compile("<\\s*serverUrl (.+?)((<\\s*/\\s*serverUrl\\s*)|/)>", Pattern.MULTILINE | Pattern.DOTALL);
                                m = p.matcher(serverUrls);

                                ArrayList<ServerUrl> serverList = new ArrayList<ServerUrl>();
                                while(m.find()){
                                    String server = m.group(1);
                                    String url = ProxyConfig.getAttributeWithRegex("url", server);
                                    String matchAll = ProxyConfig.getAttributeWithRegex("matchAll", server);
                                    String oauth2Endpoint = ProxyConfig.getAttributeWithRegex("oauth2Endpoint", server);
                                    String username = ProxyConfig.getAttributeWithRegex("username", server);
                                    String password = ProxyConfig.getAttributeWithRegex("password", server);
                                    String clientId = ProxyConfig.getAttributeWithRegex("clientId", server);
                                    String clientSecret = ProxyConfig.getAttributeWithRegex("clientSecret", server);
                                    String rateLimit = ProxyConfig.getAttributeWithRegex("rateLimit", server);
                                    String rateLimitPeriod = ProxyConfig.getAttributeWithRegex("rateLimitPeriod", server);
                                    String tokenServiceUri = ProxyConfig.getAttributeWithRegex("tokenServiceUri", server);
                                    String hostRedirect = ProxyConfig.getAttributeWithRegex("hostRedirect", server);

                                    serverList.add(new ServerUrl(url, matchAll, oauth2Endpoint, username, password, clientId, clientSecret, rateLimit, rateLimitPeriod, tokenServiceUri, hostRedirect));
                                }

                                config.setServerUrls(serverList.toArray(new ServerUrl[serverList.size()]));
                            }
                        }
                    }


                }
            }
            return config;

        }

        private static String getAttributeWithRegex(String property, String tag){
            Pattern p = Pattern.compile(property + "=\\s*\"\\s*(.+?)\\s*\"");
            Matcher m = p.matcher(tag);
            boolean found = m.find();
            String match = null;
            if (found){
                match = m.group(1);
            }

            return match;

        }

        private static ProxyConfig appConfig;

        public static ProxyConfig getCurrentConfig() throws IOException{


            ProxyConfig config = appConfig;

            if (config == null) {
                config = loadProxyConfig();

                if (config != null) {
                    appConfig = config;

                }
            }
            return config;
        }

        ServerUrl[] serverUrls;
        boolean mustMatch;
        String logFile;
        String logLevel;
        String[] allowedReferers;

        public ServerUrl[] getServerUrls() {
            return this.serverUrls;
        }
        public void setServerUrls(ServerUrl[] value){
            this.serverUrls = value;
        }

        public boolean getMustMatch(){
            return this.mustMatch;
        }
        public void setMustMatch(boolean value){
            this.mustMatch = value;
        }

        public String[] getAllowedReferers(){
            return this.allowedReferers;
        }
        public void setAllowedReferers(String[] value){
            this.allowedReferers = value;
        }

        public String getLogFile(){
            return this.logFile;
        }
        public void setLogFile(String value){
            this.logFile = value;
        }

        public String getLogLevel(){
            return this.logLevel;
        }
        public void setLogLevel(String value){
            this.logLevel = value;
        }

        public ServerUrl getConfigServerUrl(String uri) {
            //split request URL to compare with allowed server URLs
            String[] uriParts = uri.split("(/)|(\\?)");
            String[] configUriParts;

            for (ServerUrl su : serverUrls) {
                //if a relative path is specified in the proxy configuration file, append what's in the request itself
                if (!su.getUrl().startsWith("http"))
                    su.setUrl(new StringBuilder(su.getUrl()).insert(0, uriParts[0]).toString());

                configUriParts = su.getUrl().split("/");

                //if the request has less parts than the config, don't allow
                if (configUriParts.length > uriParts.length) continue;

                int i;
                //try to match configUrl to the requested url, including protocol
                for (i = 0; i < configUriParts.length; i++)
                {
                    if (!configUriParts[i].toLowerCase().equals(uriParts[i].toLowerCase()) ) break;
                }
                if (i == configUriParts.length)
                {
                    //if the urls don't match exactly, and the individual matchAll tag is 'false', don't allow
                    if (configUriParts.length == uriParts.length || su.getMatchAll())
                        return su;
                }
            }

            if (this.mustMatch)
                return null;//if nothing match and mustMatch is true, return null
            else
                return new ServerUrl(uri); //if mustMatch is false send the server URL back that is the same the uri to pass thru
        }

        public static boolean isUrlPrefixMatch(String prefix, String uri){
            return uri.toLowerCase().startsWith(prefix.toLowerCase()) ||
                    uri.toLowerCase().replace("https://","http://").startsWith(prefix.toLowerCase()) ||
                    uri.toLowerCase().substring(uri.indexOf("//")).startsWith(prefix.toLowerCase());
        }
    }

    public static class ServerUrl {
        String url;
        boolean matchAll;
        String oauth2Endpoint;
        String username;
        String password;
        String clientId;
        String clientSecret;
        String rateLimit;
        String rateLimitPeriod;
        String tokenServiceUri;
        String hostRedirect;

        public ServerUrl(String url, String matchAll, String oauth2Endpoint, String username, String password, String clientId, String clientSecret, String rateLimit,
                         String rateLimitPeriod, String tokenServiceUri, String hostRedirect){

            this.url = url;
            this.matchAll = matchAll == null || matchAll.isEmpty() || Boolean.parseBoolean(matchAll);
            this.oauth2Endpoint = oauth2Endpoint;
            this.username = username;
            this.password = password;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.rateLimit = rateLimit;
            this.rateLimitPeriod = rateLimitPeriod;
            this.tokenServiceUri = tokenServiceUri;
            this.hostRedirect = hostRedirect;

        }

        public ServerUrl(String url){
            this.url = url;
        }

        private static ConcurrentHashMap<String,String> tokenServiceMap = new ConcurrentHashMap<String,String>();

        public String getUrl(){
            return this.url;
        }
        public void setUrl(String value){
            this.url = value;
        }

        public boolean getMatchAll(){
            return this.matchAll;
        }
        public void setMatchAll(boolean value){
            this.matchAll = value;
        }
        public String getHostRedirect() {
            return hostRedirect;
        }

        public void setHostRedirect(String hostRedirect) {
            this.hostRedirect = hostRedirect;
        }

        public String getOAuth2Endpoint(){
            return this.oauth2Endpoint;
        }
        public void setOAuth2Endpoint(String value){
            this.oauth2Endpoint = value;
        }

        public String getUsername(){
            return this.username;
        }
        public void setUsername(String value){
            this.username = value;
        }

        public String getPassword(){
            return this.password;
        }
        public void setPassword(String value){
            this.password = value;
        }

        public String getClientId(){
            return this.clientId;
        }
        public void setClientId(String value){
            this.clientId = value;
        }

        public String getClientSecret(){
            return this.clientSecret;
        }
        public void setClientSecret(String value){
            this.clientSecret = value;
        }

        public int getRateLimit(){
            return (this.rateLimit == null || this.rateLimit.isEmpty() ) ? -1 : Integer.parseInt(this.rateLimit);
        }
        public void setRateLimit(int value){
            this.rateLimit = String.valueOf(value);
        }

        public int getRateLimitPeriod(){
            return (this.rateLimitPeriod == null || this.rateLimitPeriod.isEmpty() ) ? -1 : Integer.parseInt(this.rateLimitPeriod);
        }
        public void setRateLimitPeriod(int value){
            this.rateLimitPeriod = String.valueOf(value);
        }

        public String getTokenServiceUri(){
            if (this.tokenServiceUri == null && tokenServiceMap != null){
                this.tokenServiceUri = tokenServiceMap.get(this.url);
            }
            return this.tokenServiceUri;
        }

        public void setTokenServiceUri(String value){

            this.tokenServiceUri = value;

            tokenServiceMap.put(this.url, value);
        }
    }

    private static Object _rateMapLock = new Object();

    private static void sendErrorResponse(HttpServletResponse response, String errorDetails, String errorMessage, int errorCode) throws IOException{
        response.setHeader("Content-Type", "application/json");
        String message = "{" +
                "\"error\": {" +
                "\"code\": " + errorCode + "," +
                "\"details\": [" +
                "\"" + errorDetails + "\"" +
                "], \"message\": \"" + errorMessage + "\"}}";

        response.setStatus(errorCode);
        OutputStream output = response.getOutputStream();

        output.write(message.getBytes());

        output.flush();
    }

    private static void _sendURLMismatchError(HttpServletResponse response, String attemptedUri) throws IOException{
        sendErrorResponse(response, "Proxy has not been set up for this URL. Make sure there is a serverUrl in the configuration file that matches: " + attemptedUri,
                "Proxy has not been set up for this URL.", HttpServletResponse.SC_FORBIDDEN);
    }

    private static void _sendPingMessage(HttpServletResponse response, String version, String config, String log) throws IOException{
        response.setStatus(HttpServletResponse.SC_OK);
        response.setHeader("Content-Type", "application/json");
        String message = "{ " +
                "\"Proxy Version\": \"" + version + "\"" +
                //", \"Java Version\": \"" + System.getProperty("java.version") + "\"" +
                ", \"Configuration File\": \"" + config + "\""  +
                ", \"Log File\": \"" + log + "\"" +
                "}";
        OutputStream output = response.getOutputStream();
        output.write(message.getBytes());
        output.flush();
    }

    //check if the originalUri needs to be host-redirected
    private String uriHostRedirect(String originalUri, ServerUrl serverUrl) throws MalformedURLException{
        if (serverUrl.hostRedirect != null && !serverUrl.hostRedirect.isEmpty()){
            URL request = new URL(originalUri);
            String redirectHost = serverUrl.getHostRedirect();
            redirectHost = redirectHost.endsWith("/")?redirectHost.substring(0, redirectHost.length()-1):redirectHost;
            String queryString = request.getQuery();
            return redirectHost + request.getPath() + ((queryString != null) ? ("?" + queryString) : "");
        }
        return originalUri;
    }

    @SuppressWarnings("unchecked")
    private ConcurrentHashMap<String, RateMeter> castRateMap(Object rateMap){
        return (ConcurrentHashMap<String, RateMeter>) rateMap;
    }
%><%
    String originalUri = request.getQueryString();
    _log(Level.INFO, "Creating request for: " + originalUri);
    ServerUrl serverUrl;

    try {
        try {

            out.clear();
            out = pageContext.pushBody();

            //check if the originalUri to be proxied is empty
            if (originalUri == null || originalUri.isEmpty()){
                String errorMessage = "This proxy does not support empty parameters.";
                _log(Level.WARNING, errorMessage);
                sendErrorResponse(response, errorMessage, "400 - " + errorMessage, HttpServletResponse.SC_BAD_REQUEST);
                return;
            }

            //check if the originalUri to be proxied is "ping"
            if (originalUri.equalsIgnoreCase("ping")){
                String checkConfig = getConfig().canReadProxyConfig() ? "OK": "Not Readable";
                String checkLog = okToLog() ? "OK": "Not Exist/Readable";
                _sendPingMessage(response, version, checkConfig, checkLog);
                return;
            }

            //check if the originalUri is encoded then decode it
            if (originalUri.toLowerCase().startsWith("http%3a%2f%2f") || originalUri.toLowerCase().startsWith("https%3a%2f%2f")) originalUri = URLDecoder.decode(originalUri, "UTF-8");

            //check the Referer in request header against the allowedReferer in proxy.config
            String[] allowedReferers = getConfig().getAllowedReferers();
            if (allowedReferers != null && allowedReferers.length > 0 && request.getHeader("referer") != null){
                setReferer(request.getHeader("referer")); //replace PROXY_REFERER with real proxy
                String httpReferer;
                try{
                    //only use the hostname of the referer url
                    httpReferer = new URL(request.getHeader("referer")).toString();
                }catch(Exception e){
                    _log(Level.WARNING, "Proxy is being used from an invalid referer: " + request.getHeader("referer"));
                    sendErrorResponse(response, "Error verifying referer. ", "403 - Forbidden: Access is denied.", HttpServletResponse.SC_FORBIDDEN);
                    return;
                }
                if (!checkReferer(allowedReferers, httpReferer)){
                    _log(Level.WARNING, "Proxy is being used from an unknown referer: " + request.getHeader("referer"));
                    sendErrorResponse(response, "Unsupported referer. ", "403 - Forbidden: Access is denied.", HttpServletResponse.SC_FORBIDDEN);
                    return;
                }
            }

            //Check to see if allowed referer list is specified and reject if referer is null
            if (request.getHeader("referer") == null && allowedReferers != null && !allowedReferers[0].equals("*")) {
                _log(Level.WARNING, "Proxy is being called by a null referer.  Access denied.");
                sendErrorResponse(response, "Current proxy configuration settings do not allow requests which do not include a referer header.", "403 - Forbidden: Access is denied.", HttpServletResponse.SC_FORBIDDEN);
                return;
            }

            //get the serverUrl from proxy.config
            serverUrl = getConfig().getConfigServerUrl(originalUri);
            if (serverUrl == null) {
                //if no serverUrl found, send error message and get out.
                _sendURLMismatchError(response, originalUri);
                return;
            }
        } catch (IllegalStateException e) {
            _log(Level.WARNING, "Proxy is being used for an unsupported service: " + originalUri);

            _sendURLMismatchError(response, originalUri);

            return;
        }

        //Throttling: checking the rate limit coming from particular referrer
        if ( serverUrl.getRateLimit() > -1) {
            synchronized(_rateMapLock){
                ConcurrentHashMap<String, RateMeter> ratemap = castRateMap(application.getAttribute("rateMap"));
                if (ratemap == null){
                    ratemap = new ConcurrentHashMap<String, RateMeter>();
                    application.setAttribute("rateMap", ratemap);
                    application.setAttribute("rateMap_cleanup_counter", 0);
                }


                String key = "[" + serverUrl.getUrl() + "]x[" + request.getRemoteAddr() + "]";
                RateMeter rate = ratemap.get(key);
                if (rate == null) {
                    rate = new RateMeter(serverUrl.getRateLimit(), serverUrl.getRateLimitPeriod());
                    RateMeter rateCheck = ratemap.putIfAbsent(key, rate);
                    if (rateCheck != null){
                        rate = rateCheck;
                    }
                }
                if (!rate.click()) {
                    _log(Level.WARNING, "Pair " + key + " is throttled to " + serverUrl.getRateLimit() + " requests per " + serverUrl.getRateLimitPeriod() + " minute(s). Come back later.");

                    sendErrorResponse(response, "This is a metered resource, number of requests have exceeded the rate limit interval.",
                            "Error 429 - Too Many Requests", 429);

                    return;
                }

                //making sure the rateMap gets periodically cleaned up so it does not grow uncontrollably
                int cnt = (Integer) application.getAttribute("rateMap_cleanup_counter");
                cnt++;
                if (cnt >= CLEAN_RATEMAP_AFTER) {
                    cnt = 0;
                    cleanUpRatemap(ratemap);
                }
                application.setAttribute("rateMap_cleanup_counter", cnt);
            }
        }

        //readying body (if any) of POST request
        byte[] postBody = readRequestPostBody(request);
        String post = new String(postBody);
        //check if the originalUri needs to be host-redirected
        String requestUri = uriHostRedirect(originalUri, serverUrl);

        //if token comes with client request, it takes precedence over token or credentials stored in configuration
        boolean hasClientToken = requestUri.contains("?token=") || requestUri.contains("&token=") || post.contains("?token=") || post.contains("&token=");
        String token = "";
        if (!hasClientToken) {
            // Get new token and append to the request.
            // But first, look up in the application scope, maybe it's already there:
            token = (String)application.getAttribute("token_for_" + serverUrl.getUrl());
            boolean tokenIsInApplicationScope = token != null && !token.isEmpty();

            //if still no token, let's see if there are credentials stored in configuration which we can use to obtain new token
            if (!tokenIsInApplicationScope){
                token = getNewTokenIfCredentialsAreSpecified(serverUrl, requestUri);
            }

            if (token != null && !token.isEmpty() && !tokenIsInApplicationScope) {
                //storing the token in Application scope, to do not waste time on requesting new one until it expires or the app is restarted.
                application.setAttribute("token_for_" + serverUrl.getUrl(), token);
            }
        }

        //forwarding original request
        HttpURLConnection con = forwardToServer(request, addTokenToUri(requestUri, token), postBody);

        if ( token == null || token.isEmpty() || hasClientToken) {
            //if token is not required or provided by the client, just fetch the response as is:
            fetchAndPassBackToClient(con, response, true);
        } else {
            //credentials for secured service have come from configuration file:
            //it means that the proxy is responsible for making sure they were properly applied:

            //first attempt to send the request:
            boolean tokenRequired = fetchAndPassBackToClient(con, response, false);

            //checking if previously used token has expired and needs to be renewed
            if (tokenRequired) {
                _log(Level.INFO, "Renewing token and trying again.");
                //server returned error - potential cause: token has expired.
                //we'll do second attempt to call the server with renewed token:
                token = getNewTokenIfCredentialsAreSpecified(serverUrl, requestUri);
                con = forwardToServer(request, addTokenToUri(requestUri, token), postBody);

                //storing the token in Application scope, to do not waste time on requesting new one until it expires or the app is restarted.
                synchronized(this){
                    application.setAttribute("token_for_" + serverUrl.getUrl(), token);
                }

                fetchAndPassBackToClient(con, response, true);
            }
        }
    } catch (FileNotFoundException e){
        try {
            _log("404 Not Found .", e);
            response.sendError(404, e.getLocalizedMessage() + " is NOT Found.");
            return;
        }catch (IOException finalErr){
            _log("There was an error sending a response to the client.  Will not try again.", finalErr);
        }
    } catch (IOException e){
        try {
            _log("A fatal proxy error occurred.", e);
            response.sendError(500, e.getLocalizedMessage());
            return;
        } catch (IOException finalErr){
            _log("There was an error sending a response to the client.  Will not try again.", finalErr);
        }
    }
%>
