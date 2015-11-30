define([
    'onlineRateCalculator'
], function(app) {
    "use strict";
    
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
    
    
    app.factory('QueryDispatcher', QueryDispatcher);
    
    QueryDispatcher.$inject = ['$state'];
    
    function QueryDispatcher($state) {
        
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
        
        
        function dispatch(error, queryType, query) {
            
            var path;
            
            if(error) {
                return $state.go('error', { error: error });
            }
            
            switch(queryType) {
                case eQueryType.None:
                    return;
                
                case eQueryType.ShowMenu:
                    path = 'calculate.showMenu';
                    break;
                    
                default:
                    throw new NotImplementedException('handling of query type '
                            + queryType + ' is not yet supported!');
            }
            
            return $state.go(path, { queryDescription: query });
        }
    }
    
    return app;
});