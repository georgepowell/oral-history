var InterviewController = function ($scope, $routeParams, $sce, $timeout, Interviews) {

    MYSCOPE = $scope;

    Interviews.lookup($routeParams.id,
        function (data, status, headers, config) {
            $scope.interviewId = $routeParams.id;
            $scope.interview = data;
            $scope.interview.SoundcloudUrl = $sce.trustAsResourceUrl("https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/tracks/" + data.SoundcloudID + "&amp;color=ff5500&amp;auto_play=false&amp;hide_related=false&amp;show_comments=true&amp;show_user=true&amp;show_reposts=false");
            setupSoundcloud();
        },
        function (data, status, headers, config) {
            console.log(data);
        }
        );

    $scope.setTab = function (name) {
        $scope.selectedTab = name;
    }

    var pendingSeek = -1;

    $scope.searchLine = function (line) {
        $scope.selectedTab = 'search';
        $scope.searchQuery = line;
        $scope.searchWithin(line);
    };

    $scope.seek = function (time) {
        if (time === 0) return;

        var widget = SC.Widget("soundcloud_player");
        widget.seekTo(time * 1000);

        widget.isPaused(function (value) {
            if (value) {
                pendingSeek = time * 1000;
                widget.play();
            }
        });
        console.log("seeking to " + time);
    }
    $scope.searchLoading = false;

    $scope.searchWithin = function (query) {
        $scope.searchLoading = true;
        Interviews.searchInterview($routeParams.id, query,
        function (data, status, headers, config) {
            $scope.searchResults = data;
            $scope.searchLoading = false;
        },
        function (data, status, headers, config) {
            console.log(data);
            $scope.searchLoading = false;
        }
        );
    };

    var currentSegment;
    var setupSoundcloud = function () {

        $timeout(function () {
            var widget = SC.Widget("soundcloud_player");
            widget.bind(SC.Widget.Events.PLAY_PROGRESS, function (info) {
                if (pendingSeek !== -1) {
                    widget.seekTo(pendingSeek)
                    pendingSeek = -1;
                    return;
                }

                var currentPosition = info.currentPosition;
                var segs = $scope.interview.AutomaticTranscription.Segments
                var currentSeconds = currentPosition / 1000;
                $scope.currentSeconds = currentSeconds;

                if (currentSegment && currentSeconds > currentSegment.StartTime && currentSeconds < currentSegment.EndTime)
                    return;

                for (var i = 0; i < segs.length; i++) {
                    if (currentSeconds > segs[i].StartTime && currentSeconds < segs[i].EndTime) {
                        $scope.currentSentence = segs[i].Sentence;
                        currentSegment = segs[i];
                        $scope.$apply();
                        return;
                    }
                }
            });
        }, 500);
    };

    if ($routeParams.query) {
        $scope.selectedTab = 'search';
        $scope.searchQuery = $routeParams.query;
        $scope.searchWithin($routeParams.query);
    }
    else {
        $scope.selectedTab = 'details'
    }

}

InterviewController.$inject = ['$scope', '$routeParams', '$sce', '$timeout', 'Interviews'];