define([
    '../../fpProductCalculation.module',
    '../../service/rateCalculationServiceFrontend.service'
], function(module) {
    "use strict";
    
    return module.controller(
            'ShowDisplayQueryController', ShowDisplayQueryController);
        
    
    ShowDisplayQueryController.$inject = [
        'RateCalculationServiceFrontend'];
    
    function ShowDisplayQueryController(RateCalculationServiceFrontend) {

        var vm = this;

        vm.acknoledge = acknoledge;
        
        ///////////
        
        function acknoledge() {
            RateCalculationServiceFrontend.acknoledge();
        }
    }
    
    return module;
});