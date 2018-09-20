using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Models;
using Common.Lottery.Objects;
using Common.Utilities;
using External.Core;
using External.Core.Celebritys;
using GameBiz.Core;

namespace app.lottery.site.Controllers
{
    public class ArtistsController : BaseController
    {
        ///// <summary>
        ///// 名家-模型大厅，首页
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Model()
        //{
        //    ViewBag.CurrentUser = CurrentUser;
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
        //    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 4 : int.Parse(Request["pagesize"]);
        //    ViewBag.bidding = WCFClients.ExternalClient.QueryBiddingRank(ViewBag.PageIndex, ViewBag.PageSize);
        //    return View();
        //}

        /// <summary>
        /// 我的
        /// </summary>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult My()
        {

            return View();
        }
        /// <summary>
        /// 我的购买
        /// </summary>
        /// <returns></returns>

        //public PartialViewResult Buy()
        //{
        //    var year = string.IsNullOrEmpty(Request["year"]) ? DateTime.Now.Year : int.Parse(Request["year"]);
        //   var month= string.IsNullOrEmpty(Request["month"]) ? DateTime.Now.Month : int.Parse(Request["month"]);
        //    var time = Convert.ToDateTime(year + "-" + month);
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? base.PageIndex : int.Parse(Request["pageindex"]);
        //    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    try
        //    {
        //        ViewBag.BuyList = WCFClients.ExternalClient.QueryMyBuyModelSchemeList(CurrentUser.LoginInfo.UserId, time, ViewBag.PageIndex, ViewBag.PageSize);
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.BuyList = null;
        //    }
        //    return PartialView();
        //}
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>

        public PartialViewResult Fav()
        {
            ViewBag.CurrentUser = CurrentUser;
            return PartialView();
        }
        /// <summary>
        /// 我的关注
        /// </summary>
        /// <returns></returns>

        //public PartialViewResult Att()
        //{
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? base.PageIndex : int.Parse(Request["pageindex"]);
        //    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    try
        //    {
        //        ViewBag.MyAttention = WCFClients.ExternalClient.QueryMyAttentionList(CurrentUser.LoginInfo.UserId, ViewBag.PageIndex, ViewBag.PageSize);
        //    }
        //    catch (Exception)
        //    {

        //        ViewBag.MyAttention = null;
        //    }
        //    return PartialView();
        //}
        /// <summary>
        /// 我创建的模型
        /// </summary>
        /// <returns></returns>

        public PartialViewResult Create()
        {
            ViewBag.CurrentUser = CurrentUser;
            return PartialView();
        }
        /// <summary>
        /// 得到模型列表
        /// </summary>
        /// <returns></returns>
        //public JsonResult getModelList()
        //{
        //    var userId = string.IsNullOrEmpty(Request["userid"]) ? "" : Request["userid"];
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? base.PageIndex : int.Parse(Request["pageindex"]);
        //    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    var isFav = string.IsNullOrEmpty(Request["isFav"]) ? false : Convert.ToBoolean(Request["isFav"]);
        //    var modelName = string.IsNullOrEmpty(Request["modelname"]) ? "" : Request["modelname"];
        //    var isbuy = string.IsNullOrEmpty(Request["isbuy"]) ? false : Convert.ToBoolean(Request["isbuy"]);//当前可购买
        //    var isExperter = string.IsNullOrEmpty(Request["isExp"]) ? false : Convert.ToBoolean(Request["isExp"]);//名家模型
        //    var isFirstPayment = string.IsNullOrEmpty(Request["isFirstPayment"])
        //        ? false
        //        : Convert.ToBoolean(Request["isFirstPayment"]);//先行赔付

        //    var orderby = string.IsNullOrEmpty(Request["orderstr"]) ? "" : Request["orderstr"];//排序

        //    var modelList = new WinnerModelInfo_Collection();
        //    #region 我收藏的模型
        //    if (isFav)
        //    {
        //        modelList = WCFClients.ExternalClient.QueryWinnerModelShouCangCollection(userId, ViewBag.PageIndex, ViewBag.PageSize);
        //        return Json(new { Pageindex = ViewBag.PageIndex, Pagesize = ViewBag.PageSize, modelList });
        //    }
        //    #endregion

        //    #region 模型大厅
        //    if (userId != "")
        //    {
        //        //用户id不为为空-根据用户id查询
        //        modelList = WCFClients.ExternalClient.QueryWinnerModelListByUserId(userId, ViewBag.PageIndex, ViewBag.PageSize);
        //    }
        //    else
        //    {
        //        //首页模型大厅
        //        modelList = WCFClients.ExternalClient.QueryWinnerModelCollection(modelName, isbuy, isExperter, isFirstPayment, orderby, ViewBag.PageIndex, ViewBag.PageSize);
        //    }
        //    return Json(new { Pageindex = ViewBag.PageIndex, Pagesize = ViewBag.PageSize, modelList });
        //    #endregion
        //}
        /// <summary>
        /// 创建新模型
        /// </summary>
        /// <returns></returns>
        public ActionResult MyNavigate()
        {
            return View();
        }
        /// <summary>
        /// 自选模型
        /// </summary>
        /// <returns></returns>
        //[CheckLogin]
        //public ActionResult MyManual(string mid)
        //{
        //    var userId = CurrentUser.LoginInfo.UserId;
        //    int pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);

        //    try
        //    {
        //        ViewBag.SaveOrderList = WCFClients.ExternalClient.QuerySaveOrderToModel(userId, PageIndex, pageSize);
        //        if (!string.IsNullOrEmpty(mid))
        //        {
        //            ViewBag.CycleList = WCFClients.ExternalClient.QueryAllWinnerModelCycleList(mid, PageIndex, -1);
        //            ViewBag.Mid = mid;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.SaveOrderList = null;
        //    }
        //    return View();
        //}
        ///// <summary>
        ///// 添加模型和每期方案
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult AddModel()
        //{
        //    try
        //    {
        //        var schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeid"], "订单编号不能为空");
        //        var mid = Request["mid"];
        //        if (string.IsNullOrEmpty(mid))
        //        {
        //            var modelname = PreconditionAssert.IsNotEmptyString(Request["modelname"], "模型名称不能为空");
        //            var username = CurrentUser.LoginInfo.DisplayName;
        //            var result = WCFClients.ExternalClient.AddWinnerModel(schemeId, modelname, username);
        //            return Json(result);
        //        }
        //        else
        //        {
        //            var result = WCFClients.ExternalClient.AddWinnerModelCycleScheme(mid.ToUpper(), schemeId);
        //            return Json(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Message = ex.Message });
        //    }
        //}
        /// <summary>
        /// 购买模型每期方案
        /// </summary>
        /// <returns></returns>
        //public JsonResult BuyModelCycleScheme()
        //{
        //    try
        //    {
        //        var mid = PreconditionAssert.IsNotEmptyString(Request["mid"], "模型id不能为空");
        //        var amount = string.IsNullOrEmpty(Request["amount"]) ? 1 : int.Parse(Request["amount"]);
             
        //        var bettype = (BettingType)(string.IsNullOrEmpty(Request["bettype"]) ? 20 : int.Parse(Request["bettype"]));//先行赔付/盈利
        //        var buytype = (BuyPayType)(string.IsNullOrEmpty(Request["buytype"]) ? 20 : int.Parse(Request["buytype"]));//全额付款、分期付款
        //        var profit = (ProfitBettingCategory)(string.IsNullOrEmpty(Request["profit"]) ? 10 : int.Parse(Request["profit"]));//投注模式
        //        var isstop = string.IsNullOrEmpty(Request["isstop"]) ? false : Convert.ToBoolean(Request["isstop"]);//盈利后停止'
        //        var cuserId = PreconditionAssert.IsNotEmptyString(Request["cuserId"], "用户id不能为空");//创建用户id
        //        var totalbettingMoney = string.IsNullOrEmpty(Request["bettingmoney"]) ? 0M : decimal.Parse(Request["bettingmoney"]);//总投注金额
        //        var curbettingMoney = string.IsNullOrEmpty(Request["curbettingmoney"]) ? 0M : decimal.Parse(Request["curbettingmoney"]);//当前投注金额
        //        var chaseIssuse = string.IsNullOrEmpty(Request["chaseIssuse"]) ? 15 : int.Parse(Request["chaseIssuse"]);//追号期数
        //        var SetDoubleAmount = string.IsNullOrEmpty(Request["SetDoubleAmount"]) ? 1 : int.Parse(Request["SetDoubleAmount"]);//固定翻倍设置
        //        var ProfiteRatio = string.IsNullOrEmpty(Request["ProfiteRatio"]) ? 0 : (decimal.Parse(Request["ProfiteRatio"])/100);//固定盈利率设置
        //        var bpwd = "";
        //        if (CurrentUser != null && CurrentUserBalance.CheckIsNeedPassword("Bet"))
        //            bpwd = PreconditionAssert.IsNotEmptyString(Request["bpwd"], "请输入资金密码");
      
                    
        //        var info = new BettingModelCycleInfo
        //        {
        //            ModelId = mid.ToUpper(),
        //            Amount = amount,
        //            BalancePassword = bpwd,
        //            BettingType = bettype,
        //            CurrUserId = CurrentUser.LoginInfo.UserId,
        //            BuyType = buytype,
        //            ProfitBettingCategory = profit,
        //            IsProfitedStop = isstop,
        //            CreaterUserId = cuserId,
        //            CurrBettingMoney = curbettingMoney,
        //            TotalChaseIssuseCount = chaseIssuse,
        //            SetDoubleAmount = SetDoubleAmount,
        //            TotalBettingMoney = totalbettingMoney,
        //            ProfiteRatio = ProfiteRatio
        //        };
        //        var result = WCFClients.ExternalClient.BuyModelCycleScheme(info);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Message = ex.Message, ReturnValue = "" });
        //    }
        //}
        /// <summary>
        /// 模型管理--编辑模型信息
        /// </summary>
        /// <returns></returns>
        //[CheckLogin]
        //public ActionResult Edit(string id)
        //{
        //    ViewBag.Mid = PreconditionAssert.IsNotEmptyString(id, "模型编号不能为空");
        //    try
        //    {
        //        ViewBag.modelInfo = WCFClients.ExternalClient.QueryWinnerModelCycleCollection(id.ToUpper(), PageIndex, -1);
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.modelInfo = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 编辑模型
        /// </summary>
        /// <returns></returns>
        //public JsonResult EditModel()
        //{
        //    var mid = PreconditionAssert.IsNotEmptyString(Request["mid"], "模型id不能为空");
        //    var des = string.IsNullOrEmpty(Request["desc"]) ? "" : Request["desc"];//模型描述
        //    var isShare = string.IsNullOrEmpty(Request["isshare"]) ? false : Convert.ToBoolean(Request["isshare"]);//分享
        //    var security = (ModelSecurity)(string.IsNullOrEmpty(Request["security"]) ? 10 : int.Parse(Request["security"]));//方案保密性
        //    var IsFirstPayment=string.IsNullOrEmpty(Request["isPayment"])?false:Convert.ToBoolean(Request["isPayment"]);//先行赔付
        //    var riskType = (RiskType) (string.IsNullOrEmpty(Request["riskType"]) ? 20 : int.Parse(Request["riskType"]));//风险设置 低风险/适中
        //    var lossRatio = string.IsNullOrEmpty(Request["lossRation"]) ? 0.05M : (decimal.Parse(Request["lossRation"]) / 100);//亏损比例设置
        //    var commRitio = string.IsNullOrEmpty(Request["commRitio"]) ? 0 : (decimal.Parse(Request["commRitio"]) / 100);//佣金比例设置
        //    var  proIssuseCount=string.IsNullOrEmpty(Request["proIssuseCount"])?1:int.Parse(Request["proIssuseCount"]);//盈利期数设置
        //    WinnerModelInfo info = new WinnerModelInfo
        //    {
        //        ModelId = mid.ToUpper(),
        //        ModelDescription = des,
        //        IsShare = isShare,
        //        IsFirstPayment = IsFirstPayment,
        //        RiskType=riskType,
        //        ModelSecurity = security,
        //        LossRatio = lossRatio,
        //        CommissionRitio = commRitio,
        //        ProfitIssuseCount = proIssuseCount,
        //        ModelType = ModelType.OptionalModel
        //    };
        //    try
        //    {
        //        var result = WCFClients.ExternalClient.EditWinnerModel(info);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new{IsSuccess=false,Message=ex.Message});
        //    }
           
        //}
        /// <summary>
        /// 模型详情
        /// </summary>
        /// <returns></returns>
        //public ActionResult Info()
        //{
        //    var id = Request["mid"];
        //    ViewBag.Mid = PreconditionAssert.IsNotEmptyString(id, "模型编号不能为空");
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? PageIndex : int.Parse(Request["pageindex"]);
        //    ViewBag.PaseSize = string.IsNullOrEmpty(Request["pagesize"]) ? -1 : int.Parse(Request["pagesize"]);
        //    ViewBag.CurrentUser = CurrentUser;
        //    ViewBag.CurrentUserBalance = CurrentUserBalance;
        //    if (CurrentUser != null)
        //        ViewBag.ShouCang = WCFClients.ExternalClient.QueryWinnerModelShouCang(id, CurrentUser.LoginInfo.UserId);
        //    try
        //    {
        //        ViewBag.ModelInfo = WCFClients.ExternalClient.QueryWinnerModelCycleCollection(id, ViewBag.PageIndex, ViewBag.PaseSize);
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.ModelInfo = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 排行榜
        /// </summary>
        /// <returns></returns>
        //public ActionResult Ranklist()
        //{
        //    ViewBag.querydate = string.IsNullOrEmpty(Request["querydate"]) ? 90 : int.Parse(Request["querydate"]);
        //    ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
        //    ViewBag.pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    ViewBag.CurrentUser = CurrentUser;
        //    try
        //    {
        //        ViewBag.rankList = WCFClients.ExternalClient.QueryModelRanlList(ViewBag.querydate, false, ViewBag.pageIndex, ViewBag.pageSize);
        //        //根据用户id查询关注的用户列表
        //        if (CurrentUser == null) ViewBag.AttList = null;
        //        else ViewBag.AttList = WCFClients.GameClient.QueryAttentionUserList(CurrentUser.LoginInfo.UserId, 0, -1, this.UserToken);
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.rankList = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 名家中心-排行榜
        /// </summary>
        /// <returns></returns>
        //public ActionResult Aritscenter()
        //{
        //    ViewBag.querydate = string.IsNullOrEmpty(Request["querydate"]) ? 90 : int.Parse(Request["querydate"]);
        //    ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
        //    ViewBag.pageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    try
        //    {
        //        ViewBag.rankList = WCFClients.ExternalClient.QueryModelRanlList(ViewBag.querydate, true, ViewBag.pageIndex, ViewBag.pageSize);
        //        //根据用户id查询关注的用户列表
        //        if (CurrentUser == null) ViewBag.AttList = null;
        //        else ViewBag.AttList = WCFClients.GameClient.QueryAttentionUserList(CurrentUser.LoginInfo.UserId, 0, -1, this.UserToken);
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.rankList = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        //public ActionResult MyCenter(string id)
        //{
        //    var viewUserId = PreconditionAssert.IsNotEmptyString(id, "用户编号不能为空");
        //    ViewBag.CurrentUser = CurrentUser;
        //    ViewBag.viewUserId = viewUserId;
        //    try
        //    {
        //        ViewBag.ModelCenter = WCFClients.ExternalClient.QueryWinnerModelCenter(viewUserId);
        //        if (CurrentUser != null)
        //            ViewBag.AttList = WCFClients.ExternalClient.QueryMyAttentionList(CurrentUser.LoginInfo.UserId, 0, -1);

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ModelCenter = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 得到拍砖吐槽
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetTuskList()
        //{
        //    var viewUserId = string.IsNullOrEmpty(Request["viewuserId"]) ? "" : Request["viewuserId"];
        //    try
        //    {
        //        var TuskList = WCFClients.ExternalClient.QueryCelebrityTsukkomiInfosByUserId(viewUserId, 1000);
        //        return Json(new { IsSuccess = true, TuskList });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false});
        //    }
         
        //}
        /// <summary>
        /// 转入转出先行赔付
        /// </summary>
        /// <returns></returns>
        //public JsonResult PayFirstPayment()
        //{
        //    var payType = Request["payType"];
        //    var userId = CurrentUser.LoginInfo.UserId;
        //    var modelId = string.IsNullOrEmpty(Request["modelId"]) ? "" : Request["modelId"];
        //    var payMoney = string.IsNullOrEmpty(Request["payMoney"]) ? 0M : decimal.Parse(Request["payMoney"]);
        //    try
        //    {
        //        if (payType == "in")
        //        {
        //            var result = WCFClients.ExternalClient.PayInFirstPayment(userId, modelId, payMoney);
        //            return Json(result);
        //        }
        //        else
        //        {
        //            var result = WCFClients.ExternalClient.PayOutFirstPayment(userId, modelId, payMoney);
        //            return Json(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //         return Json(new{IsSuccess=false,Message=ex.Message});
        //    }
        //}
        /// <summary>
        /// 开启或暂停现行赔付
        /// </summary>
        /// <returns></returns>
        //public JsonResult DealRiskStatus()
        //{
            
        //    var modelId = PreconditionAssert.IsNotEmptyString(Request["modelId"], "模型id不能为空");
        //    var riskType = string.IsNullOrEmpty(Request["riskType"]) ? "" : Request["riskType"];
        //    var userId = CurrentUser.LoginInfo.UserId;
        //    try
        //    {
        //        if (riskType == "start")
        //        {
        //            var result = WCFClients.ExternalClient.EnableFirstPayment(userId, modelId);
        //            return Json(result);
        //        }
        //        else
        //        {
        //            var result = WCFClients.ExternalClient.PauseFirstPayment(userId, modelId);
        //            return Json(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { IsSuccess = false, Message = ex.Message });
        //    }
        //}
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <returns></returns>
        //public JsonResult AddWinnerModelCollection()
        //{
        //    var modelId = string.IsNullOrEmpty(Request["mid"]) ? "" : Request["mid"];
        //    var type = Request["type"];
        //    var userId = CurrentUser.LoginInfo.UserId;
        //    try
        //    {
        //        if (type == "Cancel")
        //        {
        //            var result = WCFClients.ExternalClient.CancelWinnerModelCollection(modelId.ToUpper(), userId);
        //            return Json(new { IsSuccess = true, Message = result});
        //        }
        //        else
        //        {
        //            var result = WCFClients.ExternalClient.AddWinnerModelCollection(modelId.ToUpper(), userId);
        //            return Json(new { IsSuccess = true, Message = result });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new{IsSuccess=false,Message=ex.Message});
        //    }
        //}
        /// <summary>
        /// 追号计划
        /// </summary>
        //[CheckLogin]
        //public ActionResult ChasePlan(string id)
        //{
        //    ViewBag.keyLine = PreconditionAssert.IsNotEmptyString(id, "追号计划订单编号不能为空");
        //    try
        //    {
        //        ViewBag.ChaseInfo = WCFClients.ExternalClient.QueryWinnerModelSchemeByKeyLine(id);
        //        ViewBag.ChsseInfoDetail = WCFClients.ExternalClient.QueryWinnerModelSchemeDetailByKeyLine(id);
        //        ViewBag.CurrenUser = CurrentUser;
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.ChaseInfo = null;
        //    }
        //    return View();
        //}
        /// <summary>
        /// 追号计划列表
        /// </summary>
        //public ActionResult ChasePlanList()
        //{
        //    var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户编码不能为空");
        //    var time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now : Convert.ToDateTime(Request["time"]);
        //    ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? PageIndex : int.Parse(Request["pageindex"]);
        //    ViewBag.PaseSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
        //    ViewBag.ChaseList = WCFClients.ExternalClient.QueryMyBuyModelSchemeList(userId, time, ViewBag.PageIndex, ViewBag.PaseSize);
        //    return View();
        //}

        /// <summary>
        /// 停止追号
        /// </summary>
        //public JsonResult StopWinnerModelScheme()
        //{
        //    var keyLine = PreconditionAssert.IsNotEmptyString(Request["keyLine"], "追号计划订单编号不能为空");
        //    try
        //    {
        //         var result = WCFClients.ExternalClient.StopWinnerModelScheme(keyLine);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Message = ex.Message });
        //    }
        //}
        /// <summary>
        /// 保存名家自我描述
        /// </summary>
        /// <returns></returns>
        //public JsonResult EditCelebrity()
        //{
        //    var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户编号不能为空");
        //    var description = string.IsNullOrEmpty(Request["desc"]) ? "" : Request["desc"];
        //    try
        //    {
        //        var result = WCFClients.ExternalClient.UpdataCelebrity(userId, description, "");
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new{IsSuccess=false,Message=ex.Message});
        //    }
        //}
        /// <summary>
        /// 添加吐槽
        /// </summary>
        /// <returns></returns>
        //public JsonResult AddCelebrityTsukkomi()
        //{
        //    var desc = string.IsNullOrEmpty(Request["desc"]) ? "" : Request["desc"];
        //    var winnerUserId = PreconditionAssert.IsNotEmptyString(Request["winnerUserId"], "名家编号不能为空");
        //    var sendUserId = CurrentUser.LoginInfo.UserId;
        //    CelebrityTsukkomiInfo info = new CelebrityTsukkomiInfo
        //    {
        //        Content = desc,
        //        UserId = winnerUserId,
        //        SendUserId = sendUserId,
        //    };
        //    try
        //    {
        //        var result = WCFClients.ExternalClient.AddCelebrityTsukkomi(info);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //       return Json(new{IsSuccess=false,Message=ex.Message});
        //    }
        //}
        
    }
}
