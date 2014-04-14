<%@page session="false"%>
<%@page import=
"java.net.HttpURLConnection,
java.net.URL,
java.net.URLEncoder,
java.net.URLDecoder,
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
java.text.SimpleDateFormat" %>

<!-- ----------------------------------------------------------
 *
 * JSP proxy client
 *
 * Version 1.0
 * See https://github.com/Esri/resource-proxy for more information.
 *
----------------------------------------------------------- -->

<%!
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

private String PROXY_REFERER = "http://localhost/proxy.jsp";
private static String DEFAULT_OAUTH = "https://www.arcgis.com/sharing/oauth2/";
private static int CLEAN_RATEMAP_AFTER = 10000;

//setReferer if real referer exist
private void setReferer(String r){
    PROXY_REFERER = r;
}

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

private HttpURLConnection forwardToServer(HttpServletRequest request,String uri, byte[] postBody) throws IOException{
    return
            postBody.length > 0 ?
                    doHTTPRequest(uri,postBody,"POST",request.getHeader("Referer"),request.getContentType()) :
                        doHTTPRequest(uri, request.getMethod());
}

private boolean fetchAndPassBackToClient(HttpURLConnection con, HttpServletResponse clientResponse, boolean ignoreAuthenticationErrors) throws IOException{
	if (con!=null){
		if (con.getContentType() != null) clientResponse.setContentType(con.getContentType());
		if (con.getContentEncoding() != null)  clientResponse.addHeader("Content-Encoding", con.getContentEncoding());
		
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
        int bytesRead = 0;

        while ((bytesRead = byteStream.read(bytes, 0, length)) > 0) {
            buffer.write(bytes,0,bytesRead);
        }
        buffer.flush();

        byte[] byteResponse = buffer.toByteArray();
        OutputStream ostream = clientResponse.getOutputStream();
        ostream.write(byteResponse);
        ostream.close();
        byteStream.close();
	}
	return false;
}

private HttpURLConnection doHTTPRequest(String uri, String method) throws IOException{

    byte[] bytes = null;
    String contentType = null;
    if (method.equals("POST")){
        String[] uriArray = uri.split("\\?");

        if (uriArray.length > 1){
            contentType = "application/x-www-form-urlencoded";
            String queryString = uriArray[1];

            bytes = URLEncoder.encode(queryString,"UTF-8").getBytes();
        }
    }
    return doHTTPRequest(uri, bytes, method, PROXY_REFERER, contentType);
}

private HttpURLConnection doHTTPRequest(String uri, byte[] bytes, String method, String referer, String contentType) throws IOException{
    URL url = new URL(uri);
    HttpURLConnection con = (HttpURLConnection)url.openConnection();

    con.setConnectTimeout(5000);
    con.setReadTimeout(10000);

    con.setRequestProperty("Referer", referer);
    con.setRequestMethod(method);

    if (bytes != null && bytes.length > 0 || method.equals("POST")) {

    if (bytes == null){
        bytes = new byte[0];
    }

        con.setRequestMethod("POST");
        con.setDoOutput(true);
        if (contentType == null || contentType.isEmpty()){
            contentType = "application/x-www-form-urlencoded";
        }

        con.setRequestProperty("Content-Type", contentType);

        OutputStream os = con.getOutputStream();
        os.write(bytes);
    }

    return con;
}

private String webResponseToString(HttpURLConnection con) throws IOException{

    InputStream in = con.getInputStream();

    Reader reader = new BufferedReader(new InputStreamReader(in,"UTF-8"));
    StringBuffer content = new StringBuffer();
    char[] buffer = new char[5000];
    int n;

    while ( ( n = reader.read(buffer)) != -1 ) {
        content.append(buffer,0,n);
    }
    reader.close();

    String strResponse = content.toString();

    return strResponse;
}

private String getNewTokenIfCredentialsAreSpecified(ServerUrl su, String url) throws IOException{
    String token = "";
    boolean isUserLogin = (su.getUsername() != null && !su.getUsername().isEmpty()) && (su.getPassword() != null && !su.getPassword().isEmpty());
    boolean isAppLogin = (su.getClientId() != null && !su.getClientId().isEmpty()) && (su.getClientSecret() != null && !su.getClientSecret().isEmpty());
    if (isUserLogin || isAppLogin) {
        _log(Level.INFO,"Matching credentials found in configuration file. OAuth 2.0 mode: " + isAppLogin);
        if (isAppLogin) {
            //OAuth 2.0 mode authentication
            //"App Login" - authenticating using client_id and client_secret stored in config
            if (su.getOAuth2Endpoint() == null || su.getOAuth2Endpoint().isEmpty()){
                su.setOAuth2Endpoint(DEFAULT_OAUTH);
            }
            if (su.getOAuth2Endpoint().charAt(su.getOAuth2Endpoint().length() - 1) != '/') {
                su.setOAuth2Endpoint(su.getOAuth2Endpoint() + "/");
            }
            _log(Level.INFO,"Service is secured by " + su.getOAuth2Endpoint() + ": getting new token...");
            String uri = su.getOAuth2Endpoint() + "token?client_id=" + su.getClientId() + "&client_secret=" + su.getClientSecret() + "&grant_type=client_credentials&f=json";
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
            
            String infoUrl = "";
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
            	
            if (infoUrl != "") {

                _log(Level.INFO,"[Info]: Querying security endpoint...");

                String tokenServiceUri = su.getTokenServiceUri();

                if (tokenServiceUri == null || tokenServiceUri.isEmpty()){
                    _log(Level.INFO,"Token URL not cached.  Querying rest info page...");
                    String infoResponse = webResponseToString(doHTTPRequest(infoUrl, "GET"));
                    tokenServiceUri = getJsonValue(infoResponse, "tokenServicesUrl");
                    su.setTokenServiceUri(tokenServiceUri);
                }

                 if (tokenServiceUri != null & !tokenServiceUri.isEmpty()){
                    _log(Level.INFO,"[Info]: Service is secured by " + tokenServiceUri + ": getting new token...");
                    String uri = tokenServiceUri + "?f=json&request=getToken&referer=" + PROXY_REFERER + "&expiration=60&username=" + su.getUsername() + "&password=" + su.getPassword();
                    String tokenResponse = webResponseToString(doHTTPRequest(uri, "POST"));
                    token = extractToken(tokenResponse, "token");
                 }
            }
        }
    }
    return token;
}

private String getFullUrl(String url){
    return url.startsWith("//") ? url.replace("//","https://") : url;
}

private String exchangePortalTokenForServerToken(String portalToken, ServerUrl su) throws IOException{
    String url = getFullUrl(su.getUrl());
    _log(Level.INFO,"[Info]: Exchanging Portal token for Server-specific token for " + url + "...");
    String uri = su.getOAuth2Endpoint().substring(0, su.getOAuth2Endpoint().toLowerCase().indexOf("/oauth2/")) +
         "/generateToken?token=" + portalToken + "&serverURL=" + url + "&f=json";
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
        _log(Level.WARNING,"Token cannot be obtained: " + tokenResponse);
    } else {
        _log(Level.INFO,"Token obtained: " + token);
    }
    return token;
}

private String getJsonValue(String text, String key) {
    _log(Level.FINE,"JSON Response: " + text);
    int i = text.indexOf(key);
    String value = "";
    if (i > -1) {
        value = text.substring(text.indexOf(':', i) + 1).trim();
        value = (value.length() > 0 && value.charAt(0) == '"') ?
            value.substring(1, value.indexOf('"', 1)) :
            value.substring(0, Math.max(0, Math.min(Math.min(value.indexOf(","), value.indexOf("]")), value.indexOf("}"))));
    }
    _log(Level.FINE,"Extracted Value: " + value);
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
    private static void _log(Level level,String s,Throwable thrown) {
        try {

            ProxyConfig proxyConfig = getConfig();
            String filename = proxyConfig.getLogFile();
            boolean okToLog = filename != null && !filename.isEmpty() && logger != null;
            synchronized (_lockobject) {

                if (okToLog) {

                    if (logger.getUseParentHandlers()){
                        FileHandler fh = new FileHandler(filename,true);
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
                        logger.log(level,s,thrown);
                    } else {
                        logger.log(level,s);
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

    private static void _log(String s,Throwable thrown){
        _log(Level.SEVERE,s,thrown);
    }

    private static void _log(Level level, String s){
        _log(level,s,null);
    }

public static class ProxyConfig
{

    public synchronized static ProxyConfig loadProxyConfig()  throws IOException{
        ProxyConfig config = null;

        InputStream configFile = ProxyConfig.class.getClassLoader().getResourceAsStream("proxy.config");
         if (configFile != null) {
             BufferedReader reader = new BufferedReader( new InputStreamReader (configFile,"UTF-8"));
             String line = null;
             StringBuilder stringBuilder = new StringBuilder();

             while( ( line = reader.readLine() ) != null ) {
                 stringBuilder.append( line );
             }

             String configFileStr = stringBuilder.toString();
             configFileStr = configFileStr.replaceAll("(?ms)<!\\-\\-(.+?)\\-\\->","");

             Pattern p = Pattern.compile("<\\s*ProxyConfig(.+?)>",Pattern.MULTILINE | Pattern.DOTALL);
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

                    String logFile = ProxyConfig.getAttributeWithRegex("logFile",proxyConfigAttributes);
                    if (logFile != null && !logFile.isEmpty()){
                        config.setLogFile(logFile);
                    }

                    String logLevel = ProxyConfig.getAttributeWithRegex("logLevel",proxyConfigAttributes);
                    if (logLevel != null && !logLevel.isEmpty()){
                        config.setLogLevel(logLevel);
                    }


                    p = Pattern.compile("<\\s*serverUrls\\s*>(.+?)<\\s*/\\s*serverUrls\\s*>",Pattern.MULTILINE | Pattern.DOTALL);
                    m = p.matcher(configFileStr);
                    found = m.find();

                    if (found) {
                        String serverUrls = m.group(1);
                        if (serverUrls != null && !serverUrls.isEmpty()) {
                            p = Pattern.compile("<\\s*serverUrl (.+?)((<\\s*/\\s*serverUrl\\s*)|/)>",Pattern.MULTILINE | Pattern.DOTALL);
                            m = p.matcher(serverUrls);

                            ArrayList<ServerUrl> serverList = new ArrayList<ServerUrl>();
                            while(m.find()){
                                String server = m.group(1);
                                String url = ProxyConfig.getAttributeWithRegex("url",server);
                                String matchAll = ProxyConfig.getAttributeWithRegex("matchAll",server);
                                String oauth2Endpoint = ProxyConfig.getAttributeWithRegex("oauth2Endpoint",server);
                                String username = ProxyConfig.getAttributeWithRegex("username",server);
                                String password = ProxyConfig.getAttributeWithRegex("password",server);
                                String clientId = ProxyConfig.getAttributeWithRegex("clientId",server);
                                String clientSecret = ProxyConfig.getAttributeWithRegex("clientSecret",server);
                                String rateLimit = ProxyConfig.getAttributeWithRegex("rateLimit",server);
                                String rateLimitPeriod = ProxyConfig.getAttributeWithRegex("rateLimitPeriod",server);
                                String tokenServiceUri = ProxyConfig.getAttributeWithRegex("tokenServiceUri",server);

                                serverList.add(new ServerUrl(url,matchAll,oauth2Endpoint,username,password,clientId,clientSecret,rateLimit,rateLimitPeriod,tokenServiceUri));
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
        String[] configUriParts = new String[] {};
                
        for (ServerUrl su : serverUrls) {
        	//if a relative path is specified in the proxy configuration file, append what's in the request itself
            if (!su.getUrl().startsWith("http"))
                su.setUrl(new StringBuilder(su.getUrl()).insert(0, uriParts[0]).toString());
        	
            configUriParts = su.getUrl().split("/");
            
            //if the request has less parts than the config, don't allow
            if (configUriParts.length > uriParts.length) continue;
            
            int i = 1;
            //skip comparing the protocol, so that either http or https is considered valid
            for (i = 1; i < configUriParts.length; i++)                
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

    public static boolean isUrlPrefixMatch(String prefix,String uri){
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

    public ServerUrl(String url,String matchAll,String oauth2Endpoint,String username,String password,String clientId,String clientSecret, String rateLimit,
            String rateLimitPeriod, String tokenServiceUri){

        this.url = url;
        this.matchAll = matchAll == null || matchAll.isEmpty() ? true : Boolean.parseBoolean(matchAll);
        this.oauth2Endpoint = oauth2Endpoint;
        this.username = username;
        this.password = password;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.rateLimit = rateLimit;
        this.rateLimitPeriod = rateLimitPeriod;
        this.tokenServiceUri = tokenServiceUri;

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

        tokenServiceMap.put(this.url,value);
    }
}

private static Object _rateMapLock = new Object();

private static void sendErrorResponse(HttpServletResponse response, String errorDetails, String errorMessage, int errorCode) throws IOException{
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

private static void _sendURLMismatchError(HttpServletResponse response) throws IOException{
     sendErrorResponse(response,"The proxy tried to resolve a prohibited or malformed URL. The server does not meet one of the preconditions that the requester put on the request.",
                "403 - Forbidden: Access is denied.",HttpServletResponse.SC_FORBIDDEN);
}
%><%
String uri = request.getQueryString();
_log(Level.INFO,"Creating request for: " + uri);
ServerUrl serverUrl;
boolean passThrough = false;
try {
    try {

        out.clear();
        out = pageContext.pushBody();
		
		if (uri == null || uri.isEmpty()){
            response.sendError(403,"This proxy does not support empty parameters.");
            return;
        }

        //check if the uri is encoded then decode it
         if (uri.startsWith("http%3a%2f%2f") || uri.startsWith("https%3a%2f%2f")) uri= URLDecoder.decode(uri, "UTF-8");

        String[] allowedReferers = getConfig().getAllowedReferers();
        if (allowedReferers != null && allowedReferers.length > 0 && request.getHeader("referer") != null){
            setReferer(request.getHeader("referer")); //replace PROXY_REFERER with real proxy
            boolean allowed = false;
            for (String allowedReferer : allowedReferers){
                if (ProxyConfig.isUrlPrefixMatch(allowedReferer, request.getHeader("referer")) || allowedReferer.equals("*")){
                    allowed = true;
                    break;
                }
            }

            if (!allowed){
                _log(Level.WARNING,"Proxy is being used from an unsupported referer: " + request.getHeader("referer"));
                sendErrorResponse(response, "Proxy is being used from an unsupported referer. ", "403 - Forbidden: Access is denied.",HttpServletResponse.SC_FORBIDDEN);
                return;
            }
        }

        serverUrl = getConfig().getConfigServerUrl(uri);
		if (serverUrl == null) {
        	//if no serverUrl found, send error message and get out.
        	_sendURLMismatchError(response);
        	return;
        } 
        passThrough = serverUrl == null;
    } catch (IllegalStateException e) {
        _log(Level.WARNING,"Proxy is being used for an unsupported service: " + uri);

        _sendURLMismatchError(response);

        return;
    }

    //Throttling: checking the rate limit coming from particular referrer
    if (!passThrough && serverUrl.getRateLimit() > -1) {
        synchronized(_rateMapLock){
            ConcurrentHashMap<String, RateMeter> ratemap = (ConcurrentHashMap<String, RateMeter>)application.getAttribute("rateMap");
            if (ratemap == null){
                ratemap = new ConcurrentHashMap<String, RateMeter>();
                application.setAttribute("rateMap",ratemap);
                application.setAttribute("rateMap_cleanup_counter",0);
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
                _log(Level.WARNING,"Pair " + key + " is throttled to " + serverUrl.getRateLimit() + " requests per " + serverUrl.getRateLimitPeriod() + " minute(s). Come back later.");

                sendErrorResponse(response,"This is a metered resource, number of requests have exceeded the rate limit interval.",
                        "Unable to proxy request for requested resource.",HttpServletResponse.SC_PAYMENT_REQUIRED);

                return;
            }

            //making sure the rateMap gets periodically cleaned up so it does not grow uncontrollably
            int cnt = ((Integer)application.getAttribute("rateMap_cleanup_counter")).intValue();
            cnt++;
            if (cnt >= CLEAN_RATEMAP_AFTER) {
                cnt = 0;
                cleanUpRatemap(ratemap);
            }
            application.setAttribute("rateMap_cleanup_counter",new Integer(cnt));
        };
    }

    //readying body (if any) of POST request
    byte[] postBody = readRequestPostBody(request);
    String post = new String(postBody);

    //if token comes with client request, it takes precedence over token or credentials stored in configuration
    boolean hasClientToken = uri.contains("?token=") || uri.contains("&token=") || post.contains("?token=") || post.contains("&token=");
    String token = "";
    if (!passThrough && !hasClientToken) {
        // Get new token and append to the request.
        // But first, look up in the application scope, maybe it's already there:
        token = (String)application.getAttribute("token_for_" + serverUrl.getUrl());
        boolean tokenIsInApplicationScope = token != null && !token.isEmpty();

        //if still no token, let's see if there are credentials stored in configuration which we can use to obtain new token
        if (!tokenIsInApplicationScope){
            token = getNewTokenIfCredentialsAreSpecified(serverUrl,uri);
        }

        if (token != null && !token.isEmpty() && !tokenIsInApplicationScope) {
            //storing the token in Application scope, to do not waste time on requesting new one until it expires or the app is restarted.
            application.setAttribute("token_for_" + serverUrl.getUrl(),token);
        }
    }

    //forwarding original request
    HttpURLConnection con = null;
    con = forwardToServer(request, addTokenToUri(uri, token), postBody);


    if (passThrough || token == null || token.isEmpty() || hasClientToken) {
        //if token is not required or provided by the client, just fetch the response as is:
        fetchAndPassBackToClient(con, response, true);
    } else {
        //credentials for secured service have come from configuration file:
        //it means that the proxy is responsible for making sure they were properly applied:

        //first attempt to send the request:
        boolean tokenRequired = fetchAndPassBackToClient(con, response, false);

        //checking if previously used token has expired and needs to be renewed
        if (tokenRequired) {
            _log(Level.INFO,"Renewing token and trying again.");
            //server returned error - potential cause: token has expired.
            //we'll do second attempt to call the server with renewed token:
            token = getNewTokenIfCredentialsAreSpecified(serverUrl,uri);
            con = forwardToServer(request, addTokenToUri(uri, token), postBody);

            //storing the token in Application scope, to do not waste time on requesting new one until it expires or the app is restarted.
            synchronized(this){
                application.setAttribute("token_for_" + serverUrl.getUrl(),token);
            }

            fetchAndPassBackToClient(con, response, true);
        }
    }
} catch (FileNotFoundException e){
	try {
		_log("404 Not Found .",e);
		response.sendError(404,e.getLocalizedMessage()+" is NOT Found.");
		return;
	}catch (IOException finalErr){
        _log("There was an error sending a response to the client.  Will not try again.", finalErr);
    }
} catch (IOException e){
    try {
        _log("A fatal proxy error occurred.",e);
        response.sendError(500,e.getLocalizedMessage());
        return;
    } catch (IOException finalErr){
        _log("There was an error sending a response to the client.  Will not try again.", finalErr);
    }
}
%>
