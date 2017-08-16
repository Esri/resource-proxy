define([
    './fpErrorHandler.module'
], function (module) {
    "use strict";

    module.factory('FpException', FpExceptionFactory);
    
    function FpExceptionFactory() {
    
        function FpException(message, fileName, lineNumber) {
            
            // Due to special rules for the Error constructor we cannot simply
            // do 'Error.call(this, arguments)'.
            var error = Error.apply(this, arguments);
            
            this.message = error.message;
            if(error.stack) {
                this.stack = error.stack;
            }
        }
        FpException.prototype = Object.create(Error.prototype);
        FpException.prototype.constructor = FpException;
        FpException.prototype.name = 'FpException';
    
        FpException.create = create;
          
        ////////

        function create(message, fileName, lineNumber) {
            return new FpException(message, fileName, lineNumber);
        }
        
        return FpException;
    }

    return module;
});