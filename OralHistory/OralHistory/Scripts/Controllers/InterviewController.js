var InterviewController = function ($scope, $routeParams, $sce, Interviews) {

    MYSCOPE = $scope;
    Interviews.lookup($routeParams.id,
        function (data, status, headers, config) {
            $scope.interviewId = $routeParams.id;
            $scope.interview = data;
            $scope.interview.SoundcloudUrl = $sce.trustAsResourceUrl("https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/tracks/" + data.SoundcloudID + "&amp;color=ff5500&amp;auto_play=false&amp;hide_related=false&amp;show_comments=true&amp;show_user=true&amp;show_reposts=false");
            $scope.selectedTab = "details";
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );

    $scope.setTab = function(name) {
        $scope.selectedTab = name;
    }

    $scope.seek = function (time) {
        if (time === 0) return;
        var widget = SC.Widget("soundcloud_player");
        widget.seekTo(time * 1000)
        console.log("seeking to " + time);
    }
}

InterviewController.$inject = ['$scope', '$routeParams', '$sce', 'Interviews'];