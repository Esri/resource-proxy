<%@ WebHandler Language="C#" Class="Proxy" %>

/*
 * DotNet proxy client.
 *
 * Version 1.1.1-beta
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
using System.Net;
using System.Text;

/// <summary>
/// class proxy
/// </summary>
public class Proxy : IHttpHandler
{
    private static string version = "1.1.1-beta";

    private static string PROXY_REFERER = "http://localhost/proxy/proxy.ashx";

    private static string DEFAULT_OAUTH = "https://www.arcgis.com/sharing/oauth2/";

    /// <summary>
    /// clean the rateMap every xxxx requests
    /// </summary>
    private static int CLEAN_RATEMAP_AFTER = 10000;

    /// <summary>
    /// Use the default system proxy
    /// </summary>
    private static IWebProxy SYSTEM_PROXY = HttpWebRequest.DefaultWebProxy;

    private static LogTraceListener logTraceListener = null;

    private static Object rateMapLock = new Object();

    /// <summary>
    /// class rate meter
    /// </summary>
    private class RateMeter
    {
        /// <summary>
        /// internal rate is stored in requests per second
        /// </summary>
        private double rate;

        private int countCap;

        private double count = 0;

        private DateTime lastUpdate = DateTime.Now;

        public RateMeter(int rateLimit, int rateLimitPeriod)
        {
            this.rate = (double)rateLimit / rateLimitPeriod / 60;
            this.countCap = rateLimit;
        }

        /// <summary>
        /// called when rate-limited endpoint is invoked
        /// </summary>
        /// <returns></returns>
        public bool Click()
        {
            TimeSpan ts = DateTime.Now - this.lastUpdate;
            this.lastUpdate = DateTime.Now;

            // assuming uniform distribution of requests over time,
            // reducing the counter according to # of seconds passed
            // since last invocation
            this.count = Math.Max(0, this.count - ts.TotalSeconds * this.rate);

            if (this.count <= this.countCap)
            {
                // good to proceed
                this.count++;
                return true;
            }

            return false;
        }

        public bool CanBeCleaned()
        {
            TimeSpan ts = DateTime.Now - this.lastUpdate;
            return this.count - ts.TotalSeconds * this.rate <= 0;
        }
    }

    public void ProcessRequest(HttpContext context)
    {
        if (Proxy.logTraceListener == null)
        {
            Proxy.logTraceListener = new LogTraceListener();
            Trace.Listeners.Add(logTraceListener);
        }

        HttpResponse response = context.Response;
        if (context.Request.Url.Query.Length < 1)
        {
            string errorMsg = "This proxy does not support empty parameters.";
            Proxy.Log(TraceLevel.Error, errorMsg);
            Proxy.SendErrorResponse(context.Response, null, errorMsg, HttpStatusCode.BadRequest);
            return;
        }

        string uri = context.Request.Url.Query.Substring(1);
        Proxy.Log(TraceLevel.Verbose, $"URI requested: {uri}");

        //if uri is ping
        if (uri.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
        {
            ProxyConfig proxyConfig = ProxyConfig.GetCurrentConfig();

            string checkConfig = (proxyConfig == null) ? "Not Readable" : "OK";
            string checkLog = string.Empty;

            if (checkConfig != "OK")
            {
                checkLog = "Can not verify";
            }
            else
            {
                string filename = proxyConfig.LogFile;
                checkLog = (!string.IsNullOrEmpty(filename)) ? "OK" : "Not Exist/Readable";

                if (checkLog == "OK")
                {
                    Proxy.Log(TraceLevel.Info, "Pinged");
                }
            }

            Proxy.SendPingResponse(response, version, checkConfig, checkLog);
            return;
        }

        //if url is encoded, decode it.
        if (uri.StartsWith("http%3a%2f%2f", StringComparison.InvariantCultureIgnoreCase) || uri.StartsWith("https%3a%2f%2f", StringComparison.InvariantCultureIgnoreCase))
        {
            uri = HttpUtility.UrlDecode(uri);
        }

        ServerUrl serverUrl;
        try
        {
            serverUrl = Proxy.GetConfig().GetConfigServerUrl(uri);

            if (serverUrl == null)
            {
                //if no serverUrl found, send error message and get out.
                string errorMsg = "The request URL does not match with the ServerUrl in proxy.config! Please check the proxy.config!";
                Proxy.Log(TraceLevel.Error, errorMsg);
                Proxy.SendErrorResponse(context.Response, null, errorMsg, HttpStatusCode.BadRequest);
                return;
            }
        }
        //if XML couldn't be parsed
        catch (InvalidOperationException ex)
        {
            string errorMsg = $"{ex.InnerException.Message} {uri}";
            Proxy.Log(TraceLevel.Error, errorMsg);
            Proxy.SendErrorResponse(context.Response, null, errorMsg, HttpStatusCode.InternalServerError);
            return;
        }
        //if mustMatch was set to true and URL wasn't in the list
        catch (ArgumentException ex)
        {
            string errorMsg = $"{ex.Message} {uri}";
            Proxy.Log(TraceLevel.Error, errorMsg);
            Proxy.SendErrorResponse(context.Response, null, errorMsg, HttpStatusCode.Forbidden);
            return;
        }

        //use actual request header instead of a placeholder, if present
        if (context.Request.Headers["referer"] != null)
        {
            Proxy.PROXY_REFERER = context.Request.Headers["referer"];
        }

        //referer
        //check against the list of referers if they have been specified in the proxy.config
        string[] allowedReferersArray = ProxyConfig.GetAllowedReferersArray();
        if (allowedReferersArray?.Length > 0 && context.Request.Headers["referer"] != null)
        {
            Proxy.PROXY_REFERER = context.Request.Headers["referer"];
            string requestReferer = context.Request.Headers["referer"];
            try
            {
                string checkValidUri = new UriBuilder(requestReferer.StartsWith("//") ? requestReferer.Substring(requestReferer.IndexOf("//") + 2) : requestReferer).Host;
            }
            catch
            {
                Proxy.Log(TraceLevel.Warning, $"Proxy is being used from an invalid referer: {context.Request.Headers["referer"]}");
                Proxy.SendErrorResponse(context.Response, "Error verifying referer. ", "403 - Forbidden: Access is denied.", HttpStatusCode.Forbidden);
                return;
            }

            if (!this.CheckReferer(allowedReferersArray, requestReferer))
            {
                Proxy.Log(TraceLevel.Warning, $"Proxy is being used from an unknown referer: {context.Request.Headers["referer"]}");
                Proxy.SendErrorResponse(context.Response, "Unsupported referer. ", "403 - Forbidden: Access is denied.", HttpStatusCode.Forbidden);
            }


        }

        //Check to see if allowed referer list is specified and reject if referer is null
        if (context.Request.Headers["referer"] == null && !((allowedReferersArray?[0]).Equals("*")))
        {
            Proxy.Log(TraceLevel.Warning, "Proxy is being called by a null referer.  Access denied.");
            Proxy.SendErrorResponse(response, "Current proxy configuration settings do not allow requests which do not include a referer header.", "403 - Forbidden: Access is denied.", HttpStatusCode.Forbidden);
            return;
        }

        //Throttling: checking the rate limit coming from particular client IP
        if (serverUrl.RateLimit > -1)
        {
            lock (Proxy.rateMapLock)
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

                if (!rate.Click())
                {
                    Proxy.Log(TraceLevel.Warning, $" Pair {key} is throttled to {serverUrl.RateLimit} requests per {serverUrl.RateLimitPeriod} minute(s). Come back later.");
                    Proxy.SendErrorResponse(context.Response, "This is a metered resource, number of requests have exceeded the rate limit interval.", "Unable to proxy request for requested resource", (HttpStatusCode)429);
                    return;
                }

                //making sure the rateMap gets periodically cleaned up so it does not grow uncontrollably
                int cnt = (int)context.Application["rateMap_cleanup_counter"];
                cnt++;

                if (cnt >= Proxy.CLEAN_RATEMAP_AFTER)
                {
                    cnt = 0;
                    this.CleanUpRatemap(ratemap);
                }

                context.Application["rateMap_cleanup_counter"] = cnt;
            }
        }

        //readying body (if any) of POST request
        byte[] postBody = this.ReadRequestPostBody(context);
        string post = Encoding.UTF8.GetString(postBody);

        NetworkCredential credentials = null;
        string requestUri = uri;
        bool hasClientToken = false;
        string token = string.Empty;
        string tokenParamName = null;

        if (!string.IsNullOrEmpty(serverUrl.HostRedirect))
        {
            requestUri = serverUrl.HostRedirect + new Uri(requestUri).PathAndQuery;
        }

        if (serverUrl.Domain != null)
        {
            credentials = new NetworkCredential(serverUrl.Username, serverUrl.Password, serverUrl.Domain);
        }
        else if (serverUrl?.HttpBasicAuth == true)
        {
            credentials = new NetworkCredential(serverUrl.Username, serverUrl.Password);
        }
        else
        {
            //if token comes with client request, it takes precedence over token or credentials stored in configuration
            hasClientToken = requestUri.Contains("?token=") || requestUri.Contains("&token=") || post.Contains("?token=") || post.Contains("&token=");

            if (!hasClientToken)
            {
                // Get new token and append to the request.
                // But first, look up in the application scope, maybe it's already there:
                token = Convert.ToString(context.Application["token_for_" + serverUrl.Url]);
                bool tokenIsInApplicationScope = !string.IsNullOrEmpty(token);

                //if still no token, let's see if there is an access token or if are credentials stored in configuration which we can use to obtain new token
                if (!tokenIsInApplicationScope)
                {
                    token = serverUrl.AccessToken;
                    if (string.IsNullOrEmpty(token))
                    {
                        token = this.GetNewTokenIfCredentialsAreSpecified(serverUrl, requestUri);
                    }
                }

                if (!string.IsNullOrEmpty(token) && !tokenIsInApplicationScope)
                {
                    //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                    context.Application.Lock();
                    context.Application["token_for_" + serverUrl.Url] = token;
                    context.Application.UnLock();
                }

                //name by which token parameter is passed (if url actually came from the list)
                tokenParamName = serverUrl?.TokenParamName;

                if (string.IsNullOrEmpty(tokenParamName))
                {
                    tokenParamName = "token";
                }
            }
        }

        //forwarding original request
        WebResponse serverResponse = null;
        try
        {
            serverResponse = this.ForwardToServer(context, this.AddTokenToUri(requestUri, token, tokenParamName), postBody, credentials);
        }
        catch (WebException webExc)
        {

            string errorMsg = $"{webExc.Message} {uri}";
            Proxy.Log(TraceLevel.Error, errorMsg);

            if (webExc.Response != null)
            {
                this.CopyHeaders(webExc.Response as HttpWebResponse, context.Response);

                using (Stream responseStream = webExc.Response.GetResponseStream())
                {
                    byte[] bytes = new byte[32768];
                    int bytesRead = 0;

                    while ((bytesRead = responseStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        responseStream.Write(bytes, 0, bytesRead);
                    }

                    context.Response.StatusCode = (int)(webExc.Response as HttpWebResponse).StatusCode;
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                Proxy.SendErrorResponse(context.Response, null, errorMsg, statusCode);
            }

            return;
        }

        if (string.IsNullOrEmpty(token) || hasClientToken)
        {
            // if token is not required or provided by the client, just fetch the response as is:
            this.FetchAndPassBackToClient(serverResponse, response, true);
        }
        else
        {
            // credentials for secured service have come from configuration file:
            // it means that the proxy is responsible for making sure they were properly applied:

            // first attempt to send the request:
            bool tokenRequired = this.FetchAndPassBackToClient(serverResponse, response, false);


            // checking if previously used token has expired and needs to be renewed
            if (tokenRequired)
            {
                Proxy.Log(TraceLevel.Info, "Renewing token and trying again.");

                // server returned error - potential cause: token has expired.
                // we'll do second attempt to call the server with renewed token:

                token = this.GetNewTokenIfCredentialsAreSpecified(serverUrl, requestUri);
                serverResponse = this.ForwardToServer(context, this.AddTokenToUri(requestUri, token, tokenParamName), postBody);

                //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                context.Application.Lock();
                context.Application["token_for_" + serverUrl.Url] = token;
                context.Application.UnLock();

                this.FetchAndPassBackToClient(serverResponse, response, true);
            }
        }

        // Use instead of response.End() to avoid the "Exception thrown: 'System.Threading.ThreadAbortException' in mscorlib.dll" error
        // that appears in the output of Visual Studio.  response.End() appears to only really be necessary if you need to end the thread immediately
        // (i.e. no more code is processed).  Since this call is at the end of the main subroutine we can safely call ApplicationInstance.CompleteRequest()
        // and avoid unnecessary exceptions.
        // Sources:
        // http://stackoverflow.com/questions/14590812/what-is-the-difference-between-use-cases-for-using-response-endfalse-vs-appl
        // http://weblogs.asp.net/hajan/why-not-to-use-httpresponse-close-and-httpresponse-end
        // http://stackoverflow.com/questions/1087777/is-response-end-considered-harmful

        context.ApplicationInstance.CompleteRequest();
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private byte[] ReadRequestPostBody(HttpContext context)
    {
        if (context.Request.InputStream.Length > 0)
        {
            byte[] bytes = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(bytes, 0, (int)context.Request.InputStream.Length);
            return bytes;
        }

        return new byte[0];
    }

    private WebResponse ForwardToServer(HttpContext context, string uri, byte[] postBody, NetworkCredential credentials = null)
    {
        return
            postBody.Length > 0 ?
            this.DoHTTPRequest(uri, postBody, WebRequestMethods.Http.Post, context.Request.Headers["referer"], context.Request.ContentType, credentials) :
            this.DoHTTPRequest(uri, context.Request.HttpMethod, credentials);
    }

    /// <summary>
    /// Attempts to copy all headers from the fromResponse to the the toResponse.
    /// </summary>
    /// <param name="fromResponse">The response that we are copying the headers from</param>
    /// <param name="toResponse">The response that we are copying the headers to</param>
    private void CopyHeaders(WebResponse fromResponse, HttpResponse toResponse)
    {
        foreach (var headerKey in fromResponse.Headers.AllKeys)
        {
            switch (headerKey.ToLower())
            {
                case "content-type":
                case "transfer-encoding":
                case "accept-ranges":   // Prevent requests for partial content
                case "access-control-allow-origin":
                case "access-control-allow-credentials":
                case "access-control-expose-headers":
                case "access-control-max-age":
                    continue;
                default:
                    toResponse.AddHeader(headerKey, fromResponse.Headers[headerKey]);
                    break;
            }
        }

        // Reset the content-type for OGC WMS - issue #367
        // Note: this might not be what everyone expects, but it helps some users
        // TODO: make this configurable
        if (fromResponse.ContentType.Contains("application/vnd.ogc.wms_xml"))
        {
            toResponse.ContentType = "text/xml";
            Proxy.Log(TraceLevel.Verbose, "Adjusting Content-Type for WMS OGC: " + fromResponse.ContentType);
        }
        else
        {
            toResponse.ContentType = fromResponse.ContentType;
        }
    }

    private bool FetchAndPassBackToClient(WebResponse serverResponse, HttpResponse clientResponse, bool ignoreAuthenticationErrors)
    {
        if (serverResponse != null)
        {
            using (Stream byteStream = serverResponse.GetResponseStream())
            {
                // Text response
                if (serverResponse.ContentType.Contains("text") ||
                    serverResponse.ContentType.Contains("json") ||
                    serverResponse.ContentType.Contains("xml"))
                {
                    using (StreamReader sr = new StreamReader(byteStream))
                    {
                        string strResponse = sr.ReadToEnd();
                        if (
                            !ignoreAuthenticationErrors
                            && strResponse.Contains("error")
                            && Regex.Match(strResponse, "\"code\"\\s*:\\s*49[89]").Success
                        )
                        {
                            return true;
                        }

                        //Copy the header info and the content to the reponse to client
                        this.CopyHeaders(serverResponse, clientResponse);
                        clientResponse.Write(strResponse);
                    }
                }
                else
                {
                    // Binary response (image, lyr file, other binary file)

                    //Copy the header info to the reponse to client
                    this.CopyHeaders(serverResponse, clientResponse);

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
        }

        return false;
    }

    private WebResponse DoHTTPRequest(string uri, string method, NetworkCredential credentials = null)
    {
        byte[] bytes = null;
        String contentType = null;
        Proxy.Log(TraceLevel.Info, $"Sending {method} request: {uri}");

        if (method.Equals(WebRequestMethods.Http.Post))
        {
            string[] uriArray = uri.Split(new char[] { '?' }, 2);
            uri = uriArray[0];
            if (uriArray.Length > 1)
            {
                contentType = "application/x-www-form-urlencoded";
                string queryString = uriArray[1];

                bytes = Encoding.UTF8.GetBytes(queryString);
            }
        }

        return this.DoHTTPRequest(uri, bytes, method, PROXY_REFERER, contentType, credentials);
    }

    private WebResponse DoHTTPRequest(string uri, byte[] bytes, string method, string referer, string contentType, NetworkCredential credentials = null)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
        req.ServicePoint.Expect100Continue = false;
        req.Referer = referer;
        req.Method = method;

        // Use the default system proxy
        req.Proxy = Proxy.SYSTEM_PROXY;

        if (credentials != null)
        {

            if (string.IsNullOrEmpty(credentials.Domain))
            {
                Proxy.Log(TraceLevel.Info, "Enabling PreAuthenticate for Basic Authentication");

                string auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{credentials.UserName}:{credentials.Password}"));
                req.PreAuthenticate = true;
                req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                req.Headers.Add("Authorization", auth);
            }
            else
            {
                req.Credentials = credentials;
            }
        }

        if (bytes?.Length > 0 || method == WebRequestMethods.Http.Post)
        {
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
            if (bytes?.Length > 0)
            {
                req.ContentLength = bytes.Length;
            }

            using (Stream outputStream = req.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }

        return req.GetResponse();
    }

    private string WebResponseToString(WebResponse serverResponse)
    {
        using (Stream byteStream = serverResponse.GetResponseStream())
        {
            using (StreamReader sr = new StreamReader(byteStream))
            {
                string strResponse = sr.ReadToEnd();
                return strResponse;
            }
        }
    }

    private string GetNewTokenIfCredentialsAreSpecified(ServerUrl su, string reqUrl)
    {
        string token = string.Empty;
        string infoUrl = string.Empty;

        bool isUserLogin = !string.IsNullOrEmpty(su.Username) && !string.IsNullOrEmpty(su.Password);
        bool isAppLogin = !string.IsNullOrEmpty(su.ClientId) && !string.IsNullOrEmpty(su.ClientSecret);

        if (isUserLogin || isAppLogin)
        {
            Proxy.Log(TraceLevel.Info, $"Matching credentials found in configuration file. OAuth 2.0 mode: {isAppLogin}");

            if (isAppLogin)
            {
                //OAuth 2.0 mode authentication
                //"App Login" - authenticating using client_id and client_secret stored in config
                su.OAuth2Endpoint = string.IsNullOrEmpty(su.OAuth2Endpoint) ? DEFAULT_OAUTH : su.OAuth2Endpoint;
                if (su.OAuth2Endpoint[su.OAuth2Endpoint.Length - 1] != '/')
                {
                    su.OAuth2Endpoint += "/";
                }

                Proxy.Log(TraceLevel.Info, $"Service is secured by {su.OAuth2Endpoint}: getting new token...");
                string uri = $"{su.OAuth2Endpoint}token?client_id={su.ClientId}&client_secret={su.ClientSecret}&grant_type=client_credentials&f=json";
                string tokenResponse = this.WebResponseToString(this.DoHTTPRequest(uri, WebRequestMethods.Http.Post));
                token = this.ExtractToken(tokenResponse, "token");
                if (!string.IsNullOrEmpty(token))
                {
                    token = this.ExchangePortalTokenForServerToken(token, su);
                }
            }
            else
            {
                //standalone ArcGIS Server/ArcGIS Online token-based authentication

                //if a request is already being made to generate a token, just let it go
                if (reqUrl.ToLower().Contains("/generatetoken"))
                {
                    string tokenResponse = this.WebResponseToString(this.DoHTTPRequest(reqUrl, WebRequestMethods.Http.Post));
                    token = this.ExtractToken(tokenResponse, "token");
                    return token;
                }

                //lets look for '/rest/' in the requested URL (could be 'rest/services', 'rest/community'...)
                if (reqUrl.ToLower().Contains("/rest/"))
                {
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/rest/", StringComparison.OrdinalIgnoreCase));
                }

                //if we don't find 'rest', lets look for the portal specific 'sharing' instead
                else if (reqUrl.ToLower().Contains("/sharing/"))
                {
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/sharing/", StringComparison.OrdinalIgnoreCase));
                    infoUrl = infoUrl + "/sharing";
                }
                else
                {
                    throw new ApplicationException("Unable to determine the correct URL to request a token to access private resources.");
                }

                if (infoUrl != "")
                {
                    Proxy.Log(TraceLevel.Info, " Querying security endpoint...");
                    infoUrl += "/rest/info?f=json";

                    //lets send a request to try and determine the URL of a token generator
                    string infoResponse = this.WebResponseToString(this.DoHTTPRequest(infoUrl, WebRequestMethods.Http.Get));
                    string tokenServiceUri = this.GetJsonValue(infoResponse, "tokenServicesUrl");

                    if (string.IsNullOrEmpty(tokenServiceUri))
                    {
                        string owningSystemUrl = this.GetJsonValue(infoResponse, "owningSystemUrl");
                        if (!string.IsNullOrEmpty(owningSystemUrl))
                        {
                            tokenServiceUri = owningSystemUrl + "/sharing/generateToken";
                        }
                    }

                    if (tokenServiceUri != string.Empty)
                    {
                        Proxy.Log(TraceLevel.Info, $" Service is secured by {tokenServiceUri}: getting new token...");
                        string uri = $"{tokenServiceUri}?f=json&request=getToken&referer={Proxy.PROXY_REFERER}&expiration=60&username={su.Username}&password={su.Password}";
                        string tokenResponse = this.WebResponseToString(this.DoHTTPRequest(uri, WebRequestMethods.Http.Post));
                        token = this.ExtractToken(tokenResponse, "token");
                    }
                }
            }
        }

        return token;
    }

    private bool CheckWildcardSubdomain(string allowedReferer, string requestedReferer)
    {
        string[] allowedRefererParts = Regex.Split(allowedReferer, "(\\.)");
        string[] refererParts = Regex.Split(requestedReferer, "(\\.)");

        if (allowedRefererParts.Length != refererParts.Length)
        {
            return false;
        }

        int index = allowedRefererParts.Length - 1;
        while (index >= 0)
        {
            if (allowedRefererParts[index].Equals(refererParts[index], StringComparison.OrdinalIgnoreCase))
            {
                index--;
            }
            else
            {
                if (allowedRefererParts[index].Equals("*"))
                {
                    index--;
                    continue; //next
                }

                return false;
            }
        }
        return true;
    }

    private bool PathMatched(string allowedRefererPath, string refererPath)
    {
        // If equal, return true
        if (refererPath.Equals(allowedRefererPath))
        {
            return true;
        }

        // If the allowedRefererPath contain a ending star and match the begining part of referer, it is proper start with.
        if (allowedRefererPath.EndsWith("*"))
        {
            string allowedRefererPathShort = allowedRefererPath.Substring(0, allowedRefererPath.Length - 1);
            if (refererPath.ToLower().StartsWith(allowedRefererPathShort.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    private bool DomainMatched(String allowedRefererDomain, String refererDomain)
    {
        if (allowedRefererDomain.Equals(refererDomain))
        {
            return true;
        }

        // try if the allowed referer contains wildcard for subdomain
        if (allowedRefererDomain.Contains("*"))
        {
            if (this.CheckWildcardSubdomain(allowedRefererDomain, refererDomain))
            {
                // return true if match wildcard subdomain
                return true;
            }
        }

        return false;
    }

    private bool ProtocolMatch(string allowedRefererProtocol, string refererProtocol)
    {
        return allowedRefererProtocol.Equals(refererProtocol);
    }

    private string GetDomainfromURL(string url, string protocol)
    {
        string domain = url.Substring(protocol.Length + 3);

        domain = domain.IndexOf('/') >= 0 ? domain.Substring(0, domain.IndexOf('/')) : domain;

        return domain;
    }

    private bool CheckReferer(string[] allowedReferers, string referer)
    {
        if (allowedReferers != null && allowedReferers.Length > 0)
        {
            if (allowedReferers.Length == 1 && allowedReferers[0].Equals("*")) return true; //speed-up

            foreach (string allowedReferer in allowedReferers)
            {

                //Parse the protocol, domain and path of the referer
                string refererProtocol = referer.StartsWith("https://") ? "https" : "http";
                string refererDomain = this.GetDomainfromURL(referer, refererProtocol);
                string refererPath = referer.Substring(refererProtocol.Length + 3 + refererDomain.Length);


                string allowedRefererCannonical = null;

                //since the allowedReferer can be a malformed URL, we first construct a valid one to be compared with referer
                //if allowedReferer starts with https:// or http://, then exact match is required
                if (Regex.IsMatch(allowedReferer,"^https?://.*"))
                {
                    allowedRefererCannonical = allowedReferer;
                }
                else
                {
                    string protocol = refererProtocol;

                    //if allowedReferer starts with "//" or no protocol, we use the one from refererURL to prefix to allowedReferer.
                    if (allowedReferer.StartsWith("//"))
                    {
                        allowedRefererCannonical = $"{protocol}:{allowedReferer}";
                    }
                    else
                    {
                        //if the allowedReferer looks like "example.esri.com"
                        allowedRefererCannonical = $"{protocol}://{allowedReferer}";
                    }
                }

                //parse the protocol, domain and the path of the allowedReferer
                string allowedRefererProtocol = allowedRefererCannonical.StartsWith("https://") ? "https" : "http";
                string allowedRefererDomain = this.GetDomainfromURL(allowedRefererCannonical, allowedRefererProtocol);
                string allowedRefererPath = allowedRefererCannonical.Substring(allowedRefererProtocol.Length + 3 + allowedRefererDomain.Length);

                //Check if both domain and path match
                if (this.ProtocolMatch(allowedRefererProtocol, refererProtocol) &&
                        this.DomainMatched(allowedRefererDomain, refererDomain) &&
                        this.PathMatched(allowedRefererPath, refererPath))
                {
                    return true;
                }
            }
            return false;//no-match
        }
        return true;//when allowedReferer is null, then allow everything
    }

    private string ExchangePortalTokenForServerToken(string portalToken, ServerUrl su)
    {
        //ideally, we should POST the token request
        Proxy.Log(TraceLevel.Info, $" Exchanging Portal token for Server-specific token for {su.Url}...");
        string uri = $"{su.OAuth2Endpoint.Substring(0, su.OAuth2Endpoint.IndexOf("/oauth2/", StringComparison.OrdinalIgnoreCase))}/generateToken?token={portalToken}&serverURL={su.Url}&f=json";
        string tokenResponse = this.WebResponseToString(this.DoHTTPRequest(uri, WebRequestMethods.Http.Get));
        return this.ExtractToken(tokenResponse, "token");
    }


    private static void SendPingResponse(HttpResponse response, string version, string config, string log)
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

    private static void SendErrorResponse(HttpResponse response, string errorDetails, string errorMessage, HttpStatusCode errorCode)
    {
        string message = $"{{\"error\": {{\"code\": {(int)errorCode},\"message\":\"{errorMessage}\"";
        if (!string.IsNullOrEmpty(errorDetails))
        {
            message += $",\"details\":[\"message\":\"{errorDetails}\"]";
        }

        message += "}}";
        response.StatusCode = (int)errorCode;

        //custom status description for when the rate limit has been exceeded
        if (response.StatusCode == 429)
        {
            response.StatusDescription = "Too Many Requests";
        }

        //this displays our customized error messages instead of IIS's custom errors
        response.TrySkipIisCustomErrors = true;
        response.Write(message);
        response.Flush();
    }

    private static string GetClientIp(HttpRequest request)
    {
        if (request == null)
        {
            return null;
        }

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

    private string AddTokenToUri(string uri, string token, string tokenParamName)
    {
        if (!string.IsNullOrEmpty(token))
        {
            uri += uri.Contains("?") ? $"&{tokenParamName}={token}" : $"?{tokenParamName}={token}";
        }

        return uri;
    }

    private string ExtractToken(string tokenResponse, string key)
    {
        string token = this.GetJsonValue(tokenResponse, key);
        if (string.IsNullOrEmpty(token))
        {
            Proxy.Log(TraceLevel.Error, $" Token cannot be obtained: {tokenResponse}");
        }
        else
        {
            Proxy.Log(TraceLevel.Info, $" Token obtained: {token}");
        }

        return token;
    }

    private string GetJsonValue(string text, string key)
    {
        int i = text.IndexOf(key);
        string value = string.Empty;
        if (i > -1)
        {
            value = text.Substring(text.IndexOf(':', i) + 1).Trim();

            value = value.Length > 0 && value[0] == '"' ?
                // Get the rest of a quoted string
                value.Substring(1, Math.Max(0, value.IndexOf('"', 1) - 1)) :
                // Get a string up to the closest comma, bracket, or brace
                value = value.Substring(0,
                    Math.Min(
                        value.Length,
                        Math.Min(
                            this.IndexOf_HighFlag(value, ","),
                            Math.Min(
                                this.IndexOf_HighFlag(value, "]"),
                                this.IndexOf_HighFlag(value, "}")
                            )
                        )
                    )
                );
        }

        return value;
    }

    private int IndexOf_HighFlag(string text, string key)
    {
        int i = text.IndexOf(key);
        if (i < 0) i = int.MaxValue;
        return i;
    }

    private void CleanUpRatemap(ConcurrentDictionary<string, RateMeter> ratemap)
    {
        foreach (string key in ratemap.Keys)
        {
            RateMeter rate = ratemap[key];
            if (rate.CanBeCleaned())
            {
                ratemap.TryRemove(key, out rate);
            }
        }
    }

    /// <summary>
    /// Get Config
    /// </summary>
    /// <returns>object ProxyConfig</returns>
    private static ProxyConfig GetConfig()
    {
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        if (config != null)
        {
            return config;
        }
        else
        {
            throw new ApplicationException("The proxy configuration file cannot be found, or is not readable.");
        }
    }

    /// <summary>
    /// writing Log file
    /// </summary>
    /// <param name="logLevel">log level</param>
    /// <param name="msg">message</param>
    private static void Log(TraceLevel logLevel, string msg)
    {
        string logMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {msg}";

        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        TraceSwitch ts = null;

        if (config.LogLevel != null)
        {
            ts = new TraceSwitch("TraceLevelSwitch2", "TraceSwitch in the proxy.config file", config.LogLevel);
        }
        else
        {
            ts = new TraceSwitch("TraceLevelSwitch2", "TraceSwitch in the proxy.config file", "Error");
            config.LogLevel = "Error";
        }

        Trace.WriteLineIf(logLevel <= ts.Level, logMessage);
    }
}

public class LogTraceListener : TraceListener
{
    private static object lockobject = new object();
    public override void Write(string message)
    {
        // Only log messages to disk if logFile has value in configuration, otherwise log nothing.
        ProxyConfig config = ProxyConfig.GetCurrentConfig();

        if (config.LogFile != null)
        {
            string log = config.LogFile;
            if (!log.Contains("\\") || log.Contains(".\\"))
            {
                // If this type of relative pathing .\log.txt
                if (log.Contains(".\\"))
                {
                    log = log.Replace(".\\", "");
                }

                // Cannot use System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath b/ config may be in a child directory
                string configDirectory = HttpContext.Current.Server.MapPath("proxy.config");
                string path = configDirectory.Replace("proxy.config", string.Empty);
                log = path + log;
            }

            lock (LogTraceListener.lockobject)
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
        // Only log messages to disk if logFile has value in configuration, otherwise log nothing.
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        if (config.LogFile != null)
        {
            string log = config.LogFile;
            if (!log.Contains("\\") || log.Contains(".\\"))
            {
                // If this type of relative pathing .\log.txt
                if (log.Contains(".\\"))
                {
                    log = log.Replace(".\\", string.Empty);
                }

                // Cannot use System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath b/ config may be in a child directory
                string configDirectory = HttpContext.Current.Server.MapPath("proxy.config");
                string path = configDirectory.Replace("proxy.config", string.Empty);
                log = path + log;
            }

            lock (LogTraceListener.lockobject)
            {
                using (StreamWriter sw = File.AppendText(log))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }

}

/// <summary>
/// class ProxyConfig
/// </summary>
[XmlRoot("ProxyConfig")]
public class ProxyConfig
{
    private static object lockobject = new object();

    public static ProxyConfig LoadProxyConfig(string fileName)
    {
        ProxyConfig config = null;
        lock (ProxyConfig.lockobject)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(ProxyConfig));
                using (StreamReader file = new StreamReader(fileName))
                {
                    try
                    {
                        config = (ProxyConfig)reader.Deserialize(file);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        return config;
    }

    public static ProxyConfig GetCurrentConfig()
    {
        ProxyConfig config = HttpRuntime.Cache["proxyConfig"] as ProxyConfig;
        if (config == null)
        {
            string fileName = HttpContext.Current.Server.MapPath("proxy.config");
            config = ProxyConfig.LoadProxyConfig(fileName);
            if (config != null)
            {
                CacheDependency dep = new CacheDependency(fileName);
                HttpRuntime.Cache.Insert("proxyConfig", config, dep);
            }
        }

        return config;
    }

    /// <summary>
    /// create an array with valid referers using the allowedReferers String that is defined in the proxy.config
    /// </summary>
    /// <returns>array with valid referers</returns>
    public static string[] GetAllowedReferersArray()
    {
        if (ProxyConfig.allowedReferers == null)
        {
            return null;
        }

        return ProxyConfig.allowedReferers.Split(',');
    }

    //referer
    private static string allowedReferers;

    [XmlArray("serverUrls")]
    [XmlArrayItem("serverUrl")]
    public ServerUrl[] ServerUrls
    {
        get;
        set;
    }

    [XmlAttribute("mustMatch")]
    public bool MustMatch
    {
        get;
        set;
    }

    //logFile
    [XmlAttribute("logFile")]
    public string LogFile
    {
        get;
        set;
    }

    //logLevel
    [XmlAttribute("logLevel")]
    public string LogLevel
    {
        get;
        set;
    }

    //referer
    [XmlAttribute("allowedReferers")]
    public string AllowedReferers
    {
        get
        {
            return allowedReferers;
        }

        set
        {
            allowedReferers = Regex.Replace(value, @"\s", string.Empty);
        }
    }

    public ServerUrl GetConfigServerUrl(string uri)
    {

        // split both request and proxy.config urls and compare them
        string[] uriParts = uri.Split(new char[] {'/','?'}, StringSplitOptions.RemoveEmptyEntries);

        string[] configUriParts = new string[] {};

        foreach (ServerUrl su in this.ServerUrls) {

            // if a relative path is specified in the proxy.config, append what's in the request itself
            if (!su.Url.StartsWith("http"))
            {
                su.Url = su.Url.Insert(0, uriParts[0]);
            }

            configUriParts = su.Url.Split(new char[] { '/','?' }, StringSplitOptions.RemoveEmptyEntries);

            // if the request has less parts than the config, don't allow
            if (configUriParts.Length > uriParts.Length)
            {
                continue;
            }

            int i = 0;
            for (i = 0; i < configUriParts.Length; i++)
            {
                if (!configUriParts[i].ToLower().Equals(uriParts[i].ToLower()))
                {
                    break;
                }
            }

            if (i == configUriParts.Length)
            {
                //if the urls don't match exactly, and the individual matchAll tag is 'false', don't allow
                if (configUriParts.Length == uriParts.Length || su.MatchAll)
                {
                    return su;
                }
            }

        }

        if (!this.MustMatch)
        {
            return new ServerUrl(uri);
        }
        else
        {
            throw new ArgumentException($"Proxy has not been set up for this URL. Make sure there is a serverUrl in the configuration file that matches: {uri}");
        }
    }
}

/// <summary>
/// class ServerUrl
/// </summary>
public class ServerUrl
{

    string rateLimit;
    string rateLimitPeriod;

    private ServerUrl()
    {
    }

    public ServerUrl(string url)
    {
        this.Url = url;
    }

    [XmlAttribute("url")]
    public string Url
    {
        get;
        set;
    }

    [XmlAttribute("hostRedirect")]
    public string HostRedirect
    {
        get;
        set;
    }

    [XmlAttribute("matchAll")]
    public bool MatchAll
    {
        get;
        set;
    }

    [XmlAttribute("oauth2Endpoint")]
    public string OAuth2Endpoint
    {
        get;
        set;
    }

    [XmlAttribute("domain")]
    public string Domain
    {
        get;
        set;
    }
    [XmlAttribute("username")]
    public string Username
    {
        get;
        set;
    }

    [XmlAttribute("password")]
    public string Password
    {
        get;
        set;
    }

    [XmlAttribute("clientId")]
    public string ClientId
    {
        get;
        set;
    }

    [XmlAttribute("clientSecret")]
    public string ClientSecret
    {
        get;
        set;
    }

    [XmlAttribute("accessToken")]
    public string AccessToken
    {
        get;
        set;
    }

    [XmlAttribute("tokenParamName")]
    public string TokenParamName
    {
        get;
        set;
    }

    [XmlAttribute("rateLimit")]
    public int RateLimit
    {
        get
        {
            return string.IsNullOrEmpty(this.rateLimit) ? -1 : int.Parse(this.rateLimit);
        }

        set
        {
            this.rateLimit = value.ToString();
        }
    }

    [XmlAttribute("rateLimitPeriod")]
    public int RateLimitPeriod
    {
        get
        {
            return string.IsNullOrEmpty(this.rateLimitPeriod) ? 60 : int.Parse(this.rateLimitPeriod);
        }

        set
        {
            this.rateLimitPeriod = value.ToString();
        }
    }

    [XmlAttribute("httpBasicAuth")]
    public bool HttpBasicAuth
    {
        get;
        set;
    }
}

