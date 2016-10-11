Java Proxy File
===============

A Java proxy that handles support for
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* [OAuth 2.0 app logins](https://developers.arcgis.com/en/authentication).
* Enabling logging
* Both resource and client IP based rate limiting

## Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Install the contents of the Java folder as a Web Application in a web container such as Apache Tomcat.
* Test that the proxy is installed and available:
```
http://[yourmachine]:8080/Java/proxy.jsp?ping
```
* Test that the proxy is able to forward requests directly in the browser using:
```
http://[yourmachine]:8080/Java/proxy.jsp?http://services.arcgisonline.com/ArcGIS/rest/services/?f=pjson
```
* Edit the proxy.config file in a text editor to set up your [proxy configuration settings](../README.md#proxy-configuration-settings).
* Update your application to use the proxy for the specified services. In this JavaScript example requests to route.arcgis.com will utilize the proxy.

```
    urlUtils.addProxyRule({
        urlPrefix: "route.arcgis.com",
        proxyUrl: "http://[yourmachine]:8080/Java/proxy.jsp"
    });
```
* Security tip: By default, the proxy.config allows any referrer. To lock this down, replace the  ```*``` in the ```allowedReferers``` property with your own application URLs.

## Folders and Files

The proxy consists of the following files:
* proxy.jsp: The actual proxy application. In most cases you will not need to modify this file.
* WEB-INF/classes/proxy.config: This file contains the [configuration settings for the proxy](../README.md#proxy-configuration-settings). This is where you will define all the resources that will use the proxy. After updating this file you will need to restart or update the proxy application from your web container. **Important note:** In order to keep your credentials safe, ensure that your web server will not display the text inside your proxy.config in the browser (ie: http://[yourmachine]:8080/Java/proxy.config).

## Requirements

* Java 7 or greater

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
