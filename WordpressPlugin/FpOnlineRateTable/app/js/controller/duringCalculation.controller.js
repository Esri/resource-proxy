define([
    'onlineRateCalculator',
    'filters/postageAsString',
    'filters/weightAsString',
    'services/rateCalculationService',
    'services/weight'
], function(app) {
    "use strict";
    
    return app.controller(
            'DuringCalculationController', DuringCalculationController);
        
    
    DuringCalculationController.$inject([
        '$state',
        'Translation',
        'RateCalculationService',
        'Weight']);
    
    function DuringCalculationController($state, Translation,
                RateCalculationService, Weight) {

        var vm = this;

        vm.serviceState = RateCalculationService.getServiceState();
        // in case we have no useful parameters (e.g. the calculate page
        // was accessed directly instead of getting here through the start
        // page) redirect to start page.
        if(!vm.serviceState) {
             $state.go("start");
             return;
        }

        vm.translation = Translation.table();
        vm.weightInputUnit = Weight.getLocalizedMinorUnit();

        vm.changeWeight = changeWeight;
        vm.selectProductOption = selectProductOption;

        /////////////////////////

        function changeWeight() {
            vm.serviceState
                    = RateCalculationService.updateWeight(vm.weightValue);
            vm.showWeightInput = false;
        };

        function selectProductOption(index) {
            vm.serviceState = RateCalculationService.calculate(index);
        }
    }
});