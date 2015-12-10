define([
    'angular-resource'
], function() {
    var service = angular.module('OnlineRateTableService', [
        'ngResource'
    ]);
    service.factory('ActiveTables', ['$resource',
        function($resource) {
            return $resource('http://localhost/wordpress/wp-content/plugins/FpOnlineRateTable/3rdParty/lib/resource-proxy/proxy.php?http://localhost:33115/api/RateCalculation/GetActiveTables')
        }
    ]);
});