define([
    'onlineRateCalculator',
    'services/translation'
], function(app) {
    "use strict";
    
    app.controller('CalculationErrorController', CalculationErrorController);
        
    
    CalculationErrorController.$inject = [
        '$state',
        '$stateParams',
        'Translation'];
    
    function CalculationErrorController($state, $stateParams, Translation) {

        var vm = this;

        // We need an error argument to proceed. Otherwise redirect to start.
        if(!$stateParams.error) {
            return $state.go('start');
        }

        vm.error = $stateParams.error;
        vm.translation = Translation.table();
        vm.restartCalculation = restartCalculation;
        
        //////////
        
        function restartCalculation() {
            $state.go('start');
        }
    }
    
    
    return app;
});