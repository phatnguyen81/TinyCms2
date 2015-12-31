$(document).ready(function(){
    $('.subnav').click(function(event){
        event.stopPropagation();
         $(this).find('.sub_info').slideToggle("selected");
    });

});

$(document).on("click", function () {
    $(".sub_info").hide();
});

