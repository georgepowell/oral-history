var SearchController = function ($scope, $routeParams, SearchFactory) {

    SearchFactory.search($routeParams.q || "attractive",
        function (data, status, headers, config) {
            $scope.results = data;
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );
}

SearchController.$inject = ['$scope', '$routeParams', 'SearchFactory'];