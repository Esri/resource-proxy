define([
    'onlineRateCalculator',
    'services/formatFilters',
    'services/rateCalculationService'
], function(app) {
    "use strict";
    
    return app.controller('DuringCalculationCtrl', [
        '$state',
        '$stateParams',
        'Translation',
        'RateCalculationService',
        function($state, $stateParams, Translation, RateCalculationService) {
            
            var vm = this;
            
            // in case we have no useful parameters (e.g. the calculate page
            // was accessed directly instead of getting here through the start
            // page) redirect to start page.
            if(!angular.isObject($stateParams.serviceState)) {
                $state.go("start");
            }
            
            vm.translation = Translation.table();
            vm.serviceState = $stateParams.serviceState;
            
            vm.changeWeight = changeWeight;
            vm.selectProductOption = selectProductOption;

            
            function changeWeight() {
                var t = 42;
            };
            
            function selectProductOption(index) {
                vm.serviceState = RateCalculationService.calculate(index);
            }
        }
    ]);
});