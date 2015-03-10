var OralHistoryApp = angular.module('OralHistoryApp', ['ngRoute']);

OralHistoryApp.controller('LandingPageController', LandingPageController);
OralHistoryApp.controller('SearchController', SearchController);
OralHistoryApp.controller('InterviewController', InterviewController);

OralHistoryApp.factory('Interviews', InterviewsApiFactory);

var configFunction = function ($routeProvider) {
    $routeProvider.when('/search/:q', {
        controller: 'SearchController',
        templateUrl: '/Partials/Search',
    });
    $routeProvider.when('/interview/:id', {
        controller: 'InterviewController',
        templateUrl: '/Partials/Interview',
    });
}

configFunction.$inject = ['$routeProvider'];

OralHistoryApp.config(configFunction);