var SearchController = function ($scope, $routeParams, Interviews) {
    window.SCOPE = $scope;

    $scope.$parent.q = $routeParams.q;
    $scope.q = $routeParams.q;

    Interviews.search($routeParams.q,
        function (data, status, headers, config) {
            $scope.results = data;
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );
}

SearchController.$inject = ['$scope', '$routeParams', 'Interviews'];