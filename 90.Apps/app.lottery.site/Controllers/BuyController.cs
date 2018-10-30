using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using app.lottery.site.cbbao.Models;
using app.lottery.site.jsonManager;
using External.Client;
using GameBiz.Core;
using System.Net;
using Common.Utilities;
using System.IO;
using System.Text;
using System.Web.UI;
using Common.Net;
using System.Threading;
using Common.Log;
using MatchBiz.Core;
using Common.JSON;
using Common.Communication;
using Common.Lottery;
using Common.XmlAnalyzer;
using app.lottery.site.Models;
using Common.Snapshot;
using OrderLogWriter = app.lottery.site.Models.OrderLogWriter;
using app.lottery.site.iqucai;
using System.Threading.Tasks;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;

namespace app.lottery.site.Controllers
{
    public class BuyController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public BuyController(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion
        #region 数字彩

        public ActionResult Szc(string id)
        {
            var gameCode = string.IsNullOrEmpty(id) ? "CQSSC" : id.ToUpper();
            if (gameCode == "SSC")
                gameCode = "CQSSC";
            ViewBag.GameCode = gameCode;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;

            if (GameList.Where(a => a.GameCode.ToLower() == id.ToLower()).Count() < 1)
            {
                //throw new HttpException(404, "彩种不存在 - " + id);
            }
            //try
            //{
            //    var cur = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(ViewBag.GameCode);
            //    ViewBag.CurrentIssuse = cur;
            //    ViewBag.PreviewIssuse = BuildLastIssuseNumber(ViewBag.GameCode, cur.IssuseNumber);
            //    ViewBag.PreviewWinNumber = GetIssuseWinNumber(ViewBag.GameCode, ViewBag.PreviewIssuse);
            //    ViewBag.LastBig = WCFClients.GameQueryClient.QueryRankReport_BigBonus_Sport(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1), 30, ViewBag.GameCode, "", 0, 30);
            //}
            //catch
            //{
            //    throw new HttpException(404, "奖期不存在 - " + id);
            //}
            return View();
        }

        public ActionResult JingCai(string id)
        {

            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //是否是方案欲投
            var SchemeId = string.IsNullOrEmpty(Request["schemeid"]) ? "" : Request["schemeid"];
            var isYt = string.IsNullOrEmpty(Request["isYt"]) ? false : Convert.ToBoolean(int.Parse(Request["isYt"].ToLower()));
            if (isYt)
            {
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(SchemeId);
                ViewBag.Money = info.TotalMoney;
                ViewBag.UserId = info.UserId;
            }
            else
            {
                ViewBag.Money = 0M;
                ViewBag.UserId = "";
            }
            ViewBag.SchemeId = SchemeId;






            //查询大奖排行
            //var now = DateTime.Now;
            //var weekRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-7), now, 10, "JCZQ");
            //var monthRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-30), now, 10, "JCZQ");
            //var rank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-90), now, 10, "JCZQ");
            //ViewBag.WeekRank = weekRank;
            //ViewBag.MonthRank = monthRank;
            //ViewBag.Rank = rank;
            return View();
        }
        public ActionResult Danchang(string id)
        {
            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //是否是方案欲投
            var SchemeId = string.IsNullOrEmpty(Request["schemeid"]) ? "" : Request["schemeid"];
            var isYt = string.IsNullOrEmpty(Request["isYt"]) ? false : Convert.ToBoolean(int.Parse(Request["isYt"].ToLower()));
            if (isYt)
            {
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(SchemeId);
                ViewBag.Money = info.TotalMoney;
                ViewBag.UserId = info.UserId;
            }
            else
            {
                ViewBag.Money = 0M;
                ViewBag.UserId = "";
            }
            ViewBag.SchemeId = SchemeId;
            //查询大奖排行
            var now = DateTime.Now;
            var weekRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-7), now, 10,
                                                                                          "BJDC");
            var monthRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-30), now, 10,
                                                                                          "BJDC");
            var rank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-90), now, 10,
                                                                                          "BJDC");
            ViewBag.WeekRank = weekRank;
            ViewBag.MonthRank = monthRank;
            ViewBag.Rank = rank;
            return PartialView();
        }
        //北单-各玩法页
        public PartialViewResult Bjdc(string id)
        {
            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            var issue = PreconditionAssert.IsNotEmptyString(Request["isue"], "期号不能为空");
            var requestissue = Request["risue"];
            ViewBag.IsCurrent = issue != requestissue && !string.IsNullOrEmpty(requestissue);
            issue = string.IsNullOrEmpty(requestissue) ? issue : requestissue;
            var matches = Json_BJDC.MatchList_WEB(2, id, issue);
            ViewBag.Matches = matches;
            return PartialView("bjdc/" + id);
        }

        //竞彩篮球-投注页面
        public ActionResult Jingcaibasket(string id)
        {
            id = string.IsNullOrEmpty(id) ? "sf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //查询大奖排行
            var now = DateTime.Now;
            var weekRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-7), now, 10,
                                                                                          "JCLQ");
            var monthRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-30), now, 10,
                                                                                          "JCLQ");
            var rank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-90), now, 10,
                                                                                          "JCLQ");
            ViewBag.WeekRank = weekRank;
            ViewBag.MonthRank = monthRank;
            ViewBag.Rank = rank;
            return View();
        }

        public PartialViewResult Jclq(string id)
        {
            id = string.IsNullOrEmpty(id) ? "hh" : id.ToLower();
            if (!string.IsNullOrEmpty(id))
            {
                if ("hh" == id || "sf" == id || "rfsf" == id || "sfc" == id || "dxf" == id)
                {
                    int ot;
                    var oddtype = Request["ot"];
                    int.TryParse(oddtype, out ot);
                    ot = ot == 0 ? 2 : ot;
                    //读取数据
                    try
                    {
                        var match = Json_JCLQ.MatchList_WEB(id, ot);
                        if (match.Count > 0)
                        {
                            var minsaledate =
                                match.Where(p => Convert.ToDateTime(p.StartDateTime) > DateTime.Today)
                                    .Min(a => Convert.ToDateTime(a.StartDateTime));
                            var mindate = minsaledate.Hour < 12
                                ? minsaledate.AddDays(-1).ToString("yyyy-MM-dd")
                                : minsaledate.ToString("yyyy-MM-dd");
                            ViewBag.MinDate = Convert.ToDateTime(mindate);
                            var date = string.IsNullOrEmpty(Request["date"]) ? mindate : Request["date"];
                            ViewBag.Date = date;
                            if (date == mindate)
                            {
                                //match = match.Where(
                                //    a => DateTime.Parse(a.StartDateTime) > DateTime.Parse(mindate).AddHours(12))
                                //    .ToList();
                            }
                            else
                            {
                                var matchDate = date.Replace("-", "").Remove(0, 2);
                                match = Json_JCLQ.MatchList_WEB(id, ot, matchDate);
                                //match = match.Where(a => a.StartDateTime > DateTime.Parse(date).AddHours(11) && a.StartDateTime <= DateTime.Parse(date).AddDays(1).AddHours(11)).ToList();
                            }
                            ViewBag.Match = match;
                        }
                        else
                        {
                            ViewBag.MinDate = DateTime.Now;
                            //ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                            ViewBag.Match = new List<JCZQ_MatchInfo_WEB>();
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.MinDate = DateTime.Now;
                        ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                        ViewBag.Match = new List<JCZQ_MatchInfo_WEB>();
                    }
                    ViewBag.Type = id;
                }
            }
            return PartialView("jclq/" + id);
        }
        public ActionResult Toto(string id)
        {
            id = string.IsNullOrEmpty(id) ? "t14c" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        public PartialViewResult Ctzq(string id)
        {
            id = string.IsNullOrEmpty(id) ? "t14c" : id.ToLower();
            var currentIssue = Request["isue"];
            var requestIssue = Request["risue"];
            var stoptime = Request["st"];
            ViewBag.CurrentIssue = currentIssue;
            currentIssue = string.IsNullOrEmpty(requestIssue) ? currentIssue : requestIssue;
            var matches = string.IsNullOrEmpty(currentIssue) ? new List<CTZQ_MatchInfo_WEB>() : Json_CTZQ.MatchList_WEB(currentIssue, id);
            ViewBag.Matches = matches;
            if (string.IsNullOrEmpty(stoptime))
            {
                stoptime = "0000-00-00 00:00:00";
            }
            ViewBag.StopTime = stoptime;

            return PartialView("ctzq/" + id);
        }
        //奖金计算说明
        public PartialViewResult Bonusdesc(string id)
        {
            id = string.IsNullOrEmpty(id) ? "SYX5" : id;
            if (id == "SD11X5" || id == "GD11X5" || id == "JX11X5")
            {
                id = "SYX5";
            }
            return PartialView("BonusDesc/" + id);
        }

        //中奖排行榜
        public PartialViewResult Bonustop(string id)
        {
            try
            {
                ViewBag.User = CurrentUser;
                var jcstr = "jczq,jclq";
                var szcstr = "ssq,dlt,fc3d,pl3,cqssc,jxssc,sd11x5,gd11x5,jx11x5,sdqyh,gdklsf";
                ViewBag.JCTops = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(1), 10, jcstr);
                ViewBag.BDTops = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(1), 10, "bjdc");
                ViewBag.CTZQTops = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(1), 10, "ctzq");
                ViewBag.SZCTops = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(1), 10, szcstr);
            }
            catch
            {
            }

            return PartialView();
        }



        //彩票相关文章
        public PartialViewResult Lotteryconsulting(string id)
        {
            ViewBag.Art = WCFClients.ExternalClient.QueryArticleList("", id, "INFO", 0, 10, UserToken);
            ViewBag.GameCode = id;
            return PartialView();
        }

        #region 投注页面ajax
        //彩种当前奖期信息
        public JsonResult Curissuse(string id)
        {
            try
            {
                //var list = base.LoadAllGameIssuse_RefreshByLocalStopTime();
                var gameInfo = base.QueryCurrentIssuseByLocalStopTime(id.ToUpper());
                //var gameInfo = list.FirstOrDefault(p => p.GameCode == id.ToUpper());
                if (gameInfo == null)
                    throw new Exception("查询奖期错误");
                return Json(new
                {
                    issuse = gameInfo.IssuseNumber,
                    bettime = ((gameInfo.OfficialStopTime - DateTime.Now).TotalSeconds - gameInfo.GameDelaySecond).ToString("0"),
                    awardtime = (gameInfo.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0"),
                    stoptime = gameInfo.LocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    stopdate = gameInfo.OfficialStopTime.ToString(),
                    nowsdate = DateTime.Now.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                base.WriteLog("buy-Curissuse", LogType.Error, "查询当前奖期出错", ex.ToString());
                return Json(new { issuse = "-", bettime = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        //查询彩种最新开奖号码
        public JsonResult GetNewWinNumber(string id)
        {
            try
            {
                var cur = WCFClients.GameIssuseClient.GetNewWinNumber(id, string.Empty);
                return Json(new
                {
                    issuse = cur.IssuseNumber,
                    winNumber = cur.WinNumber
                },
                    JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { issuse = "-", bettime = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        //幸运选号-当前奖期信息
        public JsonResult ajax_current(string id)
        {
            try
            {
                var cur = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(id);
                return Json(new { code = 0, message = "", issuse = cur.IssuseNumber, bettime = (cur.LocalStopTime - DateTime.Now).TotalSeconds.ToString("0"), awardtime = (cur.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 0, message = ex.Message, issuse = "-", bettime = 0, JsonRequestBehavior.AllowGet });
            }
        }

        //幸运选号
        public JsonResult ajax_lucky_numbers(string id)
        {
            try
            {
                var cur = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(id);
                return Json(new { code = 0, message = "", issuse = cur.IssuseNumber, bettime = (cur.LocalStopTime - DateTime.Now).TotalSeconds.ToString("0"), awardtime = (cur.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 0, message = ex.Message, issuse = "-", bettime = 0, JsonRequestBehavior.AllowGet });
            }
        }

        //获取奖期信息
        public JsonResult LastWinInfo(string id)
        {
            try
            {
                var cur = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(id);
                if (cur != null)
                {
                    var pre = BuildLastIssuseNumber(id, cur.IssuseNumber);
                    return Json(new { pre = pre, win = GetIssuseWinNumber(id, pre), now = cur.IssuseNumber, time = (cur.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0") }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { pre = "-", win = string.Empty, now = "-", time = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { pre = "-", win = string.Empty, now = "-", time = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        //开奖公告子视图页面
        public PartialViewResult AwardList(string id)
        {
            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "CQSSC" : id;
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 5 : int.Parse(Request.QueryString["pageSize"]);
            DateTime begin = DateTime.Today.AddDays(-1);
            if (ViewBag.GameCode == "FC3D" || ViewBag.GameCode == "PL3")
            {
                begin = DateTime.Today.AddDays(-50);
            }
            else if (ViewBag.GameCode == "DLT" || ViewBag.GameCode == "SSQ")
            {
                begin = DateTime.Today.AddYears(-1);
            }
            ViewBag.NumberHistoryList = WCFClients.GameIssuseClient.QueryWinNumberHistory(ViewBag.GameCode, begin, DateTime.Today.AddDays(1), ViewBag.pageNo, ViewBag.PageSize);
            return PartialView();
        }

        //获取彩种和玩法相应的遗漏冷热数据
        public JsonResult Losehot(string id)
        {
            try
            {
                var type = PreconditionAssert.IsNotEmptyString(Request["type"], "获取信息类型不能为空");
                var col = HotLoseList(id);
                return Json(new { IsSuccess = true, Message = col[type] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //开奖详情
        public PartialViewResult LotteryDetail(string game, string gameName)
        {
            ViewBag.Game = game;
            ViewBag.GameName = gameName;
            return PartialView();
        }

        #region 各彩种玩法页面

        public PartialViewResult Cqssc(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "1XDX" : id;
            return PartialView("CQSSC/" + gameType);
        }

        public PartialViewResult Jxssc(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "1XDX" : id;
            return PartialView("JXSSC/" + gameType);
        }

        public PartialViewResult Sd11X5(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "RX1" : id;
            return PartialView("SYX5/" + gameType);
        }

        public PartialViewResult Gd11X5(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "RX1" : id;
            return PartialView("SYX5/" + gameType);
        }

        public PartialViewResult Jx11X5(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "RX1" : id;
            return PartialView("SYX5/" + gameType);
        }

        public PartialViewResult Fc3D(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "FS_Group" : id;
            return PartialView("FCPL/" + gameType);
        }

        public PartialViewResult Pl3(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "FS_Group" : id;
            return PartialView("FCPL/" + gameType);
        }

        public PartialViewResult Sdqyh(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "RX1" : id;
            return PartialView("SDQYH/" + gameType);
        }

        public PartialViewResult Gdklsf(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "X2LZU_Group" : id;
            return PartialView("GDKLSF/" + gameType);
        }

        public PartialViewResult Gxklsf(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "ZXHYT" : id;
            return PartialView("GXKLSF/" + gameType);
        }

        public PartialViewResult Ssq(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "BZXH" : id;
            return PartialView("SSQ/" + gameType);
        }

        public PartialViewResult Dlt(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "PTTZ" : id;
            return PartialView("DLT/" + gameType);
        }

        public PartialViewResult Jsk3(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "2THDX" : id;
            return PartialView("JSK3/" + gameType);
        }

        public PartialViewResult Jsks(string id)
        {
            string gameType = string.IsNullOrEmpty(id) ? "2THDX" : id;
            return PartialView("JSKS/" + gameType);
        }
        //竞彩足球-各玩法页
        public PartialViewResult Jczq(string id)
        {
            var writer = Common.Log.LogWriterGetter.GetLogWriter();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = id.ToLower();
                    if ("hh" == id || "zjq" == id || "bqc" == id || "bf" == id || "exy" == id)
                    {
                        int ot;
                        var oddtype = Request["ot"];
                        int.TryParse(oddtype, out ot);
                        ot = ot == 0 ? 2 : ot;
                        //读取数据
                        try
                        {
                            var match = Json_JCZQ.MatchList_WEB(id, ot);
                            writer.Write("match1", "PartialViewResult", Common.Log.LogType.Error, string.Format("读取数据MatchList_WEB", "match1"), match.LongCount().ToString());
                            if (match.Count > 0)
                            {
                                var minsaledate =
                                    match.Where(p => Convert.ToDateTime(p.StartDateTime) > DateTime.Today)
                                        .Min(a => Convert.ToDateTime(a.StartDateTime));
                                var mindate = minsaledate.Hour < 11
                                    ? minsaledate.AddDays(-1).ToString("yyyy-MM-dd")
                                    : minsaledate.ToString("yyyy-MM-dd");
                                ViewBag.MinDate = Convert.ToDateTime(mindate);
                                var date = string.IsNullOrEmpty(Request["date"]) ? mindate : Request["date"];
                                ViewBag.Date = date;
                                if (date == mindate)
                                {
                                    //match = match.Where(
                                    //    a => DateTime.Parse(a.StartDateTime) > DateTime.Parse(mindate).AddHours(11))
                                    //    .ToList();
                                }
                                else
                                {
                                    var matchDate = date.Replace("-", "").Remove(0, 2);
                                    match = Json_JCZQ.MatchList_WEB(id, ot, matchDate);
                                    //match = match.Where(a => a.StartDateTime > DateTime.Parse(date).AddHours(11) && a.StartDateTime <= DateTime.Parse(date).AddDays(1).AddHours(11)).ToList();
                                }
                                ViewBag.Match = match;
                                writer.Write("match2", "PartialViewResult", Common.Log.LogType.Error, string.Format("读取数据{0}", "match2"), match.LongCount().ToString());
                            }
                            else
                            {
                                ViewBag.MinDate = DateTime.Now;
                                ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                                ViewBag.Match = new List<JCZQ_MatchInfo_WEB>();
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.MinDate = DateTime.Now;
                            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                            ViewBag.Match = new List<JCZQ_MatchInfo_WEB>();

                            writer.Write("ERROR_PartialViewResult", "PartialViewResult", Common.Log.LogType.Error, string.Format("读取数据{0}", "match3"), ex.ToString());
                        }
                        ViewBag.Type = id;
                    }
                }
            }
            catch (Exception ex)
            {

                writer.Write("ERROR_PartialViewResult", "PartialViewResult", Common.Log.LogType.Error, string.Format("读取数据{0}", "match3"), ex.ToString());
                throw;
            }



            return PartialView("jczq/" + id);
        }
        /// <summary>
        /// 欧洲杯
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetOZBMatchJson(string id)
        {
            List<JCZQ_OZBMatchInfo> list = Json_JCZQ.OZBMatchList(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 世界杯
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSJBMatchJson(string id)
        {
            string photourl = ConfigurationManager.AppSettings["ResourceSiteUrl"].ToString();
            string data = string.Empty;
            if (id.ToUpper() == "GJ")
            {
                List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList(id);
                foreach (var item in list)
                {
                    string image = string.Format("{0}/images/football/{1}.png", photourl, item.Team);
                    data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                        item.Probadbility, image);
                }
                return "cphData({\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467130\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"CHP\"}]});";
            }
            else if (id.ToUpper() == "GYJ")
            {
                List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList(id);
                foreach (var item in list)
                {
                    if (item.Team != "其它")
                    {
                        var arr = item.Team.Split('—');
                        string image1 = string.Format("{0}/images/football/{1}.png", photourl, arr[0]);
                        string image2 = string.Format("{0}/images/football/{1}.png", photourl, arr[1]);
                        data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}—{7}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                            item.Probadbility, image1, image2);
                    }
                    else
                    {
                        data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-nopic|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                            item.Probadbility);

                    }

                }
                return "getList({\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467131\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"FNL\"}]});";
            }
            else if (id == "GYJB")
            {
                List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList("GYJ");
                foreach (var item in list)
                {
                    if (item.Team != "其它")
                    {
                        var arr = item.Team.Split('—');
                        string image1 = string.Format("{0}/images/football/{1}.png", photourl, arr[0]);
                        string image2 = string.Format("{0}/images/football/{1}.png", photourl, arr[1]);
                        data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}—{7}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                            item.Probadbility, image1, image2);
                    }
                    else
                    {
                        data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-nopic|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                            item.Probadbility);

                    }

                }
                return "getBonus({\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467131\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"FNL\"}]});";
            }
            else
            {
                return string.Empty;
            }
        }

        //竞彩足球单式-各玩法页
        public PartialViewResult Jczqds(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = "spf";
            }
            if ("spf" == id || "rqspf" == id || "zjq" == id || "bqc" == id || "bf" == id)
            {
                //读取数据
                try
                {
                    //var matches = new List<JczqWeb>();
                    var match = Json_JCZQ.MatchList_WEB(id, 3);
                    if (match.Count > 0)
                    {
                        var minsaledate =
                            match.Where(p => Convert.ToDateTime(p.StartDateTime) > DateTime.Today)
                                .Min(a => Convert.ToDateTime(a.StartDateTime));
                        var mindate = minsaledate.Hour < 11
                            ? minsaledate.AddDays(-1).ToString("yyyy-MM-dd")
                            : minsaledate.ToString("yyyy-MM-dd");
                        match = match.Where(
                            a => DateTime.Parse(a.StartDateTime) > DateTime.Parse(mindate).AddHours(11))
                            .ToList();
                        //开售的比赛
                        match = match.Where(p => Convert.ToDateTime(p.DSStopBettingTime) > DateTime.Now).ToList();
                        //过滤掉比赛中有赔率停售的场次
                        switch (id.ToLower())
                        {
                            case "spf":
                                match = match.Where(item => item.BRQSPF.NoSaleState == "0").ToList();
                                break;
                            case "rqspf":
                                match = match.Where(item => item.SPF.NoSaleState == "0").ToList();
                                break;
                            case "zjq":
                                match = match.Where(item => item.ZJQ.NoSaleState == "0").ToList();
                                break;
                            case "bqc":
                                match = match.Where(item => item.BQC.NoSaleState == "0").ToList();
                                break;
                            case "bf":
                                match = match.Where(item => item.BF.NoSaleState == "0").ToList();
                                break;
                        }

                        ViewBag.Match = match;
                    }
                    else
                    {
                        //ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                        ViewBag.Match = new List<JczqWeb>();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Match = new List<JczqWeb>();
                }
            }
            ViewBag.Type = id;
            return PartialView("jczq/jczqds");
        }
        //北京单场单式-各玩法页
        public PartialViewResult Bjdcds(string id)
        {
            var matches = new List<BJDC_MatchInfo_WEB>(); if (!string.IsNullOrEmpty(id))
            {
                id = "spf";
            }
            var issue = string.Empty;
            if ("spf" == id || "zjq" == id || "sxds" == id || "bqc" == id || "bf" == id)
            {
                //读取数据
                try
                {
                    //期数列表
                    var issuses = Json_BJDC.IssuseList().OrderByDescending(o => o.IssuseNumber).ToList();
                    if (issuses.Count > 0)
                    {
                        issue = issuses[0].IssuseNumber;
                        matches = Json_BJDC.MatchList_WEB(3, id, issue);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewBag.CurIssue = issue;
            ViewBag.Matches = matches;
            ViewBag.Type = id;
            return PartialView("bjdc/bjdcds");
        }

        #endregion

        #endregion

        /// <summary>
        /// 欧洲冠军，欧洲冠亚军
        /// </summary>
        /// <returns></returns>
        public JsonResult bet_ozb()
        {
            try
            {
                //return Json(new { IsSuccess = false, Message = "测试服务器" },JsonRequestBehavior.AllowGet);
                #region Request参数

                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "彩种编码不能为空。");
                var number = PreconditionAssert.IsNotEmptyString(Request["Number"], "投注号码不能为空。");
                var issuseNumberArray = Request["IssuseNumber"];
                string curIssuse = Request["CurrentIssuse"];
                var balancepwd = Request["balancepwd"];
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                bool isStopAfterBonus = string.IsNullOrEmpty(Request["IsStopAfterBonus"]) ? false : bool.Parse(Request["IsStopAfterBonus"]);
                var amount = string.IsNullOrEmpty(Request["amount"]) ? 0 : int.Parse(Request["amount"]);
                var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var IsAppend = string.IsNullOrEmpty(Request["IsAppend"]) ? false : Boolean.Parse(Request["IsAppend"]);
                var activityType = 2;
                #endregion



                #region 普通投注
                //投注信息
                var info = new LotteryBettingInfo
                {
                    ActivityType = ActivityType.NoParticipate,
                    BettingCategory = SchemeBettingCategory.GeneralBetting,
                    CurrentBetTime = DateTime.Now,
                    GameCode = "OZB",
                    IsAppend = false,
                    IsRepeat = false,
                    IsSubmit = false,
                    SchemeSource = SchemeSource.Web,
                    Security = TogetherSchemeSecurity.Public,
                    StopAfterBonus = false,
                    TicketTime = DateTime.Now,
                    TotalMoney = totalMoney,
                };
                info.AnteCodeList.Add(new LotteryAnteCodeInfo
                {
                    GameType = gameType,
                    IsDan = false,
                    AnteCode = number,
                });
                info.IssuseNumberList.Add(new LotteryBettingIssuseInfo
                {
                    IssuseNumber = "2016",
                    Amount = amount,
                    IssuseTotalMoney = totalMoney,
                });

                var result = WCFClients.GameClient.BetOZB(info, "", 0, UserToken);
                return Json(result);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 数字彩、传统足球普通投注
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> Order()
        {
            try
            {
                #region Request参数
                var isFilter = string.IsNullOrEmpty(Request["isFilter"]) ? false : bool.Parse(Request["isFilter"]);
                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空。");
                var number = PreconditionAssert.IsNotEmptyString(Request["Number"], "投注号码不能为空。");
                var issuseNumberArray = Request["IssuseNumber"].Split('#');
                string curIssuse = Request["CurrentIssuse"];
                var balancepwd = Request["balancepwd"];
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                bool isStopAfterBonus = string.IsNullOrEmpty(Request["IsStopAfterBonus"]) ? false : bool.Parse(Request["IsStopAfterBonus"]);
                var amount = string.IsNullOrEmpty(Request["amount"]) ? 0 : int.Parse(Request["amount"]);
                var sercu = (EntityModel.Enum.TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var schemeCategory = isFilter ? EntityModel.Enum.SchemeBettingCategory.FilterBetting : EntityModel.Enum.SchemeBettingCategory.GeneralBetting;
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var IsAppend = string.IsNullOrEmpty(Request["IsAppend"]) ? false : Boolean.Parse(Request["IsAppend"]);
                var activityType = 2;
                if (gameCode.ToUpper() == "CTZQ")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }
                #endregion

                #region 投注号码
                EntityModel.CoreModel.LotteryAnteCodeInfoCollection codeList = new EntityModel.CoreModel.LotteryAnteCodeInfoCollection();
                foreach (var item in number.Split('#'))
                {
                    var codeArray = item.Split('.');
                    codeList.Add(new EntityModel.CoreModel.LotteryAnteCodeInfo
                    {
                        GameType = codeArray[0],
                        AnteCode = codeArray[1],
                    });
                }

                //合买投注号码对象
                var togAnteList = new EntityModel.CoreModel.Sports_AnteCodeInfoCollection();
                foreach (var item in number.Split('#'))
                {
                    var codeArray = item.Split('.');
                    var code = new EntityModel.CoreModel.Sports_AnteCodeInfo()
                    {
                        GameType = codeArray[0],
                        AnteCode = codeArray[1]
                    };
                    togAnteList.Add(code);
                }
                #endregion

                #region 追号信息，期号列表

                if (issuseNumberArray.Length <= 0)
                {
                    throw new Exception("投注没有包含期号信息。");
                }
                EntityModel.CoreModel.LotteryBettingIssuseInfoCollection issuseList = new EntityModel.CoreModel.LotteryBettingIssuseInfoCollection();
                foreach (var item in issuseNumberArray)
                {
                    var issuseArray = item.Split('|');
                    issuseList.Add(new EntityModel.CoreModel.LotteryBettingIssuseInfo()
                    {
                        IssuseNumber = issuseArray[0],
                        Amount = int.Parse(issuseArray[1]),
                        IssuseTotalMoney = decimal.Parse(issuseArray[2])
                    });
                }
                #endregion
                var param = new Dictionary<string, object>();
                #region 合买投注
                if (isHemai)
                {
                    //合买属性
                    var title = Request["title"];
                    var desc = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : Request["description"];
                    //var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                    //var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]); // 默认每份单价为1元
                    //updated by klr 2014-03-12 
                    const int price = 1; // 默认每份单价为1元
                    var totalCount = Convert.ToInt32(totalMoney); // 拆分为总金额的分数
                    var guarantees = string.IsNullOrEmpty(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]); //我要保底金额(原来为保底份数)
                    var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                    var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]); //我要认购金额(原来为认购份数)
                    var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]); //提成比例

                    var togInfo = new EntityModel.CoreModel.Sports_TogetherSchemeInfo
                    {
                        GameCode = gameCode,
                        GameType = codeList.FirstOrDefault().GameType,
                        IssuseNumber = issuseList.FirstOrDefault().IssuseNumber,
                        Amount = amount,
                        AnteCodeList = togAnteList,
                        PlayType = "",
                        SchemeSource = EntityModel.Enum.SchemeSource.Web,
                        TotalMoney = totalMoney,
                        TotalMatchCount = 14,
                        Title = title,
                        Description = desc,
                        TotalCount = totalCount,
                        Security = sercu,
                        Price = price,
                        BonusDeduct = bonusdeduct,
                        Guarantees = guarantees,
                        JoinPwd = joinpwd,
                        Subscription = subscription,
                        BettingCategory = schemeCategory,
                        ActivityType = (EntityModel.Enum.ActivityType)activityType,
                        IsAppend = IsAppend
                    };
                    
                    param.Add("info", togInfo);
                    param.Add("balancePassword", balancepwd);
                    param.Add("userid", UserToken);
                    var hmResult = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/CreateSportsTogether");

                    //清除当前用户过滤信息
                    //OrderLogWriter _logWriter = new OrderLogWriter(Server);
                    //_logWriter.UpdateFilterList(CurrentUser.LoginInfo.UserId);
                    return Json(hmResult);
                }
                #endregion

                #region 普通投注
                //投注信息
                EntityModel.CoreModel.LotteryBettingInfo info = new EntityModel.CoreModel.LotteryBettingInfo()
                {
                    GameCode = gameCode,
                    AnteCodeList = codeList,
                    IssuseNumberList = issuseList,
                    SchemeSource = EntityModel.Enum.SchemeSource.Web,
                    StopAfterBonus = isStopAfterBonus,
                    TotalMoney = totalMoney,
                    Security = sercu,
                    BettingCategory = schemeCategory,
                    ActivityType = (EntityModel.Enum.ActivityType)activityType,
                    IsAppend = IsAppend
                };
                param.Clear();
                param.Add("info", info);
                param.Add("balancePassword", balancepwd);
                param.Add("redBagMoney", 0M);
                param.Add("userid", UserToken);
                var saveparam = new Dictionary<string, object>();
                saveparam.Add("info", info);
                saveparam.Add("userid", UserToken);
                //如果是0表示免费保存订单
                var result = kind == 0 ? await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(saveparam, "api/Betting/SaveOrderLotteryBetting") : await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/LotteryBetting");
                if (kind != 0 && result.IsSuccess && !isHemai)//发送快照
                {
                    string schemeType = string.Empty;
                    if (issuseList.Count > 1)
                        schemeType = "2";
                    else if (issuseList.Count == 1)
                        schemeType = "1";
                    SendSchemeSnapshotToEmail(schemeType, result.ReturnValue.Split('|')[0], gameCode, UserToken);
                }

                //#region 日志
                //if (result.IsSuccess)
                //{
                //    try
                //    {
                //        OrderLogWriter _logWriter = new OrderLogWriter(Server);
                //        string log = "用户来源:" + CurrentUser.LoginInfo.LoginFrom + " 登录名:" + CurrentUser.LoginInfo.LoginName + " 显示名称:" + CurrentUser.LoginInfo.DisplayName;
                //        log = log + "\r\n";
                //        log = log + "\r\n    方案编号:" + result.ReturnValue + "   总额:" + info.TotalMoney.ToString("0.00") + "元";
                //        log = log + "\r\n";
                //        log = log + "\r\n    投注号码:";
                //        foreach (var item in codeList)
                //        {
                //            log = log + "[" + gameCode + "|" + item.GameType + "|" + item.AnteCode + "]  ";
                //        }
                //        log = log + "\r\n";
                //        log = log + "\r\n    追号期数：";
                //        foreach (var issuseItem in issuseList)
                //        {
                //            log = log + "[" + issuseItem.IssuseNumber + "  " + issuseItem.Amount + "倍  " + issuseItem.IssuseTotalMoney.ToString("f") + "元] ";
                //        }
                //        _logWriter.Write(CurrentUser.LoginInfo.UserId, result.ReturnValue, LogType.Information, "来源IP：" + IpManager.IPAddress, log);

                //        //清除当前用户过滤信息
                //        _logWriter.UpdateFilterList(CurrentUser.LoginInfo.UserId);
                //    }
                //    catch
                //    {
                //    }
                //}
                //#endregion

                return Json(result);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //北单、竞彩足球、竞彩篮球-投注函数
        [HttpPost]
        public async Task<JsonResult> bet_sports()
        {
            try
            {
                #region Request参数
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["game"], "彩种编码不能为空");
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空。");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空。");
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                var antecode = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
                var amount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空。"));
                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var matchcount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["matchcount"], "投注场数不能为空。"));
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];
                var sercu = (EntityModel.Enum.TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var activityType = 2;
                var isexy = string.IsNullOrEmpty(Request["isExy"]) ? false : Boolean.Parse(Request["isExy"]);
                if (gameCode.ToUpper() == "JCZQ" || gameCode.ToUpper() == "BJDC")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                #endregion

                #region 投注号码
                //投注号码对象
                var anteCodeList = new EntityModel.CoreModel.Sports_AnteCodeInfoCollection();
                var codeArray = antecode.Split('#');
                foreach (var item in codeArray)
                {
                    var cods = item.Split('|');
                    var code = new EntityModel.CoreModel.Sports_AnteCodeInfo()
                    {
                        IsDan = bool.Parse(cods[0]),
                        MatchId = cods[1],
                        AnteCode = cods[2],
                    };
                    if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf") && cods.Length > 3)
                    {
                        code.GameType = cods[3];
                    }
                    else
                    {
                        code.GameType = gameType;
                    }
                    anteCodeList.Add(code);
                }

                //投注过关方式
                playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                #endregion
                var param = new Dictionary<string, object>();
                #region 合买投注
                if (isHemai)
                {
                    //合买属性
                    var title = Request["title"];
                    var desc = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : Request["description"];
                    var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                    var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]); // 默认每份单价为1元
                    var guarantees = string.IsNullOrEmpty(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]); //我要保底份数
                    var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                    var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]); //我要认购份数
                    var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]); //提成比例

                    EntityModel.CoreModel.Sports_TogetherSchemeInfo togInfo = new EntityModel.CoreModel.Sports_TogetherSchemeInfo()
                    {
                        GameCode = gameCode,
                        GameType = gameType,
                        IssuseNumber = issuseNumber,
                        Amount = amount,
                        AnteCodeList = anteCodeList,
                        PlayType = playType,
                        SchemeSource = EntityModel.Enum.SchemeSource.Web,
                        TotalMoney = totalMoney,
                        TotalMatchCount = matchcount,
                        Title = title,
                        Description = desc,
                        TotalCount = totalCount,
                        Security = sercu,
                        Price = price,
                        BonusDeduct = bonusdeduct,
                        Guarantees = guarantees,
                        JoinPwd = joinpwd,
                        Subscription = subscription,
                        ActivityType = (EntityModel.Enum.ActivityType)activityType,
                        BettingCategory = isexy ? EntityModel.Enum.SchemeBettingCategory.ErXuanYi : EntityModel.Enum.SchemeBettingCategory.GeneralBetting

                    };

                   
                    param.Add("info", togInfo);
                    param.Add("balancePassword", balancepwd);
                    param.Add("userid", UserToken);
                    var hmResult = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/CreateSportsTogether");
                    return Json(hmResult);
                }
                #endregion

                #region 普通投注

                //投注对象
                EntityModel.CoreModel.Sports_BetingInfo info = new EntityModel.CoreModel.Sports_BetingInfo()
                {
                    SchemeSource = EntityModel.Enum.SchemeSource.Web,
                    PlayType = playType,
                    GameType = gameType,
                    IssuseNumber = issuseNumber,
                    Amount = amount,
                    TotalMoney = totalMoney,
                    TotalMatchCount = matchcount,
                    AnteCodeList = anteCodeList,
                    GameCode = gameCode,
                    Security = sercu,

                    SoldCount = (int)totalMoney,
                    ActivityType = (EntityModel.Enum.ActivityType)activityType,
                    BettingCategory = isexy ? EntityModel.Enum.SchemeBettingCategory.ErXuanYi : EntityModel.Enum.SchemeBettingCategory.GeneralBetting,
                    SchemeProgress = EntityModel.Enum.TogetherSchemeProgress.Finish
                };

                //竞彩足球单场  人气统计
                //if (gameCode == "jczq" && playType == "1_1")
                //{
                //    try
                //    {
                //        WCFClients.ExternalClient.UpdatePopularityByMatchId(info);
                //    }
                //    catch
                //    {
                //    }

                //}

                //如果是0表示免费保存订单
                param.Clear();
                param.Add("info", info);
                param.Add("balancePassword", balancepwd);
                param.Add("redBagMoney", 0M);
                param.Add("userid", UserToken);
                var saveparam = new Dictionary<string, object>();
                saveparam.Add("info", info);
                saveparam.Add("userid", UserToken);
                //如果是0表示免费保存订单
                
                //  var result = kind == 0 ? WCFClients.GameClient.SaveOrderSportsBetting(info, UserToken) : WCFClients.GameClient.Sports_Betting(info, balancepwd, UserToken);
                var result = kind == 0 ? await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(saveparam, "api/Betting/SaveOrderSportsBetting") : await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/Sports_Betting");
                if (kind != 0 && result.IsSuccess)//发送快照
                {
                    if (!isHemai)
                    {
                        string schemeType = "1";
                        SendSchemeSnapshotToEmail(schemeType, result.ReturnValue.Split('|')[0], gameCode, UserToken);
                    }
                }


                //#region 日志
                //if (result.IsSuccess)
                //{
                //    try
                //    {
                //        OrderLogWriter _logWriter = new OrderLogWriter(Server);
                //        string log = "用户来源:" + CurrentUser.LoginInfo.LoginFrom + " 登录名:" + CurrentUser.LoginInfo.LoginName + " 显示名称:" + CurrentUser.LoginInfo.DisplayName;
                //        log = log + "\r\n";
                //        log = log + "\r\n    方案编号:" + result.ReturnValue + "   总额:" + info.TotalMoney.ToString("0.00") + "元";
                //        log = log + "\r\n";
                //        log = log + "\r\n    投注号码:";
                //        foreach (var item in anteCodeList)
                //        {
                //            log = log + "【场次:" + item.MatchId + " 过关:" + playType + " 投注:" + item.AnteCode + " 胆:" + item.IsDan + "】  ";
                //        }
                //        log = log + "\r\n";
                //        log = log + "\r\n    追号期数：";

                //        log = log + "[" + issuseNumber + "  " + amount + "倍  " + totalMoney.ToString("f") + "元] ";
                //        _logWriter.Write(CurrentUser.LoginInfo.UserId, result.ReturnValue, LogType.Information, "来源IP：" + IpManager.IPAddress, log);
                //    }
                //    catch
                //    {
                //    }
                //}
                //#endregion

                return Json(result);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 保存订单购买
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> save_spotts()
        {
            try
            {
                var schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeId"], "订单编号不能为空");
                var balancePassword = string.IsNullOrEmpty(Request["bpwd"]) ? "" : Request["bpwd"];
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = schemeId;
                param["balancePassword"] = balancePassword;
                param["redBagMoney"] = 0M;
                param["userId"] = UserToken;
                var result = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/BettingUserSavedOrder");
                return Json(result);
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //单式上传
        public async Task<JsonResult> sing_sports()
        {
            try
            {
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空");
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空");//过关方式
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];//期号
                var containsMatchId = string.IsNullOrEmpty(Request["upFlag"]) ? false : bool.Parse(Request["upFlag"]);
                string selectMatchId = !containsMatchId
                    ? PreconditionAssert.IsNotEmptyString(Request["selectMatchId"], "选择的比赛id不能为空")
                    : "";
                var allowCodes = PreconditionAssert.IsNotEmptyString(Request["allowCodes"], "允许投注的号码不能为空");
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];//资金密码

                var ser = (EntityModel.Enum.TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性;//方案保密性
                var antecode = "";//投注号码
                var amount = PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空");
                var totalMoney = PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空");
                var fileBuffer = Encoding.UTF8.GetBytes(Session["fileStream"].ToString());
                var activityType = (EntityModel.Enum.ActivityType)(string.IsNullOrEmpty(Request["activityType"]) ? 2 : int.Parse(Request["activityType"]));
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                List<string> matchIdList = new List<string>();
                var checkresult = AnalyzerFactory.CheckSingleSchemeAnteCode(Session["fileStream"].ToString(), playType, containsMatchId, selectMatchId.Split(','), allowCodes.Split(','), out matchIdList);
                var codeArray = checkresult;
                //投注号码对象
                var anteCodeList = new EntityModel.CoreModel.Sports_AnteCodeInfoCollection();
                foreach (var item in codeArray)
                {
                    var cods = item.Split('#');

                    foreach (var c in cods)
                    {
                        var cod = c.Split('|');
                        var code = new EntityModel.CoreModel.Sports_AnteCodeInfo()
                        {
                            IsDan = false,
                            MatchId = cod[0],
                            AnteCode = CheckanteCode(cod[1], gameType),
                            PlayType = cod[2]
                        };

                        code.GameType = gameType;
                        anteCodeList.Add(code);
                    }
                }
                EntityModel.CoreModel.SingleSchemeInfo info = new EntityModel.CoreModel.SingleSchemeInfo
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    PlayType = playType,
                    IssuseNumber = issuseNumber,
                    SelectMatchId = selectMatchId,
                    AllowCodes = allowCodes,
                    ContainsMatchId = containsMatchId,
                    SchemeSource = EntityModel.Enum.SchemeSource.Web,
                    Security = ser,
                    BettingCategory = EntityModel.Enum.SchemeBettingCategory.SingleBetting,
                    AnteCodeList = anteCodeList,
                    Amount = int.Parse(amount),
                    TotalMoney = decimal.Parse(totalMoney),
                    FileBuffer = fileBuffer,
                    ActivityType = activityType,

                };
                Dictionary<string, object> param = new Dictionary<string, object>();
                #region 合买
                if (isHemai)
                {
                    //合买属性
                    var title = Request["title"];
                    var desc = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : Request["description"];
                    var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                    var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]); // 默认每份单价为1元
                    var guarantees = string.IsNullOrEmpty(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]); //我要保底份数
                    var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                    var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]); //我要认购份数
                    var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]); //提成比例
                    //合买
                    EntityModel.CoreModel.SingleScheme_TogetherSchemeInfo togetherinfo = new EntityModel.CoreModel.SingleScheme_TogetherSchemeInfo
                    {
                        BettingInfo = info,
                        Title = title,
                        Description = desc,
                        TotalCount = totalCount,
                        TotalMoney = decimal.Parse(totalMoney),
                        Price = price,
                        Guarantees = guarantees,
                        BonusDeduct = bonusdeduct,
                        JoinPwd = joinpwd,
                        Subscription = subscription
                    };

                  
                    param["info"] = togetherinfo;
                    param["balancePassword"] = balancepwd;
                    param["userId"] = UserToken;
                    var hmresult = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/CreateSingleSchemeTogether");
                    return Json(hmresult);
                }
                #endregion

                #region 普通投注
                param.Clear();
                param["info"] = info;
                param["password"] = balancepwd;
                param["redBagMoney"] = 0M;
                param["userId"] = UserToken;
                var result = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/Betting/SingleSchemeBettingAndChase");
                return Json(result);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 欲投
        /// </summary>
        /// <returns></returns>
        public JsonResult yt_sports()
        {
            try
            {
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空");
                var amount = PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空");
                var totalMoney = PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空");
                var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]);//总分数
                var issuesNum = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];//期号
                var title = Request["title"];
                var des = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : Request["description"];
                var guarantees = string.IsNullOrWhiteSpace(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]);//保底
                var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]);//认购
                var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]);//提成
                var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]);//每份价格
                var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"];//认购密码
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];//资金密码
                var ser = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["security"]) ? 0 : int.Parse(Request["security"]));//方案保密性
                var activeType = 2;//活动类型
                SingleSchemeInfo info = new SingleSchemeInfo
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    PlayType = "",
                    IssuseNumber = issuesNum,
                    SchemeSource = SchemeSource.Web,
                    Security = ser,
                    BettingCategory = SchemeBettingCategory.XianFaQiHSC,
                    AnteCodeList = null,
                    Amount = int.Parse(amount),
                    TotalMoney = decimal.Parse(totalMoney),
                    ActivityType = (ActivityType)activeType,

                };
                SingleScheme_TogetherSchemeInfo Singleinfo = new SingleScheme_TogetherSchemeInfo
                {
                    BettingInfo = info,
                    BonusDeduct = bonusdeduct,
                    Description = des,
                    Guarantees = guarantees,
                    JoinPwd = joinpwd,
                    Price = price,
                    Subscription = subscription,
                    Title = title,
                    TotalMoney = decimal.Parse(totalMoney),
                    TotalCount = totalCount
                };

                var result = WCFClients.GameClient.XianFaQiHSC_CreateSportsTogether(Singleinfo, balancepwd, base.UserToken);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 欲投上传方案
        /// </summary>
        /// <returns></returns>
        public JsonResult upyt()
        {
            try
            {
                var schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeId"], "订单号不能为空");
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空");
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空");
                var allowCodes = string.IsNullOrEmpty(Request["allowCodes"]) ? "" : Request["allowCodes"];
                var amount = PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空");
                var issuseNum = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                var containsMatchId = string.IsNullOrEmpty(Request["upFlag"]) ? false : bool.Parse(Request["upFlag"]);
                string selectMatchId = !containsMatchId
                   ? PreconditionAssert.IsNotEmptyString(Request["selectMatchId"], "选择的比赛id不能为空")
                   : "";
                var totalMoney = PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空");//实际投注金额
                var filestr = Encoding.UTF8.GetBytes(Session["fileStream"].ToString());//文件流
                //合买信息
                var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额

                List<string> matchIdList = new List<string>();
                var checkresult = AnalyzerFactory.CheckSingleSchemeAnteCode(Session["fileStream"].ToString(), playType, containsMatchId, selectMatchId.Split(','), allowCodes.Split(','), out matchIdList);
                var codeArray = checkresult;
                var anteCodeList = new Sports_AnteCodeInfoCollection();
                foreach (var item in codeArray)
                {
                    var cods = item.Split('#');

                    foreach (var c in cods)
                    {
                        var cod = c.Split('|');
                        var code = new Sports_AnteCodeInfo()
                        {
                            IsDan = false,
                            MatchId = cod[0],
                            AnteCode = CheckanteCode(cod[1], gameType),
                            PlayType = cod[2]
                        };

                        code.GameType = gameType;
                        anteCodeList.Add(code);
                    }
                }
                SingleSchemeInfo singleinfo = new SingleSchemeInfo
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    PlayType = playType,
                    AllowCodes = allowCodes,
                    Amount = int.Parse(amount),
                    ContainsMatchId = containsMatchId,
                    AnteCodeList = anteCodeList,
                    FileBuffer = filestr,
                    IssuseNumber = issuseNum,
                    SelectMatchId = selectMatchId,
                    TotalMoney = int.Parse(totalMoney),
                };
                SingleScheme_TogetherSchemeInfo info = new SingleScheme_TogetherSchemeInfo
                {
                    BettingInfo = singleinfo,
                    TotalCount = totalCount,
                    TotalMoney = int.Parse(totalMoney),
                    Price = 1

                };
                var result = WCFClients.GameClient.XianFaQi_UpLoadScheme(schemeId, info, CurrentUser.LoginInfo.UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //竞彩足球发布竞彩方案
        [HttpPost]
        public JsonResult Plans()
        {
            try
            {
                #region Request参数
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["game"], "彩种编码不能为空");
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空。");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空。");
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                var antecode = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
                var amount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空。"));
                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var matchcount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["matchcount"], "投注场数不能为空。"));
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];
                var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型

                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                #endregion

                #region 投注号码
                //投注号码对象
                var anteCodeList = new Sports_AnteCodeInfoCollection();
                var codeArray = antecode.Split('#');
                foreach (var item in codeArray)
                {
                    var cods = item.Split('|');
                    var code = new Sports_AnteCodeInfo()
                    {
                        IsDan = bool.Parse(cods[0]),
                        MatchId = cods[1],
                        AnteCode = cods[2],
                    };
                    if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf") && cods.Length > 3)
                    {
                        code.GameType = cods[3];
                    }
                    else
                    {
                        code.GameType = gameType;
                    }
                    anteCodeList.Add(code);
                }

                //投注过关方式
                playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                #endregion

                #region 普通投注

                //投注对象
                Sports_BetingInfo info = new Sports_BetingInfo()
                {
                    SchemeSource = SchemeSource.Web,
                    PlayType = playType,
                    GameType = gameType,
                    IssuseNumber = issuseNumber,
                    Amount = amount,
                    TotalMoney = totalMoney,
                    TotalMatchCount = matchcount,
                    AnteCodeList = anteCodeList,
                    GameCode = gameCode,
                    Security = sercu,

                    SoldCount = (int)totalMoney,
                    ActivityType = ActivityType.AddAward,
                    BettingCategory = SchemeBettingCategory.GeneralBetting,
                    SchemeProgress = TogetherSchemeProgress.Finish,
                };

                //如果是0表示免费保存订单

                var result = WCFClients.GameClient.PublishExperterScheme(info, UserToken);

                #region 日志
                if (result.IsSuccess)
                {
                    try
                    {
                        OrderLogWriter _logWriter = new OrderLogWriter(Server);
                        string log = "用户来源:" + CurrentUser.LoginInfo.LoginFrom + " 登录名:" + CurrentUser.LoginInfo.LoginName + " 显示名称:" + CurrentUser.LoginInfo.DisplayName;
                        log = log + "\r\n";
                        log = log + "\r\n    方案编号:" + result.ReturnValue + "   总额:" + info.TotalMoney.ToString("0.00") + "元";
                        log = log + "\r\n";
                        log = log + "\r\n    投注号码:";
                        foreach (var item in anteCodeList)
                        {
                            log = log + "【场次:" + item.MatchId + " 过关:" + playType + " 投注:" + item.AnteCode + " 胆:" + item.IsDan + "】  ";
                        }
                        log = log + "\r\n";
                        log = log + "\r\n    追号期数：";

                        log = log + "[" + issuseNumber + "  " + amount + "倍  " + totalMoney.ToString("f") + "元] ";
                        _logWriter.Write(CurrentUser.LoginInfo.UserId, result.ReturnValue, LogType.Information, "来源IP：" + IpManager.IPAddress, log);
                    }
                    catch
                    {
                    }
                }
                #endregion

                return Json(result);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //导入号码函数
        public ContentResult UploadNumber()
        {
            try
            {
                var fileObj = Request.Files["import_file"]; //上传文件
                if (fileObj == null)
                {
                    throw new Exception("上传对象为空");
                }

                var FILE_MAX_LENGTH = 1024 * 1024;//限制文件大小为1MB
                if (fileObj.ContentLength > FILE_MAX_LENGTH)
                {
                    throw new Exception("文件内容长度：" + fileObj.ContentLength + "超过了最大限制长度：" + FILE_MAX_LENGTH / 1024 + "KB");
                }

                StreamReader reader = new StreamReader(fileObj.InputStream, System.Text.Encoding.Default);
                string txt = reader.ReadToEnd();
                txt = txt.Replace("\r\n", "<br>");

                return Content("{state:true,msg:'" + txt + "'}");
            }
            catch (Exception ex)
            {
                return Content("{state:false,msg:\"" + ex.Message + "\"}");
            }

        }

        public PartialViewResult Single(string g)
        {
            var needPassword = CurrentUserBalance.CheckIsNeedPassword("Bet");
            ViewBag.NeedPassword = needPassword;
            //是否显示竞彩投注风险须知
            ViewBag.FlagCtzq = true;
            if (!string.IsNullOrEmpty(Request["g"]))
            {
                if (Request["g"].ToLower() == "ctzq")
                {
                    ViewBag.FlagCtzq = false;
                }
                if (new[] { "t14c", "tr9", "t6bqc", "t4cjq" }.Contains(g.ToLower()))
                {
                    g = "ctzq";
                }
            }

            if (Request["isDanGuan"] == "true")
            {
                g = "jczqdg";
            }
            //查询可用红包比例
            var bonus = WebRedisHelper.QueryRedBagUseConfigFromRedis(g);// WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(g);
            ViewBag.BonusRatio = bonus / 100;
            //可用红包
            ViewBag.Bonus = CurrentUserBalance.RedBagBalance;
            return PartialView();
        }
        public PartialViewResult Hemai()
        {
            var needPassword = CurrentUserBalance.CheckIsNeedPassword("Bet");
            ViewBag.NeedPassword = needPassword;
            var g = Request["g"];
            //是否显示竞彩投注风险须知
            ViewBag.FlagCtzq = true;
            if (!string.IsNullOrEmpty(g))
            {
                if (g.ToLower() == "ctzq") ViewBag.FlagCtzq = false;
                ViewBag.Game = g;
            }
            return PartialView();
        }

        //奖金优化
        public ActionResult Optimize()
        {
            //投注内容
            var itemsContent = Request["optimizeForm.itemsContent"];
            if (string.IsNullOrEmpty(itemsContent)) return PartialView();
            //过关方式
            var passContent = Request["optimizeForm.passContent"];
            if (string.IsNullOrEmpty(passContent)) return PartialView();
            //投注金额
            var schemeCost = PreconditionAssert.IsDecimal(Request["optimizeForm.schemeCost"], "投注金额格式不正确");
            if (schemeCost == 0) return PartialView();
            //投注倍数
            var multiple = PreconditionAssert.IsInt32(Request["optimizeForm.multiple"], "投注倍数格式不正确");
            if (multiple <= 0) return PartialView();

            //optimizeForm.opType
            //优化类型 
            var opType = 1;
            if (!string.IsNullOrEmpty(Request["optimizeForm.opType"]))
            {
                //直接从优化页面提交
                opType = PreconditionAssert.IsInt32(Request["optimizeForm.opType"], "优化类型格式不正确");
                if (opType != 1 && opType != 2 && opType != 3) opType = 1;
                //投注金额
                var orgMoney = PreconditionAssert.IsDecimal(Request["optimizeForm.orgMoney"], "投注金额格式不正确");
                if (orgMoney == 0) return PartialView();
                ViewBag.OrgMoney = orgMoney;
                ViewBag.SchemeCost = schemeCost < orgMoney * 2 ? orgMoney * 2 : schemeCost;
            }
            else
            {
                //从购彩页面提交过来，如果是单倍投注，投注金额=投注金额*2
                ViewBag.SchemeCost = multiple == 1 ? schemeCost * 2 : schemeCost;
                ViewBag.OrgMoney = schemeCost;
            }
            List<Optimization> json;
            try
            {
                json = JsonSerializer.Deserialize<List<Optimization>>(itemsContent);
            }
            catch (Exception)
            {
                return PartialView();
            }
            if (json == null) return PartialView();

            ViewBag.OpType = opType;
            ViewBag.ItemsContent = itemsContent;
            ViewBag.Contents = json;
            ViewBag.Gg = passContent;
            ViewBag.Multiple = multiple;
            return PartialView();
        }

        [HttpPost]
        public JsonResult OptSportBuy()
        {
            #region Request参数
            var gameCode = PreconditionAssert.IsNotEmptyString(Request["game"], "彩种编码不能为空");
            var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空。");
            var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空。");
            var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
            var antecode = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
            var amount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空。"));
            var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
            var org = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["orgMoney"], "原始投注金额不能为空。"));
            var matchcount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["matchcount"], "投注场数不能为空。"));
            var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];
            var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
            var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
            var attach = PreconditionAssert.IsNotEmptyString(Request["attach"], "附加属性不能为空。");
            var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
            #endregion
            #region 投注号码
            //投注号码对象
            var anteCodeList = new Sports_AnteCodeInfoCollection();
            var codeArray = antecode.Split('#');
            foreach (var item in codeArray)
            {
                var cods = item.Split('|');
                var code = new Sports_AnteCodeInfo()
                {
                    IsDan = bool.Parse(cods[0]),
                    MatchId = cods[1],
                    AnteCode = cods[2],
                };
                //if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf") && cods.Length > 3)
                if (cods.Length > 3)
                {
                    code.GameType = cods[3];
                }
                anteCodeList.Add(code);
            }

            //投注过关方式
            playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
            #endregion
            #region 合买投注
            if (isHemai)
            {
                //合买属性
                var title = Request["title"];
                var desc = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : Request["description"];
                var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]); // 默认每份单价为1元
                var guarantees = string.IsNullOrEmpty(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]); //我要保底份数
                var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]); //我要认购份数
                var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]); //提成比例
                var togInfo = new Sports_TogetherSchemeInfo()
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = issuseNumber,
                    Amount = amount,
                    AnteCodeList = anteCodeList,
                    Attach = attach,
                    PlayType = playType,
                    SchemeSource = SchemeSource.Web,
                    TotalMoney = org,
                    TotalMatchCount = matchcount,
                    Title = title,
                    Description = desc,
                    TotalCount = totalCount,
                    Security = sercu,
                    Price = price,
                    BonusDeduct = bonusdeduct,
                    Guarantees = guarantees,
                    JoinPwd = joinpwd,
                    Subscription = subscription
                };
                try
                {
                    //资金密码
                    var hmResult = WCFClients.GameClient.CreateYouHuaSchemeTogether(togInfo, balancepwd, totalMoney, UserToken);
                    return Json(hmResult);
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Message = ex.Message });
                }
            }
            #endregion
            var opt = new Sports_BetingInfo
                {
                    ActivityType = ActivityType.AddAward,
                    Amount = amount,
                    AnteCodeList = anteCodeList,
                    Attach = attach,
                    BettingCategory = SchemeBettingCategory.YouHua,
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = issuseNumber,
                    PlayType = playType,
                    SchemeProgress = TogetherSchemeProgress.Standard,
                    SchemeSource = SchemeSource.Web,
                    Security = TogetherSchemeSecurity.Public,
                    SoldCount = 0,
                    TotalMatchCount = matchcount,
                    TotalMoney = org
                };

            try
            {
                //资金密码
                //var result = WCFClients.GameClient.YouHuaBet(opt, balancepwd, totalMoney, UserToken);
                var result = WCFClients.GameClient.YouHuaBetAndChase(opt, balancepwd, totalMoney, 0M, UserToken);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        public ActionResult test()
        {
            return View();
        }

        #region 方案快照

        private void SendSchemeSnapshotToEmail(string schemeType, string schemeId, string gameCode, string userToken)
        {
            try
            {
                //var siteUrl = ConfigurationManager.AppSettings["SelfDomain"] ?? "http://www.wancai.com";
                //var schemeAddress = siteUrl + "/buy/SchemesSnapshoot?SchemeType=" + schemeType + "&schemeId=" + schemeId + "&GameCode=" + gameCode + "";//方案地址
                //var emailInfo = WCFClients.ExternalClient.GetMyEmailInfo(userToken);//获取邮箱信息
                //if (emailInfo == null || string.IsNullOrEmpty(emailInfo.Email))
                //    throw new Exception("发送快照失败,未查询到邮箱地址！");
                //var emailSmtp = WCFClients.GameClient.QueryCoreConfigByKey("Email.Smtp").ConfigValue;
                //var emailAccount = WCFClients.GameClient.QueryCoreConfigByKey("Email.Account").ConfigValue;
                //var emailDisplayName = WCFClients.GameClient.QueryCoreConfigByKey("Email.DisplayName").ConfigValue;
                //var emailPassword = WCFClients.GameClient.QueryCoreConfigByKey("Email.Password").ConfigValue;
                //var emailTitle = WCFClients.GameClient.QueryCoreConfigByKey("Email.Title").ConfigValue;
                //if (string.IsNullOrEmpty(emailSmtp) || string.IsNullOrEmpty(emailAccount) || string.IsNullOrEmpty(emailDisplayName) || string.IsNullOrEmpty(emailPassword) || string.IsNullOrEmpty(emailTitle))
                //    throw new Exception("发送快照失败，配置信息不完整！");
                //SendSchemeSnapshot snapshot = new SendSchemeSnapshot();
                //snapshot.SendSchemeSnapshotToEmail(schemeAddress, emailInfo.Email, emailSmtp, emailAccount, emailDisplayName, emailPassword, emailTitle);

            }
            catch { }
        }

        /// <summary>
        /// 方案快照(投注后调用)
        /// </summary>
        /// <returns></returns>
        public ActionResult SchemesSnapshoot()
        {

            var schemeType = (SchemeType)int.Parse(string.IsNullOrEmpty(Request["SchemeType"]) ? "0" : Request["SchemeType"]);
            var schemeId = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
            var GameCode = string.IsNullOrEmpty(Request["GameCode"]) ? "" : Request["GameCode"];
            switch (schemeType)
            {
                case SchemeType.GeneralBetting:
                    if (GameCode != null && GameCode != "" && GameCode.ToUpper() == "JCLQ" || GameCode.ToUpper() == "JCZQ" || GameCode.ToUpper() == "BJDC")
                    {
                        ViewBag.JCBetting = WCFClients.GameClient.QueryJCBettingSnapshotInfo(schemeId, GameCode);//普通竞彩投注
                    }
                    else
                    {
                        ViewBag.PTBetting = WCFClients.GameClient.QueryPTBettingSnapshotInfo(schemeId);//普通投注
                    }
                    break;
                case SchemeType.ChaseBetting:
                    ViewBag.ChaseBetting = WCFClients.GameClient.QueryChaseBettingSnapshotInfo(schemeId);//追号投注
                    break;
                case SchemeType.TogetherBetting:
                    ViewBag.TogetherBetting = WCFClients.GameClient.QueryTogetherBettingSnapshotInfo(schemeId);//合买投注
                    break;
                default:
                    break;
            }
            ViewBag.GameCode = GameCode;
            ViewBag.SchemeType = schemeType;
            return View();
        }

        #endregion

        #region 单式上传

        public ContentResult Upload()
        {

            try
            {
                Session["fileStream"] = null; //清空文件流
                var fileObj = Request.Files["upload_file"];
                if (fileObj == null || fileObj.ContentLength <= 0) throw new Exception("上传文件不能为空");
                var FILE_MAX_LENGTH = 1024 * 1024; //限制文件大小为1MB
                var fileName = fileObj.FileName.Split('.')[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + ".txt";
                if (fileObj.ContentLength > FILE_MAX_LENGTH)
                    throw new Exception("文件内容长度：" + fileObj.ContentLength + "超过了最大限制长度：" + FILE_MAX_LENGTH / 1024 + "KB");
                StreamReader reader = new StreamReader(fileObj.InputStream, System.Text.Encoding.Default);
                var str = reader.ReadToEnd(); //文件流
                var antecodelist = str.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in antecodelist)
                {
                    if (item.Contains("→") && item.Contains("*")) return Content("{state:false,msg:'投注内容格式不正确,暂不支持'*'号'}");
                }
                Session["fileStream"] = str;
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空"); //串关玩法
                var gameCode = Request["gameCode"];
                var gameType = Request["gameType"];
                var containsMatchId = string.IsNullOrEmpty(Request["containsMatchId"])
                    ? false
                    : bool.Parse(Request["containsMatchId"]); //是否包含场次id
                string[] selectMatchIdArray = string.IsNullOrEmpty(Request["selectMatchId"])
                    ? null
                    : Request["selectMatchId"].Split(',');
                //允许投注的号码
                string[] codes = PreconditionAssert.IsNotEmptyString(Request["allowCodes"], "允许投注的号码不能为空").Split(',');
                List<string> matchIdList = new List<string>();
                var result = AnalyzerFactory.CheckSingleSchemeAnteCode(str, playType, containsMatchId, selectMatchIdArray, codes, out matchIdList);
                return Content("{state:true,count:'" + result.Count + "',fileName:'" + fileName + "'}");
            }
            catch (Exception ex)
            {

                return Content("{state:false,msg:'" + ex.Message + "'}");
            }
        }

        public static string CheckanteCode(string code, string type)
        {
            var str = code;
            if (type.ToLower() == "sxds")
            {
                switch (code)
                {
                    case "3":
                        str = "sd";
                        break;
                    case "2":
                        str = "ss";
                        break;
                    case "1":
                        str = "xd";
                        break;
                    case "0":
                        str = "xs";
                        break;
                    default:
                        return code;

                }
            }
            return str;

        }

        #endregion


    }
}
