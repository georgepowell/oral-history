Number.prototype.toHHMMSS = function () {
    var sec_num = this;
    var hours = Math.floor(sec_num / 3600);
    var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
    var seconds = sec_num - (hours * 3600) - (minutes * 60);

    if (hours < 10) { hours = "0" + hours; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    var time = hours + ':' + minutes + ':' + Math.floor(seconds);
    return time;
}


var OralHistoryApp = angular.module('OralHistoryApp', ['ngRoute', 'ngSanitize']);

OralHistoryApp.controller('LandingPageController', LandingPageController);
OralHistoryApp.controller('SearchController', SearchController);
OralHistoryApp.controller('InterviewController', InterviewController);
OralHistoryApp.controller('HomeController', HomeController);

OralHistoryApp.factory('Interviews', InterviewsApiFactory);

var configFunction = function ($routeProvider) {
    $routeProvider.when('/home', {
        controller: 'HomeController',
        templateUrl: '/Partials/Home',
    }).when('/search/:q', {
        controller: 'SearchController',
        templateUrl: '/Partials/Search',
    }).when('/interview/:id', {
        controller: 'InterviewController',
        templateUrl: '/Partials/Interview',
    }).when('/interview/:id/search/:query', {
        controller: 'InterviewController',
        templateUrl: '/Partials/Interview',
    }).when('/interview/:id/tab/:tab', {
        controller: 'InterviewController',
        templateUrl: '/Partials/Interview',
    }).otherwise({ redirectTo: '/home' });
}

configFunction.$inject = ['$routeProvider'];

OralHistoryApp.config(configFunction);