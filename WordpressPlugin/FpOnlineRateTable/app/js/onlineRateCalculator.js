define([
    'angular',
    'angular-ui-router',
    'angular-resource',
    'errorHandler'
], function (angular) {
    "use strict";
 
    var app = angular.module('OnlineRateCalculator', [
        'ui.router',
        'ngResource',
        'ErrorHandler'
    ]);
    
    return app;
});