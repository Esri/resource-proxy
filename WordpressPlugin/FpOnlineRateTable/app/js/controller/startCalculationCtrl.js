define([
    'onlineRateCalculator',
    'services/rateCalculationService',
    'services/appSettings',
    'services/translation'
], function(app) {
    "use strict";
    
    return app.controller('StartCalculationCtrl', [
        '$scope',
        '$state',
        'RateCalculationService',
        'AppSettings',
        'Translation',
        function($scope, $state, RateCalculationService,
                AppSettings, Translation) {
            var vm = this;
            
            // Do settings and translation initialization.
            $scope.$watch('appData', function(newVal, oldVal) {
                if(newVal) {
                    AppSettings.init(newVal.config);
                    Translation.init(newVal.translation);
                    // make the translation table available as model so it can
                    // be used in views.
                    vm.translation = Translation.table();
                }
            });
            
            // This is our own pattern matching validation code.
            // We need to to implement it ourselves instead of using the
            // 'ng-pattern' directive because the pattern is not fixed but set
            // using 'ng-init'. Obviously 'ng-pattern' does not work on
            // patterns set via 'ng-init'.
            $scope.$watch('vm.zipCode', function(newVal, oldVal) {
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
                return;
            });
            
            vm.proceedToCalculation = function() {
                if(vm.ProductCalculationStart.SenderZip.$valid) {
                    RateCalculationService.init(vm.zipCode);
                    var serviceState = RateCalculationService.start();
                    
                    $state.go("calculate", {
                        'serviceState': serviceState});
                }
            };
        }
    ]);
});