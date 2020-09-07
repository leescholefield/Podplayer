$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

$(document).on('submit', ".login-form", function (e) {
    if (!($('#play-footer').css('display') == 'none' || $('#play-footer').css("visibility") == "hidden")) {
        e.preventDefault();
        InternalFormPartial($(this), true);
    }
});

$(document).on('click', ".internal-nav", function (e) {
    if (!($('#play-footer').css('display') == 'none' || $('#play-footer').css("visibility") == "hidden")) {
        e.preventDefault();
        url = e.currentTarget.getAttribute('href');
        req = $.ajax({
            url: url,
            headers: {"only-main": "true"},
            success: function (content, _, response) {
                document.getElementsByTagName('main')[0].innerHTML = content;
                window.history.pushState({ "html": response.html}, "", url);
            }
        });
    }

    e.stopPropagation();
});

$(document).on('submit', ".internal-form", function (e) {
    if (!($('#play-footer').css('display') == 'none' || $('#play-footer').css("visibility") == "hidden")) {
        e.preventDefault();
        InternalFormPartial($(this));
    }
});

function InternalFormPartial(form, reloadLoginView) {
        var url = form.attr('action');

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            headers: { "only-main": "true" },
            success: function (content, _, response) {
                document.getElementsByTagName('main')[0].innerHTML = content;
                window.history.pushState({ "html": response.html }, "", url);

                if (reloadLoginView) {
                    SetLoginView();
                }
            }
        })
}

$(document).on('submit', ".subscribe-form", function (e) {
    e.preventDefault();

        var form = $(this);
        var url = form.attr('action');

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            headers: { "only-main": "true" },
            success: function () {
                var na = form.attr('action').replace(/subscribe/gi, "unsubscribe")
                form.attr('action', na);
                form.toggleClass("unsubscribe-form");
                form.toggleClass("subscribe-form");
                form.children("#sub-button").attr("src", "/images/baseline_favorite_black_48dp.png");
            },
            error: function (_, _, errorThrown) {
                if (errorThrown == "Unauthorized") {
                    window.location = "/Account/SignIn";
                }
            }
        });
});

$(document).on('submit', ".unsubscribe-form", function (e) {
    e.preventDefault();

    var form = $(this);
    var url = form.attr('action');

    var removeElem = form.children("#should-remove");
    var shouldRemove = false;
    if (removeElem != null) {
        if (removeElem.attr('value') == 'true') {
            shouldRemove = true;
        }
    }

    $.ajax({
        type: "POST",
        url: url,
        data: form.serialize(),
        headers: { "only-main": "true" },
        success: function (content, _, response) {
            if (shouldRemove) {
                form.closest(".model-item").remove();
            }
            else {
                var na = form.attr('action').replace(/unsubscribe/gi, "subscribe")
                form.attr('action', na);
                form.toggleClass("unsubscribe-form");
                form.toggleClass("subscribe-form");
                form.children("#sub-button").attr("src", "/images/baseline_favorite_border_black_48dp.png");
            }
        }
    });
});

function SetLoginView() {
    var url = "/Account/LoginPartialView";

    $.ajax({
        type: "GET",
        url: url,
        success: function (content) {
            $("#login-container > ul").replaceWith(content);
        }
    });
}

$(document).on("click", ".details-expand", function (e) {
    var details = $(this).prev();
    details.css({
        height: 'auto'
    });
    $(this).children(".expand-icon").attr('src', "/images/expand_less.png");
    $(this).toggleClass("details-contract").toggleClass("details-expand");
});

$(document).on("click", ".details-contract", function (e) {
    var epDetails = $(this).prev();
    var className = epDetails.attr('class');
    var height = className == "ep-details" ? '20px' : '100px'
    epDetails.css({
        height: height
    });
    $(this).children(".expand-icon").attr('src', "/images/expand_more.png");
    $(this).toggleClass("details-contract").toggleClass("details-expand");
});

$(document).on("click", "#episode-date", function () {
    $('#ep-table tbody').html($('.podcast-episode').get().reverse());
});



