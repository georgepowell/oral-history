var LandingPageController = function ($scope, $location, $routeParams) {
    $scope.navigateSearch = function () {
        $location.path("/search/" + $scope.q);
    };

    $scope.q = $routeParams.q;
}

LandingPageController.$inject = ['$scope', '$location', '$routeParams'];