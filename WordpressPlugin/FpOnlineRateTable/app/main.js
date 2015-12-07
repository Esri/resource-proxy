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
        
        // manual angularJs bootstrapping
        require([
            'angular',
            'app'
        ], function(angular) {
            var baseElement
                    = document.getElementsByClassName('onlineRateCalculator');
            angular.bootstrap(baseElement, ['FP.OnlineRateCalculator']);
        });
    }
});