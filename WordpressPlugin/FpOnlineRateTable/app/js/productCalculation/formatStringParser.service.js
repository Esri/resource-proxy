define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    // Some constant data we nedd.
    // Note: we are not using angular constants as they're of pure local
    // use in this file.
    var defaultFieldWidth = {
        u: 16,  // number of digits of 2^53-1
        d: 17,  // number of digits of -2^53-1
        x: 14,  // number of hex digits of 2^53-1
        X: 14,  // number of hex digits of 2^53-1
        f: 17   // number of significant digits of double incl. sign and period
    };
    var inputType = {
        u: 'number',
        d: 'number',
        x: 'text',
        X: 'text',
        f: 'number'
    };
    // Note: DIGITS = WIDTH
    var fieldRegex = {
        u: '^\d{1,DIGITS}$',
        d: '^[+-]?\d{1,DIGITS}$',                   // Exception: DIGITS = WIDTH - 1
        x: '^[0-9a-f]{1,DIGITS}$',
        X: '^[0-9A-F]{1,DIGITS}$',
        f: '^[+-]?\d{1,DIGITS}(.\d{PRECISION})?$'   // Exception: DIGITS = WIDTH - PRECISION - 2
    };
    var defaultPrecision = 6;                       // as is standard in C
    var splitRegex = /(%.*?[udxXf])/;
    var formatRegex = /%((\d{0,2})(\.(\d{0,2}))?)([duxXf])/;
    
    app.factory('FormatStringParser', FormatStringParser);
    
    FormatStringParser.$inject = [];
    
    function FormatStringParser() {
        
        return {
        };
        
        /////////////
     
        /**
         * Parse a string containing printf style format specifictations into
         * an array containing field descriptions and strings in between fields.
         * @param {string} string
         * @returns {Array}
         */
        function parse(string) {
            
            // Split format string into format tokens. I.e. the string is split
            // on each printf format string specification. The resulting array
            // includes the printf format strings as well as all strings before,
            // after and in between format strings.
            var tokens = string.split(splitRegex);
            
            // The result of this function will be an array of objects. Each of
            // these objects bundles a format string (aka field) with it's
            // leading non-format string. Any one of both field can be zero.
            var result = [{field: null, leading: null}];
            
            // This is the array index into the result array.
            var fieldNo = 0;
            
            // For all tokens
            for(var i = 0; i < tokens.length; ++i) {
                
                // Check if the token matches a printf format specification
                var token = tokens[i];
                var matches = token.match(formatRegex);
                if(matches) {
                    // If it does match, extract and store type ('u', 'd', 'x',
                    // 'X' or 'f'), field width and field precision (aka number
                    // of decimals). The width and precision may be undefined if
                    // not specified.
                    result[fieldNo].field = {
                        type: matches[5],
                        width: matches[2],
                        precision: matches[4] };
                    
                    // As we've found a format string proceed to the next array
                    // element of the resulting array and create a dummy entry.
                    ++fieldNo;
                    result[fieldNo] = {field: null, leading: null};
                } else {
                    // If it does not match, just store the token in the
                    // 'leading' field.
                    result[fieldNo].leading = token;
                }
            }
            
            return result;
        }
        
        /**
         * Takes the result array by parse and transforms it into a new array
         * that conatains all information to render a corresponding input form.
         * @param {Array} descr
         * @returns {Array}
         */
        function interpretFormatDescription(descr) {
            
            var result = [];
            
            for(var i = 0; i < descr.length; ++i) {
                result[i] = {};
                var field = descr[i].field;
                if(field) {
                    var type = field.type;
                    var width = field.width || defaultFieldWidth[type];
                    var digits = width;
                    var precision = field.precision || defaultPrecision;
                    
                    switch(field.type) {
                        case 'd':
                            digits -= 1;
                            break;
                        
                        case 'f':
                            digits -= precision+2;
                            break;
                        
                        default:
                            // we should never get here. If we do - ignore.
                            break;
                    }
                    
                    result[i].type = inputType[type];
                    result[i].width = width;
                    var regexStr = fieldRegex
                            .replace('DIGITS', digits)
                            .replace('PRECISION', precision);
                    result[i].regex = new RegExp(regexStr);
                }
            }
        }
    }
    
    return app;
});