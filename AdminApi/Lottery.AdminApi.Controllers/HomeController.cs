﻿using KaSon.FrameWork.Common.ValidateCodeHelper;
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
using Microsoft.AspNetCore.Cors;
using KaSon.FrameWork.Common.Redis;
using EntityModel.CoreModel;

namespace Lottery.AdminApi.Controllers
{
    [Area("api")]
    //[ReusltFilter]
    //[EnableCors("any")]
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
                    //返回的key
                    var guidkey = Guid.NewGuid().ToString("N");
                    string key = "R_" + guidkey;
                    var db = RedisHelperEx.DB_Other;
                    var flag = db.Set(key, num.ToString(), 60 * 10);
                    //HttpContext.Session.SetObj<string>("ValidateCode", num.ToString());
                    if (!base64.StartsWith("data:image"))
                    {
                        base64 = "data:image/gif;base64," + base64;
                    }
                    result.Value = base64;
                    result.MsgId = guidkey;
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
                string MsgId = p.MsgId;
                string userName = PreconditionAssert.IsNotEmptyString(userNamestr, "登录账号不能为空！");
                string passWord = PreconditionAssert.IsNotEmptyString(passWordstr, "登录密码不能为空！");
                //MsgId = PreconditionAssert.IsNotEmptyString(MsgId, "验证码有误");

                //string key = "R_" + MsgId;
                //var db = RedisHelperEx.DB_Other;
                //var theNum = db.Get(key);
                //if (verifyCode != theNum)
                //{
                //    throw new Exception("验证码输入有误或已超时");
                //}
                AdminService service = new AdminService();
                var model= service.LoginAdmin(userName, passWord, IpManager.GetClientUserIp(HttpContext));
                if (model != null && model.IsSuccess)
                {
                    SetUser(model);
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "登录成功", Value = model.UserToken });
                }
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = model==null?"登陆失败":model.Message });
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
                var flag = CurrentUser.IsAdmin;
                var functionList = new List<C_Auth_Function_List>();
                if (!flag)
                {
                    functionList = GetAllFunciton();
                }
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "获取菜单成功",
                    Value = new {
                        menu,
                        isAdmin = flag,
                        functionList = functionList
                    },
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
            return service.QueryMyMenuCollection(CurrentUser.UserId, CurrentUser.IsAdmin);
        }

        private List<C_Auth_Function_List> GetAllFunciton()
        {
            var service = new AdminService();
            return service.GetMyAllFunciton(CurrentUser.UserId);
        }

        #region 首页
   
    
        public ActionResult Index(LotteryServiceRequest entity)
        {
            try
            {
                object Infos;
                var service = new AdminService();
                var p = JsonHelper.Decode(entity.Param);
                int id = 0;
                if (!int.TryParse(CurrentUser.UserId, out id))
                {
                    id = 0;
                }

                if (id < 100000)
                {
                     Infos = service.QuerySiteSummary();
                }
                else
                {
                     Infos = null;
                }
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = Infos });

            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message, Value = "" });
            }
        }

        /// <summary>
        /// 查询统计会员分布
        /// </summary>
        /// <returns></returns>
        public JsonResult MemberSpread()
        {
            try
            {
                var service = new AdminService();
                var str = "[";
                //会员分布
                MemberSpreadInfoCollection msic = service.QueryMemberSpread();
                foreach (MemberSpreadInfo msi in msic.infoList)
                {
                    str += string.Format(" ['" + msi.ProvinceName + "', " + msi.tcount + "],");
                }
                str += "]";
                //ViewBag.MemberSpread = str.ToString();
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = str.ToString() });
              
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message, Value = "" });
            }
        }
        /// <summary>
        /// 查询统计充值提现信息（按月统计）
        /// </summary>
        /// <returns></returns>
        public JsonResult FillMoneyWithdrawInfo()
        {
            try
            {
                var service = new AdminService();
                var strFm = "[";
                var strMonth = "[";
                //充值
                FillMoneyWithdrawInfoCollection fm = service.FillMoneyWithdrawInfo();
                foreach (FillMoneyWithdrawInfo fmi in fm.fillMoneyInfoList)
                {
                    strFm += string.Format(fmi.TotalMoney + ",");
                    strMonth += string.Format(fmi.Month.Replace("-", "") + ",");
                }
                strFm += "]";
                strMonth += "]";
                //提现
                var strWd = "[";
                foreach (FillMoneyWithdrawInfo wdi in fm.WithdrawInfoList)
                {
                    strWd += string.Format(wdi.TotalMoney + ",");
                }
                strWd += "]";
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = new {
                    MsgFm = strFm.ToString(),
                    MsgWd = strWd.ToString(),
                    MsgMonth = strMonth.ToString()
                } });
               
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.Message,
                    Value = new
                    {
                        MsgFm = "",
                        MsgWd = "",
                        MsgMonth = ""
                    }
                });
            }
        }

        /// <summary>
        /// 查询总注册、pc、安卓、ios、wap 当天的注册人数、实名人数、充值人数统计情况
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryMemberTotal()
        {
            try
            {
                var service = new AdminService();
                var Day = "[";
                var TotalCount = "[";
                var PcTotalCount = "[";
                var TouchTotalCount = "[";
                var AndroidTotalCount = "[";
                var IosTotalCount = "[";
                var NewTouchTotalCount = "[";
                var FillMoneyTotalCount = "[";
                var AuthTotalCount = "[";
                var NewAndroidCount = "[";
                var NewIOSCount = "[";
                MemberTotalCollection mtc = service.QueryMemberTotal();
                foreach (MemberTotalInfo mti in mtc.list)
                {
                    Day += string.Format(mti.Day.Replace("-", "") + ",");
                    TotalCount += string.Format(mti.TotalCount + ",");
                    PcTotalCount += string.Format(mti.PcTotalCount + ",");
                    TouchTotalCount += string.Format(mti.TouchTotalCount + ",");
                    AndroidTotalCount += string.Format(mti.AndroidTotalCount + ",");
                    IosTotalCount += string.Format(mti.IosTotalCount + ",");
                    NewTouchTotalCount += string.Format(mti.NewTouchTotalCount + ",");
                    FillMoneyTotalCount += string.Format(mti.FillMoneyTotalCount + ",");
                    AuthTotalCount += string.Format(mti.AuthTotalCount + ",");
                    NewAndroidCount += string.Format(mti.NewAndroidCount + ",");
                    NewIOSCount += string.Format(mti.NewIOSCount + ",");
                }
                Day += "]";
                TotalCount += "]";
                PcTotalCount += "]";
                TouchTotalCount += "]";
                AndroidTotalCount += "]";
                IosTotalCount += "]";
                NewTouchTotalCount += "]";
                FillMoneyTotalCount += "]";
                AuthTotalCount += "]";
                NewAndroidCount += "]";
                NewIOSCount += "]";
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = new
                {
                    IsSuccess = true,
                    MsgDay = Day.ToString(),
                    MsgTotalCount = TotalCount.ToString(),
                    MsgPcTotalCount = PcTotalCount.ToString(),
                    MsgTouchTotalCount = TouchTotalCount.ToString(),
                    MsgAndroidTotalCount = AndroidTotalCount.ToString(),
                    MsgIosTotalCount = IosTotalCount.ToString(),
                    MsgNewTouchTotalCount = NewTouchTotalCount.ToString(),
                    MsgFillMoneyTotalCount = FillMoneyTotalCount.ToString(),
                    MsgAuthTotalCount = AuthTotalCount.ToString(),
                    MsgNewAndroidCount = NewAndroidCount.ToString(),
                    MsgNewIOSCount = NewIOSCount.ToString(),
                }
                });
              
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    Value= new
                    {
                        IsSuccess = false,
                        MsgDay = "",
                        MsgTotalCount = "",
                        MsgPcTotalCount = "",
                        MsgTouchTotalCount = "",
                        MsgAndroidTotalCount = "",
                        MsgIosTotalCount = "",
                        MsgNewTouchTotalCount = "",
                        MsgFillMoneyTotalCount = "",
                        MsgAuthTotalCount = "",
                        MsgNewAndroidCount = "",
                        MsgNewIOSCount = ""
                    }

                });
            }
        }

        #endregion
    }
}
