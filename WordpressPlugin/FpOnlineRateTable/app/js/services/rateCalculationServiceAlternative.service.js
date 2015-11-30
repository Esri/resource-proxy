define([
    'onlineRateCalculator',
    'services/appSettings'
], function(app) {
    "use strict";
    
    app.factory('RateCalculationServiceAlternative',
            RateCalculationServiceAlternative);
    
    RateCalculationServiceAlternative.$inject = [
        '$resource',
        'AppSettings'];
    
    function RateCalculationServiceAlternative(
            $resource, AppSettings) {

        var service = null;
        var startServiceUrl;
        var calculateServiceUrl;
        var backServiceUrl;
        var updateWeightServiceUrl;


        function ServiceException(message) {
            this.name = 'ServiceEexception';
            this.message= message;
        }
        ServiceException.prototype = new Error();
        ServiceException.prototype.constructor = ServiceException;


        // Note: we use the methode 'save' to call a service as this the the
        // angular-resource synonym for 'post'.
        return {
            start: start,
            calculate: calculate,
            back: back,
            updateWeight: updateWeight
        };
            
        function getService() {
            
            if(!service) {
                
                var url = AppSettings.rateCalculationUrl();

                startServiceUrl = url + '/Start';
                calculateServiceUrl = url + '/Calculate';
                backServiceUrl = url + '/Back';
                updateWeightServiceUrl = url + '/UpdateWeight';

                service = $resource(url, {}, {
                    'start': { method: 'POST', url: startServiceUrl },
                    'calculate': { method: 'POST', url: calculateServiceUrl },
                    'back': { method: 'POST', url: backServiceUrl },
                    'updateWeight': { method: 'POST', url: updateWeightServiceUrl }
                });
                
            }
            
            return service;
        }
        
        function callService(serviceMethod, serviceUrl, serviceParams) {

            var call = serviceMethod.call(service, {}, serviceParams,
                null,
                function error(error) {
                    throw new ServiceException(
                            'service error occurred when trying do a post request for "'
                            + serviceUrl + '": ' + error.data.Message);
                });

            return call.$promise;
        }

        function start(weight, environment) {

            var service = getService();

            return callService(service.start, startServiceUrl, {
                Weight: weight,
                Environment: environment });
        }

        function calculate(productDescription, actionResult, environment) {

            var service = getService();

            return callService(service.calculate, calculateServiceUrl, {
                ProductDescription: productDescription,
                ActionResult: actionResult,
                Environment: environment });

        }

        function back(productDescription, environment) {

            var service = getService(); 

            return callService(
                    service.back, updateWeightServiceUrl, {
                        ProductDescription: productDescription,
                        Environment: environment });
        }

        function updateWeight(productDescription, environment) {

            var service = getService(); 

            return callService(
                    service.updateWeight, updateWeightServiceUrl, {
                        ProductDescription: productDescription,
                        Environment: environment });
        }
    }
    
    return app;
});