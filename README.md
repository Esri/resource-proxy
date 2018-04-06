Proxy files for DotNet, Java and PHP
====================================

These proxy files support:
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* [OAuth 2.0 app logins](https://developers.arcgis.com/authentication).
* Enabling logging
* Both resource and referer based rate limiting

## Alternatives
There are several good alternative solutions to use instead of your own resource-proxy:
* [Enable CORS on your server](http://enable-cors.org/server.html) - in order to access cross domain resources.
* Access secure services [using ArcGIS Online](https://doc.arcgis.com/en/arcgis-online/reference/arcgis-server-services.htm#ESRI_SECTION1_FEB0DF92DA064B6A970DFB59A18AA4C2) to store your username/password credentials. See also the [Working with Proxy Services](https://developers.arcgis.com/authentication/working-with-proxies/) topic.

## Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Follow the instructions in the readme file in the folder of the proxy you want to install (DotNet, Java, PHP) for installation instructions.

## Folders and Main Files

* [DotNet: .NET version of the proxy](DotNet/README.md)
    * proxy.ashx
    * proxy.config
    * README.md
* [Java: Java version of the proxy](Java/README.md)
    * proxy.jsp
    * WEB-INF/classes/proxy.config
    * README.md
* [PHP: PHP version of the proxy](PHP/README.md)
    * proxy.php
    * proxy.config
    * README.md

## Troubleshooting

* Watch the web requests being handled by the proxy to ensure that the proxy and the web resource locations are correct and properly configured in the application. Use something like [Fiddler](http://www.telerik.com/fiddler) or developer tools like [Network panel in Chrome Developer Tools](https://developer.chrome.com/devtools/docs/network#network-panel-overview).

## Product Resources

* [ArcGIS API for JavaScript](https://developers.arcgis.com/javascript/jshelp/ags_proxy.html)

* [Web AppBuilder for ArcGIS (Developer Edition)](https://developers.arcgis.com/web-appbuilder/guide/use-proxy.htm)

* [Esri Leaflet](https://developers.arcgis.com/authentication/working-with-proxies/#esri-leaflet)

* [Setting up a Proxy blog](http://blogs.esri.com/esri/supportcenter/2015/04/07/setting-up-a-proxy)

## Proxy Configuration Settings

All three proxies respect the XML configuration properties listed below.

* Use the ProxyConfig tag to specify the following proxy level settings.
    * **mustMatch="true"** : When `true` only the sites listed using serverUrl will be proxied. Set to `false` to proxy any site, which can be useful in testing. However, we recommend setting it to `true` for production sites.
    * **allowedReferers="http://server.com/app1,http://server.com/app2"** : A comma-separated list of referer URLs. Only requests coming from referers in the list will be proxied. See https://github.com/Esri/resource-proxy/issues/282 for detailed usage.
    * **logFile="proxylog.txt"** : When a logFile is specified, the proxy will log messages to this file. *N.B.: The folder containing the logFile must be writable by the web server.* If a path is not specified, the .Net proxy uses the folder where the proxy.config file is found. (The Java proxy uses java.util.logging.FileHandler to open the file; the PHP proxy uses fopen to open the file.)
    * **logLevel="Error"** : An optional flag indicating the level of detail to write to the logFile. Flags for each of the various languages are listed below.
        *  .Net levels are "Error", "Warning", "Info", and "Verbose" in order from fewest to most messages; the default is "Error".
        *  Java levels are "SEVERE", "WARNING", "INFO", "CONFIG", "FINE", "FINER", and "FINEST" in order from fewest to most messages; the default is "SEVERE".
        *  PHP levels are 0 (writes messages and errors to logs), 1 (shows proxy errors and messages in browser console), 2 (combination of levels 0 and 1), and 3 (no logging); the default is 0.
* Add a new `<serverUrl>` entry for each service that will use the proxy. The proxy.config allows you to use the serverUrl tag to specify one or more ArcGIS Server services that the proxy will forward requests to. The serverUrl tag has the following attributes:
    * **url**: Location of the ArcGIS Server service (or other URL) to proxy. Specify either the specific URL or the root (in which case you should set matchAll="false").
    * **matchAll="true"**: When `true` all requests that begin with the specified URL are forwarded. Otherwise, the URL requested must match exactly.
    * **username**: Username to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **password**: Password to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **tokenServiceUri**: If username and password are specified, the proxy will use the supplied token service uri to request a token.  If this value is left blank, the proxy will request a token URL from the ArcGIS server.
    * **useAppPoolIdentity**: When `true`, the IIS application pool identity will be used for authenticating with secured resources. This configuration will supersede the domain, username, and password configurations. The default is `false`. Only applies to DotNet proxy.
    * **domain**: The Windows domain to use with username/password when using Windows Authentication. Only applies to DotNet proxy.
    * **clientId**.  Used with clientSecret for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication. **NOTE**: If used to access hosted services, the service(s) must be owned by the user accessing it, (with the exception of credit-based esri services, e.g. routing, geoenrichment, etc.)
    * **clientSecret**: Used with clientId for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication.
    * **oauth2Endpoint**: When using OAuth 2.0 authentication specify the portal specific OAuth 2.0 authentication endpoint. The default value is https://www.arcgis.com/sharing/oauth2/.
    * **accessToken**: OAuth2 access token to use instead of on-demand access-token generation using clientId & clientSecret. Only applies to DotNet proxy.
    * **rateLimit**: The maximum number of requests with a particular referer over the specified **rateLimitPeriod**.
    * **rateLimitPeriod**: The time period (in minutes) within which the specified number of requests (rate_limit) sent with a particular referer will be tracked. The default value is 60 (one hour).
    * **hostRedirect**: The real URL to use instead of the "alias" one provided in the `url` property and that should be redirected. Example: `<serverUrl url="http://fakedomain" hostRedirect="http://172.16.85.2"/>`.

Note: Refresh the proxy application after updates to the proxy.config have been made.

Example of proxy using application credentials and limiting requests to 10/minute
```xml
<serverUrl url="http://route.arcgis.com"
    clientId="6Xo1d-example-9Kn2"
    clientSecret="5a5d50-example-c867b6efcf969bdcc6a2"
    rateLimit="600"
    rateLimitPeriod="60"
    matchAll="true">
</serverUrl>
```
Example of a tag for a resource which does not require authentication
```xml
<serverUrl url="http://sampleserver6.arcgisonline.com/arcgis/rest/services"
    matchAll="true">
</serverUrl>
```

## Requirements

* See the README.md file in the folder of the proxy you want to install for platform specific requirements.

## Issues

Found a bug or want to request a new feature? Check out previously logged [Issues](https://github.com/Esri/resource-proxy/issues) and/or our [FAQ](FAQ.md).  If you don't see what you're looking for, feel free to submit a [new issue](https://github.com/Esri/resource-proxy/issues/new).

## Contributing

Esri welcomes [contributions](CONTRIBUTING.md) from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

## Release steps (for maintainers)

0. run `npm install gh-release` inside the directory which houses the repository (this only needs to be done once)
1. bump proxy version numbers and changelog
2. commit your changes `git commit -m ':package: 1.x.x'`
3. run `gh-release` at the command line. this will create a tag using the current SHA and use whats in the changelog in the release notes

## License

Copyright 2017 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

> http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's [LICENSE](./LICENSE) file.

