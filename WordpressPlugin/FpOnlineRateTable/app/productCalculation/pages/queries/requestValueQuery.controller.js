define([
    '../../fpProductCalculation.module',
    '../../service/rateCalculationServiceFrontend.service',
    '../../../helper/fpInputLine.directive'
], function(module) {
    "use strict";
    
    return module.controller(
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
    
    return module;
});