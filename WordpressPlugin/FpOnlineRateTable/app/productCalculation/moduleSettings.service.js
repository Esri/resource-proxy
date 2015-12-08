define([
    './fpProductCalculation.module'
], function(module) {
    "use strict";
    
    module.factory('ModuleSettings', ModuleSettings);
            
    function ModuleSettings() {
        
        var appSettings = null;
        
        return {
            isInitialized: isInitialized,
            init: init,
            usesUsUnits: usesUsUnits,
            usesMetricunits: usesMetricunits,
            culture: culture,
            countryCode: countryCode,
            carrierId: carrierId,
            zipRegex: zipRegex,
            rateCalculationUrl: rateCalculationUrl,
            maxWeight: maxWeight
        };
        
        function isInitialized() {
            return !!appSettings;
        }
            
        function init(settings) {
            
            appSettings = settings;
                
            // in case we get here a second time (e.g. the user pressed
            // 'back' the zipRegex was already transformed into a RegExp
            // object. 
            if(!(settings.zipRegex instanceof RegExp)) {
                // transform PHP regex string into Javascript Regex Object
                var regexWithoutSlashes
                        = settings.zipRegex.replace(/^\/|\/$/g, '');
                appSettings.zipRegex = new RegExp(regexWithoutSlashes);
            }
                
            if('US' === settings.culture.slice(-2)) {
                appSettings.usesUsUnits = true;
                appSettings.usesMetricUnits = false;
            } else {
                appSettings.usesUsUnits = false;
                appSettings.usesMetricUnits = true;
            }
        }
            
        function usesUsUnits() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.usesUsUnits;
            }
        }
            
        function usesMetricunits() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.usesMetricUnits;
            }
        }
            
        function culture() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.culture;
            }
        }
            
        function countryCode() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.countryCode;
            }
        }
            
        function carrierId() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.carrierId;
            }
        }
            
        function zipRegex() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.zipRegex;
            }
        }
            
        function rateCalculationUrl() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.rateCalculationUrl;
            }
        }
        
        function maxWeight() {
            
            if(angular.isObject(appSettings)) {
                return appSettings.maxWeight;
            }
        }
    }
    
    return module;
});