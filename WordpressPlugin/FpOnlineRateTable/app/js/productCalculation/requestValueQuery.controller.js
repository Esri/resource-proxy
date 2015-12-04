define([
    'onlineRateCalculator',
    'services/rateCalculationServiceFrontend.service',
    'helper/fpInputLine.directive'
], function(app) {
    "use strict";
    
    return app.controller(
            'RequestValueQueryController', RequestValueQueryController);
        
    
    RequestValueQueryController.$inject = [
        'RateCalculationServiceFrontend'];
    
    function RequestValueQueryController(RateCalculationServiceFrontend) {

        var vm = this;

        vm.requestValue = requestValue;
     
        ///////////
        
        function requestValue() {
            RateCalculationServiceFrontend.requestValue(vm.value);
        }
    }
    
    return app;
});