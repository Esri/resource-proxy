define([
    'onlineRateCalculator',
    'services/appSettings',
    'services/translation',
    'services/numberFormatting'
], function(app) {
    "use strict";
    
    var eWeightUnit = {
        TenthGram: 0,
        Gram: 1,
        TenthOunce: 2,
        HundrethOunce: 3
    };
                
    var KG_TO_GRAM = 1000;
    var POUND_TO_OUNCE = 16;
    var OUNCE_TO_GRAMS = 28.349523125;
    
    
    app.factory('Weight', [
        'AppSettings',
        'Translation',
        'NumberFormatting',
        function(AppSettings, Translation, NumberFormatting) {
        
            return {
                getWeightInfo: getWeightInfo,
                getWeightInGram: getWeightInGram,
                getWeightInOunce: getWeightInOunce,
                getMetricWeightString: getMetricWeightString,
                getUsWeightString: getUsWeightString,
                getLocalizedWeightString: getLocalizedWeightString,
                getLocalizedMajorUnit: getLocalizedMajorUnit,
                getLocalizedMinorUnit: getLocalizedMinorUnit
            };
        
            /**
             * Creates a WeightInfo object from a weight value.
             * @param integer weightValue The weight value as entered by the user.
             *  The unit is either grams or ounces depending on the current
             *  rate table culture.
             * @returns object Object that mimics a C# WeightInfo object.
             */
            function getWeightInfo(weightValue) {
                
                var weightUnit;
                if(AppSettings.usesUsUnits()) {
                    weightUnit = eWeightUnit.TenthOunce;
                } else {
                    weightUnit = eWeightUnit.TenthGram;
                }
                
                weightValue *= 10;  // the weight value is sepcified
                var weightInGram = getWeightInGram(weightValue, weightUnit);
                var weightInOunces = getWeightInOunce(weightValue, weightUnit);
                
                return {
                    WeightValue: weightValue,
                    WeightUnit: weightUnit,
                    WeightInGram: weightInGram,
                    WeightInOunces: weightInOunces,
                    FormattedWeight: getMetricWeightString(weightInGram),
                    FormattedWeightImperial: getUsWeightString(weightInOunces)
                };
            }
            
            function getWeightInGram(value, unit) {
                 
                switch(unit) {
                    case eWeightUnit.Gram:
                        return value;

                    case eWeightUnit.TenthGram:
                        return value / 10;

                    case eWeightUnit.TenthOunce:
                        return value * OUNCE_TO_GRAMS / 10;

                    case eWeightUnit.HundrethOunce:
                        return value * OUNCE_TO_GRAMS / 100;

                    default:
                        return null;
                }
            }
            
            function getWeightInOunce(value, unit) {
                    
                switch(unit) {
                    case eWeightUnit.Gram:
                        return value / OUNCE_TO_GRAMS;

                    case eWeightUnit.TenthGram:
                        return value / (10*OUNCE_TO_GRAMS);

                    case eWeightUnit.TenthOunce:
                        return value / 10;

                    case eWeightUnit.HundrethOunce:
                        return value / 100;

                    default:
                        return null;
                }
            }
            
            function getMetricWeightString(weightInGram) {
                return NumberFormatting.formatAsUnitString(
                        weightInGram,
                        KG_TO_GRAM,
                        '',
                        Translation.table().kg + ' ',
                        Translation.table().g);
            }

            function getUsWeightString(weightInOunces) {
                return NumberFormatting.formatAsUnitString(
                        weightInOunces,
                        POUND_TO_OUNCE,
                        '',
                        Translation.table().lb + ' ',
                        Translation.table().oz);
            }
            
            /**
             * Converts the given WeightInfo object into a localized weight
             * string.
             * @param object weightInfo An object mimicking the WeightInfo class
             * @returns string the converted string
             */
            function getLocalizedWeightString(weightInfo) {
                
                if(typeof weightInfo !== 'object') {
                    return null;
                }
                
                var weight = weightInfo.WeightValue;
                var unit = weightInfo.WeightUnit;
                
                if(AppSettings.usesUsUnits()) {
                    var ounces = getWeightInOunce(weight, unit);
                    return getUsWeightString(ounces);
                } else {
                    var grams = getWeightInGram(weight, unit);
                    return getMetricWeightString(grams);
                }
            }
            
            function getLocalizedMajorUnit() {
                
                if(AppSettings.usesUsUnits()) {
                    return Translation.table().lb;
                } else {
                    return Translation.table().kg;
                }
            }
            
            function getLocalizedMinorUnit() {
                
                if(AppSettings.usesUsUnits()) {
                    return Translation.table().oz;
                } else {
                    return Translation.table().g;
                }
            }
    }]);
    
    return app;
});