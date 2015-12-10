define([
    '../../errorHandler/fpException.factory'
], function (module) {
    "use strict";

    module.factory('CalculationException', CalculationExceptionFactory);
    
    CalculationExceptionFactory.$inject = [
        'FpException'
    ];
          
    function CalculationExceptionFactory(FpException) {
        
        function CalculationException(
                message, code, subCode1, subCode2, fileName, lineNumber) {
            
            FpException.call(this, message, fileName, lineNumber);
            
            this.code = code;
            this.subCode1 = subCode1;
            this.subCode2 = subCode2;
        }
        CalculationException.prototype = Object.create(FpException.prototype);
        CalculationException.prototype.constructor = CalculationException;
        CalculationException.prototype.name = 'CalculationException';
    
        CalculationException.create = create;
        
        return CalculationException;
        
        ////////

        function create(message, code, subCode1, subCode2,
                fileName, lineNumber) {
            return new CalculationException(
                    message, code, subCode1, subCode2, fileName, lineNumber);
        }
    }

    return module;
});