using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using Common.Utilities;
using System.Text.RegularExpressions;

namespace app.lottery.site.iqucai.Controllers
{
    public class ActivityController : BaseController
    {
        //
        // GET: /Activity/
        #region 暂时无用
        //public ActionResult Index()
        //{
        //    return View();
        //}
        ///// <summary>
        ///// 疯狂加奖18%
        ///// </summary>
        //public ActionResult AddAward() {

        //    return View();
        //}
        ///// <summary>
        ///// 买彩票不花钱
        ///// </summary>
        //public ActionResult NoMoney()
        //{
        //    return View();
        //}
        ///// <summary>
        ///// 买彩票得返利
        ///// </summary>
        //public ActionResult Rebate()
        //{

        //    return View();
        //}
        ///// <summary>
        ///// 注册送彩金
        ///// </summary>
        //public ActionResult Login()
        //{
        //    return View();
        //}
        ///// <summary>
        ///// 足彩不中也有奖
        ///// </summary>
        //public ActionResult FootballLottery()
        //{
        //    return View();
        //}
        #endregion

        #region   @SiteString.getHZSiteName(Request)活动

        /// <summary>
        /// 购彩
        /// </summary>
        /// <returns></returns>
        //public ActionResult BuyLottery()
        //{
        //    return View();
        //}
        /// <summary>
        ///  泰国游
        /// </summary>
        /// <returns></returns>
        public ActionResult ThailandTravel()
        {
            return View();
        }
        /// <summary>
        ///cbbao 足彩不中也有奖
        /// </summary>
        public ActionResult footballagatha()
        {
            return View();
        }
        /// <summary>
        ///cbbao 注册送彩金
        /// </summary>
        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        ///cbbao 手机活动页
        /// </summary>
        public ActionResult phoneactivity()
        {
            return View();
        }
        /// <summary>
        ///cbbao 关注微信
        /// </summary>
        public ActionResult attentionWechat()
        {
            return View();
        }
        /// <summary>
        /// 疯狂加奖
        /// </summary>
        /// <returns></returns>
        public ActionResult AppendAward()
        {
            return View();
        }
        /// <summary>
        /// 彩票返利
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnProfit()
        {
            return View();
        }
        /// <summary>
        /// 买彩票不花钱
        /// </summary>
        /// <returns></returns>
        public ActionResult NoMoney()
        {
            ViewBag.BuyCpBhq1 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150109");
            ViewBag.BuyCpBhq2 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150110");
            ViewBag.BuyCpBhq3 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150111");
            ViewBag.BuyCpBhq4 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150112");
            ViewBag.BuyCpBhq5 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150113");
            ViewBag.BuyCpBhq6 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150114");
            ViewBag.BuyCpBhq7 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150115");
            ViewBag.BuyCpBhq8 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150116");
            ViewBag.BuyCpBhq9 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150117");
            ViewBag.BuyCpBhq10 = WCFClients.ActivityClient.QueryGiveRedPageInfo("20150118");
            return View();
        }
        /// <summary>
        /// 幸运转盘
        /// </summary>
        /// <returns></returns>
        public ActionResult Lucky()
        {
            try
            {
                ViewBag.CurrentUser = CurrentUser;
                //ViewBag.QueryJoinLuckDraw = WCFClients.ActivityClient.QueryJoinLuckDraw(1000);
            }
            catch (Exception)
            {
                ViewBag.CurrentUser = null;
                ViewBag.QueryJoinLuckDraw = null;
            }

            return View();
        }
        /// <summary>
        /// 获奖名单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetJoinLuckDraw()
        {
            try
            {
                //var result = WCFClients.ActivityClient.QueryJoinLuckDraw(4);
                return Json(new { IsSuccess = true, Msg = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        public JsonResult GetCount()
        {
            try
            {
                var count = WCFClients.ExternalClient.QueryUserRegisterCount();
                var user = CurrentUser;
                return Json(new { IsSuccess = true, Counts = count, User = user });
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

        }
        /// <summary>
        /// 抽奖码验证
        /// </summary>
        /// <returns></returns>
        public JsonResult Rotate()
        {
            try
            {
                string code = PreconditionAssert.IsNotEmptyString(Request["code"], "验证码不能为空");
                //var result = WCFClients.ActivityClient.UserLuckyDrew(CurrentUser.LoginInfo.UserId, code, 1);
                return Json(new { IsSuccess = true, result = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }


        /// <summary>
        /// 底部
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        #endregion
        public ActionResult TwoDimensionCode()
        {
            var url = "http://" + HttpContext.Request.Url.Host;
            Regex reg = new Regex("\\d+", RegexOptions.IgnoreCase);
            var url1 = "http://m.wancai.com";
            var storeid = WCFClients.ExternalClient.QueryStoreIdByUrl(url);
            var userid = WCFClients.ExternalClient.QueryAgentUserIdByCustomerDomain(url);
            if (!string.IsNullOrEmpty(userid))
            {
                url1 = "http://m.wancai.com/user/registerstore?pid=" + userid;
            }

            ViewBag.Url = url1;
            return View();
        }

        /// <summary>
        /// 神单主题活动
        /// </summary>
        public ActionResult Baodan()
        {
            return View();
        }
    }
}
