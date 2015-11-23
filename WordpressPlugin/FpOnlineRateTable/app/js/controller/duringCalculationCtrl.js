define([
    'onlineRateCalculator',
    'services/formatFilters'
], function(app) {
    "use strict";
    
    return app.controller('DuringCalculationCtrl', [
        '$scope',
        '$state',
        '$stateParams',
        function($scope, $state, $stateParams) {
            
            $scope.translation = $scope.appData.translation;
            $scope.zip = $stateParams.zip;
            $scope.serviceState = $stateParams.serviceState;
            
            // in case we have no useful parameters (e.g. the calculate page
            // was accessed directly instead of getting here through the start
            // page) redirect to start page.
            if((null === $scope.zip === null)
                    || (null === $scope.serviceState)) {
                $state.go("start");
            }
            
            $scope.changeWeight = function() {
                var t = 42;
            };
        }
    ]);
});