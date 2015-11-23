define([
    'onlineRateCalculator',
    'services/onlineRateTableServices',
    'services/appSettings',
    'services/translation'
], function(app) {
    "use strict";
    
    return app.controller('StartCalculationCtrl', [
        '$scope',
        '$state',
        'RateCalculationStartService',
        'AppSettings',
        'Translation',
        function($scope, $state, RateCalculationStartService,
                AppSettings, Translation) {
            
            // Do settings and translation initialization.
            $scope.$watch('appData', function(newVal, oldVal) {
                if(newVal) {
                    AppSettings.init(newVal.config);
                    Translation.init(newVal.translation);
                    // make the translation table available as model so it can
                    // be used in views.
                    $scope.translation = Translation.table();
                }
            });
            
            // This is our own pattern matching validation code.
            // We need to to implement it ourselves instead of using the
            // 'ng-pattern' directive because the pattern is not fixed but set
            // using 'ng-init'. Obviously 'ng-pattern' does not work on
            // patterns set via 'ng-init'.
            $scope.$watch('zipCode', function(newVal, oldVal) {
                var model = $scope.ProductCalculationStart.SenderZip;
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
            
            $scope.proceedToCalculation = function() {
                if($scope.ProductCalculationStart.SenderZip.$valid) {
                    var weight = {
                        WeightValue: 1,
                        WeightUnit: 0
                    };
                    var environment = {
                        Iso3CountryCode: $scope.appData.config.countryCode,
                        CarrierId: $scope.appData.config.carrierId,
                        UtcDate: new Date(),
                        Culture: $scope.appData.config.culture,
                        SenderZipCode: $scope.zipCode
                    };
                    RateCalculationStartService.init(environment, weight);
                    var serviceState = RateCalculationStartService.start();
                    $state.go("calculate", {
                        'zip': $scope.zipCode,
                        'serviceState': serviceState});
                }
            };
        }
    ]);
});