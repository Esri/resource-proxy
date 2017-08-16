define([
    './fpProductCalculation.module',
    './currentProductDescription.service',
    './currentQueryDescription.service',
    '../errorHandler/notImplementedException.factory'
], function(module) {
    "use strict";
    
    var EQueryType = {
        None: 0,
        RequestPostage: 1,
        RequestValue: 2,
        RequestString: 3,
        SelectIndex: 4,
        ShowMenu: 5,
        ShowDisplay: 6,
        SelectValue: 7
    };
    
    
    module.factory('QueryDispatcher', QueryDispatcher);
    
    QueryDispatcher.$inject = [
        '$state',
        'CurrentProductDescription',
        'CurrentQueryDescription',
        'NotImplementedException'
    ];
    
    function QueryDispatcher($state, CurrentProductDescription,
            CurrentQueryDescription, NotImplementedException) {
        
        return {
            dispatch: dispatch
        };
        
        /////////////
        
        function dispatch(error, productDescription, queryType, query) {
            
            var path;
            
            if(error) {
                return $state.go('error', {error: error});
            }
            
            switch(queryType) {
                case EQueryType.None:
                    // no query description needs to be set
                    return;
                    
                case EQueryType.RequestValue:
                    path = 'calculate.requestValue';
                    CurrentQueryDescription.set(queryType, query);
                    break;
                
                case EQueryType.ShowMenu:
                    path = 'calculate.showMenu';
                    CurrentQueryDescription.set(queryType, query);
                    break;
                    
                case EQueryType.ShowDisplay:
                    path = 'calculate.showDisplay';
                    CurrentQueryDescription.set(queryType, query);
                    break;
                    
                case EQueryType.SelectValue:
                    path = 'calculate.selectValue';
                    CurrentQueryDescription.set(queryType, query);
                    break;
                    
                default:
                    var exception = NotImplementedException.create(
                            'Handling of query type '+ queryType
                            + ' is not yet supported!');
                    return $state.go('error', {error: exception}); 
            }
            
            CurrentProductDescription.set(productDescription);
            
            return $state.go(path, {queryDescription: query});
        }
    }
    
    return module;
});