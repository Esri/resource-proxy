define([
    'angular',
    '../fpProductCalculation.module',
    '../moduleSettings.service',
    '../weightHelper.service',
    '../queryDispatcher.service',
    '../currentProductDescription.service',
    './rateCalculationService.service',
    './actionResult.service',
    './serviceException.factory',
    './calculationException.factory',
    './calculationModuleException.factory',
    './noRateTableException.factory',
    '../../errorHandler/unknownErrorException.factory'
], function(angular, module) {
    "use strict";
    
    module.factory(
            'RateCalculationServiceFrontend', RateCalculationServiceFrontend);
    
    RateCalculationServiceFrontend.$inject = [
        'RateCalculationService',
        'CurrentProductDescription',
        'ActionResult',
        'QueryDispatcher',
        'WeightHelper',
        'ModuleSettings',
        'ServiceException',
        'CalculationException',
        'CalculationModuleException',
        'NoRateTableException',
        'UnknownErrorException'];
    
    
    function RateCalculationServiceFrontend(RateCalculationService,
            CurrentProductDescription, ActionResult, QueryDispatcher,
            WeightHelper, ModuleSettings, ServiceException,
            CalculationException, CalculationModuleException,
            NoRateTableException, UnknownErrorException) {
        
        return {
            getActiveTables: getActiveTables,
            start: start,
            back: back,
            updateWeight: updateWeight,
            selectMenuIndex: selectMenuIndex,
            selectIndex: selectIndex,
            selectValue: selectValue,
            requestString: requestString,
            requestValue: requestValue,
            requestPostage: requestPostage,
            acknoledge: acknoledge
        };
        
        //////////
        
        var environment;
        var rateTableDate;
        
        function createEnvironment(zipCode) {
            return {
                Iso3CountryCode: ModuleSettings.countryCode(),
                CarrierId: ModuleSettings.carrierId(),
                UtcDate: new Date(),
                Culture: ModuleSettings.culture(),
                SenderZipCode: zipCode
            };
        }
        
        function createWeightInfo(weightValue) {
            
            if(!weightValue) {
                weightValue = 1; // gram
            }
            
            return WeightHelper.getWeightInfo(weightValue);
        }
        
        function calculate(actionResult) {
            
            var productDescription = CurrentProductDescription.get();
            var promise = RateCalculationService.calculate(
                    productDescription, actionResult, environment);
            return handleResponse(promise);
        }
        
        function getActiveTables() {
            
            rateTableDate = new Date();
            var promise = RateCalculationService.getActiveTables(
                    rateTableDate.toISOString());
            return promise.then(handleRateTables, handleFailure);
        }
        
        function start(zipCode, weightValue) {

            environment = createEnvironment(zipCode);
            var weightInfo = createWeightInfo(weightValue);
            var promise = RateCalculationService.start(weightInfo, environment);
            return handleResponse(promise);
        }
        
        function selectMenuIndex(index) {
            
            var actionResult = ActionResult.selectMenuIndex(index);
            return calculate(actionResult);
        }
        
        function selectIndex(index) {
            
            var actionResult = ActionResult.selectIndex(index);
            return calculate(actionResult);
        }
        
        function selectValue(value) {
            
            var actionResult = ActionResult.selectValue(value);
            return calculate(actionResult);
        }
        
        function requestString(value) {

            var actionResult = ActionResult.requestString(value);
            return calculate(actionResult);
        }
        
        function requestValue(value) {

            var actionResult = ActionResult.requestValue(value);
            return calculate(actionResult);
        }
        
        function requestPostage(value) {

            var actionResult = ActionResult.requestPostage(value);
            return calculate(actionResult);
        }
        
        function acknoledge() {
            
            var actionResult = ActionResult.acknoledge();
            return calculate(actionResult);
        }
        
        function back() {
            
            var productDescription = CurrentProductDescription.get();
            var promise = RateCalculationService.back(
                    productDescription, environment);
            return handleResponse(promise);
        }
        
        function updateWeight(weight) {
            
            var productDescription
                    = CurrentProductDescription.updateWeight(weight);
            var promise = RateCalculationService.updateWeight(
                    productDescription, environment);
            return handleResponse(promise);
        }
        
        function handleResponse(promise) {
            return promise.then(handleSuccess, handleFailure);
        }
        
        function tryGetCalculationException(data) {

            var error = data.CalculationError;
            var exception;
            if(error) {
                exception = CalculationException.create(
                        error.ErrorMessage,
                        error.ErrorCode,
                        error.ErrorSubCode1,
                        error.ErrorSubCode2);
            }
            
            return exception;
        }
        
        function tryGetServiceException(data) {
                    
            var error = data.error;
            var exception;
            if(error) {
                exception = ServiceException.create(
                        error.message, error.code, error.details);
            } 
            
            return exception;
        }
        
        function tryGetCalculationModuleException(data) {
            
            var message = data.Message;
            var exception;
            if(message) {
                exception = CalculationModuleException.create(message);
            }
            
            return exception;
        }
        
        function tryGetRateTableException(data) {
            
            if(!angular.isArray(data)) {
                return;
            }
            
            var exception;
            var culture = ModuleSettings.culture();
            var found = data.filter(function(rateTable) {
                if(culture === rateTable.Culture) {
                    return true;
                } else {
                    return false;
                }
            });
            
            if(0 === found.length) {
                exception = NoRateTableException.create(
                        "No current rate table could be found",
                        rateTableDate,
                        culture);
            }
            
            return exception;
        }
        
        // Takes an array of tryGetXxxException functions and a service result
        // data item.
        // Evaluates the service result data against all functions an initiates
        // a disptach if an exception was found.
        function errorDispatchOnException(fns, data) {
            
            var exceptions = fns
                    .map(function(fn) {
                        return fn(data);
                    })
                    .filter(function(exception) {
                        return exception;
                    });
                    
            if(exceptions.length > 0) {
               return QueryDispatcher.dispatch(exceptions[0]);
            }
        }
        
        function handleSuccess(result) {
            
            var data = result.data;
            
            // we can get a service exception from the proxy server even though
            // we are in "success" branch.
            var exception = tryGetServiceException(data)
                    || tryGetCalculationException(data);
                    
            return QueryDispatcher.dispatch(
                    exception,
                    data.ProductDescription,
                    data.QueryType,
                    data.QueryDescription);
        }
        
        function handleFailure(result) {
            
            var data = result.data;
            var promise = errorDispatchOnException([
                tryGetServiceException,
                tryGetCalculationModuleException
            ], data);
            
            if(promise) {
                return promise;
            }
            
            // if we get here we have encountered an unknown error.
            var exception = UnknownErrorException(
                    'encountered an unknown service exception');
            
            return QueryDispatcher.dispatch(exception);
        }
        
        function handleRateTables(result) {
            
            var data = result.data;
            var promise = errorDispatchOnException([
                tryGetCalculationException,
                tryGetRateTableException
            ], data);
           
            // no special treatment in case no error was encountered.
            // Just return undefined in this case.
            
            return promise;
        }
    }
    
    
    return module;
});