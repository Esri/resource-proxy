require.config({
    //baseUrl: 'app/js',
    deps: ['onlineRateCalculator'],
    paths: {
        'angular': '../../bower_components/angular/angular.min',
        'angular-route': '../../bower_components/angular-route/angular-route.min',
        'angular-resource': '../../bower_components/angular-resource/angular-resource.min',
        'angular-translate': '../../bower_components//angular-translate/angular-translate.min'
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'angular-route': {
            deps: ['angular'],
            'exports': 'angular-route'
        },
        'angular-resource': {
            deps: ['angular'],
            'exports': 'angular-resource'
        }
    },
    callback: function() {
        "use strict";
        
        // manual angularJs bootstrapping
        require([
            'angular',
            'routing'
        ], function(angular) {
            var baseElement
                    = document.getElementsByClassName('onlineRateCalculator');
            angular.bootstrap(baseElement, ['onlineRateCalculator']);
        });
    }
});