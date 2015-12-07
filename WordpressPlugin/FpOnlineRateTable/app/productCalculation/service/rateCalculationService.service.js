define([
    '../fpProductCalculation.module',
    '../moduleSettings.service'
], function(module) {
    "use strict";
    
    function ServiceException(message) {
        this.name = 'ServiceException';
        this.message= message;
    }
    ServiceException.prototype = new Error();
    ServiceException.prototype.constructor = ServiceException;
    
    
    module.factory('RateCalculationService', RateCalculationService);
    
    RateCalculationService.$inject = ['$http', 'ModuleSettings'];
    
    
    function RateCalculationService($http, ModuleSettings) {
        
        return {
            start: start,
            calculate: calculate,
            back: back,
            updateWeight: updateWeight
        };
        
        //////////

        function start(weight, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Start';
            var params = {
                Weight: weight,
                Environment: environment};
            var promise = $http.post(url, params);
            return handleFailure(promise, url);
        }
        
        function calculate(productDescription, actionResult, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Calculate';
            var params = {
                ProductDescription: productDescription,
                ActionResult: actionResult,
                Environment: environment};
            var promise = $http.post(url, params);
            return handleFailure(promise, url);
        }
        
        function back(productDescription, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Back';
            var params = {
                ProductDescription: productDescription,
                Environment: environment};
            var promise = $http.post(url, params);
            return handleFailure(promise, url);
        }
        
        function updateWeight(productDescription, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/UpdateWeight';
            var params = {
                ProductDescription: productDescription,
                Environment: environment};
            var promise = $http.post(url, params);
            return handleFailure(promise, url);
        }
        
        function handleFailure(promise, url) {
            return promise.catch(function(error) {
                throw new ServiceException(
                        'service error occurred when trying do a post request for "'
                        + url + '": ' + error.data.Message);
            });
        }
    }
    
    return module;
});