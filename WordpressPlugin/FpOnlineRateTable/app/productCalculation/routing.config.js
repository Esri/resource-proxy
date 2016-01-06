define([
    './fpProductCalculation.module',
    './pages/startCalculation.controller',
    './pages/duringCalculation.controller',
    './pages/calculationError.controller',
    './pages/queries/showMenuQuery.controller',
    './pages/queries/showDisplayQuery.controller',
    './pages/queries/requestValueQuery.controller',
    './pages/queries/selectValueQuery.controller',
    './pages/queries/finish.controller',
    '../helper/fpAdjustHeight.directive'
], function(module) {
    "use strict";
    
    module.config(RoutingTable);
    
    RoutingTable.$inject = [
        '$stateProvider',
        '$urlRouterProvider',
        'BaseUrl'               // defined in main.js
    ];
            
    function RoutingTable($stateProvider, $urlRouterProvider, BaseUrl) {
        
        var baseUrl = BaseUrl + '/productCalculation/pages/';
        $stateProvider
            .state('error', {
                url: '/error',
                templateUrl: baseUrl + 'calculationError.html',
                controller: 'CalculationErrorController',
                controllerAs: 'vm',
                params: {'error': null}
            })
            .state('start', {
                url: '/start',
                templateUrl: baseUrl + 'startCalculation.html',
                controller: 'StartCalculationController',
                controllerAs: 'vm'
            })
            .state('calculate', {
                url: '/calculate',
                templateUrl: baseUrl + 'duringCalculation.html',
                controller: 'DuringCalculationController',
                controllerAs: 'calc',
                params: {'queryDescription': null}
            })
            .state('calculate.showMenu', {
                url: '/',
                templateUrl: baseUrl + 'queries/showMenuQuery.html',
                controller: 'ShowMenuQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.showDisplay', {
                url: '/',
                templateUrl: baseUrl + 'queries/showDisplayQuery.html',
                controller: 'ShowDisplayQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.requestValue', {
                url: '/',
                templateUrl: baseUrl + 'queries/requestValueQuery.html',
                controller: 'RequestValueQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.selectValue', {
                url: '/',
                templateUrl: baseUrl + 'queries/selectValueQuery.html',
                controller: 'SelectValueQueryController',
                controllerAs: 'vm',
                parent: 'calculate'
            })
            .state('calculate.finish', {
                url: '/finish',
                templateUrl: baseUrl + 'queries/finish.html',
                controller: 'FinishController',
                controllerAs: 'vm',
                parent: 'calculate'
            });
            
        $urlRouterProvider.otherwise('/start');
    }
    
    return module;
});