__Note: this README.md is for people wanting to submit pull requests of updates to the proxies.
It's not needed if you simply want to use the proxies.__


Testing Instructions
====================

Some test files to help with the testing of the new resource-proxy.

### Requirements

* node.js - see http://nodejs.org/
  * You can test if installed with `node --version`
  * Only tested with node `v7.0.0`
* grunt - see http://gruntjs.com/getting-started
  * You can test if installed with `grunt --version`
  * Only tested with `grunt-cli v0.1.13`
  * To install, once you've installed node.js:
    * `npm install -g grunt-cli`
    * On a Mac, you might get a message about "Please try running this command again as root/administrator". If so, try running it with:
      * `sudo npm install -g grunt-cli`

### Set up

1. Once node.js (and thus npm) is installed, have npm install the node modules specified in the package.json file:
  * `npm install` or `npm update`
2. Change the default proxyURL in the package.json to your proxy location:
  * `"defaultProxyURL": "http://localhost/DotNet/proxy.ashx"`
3. Replace your own proxy.config with the [test proxy.config](proxy.config). Don't forget to back up your own.
  * e.g. `copy proxy.config \inetpub\wwwroot\resource-proxy\DotNet\proxy.config`

### Run it

Run from command prompt:

    grunt

To make the output more verbose (print all test cases, even the ones that take less than 1% of total time)

    grunt --verbose

To make it run all tests, even if some fails:

    grunt --force

### Common errors when running `grunt`

* `Oh no, something's not right with test case # 0 ... Expected string 'World' NOT found`
  * Did you up update your proxy.config with the testing one?

### How does it work?

The grunt framework will run the default task in gruntfile.js.
By default it will use defaultProxyURL specified in the package.json file, or fallback to the one specified in gruntfile.js.

### Files and folders

  - configs
  - tasks
  gruntfile.js
  proxy.config
  README.md
  package.json

In addition, `node_modules` gets created once you run `npm install`.

### FAQ

* When running grunt, if you get "Fatal error: Unable to find local grunt.", please follow the instructions for installing grunt - http://gruntjs.com/getting-started
* To create new test cases, edit the configs/testcases.json file

