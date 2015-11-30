define([
    'onlineRateCalculator',
    'services/appSettings',
    'services/weight',
    'productCalculation/queryDispatcher.service',
    'services/rateCalculationService.service',
    'services/rateCalculationServiceAlternative.service'
], function(app) {
    "use strict";
    
    // action id enum needed by the web service interface
    var eActionId = {
        Finish: 1,
        ShowMenu: 2,
        Display: 3,
        RequestValue: 5,
        SelectIndex: 7,
        SelectValue: 8,
        NoProduct: 11,
        Continue: 12,
        TestImprint: 13,
        NoAction: 0,
        ManualPostage: 14,
        RequestString: 15,
        Unknown: 100
    };
    
    // enum to denote the any type needed by the web service interface
    var eAnyType = {
        UNDEFINED: 0,
        INT32: 1,
        UINT32: 2,
        STRING: 3,
        INT16: 4,
        UINT16: 5,
        BOOLEAN: 6,
        UINT64: 7,
        INT64: 8
    };
    
    
    app.factory(
            'RateCalculationServiceFrontend', RateCalculationServiceFrontend);
    
    RateCalculationServiceFrontend.$inject = [
        '$rootScope',
        'RateCalculationService',
        'RateCalculationServiceAlternative',
        'QueryDispatcher',
        'Weight',
        'AppSettings'];
    
    
    function RateCalculationServiceFrontend($rootScope,
            RateCalculationService, RateCalculationServiceAlternative,
            QueryDispatcher, Weight, AppSettings) {
        
        return {
            getProductDescription: getProductDescription,
            start: start,
            calculate: calculate,
            back: back,
            updateWeight: updateWeight
        };
        
        //////////
        
        var environment;
        var productDescription;
        
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
        
        function createActionResultFromIndex(index) {
            return  {
                Action: eActionId.ShowMenu,
                Label: 0,
                Results: [
                    { AnyType: eAnyType.INT32, AnyValue: index } ]
            };
        }
        
        function getProductDescription() {
            return productDescription;
        }
        
        function start(zipCode, weightValue) {

            environment = createEnvironment(zipCode);
            var weightInfo = createWeightInfo(weightValue);
//            var promise = RateCalculationService.start(environment, weightInfo);
            var promise = RateCalculationServiceAlternative.start(
                    environment, weightInfo);
            return handleSuccess(promise);
        }
        
        function calculate(index) {
            
            var actionResult = createActionResultFromIndex(index);
//            var promise = RateCalculationService.calculate(
//                    productDescription, actionResult, environment);
            var promise = RateCalculationServiceAlternative.calculate(
                    productDescription, actionResult, environment);
            return handleSuccess(promise);
        }
        
        function back() {
            
//            var promise = RateCalculationService.back(
//                    productDescription, environment);
            var promise = RateCalculationServiceAlternative.back(
                    productDescription, environment);
            return handleSuccess(promise);
        }
        
        function updateWeight(weight) {
            
            productDescription.Weight = Weight.getWeightInfo(weight);
//            var promise = RateCalculationService.back(
//                    productDescription, environment);
            var promise = RateCalculationServiceAlternative.back(
                    productDescription, environment);
            return handleSuccess(promise);
        }
        
        // we only need to handle the success case here as the failure case is
        // already handled by the lower level RateCalculationService.
        function handleSuccess(promise) {
            return promise.then(
                function (result) {
                    productDescription = result.ProductDescription;
                    $rootScope.$emit('productDescriptionChanged',
                            productDescription);
                    QueryDispatcher.dispatch(
                            result.CalculationError,
                            result.QueryType,
                            result.QueryDescription);
                });
        }
    }
    
    
    return app;
});