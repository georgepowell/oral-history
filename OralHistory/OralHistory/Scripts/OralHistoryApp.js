var OralHistoryApp = angular.module('OralHistoryApp', ['ngRoute']);

OralHistoryApp.controller('LandingPageController', LandingPageController);

var configFunction = function ($routeProvider) {
    $routeProvider.
        when('/routeOne', {
            templateUrl: 'routesDemo/one'
        })
        .when('/routeTwo', {
            templateUrl: 'routesDemo/two'
        })
        .when('/routeThree', {
            templateUrl: 'routesDemo/three'
        });
}

configFunction.$inject = ['$routeProvider'];

OralHistoryApp.config(configFunction);