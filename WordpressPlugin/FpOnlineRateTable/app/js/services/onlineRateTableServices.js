define([
    'angular',
    'angular-resource'
], function() {
    "use strict";
    
    var service = angular.module('OnlineRateTableServices', [
        'ngResource'
    ]);
    
    service.factory('RateCalculationStartService', ['$resource',
        function($resource) {
            var startService;
            var calculateService;
            var backService;
            var updateWeightService;
            
            // Note: we use the methode 'save' to call a service as this the the
            // angular-resource synonym for 'post'.
            return {
                init: function(url) {
                    startService = $resource(url + '/Start');
                    calculateService = $resource(url + '/Calculate');
                    backService = $resource(url + '/Back');
                    updateWeightService = $resource(url + '/UpdateWeight');
                },
                
                start: function(weight, environment) {
                    var result = startService.save({}, {
                        Weight: weight,
                        Environment: environment },
                        function success() {
                            var test = result;
                        },
                        function error() {
                            var test = result;
                        });
                },
                
                calculate: function(productDescription, actionResult, environment) {
                    return calculateService.save({}, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment
                    });
                },
                
                back: function(productDescription, environment) {
                    return backService.save({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                },
                
                updateWeight: function(productDescription, environment) {
                    return updateWeightService.save({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                }
            };
        }
    ]);
    
    return service;
});