define([
    'onlineRateCalculator',
    'services/weight'
], function(app) {
    "use strict";
    
    var eProductDescriptionState = {
        Complete: 0,
        Incomplete: 1
    };
    
    
    app.factory('CurrentProductDescription', CurrentProductDescription);
    
    CurrentProductDescription.$inject = [
        '$rootScope',
        'Weight'];
    
    function CurrentProductDescription($rootScope, Weight) {
        
        return {
            get: get,
            set: set,
            updateWeight: updateWeight,
            isComplete: isComplete,
            hasHistory: hasHistory
        };
        
        /////////////
        
        var productDescription;
        
        function get() {
            return productDescription;
        }
        
        function set(value) {
            
            productDescription = value;
            notifyListeners();
                    
            return productDescription;
        }
        
        function updateWeight(weightValue) {
            
            productDescription.Weight = Weight.getWeightInfo(weightValue);
            notifyListeners();
            
            return productDescription;
        }
        
        function isComplete() {
            return (productDescription.State
                    === eProductDescriptionState.Complete);
        }
        
        function hasHistory() {
            return (productDescription.ReadyModeSelection.length !== 0);
        }
        
        function notifyListeners() {
            $rootScope.$emit('currentProductDescriptionChanged');
        }
    }
    
    return app;
});