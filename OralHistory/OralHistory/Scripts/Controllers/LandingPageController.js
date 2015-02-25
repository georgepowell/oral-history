var LandingPageController = function ($scope, $routeParams, SearchFactory) {


    this.performSearch = function () {
        SearchFactory.search($routeParams.q || "attractive",
            function (data, status, headers, config) {
                $scope.results = data;
            },
            function (data, status, headers, config) {
                console.log(data);
            }
            );
    }

    $scope.models = {
        helloAngular: 'I work!'
    };
}

LandingPageController.$inject = ['$scope', '$routeParams', 'SearchFactory'];