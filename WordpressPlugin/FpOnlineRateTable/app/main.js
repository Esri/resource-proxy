require.config({
    //deps: ['routing'],
    paths: {
        'angular': '../bower_components/angular/angular',
        'angular-growl': '../bower_components/angular-growl-v2/build/angular-growl',
        'angular-ui-router': '../bower_components/angular-ui-router/release/angular-ui-router'
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'angular-growl': {
            deps: ['angular'],
            'exports': 'angular-growl'
        },
        'angular-ui-router': {
            deps: ['angular'],
            'exports': 'angular-ui-router'
        }
    },
    callback: function() {
        "use strict";
        
        // access the data-main attribute of the script tag that loads
        // requirejs. This path will be used as base path when loading template
        // file from angularjs.
        // WARNING: this procedure can be considered as hack.
        var id = "requirejs-script-tag";
        var main = document.getElementById(id).getAttribute('data-main');
        var baseUrl = main.substring(0, main.lastIndexOf('/'));
        
        // manual angularJs bootstrapping
        require([
            'angular',
            'app'
        ], function(angular, app) {
            
            // define a constant on the FP.ProductCalculation module to pass the
            // baseUrl into it.
            // WARNING: this can be considered as hack
            angular.module('FP.ProductCalculation')
                .constant('BaseUrl', baseUrl);
            
            var baseElement
                    = document.getElementsByClassName('onlineRateCalculator');
            angular.bootstrap(baseElement, ['FP.OnlineRateCalculator']);
        });
    }
});