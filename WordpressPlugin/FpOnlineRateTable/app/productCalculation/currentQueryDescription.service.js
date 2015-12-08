define([
    './fpProductCalculation.module'
], function(module) {
    "use strict";
    
    
    module.factory('CurrentQueryDescription', CurrentQueryDescription);
    
    function CurrentQueryDescription() {
        
        var currentQuery;
        
        return {
            get: get,
            getType: getType,
            set: set
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
                    
            return currentQuery;
        }
    }
    
    return module;
});