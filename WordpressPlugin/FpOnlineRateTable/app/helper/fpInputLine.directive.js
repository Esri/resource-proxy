define([
    './fpHelper.module',
    './formatStringParser.service',
    './fpDynamicAttributes.directive'
], function(module) {
    "use strict";
   
    
    module.directive('fpInputLine', fpInputLine);
   
    fpInputLine.$inject = [];

    function fpInputLine() {
        
        return {
            restrict: 'E',  // This dirctive is meant to be used as attribute
            scope: {},
            bindToController: {
                model: '=ngModel',
                descr: '@', // The attribute that contains the format description. We use a oneway binding here.
                class: '@'
            },
            controller: InputLineController,
            controllerAs: 'vm',
            templateUrl: '/wordpress/wp-content/plugins/FpOnlineRateTable/app/js/helper/fpInputLine.html'
        };
    }
    
    
    InputLineController.$inject = [
        'FormatStringParser'
    ];
    
    function InputLineController(FormatStringParser) {
        
        var vm = this;

        var attrsToAppend = { class: vm.class };
        vm.fields = FormatStringParser.parse(vm.descr, attrsToAppend);
        vm.model = [];
        vm.renderAsString = renderAsString;
        vm.renderAsInput = renderAsInput;
        
        ////////
        
        function renderAsString(stuff) {
            return ('string' === typeof stuff);
        }
        
        function renderAsInput(stuff) {
            return !renderAsString(stuff);
        }
    }
    
    return module;
});