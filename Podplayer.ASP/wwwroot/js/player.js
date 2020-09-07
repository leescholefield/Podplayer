var currentlyPlayingPodcastUrl = null;
var currentlyPlayingEpisodeUrl = null;

$(document).ready(function () {
    $("#jquery_jplayer_1").jPlayer({
        cssSelectorAncestor: "#jp_container_1",
        swfPath: "/js",
        supplied: "m4a, oga",
        useStateClassSkin: true,
        autoBlur: false,
        smoothPlayBar: true,
        keyEnabled: true,
        remainingDuration: true,
        toggleDuration: true,
        mode: 'window'
    });
});

$(document).on("click", ".play-button", function () {
    url = $(this).attr('id');
    $("#jquery_jplayer_1").jPlayer('setMedia', { mp3: url, oga: url }).jPlayer('play');
    $('#play-footer').css("visibility", "visible");

    // set now playing details
    var epTitle = $(this).closest(".podcast-episode").find(".episode-title").text();
    var podTitle = $(":root").find(".podcast-author").text();

    currentlyPlayingPodcastUrl = $(":root").find(".podcast-rss-loc").attr("id");
    if (currentlyPlayingEpisodeUrl != null) {
        clearNowPlayingColor();
    }
    currentlyPlayingEpisodeUrl = url;

    $(this).closest(".podcast-episode").css("background-color", "#E9E9E9");
    $(".play-bar-title").html(epTitle);
    $(".play-bar-podcast").html(podTitle);
});

function goToPodcast() {
    req = $.ajax({
        url: "/podcast?podUrl=" + currentlyPlayingPodcastUrl,
        headers: { "only-main": "true" },
        success: function (content, _, response) {
            document.getElementsByTagName('main')[0].innerHTML = content;
            window.history.pushState({ "html": response.html }, "", url);
            highlightNowPlaying();
        }
    });
}

function highlightNowPlaying() {
    $("input[id='" + currentlyPlayingEpisodeUrl + "']").closest(".podcast-episode").css("background-color", "#E9E9E9");
}

function clearNowPlayingColor() {
    $("input[id='" + currentlyPlayingEpisodeUrl + "']").closest(".podcast-episode").css("background-color", "#f5f5f5");
}