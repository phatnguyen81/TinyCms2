
if (typeof Dropzone != "undefined") {
    Dropzone.options.myAwesomeDropzone = false;
    Dropzone.autoDiscover = false;
}

var isdevice = (/android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(navigator.userAgent.toLowerCase()));

function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = encodeURIComponent(name) + "=";
    var ca = document.cookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === " ") c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
    }
    return null;
}


(function() {

    window.isLogin = $("#profileinfo .sub_info").size() > 0;

    var menu = $("#profileinfo");
    if (isLogin) {
        menu.find("img, span.botarr").click(function(e) {
            e.preventDefault();
            if (menu.hasClass("selected")) {
                menu.removeClass("selected");
            } else {
                menu.addClass("selected");
            }
        });
    } else {
        menu.click(function(e) {
            if (isdevice) {
                return;
            }

            e.preventDefault();
            login();
        });
    }

    var menuM = $("#profilemobile");
    if (isLogin) {
        menuM.find("img, span.botarr").click(function(e) {
            e.preventDefault();
            cateinfo.removeClass("selected");
            if (menuM.hasClass("selected")) {
                menuM.removeClass("selected");
            } else {
                menuM.addClass("selected");
            }
        });
    } else {
        menuM.click(function(e) {
            //e.preventDefault();
        });
    }

    var cateinfo = $("#cateinfo");
    cateinfo.find(".navcate").click(function(e) {
        e.preventDefault();
        menuM.removeClass("selected");
        if (cateinfo.hasClass("selected")) {
            cateinfo.removeClass("selected");
        } else {
            cateinfo.addClass("selected");
        }
    });

    function loginFB() {
        function loginSub() {
            FB.api("/me?fields=id,name,email", function(resp) {

                $.boxy.alert("Đăng nhập", "<p class=\"scctxt\"><span class=\"successicn icn\"></span>Đăng nhập facebook thành công...</p>");

                var url = "/auth/signup2";
                var data = { fullname: resp.name, email: resp.email, fbid: resp.id };

                FB.api("/me/picture?type=large", function(resp) {

                    data.avatar = resp.data.url;

                    $.post(url, data, function(data) {

                        if (!data.error) {
                            setTimeout(function() { window.location.reload(); }, 100);
                        } else {
                            $.boxy.alert("Thông báo", data.msg);
                        }
                    });
                });

            });
        }

        FB.getLoginStatus(function(resp) {

            if (resp.status === "connected") {
                //
                loginSub();
            } else {
                FB.login(function(resp) {
                    //
                    if (resp.status === "connected") {
                        //
                        loginSub();
                    } else {
                        $.boxy.alert("Thông báo", "Kết nối với facebook bị từ chối!");
                    }

                }, { scope: "public_profile,email" });
            }
        });
    }

    //function login()
    //{
    //    var node;
    //    var bx;

    //    if (arguments.length > 0) {

    //        node = $('#loginpage');
    //    }
    //    else
    //    {
    //        var tpl = '<label><input type="text" class="txtinput username" value="" placeholder="Tài khoản"></label>\
    //        <label><input type="password" class="txtinput password" value="" placeholder="Mật khẩu"></label>\
    //        <div class="subact">\
    //            <label><input type="checkbox" checked class="checkbox">Ghi nhớ</label>\
    //        </div>\
    //        <p class="actbtn"><a href="#" class="loginbtn">Đăng nhập</a></p>\
    //        <div class="newact"><p class="newreg">Chưa có tài khoản?<a href="#" class="regbtn">Đăng ký</a></p>Đăng nhập bằng <a href="#" class="facelogin icn"></a></div>';

    //        bx = $.boxy.alert('Đăng nhập', tpl);
    //        node = bx.getNode();
    //    }

    //    function sendLogin()
    //    {
    //        var username = node.find('.username').val().trim();
    //        var password = node.find('.password').val();

    //        if (username && password) {
    //            $(node).find("form").submit();
    //        }
    //        else {
    //            $.boxy.alert('Thông báo', 'Vui lòng nhập thông tin đăng nhập!');
    //        }
    //    }

    //    node.find('.password').keypress(function (e) {

    //        var key = e.which || e.keyCode;
    //        if (key == 13)
    //        {
    //            node.find('.password').blur();
    //            sendLogin();
    //        }
    //    });

    //    node.find('.loginbtn').click(function (e) {
    //        e.preventDefault();

    //        sendLogin();
    //    });

    //    //node.find('.regbtn').click(function (e) {
    //    //    e.preventDefault();
    //    //    bx && bx.close();
    //    //    signup();
    //    //});

    ////    node.find('.facelogin').click(function (e) {
    ////        e.preventDefault();
    ////        bx && bx.close();
    ////        loginFB();
    ////    });
    //}
    //login(1);

    function signup() {
        var tpl = "<label><input type=\"text\" class=\"txtinput username\" value=\"\" placeholder=\"Tên tài khoản\"></label>\
            <label><input type=\"text\" class=\"txtinput email\" value=\"\" placeholder=\"Email\"></label>\
            <label><input type=\"password\" class=\"txtinput password\" value=\"\" placeholder=\"Mật khẩu\"></label>\
            <label><input type=\"password\" class=\"txtinput repassword\" value=\"\" placeholder=\"Xác nhận mật khẩu\"></label>\
            <div class=\"subact\">\
            <label>\
            	<input id=\"agree\" type=\"checkbox\" checked class=\"checkbox\">Tôi đồng ý với <a target=\"_blank\" href=\"#\">thỏa thuận sử dụng</a></label>\
            </div>\
            <p class=\"actbtn\"><a href=\"#\" class=\"regbtn\">Đăng ký</a></p>\
        </div>";

        var bx = $.boxy.alert("Đăng ký tài khoản", tpl);
        bx.getNode().find(".popup").addClass("reg");

        bx.getNode().find(".regbtn").click(function(e) {
            e.preventDefault();

            var username = bx.getNode().find(".username").val().trim();
            var email = bx.getNode().find(".email").val().trim();
            var password = bx.getNode().find(".password").val();
            var repassword = bx.getNode().find(".repassword").val();

            if (username && email && password && repassword) {

                if (password != repassword) {
                    return $.boxy.alert("Thông báo", "Mật khẩu không giống nhau!");
                }

                if (!(new RegExp(/^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i).test(email))) {
                    return $.boxy.alert("Thông báo", "Email không hợp lệ!");
                }

                var url = "/auth/signup";
                var data = { username: username, password: password, email: email };

                $.post(url, data, function(data) {

                    if (!data.error) {
                        bx.close();
                        $.boxy.alert("Thông báo", data.msg);
                        setTimeout(function() { window.location.reload(); }, 100);
                    } else {
                        $.boxy.alert("Thông báo", data.msg);
                    }
                });
            } else {
                $.boxy.alert("Thông báo", "Vui lòng nhập thông tin đăng ký!");
            }
        });

        bx.getNode().find(".regbtn").click(function(e) {
            e.preventDefault();

        });
    }


})($);

$(function() {

    if ($("#post_tags").size() > 0) {
        $("#post_tags").tagsInput({ defaultText: "Từ khóa...", width: "930px", height: "35px" });
        CKEDITOR.replace("post_content");
    }

    $("#scrollpage").click(function(e) {
        e.preventDefault();
        $(document.body).animate({ scrollTop: 0 }, "500", "swing");
    });

    if (!isdevice) {
        $("#othernews").slimscroll({ width: "178px", height: "372px", size: "4px" });
    }

    if (typeof Dropzone != "undefined")
        $("#avatarupdate").find("img:first, a:first, span:first").dropzone({
            url: "/auth/upload",
            paramName: "file",
            acceptedFiles: "image/*",
            previewsContainer: "#avatarpreview",
            maxFilesize: 50,
            success: function(data) {

                data = $.parseJSON(data.xhr.responseText);
                $("#avatarupdate").find("img:first").attr("src", data.url);

                if (!data.error) {
                    var url = "/auth/updateinfo";
                    data = { avatar: data.url };

                    $.post(url, data, function(data) {

                        if (!data.error) {
                            return;
                        } else
                            $.boxy.alert("Thông báo", "Có lỗi xảy ra!");
                    });
                } else
                    $.boxy.alert("Thông báo", "Có lỗi xảy ra!");
            }
        });

    if (typeof Dropzone != "undefined")
        $("#thumbs").dropzone({
            url: "/auth/upload",
            paramName: "file",
            acceptedFiles: "image/*",
            previewsContainer: "#avatarpreview",
            maxFilesize: 50,
            success: function(data) {

                data = $.parseJSON(data.xhr.responseText);
                if (!data.error) {
                    $("#thumbspreview").attr("src", data.url).show();
                    $("#thumbsinput").val(data.url);
                } else
                    $.boxy.alert("Thông báo", "Có lỗi xảy ra!");
            }
        });

    // update counter
    if (typeof postId != "undefined") {

        function updateCounter() {
            if (typeof FB == "undefined") {
                return setTimeout(updateCounter, 1000);
            }

            FB.api("/?access_token=687882337980230|MfUORmDraUVmBupQ2FMc2o3BK8o", { id: window.location.href }, function(data) {

                if (typeof data.error != "undefined") {
                    return setTimeout(updateCounter, 1000);
                }

                var data = { id: postId, comment: data.share.comment_count, share: data.share.share_count };
                $.post("/apis/counter", data, function(data) {

                    if (!data.error) {
                        //
                    }
                });
            });
        }

        setTimeout(updateCounter, 1000);
    }

});

var cbFile = function() {};

function uploadCK() {
    var bx = $.boxy.alert("Tải file", "<div style=\"text-align:center;font-size:14px;\"><a class=\"uploadbtn\" href=\"javascript:;\">Chọn file để tải...</a></div>");
    bx.getNode().find("a.uploadbtn").dropzone({
        url: "/auth/upload",
        paramName: "file",
        acceptedFiles: "image/*",
        previewsContainer: "#avatarpreview",
        maxFilesize: 50,
        success: function(data) {

            data = $.parseJSON(data.xhr.responseText);
            if (!data.error) {
                bx.close();
                cbFile(data);
            } else
                $.boxy.alert("Thông báo", "Có lỗi xảy ra!");
        }
    });
}


//=================== TRUNGPH ====================
$(function() {
    //show submit message
    window.isSubmitMessage = $("#submitMsg").size() > 0;
    if (isSubmitMessage) {
        submitMsg();
    }

    function submitMsg() {
        var bx = $.boxy.alert("Thông báo", $("#submitMsg").text());
    }


    //preview submit post
    $("#postPreviewButton").click(function(e) {
        e.preventDefault();
        preview();
    });

    function preview() {

        var content = "<p class=\"txthead\">Bản Xem Trước</p>\
            <div class=\"writeblog_box\">\
            <div class=\"inner_writeblog\">" + CKEDITOR.instances["post_content"].getData() + "</div></div>";

        $("#preview_block").html(content);
    }

    //check validation for submit post
    $("#submitPostButton").click(function(e) {
        e.preventDefault();
        if ($("#title").val().trim().length > 0 && CKEDITOR.instances["post_content"].getData().trim().length > 0) {
            $("#postfrm").submit();
        } else {
            $.boxy.alert("Thông báo", "Vui lòng nhập thông tin tiêu đề và nội dung bài viết!");
        }
    });

    //load more post from cate page
    var loadMoreCateButton = $("#loadMoreCateButton");
    loadMoreCateButton.click(function(e) {
        e.preventDefault();
        loadMoreCate();
    });

    function loadMoreCate() {
        var url = "/Home/CateAjax";
        var data = { data1: loadMoreCateButton.attr("data1"), data2: loadMoreCateButton.attr("data2") };

        $.post(url, data, function(data) {
            if (!data.error) {
                var html = "";
                for (var i = 0; i < data.keys.length; i++) {
                    html += getPostItem(data[data.keys[i]]);
                }
                $("#moreitemhere").html($("#moreitemhere").html() + html);
                if (data.data2 == "0") loadMoreCateButton.hide();
                else loadMoreCateButton.attr("data2", data.data2);
            } else {
                var bx = $.boxy.alert("Thông báo", "Thông tin đăng nhập không hợp lệ!");
            }

        });
    }

    function getPostItem(data) {
        var result = "<div class=\"catebox\">\
                    <div class=\"newscont\">\
                        <a class=\"boximg\" href=\"[URL]\"><img width=\"143\" height=\"105\" alt=\"TITLE\" src=\"[IMAGE]\"></a>\
                        <div class=\"detail\">\
                            <h3><a href=\"[URL]\">TITLE</a> </h3>\
                            <p>[SUMMARY]</p>\
                        </div>\
                    </div>\
                </div>\
                ";
        result = result.replace("[URL]", data.url);
        result = result.replace(/TITLE/g, data.title);
        result = result.replace("[SUMMARY]", data.summary);
        result = result.replace("[IMAGE]", data.image);
        return result;
    }


    //search paging link to submit
    var searchDiv = $("#searchPagingBox");
    searchDiv.find("a").click(function(e) {
        e.preventDefault();
        if ($(this).attr("href")) {
            var page = $(this).attr("href").split("=").pop();
            $("#page").val(page);
            $("#page").closest("form").submit();
        }
    });

    ////survey module
    //$('#surveyresultlink').click(function (e) {
    //    e.preventDefault();
    //    surveyResultBoard(false);
    //});

    //$('#surveyresultbutton').click(function (e) {
    //    e.preventDefault();

    //    var flag = $('#surveyresultbutton').attr('vote') == '1';
    //    var a = $('#surveyquestion').attr('surveyid');
    //    if (readCookie("survey_" + a) != null)
    //    {
    //        flag = false;
    //    }

    //    surveyResultBoard( flag );
    //});

    function surveyResultBoard(isvoting) {
        var tpl = "<div class=\"content\">\
        <p class=\"quesvote\">" + $("#surveybox").find("h2").text() + "</p>\
        <table align=\"center\">\
          <tbody>\
            [ITEMS]\
          </tbody>\
        </table>\
      </div>";

        var a = $("#surveyquestion").attr("surveyid");
        var b = $("#surveybox input:checked").val();
        if (isvoting && b + "" == "undefined") {
            var bx = $.boxy.alert("Thông báo", "Xin vui lòng chọn một ý kiến để trả lời!");
            return;
        }

        var url = "/Home/SurveyAjax";
        var data = { data1: a, data2: isvoting ? b : "*" };

        $.post(url, data, function(data) {
            if (!data.error) {
                var html = "";
                for (var i = 0; i < data.keys.length; i++) {
                    html += getSurveyItem(data[data.keys[i]]);
                }

                tpl = tpl.replace("[ITEMS]", html);
                var bx = $.boxy.alert("Kết quả bình chọn", tpl);
                bx.getNode().find(".popup").addClass("vote");

                if (isvoting) {
                    $("#surveyresultbutton").attr("vote", "0");
                    createCookie("survey_" + a, 1, 10);
                }
            } else {
                var bx = $.boxy.alert("Thông báo", "Thông tin đăng nhập không hợp lệ!");
            }

        });
    }

    function getSurveyItem(data) {
        var result = "<tr>\
              <td align=\"left\" style=\"width:45%\"> [NAME]&nbsp;&nbsp; </td>\
              <td align=\"left\" style=\"width:40%\" height=\"16px\" width=\"200\" >\
                <div style=\"background:#01a260; display: block; height: 16px; width: 100%; background:#d3d3d3;\" >\
                    <div style=\"background:#01a260; display: block; height: 100%; width: PERCENT%;\" >&nbsp;</div>\
                </div>\
                </td>\
              <td align=\"right\">&nbsp;&nbsp;<strong> PERCENT % </strong></td>\
            </tr>\
                ";
        result = result.replace("[NAME]", data.name);
        result = result.replace("[VOTE]", data.vote);
        result = result.replace(/PERCENT/g, data.percent);
        return result;
    }

});

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