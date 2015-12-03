define([
    'onlineRateCalculator',
    'services/rateCalculationServiceFrontend.service'
], function(app) {
    "use strict";
    
    return app.controller(
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
    
    return app;
});