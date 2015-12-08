define([
    '../fpProductCalculation.module',
    '../postageAsString.filter',
    '../weightAsString.filter',
    '../service/rateCalculationServiceFrontend.service',
    '../weightHelper.service',
    '../currentProductDescription.service',
    '../moduleSettings.service'
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
        'WeightHelper',
        'ModuleSettings'];
    
    function DuringCalculationController($rootScope, $state, $stateParams,
                Translation, RateCalculationServiceFrontend,
                CurrentProductDescription, WeightHelper, ModuleSettings) {

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
        calc.maxWeight = ModuleSettings.maxWeight();
        calc.showFinish = false;
        calc.showBack = false;
        calc.isComplete = false;
        
        calc.changeWeight = changeWeight;
        calc.stepBack = stepBack;

        activate();

        /////////////////////////

        function activate() {
            
            $rootScope.$watch('currentProductDescriptionChanged',
                function() {
                    calc.productDescription = CurrentProductDescription.get(); 
                    calc.isComplete = CurrentProductDescription.isComplete();
                    calc.showBack = CurrentProductDescription.hasHistory();
                    
                    // showFinish is set independently from isComplete as they are
                    // not the same once the user presses 'Finsish'.
                    calc.showFinish = calc.isComplete;
                });
        }

        function changeWeight() {
            
            if(calc.ChangeWeightForm.WeightValue.$valid) {
                calc.showWeightInput = false;
                RateCalculationServiceFrontend.updateWeight(calc.weightValue);
            }   
        };
        
        function stepBack() {
            RateCalculationServiceFrontend.back();
        }
    }
    
    return module;
});