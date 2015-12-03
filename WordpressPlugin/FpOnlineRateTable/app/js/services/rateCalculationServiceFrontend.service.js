define([
    'onlineRateCalculator',
    'services/appSettings',
    'services/weight',
    'productCalculation/queryDispatcher.service',
    'productCalculation/currentProductDescription.service',
    'services/rateCalculationService.service',
    'productCalculation/actionResult.service'
], function(app) {
    "use strict";
    
    app.factory(
            'RateCalculationServiceFrontend', RateCalculationServiceFrontend);
    
    RateCalculationServiceFrontend.$inject = [
        'RateCalculationService',
        'CurrentProductDescription',
        'ActionResult',
        'QueryDispatcher',
        'Weight',
        'AppSettings'];
    
    
    function RateCalculationServiceFrontend(RateCalculationService,
            CurrentProductDescription, ActionResult, QueryDispatcher,
            Weight, AppSettings) {
        
        return {
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
        
        function createEnvironment(zipCode) {
            return {
                Iso3CountryCode: AppSettings.countryCode(),
                CarrierId: AppSettings.carrierId(),
                UtcDate: new Date(),
                Culture: AppSettings.culture(),
                SenderZipCode: zipCode
            };
        }
        
        function createWeightInfo(weightValue) {
            
            if(!weightValue) {
                weightValue = 1; // gram
            }
            
            return Weight.getWeightInfo(weightValue);
        }
        
        function calculate(actionResult) {
            
            var productDescription = CurrentProductDescription.get();
            var promise = RateCalculationService.calculate(
                    productDescription, actionResult, environment);
            return handleSuccess(promise);
        }
        
        function start(zipCode, weightValue) {

            environment = createEnvironment(zipCode);
            var weightInfo = createWeightInfo(weightValue);
            var promise = RateCalculationService.start(weightInfo, environment);
            return handleSuccess(promise);
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
            return handleSuccess(promise);
        }
        
        function updateWeight(weight) {
            
            var productDescription
                    = CurrentProductDescription.updateWeight(weight);
            var promise = RateCalculationService.updateWeight(
                    productDescription, environment);
            return handleSuccess(promise);
        }
        
        // we only need to handle the success case here as the failure case is
        // already handled by the lower level RateCalculationService.
        function handleSuccess(promise) {
            return promise.then(
                function (result) {
                    QueryDispatcher.dispatch(
                            result.data.CalculationError,
                            result.data.ProductDescription,
                            result.data.QueryType,
                            result.data.QueryDescription);
                });
        }
    }
    
    
    return app;
});