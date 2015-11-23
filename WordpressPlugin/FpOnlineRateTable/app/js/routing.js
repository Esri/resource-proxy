define([
    'onlineRateCalculator',
    'controller/startCalculationCtrl',
    'controller/duringCalculationCtrl'
], function(app) {
    "use strict";
    
    return app.config(function($stateProvider, $urlRouterProvider) {
        
        var baseUrl = '/wordpress/wp-content/plugins/FpOnlineRateTable/app/';
        $stateProvider
            .state('start', {
                url: '/start',
                templateUrl: baseUrl + 'partials/startCalculation.html',
                controller: 'StartCalculationCtrl'
            })
            .state('calculate', {
                url: '/calculate',
                templateUrl: baseUrl + 'partials/duringCalculation.html',
                controller: 'DuringCalculationCtrl',
                params: {zip: null, serviceState: null}
            });
            
        $urlRouterProvider.otherwise('/start');
    });
});