var HomeController = function ($scope, Interviews) {
    $scope.allInterviews = Interviews.allInterviews(
        function (data, status, headers, config) {
            $scope.allInterviews = data;
        },
        function (data, status, headers, config) {
            console.log(data);
        });
}

HomeController.$inject = ['$scope', 'Interviews'];