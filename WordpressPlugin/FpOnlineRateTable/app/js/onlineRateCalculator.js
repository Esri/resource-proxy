define([
    'angular',
    'angular-route',
    'errorHandler'
], function (angular) {
    "use strict";
 
    return angular.module(
            'OnlineRateCalculator', [
                'ngRoute',
                'ErrorHandler'
            ]);
});