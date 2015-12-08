define([
    '../../fpProductCalculation.module'
], function(app) {
    "use strict";
    
    return app.controller(
            'FinishController', FinishController);
        
    
    FinishController.$inject = [
        '$scope'
    ];
    
    function FinishController($scope) {
        
        $scope.$parent.calc.showBack = false;
        $scope.$parent.calc.showFinish = false;
    }
    
    return app;
});