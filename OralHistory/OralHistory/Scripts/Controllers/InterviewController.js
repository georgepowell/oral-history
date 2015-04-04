var InterviewController = function ($scope, $routeParams, $sce, Interviews) {
    Interviews.lookup($routeParams.id,
        function (data, status, headers, config) {
            $scope.interview = data;
            $scope.interview.SoundcloudUrl = $sce.trustAsResourceUrl("https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/tracks/" + data.SoundcloudID + "&amp;color=ff5500&amp;auto_play=false&amp;hide_related=false&amp;show_comments=true&amp;show_user=true&amp;show_reposts=false");
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );
}

InterviewController.$inject = ['$scope', '$routeParams', '$sce', 'Interviews'];