define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
    app.constant('EActionId', {
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
    });
    
    app.constant('EAnyType', {
        UNDEFINED: 0,
        INT32: 1,
        UINT32: 2,
        STRING: 3,
        INT16: 4,
        UINT16: 5,
        BOOLEAN: 6,
        UINT64: 7,
        INT64: 8
    });
    
    app.constant('EActionDisplayResult', {
        NOT_DISPLAYED: 0,
        DISPLAYED: 1
    });
    
    app.factory('ActionResult', ActionResult);
    
    ActionResult.$inject = [
        'EActionId',
        'EAnyType',
        'EActionDisplayResult'];
    
    function ActionResult(
            EActionId, EAnyType, EActionDisplayResult) {
        
        return {
            selectMenuIndex: selectMenuIndex,
            selectIndex: selectIndex,
            selectValue: selectValue,
            requestString: requestString,
            requestValue: requestValue,
            requestPostage: requestPostage,
            acknoledge: acknoledge
        };
        
        /////////////
        
        function selectMenuIndex(index) {
            return {
                Action: EActionId.ShowMenu,
                Results: [{
                    AnyType: EAnyType.INT32,
                    AnyValue: index }]
            };
        }
        
        function selectIndex(index) {
            return {
                Action: EActionId.SelectIndex,
                Results: [{
                    AnyType: EAnyType.INT32,
                    AnyValue: index }]
            };
        }
        
        function selectValue(value) {
            return {
                Action: EActionId.SelectValue,
                Results: [{
                    AnyType: EAnyType.INT32,
                    AnyValue: value }]
            };
        }
        
        function requestString(value) {
            return {
                Action: EActionId.RequestString,
                Results: [{
                    AnyType: EAnyType.STRING,
                    AnyValue: value }]
            };
        }
        
        function requestValue(value) {
            return {
                Action: EActionId.RequestValue,
                Results: [{
                    AnyType: EAnyType.UINT32,
                    AnyValue: value }]
            };
        }
        
        function requestPostage(value) {
            return {
                Action: EActionId.ManualPostage,
                Results: [{
                    AnyType: EAnyType.UINT32,
                    AnyValue: value }]
            };
        }
        
        function acknoledge() {
            return {
                Action: EActionId.Display,
                Results: [{
                    AnyType: EAnyType.INT32,
                    AnyValue: EActionDisplayResult.DISPLAYED }]
            };
        }
    }
    
    return app;
});