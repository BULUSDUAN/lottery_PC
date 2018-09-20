using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Communication;
using GameBiz.Core;
using Common.Utilities;

namespace app.lottery.site.Controllers
{
    public class HemaiController : BaseController
    {
        /// <summary>
        /// 合买首页
        /// </summary>
        //public ActionResult Index__(string id)
        //{
        //    try
        //    {
        //        ViewBag.User = CurrentUser;
        //        ViewBag.Game = string.IsNullOrEmpty(id) ? "" : id;
        //        ViewBag.GameType = string.IsNullOrEmpty(Request["PlayType"]) ? "" : Request["PlayType"];
        //        ViewBag.IsMine = string.IsNullOrEmpty(Request["isMine"]) ? "false" : Request["isMine"];
        //        ViewBag.issuseNumber = string.IsNullOrEmpty(Request["issuseNumber"]) ? "" : Request["issuseNumber"];
        //        //最低金额-最大金额
        //        ViewBag.minMoney = string.IsNullOrEmpty(Request["minMoney"]) ? -1 : decimal.Parse(Request["minMoney"]);
        //        ViewBag.maxMoney = string.IsNullOrEmpty(Request["maxMoney"]) ? -1 : decimal.Parse(Request["maxMoney"]);
        //        //最小进度-最大进度
        //        ViewBag.minProgress = string.IsNullOrEmpty(Request["minProgress"]) ? -1 : decimal.Parse(Request["minProgress"]);
        //        ViewBag.maxProgress = string.IsNullOrEmpty(Request["maxProgress"]) ? -1 : decimal.Parse(Request["maxProgress"]);
        //        //合买方案保密性 0未知
        //        ViewBag.SchemeSecurity = string.IsNullOrEmpty(Request["SchemeSecurity"]) ? null : (TogetherSchemeSecurity?)int.Parse(Request["SchemeSecurity"]);
        //        //方案投注类别 0普通
        //        ViewBag.SchemeBetting = string.IsNullOrEmpty(Request["SchemeBetting"]) ? null : (SchemeBettingCategory?)int.Parse(Request["SchemeBetting"]);
        //        //合买方案进度
        //        ViewBag.SchemeProgress = string.IsNullOrEmpty(Request["SchemeProgress"]) ? null : (TogetherSchemeProgress?)int.Parse(Request["SchemeProgress"]);
        //        //排序
        //        //ViewBag.orderBy = string.IsNullOrEmpty(Request["orderBy"]) ? "" : Request["orderBy"];
        //        ViewBag.orderByName = string.IsNullOrEmpty(Request["orderByName"]) ? "" : Request["orderByName"];
        //        ViewBag.orderBySort = string.IsNullOrEmpty(Request["orderBySort"]) ? "" : Request["orderBySort"];
        //        //保底和进度
        //        var orderBy = "";
        //        if (ViewBag.orderByName == "0")
        //            orderBy = "ISTOP DESC,ProgressStatus ASC, Progress " + ViewBag.orderBySort + ",TotalMoney DESC";
        //        else if (ViewBag.orderByName == "1")
        //            orderBy = "ISTOP DESC,ProgressStatus ASC,TotalMoney " + ViewBag.orderBySort + ", Progress DESC";
        //        //关键字
        //        var searchKey = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"];
        //        if (ViewBag.IsMine == "true" && CurrentUser != null)
        //        {
        //            searchKey = CurrentUser.LoginInfo.DisplayName;
        //        }
        //        ViewBag.key = searchKey;
        //        ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
        //        ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 30 : int.Parse(Request.QueryString["pageSize"]);
        //        string userId = string.Empty;
        //        if (CurrentUser != null)
        //            userId = CurrentUser.LoginInfo.UserId;
        //        ViewBag.SuperList = WCFClients.GameClient.QueryHotUserTogetherOrderList(userId);
        //        ViewBag.TogList = WCFClients.GameClient.QuerySportsTogetherList(searchKey, ViewBag.issuseNumber, ViewBag.Game, ViewBag.GameType, ViewBag.SchemeSecurity,
        //            ViewBag.SchemeBetting, ViewBag.SchemeProgress, ViewBag.minMoney, ViewBag.maxMoney, ViewBag.minProgress,
        //            ViewBag.maxProgress, orderBy, ViewBag.pageNo, ViewBag.PageSize,userId);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.TogList = new Sports_TogetherSchemeQueryInfoCollection();
        //    }
        //    return View();
        //}

        /// <summary>
        /// 自动跟单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Doclist(string id)
        {
            ViewBag.Game = string.IsNullOrEmpty(id) ? "jczq" : id.ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["type"]) ? "spf" : Request["type"].ToLower();
            ViewBag.User = CurrentUser;
            string key = string.IsNullOrEmpty(Request["userName"]) ? "" : Request["userName"];
            ViewBag.UserName = key;
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? PageIndex : int.Parse(Request["pageindex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["PageSize"]) ? PageSize : int.Parse(Request["PageSize"]);
            ViewBag.orderByProperty = string.IsNullOrEmpty(Request["orderByProperty"]) ? 3 : int.Parse(Request["orderByProperty"]);
            ViewBag.orderByCategory = string.IsNullOrEmpty(Request["orderByCategory"]) ? 0 : int.Parse(Request["orderByCategory"]);
            ViewBag.BeedingList = WCFClients.GameClient.QueryUserBeedingList(ViewBag.Game, ViewBag.GameType, string.Empty, key, ViewBag.PageIndex, ViewBag.PageSize, (QueryUserBeedingListOrderByProperty)ViewBag.orderByProperty, (OrderByCategory)ViewBag.orderByCategory, UserToken);
            return View();
        }
        #region 参与合买
        [HttpPost]
        public JsonResult join_sports(string id)
        {
            try
            {
                string schemeid = PreconditionAssert.IsNotEmptyString(id, "方案编号不能为空。");
                int buycount = int.Parse(PreconditionAssert.IsNotEmptyString(Request["buyCount"], "购买份数不能为空。"));
                PreconditionAssert.IsTrue(buycount > 0, "购买份数不能小于1。");
                var isYt = string.IsNullOrEmpty(Request["isYt"]) ? false : bool.Parse(Request["isYt"]);
                var joinpwd = Request["joinpwd"];
                var balancepwd = Request["balancepwd"];
                var result = new CommonActionResult();
                if (isYt)
                    result = WCFClients.GameClient.XianFaQiHSC_JoinSportsTogether(schemeid, buycount, joinpwd, balancepwd, UserToken);
                else
                    result = WCFClients.GameClient.JoinSportsTogether(schemeid, buycount, joinpwd, balancepwd, UserToken);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //退订
        [HttpPost]
        public JsonResult exit_sports(string id)
        {
            try
            {
                string schemeid = PreconditionAssert.IsNotEmptyString(id, "方案编号不能为空。");
                var joinId = int.Parse(PreconditionAssert.IsNotEmptyString(Request["joinid"], "跟单ID不能为空"));
                var res = WCFClients.GameClient.ExitTogether(schemeid, joinId, UserToken);
                return Json(res);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        //撤销合买订单
        [HttpPost]
        public JsonResult cancel_sports(string id)
        {
            try
            {
                string schemeid = PreconditionAssert.IsNotEmptyString(id, "方案编号不能为空。");
                var res = WCFClients.GameClient.CancelTogether(schemeid, UserToken);
                return Json(res);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
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
    }
}
