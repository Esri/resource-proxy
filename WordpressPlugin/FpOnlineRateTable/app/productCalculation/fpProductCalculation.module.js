define([
    'angular',
    'angular-ui-router',
    'anim-in-out',
    '../helper/fpHelper.module'
], function (angular) {
    "use strict";
 
    var module = angular.module('FP.ProductCalculation', [
        'ui.router',
        'ngAnimate',
        'anim-in-out',
        'FP.Helper'
    ]);
    
    return module;
});