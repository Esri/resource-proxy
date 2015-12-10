define([
    '../../fpProductCalculation.module',
    '../../service/rateCalculationServiceFrontend.service'
], function(module) {
    "use strict";
    
    return module.controller(
            'SelectValueQueryController', SelectValueQueryController);
        
    
    SelectValueQueryController.$inject = [
        'RateCalculationServiceFrontend'];
    
    function SelectValueQueryController(RateCalculationServiceFrontend) {

        var vm = this;

        vm.selectValue = selectValue;
        
        ///////////
        
        function selectValue(value) {
            RateCalculationServiceFrontend.selectValue(value);
        }
    }
    
    return module;
});