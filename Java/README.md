Java Proxy File
===============

A Java proxy that handles support for
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* [OAuth 2.0 app logins](https://developers.arcgis.com/en/authentication).
* Enabling logging
* Both resource and client IP based rate limiting

##Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Install the contents of the Java folder as a Web Application in an web container such as Apache Tomcat.
* Test that the proxy is able to forward requests directly in the browser using:
```
http://[yourmachine]/Java/proxy.jsp?http://services.arcgisonline.com/ArcGIS/rest/services/?f=pjson
```
* Edit the proxy.config file in a text editor to set up your proxy configuration settings.
* Update your application to use the proxy for the specified services. In this JavaScript example requests to route.arcgis.com will utilize the proxy.

```
    urlUtils.addProxyRule({
        urlPrefix: "route.arcgis.com",
        proxyUrl: "http://[yourmachine]/proxy/proxy.jsp"
    });
```
* Security tip: By default, the proxy.config allows any referrer. To lock this down, replace the  ```*``` in the ```allowedReferers``` property with your own application URLs.

##Proxy Configuration Settings

* Use the ProxyConfig tag to specify the following proxy level settings.
    * **mustMatch="true"** : When true only the sites listed using serverUrl will be proxied. Set to false to proxy any site, which can be useful in testing. However, we recommend setting it to "true" for production sites.
    * **logFile="\<file with local path\>"** : When a path to a local file is specified event messages will be logged.
    * **logLevel="SEVERE"** : Sets the level of logging to be used.  Defaults to SEVERE. Possible values are SEVERE, WARNING, INFO, CONFIG, FINE, FINER and FINEST.
    * **allowedReferers ="http://server.com/application1,https://server.com/application2"**: A comma-separated list of referer URLs. Only requests coming from referers in the list will be proxied.
* Add new \<serverUrl\> entry for each service that will use the proxy. The proxy.config allows you to use the serverUrl tag to specify one or more ArcGIS Server services that the proxy will forward requests to. The serverUrl tags has the following attributes:
    * **url**: Location of the ArcGIS Server service (or other URL) to proxy. Specify either the specific URL or the root (in which case you shoould set matchAll="false"). If the location starts with "//", any protocol will be accepted, if the location starts with "http://", both http or https will be accepted, and if the location starts with "https://", only https will be accepted.
    * **matchAll="true"**: When true all requests that begin with the specified URL are forwarded. Otherwise, the URL requested must match exactly.
    * **username**: Username to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **password**: Password to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **tokenServiceUri**: If username and password are specified, the proxy will use the supplied token service uri to request a token.  If this value is left blank, the proxy will request a token URL from the ArcGIS server.
    * **clientId**.  Used with clientSecret for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication. **NOTE**: If used to access hosted services, the service(s) must be owned by the user accessing it, (with the exception of credit-based esri services, e.g. routing, geoenrichment, etc.)
    * **clientSecret**: Used with clientId for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication.
    * **oauth2Endpoint**: When using OAuth 2.0 authentication specify the portal specific OAuth 2.0 authentication endpoint. The default value is https://www.arcgis.com/sharing/oauth2/.
    * **rateLimit**: The maximum number of requests from a particular client ip address over the specified **rateLimitPeriod**.
    * **rateLimitPeriod**: The time period (in minutes) within which the specified number of requests (rate_limit) sent from a particular client ip address will be tracked. The default value is 60 (one hour).

See the proxy.config for examples. Note: Refresh the proxy application after updates to the proxy.config have been made.

##Folders and Files

The proxy consists of the following files:
* proxy.jsp: The actual proxy application. In most cases you will not need to modify this file.
* WEB-INF/classes/proxy.config: This file contains the configuration settings for the proxy. This is where you will define all the resources that will use the proxy. After updating this file you will need to restart or update the proxy application from your web container. **Important note:** In order to keep your credentials safe, ensure that your web server will not display the text inside your proxy.config in the browser (ie: http://[yourmachine]/proxy/proxy.config).

##Requirements

* Java 6 or greater

##Issues

Found a bug or want to request a new feature? Let us know by submitting an issue.

##Contributing

All contributions are welcome.

##Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for specific language governing permissions and limitations under the license.
