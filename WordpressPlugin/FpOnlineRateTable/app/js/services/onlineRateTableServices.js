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
            var startServiceUrl;
            var calculateService;
            var calculateServiceUrl;
            var backService;
            var backServiceUrl;
            var updateWeightService;
            var updateWeightServiceUrl;
            
            var weight;
            var environment;
            var productDescription;
            
            function ServiceEexception(message) {
                this.name = 'ServiceEexception';
                this.message= message;
            }
            ServiceEexception.prototype = new Error();
            ServiceEexception.prototype.constructor = ServiceEexception;
            
            
            // Note: we use the methode 'save' to call a service as this the the
            // angular-resource synonym for 'post'.
            return {
                init: function(url, env, startWeight) {
                    startServiceUrl = url + '/Start';
                    startService = $resource(startServiceUrl);
                    calculateServiceUrl = url + '/Calculate';
                    calculateService = $resource(calculateServiceUrl);
                    backServiceUrl = url + '/Back';
                    backService = $resource(backServiceUrl);
                    updateWeightServiceUrl = url + '/UpdateWeight';
                    updateWeightService = $resource(updateWeightServiceUrl);
                    
                    weight = startWeight;
                    environment = env;
                },
                
                start: function() {
                    var result = startService.save({}, {
                        Weight: weight,
                        Environment: environment },
                        function success() {
                            // now that the service call has returned
                            // successfully we can access the ProductDescription
                            // field in th promise. So store the returned
                            // product description for further use.
                            productDescription = result.ProductDescription;
                            
                            // also append the current weight and envoronment
                            // objects to the result so they're available in for
                            // the caller (presumably a controller).
                            result.Weight = weight;
                            result.Environment = environment;
                        },
                        function error(error) {
                            throw new ServiceException(
                                    'service error occurred when trying do a post request for "'
                                    + startServiceUrl + '"');
                        });
                        
                    // returns the promise for the service call
                    return result;
                },
                
                calculate: function(actionResult, environment) {
                    return calculateService.save({}, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment
                    });
                },
                
                back: function(environment) {
                    return backService.save({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                },
                
                updateWeight: function(environment) {
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