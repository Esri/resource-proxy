define([
    'angular',
    './fpHelper.module'
], function(angular, module) {
    "use strict";
    
    module.directive('fpAdjustHeight', fpAdjustHeight);
   
    fpAdjustHeight.$inject = [
    ];

    // Use this directive to adjust the height of an element according to the
    // height of its children. This may be necessary in case you use the CSS
    // postition style "relative" so the height is not automatically adapted.
    function fpAdjustHeight() {
        
        return {
            restrict: 'A',  // This dirctive is meant to be used as attribute
            link: link
        };
        
        ////////
        
        function link(scope, elem, attrs) {
            
            // We would like to recompute the element height every time the sub
            // DOM of the element changes. So we need to register a watch
            // function.
            scope.$watch(
                    // maybe watching the html of the element is not the most
                    // performant option.
                    function() { return elem.html(); },
                    function(newVal, oldVal) {
                        if(newVal !== oldVal) {
                            // compute the sum of the heights of all child
                            // elements
                            var children = elem.children();
                            var height = 0;
                            angular.forEach(children, function(child) {
                                height += child.offsetHeight;
                            });
                            
                            // set the element height correspondingly
                            elem.css('height', height + 'px');
                        }
                    });
        }
    }
    
    return module;
});