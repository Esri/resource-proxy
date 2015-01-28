FAQ
===

##### I'm using appId - why can the proxy not access my private content?

This is by design in the security model for the ArcGIS platform. 
[Applications using appId/appSecret by design](https://developers.arcgis.com/authentication/app-logins.html) don't have permission to search for private content, even if owned by the same person.

##### When choosing an application pool in IIS, I only see 2.0 as an option

Check out [this](http://stackoverflow.com/questions/4890245/how-to-add-asp-net-4-0-as-application-pool-on-iis-7-windows-7) StackOverflow thread for instructions on installing ASP.NET 4.0.

##### Where can I get help?

Contact [Esri Support](http://support.esri.com/) or post on the [ArcGIS forums](http://forums.arcgis.com/forums/15-ArcGIS-API-for-JavaScript).
If you think you've found a bug, report it as an [issue ](https://github.com/Esri/resource-proxy/issues) and include specific steps to reproduce, the observed and expected behavior.

##### Where and how can I suggest improvements?

Esri welcomes [contributions](CONTRIBUTING.md) from anyone and everyone. Please see our general [guidelines for contributing](https://github.com/esri/contributing).
