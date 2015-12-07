define([
    './fpProductCalculation.module'
], function(module) {
    "use strict";
    
    
    module.factory('CurrentQueryDescription', CurrentQueryDescription);
    
    function CurrentQueryDescription() {
        
        var queryStack = [];
        var currentQuery;
        
        return {
            get: get,
            getType: getType,
            set: set,
            restoreLast: restoreLast
        };
        
        /////////////
        
        function get() {
            return currentQuery.query;
        }
        
        function getType() {
            return currentQuery.type;
        }
        
        function set(type, query) {
            
            currentQuery = {
                type: type,
                query: query
            };
            queryStack.push(currentQuery);
                    
            return currentQuery;
        }
        
        function restoreLast() {
            
            var last = queryStack.pop();
            if(last) {
                currentQuery = last;
            }
            
            return currentQuery;
        }
    }
    
    return module;
});