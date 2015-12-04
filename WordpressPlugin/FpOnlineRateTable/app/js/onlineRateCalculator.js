define([
    'angular',
    'angular-ui-router',
    'angular-resource',
    'errorHandler',
    'helper/fpHelper.module'
], function (angular) {
    "use strict";
 
    var app = angular.module('OnlineRateCalculator', [
        'ui.router',
        'ngResource',
        'ErrorHandler',
        'FP.Helper'
    ]);
    
    return app;
});