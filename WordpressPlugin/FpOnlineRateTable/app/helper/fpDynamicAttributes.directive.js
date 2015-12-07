define([
    './fpHelper.module'
], function(module) {
    "use strict";
    
    module.directive('fpDynamicAttributes', fpDynamicAttributes);
   
    fpDynamicAttributes.$inject = [
        '$parse'
    ];

    function fpDynamicAttributes($parse) {
        
        return {
            restrict: 'A',  // This dirctive is meant to be used as attribute
            link: link
        };
        
        ////////
        
        function link(scope, iElement, iAttrs) {
            
            // This is a 'magic' call to get the attribute we intend to set on
            // the element.
            // Note: we call $parse on the fp-dynamic-attributes value to
            // resolve any angulat expressions and pass the scope to the
            // resulting function.
            var attrsToSet = $parse(iAttrs.fpDynamicAttributes)(scope);
            
            // delete property '$$hashkey' created by '$parse' so we can safely
            // iterate over all properties.
            delete attrsToSet.$$hashKey;
            
            // Set the attributes
            angular.forEach(attrsToSet, function(value, name) {
                iAttrs.$set(name, value);
            });
        }
    }
    
    return module;
});