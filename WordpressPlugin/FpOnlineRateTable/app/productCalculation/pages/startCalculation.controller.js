define([
    '../fpProductCalculation.module',
    '../service/rateCalculationServiceFrontend.service',
    '../moduleSettings.service',
    '../translation.service'
], function(module) {
    "use strict";
    
    return module.controller(
            'StartCalculationController', StartCalculationController);
    
    
    StartCalculationController.$inject = [
        '$scope',
        'RateCalculationServiceFrontend',
        'ModuleSettings',
        'Translation'];
    
    function StartCalculationController($scope,
            RateCalculationServiceFrontend, ModuleSettings, Translation) {

        var vm = this;
        
        // as long as ng-init has not been evaluated we accept everything as
        // zip code input.
        vm.zipRegex = /.*/;
        
        vm.proceedToCalculation = proceedToCalculation;
        
        activate();

        //////////////

        function activate() {
            // Do settings and translation initialization.
            $scope.$watch('appData', appDataChanged);
        }

        function appDataChanged(newVal) {
            
            if(newVal) {
                ModuleSettings.init(newVal.config);
                Translation.init(newVal.translation);
                // make the translation table available as model so it can
                // be used in views.
                vm.translation = Translation.table();
                vm.zipRegex = ModuleSettings.zipRegex();
                
                // call the GetActivateTables service. This has no effect except
                // if no current rate table could be found. In this case we are
                // redirected to the error page.
                RateCalculationServiceFrontend.getActiveTables();
            }
        }

        function proceedToCalculation() {
            
            if(vm.ProductCalculationStart.SenderZip.$valid) {
                RateCalculationServiceFrontend.start(vm.zipCode);
            }
        };
    }
    
    return module;
});