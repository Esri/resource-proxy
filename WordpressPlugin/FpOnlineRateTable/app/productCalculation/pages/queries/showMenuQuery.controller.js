define([
    '../../fpProductCalculation.module',
    '../../service/rateCalculationServiceFrontend.service'
], function(module) {
    "use strict";
    
    return module.controller(
            'ShowMenuQueryController', ShowMenuQueryController);
        
    
    ShowMenuQueryController.$inject = [
        'RateCalculationServiceFrontend'];
    
    function ShowMenuQueryController(RateCalculationServiceFrontend) {

        var vm = this;

        vm.selectMenuItem = selectMenuItem;
        
        ///////////
        
        function selectMenuItem(index) {
            RateCalculationServiceFrontend.selectMenuIndex(index);
        }
    }
    
    return module;
});