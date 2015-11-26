define([
    'onlineRateCalculator'
], function(app) {
    "use strict";

    app.factory('NumberFormatting',
        function() {
        
            return {
                formatAsUnitString: formatAsUnitString,
                formatAsDecimalString: formatAsDecimalString
            };


            /**
             * Converts a value to a 'bigger' unit (e.g. 1853g to 1.853kg).
             * @param float value The original value
             * @param integer factor The factor between the original unit and
             *  the 'bigger' unit.
             * @returns object An object containing the integral part and the
             *  decimal part of the converted value in terms of integral values.
             */
            function unitTransform(value, factor) {
                return {
                    integral: Math.floor(value/factor),
                    decimal: Math.floor(value%factor)
                };
            }
            
            /**
             * Formats a value according to a unit and a 'sub-unit'.
             * E.g.: '1kg 53g'.
             * @param float value The value to be formatted
             * @param integer divider the factor between the main unit and the
             *  sub unit (1000 in the above example)
             * @param string leading A string to place before the formatted
             *  number (empty in the above example)
             * @param {type} separator A string to be placed between the two
             *  numbers ('kg ' in the above example)
             * @param {type} trailing A string to be placed after the formatted
             *  numbers ('g' in the example above).
             * @returns string The formatted number string
             */
            function formatAsUnitString(
                    value, divider, leading, separator, trailing) {

                var parts = unitTransform(value, divider);
                return leading + parts.integral + separator + parts.decimal
                    + trailing;
            }

            /**
             * Adds leading zeros to a number
             * @param integer value The number to be padded
             * @param integer precision The number of digits the padded number
             *  should consits of
             * @returns string The padded number
             */
            function padWithZeros(value, digits) {
                return (Array(digits-1).join('0')+value)
                    .slice(-digits);
            }

            /**
             * Formats a value as decimal digits.
             * E.g.: 1853 (cents) as '€18,53'.
             * @param float value The value to be formatted in terms of the
             *  'smaller' unit (cents in the above example).
             * @param integer precision The number of decimal digits
             *  sub unit (2 in the above example).
             * @param string leading A string to place before the formatted
             *  number ('€' in the above example).
             * @param string separator A string to be placed between the two
             *  numbers (',' in the above example).
             * @param string trailing A string to be placed after the formatted
             *  numbers (empty in the above example).
             * @returns string The formatted number string
             */
            function formatAsDecimalString(
                    value, precision, leading, separator, trailing) {

                var divider = Math.pow(10, precision);
                var parts = unitTransform(value, divider);
                var decimal = padWithZeros(parts.decimal, precision);
                return leading + parts.integral + separator + decimal
                    + trailing;
            }
    });
    
    return app;
});