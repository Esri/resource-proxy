define([
    './fpProductCalculation.module'
], function(module) {
    "use strict";
    
    module.factory('Translation', Translation);
    
    function Translation() {
        
        var translation = null;
        
        return {
            isInitialized: isInitialized,
            init: init,
            table: table
        };
        
        function isInitialized() {
            return !!translation;
        }
            
        function init(tr) {
            translation = tr;
        }
            
        function table() {
            return translation;
        }
    }
    
    return module;
});