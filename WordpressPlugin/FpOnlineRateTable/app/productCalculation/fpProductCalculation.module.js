define([
    'angular',
    'angular-ui-router',
    '../helper/fpHelper.module'
], function (angular) {
    "use strict";
 
    return angular.module('FP.ProductCalculation', [
        'ui.router',
        'FP.Helper'
    ]);
});