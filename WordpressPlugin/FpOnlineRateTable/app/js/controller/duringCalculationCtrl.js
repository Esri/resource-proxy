define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    return app.controller('DuringCalculationCtrl', [
        '$scope',
        '$stateParams',
        function($scope, $stateParams) {
            $scope.translation = $scope.appData.translation;
            $scope.zip = $stateParams.zip;
        }
    ]);
});