# Change Log
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased][unreleased]

### Added 
- Support 10.3 tokens
- Add "?ping" for easier testing
- (DotNet) Support Windows authentication
- Support federated service

### Fixed
- Better referrer handling
- Support ? in password
- Multiple redirects
- Handle token expiration
- Remove HTTP range-request Header

### Security
- Better HTTP header handling
- Security enhancements


## [1.0] - 2014-04-14

- Better proxies
- Case insensitive URL comparisons
- Support encoded URLs
- Improved handling of token URL logic
- Notepad friendly line endings
- Passing along http headers from proxied servers
- Support token authentication for ArcGIS Portal
- Support protocol relative urls in configuration file
- (DotNet) Use relative path for proxy.config
- (Java) Support allowedRerefer="*"
- (Java) support mustMatch="false"
- (php) Support redirects
- Improved error handling and error messages
- Improved documentation
- Added version number to each proxy code file


## [0.9] - 2014-02-14

 - Initial public release
