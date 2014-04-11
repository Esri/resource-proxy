PHP Proxy File
=================

A PHP proxy that handles support for
* Accessing cross domain resources
* Requests that exceed 2048 characters
* Accessing resources secured with token based authentication.
* [OAuth 2.0 app logins](https://developers.arcgis.com/en/authentication).
* Enabling logging
* Both resource and referer based rate limiting

##Instructions

* Download and unzip the .zip file or clone the repository. You can download [a released version](https://github.com/Esri/resource-proxy/releases) (recommended) or the [most recent daily build](https://github.com/Esri/resource-proxy/archive/master.zip).
* Install the contents of the PHP folder by adding all files into a web directory.
* Test that the proxy is able to forward requests directly in the browser using:
```
http://[yourmachine]/PHP/proxy.php?http://services.arcgisonline.com/ArcGIS/rest/services/?f=pjson
```
* Edit the proxy.config file in a text editor to set up your proxy configuration settings.
* Update your application to use the proxy for the specified services. In this JavaScript example requests to route.arcgis.com will utilize the proxy.

```
    urlUtils.addProxyRule({
        urlPrefix: "route.arcgis.com",
        proxyUrl: "http://[yourmachine]/PHP/proxy.php"
    });
```
* Security tip: By default, the proxy.config allows any referrer. To lock this down, replace the  ```*``` in the ```allowedReferers``` property with your own application URLs.
* Security tip: Verify that the ```proxy.config``` file is not accessible via the Internet and that the PHP server is configured correctly. To verify the proxy setup, open ```http://[yourmachine]/PHP/proxy-verification.php``` in a web browser and follow the  instructions.


##Proxy Configuration Settings

* Use the ProxyConfig tag to specify the following proxy level settings.
    * **mustMatch="true"** : When true only the sites listed using serverUrl will be proxied. Set to false to proxy any site, which can be useful in testing. However, we recommend setting it to "true" for production sites.
    * **logFile="<file with local path>"** : When a path to a local file is specified event messages will be logged.
    * **allowedReferers="http://server.com/application1,https://server.com/application2"**: A list of referer URLs. Only requests coming from referers in the list will be proxied.
* Add a new \<serverUrl\> entry for each service that will use the proxy page. The proxy.config allows you to use the serverUrl tag to specify one or more ArcGIS Server services that the proxy will forward requests to. The serverUrl tag has the following attributes:
    * **url**: Location of the ArcGIS Server service (or other URL) to proxy. Specify either the specific URL or the root (in which case you should set matchAll="true").
    * **matchAll="true"**: When true all requests that begin with the specified URL are forwarded. Otherwise, the URL requested must match exactly.
    * **username**: Username to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **password**: Password to use when requesting a token - if needed for ArcGIS Server token based authentication.
    * **clientId**:  Used with clientSecret for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication. **NOTE**: If used to access hosted services, the service(s) must be owned by the user accessing it, (with the exception of credit-based esri services, e.g. routing, geoenrichment, etc.)
    * **clientSecret**: Used with clientId for OAuth authentication to obtain a token - if needed for OAuth 2.0 authentication.
    * **oauth2Endpoint**: When using OAuth 2.0 authentication specify the portal specific OAuth 2.0 authentication endpoint. The default value is https://www.arcgis.com/sharing/oauth2/.
    * **rateLimit**: The maximum number of requests with a particular referer over the specified **rateLimitPeriod**.
    * **rateLimitPeriod**: The time period (in minutes) within which the specified number of requests (rate_limit) sent with a particular referer will be tracked. The default value is 60 (one hour).

##Folders and Files

The proxy consists of the following files:
* proxy.config: This file contains the configuration settings for the proxy. This is where you will define all the resources that will use the proxy.
* proxy.php: The actual proxy application. In most cases you will not need to modify this file.

Other useful files in the repo:
* .htaccess: This file is an example Apache web server file which includes recommended file filtering.
* proxy-verification.php: Useful testing page if you have installation problem.

Files created by the proxy:
* proxy.sqlite: This file is created dynamically after proxy.php runs.  This file supports rate metering.
* proxy_log.log: This file is created when the proxy.php runs (and logging is enabled). Note: If you do not have write permissions to this directory this file will not be created for you. To check for write permissions run the proxy-verification.php. 

##Requirements

* PHP 5.4.2 (recommended)
* cURL PHP extension
* OpenSSL PHP extension
* PDO_SQLITE PDO PHP extension

### Example Configurations

The PHP proxy supports both XML and JSON configurations.
XML is the default.
To change the default you must switch ````$proxyConfig->useXML();```` to ````$proxyConfig->useJSON();```` at the bottom of the proxy.php file.
No matter what style configuration chosen, always save the configuration as ```proxy.config```.
When using this proxy for testing or research and development you may want to add ```*``` to the ```allowedReferers``` property.
However, using ```*``` in production is not recommended.
In order to test the proxy like below make sure to add a ```*``` to the ```allowedReferers``` property.
Note, the example configuration file contains the ```*``` within ```allowedReferers```.

```
http://[yourmachine]/PHP/proxy.php?http://[machineyouknow]/arcgis/rest/services
```

XML example

```
<ProxyConfig
    mustMatch="true"
    logFile="proxy_log_xml.log"
    allowedReferers="http://server.com/application1,https://server.com/application2,*">

  <serverUrls>

      <serverUrl
          url="http://sampleserver6.arcgisonline.com"
          username="username"
          password="password"
          rateLimit="120"
          rateLimitPeriod="60"
          matchAll="true"/>

      <serverUrl
          url="geoenrich.arcgis.com"
          username="username"
          password="password"
          rateLimit="120"
          rateLimitPeriod="60"
          matchAll="true"/>

      <serverUrl
          url="https://route.arcgis.com"
          matchAll="true"
          oauth2Endpoint="https://www.arcgis.com/sharing/oauth2"
          clientId="6Xo1d-example-9Kn2"
          clientSecret="5a5d50-example-c867b6efcf969bdcc6a2"
          rateLimit="120"
          rateLimitPeriod="60">
      </serverUrl>

      <serverUrl
          url="http://services.arcgisonline.com/ArcGIS/rest/services/"
          rateLimit="120"
          rateLimitPeriod="60"
          matchAll="false"/>

  </serverUrls>

</ProxyConfig>
```

JSON example

```
{
    "proxyConfig": [
        {
            "mustMatch": true,
            "logFile": "proxy_log_json.log",
            "allowedReferers":["http://server.com/application1","https://server.com/application2","*"]
        }
    ],
    "serverUrls": [
        {
            "serverUrl" : [
                {
                    "url": "http://sampleserver6.arcgisonline.com",
                    "username": "username",
                    "password": "password",
                    "rateLimit": "120",
                    "rateLimitPeriod": "60",
                    "matchAll": true
                }
            ]
        },
        {
            "serverUrl" : [
                {
                    "url": "geoenrich.arcgis.com",
                    "username": "username",
                    "password": "password",
                    "rateLimit": "120",
                    "rateLimitPeriod": "60",
                    "matchAll": true
                }
            ]
        },
        {
            "serverUrl" : [
                {
                    "url": "http://route.arcgis.com",
                    "oauth2Endpoint": "https://www.arcgis.com/sharing/oauth2",
                    "clientId": "6Xo1d-example-9Kn2",
                    "clientSecret": "5a5d50-example-c867b6efcf969bdcc6a2",
                    "rateLimit": "120",
                    "rateLimitPeriod": "60",
                    "matchAll": true
                }
            ]
        },
        {
            "serverUrl" : [
                {
                    "url": "http://services.arcgisonline.com/ArcGIS/rest/services/",
                    "rateLimit": "120",
                    "rateLimitPeriod": "60",
                    "matchAll": false
                }
            ]
        }
    ]
}
```

## Guide for Unix and Mac using Apache's HTTP Server

### I see my configuration file on the Internet after running the verification test

This is a problem because ```proxy.config``` may contain sensitive credentials.  To resolve this, enable file filtering on your web server.  Apache users can do this by adding
 the following lines to the ```.htaccess``` file.  Note in order to use ```.htaccess``` files you must enable
```mod_rewrite.so``` in your ```httpd.conf``` and change the server directory so that ```AllowOverride``` is set to  ```All```.  The example below also
shows how to filter ```.sqlite``` files.  Sqlite is used to implement Rate Metering in this proxy. It's a good idea to filter ```.sqlite``` and ```.log``` files as well.

Example .htaccess file:

```
<Files ~ "\.sqlite$">
    Order allow,deny
    Deny from All
</Files>

<Files ~ "\.config$">
    Order allow,deny
    Deny from all
</Files>

<Files ~ "\.log$">
    Order allow,deny
    Deny from all
</Files>
```

Note if system permissions allow write access to Apache's main ```httpd.conf``` server config file, avoid doing file request filtering inside the ```.htaccess``` altogether.
File filtering directives added to Apache's main ```httpd.conf``` can lead to faster performance.  Find the ```Directory``` or existing ```Files``` tags in the main 
```httpd.conf``` and add these lines after to prevent users from downloading the .sqlite, .log and .config files from the server.

```
<Files ~ "\.(log|config|sqlite)$">
    Order allow,deny
    Deny from all
</Files>
```

### When I ran the verification test my browser displays raw PHP code

Raw PHP code in the browser indicates that the PHP server is not configured correctly.
If you see this, troubleshoot the Apache ```httpd.conf``` file.  On Mac, typically your built in Apache httpd.conf file is located here ```/private/etc/apache2/httpd.conf```.
On a Linux machine sometimes the ```httpd.conf``` file is located in ```etc/httpd/conf```.
A properly configured httpd.conf file, looks something like below. Note that the php_5module
is uncommented and libphp5.so is present in the modules directory.  More help [here](http://php.net/manual/en/install.macosx.bundled.php).


In addition Apache must be told to accept .php file extensions.  This can also be done by adding the below
lines in your ```httpd.conf```.


```
<IfModule php5_module>
    AddType application/x-httpd-php .php
    AddType application/x-httpd-php-source .phps

    <IfModule dir_module>
        DirectoryIndex index.html index.php
    </IfModule>
</IfModule>
```

On a Mac an alternative is to uncommented the line that says ```Include /private/etc/apache2/other/*.conf``` in the ```https.conf``` file.

###PHP version check has failed

This is because the minimum proxy requirements are set to PHP version 5.4.2.  If this fails, the recommendation is to install the most recent stable PHP release.
Other PHP versions may work, however PHP version 5.4.2 contains important security updates.

### My directory is not writable

To resolve this you'll need to change the directory to read/write where the ```proxy-verification.php``` file is located.  Changing permission on a folder typically requires administrative permissions to make the change.  The reason read/write is needed is because this proxy writes to a log and to a sqlite file.

### OpenSSL, PDO Sqlite and cURL failed

OpenSSL, PDO Sqlite and cURL are PHP extensions.  Depending on how you installed PHP there may be a  wizard to install PHP extensions or you may need to manually download / build and configure the required extensions.  This guide discusses both options to obtain OpenSSL, PDO Sqlite and Curl.

To build and configure the required extensions yourself, download the PHP source code from http://php.net and then compile it using the below commands.  Since the default version of PHP comes with Sqlite there is no need to issue a command to install this extension.  For cURL specific help go [here](http://www.php.net/manual/en/curl.installation.php).

```
./configure --with-apxs2=/usr/local/server/sept13/httpd-2.4.6/bin/apxs --with-openssl --enable-shmop --enable-mbstring --with-curl[/usr/curl]

make

sudo make install
```
For more detailed instructions use this [help on building PHP with extensions](http://www.php.net/manual/en/install.unix.apache2.php).

Alternatively (if you don't want to compile PHP yourself), there are plenty of good 3rd party Apache / PHP products that come with many PHP extensions and a standalone Apache web server.

You may want to try out these solutions:

* [Zend Server Project](http://www.zend.com/en/products/server/index)
* [MAMP](http://www.mamp.info/en/index.html)

Note, on Linux the ```configure```, ```make```, ```make install``` may likely be the shortest path to success to resolve missing extensions.


### I see errors regarding dates and times

Make sure you've set the ```date.timezone``` value in the ```php.ini``` file.  For valid PHP timezones, see this [documentation](http://php.net/manual/en/timezones.php). The ```proxy-verification.php``` page
makes an attempt to output the location of the ```php.ini``` used by the PHP server.


### Why do I see fail messages after adding extensions?

Anytime you make a configuration change to PHP ```php.ini``` or Apache ```httpd.conf``` you must do an Apache restart for those changes to take effect.  Restarting can be accomplished
by finding the Apache executable and then issuing the proper command in the terminal to ```start```, ```stop```, ```restart```.  See example, below.  Proper commands are ```start```, ```stop``` and
```restart```. In this example, the Apache executable is ```apachectl```.

```
sudo /usr/sbin/apachectl restart
```

### When requesting Tiled Map services via the proxy slower performance is observed then when not using the proxy

Proxies are helpful for overcoming certain browser limitations, requests that exceed 2048 characters, accessing resources secured with
token based authentication and ideal for implementing features like rate limiting.  However proxies do add a little overhead.  If overhead is a concern,
it's recommended to use proxy rules and avoid such things like ```esri.config.defaults.io.alwaysUseProxy```.  Alternatively, consider
implementing Cross-origin resource sharing ```CORS``` on the application server.

###Where do I get clientId and clientSecret credentials to leverage OAuth2?

There are several ways to obtain these credentials.  Credentials can be created by signing into [ArcGIS for Developers](https://developers.arcgis.com) and clicking ```Applications``` then ```Create an Application```.  Another option is to sign into [ArcGIS Online](https://arcgis.com) click ```My Content``` then click ```Add Item``` to go through the steps to add an application.  Once the application has been added click the item to ```View item details``` and click ```Register``` within the App Registration section.  Tip: OAuth2 workflows contain a variety of value added features for distributing apps, accessing billable services, and getting usage reports.



## Guide for Windows using IIS

### I see my configuration file on the Internet after running the verification test

This is a problem because ```proxy.config``` may contain sensitive credentials.   The solution to this problem is update IIS so that ```.config``` and ```.sqlite``` file types will be filtered and not be shown to the end user.
Please refer to the IIS help documentation explaining
[how to set up request filtering on IIS](http://www.iis.net/configreference/system.webserver/security/requestfiltering/fileextensions).


### When I ran the verification test my browser displays raw PHP code

When PHP is not installed or configured correctly this is the symptom.  On Windows there are several good 3rd party utilities that make installing and configuring PHP easy.
The utilities worth looking into are:

* [Microsoft's Web Platform Installer] (http://www.microsoft.com/web/downloads/platform.aspx)
* [Zend Server Project](http://www.zend.com/en/products/server/index)
* [WAMP](http://www.wampserver.com/)

Use the help documentation included in these products to get your PHP server up and running on Windows.

###PHP version check has failed

This is because the minimum proxy requirements are set to PHP version 5.4.2.  If this fails, the recommendation is to install the most recent stable PHP release.
Other PHP versions may work, however PHP version 5.4.2 contains important security updates.


### My directory is not writable

To resolve this you'll need to change the directory to read/write where the ```proxy-verification.php``` file is located.  In most cases, administrative permissions are needed to change file permission levels.  The reason read/write is needed
is because this proxy writes to a log and to a sqlite file.  This is usually accomplished by right clicking on the folder
to open the ```properties``` dialog box and clicking the ```security``` tab.  Use the ```name list box```, select the user, contact, computer, or group whose permissions you want to make writable and assign ```write``` permissions.

### OpenSSL, PDO Sqlite and cURL failed

If you've chosen to install PHP using either of these utilities, OpenSSL and PDO Sqlite are a part of the PHP install.  However, these extensions need to be activated.  You can do this by uncommenting these lines in the ```php.ini``` file.

``` extension=php_openssl.dll ```
``` extension=php_pdo.dll ```
``` extension=php_curl.dll ```

After modifying ```php.ini``` restart IIS and run the ```proxy-verification.php``` again.

### When requesting Tiled Map services via the proxy slower performance is observed then when not using the proxy

Proxies are helpful for overcoming certain browser limitations, requests that exceed 2048 characters, accessing resources secured with
token based authentication and ideal for implementing features like rate limiting.  However proxies do add a little overhead.  If overhead is a concern,
it's recommended to use proxy rules and avoid such things like ```esri.config.defaults.io.alwaysUseProxy```.  Alternatively, consider
implementing Cross-origin resource sharing ```CORS``` on the application server.

###Where do I get clientId and clientSecret credentials to leverage OAuth2?

There are several ways to obtain these credentials.  Credentials can be created by signing into [ArcGIS for Developers](https://developers.arcgis.com) and clicking ```Applications``` then ```Create an Application```.  Another option is to sign into [ArcGIS Online](https://arcgis.com) click ```My Content``` then click ```Add Item``` to go through the steps to add an application.  Once the application has been added click the item to ```View item details``` and click ```Register``` within the App Registration section.  Tip: OAuth2 workflows contain a variety of value added features for distributing apps, accessing billable services, and getting usage reports.


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
