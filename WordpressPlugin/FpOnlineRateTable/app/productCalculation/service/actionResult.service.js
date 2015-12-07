define([
    '../fpProductCalculation.module'
], function(module) {
    "use strict";
    
    var EActionId = {
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
    
    var EAnyType = {
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
    
    var EActionDisplayResult = {
        NOT_DISPLAYED: 0,
        DISPLAYED: 1
    };
    
    module.factory('ActionResult', ActionResult);
    
    
    function ActionResult() {
        
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
        
        function requestValue(values) {
            
            if(!Array.isArray(values)) {
                values = [values];
            }
            var result = {
                Action: EActionId.RequestValue,
                Results: []
            };
            
            for(var i = 0; i < values.length; ++i) {
                result.Results.push({
                    AnyType: EAnyType.UINT32,
                    AnyValue: values[i]
                });
            }
            
            return result;
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
    
    return module;
});