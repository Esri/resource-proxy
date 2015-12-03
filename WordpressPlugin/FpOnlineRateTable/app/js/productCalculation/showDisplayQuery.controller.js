define([
    'onlineRateCalculator',
    'services/rateCalculationServiceFrontend.service'
], function(app) {
    "use strict";
    
    return app.controller(
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
    
    return app;
});