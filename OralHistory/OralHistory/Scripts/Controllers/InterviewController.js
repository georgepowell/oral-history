var InterviewController = function ($scope, $routeParams, Interviews) {
    Interviews.lookup($routeParams.id,
        function (data, status, headers, config) {
            $scope.interview = data;
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );
}

InterviewController.$inject = ['$scope', '$routeParams', 'Interviews'];