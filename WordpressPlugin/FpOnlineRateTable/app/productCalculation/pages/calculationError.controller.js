define([
    '../fpProductCalculation.module',
    '../translation.service',
    '../../errorHandler/fpException.factory'
], function(module) {
    "use strict";
    
    module.controller('CalculationErrorController', CalculationErrorController);
        
    
    CalculationErrorController.$inject = [
        '$state',
        '$stateParams',
        'Translation',
        'FpException'];
    
    function CalculationErrorController(
            $state, $stateParams, Translation, FpException) {

        var vm = this;

        // We need an error argument to proceed. Otherwise redirect to start.
        if(!$stateParams.error) {
            return restartCalculation();
        }

        var error = $stateParams.error;
        if(error instanceof FpException) {
            vm.errorMessage = error.message;
        } else if('string' === typeof error) {
            vm.errorMessage = error;
        }
        vm.translation = Translation.table();
        vm.restartCalculation = restartCalculation;
        
        //////////
        
        function restartCalculation() {
            return $state.go('start');
        }
    }
    
    return module;
});