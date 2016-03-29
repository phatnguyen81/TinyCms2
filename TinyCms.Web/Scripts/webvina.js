

function doSubmit(formName) {
    document.getElementById(formName + "frm").submit();
}

function closePopup() {
    $(".bg_overlay").remove();
}

function showPopup(url, title) {
    var overlay = $("<div class='bg_overlay'><div class='popup'><h1><a href='#' class='close_popup icn'></a>" + title + "</h1><div class='popup_inner'></div></div></div>");
    overlay.appendTo($("body"));
    var popup = $(overlay).find(".popup");
    $(popup).css("top", (($(window).height() - $(popup).height()) / 2));
    $(popup).css("left", (($(window).width() - $(popup).width()) / 2));
    $(overlay).find(".popup_inner").load(url, function() {
        $(popup).css("top", (($(window).height() - $(popup).height()) / 2));
        $(popup).css("left", (($(window).width() - $(popup).width()) / 2));
    });
    $(overlay).find(".close_popup").click(function() {
        $(overlay).remove();
    });

}

function showLoading() {
    $(".loading").show();
}

function hideLoading() {
    $(".loading").hide();
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = "resizable=1, height=" + h + ",width=" + w + ",top=" + t + ",left=" + l + "w";
    if (scroll) winprops += ",scrollbars=1";
    var f = window.open(query, "_blank", winprops);
}