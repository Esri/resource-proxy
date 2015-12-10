define([
    'angular',
    'angular-ui-router',
    '../helper/fpHelper.module'
], function (angular) {
    "use strict";
 
    var module = angular.module('FP.ProductCalculation', [
        'ui.router',
        'FP.Helper'
    ]);
    
    return module;
});