(function() {
    'use strict';

    var module = angular.module('hj.animInOut', ['ngAnimate']);

    angular.module('anim-in-out', ['hj.animInOut']);

    module.animation('.anim-in-out', ['$rootScope', '$timeout', '$window',
        function($rootScope, $timeout, $window) {
            return {
                enter: function(element, done) {
                    var sync = $rootScope.$eval(angular.element(element).attr('data-anim-sync')) !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-sync')) : false,
                        speed = angular.element(element).attr('data-anim-speed') !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-speed')) : 1000,
                        inSpeed = angular.element(element).attr('data-anim-in-speed') !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-in-speed')) : speed,
                        outSpeed = angular.element(element).attr('data-anim-out-speed') !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-out-speed')) : speed;

                    try {
                        var observer = new MutationObserver(function(mutations) {
                            observer.disconnect();

                            $window.requestAnimationFrame(function() {
                                $timeout(done, sync ? 0 : outSpeed);
                            });
                        });

                        observer.observe(element[0], {
                            attributes: true,
                            childList: false,
                            characterData: false
                        });

                    } catch (e) {
                        $timeout(done, Math.max(100, sync ? 0 : outSpeed));
                    }

                    angular.element(element).addClass('anim-in-setup');

                    return function(cancelled) {
                        angular.element(element).removeClass('anim-in-setup');
                        angular.element(element).addClass('anim-in');

                        if (!cancelled) {
                            $timeout(function() {
                                $rootScope.$broadcast('animEnd', element, inSpeed);

                                angular.element(element).removeClass('anim-in');
                            }, inSpeed);
                        }
                    };
                },
                leave: function(element, done) {
                    var speed = angular.element(element).attr('data-anim-speed') !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-speed')) : 1000,
                        outSpeed = angular.element(element).attr('data-anim-out-speed') !== undefined ? $rootScope.$eval(angular.element(element).attr('data-anim-out-speed')) : speed;

                    $rootScope.$broadcast('animStart', element, outSpeed);

                    try {
                        var observer = new MutationObserver(function(mutations) {
                            observer.disconnect();

                            $window.requestAnimationFrame(function() {
                                angular.element(element).removeClass('anim-out-setup');
                                angular.element(element).addClass('anim-out');

                                $timeout(done, outSpeed);
                            });
                        });

                        observer.observe(element[0], {
                            attributes: true,
                            childList: false,
                            characterData: false
                        });

                    } catch (e) {
                        angular.element(element).removeClass('anim-out-setup');
                        angular.element(element).addClass('anim-out');

                        $timeout(done, Math.max(100, outSpeed));
                    }

                    angular.element(element).addClass('anim-out-setup');
                }
            };
        }
    ]);

})();
