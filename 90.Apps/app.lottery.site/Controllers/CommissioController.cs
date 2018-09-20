using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameBiz.Core;
using app.lottery.site.Controllers;
using External.Core.Agnet;
using System.Configuration;
using External.Client;
using External.Core.Login;
using app.lottery.site.Models;
using Common.Utilities;
using Common.Net;


namespace app.lottery.site.iqucai.Controllers
{
    [CheckLogin]
    [CheckIsAgent]
    public class CommissioController : BaseController
    {
        //
        // GET: /Commissio/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 成功方案
        /// </summary>
        /// <returns></returns>
        public ActionResult WinScheme()
        {
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : Convert.ToDateTime(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                //var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.JuniorCreateTogether = null;
                else
                    ViewBag.JuniorCreateTogether = WCFClients.ExternalClient.QuerySubUserPayRebateOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
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
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.AgentFillMoney = null;
                else
                    ViewBag.AgentFillMoney = WCFClients.ExternalClient.QueryAgentFillMoneyTopList(string.Empty, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize, UserToken);

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

                ViewBag.AgentSales = WCFClients.ExternalClient.QueryLowerAgentSaleByUserId(agentId, ViewBag.userId, ViewBag.key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
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
        public ActionResult SchemeManage()
        {
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                //var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.JuniorCreateTogether = null;
                else
                    ViewBag.JuniorCreateTogether = WCFClients.ExternalClient.QuerySubUserCreateingOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
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
            ViewBag.CurrentUser = CurrentUser;
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
                //WCFClients.ExternalClient.AddOCAgent(OCAgentCategory.GeneralAgent, CurrentUser.LoginInfo.UserId, userId);
                return Json(new { IsSuccess = true, Msg = "添加成功！" });
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
                string id = Request["txtDealerId"];
                var UserMsg = WCFClients.ExternalClient.GetUserByAgentId(id, CurrentUser.LoginInfo.UserId);
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
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                //var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                ViewBag.Key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.JuniorCompleteTogether = null;
                else
                    ViewBag.JuniorCompleteTogether = WCFClients.ExternalClient.QuerySubUserNoPayRebateOrderList(ViewBag.Key, CurrentUser.LoginInfo.UserId, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize);
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
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var key = string.IsNullOrEmpty(Request.QueryString["key"]) ? "" : Request.QueryString["key"];
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                {
                    ViewBag.AgentLotto = null;
                }
                else
                {
                    ViewBag.AgentLotto = WCFClients.ExternalClient.QueryAgentLottoTopList(string.Empty, key, ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.PageSize, UserToken);
                }

            }
            catch (Exception)
            {

                ViewBag.AgentLotto = null;
            }

            return View();
        }
        /// <summary>
        /// 提现明细
        /// </summary>
        /// <returns></returns>
        public ActionResult TxDetail()
        {
            return View();
        }
        /// <summary>
        /// 推广链接
        /// </summary>
        /// <returns></returns>
        public ActionResult PopularizeLink()
        {
            //var agentMapping = WCFClients.GameClient.QueryCoreConfigByKey("AgentMapping").ConfigValue;
            //var userid = CurrentUser.LoginInfo.UserId;
            //if (!string.IsNullOrEmpty(agentMapping))
            //{
            //    var agentList = agentMapping.Split('#');

            //    for (int i = 0; i < agentList.Length; i++)
            //    {
            //        var agent = agentList[i].Split('|');
            //        for (int k = 0; k < agent.Length; k++)
            //        {
            //            if (agent[k].ToString() == userid)
            //            {
            //                ViewBag.Url = "http://" + agent[k - 1];
            //                return View();
            //            }
            //            else
            //            {
            //                ViewBag.Url = ConfigurationManager.AppSettings["SelfDomain"] + "?pid=" + CurrentUser.LoginInfo.UserId;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    ViewBag.Url = ConfigurationManager.AppSettings["SelfDomain"] + "?pid=" + CurrentUser.LoginInfo.UserId;
            //}

            //查询当前用户代理类型   是否为总代理  或者为  总代理的下级代理
            string url = "";
            ViewBag.CurrentUser = CurrentUser;
            if (CurrentUser.LoginInfo.IsAgent)
            {
                var AgentType = WCFClients.ExternalClient.QueryAgentType(CurrentUser.LoginInfo.UserId);
                if (AgentType.OCAgentCategory == OCAgentCategory.SportLotterySubAgent)
                {
                    var storeid = AgentType.StoreId;
                    if (!string.IsNullOrEmpty(storeid))
                    {
                        url = "http://" + storeid + ".wancai.com";
                    }
                }
                ViewBag.AgentType = AgentType;
            }
            var domain = WCFClients.ExternalClient.QueryCustmerDomainByUserId(this.CurrentUser.LoginInfo.UserId);
            ViewBag.CustomerUrl = domain;
            if (url != "")
            {
                ViewBag.Url = url;
            }
            else
            {
                ViewBag.Url = ConfigurationManager.AppSettings["SelfDomain"] + "?pid=" + CurrentUser.LoginInfo.UserId;
            }
            ViewBag.UserId = CurrentUser.LoginInfo.UserId;
            return View();
        }

        public JsonResult SaveCustomerDomain()
        {
            try
            {
                var customerDomain = Request["customerDomain"];
                WCFClients.ExternalClient.SetOCAgentCustomerDomain(this.CurrentUser.LoginInfo.UserId, customerDomain);
                return Json(new { IsSuccess = true, Msg = "保存自定义链接成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }


        /// <summary>
        /// 设置新用户返点
        /// </summary>
        /// <returns></returns>
        public ActionResult SetNewUserRebate()
        {
            //查询当前用户代理类型   是否为总代理  或者为  总代理的下级代理
            if (CurrentUser.LoginInfo.IsAgent)
            {
                ViewBag.AgentType = WCFClients.ExternalClient.QueryAgentType(CurrentUser.LoginInfo.UserId);
            }
            ViewBag.InItNewUserRebate = WCFClients.ExternalClient.QueryUserRebate(this.CurrentUser.LoginInfo.UserId);
            return View();
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
                return Json(new { IsSuccess = true, Msg = UserRebate.Message });

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
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

                var isRealName = WCFClients.ExternalClient.CheckIsAuthenticatedUserRealName(id, UserToken);
                if (!isRealName)
                {
                    PreconditionAssert.IsNotEmptyString("", "对不起，用户未进行实名认证");
                }
                var UserRebate = WCFClients.ExternalClient.UpdateOCAgentRebate(this.CurrentUser.LoginInfo.UserId, id, dataStr);
                return Json(new { IsSuccess = true, Msg = UserRebate.Message });
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

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
                ViewBag.SetNewUserRebate = WCFClients.ExternalClient.QueryUserRebate(userId);
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
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                var key = Request["key"];
                if (!string.IsNullOrEmpty(key))
                {
                    ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                    ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                    ViewBag.GeneralizeUser = WCFClients.ExternalClient.GetAgentUserByKeyword(key, 1, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                }
                else
                {
                    ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                    ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                    ViewBag.GeneralizeUser = WCFClients.ExternalClient.GetLowerAgentUserByAgentId(CurrentUser.LoginInfo.UserId, 1, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                }
            }
            catch (Exception)
            {
                ViewBag.GeneralizeUser = null;
            }
            return View();
        }
        /// <summary>
        /// 关键字查询代理
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryAgentByKeyWord()
        {
            try
            {
                string agentId = Request["agentId"];
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var AgentInfo = WCFClients.ExternalClient.GetLowerAgentUserByAgentId(agentId, 1, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                return Json(new { IsSuccess = true, AgentInfo = AgentInfo });
            }
            catch (Exception)
            {
                ViewBag.AgentInfo = null;
                return Json(new { });
            }

        }
        /// <summary>
        /// 根据关键字查询代理的部分视图
        /// </summary>
        /// <param name="AgentUser"></param>
        /// <returns></returns>
        public PartialViewResult AgentUserShow(AgentUserInfoCollection AgentUser)
        {
            ViewBag.AgentUserInfo = AgentUser;
            return PartialView();
        }
        /// <summary>
        /// 我的提成
        /// </summary>
        /// <returns></returns>
        public ActionResult MyTc()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.InItNewUserRebate = WCFClients.ExternalClient.QueryUserRebate(this.CurrentUser.LoginInfo.UserId);
            return View();
        }
        /// <summary>
        /// 银行卡返点提现
        /// </summary>
        /// <returns></returns>
        public ActionResult BankCommissionExtract()
        {
            return View();
        }
        /// <summary>
        /// 返点结算
        /// </summary>
        /// <returns></returns>
        public ActionResult CommissionManage()
        {
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                //var a = new AgentPayDetailReportInfo { GameType = "spf", GameCode = "jczq", CommissionMoney = 100, TotalMoney = 100 };
                //var b = new AgentPayDetailReportInfo { GameType = "sf", GameCode = "jclq", CommissionMoney = 100, TotalMoney = 100 };
                //var c = new AgentPayDetailReportInfo { GameType = "t14c", GameCode = "ctzq", CommissionMoney = 100, TotalMoney = 100 };
                //var d = new AgentPayDetailReportInfo { GameType = "sxds", GameCode = "bjdc", CommissionMoney = 100, TotalMoney = 100 };
                //var e = new AgentPayDetailReportInfo { GameType = "", GameCode = "cqssc", CommissionMoney = 100, TotalMoney = 100 };
                //List<AgentPayDetailReportInfo> aa = new List<AgentPayDetailReportInfo>();
                //aa.Add(a);
                //aa.Add(b);
                //aa.Add(c);
                //aa.Add(d);
                //aa.Add(e);
                //ViewBag.AgentPayDetailReport = new AgentPayDetailReportInfoCollection { List = aa, TotalCount = 10 };
                ViewBag.AgentPayDetailReport = WCFClients.ExternalClient.QueryAgentPayDetailReportInfo(ViewBag.Begin, ViewBag.End, UserToken);
            }
            catch (Exception)
            {

                ViewBag.AgentPayDetailReport = null;
            }
            return View();
        }
        /// <summary>
        /// 返点记录
        /// </summary>
        /// <returns></returns>
        public ActionResult CommissionRecord()
        {
            ViewBag.CurrentUser = CurrentUser;
            try
            {
                ViewBag.Begin = string.IsNullOrEmpty(Request.QueryString["begin"]) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(Request.QueryString["begin"]);
                ViewBag.End = string.IsNullOrEmpty(Request.QueryString["end"]) ? DateTime.Today.AddDays(1) : DateTime.Parse(Request.QueryString["end"]);
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                var beginTime = ViewBag.Begin;
                if (beginTime < DateTime.Now.AddMonths(-3))
                    ViewBag.OCAgentPayDetail = null;
                else
                    ViewBag.OCAgentPayDetail = WCFClients.ExternalClient.QueryOCAgentPayDetailList(ViewBag.Begin, ViewBag.End, ViewBag.pageNo, ViewBag.pageSize, UserToken);
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        /// <summary>
        /// 支付宝返点提取
        /// </summary>
        /// <returns></returns>
        public ActionResult AlipayCommissionExtract()
        {
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
                //查询当前用户代理类型   是否为总代理  或者为  总代理的下级代理
                ViewBag.CurrentUser = CurrentUser;

                var key = Request["key"];
                if (!string.IsNullOrEmpty(key))
                {
                    ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                    ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                    ViewBag.GeneralizeUser = WCFClients.ExternalClient.GetAgentUserByKeyword(key, 0, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                }
                else
                {
                    ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                    ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                    ViewBag.GeneralizeUser = WCFClients.ExternalClient.GetLowerAgentUserByAgentId(CurrentUser.LoginInfo.UserId, 0, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                }
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
            return View();
        }

        /// <summary>
        /// 注册新代理
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterNewAgency()
        {
            //查询当前用户代理类型   是否为总代理  或者为  总代理的下级代理
            ViewBag.CurrentUser = CurrentUser;
            if (CurrentUser.LoginInfo.IsAgent)
            {
                ViewBag.AgentType = WCFClients.ExternalClient.QueryAgentType(CurrentUser.LoginInfo.UserId);
            }
            return View();
        }
        public ActionResult NewAgencyList()
        {
            ViewBag.StoreMessage = WCFClients.ExternalClient.QueryStoreMessageByusreId(CurrentUser.LoginInfo.UserId);
            ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        public JsonResult NewAgency()
        {
            var urls = Request.Url;
            ViewBag.Error = "";
            try
            {
                string userName = PreconditionAssert.IsNotEmptyString(Request["userName"], "登录账号不能为空。");
                string passWord = PreconditionAssert.IsNotEmptyString(Request["passWord"], "登录密码不能为空。");

                PreconditionAssert.IsFalse(ValidateHelper.IsEmail(userName), "用户名不能是邮箱地址。");

                RegisterInfo_Local registerInfo = new RegisterInfo_Local()
                {
                    RegType = "LOCAL",
                    LoginName = userName,
                    Password = passWord,
                    RegisterIp = IpManager.IPAddress,
                    ReferrerUrl = urls.ToString(),

                };

                #region ------ 读取用户注册访问参数 -----
                string referrer = "";
                string userId = "";
                string userComeFrom = "";
                string agentId = "";

                //从Session里读取推广访问参数
                if (Session["pid"] != null)
                {
                    agentId = Session["pid"].ToString();
                }

                //如果用户来源不为空，则记录为注册类型
                if (!string.IsNullOrEmpty(userComeFrom))
                {
                    registerInfo.RegType = userComeFrom;
                }
                registerInfo.Referrer = referrer;
                registerInfo.AgentId = agentId;
                #endregion

                var result = WCFClients.ExternalClient.RegisterLoacalAgent(registerInfo, CurrentUser.LoginInfo.UserId, OCAgentCategory.SportLotterySubAgent);
                LoginInfo loginInfo;
                if (result.IsSuccess)
                {
                    loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                    //if (loginInfo.IsSuccess)
                    //{
                    //    WCFClients.ExternalClient.AddAgent(OCAgentCategory.SportLotterySubAgent, CurrentUser.LoginInfo.UserId, loginInfo.UserId);
                    //}
                }
                return Json(new { IsSuccess = true, Msg = "添加成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }

        }

    }
}
