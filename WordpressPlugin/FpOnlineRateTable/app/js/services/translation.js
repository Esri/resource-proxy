define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    app.factory('Translation', function() {
        var translation = null;
        
        return {
            isInitialized: function() {
                return !!translation;
            },
            
            init: function(tr) {
                translation = tr;
            },
            
            table: function() {
                return translation;
            }
        };
    });
    
    return app;
});