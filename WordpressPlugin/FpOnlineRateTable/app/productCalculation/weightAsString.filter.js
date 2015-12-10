define([
    './fpProductCalculation.module',
    './numberFormatting.service',
    './weightHelper.service'
], function(module) {
    "use strict";
    
    module.filter('weightAsString', weightAsString);
    
    weightAsString.$inject = [
        'WeightHelper'
    ];
    
    function weightAsString(WeightHelper) {

        return converter;

        function converter(weightInfo) {
            return WeightHelper.getLocalizedWeightString(weightInfo);
        }
    }
    
    return module;
});