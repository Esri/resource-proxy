define([
    'onlineRateCalculator',
    'services/weight'
], function(app) {
    "use strict";
    
    app.constant('EProductDescriptionState', {
        Complete: 0,
        Incomplete: 1
    });
    
    app.factory('CurrentProductDescription', CurrentProductDescription);
    
    CurrentProductDescription.$inject = [
        '$rootScope',
        'Weight',
        'EProductDescriptionState'];
    
    function CurrentProductDescription(
            $rootScope, Weight, EProductDescriptionState) {
        
        var productDescriptionStack = [];
        var currentProductDescription;
        
        return {
            get: get,
            set: set,
            restoreLast: restoreLast, 
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
            productDescriptionStack.push(value);
            notifyListeners();
                    
            return currentProductDescription;
        }
        
        function restoreLast() {
            
            var last = productDescriptionStack.pop();
            if(last) {
                currentProductDescription = last;
                notifyListeners();
            }
            
            return currentProductDescription;
        }
        
        function updateWeight(weightValue) {
            
            currentProductDescription.Weight
                    = Weight.getWeightInfo(weightValue);
            notifyListeners();
            
            return currentProductDescription;
        }
        
        function isComplete() {
            return (currentProductDescription.State
                    === EProductDescriptionState.Complete);
        }
        
        function hasHistory() {
            return (currentProductDescription.ReadyModeSelection.length !== 0);
        }
        
        function notifyListeners() {
            $rootScope.$emit('currentProductDescriptionChanged');
        }
    }
    
    return app;
});