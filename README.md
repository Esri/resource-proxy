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

##Main Folders and Files

* DotNet: .NET version of the proxy
    * README.md
    * proxy.ashx
    * proxy.config
*  Java: Java version of the proxy
    * README.md
    * proxy.jsp
    * WEB-INF/classes/proxy.config
*  PHP: PHP version of the proxy
    * proxy.php
    * proxy.config
    * README.md

##Requirements

* See the README.md file in the folder of the proxy you want to install for platform specific requirements.

##Issues

Found a bug or want to request a new feature? Let us know by submitting an issue.

##Contributing

Esri welcomes [contributions](CONTRIBUTING.md) from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

##Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for specific language governing permissions and limitations under the license.

