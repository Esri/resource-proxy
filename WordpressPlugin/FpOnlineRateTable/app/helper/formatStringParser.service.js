define([
    'angular',
    './fpHelper.module'
], function(angular, module) {
    "use strict";
    
    // Some constant data we nedd.
    // Note: we are not using angular constants as they're of pure local
    // use in this file.
    var defaultFieldWidth = {
        u: 15,  // number of digits of 2^53 reduced by one so we get full decimal value range
        d: 16,  // number of digits of -2^53 reduced by one so we get full decimal value range
        x: 13,  // number of hex digits of 2^53 reduced by one so we get full hex value range
        X: 13,  // number of hex digits of 2^53 reduced by one so we get full hex value range
        f: 16   // number of significant digits of double incl. period. Note: we do not allow negative values!
    };
    var fieldRegex = {
        // we only we regex for the hex input
        x: '^[0-9a-f]{1,WIDTH}$',
        X: '^[0-9A-F]{1,WIDTH}$'
    };
    var defaultPrecision = 6;       // as is standard in C
    var splitRegex = /(%.*?[udxXf])/;
    var formatRegex = /^%((\d{0,2})(\.(\d{0,2}))?)([duxXf])$/;
    
    
    module.factory('FormatStringParser', FormatStringParser);
    
    FormatStringParser.$inject = [];
    
    function FormatStringParser() {
        
        return {
            parse: parse
        };
        
        /////////////
     
        /**
         * Parse a string also containing printf style format specifictations
         * into an array of strings and objects specifying input field
         * attributes representing the corresponding printf style.
         * @param {type} string The string to parse
         * @returns {Array}
         */
        function parse(string, attrsToAppend) {
            
            // Split format string into format tokens. I.e. the string is split
            // on each printf format string specification. The resulting array
            // includes the printf format strings as well as all strings before,
            // after and in between format strings.
            var tokens = string
                    .split(splitRegex)
                    .filter(function(e) {   // remove all empty strings from the result
                        if(e) {
                            return e;
                        }
                    });
            
            var result = tokens.map(function(token) {
                // Check if the token matches a printf format specification
                var matches = token.match(formatRegex);
                if(!matches) {
                    // if not just return the token as string
                    return token;
                }
                
                // extract the relevant information from the match
                var type = matches[5];
                var width = parseInt(matches[2]);
                var precision = parseInt(matches[4]);
                
                return inputFieldAttributes(
                        type, width, precision, attrsToAppend);
            });

            return result;
        }
   
        function createBaseAttributes(type, width, attrsToAppend) {
            
             return angular.extend({
                type: type,
                size: width,
                maxlength: width,
                required: true
            }, attrsToAppend);
        }
   
        function createUnsignedAttributes(width, attrsToAppend) {
            
            var result = createBaseAttributes('number', width, attrsToAppend);
            
            result.step = 1;
            result.max = Math.pow(10, width) - result.step;
            result.min = 0;
            
            return result;
        }
        
        function createSignedAttributes(width, attrsToAppend) {
            
            var result = createBaseAttributes('number', width, attrsToAppend);
            
            result.step = 1;
            result.max = Math.pow(10, width-1) - result.step;
            result.min = -result.max;
            
            return result;
        }
        
        function createHexAttributes(type, width, attrsToAppend) {
            
            var result = createBaseAttributes('text', width, attrsToAppend);
       
            result['ng-pattern'] = fieldRegex[type].replace('WIDTH', width);
            
            return result;
        }
        
        function createFloatAttributes(
                width, specifiedPrecision, attrsToAppend) {
            
            // Note: we have to add one to the width to include the period
            var result = createBaseAttributes('number', width+1, attrsToAppend);
            var precision = specifiedPrecision || defaultPrecision;
                            
            // compute number of digits (before period).
            var digits = width-precision;
            // we need at least one digit. If this is not the case adapt the
            // precision
            if(digits < 1) {
                digits = 1;             // one digit before period
                precision = width-digits;
            }
            
            result.step = Math.pow(10, -precision);
            result.max = Math.pow(10, digits) - result.step;
            result.min = 0;
            
            return result;
        }
   
        /**
         * Takes the result array by parse and transforms it into a new array
         * that conatains all information to render a corresponding input form.
         * @param {type} type 'd', 'u', 'x', 'X' or 'f'
         * @param {type} specifiedWidth
         * @param {type} specifiedPrecision
         * @param {type} attrsToAppend
         * @returns {object} containing the attributes for an input field
         */
        function inputFieldAttributes(
                type, specifiedWidth, specifiedPrecision, attrsToAppend) {
            
            var width = specifiedWidth || defaultFieldWidth[type];
                    
            switch(type) {
                case 'u':
                    return createUnsignedAttributes(width, attrsToAppend);
                        
                case 'd':
                    return createSignedAttributes(width, attrsToAppend);
                            
                case 'x':
                case 'X':
                    return createHexAttributes(type, width, attrsToAppend);
                        
                case 'f':
                    return createFloatAttributes(
                            width, specifiedPrecision, attrsToAppend);
                        
                default:
                    // we should never get here. If we do - ignore.
                    break;
            }
        }
    }
    
    return module;
});