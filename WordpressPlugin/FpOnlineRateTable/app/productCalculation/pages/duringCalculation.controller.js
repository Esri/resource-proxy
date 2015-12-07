define([
    '../fpProductCalculation.module',
    '../postageAsString.filter',
    '../weightAsString.filter',
    '../service/rateCalculationServiceFrontend.service',
    '../weightHelper.service',
    '../currentProductDescription.service'
], function(module) {
    "use strict";
    
    return module.controller(
            'DuringCalculationController', DuringCalculationController);
        
    
    DuringCalculationController.$inject = [
        '$rootScope',
        '$state',
        '$stateParams',
        'Translation',
        'RateCalculationServiceFrontend',
        'CurrentProductDescription.service',
        'WeightHelper'];
    
    function DuringCalculationController($rootScope, $state, $stateParams,
                Translation, RateCalculationServiceFrontend,
                CurrentProductDescription, WeightHelper) {

        var calc = this;

        // Don't allow direct access to this state. Also we need a query
        // description argument to proceed.
        if(($state.current.name === 'calculate')
                || !$stateParams.queryDescription) {
            return $state.go('start');
        }

        calc.queryDescription = $stateParams.queryDescription;
        calc.translation = Translation.table();
        calc.productDescription = CurrentProductDescription.get();
        calc.weightInputUnit = WeightHelper.getLocalizedMinorUnit();
        calc.complete = false;
        calc.hasHistory = false;
        
        calc.changeWeight = changeWeight;
        calc.stepBack = stepBack;

        activate();

        /////////////////////////

        function activate() {
            
            $rootScope.$watch('currentProductDescriptionChanged',
                function() {
                    calc.productDescription = CurrentProductDescription.get(); 
                    calc.complete = CurrentProductDescription.isComplete();
                    calc.hasHistory = CurrentProductDescription.hasHistory();
                });
        }

        function changeWeight() {
            
            calc.showWeightInput = false;
            if(calc.weightValue) {
                RateCalculationServiceFrontend.updateWeight(calc.weightValue);
            }   
        };
        
        function stepBack() {
            RateCalculationServiceFrontend.back();
        }
    }
    
    return module;
});