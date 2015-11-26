define([
    'onlineRateCalculator',
    'services/rateCalculationService',
    'services/appSettings',
    'services/translation'
], function(app) {
    "use strict";
    
    return app.controller(
            'StartCalculationController', StartCalculationController);
    
    
    StartCalculationController.$inject([
        '$scope',
        '$state',
        'RateCalculationService',
        'AppSettings',
        'Translation']);
    
    function StartCalculationController($scope, $state, RateCalculationService,
                AppSettings, Translation) {

        var vm = this;
        
        vm.proceedToCalculation = proceedToCalculation;
        
        activate();

        //////////////

        function activate() {
            // Do settings and translation initialization.
            $scope.$watch('appData', appDataChanged);

            // This is our own pattern matching validation code.
            // We need to to implement it ourselves instead of using the
            // 'ng-pattern' directive because the pattern is not fixed but set
            // using 'ng-init'. Obviously 'ng-pattern' does not work on
            // patterns set via 'ng-init'.
            $scope.$watch('vm.zipCode', zipCodeChanged);
        }

        function appDataChanged(newVal, oldVal) {
            
            if(newVal) {
                AppSettings.init(newVal.config);
                Translation.init(newVal.translation);
                // make the translation table available as model so it can
                // be used in views.
                vm.translation = Translation.table();
            }
        }

        function proceedToCalculation() {
            
            if(vm.ProductCalculationStart.SenderZip.$valid) {
                RateCalculationService.init(vm.zipCode);
                var serviceState = RateCalculationService.start();

                $state.go("calculate", {
                    'serviceState': serviceState});
            }
        };
        
        function zipCodeChanged(newVal, oldVal) {
            
            var model = vm.ProductCalculationStart.SenderZip;
            var zipRegex = AppSettings.zipRegex();
            var regexIsPresent = !!zipRegex;
            var newValIsSet = !!newVal;
            if(regexIsPresent && newValIsSet) {
                var regexMatches = zipRegex.test(newVal);
                if(regexMatches) {
                    model.$setValidity('pattern', true);
                    return newVal;
                }
            }

            model.$setValidity('pattern', false);
        }
    }
});