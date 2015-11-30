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