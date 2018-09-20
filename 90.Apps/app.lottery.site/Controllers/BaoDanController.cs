using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using app.lottery.site.Models;
using Common.Communication;
using Common.Utilities;
using GameBiz.Core;

namespace app.lottery.site.cbbao.Controllers
{
    public class BaoDanController : BaseController
    {
        public ActionResult Rdheader()
        {
            return View();
        }
        #region 神单分享

        /// <summary>
        /// 今日分享
        /// </summary>
        /// <returns></returns>
        public ActionResult redantoday()
        {
            try
            {
                ViewBag.UserName = string.IsNullOrEmpty(Request.QueryString["username"]) ? "" : Request.QueryString["username"];
                //ViewBag.Userid = string.IsNullOrEmpty(Request.QueryString["userid"]) ? "" : Request.QueryString["userid"];
                ViewBag.Follow = string.IsNullOrEmpty(Request.QueryString["follow"]) ? "" : CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId;
                ViewBag.Rebatesrate = string.IsNullOrEmpty(Request.QueryString["Rebatesrate"]) ? "" : Request.QueryString["Rebatesrate"];
                ViewBag.CopyMoneyCount = string.IsNullOrEmpty(Request.QueryString["CopyMoneyCount"]) ? "" : Request.QueryString["CopyMoneyCount"];
                ViewBag.CopypeopleCount = string.IsNullOrEmpty(Request.QueryString["CopypeopleCount"]) ? "" : Request.QueryString["CopypeopleCount"];
                var strorderby = ViewBag.Rebatesrate != "" ? ViewBag.Rebatesrate : ViewBag.CopyMoneyCount != "" ? ViewBag.CopyMoneyCount : ViewBag.CopypeopleCount != "" ? ViewBag.CopypeopleCount : "";
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
                //ViewBag.TodayBdfxList = WCFClients.ExternalClient.QueryTodayBDFXList("", ViewBag.UserName, "", strorderby, ViewBag.Follow, DateTime.Now, DateTime.Now, "", ViewBag.pageNo, ViewBag.PageSize);

                //从jsonData读取数据
                ViewBag.TodayBdfxList = QueryTodayBDFXList(ViewBag.UserName, strorderby, ViewBag.Follow, ViewBag.pageNo, ViewBag.PageSize);

                //ViewBag.dayNR  = WCFClients.ExternalClient.QueryYesterdayNR(DateTime.Now, DateTime.Now,3);
                //关注列表查询
                ViewBag.AttenList = WCFClients.ExternalClient.QueryBDFXAllAttentionIList();

                //从jsonData读取数据
                //ViewBag.dayNR = QueryYesterdayNR();
                ViewBag.User = CurrentUser;
            }
            catch
            {
                ViewBag.TodayBdfxList = new TotalSingleTreasure_Collection();
            }

            return View();
        }

        public ActionResult nrandrecommend()
        {
            //专家推荐
            int queryCount = 6;
            int expertType = 20;
            ViewBag.Expert = WCFClients.ExternalClient.QueryWebUserSchemeShareExpertList(queryCount, expertType);
            //昨日牛人
            ViewBag.dayNR = WCFClients.ExternalClient.QueryNRRankList(DateTime.Now, DateTime.Now, 3);
            return View();
        }
        public PartialViewResult LoadYesterdayNR()
        {
            //从jsonData读取数据
            ViewBag.dayNR = QueryYesterdayNR();
            return PartialView();
        }

        /// <summary>
        /// 我的关注
        /// </summary>
        /// <returns></returns>
        public ActionResult redanmyattention()
        {
            try
            {
                DateTime dt = DateTime.Now; //当前时间

                int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
                DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));   //本周周一
                DateTime endWeek = startWeek.AddDays(6);  //本周周日
                ViewBag.NowWeek = startWeek.ToString("MM.dd") + "-" + endWeek.ToString("MM.dd");
                ViewBag.UpNowWeek = startWeek.AddDays(-7).ToString("MM.dd") + "-" + endWeek.AddDays(-7).ToString("MM.dd");
                var week = string.IsNullOrEmpty(Request.QueryString["week"]) ? "" : Request.QueryString["week"];
                ViewBag.Week = string.IsNullOrEmpty(Request.QueryString["week"]) ? "" : Request.QueryString["week"];
                ViewBag.User = CurrentUser;
                var s = "";
                var e = "";
                if (week != null && week != "")
                {
                    s = week.Split('-')[0];
                    e = week.Split('-')[1];
                }
                ViewBag.RankList = WCFClients.ExternalClient.QueryGSRankList(s == "" ? startWeek.ToString("MM.dd") : s, e == "" ? endWeek.ToString("MM.dd") : e, CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId, "true");
            }
            catch (Exception ex)
            {

                ViewBag.RankList = null;
            }

            return View();
        }

        /// <summary>
        /// 高手排行
        /// </summary>
        /// <returns></returns>
        public ActionResult redanrank()
        {
            try
            {
                DateTime dt = DateTime.Now; //当前时间
                int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
                DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));   //本周周一
                DateTime endWeek = startWeek.AddDays(6);  //本周周日
                ViewBag.User = CurrentUser;
                ViewBag.NowWeek = startWeek.ToString("MM.dd") + "-" + endWeek.ToString("MM.dd");
                ViewBag.UpNowWeek = startWeek.AddDays(-7).ToString("MM.dd") + "-" + endWeek.AddDays(-7).ToString("MM.dd");
                var week = string.IsNullOrEmpty(Request.QueryString["week"]) ? "" : Request.QueryString["week"];
                ViewBag.Week = string.IsNullOrEmpty(Request.QueryString["week"]) ? "" : Request.QueryString["week"];
                var s = "";
                var e = "";
                if (week != null && week != "")
                {
                    s = week.Split('-')[0];
                    e = week.Split('-')[1];
                }

                ViewBag.RankList = WCFClients.ExternalClient.QueryGSRankList(s == "" ? startWeek.ToString("MM.dd") : s, e == "" ? endWeek.ToString("MM.dd") : e, CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId, "");
            }
            catch (Exception ex)
            {

                ViewBag.RankList = null;
            }

            return View();
        }

        /// <summary>
        /// 我的分享
        /// </summary>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult redanmy()
        {
            //try
            //{
            //    ViewBag.Current = CurrentUser;
            //   // ViewBag.Follow = string.IsNullOrEmpty(Request.QueryString["follow"]) ? "" : Request.QueryString["follow"];
            //    ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            //    ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
            //    ViewBag.TodayBdfxList = WCFClients.ExternalClient.QueryTodayBDFXList(CurrentUser.LoginInfo.UserId, "", "", "", "", DateTime.Parse("2015-06-06"), DateTime.Now, "1", ViewBag.pageNo, ViewBag.PageSize);
            //}
            //catch (Exception ex)
            //{

            //    ViewBag.TodayBdfxList = new TotalSingleTreasure_Collection();
            //}

            try
            {
                ViewBag.Follow = string.IsNullOrEmpty(Request.QueryString["follow"]) ? "" : Request.QueryString["follow"];
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.TodayBdfxList = WCFClients.ExternalClient.QueryTodayBDFXList(CurrentUser.LoginInfo.UserId, "", "", "", "", DateTime.Parse("2015-06-06"), DateTime.Now, "1", ViewBag.pageNo, ViewBag.PageSize);
            }
            catch (Exception ex)
            {

                ViewBag.TodayBdfxList = new TotalSingleTreasure_Collection();
            }
            return View();
        }

        /// <summary>
        /// 作者主页
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult redanauthorhome(string id)
        {
            try
            {
                var user = CurrentUser;
                var lastweekProfitRate = string.IsNullOrEmpty(Request["LastweekProfitRate"]) ? "" : Request["LastweekProfitRate"];
                ViewBag.LastweekProfitRate = lastweekProfitRate;
                ViewBag.User = user;
                ViewBag.userid = string.IsNullOrEmpty(id) ? user.LoginInfo.UserId : id;
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.resultinfo = WCFClients.ExternalClient.QueryConcernedByUserId(ViewBag.userid, user == null ? "" : user.LoginInfo.UserId, "", "");
                ViewBag.filter = string.IsNullOrEmpty(Request["filter"]) ? "" : Request["filter"];
                ViewBag.UserImg = Request["uimg"];
                ViewBag.UserLvl = Request["ulvl"];
                var filter = ViewBag.filter;
                var award = "";
                if (filter != "" && filter == "buy")
                {
                    ViewBag.HomePage = WCFClients.ExternalClient.QueryBDFXAutherHomePage(ViewBag.userid, "", DateTime.Now.ToString(), ViewBag.pageNo, ViewBag.PageSize);
                }
                else
                {
                    if (filter == "award")
                    {
                        award = "1";
                    }
                    else if (filter == "noaward")
                    {
                        award = "0";
                    }
                    ViewBag.HomePage = WCFClients.ExternalClient.QueryBDFXAutherHomePage(ViewBag.userid, award, "", ViewBag.pageNo, ViewBag.PageSize);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        /// <summary>
        /// 投注确认
        /// </summary>
        /// <returns></returns>
        public ActionResult redanbuyconfirm()
        {
            try
            {
                var SchemeId = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
                var amount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["WagerMultiple"], "投注倍数不能为空。"));
                var detail = WCFClients.ExternalClient.QueryBDFXOrderDetailBySchemeId(SchemeId);
                ViewBag.Amount = amount;
                ViewBag.BdDetails = detail;
                ViewBag.TotalMoney = amount * detail.CurrentBetMoney;


            }
            catch (Exception ex)
            {

                ViewBag.BdDetails = new BDFXOrderDetailInfo();
            }

            return View();
        }

        /// <summary>
        /// 抄单
        /// </summary>
        /// <returns></returns>
        public JsonResult Sports_BettingAndChase_BDFX()
        {
            try
            {
                var amount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["amount"], "投注倍数不能为空。"));
                var totalMoney = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["TotalMoney"], "投注金额不能为空。"));
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["GameCode"], "彩种编码不能为空");
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空。");
                var gameType = PreconditionAssert.IsNotEmptyString(Request["gameType"], "玩法不能为空。");
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                var antecode = PreconditionAssert.IsNotEmptyString(Request["AnteCodeList"], "投注号码不能为空。");
                var matchcount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["TotalMatchCount"], "投注场数不能为空。"));
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];
                var sercu = (TogetherSchemeSecurity)(string.IsNullOrEmpty(Request["Security"]) ? 6 : int.Parse(Request["Security"])); //方案保密性
                var singleTreasureDeclaration = string.IsNullOrEmpty(Request["SingleTreasureDeclaration"]) ? "" : Request["SingleTreasureDeclaration"];
                var commission = string.IsNullOrEmpty(Request["Commission"]) ? 0 : decimal.Parse(Request["Commission"]);
                var schemeId = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
                var isRepeat = string.IsNullOrEmpty(Request["IsRepeat"]) ? false : Boolean.Parse(Request["IsRepeat"]);
                var isexy = string.IsNullOrEmpty(Request["isExy"]) ? false : Boolean.Parse(Request["isExy"]);
                var activityType = 2;
                if (gameCode.ToUpper() == "JCZQ" || gameCode.ToUpper() == "BJDC")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }
                #region 投注号码

                var bfCount = 0;
                var bqcCount = 0;
                var sfcCount = 0;

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


                #endregion

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
                    BDFXCommission = commission,
                    SingleTreasureDeclaration = singleTreasureDeclaration,
                    BDFXSchemeId = schemeId,
                    IsRepeat = isRepeat,
                };

                var fxResult = WCFClients.GameClient.Sports_BettingAndChase_BDFX(fxinfo, balancepwd, CurrentUser.LoginInfo.UserId);
                if (fxResult.IsSuccess)
                {
                    LoadUerBalance();
                }
                return Json(fxResult);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }

        }

        public PartialViewResult buyneedPassword()
        {
            var needPassword = CurrentUserBalance.CheckIsNeedPassword("Bet");
            ViewBag.NeedPassword = needPassword;
            return PartialView();
        }

        /// <summary>
        /// 神单分享前言
        /// </summary>
        /// <returns></returns>
        public ActionResult redan()
        {
            ViewBag.Current = CurrentUser;
            return View();
        }
        /// <summary>
        /// 我的神单集合
        /// </summary>
        /// <returns></returns>
        public JsonResult myredanlist()
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 悬停查看用户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult lotterywagercard()
        {

            var userid = Request["userId"];
            DateTime dt = DateTime.Now; //当前时间
            int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
            DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));   //本周周一
            DateTime endWeek = startWeek.AddDays(6);  //本周周日

            //var result = WCFClients.ExternalClient.QueryConcernedByUserId(userid, CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId, startWeek.ToString("MM.dd"), endWeek.ToString("MM.dd"));

            //从jsonData读取
            var result = QueryConcernedByUserId(userid, CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId, startWeek, endWeek);


            // object[] obj = { 0, 1, 2, null, 0, null, 0, 0, -1, -2, 0 };
            object IsGz = "";
            if (CurrentUser != null && CurrentUser.LoginInfo != null)
            {
                if (CurrentUser.LoginInfo.UserId == userid)
                {
                    IsGz = -1;
                }
                else
                {
                    IsGz = result.IsGZ;
                }
            }
            else
            {
                IsGz = result.IsGZ;
            }
            List<decimal> list = new List<decimal>();
            if (result.NearTimeProfitRateCollection != null && result.NearTimeProfitRateCollection.NearTimeProfitRateList != null)
            {
                foreach (var item in result.NearTimeProfitRateCollection.NearTimeProfitRateList)
                {
                    list.Add(decimal.Parse((item.CurrProfitRate * 100).ToString("N0")));
                }
            }
            else
            {
                list = new List<decimal>();
            }

            return Json(new { wager_card_response = new { userId = result.UserId, userName = result.UserName, prevRank = result.RankNumber, wagerNum = result.SingleTreasureCount, buyNum = 0, totalPrize = 0, atteNum = result.ConcernedUserCount, fansNum = result.BeConcernedUserCount, rate = list, isAtten = IsGz } }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 关注
        /// </summary>
        /// <returns></returns>
        public JsonResult BDFXAttention()
        {
            try
            {
                var bgzUserid = Request["bgzuserId"];
                var type = Request["type"];
                try
                {
                    DateTime dt = DateTime.Now; //当前时间
                    int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
                    DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));   //本周周一
                    DateTime endWeek = startWeek.AddDays(6);  //本周周日
                    var strUserIds = CurrentUser.LoginInfo.UserId + "|" + bgzUserid;
                    this.BuildConcerned(bgzUserid, CurrentUser.LoginInfo.UserId, startWeek, endWeek);
                }
                catch { }
                if (type != null && type == "createFollow")
                {
                    var result = WCFClients.ExternalClient.BDFXAttention(CurrentUser.LoginInfo.UserId, bgzUserid);
                    return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message, ReturnValue = result.ReturnValue });
                }
                else
                {
                    var result = WCFClients.ExternalClient.BDFXCancelAttention(CurrentUser.LoginInfo.UserId, bgzUserid);
                    return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message, ReturnValue = result.ReturnValue });
                }
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Msg = ex.Message });
            }


        }

        /// <summary>
        /// 神单详情页
        /// </summary>
        /// <returns></returns>
        public PartialViewResult bddetailsbody()
        {

            ViewBag.SchemeId = string.IsNullOrEmpty(Request.QueryString["SchemeId"]) ? "" : Request.QueryString["SchemeId"];
            ViewBag.User = CurrentUser;
            var result = WCFClients.ExternalClient.QueryBDFXOrderDetailBySchemeId(ViewBag.SchemeId);
            if (result != null)
            {
                ViewBag.BdDetails = result;
            }
            else
            {
                ViewBag.BdDetails = new BDFXOrderDetailInfo();
            }
            return PartialView();
        }


        /// <summary>
        /// 票数据
        /// </summary>
        /// <returns></returns>
        public PartialViewResult SportsTicket()
        {
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"]);
            ViewBag.UserId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
            ViewBag.schemeStatus = string.IsNullOrEmpty(Request["status"]) ? "" : Request["status"];

            var id = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
            ViewBag.SchemeId = id;
            ViewBag.SportsTicket = WCFClients.GameClient.QuerySportsTicketList(id, ViewBag.PageIndex, ViewBag.PageSize, UserToken);
            return PartialView();
        }
        #endregion

    }
}
