var LandingPageController = function ($scope, SearchFactory) {
    SearchFactory.search("attractive",
        function (data, status, headers, config) {
            console.log(data);
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );

    $scope.models = {
        helloAngular: 'I work!'
    };
}

LandingPageController.$inject = ['$scope', 'SearchFactory'];