Proxy files for DotNet, Java and PHP
====================================

These proxy files support:
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* [OAuth 2.0 app logins](https://developers.arcgis.com/authentication).
* Enabling logging
* Both resource and referer based rate limiting

##Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Follow the instructions in the readme file in the folder of the proxy you want to install (DotNet, Java, PHP) for installation instructions.

##Folders and Main Files

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

##Troubleshooting

* Watch the web requests being handled by the proxy to ensure that the proxy and the web resource locations are correct and properly configured in the application. Use something like [Fiddler](http://www.telerik.com/fiddler) or developer tools like [Network panel in Chrome Developer Tools](https://developer.chrome.com/devtools/docs/network#network-panel-overview)
    
##Product Resources

* [ArcGIS API for JavaScript – Using the proxy](https://developers.arcgis.com/javascript/jshelp/ags_proxy.html)

* [Web AppBuilder for ArcGIS (Developer Edition) – Use proxy](https://developers.arcgis.com/web-appbuilder/guide/use-proxy.htm)

* [Esri Leaflet – Working with Proxies](https://developers.arcgis.com/authentication/working-with-proxies/#esri-leaflet)

* [ArcGIS API for Flex – Using the proxy page](https://developers.arcgis.com/flex/guide/using-the-proxy-page.htm)

* [ArcGIS API for Silverlight – Secure services](https://developers.arcgis.com/silverlight/guide/secure-services.htm)

* [Setting up a Proxy blog](http://blogs.esri.com/esri/supportcenter/2015/04/07/setting-up-a-proxy)
    
##Requirements

* See the README.md file in the folder of the proxy you want to install for platform specific requirements.

##Issues

Found a bug or want to request a new feature? Check out previously logged [Issues](https://github.com/Esri/resource-proxy/issues) and/or our [FAQ](FAQ.md).  If you don't see what you're looking for, feel free to submit a [new issue](https://github.com/Esri/resource-proxy/issues/new).

##Contributing

Esri welcomes [contributions](CONTRIBUTING.md) from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

##Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for specific language governing permissions and limitations under the license.

