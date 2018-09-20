using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using Common.Utilities;
using GameBiz.Core;

namespace app.lottery.site.cbbao.Controllers
{
    [CheckBrowser]
    public class OneFixController : BaseController
    {
        /// <summary>
        /// 单关专场
        /// </summary>
        public ActionResult Jcdgzt(string id)
        {
            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
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
            //var result = WCFClients.ExternalClient.QueryPopularitysList();
            //ViewBag.Result = result;
            return View();
        }
        /// <summary>
        /// 单关高手
        /// </summary>
        public ActionResult Jcdg()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
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
            //var result = WCFClients.ExternalClient.QueryPopularitysList();
            //ViewBag.Result = result;
            return View();
        }
        /// <summary>
        /// 篮球
        /// </summary>
        public ActionResult Lcdg(string id)
        {
            id = string.IsNullOrEmpty(id) ? "hh" : id.ToLower();
            ViewBag.CurrentUserBalance = CurrentUserBalance; 
            ViewBag.Type = id;
            return View();
        }
        public PartialViewResult dgzc(string type)
        {
            return PartialView("dgzc/" + type);
        }
        /// <summary>
        /// 
        /// </summary>
        public JsonResult Jcdg_sports()
        {
            try
            {
                #region Request参数

                var antecode = PreconditionAssert.IsNotEmptyString(Request["antecode"], "投注号码不能为空。");
                var gameCode = PreconditionAssert.IsNotEmptyString(Request["game"], "彩种编码不能为空");
                var isHemai = string.IsNullOrEmpty(Request["isHemai"]) ? false : bool.Parse(Request["isHemai"]);
                var balancepwd = string.IsNullOrEmpty(Request["balancepwd"]) ? "" : Request["balancepwd"];
                var sercu =
                    (TogetherSchemeSecurity)
                        (string.IsNullOrEmpty(Request["sercurity"]) ? 4 : int.Parse(Request["sercurity"])); //方案保密性
                var kind = string.IsNullOrEmpty(Request["kind"]) ? 2 : int.Parse(Request["kind"]); //方案投注类型
                var activityType = 2;
                if (gameCode.ToUpper() == "JCZQ" || gameCode.ToUpper() == "BJDC")
                {
                    activityType = PreconditionAssert.IsInt32(Request["activityType"], "活动类型不能为空");
                    if (activityType != 0 && activityType != 1 && activityType != 2) activityType = 2;
                }
                var matchcount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["matchcount"], "投注场数不能为空。"));
                var issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
                var playType = PreconditionAssert.IsNotEmptyString(Request["playType"], "过关方式不能为空");

                #endregion
                //投注号码对象

                var codeArray = antecode.Split('#');
                var list = new List<string>();
                foreach (var item in codeArray)
                {
                    var anteCodeList = new Sports_AnteCodeInfoCollection();
                    var cods = item.Split('|');
                    var code = new Sports_AnteCodeInfo()
                    {
                        IsDan = bool.Parse(cods[0]),
                        MatchId = cods[1],
                        AnteCode = cods[2],
                        GameType = cods[3],
                        PlayType = playType
                    };

                    var gameType = cods[3];
                    var totalMoney = int.Parse(cods[4]);
                    var amount = totalMoney / 2;

                    anteCodeList.Add(code);

                    #region 合买投注

                    if (isHemai)
                    {
                        //合买属性
                        var title = Request["title"];
                        var desc = string.IsNullOrEmpty(Request["description"])
                            ? amount + "倍，共" + totalMoney + "元"
                            : Request["description"];
                        var totalCount = string.IsNullOrEmpty(Request["totalCount"])
                            ? Convert.ToInt32(totalMoney)
                            : int.Parse(Request["totalCount"]); // 默认份数为方案金额
                        var price = string.IsNullOrEmpty(Request["price"]) ? 1 : int.Parse(Request["price"]);
                        // 默认每份单价为1元
                        var guarantees = string.IsNullOrEmpty(Request["guarantees"])
                            ? 0
                            : int.Parse(Request["guarantees"]); //我要保底份数
                        var joinpwd = string.IsNullOrEmpty(Request["joinpwd"]) ? "" : Request["joinpwd"]; //认购密码
                        var subscription = string.IsNullOrEmpty(Request["subscription"])
                            ? 0
                            : int.Parse(Request["subscription"]); //我要认购份数
                        var bonusdeduct = string.IsNullOrEmpty(Request["bonusdeduct"])
                            ? 0
                            : int.Parse(Request["bonusdeduct"]); //提成比例

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

                        };
                        var hmResult = WCFClients.GameClient.CreateSportsTogether(togInfo, balancepwd, UserToken);
                        if (hmResult.IsSuccess)
                        {
                            list.Add(hmResult.ReturnValue);
                        }
                        //  return Json(hmResult);
                    }

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
                        ActivityType = (ActivityType)activityType,
                        BettingCategory = SchemeBettingCategory.GeneralBetting,
                        SchemeProgress = TogetherSchemeProgress.Finish
                    };

                    var result = kind == 0
                        ? WCFClients.GameClient.SaveOrderSportsBetting(info, UserToken)
                        : WCFClients.GameClient.Sports_BettingAndChase(info, balancepwd, 0M, UserToken);
                    if (result.IsSuccess)
                    {
                        list.Add(result.ReturnValue);
                    }

                    #endregion
                }
                return Json(new { IsSuccess = true, ReturnValue = list });

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }


        public PartialViewResult FixHeader(string id)
        {
            id = string.IsNullOrEmpty(id) ? "spf" : id.ToLower();
            ViewBag.Type = id;
            ViewBag.CurrentUser = CurrentUser; 
            ViewBag.CurrentUserBalance = CurrentUserBalance;

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
            return PartialView();
        }

    }
}
