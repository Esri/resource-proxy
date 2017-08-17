/*jslint node: true */

// USAGE: see README.md

'use strict';

module.exports = function(grunt) {

    grunt.loadTasks('tasks');

    require('time-grunt')(grunt);

    grunt.initConfig({
        // includes default proxy URL
        pkg: grunt.file.readJSON("package.json"),
        // config with testcases
        testconfig: grunt.file.readJSON("configs/testcases.json")
    });

    grunt.registerTask('default', "Run all the test cases", function() {
        grunt.log.writeln('-------------------------------------------------');
        grunt.task.run('request-tester');
        if (grunt.config("pkg.defaultProxyURL")) {
            global.proxyURL = grunt.config("pkg.defaultProxyURL");
        } else {
            global.proxyURL = 'http://localhost/DotNet/proxy.ashx';
        }
        grunt.log.writeln("proxyURL: " + global.proxyURL);
        grunt.verbose.writeln("You can change the proxy in the package.json file.");
        grunt.log.writeln('-------------------------------------------------');
    });
};
