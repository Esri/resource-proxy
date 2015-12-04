define([
    'onlineRateCalculator',
    'services/rateCalculationServiceFrontend.service',
    'services/appSettings',
    'services/translation'
], function(app) {
    "use strict";
    
    return app.controller(
            'StartCalculationController', StartCalculationController);
    
    
    StartCalculationController.$inject = [
        '$scope',
        'RateCalculationServiceFrontend',
        'AppSettings',
        'Translation'];
    
    function StartCalculationController($scope,
            RateCalculationServiceFrontend, AppSettings, Translation) {

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
                AppSettings.init(newVal.config);
                Translation.init(newVal.translation);
                // make the translation table available as model so it can
                // be used in views.
                vm.translation = Translation.table();
                vm.zipRegex = AppSettings.zipRegex();
            }
        }

        function proceedToCalculation() {
            
            if(vm.ProductCalculationStart.SenderZip.$valid) {
                RateCalculationServiceFrontend.start(vm.zipCode);
            }
        };
    }
    
    return app;
});