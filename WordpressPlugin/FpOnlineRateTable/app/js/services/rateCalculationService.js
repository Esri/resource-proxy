define([
    'onlineRateCalculator',
    'services/appSettings',
    'services/weight',
    'productCalculation/queryDispatcher.service'
], function(app) {
    "use strict";
    
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
    
    // query type returned by a calculation request
    var eQueryType = {
        None: 0,
        RequestPostage: 1,
        RequestValue: 2,
        RequestString: 3,
        SelectIndex: 4,
        ShowMenu: 5,
        ShowDisplay: 6,
        SelectValue: 7
    };
    
    // calculation state to be found in the product description
    var eProductDescriptionState = {
        Complete: 0,
        Incomplete: 1
    };
    
    app.factory('RateCalculationService', [
        '$resource',
        'AppSettings',
        'Weight',
        'QueryDispatcher',
        function($resource, AppSettings, Weight, QueryDispatcher) {
            
            var startServiceUrl;
            var calculateServiceUrl;
            var backServiceUrl;
            var updateWeightServiceUrl;
            var service = null;
            
            var environment;
            var productDescription;
            var serviceState = null;
            
            function ServiceException(message) {
                this.name = 'ServiceEexception';
                this.message= message;
            }
            ServiceException.prototype = new Error();
            ServiceException.prototype.constructor = ServiceException;
            

            // Note: we use the methode 'save' to call a service as this the the
            // angular-resource synonym for 'post'.
            return {
                getServiceState: function() {
                    return serviceState;
                },
                
                init: function(zipCode) {
                    
                    if(!AppSettings.isInitialized()) {
                        return null;
                    }
                    
                    var url = AppSettings.rateCalculationUrl();
                    
                    startServiceUrl = url + '/Start';
                    calculateServiceUrl = url + '/Calculate';
                    backServiceUrl = url + '/Back';
                    updateWeightServiceUrl = url + '/UpdateWeight';
                    
                    service = $resource(url, {}, {
                        'start': { method: 'POST', url: startServiceUrl },
                        'calculate': { method: 'POST', url: calculateServiceUrl },
                        'back': { method: 'POST', url: backServiceUrl },
                        'updateWeight': { method: 'POST', url: updateWeightServiceUrl }
                    });
                    
                    // setup environment
                    environment = {
                        Iso3CountryCode: AppSettings.countryCode(),
                        CarrierId: AppSettings.carrierId(),
                        UtcDate: new Date(),
                        Culture: AppSettings.culture(),
                        SenderZipCode: zipCode
                    };
                },
                
                start: function() {
                    
                    if(!service) {
                        return null;
                    }
                    
                    // some arbitary initial weight
                    var weight = {
                        WeightValue: 1,
                        WeightUnit: 0
                    };
                    
                    return callService(service.start, startServiceUrl, {
                        Weight: weight,
                        Environment: environment });
                },
                
                calculate: function(index) {
                    
                    if(!service) {
                        return null;
                    }
                    
                    var actionResult = {
                        Action: eActionId.ShowMenu,
                        Label: 0,
                        Results: [
                            { AnyType: eAnyType.INT32, AnyValue: index } ]
                    };
                    
                    return callService(service.calculate, calculateServiceUrl, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment });

                },
                
                back: function() {
                    
                    if(!service) {
                        return null;
                    }
                    
                    return callService(
                            service.back, updateWeightServiceUrl, {
                                ProductDescription: productDescription,
                                Environment: environment });
                },
                
                updateWeight: function(weightValue) {
                    
                    if(!service) {
                        return null;
                    }
                    
                    productDescription.Weight
                            = Weight.getWeightInfo(weightValue);
                    
                    return callService(
                            service.updateWeight, updateWeightServiceUrl, {
                                ProductDescription: productDescription,
                                Environment: environment });
                }
            };
            
            function callService(serviceMethod, serviceUrl, serviceParams) {
                
                serviceState = serviceMethod.call(service, {}, serviceParams,
                    function success() {
                        // now that the service call has returned
                        // successfully we can access the ProductDescription
                        // field in th promise. So store the returned
                        // product description for further use.
                        productDescription = serviceState.ProductDescription;
                        
                        QueryDispatcher.dispatch(
                                serviceState.CalculationError,
                                serviceState.QueryType,
                                serviceState.QueryDescription);
                    },
                    function error(error) {
                        throw new ServiceException(
                                'service error occurred when trying do a post request for "'
                                + serviceUrl + '": ' + error.data.Message);
                    });

                return serviceState;
            }
        }
    ]);
    
    return app;
});