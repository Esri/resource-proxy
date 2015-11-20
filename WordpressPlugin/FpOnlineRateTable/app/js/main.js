require.config({
    deps: ['routing'],
    paths: {
        'angular': '../../bower_components/angular/angular',
        'angular-resource': '../../bower_components/angular-resource/angular-resource.min',
        'angular-growl': '../../bower_components/angular-growl-v2/build/angular-growl',
        'angular-ui-router': '../../bower_components/angular-ui-router/release/angular-ui-router.min'
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'angular-resource': {
            deps: ['angular'],
            'exports': 'angular-resource'
        },
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
            'angular'
        ], function(angular) {
            var baseElement
                    = document.getElementsByClassName('onlineRateCalculator');
            angular.bootstrap(baseElement, ['OnlineRateCalculator']);
        });
    }
});