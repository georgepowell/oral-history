$(document).ready(function () {
    $(".audio-link").click(function (event) {
        var audioElement = document.getElementById("testAudio");
        audioElement.currentTime = $(event.delegateTarget).data("time");
        //console.log($(event.delegateTarget).data("time"));
    });
});