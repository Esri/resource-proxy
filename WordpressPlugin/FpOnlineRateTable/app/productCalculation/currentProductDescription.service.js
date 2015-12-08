define([
    './fpProductCalculation.module',
    './weightHelper.service'
], function(module) {
    "use strict";
    
    var eProductDescriptionState = {
        Complete: 0,
        Incomplete: 1 };
    
    
    module.factory('CurrentProductDescription', CurrentProductDescription);
    
    CurrentProductDescription.$inject = [
        '$rootScope',
        'WeightHelper'];
    
    function CurrentProductDescription($rootScope, WeightHelper) {
        
        var currentProductDescription;
        
        return {
            get: get,
            set: set,
            updateWeight: updateWeight,
            isComplete: isComplete,
            hasHistory: hasHistory
        };
        
        /////////////
        
        function get() {
            return currentProductDescription;
        }
        
        function set(value) {
            
            currentProductDescription = value;
            notifyListeners();
                    
            return currentProductDescription;
        }
        
        function updateWeight(weightValue) {
            
            currentProductDescription.Weight
                    = WeightHelper.getWeightInfo(weightValue);
            notifyListeners();
            
            return currentProductDescription;
        }
        
        function isComplete() {
            return (currentProductDescription.State
                    === eProductDescriptionState.Complete);
        }
        
        function hasHistory() {
            return (currentProductDescription.ReadyModeSelection.length !== 0);
        }
        
        function notifyListeners() {
            $rootScope.$emit('currentProductDescriptionChanged');
        }
    }
    
    return module;
});