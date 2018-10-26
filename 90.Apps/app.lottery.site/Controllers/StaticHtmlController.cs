using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Common.Cryptography;
using GameBiz.Core;
using External.Core.SiteMessage;
using Common.Net;
using Common.JSON;
using LotteryData.Core;
using app.lottery.site.cbbao.Models;
using app.lottery.site.iqucai.api;
using app.lottery.site.iqucai;
using System.Threading.Tasks;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;

namespace app.lottery.site.Controllers
{
    public class StaticHtmlController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public StaticHtmlController(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion


        public ContentResult BuildStaticPageEvent()
        {
            try
            {
                var pageType = Request["pageType"];
                var code = Request["code"];
                var data = Request["data"];
                var myCode = Encipherment.MD5(string.Format("Home_BuildStaticPageEvent_{0}", pageType), Encoding.UTF8);
                if (code.ToUpper() != myCode.ToUpper())
                    throw new Exception("加密值不正确");
                var eventType = (WebBuildStaticFileEventCategory)int.Parse(pageType);
                switch (eventType)
                {
                    case WebBuildStaticFileEventCategory.OnOrderPrize:
                        //订单派奖
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateArticle:
                        //更新网站文章
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateBJDCResult:
                        //更新北京单场开奖结果
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateBanner:
                        //更新广告图
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateBulletin:
                        //更新网站公告
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateJCLQResult:
                        //更新竞彩篮球开奖结果
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateJCZQResult:
                        //更新竞彩足球开奖结果
                        break;
                    case WebBuildStaticFileEventCategory.OnUpdateLotteryNumber:
                        //更新数字彩开奖结果
                        DoOnUpdateLotteryNumber(data);
                        break;
                    default:
                        break;
                }

                return Content("静态态面生成完成");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        #region 事件响应

        private void DoOnUpdateLotteryNumber(string data)
        {
            var dataArray = data.Split('_');
            if (dataArray.Length != 4) return;
            string gameCode = dataArray[0];
            string gameType = dataArray[1];
            string issuseNumber = dataArray[2];
            string winNumber = dataArray[3];

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region 全部彩种最新开奖号

            var list = new List<GameWinNumberInfo>();
            var fileFullPath = Path.Combine(path, "lottery_new_number.json");
            //读取json文件
            if (System.IO.File.Exists(fileFullPath))
            {
                var oldJson = System.IO.File.ReadAllText(fileFullPath, Encoding.UTF8);
                list = JsonSerializer.Deserialize<List<GameWinNumberInfo>>(oldJson);
                list.RemoveAll((i) =>
                {
                    return i.GameCode == gameCode;
                });
                System.IO.File.Delete(fileFullPath);
            }
            list.Add(new GameWinNumberInfo
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                GameType = gameType,
                GameCode = gameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
            });
            //写文件
            var json = JsonSerializer.Serialize(list);
            System.IO.File.WriteAllText(fileFullPath, json, Encoding.UTF8);

            #endregion

            #region 某彩种最新N条开奖号

            var openList = new List<GameWinNumberInfo>();
            WCFClients.ChartClient.QueryGameWinNumber(gameCode, 0, 10).List.ForEach((o) =>
            {
                openList.Add(new GameWinNumberInfo
                {
                    CreateTime = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    GameCode = o.GameCode,
                    GameType = o.GameType,
                    IssuseNumber = o.IssuseNumber,
                    WinNumber = o.WinNumber,
                });
            });
            var openListFilePath = Path.Combine(path, string.Format("lottery_new_open_list_{0}.json", gameCode));
            if (System.IO.File.Exists(openListFilePath))
                System.IO.File.Delete(openListFilePath);

            //写文件
            var openListJson = JsonSerializer.Serialize(openList);
            System.IO.File.WriteAllText(openListFilePath, openListJson, Encoding.UTF8);

            #endregion

            #region 生成走势图表页面



            #endregion
        }

        #endregion

        private void BuildViewHtml(string viewName, string fileFullName)
        {
            try
            {
                var staticHtml = string.Empty;
                IView v = ViewEngines.Engines.FindView(this.ControllerContext, viewName, "").View;
                if (v == null)
                    throw new Exception(string.Format("查找视图{0}失败", viewName));
                using (StringWriter sw = new StringWriter())
                {
                    ViewContext vc = new ViewContext(this.ControllerContext, v, this.ViewData, this.TempData, sw);
                    vc.View.Render(vc, sw);
                    staticHtml = sw.ToString();
                }
                //保存html
                if (System.IO.File.Exists(fileFullName))
                    System.IO.File.Delete(fileFullName);
                System.IO.File.WriteAllText(fileFullName, staticHtml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BuildViewHtml(ControllerContext context, string viewName, string fileFullName)
        {
            try
            {
                var staticHtml = string.Empty;
                IView v = ViewEngines.Engines.FindView(context, viewName, "").View;
                if (v == null)
                    throw new Exception(string.Format("查找视图{0}失败", viewName));
                using (StringWriter sw = new StringWriter())
                {
                    ViewContext vc = new ViewContext(context, v, context.Controller.ViewData, context.Controller.TempData, sw);
                    vc.View.Render(vc, sw);
                    staticHtml = sw.ToString();
                }
                //保存html
                if (System.IO.File.Exists(fileFullName))
                    System.IO.File.Delete(fileFullName);
                System.IO.File.WriteAllText(fileFullName, staticHtml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 生成指定的页面
        /// </summary>
        public ContentResult BuildSpecificPage()
        {
            try
            {
                var error = string.Empty;
                var pageType = Request["pageType"];
                var code = Request["code"];
                //var myCode = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                //if (code.ToUpper() != myCode.ToUpper())
                //    throw new Exception("加密值不正确");
                var key = Request["key"];
                switch (pageType)
                {
                    case "10":
                        //首页
                        //Task.Factory.StartNew(() =>
                        //{

                        //});
                        BuildIndex();
                        break;
                    case "11":
                        //中间页面
                        BuildIndex_App();
                        break;
                    case "20":
                        //中奖排行
                        BuildBonusDetail();
                        break;
                    case "30":
                        //全部开奖结果
                        BuildOpenResult();
                        break;
                    case "301":
                        //开奖结果首页
                        BuildOpenResultIndex();
                        break;
                    case "302":
                        //彩种开奖历史
                        BuildOpenResultHistory_ByGameCode(key);
                        break;
                    case "303":
                        //彩种开奖详细
                        //Task.Factory.StartNew(() =>
                        //  {
                        //      BuildOpenResultDetail_ByGameCode(key);
                        //  });
                        BuildOpenResultDetail_ByGameCode(key);
                        break;
                    case "40":
                        //全部最新开奖号
                        BuildLotteryNewNumber();
                        break;
                    case "401":
                        //指定彩种最新开奖
                        BuildLotteryNewNumber_ByGameCode(key);
                        break;
                    case "50":
                        //全部资讯首页和目录
                        BuildZiXunIndexAndCategoryPage();
                        break;
                    case "60":
                        //全部资讯明细
                        BuildZiXunDetail();
                        break;
                    case "70":
                        //全部公告
                        BuildBulletin();
                        break;
                    case "80":
                        //神单首页
                        BuildReDan();
                        break;
                    case "90":
                        //走势图首页
                        BuildLotteryTrend();
                        break;
                    case "900":
                        //生成指定彩种的走势图
                        BuildLotteryTrend_ByGameCode(key);
                        break;
                    case "901":
                        BuildLotteryTrend_SSQ();
                        break;
                    case "902":
                        BuildLotteryTrend_FC3D();
                        break;
                    case "903":
                        BuildLotteryTrend_QLC();
                        break;
                    case "904":
                        BuildLotteryTrend_JLK3();
                        break;
                    case "905":
                        BuildLotteryTrend_JSKS();
                        break;
                    case "906":
                        BuildLotteryTrend_HBK3();
                        break;
                    case "907":
                        BuildLotteryTrend_SDQYH();
                        break;
                    case "908":
                        BuildLotteryTrend_HC1();
                        break;
                    case "909":
                        BuildLotteryTrend_HD15X5();
                        break;
                    case "910":
                        BuildLotteryTrend_DF6J1();
                        break;
                    case "911":
                        BuildLotteryTrend_DLT();
                        break;
                    case "912":
                        BuildLotteryTrend_PL3();
                        break;
                    case "913":
                        BuildLotteryTrend_PL5();
                        break;
                    case "914":
                        BuildLotteryTrend_QXC();
                        break;
                    case "915":
                        BuildLotteryTrend_SD11X5();
                        break;
                    case "916":
                        BuildLotteryTrend_GD11X5();
                        break;
                    case "917":
                        BuildLotteryTrend_JX11X5();
                        break;
                    case "918":
                        BuildLotteryTrend_CQ11X5();
                        break;
                    case "919":
                        BuildLotteryTrend_LN11X5();
                        break;
                    case "920":
                        BuildLotteryTrend_CQSSC();
                        break;
                    case "921":
                        BuildLotteryTrend_JXSSC();
                        break;
                    case "922":
                        BuildLotteryTrend_GDKLSF();
                        break;
                    case "923":
                        BuildLotteryTrend_HNKLSF();
                        break;
                    case "924":
                        BuildLotteryTrend_CQKLSF();
                        break;
                    case "925":
                        BuildLotteryTrend_SDKLPK3();
                        break;
                    case "100":
                        //购彩大厅
                        BuildBuyHall();
                        break;
                    case "200":
                        //个人博客
                        error = BuildBlog(key);
                        break;
                    case "300":
                        //注册页
                        BuildUserRegister();
                        break;
                    case "400":
                        //vip会员页
                        BuildVip();
                        break;
                    case "500":
                        //合买大厅数据
                        BuildHeMaiHall();
                        break;
                    case "600":
                        //手机购彩
                        BuildPhoneBuy();
                        break;
                    case "700":
                        //活动首页
                        BuildHuoDong();
                        break;
                    case "800":
                        //生成过关统计
                        BuilGuoGuanTongJi();
                        break;
                    case "1000":
                        //帮助中心
                        BuildHelp();
                        break;
                    //case "1010":
                    //    //派奖后生成订单详情
                    default:
                        break;
                }

                return Content("静态态面生成完成" + error);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        #region 生成指定页面

        /// <summary>
        /// 生成首页
        /// </summary>
        private async Task BuildIndex()
        {

            var tempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempIndex.html");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param["category"] = "Lottery_Hot"; param["gameCode"] = ""; param["pageIndex"] = 0; param["pageSize"] = 3;
            //咨询优化
            ViewBag.Cphot = await serviceProxyProvider.Invoke<EntityModel.CoreModel.ArticleInfo_QueryCollection>(param, "api/data/QueryArticleList_YouHua");
            param.Clear();
            param["category"] = "Lottery_GameCode"; param["gameCode"] = "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3"; param["pageIndex"] = 0; param["pageSize"] = 3;
            ViewBag.Gpc = await serviceProxyProvider.Invoke<EntityModel.CoreModel.ArticleInfo_QueryCollection>(param, "api/data/QueryArticleList_YouHua");
            param.Clear();
            param["category"] = "Lottery_GameCode"; param["gameCode"] = "SSQ|DLT|PL3|FC3D"; param["pageIndex"] = 0; param["pageSize"] = 4;
            ViewBag.Scz = await serviceProxyProvider.Invoke<EntityModel.CoreModel.ArticleInfo_QueryCollection>(param, "api/data/QueryArticleList_YouHua");
            param.Clear();
            param["category"] = "Lottery_GameCode"; param["gameCode"] = "JCZQ|JCLQ|BJDC"; param["pageIndex"] = 0; param["pageSize"] = 4;
            ViewBag.Jjc = await serviceProxyProvider.Invoke<EntityModel.CoreModel.ArticleInfo_QueryCollection>(param, "api/data/QueryArticleList_YouHua");
            ViewBag.CurrentUser = CurrentUser;
            //神单排行
            var now = DateTime.Now;
            ViewBag.RankList = WCFClients.ExternalClient.QueryGSRankList(now.ToString("MM.dd"), now.ToString("MM.dd"), "", "");
            //焦点新闻想     
            ViewBag.FocusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 10);
            //中奖新闻
            ViewBag.BonusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 10);
            //总注册人数
            ViewBag.TotalUserCount = WCFClients.ExternalClient.QueryUserRegisterCount();
            //神单红人月/总排行
            var MonthBeginTime = DateTime.Now.AddDays(-30);
            var TotalBeginTime = DateTime.Now.AddDays(-90);
            var endTime = DateTime.Now.AddDays(1);
            ViewBag.mRankList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(MonthBeginTime, endTime, "", "", 0, 10);
            ViewBag.tRankList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(TotalBeginTime, endTime, "", "", 0, 10);
            ViewBag.LotteryBonus = WCFClients.ExternalClient.QueryLotteryNewBonusInfoList(15);
            BuildViewHtml("Default", tempFilePath);

            var realPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
            System.IO.File.Copy(tempFilePath, realPath, true);

        }

        /// <summary>
        /// 中间页面
        /// </summary>
        private void BuildIndex_App()
        {
            var filePath_app = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index_app.html");
            BuildViewHtml("default_app", filePath_app);
        }

        /// <summary>
        /// 生成中奖排行
        /// </summary>
        private void BuildBonusDetail()
        {
            //中奖排行主页
            var indexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "bonus");
            if (!Directory.Exists(indexPath))
                Directory.CreateDirectory(indexPath);
            var indexFullName = Path.Combine(indexPath, "index.html");
            BuildViewHtml("bonus/newbonus", indexFullName);

            //分类子页
            foreach (var type in new string[] { "djph", "fdyl", "gdyl", "hmrq", "cgzj", "zdgd", "ljzj" })
            {
                foreach (var gameCode in new string[] { "jczq", "jclq", "bjdc", "ctzq_t14c", "ctzq_tr9", "ctzq_t6bqc", "ctzq_t4cjq", "ssq", "dlt", "pl3", "fc3d" })
                {
                    foreach (var date in new int[] { 7, 30, 90 })
                    {
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "bonus", type, gameCode);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var fileFullName = Path.Combine(path, string.Format("{0}.html", date));
                        var beginTime = DateTime.Now.AddDays(-date);
                        var endTime = DateTime.Now.AddDays(1);
                        var _gameCode = gameCode;
                        var gameType = "";
                        var gameCodeArray = gameCode.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                        if (gameCodeArray.Length == 2)
                        {
                            _gameCode = gameCodeArray[0];
                            gameType = gameCodeArray[1];
                        }

                        var pageIndex = 0;
                        var pageSize = 30;
                        switch (type)
                        {
                            case "djph":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BigBonus_Sport(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "fdyl":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "gdyl":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_JoinProfit_Sport(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "hmrq":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_HotTogether(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "cgzj":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_SuccessOrder_Sport(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "zdgd":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankInfoList_BeFollowerCount(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                            case "ljzj":
                                ViewBag.FDList = WCFClients.GameQueryClient.QueryRankReport_TotalBonus_Sport(beginTime, endTime, _gameCode, gameType, pageIndex, pageSize);
                                break;
                        }

                        BuildViewHtml("bonus/" + type, fileFullName);
                    }
                }
            }
        }

        /// <summary>
        /// 生成开奖结果首页
        /// </summary>
        private void BuildOpenResultIndex()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lottery");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileFullName = Path.Combine(path, "index.html");
            ViewBag.GameCode = "newindex";
            BuildViewHtml("lottery/lotteryhall", fileFullName);
        }

        /// <summary>
        /// 生成指定彩种的开奖历史
        /// </summary>
        private void BuildOpenResultHistory_ByGameCode(string gameCode)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lottery");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            ViewBag.GameCode = gameCode.ToUpper();
            ViewBag.GameType = "";
            var historyFullName = "";
            var startTime = DateTime.Today;
            var endTime = startTime.AddDays(1);
            if (gameCode.ToUpper() == "CTZQ")
            {
                foreach (var gameType in new string[] { "t14c", "tr9", "t6bqc", "t4cjq" })
                {
                    ViewBag.GameType = gameType;
                    historyFullName = Path.Combine(path, string.Format("history_{0}_{1}.html", gameCode, gameType));
                    startTime = startTime.AddMonths(-1);

                    ViewBag.NumberHistoryList = WCFClients.ChartClient.QueryGameWinNumberByDate(startTime, endTime, string.Format("{0}_{1}", gameCode, gameType), 0, MaxIssuseCount(ViewBag.GameCode));
                    BuildViewHtml("lottery/history", historyFullName);
                }

            }
            else
            {
                historyFullName = Path.Combine(path, string.Format("history_{0}.html", gameCode));
                if (gameCode.ToLower() == "ssq" || gameCode.ToLower() == "dlt" || gameCode.ToLower() == "fc3d" || gameCode.ToLower() == "pl3")
                {
                    startTime = startTime.AddMonths(-1);
                }
                ViewBag.NumberHistoryList = WCFClients.ChartClient.QueryGameWinNumberByDate(startTime, endTime, ViewBag.GameCode, 0, MaxIssuseCount(ViewBag.GameCode));
                BuildViewHtml("lottery/history", historyFullName);
            }
        }

        /// <summary>
        /// 生成开奖详细
        /// </summary>
        private void BuildOpenResultDetail_ByGameCode(string currentGameCode)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lottery");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //数字彩
            var sscArray = new string[] { "ssq", "dlt", "pl3", "fc3d" };
            if (sscArray.Contains(currentGameCode.ToLower()))
            {
                var sscIssuseArray = WCFClients.GameClient.QueryPrizedIssuseList(currentGameCode, "", 10, UserToken).Split(',');
                for (int i = 0; i < sscIssuseArray.Length; i++)
                {
                    //最后一期
                    var currentIssuseNumber = sscIssuseArray[i];
                    if (i == 0)
                    {
                        var detailFullName = Path.Combine(path, string.Format("detail_{0}.html", currentGameCode));
                        ViewBag.GameCode = currentGameCode;
                        ViewBag.IssuseNumber = currentIssuseNumber;
                        BuildViewHtml("lottery/NewIndex", detailFullName);
                    }
                    var detailFullNameByIssuse = Path.Combine(path, string.Format("detail_{0}_{1}.html", currentGameCode, currentIssuseNumber));
                    ViewBag.GameCode = currentGameCode;
                    ViewBag.IssuseNumber = currentIssuseNumber;
                    BuildViewHtml("lottery/NewIndex", detailFullNameByIssuse);
                }
            }

            //传统足球
            var ctzqArray = new string[] { "t14c", "tr9", "t6bqc", "t4cjq" };
            if (ctzqArray.Contains(currentGameCode.ToLower()))
            {
                var ctzqIssuseArray = WCFClients.GameClient.QueryStopIssuseList("CTZQ", currentGameCode, 5, UserToken).Split(',');
                for (int i = 0; i < ctzqIssuseArray.Length; i++)
                {
                    var ctzqIssuse = ctzqIssuseArray[i];
                    //最后一期
                    if (i == 0)
                    {
                        var detailFullName = Path.Combine(path, string.Format("detail_{0}.html", currentGameCode));
                        ViewBag.CurrentIssuse = ctzqIssuse;
                        ViewBag.Type = currentGameCode;
                        BuildViewHtml("lottery/newctzq", detailFullName);
                    }

                    var detailIssuseFullName = Path.Combine(path, string.Format("detail_{0}_{1}.html", currentGameCode, ctzqIssuse));
                    ViewBag.CurrentIssuse = ctzqIssuse;
                    ViewBag.Type = currentGameCode;
                    BuildViewHtml("lottery/newctzq", detailIssuseFullName);
                }
            }

            var maxDay = 3;
            //竞彩足球
            if (currentGameCode.ToLower() == "jczq")
            {
                if (!Directory.Exists(Path.Combine(path, "jczq")))
                {
                    //没有对应目录，说明是第一次运行
                    maxDay = 60;
                }
                for (int i = 0; i < maxDay; i++)
                {
                    var date = DateTime.Today.AddDays(-i);
                    var gameTypeArray = new string[] { "all", "spf", "brqspf", "bf", "zjq", "bqc" };
                    foreach (var gameType in gameTypeArray)
                    {
                        ViewBag.Type = gameType == "all" ? "" : gameType;
                        ViewBag.Begin = date;
                        ViewBag.IsDefault = false;
                        ViewBag.Match = WCFClients.GameIssuseClient.QueryJCZQMatchResultByTime(ViewBag.Begin);

                        var tempPath = Path.Combine(path, "jczq", date.ToString("yyyyMM"));
                        if (!Directory.Exists(tempPath))
                            Directory.CreateDirectory(tempPath);
                        var detailFullName = Path.Combine(tempPath, string.Format("detail_jczq_{0}_{1}.html", gameType, date.ToString("yyyyMMdd")));
                        BuildViewHtml("lottery/newjczq", detailFullName);
                        if (i == 0)
                        {
                            ViewBag.IsDefault = true;
                            var curentDetailFullName = Path.Combine(path, "jczq", string.Format("detail_jczq_{0}.html", gameType));
                            BuildViewHtml("lottery/newjczq", curentDetailFullName);
                        }
                    }
                }
            }
            //竞彩篮球
            if (currentGameCode.ToLower() == "jclq")
            {
                for (int i = 0; i < maxDay; i++)
                {
                    var date = DateTime.Today.AddDays(-i);
                    var gameTypeArray = new string[] { "all", "sf", "rfsf", "sfc", "dxf" };
                    foreach (var gameType in gameTypeArray)
                    {
                        ViewBag.Type = gameType == "all" ? "" : gameType;
                        ViewBag.Begin = date;
                        ViewBag.IsDefault = false;
                        ViewBag.Match = WCFClients.GameIssuseClient.QueryJCLQMatchResultByTime(ViewBag.Begin);

                        var tempPath = Path.Combine(path, "jclq", date.ToString("yyyyMM"));
                        if (!Directory.Exists(tempPath))
                            Directory.CreateDirectory(tempPath);
                        var detailFullName = Path.Combine(tempPath, string.Format("detail_jclq_{0}_{1}.html", gameType, date.ToString("yyyyMMdd")));
                        BuildViewHtml("lottery/newjclq", detailFullName);
                        if (i == 0)
                        {
                            ViewBag.IsDefault = true;
                            var curentDetailFullName = Path.Combine(path, "jclq", string.Format("detail_jclq_{0}.html", gameType));
                            BuildViewHtml("lottery/newjclq", curentDetailFullName);
                        }
                    }
                }
            }

            //北京单场
            if (currentGameCode.ToLower() == "bjdc")
            {
                var issuseArray = WCFClients.GameIssuseClient.QueryBJDCLastIssuseNumber(5).Split('|');
                for (int i = 0; i < issuseArray.Length; i++)
                {
                    var bjdcIssuse = issuseArray[i];
                    var gameTypeArray = new string[] { "all", "spf", "zjq", "sxds", "bf", "bqc" };
                    foreach (var gameType in gameTypeArray)
                    {
                        ViewBag.Type = gameType == "all" ? "" : gameType;
                        ViewBag.IssuseList = issuseArray;
                        ViewBag.IssuseNum = bjdcIssuse;
                        ViewBag.MathList = WCFClients.GameIssuseClient.QueryBJDC_MatchResultList(bjdcIssuse);

                        var detailFullName = Path.Combine(path, string.Format("detail_bjdc_{0}_{1}.html", gameType, bjdcIssuse));
                        BuildViewHtml("lottery/newbjdc", detailFullName);

                        if (i == 0)
                        {
                            ViewBag.IssuseNum = "";
                            var currentDetailFullName = Path.Combine(path, string.Format("detail_bjdc_{0}.html", gameType));
                            BuildViewHtml("lottery/newbjdc", currentDetailFullName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成开奖结果
        /// </summary>
        private void BuildOpenResult()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lottery");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region 开奖结果首页

            BuildOpenResultIndex();

            #endregion

            #region 开奖历史

            foreach (var gameCode in new string[] { "ctzq", "ssq", "dlt", "pl3", "fc3d", "cqssc", "jx11x5" })
            {
                BuildOpenResultHistory_ByGameCode(gameCode);
            }

            #endregion

            #region 开奖详细

            foreach (var gameCode in new string[] { "ssq", "dlt", "pl3", "fc3d", "t14c", "tr9", "t6bqc", "t4cjq", "jczq", "jclq", "bjdc" })
            {
                BuildOpenResultDetail_ByGameCode(gameCode);
            }

            #endregion
        }

        /// <summary>
        /// 生成指定彩种的最新开奖号
        /// </summary>
        private void BuildLotteryNewNumber_ByGameCode(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode)) return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region 所有彩种生成到同一个文件

            var list = new List<GameWinNumberInfo>();
            var info = WCFClients.ChartClient.QueryNewWinNumber(gameCode);
            if (info != null)
            {
                var fileFullPath = Path.Combine(path, "lottery_new_number.json");
                //读取json文件
                if (System.IO.File.Exists(fileFullPath))
                {
                    var oldJson = System.IO.File.ReadAllText(fileFullPath, Encoding.UTF8);
                    list = JsonSerializer.Deserialize<List<GameWinNumberInfo>>(oldJson);
                    list.RemoveAll((i) =>
                    {
                        return i.GameCode == gameCode;
                    });
                    System.IO.File.Delete(fileFullPath);
                }
                list.Add(new GameWinNumberInfo
                {
                    CreateTime = info.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    IssuseNumber = info.IssuseNumber,
                    WinNumber = info.WinNumber,
                });
                //写文件
                var json = JsonSerializer.Serialize(list);
                System.IO.File.WriteAllText(fileFullPath, json, Encoding.UTF8);
            }

            #endregion

            #region 某彩种最新N条开奖号

            var openList = new List<GameWinNumberInfo>();
            WCFClients.ChartClient.QueryGameWinNumber(gameCode, 0, 10).List.ForEach((o) =>
            {
                openList.Add(new GameWinNumberInfo
                {
                    CreateTime = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    GameCode = o.GameCode,
                    GameType = o.GameType,
                    IssuseNumber = o.IssuseNumber,
                    WinNumber = o.WinNumber,
                });
            });
            var openListFilePath = Path.Combine(path, string.Format("lottery_new_open_list_{0}.json", gameCode));
            if (System.IO.File.Exists(openListFilePath))
                System.IO.File.Delete(openListFilePath);

            //写文件
            var openListJson = JsonSerializer.Serialize(openList);
            System.IO.File.WriteAllText(openListFilePath, openListJson, Encoding.UTF8);

            #endregion
        }

        /// <summary>
        /// 生成数字彩最新开奖号
        /// </summary>
        private void BuildLotteryNewNumber()
        {
            #region 生成所有彩种最新开奖号

            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                BuildLotteryNewNumber_ByGameCode(gameCode);
            }

            #endregion
        }

        /// <summary>
        /// 生成 资讯首页 和 分类列表页
        /// </summary>
        private void BuildZiXunIndexAndCategoryPage()
        {
            #region 资讯页首页

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "zixun");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var zixunIndexFileName = Path.Combine(path, "index.html");
            ViewBag.jdxw = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 6);
            ViewBag.ssdp = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", 0, 7);
            ViewBag.szc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", 0, 7);
            ViewBag.gpc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", 0, 7);
            ViewBag.jjc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", 0, 7);
            ViewBag.s_d = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", 0, 7);
            ViewBag.z_s_l = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", 0, 7);
            ViewBag.z_j = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", 0, 7);
            ViewBag.f_p = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", 0, 7);
            ViewBag.zjxw = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 5);
            ViewBag.r_cpbk = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", 0, 10);
            ViewBag.Ads = WCFClients.ExternalClient.QuerySitemessageBanngerList_Web(BannerType.Index_ZiXun);
            BuildViewHtml("zixun/HotCaiXun", zixunIndexFileName);

            #endregion

            #region 分类列表页

            foreach (var c in new string[] { "jdxw", "ssdp", "szczx", "gpczx", "jjczx", "ssq_dlt", "zc_siliu", "zd_jl", "fc3d_pl3_pl5", "zjxw", "cpbk" })
            {
                var categoryPath = Path.Combine(path, c);
                if (!Directory.Exists(categoryPath))
                    Directory.CreateDirectory(categoryPath);

                var pageIndex = 0;
                var pageSize = 30;
                var totalCount = 0;
                while (true)
                {
                    ArticleInfo_QueryCollection data = null;
                    switch (c)
                    {
                        case "jdxw":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "ssdp":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "szczx":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "gpczx":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "jjczx":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "ssq_dlt":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "zc_siliu":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "zd_jl":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "fc3d_pl3_pl5":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "zjxw":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        case "cpbk":
                            data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", pageIndex, pageSize);
                            totalCount = data.TotalCount;
                            break;
                        default:
                            break;
                    }

                    ViewBag.DataSource = data;
                    ViewBag.category = c;
                    ViewBag.PageIndex = pageIndex;
                    ViewBag.PageSize = pageSize;
                    ViewBag.TotalCount = totalCount;
                    ViewBag.Url = string.Format("/statichtml/zixun/{0}/{0}_", c);

                    var fileFullPath = Path.Combine(categoryPath, string.Format("{0}_{1}.html", c, pageIndex));
                    BuildViewHtml("zixun/more", fileFullPath);

                    if (data.ArticleList == null || data.ArticleList.Count < pageSize)
                    {
                        break;
                    }
                    pageIndex++;
                }
            }

            #endregion
        }

        /// <summary>
        /// 生成资讯明细
        /// </summary>
        private void BuildZiXunDetail()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "zixun");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            #region 查询11个类型的前N条最新资讯

            var topNumber = 7;
            var totalCategoryArticleList = new ArticleInfo_QueryCollection();
            foreach (var c in new string[] { "jdxw", "ssdp", "szczx", "gpczx", "jjczx", "ssq_dlt", "zc_siliu", "zd_jl", "fc3d_pl3_pl5", "zjxw", "cpbk" })
            {
                switch (c)
                {
                    case "jdxw":
                        var topN_jdxw = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, topNumber);
                        foreach (var item in topN_jdxw.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "ssdp":
                        var topN_ssdp = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", 0, topNumber);
                        foreach (var item in topN_ssdp.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "szczx":
                        var topN_szczx = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", 0, topNumber);
                        foreach (var item in topN_szczx.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "gpczx":
                        var topN_gpczx = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", 0, topNumber);
                        foreach (var item in topN_gpczx.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "jjczx":
                        var topN_jjczx = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", 0, topNumber);
                        foreach (var item in topN_jjczx.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "ssq_dlt":
                        var topN_ssq_dlt = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", 0, topNumber);
                        foreach (var item in topN_ssq_dlt.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "zc_siliu":
                        var topN_zc_siliu = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", 0, topNumber);
                        foreach (var item in topN_zc_siliu.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "zd_jl":
                        var topN_zd_jl = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", 0, topNumber);
                        foreach (var item in topN_zd_jl.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "fc3d_pl3_pl5":
                        var topN_fc3d_pl3_pl5 = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", 0, topNumber);
                        foreach (var item in topN_fc3d_pl3_pl5.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "zjxw":
                        var topN_zjxw = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, topNumber);
                        foreach (var item in topN_zjxw.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    case "cpbk":
                        var topN_cpbk = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", 0, topNumber);
                        foreach (var item in topN_cpbk.ArticleList)
                        {
                            totalCategoryArticleList.ArticleList.Add(item);
                        }
                        break;
                    default:
                        break;
                }
            }

            #endregion

            //生成分类文章的详细
            foreach (var item in totalCategoryArticleList.ArticleList)
            {
                BuildArticleDetail(item, path);
            }

            //生成其它文章
            var pageIndex = 0;
            var pageSize = 25;
            while (true)
            {
                var list = WCFClients.ExternalClient.QueryNoStaticPathArticleList(pageIndex, pageSize);
                foreach (var item in list.ArticleList)
                {
                    BuildArticleDetail(item, path);
                }

                if (list.ArticleList == null || list.ArticleList.Count < pageSize)
                {
                    break;
                }
                pageIndex++;
            }
        }

        /// <summary>
        /// 生成一条资讯详细
        /// </summary>
        private void BuildArticleDetail(ArticleInfo_Query info, string path)
        {
            var detailPath = Path.Combine(path, "details", info.CreateTime.ToString("yyyyMMdd"));
            if (!Directory.Exists(detailPath))
                Directory.CreateDirectory(detailPath);
            var fileFullPath = Path.Combine(detailPath, string.Format("{0}.html", info.Id));
            var urlPath = string.Format("/statichtml/zixun/details/{0}/{1}.html", info.CreateTime.ToString("yyyyMMdd"), info.Id);
            info.StaticPath = urlPath;
            ViewBag.Details = info;
            ViewBag.category = FindArticleCategory(info);
            ViewBag.textNum = info.Id;
            BuildViewHtml("zixun/textpart", fileFullPath);

            //更新数据库
            WCFClients.ExternalClient.UpdateArticleStaticPath(info.Id, urlPath, info.PreId, info.NextId);
        }

        /// <summary>
        /// 查找文章小分类
        /// </summary>
        private string FindArticleCategory(ArticleInfo_Query info)
        {
            switch (info.Category)
            {
                case "FocusCMS":
                    return "jdxw";
                case "Match_Comment":
                    return "ssdp";
                case "BonusCMS":
                    return "zjxw";
                case "Lottery_Know":
                    return "cpbk";
                case "Lottery_GameCode":
                    if ("SSQ|FC3D|DLT|PL3|CQSSC|JX11X5|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3".Split('|').Contains(info.GameCode))
                        return "szczx";
                    if (info.GameCode == "JCZQ")
                        return "jjczx";
                    if ("T14C|TR9|T6BQC|T4CJQ".Split('|').Contains(info.GameCode))
                        return "zc_siliu";
                    if ("JCLQ|BJDC".Split('|').Contains(info.GameCode))
                        return "zd_jl";
                    break;
            }
            return "jdxw";
        }

        /// <summary>
        /// 生成公告
        /// </summary>
        private void BuildBulletin()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "bulletin");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var detailPath = Path.Combine(path, "detail");
            if (!Directory.Exists(detailPath))
                Directory.CreateDirectory(detailPath);

            var pageIndex = 0;
            var pageSize = 30;
            while (true)
            {
                //公告列表
                var fileFullPath = Path.Combine(path, string.Format("notice_list_{0}.html", pageIndex));
                var list = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, pageIndex, pageSize, UserToken);
                ViewBag.NoticeList = list;
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = list.TotalCount;
                ViewBag.Url = "/statichtml/bulletin/notice_list_";
                BuildViewHtml("zixun/notecilist", fileFullPath);

                foreach (var notice in list.BulletinList)
                {
                    //公告详细
                    ViewBag.NoticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, 0, 10, UserToken);
                    ViewBag.Notice = WCFClients.ExternalClient.QueryDisplayBulletinDetailById(notice.Id);
                    var detailFileFullPath = Path.Combine(detailPath, string.Format("{0}.html", notice.Id));
                    BuildViewHtml("zixun/notice", detailFileFullPath);
                }

                if (list.BulletinList == null || list.BulletinList.Count < pageSize)
                {
                    break;
                }
                pageIndex++;
            }
        }

        /// <summary>
        /// 生成神单主页
        /// </summary>
        private void BuildReDan()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "baodan");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("baodan/redan", filePath);
        }

        /// <summary>
        /// 生成走势图主页
        /// </summary>
        private void BuildLotteryTrend()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("lotterytrend/tendency", filePath);
        }

        /// <summary>
        /// 生成指定彩种的走势图
        /// </summary>
        private void BuildLotteryTrend_ByGameCode(string gameCode)
        {
            switch (gameCode.ToUpper())
            {
                case "SSQ":
                    BuildLotteryTrend_SSQ();
                    break;
                case "DLT":
                    BuildLotteryTrend_DLT();
                    break;
                case "FC3D":
                    BuildLotteryTrend_FC3D();
                    break;
                case "PL3":
                    BuildLotteryTrend_PL3();
                    break;
                case "CQSSC":
                    BuildLotteryTrend_CQSSC();
                    break;
                case "JX11X5":
                    BuildLotteryTrend_JX11X5();
                    break;
                case "SD11X5":
                    BuildLotteryTrend_SD11X5();
                    break;
                case "GD11X5":
                    BuildLotteryTrend_GD11X5();
                    break;
                case "GDKLSF":
                    BuildLotteryTrend_GDKLSF();
                    break;
                case "JSKS":
                    BuildLotteryTrend_JSKS();
                    break;
                case "SDKLPK3":
                    BuildLotteryTrend_SDKLPK3();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 生成双色球走势图
        /// </summary>
        private void BuildLotteryTrend_SSQ()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "SSQ");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryCache_SSQ_JiBenZouSi_Info(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryCache_SSQ_DX_Info(totalPageSize);
            var c3_list = WCFClients.ChartClient.QueryCache_SSQ_C3_Info(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryCache_SSQ_JiOu_Info(totalPageSize);
            var kdsw_list = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_SW_Info(totalPageSize);
            var hezhi_list = WCFClients.ChartClient.QueryCache_SSQ_HeZhi_Info(totalPageSize);
            var zhihe_list = WCFClients.ChartClient.QueryCache_SSQ_ZhiHe_Info(totalPageSize);
            var kd1_6_list = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/jbzs", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("daxiao{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/DaXiao", filePath);

                ViewBag.DataList = c3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/Chu3", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jiou{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/JiOu", filePath);

                ViewBag.DataList = kdsw_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_sw{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_sw", filePath);

                ViewBag.DataList = hezhi_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hezhi{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/HeZhi", filePath);

                ViewBag.DataList = zhihe_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ZhiHe{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/zhihe", filePath);

                ViewBag.DataList = kd1_6_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_12{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_12", filePath);

                ViewBag.DataList = kd1_6_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_23{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_23", filePath);

                ViewBag.DataList = kd1_6_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_34{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_34", filePath);

                ViewBag.DataList = kd1_6_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_45{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_45", filePath);

                ViewBag.DataList = kd1_6_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_56{0}.html", item));
                BuildViewHtml("lotterytrend/SSQ/kuadu_56", filePath);
            }
        }

        /// <summary>
        /// 生成福彩3D走势图
        /// </summary>
        private void BuildLotteryTrend_FC3D()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "FC3D");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var zxzs_list = WCFClients.ChartClient.QueryCache_FC3D_ZhiXuanZouSi_Info(totalPageSize);
            var kd_list = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_Z_Info(totalPageSize);
            var hwzs_list = WCFClients.ChartClient.QueryCache_FC3D_HZZS_Info(totalPageSize);
            var johm_list = WCFClients.ChartClient.QueryCache_FC3D_JOHM_Info(totalPageSize);
            var chu31_list = WCFClients.ChartClient.QueryCache_FC3D_Chu33_Info(totalPageSize);
            var dxhm_list = WCFClients.ChartClient.QueryCache_FC3D_DXHM_Info(totalPageSize);
            var zuxzs_list = WCFClients.ChartClient.QueryCache_FC3D_ZuXuanZouSi_Info(totalPageSize);
            var chu32_list = WCFClients.ChartClient.QueryCache_FC3D_Chu31_Info(totalPageSize);
            var chu33_list = WCFClients.ChartClient.QueryCache_FC3D_Chu32_Info(totalPageSize);
            var kd12_list = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_12_Info(totalPageSize);
            var kd13_list = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_13_Info(totalPageSize);
            var kd23_list = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_23_Info(totalPageSize);
            var dxxt_list = WCFClients.ChartClient.QueryCache_FC3D_DXXT_Info(totalPageSize);
            var hzfb_list = WCFClients.ChartClient.QueryCache_FC3D_HZFB_Info(totalPageSize);
            var hztz_list = WCFClients.ChartClient.QueryCache_FC3D_HZTZ_Info(totalPageSize);
            var joxt_list = WCFClients.ChartClient.QueryCache_FC3D_JOXT_Info(totalPageSize);
            var zhhm_list = WCFClients.ChartClient.QueryCache_FC3D_ZHHM_Info(totalPageSize);
            var zhxt_list = WCFClients.ChartClient.QueryCache_FC3D_ZHXT_Info(totalPageSize);

            foreach (var item in arr)
            {
                ViewBag.DataList = zxzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/zxzs", filePath);

                ViewBag.DataList = kd_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/kuadu", filePath);

                ViewBag.DataList = hwzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hwzs{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/hwzs", filePath);

                ViewBag.DataList = johm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("johm{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/johm", filePath);

                ViewBag.DataList = chu31_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu31{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/chu31", filePath);

                ViewBag.DataList = dxhm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dxhm{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/dxhm", filePath);

                ViewBag.DataList = zuxzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zuxzs{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/zuxzs", filePath);

                ViewBag.DataList = chu32_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu32{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/chu32", filePath);

                ViewBag.DataList = chu33_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu33{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/chu33", filePath);

                ViewBag.DataList = kd12_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu12{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/kuadu12", filePath);

                ViewBag.DataList = kd13_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu13{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/kuadu13", filePath);

                ViewBag.DataList = kd23_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu23{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/kuadu23", filePath);

                ViewBag.DataList = dxxt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dxxt{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/dxxt", filePath);

                ViewBag.DataList = hzfb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzfb{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/hzfb", filePath);

                ViewBag.DataList = hztz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hztz{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/hztz", filePath);

                ViewBag.DataList = joxt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("joxt{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/joxt", filePath);

                ViewBag.DataList = zhhm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhhm{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/zhhm", filePath);

                ViewBag.DataList = zhxt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhxt{0}.html", item));
                BuildViewHtml("lotterytrend/FC3D/zhxt", filePath);
            }
        }

        /// <summary>
        /// 生成七乐彩走势图
        /// </summary>
        private void BuildLotteryTrend_QLC()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "QLC");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryQLC_JBZS(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryQLC_DX(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryQLC_JO(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryQLC_ZH(totalPageSize);
            var chu3_list = WCFClients.ChartClient.QueryQLC_Chu3(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/QLC/jbzs", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/QLC/dx", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/QLC/jo", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/QLC/zh", filePath);

                ViewBag.DataList = chu3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/QLC/chu3", filePath);
            }
        }

        /// <summary>
        /// 生成吉林快3走势图
        /// </summary>
        private void BuildLotteryTrend_JLK3()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "JLK3");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryJLK3_JBZS_Info(totalPageSize);
            var hz_list = WCFClients.ChartClient.QueryJLK3_HZ_Info(totalPageSize);
            var xt_list = WCFClients.ChartClient.QueryJLK3_XT_Info(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryJLK3_ZH_Info(totalPageSize);
            var zhzs_list = WCFClients.ChartClient.QueryJLK3_ZHZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/JLK3/jbzs", filePath);

                ViewBag.DataList = hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hz{0}.html", item));
                BuildViewHtml("lotterytrend/JLK3/hz", filePath);

                ViewBag.DataList = xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xt{0}.html", item));
                BuildViewHtml("lotterytrend/JLK3/xt", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/JLK3/zh", filePath);

                ViewBag.DataList = zhzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/JLK3/zhzs", filePath);

            }
        }

        /// <summary>
        /// 生成江苏快3走势图
        /// </summary>
        private void BuildLotteryTrend_JSKS()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "JSKS");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryJSK3_JBZS_Info(totalPageSize);
            var hz_list = WCFClients.ChartClient.QueryJSK3_HZ_Info(totalPageSize);
            var xt_list = WCFClients.ChartClient.QueryJSK3_XT_Info(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryJSK3_ZH_Info(totalPageSize);
            var zhzs_list = WCFClients.ChartClient.QueryJSK3_ZHZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/JSKS/jbzs", filePath);

                ViewBag.DataList = hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hz{0}.html", item));
                BuildViewHtml("lotterytrend/JSKS/hz", filePath);

                ViewBag.DataList = xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xt{0}.html", item));
                BuildViewHtml("lotterytrend/JSKS/xt", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/JSKS/zh", filePath);

                ViewBag.DataList = zhzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/JSKS/zhzs", filePath);
            }
        }

        /// <summary>
        /// 生成山东快乐扑克3走势图
        /// </summary>
        private void BuildLotteryTrend_SDKLPK3()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "SDKLPK3");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs = WCFClients.ChartClient.QueryCache_SDKLPK3_JBZS_Info(totalPageSize);
            var zhxzs = WCFClients.ChartClient.QueryCache_SDKLPK3_ZHXZS_Info(totalPageSize);
            var hszs = WCFClients.ChartClient.QueryCache_SDKLPK3_HSZS_Info(totalPageSize);
            var dxzs = WCFClients.ChartClient.QueryCache_SDKLPK3_DXZS_Info(totalPageSize);
            var jozs = WCFClients.ChartClient.QueryCache_SDKLPK3_JOZS_Info(totalPageSize);
            var zhzs = WCFClients.ChartClient.QueryCache_SDKLPK3_ZHZS_Info(totalPageSize);
            var c3yzs = WCFClients.ChartClient.QueryCache_SDKLPK3_C3YZS_Info(totalPageSize);
            var hzzs = WCFClients.ChartClient.QueryCache_SDKLPK3_HZZS_Info(totalPageSize);
            var lxzs = WCFClients.ChartClient.QueryCache_SDKLPK3_LXZS_Info(totalPageSize);

            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/jbzs", filePath);

                ViewBag.DataList = zhxzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhxzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/zhxzs", filePath);

                ViewBag.DataList = hszs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hszs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/hszs", filePath);

                ViewBag.DataList = dxzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/dxzs", filePath);

                ViewBag.DataList = jozs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jozs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/jozs", filePath);

                ViewBag.DataList = zhzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/zhzs", filePath);

                ViewBag.DataList = c3yzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("c3yzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/c3yzs", filePath);

                ViewBag.DataList = hzzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/hzzs", filePath);

                ViewBag.DataList = lxzs.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("lxzs{0}.html", item));
                BuildViewHtml("lotterytrend/SDKLPK3/lxzs", filePath);
            }
        }

        /// <summary>
        /// 生成湖北快3走势图
        /// </summary>
        private void BuildLotteryTrend_HBK3()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "HBK3");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryHBK3_JBZS_Info(totalPageSize);
            var hz_list = WCFClients.ChartClient.QueryHBK3_HZ_Info(totalPageSize);
            var xt_list = WCFClients.ChartClient.QueryHBK3_XT_Info(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryHBK3_ZH_Info(totalPageSize);
            var zhzs_list = WCFClients.ChartClient.QueryHBK3_ZHZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/HBK3/jbzs", filePath);

                ViewBag.DataList = hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hz{0}.html", item));
                BuildViewHtml("lotterytrend/HBK3/hz", filePath);

                ViewBag.DataList = xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xt{0}.html", item));
                BuildViewHtml("lotterytrend/HBK3/xt", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/HBK3/zh", filePath);

                ViewBag.DataList = zhzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/HBK3/zhzs", filePath);
            }

        }

        /// <summary>
        /// 生成山东群英会走势图
        /// </summary>
        private void BuildLotteryTrend_SDQYH()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "SDQYH");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chu3_list = WCFClients.ChartClient.QuerySDQYH_Chu3_Info(totalPageSize);
            var dx_list = WCFClients.ChartClient.QuerySDQYH_RXDX_Info(totalPageSize);
            var jo_list = WCFClients.ChartClient.QuerySDQYH_RXJO_Info(totalPageSize);
            var sx1_list = WCFClients.ChartClient.QuerySDQYH_SX1_Info(totalPageSize);
            var sx2_list = WCFClients.ChartClient.QuerySDQYH_SX2_Info(totalPageSize);
            var sx3_list = WCFClients.ChartClient.QuerySDQYH_SX3_Info(totalPageSize);
            var zh_list = WCFClients.ChartClient.QuerySDQYH_RXZH_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chu3_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/chu3", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/dx", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/jo", filePath);

                ViewBag.DataList = sx1_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("sx1{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/sx1", filePath);

                ViewBag.DataList = sx2_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("sx2{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/sx2", filePath);

                ViewBag.DataList = sx3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("sx3{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/sx3", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/SDQYH/zh", filePath);
            }
        }

        /// <summary>
        /// 生成好彩1走势图
        /// </summary>
        private void BuildLotteryTrend_HC1()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "HC1");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var jbzs_list = WCFClients.ChartClient.QueryCache_HC1_JBZS_Info(totalPageSize);
            var sxjjf_list = WCFClients.ChartClient.QueryCache_HC1_SXJJFWZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = jbzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/HC1/jbzs", filePath);

                ViewBag.DataList = sxjjf_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("sxjjfwzs{0}.html", item));
                BuildViewHtml("lotterytrend/HC1/sxjjfwzs", filePath);
            }
        }

        /// <summary>
        /// 生成华东15选5走势图
        /// </summary>
        private void BuildLotteryTrend_HD15X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "HD15X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chzs_list = WCFClients.ChartClient.QueryCache_HD15X5_CH_Info(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryHD15X5_DX(totalPageSize);
            var hz_list = WCFClients.ChartClient.QueryHD15X5_HZ(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryHD15X5_JBZS(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryHD15X5_JO(totalPageSize);
            var lhzs_list = WCFClients.ChartClient.QueryCache_HD15X5_LH_Info(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryHD15X5_ZH(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chzs{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/chzs", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/dx", filePath);

                ViewBag.DataList = hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hz{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/hz", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/jo", filePath);

                ViewBag.DataList = lhzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("lhzs{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/lhzs", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/HD15X5/zh", filePath);
            }
        }

        /// <summary>
        /// 东方6+1走势图
        /// </summary>
        private void BuildLotteryTrend_DF6J1()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "DF6J1");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dxzs_list = WCFClients.ChartClient.QueryCache_DF6_1_DXZS_Info(totalPageSize);
            var hzzs_list = WCFClients.ChartClient.QueryCache_DF6_1_HZZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_DF6_1_JBZS_Info(totalPageSize);
            var jozs_list = WCFClients.ChartClient.QueryCache_DF6_1_JOZS_Info(totalPageSize);
            var kdzs_list = WCFClients.ChartClient.QueryCache_DF6_1_KDZS_Info(totalPageSize);
            var zhzs_list = WCFClients.ChartClient.QueryCache_DF6_1_ZHZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = dxzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/dxzs", filePath);

                ViewBag.DataList = hzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/hzzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/jbzs", filePath);

                ViewBag.DataList = jozs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jozs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/jozs", filePath);

                ViewBag.DataList = kdzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdzs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/kdzs", filePath);

                ViewBag.DataList = zhzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/DF6J1/zhzs", filePath);
            }
        }

        /// <summary>
        /// 生成大乐透走势图
        /// </summary>
        private void BuildLotteryTrend_DLT()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "DLT");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chu3_list = WCFClients.ChartClient.QueryDLT_Chu3_Info(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryDLT_DX_Info(totalPageSize);
            var hezhi_list = WCFClients.ChartClient.QueryDLT_HeZhi_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryDLT_JiBenZouSi_Info(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryDLT_JiOu_Info(totalPageSize);
            var kd12_list = WCFClients.ChartClient.QueryDLT_KuaDu_12_Info(totalPageSize);
            var kd23_list = WCFClients.ChartClient.QueryDLT_KuaDu_23_Info(totalPageSize);
            var kd34_list = WCFClients.ChartClient.QueryDLT_KuaDu_34_Info(totalPageSize);
            var kd45_list = WCFClients.ChartClient.QueryDLT_KuaDu_45_Info(totalPageSize);
            var kdsw_list = WCFClients.ChartClient.QueryDLT_KuaDu_SW_Info(totalPageSize);
            var zhihe_list = WCFClients.ChartClient.QueryDLT_ZhiHe_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chu3_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/chu3", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/dx", filePath);

                ViewBag.DataList = hezhi_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hezhi{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/hezhi", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jiou{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/jiou", filePath);

                ViewBag.DataList = kd12_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_12{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/kuadu_12", filePath);

                ViewBag.DataList = kd23_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_23{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/kuadu_23", filePath);

                ViewBag.DataList = kd34_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_34{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/kuadu_34", filePath);

                ViewBag.DataList = kd45_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_45{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/kuadu_45", filePath);

                ViewBag.DataList = kdsw_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kuadu_sw{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/kuadu_sw", filePath);

                ViewBag.DataList = zhihe_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhihe{0}.html", item));
                BuildViewHtml("lotterytrend/DLT/zhihe", filePath);
            }
        }

        /// <summary>
        /// 生成排列三走势图
        /// </summary>
        private void BuildLotteryTrend_PL3()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "PL3");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chu31_list = WCFClients.ChartClient.QueryPL3_PL3_Chu33_Info(totalPageSize);
            var chu32_list = WCFClients.ChartClient.QueryPL3_PL3_Chu31_Info(totalPageSize);
            var chu33_list = WCFClients.ChartClient.QueryPL3_PL3_Chu32_Info(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryPL3_DX_info(totalPageSize);
            var dxhm_list = WCFClients.ChartClient.QueryPL3_DXHM_info(totalPageSize);
            var hezhi_list = WCFClients.ChartClient.QueryPL3_HeiZhi_Info(totalPageSize);
            var hzhw_list = WCFClients.ChartClient.QueryPL3_PL3_HZHW_Info(totalPageSize);
            var hztz_list = WCFClients.ChartClient.QueryPL3_PL3_HZTZ_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryPL3_JiBenZouSi_Info(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryPL3_JIOU_info(totalPageSize);
            var johm_list = WCFClients.ChartClient.QueryPL3_JOHM_info(totalPageSize);
            var kdbg_list = WCFClients.ChartClient.QueryPL3_KuaDu_13_Info(totalPageSize);
            var kdbs_list = WCFClients.ChartClient.QueryPL3_KuaDu_12_Info(totalPageSize);
            var kdsg_list = WCFClients.ChartClient.QueryPL3_KuaDu_23_Info(totalPageSize);
            var zhhm_list = WCFClients.ChartClient.QueryPL3_ZHHM_info(totalPageSize);
            var zhihe_list = WCFClients.ChartClient.QueryPL3_ZhiHe_info(totalPageSize);
            var zxzs_list = WCFClients.ChartClient.QueryPL3_ZuXuanZouSi_info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chu31_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chu31{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/chu31", filePath);

                ViewBag.DataList = chu32_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu32{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/chu32", filePath);

                ViewBag.DataList = chu33_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("chu33{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/chu33", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/dx", filePath);

                ViewBag.DataList = dxhm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dxhm{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/dxhm", filePath);

                ViewBag.DataList = hezhi_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hezhi{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/hezhi", filePath);

                ViewBag.DataList = hzhw_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzhw{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/hzhw", filePath);

                ViewBag.DataList = hztz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hztz{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/hztz", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/jo", filePath);

                ViewBag.DataList = johm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("johm{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/johm", filePath);

                ViewBag.DataList = kdbg_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdbg{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/kdbg", filePath);

                ViewBag.DataList = kdbs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdbs{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/kdbs", filePath);

                ViewBag.DataList = kdsg_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdsg{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/kdsg", filePath);

                ViewBag.DataList = zhhm_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhhm{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/zhhm", filePath);

                ViewBag.DataList = zhihe_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zhihe{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/zhihe", filePath);

                ViewBag.DataList = zxzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/PL3/zxzs", filePath);
            }
        }

        /// <summary>
        /// 生成排列五走势图
        /// </summary>
        private void BuildLotteryTrend_PL5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "PL5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chu3_list = WCFClients.ChartClient.QueryPL5_Chu3(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryPL5_DX(totalPageSize);
            var hz_list = WCFClients.ChartClient.QueryPL5_HZ(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryPL5_JBZS(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryPL5_JO(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryPL5_ZH(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chu3_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/chu3", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/dx", filePath);

                ViewBag.DataList = hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hz{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/hz", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/jo", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/PL5/zh", filePath);
            }
        }

        /// <summary>
        /// 生成七星彩走势图
        /// </summary>
        private void BuildLotteryTrend_QXC()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "QXC");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chu3_list = WCFClients.ChartClient.QueryQXC_Chu3(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryQXC_DX(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryQXC_JBZS(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryQXC_JO(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryQXC_ZH(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chu3_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chu3{0}.html", item));
                BuildViewHtml("lotterytrend/QXC/chu3", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/QXC/dx", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/QXC/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/QXC/jo", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/QXC/zh", filePath);
            }
        }

        /// <summary>
        /// 生成山东11选5走势图
        /// </summary>
        private void BuildLotteryTrend_SD11X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "SD11X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chzs_list = WCFClients.ChartClient.QueryCache_YDJ11_CHZS_Info(totalPageSize);
            var dlzs_list = WCFClients.ChartClient.QueryCache_YDJ11_DLZS_Info(totalPageSize);
            var dwzs_list = WCFClients.ChartClient.QueryCache_YDJ11_012DWZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_YDJ11_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_YDJ11_GHZS_Info(totalPageSize);
            var hzzs_list = WCFClients.ChartClient.QueryCache_YDJ11_HZZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_YDJ11_JBZS_Info(totalPageSize);
            var kdzs_list = WCFClients.ChartClient.QueryCache_YDJ11_KDZS_Info(totalPageSize);
            var lzzs_list = WCFClients.ChartClient.QueryCache_YDJ11_012LZZS_Info(totalPageSize);
            var q1jb_list = WCFClients.ChartClient.QueryCache_YDJ11_Q1JBZS_Info(totalPageSize);
            var q1xt_list = WCFClients.ChartClient.QueryCache_YDJ11_Q1XTZS_Info(totalPageSize);
            var q2jb_list = WCFClients.ChartClient.QueryCache_YDJ11_Q2JBZS_Info(totalPageSize);
            var q2xt_list = WCFClients.ChartClient.QueryCache_YDJ11_Q2XTZS_Info(totalPageSize);
            var q3jb_list = WCFClients.ChartClient.QueryCache_YDJ11_Q3JBZS_Info(totalPageSize);
            var q3xt_list = WCFClients.ChartClient.QueryCache_YDJ11_Q3XTZS_Info(totalPageSize);
            var xtzs_list = WCFClients.ChartClient.QueryCache_YDJ11_XTZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/chzs", filePath);

                ViewBag.DataList = dlzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dlzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/dlzs", filePath);

                ViewBag.DataList = dwzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dwzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/dwzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/ghzs", filePath);

                ViewBag.DataList = hzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/hzzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/jbzs", filePath);

                ViewBag.DataList = kdzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/kdzs", filePath);

                ViewBag.DataList = lzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("lzzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/lzzs", filePath);

                ViewBag.DataList = q1jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q1jbzs", filePath);

                ViewBag.DataList = q1xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q1xtzs", filePath);

                ViewBag.DataList = q2jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q2jbzs", filePath);

                ViewBag.DataList = q2xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q2xtzs", filePath);

                ViewBag.DataList = q3jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q3jbzs", filePath);

                ViewBag.DataList = q3xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/q3xtzs", filePath);

                ViewBag.DataList = xtzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/SD11X5/xtzs", filePath);
            }
        }

        /// <summary>
        /// 生成广东11选5走势图
        /// </summary>
        private void BuildLotteryTrend_GD11X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "GD11X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chzs_list = WCFClients.ChartClient.QueryCache_GD11X5_CHZS_Info(totalPageSize);
            var dlzs_list = WCFClients.ChartClient.QueryCache_GD11X5_DLZS_Info(totalPageSize);
            var dwzs_list = WCFClients.ChartClient.QueryCache_GD11X5_012DWZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_GD11X5_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_GD11X5_GHZS_Info(totalPageSize);
            var hzzs_list = WCFClients.ChartClient.QueryCache_GD11X5_HZZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_GD11X5_JBZS_Info(totalPageSize);
            var kdzs_list = WCFClients.ChartClient.QueryCache_GD11X5_KDZS_Info(totalPageSize);
            var lzzs_list = WCFClients.ChartClient.QueryCache_GD11X5_012LZZS_Info(totalPageSize);
            var q1jb_list = WCFClients.ChartClient.QueryCache_GD11X5_Q1JBZS_Info(totalPageSize);
            var q1xt_list = WCFClients.ChartClient.QueryCache_GD11X5_Q1XTZS_Info(totalPageSize);
            var q2jb_list = WCFClients.ChartClient.QueryCache_GD11X5_Q2JBZS_Info(totalPageSize);
            var q2xt_list = WCFClients.ChartClient.QueryCache_GD11X5_Q2XTZS_Info(totalPageSize);
            var q3jb_list = WCFClients.ChartClient.QueryCache_GD11X5_Q3JBZS_Info(totalPageSize);
            var q3xt_list = WCFClients.ChartClient.QueryCache_GD11X5_Q3XTZS_Info(totalPageSize);
            var xtzs_list = WCFClients.ChartClient.QueryCache_GD11X5_XTZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/chzs", filePath);

                ViewBag.DataList = dlzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dlzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/dlzs", filePath);

                ViewBag.DataList = dwzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dwzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/dwzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/ghzs", filePath);

                ViewBag.DataList = hzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/hzzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/jbzs", filePath);

                ViewBag.DataList = kdzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/kdzs", filePath);

                ViewBag.DataList = lzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("lzzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/lzzs", filePath);

                ViewBag.DataList = q1jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q1jbzs", filePath);

                ViewBag.DataList = q1xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q1xtzs", filePath);

                ViewBag.DataList = q2jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q2jbzs", filePath);

                ViewBag.DataList = q2xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q2xtzs", filePath);

                ViewBag.DataList = q3jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q3jbzs", filePath);

                ViewBag.DataList = q3xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/q3xtzs", filePath);

                ViewBag.DataList = xtzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/GD11X5/xtzs", filePath);
            }
        }

        /// <summary>
        /// 生成江西11选5走势图
        /// </summary>
        private void BuildLotteryTrend_JX11X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "JX11X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var q2hz_list = WCFClients.ChartClient.QueryJX11X5_Q2HZ_Info(totalPageSize);
            var q2zu_list = WCFClients.ChartClient.QueryJX11X5_Q2ZUS_Info(totalPageSize);
            var q2zx_list = WCFClients.ChartClient.QueryJX11X5_Q2ZS_Info(totalPageSize);
            var q3chu3_list = WCFClients.ChartClient.QueryJX11X5_Q3Chu3_Info(totalPageSize);
            var q3dx_list = WCFClients.ChartClient.QueryJX11X5_Q3DX_Info(totalPageSize);
            var q3hz_list = WCFClients.ChartClient.QueryJX11X5_Q3HZ_Info(totalPageSize);
            var q3jo_list = WCFClients.ChartClient.QueryJX11X5_Q3JO_Info(totalPageSize);
            var q3zh_list = WCFClients.ChartClient.QueryJX11X5_Q3ZH_Info(totalPageSize);
            var q3zu_list = WCFClients.ChartClient.QueryJX11X5_Q3ZUS_Info(totalPageSize);
            var q3zx_list = WCFClients.ChartClient.QueryJX11X5_Q3ZS_Info(totalPageSize);
            var rx1_list = WCFClients.ChartClient.QueryJX11X5_RX1_Info(totalPageSize);
            var rx2_list = WCFClients.ChartClient.QueryJX11X5_RX2_Info(totalPageSize);
            var rx3_list = WCFClients.ChartClient.QueryJX11X5_RX3_Info(totalPageSize);
            var rx4_list = WCFClients.ChartClient.QueryJX11X5_RX4_Info(totalPageSize);
            var rx5_list = WCFClients.ChartClient.QueryJX11X5_RX5_Info(totalPageSize);
            var rxchu3_list = WCFClients.ChartClient.QueryJX11X5_Chu3_Info(totalPageSize);
            var rxdx_list = WCFClients.ChartClient.QueryJX11X5_RXDX_Info(totalPageSize);
            var rxhz_list = WCFClients.ChartClient.QueryJX11X5_RXHZ_Info(totalPageSize);
            var rs_list = WCFClients.ChartClient.QueryJX11X5_RXJBZS_Info(totalPageSize);
            var ro_list = WCFClients.ChartClient.QueryJX11X5_RXJO_Info(totalPageSize);
            var rh_list = WCFClients.ChartClient.QueryJX11X5_RXZH_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = q2hz_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("q2hz{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q2hz", filePath);

                ViewBag.DataList = q2zu_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2zux{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q2zux", filePath);

                ViewBag.DataList = q2zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2zx{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q2zx", filePath);

                ViewBag.DataList = q3chu3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3chu3{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3chu3", filePath);

                ViewBag.DataList = q3dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3dx{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3dx", filePath);

                ViewBag.DataList = q3hz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3hz{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3hz", filePath);

                ViewBag.DataList = q3jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3jo{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3jo", filePath);

                ViewBag.DataList = q3zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zh{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3zh", filePath);

                ViewBag.DataList = q3zu_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zux{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3zux", filePath);

                ViewBag.DataList = q3zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zx{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/q3zx", filePath);

                ViewBag.DataList = rx1_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rx1{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rx1", filePath);

                ViewBag.DataList = rx2_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rx2{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rx2", filePath);

                ViewBag.DataList = rx3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rx3{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rx3", filePath);

                ViewBag.DataList = rx4_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rx4{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rx4", filePath);

                ViewBag.DataList = rx5_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rx5{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rx5", filePath);

                ViewBag.DataList = rxchu3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxchu3{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxchu3", filePath);

                ViewBag.DataList = rxdx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxdx{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxdx", filePath);

                ViewBag.DataList = rxhz_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxhz{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxhz", filePath);

                ViewBag.DataList = rs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxjbzs{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxjbzs", filePath);

                ViewBag.DataList = ro_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxjo{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxjo", filePath);

                ViewBag.DataList = rh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("rxzh{0}.html", item));
                BuildViewHtml("lotterytrend/JX11X5/rxzh", filePath);
            }
        }

        /// <summary>
        /// 生成重庆11选5走势图
        /// </summary>
        private void BuildLotteryTrend_CQ11X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "CQ11X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_CHZS_Info(totalPageSize);
            var dlzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_DLZS_Info(totalPageSize);
            var dwzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_012DWZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_GHZS_Info(totalPageSize);
            var hzzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_HZZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_JBZS_Info(totalPageSize);
            var kdzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_KDZS_Info(totalPageSize);
            var lzzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_012LZZS_Info(totalPageSize);
            var q1jb_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q1JBZS_Info(totalPageSize);
            var q1xt_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q1XTZS_Info(totalPageSize);
            var q2jb_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q2JBZS_Info(totalPageSize);
            var q2xt_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q2XTZS_Info(totalPageSize);
            var q3jb_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q3JBZS_Info(totalPageSize);
            var q3xt_list = WCFClients.ChartClient.QueryCache_CQ11X5_Q3XTZS_Info(totalPageSize);
            var xtzs_list = WCFClients.ChartClient.QueryCache_CQ11X5_XTZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/chzs", filePath);

                ViewBag.DataList = dlzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dlzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/dlzs", filePath);

                ViewBag.DataList = dwzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dwzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/dwzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/ghzs", filePath);

                ViewBag.DataList = hzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/hzzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/jbzs", filePath);

                ViewBag.DataList = kdzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("kdzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/kdzs", filePath);

                ViewBag.DataList = lzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("lzzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/lzzs", filePath);

                ViewBag.DataList = q1jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q1jbzs", filePath);

                ViewBag.DataList = q1xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q1xtzs", filePath);

                ViewBag.DataList = q2jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q2jbzs", filePath);

                ViewBag.DataList = q2xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q2xtzs", filePath);

                ViewBag.DataList = q3jb_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q3jbzs", filePath);

                ViewBag.DataList = q3xt_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/q3xtzs", filePath);

                ViewBag.DataList = xtzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("xtzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQ11X5/xtzs", filePath);
            }
        }

        /// <summary>
        /// 生成辽宁11选5走势图
        /// </summary>
        private void BuildLotteryTrend_LN11X5()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "LN11X5");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var chzs_list = WCFClients.ChartClient.QueryCache_LN11X5_CHZS_Info(totalPageSize);
            var dlzs_list = WCFClients.ChartClient.QueryCache_LN11X5_DLZS_Info(totalPageSize);
            var dxzs_list = WCFClients.ChartClient.QueryCache_LN11X5_DXZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_LN11X5_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_LN11X5_GHZS_Info(totalPageSize);
            var hzzs_list = WCFClients.ChartClient.QueryCache_LN11X5_HZZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_LN11X5_JBZS_Info(totalPageSize);
            var jozs_list = WCFClients.ChartClient.QueryCache_LN11X5_JOZS_Info(totalPageSize);
            var q1zs_list = WCFClients.ChartClient.QueryCache_LN11X5_Q1ZS_Info(totalPageSize);
            var q2zs_list = WCFClients.ChartClient.QueryCache_LN11X5_Q2ZS_Info(totalPageSize);
            var q3zs_list = WCFClients.ChartClient.QueryCache_LN11X5_Q3ZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = chzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("chzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/chzs", filePath);

                ViewBag.DataList = dlzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dlzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/dlzs", filePath);

                ViewBag.DataList = dxzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/dxzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/ghzs", filePath);

                ViewBag.DataList = hzzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/hzzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/jbzs", filePath);

                ViewBag.DataList = jozs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jozs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/jozs", filePath);

                ViewBag.DataList = q1zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1zs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/q1zs", filePath);

                ViewBag.DataList = q2zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q2zs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/q2zs", filePath);

                ViewBag.DataList = q3zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zs{0}.html", item));
                BuildViewHtml("lotterytrend/LN11X5/q3zs", filePath);
            }
        }

        /// <summary>
        /// 生成重庆时时彩走势图
        /// </summary>
        private void BuildLotteryTrend_CQSSC()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "CQSSC");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dxds_list = WCFClients.ChartClient.QueryCache_CQSSC_DXDS_Info(totalPageSize);
            var slzs_list = WCFClients.ChartClient.QueryCache_CQSSC_1X_ZS_Info(totalPageSize);
            var s2h_list = WCFClients.ChartClient.QueryCache_CQSSC_2X_HZZS_Info(totalPageSize);
            var s2z_list = WCFClients.ChartClient.QueryCache_CQSSC_2X_ZuXZS_Info(totalPageSize);
            var s2zx_list = WCFClients.ChartClient.QueryCache_CQSSC_2X_ZXZS_Info(totalPageSize);
            var s3c_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_C3YS_Info(totalPageSize);
            var s3d_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_DXZS_Info(totalPageSize);
            var s3h_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_HZZS_Info(totalPageSize);
            var s3j_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_JOZS_Info(totalPageSize);
            var s3kd_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_KD_Info(totalPageSize);
            var s3zh_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZHZS_Info(totalPageSize);
            var s3zu_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZuXZS_Info(totalPageSize);
            var s3zx_list = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZXZS_Info(totalPageSize);
            //var list = (from s in s3zx_list
            //            orderby s.CreateTime
            //            select s).ToList();
            //var s3zx_list_new = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZXZS_Info_New(totalPageSize, "up");
            var s5h_list = WCFClients.ChartClient.QueryCache_CQSSC_5X_HZZS_Info(totalPageSize);
            var s5j_list = WCFClients.ChartClient.QueryCache_CQSSC_5X_JBZS_Info(totalPageSize);
            foreach (var item in arr)
            {

                ViewBag.PageSize = item;
                #region dxds
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = dxds_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dxds{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/dxds", filePath);

                //dxds_list_new
                var dxds_list_new = (from s in dxds_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = dxds_list_new;
                filePath = Path.Combine(path, string.Format("dxds{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/dxds", filePath);
                #endregion

                #region slzs
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = slzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s1zs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s1zs", filePath);

                //slzs_list_new
                var slzs_list_new = (from s in slzs_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = slzs_list_new;
                filePath = Path.Combine(path, string.Format("s1zs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s1zs", filePath);
                #endregion

                #region s2h
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s2h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2hzzs", filePath);

                //s2h_list_new
                var s2h_list_new = (from s in s2h_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s2h_list_new;
                filePath = Path.Combine(path, string.Format("s2hzzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2hzzs", filePath);
                #endregion

                #region s2z
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s2z_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2zuxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2zuxzs", filePath);

                //s2z_list_new
                var s2z_list_new = (from s in s2z_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s2z_list_new;
                filePath = Path.Combine(path, string.Format("s2zuxzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2zuxzs", filePath);
                #endregion

                #region s2zx
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s2zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2zxzs", filePath);

                //s2zx_list_new
                var s2zx_list_new = (from s in s2zx_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s2zx_list_new;
                filePath = Path.Combine(path, string.Format("s2zxzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s2zxzs", filePath);
                #endregion

                #region s3c
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3c_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3c3ys{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3c3ys", filePath);

                //s3c_list_new
                var s3c_list_new = (from s in s3c_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3c_list_new;
                filePath = Path.Combine(path, string.Format("s3c3ys{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3c3ys", filePath);
                #endregion

                #region s3d
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3d_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3dxzs", filePath);

                //s3d_list_new
                var s3d_list_new = (from s in s3d_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3d_list_new;
                filePath = Path.Combine(path, string.Format("s3dxzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3dxzs", filePath);
                #endregion

                #region s3h
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3hzzs", filePath);

                //s3h_list_new
                var s3h_list_new = (from s in s3h_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3h_list_new;
                filePath = Path.Combine(path, string.Format("s3hzzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3hzzs", filePath);
                #endregion

                #region s3j
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3j_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3jozs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3jozs", filePath);

                //s3j_list_new
                var s3j_list_new = (from s in s3j_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3j_list_new;
                filePath = Path.Combine(path, string.Format("s3jozs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3jozs", filePath);
                #endregion

                #region s3kd
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3kd_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3kd{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3kd", filePath);

                //s3kd_list_new
                var s3kd_list_new = (from s in s3kd_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3kd_list_new;
                filePath = Path.Combine(path, string.Format("s3kd{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3kd", filePath);
                #endregion

                #region s3zh
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zhzs", filePath);

                //s3zh_list_new
                var s3zh_list_new = (from s in s3zh_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3zh_list_new;
                filePath = Path.Combine(path, string.Format("s3zhzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zhzs", filePath);
                #endregion

                #region s3zu
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3zu_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zuxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zuxzs", filePath);

                //s3zu_list_new
                var s3zu_list_new = (from s in s3zu_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3zu_list_new;
                filePath = Path.Combine(path, string.Format("s3zuxzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zuxzs", filePath);
                #endregion

                #region s3zx
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s3zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zxzs", filePath);

                //s3zx_list_new
                var s3zx_list_new = (from s in s3zx_list.Take(item).ToList()
                                     orderby s.CreateTime
                                     select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s3zx_list_new;
                filePath = Path.Combine(path, string.Format("s3zxzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s3zxzs", filePath);
                #endregion

                #region s5h
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s5h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s5hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s5hzzs", filePath);

                //s5h_list_new
                var s5h_list_new = (from s in s5h_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s5h_list_new;
                filePath = Path.Combine(path, string.Format("s5hzzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s5hzzs", filePath);
                #endregion

                #region s5j
                ViewBag.PhasseOrder = "down";
                ViewBag.DataList = s5j_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s5jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s5jbzs", filePath);


                //s5j_list_new
                var s5j_list_new = (from s in s5j_list.Take(item).ToList()
                                    orderby s.CreateTime
                                    select s).ToList();
                ViewBag.PhasseOrder = "up";
                ViewBag.DataList = s5j_list_new;
                filePath = Path.Combine(path, string.Format("s5jbzs{0}_up.html", item));
                BuildViewHtml("lotterytrend/CQSSC/s5jbzs", filePath);
                #endregion
            }
        }

        /// <summary>
        /// 生成江西时时彩走势图
        /// </summary>
        private void BuildLotteryTrend_JXSSC()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "JXSSC");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dxds_list = WCFClients.ChartClient.QueryCache_JXSSC_DXDS_Info(totalPageSize);
            var slzs_list = WCFClients.ChartClient.QueryCache_JXSSC_1X_ZS_Info(totalPageSize);
            var s2h_list = WCFClients.ChartClient.QueryCache_JXSSC_2X_HZZS_Info(totalPageSize);
            var s2z_list = WCFClients.ChartClient.QueryCache_JXSSC_2X_ZuXZS_Info(totalPageSize);
            var s2zx_list = WCFClients.ChartClient.QueryCache_JXSSC_2X_ZXZS_Info(totalPageSize);
            var s3c_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_C3YS_Info(totalPageSize);
            var s3d_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_DXZS_Info(totalPageSize);
            var s3h_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_HZZS_Info(totalPageSize);
            var s3j_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_JOZS_Info(totalPageSize);
            var s3kd_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_KD_Info(totalPageSize);
            var s3zh_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZHZS_Info(totalPageSize);
            var s3zu_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZuXZS_Info(totalPageSize);
            var s3zx_list = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZXZS_Info(totalPageSize);
            var s5h_list = WCFClients.ChartClient.QueryCache_JXSSC_5X_HZZS_Info(totalPageSize);
            var s5j_list = WCFClients.ChartClient.QueryCache_JXSSC_5X_JBZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = dxds_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dxds{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/dxds", filePath);

                ViewBag.DataList = slzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s1zs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s1zs", filePath);

                ViewBag.DataList = s2h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s2hzzs", filePath);

                ViewBag.DataList = s2z_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2zuxzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s2zuxzs", filePath);

                ViewBag.DataList = s2zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s2zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s2zxzs", filePath);

                ViewBag.DataList = s3c_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3c3ys{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3c3ys", filePath);

                ViewBag.DataList = s3d_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3dxzs", filePath);

                ViewBag.DataList = s3h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3hzzs", filePath);

                ViewBag.DataList = s3j_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3jozs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3jozs", filePath);

                ViewBag.DataList = s3kd_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3kd{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3kd", filePath);

                ViewBag.DataList = s3zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zhzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3zhzs", filePath);

                ViewBag.DataList = s3zu_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zuxzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3zuxzs", filePath);

                ViewBag.DataList = s3zx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s3zxzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s3zxzs", filePath);

                ViewBag.DataList = s5h_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s5hzzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s5hzzs", filePath);

                ViewBag.DataList = s5j_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("s5jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/JXSSC/s5jbzs", filePath);
            }
        }

        /// <summary>
        /// 生成广东快乐十分走势图
        /// </summary>
        private void BuildLotteryTrend_GDKLSF()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "GDKLSF");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dw1_list = WCFClients.ChartClient.QueryGDKLSF_DW1(totalPageSize); ;
            var dw2_list = WCFClients.ChartClient.QueryGDKLSF_DW2(totalPageSize);
            var dw3_list = WCFClients.ChartClient.QueryGDKLSF_DW3(totalPageSize);
            var dx_list = WCFClients.ChartClient.QueryGDKLSF_DX(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryGDKLSF_JBZS(totalPageSize);
            var jo_list = WCFClients.ChartClient.QueryGDKLSF_JO(totalPageSize);
            var zh_list = WCFClients.ChartClient.QueryGDKLSF_ZH(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = dw1_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dw1{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/dw1", filePath);

                ViewBag.DataList = dw2_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dw2{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/dw2", filePath);

                ViewBag.DataList = dw3_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dw3{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/dw3", filePath);

                ViewBag.DataList = dx_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("dx{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/dx", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/jbzs", filePath);

                ViewBag.DataList = jo_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jo{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/jo", filePath);

                ViewBag.DataList = zh_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("zh{0}.html", item));
                BuildViewHtml("lotterytrend/GDKLSF/zh", filePath);
            }
        }

        /// <summary>
        /// 生成湖南快乐十分走势图
        /// </summary>
        private void BuildLotteryTrend_HNKLSF()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "HNKLSF");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dxzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_DXZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_GHZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_JBZS_Info(totalPageSize);
            var jozs_list = WCFClients.ChartClient.QueryCache_HNKLSF_JOZS_Info(totalPageSize);
            var q1zs_list = WCFClients.ChartClient.QueryCache_HNKLSF_Q1ZS_Info(totalPageSize);
            var q3zs_list = WCFClients.ChartClient.QueryCache_HNKLSF_Q3ZS_Info(totalPageSize);
            var qjzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_QJZS_Info(totalPageSize);
            var slzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_3LZS_Info(totalPageSize);
            var twzs_list = WCFClients.ChartClient.QueryCache_HNKLSF_TWZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = dxzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/dxzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/ghzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/jbzs", filePath);

                ViewBag.DataList = jozs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jozs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/jozs", filePath);

                ViewBag.DataList = q1zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1zs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/q1zs", filePath);

                ViewBag.DataList = q3zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/q3zs", filePath);

                ViewBag.DataList = qjzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("qjzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/qjzs", filePath);

                ViewBag.DataList = slzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("slzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/slzs", filePath);

                ViewBag.DataList = twzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("twzs{0}.html", item));
                BuildViewHtml("lotterytrend/HNKLSF/twzs", filePath);
            }
        }

        /// <summary>
        /// 生成重庆快乐十分走势图
        /// </summary>
        private void BuildLotteryTrend_CQKLSF()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "lotterytrend", "CQKLSF");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var arr = new int[] { 30, 50, 100 };
            var totalPageSize = 100;
            var dxzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_DXZS_Info(totalPageSize);
            var elzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_2LZS_Info(totalPageSize);
            var ghzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_GHZS_Info(totalPageSize);
            var jbzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_JBZS_Info(totalPageSize);
            var jozs_list = WCFClients.ChartClient.QueryCache_CQKLSF_JOZS_Info(totalPageSize);
            var q1zs_list = WCFClients.ChartClient.QueryCache_CQKLSF_Q1ZS_Info(totalPageSize);
            var q3zs_list = WCFClients.ChartClient.QueryCache_CQKLSF_Q3ZS_Info(totalPageSize);
            var qjzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_QJZS_Info(totalPageSize);
            var slzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_3LZS_Info(totalPageSize);
            var twzs_list = WCFClients.ChartClient.QueryCache_CQKLSF_TWZS_Info(totalPageSize);
            foreach (var item in arr)
            {
                ViewBag.DataList = dxzs_list.Take(item).ToList();
                var filePath = Path.Combine(path, string.Format("dxzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/dxzs", filePath);

                ViewBag.DataList = elzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("elzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/elzs", filePath);

                ViewBag.DataList = ghzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("ghzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/ghzs", filePath);

                ViewBag.DataList = jbzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jbzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/jbzs", filePath);

                ViewBag.DataList = jozs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("jozs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/jozs", filePath);

                ViewBag.DataList = q1zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q1zs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/q1zs", filePath);

                ViewBag.DataList = q3zs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("q3zs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/q3zs", filePath);

                ViewBag.DataList = qjzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("qjzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/qjzs", filePath);

                ViewBag.DataList = slzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("slzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/slzs", filePath);

                ViewBag.DataList = twzs_list.Take(item).ToList();
                filePath = Path.Combine(path, string.Format("twzs{0}.html", item));
                BuildViewHtml("lotterytrend/CQKLSF/twzs", filePath);
            }
        }

        /// <summary>
        /// 生成购彩大厅
        /// </summary>
        private void BuildBuyHall()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "buy");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("buy/goucaihall", filePath);
        }

        /// <summary>
        /// 生成个人博客主页
        /// </summary>
        private string BuildBlog(string userId)
        {
            var log = new List<string>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "blog", userId);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var pageIndex = 0;
            var pageSize = 1000;

            var userInfo = WCFClients.GameClient.QueryProfileUserInfo(userId);
            var bonuslevel = WCFClients.GameClient.QueryProfileBonusLevelInfo(userId);
            var datareport = WCFClients.GameClient.QueryProfileDataReport(userId);
            var bonusListZj = WCFClients.GameClient.QueryProfileLastBonusCollection(userId);
            var count = WCFClients.GameQueryClient.QueryTogetherFollowerCount(userId);
            var billlist = WCFClients.GameClient.QueryUserBeedingList("", "", userId, string.Empty, 0, 100, QueryUserBeedingListOrderByProperty.TotalBonusMoney, OrderByCategory.DESC, UserToken);
            var totalBonusList = WCFClients.GameQueryClient.QueryBonusInfoList(userId, "", "", "", "", "", pageIndex, pageSize, UserToken);
            var totalCurrentOrder = WCFClients.GameClient.QueryUserCurrentOrderList(userId, "", UserToken, pageIndex, pageSize);


            var gameCodeArray = new string[] { "SZC", "CTZQ", "BJDC", "JCZQ", "JCLQ" };
            for (var i = 0; i < gameCodeArray.Length; i++)
            {
                var gameCode = gameCodeArray[i];
                var gameTypeArray = new string[] { };
                switch (gameCode)
                {
                    case "SZC":
                        gameTypeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
                        break;
                    case "CTZQ":
                        gameTypeArray = new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
                        break;
                    case "JCZQ":
                        gameTypeArray = new string[] { "SPF", "BRQSPF", "ZJQ", "BF", "BQC", "HH" };
                        break;
                    case "JCLQ":
                        gameTypeArray = new string[] { "SF", "RFSF", "SFC", "DXF", "HH" };
                        break;
                    case "BJDC":
                        gameTypeArray = new string[] { "SPF", "ZJQ", "BF", "BQC", "SXDS" };
                        break;
                    default:
                        break;
                }

                foreach (var gameType in gameTypeArray)
                {
                    var bonusList = new List<BonusOrderInfo>();
                    var currentOrder = new List<UserCurrentOrderInfo>();//合买
                    try
                    {
                        var currentPageSize = 30;
                        bonusList = totalBonusList.BonusOrderList.Where(p => p.GameCode == ((gameCode == "SZC" ? gameType : gameCode)) && p.GameType == ((gameCode == "SZC" ? "" : gameType))).Take(currentPageSize).ToList();
                        currentOrder = totalCurrentOrder.List.Where(p => p.GameCode == (gameCode == "SZC" ? gameType : gameCode)).Take(currentPageSize).ToList();
                    }
                    catch (Exception ex)
                    {
                        log.Add(string.Format("查询{0} {1} {2}异常：{3}", userId, gameCode, gameType, ex.Message));
                    }
                    try
                    {
                        ViewBag.BonusList = bonusList;
                        ViewBag.UserInfo = userInfo;
                        ViewBag.CurrentUser = CurrentUser;//
                        ViewBag.CurrentOrder = currentOrder;
                        ViewBag.Count = count;
                        ViewBag.BonusLevel = bonuslevel;
                        ViewBag.BonusListZj = bonusListZj;
                        ViewBag.DataReport = datareport;
                        ViewBag.UserId = userId;
                        ViewBag.GameCode = gameCode;
                        ViewBag.GameType = gameType;
                        ViewBag.BillList = billlist;
                        try
                        {
                            var filePath1 = Path.Combine(path, string.Format("custombill_{0}.html", userId));
                            BuildViewHtml("blog/custombill", filePath1);
                        }
                        catch (Exception ex)
                        {
                            log.Add(string.Format("生成{0}Custombill异常：{1}", userId, ex.Message));
                        }
                        if (i == 0 && gameType == "SSQ")
                        {
                            var filePathindex = Path.Combine(path, string.Format("index_{0}.html", userId));
                            BuildViewHtml("blog/standings", filePathindex);
                        }
                        else
                        {
                            var filePath = Path.Combine(path, string.Format("index_{0}_{1}_{2}.html", userId, gameCode, gameType));
                            BuildViewHtml("blog/standings", filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add(string.Format("生成{0} {1} {2}异常：{3}", userId, gameCode, gameType, ex.Message));
                    }
                }
            }
            return string.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        /// 生成用户注册页面
        /// </summary>
        private void BuildUserRegister()
        {
            ViewBag.VerifyCode = CommonAPI.QueryCoreConfigByKey();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "register.html");
            BuildViewHtml("register", filePath);
        }

        /// <summary>
        /// 生成会员中心页面
        /// </summary>
        private void BuildVip()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "vip");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("vip/supervip", filePath);

            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            filePath = Path.Combine(path, "privilege.html");
            BuildViewHtml("vip/privilege", filePath);

            filePath = Path.Combine(path, "introduction.html");
            BuildViewHtml("vip/introduction", filePath);

            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            filePath = Path.Combine(path, "ledou.html");
            BuildViewHtml("vip/ledou", filePath);
        }

        /// <summary>
        /// 生成手机购彩首页
        /// </summary>
        private void BuildPhoneBuy()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "phonebuy");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("phonebuy/phonebuy", filePath);
        }

        /// <summary>
        /// 生成活动首页
        /// </summary>
        private void BuildHuoDong()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "huodong");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("huodong/avtioncentent", filePath);
        }

        /// <summary>
        /// 生成帮助中心首页及子页页面
        /// </summary>
        private void BuildHelp()
        {
            # region 生成首页

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "index.html");
            BuildViewHtml("help/Index", filePath);

            #endregion

            #region 生成子页

            #region 注册页
            filePath = Path.Combine(path, "login.html");
            BuildViewHtml("help/login", filePath);

            filePath = Path.Combine(path, "notes.html");
            BuildViewHtml("help/notes", filePath);

            filePath = Path.Combine(path, "isokregister.html");
            BuildViewHtml("help/isokregister", filePath);

            filePath = Path.Combine(path, "validationMail.html");
            BuildViewHtml("help/validationMail", filePath);

            filePath = Path.Combine(path, "validationPhone.html");
            BuildViewHtml("help/validationPhone", filePath);
            #endregion

            #region 充值页
            var path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "chongzhi");
            if (!Directory.Exists(path1))
                Directory.CreateDirectory(path1);
            filePath = Path.Combine(path1, "payintroduce.html");
            BuildViewHtml("help/chongzhi/payintroduce", filePath);

            filePath = Path.Combine(path1, "EbanksPay.html");
            BuildViewHtml("help/chongzhi/EbanksPay", filePath);

            filePath = Path.Combine(path1, "Alipay.html");
            BuildViewHtml("help/chongzhi/Alipay", filePath);

            filePath = Path.Combine(path1, "weixin.html");
            BuildViewHtml("help/chongzhi/weixin", filePath);

            filePath = Path.Combine(path1, "HWAlipay.html");
            BuildViewHtml("help/chongzhi/HWAlipay", filePath);

            filePath = Path.Combine(path1, "Quickpayment.html");
            BuildViewHtml("help/chongzhi/Quickpayment", filePath);

            filePath = Path.Combine(path1, "payway.html");
            BuildViewHtml("help/chongzhi/payway", filePath);

            filePath = Path.Combine(path1, "payAccount.html");
            BuildViewHtml("help/chongzhi/payAccount", filePath);

            filePath = Path.Combine(path1, "payNotAccount.html");
            BuildViewHtml("help/chongzhi/payNotAccount", filePath);

            filePath = Path.Combine(path1, "paypoundage.html");
            BuildViewHtml("help/chongzhi/paypoundage", filePath);

            filePath = Path.Combine(path1, "EbanksNotAccount.html");
            BuildViewHtml("help/chongzhi/EbanksNotAccount", filePath);

            filePath = Path.Combine(path1, "EbanksDebugError.html");
            BuildViewHtml("help/chongzhi/EbanksDebugError", filePath);

            filePath = Path.Combine(path1, "EbanksSafetyLCE.html");
            BuildViewHtml("help/chongzhi/EbanksSafetyLCE", filePath);

            filePath = Path.Combine(path1, "EbanksDredge.html");
            BuildViewHtml("help/chongzhi/EbanksDredge", filePath);

            filePath = Path.Combine(path1, "EbanksOnlinePay.html");
            BuildViewHtml("help/chongzhi/EbanksOnlinePay", filePath);

            filePath = Path.Combine(path1, "EbanksNotReaction.html");
            BuildViewHtml("help/chongzhi/EbanksNotReaction", filePath);

            filePath = Path.Combine(path1, "EbanksTradePage.html");
            BuildViewHtml("help/chongzhi/EbanksTradePage", filePath);

            filePath = Path.Combine(path1, "EbanksWebpageError.html");
            BuildViewHtml("help/chongzhi/EbanksWebpageError", filePath);

            filePath = Path.Combine(path1, "AlipayPay.html");
            BuildViewHtml("help/chongzhi/AlipayPay", filePath);

            filePath = Path.Combine(path1, "AlipaySelfHelp.html");
            BuildViewHtml("help/chongzhi/AlipaySelfHelp", filePath);

            filePath = Path.Combine(path1, "QuickpaymentPay.html");
            BuildViewHtml("help/chongzhi/QuickpaymentPay", filePath);
            #endregion

            #region 我的账户页
            var path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "myAccount");
            if (!Directory.Exists(path2))
                Directory.CreateDirectory(path2);
            filePath = Path.Combine(path2, "Approve.html");
            BuildViewHtml("help/myAccount/Approve", filePath);

            filePath = Path.Combine(path2, "Attention.html");
            BuildViewHtml("help/myAccount/Attention", filePath);

            filePath = Path.Combine(path2, "information.html");
            BuildViewHtml("help/myAccount/information", filePath);

            filePath = Path.Combine(path2, "NotApprove.html");
            BuildViewHtml("help/myAccount/NotApprove", filePath);

            filePath = Path.Combine(path2, "whatApprove.html");
            BuildViewHtml("help/myAccount/whatApprove", filePath);

            filePath = Path.Combine(path2, "Administrator.html");
            BuildViewHtml("help/myAccount/Administrator", filePath);

            filePath = Path.Combine(path2, "password.html");
            BuildViewHtml("help/myAccount/password", filePath);

            filePath = Path.Combine(path2, "username.html");
            BuildViewHtml("help/myAccount/username", filePath);

            filePath = Path.Combine(path2, "verificationcode.html");
            BuildViewHtml("help/myAccount/verificationcode", filePath);

            filePath = Path.Combine(path2, "verifyImage.html");
            BuildViewHtml("help/myAccount/verifyImage", filePath);

            filePath = Path.Combine(path2, "AccountSecurity.html");
            BuildViewHtml("help/myAccount/AccountSecurity", filePath);

            filePath = Path.Combine(path2, "safety.html");
            BuildViewHtml("help/myAccount/safety", filePath);

            filePath = Path.Combine(path2, "bindingBank.html");
            BuildViewHtml("help/myAccount/bindingBank", filePath);

            filePath = Path.Combine(path2, "bindingBankNotPhone.html");
            BuildViewHtml("help/myAccount/bindingBankNotPhone", filePath);

            filePath = Path.Combine(path2, "bindingPhone.html");
            BuildViewHtml("help/myAccount/bindingPhone", filePath);

            filePath = Path.Combine(path2, "astrict.html");
            BuildViewHtml("help/myAccount/astrict", filePath);

            filePath = Path.Combine(path2, "Email.html");
            BuildViewHtml("help/myAccount/Email", filePath);

            filePath = Path.Combine(path2, "NotbindingPhone.html");
            BuildViewHtml("help/myAccount/NotbindingPhone", filePath);

            filePath = Path.Combine(path2, "Ebank.html");
            BuildViewHtml("help/myAccount/Ebank", filePath);

            filePath = Path.Combine(path2, "EbankABC.html");
            BuildViewHtml("help/myAccount/EbankABC", filePath);

            filePath = Path.Combine(path2, "EbankAlipay.html");
            BuildViewHtml("help/myAccount/EbankAlipay", filePath);

            filePath = Path.Combine(path2, "EbankBOC.html");
            BuildViewHtml("help/myAccount/EbankBOC", filePath);

            filePath = Path.Combine(path2, "EbankBOCOM.html");
            BuildViewHtml("help/myAccount/EbankBOCOM", filePath);

            filePath = Path.Combine(path2, "EbankCCB.html");
            BuildViewHtml("help/myAccount/EbankCCB", filePath);

            filePath = Path.Combine(path2, "EbankCMB.html");
            BuildViewHtml("help/myAccount/EbankCMB", filePath);

            filePath = Path.Combine(path2, "EbankCMBC.html");
            BuildViewHtml("help/myAccount/EbankCMBC", filePath);

            filePath = Path.Combine(path2, "EbankICBC.html");
            BuildViewHtml("help/myAccount/EbankICBC", filePath);

            filePath = Path.Combine(path2, "privacySet.html");
            BuildViewHtml("help/myAccount/privacySet", filePath);

            filePath = Path.Combine(path2, "privacySet1.html");
            BuildViewHtml("help/myAccount/privacySet1", filePath);

            filePath = Path.Combine(path2, "privacySet2.html");
            BuildViewHtml("help/myAccount/privacySet2", filePath);

            filePath = Path.Combine(path2, "privacySet3.html");
            BuildViewHtml("help/myAccount/privacySet3", filePath);

            filePath = Path.Combine(path2, "privacySet4.html");
            BuildViewHtml("help/myAccount/privacySet4", filePath);

            filePath = Path.Combine(path2, "Redpackets.html");
            BuildViewHtml("help/myAccount/Redpackets", filePath);

            filePath = Path.Combine(path2, "Redpackets1.html");
            BuildViewHtml("help/myAccount/Redpackets1", filePath);

            filePath = Path.Combine(path2, "Redpackets2.html");
            BuildViewHtml("help/myAccount/Redpackets2", filePath);

            filePath = Path.Combine(path2, "Redpackets3.html");
            BuildViewHtml("help/myAccount/Redpackets3", filePath);

            filePath = Path.Combine(path2, "Redpackets4.html");
            BuildViewHtml("help/myAccount/Redpackets4", filePath);

            filePath = Path.Combine(path2, "Redpackets5.html");
            BuildViewHtml("help/myAccount/Redpackets5", filePath);

            filePath = Path.Combine(path2, "Redpackets6.html");
            BuildViewHtml("help/myAccount/Redpackets6", filePath);
            #endregion

            #region 购买彩票页
            var path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "goucai");
            if (!Directory.Exists(path3))
                Directory.CreateDirectory(path3);
            filePath = Path.Combine(path3, "ForTheColor.html");
            BuildViewHtml("help/goucai/ForTheColor", filePath);

            filePath = Path.Combine(path3, "ForTheColor1.html");
            BuildViewHtml("help/goucai/ForTheColor1", filePath);

            filePath = Path.Combine(path3, "ForTheColor2.html");
            BuildViewHtml("help/goucai/ForTheColor2", filePath);

            filePath = Path.Combine(path3, "ForTheColor3.html");
            BuildViewHtml("help/goucai/ForTheColor3", filePath);

            filePath = Path.Combine(path3, "ForTheColor4.html");
            BuildViewHtml("help/goucai/ForTheColor4", filePath);

            filePath = Path.Combine(path3, "ForTheColor5.html");
            BuildViewHtml("help/goucai/ForTheColor5", filePath);

            filePath = Path.Combine(path3, "ForTheColor6.html");
            BuildViewHtml("help/goucai/ForTheColor6", filePath);

            filePath = Path.Combine(path3, "ForTheColor7.html");
            BuildViewHtml("help/goucai/ForTheColor7", filePath);

            filePath = Path.Combine(path3, "ForTheColor8.html");
            BuildViewHtml("help/goucai/ForTheColor8", filePath);

            filePath = Path.Combine(path3, "ForTheColor9.html");
            BuildViewHtml("help/goucai/ForTheColor9", filePath);

            filePath = Path.Combine(path3, "ForTheColor10.html");
            BuildViewHtml("help/goucai/ForTheColor10", filePath);

            filePath = Path.Combine(path3, "ForTheColor11.html");
            BuildViewHtml("help/goucai/ForTheColor11", filePath);

            filePath = Path.Combine(path3, "ForTheColor12.html");
            BuildViewHtml("help/goucai/ForTheColor12", filePath);

            filePath = Path.Combine(path3, "ForTheColor13.html");
            BuildViewHtml("help/goucai/ForTheColor13", filePath);

            filePath = Path.Combine(path3, "ForTheColor14.html");
            BuildViewHtml("help/goucai/ForTheColor14", filePath);

            filePath = Path.Combine(path3, "ShoppingService.html");
            BuildViewHtml("help/goucai/ShoppingService", filePath);

            filePath = Path.Combine(path3, "ShoppingService1.html");
            BuildViewHtml("help/goucai/ShoppingService1", filePath);

            filePath = Path.Combine(path3, "ShoppingService2.html");
            BuildViewHtml("help/goucai/ShoppingService2", filePath);

            filePath = Path.Combine(path3, "ShoppingService3.html");
            BuildViewHtml("help/goucai/ShoppingService3", filePath);

            filePath = Path.Combine(path3, "ShoppingService4.html");
            BuildViewHtml("help/goucai/ShoppingService4", filePath);

            filePath = Path.Combine(path3, "ShoppingService5.html");
            BuildViewHtml("help/goucai/ShoppingService5", filePath);

            filePath = Path.Combine(path3, "ShoppingService6.html");
            BuildViewHtml("help/goucai/ShoppingService6", filePath);

            filePath = Path.Combine(path3, "ShoppingService7.html");
            BuildViewHtml("help/goucai/ShoppingService7", filePath);

            filePath = Path.Combine(path3, "ShoppingService8.html");
            BuildViewHtml("help/goucai/ShoppingService8", filePath);

            filePath = Path.Combine(path3, "ShoppingService9.html");
            BuildViewHtml("help/goucai/ShoppingService9", filePath);

            filePath = Path.Combine(path3, "ShoppingService10.html");
            BuildViewHtml("help/goucai/ShoppingService10", filePath);

            filePath = Path.Combine(path3, "ShoppingService11.html");
            BuildViewHtml("help/goucai/ShoppingService11", filePath);

            filePath = Path.Combine(path3, "ShoppingService12.html");
            BuildViewHtml("help/goucai/ShoppingService12", filePath);

            filePath = Path.Combine(path3, "ShoppingService13.html");
            BuildViewHtml("help/goucai/ShoppingService13", filePath);

            filePath = Path.Combine(path3, "ShoppingService14.html");
            BuildViewHtml("help/goucai/ShoppingService14", filePath);

            filePath = Path.Combine(path3, "ShoppingService15.html");
            BuildViewHtml("help/goucai/ShoppingService15", filePath);

            filePath = Path.Combine(path3, "ShoppingService16.html");
            BuildViewHtml("help/goucai/ShoppingService16", filePath);

            filePath = Path.Combine(path3, "ShoppingService17.html");
            BuildViewHtml("help/goucai/ShoppingService17", filePath);

            filePath = Path.Combine(path3, "ShoppingService18.html");
            BuildViewHtml("help/goucai/ShoppingService18", filePath);

            filePath = Path.Combine(path3, "ShoppingService19.html");
            BuildViewHtml("help/goucai/ShoppingService19", filePath);

            filePath = Path.Combine(path3, "ShoppingService20.html");
            BuildViewHtml("help/goucai/ShoppingService20", filePath);

            filePath = Path.Combine(path3, "ShoppingService21.html");
            BuildViewHtml("help/goucai/ShoppingService21", filePath);

            filePath = Path.Combine(path3, "animationDocumentary.html");
            BuildViewHtml("help/goucai/animationDocumentary", filePath);

            filePath = Path.Combine(path3, "animationDocumentary1.html");
            BuildViewHtml("help/goucai/animationDocumentary1", filePath);

            filePath = Path.Combine(path3, "animationDocumentary2.html");
            BuildViewHtml("help/goucai/animationDocumentary2", filePath);

            filePath = Path.Combine(path3, "animationDocumentary3.html");
            BuildViewHtml("help/goucai/animationDocumentary3", filePath);

            filePath = Path.Combine(path3, "animationDocumentary4.html");
            BuildViewHtml("help/goucai/animationDocumentary4", filePath);

            filePath = Path.Combine(path3, "animationDocumentary5.html");
            BuildViewHtml("help/goucai/animationDocumentary5", filePath);

            filePath = Path.Combine(path3, "Revoke.html");
            BuildViewHtml("help/goucai/Revoke", filePath);

            filePath = Path.Combine(path3, "Revoke1.html");
            BuildViewHtml("help/goucai/Revoke1", filePath);

            filePath = Path.Combine(path3, "Revoke2.html");
            BuildViewHtml("help/goucai/Revoke2", filePath);

            filePath = Path.Combine(path3, "Revoke3.html");
            BuildViewHtml("help/goucai/Revoke3", filePath);

            filePath = Path.Combine(path3, "Revoke4.html");
            BuildViewHtml("help/goucai/Revoke4", filePath);

            filePath = Path.Combine(path3, "Revoke5.html");
            BuildViewHtml("help/goucai/Revoke5", filePath);

            filePath = Path.Combine(path3, "dsgs.html");
            BuildViewHtml("help/goucai/dsgs", filePath);

            filePath = Path.Combine(path3, "dsgs1.html");
            BuildViewHtml("help/goucai/dsgs1", filePath);
            #endregion

            #region 兑奖提款页
            var path4 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "drawMoney");
            if (!Directory.Exists(path4))
                Directory.CreateDirectory(path4);
            filePath = Path.Combine(path4, "bonusGet.html");
            BuildViewHtml("help/drawMoney/bonusGet", filePath);

            filePath = Path.Combine(path4, "bonusGet1.html");
            BuildViewHtml("help/drawMoney/bonusGet1", filePath);

            filePath = Path.Combine(path4, "bonusGet2.html");
            BuildViewHtml("help/drawMoney/bonusGet2", filePath);

            filePath = Path.Combine(path4, "bonusGet3.html");
            BuildViewHtml("help/drawMoney/bonusGet3", filePath);

            filePath = Path.Combine(path4, "bonusGet4.html");
            BuildViewHtml("help/drawMoney/bonusGet4", filePath);

            filePath = Path.Combine(path4, "bonusGet5.html");
            BuildViewHtml("help/drawMoney/bonusGet5", filePath);

            filePath = Path.Combine(path4, "bonusGet6.html");
            BuildViewHtml("help/drawMoney/bonusGet6", filePath);

            filePath = Path.Combine(path4, "drawing.html");
            BuildViewHtml("help/drawMoney/drawing", filePath);

            filePath = Path.Combine(path4, "drawing1.html");
            BuildViewHtml("help/drawMoney/drawing1", filePath);

            filePath = Path.Combine(path4, "drawing2.html");
            BuildViewHtml("help/drawMoney/drawing2", filePath);

            filePath = Path.Combine(path4, "drawing3.html");
            BuildViewHtml("help/drawMoney/drawing3", filePath);

            filePath = Path.Combine(path4, "drawing4.html");
            BuildViewHtml("help/drawMoney/drawing4", filePath);

            filePath = Path.Combine(path4, "drawing5.html");
            BuildViewHtml("help/drawMoney/drawing5", filePath);

            filePath = Path.Combine(path4, "drawing6.html");
            BuildViewHtml("help/drawMoney/drawing6", filePath);

            filePath = Path.Combine(path4, "poundage.html");
            BuildViewHtml("help/drawMoney/poundage", filePath);

            filePath = Path.Combine(path4, "poundage1.html");
            BuildViewHtml("help/drawMoney/poundage1", filePath);

            filePath = Path.Combine(path4, "poundageTime.html");
            BuildViewHtml("help/drawMoney/poundageTime", filePath);

            filePath = Path.Combine(path4, "poundageTime1.html");
            BuildViewHtml("help/drawMoney/poundageTime1", filePath);
            #endregion

            #region 交易规则页
            var path5 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "tradeRules");
            if (!Directory.Exists(path5))
                Directory.CreateDirectory(path5);
            filePath = Path.Combine(path5, "bonusAllot.html");
            BuildViewHtml("help/tradeRules/bonusAllot", filePath);

            filePath = Path.Combine(path5, "bonusAllot1.html");
            BuildViewHtml("help/tradeRules/bonusAllot1", filePath);

            filePath = Path.Combine(path5, "bonusAllot2.html");
            BuildViewHtml("help/tradeRules/bonusAllot2", filePath);

            filePath = Path.Combine(path5, "customizeDocumentary.html");
            BuildViewHtml("help/tradeRules/customizeDocumentary", filePath);

            filePath = Path.Combine(path5, "customizeDocumentary1.html");
            BuildViewHtml("help/tradeRules/customizeDocumentary1", filePath);

            filePath = Path.Combine(path5, "customizeDocumentary2.html");
            BuildViewHtml("help/tradeRules/customizeDocumentary2", filePath);

            filePath = Path.Combine(path5, "customizeDocumentary3.html");
            BuildViewHtml("help/tradeRules/customizeDocumentary3", filePath);

            filePath = Path.Combine(path5, "website.html");
            BuildViewHtml("help/tradeRules/website", filePath);

            filePath = Path.Combine(path5, "website1.html");
            BuildViewHtml("help/tradeRules/website1", filePath);

            filePath = Path.Combine(path5, "website2.html");
            BuildViewHtml("help/tradeRules/website2", filePath);

            filePath = Path.Combine(path5, "recordRule.html");
            BuildViewHtml("help/tradeRules/recordRule", filePath);

            filePath = Path.Combine(path5, "recordRule1.html");
            BuildViewHtml("help/tradeRules/recordRule1", filePath);

            filePath = Path.Combine(path5, "recordRule2.html");
            BuildViewHtml("help/tradeRules/recordRule2", filePath);
            #endregion

            #region 彩种玩法页
            var path6 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "playRules");
            if (!Directory.Exists(path6))
                Directory.CreateDirectory(path6);
            filePath = Path.Combine(path6, "bjdc.html");
            BuildViewHtml("help/playRules/bjdc", filePath);

            filePath = Path.Combine(path6, "bjdc1.html");
            BuildViewHtml("help/playRules/bjdc1", filePath);

            filePath = Path.Combine(path6, "cqssc.html");
            BuildViewHtml("help/playRules/cqssc", filePath);

            filePath = Path.Combine(path6, "cqssc1.html");
            BuildViewHtml("help/playRules/cqssc1", filePath);

            filePath = Path.Combine(path6, "dlt.html");
            BuildViewHtml("help/playRules/dlt", filePath);

            filePath = Path.Combine(path6, "dlt1.html");
            BuildViewHtml("help/playRules/dlt1", filePath);

            filePath = Path.Combine(path6, "fc3d.html");
            BuildViewHtml("help/playRules/fc3d", filePath);

            filePath = Path.Combine(path6, "fc3d1.html");
            BuildViewHtml("help/playRules/fc3d1", filePath);

            filePath = Path.Combine(path6, "jclq.html");
            BuildViewHtml("help/playRules/jclq", filePath);

            filePath = Path.Combine(path6, "jclq1.html");
            BuildViewHtml("help/playRules/jclq1", filePath);

            filePath = Path.Combine(path6, "jczq.html");
            BuildViewHtml("help/playRules/jczq", filePath);

            filePath = Path.Combine(path6, "jczq1.html");
            BuildViewHtml("help/playRules/jczq1", filePath);

            filePath = Path.Combine(path6, "jx11x5.html");
            BuildViewHtml("help/playRules/jx11x5", filePath);

            filePath = Path.Combine(path6, "jx11x51.html");
            BuildViewHtml("help/playRules/jx11x51", filePath);


            filePath = Path.Combine(path6, "sd11x5.html");
            BuildViewHtml("help/playRules/sd11x5", filePath);

            filePath = Path.Combine(path6, "sd11x51.html");
            BuildViewHtml("help/playRules/sd11x51", filePath);

            filePath = Path.Combine(path6, "gd11x5.html");
            BuildViewHtml("help/playRules/gd11x5", filePath);

            filePath = Path.Combine(path6, "gd11x51.html");
            BuildViewHtml("help/playRules/gd11x51", filePath);

            filePath = Path.Combine(path6, "gdklsf.html");
            BuildViewHtml("help/playRules/gdklsf", filePath);

            filePath = Path.Combine(path6, "gdklsf1.html");
            BuildViewHtml("help/playRules/gdklsf1", filePath);

            filePath = Path.Combine(path6, "jsks.html");
            BuildViewHtml("help/playRules/jsks", filePath);

            filePath = Path.Combine(path6, "jsks1.html");
            BuildViewHtml("help/playRules/jsks1", filePath);

            filePath = Path.Combine(path6, "sdklpk3.html");
            BuildViewHtml("help/playRules/sdklpk3", filePath);

            filePath = Path.Combine(path6, "sdklpk31.html");
            BuildViewHtml("help/playRules/sdklpk31", filePath);

            filePath = Path.Combine(path6, "pl3.html");
            BuildViewHtml("help/playRules/pl3", filePath);

            filePath = Path.Combine(path6, "pl31.html");
            BuildViewHtml("help/playRules/pl31", filePath);

            filePath = Path.Combine(path6, "ssq.html");
            BuildViewHtml("help/playRules/ssq", filePath);

            filePath = Path.Combine(path6, "ssq1.html");
            BuildViewHtml("help/playRules/ssq1", filePath);

            filePath = Path.Combine(path6, "zclcbq.html");
            BuildViewHtml("help/playRules/zclcbq", filePath);

            filePath = Path.Combine(path6, "zclcbq1.html");
            BuildViewHtml("help/playRules/zclcbq1", filePath);

            filePath = Path.Combine(path6, "zcscjq.html");
            BuildViewHtml("help/playRules/zcscjq", filePath);

            filePath = Path.Combine(path6, "zcscjq1.html");
            BuildViewHtml("help/playRules/zcscjq1", filePath);

            filePath = Path.Combine(path6, "zcsfc.html");
            BuildViewHtml("help/playRules/zcsfc", filePath);

            filePath = Path.Combine(path6, "zcsfc1.html");
            BuildViewHtml("help/playRules/zcsfc1", filePath);

            filePath = Path.Combine(path6, "tr9.html");
            BuildViewHtml("help/playRules/tr9", filePath);

            filePath = Path.Combine(path6, "tr91.html");
            BuildViewHtml("help/playRules/tr91", filePath);
            #endregion

            #region 手机购彩页
            var path7 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "phoneBuy");
            if (!Directory.Exists(path7))
                Directory.CreateDirectory(path7);
            filePath = Path.Combine(path7, "phoneForTheColor.html");
            BuildViewHtml("help/phoneBuy/phoneForTheColor", filePath);

            filePath = Path.Combine(path7, "phoneForTheColor1.html");
            BuildViewHtml("help/phoneBuy/phoneForTheColor1", filePath);

            filePath = Path.Combine(path7, "phoneForTheColor2.html");
            BuildViewHtml("help/phoneBuy/phoneForTheColor2", filePath);

            filePath = Path.Combine(path7, "phoneForTheColor3.html");
            BuildViewHtml("help/phoneBuy/phoneForTheColor3", filePath);

            filePath = Path.Combine(path7, "phoneForTheColor4.html");
            BuildViewHtml("help/phoneBuy/phoneForTheColor4", filePath);
            #endregion

            #region 安全保障页
            var path8 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "statichtml", "help", "safety");
            if (!Directory.Exists(path8))
                Directory.CreateDirectory(path8);
            filePath = Path.Combine(path8, "advantage.html");
            BuildViewHtml("help/safety/advantage", filePath);

            filePath = Path.Combine(path8, "advantage1.html");
            BuildViewHtml("help/safety/advantage1", filePath);

            filePath = Path.Combine(path8, "betNotice.html");
            BuildViewHtml("help/safety/betNotice", filePath);

            filePath = Path.Combine(path8, "betNotice1.html");
            BuildViewHtml("help/safety/betNotice1", filePath);

            filePath = Path.Combine(path8, "business.html");
            BuildViewHtml("help/safety/business", filePath);

            filePath = Path.Combine(path8, "business1.html");
            BuildViewHtml("help/safety/business1", filePath);

            filePath = Path.Combine(path8, "forTheColorSafety.html");
            BuildViewHtml("help/safety/forTheColorSafety", filePath);

            filePath = Path.Combine(path8, "forTheColorSafety1.html");
            BuildViewHtml("help/safety/forTheColorSafety1", filePath);

            filePath = Path.Combine(path8, "serviceAgreement.html");
            BuildViewHtml("help/safety/serviceAgreement", filePath);

            filePath = Path.Combine(path8, "serviceAgreement1.html");
            BuildViewHtml("help/safety/serviceAgreement1", filePath);

            filePath = Path.Combine(path8, "betpact.html");
            BuildViewHtml("help/safety/betpact", filePath);

            filePath = Path.Combine(path8, "betpact1.html");
            BuildViewHtml("help/safety/betpact1", filePath);
            #endregion

            #region 关于玩彩网页
            var path9 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "staticHtml", "help", "about");
            if (!Directory.Exists(path9))
                Directory.CreateDirectory(path9);
            filePath = Path.Combine(path9, "Aboutsite.html");
            BuildViewHtml("help/about/Aboutsite", filePath);

            filePath = Path.Combine(path9, "Aboutsite1.html");
            BuildViewHtml("help/about/Aboutsite1", filePath);

            filePath = Path.Combine(path9, "Join.html");
            BuildViewHtml("help/about/Join", filePath);

            filePath = Path.Combine(path9, "Join1.html");
            BuildViewHtml("help/about/Join1", filePath);

            filePath = Path.Combine(path9, "Join2.html");
            BuildViewHtml("help/about/Join2", filePath);

            filePath = Path.Combine(path9, "links.html");
            BuildViewHtml("help/about/links", filePath);

            filePath = Path.Combine(path9, "links1.html");
            BuildViewHtml("help/about/links1", filePath);

            filePath = Path.Combine(path9, "websiteMap.html");
            BuildViewHtml("help/about/websiteMap", filePath);

            filePath = Path.Combine(path9, "websiteMap1.html");
            BuildViewHtml("help/about/websiteMap1", filePath);

            filePath = Path.Combine(path9, "abtus.html");
            BuildViewHtml("help/about/abtus", filePath);

            filePath = Path.Combine(path9, "Recruitment.html");
            BuildViewHtml("help/about/Recruitment", filePath);
            #endregion

            #region  主页右边部分
            filePath = Path.Combine(path, "zjsmshpj.html");
            BuildViewHtml("help/zjsmshpj", filePath);

            filePath = Path.Combine(path, "zjtzzh.html");
            BuildViewHtml("help/zjtzzh", filePath);

            filePath = Path.Combine(path, "jjzmjs.html");
            BuildViewHtml("help/jjzmjs", filePath);

            filePath = Path.Combine(path, "azjjwsmzyly.html");
            BuildViewHtml("help/azjjwsmzyly", filePath);

            filePath = Path.Combine(path, "chippedBonus.html");
            BuildViewHtml("help/chippedBonus", filePath);
            #endregion
            #endregion
        }
        #endregion

        /// <summary>
        /// 生成合买大厅数据
        /// </summary>
        private void BuildHeMaiHall()
        {
            #region 生成Redis缓存

            WebRedisHelper.LoadTogetherDataToRedis();

            #endregion
            return;


            ViewBag.User = CurrentUser;
            ViewBag.Game = string.Empty;
            ViewBag.GameType = string.Empty;
            ViewBag.IsMine = "false";
            ViewBag.issuseNumber = string.Empty;
            //最低金额-最大金额
            ViewBag.minMoney = -1;
            ViewBag.maxMoney = -1;
            //最小进度-最大进度
            ViewBag.minProgress = -1;
            ViewBag.maxProgress = -1;
            //合买方案保密性 0未知
            ViewBag.SchemeSecurity = null;
            //方案投注类别 0普通
            ViewBag.SchemeBetting = null;
            //合买方案进度
            ViewBag.SchemeProgress = null;
            //排序
            //ViewBag.orderBy = string.IsNullOrEmpty(Request["orderBy"]) ? "" : Request["orderBy"];
            ViewBag.orderByName = string.Empty;
            ViewBag.orderBySort = string.Empty;
            //保底和进度
            var orderBy = "";
            if (ViewBag.orderByName == "0")
                orderBy = "ManYuan desc,ISTOP DESC,Progress " + ViewBag.orderBySort + ",TotalMoney DESC";
            else if (ViewBag.orderByName == "1")
                orderBy = "ManYuan desc,ISTOP DESC,TotalMoney " + ViewBag.orderBySort + ", Progress DESC";
            //关键字
            var searchKey = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"];
            if (ViewBag.IsMine == "true" && CurrentUser != null)
            {
                searchKey = CurrentUser.LoginInfo.DisplayName;
            }
            ViewBag.key = searchKey;
            ViewBag.pageNo = 0;
            ViewBag.PageSize = 30;
            string userId = string.Empty;
            if (CurrentUser != null)
                userId = CurrentUser.LoginInfo.UserId;

            #region 生成文件缓存

            var SuperList = WCFClients.GameClient.QueryHotUserTogetherOrderList(userId);
            var TogList = WCFClients.GameClient.QuerySportsTogetherList(searchKey, ViewBag.issuseNumber, ViewBag.Game, ViewBag.GameType, ViewBag.SchemeSecurity,
                 ViewBag.SchemeBetting, ViewBag.SchemeProgress, ViewBag.minMoney, ViewBag.maxMoney, ViewBag.minProgress,
                 ViewBag.maxProgress, orderBy, ViewBag.pageNo, 30000, userId);

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "chipped");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var strSuperList = string.Empty;
            var strTogList = string.Empty;
            if (SuperList != null && SuperList.Count > 0)
                strSuperList = JsonSerializer.Serialize<TogetherHotUserInfoCollection>(SuperList);
            if (TogList != null && TogList.List.Count > 0)
                strTogList = JsonSerializer.Serialize<Sports_TogetherSchemeQueryInfoCollection>(TogList);
            var superPath = Path.Combine(path, "superList.json");
            var toglistPath = Path.Combine(path, "hmList.json");
            System.IO.File.WriteAllText(superPath, strSuperList);
            System.IO.File.WriteAllText(toglistPath, strTogList);

            //清空内存缓存
            ClearChippedCache();

            #endregion
        }
        /// <summary>
        /// 生成过关统计
        /// </summary>
        private void BuilGuoGuanTongJi()
        {
            #region 生成Redis缓存

            WebRedisHelper.LoadGGTJ_To_Redis();

            #endregion
            return;

            var arrGameCode = new string[] { "JCZQ", "JCLQ", "BJDC", "T14C", "TR9", "T4CJQ", "T6BQC" };
            var gameCode = string.Empty;
            var gameType = string.Empty;
            var issuseNumber = string.Empty;
            var key = string.Empty;
            SchemeBettingCategory? schemeType = null;
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = DateTime.Now;
            int pageIndex = 0;
            int pageSize = 2000;
            #region 生成文件缓存

            foreach (var item in arrGameCode)
            {
                gameCode = item;
                if (item == "T14C" || item == "TR9" || item == "T4CJQ" || item == "T6BQC")
                {
                    gameCode = "CTZQ";
                    gameType = item;
                }
                if (gameCode.ToLower() == "ctzq" || gameCode.ToLower() == "bjdc")
                {
                    string prizedIssuse = WCFClients.GameClient.QueryStopIssuseList(gameCode, gameType, 20, UserToken);
                    var prizedIssuseList = prizedIssuse.Split(',');
                    issuseNumber = prizedIssuseList.FirstOrDefault();
                }

                var tojiList = WCFClients.GameQueryClient.QueryReportInfoList_GuoGuan(null, schemeType, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize);
                if (tojiList != null && tojiList.TotalCount > 0)
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "ggtj/" + gameCode + "");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var json = JsonSerializer.Serialize<SportsOrder_GuoGuanInfoCollection>(tojiList);
                    var ggtjPath = Path.Combine(path, "ggtj.json");
                    System.IO.File.WriteAllText(ggtjPath, json);
                }
            }

            //清空内存缓存
            ClearGGTJCache();

            #endregion
        }
    }
}
