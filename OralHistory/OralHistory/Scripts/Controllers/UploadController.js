var UploadController = function ($scope, $location, Interviews) {

    var onCreate = function () {
        Interviews.createInterview($scope.interview,
            function (data, status, headers, config) {
                $location.path("/upload/" + data.id);
            },
            function (data, status, headers, config) {
                $location.path("/home/");
            });
    }

    $scope.interview = {};
    $scope.onCreate = onCreate;
}

UploadController.$inject = ['$scope', '$location', 'Interviews'];