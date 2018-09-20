using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using External.Core.Task;
using GameBiz.Core;

namespace app.lottery.site.iqucai.Controllers
{
    public class HuoDongController : BaseController
    {
        /// <summary>
        /// 登录状态(部分视图)
        /// </summary>
        public PartialViewResult State()
        {
            ViewBag.CurrentUser = CurrentUser;
            return PartialView();
        }
        /// <summary>
        /// 底部
        /// </summary>
        public PartialViewResult Footer()
        {
            return PartialView();
        }

        /// <summary>
        /// 疯狂加奖高达18%
        /// </summary>
        public ActionResult FengKuangJiaJiang()
        {
            return View();
        }
        /// <summary>
        /// 买彩票不花钱
        /// </summary>
        public ActionResult BuyCaiBhq()
        {
            ViewBag.BuyCpBhq = WCFClients.ActivityClient.QueryGiveRedPageInfo(DateTime.Now.ToString("yyyyMMdd"));
            return View();
        }
        /// <summary>
        /// 足彩不中也有奖
        /// </summary>
        public ActionResult Zucai()
        {
            return View();
        }
       
        /// <summary>
        /// 买彩票得返利
        /// </summary>
        public ActionResult Fanli()
        {
            return View();
        }
        /// <summary>
        /// 注册送彩金充值100送100
        /// </summary>
        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// 名家争霸赛
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpertPK()
        {
            var beginTime = Convert.ToDateTime("2014-10-01");
            var endTime = Convert.ToDateTime("2014-10-31");
            //var Time = string.IsNullOrEmpty(Request["data"]) ? 30 : int.Parse(Request["data"]);
            ViewBag.RankingList = WCFClients.ExperterClient.QueryExperterRankingList(beginTime, endTime);
            return View();

        }
        /// <summary>
        /// 活动中心
        /// </summary>
        public ActionResult NewAvtionCentent()
        {
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 20 : int.Parse(Request["pageSize"]);
            ViewBag.Avtion = WCFClients.ExternalClient.QueryActivInfoList(ViewBag.PageIndex, ViewBag.PageSize);
            return View();
        }
        /// <summary>
        /// 天伦之乐
        /// </summary>
        /// <returns></returns>
        public ActionResult Family()
        {
            return View();
        }
        /// <summary>
        /// 新  活动专题
        /// </summary>
        /// <returns></returns>
        public ActionResult AvtionCentent()
        {
            ViewBag.ActivityList = GetActivityList();
            return View();
        }

        public JsonResult ActivityTime()
        {
            var activity = GetActivityList();
            return Json(activity);
        }
        /// <summary>
        /// 彩票包邮
        /// </summary>
        /// <returns></returns>
        public ActionResult CpMail()
        {
            return View();
        }

        public ActivityListInfoCollection GetActivityList()
        {
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10000 : int.Parse(Request["pageSize"]);
            return WCFClients.ExternalClient.QueryActivInfoList(ViewBag.PageIndex, ViewBag.PageSize);
        }

    }
}
