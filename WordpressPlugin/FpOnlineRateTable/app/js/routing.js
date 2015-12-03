define([
    'onlineRateCalculator',
    'controller/startCalculation.controller',
    'controller/duringCalculation.controller',
    'controller/calculationError.controller',
    'productCalculation/showMenuQuery.controller',
    'productCalculation/showDisplayQuery.controller',
    'productCalculation/requestValueQuery.controller',
    'productCalculation/finish.controller'
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
                controllerAs: 'calc',
                params: { 'queryDescription': null }
            })
            .state('calculate.showMenu', {
                url: '/',
                templateUrl: baseUrl + 'js/productCalculation/showMenuQuery.html',
                controller: 'ShowMenuQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.showDisplay', {
                url: '/',
                templateUrl: baseUrl + 'js/productCalculation/showDisplayQuery.html',
                controller: 'ShowDisplayQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.requestValue', {
                url: '/',
                templateUrl: baseUrl + 'js/productCalculation/requestValueQuery.html',
                controller: 'RequestValueQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('finish', {
                url: '/finish',
                templateUrl: baseUrl + 'js/productCalculation/finish.html',
                controller: 'FinishController',
                controllerAs: 'vm',
                parent: 'calculate'
            });
            
        $urlRouterProvider.otherwise('/start');
    }
    
    return app;
});