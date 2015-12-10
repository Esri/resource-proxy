define([
    'angular',
    'errorHandler/errorHandler.service',
    'productCalculation/routing.config'
], function (angular) {
    "use strict";
 
    var app = angular.module('FP.OnlineRateCalculator', [
        'FP.ErrorHandler',
        'FP.ProductCalculation'
    ]);
    
    return app;
});