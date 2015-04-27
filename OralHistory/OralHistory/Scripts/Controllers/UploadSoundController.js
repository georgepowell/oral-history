var UploadSoundController = function ($scope, $routeParams, $sce, Interviews) {
    var id = $routeParams.id;

    $scope.actionUrl = $sce.trustAsResourceUrl("api/UploadSound/" + id);
}

UploadSoundController.$inject = ['$scope', '$routeParams', '$sce', 'Interviews'];