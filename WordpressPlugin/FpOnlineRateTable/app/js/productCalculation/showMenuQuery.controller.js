define([
    'onlineRateCalculator',
    'services/rateCalculationServiceFrontend.service'
], function(app) {
    "use strict";
    
    return app.controller(
            'ShowMenuQueryController', ShowMenuQueryController);
        
    
    ShowMenuQueryController.$inject = [
        '$stateParams',
        'RateCalculationServiceFrontend'];
    
    function ShowMenuQueryController(
            $stateParams, RateCalculationServiceFrontend) {

        var vm = this;

        vm.queryDescription = $stateParams.queryDescription;
        vm.selectMenuItem = selectMenuItem;
        
        ///////////
        
        function selectMenuItem(index) {
            RateCalculationServiceFrontend.calculate(index);
        }
    }
    
    return app;
});