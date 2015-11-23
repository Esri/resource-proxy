define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    app.factory('AppSettings', function() {
        var appSettings = null;
        
        return {
            isInitialized: function() {
                return !!appSettings;
            },
            
            init: function(settings) {
                appSettings = settings;
                
                // transform PHP regex string into Javascript Regex Object
                var regexWithoutSlashes
                        = settings.zipRegex.replace(/^\/|\/$/g, '');
                appSettings.zipRegex = new RegExp(regexWithoutSlashes);
                
                if('US' === settings.culture.slice(-2)) {
                    appSettings.usesUsUnits = true;
                    appSettings.usesMetricUnits = false;
                } else {
                    appSettings.usesUsUnits = false;
                    appSettings.usesMetricUnits = true;
                }
            },
            
            usesUsUnits: function() {
                return appSettings.usesUsUnits;
            },
            
            usesMetricunits: function() {
                return appSettings.usesMetricUnits;
            },
            
            culture: function() {
                return appSettings.culture;
            },
            
            countryCode: function() {
                return appSettings.countryCode;
            },
            
            carrierId: function() {
                return appSettings.carrierId;
            },
            
            zipRegex: function() {
                return appSettings.zipRegex;
            },
            
            rateCalculationUrl: function() {
                return appSettings.rateCalculationUrl;
            }
        };
    });
    
    return app;
});