define([
    '../fpProductCalculation.module',
    '../moduleSettings.service'
], function(module) {
    "use strict";    
    
    module.factory('RateCalculationService', RateCalculationService);
    
    RateCalculationService.$inject = ['$http', 'ModuleSettings'];
    
    
    function RateCalculationService($http, ModuleSettings) {
        
        return {
            getActiveTables: getActiveTables,
            start: start,
            calculate: calculate,
            back: back,
            updateWeight: updateWeight
        };
        
        //////////

        function getActiveTables(date) {
            
            var url = ModuleSettings.rateCalculationUrl()
                    + '/GetActiveTables?clientUtcDate=' + date;
            var promise = $http.get(url);
            return promise;
        }

        function start(weight, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Start';
            var params = {
                Weight: weight,
                Environment: environment};
            var promise = $http.post(url, params);
            return promise;
        }
        
        function calculate(productDescription, actionResult, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Calculate';
            var params = {
                ProductDescription: productDescription,
                ActionResult: actionResult,
                Environment: environment};
            var promise = $http.post(url, params);
            return promise;
        }
        
        function back(productDescription, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/Back';
            var params = {
                ProductDescription: productDescription,
                Environment: environment};
            var promise = $http.post(url, params);
            return promise;
        }
        
        function updateWeight(productDescription, environment) {
            
            var url = ModuleSettings.rateCalculationUrl() + '/UpdateWeight';
            var params = {
                ProductDescription: productDescription,
                Environment: environment};
            var promise = $http.post(url, params);
            return promise;
        }
    }
    
    return module;
});