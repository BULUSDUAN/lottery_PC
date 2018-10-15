using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.cbbao.Models;
using app.lottery.site.jsonManager;
using app.lottery.site.Models;
using Common.Communication;
using Common.JSON;
using GameBiz.Core;
using Common.Utilities;
using Common.XmlAnalyzer;
using External.Core.SiteMessage;
using System.Configuration;
using System.IO;
using Common.Lottery;
using System.Text;
using Common.Snapshot;
using System.Globalization;
using OrderLogWriter = app.lottery.site.Models.OrderLogWriter;
using Common.Net;
using app.lottery.site.iqucai;

namespace app.lottery.site.Controllers
{
    [CheckReferer]
    public class CommonController : BaseController
    {
        #region 合买
        public ActionResult Chipped(string id)
        {
            try
            {
                ViewBag.User = CurrentUser;
                ViewBag.Game = string.IsNullOrEmpty(id) ? "" : id;
                ViewBag.GameType = string.IsNullOrEmpty(Request["PlayType"]) ? "" : Request["PlayType"];
                ViewBag.IsMine = string.IsNullOrEmpty(Request["isMine"]) ? "false" : Request["isMine"];
                ViewBag.issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                //最低金额-最大金额
                ViewBag.minMoney = string.IsNullOrEmpty(Request["minMoney"]) ? -1 : decimal.Parse(Request["minMoney"]);
                ViewBag.maxMoney = string.IsNullOrEmpty(Request["maxMoney"]) ? -1 : decimal.Parse(Request["maxMoney"]);
                //最小进度-最大进度
                ViewBag.minProgress = string.IsNullOrEmpty(Request["minProgress"]) ? -1 : decimal.Parse(Request["minProgress"]);
                ViewBag.maxProgress = string.IsNullOrEmpty(Request["maxProgress"]) ? -1 : decimal.Parse(Request["maxProgress"]);
                //合买方案保密性 0未知
                ViewBag.SchemeSecurity = string.IsNullOrEmpty(Request["SchemeSecurity"]) ? null : (TogetherSchemeSecurity?)int.Parse(Request["SchemeSecurity"]);
                //方案投注类别 0普通
                ViewBag.SchemeBetting = string.IsNullOrEmpty(Request["SchemeBetting"]) ? null : (SchemeBettingCategory?)int.Parse(Request["SchemeBetting"]);
                //合买方案进度
                ViewBag.SchemeProgress = string.IsNullOrEmpty(Request["SchemeProgress"]) ? null : (TogetherSchemeProgress?)int.Parse(Request["SchemeProgress"]);
                //排序
                //ViewBag.orderBy = string.IsNullOrEmpty(Request["orderBy"]) ? "" : Request["orderBy"];
                ViewBag.orderByName = string.IsNullOrEmpty(Request["orderByName"]) ? "" : Request["orderByName"];
                ViewBag.orderBySort = string.IsNullOrEmpty(Request["orderBySort"]) ? "" : Request["orderBySort"];
                //保底和进度
                var orderBy = "";
                //if (ViewBag.orderByName == "0")
                //    orderBy = "ISTOP DESC,ProgressStatus ASC, Progress " + ViewBag.orderBySort + ",TotalMoney DESC";
                //else if (ViewBag.orderByName == "1")
                //    orderBy = "ISTOP DESC,ProgressStatus ASC,TotalMoney " + ViewBag.orderBySort + ", Progress DESC";

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
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
                string userId = string.Empty;
                if (CurrentUser != null)
                    userId = CurrentUser.LoginInfo.UserId;

                //ViewBag.SuperList = new TogetherHotUserInfoCollection();
                //ViewBag.TogList = new Sports_TogetherSchemeQueryInfoCollection();

                //从SQL数据库查询
                //ViewBag.SuperList = WCFClients.GameClient.QueryHotUserTogetherOrderList(userId);
                //ViewBag.TogList = WCFClients.GameClient.QuerySportsTogetherList(searchKey, ViewBag.issuseNumber, ViewBag.Game, ViewBag.GameType, ViewBag.SchemeSecurity,
                //    ViewBag.SchemeBetting, ViewBag.SchemeProgress, ViewBag.minMoney, ViewBag.maxMoney, ViewBag.minProgress,
                //    ViewBag.maxProgress, orderBy, ViewBag.pageNo, ViewBag.PageSize, userId);

                //从缓存文件查询
                //ViewBag.SuperList = this.QueryHotUserTogetherOrderList();
                //ViewBag.TogList = QuerySportsTogetherList(searchKey, ViewBag.issuseNumber, ViewBag.Game, ViewBag.GameType, ViewBag.SchemeSecurity,
                //  ViewBag.SchemeBetting, ViewBag.SchemeProgress, ViewBag.minMoney, ViewBag.maxMoney, ViewBag.minProgress,
                //  ViewBag.maxProgress, orderBy, ViewBag.pageNo, ViewBag.PageSize);

                //从Redis库查询
                ViewBag.SuperList = WebRedisHelper.QueryHotTogetherUserListFromRedis();
                ViewBag.TogList = WebRedisHelper.QuerySportsTogetherListFromRedis(searchKey, ViewBag.issuseNumber, ViewBag.Game, ViewBag.GameType, ViewBag.SchemeSecurity,
                  ViewBag.SchemeBetting, ViewBag.SchemeProgress, ViewBag.minMoney, ViewBag.maxMoney, ViewBag.minProgress,
                  ViewBag.maxProgress, orderBy, ViewBag.pageNo, ViewBag.PageSize);
            }
            catch (Exception ex)
            {
                ViewBag.TogList = new Sports_TogetherSchemeQueryInfoCollection();
            }
            return View();
        }
        /// <summary>
        /// 追加保底
        /// </summary>
        /// <returns></returns>
        public JsonResult AddGuarantees()
        {
            try
            {
                var schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeId"], "订单编号不能为空");
                var buycount = string.IsNullOrEmpty(Request["buycount"]) ? 1 : int.Parse(Request["buycount"]);
                var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户编号不能为空");
                var balanecpwd = string.IsNullOrEmpty(Request["balanecpwd"]) ? "" : Request["balanecpwd"];
                var result = WCFClients.GameClient.XianFaQi_AddGuarantees(schemeId, buycount, userId, balanecpwd);
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        #endregion

        #region 定制跟单管理

        //撤销定制跟单
        [HttpPost]
        public JsonResult doc_cancel(string id)
        {
            try
            {
                var followId = long.Parse(PreconditionAssert.IsNotEmptyString(id, "定制跟单编号不能为空"));
                var result = WCFClients.GameClient.ExistTogetherFollower(followId, UserToken);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //定制跟单
        [HttpPost]
        public JsonResult doc_setup()
        {
            try
            {
                var isEdite = string.IsNullOrEmpty(Request["isEdite"]) ? false : bool.Parse(Request["isEdite"]);
                var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "被跟单对象错误");
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "被跟单彩种不能为空");
                var gameType = string.IsNullOrEmpty(Request["gameType"]) ? "" : Request["gameType"];
                var docType = string.IsNullOrEmpty(Request["docType"]) ? 0 : int.Parse(Request["docType"]);
                var buyCount = string.IsNullOrEmpty(Request["buyCount"]) ? 1 : int.Parse(Request["buyCount"]);
                var allBuy = string.IsNullOrEmpty(Request["allBuy"]) ? "不限" : Request["allBuy"];
                var maxMoney = string.IsNullOrEmpty(Request["maxMoney"]) ? "不限" : Request["maxMoney"];
                var minMoney = string.IsNullOrEmpty(Request["minMoney"]) ? "不限" : Request["minMoney"];
                var minBalance = string.IsNullOrEmpty(Request["minBalance"]) ? "不限" : Request["minBalance"];
                var isBuySchemeMoneyNot = string.IsNullOrEmpty(Request["isBuySchemeMoneyNot"]) ? false : bool.Parse(Request["isBuySchemeMoneyNot"]);
                var isUsed = string.IsNullOrEmpty(Request["isUsed"]) ? true : bool.Parse(Request["isUsed"]);
                var isAutoStop = string.IsNullOrEmpty(Request["isAutoStop"]) ? false : bool.Parse(Request["isAutoStop"]);
                var autoStopCount = string.IsNullOrEmpty(Request["autoStopCount"]) ? 10 : int.Parse(Request["autoStopCount"]);

                TogetherFollowerRuleInfo info = new TogetherFollowerRuleInfo()
                {
                    CreaterUserId = userId,
                    FollowerUserId = CurrentUser.LoginInfo.UserId,
                    GameCode = gameCode,
                    GameType = gameType,
                    MaxSchemeMoney = maxMoney == "不限" ? -1 : int.Parse(maxMoney),
                    MinSchemeMoney = minMoney == "不限" ? -1 : int.Parse(minMoney),
                    SchemeCount = allBuy == "不限" ? -1 : int.Parse(allBuy),
                    StopFollowerMinBalance = minBalance == "不限" ? -1 : int.Parse(minBalance),
                    FollowerCount = docType == 0 ? buyCount : -1,
                    FollowerPercent = docType == 1 ? buyCount : -1,
                    CancelNoBonusSchemeCount = isAutoStop ? autoStopCount : -1,
                    CancelWhenSurplusNotMatch = isBuySchemeMoneyNot,
                    IsEnable = isUsed
                };
                if (isEdite)
                {
                    var followId = long.Parse(PreconditionAssert.IsNotEmptyString(Request["ruleId"], "定制跟单编号不能为空"));
                    var result = WCFClients.GameClient.EditTogetherFollower(info, followId, UserToken);
                    return Json(result);
                }
                else
                {
                    var result = WCFClients.GameClient.CustomTogetherFollower(info, UserToken);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        #endregion

        #region 过关统计
        public ActionResult Soccer(string id)
        {
            ViewBag.User = CurrentUser;

            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "JCZQ" : id.ToLower();
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

            //是否显示我的过关-true为显示-其余为false
            ViewBag.IsShowMine = string.IsNullOrEmpty(Request["IsShowMine"]) || Request["IsShowMine"] == "false" || Request["IsShowMine"] == "False" ? false : true;
            if (ViewBag.IsShowMine && CurrentUser != null)
                ViewBag.Key = CurrentUser.LoginInfo.DisplayName;



            var startTime = DateTime.Now.AddDays(-1);//过关统计生成最近三天数据
            var endTime = DateTime.Now;
            //从sql查询
            //var list = WCFClients.GameQueryClient.QueryReportInfoList_GuoGuan(ViewBag.isVirtualOrder, schemeType, ViewBag.Key, ViewBag.GameCode, ViewBag.GameType, ViewBag.IssuseNumber, startTime, endTime, ViewBag.PageIndex, ViewBag.PageSize);

            //从文件查询
            //var list = this.QueryReportInfoList_GuoGuan(ViewBag.isVirtualOrder, schemeType, ViewBag.Key, ViewBag.GameCode, ViewBag.GameType, ViewBag.IssuseNumber, startTime, endTime, ViewBag.PageIndex, ViewBag.PageSize);

            //从Redis查询
            var list = WebRedisHelper.QueryReportInfoList_GuoGuan(ViewBag.isVirtualOrder, schemeType, ViewBag.Key, ViewBag.GameCode, ViewBag.GameType, ViewBag.IssuseNumber, startTime, endTime, ViewBag.PageIndex, ViewBag.PageSize);

            ViewBag.TojiList = list;
            return View();
        }
        #endregion

        #region 开奖结果
        public ActionResult Lotteryhall(string id)
        {
            id = string.IsNullOrEmpty(id) ? "newindex" : id;
            ViewBag.GameCode = id;
            return View();
        }
        public PartialViewResult Detail(string id)
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
            return PartialView("newdetail/" + id);
        }
        #endregion

        #region 中奖排行榜

        public ActionResult Bonus(string id)
        {
            ViewBag.User = CurrentUser;
            return View();
        }

        public ActionResult NewBonus(string id)
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        #endregion

        #region 会员中心
        public ActionResult SuperVip()
        {
            return View();
        }
        #endregion

        #region 购买
        public ActionResult Szc(string id)
        {
            var gameCode = string.IsNullOrEmpty(id) ? "CQSSC" : id.ToUpper();
            if (gameCode == "SSC")
                gameCode = "CQSSC";
            ViewBag.GameCode = gameCode;
            ViewBag.CurrentUser = CurrentUser;
            //ViewBag.IsInBetWhiteList = IsInBetWhiteList();
            //if (GameList.Where(a => a.GameCode.ToLower() == id.ToLower()).Count() < 1)
            //{
            //    //throw new HttpException(404, "彩种不存在 - " + id);
            //}
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
        public PartialViewResult Szc_FB(string id)
        {
            var gameCode = string.IsNullOrEmpty(id) ? "CQSSC" : id.ToUpper();
            if (gameCode == "SSC")
                gameCode = "CQSSC";
            ViewBag.GameCode = gameCode;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return PartialView();
        }

        public PartialViewResult Cqssc_All()
        {
            return PartialView();
        }
        public PartialViewResult Jx11x5_All()
        {
            return PartialView();
        }
        public PartialViewResult Jsks_All()
        {
            return PartialView();
        }
        public PartialViewResult Sdklpk3_All()
        {
            return PartialView();
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

                var FILE_MAX_LENGTH = 200 * 1024;//限制文件大小为1MB
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


        /// <summary>
        /// 传统足球单式上传验证格式
        /// </summary>
        /// <returns></returns>
        public ContentResult Uploadcz()
        {

            try
            {
                Session["fileStream"] = null; //清空文件流
                var fileObj = Request.Files["upload_file"];
                if (fileObj == null || fileObj.ContentLength <= 0) throw new Exception("上传文件不能为空");
                var FILE_MAX_LENGTH = 10 * 1024; //限制文件大小为10MB
                var fileName = fileObj.FileName.Split('.')[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + ".txt";
                if (fileObj.ContentLength > FILE_MAX_LENGTH)
                    throw new Exception("文件内容长度：" + fileObj.ContentLength + "超过了最大限制长度：" + FILE_MAX_LENGTH / 1024 + "KB");
                StreamReader reader = new StreamReader(fileObj.InputStream, System.Text.Encoding.Default);
                var str = reader.ReadToEnd(); //文件流
                var antecodelist = str.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                var gameType = Request["gameType"];
                if (string.IsNullOrEmpty(gameType))
                    throw new Exception("参数未正确提交");
                var itemCount = 0;
                switch (gameType)
                {
                    case "t14c":
                        itemCount = 14;
                        break;
                    case "tr9":
                        itemCount = 9;
                        break;
                    case "t6bqc":
                        itemCount = 12;
                        break;
                    case "t4cjq":
                        itemCount = 8;
                        break;
                }
                if (itemCount != 14 && itemCount != 9 && itemCount != 12 && itemCount != 8)
                    throw new Exception("参数未正确提交");

                var separator = ",";
                var separatorCount = 0;
                foreach (var item in antecodelist)
                {
                    if (!item.Contains("3") && !item.Contains("1") && !item.Contains("0"))
                        return Content("{state:false,msg:'投注内容格式不正确,投注内容只能包含3，1，0}");
                    separatorCount = 0;
                    if (item.Contains(","))
                    {
                        separatorCount++;
                        separator = "";
                    }
                    if (item.Contains("*"))
                    {
                        separatorCount++;
                        separator = "*";
                    }
                    if (item.Contains("|"))
                    {
                        separatorCount++;
                        separator = "|";
                    }
                    if (item.Contains("#"))
                    {
                        separatorCount++;
                        separator = "#";
                    }
                    if (separatorCount > 1)
                        return Content("{state:false,msg:'投注内容格式不正确,分隔符不能同时包含2个或2个以上'}");
                    var length = 0;
                    switch (separatorCount)
                    {
                        case 0:
                            length = item.Length;
                            break;
                        case 1:
                            length = item.Replace(separator, "").Length;
                            break;
                    }
                    if (itemCount != length)
                        return Content("{state:false,msg:'投注内容格式不正确'}");
                }
                Session["fileStream"] = str;

                //允许投注的号码
                var codes = PreconditionAssert.IsNotEmptyString(Request["allowCodes"], "允许投注的号码不能为空").Split(',');
                var matchIdList = new List<string>();
                var result = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(str, gameType.ToUpper(), codes, out matchIdList);
                return Content("{state:true,count:'" + result.Count + "',fileName:'" + fileName + "'}");
            }
            catch (Exception ex)
            {
                return Content("{state:false,msg:'" + ex.Message.Replace("\r", "").Replace("\n", "") + "'}");
            }
        }

        public ActionResult JingCai(string id)
        {
            id = string.IsNullOrEmpty(id) ? "hh" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //是否是方案欲投
            var schemeId = string.IsNullOrEmpty(Request["schemeid"]) ? "" : Request["schemeid"];
            var isYt = !string.IsNullOrEmpty(Request["isYt"]) && Convert.ToBoolean(int.Parse(Request["isYt"].ToLower()));
            if (isYt)
            {
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(schemeId);
                ViewBag.Money = info.TotalMoney;
                ViewBag.UserId = info.UserId;
            }
            else
            {
                ViewBag.Money = 0M;
                ViewBag.UserId = "";
            }

            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 2 : int.Parse(Request["oddstype"]);
            ViewBag.OddType = oddtype;
            //if (oddtype == 3)  //单式上传
            //{
            //    if (!string.IsNullOrEmpty(id))
            //    {
            //        if ("spf" == id.ToLower() || "rqspf" == id.ToLower() || "zjq" == id.ToLower() || "bqc" == id.ToLower() || "bf" == id.ToLower())
            //        {
            //            //读取数据
            //            try
            //            {
            //                //var matches = new List<JczqWeb>();
            //                var match = Json_JCZQ.MatchList_WEB(id, oddtype);
            //                if (match.Count > 0)
            //                {
            //                    var minsaledate =
            //                        match.Where(p => Convert.ToDateTime(p.StartDateTime) > DateTime.Today)
            //                            .Min(a => Convert.ToDateTime(a.StartDateTime));
            //                    var mindate = minsaledate.Hour < 11
            //                        ? minsaledate.AddDays(-1).ToString("yyyy-MM-dd")
            //                        : minsaledate.ToString("yyyy-MM-dd");
            //                    match = match.Where(
            //                        a => DateTime.Parse(a.StartDateTime) > DateTime.Parse(mindate).AddHours(11))
            //                        .ToList();
            //                    //开售的比赛
            //                    match = match.Where(p => Convert.ToDateTime(p.DSStopBettingTime) > DateTime.Now).ToList();
            //                    //过滤掉比赛中有赔率停售的场次
            //                    switch (id.ToLower())
            //                    {
            //                        case "spf":
            //                            match = match.Where(item => item.BRQSPF.NoSaleState == "0").ToList();
            //                            break;
            //                        case "rqspf":
            //                            match = match.Where(item => item.SPF.NoSaleState == "0").ToList();
            //                            break;
            //                        case "zjq":
            //                            match = match.Where(item => item.ZJQ.NoSaleState == "0").ToList();
            //                            break;
            //                        case "bqc":
            //                            match = match.Where(item => item.BQC.NoSaleState == "0").ToList();
            //                            break;
            //                        case "bf":
            //                            match = match.Where(item => item.BF.NoSaleState == "0").ToList();
            //                            break;
            //                    }

            //                    ViewBag.Match = match;
            //                }
            //                else
            //                {
            //                    //ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            //                    ViewBag.Match = new List<JczqWeb>();
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                ViewBag.Match = new List<JczqWeb>();
            //            }
            //        }
            //    }
            //}

            ViewBag.SchemeId = schemeId;
            //查询大奖排行
            //var now = DateTime.Now;
            //暂时屏蔽中奖排行
            //var weekRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-7), now, 10,
            //                                                                              "JCZQ");
            //var monthRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-30), now, 10,
            //                                                                              "JCZQ");
            //var rank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-90), now, 10,
            //                                                                              "JCZQ");
            //ViewBag.WeekRank = weekRank;
            //ViewBag.MonthRank = monthRank;
            //ViewBag.Rank = rank;
            //控制网站部分功能是否显示
            //ViewBag.IsShowData = this.IsShowData();
            //ViewBag.IsInBetWhiteList = IsInBetWhiteList();
            return View();
        }
        public ActionResult Jingcaibasket(string id)
        {
            id = string.IsNullOrEmpty(id) ? "hh" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //查询大奖排行
            //var now = DateTime.Now;
            //暂时屏蔽中奖排行
            //var weekRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-7), now, 10,
            //                                                                              "JCLQ");
            //var monthRank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-30), now, 10,
            //                                                                              "JCLQ");
            //var rank = WCFClients.GameQueryClient.QueryRankReport_BonusByGameCode_All(now.AddDays(-90), now, 10,
            //                                                                              "JCLQ");
            //ViewBag.WeekRank = weekRank;
            //ViewBag.MonthRank = monthRank;
            //ViewBag.Rank = rank;
            //ViewBag.IsShowData = this.IsShowData();
            //ViewBag.IsInBetWhiteList = IsInBetWhiteList();
            return View();
        }
        public ActionResult Toto(string id)
        {
            id = string.IsNullOrEmpty(id) ? "t14c" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 0 : int.Parse(Request["oddstype"]);
            ViewBag.OddType = oddtype;
            //ViewBag.IsInBetWhiteList = IsInBetWhiteList();

            //var issuses = Json_CTZQ.IssuseList(id).Where(p => DateTime.Parse(p.StopBettingTime) > DateTime.Now).OrderBy(a => a.StopBettingTime).ToList();
            var issuses = Json_CTZQ.IssuseList(id).OrderBy(a => a.StopBettingTime);
            var saleIssue = new List<CtzqIssuesWeb>();
            var stopIssue = new List<string>();
            foreach (var issue in issuses)
            {
                if (Convert.ToDateTime(issue.StopBettingTime) > DateTime.Now)  //可售
                {
                    saleIssue.Add(new CtzqIssuesWeb { IssuseNumber = issue.IssuseNumber, StopBettingTime = issue.StopBettingTime, StartTime = issue.StartTime });
                }
                else   //过期
                {
                    stopIssue.Add(issue.IssuseNumber);
                }
            }
            ViewBag.SaleIssue = saleIssue;
            ViewBag.StopIssue = stopIssue;
            return View();
        }
        public ActionResult Danchang(string id)
        {
            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //是否是方案欲投
            var schemeId = string.IsNullOrEmpty(Request["schemeid"]) ? "" : Request["schemeid"];
            var isYt = !string.IsNullOrEmpty(Request["isYt"]) && Convert.ToBoolean(int.Parse(Request["isYt"].ToLower()));
            if (isYt)
            {
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(schemeId);
                ViewBag.Money = info.TotalMoney;
                ViewBag.UserId = info.UserId;
            }
            else
            {
                ViewBag.Money = 0M;
                ViewBag.UserId = "";
            }

            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 2 : int.Parse(Request["oddstype"]);
            ViewBag.OddType = oddtype;
            if (oddtype == 2)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if ("spf" == id || "zjq" == id || "sxds" == id || "bf" == id || "bqc" == id)
                    {
                        //读取数据
                        try
                        {
                            //期数列表
                            var issuses = Json_BJDC.IssuseList().OrderByDescending(o => o.IssuseNumber).ToList();
                            ViewBag.RequestIssue = string.IsNullOrEmpty(Request["issueNum"]) ? "" : Request["issueNum"];
                            ViewBag.CurrentIssue = issuses[0].IssuseNumber;
                            ViewBag.Issuses = issuses;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Match = new List<JczqWeb>();
                        }
                    }
                }
            }
            else if (oddtype == 3)//单式上传
            {

            }

            ViewBag.SchemeId = schemeId;
            return PartialView();
        }
        public PartialViewResult Single(string g)
        {
            var balance = CurrentUserBalance;
            var needPassword = balance.CheckIsNeedPassword("Bet");
            var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]);
            ViewBag.Kind = kind;
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
            ViewBag.Bonus = balance.RedBagBalance;
            return PartialView();
        }
        public PartialViewResult Szcpop(string g)
        {
            var balance = CurrentUserBalance;
            var needPassword = balance.CheckIsNeedPassword("Bet");
            ViewBag.NeedPassword = needPassword;
            if (bool.Parse(Request["ishm"]) || bool.Parse(Request["ischase"]))
            {
                ViewBag.BonusRatio = 0M;
                ViewBag.Bonus = 0M;
            }
            else
            {
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
                ViewBag.Bonus = balance.RedBagBalance;
            }
            return PartialView();
        }

        public PartialViewResult SchemeShare()
        {
            var needPassword = CurrentUserBalance.CheckIsNeedPassword("Bet");
            var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]);
            ViewBag.Kind = kind;
            ViewBag.BDFXMaxCommission = WCFClients.GameClient.QueryConfigByKey("BDFXMaxCommission");
            ViewBag.NeedPassword = needPassword;
            return PartialView();
        }

        public PartialViewResult Hemai()
        {
            var needPassword = CurrentUserBalance.CheckIsNeedPassword("Bet");
            var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]);
            ViewBag.Kind = kind;
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
        /// <summary>
        /// 欧洲冠军，欧洲冠亚军
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult bet_ozb()
        {
            try
            {
                #region Request参数

                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "彩种编码不能为空。");
                var number = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
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

                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }
                if (redBagMoney > 0)
                {
                    //红包余额
                    var rbTotal = CurrentUserBalance.RedBagBalance;
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "红包余额为0" });
                    }
                    if (redBagMoney > rbTotal)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("红包余额不足{0}元", redBagMoney) });
                    }
                    //查询可用红包比例
                    var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis("OZB");//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "当前彩种不允许使用红包" });

                    var maxRbMoney = totalMoney * ratio / 100;//该彩种可用金额
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }
                var useBalance = info.TotalMoney - redBagMoney;
                if (useBalance > CurrentUserBalance.GetTotalCashMoney())
                {
                    return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", useBalance) });
                }

                var result = new CommonActionResult();
                if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                {
                    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                    WebRedisHelper.CheckGameStatus(info.GameCode, gameType);

                    //投注到Redis消息队列
                    var schemeId = WebRedisHelper.Redis_Bet_SZC(new RedisBet_LotteryBetting
                    {
                        BetInfo = info,
                        RedBagMoney = redBagMoney,
                        UserToken = UserToken,
                        BalancePassword = balancepwd,
                    });
                    result = new CommonActionResult
                    {
                        IsSuccess = true,
                        Message = "订单提交成功，等待出票",
                        ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney)
                    };
                }
                else
                {
                    result = WCFClients.GameClient.BetOZB(info, balancepwd, redBagMoney, UserToken);
                }
                return Json(result);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 世界杯
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult bet_sjb()
        {
            try
            {
                #region Request参数

                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "彩种编码不能为空。");
                var number = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
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
                    GameCode = "SJB",
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
                    IssuseNumber = "2018",
                    Amount = amount,
                    IssuseTotalMoney = totalMoney,
                });

                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }
                if (redBagMoney > 0)
                {
                    //红包余额
                    var rbTotal = CurrentUserBalance.RedBagBalance;
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "红包余额为0" });
                    }
                    if (redBagMoney > rbTotal)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("红包余额不足{0}元", redBagMoney) });
                    }
                    //查询可用红包比例
                    var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis("SJB");//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "当前彩种不允许使用红包" });

                    var maxRbMoney = totalMoney * ratio / 100;//该彩种可用金额
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }
                var useBalance = info.TotalMoney - redBagMoney;
                if (useBalance > CurrentUserBalance.GetTotalCashMoney())
                {
                    return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", useBalance) });
                }

                var result = new CommonActionResult();
                //if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                //{
                //    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                //    WebRedisHelper.CheckGameStatus(info.GameCode, gameType);

                //    //投注到Redis消息队列
                //    var schemeId = WebRedisHelper.Redis_Bet_SZC(new RedisBet_LotteryBetting
                //    {
                //        BetInfo = info,
                //        RedBagMoney = redBagMoney,
                //        UserToken = UserToken,
                //        BalancePassword = balancepwd,
                //    });
                //    result = new CommonActionResult
                //    {
                //        IsSuccess = true,
                //        Message = "订单提交成功，等待出票",
                //        ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney)
                //    };
                //}
                //else
                //{
                //    result = WCFClients.GameClient.BetOZB(info, balancepwd, redBagMoney, UserToken);
                //}
                result = WCFClients.GameClient.BetSJB(info, balancepwd, redBagMoney, UserToken);
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
        /// 
        [HttpPost]
        public JsonResult Order()
        {
            try
            {
                #region Request参数
                var isFilter = string.IsNullOrEmpty(Request["isFilter"]) ? false : bool.Parse(Request["isFilter"]);
                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空。"));
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空。");
                var number = PreconditionAssert.IsNotEmptyString(Request["Number"], "投注号码不能为空。");
                var issuseNumberArray = Request["IssuseNumber"].Split('#');//投注期数 20160623-071|1|2
                string curIssuse = Request["CurrentIssuse"];//当前期 20160623-068
                var balancepwd = Request["balancepwd"];
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                bool isStopAfterBonus = string.IsNullOrEmpty(Request["IsStopAfterBonus"]) ? false : bool.Parse(Request["IsStopAfterBonus"]);
                var amount = string.IsNullOrEmpty(Request["amount"]) ? 0 : int.Parse(Request["amount"]);
                var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var schemeCategory = isFilter ? SchemeBettingCategory.FilterBetting : SchemeBettingCategory.GeneralBetting;
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var IsAppend = string.IsNullOrEmpty(Request["IsAppend"]) ? false : Boolean.Parse(Request["IsAppend"]);
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
                var activityType = 2;
                if (gameCode.ToUpper() == "CTZQ")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }

                #endregion

                #region 投注号码
                LotteryAnteCodeInfoCollection codeList = new LotteryAnteCodeInfoCollection();
                foreach (var item in number.Split('#'))
                {
                    var codeArray = item.Split('.');
                    if (codeArray.Length == 3)
                    {
                        codeList.Add(new LotteryAnteCodeInfo
                        {
                            GameType = codeArray[0],
                            AnteCode = codeArray[1],
                            IsDan = Boolean.Parse(codeArray[2])
                        });
                    }
                }

                //合买投注号码对象
                var togAnteList = new Sports_AnteCodeInfoCollection();
                foreach (var item in number.Split('#'))
                {
                    var codeArray = item.Split('.');
                    if (codeArray.Length == 3)
                    {
                        var code = new Sports_AnteCodeInfo()
                        {
                            GameType = codeArray[0],
                            AnteCode = codeArray[1],
                            IsDan = Boolean.Parse(codeArray[2])
                        };
                        togAnteList.Add(code);
                    }
                }
                #endregion

                #region 追号信息，期号列表

                if (issuseNumberArray.Length <= 0)
                {
                    throw new Exception("投注没有包含期号信息。");
                }
                LotteryBettingIssuseInfoCollection issuseList = new LotteryBettingIssuseInfoCollection();
                foreach (var item in issuseNumberArray)
                {
                    var issuseArray = item.Split('|');
                    if (issuseArray.Length == 3)
                    {
                        issuseList.Add(new LotteryBettingIssuseInfo()
                        {
                            IssuseNumber = issuseArray[0],
                            Amount = int.Parse(issuseArray[1]),
                            IssuseTotalMoney = decimal.Parse(issuseArray[2])
                        });
                    }
                }
                #endregion

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

                    var togInfo = new Sports_TogetherSchemeInfo
                    {
                        GameCode = gameCode,
                        GameType = codeList.FirstOrDefault().GameType,
                        IssuseNumber = issuseList.FirstOrDefault().IssuseNumber,
                        Amount = amount,
                        AnteCodeList = togAnteList,
                        PlayType = "",
                        SchemeSource = SchemeSource.Web,
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
                        ActivityType = (ActivityType)activityType,
                        IsAppend = IsAppend,
                        IsRepeat = IsRepeat,
                        CurrentBetTime = DateTime.Now,

                    };
                    var hmUseBalance = (guarantees + subscription) * 1M;
                    if (hmUseBalance > CurrentUserBalance.GetTotalCashMoney())
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", hmUseBalance) });
                    }
                    var hmResult = new CommonActionResult();
                    //if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                    //{
                    //    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                    //    WebRedisHelper.CheckGameStatus(togInfo.GameCode, codeList.FirstOrDefault().GameType);
                    //    WebRedisHelper.CheckGeneralBettingMatch(togInfo.GameCode, togInfo.GameType, togInfo.PlayType, togInfo.AnteCodeList, togInfo.IssuseNumber, togInfo.BettingCategory);
                    //    var schemeId = WebRedisHelper.Redis_Bet_Together_Sports(new RedisBet_CreateSportsTogether
                    //    {
                    //        BalancePassword = balancepwd,
                    //        BetInfo = togInfo,
                    //        UserToken = UserToken
                    //    });
                    //    hmResult = new CommonActionResult
                    //    {
                    //        IsSuccess = true,
                    //        Message = "合买订单提交成功",
                    //        ReturnValue = string.Format("{0}|{1}", schemeId, togInfo.TotalMoney)
                    //    };
                    //}
                    //else
                    //{
                    //    hmResult = WCFClients.GameClient.CreateSportsTogether(togInfo, balancepwd, UserToken);
                    //}
                    hmResult = WCFClients.GameClient.CreateSportsTogether(togInfo, balancepwd, UserToken);

                    //刷新余额
                    LoadUerBalance();

                    return Json(hmResult);
                }
                #endregion

                #region 普通投注
                //投注信息
                LotteryBettingInfo info = new LotteryBettingInfo()
                {
                    GameCode = gameCode,
                    AnteCodeList = codeList,
                    IssuseNumberList = issuseList,
                    SchemeSource = SchemeSource.Web,
                    StopAfterBonus = isStopAfterBonus,
                    TotalMoney = totalMoney,
                    Security = sercu,
                    BettingCategory = schemeCategory,
                    ActivityType = (ActivityType)activityType,
                    IsAppend = IsAppend,
                    IsRepeat = IsRepeat,
                    CurrentBetTime = DateTime.Now,
                };
                //如果使用红包，则检测红包金额是否合格
                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }
                if (redBagMoney > 0)
                {
                    //红包余额
                    var rbTotal = CurrentUserBalance.RedBagBalance;
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "红包余额为0" });
                    }
                    if (redBagMoney > rbTotal)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("红包余额不足{0}元", redBagMoney) });
                    }
                    //查询可用红包比例
                    var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "当前彩种不允许使用红包" });

                    var maxRbMoney = totalMoney * ratio / 100;//该彩种可用金额
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }
                var useBalance = info.TotalMoney - redBagMoney;
                if (useBalance > CurrentUserBalance.GetTotalCashMoney())
                {
                    return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", useBalance) });
                }


                //如果是0表示免费保存订单
                //var result = kind == 0 ? WCFClients.GameClient.SaveOrderLotteryBetting(info, UserToken) 
                //    : WCFClients.GameClient.LotteryBetting(info, balancepwd, redBagMoney, UserToken);

                var result = new CommonActionResult();
                if (kind == 0)
                {
                    //如果是0表示免费保存订单
                    result = WCFClients.GameClient.SaveOrderLotteryBetting(info, UserToken);
                }
                else
                {
                    //if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                    //{
                    //    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                    //    WebRedisHelper.CheckGameStatus(info.GameCode, codeList.FirstOrDefault().GameType);
                    //    if (issuseList.Count > 0)
                    //        WebRedisHelper.CheckBetIssuse(info.GameCode, info.IssuseNumberList[0].IssuseNumber);

                    //    //投注到Redis消息队列
                    //    var schemeId = WebRedisHelper.Redis_Bet_SZC(new RedisBet_LotteryBetting
                    //    {
                    //        BetInfo = info,
                    //        RedBagMoney = redBagMoney,
                    //        UserToken = UserToken,
                    //        BalancePassword = balancepwd,
                    //    });
                    //    result = new CommonActionResult
                    //    {
                    //        IsSuccess = true,
                    //        Message = "订单提交成功，等待出票",
                    //        ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney)
                    //    };
                    //}
                    //else
                    //{
                    //    result = WCFClients.GameClient.LotteryBetting(info, balancepwd, redBagMoney, UserToken);
                    //}
                    result = WCFClients.GameClient.LotteryBetting(info, balancepwd, redBagMoney, UserToken);
                }

                //刷新余额
                LoadUerBalance();

                return Json(result);

                #endregion
            }
            catch (Exception ex)
            {
                //string msg = string.Format("很抱歉，未经过实名认证，不允许购买彩票。<br />请移步去个人资料完成<a href=\"/member/safe\" style=\"color:Red\">实名认证</a>");
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //北单、竞彩足球、竞彩篮球-投注函数
        [HttpPost]
        public JsonResult bet_sports()
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
                var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 6 : int.Parse(Request["sercurity"])); //方案保密性
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var activityType = 2;
                var isexy = string.IsNullOrEmpty(Request["isExy"]) ? false : Boolean.Parse(Request["isExy"]);
                if (gameCode.ToUpper() == "JCZQ" || gameCode.ToUpper() == "BJDC")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
                var isBdfx = string.IsNullOrEmpty(Request["isbdfx"]) ? false : bool.Parse(Request["isbdfx"]);

                #endregion

                #region 投注号码

                var bfCount = 0;
                var bqcCount = 0;
                var sfcCount = 0;
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
                        if (gameCode.ToLower() == "jczq")
                        {
                            if (code.GameType.ToLower() == "bf")
                            {
                                bfCount++;
                            }
                            else if (code.GameType.ToLower() == "bqc")
                            {
                                bqcCount++;
                            }
                        }
                        else if (gameCode.ToLower() == "jclq")
                        {
                            if (code.GameType.ToLower() == "sfc")
                                sfcCount++;
                        }
                    }
                    else
                    {
                        code.GameType = gameType;
                    }
                    anteCodeList.Add(code);
                }

                //投注过关方式
                playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                var tempMatchCount = 0;
                if (playType != null && playType.Length > 0)
                {
                    tempMatchCount = playType.Split('|').Select(s => s.Split('_')[0]).Where(s => int.Parse(s) > 4).Count();
                }
                if (bfCount > 0 || bqcCount > 0)
                {
                    if (tempMatchCount > 0)
                        PreconditionAssert.IsNotEmptyString("", "竞彩足球-包含比分或半全场玩法投注，串关方式最多不能超过四串!");
                }
                else if (sfcCount > 0)
                {
                    if (tempMatchCount > 0)
                        PreconditionAssert.IsNotEmptyString("", "竞彩篮球-包含胜分差玩法投注，串关方式最多不能超过四串!");
                }

                #endregion

                #region 神单分享

                if (isBdfx)
                {
                    var fxbonusdeduct = string.IsNullOrEmpty(Request["bonusdeductfx"]) ? 0 : Decimal.Parse(Request["bonusdeductfx"]); //提成比例
                    var fxdesc = string.IsNullOrEmpty(Request["descriptionfx"]) ? amount + "倍，共" + totalMoney + "元" : Request["descriptionfx"];

                    var BDFXCommissionStr = WCFClients.GameClient.QueryConfigByKey("BDFXCommission");
                    if (fxbonusdeduct > Convert.ToInt32(BDFXCommissionStr.ConfigValue))
                    {
                        return Json(new {IsSuccess=false,Message="神单提成比例最大值为:" + BDFXCommissionStr.ConfigValue });
                    }

                    //投注对象
                    Sports_BetingInfo fxinfo = new Sports_BetingInfo()
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
                        ActivityType = (ActivityType)activityType,
                        BettingCategory = isexy ? SchemeBettingCategory.ErXuanYi : SchemeBettingCategory.GeneralBetting,
                        SchemeProgress = TogetherSchemeProgress.Finish,
                        IsRepeat = IsRepeat,
                        BDFXCommission = fxbonusdeduct,
                        SingleTreasureDeclaration = fxdesc,
                        CurrentBetTime = DateTime.Now
                    };

                    var fxResult = WCFClients.GameClient.SaveOrderSportsBetting_DBFX(fxinfo, CurrentUser.LoginInfo.UserId);
                    if (fxResult.IsSuccess)
                    {
                        LoadUerBalance();
                    }
                    return Json(fxResult);
                }

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
                    ActivityType = (ActivityType)activityType,
                    BettingCategory = isexy ? SchemeBettingCategory.ErXuanYi : SchemeBettingCategory.GeneralBetting,
                    SchemeProgress = TogetherSchemeProgress.Finish,
                    IsRepeat = IsRepeat,
                    CurrentBetTime = DateTime.Now,
                };

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

                    Sports_TogetherSchemeInfo togInfo = new Sports_TogetherSchemeInfo()
                    {
                        GameCode = gameCode,
                        GameType = gameType,
                        IssuseNumber = issuseNumber,
                        Amount = amount,
                        AnteCodeList = anteCodeList,
                        PlayType = playType,
                        SchemeSource = SchemeSource.Web,
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
                        ActivityType = (ActivityType)activityType,
                        BettingCategory = isexy ? SchemeBettingCategory.ErXuanYi : SchemeBettingCategory.GeneralBetting,
                        IsRepeat = IsRepeat,
                        CurrentBetTime = DateTime.Now,
                    };

                    var hmUseBalance = (guarantees + subscription) * 1M;
                    if (hmUseBalance > CurrentUserBalance.GetTotalCashMoney())
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", hmUseBalance) });
                    }

                    var hmResult = new CommonActionResult();
                    if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                    {
                        WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                        WebRedisHelper.CheckGameStatus(info.GameCode, anteCodeList.FirstOrDefault().GameType);
                        WebRedisHelper.CheckGeneralBettingMatch(info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);
                        var schemeId = WebRedisHelper.Redis_Bet_Together_Sports(new RedisBet_CreateSportsTogether
                        {
                            BalancePassword = balancepwd,
                            BetInfo = togInfo,
                            UserToken = UserToken
                        });
                        hmResult = new CommonActionResult
                        {
                            IsSuccess = true,
                            Message = "合买订单提交成功",
                            ReturnValue = string.Format("{0}|{1}", schemeId, togInfo.TotalMoney)
                        };
                    }
                    else
                    {
                        hmResult = WCFClients.GameClient.CreateSportsTogether(togInfo, balancepwd, UserToken);
                    }
                    //刷新余额
                    LoadUerBalance();

                    //var hmResult = WCFClients.GameClient.CreateSportsTogether(togInfo, balancepwd, UserToken);
                    //if (hmResult.IsSuccess)
                    //{
                    //    LoadUerBalance();
                    //    LoadUerBalance();

                    return Json(hmResult);
                }
                #endregion

                #region 普通投注


                //如果使用红包，则检测红包金额是否合格
                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }
                if (redBagMoney > 0)
                {
                    //红包余额
                    var rbTotal = CurrentUserBalance.RedBagBalance;
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "红包余额为0" });
                    }
                    if (redBagMoney > rbTotal)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("红包余额不足{0}元", redBagMoney) });
                    }
                    var ratio = 0M;
                    if (playType == "1_1" && gameCode.ToUpper() == "JCZQ")//JCZQ单关红包使用比例
                    {
                        ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis("jczqdg");
                    }
                    else
                    {
                        //查询可用红包比例
                        ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    }
                    //查询可用红包比例
                    //var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "当前彩种不允许使用红包" });

                    var maxRbMoney = totalMoney * ratio / 100;//该彩种可用金额
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }
                var useBalance = info.TotalMoney - redBagMoney;
                if (useBalance > CurrentUserBalance.GetTotalCashMoney())
                {
                    return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", useBalance) });
                }

                //如果是0表示免费保存订单
                var result = new CommonActionResult();
                if (kind == 0)
                {
                    result = WCFClients.GameClient.SaveOrderSportsBetting(info, UserToken);
                }
                else
                {
                    if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                    {
                        WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                        WebRedisHelper.CheckGameStatus(info.GameCode, anteCodeList.FirstOrDefault().GameType);
                        WebRedisHelper.CheckGeneralBettingMatch(info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);

                        var schemeId = WebRedisHelper.Redis_Bet_Sports(new RedisBet_SportsBetting
                        {
                            BalancePassword = balancepwd,
                            BetInfo = info,
                            RedBagMoney = redBagMoney,
                            UserToken = UserToken,
                        });
                        //var result = kind == 0 ? : WCFClients.GameClient.Sports_Betting(info, balancepwd, redBagMoney, UserToken);
                        result = new CommonActionResult
                        {
                            IsSuccess = true,
                            //Message = "订单提交成功，等待出票",
                            Message = "提交成功，请到我的投注记录里查看是否出票成功",
                            ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney)
                        };
                    }
                    else
                    {
                        result = WCFClients.GameClient.Sports_Betting(info, balancepwd, redBagMoney, UserToken);
                    }
                }

                LoadUerBalance();

                return Json(result);

                #endregion

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.IndexOf("【在线客服】") > 0)//为在线客服添加链接
                {
                    var configInfo = WCFClients.GameClient.QueryConfigByKey("Site.Service.KeFuUrl");
                    if (configInfo != null && !string.IsNullOrEmpty(configInfo.ConfigValue))
                    {
                        string msg = ex.Message.Replace("【在线客服】", "<a target='_blank' href='" + configInfo.ConfigValue + "'>【在线客服】</a>");
                        return Json(new { IsSuccess = false, Message = msg });
                    }
                }
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 保存订单购买
        /// </summary>
        /// <returns></returns>
        public JsonResult save_spotts()
        {
            try
            {
                var schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeId"], "订单编号不能为空");
                var balancePassword = string.IsNullOrEmpty(Request["bpwd"]) ? "" : Request["bpwd"];
                var result = WCFClients.GameClient.BettingUserSavedOrder(schemeId, balancePassword, 0M, base.UserToken);
                return Json(result);
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }


        //单式上传
        public JsonResult sing_sports()
        {
            try
            {
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["gameCode"], "彩种编码不能为空");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空");
                var playType = string.Empty;
                if ("CTZQ" != gameCode.ToUpper())
                {
                    playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空");//过关方式
                }
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];//期号
                var containsMatchId = string.IsNullOrEmpty(Request["upFlag"]) ? false : bool.Parse(Request["upFlag"]);
                string selectMatchId = !containsMatchId
                    ? PreconditionAssert.IsNotEmptyString(Request["selectMatchId"], "选择的比赛id不能为空")
                    : "";
                var allowCodes = PreconditionAssert.IsNotEmptyString(Request["allowCodes"], "允许投注的号码不能为空");
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];//资金密码

                var ser = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性;//方案保密性
                var antecode = "";//投注号码
                var amount = PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空");
                var totalMoney = PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空");
                var fileBuffer = Encoding.UTF8.GetBytes(Session["fileStream"].ToString());
                var activityType = (ActivityType)(string.IsNullOrEmpty(Request["activityType"]) ? 2 : int.Parse(Request["activityType"]));
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
                List<string> matchIdList = new List<string>();
                var checkresult = new List<string>();
                if ("CTZQ" == gameCode.ToUpper())
                {
                    checkresult = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(Session["fileStream"].ToString(), gameType.ToUpper(), allowCodes.Split(','), out matchIdList);
                    containsMatchId = false;
                }
                else
                {
                    checkresult = AnalyzerFactory.CheckSingleSchemeAnteCode(Session["fileStream"].ToString(), playType, containsMatchId, selectMatchId.Split(','), allowCodes.Split(','), out matchIdList);
                }
                var codeArray = checkresult;
                //投注号码对象
                var anteCodeList = new Sports_AnteCodeInfoCollection();
                if ("CTZQ" == gameCode.ToUpper())
                {
                    var contentList = checkresult.Select(item => string.Join(",", item.ToCharArray())).ToList();
                    var content = string.Join("|", contentList);
                    foreach (var item in codeArray)
                    {
                        var code = new Sports_AnteCodeInfo
                        {
                            IsDan = false,
                            //AnteCode = item,
                            AnteCode = content,
                            PlayType = playType
                        };

                        code.GameType = gameType;
                        anteCodeList.Add(code);
                    }
                }
                else
                {
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
                }

                SingleSchemeInfo info = new SingleSchemeInfo
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    PlayType = playType,
                    IssuseNumber = issuseNumber,
                    SelectMatchId = selectMatchId,
                    AllowCodes = allowCodes,
                    ContainsMatchId = containsMatchId,
                    SchemeSource = SchemeSource.Web,
                    Security = ser,
                    BettingCategory = SchemeBettingCategory.SingleBetting,
                    AnteCodeList = anteCodeList,
                    Amount = int.Parse(amount),
                    TotalMoney = decimal.Parse(totalMoney),
                    FileBuffer = fileBuffer,
                    ActivityType = activityType,
                    IsRepeat = IsRepeat

                };
                #region 合买old
                //if (isHemai)
                //{
                //    //合买属性
                //    var title = string.IsNullOrEmpty(Request["title"]) ? string.Empty : CheckReplaceNumber(Request["title"]);
                //    var desc = string.IsNullOrEmpty(Request["description"]) ? amount + "倍，共" + totalMoney + "元" : CheckReplaceNumber(Request["description"]);
                //    var totalCount = string.IsNullOrEmpty(Request["totalCount"]) ? Convert.ToInt32(totalMoney) : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                //    var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]); // 默认每份单价为1元
                //    var guarantees = string.IsNullOrEmpty(Request["guarantees"]) ? 0 : int.Parse(Request["guarantees"]); //我要保底份数
                //    var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                //    var subscription = string.IsNullOrEmpty(Request["subscription"]) ? 0 : int.Parse(Request["subscription"]); //我要认购份数
                //    var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"]) ? 0 : int.Parse(Request["bonusdeduct"]); //提成比例
                //    //合买
                //    SingleScheme_TogetherSchemeInfo togetherinfo = new SingleScheme_TogetherSchemeInfo
                //    {
                //        BettingInfo = info,
                //        Title = title,
                //        Description = desc,
                //        TotalCount = totalCount,
                //        TotalMoney = decimal.Parse(totalMoney),
                //        Price = price,
                //        Guarantees = guarantees,
                //        BonusDeduct = bonusdeduct,
                //        JoinPwd = joinpwd,
                //        Subscription = subscription
                //    };
                //    var hmresult = WCFClients.GameClient.CreateSingleSchemeTogether(togetherinfo, balancepwd, UserToken);
                //    if (hmresult.IsSuccess)
                //    {
                //        LoadUerBalance();
                //    }
                //    return Json(hmresult);
                //}
                #endregion

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
                    SingleScheme_TogetherSchemeInfo togetherinfo = new SingleScheme_TogetherSchemeInfo
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
                    var hmresult = WCFClients.GameClient.CreateSingleSchemeTogether(togetherinfo, balancepwd, UserToken);
                    if (hmresult.IsSuccess)
                    {
                        LoadUerBalance();
                    }
                    return Json(hmresult);
                }
                #endregion

                #region 普通投注old
                //var result = WCFClients.GameClient.SingleScheme(info, balancepwd, UserToken);
                //if (result.IsSuccess)
                //{
                //    LoadUerBalance();
                //}
                //return Json(result);                
                #endregion
                #region 普通投注
                //如果使用红包，则检测红包金额是否合格
                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }

                if (redBagMoney > 0)
                {
                    var rbTotal = CurrentUserBalance.RedBagBalance;//红包余额
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "可用红包金额不足." });
                    }
                    //查询可用红包比例
                    var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);// WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "可用红包金额不足.." });

                    var maxRbMoney = Convert.ToDecimal(totalMoney) * ratio / 100;//该彩种可用金额
                    //if (maxRbMoney > rbTotal)
                    //{
                    //    return Json(new { IsSuccess = false, Message = "可用红包金额不足..." });
                    //}
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }

                var result = WCFClients.GameClient.SingleScheme(info, balancepwd, Convert.ToDecimal(redBagMoney), UserToken);
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
                return Json(result);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //单式上传
        public JsonResult sing_sports1()
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

                var ser = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性;//方案保密性
                var antecode = "";//投注号码
                var amount = PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空");
                var totalMoney = PreconditionAssert.IsNotEmptyString(Request["totalMoney"], "投注金额不能为空");
                var fileBuffer = Encoding.UTF8.GetBytes(Session["fileStream"].ToString());
                var activityType = (ActivityType)(string.IsNullOrEmpty(Request["activityType"]) ? 2 : int.Parse(Request["activityType"]));
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
                List<string> matchIdList = new List<string>();
                //var checkresult = AnalyzerFactory.CheckSingleSchemeAnteCode(Session["fileStream"].ToString(), playType, containsMatchId, selectMatchId.Split(','), allowCodes.Split(','), out matchIdList);
                var checkresult = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(Session["fileStream"].ToString(), gameType, allowCodes.Split(','), out matchIdList);
                var codeArray = checkresult;
                //投注号码对象
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
                SingleSchemeInfo info = new SingleSchemeInfo
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    PlayType = playType,
                    IssuseNumber = issuseNumber,
                    SelectMatchId = selectMatchId,
                    AllowCodes = allowCodes,
                    ContainsMatchId = containsMatchId,
                    SchemeSource = SchemeSource.Web,
                    Security = ser,
                    BettingCategory = SchemeBettingCategory.SingleBetting,
                    AnteCodeList = anteCodeList,
                    Amount = int.Parse(amount),
                    TotalMoney = decimal.Parse(totalMoney),
                    FileBuffer = fileBuffer,
                    ActivityType = activityType,
                    IsRepeat = IsRepeat

                };
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
                    SingleScheme_TogetherSchemeInfo togetherinfo = new SingleScheme_TogetherSchemeInfo
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
                    var hmresult = WCFClients.GameClient.CreateSingleSchemeTogether(togetherinfo, balancepwd, UserToken);
                    if (hmresult.IsSuccess)
                    {
                        LoadUerBalance();
                    }
                    return Json(hmresult);
                }
                #endregion

                #region 普通投注
                //如果使用红包，则检测红包金额是否合格
                var rbMoneyStr = Request["rbmoney"];
                var redBagMoney = 0M;
                if (!string.IsNullOrEmpty(rbMoneyStr))
                {
                    try
                    {
                        redBagMoney = decimal.Parse(rbMoneyStr);
                    }
                    catch (Exception)
                    {
                        throw new Exception("红包金额输入不正确");
                    }
                }

                if (redBagMoney > 0)
                {
                    var rbTotal = CurrentUserBalance.RedBagBalance;//红包余额
                    if (rbTotal <= 0)
                    {
                        return Json(new { IsSuccess = false, Message = "可用红包金额不足." });
                    }
                    //查询可用红包比例
                    var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);// WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                    if (ratio <= 0)
                        return Json(new { IsSuccess = false, Message = "可用红包金额不足.." });

                    var maxRbMoney = Convert.ToDecimal(totalMoney) * ratio / 100;//该彩种可用金额
                    //if (maxRbMoney > rbTotal)
                    //{
                    //    return Json(new { IsSuccess = false, Message = "可用红包金额不足..." });
                    //}
                    if (redBagMoney > maxRbMoney)
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                    }
                }

                var result = WCFClients.GameClient.SingleScheme(info, balancepwd, Convert.ToDecimal(redBagMoney), UserToken);
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
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
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
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
                    IsRepeat = IsRepeat

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
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
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
                var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);

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
                    IsRepeat = IsRepeat,
                    SchemeSource = SchemeSource.Web
                };
                SingleScheme_TogetherSchemeInfo info = new SingleScheme_TogetherSchemeInfo
                {
                    BettingInfo = singleinfo,
                    TotalCount = totalCount,
                    TotalMoney = int.Parse(totalMoney),
                    Price = 1

                };
                var result = WCFClients.GameClient.XianFaQi_UpLoadScheme(schemeId, info, CurrentUser.LoginInfo.UserId);
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 奖金优化
        /// </summary>
        /// <returns></returns>
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
                ViewBag.OrgMoney = schemeCost / multiple;
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
            ViewBag.CurrentUser = CurrentUser;
            //ViewBag.IsInBetWhiteList = IsInBetWhiteList();
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
            var IsRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
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
                    BettingCategory = SchemeBettingCategory.YouHua,
                    TotalMoney = org * amount,
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
                    ActivityType = ActivityType.NoParticipate,
                    IsRepeat = IsRepeat,
                    CurrentBetTime = DateTime.Now,
                };
                try
                {
                    var hmUseBalance = (guarantees + subscription) * 1M;
                    if (hmUseBalance > CurrentUserBalance.GetTotalCashMoney())
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", hmUseBalance) });
                    }

                    var hmResult = new CommonActionResult();
                    //if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                    //{
                    //    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                    //    WebRedisHelper.CheckGameStatus(togInfo.GameCode, anteCodeList.FirstOrDefault().GameType);
                    //    WebRedisHelper.CheckGeneralBettingMatch(togInfo.GameCode, togInfo.GameType, togInfo.PlayType, togInfo.AnteCodeList, togInfo.IssuseNumber, togInfo.BettingCategory);
                    //    var schemeId = WebRedisHelper.Redis_Bet_Together_YouHua(new RedisBet_CreateYouHuaSchemeTogether
                    //    {
                    //        BalancePassword = balancepwd,
                    //        BetInfo = togInfo,
                    //        UserToken = UserToken,
                    //        RealTotalMoney = totalMoney
                    //    });
                    //    hmResult = new CommonActionResult
                    //    {
                    //        IsSuccess = true,
                    //        Message = "合买订单提交成功",
                    //        ReturnValue = string.Format("{0}|{1}", schemeId, totalMoney)
                    //    };
                    //}
                    //else
                    //{
                    //    hmResult = WCFClients.GameClient.CreateYouHuaSchemeTogether(togInfo, balancepwd, totalMoney, UserToken);
                    //}
                    hmResult = WCFClients.GameClient.CreateYouHuaSchemeTogether(togInfo, balancepwd, totalMoney, UserToken);

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
                ActivityType = ActivityType.NoParticipate,
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
                //TotalMoney = totalMoney,
                TotalMoney = org * amount,
                IsRepeat = IsRepeat,
                CurrentBetTime = DateTime.Now,
            };
            //如果使用红包，则检测红包金额是否合格
            var rbMoneyStr = Request["rbmoney"];
            var redBagMoney = 0M;
            if (!string.IsNullOrEmpty(rbMoneyStr))
            {
                try
                {
                    redBagMoney = decimal.Parse(rbMoneyStr);
                }
                catch (Exception)
                {
                    throw new Exception("红包金额输入不正确");
                }
            }

            if (redBagMoney > 0)
            {
                //红包余额
                var rbTotal = CurrentUserBalance.RedBagBalance;
                if (rbTotal <= 0)
                {
                    return Json(new { IsSuccess = false, Message = "红包余额为0" });
                }
                if (redBagMoney > rbTotal)
                {
                    return Json(new { IsSuccess = false, Message = string.Format("红包余额不足{0}元", redBagMoney) });
                }
                //查询可用红包比例
                var ratio = WebRedisHelper.QueryRedBagUseConfigFromRedis(gameCode);//WCFClients.ActivityClient.QueryRedBagUseConfigByGameCode(gameCode);
                if (ratio <= 0)
                    return Json(new { IsSuccess = false, Message = "当前彩种不允许使用红包" });

                var maxRbMoney = totalMoney * ratio / 100;//该彩种可用金额
                if (redBagMoney > maxRbMoney)
                {
                    return Json(new { IsSuccess = false, Message = string.Format("订单最多可使用红包{0}元....", maxRbMoney) });
                }
            }
            var useBalance = totalMoney - redBagMoney;
            if (useBalance > CurrentUserBalance.GetTotalCashMoney())
            {
                return Json(new { IsSuccess = false, Message = string.Format("用户余额不足{0}元", useBalance) });
            }

            try
            {
                var result = new CommonActionResult();
                //if (base.IsBetOrderToRedisList && WebRedisHelper.IsUserHasRealName(RealNameInfo))
                //{
                //    WebRedisHelper.CheckBalancePwd(CurrentUserBalance, balancepwd);
                //    WebRedisHelper.CheckGameStatus(opt.GameCode, anteCodeList.FirstOrDefault().GameType);
                //    WebRedisHelper.CheckGeneralBettingMatch(opt.GameCode, opt.GameType, opt.PlayType, opt.AnteCodeList, opt.IssuseNumber, opt.BettingCategory);
                //    var schemeId = WebRedisHelper.Redis_Bet_YouHua(new RedisBet_YouHuaBet
                //    {
                //        BalancePassword = balancepwd,
                //        BetInfo = opt,
                //        RealTotalMoney = totalMoney,
                //        RedBagMoney = redBagMoney,
                //        UserToken = UserToken,
                //    });
                //    result = new CommonActionResult
                //    {
                //        IsSuccess = true,
                //        Message = "订单提交成功，等待出票",
                //        ReturnValue = string.Format("{0}|{1}", schemeId, totalMoney)
                //    };
                //}
                //else
                //{
                //    result = WCFClients.GameClient.YouHuaBet(opt, balancepwd, totalMoney, redBagMoney, UserToken);
                //}
                result = WCFClients.GameClient.YouHuaBet(opt, balancepwd, totalMoney, redBagMoney, UserToken);

                LoadUerBalance();
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 方案快照
        /// </summary>
        /// <param name="schemeType"></param>
        /// <param name="schemeId"></param>
        /// <param name="gameCode"></param>
        /// <param name="userToken"></param>
        private void SendSchemeSnapshotToEmail(string schemeType, string schemeId, string gameCode, string userToken)
        {
            try
            {
                var siteUrl = ConfigurationManager.AppSettings["SelfDomain"] ?? "http://www.wancai.com";
                var schemeAddress = siteUrl + "/buy/SchemesSnapshoot?SchemeType=" + schemeType + "&schemeId=" + schemeId + "&GameCode=" + gameCode + "";//方案地址
                var emailInfo = WCFClients.ExternalClient.GetMyEmailInfo(userToken);//获取邮箱信息
                if (emailInfo == null || string.IsNullOrEmpty(emailInfo.Email))
                    throw new Exception("发送快照失败,未查询到邮箱地址！");
                var emailSmtp = WCFClients.GameClient.QueryCoreConfigByKey("Email.Smtp").ConfigValue;
                var emailAccount = WCFClients.GameClient.QueryCoreConfigByKey("Email.Account").ConfigValue;
                var emailDisplayName = WCFClients.GameClient.QueryCoreConfigByKey("Email.DisplayName").ConfigValue;
                var emailPassword = WCFClients.GameClient.QueryCoreConfigByKey("Email.Password").ConfigValue;
                var emailTitle = WCFClients.GameClient.QueryCoreConfigByKey("Email.Title").ConfigValue;
                if (string.IsNullOrEmpty(emailSmtp) || string.IsNullOrEmpty(emailAccount) || string.IsNullOrEmpty(emailDisplayName) || string.IsNullOrEmpty(emailPassword) || string.IsNullOrEmpty(emailTitle))
                    throw new Exception("发送快照失败，配置信息不完整！");
                SendSchemeSnapshot snapshot = new SendSchemeSnapshot();
                snapshot.SendSchemeSnapshotToEmail(schemeAddress, emailInfo.Email, emailSmtp, emailAccount, emailDisplayName, emailPassword, emailTitle);

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

        /// <summary>
        /// 单式上传
        /// </summary>
        /// <returns></returns>
        public ContentResult Upload()
        {

            try
            {
                Session["fileStream"] = null; //清空文件流
                var fileObj = Request.Files["upload_file"];
                if (fileObj == null || fileObj.ContentLength <= 0) throw new Exception("上传文件不能为空");
                const int FILE_MAX_LENGTH = 100 * 1024; //限制文件大小为1MB
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

        #region 购彩大厅

        //public ActionResult GouCaiHall()
        //{
        //    return View();
        //}

        #endregion

        #region  帮助中心
        public ActionResult Assist()
        {
            return View();
        }
        #endregion

        #region  走势图
        public ActionResult Tendency()
        {
            return View();
        }
        #endregion

        #region  手机购彩
        public ActionResult PhoneBuy()
        {
            return View();
        }
        #endregion

        #region 代理推广

        public RedirectResult svip(string id)
        {
            string _decodeUserId = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    _decodeUserId = WCFClients.GameClient.DecodeBase64(id);
                    Session["pid"] = _decodeUserId;
                }
                catch { }
            }
            return Redirect("/");
        }

        #endregion


        #region UCenter

        ///// <summary>
        ///// 查询用户信息
        ///// </summary>
        //public ContentResult QueryInfo()
        //{
        //    var userName = Request["userName"];
        //    if (string.IsNullOrEmpty(userName))
        //        return Content("username is null");
        //    var client = new DS.Web.UCenter.Client.UcClient();
        //    var r = client.UserInfo(userName);
        //    return Content(string.Format("Success:{0},Uid:{1},UserName:{2},Mail:{3}", r.Success, r.Uid, r.UserName, r.Mail));
        //}

        ///// <summary>
        ///// 注册到ucenter
        ///// </summary>
        //public ContentResult Registe()
        //{
        //    var userName = Request["userName"];
        //    if (string.IsNullOrEmpty(userName))
        //        return Content("username is null");
        //    var pwd = Request["pwd"];
        //    if (string.IsNullOrEmpty(pwd))
        //        return Content("pwd is null");


        //    userName = HttpContext.Server.UrlEncode(userName);

        //    var email = "test@163.com";// string.Format("{0}@126.com", userName);
        //    var client = new DS.Web.UCenter.Client.UcClient();
        //    var r = client.UserRegister(userName, pwd, email);

        //    //模拟登录激活用户
        //    var url = "http://bbs.wancai.com/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1";
        //    var a = PostManager.Post(url, string.Format("fastloginfield=username&username={0}&password={1}&quickforward=yes&handlekey=ls", userName, pwd), Encoding.GetEncoding("gbk"));

        //    return Content(string.Format("Result:{0},Uid:{1},activity:{2}", r.Result, r.Uid, a));
        //}

        ///// <summary>
        ///// 登录
        ///// </summary>
        //public ContentResult Login()
        //{
        //    var userName = Request["userName"];
        //    if (string.IsNullOrEmpty(userName))
        //        return Content("username is null");
        //    var pwd = Request["pwd"];
        //    if (string.IsNullOrEmpty(pwd))
        //        return Content("pwd is null");
        //    var client = new DS.Web.UCenter.Client.UcClient();
        //    var r = client.UserLogin(userName, pwd);
        //    return Content(string.Format("Success:{0},HasSameName:{1},Mail:{2},PassWord:{3},Result:{4},Uid:{5},UserName:{6}", r.Success, r.HasSameName, r.Mail, r.PassWord, r.Result, r.Uid, r.UserName));
        //}

        ///// <summary>
        ///// 更新用户
        ///// </summary>
        //public ContentResult Update()
        //{
        //    var userName = Request["userName"];
        //    if (string.IsNullOrEmpty(userName))
        //        return Content("username is null");
        //    var pwd = Request["pwd"];
        //    if (string.IsNullOrEmpty(pwd))
        //        return Content("pwd is null");
        //    var client = new DS.Web.UCenter.Client.UcClient();
        //    var r = client.UserEdit(userName, string.Empty, pwd, string.Empty, ignoreOldPw: true);
        //    return Content(string.Format("Result:{0}", r.Result));
        //}

        /// <summary>
        /// 同步登录
        /// </summary>
        public ActionResult syncLogin()
        {
            try
            {
                //if (this.CurrentUser == null || this.CurrentUser.LoginInfo == null || string.IsNullOrEmpty(this.CurrentUser.LoginInfo.DisplayName))
                //    throw new Exception("请先登录");
                //var client = new DS.Web.UCenter.Client.UcClient();
                //var userName = HttpContext.Server.UrlEncode(this.CurrentUser.LoginInfo.DisplayName);
                //var bbsInfo = client.UserInfo(userName);
                //if (!bbsInfo.Success || bbsInfo.Uid <= 0)
                //    throw new Exception("查询用户信息失败");

                //var html = client.UserSynlogin(bbsInfo.Uid);
                //ViewBag.Html = html;
                //ViewBag.BBS = BBS_Url;
                //ViewBag.Error = "";

                throw new Exception("查询用户信息失败");

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Html = "";
            }

            return View();
        }

        //public ContentResult AuthCode()
        //{
        //    var code=Request["code"];
        //    var str = DS.Web.UCenter.UcUtility.AuthCodeDecode(code);
        //    return Content(str);
        //}

        public string BBS_Url
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BBS_URL"];
            }
        }

        #endregion



        public ActionResult WorldCup(string id)
        {
            id = string.IsNullOrEmpty(id) ? "hh" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            //是否是方案欲投
            var schemeId = string.IsNullOrEmpty(Request["schemeid"]) ? "" : Request["schemeid"];
            var isYt = !string.IsNullOrEmpty(Request["isYt"]) && Convert.ToBoolean(int.Parse(Request["isYt"].ToLower()));
            if (isYt)
            {
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(schemeId);
                ViewBag.Money = info.TotalMoney;
                ViewBag.UserId = info.UserId;
            }
            else
            {
                ViewBag.Money = 0M;
                ViewBag.UserId = "";
            }
            var oddtype = string.IsNullOrEmpty(Request["oddstype"]) ? 2 : int.Parse(Request["oddstype"]);
            ViewBag.OddType = oddtype;
            ViewBag.SchemeId = schemeId;
            return View();
        }

    }
}
