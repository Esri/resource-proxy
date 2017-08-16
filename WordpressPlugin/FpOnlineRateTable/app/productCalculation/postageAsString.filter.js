define([
    './fpProductCalculation.module',
    './numberFormatting.service',
    './weightHelper.service'
], function(module) {
    "use strict";
    
    return module.filter('postageAsString', postageAsString);
    
    postageAsString.$inject = [
        'NumberFormatting'
    ];
    
    function postageAsString(NumberFormatting) {
        
        return function(postage) {

            if(typeof postage !== 'object') {
                return null;
            }

            return NumberFormatting.formatAsDecimalString(
                    postage.PostageValue,
                    postage.PostageDecimals,
                    postage.CurrencySymbol,
                    postage.CurrencyDecimalSeparator,
                    '');
        };
    }
    
    return module;
});