using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameBiz.Core;

namespace app.lottery.site.Controllers
{
    public class lotteryController : BaseController
    {
        /// <summary>
        /// 开奖信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            id = string.IsNullOrEmpty(id) ? "index" : id;
            ViewBag.GameCode = id;
            //if (id != "index" && GameList.Where(a => a.GameCode.ToLower() == id.ToLower()).Count() < 1)
            //{
            //    throw new HttpException(404, "彩种不存在 - " + id);
            //}

            return View();
        }
        /// <summary>
        /// 部分试图
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Detail(string id)
        {
            id = string.IsNullOrEmpty(id) ? "index" : id;
            if (id == "index")
            {
                try
                {
                    ViewBag.AwardList = WCFClients.ChartClient.QueryAllGameNewWinNumber("CQ11X5|GD11X5|JX11X5|SD11X5|GDKLSF|HNKLSF|JLK3|JSKS|HBK3|CQSSC|JXSSC|SDQYH|SSQ|DLT|FC3D|PL3|PL5|DF6J1|HC1|HD15X5|QLC|QXC|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ");
                }
                catch
                {
                    ViewBag.AwardList = new LotteryData.Core.GameWinNumber_InfoCollection();
                }
            }
            else
            {

                try
                {
                    var queryGameString = id;
                    //查询传统足球开奖信息
                    var gameType = string.IsNullOrEmpty(Request["gameType"]) ? "t14c" : Request["gameType"];
                    if (id.ToLower() == "ctzq")
                    {
                        queryGameString = "ctzq_" + gameType;
                    }

                    if (!string.IsNullOrEmpty(Request["issusenumber"]))
                    {
                        ViewBag.IssuseNum = Request["issusenumber"];
                        ViewBag.Award = WCFClients.ChartClient.QueryNumber(queryGameString.ToUpper(), Request["issusenumber"]);

                    }
                    else
                    {
                        var win = WCFClients.ChartClient.QueryNewWinNumber(queryGameString.ToUpper());
                        ViewBag.Award = win;
                        ViewBag.IssuseNum = win == null ? "" : win.IssuseNumber;
                    }
                    ViewBag.CurIssuse = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(id);
                }
                catch
                {
                    ViewBag.Award = new WinNumber_QueryInfo() { WinNumber = "", GameCode = id, IssuseNumber = "" };
                }
            }
            return PartialView("detail/" + id);
        }
        /// <summary>
        /// 传统足球开奖详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ctzq(string id)
        {
            ViewBag.Type = string.IsNullOrEmpty(id) ? "t14c" : id.ToLower();
            return View();
        }
        public PartialViewResult ctzqmatchlist()
        {
            ViewBag.Type = string.IsNullOrEmpty(Request["Gametype"]) ? "" : Request["Gametype"];
            ViewBag.issuseNum = string.IsNullOrEmpty(Request["issueNum"]) ? "" : Request["issueNum"].ToString();
            var type = ViewBag.Type;
            var issuseNum = ViewBag.issuseNum;
            ViewBag.match = WCFClients.GameClient.QueryCTZQMatchListByIssuseNumber(ViewBag.Type, ViewBag.issuseNum, UserToken);
            return PartialView();
        }
        //北单-开奖信息
        public ActionResult bjdc(string id)
        {
            ViewBag.Type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public PartialViewResult bjdcmatchlist()
        {
            ViewBag.IssuseNum = string.IsNullOrEmpty(Request["issueNum"]) ? "" : Request["issueNum"];
            ViewBag.Type = string.IsNullOrEmpty(Request["Gametype"]) ? "" : Request["Gametype"];
            ViewBag.MathList = WCFClients.GameIssuseClient.QueryBJDC_MatchResultList(ViewBag.IssuseNum);
            return PartialView();
        }
        //竞彩足球-开奖查询
        public ActionResult jczq(string id)
        {
            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 1 : int.Parse(Request["oddstype"]);
            ViewBag.Type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.OddsType = oddtype;
            ViewBag.Begin = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddDays(-5) : Convert.ToDateTime(Request["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : Convert.ToDateTime(Request["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : int.Parse(Request["pageSize"]);

            var Match = WCFClients.GameIssuseClient.QueryJCZQMatchResult(ViewBag.Begin, ViewBag.End, ViewBag.pageIndex, ViewBag.PageSize);

            ViewBag.Match = Match;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        //竞彩篮球-开奖查询
        public ActionResult jclq(string id)
        {
            var type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.Type = type;
            var oddsType = string.IsNullOrEmpty(Request["oddsType"]) ? "1" : Request["oddsType"];
            ViewBag.OddsType = oddsType;
            ViewBag.Begin = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddDays(-5) : Convert.ToDateTime(Request["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : Convert.ToDateTime(Request["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : int.Parse(Request["pageSize"]);

            var Match = WCFClients.GameIssuseClient.QueryJCLQMatchResult(ViewBag.Begin, ViewBag.End, ViewBag.pageIndex, ViewBag.PageSize);

            ViewBag.Match = Match;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        //历史开奖页面
        public ActionResult history(string id)
        {
            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "CQSSC" : id.ToUpper();
            ViewBag.GameType = string.IsNullOrEmpty(Request.QueryString["gameType"]) ? "" : Request.QueryString["gameType"].ToUpper();
            var starttime = string.IsNullOrEmpty(Request["s"])
                ? DateTime.Now.AddMonths(-1)
                : Convert.ToDateTime(Request["s"]);
            var endtime = string.IsNullOrEmpty(Request["e"])
                ? DateTime.Now
                : Convert.ToDateTime(Request["e"]);
            if (endtime > DateTime.Now)
            {
                endtime = DateTime.Now;
            }
            if (starttime < Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")) || starttime > endtime)
            {
                starttime = DateTime.Now.AddMonths(-1);
            }

            ViewBag.QueryTime = starttime.ToString("yyyy-MM-dd");
            ViewBag.QueryEndTime = endtime.ToString("yyyy-MM-dd");
            var strGameCode = ViewBag.GameCode;
            if (!string.IsNullOrEmpty(ViewBag.GameType))
            {
                strGameCode = ViewBag.GameCode + "_" + ViewBag.GameType;
            }
            ViewBag.NumberHistoryList = WCFClients.GameIssuseClient.QueryWinNumberHistory(strGameCode, starttime, endtime.AddDays(1), 0, this.MaxIssuseCount(id));
            return View();
        }

        #region 新开奖结果
        /// <summary>
        /// 开奖信息
        /// </summary>
        public ActionResult NewIndex(string id)
        {
            id = string.IsNullOrEmpty(id) ? "newindex" : id;
            ViewBag.GameCode = id;
            return View();
        }
        /// <summary>
        /// 部分试图
        /// </summary>
        public PartialViewResult NewDetail(string id,string issuseNumber)
        {
            id = string.IsNullOrEmpty(id) ? "newindex" : id;
            if (id == "newindex")
            {
                try
                {
                    ViewBag.AwardList = WCFClients.ChartClient.QueryAllGameNewWinNumber("CQ11X5|GD11X5|JX11X5|SD11X5|GDKLSF|HNKLSF|JLK3|JSKS|HBK3|CQSSC|JXSSC|SDQYH|SSQ|DLT|FC3D|PL3|PL5|DF6J1|HC1|HD15X5|QLC|QXC|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ");
                }
                catch
                {
                    ViewBag.AwardList = new LotteryData.Core.GameWinNumber_InfoCollection();
                }
            }
            else
            {

                try
                {
                    var queryGameString = id;
                    //查询传统足球开奖信息
                    //var gameType = string.IsNullOrEmpty(Request["gameType"]) ? "t14c" : Request["gameType"];
                    //if (id.ToLower() == "ctzq")
                    //{
                    //    queryGameString = "ctzq_" + gameType;
                    //    ViewBag.GameType = gameType;
                    //}

                    if (!string.IsNullOrEmpty(issuseNumber))
                    {
                        //ViewBag.IssuseNum = Request["issusenumber"];
                        ViewBag.Award = WCFClients.ChartClient.QueryNumber(queryGameString.ToUpper(), issuseNumber);

                    }
                    else
                    {
                        var win = WCFClients.ChartClient.QueryNewWinNumber(queryGameString.ToUpper());
                        ViewBag.Award = win;
                        //ViewBag.IssuseNum = win == null ? "" : win.IssuseNumber;
                    }
                    ViewBag.CurIssuse = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(id);
                }
                catch
                {
                    ViewBag.Award = new WinNumber_QueryInfo() { WinNumber = "", GameCode = id, IssuseNumber = "" };
                }
            }
            return PartialView("newdetail/" + id);
        }
        /// <summary>
        /// 传统足球开奖详情
        /// </summary>
        public ActionResult newctzq(string id)
        {
            ViewBag.Type = string.IsNullOrEmpty(id) ? "t14c" : id.ToLower();
            return View();
        }
        public PartialViewResult newctzqmatchlist()
        {
            ViewBag.Type = string.IsNullOrEmpty(Request["Gametype"]) ? "" : Request["Gametype"];
            ViewBag.issuseNum = string.IsNullOrEmpty(Request["issueNum"]) ? "" : Request["issueNum"].ToString();
            var type = ViewBag.Type;
            var issuseNum = ViewBag.issuseNum;
            ViewBag.match = WCFClients.GameClient.QueryCTZQMatchListByIssuseNumber(ViewBag.Type, ViewBag.issuseNum, UserToken);
            return PartialView();
        }
        //竞彩足球-开奖查询
        public ActionResult newjczq(string id)
        {
            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 1 : int.Parse(Request["oddstype"]);
            ViewBag.Type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.OddsType = oddtype;
            ViewBag.Begin = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddDays(-5) : Convert.ToDateTime(Request["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : Convert.ToDateTime(Request["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : int.Parse(Request["pageSize"]);

            var Match = WCFClients.GameIssuseClient.QueryJCZQMatchResult(ViewBag.Begin, ViewBag.End, ViewBag.pageIndex, ViewBag.PageSize);

            ViewBag.Match = Match;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        //竞彩篮球-开奖查询
        public ActionResult newjclq(string id)
        {
            var type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.Type = type;
            var oddsType = string.IsNullOrEmpty(Request["oddsType"]) ? "1" : Request["oddsType"];
            ViewBag.OddsType = oddsType;
            ViewBag.Begin = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddDays(-5) : Convert.ToDateTime(Request["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : Convert.ToDateTime(Request["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : int.Parse(Request["pageSize"]);

            var Match = WCFClients.GameIssuseClient.QueryJCLQMatchResult(ViewBag.Begin, ViewBag.End, ViewBag.pageIndex, ViewBag.PageSize);

            ViewBag.Match = Match;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        //北单-开奖信息
        public ActionResult newbjdc(string id)
        {
            ViewBag.Type = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        public PartialViewResult newbjdcmatchlist()
        {
            ViewBag.IssuseNum = string.IsNullOrEmpty(Request["issueNum"]) ? "" : Request["issueNum"];
            ViewBag.Type = string.IsNullOrEmpty(Request["Gametype"]) ? "" : Request["Gametype"];
            ViewBag.MathList = WCFClients.GameIssuseClient.QueryBJDC_MatchResultList(ViewBag.IssuseNum);
            return PartialView();
        }

        #endregion
    }
}
