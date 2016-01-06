define([
    '../../errorHandler/fpException.factory'
], function (module) {
    "use strict";

    module.factory('ServiceException', ServiceExceptionFactory);
    
    ServiceExceptionFactory.$inject = [
        'FpException'
    ];
          
    function ServiceExceptionFactory(FpException) {
        
        function ServiceException(
                message, code, details, fileName, lineNumber) {
            
            FpException.call(this, message, fileName, lineNumber);
            
            this.code = code;
            this.details = details;
        }
        ServiceException.prototype = Object.create(FpException.prototype);
        ServiceException.prototype.constructor = ServiceException;
        ServiceException.prototype.name = 'ServiceException';
    
        ServiceException.create = create;
        
        return ServiceException;
        
        ////////

        function create(message, code, details, fileName, lineNumber) {
            return new ServiceException(
                    message, code, details, fileName, lineNumber);
        }
    }

    return module;
});