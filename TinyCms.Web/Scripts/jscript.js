$(document).ready(function(){
    $('.subnav').click(function(event){
        event.stopPropagation();
         $('.sub_info').slideToggle("selected");
    });
    $('.sub_info').on("click", function (event) {
        event.stopPropagation();
    });
});

$(document).on("click", function () {
    $(".sub_info").hide();
});

