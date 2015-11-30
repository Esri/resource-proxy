define([
    'onlineRateCalculator',
    'filters/postageAsString',
    'filters/weightAsString',
    'services/rateCalculationServiceFrontend.service',
    'services/weight'
], function(app) {
    "use strict";
    
    return app.controller(
            'DuringCalculationController', DuringCalculationController);
        
    
    DuringCalculationController.$inject = [
        '$rootScope',
        '$state',
        'Translation',
        'RateCalculationServiceFrontend',
        'Weight'];
    
    function DuringCalculationController($rootScope, $state, Translation,
                RateCalculationServiceFrontend, Weight) {

        var vm = this;

        vm.translation = Translation.table();
        vm.weightInputUnit = Weight.getLocalizedMinorUnit();

        vm.changeWeight = changeWeight;

        activate();

        /////////////////////////

        function activate() {
            
            $rootScope.$watch('productDescriptionChanged',
            function(productDescription) {
               vm.productDescription = productDescription; 
            });
        }

        function changeWeight() {
            RateCalculationServiceFrontend.updateWeight(vm.weightValue);
            vm.showWeightInput = false;
        };
    }
    
    return app;
});