using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using GameBiz.Core;
using Common.Utilities;

namespace app.lottery.site.Controllers
{
    public class BonusController : BaseController
    {
        /// <summary>
        /// 中奖排行榜
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        public PartialViewResult Detail(string id)
        {
            id = string.IsNullOrEmpty(id) ? "ljzj" : id;
            ViewBag.Type = id;
            ViewBag.Game = string.IsNullOrEmpty(Request["gamecode"]) ? "jczq" : Request["gamecode"].ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"].ToLower();
            var date = string.IsNullOrEmpty(Request["date"]) ? 7 : int.Parse(Request["date"]);
            ViewBag.BeginTime = DateTime.Now.AddDays(-date);
            ViewBag.EndTime = DateTime.Now.AddDays(1);
            // ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"];
            ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? this.PageIndex : int.Parse(Request.QueryString["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.User = CurrentUser;
            switch (id)
            {
                case "djph":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BigBonus_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "fdyl":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "gdyl":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_JoinProfit_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "hmrq":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_HotTogether(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "cgzj":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_SuccessOrder_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "zdgd":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_BeFollowerCount(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "ljzj":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_TotalBonus_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
            }
            return PartialView("detail/" + id);
        }

        //纯中奖列表
        public ActionResult bonuslist(string id)
        {
            ViewBag.User = CurrentUser;
            var game = string.IsNullOrEmpty(id) ? "jczq" : id.ToLower();
            var type = string.IsNullOrEmpty(Request["type"]) ? "" : Request["type"];
            var key = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"];
            var completeData = string.IsNullOrEmpty(Request["completeData"]) ? "" : Request["completeData"];
            var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
            //if (game == "bjdc")
            //{
            //    //var issuseList = Json_BJDC.IssuseList(Server).Select(p => p.IssuseNumber).ToList();
            //    //ViewBag.IssuseList = issuseList;
            //  //  issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? (issuseList.Count > 0 ? issuseList[0] : string.Empty) : Request["issuseNumber"];
            //}
            if (new string[] { "ssq", "dlt", "fc3d", "pl3" }.Contains(game))
            {
                string prizedIssuse = WCFClients.GameClient.QueryPrizedIssuseList(ViewBag.Game, ViewBag.Type, 20, UserToken);
                ViewBag.IssuseList = prizedIssuse.Split(',').ToList();
            }

            ViewBag.Game = game;
            ViewBag.Type = type;
            ViewBag.IssuseNum = issuseNumber;
            ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? 0 : int.Parse(Request.QueryString["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.BonusList = WCFClients.GameQueryClient.QueryBonusInfoList("", ViewBag.Game, type, issuseNumber, completeData, key, ViewBag.PageIndex, ViewBag.PageSize, UserToken);
            ViewBag.Key = string.IsNullOrEmpty(key) ? "输入用户名或方案号" : key;
            ViewBag.CompleteData = completeData;
            return View();
        }

        /// <summary>
        /// 新中奖排行榜
        /// </summary>
        public ActionResult NewIndex(string id)
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        public PartialViewResult NewDetail(string id)
        {
            id = string.IsNullOrEmpty(id) ? "fdyl" : id;
            ViewBag.Type = id;
            ViewBag.Game = string.IsNullOrEmpty(Request["gamecode"]) ? "jczq" : Request["gamecode"].ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"].ToLower();
            var date = string.IsNullOrEmpty(Request["date"]) ? 7 : int.Parse(Request["date"]);
            ViewBag.BeginTime = DateTime.Now.AddDays(-date);
            ViewBag.EndTime = DateTime.Now.AddDays(1);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? this.PageIndex : int.Parse(Request.QueryString["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.User = CurrentUser;
            switch (id)
            {
                case "djph":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BigBonus_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "fdyl":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "gdyl":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_JoinProfit_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "hmrq":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_HotTogether(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "cgzj":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_SuccessOrder_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "zdgd":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_BeFollowerCount(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
                case "ljzj":
                    ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_TotalBonus_Sport(ViewBag.BeginTime, ViewBag.EndTime, ViewBag.Game, ViewBag.GameType, ViewBag.PageIndex, ViewBag.PageSize);
                    break;
            }
            return PartialView("newdetail/" + id);
        }
    }
}
