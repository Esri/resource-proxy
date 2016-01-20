# Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [1.1.0] - 2015-11-05

### Added 
- Support for 10.3 tokens
- Support for federated services
- Support for hostRedirect to use aliased domains
- Added "?ping" for easier testing
- (DotNet) Support for Windows authentication

### Fixed
- Better referer handling
- Improved allowedReferers support
- Support for '?' in passwords
- Handling of multiple redirects
- Handling of token expiration
- Removed HTTP range-request Headers

### Security
- Better HTTP header handling
- Security enhancements

## [1.0] - 2014-04-14

- Better parity across proxies
- Case insensitive URL comparisons
- Support for encoded URLs
- Improved handling of token URL logic
- Notepad friendly line endings
- Passing along http headers from proxied servers
- Support token authentication for ArcGIS Portal
- Support protocol relative urls in configuration file
- (DotNet) Use relative path for proxy.config
- (Java) Support allowedReferer="*"
- (Java) support mustMatch="false"
- (php) Support redirects
- Improved error handling and error messages
- Improved documentation
- Added version number to each proxy code file

## 0.9 - 2014-02-14

- Initial public release

[Unreleased]: https://github.com/Esri/resource-proxy/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/Esri/resource-proxy/compare/v1.0...v1.1.0
[1.0]: https://github.com/Esri/resource-proxy/compare/v0.9...v1.0
