var LandingPageController = function ($scope, $location) {

    $scope.navigateSearch = function () {
        $location.hash("/search/" + $scope.q);
    };
}

LandingPageController.$inject = ['$scope', '$location'];