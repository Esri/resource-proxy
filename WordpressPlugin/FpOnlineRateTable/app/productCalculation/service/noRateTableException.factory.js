define([
    '../../errorHandler/fpException.factory'
], function (module) {
    "use strict";

    module.factory('NoRateTableException', NoRateTableExceptionFactory);
    
    NoRateTableExceptionFactory.$inject = [
        'FpException'
    ];
          
    function NoRateTableExceptionFactory(FpException) {
        
        function NoRateTableException(
                message, culture, date, fileName, lineNumber) {
            
            FpException.call(this, message, fileName, lineNumber);
            
            this.culture = culture;
            this.date = date;
        }
        NoRateTableException.prototype = Object.create(FpException.prototype);
        NoRateTableException.prototype.constructor = NoRateTableException;
        NoRateTableException.prototype.name = 'NoRateTableException';
    
        NoRateTableException.create = create;
        
        return NoRateTableException;
        
        ////////

        function create(message, culture, date, fileName, lineNumber) {
            return new NoRateTableException(
                    message, culture, date, fileName, lineNumber);
        }
    }

    return module;
});