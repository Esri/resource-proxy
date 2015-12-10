define([
    './fpException.factory'
], function (module) {
    "use strict";

    module.factory('UnknownErrorException', UnknownErrorExceptionFactory);
    
    UnknownErrorExceptionFactory.$inject = [
        'FpException'
    ];
          
    function UnknownErrorExceptionFactory(FpException) {
        
        function UnknownErrorException(
                message, fileName, lineNumber) {
            FpException.call(this, message, fileName, lineNumber);
        }
        UnknownErrorException.prototype
                = Object.create(FpException.prototype);
        UnknownErrorException.prototype.constructor = UnknownErrorException;
        UnknownErrorException.prototype.name = 'UnknownErrorException';
    
        UnknownErrorException.create = create;
        
        return UnknownErrorException;
        
        ////////

        function create(message, fileName, lineNumber) {
            return new UnknownErrorException(message, fileName, lineNumber);
        }
    }

    return module;
});