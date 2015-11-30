define([
    'onlineRateCalculator',
    'controller/startCalculation.controller',
    'controller/duringCalculation.controller',
    'controller/calculationError.controller',
    'productCalculation/showMenuQuery.controller'
], function(app) {
    "use strict";
    
    app.config(RoutingTable);
    
    RoutingTable.$inject = ['$stateProvider', '$urlRouterProvider'];
            
    function RoutingTable($stateProvider, $urlRouterProvider) {
        
        var baseUrl = '/wordpress/wp-content/plugins/FpOnlineRateTable/app/';
        $stateProvider
            .state('error', {
                url: '/error',
                templateUrl: baseUrl + 'partials/calculationError.html',
                controller: 'CalculationErrorController',
                controllerAs: 'vm',
                params: {'error': {}}
            })
            .state('start', {
                url: '/start',
                templateUrl: baseUrl + 'partials/startCalculation.html',
                controller: 'StartCalculationController',
                controllerAs: 'vm'
            })
            .state('calculate', {
                url: '/calculate',
                templateUrl: baseUrl + 'partials/duringCalculation.html',
                controller: 'DuringCalculationController',
                controllerAs: 'vm'
            })
            .state('calculate.showMenu', {
                url: '/',
                templateUrl: baseUrl + 'js/productCalculation/showMenuQuery.html',
                controller: 'ShowMenuQueryController',
                controllerAs: 'vm',
                parent: 'calculate',
                params: { 'queryDescription': {} }
            });
            
        $urlRouterProvider.otherwise('/start');
    }
    
    return app;
});