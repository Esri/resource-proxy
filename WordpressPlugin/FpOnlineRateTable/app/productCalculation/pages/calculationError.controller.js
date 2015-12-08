define([
    '../fpProductCalculation.module',
    '../translation.service'
], function(module) {
    "use strict";
    
    module.controller('CalculationErrorController', CalculationErrorController);
        
    
    CalculationErrorController.$inject = [
        '$state',
        '$stateParams',
        'Translation'];
    
    function CalculationErrorController($state, $stateParams, Translation) {

        var vm = this;

        // We need an error argument to proceed. Otherwise redirect to start.
        if(!$stateParams.error) {
            return restartCalculation();
        }

        var error = $stateParams.error;
        if(error.Message) {             // if error results from an $http error message
            vm.errorMessage = error.Message;
        } else if(error.ErrorMessage) { // if error is a calculation error returned from PCalc service
            vm.errorMessage = error.ErrorMessage;
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