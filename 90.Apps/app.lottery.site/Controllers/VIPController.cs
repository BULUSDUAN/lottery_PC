using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace app.lottery.site.Controllers
{
    public class VIPController : BaseController
    {
        //
        // GET: /VIP/

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Header()
        {
            ViewBag.User = CurrentUser;
            //if (CurrentUser != null)
            //{
            //    ViewBag.UnReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken);
            //}
            return PartialView();
        }
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        public PartialViewResult Menu()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.NewUserList = WCFClients.ExternalClient.QueryTaskHotTodayInfoList(100);
            ViewBag.LoginCount = WCFClients.ExternalClient.QueryLoginDayByUserId(CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId);
            return PartialView();
        }

        public PartialViewResult Menu1()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.NewUserList = WCFClients.ExternalClient.QueryTaskHotTodayInfoList(100);
            ViewBag.LoginCount = WCFClients.ExternalClient.QueryLoginDayByUserId(CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId);
            return PartialView();
        }


        public PartialViewResult Menu2()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.NewUserList = WCFClients.ExternalClient.QueryTaskHotTodayInfoList(100);
            ViewBag.LoginCount = WCFClients.ExternalClient.QueryLoginDayByUserId(CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId);
            return PartialView();
        }

        public PartialViewResult Menu3()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.NewUserList = WCFClients.ExternalClient.QueryTaskHotTodayInfoList(100);
            ViewBag.LoginCount = WCFClients.ExternalClient.QueryLoginDayByUserId(CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId);
            return PartialView();
        }
        /// <summary>
        /// 会员介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult Introduction()
        {
            return View();
        }
        /// <summary>
        /// 特权与服务
        /// </summary>
        /// <returns></returns>
        public ActionResult Privilege()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }
        /// <summary>
        /// 我的任务
        /// </summary>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult Task()
        {
            ViewBag.User = CurrentUser;
            //查询已完成的任务
            ViewBag.CompleteTask = WCFClients.ExternalClient.QueryCompleteTaskToUserList(CurrentUser.LoginInfo.UserId);
            //查询累计任务完成情况
            ViewBag.TaskProgress = WCFClients.ExternalClient.QueryTaskListProgress(CurrentUser.LoginInfo.UserId);
            //每日购彩满50元
            ViewBag.EverDayBuyLottery = WCFClients.ExternalClient.QueryTaskUserToday(CurrentUser.LoginInfo.UserId, External.Core.TaskCategory.EverDayBuyLottery);
            //购买竞彩2串1满5次
            ViewBag.JingcaiP2_1Totle5 = WCFClients.ExternalClient.QueryTaskUserToday(CurrentUser.LoginInfo.UserId, External.Core.TaskCategory.JingcaiP2_1Totle5);
            //奖金优化满5次
            ViewBag.BonusBuyLotteryTotle5 = WCFClients.ExternalClient.QueryTaskUserToday(CurrentUser.LoginInfo.UserId, External.Core.TaskCategory.BonusBuyLotteryTotle5);
            return View();
        }
        /// <summary>
        /// 我的成长明细
        /// </summary>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult Freedom()
        {
            var time = string.IsNullOrEmpty(Request["date"]) ? 30 : int.Parse(Request["date"]);
            ViewBag.Begin = DateTime.Now.AddDays(-time);
            ViewBag.End = DateTime.Now.AddDays(1);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["PageIndex"]) ? PageIndex : int.Parse(Request["PageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
            ViewBag.CompleteTask = WCFClients.GameFundClient.QueryUserGrowthDetailList(CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.PageIndex, ViewBag.PageSize);
            return View();
        }
        /// <summary>
        /// 豆豆兑换
        /// </summary>
        /// <returns></returns>
        public ActionResult Ledou()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }
        /// <summary>
        /// 乐豆列表
        /// </summary>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult LdList()
        {
            ViewBag.User = CurrentUser;
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            return View();
        }

        public JsonResult ldpage()
        {
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            var Ledou = WCFClients.GameQueryClient.QueryUserOCDouDouDetail(DateTime.Now.AddDays(-365), DateTime.Now.AddDays(1), ViewBag.PageIndex, ViewBag.PageSize, UserToken);

            return Json(new { Issucess = true, msg = Ledou, PageIndex = ViewBag.PageIndex, PageSize = ViewBag.PageSize });
        }

        public PartialViewResult LedouToptip()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return PartialView();
        }
        /// <summary>
        /// 豆豆兑换
        /// </summary>
        /// <returns></returns>
        public JsonResult Lddh()
        {
            try
            {
                var userId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
                var doudou = string.IsNullOrEmpty(Request["doudou"]) ? 0 : int.Parse(Request["doudou"]);
                var pwd = string.IsNullOrEmpty(Request["bpwd"]) ? "" : Request["bpwd"];
                ViewBag.Doudou = WCFClients.ActivityClient.ExchangeDouDou(userId, doudou, pwd);
                return Json(new { IsSucess = true, Msg = "兑换成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });

            }
        }
    }
}
