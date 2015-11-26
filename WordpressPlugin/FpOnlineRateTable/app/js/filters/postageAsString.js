define([
    'onlineRateCalculator',
    'services/numberFormatting',
    'services/weight'
], function(app) {
    "use strict";
    
    return app
        .filter('postageAsString', [
            'NumberFormatting',
            function(NumberFormatting) {
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
        }]);
});