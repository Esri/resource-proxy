define([
    'onlineRateCalculator',
    'services/appSettings',
    'services/translation'
], function(app) {
    "use strict";
    
    function splitAtDecimalPoint(weight, factor) {
        return {
            integral: Math.floor(weight/factor),
            decimal: Math.floor(weight%factor)
        };
    }

    function valueToUnitString(
            value, divider, leading, separator, trailing) {

        var parts = splitAtDecimalPoint(value, divider);
        return leading + parts.integral + separator + parts.decimal
            + trailing;
    }

    function fillInLeadingZeros(value, precision) {
        return (Array(precision-1).join('0')+value)
            .slice(-precision);
    }

    function valueToDecimalString(
            value, precision, leading, separator, trailing) {

        var divider = Math.pow(10, precision);
        var parts = splitAtDecimalPoint(value, divider);
        var decimal = fillInLeadingZeros(parts.decimal, precision);
        return leading + parts.integral + separator + decimal
            + trailing;
    }
    
    return app
        .filter('postageAsString', function() {
            return function(postage) {
                
                if(typeof postage !== 'object') {
                    return null;
                }

                return valueToDecimalString(
                        postage.PostageValue,
                        postage.PostageDecimals,
                        postage.CurrencySymbol,
                        postage.CurrencyDecimalSeparator,
                        '');
            };
        })
        .filter('weightAsString', [
            'AppSettings',
            'Translation',
            function(AppSettings, Translation) {
                
                var TenthGram = 0;
                var Gram = 1;
                var TenthOunce = 2;
                var HundrethOunce = 3;
                
                var KG_TO_GRAM = 1000;
                var POUND_TO_OUNCE = 16;
                var OUNCE_TO_GRAMS = 28.349523125;
                

                function getMetricWeightString(weightInGram) {
                    return valueToUnitString(
                            weightInGram, KG_TO_GRAM, '',
                            Translation.table().kg + ' ',
                            Translation.table().g);
                }

                function getUsWeightString(weightInOunces) {
                    return valueToUnitString(
                            weightInOunces, POUND_TO_OUNCE, '',
                            Translation.table().lb + ' ',
                            Translation.table().oz);
                }
                
                function weightToGrams(weight) {
                    
                    var value = weight.WeightValue;
                    var unit = weight.WeightUnit;
                    var grams;
                    
                    switch(unit) {
                        case Gram:
                            grams = value;
                            break;
                            
                        case TenthGram:
                            grams = value / 10;
                            break;
                            
                        case TenthOunce:
                            grams = value * OUNCE_TO_GRAMS / 10;
                            break;
                            
                        case HundrethOunce:
                            grams = value * OUNCE_TO_GRAMS / 100;
                            break;
                            
                        default:
                            grams = null;
                    }
                    
                    return grams;
                }
                
                function weightToOunces(weight) {
                    
                    var value = weight.WeightValue;
                    var unit = weight.WeightUnit;
                    var ounces;
                    
                    switch(unit) {
                        case Gram:
                            ounces = value / OUNCE_TO_GRAMS;
                            break;
                            
                        case TenthGram:
                            ounces = value / (10*OUNCE_TO_GRAMS);
                            break;
                            
                        case TenthOunce:
                            ounces = value / 10;
                            break;
                            
                        case HundrethOunce:
                            ounces = value / 100;
                            break;
                            
                        default:
                            ounces = null;
                    }
                    
                    return ounces;
                }

                return function(weight) {
                    
                    if((typeof weight !== 'object') 
                            || !AppSettings.isInitialized()) {
                        return null;
                    }

                    if(AppSettings.usesUsUnits()) {
                        var ounces = weightToOunces(weight);
                        return getUsWeightString(ounces);
                    } else {
                        var grams = weightToGrams(weight);
                        return getMetricWeightString(grams);
                    }
                };
        }]);
});