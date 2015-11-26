define([
    'onlineRateCalculator',
    'services/numberFormatting',
    'services/weight'
], function(app) {
    "use strict";
    
    return app
        .filter('weightAsString', [
            'Weight',
            function(Weight) {

                return converter;

                function converter(weightInfo) {
                    return Weight.getLocalizedWeightString(weightInfo);
                };
        }]);
});