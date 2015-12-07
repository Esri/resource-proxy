define([
    '../fpProductCalculation.module',
    '../currentProductDescription.service',
    '../currentQueryDescription.service'
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
        'CurrentQueryDescription'];
    
    function QueryDispatcher(
            $state, CurrentProductDescription, CurrentQueryDescription) {
        
        return {
            dispatch: dispatch
        };
        
        /////////////
        
        function NotImplementedException(message) {
            this.name = 'NotImplementedException';
            this.message= message;
        }
        NotImplementedException.prototype = new Error();
        NotImplementedException.prototype.constructor = NotImplementedException;
        
        
        function dispatch(error, productDescription, queryType, query) {
            
            var path;
            
            if(error) {
                return $state.go('error', { error: error });
            }
            
            switch(queryType) {
                case EQueryType.None:
                    
                    // We have a new product description but no new query
                    // description.
                    CurrentProductDescription.set(productDescription);
                    
                    return;
                    
                case EQueryType.RequestValue:
                
                    path = 'calculate.requestValue';
                    
                    CurrentProductDescription.set(productDescription);
                    CurrentQueryDescription.set(queryType, query);
                    
                    break;
                
                case EQueryType.ShowMenu:
                    
                    path = 'calculate.showMenu';
                    
                    // We have a new query description and a new product
                    // description - so store both.
                    CurrentProductDescription.set(productDescription);
                    CurrentQueryDescription.set(queryType, query);
                    
                    break;
                    
                case EQueryType.ShowDisplay:
                    
                    path = 'calculate.showDisplay';
                    
                    // We have a new query description and a new product
                    // description - so store both.
                    CurrentProductDescription.set(productDescription);
                    CurrentQueryDescription.set(queryType, query);
                    
                    break;
                    
                default:
                    throw new NotImplementedException('handling of query type '
                            + queryType + ' is not yet supported!');
            }
            
            return $state.go(path, { queryDescription: query });
        }
    }
    
    return module;
});