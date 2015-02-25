var OralHistoryApp = angular.module('OralHistoryApp', ['ngRoute']);

OralHistoryApp.controller('LandingPageController', LandingPageController);
OralHistoryApp.controller('SearchController', SearchController);
OralHistoryApp.factory('SearchFactory', SearchFactory);

var configFunction = function ($routeProvider) {
    $routeProvider.when('/search/:q', {
        controller: 'SearchController',
        templateUrl: '/Partials/Search',
        });
}

configFunction.$inject = ['$routeProvider'];

OralHistoryApp.config(configFunction);