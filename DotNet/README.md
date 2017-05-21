DotNet Proxy File
=================

A .NET proxy that handles support for
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* Accessing resources secured with Microsoft Integrated Windows Authentication (IWA) by using the configured application pool identity for the hosted resource-proxy.
* [OAuth 2.0 app logins](https://developers.arcgis.com/en/authentication).
* Enabling logging
* Both resource and referer based rate limiting

## Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Install the contents of the DotNet folder as a .NET Web Application, specifying a .NET 4.0 application pool or later. For example using the following steps:
    * Open IIS Manager
    * If you put the DotNet folder within wwwroot, right-click it and select "Convert to Application".
    * Make sure the "Application pool" is at least 4.0.
* Test that the proxy is installed and available:
```
http://[yourmachine]/DotNet/proxy.ashx?ping
```
* Test that the proxy is able to forward requests directly in the browser using:
```
http://[yourmachine]/DotNet/proxy.ashx?http://services.arcgisonline.com/ArcGIS/rest/services/?f=pjson
```
* Troubleshooting: If you get an error message 404.3, it's possible that ASP.NET have not been set up. On Windows 8, go to "Turn Windows features on or off" -> "Internet Information Services" -> "World Wide Web Services" -> "Application Development Features" -> "ASP.NET 4.5".
* Edit the proxy.config file in a text editor to set up your [proxy configuration settings](../README.md#proxy-configuration-settings).
* Update your application to use the proxy for the specified services. In this JavaScript example requests to route.arcgis.com will utilize the proxy.

```
    urlUtils.addProxyRule({
        urlPrefix: "route.arcgis.com",
        proxyUrl: "http://[yourmachine]/proxy/proxy.ashx"
    });
```
* Security tip: By default, the proxy.config allows any referrer. To lock this down, replace the  ```*``` in the ```allowedReferers``` property with your own application URLs.

## Folders and Files

The proxy consists of the following files:
* proxy.config: This file contains the [configuration settings for the proxy](../README.md#proxy-configuration-settings). This is where you will define all the resources that will use the proxy. After updating this file you might need to refresh the proxy application using IIS tools in order for the changes to take effect.  **Important note:** In order to keep your credentials safe, ensure that your web server will not display the text inside your proxy.config in the browser (ie: http://[yourmachine]/proxy/proxy.config).
* proxy.ashx: The actual proxy application. In most cases you will not need to modify this file.
* proxy.xsd: a schema file for easier editing of proxy.config in Visual Studio.
* Web.config: An XML file that stores ASP.NET configuration data.
NOTE: as of v1.1.0, log levels and log file locations are specified in proxy config. By default the proxy will write log messages to a file named auth_proxy.log located in  'C:\Temp\Shared\proxy_logs'. Note that the folder location needs to exist in order for the log file to be successfully created.

## Requirements

* ASP.NET 4.0 or greater (4.5 is required on Windows 8/Server 2012, see [this article](http://www.iis.net/learn/get-started/whats-new-in-iis-8/iis-80-using-aspnet-35-and-aspnet-45) for more information)

## Issues

Found a bug or want to request a new feature? Let us know by submitting an issue.

## Contributing

All contributions are welcome.

## Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for specific language governing permissions and limitations under the license.
