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
                if(angular.isObject(appSettings)) {
                    return appSettings.usesUsUnits;
                }
            },
            
            usesMetricunits: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.usesMetricUnits;
                }
            },
            
            culture: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.culture;
                }
            },
            
            countryCode: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.countryCode;
                }
            },
            
            carrierId: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.carrierId;
                }
            },
            
            zipRegex: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.zipRegex;
                }
            },
            
            rateCalculationUrl: function() {
                if(angular.isObject(appSettings)) {
                    return appSettings.rateCalculationUrl;
                }
            }
        };
    });
    
    return app;
});