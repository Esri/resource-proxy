/*jslint node: true */

'use strict';

// This task will run each test specified in globel.testcasesconfigs/testcases.json

module.exports = function(grunt) {
    var request = require('request');
    var testcases;
    var PassRequests = 0;
    var FailRequests = 0;

    grunt.registerTask('request-tester', 'Test http requests', function(){
        // This task will run each test specified in configs/testcases.json
        //
        // each test case in that file require a minimum of:
        //      "name"              - a short name
        //      "url"              - the URL to test
        //      "expectedString"  - a string to expect in the response from "url" via proxy
        //
        // optional fields
        //      "desc"            - descriptive oneliner for the test case
        //      "httpheaders"    - additional HTTP headers to help set up the test case, e.g. "Referer"
        //      "postbody"        - if testcase requires the body to be sent in a POST body
        //      "usetestcase"    - whether to use or skip this testcase, default is "true"
        //      "usetestcase" - whether to use or skip this testcase, default is "true"
        //      "expectedHeaderContentType" - Content-Type header value expected to be returned by proxy
        //      "expectedHeaderContentDisposition" - Content-Disposition header value expected to be returned by proxy

        testcases = grunt.config("testconfig.tests");

        if (!global.proxyURL) {
            grunt.fatal("You need to specify a proxyURL.");
        }

        // check that proxyURL is accessible
        request(global.proxyURL, function (error, response, body) {
            if (error) {
                grunt.fatal("Cannot access the proxyURL. Try it in a browser:\n" + global.proxyURL);
            }
        }) ;

        grunt.verbose.ok("------- Running " + testcases.length + " test cases");

        // run all the test cases
        for (var i = 0; i < testcases.length; i++){
            var comment = testcases[i].comment? " >> " + testcases[i].comment : "";

            // verify minimum requirements: name, url, expectedString
            if (!testcases[i].name) {
                grunt.log.error(">>> Skipping test case " + i + " due to missing 'name'.");
            } else if (!testcases[i].url && testcases[i].url !== "") {
                grunt.log.error(">>> Skipping [" + i +  "] " + testcases[i].name + " due to missing 'url'.");
            } else if (!testcases[i].expectedString && testcases[i].expectedString !== "") {
                grunt.log.error(">>> Skipping [" + i +  "] " + testcases[i].name + " due to missing 'expectedString'.");
            } else if (testcases[i].usetestcase === "false") {
                // check if should be used or not
                grunt.log.ok(">>> Skipping [" + i +  "] " + testcases[i].name + " due to 'usetestcase' being false.");
            } else {
                // go ahead and run this test case
                grunt.task.run('check:' + i + ": " + testcases[i].name);
            }
        }
    }); // end of "request-tester" task


    grunt.registerTask('check', 'Compare proxied URL response to expected result', function(i,name){
        var httpErrorHandler = function(e){
            require('request').debug = true;  // useful when debugging grunt request
            grunt.log.error("ERROR 102 URL: " + URL + "\n" + e);
            grunt.warn();
        };

        var dataHandler = function(data, statusCode, statusMessage, headers){

            // decompressed data as it is received
            var theResponse = "" + data;

            if (theResponse.search(testcases[i].expectedHeaderContentType) ){
                if (headers['content-type'].search(testcases[i].expectedHeaderContentType) != -1) {
                    if (theResponse.search(testcases[i].expectedString) != -1){
                        if (testcases[i].expectedHeaderContentDisposition) {
                            // test more
                            if (headers['content-disposition'].search(testcases[i].expectedHeaderContentDisposition) != -1) {
                                PassRequests++;
                            } else {
                                grunt.log.error("Oh no (A), something's not right with test case # " + i + ".\n" +
                                    testcases[i].name + " --- " + testcases[i].desc + "\n" +
                                    "URL to proxy: " + testcases[i].url + "\n" +
                                    "URL: " + URL + "\n" +
                                    "request statusCode: " + statusCode + "\n" +
                                    "request statusMessage: " + statusMessage + "\n" +
                                    "Expected HeaderContentDisposition: " + testcases[i].expectedHeaderContentDisposition + "\n" +
                                    "But found content-disposition:     " + headers['content-disposition']
                                );
                                // grunt.log.error("Response data:\n" + (data + "").substr(0,1000));
                                FailRequests++;
                                grunt.warn("Test case " + i + " failed. ");
                            }
                        } else {
                            PassRequests++;
                        }
                    } else {
                        grunt.log.error("Oh no (A), something's not right with test case # " + i + ".\n" +
                            testcases[i].name + " --- " + testcases[i].desc + "\n" +
                            "URL to proxy: " + testcases[i].url + "\n" +
                            "URL: " + URL + "\n" +
                            "request statusCode: " + statusCode + "\n" +
                            "request statusMessage: " + statusMessage + "\n" +
                            "Expected string '" + testcases[i].expectedString + "' NOT found");
                        grunt.log.error("Response data:\n" + (data + "").substr(0,1000));
                        FailRequests++;
                        grunt.warn("Test case " + i + " failed. ");
                    }
                } else { // expectedHeaderContentType test failed
                    grunt.log.error("Oh no (B), something's not right with test case # " + i + ".\n" +
                        testcases[i].name + " --- " + testcases[i].desc + "\n" +
                        "URL to proxy: " + testcases[i].url + "\n" +
                        "URL: " + URL + "\n" +
                        "request statusCode: " + statusCode + "\n" +
                        "request statusMessage: " + statusMessage + "\n" +
                        "expectedHeaderContentType '" + testcases[i].expectedHeaderContentType + "' NOT found");
                    grunt.log.error("Response Content-Type: " + headers['content-type']);
                    FailRequests++;
                    grunt.warn("Test case " + i + " failed. ");
                }
            } else if (theResponse.search(testcases[i].expectedString) != -1){
                PassRequests++;
            }
            else {
                grunt.log.error("Oh no (C), something's not right with test case # " + i + ".\n" +
                    testcases[i].name + " --- " + testcases[i].desc + "\n" +
                    "URL to proxy: " + testcases[i].url + "\n" +
                    "URL: " + URL + "\n" +
                    "request statusCode: " + statusCode + "\n" +
                    "request statusMessage: " + statusMessage + "\n" +
                    "Expected string '" + testcases[i].expectedString + "' NOT found");
                grunt.log.error("Response data:\n" + (data + "").substr(0,1000));
                // useful when debugging
                //grunt.file.write("failedresponse.temp", data);
                FailRequests++;
                grunt.warn("Test case " + i + " failed. ");
            }
            if (FailRequests > 0) {
                grunt.log.error(">>> Status: ", PassRequests, " OK, but ", FailRequests, " returned unexpected responses.");
            } else {
                grunt.verbose.ok(">>> Status: all ", PassRequests, " OK.");
            }
            done();
        };

        var URL = global.proxyURL + '?' + testcases[i].url;

        // Tell Grunt that this should be run asynchronous
        var done = this.async();

        var option;
        if(testcases[i].postbody){
            var formData = {};
            var querystring = testcases[i].postbody.split("&");
            for(var j=0; j<querystring.length; j++){
                var splits = querystring[j].split("=");
                formData[splits[0]]=splits[1];
            }

            option = {
                url: URL,
                headers: testcases[i].httpheaders,
                form: formData
            };

            request.post(
                option,
                function (error, response, body) {
                    if(!error){
                        dataHandler(response.body,response.statusCode,response.statusMessage, response.headers);
                    } else {
                        httpErrorHandler(error);
                    }
                });
        } else {
            option = {
                url: URL,
                headers: testcases[i].httpheaders
            };

            request(
                //{ url: URL, gzip: true },
                option,
                function (error, response, body) {
                    if(!error){
                        dataHandler(response.body, response.statusCode, response.statusMessage, response.headers);
                    } else {
                        httpErrorHandler(error);
                    }
                });
        }
    }); // end of "check" task
};
