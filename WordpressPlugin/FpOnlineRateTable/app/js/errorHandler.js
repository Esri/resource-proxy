define([
    'angular',
    'angular-growl'
], function (angular) {
    "use strict";

    return angular.module('ErrorHandler', ['angular-growl']).
            decorator('$exceptionHandler', function ($delegate, $injector) { //, $window) {
                return function (exception, cause) {
                    var msg;
                    if(exception instanceof Error) {
                        msg = exception.message;
                    } else {
                        msg = exception.toString();
                    }
                    $injector.get('growl').error(msg);
                    // Using injector to get around cyclic dependencies
                    //$injector.get('$rootScope').$broadcast('ErrorHandler.error', SCRIPT_ERROR_MSG);
                    // Bypassing angular's http abstraction to avoid infinite exception loops
                    /*$injector.get('$httpBackend')('POST', LOGGING_URL, angular.toJson({
                        message: exception.stack || exception.message || exception || '',
                        source: cause || '',
                        url: $window.location.href
                    }), angular.noop, {'content-type': 'application/json'});*/
                    $delegate(exception, cause);
                };
            });

});