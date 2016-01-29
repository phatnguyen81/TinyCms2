
(function() {

    var tpl = "<div class=\"bg_overlay\"><div class=\"popup\">\
    <h1><a href=\"#\" class=\"close_popup icn\"></a><span class=\"title\"></span></h1>\
  	<div class=\"popup_inner\">\
        <div class=\"content\">\
        </div>\
    </div>\
  </div></div>";

    $.fn.boxy = function(opts) {

        new $.boxy();
    };
    $.boxy = function(params) {

        var opts = {
            title: "Thông báo",
            content: "",

            onOK: null,
            onCancel: null,
            onClose: null,

            textOK: "Đóng",
            textCancel: ""
        };

        $.extend(opts, params);

        var node = $(tpl);

        var btns = node.find(".lb-footer a");
        node.find(".title").html(opts.title);
        node.find(".content").html(opts.content);

        btns.eq(0).html(opts.textOK).click(onOK);
        if (!opts.textCancel) {
            btns.eq(1).hide();
        } else {
            btns.eq(1).html(opts.textCancel).click(onCancel);
        }
        node.find(".close_popup").click(onClose);


        function center() {
            var obj = node.find(".popup");
            var vp = $(window);

            obj.css({ left: (vp.width() - obj.width()) / 2, top: (vp.height() - obj.height()) / 2 });
        }

        function show() {
            node.show();
            center();
        }

        function close() {
            node.remove();
        }

        function getNode() {
            return node;
        }

// events
        function onOK(e) {
            e.preventDefault();

            if (opts.onOK) {
                opts.onOK.apply();
            }

            return onClose();
        }

        function onCancel(e) {
            e.preventDefault();

            if (opts.onCancel) {
                opts.onCancel.apply();
            }

            return onClose();
        }

        function onClose(e) {
            if (e) {
                e.preventDefault();
            }

            if (opts.onClose) {
                opts.onClose.apply();
            }

            close();
        }

        $(document.body).append(node);

        return {
            getNode: getNode,
            show: show,
            close: close
        };
    };
    $.boxy.alert = function(title, content, onClose) {

        var bx = new $.boxy({ title: title, content: content, onClose: onClose });
        bx.show();

        return bx;
    };
    $.boxy.confirm = function(title, content, onOK) {

        var bx = new $.boxy({ title: title, content: content, onOK: onOK, textOK: "Đồng ý", textCancel: "Đóng" });
        bx.show();

        return bx;
    };
})($);