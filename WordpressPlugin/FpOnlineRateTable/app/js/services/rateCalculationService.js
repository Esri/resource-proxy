define([
    'onlineRateCalculator',
    'services/appSettings'
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
    
    // calculation state to be found in the product description
    var eProductDescriptionState = {
        Complete: 0,
        Incomplete: 1
    };
    
    app.factory('RateCalculationService', [
        '$resource',
        'AppSettings',
        function($resource, AppSettings) {
            
            var startServiceUrl;
            var calculateServiceUrl;
            var backServiceUrl;
            var updateWeightServiceUrl;
            var service = null;
            
            var weight;
            var environment;
            var productDescription;
            
            function ServiceException(message) {
                this.name = 'ServiceEexception';
                this.message= message;
            }
            ServiceException.prototype = new Error();
            ServiceException.prototype.constructor = ServiceException;
            

            // Note: we use the methode 'save' to call a service as this the the
            // angular-resource synonym for 'post'.
            return {
                isInitialized: function() {
                    return !!service;
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
                    
                    // some arbitary initial weight
                    weight = {
                        WeightValue: 1,
                        WeightUnit: 0
                    };
                    
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
                    
                    if(!this.isInitialized()) {
                        return null;
                    }
                    
                    var result = service.start({}, {
                        Weight: weight,
                        Environment: environment },
                        function success() {
                            // now that the service call has returned
                            // successfully we can access the ProductDescription
                            // field in th promise. So store the returned
                            // product description for further use.
                            productDescription = result.ProductDescription;
                            
                            // also append the current weight and envoronment
                            // objects to the result so they're available in for
                            // the caller (presumably a controller).
                            result.Weight = weight;
                            result.Environment = environment;
                        },
                        function error(error) {
                            throw new ServiceException(
                                    'service error occurred when trying do a post request for "'
                                    + startServiceUrl + '": ' + error.data.Message);
                        });
                        
                    // returns the promise for the service call
                    return result;
                },
                
                calculate: function(index) {
                    
                    if(!this.isInitialized()) {
                        return null;
                    }
                    
                    var actionResult = {
                        Action: eActionId.ShowMenu,
                        Label: 0,
                        Results: [
                            { AnyType: eAnyType.INT32, AnyValue: index } ]
                    };
                    
                    var result = service.calculate({}, {
                        ProductDescription: productDescription,
                        ActionResult: actionResult,
                        Environment: environment },
                        function success() {
                            // now that the service call has returned
                            // successfully we can access the ProductDescription
                            // field in th promise. So store the returned
                            // product description for further use.
                            productDescription = result.ProductDescription;
                            
                            // also append the current weight and envoronment
                            // objects to the result so they're available in for
                            // the caller (presumably a controller).
                            result.Weight = weight;
                            result.Environment = environment;
                        },
                        function error(error) {
                            throw new ServiceException(
                                    'service error occurred when trying do a post request for "'
                                    + startServiceUrl + '": ' + error.data.Message);
                        });
                        
                    return result;
                },
                
                back: function(environment) {
                    return service.back({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                },
                
                updateWeight: function(environment) {
                    return service.updateWeight({}, {
                        ProductDescription: productDescription,
                        Environment: environment
                    });
                }
            };
        }
    ]);
    
    return app;
});