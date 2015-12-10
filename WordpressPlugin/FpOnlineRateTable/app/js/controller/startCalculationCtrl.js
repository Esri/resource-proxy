define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    app.controller('StartCalculationCtrl', [
        '$scope',
        '$http',
        function($scope, $http) {
            // we need the current date (in en-US locale) as service argument.
            var currentDate = new Date().toLocaleDateString('en-US');
            var serviceUrl = $scope.config['service-url'];
            var withArgs = serviceUrl + '?clientUtcDate=' + currentDate;
            $http.get(withArgs).then(
                    function success(response) {
                        // Warning:
                        // If the proxy service is reachable but not the original
                        // service we will receive success with an empty data
                        // field.
                        if(response.data instanceof Array) {
                            $scope.choices = response.data.map(function(item) {
                                return { id: item.Id, label: item.Variant };
                            });
                            $scope.selectedRateTable = $scope.choices[0];
                        } else {
                            alert('Error');
                        }
                    },
                    function error(response) {
                        alert('Error');
                    });
        }
    ]);
});