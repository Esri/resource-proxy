define([
    'onlineRateCalculator',
    'services/onlineRateTableServices'
], function(app) {
    "use strict";
    
    return app.controller('StartCalculationCtrl', [
        '$scope',
        '$state',
        'RateCalculationStartService',
        function($scope, $state, RateCalculationStartService) {
            
            $scope.translation = $scope.appData.translation;
            
            // Once the 'ng-init' directive is evaluated take the zip code regex
            // and make it available to the controller.
            $scope.$watch('appData.config.zipRegex', function(newVal, oldVal) {
                if(newVal) {
                    // we need to remove the leading and trailing slashes as
                    // Javascript does not expect them (as opposed to PHP).
                    var withOutSlashes= $scope.appData.config.zipRegex.replace(
                            /^\/|\/$/g, '');
                    $scope.zipRegex = new RegExp(withOutSlashes);
                }
            });
            
            // This is our own pattern matching validation code.
            // We need to to implement it ourselves instead of using the
            // 'ng-pattern' directive because the pattern is not fixed but set
            // using 'ng-init'. Obviously 'ng-pattern' does not work on
            // patterns set via 'ng-init'.
            $scope.$watch('zipCode', function(newVal, oldVal) {
                var model = $scope.ProductCalculationStart.SenderZip;
                var regexIsPresent = !!$scope.zipRegex;
                var newValIsSet = !!newVal;
                if(regexIsPresent && newValIsSet) {
                    var regexMatches = $scope.zipRegex.test(newVal);
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
                    RateCalculationStartService.init(
                            $scope.appData.config.rateCalculationUrl,
                            environment, weight);
                    var serviceState = RateCalculationStartService.start();
                    $state.go("calculate", {
                        'zip': $scope.zipCode,
                        'serviceState': serviceState});
                }
            };
        }
    ]);
});