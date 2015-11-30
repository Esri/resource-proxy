define([
    'onlineRateCalculator',
    'filters/postageAsString',
    'filters/weightAsString',
    'services/rateCalculationServiceFrontend.service',
    'services/weight',
    'productCalculation/currentProductDescription.service'
], function(app) {
    "use strict";
    
    return app.controller(
            'DuringCalculationController', DuringCalculationController);
        
    
    DuringCalculationController.$inject = [
        '$rootScope',
        '$state',
        '$stateParams',
        'Translation',
        'RateCalculationServiceFrontend',
        'CurrentProductDescription.service',
        'Weight'];
    
    function DuringCalculationController($rootScope, $state, $stateParams,
                Translation, RateCalculationServiceFrontend,
                CurrentProductDescription, Weight) {

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
        calc.weightInputUnit = Weight.getLocalizedMinorUnit();
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
            RateCalculationServiceFrontend.updateWeight(calc.weightValue);
            calc.showWeightInput = false;
        };
        
        function stepBack() {
            RateCalculationServiceFrontend.back();
        }
    }
    
    return app;
});