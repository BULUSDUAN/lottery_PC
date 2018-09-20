using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameBiz.Core;

namespace app.lottery.site.Controllers
{
    public class SoccerController : BaseController
    {

        /// <summary>
        /// 过关统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            ViewBag.User = CurrentUser;

            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "jczq" : id.ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"].ToLower();

            //虚拟订单-撤销订单传true。其余为false
            ViewBag.isVirtualOrder = string.IsNullOrEmpty(Request["isVirtualOrder"]) || Request["isVirtualOrder"] == "False" || Request["isVirtualOrder"] == "false" ? false : true;
            //期号-北京单场和传统足球传参，其余为""
            if (ViewBag.GameCode == "ctzq" || ViewBag.GameCode == "bjdc")
            {
                string prizedIssuse = WCFClients.GameClient.QueryStopIssuseList(ViewBag.GameCode, ViewBag.GameType, 20, UserToken);
                var prizedIssuseList = prizedIssuse.Split(',');
                ViewBag.IssuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? prizedIssuseList.FirstOrDefault() : Request["issuseNumber"];
            }
            else
                ViewBag.IssuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];

            //搜索关键字
            ViewBag.Key = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"].ToString();

            //单式上传为1.复式为0，其余为null
            ViewBag.SchemeType = Request["schemeType"];
            SchemeBettingCategory? schemeType = null;
            if (!string.IsNullOrEmpty(Request["schemeType"]))
            {
                schemeType = (SchemeBettingCategory)int.Parse(Request["schemeType"]);
            }
            ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? 0 : int.Parse(Request.QueryString["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 20 : int.Parse(Request.QueryString["pageSize"]);

            //竞彩篮球，竞彩足球，显示当前日期。其余为""
            //string ComplateDate = "";
            //if (string.IsNullOrEmpty(ViewBag.IssuseNumber) && (ViewBag.GameCode != "ctzq" && ViewBag.GameCode != "bjdc"))
            //    ComplateDate = DateTime.Now.ToString("yyyyMMdd");

            //是否显示我的过关-true为显示-其余为false
            ViewBag.IsShowMine = string.IsNullOrEmpty(Request["IsShowMine"]) || Request["IsShowMine"] == "false" || Request["IsShowMine"] == "False" ? false : true;
            if (ViewBag.IsShowMine && CurrentUser != null)
                ViewBag.Key = CurrentUser.LoginInfo.DisplayName;

            var startTime = DateTime.Now.AddDays(-7);
            var endTime = DateTime.Now;
            ViewBag.TojiList = WCFClients.GameQueryClient.QueryReportInfoList_GuoGuan(ViewBag.isVirtualOrder, schemeType, ViewBag.Key, ViewBag.GameCode, ViewBag.GameType, ViewBag.IssuseNumber, startTime, endTime, ViewBag.PageIndex, ViewBag.PageSize);


            return View();
        }
    }
}
