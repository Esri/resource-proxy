define([
    'onlineRateCalculator',
    'controller/startCalculationCtrl'
], function(app) {
    "use strict";
    
    return app.config(['$routeProvider',
        function($routeProvider) {
            var baseUrl = '/wordpress/wp-content/plugins/FpOnlineRateTable/app/';
            $routeProvider.
                    when('/start', {
                        templateUrl: baseUrl + 'partials/startCalculation.html',
                        controller: 'StartCalculationCtrl'
                    }).
                    otherwise({
                        redirectTo: '/start'
                    });
        }]);
});