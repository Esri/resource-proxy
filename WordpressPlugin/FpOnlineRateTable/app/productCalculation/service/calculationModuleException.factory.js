define([
    '../../errorHandler/fpException.factory'
], function (module) {
    "use strict";

    module.factory('CalculationModuleException',
            CalculationModuleExceptionFactory);
    
    CalculationModuleExceptionFactory.$inject = [
        'FpException'
    ];
          
    function CalculationModuleExceptionFactory(FpException) {
        
        function CalculationModuleException(
                message, fileName, lineNumber) {
            
            FpException.call(this, message, fileName, lineNumber);
        }
        CalculationModuleException.prototype
                = Object.create(FpException.prototype);
        CalculationModuleException.prototype.constructor
                = CalculationModuleException;
        CalculationModuleException.prototype.name
                = 'CalculationModuleException';
    
        CalculationModuleException.create = create;
        
        return CalculationModuleException;
        
        ////////

        function create(message, fileName, lineNumber) {
            return new CalculationModuleException(
                    message, fileName, lineNumber);
        }
    }

    return module;
});