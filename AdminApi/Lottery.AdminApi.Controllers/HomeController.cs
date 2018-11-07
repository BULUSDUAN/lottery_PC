using KaSon.FrameWork.Common.ValidateCodeHelper;
using System;
using System.Collections.Generic;
using System.Text;
using KaSon.FrameWork.Common;
using Microsoft.AspNetCore.Mvc;
using EntityModel.Enum;
using Lottery.AdminApi.Model.HelpModel;
using System.DrawingCore;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Net;
using EntityModel;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;

namespace Lottery.AdminApi.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class HomeController:BaseController
    {
        #region 验证码相关函数
        //生成验证码并返回一个结果
        public IActionResult CreateValidateCode()
        {
            try
            {
                var num = 0;
                string randomText = SelectRandomNumber(5, out num);
                var result = new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = "获取验证码失败,请刷新验证码"
                };
                //HttpContext.Session.SetString("VerifyCode", num.ToString());
                ValidateCodeGenerator vlimg = new ValidateCodeGenerator()
                {
                    BackGroundColor = Color.FromKnownColor(KnownColor.LightGray),
                    RandomWord = randomText,
                    ImageHeight = 25,
                    ImageWidth = 100,
                    fontSize = 14,
                };
                var img = vlimg.OnPaint();
                if (img == null)
                {
                    // return Content("Error");
                }
                else
                {
                    result.Code = AdminResponseCode.成功;
                    result.Message = "成功获取验证码";
                    string base64 = Convert.ToBase64String(img);
                    HttpContext.Session.SetObj<string>("ValidateCode", num.ToString());
                    if (!base64.StartsWith("data:image"))
                    {
                        base64 = "data:image/gif;base64," + base64;
                    }
                    result.Value = base64;
                }
                return JsonEx(result);
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        private string SelectRandomNumber(int numberOfChars, out int num)
        {
            num = 0;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            char[] columns = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                if (incr == 0 || incr == 2)
                {
                    var randomNum = columns[randomSeed.Next(10)].ToString();
                    randomBuilder.Append(randomNum);//取26个字符里的任意一个
                    num += int.Parse(randomNum);
                }
                if (incr == 1)
                {
                    randomBuilder.Append("+").ToString();
                }
                if (incr == 3)
                {
                    randomBuilder.Append("=").ToString();
                }
                if (incr == 4)
                {
                    randomBuilder.Append("?").ToString();
                }
            }
            return randomBuilder.ToString();
        }
        #endregion 

        /// <summary>
        /// 登录后台
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult LoginFunction(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userNamestr = p.userName;
                string passWordstr = p.passWord;
                string verifyCodestr = p.verifyCode;
                string userName = PreconditionAssert.IsNotEmptyString(userNamestr, "登录账号不能为空！");
                string passWord = PreconditionAssert.IsNotEmptyString(passWordstr, "登录密码不能为空！");
                //string verifyCode = PreconditionAssert.IsNotEmptyString(verifyCodestr, "验证码不能为空！");
                var vCode = HttpContext.Session.GetObj<string>("ValidateCode");
                //if (vCode != verifyCode)
                //{
                //    throw new Exception("验证码输入错误！");
                //}
                AdminService service = new AdminService();
                CurrentUser = service.LoginAdmin(userName, passWord, IpManager.GetClientUserIp(HttpContext));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功,  Message="登录成功"});
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }


        public IActionResult MenuMethod()
        {
            try
            {
                var menu = GetMenuCollection();
                //string menuItems = "";
                //foreach (var m1 in menu)
                //{
                //    if (m1.ParentId == null)
                //    {
                //        if (menu.Where(p => p.ParentId == m1.MenuId).Count() > 0)
                //        {
                //            menuItems += "<div style=\"1px solid #D3D3D3\" id=\"bg_Div\"><h4 class=\"left_top_M\"><span class=\"s_QH\" style=\"color: Red;width:10px;float:left;margin-left:3px;line-height:28px;\">+</span>&nbsp;" + m1.DisplayName + "</h4></div><ul class=\"left_bottom_M\" style=\"display:none;\">";//
                //            if (m1.MenuId.Equals("170"))
                //            {
                //                try
                //                {
                //                    //var reportList = QueryClient.GetCustomerReportList(CurrentUser.UserToken);
                //                    //var menuReportList = reportList.Where(r => r.TopOnMenu).ToList();
                //                    //foreach (var report in menuReportList)
                //                    //{
                //                    //    menuItems += "<li><a href=/StatementManage/CustomerReportExec?uuid=" + report.UUID + " style=\"overflow:hidden;\" title=" + report.DisplayName + ">" + "&nbsp;&nbsp;&nbsp;<img src='" + ConfigurationManager.AppSettings["ShareRes"].ToString() + "/images/admin/arr4.gif'  alt=\"菜单图标\"/>&nbsp;&nbsp;" + report.DisplayName + "</a></li>";
                //                    //}
                //                }
                //                catch
                //                {
                //                }
                //            }
                //            foreach (var subMenu in menu)
                //            {
                //                if (subMenu.ParentId != null)
                //                {
                //                    if (m1.MenuId == subMenu.ParentId)
                //                    {
                //                        menuItems += "<li><a href=" + subMenu.Url + " style=\"overflow:hidden;\" title=" + subMenu.DisplayName + ">" + "&nbsp;&nbsp;&nbsp;<img src='" + ConfigurationManager.AppSettings["ShareRes"].ToString() + "/images/admin/arr4.gif'  alt=\"菜单图标\"/>&nbsp;&nbsp;" + subMenu.DisplayName + "</a></li>";
                //                    }
                //                }
                //            }
                //            menuItems += "</ul>";
                //        }
                //    }
                //}
                //return menuItems;
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "获取菜单成功",
                    Value = menu,
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    Value = ex.ToGetMessage() + "●" + ex.ToString(),
                });
            }
           
        }


        private List<MenuInfo> GetMenuCollection()
        {
            var service = new AdminService();
            return service.QueryMyMenuCollection(CurrentUser.UserToken);
        }
    }
}
