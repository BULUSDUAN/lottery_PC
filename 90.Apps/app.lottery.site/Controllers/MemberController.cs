using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Common.Gateway.BBPay;
using Common.Utilities;
using External.Core;
using GameBiz.Core;
using Common.Gateway.YinBao;
using Common.Gateway.Alipay.Logistics;
using Common.Gateway;
using Common.Gateway.Alipay.Pay;
using Common.Gateway.Tenpay.Pay;
using Common.XmlAnalyzer;
using Common.Gateway.KQ.Pay;
using Common.Net;
using External.Core.Authentication;
using Common.Communication;
using app.lottery.site.Models;
using External.Core.SiteMessage;
using System.Net;
using System.IO;
using System.Text;
using Common.Log;
using app.lottery.site.cbbao.Controllers;
using Common.Gateway.BoYing;
using System.Threading;
using app.lottery.site.iqucai;
using Common.Lottery.Redis;
using Common.Pay.mobao;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;
using EntityModel.RequestModel;
using EntityModel;

namespace app.lottery.site.Controllers
{
    [CheckLogin]
    public class MemberController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public MemberController(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion
        /// <summary>
        /// 会员中心左侧菜单
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Menu()
        {
            ViewBag.User = CurrentUser;
            return PartialView();
        }
        /// <summary>
        /// 会员中心头部
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Toptip()
        {
            LoadMemberInfo();
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return PartialView();
        }


        #region 修改密码
        /// <summary>
        /// 会员中心-修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult Password()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        /// <summary>
        /// 提交修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult Passwordexcute()
        {
            try
            {
                var oldpwd = PreconditionAssert.IsNotEmptyString(Request["OldPassword"], "旧密码不能为空");
                var newpwd = PreconditionAssert.IsNotEmptyString(Request["NewPassword"], "新密码不能为空");
                //验证登录密码和资金密码是否一致
                var checkRes = WCFClients.ExternalClient.CheckIsSame2BalancePassword(newpwd, CurrentUser.LoginInfo.UserId);
                PreconditionAssert.IsTrue(checkRes.IsSuccess && checkRes.ReturnValue != "T", "登录密码不能与资金密码相同");
                var result = WCFClients.ExternalClient.ChangeMyPassword(oldpwd, newpwd, UserToken);
                ViewBag.IsSuccess = result.IsSuccess;
                ViewBag.Message = result.Message;

                if (result.IsSuccess)
                {
                    try
                    {
                        //修改密码到BBS
                        //var client = new DS.Web.UCenter.Client.UcClient();
                        //string gbStr = System.Text.Encoding.GetEncoding("gbk").GetString(System.Text.Encoding.GetEncoding("gb2312").GetBytes(CurrentUser.LoginInfo.LoginName));
                        //var r = client.UserEdit(gbStr, string.Empty, newpwd, string.Empty, ignoreOldPw: true);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public JsonResult passCheack()
        {
            try
            {
                var oldpwd = PreconditionAssert.IsNotEmptyString(Request["OldPassword"], "旧密码不能为空");
                var newpwd = PreconditionAssert.IsNotEmptyString(Request["NewPassword"], "新密码不能为空");
                //验证登录密码和资金密码是否一致
                var checkRes = WCFClients.ExternalClient.CheckIsSame2BalancePassword(newpwd, CurrentUser.LoginInfo.UserId);
                PreconditionAssert.IsTrue(checkRes.IsSuccess && checkRes.ReturnValue != "T", "登录密码不能与资金密码相同");
                var result = WCFClients.ExternalClient.ChangeMyPassword(oldpwd, newpwd, UserToken);
                //   ViewBag.IsSuccess = result.IsSuccess;
                //  ViewBag.Message = result.Message;
                if (result.IsSuccess)
                {
                    #region 调用UCenter修改密码

                    //if (this.CurrentUser != null && this.CurrentUser.LoginInfo != null && !string.IsNullOrEmpty(this.CurrentUser.LoginInfo.DisplayName))
                    //{
                    //    var userName = HttpContext.Server.UrlEncode(this.CurrentUser.LoginInfo.DisplayName);
                    //    ThreadPool.UnsafeQueueUserWorkItem((u) =>
                    //    {
                    //        try
                    //        {
                    //            var client = new DS.Web.UCenter.Client.UcClient();
                    //            var r = client.UserEdit(userName, string.Empty, newpwd, string.Empty, ignoreOldPw: true);
                    //        }
                    //        catch (Exception)
                    //        {
                    //        }

                    //    }, null);
                    //}

                    #endregion

                    return Json(new { Succuss = true, Msg = result.Message });
                }
                else
                {
                    return Json(new { Succuss = false, Msg = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }

        #endregion

        #region 资金密码管理
        public ActionResult Balancepwd()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }
        /// <summary>
        /// 设置资金密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult setbalancepwd()
        {
            try
            {
                var isset = string.IsNullOrEmpty(Request["isset"]) ? true : bool.Parse(Request["isset"]);
                var old = Request["oldpassword"];
                var pwd = PreconditionAssert.IsNotEmptyString(Request["password"], "资金密码不能为空");

                if (isset)
                {
                    if (!Regex.IsMatch(pwd, "^\\d{6}$"))
                    {
                        return Json(new { IsSuccess = false, Message = "新资金密码只能使用0-9的6位数字." });
                    }
                    var checkRes = WCFClients.ExternalClient.CheckIsSame2LoginPassword(pwd, UserToken);
                    PreconditionAssert.IsTrue(checkRes.IsSuccess && checkRes.ReturnValue != "T", "资金密码不能与登录密码相同");
                }
                var result = WCFClients.GameFundClient.SetBalancePassword(isset ? old : pwd, isset, pwd, UserToken);
                if (result.IsSuccess)
                {
                    LoadUserLeve();
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
        /// 设置自己密码服务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult setbalancepwd_place()
        {
            try
            {
                var placeList = Request["placeList"];
                var pwd = PreconditionAssert.IsNotEmptyString(Request["password"], "资金密码不能为空");
                var result = WCFClients.GameFundClient.SetBalancePasswordNeedPlace(pwd, placeList, UserToken);
                if (result.IsSuccess)
                {
                    LoadUserLeve();
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

        #region 安全中心
        //安全中心
        public ActionResult safe()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }

        public static string EncodeQQNumber(string qq)
        {
            var length = qq.Length;
            if (length <= 3) return qq;
            var showStr = qq.Substring(0, 1);
            for (int i = 0; i < length - 3; i++)
            {
                showStr += "*";
            }
            showStr += qq.Substring(length - 2);
            return showStr;
        }

        #endregion

        #region 安全中心认证
        //安全中心认证
        public ActionResult safe_Authname(string name)
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Id = name;
            return View();
        }
        #endregion

        #region 自动跟单
        /// <summary>
        /// 我的定制
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Autofollow(string id)
        {
            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"];
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);


            ViewBag.FollowList = WCFClients.GameClient.QueryUserFollowRuleByCreater(ViewBag.GameCode, ViewBag.GameType, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 成功定制记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Followrecord(string id)
        {
            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "" : id;
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.FollowRecord = WCFClients.GameClient.QuerySucessFolloweRecord(ViewBag.GameCode, -1, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 定制我的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Followme()
        {
            ViewBag.GameCode = string.IsNullOrEmpty(Request["gamecode"]) ? "" : Request["gamecode"].ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["gametype"]) ? "" : Request["gametype"].ToLower();
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.FollowMeList = WCFClients.GameClient.QueryUserFollowRule(ViewBag.GameCode, ViewBag.GameType, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 设置跟单顺序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Followusers(string id)
        {
            ViewBag.GameCode = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            ViewBag.GameType = string.IsNullOrEmpty(Request["type"]) ? "" : Request["type"].ToLower();
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.FollowList = WCFClients.GameClient.QueryUserFollowRuleByCreater(ViewBag.GameCode, ViewBag.GameType, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 跟单人列表里的用户跟单记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Followmerecord(string id)
        {
            ViewBag.Game = string.IsNullOrEmpty(id) ? "" : id;
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.RuleId = string.IsNullOrEmpty(Request["ruleId"]) ? -1 : int.Parse(Request["ruleId"]);
            ViewBag.FollowRecord = WCFClients.GameClient.QuerySucessFolloweRecord(ViewBag.Game, ViewBag.RuleId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
            return PartialView();
        }
        /// <summary>
        /// 调整跟单人顺序
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeFollowIndex()
        {
            try
            {
                var changeType = string.IsNullOrEmpty(Request["changeType"]) ? "down" : Request["changeType"];
                var game = PreconditionAssert.IsNotEmptyString(Request["game"], "请传入彩种编码");
                var type = Request["type"].ToLower();
                //PreconditionAssert.IsNotEmptyString(Request["type"], "请传入彩种类型编码");
                var creatUser = PreconditionAssert.IsNotEmptyString(Request["createUser"], "请传入发起人编号");
                var ruleId = long.Parse(PreconditionAssert.IsNotEmptyString(Request["ruleId"], "请传入规则ID"));
                var result = new CommonActionResult() { IsSuccess = false, Message = "调整顺序失败" };
                switch (changeType.ToLower())
                {
                    case "down":
                        result = WCFClients.GameClient.FollowRuleMoveDown(creatUser, game, type, ruleId, UserToken);
                        break;
                    case "up":
                        result = WCFClients.GameClient.FollowRuleMoveUp(creatUser, game, type, ruleId, UserToken);
                        break;
                    case "top":
                        result = WCFClients.GameClient.FollowRuleSetTop(creatUser, game, type, ruleId, UserToken);
                        break;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        #endregion

        #region 我的投注

        public async Task<ActionResult> betorder()
        {
            var beginDate = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            var endDate = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : DateTime.Parse(Request["end"]);
            if ((endDate - beginDate).TotalDays > 30)
            {
                beginDate = DateTime.Today.AddDays(-30);
                endDate = DateTime.Today;
            }
            ViewBag.GameCode = string.IsNullOrEmpty(Request["gameCode"]) ? string.Empty : Request["gameCode"];
            ViewBag.BonusStatus = string.IsNullOrEmpty(Request["bonusStatus"]) ? null : (BonusStatus?)int.Parse(Request["bonusStatus"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Begin = beginDate;
            ViewBag.End = endDate;
            Dictionary<string, object> param = new Dictionary<string, object>();
            var Model = new QueryMyBettingOrderParam() { UserID = UserToken, startTime = beginDate, endTime = endDate, pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, bonusStatus = ViewBag.BonusStatus, gameCode = ViewBag.GameCode };
            param["Model"] = Model;
            ViewBag.Orders = await serviceProxyProvider.Invoke<EntityModel.CoreModel.MyBettingOrderInfoCollection>(param, "api/Order/QueryMyBettingOrderList");
            return View();
        }

        public async Task<ActionResult> myCreateTogether()
        {
            var beginDate = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            var endDate = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : DateTime.Parse(Request["end"]);
            if ((endDate - beginDate).TotalDays > 30)
            {
                beginDate = DateTime.Today.AddDays(-30);
                endDate = DateTime.Today;
            }
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.GameCode = string.IsNullOrEmpty(Request["gameCode"]) ? string.Empty : Request["gameCode"];
            ViewBag.Begin = beginDate;
            ViewBag.End = endDate;

            Dictionary<string, object> param = new Dictionary<string, object>();
            var Model = new QueryCreateTogetherOrderParam() { userId = this.CurrentUser.LoginInfo.UserId, startTime = ViewBag.Begin, endTime = ViewBag.End, gameCode = ViewBag.GameCode, pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, bonus = null };
            param["Model"] = Model;
            //发起的合买
            ViewBag.CreateTogegher = await serviceProxyProvider.Invoke<EntityModel.CoreModel.TogetherOrderInfoCollection>(param, "api/Order/QueryCreateTogetherOrderListByUserId");
            return View();
        }

        public async Task<ActionResult> myJoinTogether()
        {

            var beginDate = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            var endDate = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : DateTime.Parse(Request["end"]);
            if ((endDate - beginDate).TotalDays > 30)
            {
                beginDate = DateTime.Today.AddDays(-30);
                endDate = DateTime.Today;
            }
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.GameCode = string.IsNullOrEmpty(Request["gameCode"]) ? string.Empty : Request["gameCode"];
            ViewBag.Begin = beginDate;
            ViewBag.End = endDate;
            Dictionary<string, object> param = new Dictionary<string, object>();
            var Model = new QueryCreateTogetherOrderParam() { pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, gameCode = ViewBag.GameCode, userId = this.CurrentUser.LoginInfo.UserId, startTime = ViewBag.Begin, endTime = ViewBag.End, bonus = null };
            param["Model"] = Model;
            //参与的合买
            ViewBag.JoinTogether = await serviceProxyProvider.Invoke<EntityModel.CoreModel.TogetherOrderInfoCollection>(param, "api/Order/QueryJoinTogetherOrderListByUserId");
            return View();
        }

        #endregion

        #region 保存方案
        public ActionResult saveProgram()
        {
            ViewBag.Orders = WCFClients.GameClient.QuerySaveOrder_Lottery(UserToken);
            return View();
        }
        #endregion

        #region 红包明细
        /// <summary>
        /// 红包明细
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RedBagList()
        {
            string fundType = string.IsNullOrEmpty(Request["fundType"]) ? "" : Request["fundType"];
            var cateList = "";
            //switch (fundType.ToLower())
            //{
            //    case "pay": cateList = "充值|手工充值"; break;
            //    case "bonuspoint": cateList = "资金转入"; break;
            //    case "hemaicommission": cateList = "合买返点"; break;
            //    case "bonus": cateList = "奖金|合买返点"; break;
            //    case "bet": cateList = "购彩|出票失败|合买失败|撤单|追号返还"; break;
            //    case "fetch": cateList = "申请提现|完成提现|拒绝提现"; break;
            //    case "commission": cateList = "方案返利"; break;
            //    case "in": cateList = "出票失败|奖金|追号返还|合买返点|方案返利|返还保底|合买失败|撤单|充值|手工充值|拒绝提现|资金转入|活动赠送"; break;
            //    case "out": cateList = "购彩|手工扣款|申请提现|完成提现|资金转出"; break;
            //}
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? 7 : int.Parse(Request["time"]);
            ViewBag.Begin = DateTime.Now.AddDays(-(ViewBag.Time));
            ViewBag.End = DateTime.Now.AddDays(1);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            var beginTime = ViewBag.Begin;
            if (beginTime < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);

            Dictionary<string, object> param = new Dictionary<string, object>();
            var Model = new EntityModel.RequestModel.QueryUserFundDetailParam() { viewtype = "", userid = UserToken, fromDate = ViewBag.Begin, toDate = ViewBag.End, pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, accountTypeList = "70" };
            param["Model"] = Model;
            ViewBag.RedBag = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserFundDetailCollection>(param, "api/order/QueryMyFundDetailList");
            return View();
        }
        #endregion

        #region 账户充值记录
        /// <summary>
        /// 充值记录
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> payrecord()
        {
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                FillMoneyStatus? status = string.IsNullOrEmpty(Request.QueryString["status"]) ? null : ((FillMoneyStatus?)int.Parse(Request.QueryString["status"]));
                ViewBag.Status = status;
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                Dictionary<string, object> param = new Dictionary<string, object>();
                var Model = new QueryFillMoneyListParam() { userid = UserToken, startTime = ViewBag.Begin, endTime = ViewBag.End.AddDays(1), pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, statusList = "1" };
                param["Model"] = Model;
                ViewBag.FillMoneyCollection = await serviceProxyProvider.Invoke<EntityModel.CoreModel.FillMoneyQueryInfoCollection>(param, "api/Order/QueryMyFillMoneyList");
            }
            catch (Exception ex)
            {

            }

            return View();
        }
        /// <summary>
        /// 充值向导
        /// </summary>
        /// <returns></returns>
        public ActionResult paywizzard()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        #endregion

        #region 我的追号
        public async Task<ActionResult> chaseorder()
        {
            var beginDate = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            var endDate = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : DateTime.Parse(Request["end"]);
            if ((endDate - beginDate).TotalDays > 30)
            {
                beginDate = DateTime.Today.AddDays(-30);
                endDate = DateTime.Today;
            }
            ViewBag.GameCode = string.IsNullOrEmpty(Request["gameCode"]) ? string.Empty : Request["gameCode"];
            ViewBag.BonusStatus = string.IsNullOrEmpty(Request["bonusStatus"]) ? null : (BonusStatus?)int.Parse(Request["bonusStatus"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Begin = beginDate;
            ViewBag.End = endDate;

            Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    {"gameCode",ViewBag.GameCode },
                    { "startTime",ViewBag.Begin},
                    { "endTime",ViewBag.End},
                    { "pageIndex",ViewBag.pageNo},
                    { "pageSize",ViewBag.PageSize },
                    { "userId",UserToken },
                    { "ProgressStatusType",0 }
                };
            var result = await serviceProxyProvider.Invoke<EntityModel.CoreModel.BettingOrderInfoCollection>(param, "api/Order/QueryMyChaseOrderList");
            return View();
        }
        #endregion

        #region 账户提款
        /// <summary>
        /// 支付宝提款
        /// </summary>
        /// <returns></returns>
        public ActionResult Fetch_alipay()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            if (!CurrentUser.IsAuthenticationMobile)
                ViewBag.MobileRequestInfo = WCFClients.ExternalClient.QueryMyMobileRequestInfo(UserToken);
            return View();
        }
        /// <summary>
        /// 银行卡提款
        /// </summary>
        /// <returns></returns>
        public ActionResult Fetch_bank()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            if (!CurrentUser.IsAuthenticationMobile)
                ViewBag.MobileRequestInfo = WCFClients.ExternalClient.QueryMyMobileRequestInfo(UserToken);
            return View();
        }
        //提款记录
        public ActionResult DrawingsRecord()
        {
            try
            {
                //ViewBag.Balance = string.IsNullOrEmpty(this.CurrentUser.UserBalance.ActivityBalance.ToString()) ? null : this.CurrentUser.UserBalance.ActivityBalance.ToString();
                //ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                //ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
                //ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                //ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                //ViewBag.Status = string.IsNullOrEmpty(Request.QueryString["status"]) ? null : (WithdrawStatus?)int.Parse(Request.QueryString["status"]);
                //if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                //    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                //ViewBag.WithdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(WithdrawStatus.Success, ViewBag.Begin, ViewBag.End.AddDays(1), ViewBag.pageNo, ViewBag.PageSize, UserToken);
                //


                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.Status = string.IsNullOrEmpty(Request.QueryString["status"]) ? null : (WithdrawStatus?)int.Parse(Request.QueryString["status"]);
                ViewBag.CurrentUser = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                ViewBag.WithdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(ViewBag.Status, ViewBag.Begin, ViewBag.End.AddDays(1), ViewBag.pageNo, ViewBag.PageSize, UserToken);
                ViewBag.CurrentUser = CurrentUser;
            }
            catch (Exception)
            {
                ViewBag.WithdrawList = null;
            }

            return View();
        }
        //提款向导
        public ActionResult DrawingsGuide()
        {
            ViewBag.Begin = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Now.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request["end"]) ? DateTime.Today : DateTime.Parse(Request["end"]);
            return View();
        }
        //提款验证页面
        public async Task<ActionResult> Fetchvalidate()
        {
            var bkUrl = string.IsNullOrEmpty(Request.Form["backurl"]) ? "/member/fetch_bank" : Request.Form["backurl"];
            try
            {

                if ((DateTime.Now.Hour < 8 || (DateTime.Now.Hour == 8 && DateTime.Now.Minute < 50))
                  && (DateTime.Now.Hour > 1 || (DateTime.Now.Hour == 1 && DateTime.Now.Minute > 10)))
                {
                    throw new Exception("提现时间早上9点到凌晨1点，请您明天9点再来，感谢配合");
                }

                Session["Repeat"] = "1";
                ViewBag.CurrentUser = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                ViewBag.FetchType = PreconditionAssert.IsNotEmptyString(Request.Form["fetchType"], "提款类型错误");
                ViewBag.Money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request.Form["money"], "提款返点不能为空"));
                var money = string.IsNullOrEmpty(Request.Form["money"]) ? "" : Request.Form["money"];
                if (money.IndexOf(".") > 0)
                    throw new Exception("提款金额不能为小数");
                //最小金额
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["key"] = "Site.Financial.MinWithDrwaMoney";

                var minmoney = await serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");
                PreconditionAssert.IsTrue(ViewBag.Money >= int.Parse(minmoney.ConfigValue), "提款金额不能小于" + minmoney.ConfigValue + "元");

                ViewBag.FetchAccont = string.Empty;
                switch ((string)ViewBag.FetchType)
                {
                    case "alipay":
                        PreconditionAssert.IsTrue(CurrentUser.IsAuthenticationRealName && CurrentUser.IsAuthenticationMobile, "用户未进行安全认证或未绑定手机，无法进行提现操作");
                        ViewBag.FetchAccont = PreconditionAssert.IsNotEmptyString(Request.Form["alipayaccount"], "支付宝账号不能为空。");
                        if (!ValidateHelper.IsEmail(ViewBag.FetchAccont) && !ValidateHelper.IsMobile(ViewBag.FetchAccont))
                        {
                            throw new Exception("支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号）。");
                        }
                        break;
                    case "bank":
                        PreconditionAssert.IsTrue(CurrentUser.IsAuthenticationRealName && CurrentUser.IsAuthenticationMobile && CurrentUser.IsBindBank, "用户未进行安全认证或未绑定信息，无法进行提现操作");
                        ViewBag.FetchAccont = CurrentUser.BankCardInfo.BankCardNumber;
                        break;
                }

                param.Clear();
                param["userId"] = CurrentUser.LoginInfo.UserId;
                param["requestMoney"] = decimal.Parse(money);
                ViewBag.RequestWithdraw_1 = await serviceProxyProvider.Invoke<EntityModel.CoreModel.CheckWithdrawResult>(param, "api/user/RequestWithdraw_Step1");

            }
            catch (Exception ex)
            {
                Response.Redirect(bkUrl + "?errMsg=" + ex.Message);
                return null;
            }
            return View();
        }

        //提款提交结果页
        public async Task<ActionResult> Fetchsubmit()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.FetchType = PreconditionAssert.IsNotEmptyString(Request.Form["fetchType"], "提款类型错误");
                var money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request.Form["money"], "提款金额不能为空"));
                ViewBag.Money = money;
                if (Session["Repeat"] == null)
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.Message = "提款成功";
                    switch ((string)ViewBag.FetchType)
                    {
                        case "alipay":
                            ViewBag.FetchAccount = PreconditionAssert.IsNotEmptyString(Request.Form["fetchAccount"], "支付宝账号不能为空。");
                            break;
                        case "bank":
                            ViewBag.FetchAccount = CurrentUser.BankCardInfo.BankCardNumber;
                            break;
                    }
                    return View();
                }
                PreconditionAssert.IsTrue(ViewBag.Money >= 10, "提款金额不能小于10元");
                //var validateCode = PreconditionAssert.IsNotEmptyString(Request.Form["validatecode"], "提款校验码不能为空");
                var balancepwd = Request["balancepwd"];
                EntityModel.CoreModel.Withdraw_RequestInfo withdrawinfo = new EntityModel.CoreModel.Withdraw_RequestInfo();

                switch ((string)ViewBag.FetchType)
                {
                    case "alipay":
                        ViewBag.FetchAccount = PreconditionAssert.IsNotEmptyString(Request.Form["fetchAccount"], "支付宝账号不能为空。");
                        if (!ValidateHelper.IsEmail(ViewBag.FetchAccount) && !ValidateHelper.IsMobile(ViewBag.FetchAccount))
                        {
                            throw new Exception("支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号）。");
                        }
                        withdrawinfo.BankCardNumber = ViewBag.FetchAccount;
                        withdrawinfo.BankCode = "Alipay";
                        withdrawinfo.BankName = "支付宝";
                        withdrawinfo.RequestMoney = ViewBag.Money;
                        withdrawinfo.WithdrawAgent = EntityModel.Enum.WithdrawAgentType.Alipay;
                        break;
                    case "bank":
                        ViewBag.FetchAccount = CurrentUser.BankCardInfo.BankCardNumber;
                        withdrawinfo.BankCardNumber = CurrentUser.BankCardInfo.BankCardNumber;
                        withdrawinfo.BankCode = CurrentUser.BankCardInfo.BankCode;
                        withdrawinfo.BankName = CurrentUser.BankCardInfo.BankName;
                        withdrawinfo.BankSubName = CurrentUser.BankCardInfo.BankSubName;
                        withdrawinfo.CityName = CurrentUser.BankCardInfo.CityName;
                        withdrawinfo.ProvinceName = CurrentUser.BankCardInfo.ProvinceName;
                        withdrawinfo.RequestMoney = ViewBag.Money;
                        withdrawinfo.userRealName = CurrentUser.RealNameInfo.RealName;
                        withdrawinfo.WithdrawAgent = EntityModel.Enum.WithdrawAgentType.BankCard;
                        break;
                    default:
                        throw new Exception("未知提款类型");
                }

                Dictionary<string, object> param = new Dictionary<string, object>();
                param["info"] = withdrawinfo;
                param["userId"] = CurrentUser.LoginInfo.UserId;
                param["balancepwd"] = balancepwd;
                var result = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/user/RequestWithdraw_Step2");
                //提款成功给财务人员发送短信
                //if (money >= decimal.Parse(WithdrawMoney))
                //{
                //    string[] moblie = base.fina_mobile;
                //    for (int i = 0; i < moblie.Length; i++)
                //    {
                //        WCFClients.GameClient.SendMsg(moblie[i], "财务人员请注意：用户" + CurrentUser.LoginInfo.UserId + "申请提现" + withdrawinfo.RequestMoney + "元,订单号" + result.ReturnValue + "，请及时处理。", IpManager.IPAddress, 5, CurrentUser.LoginInfo.UserId, result.ReturnValue);
                //    }
                //}
                Session["Repeat"] = null;
                ViewBag.IsSuccess = true;
                ViewBag.Message = "提款成功";
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }
            //Session["Repeat"] = null;
            return View();
        }

        #endregion

        #region 账户充值

        public ActionResult pay()
        {
            CurrentUser.IsAuthenticationRealName = IsAuthenticationRealName;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.IPS_JumpURL = base.IPS_Jump_Url;
            ViewBag.ZF_Jump_Url = base.ZF_Jump_Url;
            ViewBag.HC_Jump_Url = base.HC_Jump_Url;
            ViewBag.YS_Jump_Url = base.YS_Jump_Url;
            ViewBag.HC_NoCard_Jump_Url = base.HC_NoCard_Jump_Url;
            ViewBag.YF_WeiXin_Url = base.YF_WeiXin_Url;
            ViewBag.ZTPay_Url = base.ZTPay_Url;
            ViewBag.PAY_101ka_URL_Url = base.PAY_101ka_URL_Url;
            var domain = this.FillMoneyCallBackDomain;// "http://paytz.iqucai.com";// "http://" + Request.Url.Host;
            ViewBag.MaxFillMoney = base.MaxFillMoney;
            ViewBag.CurrentDomain = domain;
            ViewBag.EnableGateWary = this.FillMoney_Enable_GateWay;
            ViewBag.RealNameInfo = this.RealNameInfo;//用户实名认证信息
            ViewBag.BankCardInfo = this.BankCardInfo;//用户绑定的银行卡信息
            ViewBag.MobileInfo = this.MobileInfo;//用户手机绑定信息
            ViewBag.DEBPay_Url = System.Configuration.ConfigurationManager.AppSettings["DEBPay_Url"].ToString();
            ViewBag.SFBPay_Url = System.Configuration.ConfigurationManager.AppSettings["SFBPay_Url"].ToString();
            return View();
        }

        #region  "摩宝支付1401预支付"
        public string mobao_posturl
        {
            get
            {
                string defalutValue = "http://newpay.kspay.net:8080/ks_onlpay/gateways/trans";
                try
                {
                    var v = ConfigurationManager.AppSettings["mobao_posturl"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// //商户号码(摩宝分配的商户号码,正式交易时候换成正式商户号码)
        /// </summary>
        public string mobao_merId
        {
            get
            {
                string defalutValue = "936440348160056";
                try
                {
                    var v = ConfigurationManager.AppSettings["mobao_merId"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// //商户密钥(摩宝分配的商户密钥,正式的交易的时候换成正式密钥)
        /// </summary>
        public string mobao_merPubKey
        {
            get
            {
                string defalutValue = "6E8F443A385B736C";
                try
                {
                    var v = ConfigurationManager.AppSettings["mobao_merPubKey"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        public decimal Max_FillMoney
        {
            get
            {
                decimal defalutValue = 50000;
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("Max.FillMoney").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        decimal.TryParse(v, out defalutValue);
                        return defalutValue;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        public string WebSiteUrl
        {
            get
            {
                string defalutValue = "http://www.wancai.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("WebSiteUrl").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 摩宝预交易 请求下发短信验证
        /// </summary>
        /// <returns></returns>
        public JsonResult mobaoresult1401()
        {
            try
            {
                string paytype = PreconditionAssert.IsNotEmptyString(Request["payType"], "充值类型字符串不能为空！");
                string userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户信息不能为空！");
                string fromDomain = PreconditionAssert.IsNotEmptyString(Request["fromDomain"], "来源不能为空！");
                decimal money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["money"], "充值金额不能为空！"));
                money = money > Max_FillMoney ? Max_FillMoney : money;
                ViewBag.Message = money;

                PreconditionAssert.IsTrue(paytype.Contains("|"), "充值类型字符串必须包含充值接口类型和充值银行编码！");
                string gateway = paytype.Split('|')[0];
                string subpayType = paytype.Split('|')[1];

                string name = PreconditionAssert.IsNotEmptyString(Request["realName"], "持卡人姓名不能为空！");
                string bankCardNum = PreconditionAssert.IsNotEmptyString(Request["bankNum"], "银行卡号不能为空！");
                string cardType = PreconditionAssert.IsNotEmptyString(Request["hnd_Cardtype"], "银行卡类型不能为空！");
                string idCard = string.Empty;
                string cvv2 = string.Empty;
                string expireDate = string.Empty;
                //if (cardType == "01")
                idCard = PreconditionAssert.IsNotEmptyString(Request["idCard"], "银行卡类型为借记卡,身份证号不能为空！");
                //暂时只用借记卡
                //if (cardType == "00")
                //{
                //    expireDate = PreconditionAssert.IsNotEmptyString(Request["expireDate"], "银行卡类型为贷记卡,有效期不能为空！");
                //    cvv2 = PreconditionAssert.IsNotEmptyString(Request["Cvv2"], "银行卡类型为贷记卡,Cvv2不能为空！");
                //}
                string phone = PreconditionAssert.IsNotEmptyString(Request["phoneNum"], "手机号码不能为空！");

                #region 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.Alipay;
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = "在线网银充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";

                #endregion
                string express_show_url = fromDomain;
                string express_return_url = WebSiteUrl + "/member/safe";
                string express_notify_url = fromDomain + "/user/MOBAOPAYNotifyUrl";
                #region 向本地系统添加订单，返回订单号

                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.mobao_express;
                fillMoneyInfo.ShowUrl = express_show_url;
                fillMoneyInfo.ReturnUrl = express_return_url;
                fillMoneyInfo.NotifyUrl = express_notify_url;
                fillMoneyInfo.RequestExtensionInfo = bankCardNum;

                string orderId = string.Empty;
                var expressAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, userId, this.mobao_merId);
                if (expressAddOrderResult.ReturnValue.Contains('|'))
                    orderId = expressAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = expressAddOrderResult.ReturnValue;
                #endregion
                string url = this.mobao_posturl;
                //贷记卡版本
                var nowTag = DateTime.Now.ToString("yyyMMddhhmmss");
                var finalParam = new Dictionary<string, string> {
                {"versionId", "001"},  //版本号
                {"businessType", "1401"}, //交易类型
                {"insCode", ""}, //机构号 可空
                {"merId", this.mobao_merId},         //商户号
                {"orderId", orderId},  //商户订单号
                {"transDate", nowTag},   //交易日期
                {"transAmount", money.ToString()},
                {"cardByName", Convert.ToBase64String(mobaoUtil.GBK.GetBytes(name))},
                {"cardByNo", bankCardNum}, //卡号
                {"cardType", cardType},                  //卡类型
                {"expireDate", expireDate},              //有效期
                {"CVV", cvv2},                      //cvv
                {"bankCode", ""},                    //银行编号
                {"openBankName", ""},                //开户银行
                {"cerType", "01"},                     //证件类型 默认身份证
                {"cerNumber", idCard},                   //证件号码
                {"mobile", phone},           //手机号码
                {"isAcceptYzm", "00"},               //是否下发验证码
                {"pageNotifyUrl",""},//前台通知地址 可空
                {"backNotifyUrl",""},                //后台通知地址 可空
                {"orderDesc", ""},                   //商品名称
                {"instalTransFlag", "01"},           //分期标志
                {"instalTransNums", ""},             //分期期数
                {"dev", ""},                         //自定义域
                {"fee", ""},                         //空余字段
            };
                //获取URL格式字符串
                var transStr = mobaoUtil.EncodeTransMap(finalParam);
                transStr += this.mobao_merPubKey;
                //MD5加密
                var signData = mobaoUtil.GetMD5String(transStr);
                finalParam["signType"] = "MD5";
                finalParam["signData"] = signData;
                transStr = mobaoUtil.EncodeTransMap(finalParam);
                //进行AES加密
                // 加密  (参数一  明文报文   参数2 商户的私钥)
                var encryptedStr = mobaoUtil.HexString(mobaoUtil.EncryptTransData(transStr, this.mobao_merPubKey));
                //发送交易
                var transDataMap = new Dictionary<string, string> {
                {"merId", this.mobao_merId},
                {"transData", encryptedStr}
            };
                var resBody = mobaoUtil.PostTransDataMap(transDataMap, url);
                var jsonObjs = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(resBody);
                // 1401 第一次请求 返回请求内容（验证码等）
                // 01 预交易成功 02 预交易失败
                if (jsonObjs["refCode"].ToString() == "01")
                    return Json(new { refCode = jsonObjs["refCode"].ToString(), ksPayOrderId = jsonObjs["ksPayOrderId"].ToString() });
                else
                    return Json(new { refCode = jsonObjs["refCode"].ToString(), Msg = "预交易失败,请核对信息后重试。" });
            }
            catch (Exception ex)
            {
                return Json(new { refCode = "02", Msg = ex.Message });
            }
        }
        #endregion


        public ActionResult AlipaySelfHelp()
        {
            return View();
        }

        public JsonResult GetWXPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.Alipay;
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = "微信扫码充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.WXPay;
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, base.WXPay_Mchid);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode()
        {
            var money = int.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            var notify_url = base.FillMoneyCallBackDomain + "/user/WXPay_NotifyUrl";

            var api = new Common.Gateway.WXPay.WXPayApi(base.WXPay_AppId, base.WXPay_Mchid, base.WXPay_Key, notify_url);
            var url = api.GetPayUrl(orderId, money, IpManager.IPAddress);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = url
            };
        }

        public JsonResult QueryWXPayOrder()
        {
            try
            {
                var orderId = Request["orderId"];
                var api = new Common.Gateway.WXPay.WXPayApi(base.WXPay_AppId, base.WXPay_Mchid, base.WXPay_Key, "");
                var status = api.OrderQuery(orderId);
                return Json(new { IsSuccess = true, Message = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult WXPayComplate()
        {
            ViewBag.OrderId = Request["orderId"];
            ViewBag.Money = Request["money"];

            return View();
        }

        #endregion

        #region 登录历史记录
        public async Task<ActionResult> Loginhistory()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["UserId"] = UserToken;
            ViewBag.LoginHistory = await serviceProxyProvider.Invoke<EntityModel.UserLoginHistoryCollection>(param, "api/user/QueryCache_UserLoginHistoryCollection");
            ViewBag.User = CurrentUser;
            return View();
        }
        #endregion

        #region 投诉

        /// <summary>
        /// 我的投诉建议
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Mysuggestions()
        {

            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : int.Parse(Request["pageSize"]);
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["pageIndex"] = ViewBag.PageIndex;
            param["pageSize"] = ViewBag.PageSize;
            param["UserId"] = UserToken;
            ViewBag.UserIdea = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserIdeaInfo_QueryCollection>(param, "api/user/QueryMyUserIdeaList");
            return View();
        }

        /// <summary>
        /// 填写投诉建议
        /// </summary>
        /// <returns></returns>
        public ActionResult Suggestions()
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        public JsonResult Submitsuggestion()
        {
            try
            {
                var PageOpenSpeed = string.IsNullOrEmpty(Request["PageOpenSpeed"]) ? "1" : Request["PageOpenSpeed"];//页面打开速度
                var InterfaceBeautiful = string.IsNullOrEmpty(Request["InterfaceBeautiful"]) ? "1" : Request["InterfaceBeautiful"];//界面设计美观
                var ComposingReasonable = string.IsNullOrEmpty(Request["ComposingReasonable"]) ? "1" : Request["ComposingReasonable"];//排版展示合理
                var OperationReasonable = string.IsNullOrEmpty(Request["OperationReasonable"]) ? "1" : Request["OperationReasonable"];//操作过程合理
                var ContentConveyDistinct = string.IsNullOrEmpty(Request["ContentConveyDistinct"]) ? "1" : Request["ContentConveyDistinct"];//内容传达清晰
                string mobile = PreconditionAssert.IsNotEmptyString(Request.Form["mobile"], "请输入您的手机号码，以便我们更好的为您服务。");
                string category = PreconditionAssert.IsNotEmptyString(Request.Form["category"], "请选择问题分类，以便我们更好的处理问题。");
                string suggestion = PreconditionAssert.IsNotEmptyString(Request.Form["suggestion"], "请输入您的投诉内容。");
                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(suggestion), "投诉内容不允许包含敏感词，如有疑问请联系客服。");
                UserIdeaInfo_Add ideaInfo = new UserIdeaInfo_Add()
                {
                    Description = suggestion,
                    Category = category,
                    IsAnonymous = false,
                    CreateUserId = CurrentUser.LoginInfo.UserId,
                    CreateUserDisplayName = CurrentUser.LoginInfo.DisplayName,
                    CreateUserMoibile = mobile,
                    PageOpenSpeed = decimal.Parse(PageOpenSpeed),
                    InterfaceBeautiful = decimal.Parse(InterfaceBeautiful),
                    ComposingReasonable = decimal.Parse(ComposingReasonable),
                    OperationReasonable = decimal.Parse(OperationReasonable),
                    ContentConveyDistinct = decimal.Parse(ContentConveyDistinct),

                };
                var result = WCFClients.ExternalClient.SubmitUserIdea(ideaInfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }

        }
        #endregion

        #region 站内信

        /// <summary>
        /// 站内信
        /// </summary>
        /// <returns></returns>
        public ActionResult Innermail()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);

            ViewBag.UnReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken);
            var innerstatus = string.IsNullOrEmpty(Request["status"]) ? 3 : int.Parse(Request["status"]);
            //ViewBag.TotalList = WCFClients.GameQueryClient.QueryMyInnerMailList(ViewBag.pageNo, ViewBag.PageSize, UserToken);//
            ViewBag.TotalCount = WCFClients.GameQueryClient.GetUserInnerMailCount(CurrentUser.LoginInfo.UserId);

            if (innerstatus == 3)
            {
                ViewBag.InnerMailList = WCFClients.GameQueryClient.QueryMyInnerMailList(ViewBag.pageNo, ViewBag.PageSize, UserToken);
            }
            else
            {
                var type = (InnerMailHandleType)(innerstatus);
                ViewBag.InnerMailList = WCFClients.GameQueryClient.QueryUnReadInnerMailListByReceiver(CurrentUser.LoginInfo.UserId, ViewBag.pageNo, ViewBag.PageSize, type);
            }
            return View();
        }
        /// <summary>
        /// 站内信内容
        /// </summary>
        /// <returns></returns>
        public ActionResult Innermailcontent()
        {
            try
            {
                string mailId = Request.QueryString["MailId"];
                var mailContent = WCFClients.GameQueryClient.ReadInnerMail(mailId, UserToken);
                ViewBag.InnerMail = mailContent;
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        /// <summary>
        /// 删除站内信
        /// </summary>
        /// <param name="postForm"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Deleteinnermail(FormCollection postForm)
        {
            try
            {
                string mailid = PreconditionAssert.IsNotEmptyString(postForm["MailId"], "站内消息ID不能为空。");
                var mailid_ = mailid.Split(',');
                for (int i = 0; i < mailid_.Length; i++)
                {
                    WCFClients.GameQueryClient.DeleteInnerMail(mailid_[i], CurrentUser.LoginInfo.UserId);
                }
                return Json(new { IsSuccess = true, Message = "删除站内信完成" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        #endregion

        #region 返点
        /// <summary>
        /// 会员中心左侧菜单
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MenuFoud()
        {
            return PartialView();
        }/// <summary>
        /// 会员中心头部
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ToptipFoud()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return PartialView();
        }
        public PartialViewResult Mycommenu()
        {
            return PartialView();
        }
        /// <summary>
        /// 成功方案
        /// </summary>
        /// <returns></returns>
        public ActionResult WinScheme()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : Convert.ToDateTime(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.Begin = DateTime.Now.AddMonths(-3);
                ViewBag.JuniorCreateTogether = WCFClients.IntegralExternalClient.QuerySubUserPayRebateOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);
            }
            catch (Exception)
            {
                ViewBag.JuniorCreateTogether = null;
            }
            return View();
        }
        /// <summary>
        /// 充值排行
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySeniority()
        {
            try
            {
                //不是代理则跳转
                if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
                ViewBag.CurrentUser = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                ViewBag.AgentFillMoney = WCFClients.IntegralExternalClient.QueryAgentFillMoneyTopList(string.Empty, userId, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);

            }
            catch (Exception)
            {

                ViewBag.AgentFillMoney = null;
            }
            return View();
        }
        /// <summary>
        /// 代理销量
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentSales()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                var agentId = CurrentUser.LoginInfo.UserId;
                ViewBag.userId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
                ViewBag.key = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"];
                if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                ViewBag.AgentSales = WCFClients.ExternalClient.QueryLowerAgentSaleByUserId(agentId, ViewBag.userId, ViewBag.key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);
            }
            catch (Exception)
            {

                ViewBag.AgentSales = null;
            }

            return View();
        }
        /// <summary>
        /// 推广销量最新查询(测试阶段)
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAgentSales()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Now : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var agentId = CurrentUser.LoginInfo.UserId;
                ViewBag.userId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
                ViewBag.key = string.IsNullOrEmpty(Request["key"]) ? "" : Request["key"];
                if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                //ViewBag.AgentSales = WCFClients.ExternalClient.QueryNewLowerAgentSaleByUserId(agentId, ViewBag.userId, ViewBag.key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
            }
            catch (Exception)
            {

                ViewBag.AgentSales = null;
            }

            return View();
        }
        /// <summary>
        /// 发起中方案
        /// </summary>
        /// <returns></returns>
        public ActionResult FSchemeManage()
        {
            if (CurrentUser.LoginInfo.IsGeneralUser) return RedirectToAction("mytc");
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                //ViewBag.JuniorCreateTogether = WCFClients.ExternalClient.QuerySubUserCreateingOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
                ViewBag.JuniorCreateTogether = WCFClients.IntegralExternalClient.QuerySubUserCreateingOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
            }
            catch (Exception)
            {
                ViewBag.JuniorCreateTogether = null;
            }
            return View();
        }
        /// <summary>
        /// 经销商信息
        /// </summary>
        /// <returns></returns>
        public ActionResult DealerMsg()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.IsAddAgent = IsAddAgent;
            return View();
        }
        /// <summary>
        /// 添加代理
        /// </summary>
        /// <returns></returns>
        public JsonResult AddOCAgent()
        {
            try
            {
                string userId = Request["txtId"];
                string preantId = CurrentUser.LoginInfo.UserId;
                var result = WCFClients.ExternalClient.AddOCAgent(OCAgentCategory.GeneralAgent, preantId, userId, CPSMode.PayRebate, "");
                ClearUserBindInfoCache(userId);
                return Json(result);
                //return Json(new { IsSuccess = false, Msg = "待改" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

        }
        /// <summary>
        ///根据用户ID查询 
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryUser()
        {
            try
            {
                string userName = Request["txtDealerId"];
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                var UserMsg = WCFClients.IntegralExternalClient.QueryUserByUserName(userName, userId);
                return Json(new { IsSuccess = true, UserShow = UserMsg });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

        }

        /// <summary>
        /// 未结算方案
        /// </summary>
        /// <returns></returns>
        public ActionResult PjScheme()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                ViewBag.JuniorCompleteTogether = WCFClients.IntegralExternalClient.QuerySubUserNoPayRebateOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, pageSize);
            }
            catch (Exception)
            {
                ViewBag.JuniorCreateTogether = null;
            }
            return View();
        }
        /// <summary>
        /// 统计查询
        /// </summary>
        /// <returns></returns>
        public ActionResult FStatisticsQuery()
        {
            if (CurrentUser.LoginInfo.IsGeneralUser) return RedirectToAction("mytc");
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                //ViewBag.AgentLotto = WCFClients.ExternalClient.QueryAgentLottoTopList(string.Empty, UserToken, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                ViewBag.AgentLotto = WCFClients.IntegralExternalClient.QueryAgentLottoTopList(string.Empty, userId, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
            }
            catch (Exception)
            {

                ViewBag.AgentLotto = null;
            }

            return View();
        }
        /// <summary>
        /// 推广链接
        /// </summary>
        /// <returns></returns>
        public ActionResult PopularizeLink()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            //查询当前用户代理类型   是否为总代理  或者为  总代理的下级代理
            string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.IsAddAgent = IsAddAgent;
            ViewBag.Url_1 = ConfigurationManager.AppSettings["TGDomain_1"] + "?pid=" + CurrentUser.LoginInfo.UserId;
            ViewBag.Url_2 = ConfigurationManager.AppSettings["TGDomain_2"] + "?pid=" + CurrentUser.LoginInfo.UserId;
            return View();
        }
        /// <summary>
        /// 保存自定义连接
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveCustomerDomain()
        {
            try
            {
                var customerDomain = Request["customerDomain"];
                var result = WCFClients.ExternalClient.SetOCAgentCustomerDomain(this.CurrentUser.LoginInfo.UserId, customerDomain);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 保存设置新用户返点
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveSNUR()
        {
            try
            {
                var dataStr = Request["dataStr"];
                var UserRebate = WCFClients.ExternalClient.SetOCAgentRebate(CurrentUser.LoginInfo.UserId, dataStr);
                return Json(UserRebate);

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 保存配置用户返点
        /// </summary>
        /// <returns></returns>
        public JsonResult savePZSNUR()
        {
            try
            {
                var dataStr = Request["dataStr"];
                var id = Request["_id"];
                //UserRealNameInfo realinfo = WCFClients.ExternalClient.QueryRealNameByUserId(id);
                //UserRealNameInfo realinfo = WCFClients.IntegralExternalClient.QueryRealNameByUserId(id);
                //if (realinfo == null)
                //{
                //    PreconditionAssert.IsNotEmptyString("", "对不起，用户未进行实名认证");
                //}
                var UserRebate = WCFClients.ExternalClient.UpdateOCAgentRebate(this.CurrentUser.LoginInfo.UserId, id, dataStr);
                return Json(UserRebate);
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Message = ex.Message });
            }

        }
        /// <summary>
        /// 配置用户返点
        /// </summary>
        /// <returns></returns>
        public PartialViewResult FConfigurationUserRebateNew()
        {
            try
            {
                var userId = Request["userId"];
                var txtName = Request["txtName"];
                var typeName = Request["typeName"];
                ViewBag.TypeName = typeName;
                ViewBag.DisplayName = txtName;
                ViewBag.Id = userId;
                ViewBag.SetNewUserRebate = WCFClients.ExternalClient.QueryUserRebate(userId);
            }
            catch (Exception ex)
            {

                ViewBag.SetNewUserRebate = null;
            }
            return PartialView();
        }
        /// <summary>
        /// 设置新用户返点
        /// </summary>
        /// <returns></returns>
        public ActionResult SetNewUserRebate()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
            if (CurrentUser.LoginInfo.IsAgent)
            {
                ViewBag.AgentType = WCFClients.ExternalClient.QueryAgentType(userId);
            }
            ViewBag.IsAddAgent = IsAddAgent;
            ViewBag.InItNewUserRebate = WCFClients.ExternalClient.QueryUserRebate(userId);
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }
        /// <summary>
        /// 配置用户返点
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ConfigurationUserRebateNew()
        {
            try
            {
                var userId = Request["userId"];
                var txtName = Request["txtName"];
                var typeName = Request["typeName"];
                ViewBag.TypeName = typeName;
                ViewBag.DisplayName = txtName;
                ViewBag.Id = userId;
                ViewBag.SetNewUserRebate = WCFClients.IntegralExternalClient.QueryUserRebate(userId);
            }
            catch (Exception ex)
            {

                ViewBag.SetNewUserRebate = null;
            }
            return PartialView();
        }
        /// <summary>
        /// 推广用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneralizeUser()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.IsAddAgent = IsAddAgent;
            try
            {
                ViewBag.Key = Request["key"];
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                if (!string.IsNullOrEmpty(ViewBag.Key))
                    ViewBag.GeneralizeUser = WCFClients.IntegralExternalClient.GetAgentUserByKeyword(ViewBag.Key, 1, CurrentUser.LoginInfo.UserId, ViewBag.pageNo, 10);
                else
                    ViewBag.GeneralizeUser = WCFClients.IntegralExternalClient.GetLowerAgentUserByAgentId(CurrentUser.LoginInfo.UserId, 1, ViewBag.pageNo, 10);

            }
            catch (Exception)
            {
                ViewBag.GeneralizeUser = null;
            }
            return View();
        }
        /// <summary>
        /// 发起中方案
        /// </summary>
        /// <returns></returns>
        public ActionResult SchemeManage()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                ViewBag.JuniorCreateTogether = WCFClients.IntegralExternalClient.QuerySubUserCreateingOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);
            }
            catch (Exception)
            {
                ViewBag.JuniorCreateTogether = null;
            }
            return View();
        }
        /// <summary>
        /// 统计查询
        /// </summary>
        /// <returns></returns>
        public ActionResult StatisticsQuery()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                ViewBag.AgentLotto = WCFClients.IntegralExternalClient.QueryAgentLottoTopList(string.Empty, userId, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);
            }
            catch (Exception)
            {

                ViewBag.AgentLotto = null;
            }

            return View();
        }
        /// <summary>
        /// 我的提成
        /// </summary>
        /// <returns></returns>
        public ActionResult MyTc()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.InItNewUserRebate = WCFClients.IntegralExternalClient.QueryUserRebate(this.CurrentUser.LoginInfo.UserId);
            return View();
        }
        /// <summary>
        /// 返点记录
        /// </summary>
        /// <returns></returns>
        public ActionResult CommissionRecord()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                ViewBag.OCAgentPayDetail = WCFClients.IntegralExternalClient.QueryOCAgentPayDetailList(userId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, 10);
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        /// <summary>
        /// 下级用户
        /// </summary>
        /// <returns></returns>
        public ActionResult SubordinateUser()
        {
            try
            {
                //不是代理则跳转
                if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
                ViewBag.CurrentUser = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                ViewBag.IsAddAgent = IsAddAgent;
                ViewBag.Key = Request["key"];
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                string userId = CurrentUser == null ? string.Empty : CurrentUser.LoginInfo.UserId;
                if (!string.IsNullOrEmpty(ViewBag.Key))
                    ViewBag.GeneralizeUser = WCFClients.IntegralExternalClient.GetAgentUserByKeyword(ViewBag.Key, 0, userId, ViewBag.pageNo, 10);
                else
                    ViewBag.GeneralizeUser = WCFClients.IntegralExternalClient.GetLowerAgentUserByAgentId(userId, 0, ViewBag.pageNo, 10);

            }
            catch (Exception)
            {
                ViewBag.GeneralizeUser = null;
            }
            return View();
        }
        /// <summary>
        /// 细则说明
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailedDescription()
        {
            //不是代理则跳转
            if (!CurrentUser.LoginInfo.IsAgent) return RedirectToAction("integraldetail");
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        #endregion

        #region 返点管理
        /// <summary>
        /// 返点明细
        /// </summary>
        /// <returns></returns>
        public ActionResult IntegralDetail()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;

            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
            ViewBag.Account = string.IsNullOrEmpty(Request["accountType"]) ? "20|30" : Request["accountType"];
            ViewBag.PayTypeList = string.IsNullOrEmpty(Request["payType"]) ? "" : Request["payType"];
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            ViewBag.Fund = WCFClients.IntegralExternalClient.QueryUserFundDetail(CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.Account, "", ViewBag.PayTypeList, ViewBag.PageIndex, 10);
            return View();
        }
        /// <summary>
        /// 返点提取
        /// </summary>
        /// <returns></returns>
        public ActionResult IntegralExtract()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }

        public ActionResult DrawingsRecordFoud()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.Status = string.IsNullOrEmpty(Request.QueryString["status"]) ? null : (WithdrawStatus?)int.Parse(Request.QueryString["status"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
            ViewBag.orderId = string.IsNullOrEmpty(Request["orderId"]) ? "" : Request["orderId"];
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            ViewBag.Fund = WCFClients.IntegralExternalClient.QueryWithdrawList(CurrentUser.LoginInfo.UserId, ViewBag.Status, ViewBag.Begin, ViewBag.End, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.orderId);
            return View();
        }
        //提款验证页面
        public ActionResult Ffetchvalidate()
        {
            var bkUrl = string.IsNullOrEmpty(Request.Form["backurl"]) ? "/member/IntegralExtract" : Request.Form["backurl"];
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            Session["Repeat"] = 1;
            try
            {
                ViewBag.FetchType = PreconditionAssert.IsNotEmptyString(Request.Form["fetchType"], "提款类型错误");
                ViewBag.Money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request.Form["money"], "提款返点不能为空"));
                var money = string.IsNullOrEmpty(Request.Form["money"]) ? "" : Request.Form["money"];
                if (money.IndexOf(".") > 0)
                    throw new Exception("提款金额不能为小数");
                //最小金额
                var minmoney = "10";
                //WCFClients.IntegralClient.QueryCoreConfigByKey("Site.Financial.MinWithDrwaMoney").ConfigValue;
                PreconditionAssert.IsTrue(ViewBag.Money >= int.Parse(minmoney), "提款金额不能小于10元");

                ViewBag.FetchAccont = string.Empty;
                switch ((string)ViewBag.FetchType)
                {
                    case "alipay":
                        PreconditionAssert.IsTrue(CurrentUser.IsAuthenticationRealName && CurrentUser.IsAuthenticationMobile, "用户未进行安全认证或未绑定手机，无法进行提现操作");
                        ViewBag.FetchAccont = PreconditionAssert.IsNotEmptyString(Request.Form["alipayaccount"], "支付宝账号不能为空。");
                        if (!ValidateHelper.IsEmail(ViewBag.FetchAccont) && !ValidateHelper.IsMobile(ViewBag.FetchAccont))
                        {
                            throw new Exception("支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号）。");
                        }
                        break;
                    case "bank":
                        PreconditionAssert.IsTrue(CurrentUser.IsAuthenticationRealName && CurrentUser.IsAuthenticationMobile && CurrentUser.IsBindBank, "用户未进行安全认证或未绑定信息，无法进行提现操作");
                        ViewBag.FetchAccont = CurrentUser.BankCardInfo.BankCardNumber;
                        break;
                }

                ViewBag.RequestWithdraw_1 = WCFClients.IntegralExternalClient.RequestWithdraw_Step1(CurrentUser.LoginInfo.UserId, decimal.Parse(money));
            }
            catch (Exception ex)
            {
                Response.Redirect(bkUrl + "?errMsg=" + ex.Message);
            }
            return View();
        }
        //提款提交结果页
        public ActionResult Ffetchsubmit()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            try
            {
                ViewBag.FetchType = PreconditionAssert.IsNotEmptyString(Request.Form["fetchType"], "提款类型错误");
                var money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request.Form["money"], "提款金额不能为空"));
                ViewBag.Money = money;
                if (Session["Repeat"] == null)
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.Message = "提款成功";
                    switch ((string)ViewBag.FetchType)
                    {
                        case "alipay":
                            ViewBag.FetchAccount = PreconditionAssert.IsNotEmptyString(Request.Form["fetchAccount"], "支付宝账号不能为空。");
                            break;
                        case "bank":
                            ViewBag.FetchAccount = CurrentUser.BankCardInfo.BankCardNumber;
                            break;
                    }

                    return View();
                }

                PreconditionAssert.IsTrue(ViewBag.Money >= 10, "提款金额不能小于10元");
                //var validateCode = PreconditionAssert.IsNotEmptyString(Request.Form["validatecode"], "提款校验码不能为空");
                var balancepwd = Request["balancepwd"];
                Withdraw_RequestInfo withdrawinfo = new Withdraw_RequestInfo();

                switch ((string)ViewBag.FetchType)
                {
                    case "alipay":

                        if (!ValidateHelper.IsEmail(ViewBag.FetchAccount) && !ValidateHelper.IsMobile(ViewBag.FetchAccount))
                        {
                            throw new Exception("支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号）。");
                        }
                        withdrawinfo.BankCardNumber = ViewBag.FetchAccount;
                        withdrawinfo.BankCode = "Alipay";
                        withdrawinfo.BankName = "支付宝";
                        withdrawinfo.RequestMoney = ViewBag.Money;
                        withdrawinfo.WithdrawAgent = WithdrawAgentType.Alipay;
                        break;
                    case "bank":
                        withdrawinfo.BankCardNumber = CurrentUser.BankCardInfo.BankCardNumber;
                        withdrawinfo.BankCode = CurrentUser.BankCardInfo.BankCode;
                        withdrawinfo.BankName = CurrentUser.BankCardInfo.BankName;
                        withdrawinfo.BankSubName = CurrentUser.BankCardInfo.BankSubName;
                        withdrawinfo.CityName = CurrentUser.BankCardInfo.CityName;
                        withdrawinfo.ProvinceName = CurrentUser.BankCardInfo.ProvinceName;
                        withdrawinfo.RequestMoney = ViewBag.Money;
                        withdrawinfo.WithdrawAgent = WithdrawAgentType.Integral_BankCard;
                        break;
                    default:
                        throw new Exception("未知提款类型");
                }

                var result = WCFClients.IntegralExternalClient.RequestWithdraw_Step2(withdrawinfo, CurrentUser.LoginInfo.UserId, balancepwd);
                if (result.IsSuccess)
                {
                    LoadUerBalance();
                }
                //提款成功给财务人员发送短信
                if (money >= decimal.Parse(WithdrawMoney))
                {
                    string[] moblie = base.fina_mobile;
                    for (int i = 0; i < moblie.Length; i++)
                    {
                        //base.SendMessage(moblie[i], "会员" + CurrentUser.LoginInfo.DisplayName + "提款申请:" + money.ToString("N2") + "元,请财务人员及时处理！");
                        WCFClients.IntegralExternalClient.SendMsg(moblie[i], "财务人员请注意：用户" + CurrentUser.LoginInfo.UserId + "申请提现" + withdrawinfo.RequestMoney + "元,订单号" + result.ReturnValue + "，请及时处理。", IpManager.IPAddress, 5, CurrentUser.LoginInfo.UserId, result.ReturnValue);
                    }
                }
                ViewBag.IsSuccess = true;
                ViewBag.Message = "提款成功";
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }
            Session["Repeat"] = null;
            return View();
        }
        //返点转入
        public ActionResult IntegralInto()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public ActionResult IntegralIntoSubmit()
        {
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                var money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request.Form["money"], "转入返点不能为空"));
                ViewBag.Money = money;
                PreconditionAssert.IsTrue(ViewBag.Money >= 10, "提款返点不能小于10元");
                WCFClients.IntegralExternalClient.AgentFillMoney(CurrentUser.LoginInfo.UserId, money);
                ViewBag.IsSuccess = true;
                ViewBag.Message = "提款成功";
                LoadUerBalance();
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        //转入金额记录
        public ActionResult IntegralIntoDetail()
        {
            //ViewBag.CurrentUser = CurrentUser;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex"]) ? 0 : int.Parse(Request["pageindex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize"]) ? 10 : int.Parse(Request["pagesize"]);
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            ViewBag.Fund = WCFClients.IntegralExternalClient.QueryUserFundDetail(CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, "30", "返点转入", "10", ViewBag.PageIndex, ViewBag.PageSize);
            return View();
        }
        /// <summary>
        /// 我的返点
        /// </summary>
        /// <returns></returns>
        public ActionResult Jf()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.InItNewUserRebate = WCFClients.IntegralExternalClient.QueryUserRebate(this.CurrentUser.LoginInfo.UserId);
            return View();
        }
        /// <summary>
        /// 细则说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Jfsm()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return View();
        }
        #endregion

        //个人资料
        public ActionResult PersonalData()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        //购彩记录
        public ActionResult GcRecord()
        {
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                //ViewBag.OrderType = string.IsNullOrEmpty(Request.QueryString["orderType"]) ? OrderQueryType.All : (OrderQueryType?)int.Parse(Request.QueryString["orderType"]);
                ViewBag.BonusStatus = null;
                ViewBag.CurrentUser = CurrentUser;
                if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);
                ViewBag.BettingOrder = WCFClients.GameQueryClient.QueryMyBettingOrderList(ViewBag.BonusStatus, "", ViewBag.begin, ViewBag.end, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            }
            catch (Exception)
            {

                ViewBag.BettingOrder = null;
            }

            return View();
        }

        //账户明细
        public async Task<ActionResult> AccountDetail()
        {
            string accountType = string.IsNullOrEmpty(Request["accountType"]) ? "" : Request["accountType"];
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.AccountType = accountType;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            Dictionary<string, object> param = new Dictionary<string, object>();
            var Model = new EntityModel.RequestModel.QueryUserFundDetailParam() { viewtype = "", userid = UserToken, fromDate = ViewBag.Begin, toDate = ViewBag.End, pageIndex = ViewBag.pageNo, pageSize = ViewBag.PageSize, accountTypeList = accountType };
            param["Model"] = Model;
            ViewBag.FundDetails = await serviceProxyProvider.Invoke<EntityModel.CoreModel.UserFundDetailCollection>(param, "api/order/QueryMyFundDetailList");
            return View();
        }

        //充值记录
        public ActionResult RechargeRecord()
        {
            string accountType = string.IsNullOrEmpty(Request["accountType"]) ? "" : Request["accountType"];
            ViewBag.AccountType = accountType;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.FundDetails = WCFClients.GameQueryClient.QueryMyFundDetailList(ViewBag.Begin, ViewBag.End, accountType, "", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            ViewBag.CurrentUser = CurrentUser;
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            ViewBag.FillMoneyCollection = WCFClients.GameQueryClient.QueryMyFillMoneyList("1", ViewBag.Begin, ViewBag.End.AddDays(1), ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }

        /// <summary>
        /// 转移充值列表
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferFillMoneyList()
        {
            string accountType = string.IsNullOrEmpty(Request["accountType"]) ? "" : Request["accountType"];
            ViewBag.AccountType = accountType;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.FundDetails = WCFClients.GameQueryClient.QueryMyFundDetailList(ViewBag.Begin, ViewBag.End, accountType, "", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            ViewBag.CurrentUser = CurrentUser;
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);
            ViewBag.FillMoneyCollection = WCFClients.GameQueryClient.QueryMyFillMoneyListByCzzy("1", ViewBag.Begin, ViewBag.End.AddDays(1), ViewBag.pageNo, ViewBag.PageSize, this.CurrentUser.LoginInfo.UserId);
            return View();
        }
        public ActionResult TransferFillMoney()
        {

            return View();
        }
        public JsonResult TransferFillMoneyPay()
        {
            try
            {
                decimal money = decimal.Parse(Request["payMoney"]);//转移金额
                var type = Request["payType"];//转移账户类型
                var userId = Request["userId"];//用户名或者用户id
                var nameType = Request["nameType"];//用户名或者用户id类型

                //限制金额
                if (money > Max_FillMoney)
                    return Json(new { IsSuccess = false, Message = string.Format("单笔最高限额{0}", Max_FillMoney) }, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrWhiteSpace(nameType.ToString()))
                    return Json(new { IsSuccess = false, Message = "请选择用户名或者用户id" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(userId.ToString()))
                    return Json(new { IsSuccess = false, Message = "转移到某个用户id不能为空" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(money.ToString()) || money <= 0)
                    return Json(new { IsSuccess = false, Message = "转移金额不能为空且不能小于等于0" }, JsonRequestBehavior.AllowGet);

                if (!new string[] { "50" }.Contains(type))
                    return Json(new { IsSuccess = false, Message = "错误的充值类型" }, JsonRequestBehavior.AllowGet);
                //Thread.Sleep(10000);
                string uId = string.Empty;//转移对象的id
                if (nameType == "uName")
                {
                    uId = WCFClients.ExternalClient.GetUserIdByLoginName(userId);
                    if (string.IsNullOrEmpty(uId))
                    {
                        return Json(new { IsSuccess = false, Message = "用户名不存在" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    uId = userId;
                }
                if (uId == this.CurrentUser.LoginInfo.UserId)
                {
                    return Json(new { IsSuccess = false, Message = "禁止自己给自己转移" }, JsonRequestBehavior.AllowGet);
                }

                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = "充值专员充值";
                fillMoneyInfo.GoodsType = "转移充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.czzy;
                string agentid = string.Empty;
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, uId, this.CurrentUser.LoginInfo.UserId);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                CommonActionResult car = WCFClients.GameFundClient.CompleteFillMoneyOrderByCzzy(orderId, FillMoneyStatus.Success, money, "1", string.Empty, this.CurrentUser.LoginInfo.UserId, type);

                return Json(new { IsSuccess = true, Message = car.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 名家
        //中奖记录
        public ActionResult WinningRecord()
        {

            ViewBag.GameCode = string.IsNullOrEmpty(Request.QueryString["gameCode"]) ? string.Empty : Request.QueryString["gameCode"].ToString();
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            if (ViewBag.Begin < DateTime.Now.AddMonths(-1))
                ViewBag.Begin = DateTime.Now.AddMonths(-1);

            ViewBag.BonusDetails = WCFClients.GameQueryClient.QueryMyFundDetailList(ViewBag.Begin, Convert.ToDateTime(ViewBag.End).AddDays(1), "10", "奖金", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        //提款记录
        public ActionResult DrawingRecords()
        {
            string accountType = string.IsNullOrEmpty(Request["accountType"]) ? "" : Request["accountType"];
            ViewBag.AccountType = accountType;
            ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
            ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.FundDetails = WCFClients.GameQueryClient.QueryMyFundDetailList(ViewBag.Begin, ViewBag.End, accountType, "", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        //修改名家资料
        public ActionResult UpdateExpert()
        {
            //ViewBag.User = CurrentUser;
            ViewBag.User = WCFClients.ExperterClient.QueryExperterById(CurrentUser.LoginInfo.UserId);
            //提交修改名家资料
            if (Request.HttpMethod.ToUpper() == "POST")
            {
                var cz = string.Empty;
                var cz1 = Request["cz"];
                switch (cz1)
                {
                    case "JCLQ":
                        cz = "竞彩足球/" + "竞彩篮球";
                        break;
                    case "BJDC":
                        cz = "竞彩足球/" + "北京单场";
                        break;
                    case "CTZQ":
                        cz = "竞彩足球/" + "传统足球";
                        break;
                }
                var miaoshu = Request["textArea"];

                HttpPostedFileBase file = Request.Files["flPhoto"];
                //设置新的文件名
                var fileExt = Path.GetExtension(file.FileName).ToLower();//获取扩展名
                var name = Guid.NewGuid().ToString("d");
                var newFileName = name + fileExt;
                //获取文件保存路径
                var postUrl = System.Configuration.ConfigurationManager.AppSettings["ResSiteUrl"] + "/iqucai/UpLoad/upload_json.aspx";//后台接收参数的路径
                postUrl = string.Format(postUrl + "?filename={0}&upLoadFile={1}", newFileName, "/iqucai/UpLoad/add/experter/");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(respon.GetResponseStream(), Encoding.UTF8);
                var siteUrl = reader.ReadToEnd();
                var upLoadFile = "iqucai/UpLoad/add/experter";
                var url = Request.Url.Port == 80 ? string.Format("http://{0}/{1}/{2} ", request.Address.Authority, upLoadFile, newFileName)
                    : string.Format("http://{0}:{1}/{2}/{3}", request.Address.Host, request.Address.Port, upLoadFile, newFileName);

                try
                {
                    var result = WCFClients.ExperterClient.UpdateExperter(this.CurrentUser.LoginInfo.UserId, url, miaoshu, cz);
                    return Content("<script type='text/javascript'>alert('提交成功，请等待审核结果！');window.location.href='/member/UpdateExpert'</script>");
                }
                catch (Exception ex)
                {
                    return Content("<script type='text/javascript'>alert('" + ex.Message + "');window.location.href='/member/UpdateExpert'</script>");
                }
            }
            return View();
        }
        //查询资料修改记录
        public ActionResult QueryUpdateRecord()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.UpdateRecord = WCFClients.ExperterClient.QueryExperterUpdateList(this.CurrentUser.LoginInfo.UserId, ViewBag.pageNo, ViewBag.PageSize);
            return View();
        }
        //发布心水推荐
        public ActionResult AddExpertAnalyze()
        {
            return View();
        }
        //保存心水推荐
        [ValidateInput(false)]
        public JsonResult SaveExpertAnalyze(string title, string content, string price)
        {
            try
            {
                //var title = Request["title"];
                //var content = Request["content"];
                //var price = Request["price"];
                if (title == null || content == null || price == null)
                {
                    return Json(new { IsSucess = true, Msg = "请将信息填写完整！" });
                }

                var Analyze = WCFClients.ExperterClient.AddExperterAnalyzeScheme(new ExperterAnalyzeSchemeInfo
                {
                    UserId = CurrentUser.LoginInfo.UserId,
                    Price = decimal.Parse(price),
                    Title = title,
                    Content = content,
                });
                return Json(new { IsSucess = true, Msg = "提交成功，请等待后台审核！" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        //查询心水记录
        public ActionResult QueryExpertRecord()
        {
            ViewBag.User = CurrentUser;
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.ExpertRecord = WCFClients.ExperterClient.QueryUserAnalyzeList(this.CurrentUser.LoginInfo.UserId, "", ViewBag.pageNo, ViewBag.pageSize);
            return View();
        }
        //查询竞彩方案记录
        public ActionResult QueryJcRecord()
        {
            ViewBag.User = CurrentUser;
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.JcRecord = WCFClients.ExperterClient.QueryExperterHistorySchemeList(this.CurrentUser.LoginInfo.UserId, ViewBag.pageNo, ViewBag.pageSize);
            return View();
        }
        //发布竞彩方案
        public ActionResult Jcplan()
        {
            //if (CurrentUser == null || !CurrentUser.LoginInfo.IsExperter)
            //    return RedirectToAction("default", "home");
            return View();
        }

        #endregion

        public JsonResult redireAlipayUrl()
        {
            try
            {
                ViewBag.IsSuccess = true;
                string paytype = PreconditionAssert.IsNotEmptyString(Request["payType"], "充值类型字符串不能为空！");
                decimal money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["money"], "充值金额不能为空！"));
                var isFillMoney = WCFClients.ExternalClient.CurrUserIsFillMoney(CurrentUser.LoginInfo.UserId);
                if (!isFillMoney)
                    PreconditionAssert.IsNotEmptyString("", "对不起，本站已暂停彩票代购业务，给您带来的不便请谅解！");

                var LowestFillMoney = Convert.ToDecimal(WCFClients.GameClient.QueryCoreConfigByKey("LowestFillMoney"));
                if (money < LowestFillMoney)
                    PreconditionAssert.IsNotEmptyString("", string.Format("您好，充值金额最低不能低于{0}元", LowestFillMoney));
                ViewBag.Message = money;

                PreconditionAssert.IsTrue(paytype.Contains("|"), "充值类型字符串必须包含充值接口类型和充值银行编码！");

                #region 创建本地订单对象

                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.Alipay;
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = AgentHostMappingConfigAnalyzer.GetSiteNameByHostName(Request.Url.Host) +
                                                 "在线网银充值 - 充值账户Key：" + CurrentUser.LoginInfo.UserId;
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                string orderId = string.Empty; //本地生成订单后的订单号

                #endregion

                #region 向本地系统添加订单，返回订单号

                // fillMoneyInfo.ShowUrl = show_url;
                // fillMoneyInfo.ReturnUrl = return_url;
                // fillMoneyInfo.NotifyUrl = notify_url;

                var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                if (alipayAddOrderResult.ReturnValue.Contains('|'))
                {
                    orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                }
                else
                {
                    orderId = alipayAddOrderResult.ReturnValue;
                }

                #endregion

                //业务参数赋值
                string out_trade_no = orderId; //客户自己的订单号，(现取系统时间，可改成网站自己的变量)，订单号必须在自身订单系统中保持唯一性
                string total_fee = fillMoneyInfo.RequestMoney.ToString(); //商品价格，也可称为订单的总金额   0.01-50000.00
                ViewBag.CurrentUser = CurrentUser;
                //return Redirect(alipayurl);
                return Json(new { IsSucess = true, PayMoney = total_fee, OrderId = out_trade_no });
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                return Json(new { IsSucess = false, PayMoney = 0, OrderId = "", Message = ex.Message });
            }
        }

        /// <summary>
        /// 功能：设置商品有关信息（入口页）
        /// 包含支付宝支付和易宝支付，需要增加本地订单
        /// </summary>
        public ActionResult Redirectpay()
        {
            try
            {
                ViewBag.IsSuccess = true;

                string paytype = PreconditionAssert.IsNotEmptyString(Request["payType"], "充值类型字符串不能为空！");
                decimal money =
                    decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["money"], "充值金额不能为空！"));

                //#region 判断是否为允许的三方网站，如果是允许的三方网站则中转
                //var partnerHost = SettingConfigAnalyzer.GetConfigValueByKey("PartnerHost").Split(',');
                //if (partnerHost.Contains(Request.Url.Host))
                //{
                //    var requestString = "token=" + Server.UrlEncode(UserToken) + "&payType=" + paytype + "&money=" + money;
                //    Response.Redirect(SettingConfigAnalyzer.GetConfigValueByKey("PartnerHost", "RedirectUrl") + requestString + "&sign=" + Encipherment.MD5(requestString));
                //}
                //#endregion
                //if (!CurrentUser.IsAuthenticationMobile || !CurrentUser.IsAuthenticationRealName)
                //    PreconditionAssert.IsNotEmptyString("", "充值前必须实名认证和手机认证！");
                //var isFillMoney = WCFClients.ExternalClient.CurrUserIsFillMoney(CurrentUser.LastLoginInfo.UserId);
                //if (!isFillMoney)
                //    PreconditionAssert.IsNotEmptyString("", "对不起，本站已暂停彩票代购业务，给您带来的不便请谅解！");

                var LowestFillMoney =
                    Convert.ToDecimal(WCFClients.GameClient.QueryCoreConfigByKey("LowestFillMoney"));
                if (money < LowestFillMoney)
                    PreconditionAssert.IsNotEmptyString("", string.Format("您好，充值金额最低不能低于{0}元", LowestFillMoney));
                ViewBag.Message = money;

                PreconditionAssert.IsTrue(paytype.Contains("|"), "充值类型字符串必须包含充值接口类型和充值银行编码！");
                string gateway = paytype.Split('|')[0];
                string subpayType = paytype.Split('|')[1];
                // string code = paytype.Split('|')[2];

                #region 创建本地订单对象

                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.Alipay;
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription =
                    AgentHostMappingConfigAnalyzer.GetSiteNameByHostName(Request.Url.Host) + "在线网银充值 - 充值账户Key：" +
                    CurrentUser.LoginInfo.UserId;
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                string orderId = string.Empty; //本地生成订单后的订单号

                #endregion

                switch (gateway)
                {
                    case "alipay":

                        #region 支付宝充值

                        {
                            #region 支付宝充值参数

                            string show_url = SiteRoot; //展示地址，即在支付页面时，商品名称的链接地址。
                            //服务器返回url（Alipay_Return.aspx文件所在路经），必须是完整的路径地址
                            string return_url = SiteRoot + "/user/AlipayReturnUrl"; //改成自己服务器实际的绝对路径
                            //服务器通知url（Alipay_Notify.aspx文件所在路经），必须是完整的路径地址
                            string notify_url = SiteRoot + "/user/AlipayNotifyUrl"; //改成自己服务器实际的绝对路径
                            ////用户充值方式
                            string payType = subpayType;
                            ////扩展功能参数——网银提前
                            ////默认支付方式，四个值可选：bankPay(网银); cartoon(卡通); directPay(余额); CASH(网点支付)
                            string paymethod = "bankPay";

                            ////默认网银代号，代号列表见http://club.alipay.com/read.php?tid=8681379
                            string defaultbank = subpayType;

                            string qr_pay_moe = "";
                            if (payType == "alipay" || payType == "alipaysm")
                            {
                                paymethod = "directPay";
                                defaultbank = "";
                            }
                            //扫码支付 ：普通跳转扫码
                            if (payType == "alipaysm")
                            {
                                qr_pay_moe = "2";
                            }

                            #endregion

                            #region 向本地系统添加订单，返回订单号

                            // fillMoneyInfo.ShowUrl = show_url;
                            // fillMoneyInfo.ReturnUrl = return_url;
                            // fillMoneyInfo.NotifyUrl = notify_url;

                            var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (alipayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = alipayAddOrderResult.ReturnValue;
                            }

                            #endregion

                            //业务参数赋值
                            string out_trade_no = orderId; //客户自己的订单号，(现取系统时间，可改成网站自己的变量)，订单号必须在自身订单系统中保持唯一性
                            string subject = fillMoneyInfo.GoodsName; //商品名称，也可称为订单名称，该接口并不是单一的只能买一样东西，可把一次支付当作一次下订单
                            string body = fillMoneyInfo.GoodsDescription; //商品描述，即备注
                            string total_fee = fillMoneyInfo.RequestMoney.ToString(); //商品价格，也可称为订单的总金额   0.01-50000.00

                            string token = "";
                            if (CurrentUser.LoginInfo.LoginFrom == "ALIPAY" && Session["AlipayToken"] != null)
                            {
                                token = Session["AlipayToken"].ToString();
                            }
                            //个人版
                            //var aliay_url = bindAlipayurl(total_fee, out_trade_no);
                            //生成支付URL企业版
                            AlipaySign aliSign = new AlipaySign(ali_Partner, ali_key, ali_Seller_Email); //实例化提交对象
                            string aliay_url = aliSign.CreateDirectPayByUser(out_trade_no, subject, body, total_fee,
                                show_url, return_url, notify_url, paymethod, defaultbank, token, qr_pay_moe);

                            //跳转到支付宝支付页面
                            Response.Redirect(aliay_url);
                            //Response.Write(aliay_url);
                        }

                        #endregion

                        break;
                    case "tenpay":

                        #region 财付通充值

                        {
                            //服务器返回url（TenpayReturnUrl.aspx文件所在路经），必须是完整的路径地址
                            string return_url_tenpay = SiteRoot + "/User/TenpayReturnUrl"; //改成自己服务器实际的绝对路径
                            //服务器通知url（TenpayNotifyUrl.aspx文件所在路经），必须是完整的路径地址
                            string notify_url_tenpay = SiteRoot + "/User/TenpayNotifyUrl"; //改成自己服务器实际的绝对路径

                            #region 向本地系统里添加订单，并返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.Tenpay;
                            fillMoneyInfo.ShowUrl = return_url_tenpay;
                            fillMoneyInfo.ReturnUrl = return_url_tenpay;
                            fillMoneyInfo.NotifyUrl = notify_url_tenpay;

                            var tenpayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (tenpayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = tenpayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = tenpayAddOrderResult.ReturnValue;
                            }

                            #endregion

                            //当前时间 yyyyMMdd
                            string date = DateTime.Now.ToString("yyyyMMdd");
                            //订单号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                            string out_trade_no_tenpay = orderId;

                            //创建RequestHandler实例
                            RequestHandler reqHandler = new RequestHandler(HttpContext);
                            //初始化
                            reqHandler.init();
                            //设置密钥
                            reqHandler.setKey(tenpay_TenpayKey);
                            reqHandler.setGateUrl("https://gw.tenpay.com/gateway/pay.htm");

                            //-----------------------------
                            //设置支付参数
                            //-----------------------------
                            reqHandler.setParameter("total_fee", (money * 100).ToString("0"));
                            //用户的公网ip,测试时填写127.0.0.1,只能支持10分以下交易
                            reqHandler.setParameter("spbill_create_ip", IpManager.IPAddress);
                            reqHandler.setParameter("return_url", return_url_tenpay);
                            reqHandler.setParameter("partner", tenpay_Partner); //商户号
                            reqHandler.setParameter("out_trade_no", out_trade_no_tenpay);
                            reqHandler.setParameter("notify_url", notify_url_tenpay);
                            //reqHandler.setParameter("attach", "123");
                            reqHandler.setParameter("body", HttpUtility.HtmlEncode("彩金"));
                            reqHandler.setParameter("bank_type", "DEFAULT");

                            //系统可选参数
                            reqHandler.setParameter("sign_type", "MD5");
                            reqHandler.setParameter("service_version", "1.0");
                            reqHandler.setParameter("input_charset", "UTF-8");
                            reqHandler.setParameter("sign_key_index", "1");

                            //业务可选参数
                            /*
                                reqHandler.setParameter("attach", "");
                                reqHandler.setParameter("product_fee", "");
                                reqHandler.setParameter("transport_fee", "");
                                reqHandler.setParameter("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
                                reqHandler.setParameter("time_expire", "");
       
                                reqHandler.setParameter("buyer_id", "");
                                reqHandler.setParameter("goods_tag", "");
                                reqHandler.setParameter("agentid", "");
                                reqHandler.setParameter("agent_type", "");
                                 */


                            //获取请求带参数的url
                            string tenpayRequestUrl = reqHandler.getRequestURL();
                            //跳转到财付通支付页面
                            Response.Redirect(tenpayRequestUrl);

                            //post实现方式
                            /*
                                Response.Write("<form method=\"post\" action=\""+ reqHandler.getGateUrl() + "\" >\n");
                                Hashtable ht = reqHandler.getAllParameters();
                                foreach(DictionaryEntry de in ht) 
                                {
                                    Response.Write("<input type=\"hidden\" name=\"" + de.Key + "\" value=\"" + de.Value + "\" >\n");
                                }
                                Response.Write("<input type=\"submit\" value=\"财付通支付\" >\n</form>\n");
                                */

                            //获取debug信息,建议把请求和debug信息写入日志，方便定位问题
                            //string debuginfo = reqHandler.getDebugInfo();
                            //Response.Write("<br/>requestUrl:" + requestUrl + "<br/>");
                            //Response.Write("<br/>debuginfo:" + debuginfo + "<br/>");
                        }

                        #endregion

                        break;
                    case "kuaiqian":

                        #region 快钱充值

                        {
                            //服务器返回url（KQReturnUrl.aspx文件所在路经），必须是完整的路径地址
                            string return_url_kq = SiteRoot + "/User/KQShow"; //改成自己服务器实际的绝对路径
                            //服务器通知url（KQNotifyUrl.aspx文件所在路经），必须是完整的路径地址
                            string notify_url_kq = SiteRoot + "/User/KQNotifyUrl"; //改成自己服务器实际的绝对路径

                            //充值类型 00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）
                            var payType = "00";

                            #region 向本地系统里添加订单，并返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.KuanQian;
                            fillMoneyInfo.ShowUrl = return_url_kq;
                            fillMoneyInfo.ReturnUrl = notify_url_kq;
                            fillMoneyInfo.NotifyUrl = notify_url_kq;

                            var kqAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (kqAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = kqAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = kqAddOrderResult.ReturnValue;
                            }

                            #endregion

                            if (subpayType == "kuaiqian")
                            {
                                payType = "00";
                                subpayType = "";
                            }
                            else
                            {
                                payType = "10";
                            }

                            string orderTime = DateTime.Now.ToString("yyyyMMddHHmmss"); //订单时间
                            string out_trade_no_kq = orderId;

                            KQHandler.SetKQConfig(kq_MerchantAcctId, kq_CertificatePW);
                            string kqRequestUrl = KQHandler.GreateRequestUrl_MD5(CurrentUser.LoginInfo.DisplayName, "",
                                orderId, "", fillMoneyInfo.GoodsName, fillMoneyInfo.GoodsDescription,
                                (money * 100).ToString("0"), "", notify_url_kq, orderTime, payType, "0", subpayType, "", "",
                                "", "1", "1", false);
                            //跳转到快钱支付页面
                            Response.Redirect(kqRequestUrl);
                        }

                        #endregion

                        break;
                    case "wap":

                        #region 手机浏览网站支付

                        {
                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.AlipayWAP;
                            fillMoneyInfo.GoodsName = "WAP彩金";

                            #region 向本地系统里添加订单，并返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.AlipayWAP;
                            fillMoneyInfo.ShowUrl = SiteRoot;
                            fillMoneyInfo.ReturnUrl = SiteRoot + "/user/wapalipayreturn";
                            fillMoneyInfo.NotifyUrl = SiteRoot + "/user/wapalipaynotify";
                            var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (alipayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = alipayAddOrderResult.ReturnValue;
                            }

                            #endregion

                            //初始化Service
                            var ali = new Common.Gateway.Alipay.WAPPay.Service(ali_Partner, ali_key, ali_Seller_Email);


                            //创建交易接口
                            string token = ali.alipay_wap_trade_create_direct(fillMoneyInfo.GoodsName, orderId,
                                fillMoneyInfo.RequestMoney.ToString("f"), fillMoneyInfo.NotifyUrl,
                                CurrentUser.LoginInfo.UserId, "", fillMoneyInfo.ReturnUrl, orderId);

                            //构造，重定向URL
                            string url = ali.alipay_Wap_Auth_AuthAndExecute(fillMoneyInfo.ReturnUrl, token);
                            //跳转收银台支付页面
                            Response.Redirect(url);
                        }

                        #endregion

                        break;
                    case "yinbao":

                        #region 银宝充值

                        {
                            string return_url = SiteRoot + "/user/yinbaoReturnUrl"; //改成自己服务器实际的绝对路径

                            #region 向本地系统添加订单，返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.YingBao;
                            fillMoneyInfo.ShowUrl = "";
                            fillMoneyInfo.ReturnUrl = return_url;
                            fillMoneyInfo.NotifyUrl = "";

                            var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (alipayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = alipayAddOrderResult.ReturnValue;
                            }

                            #endregion

                            YinBaoSign sign = new YinBaoSign(yinbao_id, yinbao_key);
                            var ybUrl = sign.CreateDirectPayUrl(orderId, subpayType, fillMoneyInfo.GoodsName,
                                money.ToString(), fillMoneyInfo.GoodsType, return_url);

                            //跳转到支付页面
                            Response.Redirect(ybUrl);
                        }

                        #endregion

                        break;
                    case "chinabank":

                        #region 网银在线支付

                        {
                            //商户编号
                            //var v_mid = "23075914";
                            //消费者完成购物后页面返回的商户页面,URL参数是以http://开头的完整URL地址
                            var v_url = SiteRoot + "/user/chinabankreturnurl";
                            var notify_url = SiteRoot + "/user/chinabanknotifyurl"; //异步通知地址
                            //订单编号
                            var v_oid = "";
                            //金额
                            var v_amount = money;
                            //币种
                            //var v_moneytype = "CNY";
                            //key(商户编号订单编号金额币种url+key的md5字符串)
                            //var v_md5info = "";
                            // 商户自定义返回接收支付结果的页面
                            // MD5密钥要跟订单提交页相同，如Send.asp里的 key = "test" ,修改""号内 test 为您的密钥
                            // string key = "96763deb3f8dd25447d8f8c12648aad1";// 如果您还没有设置MD5密钥请登陆我们为您提供商户后台，地址：https://merchant3.chinabank.com.cn/
                            //服务器返回url（Alipay_Return.aspx文件所在路经），必须是完整的路径地址

                            //支付网银ID
                            string bankid = subpayType;

                            #region 向本地系统添加订单，返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.ChinaPay;
                            fillMoneyInfo.ShowUrl = v_url;
                            fillMoneyInfo.ReturnUrl = v_url;
                            fillMoneyInfo.NotifyUrl = notify_url;

                            var chinabankAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (chinabankAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = chinabankAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = chinabankAddOrderResult.ReturnValue;
                            }

                            #endregion

                            v_oid = orderId;

                            ////生成支付html并提交
                            //var cb = new ChinaBank(cb_mid, cb_key);
                            var cb = new ChinaBank("test", "test");
                            var submitHtml = cb.BuildFormHtml(orderId, fillMoneyInfo.RequestMoney.ToString(), bankid, v_url,
                                payVerifyString, notify_url);
                            Response.Write(submitHtml);
                        }

                        #endregion

                        break;
                    case "yijifu":

                        #region 易极付充值

                        {
                            string show_url = SiteRoot; //展示地址，即在支付页面时，商品名称的链接地址。
                            //服务器返回url（Alipay_Return.aspx文件所在路经），必须是完整的路径地址
                            string return_url = SiteRoot + "/user/yjfreturnurl?money=" + money + "&username=" +
                                                CurrentUser.LoginInfo.LoginName + "&"; //改成自己服务器实际的绝对路径
                            //服务器通知url（Alipay_Notify.aspx文件所在路经），必须是完整的路径地址
                            string notify_url = SiteRoot + "/user/yjfnotifyurl?money=" + money + "&username=" +
                                                CurrentUser.LoginInfo.LoginName; //改成自己服务器实际的绝对路径
                            //用户充值方式
                            string payType = subpayType;
                            //扩展功能参数——网银提前
                            //默认支付方式，四个值可选：bankPay(网银); cartoon(卡通); directPay(余额); CASH(网点支付)
                            string paymethod = "bankPay";

                            //默认网银代号，代号列表见http://club.alipay.com/read.php?tid=8681379
                            string defaultbank = subpayType;

                            #region 向本地系统添加订单，返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.YiJiFu;
                            fillMoneyInfo.ShowUrl = show_url;
                            fillMoneyInfo.ReturnUrl = return_url;
                            fillMoneyInfo.NotifyUrl = notify_url;
                            fillMoneyInfo.CustomerOrderId = YJFService.OrderNo;
                            orderId = fillMoneyInfo.CustomerOrderId;
                            var res = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);

                            #endregion

                            //业务参数赋值
                            string out_trade_no = orderId; //客户自己的订单号，(现取系统时间，可改成网站自己的变量)，订单号必须在自身订单系统中保持唯一性
                            string subject = fillMoneyInfo.GoodsName; //商品名称，也可称为订单名称，该接口并不是单一的只能买一样东西，可把一次支付当作一次下订单
                            string body = fillMoneyInfo.GoodsDescription; //商品描述，即备注
                            string total_fee = fillMoneyInfo.RequestMoney.ToString(); //商品价格，也可称为订单的总金额   0.01-50000.00

                            //生成支付URL
                            string url = YJFService.FastPay(out_trade_no, CurrentUser.LoginInfo.LoginName,
                                CurrentUser.IsAuthenticationRealName ? CurrentUser.RealNameInfo.RealName : "xinticai", "ID",
                                CurrentUser.IsAuthenticationRealName ? CurrentUser.RealNameInfo.IdCardNumber : "100001", "m",
                                CurrentUser.IsAuthenticationMobile ? CurrentUser.MobileInfo.Mobile : "13000000000",
                                payType.ToLower(), "GOODS_BUY", total_fee, yjf_partnerid, "20131028-xfpxuntiPC", return_url,
                                notify_url, "");
                            //跳转到支付页面
                            Response.Redirect(url);
                        }
                        break;

                        #endregion

                    case "bifubao": //2014.11.17 dj 新加

                        #region 币付宝

                        {
                            var bbPayUrl = System.Configuration.ConfigurationManager.AppSettings["BBPayUrl"] ?? "";
                            var bbPayID = WCFClients.GameClient.QueryCoreConfigByKey("BiFuBao.ID").ConfigValue;
                            var bbPayKey = WCFClients.GameClient.QueryCoreConfigByKey("BiFuBao.Key").ConfigValue;
                            var callBackAdress = System.Configuration.ConfigurationManager.AppSettings["CallBackAdress"] ??
                                                 "";

                            if (string.IsNullOrEmpty(bbPayUrl))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "未查询到支付地址!";
                                break;
                            }
                            else if (bbPayID == null || string.IsNullOrEmpty(bbPayID))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "未查询到商户ID!";
                                break;
                            }
                            else if (bbPayKey == null || string.IsNullOrEmpty(bbPayKey))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "未查询到商户Key值!";
                                break;
                            }
                            else if (string.IsNullOrEmpty(callBackAdress))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "未查询到回调地址!";
                                break;
                            }
                            Common.Gateway.BiFuBao.BBPayAPI api = new Common.Gateway.BiFuBao.BBPayAPI();
                            callBackAdress = SiteRoot + callBackAdress;

                            #region 向本地系统添加订单，返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.BiFuBao;
                            fillMoneyInfo.ShowUrl = "";
                            fillMoneyInfo.ReturnUrl = callBackAdress;
                            fillMoneyInfo.NotifyUrl = "";

                            var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (alipayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = alipayAddOrderResult.ReturnValue;
                            }

                            #endregion

                            Common.Gateway.BiFuBao.BBPayAPIInfo info = new Common.Gateway.BiFuBao.BBPayAPIInfo();
                            info.p1_md = 1;
                            info.p2_xn = orderId;
                            info.p3_bn = bbPayID;
                            info.p4_pd = subpayType;
                            info.p5_name = fillMoneyInfo.GoodsName;
                            info.p6_amount = money;
                            info.p7_cr = 1;
                            info.p8_ex = fillMoneyInfo.GoodsType;
                            info.p9_url = callBackAdress;
                            info.p10_reply = 1;
                            info.p11_mode = 2;
                            info.p12_ver = 1;
                            info.BBPayUrl = bbPayUrl;
                            info.BBPayKey = bbPayKey;
                            var formHtml = api.CreateBBPayForm(info);
                            if (string.IsNullOrEmpty(formHtml))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "跳转地址失败";
                                break;
                            }
                            Response.Write(formHtml);
                            break;
                        }

                        #endregion

                    case "bbpay":

                        #region 新版币币支付

                        {
                            var bbPayUrl = System.Configuration.ConfigurationManager.AppSettings["BBPayNewUrl"] ?? "";
                            var bbPayID = WCFClients.GameClient.QueryCoreConfigByKey("NewBiFuBao.ID").ConfigValue;
                            var bbPayKey = WCFClients.GameClient.QueryCoreConfigByKey("NewBiFuBao.Key").ConfigValue;
                            var callBackAdress = System.Configuration.ConfigurationManager.AppSettings["NewCallBackAdress"] ?? "";
                            var callBackReturnurl = System.Configuration.ConfigurationManager.AppSettings["NewCallBackReturnurl"] ?? "";

                            #region 向本地系统添加订单，返回订单号

                            fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.BiFuBao;
                            fillMoneyInfo.ShowUrl = "";
                            fillMoneyInfo.ReturnUrl = callBackAdress;
                            fillMoneyInfo.NotifyUrl = "";

                            var alipayAddOrderResult = WCFClients.GameFundClient.UserFillMoney(fillMoneyInfo, UserToken);
                            if (alipayAddOrderResult.ReturnValue.Contains('|'))
                            {
                                orderId = alipayAddOrderResult.ReturnValue.Split('|')[0];
                            }
                            else
                            {
                                orderId = alipayAddOrderResult.ReturnValue;
                            }

                            #endregion
                            Common.Gateway.BBPay.BBPayAPI api = new Common.Gateway.BBPay.BBPayAPI();
                            Common.Gateway.BBPay.BBPayAPIInfo info = new Common.Gateway.BBPay.BBPayAPIInfo();
                            info.BBPayUrl = bbPayUrl;
                            info.BBPayKey = bbPayKey;
                            info.amount = Convert.ToInt32(money) * 100;
                            // info.amount = Convert.ToInt32(1)*100;
                            info.areturl = callBackAdress;
                            info.sreturl = callBackReturnurl;
                            info.order = orderId;
                            info.productdesc = fillMoneyInfo.GoodsDescription;
                            info.productname = fillMoneyInfo.GoodsName;
                            info.productcategory = "8";
                            info.productcount = 1;
                            info.identityid = CurrentUser.LoginInfo.UserId;
                            info.identitytype = "0";
                            info.pnc = int.Parse(subpayType);
                            info.currency = 1;
                            info.userip = IpManager.IPAddress;
                            info.userua = Request.ServerVariables["HTTP_USER_AGENT"];
                            info.transtime = GetTimeStamp();
                            info.BBPayId = bbPayID;

                            var formHtml = api.Pay(info);
                            if (string.IsNullOrEmpty(formHtml))
                            {
                                ViewBag.IsSuccess = false;
                                ViewBag.Message = "跳转地址失败";
                                break;
                            }
                            Response.Write(formHtml);
                        }

                        #endregion

                        break;
                    default:
                        throw new Exception("不支持该充值接口类型：" + paytype);
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;


            }
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }


        public static long GetTimeStamp()
        {
            System.DateTime startTime = new System.DateTime(1970, 1, 1);
            var transtime = (long)((DateTime.Now.AddHours(-8) - startTime).TotalMilliseconds);
            return transtime;
        }

        #region 用户信息绑定、安全认证、隐藏用户名
        //手机认证-申请 执行程序，认证完成后跳转到认证来源页面
        public JsonResult authmobile_request()
        {
            var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? SiteRoot : Server.UrlDecode(Request["backurl"]);
            // var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? "/member/@UserHelper.AuthMobile_Submit(requestInfo)" : Server.UrlDecode(Request["backurl"]);
            try
            {
                string mobile = PreconditionAssert.IsNotEmptyString(Request["mobile"], "手机号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
                var requestType = Request["RequestType"];
                CommonActionResult result = null;
                if (Request["RequestType"] != null && Request["RequestType"].ToString() == "1")//当用户重发验证码或者更换手机号
                    result = WCFClients.ExternalClient.RepeatRequestMobile(CurrentUser.LoginInfo.UserId, mobile, CurrentUser.LoginInfo.UserId);
                else
                    result = WCFClients.ExternalClient.RequestAuthenticationMobile(mobile, UserToken);

                return Json(new { Succuss = true, Msg = result.Message, mobile = mobile });

            }
            catch (Exception ex)
            {
                //Response.Redirect(bkUrl + "?errMsg=" + ex.Message);

                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        //手机认证-提交执行程序，认证完成后跳转到认证来源页面
        public JsonResult authmobile_submit()
        {
            var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? SiteRoot : Server.UrlDecode(Request["backurl"]);
            try
            {
                var isnew = Session["isnew"] == null ? false : bool.Parse(Session["isnew"].ToString());
                bkUrl = isnew ? "/member/GoPay" : bkUrl;

                //  string mobileCode = PreconditionAssert.IsNotEmptyString(Request.Form["mobileCode"], "校验码不能为空。");
                string mobileCode = PreconditionAssert.IsNotEmptyString(Request["mobileCode"], "校验码不能为空。");
                var result = WCFClients.ExternalClient.ResponseAuthenticationMobile(mobileCode, SchemeSource.Web, UserToken);

                if (result.IsSuccess)
                {
                    //DelCacheMobileInfo();
                    LoadUserLeve("mobile");
                    return Json(new { Succuss = true, Msg = result.Message });
                }
                else
                {
                    //throw new Exception(result.Message);
                    return Json(new { Succuss = false, Msg = result.Message });
                }

                //Response.Redirect(bkUrl);
            }
            catch (Exception ex)
            {
                //Response.Redirect(bkUrl + "?errMsg=" + ex.Message);
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }

        public JsonResult AuthUser()
        {
            try
            {
                var realName = PreconditionAssert.IsNotEmptyString(Request["realName"], "真是姓名不能为空");
                var cardNumber = PreconditionAssert.IsNotEmptyString(Request["idCardNumber"], "身份证号码不能为空");
                string mobileCode = PreconditionAssert.IsNotEmptyString(Request["mobileCode"], "校验码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(cardNumber), "请输入正确的身份证号码");
                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(realName), "真实姓名不允许包含敏感词，如有疑问请联系客服");
                UserRealNameInfo realInfo = new UserRealNameInfo() { IdCardNumber = cardNumber, RealName = realName };//  
                //var result = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                // var resultCode = WCFClients.ExternalClient.ResponseAuthenticationMobile(mobileCode, SchemeSource.Web, UserToken); 
                // DelCacheRealNameInfo();
                //if (result.IsSuccess && resultCode.IsSuccess)
                //{
                //    LoadUserLeve();
                //    return Json(new { Succuss = true, Msg = result.Message });
                //}
                //else
                //{
                //    return Json(new { Succuss = false, Msg = result.Message });
                //}

                if (CurrentUser.IsAuthenticationMobile == false)
                {
                    var resultCode = WCFClients.ExternalClient.ResponseAuthenticationMobile(mobileCode, SchemeSource.Web, UserToken);
                    if (!resultCode.IsSuccess)
                    {
                        return Json(new { code = false, Msg = resultCode.Message });
                    }
                    LoadUserLeve("mobile");
                }
                if (CurrentUser.IsAuthenticationRealName == false)
                {
                    var result = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                    DelCacheRealNameInfo();
                    if (!result.IsSuccess)
                    {
                        return Json(new { code = false, Msg = result.Message });
                    }
                    LoadUserLeve("realname");
                }
                return Json(new { code = true, Msg = "完善信息成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 实名认证执行程序，认证完成后跳转到认证来源页面
        /// </summary>
        /// <returns></returns>
        public JsonResult authrealname()
        {
            //  var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? SiteRoot : Server.UrlDecode(Request["backurl"]);
            var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? SiteRoot : Server.UrlDecode(Request["backurl"]);
            try
            {
                Session["isnew"] = string.IsNullOrEmpty(Request["isnew"]) ? false : bool.Parse(Request["isnew"]);
                //var realName = PreconditionAssert.IsNotEmptyString(Request.Form["realName"], "真实姓名不能为空。");
                //var cardNumber = PreconditionAssert.IsNotEmptyString(Request.Form["idCardNumber"], "身份证号码不能为空。");
                var realName = PreconditionAssert.IsNotEmptyString(Request["realName"], "真实姓名不能为空。");
                var cardNumber = PreconditionAssert.IsNotEmptyString(Request["idCardNumber"], "身份证号码不能为空。");

                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(cardNumber), "请输入正确的身份证号码。");

                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(realName), "真实姓名不允许包含敏感词，如有疑问请联系客服。");

                UserRealNameInfo realInfo = new UserRealNameInfo() { IdCardNumber = cardNumber, RealName = realName };
                var result = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                DelCacheRealNameInfo();
                if (result.IsSuccess)
                {
                    LoadUserLeve("realname");
                    return Json(new { Succuss = true, Msg = result.Message });
                }
                else
                {
                    return Json(new { Succuss = false, Msg = result.Message });
                }
                // Response.Redirect(bkUrl);
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
                // Response.Redirect(bkUrl + "?errMsg=" + ex.Message);
            }
        }

        #region "20171108增加配置（禁止注册的银行卡号码）"
        /// <summary>
        /// 禁止注册的银行卡号码
        /// </summary>
        public string BanRegistrBankCard
        {
            get
            {
                string defalutValue = "";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("BanRegistrBankCard").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        #endregion

        //绑定银行卡执行程序，绑定完成后跳转到银行卡绑定页面
        public JsonResult bindbank()
        {
            var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? SiteRoot : Server.UrlDecode(Request["backurl"]);
            try
            {
                var RealName = PreconditionAssert.IsNotEmptyString(Request["RealName"], "请输入开户人姓名");
                var bankcode = PreconditionAssert.IsNotEmptyString(Request["bankcode"], "请选择开户银行");
                var bankDisplayName = PreconditionAssert.IsNotEmptyString(Request["bankDisplayName"], "请选择开户银行");
                var province = PreconditionAssert.IsNotEmptyString(Request["province"], "请选择开户银行省市");
                var city = PreconditionAssert.IsNotEmptyString(Request["city"], "请选择开户银行地区");
                var subBankName = bankDisplayName;
                //if (!new string[] { "CMB", "ICBC", "CCB", "BOC" }.Contains(bankcode))
                //{
                //    //subBankName = PreconditionAssert.IsNotEmptyString(Request.Form["subBankName"], "请输入开户银行支行");
                //    subBankName = PreconditionAssert.IsNotEmptyString(Request["subBankName"], "请输入开户银行支行");
                //}
                var cardnumber = PreconditionAssert.IsNotEmptyString(Request["cardnumber"], "请输入银行卡号码");

                PreconditionAssert.IsTrue(ValidateHelper.IsBankCardNumber(cardnumber), "请输入正确的银行卡号码");
                cardnumber = Chinese2Spell.ToDBC(cardnumber);//全角转换成半角
                #region "20171108增加配置（禁止注册的银行卡号码）"
                string banRegistrBankCard = BanRegistrBankCard;
                if (banRegistrBankCard.Contains(cardnumber))
                {
                    return Json(new { IsSuccess = false, Msg = "因检测到该银行卡号码在黑名单中，无法绑定，请联系在线客服。" });
                }
                #endregion
                BankCardInfo bankCard = new BankCardInfo()
                {
                    UserId = CurrentUser.LoginInfo.UserId,
                    BankCode = bankcode,
                    BankName = bankDisplayName,
                    BankSubName = subBankName,
                    BankCardNumber = cardnumber,
                    ProvinceName = province,
                    CityName = city,
                    RealName = RealName// CurrentUser.RealNameInfo.RealName,
                };
                //var bindType = Request.Form["bindType"];//绑定类型，用户差别新增或修改
                var bindType = Request["bindType"];
                if (bindType == "mod")
                {
                    var result = WCFClients.GameFundClient.UpdateBankCard(bankCard, UserToken);
                    if (result.IsSuccess)
                    {
                        ClearUserBindInfoCache(bankCard.UserId);
                        LoadUserLeve("bankcard");
                        return Json(new { Success = true, Msg = result.Message });
                    }
                    else
                    {
                        return Json(new { Success = false, Msg = result.Message });
                    }
                }
                else
                {
                    //var giveFillMoney = decimal.Parse(WCFClients.ActivityClient.QueryActivityConfig("ActivityConfig.FirstBindBankCardGiveFillMoney").ConfigValue);
                    var result = WCFClients.GameFundClient.AddBankCard(bankCard, UserToken);
                    if (result.IsSuccess)
                    {
                        LoadUserLeve("bankcard");
                        var giveFillMoney = WCFClients.ActivityClient.QueryActivityConfig("ActivityConfig.FirstBindBankCardGiveFillMoney");
                        var msg = string.Format("恭喜您成功获得{0}元现金!", giveFillMoney);
                        return Json(new { Success = true, Msg = msg });//result.Message
                    }
                    else
                    {
                        return Json(new { Success = false, Msg = result.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }




        ///绑定邮箱执行程序，绑定完成后跳转到邮箱绑定页面
        public void bindemail()
        {
            var bkUrl = string.IsNullOrWhiteSpace(Request["backurl"]) ? "/member/safe" : Server.UrlDecode(Request["backurl"]);
            try
            {
                var email = PreconditionAssert.IsNotEmptyString(Request.Form["email"], "请输入邮箱");
                var result = WCFClients.ExternalClient.AuthenticationUserEmail(CurrentUser.LoginInfo.UserId, email, SchemeSource.Web, UserToken);
                if (result.IsSuccess)
                    LoadUserLeve("email");
                Response.Redirect(bkUrl);
            }
            catch (Exception ex)
            {
                Response.Redirect(bkUrl + "?errMsg=" + ex.Message);
            }
        }
        /// <summary>
        /// 实名认证页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Realnameauth()
        {
            ViewBag.User = CurrentUser;
            ViewBag.Active = WCFClients.ExternalClient.QueryActivInfo("register");
            return View();
        }
        /// <summary>
        /// 支付宝绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult AliPay()
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        [HttpPost]
        public JsonResult AliPay_Bind()
        {
            try
            {
                string AliPayNumber = PreconditionAssert.IsNotEmptyString(Request["alipayNumber"], "支付宝账号不能为空");
                if (!ValidateHelper.IsEmail(AliPayNumber) && !ValidateHelper.IsMobile(AliPayNumber))
                {
                    return Json(new { Succuss = false, Msg = "支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号)。" });
                }
                else
                {
                    var result = WCFClients.ExternalClient.AddUserAlipay(CurrentUser.LoginInfo.UserId, AliPayNumber);
                    if (result.IsSuccess)
                    {
                        CurrentUser.AlipayInfo = AliPayNumber;
                        return Json(new { Succuss = true, Msg = "绑定成功" });
                    }
                    else
                    {
                        return Json(new { Succuss = false, Msg = result.Message });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { Succuss = false, Msg = ex.Message });
            }

        }
        /// <summary>
        /// 绑定银行卡
        /// </summary>
        /// <returns></returns>
        public ActionResult Bankcard()
        {
            ViewBag.User = CurrentUser;
            return View();
        }

        #region 绑定手机-修改手机绑定-手机解绑
        /// <summary>
        /// 绑定手机号码
        /// </summary>
        /// <returns></returns>
        public ActionResult Mobile()
        {
            ViewBag.User = CurrentUser;
            if (!CurrentUser.IsAuthenticationMobile)
            {
                ViewBag.MobileRequestInfo = WCFClients.ExternalClient.QueryMyMobileRequestInfo(UserToken);
            }
            return View();
        }

        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMobile()
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        /// <summary>
        /// 验证就电话号码
        /// </summary>
        /// <returns></returns>
        public JsonResult SendMsg()
        {
            try
            {
                var phoneNumber = PreconditionAssert.IsNotEmptyString(Request["moblie"], "电话号码不能为空");
                var type = string.IsNullOrEmpty(Request["type"]) ? 1 : int.Parse(Request["type"]);
                var code = WCFClients.ExternalClient.SendValidateCodeToUserMobile(phoneNumber);
                var str = "";
                switch (type)
                {
                    case 1:
                        str = "更换手机验证码";
                        break;
                    case 2:
                        str = "手机绑定验证码";
                        break;
                    case 3:
                        str = "解除手机绑定验证码";
                        break;
                    default:
                        str = "更换手机验证码";
                        break;
                }
                var content = str + "：" + code.ReturnValue + "，请勿将验证码告知他人。";

                //var result = WCFClients.GameClient.SendMsg(phoneNumber, content, IpManager.IPAddress, 1, string.Empty, string.Empty);
                var result = WCFClients.GameQueryClient.SendSMS(phoneNumber, content, CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId);
                if (result.IsSuccess)
                    return Json(new { Succuss = true, Msg = "发送成功，请勿将验证码告知他人" });
                else
                    return Json(new { Succuss = false, Msg = "发送失败！" });
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <returns></returns>
        public JsonResult CheckOldCode()
        {
            try
            {
                var code = PreconditionAssert.IsNotEmptyString(Request["code"], "请输入验证码");
                var moblie = PreconditionAssert.IsNotEmptyString(Request["moblie"], "请输入电话号码");
                var result = WCFClients.ExternalClient.CheckValidationCode(moblie, "SendValidateCodeToUserMobile", code);
                if (result.IsSuccess)
                    return Json(new { Succuss = true, Msg = "" });
                else
                    return Json(new { Succuss = false, Msg = "请输入正确的验证码！" });
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 验证手机是否已绑定
        /// </summary>
        /// <returns></returns>
        public JsonResult CheckMobileIsBind()
        {
            try
            {
                var moblie = PreconditionAssert.IsNotEmptyString(Request["moblie"], "请输入电话号码");
                var result = WCFClients.ExternalClient.CheckMobileIsBind(moblie);
                if (result)
                    return Json(new { Succuss = false, Msg = "输入手机号码与绑定手机一致，无需修改！" });
                else
                    return Json(new { Succuss = true, Msg = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 查询最近50天是否有购彩行为
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryBetMoneyByDay()
        {
            try
            {
                var result = WCFClients.ExternalClient.QueryBetMoneyByDay(CurrentUser.LoginInfo.UserId, 50);
                if (result > 0)
                    return Json(new { Succuss = false, Msg = "对不起！您在最近50天内有购彩行为，无法解除绑定。" });
                else
                    return Json(new { Succuss = true, Msg = "" });

            }
            catch (Exception ex)
            {
                return Json(new { Succuss = true, Msg = ex.Message });
            }

        }
        /// <summary>
        /// 修改绑定
        /// </summary>
        /// <returns></returns>
        public JsonResult EditMoblie()
        {
            try
            {
                string mobileCode = PreconditionAssert.IsNotEmptyString(Request["mobileCode"], "校验码不能为空");
                var moblie = PreconditionAssert.IsNotEmptyString(Request["mobile"], "电话号码不能为空");
                var result = WCFClients.ExternalClient.EditMobileAuthen(CurrentUser.LoginInfo.UserId, moblie, "SendValidateCodeToUserMobile", mobileCode);

                if (result.IsSuccess)
                {
                    //DelCacheMobileInfo();
                    LoadUserLeve("mobile");
                    return Json(new { Succuss = true, Msg = "修改成功" });
                }
                else
                {
                    return Json(new { Succuss = false, Msg = result.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Succuss = false, Msg = ex.Message });
            }

        }
        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <returns></returns>
        public JsonResult LogOffMobileAuthen()
        {

            try
            {
                var result = WCFClients.ExternalClient.LogOffMobileAuthen(CurrentUser.LoginInfo.UserId, CurrentUser.LoginInfo.UserToken);
                if (result.IsSuccess)
                {
                    DelCacheMobileInfo();
                    return Json(new { Succuss = true, Msg = "解除绑定成功" });
                }
                else
                {
                    return Json(new { Succuss = false, Msg = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        #endregion

        /// <summary>
        /// 绑定邮箱
        /// </summary>
        /// <returns></returns>
        public ActionResult Email()
        {
            ViewBag.User = CurrentUser;
            return View();
        }

        /// <summary>
        /// 绑定QQ
        /// </summary>
        /// <returns></returns>
        public ActionResult QQ()
        {
            ViewBag.User = CurrentUser;
            return View();
        }
        [HttpPost]
        public JsonResult qq_bind()
        {
            try
            {
                string qqNumber = PreconditionAssert.IsNotEmptyString(Request["qqNumber"], "QQ号码不能为空");
                var result = WCFClients.ExternalClient.AddUserQQ(CurrentUser.LoginInfo.UserId, qqNumber);
                if (result.IsSuccess)
                {
                    CurrentUser.QQNumber = qqNumber;
                    return Json(new { Succuss = true, Msg = "绑定成功" });
                }
                else
                {
                    return Json(new { Succuss = false, Msg = result.Message });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Succuss = false, Msg = ex.Message });
            }

        }
        #endregion

        #region 帮助中心
        public ActionResult helpcenter()
        {

            return View();
        }
        #endregion

        #region 隐藏用户名


        public ActionResult HideUserName()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }


        [HttpPost]
        public JsonResult setHideLen()
        {
            try
            {
                var len = string.IsNullOrEmpty(Request["hideLen"]) ? 0 : int.Parse(Request["hideLen"]);
                if (len > CurrentUser.LoginInfo.DisplayName.Length)
                {
                    throw new Exception("隐藏用户名长度不能大于显示用户名长度");
                }
                var result = WCFClients.GameClient.ChangeUserHideDisplayNameCount(len, UserToken);
                if (result.IsSuccess)
                {
                    LoadUserLeve();
                }
                //var tokenLogin = WCFClients.ExternalClient.LoginByUserToken(UserToken);
                //CurrentUser = new CurrentUserInfo { LoginInfo = tokenLogin };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        #endregion

        /// <summary>
        /// 提款记录（全部状态）
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DrawingsRecordAll()
        {
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.Status = string.IsNullOrEmpty(Request.QueryString["status"]) ? null : (EntityModel.Enum.WithdrawStatus?)int.Parse(Request.QueryString["status"]);
                ViewBag.CurrentUser = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-1))
                    ViewBag.Begin = DateTime.Now.AddMonths(-1);

                Dictionary<string, object> param = new Dictionary<string, object>();
                param["status"] = ViewBag.Status;
                param["startTime"] = ViewBag.Begin;
                param["endTime"] = ViewBag.End.AddDays(1);
                param["pageIndex"] = ViewBag.pageNo;
                param["pageSize"] = ViewBag.PageSize;
                param["UserId"] = UserToken;

                ViewBag.WithdrawList = await serviceProxyProvider.Invoke<EntityModel.CoreModel.Withdraw_QueryInfoCollection>(param, "api/user/QueryMyWithdrawList");
            }
            catch (Exception)
            {
                ViewBag.WithdrawList = null;
            }
            return View();
        }

        #region 关注用户
        //关注用户-关注和取消关注
        [HttpPost]
        public async Task<JsonResult> attentionExec(string id)
        {
            try
            {
                var attUser = PreconditionAssert.IsNotEmptyString(Request["attentionUserId"], "被关注用户编号错误");
                var isAttention = string.IsNullOrEmpty(id) ? true : bool.Parse(id);
                var usrList = attUser.Split('|');
                var result = new EntityModel.Communication.CommonActionResult() { IsSuccess = false, Message = "未执行操作" };
                foreach (var item in usrList)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param["beAttentionUserId"] = item;
                        param["UserId"] = UserToken;
                        if (isAttention)
                        {
                            result = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/user/AttentionUser");
                        }
                        else
                        {
                            result = await serviceProxyProvider.Invoke<EntityModel.Communication.CommonActionResult>(param, "api/user/CancelAttentionUser");
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        public async Task<JsonResult> AttentAndGd()
        {
            try
            {
                var user = Request["User"];
                var result = false;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["beAttentionUserId"] = CurrentUser.LoginInfo.UserId;
                param["currentUserId"] = user;
                if (CurrentUser.LoginInfo.UserId != null)
                {

                    result = await serviceProxyProvider.Invoke<bool>(param, "api/user/QueryIsAttention");
                    if (result)
                    {
                        return Json(new { Issucse = true, Msg = "关注成功" });
                    }
                    else
                    {
                        return Json(new { Issucse = false, Msg = "已关注" });
                    }
                }
                else
                {
                    return Json(new { Issucse = false, Msg = "请登录" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Issucse = false, Msg = ex.Message });
            }
        }

        public JsonResult Makeorder()
        {
            try
            {
                var user = Request["User"];
                if (CurrentUser.LoginInfo.UserId != null || CurrentUser.LoginInfo.UserId != user)
                {
                    return Json(new { Issucse = true });
                }
                else
                {
                    return Json(new { Issucse = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Issucse = false, Msg = ex.Message });
            }
        }

        #endregion

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateUser()
        {
            return Json(new { });
        }

        #region 票样申请

        public ActionResult MySampleTicket()
        {
            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["begin"]) ? DateTime.Today.AddMonths(-1) : Convert.ToDateTime(Request["begin"]);
            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["end"]) ? DateTime.Today.AddDays(1) : Convert.ToDateTime(Request["end"]);

            ViewBag.GameType = string.IsNullOrEmpty(Request["game"]) ? string.Empty : Request["game"];

            ViewBag.ApplyState = string.IsNullOrEmpty(Request["state"]) ? string.Empty : Request["state"];

            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
            //ViewBag.AllRequestSampleTicketCollection = WCFClients.ExternalClient.QueryAllRequestSampleTicket(ViewBag.StartTime, ViewBag.EndTime, "UserId", CurrentUser.LastLoginInfo.UserId, ViewBag.GameType, ViewBag.ApplyState, ViewBag.PageIndex, ViewBag.PageSize);

            return View();
        }

        #endregion
        #region 支付宝充值连接

        public string bindAlipayurl(string payAmount, string orderId)
        {
            var url = string.Format("https://shenghuo.alipay.com/send/payment/fill.htm?optEmail={0}&payAmount={1}&title={2}", "ziqiangdeng@126.com", payAmount, orderId);
            return url;
        }

        #endregion

        #region  短信提醒
        public ActionResult NoteRemind()
        {
            try
            {
                var info = WCFClients.GameClient.QueryUserSiteServiceByUserId(CurrentUser.LoginInfo.UserId);
                ViewBag.Money = info.ExtendedTwo.ToString("0");
                ViewBag.IsEnable = info.IsEnable;
                ViewBag.Id = info.Id;
            }
            catch (Exception)
            {
                ViewBag.Money = 1000;
                ViewBag.IsEnable = true;
                ViewBag.Id = 0;
            }

            return View();
        }
        public JsonResult UpdateDrawingNotice()
        {
            try
            {
                var infoId = PreconditionAssert.IsNotEmptyString(Request["infoId"], "id不能为空！");
                var IsEnable = bool.Parse(PreconditionAssert.IsNotEmptyString(Request["isEnable"], "状态不能为空！"));
                var Money = decimal.Parse(PreconditionAssert.IsNotEmptyString(Request["Money"], "金额不能为空！"));

                UserSiteServiceInfo info = new UserSiteServiceInfo();
                if (!string.IsNullOrEmpty(infoId) && Convert.ToInt32(infoId) > 0)
                {
                    info.IsEnable = IsEnable;
                    info.Id = int.Parse(infoId);
                    info.ServiceType = ServiceType.DrawingNotice;
                }
                else
                {
                    info.ExtendedOne = string.Empty;
                    info.ExtendedTwo = Money;
                    info.IsEnable = true;
                    info.Remarks = string.Empty;
                    info.ServiceType = ServiceType.DrawingNotice;
                    info.UserId = CurrentUser.LoginInfo.UserId;
                }

                var re = WCFClients.GameClient.UpdateSiteService(info);

                return Json(new { IsSucess = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Message = ex.Message });
            }
        }

        #endregion


        public ActionResult payTest()
        {

            var partnerId = WCFClients.GameClient.QueryCoreConfigByKey("BoYing.PartnerId").ConfigValue;// "10001";
            var hashKey = WCFClients.GameClient.QueryCoreConfigByKey("BoYing.HashKey").ConfigValue;// "dd430a74eaa81d8a";

            var orderId = Guid.NewGuid().ToString("N");
            var bank = 10005;
            var money = 10;
            var callback_front = "http://www.wancai.com/member/pay_by_front";//页面回复地址
            var callback_push = "http://www.wancai.com/member/pay_by_push";//通知地址


            var data = BoYingAPI.BuildPayRequestParams(orderId, bank, money, callback_front, callback_push);
            var sign = BoYingAPI.BuildPaySign(partnerId, hashKey, data);

            ViewBag.PostUrl = BoYingAPI.BoYingUrl;
            ViewBag.PartnerId = partnerId;
            ViewBag.Data = data;
            ViewBag.Sign = sign;

            return View();
        }

        public ContentResult pay_by_push()
        {
            var partnerId = WCFClients.GameClient.QueryCoreConfigByKey("BoYing.PartnerId").ConfigValue;// "10001";
            var hashKey = WCFClients.GameClient.QueryCoreConfigByKey("BoYing.HashKey").ConfigValue;// "dd430a74eaa81d8a";

            var vendor = Request["vendor"];
            var data = Request["data"];
            var sign = Request["sign"];

            var logList = new List<string>();
            foreach (string key in Request.Form.Keys)
            {
                logList.Add(string.Format("{0}=>{1}", key, Request.Form[key]));
            }

            var responseData = BoYingAPI.GetResponseData(data);
            logList.Add("==================");
            logList.Add(string.Format("{0}=>{1}", "responseData", responseData));
            var mySign = BoYingAPI.GetResponseSign(vendor, hashKey, responseData);
            logList.Add(string.Format("{0}=>{1}", "mySign", mySign));
            this.WriteLog(string.Join(Environment.NewLine, logList.ToArray()));

            if (sign.ToUpper() == mySign.ToUpper())
            {
                //验证成功


                return Content("true");
            }

            return Content("加密错误");

        }

        public ActionResult pay_by_front()
        {
            var logList = new List<string>();
            foreach (string key in Request.Form.Keys)
            {
                logList.Add(string.Format("{0}=>{1}", key, Request.Form[key]));
            }
            this.WriteLog(string.Join(Environment.NewLine, logList.ToArray()));


            return View();
        }

        private static ILogWriter log = Common.Log.LogWriterGetter.GetLogWriter();
        private void WriteLog(string txt)
        {
            log.Write("Member", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "BoYingLog", txt);
        }


        public QRCodeImg LoadWxPayImg()
        {
            return new QRCodeImg("");
        }



        public ActionResult ShowAliPay()
        {
            return View();
        }

        public ActionResult ShowWePay()
        {
            return View();
        }


        public JsonResult GetJHZPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                var type = Request["type"];
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号

                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = "微信扫码充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                if (type == "weixin")
                {
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.jhz_weixin;
                }
                else
                {
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.jhz_alipay;
                }
                string jhz_merchantNo = System.Configuration.ConfigurationManager.AppSettings["jhz_merchantNo"].ToString();
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, jhz_merchantNo);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_JHZ_WX()
        {
            var money = int.Parse(Request["requestMoney"]) * 100;
            var orderId = Request["orderId"];
            //string money = "100";
            //string orderId = Guid.NewGuid().ToString();
            var notify_url = base.FillMoneyCallBackDomain + "/user/JHZNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";
            var url = Common.Pay.jhz.JHZPayAPI.GetPrePayUrl(orderId, money.ToString(), return_url, notify_url, "6001");
            return new WXPayQRCodeImg
            {
                QRCodeUrl = url
            };
        }


        /// <summary>
        /// 生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_JHZ_ALI()
        {
            var money = int.Parse(Request["requestMoney"]) * 100;
            var orderId = Request["orderId"];
            //string money = "100";
            //string orderId = Guid.NewGuid().ToString();
            var notify_url = base.FillMoneyCallBackDomain + "/user/JHZNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";
            var url = Common.Pay.jhz.JHZPayAPI.GetPrePayUrl(orderId, money.ToString(), return_url, notify_url, "6003");
            return new WXPayQRCodeImg
            {
                QRCodeUrl = url
            };
        }

        #region "中铁---开始"

        /// <summary>
        /// 中铁通付商户号
        /// </summary>
        public string ZT_merchant_no
        {
            get
            {
                string defalutValue = "TOF00016";
                try
                {
                    var v = ConfigurationManager.AppSettings["ZT_merchant_no"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// md5 密钥
        /// </summary>
        public string ZT_privateKey
        {
            get
            {
                string defalutValue = "c9ab0bfba6734cdba9b1c2038e16035b";
                try
                {
                    var v = ConfigurationManager.AppSettings["ZT_privateKey"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 扫描请求地址
        /// </summary>
        public string ZT_codepayUrl
        {
            get
            {
                string defalutValue = "http://api.crpay.com/payapi/gateway";
                try
                {
                    var v = ConfigurationManager.AppSettings["ZT_codepayUrl"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 中铁
        /// </summary>
        /// <returns></returns>
        public JsonResult GetZTPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                var type = Request["type"];
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号

                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = type == "weixin" ? "微信扫码充值" : "支付宝扫码充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                if (type == "weixin")
                {
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.ZTPay;
                }
                else
                {
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.ZTAlipay;
                }
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, this.ZT_merchant_no);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 中铁微信生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_ZT_WX()
        {
            var money = int.Parse(Request["requestMoney"]) * 100;
            var orderId = Request["orderId"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/ZTNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";
            string merchant_no = this.ZT_merchant_no;
            string method = "unified.trade.pay";
            string version = "1.0";

            string out_trade_no = orderId;
            string timestamp = FancyOne.Payment.PayHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now).ToString();
            string amount = money.ToString();
            string body = "body";
            string remark = "remark";
            string way = "wxQR";//微信

            string md5sign = string.Empty;
            string signStr = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", amount, body, merchant_no, method, notify_url, out_trade_no, remark, timestamp, version, way, this.ZT_privateKey);
            md5sign = FancyOne.Payment.PayHelper.getmd5(signStr);
            string parm = string.Format("&out_trade_no={0}&timestamp={1}&amount={2}&body={3}&remark={4}&notify_url={5}&way={6}", out_trade_no, timestamp, amount, body, remark, notify_url, way);
            string para = string.Format("merchant_no={0}&method={1}&version={2}&sign={3}{4}", merchant_no, method, version, md5sign, parm);
            string _xml = FancyOne.Payment.PayHelper.HttpPost(this.ZT_codepayUrl, para);
            string codeUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(_xml))
            {
                var jsonObjs = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(_xml);
                if (jsonObjs["code"] == "0000")
                {
                    codeUrl = jsonObjs["code_url"].ToString();
                }
            }
            log.Write("WXPayQRCodeImg", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode_ZT_WX", codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        /// <summary>
        /// 中铁alipay生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_ZT_Alipay()
        {
            var money = int.Parse(Request["requestMoney"]) * 100;
            var orderId = Request["orderId"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/ZTNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";
            string merchant_no = this.ZT_merchant_no;
            string method = "unified.trade.pay";
            string version = "1.0";

            string out_trade_no = orderId;
            string timestamp = FancyOne.Payment.PayHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now).ToString();
            string amount = money.ToString();
            string body = "body";
            string remark = "remark";
            string way = "alipayQR";//alipay

            string md5sign = string.Empty;
            string signStr = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", amount, body, merchant_no, method, notify_url, out_trade_no, remark, timestamp, version, way, this.ZT_privateKey);
            md5sign = FancyOne.Payment.PayHelper.getmd5(signStr);
            string parm = string.Format("&out_trade_no={0}&timestamp={1}&amount={2}&body={3}&remark={4}&notify_url={5}&way={6}", out_trade_no, timestamp, amount, body, remark, notify_url, way);
            string para = string.Format("merchant_no={0}&method={1}&version={2}&sign={3}{4}", merchant_no, method, version, md5sign, parm);
            string _xml = FancyOne.Payment.PayHelper.HttpPost(this.ZT_codepayUrl, para);
            string codeUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(_xml))
            {
                var jsonObjs = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(_xml);
                if (jsonObjs["code"] == "0000")
                {
                    codeUrl = jsonObjs["code_url"].ToString();
                }
            }
            log.Write("WXPayQRCodeImg", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode_ZT_Alipay", codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        #endregion "中铁---结束"


        #region "天下付"
        /// <summary>
        /// 天付宝商户号
        /// </summary>
        public string tfb_spid
        {
            get
            {
                string defalutValue = "1800257907";
                try
                {
                    var v = ConfigurationManager.AppSettings["tfb_spid"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 天付宝key
        /// </summary>
        public string tfb_Key
        {
            get
            {
                string defalutValue = "i,?a9;WVQ0";
                try
                {
                    var v = ConfigurationManager.AppSettings["tfb_Key"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 天付宝微信请求地址
        /// </summary>
        public string tfb_PostUrl_weixin
        {
            get
            {
                string defalutValue = "http://upay.tfb8.com/cgi-bin/v2.0/api_wx_pay_apply.cgi";
                try
                {
                    var v = ConfigurationManager.AppSettings["tfb_PostUrl_weixin"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 天付宝支付宝请求地址
        /// </summary>
        public string tfb_PostUrl_alipay
        {
            get
            {
                string defalutValue = "http://upay.tfb8.com/cgi-bin/v2.0/api_ali_pay_apply.cgi";
                try
                {
                    var v = ConfigurationManager.AppSettings["tfb_PostUrl_alipay"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        /// <summary>
        /// 天下付
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTXFPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                var type = Request["type"];
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号

                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                // fillMoneyInfo.GoodsDescription = type == "weixin" ? "微信扫码充值" : "支付宝扫码充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                if (type == "weixin")
                {
                    fillMoneyInfo.GoodsDescription = "微信扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.txf_weixin;
                }
                else if (type == "qq")
                {
                    fillMoneyInfo.GoodsDescription = "QQ扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.txf_qq;
                }
                else if (type == "alipay")
                {
                    fillMoneyInfo.GoodsDescription = "支付宝扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.txf_alipay;
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "未知的支付类型" }, JsonRequestBehavior.AllowGet);
                }
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, this.tfb_spid);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 天下付微信生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_TXF_WX()
        {
            var money = int.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/TFBNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";

            string codeUrl = Common.Pay.tianfubao.Pay.tfbweixinresult(notify_url, return_url, Convert.ToDecimal(money), this.CurrentUser.LoginInfo.UserId.ToString(), "", orderId.ToString(), this.tfb_spid, this.tfb_Key, this.tfb_PostUrl_weixin);

            log.Write("WXPayQRCodeImg", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode_TXF_WX", codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        /// <summary>
        /// 天下付支付宝生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_TXF_ALIPAY()
        {
            var money = int.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/TFBNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";

            string codeUrl = Common.Pay.tianfubao.Pay.tfbalipayresult(notify_url, return_url, Convert.ToDecimal(money), this.CurrentUser.LoginInfo.UserId.ToString(), "", orderId.ToString(), this.tfb_spid, this.tfb_Key, this.tfb_PostUrl_alipay);

            log.Write("WXPayQRCodeImg", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode_TXF_ALIPAY", codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        /// <summary>
        /// 天下付QQ生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCode_TXF_QQ()
        {
            var money = int.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/TFBNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";

            string codeUrl = Common.Pay.tianfubao.Pay.tfbqqresult(notify_url, return_url, Convert.ToDecimal(money), this.CurrentUser.LoginInfo.UserId.ToString(), "", orderId.ToString(), this.tfb_spid, this.tfb_Key, this.tfb_PostUrl_weixin);

            log.Write("WXPayQRCodeImg", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode_TXF_QQ", codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        #endregion


        #region 速汇宝
        /// <summary>
        /// 速汇宝商户ID
        /// </summary>
        public string sfb_merchantcode
        {
            get
            {
                string defalutValue = "1111110166";
                try
                {
                    var v = ConfigurationManager.AppSettings["sfb_merchantcode"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 速汇宝商户ID
        /// </summary>
        public string sfb_privatekey
        {
            get
            {
                string defalutValue = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMJmXI2hjDRo6yILATUqKJosrPqvpUv3UR1rwVO7YudBsi708HOFH2cnVPeYMGwNAoaR6zras26SHssWoo5kkaYjCXIISxWtQyEPYn/r9cUNqay7bMYAGgTLIhglOvvvNM31DbuWATed9PX2vAYzLE2c8DsDGX3hNolc6iIiQiMtAgMBAAECgYAVxz3rKAP7Ax4EbFMwT47I5uRiGTddcVGHCEFaTg3gdn2twQcHCgzgk4lzS3txP2vfA43kxAeCBaCpg9mmNiIS1ptQ1nSilurzDQJAq6aotvN9KnEz8Ao7yhHkZmp4S06fhRYEs6RSJLbaooFbYiyJuEq+Eb+DjHqGYbdbgXWySQJBAOzJ1mJZTzaRejTRTo+xGgVadAYbSfZDsq8EXhjzJ4IXs686pyPR6i6YoTq38DZet+ZgMefhxQ+k4BnC/DQVH+cCQQDSLBnymjhtb2dlS79GrGwTHIUAaS5y7EsgLAGWDFIHMp9E3UTI5oF91Le2Omn3OhjtSgIdmmwt5YX3tTogEJHLAkEA3Hm68mwyA59FaLSTL9w5XE6yxZTXM0Qptiic7SJK4Sjsl/ZG9mVYZfab+S6XrihXl1xuW3iuojhkqdgSOPSKdQJAGVQLRHtldXrJgSGhyYiZ9auoM6Z5XIwxeY0UG9scP5XQL+Jimbt9u4ZZJXLgtlSgEGis3JhxlQ5mGLYUbSzSBQJACf1lPF1Xv9YYqchsNPXTewoge26BKB+4X+GksWW+yfTVgxgGdpDHUs39jWE8uDS5Vvqrl6q4syBH4j5GljCLfQ==";
                try
                {
                    var v = ConfigurationManager.AppSettings["sfb_privatekey"].ToString();
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                var type = Request["type"];
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号

                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                // fillMoneyInfo.GoodsDescription = type == "weixin" ? "微信扫码充值" : "支付宝扫码充值";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                string agentid = string.Empty;
                if (type == "sfb_weixin")
                {
                    fillMoneyInfo.GoodsDescription = "微信扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.sfb_weixin;
                    agentid = this.sfb_merchantcode;
                }
                else if (type == "sfb_qq")
                {
                    fillMoneyInfo.GoodsDescription = "QQ扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.sfb_qq;
                    agentid = this.sfb_merchantcode;
                }
                else if (type == "sfb_alipay")
                {
                    fillMoneyInfo.GoodsDescription = "支付宝扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.sfb_alipay;
                    agentid = this.sfb_merchantcode;
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "未知的支付类型" }, JsonRequestBehavior.AllowGet);
                }
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, agentid);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        public WXPayQRCodeImg LoadQRCodeIMG()
        {
            var money = decimal.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            var type = Request["type"];
            string notify_url = string.Empty;
            string return_url = string.Empty;
            string codeUrl = string.Empty;
            switch (type)
            {
                case "sfb_weixin":
                    notify_url = base.FillMoneyCallBackDomain + "/user/SFBNotifyUrl";
                    return_url = WebSiteUrl + "/member/safe";
                    codeUrl = Common.Pay.sfb.pay.sfb_result(return_url, notify_url, this.sfb_merchantcode, orderId.ToString(), money, this.CurrentUser.LoginInfo.UserId.ToString(), "", this.sfb_privatekey, "weixin_scan");
                    break;
                case "sfb_alipay":
                    notify_url = base.FillMoneyCallBackDomain + "/user/SFBNotifyUrl";
                    return_url = WebSiteUrl + "/member/safe";
                    codeUrl = Common.Pay.sfb.pay.sfb_result(return_url, notify_url, this.sfb_merchantcode, orderId.ToString(), money, this.CurrentUser.LoginInfo.UserId.ToString(), "", this.sfb_privatekey, "alipay_scan");
                    break;
                case "sfb_qq":
                    notify_url = base.FillMoneyCallBackDomain + "/user/SFBNotifyUrl";
                    return_url = WebSiteUrl + "/member/safe";
                    codeUrl = Common.Pay.sfb.pay.sfb_result(return_url, notify_url, this.sfb_merchantcode, orderId.ToString(), money, this.CurrentUser.LoginInfo.UserId.ToString(), "", this.sfb_privatekey, "qq_scan");
                    codeUrl = codeUrl.Replace("&amp;", "&");
                    break;
                default:
                    break;
            }
            log.Write("LoadQRCodeIMG", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "LoadQRCode" + type, codeUrl);
            return new WXPayQRCodeImg
            {
                QRCodeUrl = codeUrl
            };
        }
        #endregion

        #region 推广管理
        public ActionResult SpreadLinks()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            ViewBag.Url_1 = ConfigurationManager.AppSettings["TGDomain_1"] + "?yqid=" + CurrentUser.LoginInfo.UserId;
            ViewBag.Url_2 = ConfigurationManager.AppSettings["TGDomain_2"] + "?yqid=" + CurrentUser.LoginInfo.UserId;
            return View();
        }

        public ActionResult SporeadUsers()
        {
            var beginDate = string.IsNullOrEmpty(Request["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request["begin"]);
            var endDate = string.IsNullOrEmpty(Request["end"]) ? Convert.ToDateTime(string.Format("{0} 23:59:59", DateTime.Now.ToString("yyyy-MM-dd"))) : Convert.ToDateTime(string.Format("{0} 23:59:59", DateTime.Parse(Request["end"]).ToString("yyyy-MM-dd")));
            //if ((endDate - beginDate).TotalDays > 30)
            //{
            //    beginDate = DateTime.Today.AddDays(-30);
            //    endDate = DateTime.Today;
            //}
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Begin = beginDate;
            ViewBag.End = endDate;
            ViewBag.Sporeds = WCFClients.ExternalClient.QuerySporeadUsers(CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
            return View();
        }
        #endregion


        #region "艾付"
        string af_merchant_no = System.Configuration.ConfigurationManager.AppSettings["af_merchant_no"].ToString() == "" ? "144801008923" : System.Configuration.ConfigurationManager.AppSettings["af_merchant_no"].ToString();
        string af_key = System.Configuration.ConfigurationManager.AppSettings["af_key"].ToString() == "" ? "560d1319-fff4-11e7-8d889c40f997" : System.Configuration.ConfigurationManager.AppSettings["af_key"].ToString();
        string af_url = System.Configuration.ConfigurationManager.AppSettings["af_url"].ToString() == "" ? "http://pay.ifeepay.com/gateway/pay.jsp" : System.Configuration.ConfigurationManager.AppSettings["af_url"].ToString();

        public JsonResult GetAFPayOrder()
        {
            try
            {
                var money = int.Parse(Request["requestMoney"]);
                var type = Request["type"];
                // 创建本地订单对象
                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                //向本地系统里添加订单，并返回订单号
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsType = "账户充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                string agentid = string.Empty;
                if (type == "af_weixin")
                {
                    fillMoneyInfo.GoodsDescription = "微信扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.af_weixin;
                    agentid = this.af_merchant_no;
                }
                else if (type == "af_qq")
                {
                    fillMoneyInfo.GoodsDescription = "QQ扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.af_qq;
                    agentid = this.af_merchant_no;
                }
                else if (type == "af_alipay")
                {
                    fillMoneyInfo.GoodsDescription = "支付宝扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.af_alipay;
                    agentid = this.af_merchant_no;
                }
                else if (type == "af_upay")
                {
                    fillMoneyInfo.GoodsDescription = "银联扫码充值";
                    fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.af_upay;
                    agentid = this.af_merchant_no;
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "未知的支付类型" }, JsonRequestBehavior.AllowGet);
                }
                var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, this.CurrentUser.LoginInfo.UserId, agentid);
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                return Json(new { IsSuccess = true, Message = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public WXPayQRCodeImg LoadAFQRCodeIMG()
        {
            var money = decimal.Parse(Request["requestMoney"]);
            var orderId = Request["orderId"];
            var type = Request["type"];
            string notify_url = base.FillMoneyCallBackDomain + "/user/AFNotifyUrl";
            string return_url = WebSiteUrl + "/member/safe";
            string codeUrl = string.Empty;
            switch (type)
            {
                case "af_weixin":
                    codeUrl = Common.Pay.af.afPay.af_reslut(af_url, af_key, af_merchant_no, orderId, notify_url, return_url, money, "WECHAT");
                    break;
                case "af_alipay":
                    codeUrl = Common.Pay.af.afPay.af_reslut(af_url, af_key, af_merchant_no, orderId, notify_url, return_url, money, "ALIPAY");
                    break;
                case "af_qq":
                    codeUrl = Common.Pay.af.afPay.af_reslut(af_url, af_key, af_merchant_no, orderId, notify_url, return_url, money, "QQSCAN");
                    break;
                case "af_upay":
                    codeUrl = Common.Pay.af.afPay.af_reslut(af_url, af_key, af_merchant_no, orderId, notify_url, return_url, money, "UNIONPAY");
                    break;
                default:
                    break;
            }
            var json = Newtonsoft.Json.Linq.JObject.Parse(codeUrl);
            if (json["result_code"].ToString() == "00")
            {
                return new WXPayQRCodeImg
                {
                    QRCodeUrl = json["code_url"].ToString()
                };
            }
            else
            {
                return new WXPayQRCodeImg
                {
                    QRCodeUrl = ""
                };
            }

        }
        #endregion
    }
}