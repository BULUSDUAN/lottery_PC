if ($.cookie('close_hongbao') != 1) {
    $("#hongbao").show();
}
if ($.cookie('close_mbBox') != 1) {
    $("#mbBox").show();
}
$(".di_close").click(function () {
    $.cookie('close_mbBox', 1);
    $("#mbBox").hide();
});
$("#close_hongbao1").click(function () {
    $.cookie('close_hongbao', 1);
    $(".hongbao1").hide();
    $(".hongbao2").hide();
    return false;
});
$("#close_hongbao2").click(function () {
    $.cookie('close_hongbao', 1);
    $(".hongbao2").hide();
    $(".hongbao1").hide();
    return false;
});	