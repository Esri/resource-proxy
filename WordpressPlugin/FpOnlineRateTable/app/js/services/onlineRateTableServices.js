define([
    'onlineRateCalculator',
    'services/appSettings'
], function(app) {
    "use strict";
    
    app.factory('RateCalculationStartService', [
        '$resource', 'AppSettings',
        function($resource, AppSettings) {
            var startServiceUrl;
            var calculateServiceUrl;
            var backServiceUrl;
            var updateWeightServiceUrl;
            var service = null;
            
            var weight;
            var environment;
            var productDescription;
            
            function ServiceException(message) {
                this.name = 'ServiceEexception';
                this.message= message;
            }
            ServiceException.prototype = new Error();
            ServiceException.prototype.constructor = ServiceException;
            
            
            // Note: we use the methode 'save' to call a service as this the the
            // angular-resource synonym for 'post'.
            return {
                isInitialized: function() {
                    return !!service;
                },
                
                init: function(env, startWeight) {
                    
                    if(!AppSettings.isInitialized()) {
                        return null;
                    }
                    
                    var url = AppSettings.rateCalculationUrl();
                    
                    startServiceUrl = url + '/Start';
                    calculateServiceUrl = url + '/Calculate';
                    backServiceUrl = url + '/Back';
                    updateWeightServiceUrl = url + '/UpdateWeight';
                    
                    weight = startWeight;
                    environment = env;
                    
                    service = $resource(url, {}, {
                        'start': { method: 'POST', url: startServiceUrl },
                        'calculate': { method: 'POST', url: calculateServiceUrl },
                        'back': { method: 'POST', url: backServiceUrl },
                        'updateWeight': { method: 'POST', url: updateWeightServiceUrl }
                    });
                },
                
                start: function() {
                    //var result = startService.save({}, {
                    var result = service.start({}, {
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
                    return service.calculate({}, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment
                    });
                },
                
                back: function(environment) {
                    return service.back({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                },
                
                updateWeight: function(environment) {
                    return service.updateWeight({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                }
            };
        }
    ]);
    
    return app;
});