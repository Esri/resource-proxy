define([
    './fpProductCalculation.module',
    './pages/startCalculation.controller',
    './pages/duringCalculation.controller',
    './pages/calculationError.controller',
    './pages/queries/showMenuQuery.controller',
    './pages/queries/showDisplayQuery.controller',
    './pages/queries/requestValueQuery.controller',
    './pages/queries/finish.controller'
], function(module) {
    "use strict";
    
    module.config(RoutingTable);
    
    RoutingTable.$inject = ['$stateProvider', '$urlRouterProvider'];
            
    function RoutingTable($stateProvider, $urlRouterProvider) {
        
        var baseUrl = '/wordpress/wp-content/plugins/FpOnlineRateTable/app/productCalculation/';
        $stateProvider
            .state('error', {
                url: '/error',
                templateUrl: baseUrl + 'pages/calculationError.html',
                controller: 'CalculationErrorController',
                controllerAs: 'vm',
                params: {'error': {}}
            })
            .state('start', {
                url: '/start',
                templateUrl: baseUrl + 'pages/startCalculation.html',
                controller: 'StartCalculationController',
                controllerAs: 'vm'
            })
            .state('calculate', {
                url: '/calculate',
                templateUrl: baseUrl + 'pages/duringCalculation.html',
                controller: 'DuringCalculationController',
                controllerAs: 'calc',
                params: { 'queryDescription': null }
            })
            .state('calculate.showMenu', {
                url: '/',
                templateUrl: baseUrl + 'pages/queries/showMenuQuery.html',
                controller: 'ShowMenuQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.showDisplay', {
                url: '/',
                templateUrl: baseUrl + 'pages/queries/showDisplayQuery.html',
                controller: 'ShowDisplayQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.requestValue', {
                url: '/',
                templateUrl: baseUrl + 'pages/queries/requestValueQuery.html',
                controller: 'RequestValueQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('finish', {
                url: '/finish',
                templateUrl: baseUrl + 'pages/queries/finish.html',
                controller: 'FinishController',
                controllerAs: 'vm',
                parent: 'calculate'
            });
            
        $urlRouterProvider.otherwise('/start');
    }
    
    return module;
});