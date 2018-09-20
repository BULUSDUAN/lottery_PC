if(getCookie('close_mbBox')!=1){
   $("#mbBox").show();
}
 $(".di_close").click(function(){
 QsetCookie('close_mbBox',1);
     $("#mbBox").hide();
 });
 
 //∂¡»°cookies
function getCookie(name)
{
    var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)");
  
    if(arr=document.cookie.match(reg))
  
        return (arr[2]);
    else
        return null;
}
function QsetCookie(name,value)
{
    var strsec = 1*6*60*60*1000;
    var exp = new Date();
    exp.setTime(exp.getTime() + strsec*1);
    document.cookie = name + "="+ escape (value) + ";expires=" + exp.toGMTString()+"path=/;";
}