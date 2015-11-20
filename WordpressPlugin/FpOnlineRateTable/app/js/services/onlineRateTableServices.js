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
            var service;
            
            return {
                init: function(url) {
                    service = $resource(url, {}, {
                        Start: {
                            method: 'POST',
                            params: {Weight: null, Environment: null} },
                        Calculate: {
                            method: 'POST',
                            params: {ProductDescription: null, ActionResult: null, Environment: null} },
                        Back: {
                            method: 'POST',
                            params: {ProductDescription: null, Environment: null} },
                        UpdateWeight: {
                            method: 'POST',
                            params: {ProductDescription: null, Environment: null} }
                        });
                },
                
                start: function(weight, environment) {
                    return service.Start({}, {
                        Weight: weight,
                        Environment: environment
                    });
                },
                
                calculate: function(productDescription, actionResult, environment) {
                    return service.Calculate({}, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment
                    });
                },
                
                back: function(productDescription, environment) {
                    return service.Calculate({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                },
                
                updateWeight: function(productDescription, environment) {
                    return service.Calculate({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                }
            };
        }
    ]);
    
    return service;
});