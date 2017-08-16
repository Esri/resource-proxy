define([
    './fpException.factory'
], function (module) {
    "use strict";

    module.factory('NotImplementedException', NotImplementedExceptionFactory);
    
    NotImplementedExceptionFactory.$inject = [
        'FpException'
    ];
          
    function NotImplementedExceptionFactory(FpException) {
        
        function NotImplementedException(
                message, fileName, lineNumber) {
            FpException.call(this, message, fileName, lineNumber);
        }
        NotImplementedException.prototype
                = Object.create(FpException.prototype);
        NotImplementedException.prototype.constructor = NotImplementedException;
        NotImplementedException.prototype.name = 'NotImplementedException';
    
        NotImplementedException.create = create;
        
        return NotImplementedException;
        
        ////////

        function create(message, fileName, lineNumber) {
            return new NotImplementedException(
                    message, fileName, lineNumber);
        }
    }

    return module;
});