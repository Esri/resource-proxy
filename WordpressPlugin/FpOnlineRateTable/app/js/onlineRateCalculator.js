define([
    'angular',
    'angular-ui-router',
    'errorHandler',
    'services/onlineRateTableServices',
], function (angular) {
    "use strict";
 
    var app = angular.module('OnlineRateCalculator', [
        'ui.router',
        'ErrorHandler',
        'OnlineRateTableServices'
    ]);
    
    return app;
});