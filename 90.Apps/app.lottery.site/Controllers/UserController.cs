using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using Common.Cryptography;
using Common.Gateway;
using Common.Gateway.Alipay.WAPPay;
using Common.Gateway.KQ.Pay;
using Common.Gateway.Tenpay.Pay;
using Common.Gateway.YinBao;
using Common.JSON;
using Common.Log;
using Common.Net;
using Common.Net.SMS;
using Common.Utilities;
using Common.XmlAnalyzer;
using External.Core.Authentication;
using External.Core.Login;
using External.Core.SiteMessage;
using GameBiz.Core;
using app.lottery.site.Controllers;
using app.lottery.site.Models;
using Common.Gateway.Alipay.Pay;
using Common.Gateway.Alipay.Login;
using System.Configuration;
using System.Web.Security;
using app.lottery.site.iqucai.api;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using System.Threading.Tasks;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;
using EntityModel;
using EntityModel.Communication;

namespace app.lottery.site.iqucai.Controllers
{
    public class UserController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public UserController(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion

        #region 配置
        /// <summary>
        /// 禁止注册IP
        /// </summary>
        public string BanRegistrIP
        {
            get
            {
                string defalutValue = "";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("BanRegistrIP").ConfigValue;
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
        /// 禁止注册IP次数
        /// </summary>
        public int BanRegistrFrequencyIPCount
        {
            get
            {
                int defalutValue = 0;
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("BanRegistrFrequencyIPCount").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        int.TryParse(v, out defalutValue);
                        return defalutValue;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 禁止注册IP分钟
        /// </summary>
        public int BanRegistrFrequencyIPTime
        {
            get
            {
                int defalutValue = 0;
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("BanRegistrFrequencyIPTime").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        int.TryParse(v, out defalutValue);
                        return defalutValue;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }
        #endregion


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            CurrentUser = null;//跳转到登录页面时，把当前用户Session置空
            return View();
        }

        public ActionResult Register_old()
        {
            if (!string.IsNullOrEmpty(Request["pid"]))
            {
                string pid = Request["pid"];
                Session["pid"] = pid;
            }

            return View();
        }

        public ActionResult Register()
        {
            ViewBag.VerifyCode = CommonAPI.QueryCoreConfigByKey();
            return View();
        }


        public ActionResult Registersuc()
        {
            if (CurrentUser == null)
            {
                Response.Redirect("/statichtml/register.html");
            }
            ViewBag.UserRegister = WCFClients.ExternalClient.QueryUserRegisterCount();
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;

            var activity = WCFClients.ExternalClient.QueryActivInfoList(0, 1000);
            ViewBag.IsShow = activity.List.Where(p => p.ActiveName == "register" && p.EndTime >= DateTime.Now.Date).FirstOrDefault();
            return View();
        }
        /// <summary>
        /// 新注册成功页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult NewRegistersuc()
        //{
        //    string userName = Request["userName"];
        //    string passWord = Request["passWord"];
        //    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passWord))
        //    {
        //        LoginInfo loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
        //        if (loginInfo.IsSuccess)
        //        {
        //            CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
        //        }
        //    }
        //    if (CurrentUser == null)
        //    {
        //        Response.Redirect("/statichtml/register.html");
        //        return null;
        //    }
        //    //ViewBag.UserRegister = WCFClients.ExternalClient.QueryUserRegisterCount();
        //    ViewBag.CurrentUser = CurrentUser;
        //    return View();
        //}

        /// <summary>
        /// 指定代理注册
        /// </summary>
        [UnionFilter]
        public ActionResult AgencyRegister()
        {
            //ILogWriter log = Common.Log.LogWriterGetter.GetLogWriter();
            if (Request.HttpMethod == "POST")
            {
                var urls = Request.Url;
                ViewBag.Error = "";
                try
                {
                    var TouchSite = ConfigurationManager.AppSettings["TouchSite"];
                    string userName = PreconditionAssert.IsNotEmptyString(Request.Form["iName"], "登录账号不能为空。");
                    string passWord = PreconditionAssert.IsNotEmptyString(Request.Form["iCode"], "登录密码不能为空。");

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

                    var result = WCFClients.ExternalClient.RegisterLoacal(registerInfo);
                    LoginInfo loginInfo;
                    if (result.IsSuccess)
                    {
                        loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                        if (loginInfo.IsSuccess)
                        {
                            //CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                            Session["userCurrentPassWord"] = passWord;
                        }
                        string userToken = loginInfo.UserToken.Replace("+", "%2B");
                        string sbumiturl = TouchSite + "/home/loginfortoken?token=" + userToken;
                        Response.Redirect(sbumiturl);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View();
                }
            }
            return View();
        }

        #region 订单详情
        //订单详情
        public ActionResult Scheme(string id)
        {
            ViewBag.SchemeId = string.IsNullOrEmpty(id) ? "" : id;
            return View();
        }

        //普通订单详情
        public PartialViewResult Ordinary(string id)
        {
            try
            {
                #region 通过json文件查询数据

                ViewBag.User = CurrentUser;
                var info = WebRedisHelper.QuerySportsSchemeInfo(id).Result;// this.QuerySportsSchemeInfo(id);
                ViewBag.SchemeInfo = info;
                ViewBag.OrderDetail = WebRedisHelper.QueryOrderDetailBySchemeId(id);// this.QueryOrderDetailBySchemeId(id);
                ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"].ToString());
                ViewBag.FollowedCount = WebRedisHelper.QueryProfileFollowedCount(info.UserId, info.GameCode, info.GameType);// WCFClients.GameClient.QueryProfileFollowedCount(info.UserId, info.GameCode, info.GameType);
                ViewBag.SportsTicket = WebRedisHelper.QuerySportsTicketList(info.SchemeId, 0, 5).Result.TicketList;// this.QuerySportsTicketList(info.SchemeId, 0, 5, UserToken, out totalTicketCount);
                if (info.SchemeType ==EntityModel.Enum.SchemeType.SingleCopy)
                    ViewBag.BDFXCommision = WebRedisHelper.QueryBDFXCommision(id, info.BonusStatus);// this.QueryBDFXCommision(id, info.BonusStatus);

                var sign = string.IsNullOrEmpty(Request["sign"]) ? "" : Request["sign"];
                var mySign = Encipherment.MD5(id);
                if (sign == mySign
                    || (CurrentUser != null && CurrentUser.LoginInfo.UserId == info.UserId)
                    || info.Security == EntityModel.Enum.TogetherSchemeSecurity.Public
                    //|| (info.Security == TogetherSchemeSecurity.JoinPublic && WCFClients.GameClient.IsUserJoinSportsTogether(id, UserToken))
                    || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.CompletePublic && info.StopTime <= DateTime.Now)
                    || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.FirstMatchStopPublic && info.StopTime <= DateTime.Now))
                {
                    var ante = WebRedisHelper.QuerySportsOrderAnteCodeList(id, info.BonusStatus);// this.QuerySportsOrderAnteCodeList(id, UserToken, info.BonusStatus);
                    ViewBag.AnteList = ante;

                    if (info.SchemeBettingCategory == EntityModel.Enum.SchemeBettingCategory.SingleBetting || info.SchemeBettingCategory == EntityModel.Enum.SchemeBettingCategory.FilterBetting)
                    {
                        // 查询单式上传
                        var singleInfo = WebRedisHelper.QuerySingleSchemeFullFileName(info.SchemeId, info.BonusStatus);// this.QuerySingleSchemeFullFileName(info.SchemeId, UserToken, info.BonusStatus);
                        ViewBag.SingleInfo = singleInfo;
                    }
                }
                #endregion
            }
            catch
            {
            }
            return PartialView();
        }
        //追号方案详情
        public PartialViewResult Chase(string id)
        {
            try
            {
                #region 通过json文件查询数据

                ViewBag.SchemeId = id;
                ViewBag.User = CurrentUser;
                var detailResult = WebRedisHelper.QueryBettingOrderListByChaseKeyLine(id).Result;// this.QueryBettingOrderListByChaseKeyLine(id, UserToken);
                ViewBag.ChaseSchemeInfo = detailResult;
                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"].ToString());
                ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"].ToString());
                if (detailResult.OrderList.Count > 0)
                {
                    var first = detailResult.OrderList[0];
                    ViewBag.FirstItem = first;
                    var sign = string.IsNullOrEmpty(Request["sign"]) ? "" : Request["sign"];
                    var mySign = Encipherment.MD5(id);
                    if (mySign == sign || first.Security != EntityModel.Enum.TogetherSchemeSecurity.KeepSecrecy)
                    {
                        ViewBag.FirstAnteCode = WebRedisHelper.QueryAnteCodeListBySchemeId(first.SchemeId, detailResult.OrderList[0].BonusStatus);// this.QueryAnteCodeListBySchemeId(first.SchemeId, UserToken, detailResult.OrderList[0].BonusStatus);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
            }
            return PartialView();
        }

        [HttpPost]
        public JsonResult Cancel(string schemeId)
        {
            try
            {
                schemeId = PreconditionAssert.IsNotEmptyString(Request["schemeId"], "该方案号不存在");
                var result = WCFClients.GameClient.CancelChaseOrder(schemeId);
                if (result.IsSuccess)
                {
                    return Json(new { Success = true, Msg = "取消追号成功" });
                }
                return Json(new { Success = false, Msg = "取消追号失败" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Msg = ex.Message });
            }
        }

        public JsonResult page()
        {
            var PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"].ToString());
            var PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"].ToString());
            var id = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
            try
            {
                var SportsTicket = WebRedisHelper.QuerySportsTicketList(id, PageIndex, PageSize);// WCFClients.GameClient.QuerySportsTicketList(id, PageIndex, PageSize, UserToken);
                return Json(new { Issucess = true, msg = SportsTicket, PageIndex = PageIndex, PageSize = PageSize, SchemeId = id });
            }
            catch (Exception ex)
            {
                return Json(new { Issucess = false, msg = ex.Message });
            }

        }

        /// <summary>
        /// 票数据
        /// </summary>
        /// <returns></returns>
        public PartialViewResult SportsTicket()
        {
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"]);
            var id = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
            ViewBag.UserId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
            ViewBag.schemeStatus = string.IsNullOrEmpty(Request["status"]) ? "" : Request["status"];
            ViewBag.SchemeId = id;
            var collection = WebRedisHelper.QuerySportsTicketList(id, (int)ViewBag.PageIndex, (int)ViewBag.PageSize).Result;// this.QuerySportsTicketList(id, ViewBag.PageIndex, ViewBag.PageSize, UserToken, out  totalTicketCount);
            ViewBag.SportsTicket = collection.TicketList;
            ViewBag.TotalTicketCount = collection.TotalCount;
            return PartialView();
        }
        /// <summary>
        /// 参与合买用户记录
        /// </summary>
        /// <returns></returns>
        public PartialViewResult JoinUserList()
        {
            var schemeId = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];
            var pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"].ToString());
            var pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"].ToString());
            // var bonus = string.IsNullOrEmpty(Request["BonusStatus"]) ? BonusStatus.Waitting : (BonusStatus)int.Parse(Request["BonusStatus"]);
            var info = WebRedisHelper.QuerySportsTogetherDetail(schemeId).Result;// this.QuerySportsTogetherDetail(id);
            var allJoinInfo = WebRedisHelper.QuerySportsTogetherJoinList(schemeId, info.BonusStatus).Result;// this.QuerySportsTogetherJoinList(schemeId, UserToken, bonus);
            var joinInfo = new EntityModel.CoreModel.Sports_TogetherJoinInfoCollection();
            joinInfo.TotalCount = allJoinInfo.TotalCount;
            joinInfo.List = allJoinInfo.List.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            ViewBag.join = joinInfo;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.SchemeId = schemeId;
            ViewBag.TotalMoney = string.IsNullOrEmpty(Request["TotalMoney"]) ? 0 : decimal.Parse(Request["TotalMoney"]);
            ViewBag.SoldCount = string.IsNullOrEmpty(Request["SoldCount"]) ? 0 : decimal.Parse(Request["SoldCount"]);
            ViewBag.Guarantees = string.IsNullOrEmpty(Request["Guarantees"]) ? 0 : decimal.Parse(Request["Guarantees"]);
            ViewBag.ProgressStatus = string.IsNullOrEmpty(Request["ProgressStatus"]) ? 0 : (int.Parse(Request["ProgressStatus"]));
            ViewBag.CreateName = string.IsNullOrEmpty(Request["CreaterDisplayName"]) ? "" : Request["CreaterDisplayName"];
            ViewBag.User = CurrentUser;
            return PartialView();
        }

        public JsonResult HmUserPager()
        {
            var PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"].ToString());
            var PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"].ToString());
            var id = string.IsNullOrEmpty(Request["SchemeId"]) ? "" : Request["SchemeId"];

            try
            {
                var info = WebRedisHelper.QuerySportsTogetherDetail(id).Result;// this.QuerySportsTogetherDetail(id);
                var join = WebRedisHelper.QuerySportsTogetherJoinList(id, info.BonusStatus);// WCFClients.GameClient.QuerySportsTogetherJoinList(id, PageIndex, PageSize, UserToken);
                return Json(new { Issucess = true, msg = join, PageIndex = PageIndex, PageSize = PageSize, SchemeId = id });
            }
            catch (Exception ex)
            {
                return Json(new { Issucess = false, msg = ex.Message });
            }
        }
        //合买方案详情
        public async Task<PartialViewResult> Hmdetail(string id)
        {
            try
            {
                #region 通过json文件查询数据
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["schemeId"] = id; param["userId"] = UserToken;
                ViewBag.SchemeId = id;
                ViewBag.user = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;

                var info = WebRedisHelper.QuerySportsTogetherDetail(id).Result;// this.QuerySportsTogetherDetail(id);
                ViewBag.Detail = info;
                var allJoinInfo = WebRedisHelper.QuerySportsTogetherJoinList(id, info.BonusStatus);// this.QuerySportsTogetherJoinList(id, UserToken, info.BonusStatus);
                ViewBag.JoinAllList = allJoinInfo;
                ViewBag.OrderDetail = WebRedisHelper.QueryOrderDetailBySchemeId(id);// this.QueryOrderDetailBySchemeId(id);
                ViewBag.FollowedCount = WebRedisHelper.QueryProfileFollowedCount(info.CreateUserId, info.GameCode, info.GameType);// WCFClients.GameClient.QueryProfileFollowedCount(info.CreateUserId, info.GameCode, info.GameType);
                var collection = WebRedisHelper.QuerySportsTicketList(info.SchemeId, 0, 5).Result;
                ViewBag.SportsTicket = collection.TicketList;
                var sign = string.IsNullOrEmpty(Request["sign"]) ? "" : Request["sign"];
                var mySign = Encipherment.MD5(id);
                if (sign == mySign
                    || (CurrentUser != null && CurrentUser.LoginInfo.UserId == info.CreateUserId)
                    || info.Security == EntityModel.Enum.TogetherSchemeSecurity.Unkown
                    || info.Security == EntityModel.Enum.TogetherSchemeSecurity.Public
                    || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.JoinPublic && await serviceProxyProvider.Invoke<bool>(param, "api/order/IsUserJoinSportsTogether"))
                    || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.CompletePublic && info.StopTime <= DateTime.Now))
                {
                    var ante = WebRedisHelper.QuerySportsOrderAnteCodeList(id, info.BonusStatus);// this.QuerySportsOrderAnteCodeList(id, UserToken, info.BonusStatus);
                    ViewBag.AnteList = ante;

                    if (info.SchemeBettingCategory == EntityModel.Enum.SchemeBettingCategory.SingleBetting || info.SchemeBettingCategory == EntityModel.Enum.SchemeBettingCategory.FilterBetting || info.SchemeBettingCategory == EntityModel.Enum.SchemeBettingCategory.XianFaQiHSC)
                    {
                        var singleInfo = WebRedisHelper.QuerySingleSchemeFullFileName(info.SchemeId, info.BonusStatus);// this.QuerySingleSchemeFullFileName(info.SchemeId, UserToken, info.BonusStatus);
                        ViewBag.SingleInfo = singleInfo;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
            }
            return PartialView();
        }

        //单式方案详情
        public async Task<ActionResult> SchemeFile(string id)
        {
            try
            {
                ViewBag.SchemeId = id;
                var sign = string.IsNullOrEmpty(Request["sign"]) ? "" : Request["sign"].ToLower();
                var mySign = Encipherment.MD5(id, Encoding.UTF8).ToLower();
                if (id.StartsWith("TSM"))
                {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["schemeId"] = id; param["userId"] = UserToken;
                    var info = WebRedisHelper.QuerySportsTogetherDetail(id).Result;// this.QuerySportsTogetherDetail(id);
                    if (sign == mySign
                        || (CurrentUser != null && CurrentUser.LoginInfo.UserId == info.CreateUserId)
                        || info.Security == EntityModel.Enum.TogetherSchemeSecurity.Public
                        || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.JoinPublic && await serviceProxyProvider.Invoke<bool>(param, "api/order/IsUserJoinSportsTogether"))
                        || (info.Security == EntityModel.Enum.TogetherSchemeSecurity.CompletePublic && info.StopTime <= DateTime.Now))
                    {
                        var singleInfo = WebRedisHelper.QueryOrderSingleScheme(id, info.BonusStatus);// this.QueryOrderSingleScheme(id, info.BonusStatus);
                        ViewBag.SingleInfo = singleInfo;
                    }
                    else
                    {
                        return Content("<script type='text/javascript'>alert('您暂无权限打开该页面');window.close();</script>");
                    }
                }
                else
                {
                    var singleinfo = WebRedisHelper.QuerySportsSchemeInfo(id).Result;//this.QuerySportsSchemeInfo(id);
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["schemeId"] = id; param["userId"] = UserToken;
                    if (sign == mySign
                        || (CurrentUser != null && CurrentUser.LoginInfo.UserId == singleinfo.UserId)
                        || singleinfo.Security == EntityModel.Enum.TogetherSchemeSecurity.Public
                        || (singleinfo.Security == EntityModel.Enum.TogetherSchemeSecurity.JoinPublic && await serviceProxyProvider.Invoke<bool>(param, "api/order/IsUserJoinSportsTogether"))
                        || (singleinfo.Security == EntityModel.Enum.TogetherSchemeSecurity.CompletePublic && singleinfo.StopTime <= DateTime.Now))
                    {
                        var singleInfo = WebRedisHelper.QueryOrderSingleScheme(id, singleinfo.BonusStatus);// this.QueryOrderSingleScheme(id, singleinfo.BonusStatus);
                        ViewBag.SingleInfo = singleInfo;
                    }
                    else
                    {
                        return Content("<script type='text/javascript'>alert('您暂无权限打开该页面');window.close();</script>");

                    }
                }
            }
            catch (Exception ex)
            {
                return Content("<script type='text/javascript'>alert('" + ex.Message + "');window.close();</script>");
            }
            return View();
        }

        //过滤详情
        public ActionResult filterfile(string id)
        {
            try
            {
                ViewBag.SchemeId = id;
                //var singleInfo = WebRedisHelper.QuerySingleSchemeFullFileName(id);// WCFClients.GameClient.QuerySingleSchemeFullFileName(id, UserToken);
                //ViewBag.SingleInfo = singleInfo;
                //ViewBag.Game = Request["game"];
                ////过滤的第几步
                //var step = string.IsNullOrEmpty(Request["step"]) ? "1" : Request["step"];

                //获取过滤方案文件
                //var filterListPath = singleInfo.AnteCodeFullFileName.Replace("AfterFilter", "FilterList");
                //var filterStr = ReadFileString(filterListPath);


                //获取详细过滤文件
                //var detailStr = ReadFileString(singleInfo.AnteCodeFullFileName.Replace("AfterFilter", "DetailFilter"));
                //var details = detailStr.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                ////获取详细过滤文件
                //var detailStr = ReadFileString(singleInfo.AnteCodeFullFileName.Replace("AfterFilter", "DetailFilter"));
                //var details = detailStr.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);


                List<string> anteList = new List<string>();
                #region 拆分过滤条件 - filterList
                //var steps = filterStr.Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var item in steps)
                //{
                //    var tmpFilter = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                //    //获取过滤过剩余方案
                //    var i = 0;
                //    var lastFids = new List<string>();
                //    foreach (var of in tmpFilter.Skip(1))
                //    {
                //        i++;
                //        if (i.ToString() != step)
                //        {
                //            continue;
                //        }
                //        var thisFid = of.Split(',')[0];
                //        lastFids.Add(thisFid);
                //        foreach (var di in details)
                //        {
                //            var tmp = 0;
                //            var dfList = di.Split('^')[0].Split(',');
                //            var ante = di.Split('^')[1];
                //            foreach (var fid in lastFids)
                //            {
                //                if (dfList.Select(a => a.Split('-')[0]).Contains(fid))
                //                {
                //                    tmp++;
                //                }
                //            }
                //            if (lastFids.Count == tmp)
                //            {
                //                anteList.Add(ante);
                //            }
                //        }

                //    }
                //}
                #endregion
                ViewBag.CheckResult = anteList;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public ContentResult ClearOrderCacheFile()
        {
            try
            {
                var schemeId = Request["schemeId"];
                //base.DoClearOrderCacheFile(schemeId);
                WebRedisHelper.DoClearOrderCacheFile(schemeId);
                return Content("清理缓存完成");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public ContentResult RefreshUserBalance()
        {
            try
            {
                var userId = Request["UserId"];
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("用户名不能为空");
                //var sign = Request["Sign"];
                //var source = string.Format("www.qcw.com_{0}", userId);
                //var mySign = Encipherment.MD5(source, Encoding.UTF8);
                //if (sign.ToUpper() != mySign.ToUpper())
                //    throw new Exception("加密值不匹配");
                //var userId = WCFClients.ExternalClient.GetUserId(userName);
                //if (string.IsNullOrEmpty(userId))
                //    throw new Exception(string.Format("未查询到用户名：{0}的信息", userName));
                WebRedisHelper.RefreshRedisUserBalance(userId);
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        #region 票样索取

        /// <summary>
        /// 索取票样
        /// </summary>
        public PartialViewResult RequestSampleTickets()
        {
            ViewBag.User = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;

            var schemeId = string.IsNullOrEmpty((Request["id"])) ? "" : Request["id"];
            ViewBag.SchemeId = schemeId;
            //ViewBag.SampleTicket = WCFClients.ExternalClient.QuerySampleTicketBySchemeCode(schemeId, CurrentUser.LoginInfo.UserId);
            return PartialView();
        }

        /// <summary>
        /// 票样申请请求
        /// </summary>
        public JsonResult RequestAddSampleTicket()
        {
            try
            {
                var schemeId = string.IsNullOrEmpty((Request["schemeid"])) ? "" : Request["schemeid"];
                var province = string.IsNullOrEmpty((Request["province"])) ? "" : Request["province"];
                var city = string.IsNullOrEmpty((Request["city"])) ? "" : Request["city"];
                var textaddress = string.IsNullOrEmpty((Request["textaddress"])) ? "" : Request["textaddress"];
                var addressMoney = string.IsNullOrEmpty((Request["addressMoney"])) ? 0 : decimal.Parse(Request["addressMoney"]);
                var info = WCFClients.GameClient.QuerySportsSchemeInfo(schemeId);
                var address = province + " " + city + " " + " " + textaddress;

                return Json(new { IsSuccess = true, Msg = "申请成功" });
                //var result = WCFClients.ExternalClient.AddSampleTicket(CurrentUser.LoginInfo.UserId, info.GameCode, info.GameType, schemeId, DealWithType.NoneDealWith, 0, "", "", address, addressMoney);
                //if (result.IsSuccess) return Json(new { IsSuccess = true, Msg = result.Message });
                //else return Json(new { IsSuccess = false, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 取消票样声请
        /// </summary>
        public JsonResult CancelSTRquest()
        {
            try
            {
                var id = int.Parse(Request["id"]);
                return Json(new { IsSuccess = true, Msg = "取消成功" });
                //var result = WCFClients.ExternalClient.CancelByUser(id);
                //if (result.IsSuccess)
                //{
                //    return Json(new { IsSuccess = true, Msg = result.Message });
                //}
                //else
                //{
                //    return Json(new { IsSuccess = false, Msg = result.Message });
                //}
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }


        }

        /// <summary>
        /// 确认收取票样
        /// </summary>
        /// <returns></returns>
        public JsonResult ConfirmReceiving()
        {
            try
            {
                var id = int.Parse(Request["id"]);
                return Json(new { IsSuccess = true, Msg = "确认成功" });
                //var result = WCFClients.ExternalClient.ConfirmReceiving(id);
                //if (result.IsSuccess)
                //{
                //    return Json(new { IsSuccess = true, Msg = result.Message });
                //}
                //else
                //{
                //    return Json(new { IsSuccess = false, Msg = result.Message });
                //}
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        #endregion

        #region 充值回调

        #region 支付宝充值

        /// <summary>
        /// 功能：付完款后跳转的页面（返回页）
        /// 版本：3.0
        /// 日期：2010-06-09
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////////页面功能说明///////////////////////
        /// 该页面可在本机电脑测试
        /// 该页面称作“返回页”，是由支付宝服务器同步调用，可当作是支付完成后的提示信息页，如“您的某某某订单，多少金额已支付成功”。
        /// 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
        /// 该页面可以使用ASP.NET开发工具调试，也可以使用写文本函数Log_result进行调试，该函数已被默认关闭
        /// TRADE_FINISHED(表示交易已经成功结束，为普通即时到帐的交易状态成功标识);
        /// TRADE_SUCCESS(表示交易已经成功结束，为高级即时到帐的交易状态成功标识);
        /// </summary>
        public ActionResult AlipayReturnUrl()
        {
            string notifyid = Request.QueryString["notify_id"];

            //获取支付宝ATN返回结果，true是正确的返回信息，false 是无效的
            NotifyHandler notify = new NotifyHandler(ali_Partner, ali_key, ali_Seller_Email);
            bool responseTxt = notify.VerificationNotifyID(notifyid);

            //生成Md5摘要
            string mysign = notify.GetMD5Sign(Request.QueryString);
            string sign = Request.QueryString["sign"];
            string trade_status = Request.QueryString["trade_status"]; //交易状态

            FillMoneyStatus status = FillMoneyStatus.Failed;
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();
            logWriter.Write("FillMoney", "AlipayNotifyUrl", LogType.Warning, "验证签名", "MD5结果:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt);

            if (mysign == sign && responseTxt == true)   //验证支付发过来的消息，签名是否正确
            {
                //获取支付宝的通知返回参数
                string trade_no = Request.QueryString["trade_no"];      //支付宝交易号
                string order_no = Request.QueryString["out_trade_no"];	//获取订单号
                string total_fee = Request.QueryString["total_fee"];	//获取总金额
                string subject = Request.QueryString["subject"];        //商品名称、订单名称
                string body = Request.QueryString["body"];              //商品描述、订单备注、描述
                string buyer_email = Request.QueryString["buyer_email"];//买家支付宝账号

                if (trade_status == "WAIT_BUYER_PAY")//   判断支付状态_等待买家付款（文档中有枚举表可以参考）            
                {
                    status = FillMoneyStatus.Requesting;
                }
                else if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")//   判断支付状态_TRADE_FINISHED交易成功（文档中有枚举表可以参考）
                //如果trade_status的值是TRADE_SUCCESSED，是因为您用的测试帐号是返回的这个结果，改成您自己的支付宝帐号和PID以及KEY，则返回的值才是TRADE_FINISHED            
                {
                    status = FillMoneyStatus.Success;
                }

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(order_no, status, decimal.Parse(total_fee), trade_status, body, trade_no, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                    else
                    {
                        ViewBag.Message = "充值已完成。 ";
                    }
                    ViewBag.IsSuccess = result.IsSuccess;
                    ViewBag.Message = result.IsSuccess ? total_fee : result.Message;
                    ViewBag.OrderNum = order_no;
                    ViewBag.TotalMoney = total_fee;
                }
                catch (Exception ex)
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "更新本地订单失败，请立即联系客服人员为你处理 - " + ex.Message;
                }
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "验证失败";    //支付失败，提示信息
            }
            return View();
        }

        /// <summary>
        /// 功能：支付宝主动通知调用的页面（通知页）
        /// 版本：3.0
        /// 日期：2010-05-28
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////页面功能说明///////////////////
        /// 创建该页面文件时，请留心该页面文件中无任何HTML代码及空格。
        /// 该页面不能在本机电脑测试，请到服务器上做测试。请确保外部可以访问该页面。
        /// 该页面调试工具请使用写文本函数log_result，该函数已被默认开启
        /// TRADE_FINISHED(表示交易已经成功结束，通用即时到帐反馈的交易状态成功标志);
        /// TRADE_SUCCESS(表示交易已经成功结束，高级即时到帐反馈的交易状态成功标志);
        /// 该通知页面主要功能是：对于返回页面（return_url.aspx）做补单处理。如果没有收到该页面返回的 success 信息，支付宝会在24小时内按一定的时间策略重发通知
        /// </summary>
        public void AlipayNotifyUrl()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();

            string notifyid = Request.Form["notify_id"];

            //获取支付宝ATN返回结果，true是正确的返回信息，false 是无效的
            NotifyHandler notify = new NotifyHandler(ali_Partner, ali_key, ali_Seller_Email);
            bool responseTxt = notify.VerificationNotifyID(notifyid);

            //生成Md5摘要
            string mysign = notify.GetMD5Sign(Request.Form);
            string sign = Request.Form["sign"];

            //获取支付宝的通知返回参数
            string trade_no = Request.Form["trade_no"];      //支付宝交易号
            string order_no = Request.Form["out_trade_no"];	//获取订单号
            string total_fee = Request.Form["total_fee"];	//获取总金额
            string subject = Request.Form["subject"];        //商品名称、订单名称
            string body = Request.Form["body"];              //商品描述、订单备注、描述
            string buyer_email = Request.Form["buyer_email"];//买家支付宝账号
            string trade_status = Request.Form["trade_status"];//交易状态

            FillMoneyStatus status = FillMoneyStatus.Failed;

            if (mysign == sign && responseTxt == true)   //验证支付发过来的消息，签名是否正确，只要成功进入这个判断里，则表示该页面已被支付宝服务器成功调用
            //但判断内出现自身编写的程序相关错误导致通知给支付宝并不是发送success的消息的情况或没有更新客户自身的数据库，请自身程序编写好应对措施，否则查明原因时困难之极
            {
                if (trade_status == "WAIT_BUYER_PAY")//   判断支付状态_等待买家付款（文档中有枚举表可以参考）            
                {
                    status = FillMoneyStatus.Requesting;
                }
                else if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")//   判断支付状态_TRADE_FINISHED交易成功（文档中有枚举表可以参考）
                //如果trade_status的值是TRADE_SUCCESSED，是因为您用的测试帐号是返回的这个结果，改成您自己的支付宝帐号和PID以及KEY，则返回的值才是TRADE_FINISHED            
                {
                    status = FillMoneyStatus.Success;
                }

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(order_no, status, decimal.Parse(total_fee), trade_status, body, trade_no, UserToken);
                    if (result.IsSuccess && status == FillMoneyStatus.Success)
                    {
                        DelCacheUserBalance();
                    }
                    //放入订单交易完成后的数据库更新程序代码，请务必保证response.Write出来的信息只有success
                    //为了保证不被重复调用，或重复执行数据库更新程序，请判断该笔交易状态是否是订单未处理状态
                    //logWriter.Write("FillMoney", "AlipayNotifyUrl", LogType.Information, "支付宝充值成功", "订单号：" + order_no + " 支付宝订单号：" + trade_no);
                    Response.Write("success");
                }
                catch (Exception ex)
                {
                    logWriter.Write("FillMoney", "AlipayNotifyUrl", ex);
                    Response.Write("fail");
                }

                //Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                //success与fail及其他字符的区别在于，支付宝的服务器若遇到success时，则不再发送请求通知（即不再调用该页面，让该页面再次运行起来），
                //若不是success，则支付宝默认没有收到成功的信息，则会反复不停地调用该页面直到失效，有效调用时间是24小时以内。
                //程序编写时，千万不要把这个当作是数据库执行成功后才通知给支付宝服务器是success，这个与数据库有关的程序代码工作及数据库更改成功与否无任何关系。
            }
            else
            {
                Response.Write("fail"); //支付失败，返回失败
                logWriter.Write("FillMoney", "AlipayNotifyUrl", LogType.Warning, "支付失败，签名验证失败", "MD5结果:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt);
                //写TXT文件，以记录支付宝是否异步返回记录，比对md5计算结果（如网站不支持写txt文件，可改成写数据库）
                //string TOEXCELLR = "MD5结果:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt;

            }
        }

        #endregion

        #region 财付通充值
        /// <summary>
        /// 付完款后跳转的页面（跳转页）
        /// </summary>
        [ValidateInput(false)]
        public ActionResult TenpayReturnUrl()
        {
            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(HttpContext);
            resHandler.setKey(tenpay_TenpayKey);

            //判断签名
            if (resHandler.isTenpaySign())
            {
                ///通知id
                string notify_id = resHandler.getParameter("notify_id");
                //商户订单号
                string out_trade_no = resHandler.getParameter("out_trade_no");
                //财付通订单号
                string transaction_id = resHandler.getParameter("transaction_id");
                //金额,以分为单位
                string total_fee = resHandler.getParameter("total_fee");
                //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                string discount = resHandler.getParameter("discount");
                //支付结果
                string trade_state = resHandler.getParameter("trade_state");

                string trade_mode = resHandler.getParameter("trade_mode");

                FillMoneyStatus status = FillMoneyStatus.Failed;

                if ("0".Equals(trade_state) && "1".Equals(trade_mode))
                {
                    status = FillMoneyStatus.Success;
                    //------------------------------
                    //处理业务开始
                    //------------------------------
                    try
                    {
                        var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(out_trade_no, status, decimal.Parse(total_fee) / 100, trade_state, "充值已完成-订单号：" + out_trade_no, transaction_id, UserToken);
                        if (result.IsSuccess)
                        {
                            DelCacheUserBalance();
                        }
                        else
                        {
                            ViewBag.Message = "充值已完成。 ";
                        }
                        ViewBag.IsSuccess = result.IsSuccess;
                        ViewBag.Message = result.IsSuccess ? total_fee : result.Message;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.IsSuccess = false;
                        ViewBag.Message = "更新本地订单失败，请立即联系客服人员为你处理 - " + ex.Message;
                    }
                    //注意交易单不要重复处理
                    //注意判断返回金额

                    //------------------------------
                    //处理业务完毕
                    //------------------------------
                }
                else
                {
                    //当做不成功处理
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "支付失败";
                }

            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "认证签名失败";    //支付失败，提示信息
            }

            //获取debug信息,建议把debug信息写入日志，方便定位问题
            //string debuginfo = resHandler.getDebugInfo();
            //Response.Write("<br/>debuginfo:" + debuginfo + "<br/>");

            return View();
        }

        /// <summary>
        /// 财付通主动通知调用的页面（通知页）
        /// </summary>
        [ValidateInput(false)]
        public void TenpayNotifyUrl()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();

            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(HttpContext);
            resHandler.setKey(tenpay_TenpayKey);

            //判断签名
            if (resHandler.isTenpaySign())
            {
                ///通知id
                string notify_id = resHandler.getParameter("notify_id");

                //通过通知ID查询，确保通知来至财付通
                //创建查询请求
                RequestHandler queryReq = new RequestHandler(HttpContext);
                queryReq.init();
                queryReq.setKey(tenpay_TenpayKey);
                queryReq.setGateUrl("https://gw.tenpay.com/gateway/verifynotifyid.xml");
                queryReq.setParameter("partner", tenpay_Partner);
                queryReq.setParameter("notify_id", notify_id);

                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                httpClient.setTimeOut(5);
                //设置请求内容
                httpClient.setReqContent(queryReq.getRequestURL());

                //后台调用
                if (httpClient.call())
                {
                    //设置结果参数
                    ClientResponseHandler queryRes = new ClientResponseHandler();
                    queryRes.setContent(httpClient.getResContent());
                    queryRes.setKey(tenpay_TenpayKey);

                    //判断签名及结果
                    //只有签名正确,retcode为0，trade_state为0才是支付成功
                    if (queryRes.isTenpaySign() && queryRes.getParameter("retcode") == "0" && queryRes.getParameter("trade_state") == "0" && queryRes.getParameter("trade_mode") == "1")
                    {
                        //取结果参数做业务处理
                        string out_trade_no = queryRes.getParameter("out_trade_no");
                        //财付通订单号
                        string transaction_id = queryRes.getParameter("transaction_id");
                        //金额,以分为单位
                        string total_fee = queryRes.getParameter("total_fee");
                        //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                        string discount = queryRes.getParameter("discount");

                        //------------------------------
                        //处理业务开始
                        //------------------------------
                        FillMoneyStatus status = FillMoneyStatus.Success;
                        try
                        {
                            var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(out_trade_no, status, decimal.Parse(total_fee) / 100, queryRes.getParameter("trade_state"), "充值已完成-订单号：" + out_trade_no, transaction_id, UserToken);
                            if (result.IsSuccess)
                            {
                                DelCacheUserBalance();
                            }
                            //放入订单交易完成后的数据库更新程序代码，请务必保证response.Write出来的信息只有success
                            //为了保证不被重复调用，或重复执行数据库更新程序，请判断该笔交易状态是否是订单未处理状态
                            //logWriter.Write("FillMoney", "AlipayNotifyUrl", LogType.Information, "支付宝充值成功", "订单号：" + order_no + " 支付宝订单号：" + trade_no);
                            Response.Write("success");
                        }
                        catch (Exception ex)
                        {
                            logWriter.Write("FillMoney", "TenpayNotifyUrl", ex);
                            Response.Write("fail");
                        }

                        //------------------------------
                        //处理业务完毕
                        //------------------------------
                        //通知财付通已经处理成功，无需重新通知
                        Response.Write("success");

                    }
                    else
                    {
                        //错误时，返回结果可能没有签名，写日志trade_state、retcode、retmsg看失败详情。
                        //通知财付通处理失败，需要重新通知
                        Response.Write("fail");
                    }

                    //获取查询的debug信息,建议把请求、应答内容、debug信息，通信返回码写入日志，方便定位问题
                    /*
                    Response.Write("http res:" + httpClient.getResponseCode() + "," + httpClient.getErrInfo() + "<br>");
                    Response.Write("query req url:" + queryReq.getRequestURL() + "<br/>");
                    Response.Write("query req debug:" + queryReq.getDebugInfo() + "<br/>");
                    Response.Write("query res content:" + Server.HtmlEncode(httpClient.getResContent()) + "<br/>");
                    Response.Write("query res debug:" + Server.HtmlEncode(queryRes.getDebugInfo()) + "<br/>");
                     */
                }
                else
                {
                    //通知财付通处理失败，需要重新通知
                    Response.Write("fail");
                    //写错误日志
                    logWriter.Write("FillMoney", "TenpayNotifyUrl", LogType.Warning, "支付失败，签名验证失败", "错误信息：" + httpClient.getErrInfo() + " 响应代码：" + httpClient.getResponseCode());
                    //写TXT文件，以记录支付宝是否异步返回记录，比对md5计算结果（如网站不支持写txt文件，可改成写数据库）

                }

                //获取debug信息,建议把debug信息写入日志，方便定位问题
                /*
                string debuginfo = resHandler.getDebugInfo();
                Response.Write("<br/>debuginfo:" + debuginfo + "<br/>");
                */

            }
        }

        #endregion

        #region 快钱充值
        /// <summary>
        /// 付完款后跳转的页面（跳转页）
        /// </summary>
        public ActionResult KQShow()
        {
            try
            {
                string orderId = Request["orderId"].ToString().Trim();
                string orderAmount = Request["orderAmount"];
                string msg = Request["msg"].ToString().Trim();
                if (msg == "success")
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.Message = orderAmount;
                }
                else
                {
                    throw new Exception("充值失败，订单号：" + orderId + " 充值金额：" + orderAmount);
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        /// <summary>
        /// 快钱主动通知页面（通知页）
        /// </summary>
        public void KQNotifyUrl()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();
            string rtnOk = "0", msg = "", orderId = "", orderAmount = "";
            try
            {
                //获取商户订单号
                orderId = Request["orderId"].ToString().Trim();

                //获取原始订单金额
                ///订单提交到快钱时的金额，单位为分。
                ///比方2 ，代表0.02元
                orderAmount = Request["orderAmount"].ToString().Trim();

                //获取处理结果
                ///10代表 成功; 11代表 失败
                string payResult = Request["payResult"].ToString().Trim();

                //获取快钱交易号
                ///获取该交易在快钱的交易号
                string dealId = Request["dealId"].ToString().Trim();

                //string pubkey_path = Server.MapPath("~/certificate") + "\\" + kqConfig["PubKeyName"];
                KQHandler.SetKQConfig(kq_MerchantAcctId, kq_CertificatePW);
                string kqResult = KQHandler.VerifyResponse_MD5(Request);
                FillMoneyStatus status = FillMoneyStatus.Failed;

                msg = kqResult;

                if (kqResult == "success")
                {
                    status = FillMoneyStatus.Success;
                    rtnOk = "1";
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(orderId, status, decimal.Parse(orderAmount) / 100, payResult, "充值已完成-订单号：" + orderId, dealId, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                }
                else
                {
                    //支付失败，把订单置为失败
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(orderId, status, decimal.Parse(orderAmount) / 100, payResult, "充值失败，错误代码：" + Request["errCode"].ToString().Trim(), dealId, UserToken);
                    //写错误日志
                    logWriter.Write("FillMoney", "KQNotifyUrl", LogType.Warning, "支付失败，订单充值未成功。", "返回错误码：" + kqResult);
                }

            }
            catch (Exception ex)
            {
                //写错误日志
                logWriter.Write("FillMoney", "KQNotifyUrl", LogType.Error, "支付失败，完成订单出错", "错误信息：" + ex.Message);
            }
            string returnString = "<result>" + rtnOk + "</result><redirecturl>" + SiteRoot + "/User/KQShow?msg=" + Server.UrlEncode(msg) + "&orderId=" + orderId + "&orderAmount=" + decimal.Parse(orderAmount) / 100 + "</redirecturl>";
            Response.Write(returnString);
        }

        #endregion

        #region 支付宝WAP充值回调
        //支付宝WAP充值 - 页面跳转同步通知页面
        [ValidateInput(false)]
        public ActionResult wapalipayreturn()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();
            try
            {
                //获取所有get请求的参数
                SortedDictionary<string, string> sArrary = GetRequestGet();

                if (sArrary.Count < 1)
                {
                    //验签失败
                    Response.Write("fail");
                    throw new Exception("签名参数为空");
                }

                //生成本地签名sign
                string strSign = Function.BuildMysign(sArrary, Config.Key, Config.Sec_id, Config.Input_charset_UTF8);

                //获取支付宝返回sign
                string aliSign = Request["sign"];

                //验签对比
                if (!aliSign.Equals(strSign))
                {
                    //验签失败
                    Response.Write("fail");
                    throw new Exception("签名验证失败，本地签名：" + strSign + " 支付宝签名：" + aliSign);
                }

                string result = Request["result"];
                //比较result值是否为success
                if (!result.Equals("success"))
                {
                    //交易未成功
                    Response.Write("fail");
                    throw new Exception("支付失败");
                }
                else
                {
                    //交易成功，请填写自己的业务代码
                    Response.Write("success");

                    ///////////////////////////////处理数据/////////////////////////////////
                    // 用户这里可以写自己的商业逻辑
                    // 例如：修改数据库订单状态
                    // 仅进行演示如何调取
                    // 参数对照请详细查阅开发文档
                    // 里面有详细说明
                    // 参数获取 直接用Request["参数名"] GET方式获取即可
                    string out_trade_no = Request["out_trade_no"];
                    string trade_no = Request["trade_no"];

                    //根据支付宝返回的订单号查询本地订单信息
                    FillMoneyQueryInfo fillinfo = WCFClients.GameFundClient.QueryFillMoneyOrder(out_trade_no, UserToken);

                    var addRes = WCFClients.GameFundClient.CompleteFillMoneyOrder(out_trade_no, FillMoneyStatus.Success, fillinfo.RequestMoney, result, "充值完成", trade_no, UserToken);
                    if (addRes.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                    else
                    {
                        throw new Exception(addRes.Message);
                    }
                    ViewBag.IsSuccess = true;
                    ViewBag.Message = fillinfo.RequestMoney.ToString("N");

                    ////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception ex)
            {
                Response.Write("fail");
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
                logWriter.Write("FillMoney-ALIPAYWAP", "wapalipayreturn", LogType.Error, ex.Message, ex.ToString());
            }
            Response.Clear();
            return View();
        }

        //支付宝WAP充值 - 服务器异步通知页面
        [ValidateInput(false)]
        public void wapalipaynotify()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();
            try
            {
                //创建待签名数组，注意Notify这里数组不需要进行排序，请保持以下顺序
                Dictionary<string, string> sArrary = new Dictionary<string, string>();
                sArrary.Add("service", Request.Form["service"]);
                sArrary.Add("v", Request.Form["v"]);
                sArrary.Add("sec_id", Request.Form["sec_id"]);
                sArrary.Add("notify_data", Request.Form["notify_data"]);

                if (sArrary.Count < 1)
                {
                    //验签失败
                    Response.Write("fail");
                    logWriter.Write("FillMoney-ALIPAYWAP", "wapalipaynotify", LogType.Error, "签名验证失败", "签名参数为空");
                    return;
                }

                //生成签名，用于和post过来的签名进行对照
                string mysign = Function.BuildMysign(sArrary, Config.Key, Config.Sec_id, Config.Input_charset_UTF8);
                //支付宝post的签名
                string aliSign = Request.Form["sign"];

                if (!aliSign.Equals(mysign))
                {
                    //签名验证失败
                    Response.Write("fail");
                    logWriter.Write("FillMoney-ALIPAYWAP", "wapalipaynotify", LogType.Error, "签名验证失败", "本地签名：" + mysign + " 支付宝post签名：" + aliSign);
                    return;
                }

                //获取notify_data的值
                string notify_data = Request.Form["notify_data"];
                //获取 notify_data 参数中xml格式里面的 trade_status 值
                string trade_status = Function.GetStrForXmlDoc(notify_data, "notify/trade_status");

                //判断trade_status是否为TRADE_FINISHED
                if (!trade_status.Equals("TRADE_FINISHED"))
                {
                    //交易未成功
                    Response.Write("fail");
                }
                else
                {
                    //交易成功并在页面返回success
                    Response.Write("success");

                    ///////////////////////////////处理数据/////////////////////////////////
                    // 用户这里可以写自己的商业逻辑
                    // 例如：修改数据库订单状态
                    // 以下数据仅仅进行演示如何调取
                    // 参数对照请详细查阅开发文档
                    // 里面有详细说明

                    string out_trade_no = Function.GetStrForXmlDoc(notify_data, "notify/out_trade_no");
                    string total_fee = Function.GetStrForXmlDoc(notify_data, "notify/total_fee");
                    string trade_no = Function.GetStrForXmlDoc(notify_data, "notify/trade_no");

                    var addRes = WCFClients.GameFundClient.CompleteFillMoneyOrder(out_trade_no, FillMoneyStatus.Success, decimal.Parse(total_fee), trade_status, "充值完成", trade_no, UserToken);
                    if (addRes.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }

                    //string subject = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/subject");
                    //string buyer_email = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/buyer_email");
                    //string out_trade_no = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/out_trade_no");
                    //string total_fee = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/total_fee");
                    //string seller_email = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/seller_email");
                    //string price = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/price");
                    //string notify_id = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/notify_id");
                    //string gmt_payment = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/gmt_payment");
                    //string gmt_close = Alipay.Class.Function.GetStrForXmlDoc(notify_data, "notify/gmt_close");
                    ////////////////////////////////////////////////////////////////////////////
                }

            }
            catch (Exception ex)
            {
                Response.Write("fail");
                logWriter.Write("FillMoney-ALIPAYWAP", "wapalipaynotify", LogType.Error, ex.Message, ex.ToString());
            }
        }
        #endregion

        #region 银宝充值

        /// <summary>
        /// 功能：付完款后跳转的页面（返回页）
        /// 版本：3.0
        /// 日期：2010-06-09
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////////页面功能说明///////////////////////
        /// 该页面可在本机电脑测试
        /// 该页面称作“返回页”，是由支付宝服务器同步调用，可当作是支付完成后的提示信息页，如“您的某某某订单，多少金额已支付成功”。
        /// 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
        /// 该页面可以使用ASP.NET开发工具调试，也可以使用写文本函数Log_result进行调试，该函数已被默认关闭
        /// TRADE_FINISHED(表示交易已经成功结束，为普通即时到帐的交易状态成功标识);
        /// TRADE_SUCCESS(表示交易已经成功结束，为高级即时到帐的交易状态成功标识);
        /// </summary>
        public ActionResult yinbaoReturnUrl()
        {
            string p1_md = Request.QueryString["p1_md"];//网银
            string p2_sn = Request.QueryString["p2_sn"];//银宝系统生成的订单号
            string p3_xn = Request.QueryString["p3_xn"];//商户订单号
            string p4_amt = Request.QueryString["p4_amt"];//支付金额(两位小数)
            string p5_ex = Request.QueryString["p5_ex"];//商户自定义信息，原样返回
            string p6_pd = Request.QueryString["p6_pd"];//支付方式
            string p7_st = Request.QueryString["p7_st"];//成功=success,失败=faile
            string p8_reply = Request.QueryString["p8_reply"];//1=通知,2=显示
            string sign = Request.QueryString["sign"];//由以上字段加上商户KEY生成的MD5签名
            //生成Md5摘要
            var notify = new YinBaoNotify(yinbao_key);
            var singString = BuildSignString(Request.QueryString, yinbao_key);
            string mysign = YinBao.GetMD5(singString, YinBao._Input_Charset);  //  notify.GetMD5Sign(Request.QueryString);
            FillMoneyStatus status = FillMoneyStatus.Failed;
            WriteLog("YinBaoNotify", LogType.Error, "Sign加密信息", "singString：" + singString + "\r\n mysign:" + mysign + "\r\n sign:" + sign);
            if (mysign.ToLower() == sign.ToLower())   //验证支付发过来的消息，签名是否正确
            {
                if (p7_st == "success")//   判断支付状态_TRADE_FINISHED交易成功（文档中有枚举表可以参考）
                {
                    status = FillMoneyStatus.Success;
                }

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(p3_xn, status, decimal.Parse(p4_amt), p7_st, p5_ex, p2_sn, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                    else
                    {
                        ViewBag.Message = "充值已完成。 ";
                    }
                    ViewBag.IsSuccess = result.IsSuccess;
                    ViewBag.Message = result.IsSuccess ? p4_amt : result.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "更新本地订单失败，请立即联系客服人员为你处理 - " + ex.Message;
                    WriteException("YinBaoNotify", ex);
                }
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "验证失败";    //支付失败，提示信息
            }

            if (p8_reply == "1")  //判断通知的P8参数是否为１，若为１，要回复银宝success
            {
                Response.Clear();
                Response.Write("success");
                Response.End();
            }

            return View();
        }

        /// <summary>
        /// 构造md5签名字符串
        /// </summary>
        /// <param name="nvc">NameValues集合</param>
        /// <param name="key">安全校验码</param>
        /// <returns>返回排序后的字符串（自动剔除末尾的sign和sign_type类型）</returns>
        private string BuildSignString(System.Collections.Specialized.NameValueCollection nvc, string key)
        {
            string[] Sortedstr = YinBao.BubbleSort(nvc.AllKeys);  //对参数进行排序
            List<string> md5Str = new List<string>();
            foreach (var item in Sortedstr)
            {
                if (nvc.Get(item) != "" && item != "sign")
                {
                    md5Str.Add(item + "=" + nvc.Get(item));
                }
            }

            return string.Join("&", md5Str) + key;
        }

        #endregion

        #region 网银在线支付

        /// <summary>
        /// 功能：付完款后跳转的页面（返回页）
        /// </summary>
        [ValidateInput(false)]
        public ActionResult chinabankreturnurl()
        {
            //var result=Request
            var v_oid = Request["v_oid"];
            var v_pstatus = Request["v_pstatus"];
            //var v_pstring = Request["v_pstring"];

            //var v_pmode = Request["v_pmode"];
            Request.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            StreamReader sr = new StreamReader(Request.InputStream, Encoding.GetEncoding(936));
            String query = sr.ReadToEnd();
            System.Collections.Specialized.NameValueCollection reqResult = HttpUtility.ParseQueryString(query, Encoding.GetEncoding(936));

            var v_pstring = reqResult["v_pstring"];
            var v_pmode = reqResult["v_pmode"];

            var v_md5str = Request["v_md5str"];
            var v_amount = Request["v_amount"];
            var v_moneytype = Request["v_moneytype"];
            var remark1 = Request["remark1"];
            var remark2 = Request["remark2"];
            // orderId + status + money + moneytype + key;

            FillMoneyStatus status = FillMoneyStatus.Failed;
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();

            var cb = new ChinaBank(cb_mid, cb_key);

            if (cb.GetReciveSign(v_oid, v_pstatus, v_amount).ToLower() == v_md5str.ToLower())   //验证支付发过来的消息，签名是否正确
            {
                if (v_pstatus.Equals("20"))//   20:支付成功  30:支付失败
                {
                    status = FillMoneyStatus.Success;
                }
                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(v_oid, status, decimal.Parse(v_amount), v_pstatus, "支付银行编码:" + v_pmode, v_oid, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                    else
                    {
                        ViewBag.Message = "充值已完成。 ";
                    }

                    ViewBag.IsSuccess = result.IsSuccess;
                    ViewBag.Message = result.IsSuccess ? v_amount : result.Message;
                    ViewBag.OrderId = v_oid;
                }
                catch (Exception ex)
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "更新本地订单失败，请立即联系客服人员为你处理 - " + ex.Message;

                    WriteException("ChinaBankReturn", ex);
                }
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "验证失败";    //支付失败，提示信息
                WriteLog("ChinaBankReturn", LogType.Error, "验证失败", "OrderId:" + v_oid + " pstatus:" + v_pstatus + " money:" + v_amount + " sign:" + v_md5str + " moneytype:" + v_moneytype + " mysign:" + cb.GetSign(v_oid + v_pstatus + v_amount));
            }
            logWriter.Write("FillMoney", "chinabanknotifyurl", LogType.Warning, "验证签名", "MD5结果:mysign=" + cb.GetSign(v_oid + v_pstatus + v_amount) + ",sign:" + v_md5str + ",金额:money=" + v_amount + ",responseTxt=" + status + ",返回详细参数=" + reqResult.ToString());
            return View();
        }

        /// <summary>
        /// 功能：付完款 异步通知页
        /// </summary>
        [ValidateInput(false)]
        public ActionResult chinabanknotifyurl()
        {
            var v_oid = Request["v_oid"];
            var v_pstatus = Request["v_pstatus"];


            Request.ContentEncoding = System.Text.Encoding.GetEncoding("GBK");
            StreamReader sr = new StreamReader(Request.InputStream, Encoding.GetEncoding(936));
            String query = sr.ReadToEnd();
            System.Collections.Specialized.NameValueCollection reqResult = HttpUtility.ParseQueryString(query, Encoding.GetEncoding(936));
            var v_pstring = reqResult["v_pstring"];
            var v_pmode = reqResult["v_pmode"];

            var v_md5str = Request["v_md5str"];
            var v_amount = Request["v_amount"];
            var v_moneytype = Request["v_moneytype"];
            var remark1 = Request["remark1"];
            var remark2 = Request["remark2"];

            ILogWriter logWriter = LogWriterGetter.GetLogWriter();
            FillMoneyStatus status = FillMoneyStatus.Failed;
            var cb = new ChinaBank(cb_mid, cb_key);
            if (cb.GetReciveSign(v_oid, v_pstatus, v_amount).ToLower() == v_md5str.ToLower())   //验证支付发过来的消息，签名是否正确
            {
                if (v_pstatus.Equals("20"))//   20:支付成功  30:支付失败
                {
                    status = FillMoneyStatus.Success;
                }

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(v_oid, status, decimal.Parse(v_amount), v_pstatus, "支付银行编码:" + v_pmode, v_oid, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }

                    Response.Clear();
                    Response.Write("ok");
                    Response.End();
                }
                catch (Exception ex)
                {
                    WriteException("ChinaBankNotify", ex);
                    logWriter.Write("FillMoney", "chinabanknotifyurl", LogType.Warning, "验证签名", "MD5结果:mysign=" + cb.GetSign(v_oid + v_pstatus + v_amount) + "sign:" + v_md5str + "金额:money=" + v_amount + ",responseTxt=" + status + ",errMsg=" + ex.Message);
                    Response.Clear();
                    Response.Write("error");
                    Response.End();
                }
            }
            else
            {
                WriteLog("ChinaBankNotify", LogType.Error, "验证失败", "OrderId:" + v_oid + " pstatus:" + v_pstatus + " money:" + v_amount + " sign:" + v_md5str + " moneytype:" + v_moneytype + " mysign:" + cb.GetSign(v_oid + v_pstatus + v_amount));
                Response.Clear();
                Response.Write("error");
                Response.End();
            }
            logWriter.Write("FillMoney", "chinabanknotifyurl", LogType.Warning, "验证签名", "MD5结果:mysign=" + cb.GetSign(v_oid + v_pstatus + v_amount) + "sign:" + v_md5str + "金额:money=" + v_amount + ",responseTxt=" + status + ",返回参数=" + reqResult.ToString());
            return View();
        }
        #endregion

        #region 易极付充值

        /// <summary>
        /// 功能：付完款后跳转的页面（返回页）
        /// 版本：3.0
        /// 日期：2010-06-09
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////////页面功能说明///////////////////////
        /// 该页面可在本机电脑测试
        /// 该页面称作“返回页”，是由支付宝服务器同步调用，可当作是支付完成后的提示信息页，如“您的某某某订单，多少金额已支付成功”。
        /// 可放入HTML等美化页面的代码和订单交易完成后的数据库更新程序代码
        /// 该页面可以使用ASP.NET开发工具调试，也可以使用写文本函数Log_result进行调试，该函数已被默认关闭
        /// TRADE_FINISHED(表示交易已经成功结束，为普通即时到帐的交易状态成功标识);
        /// TRADE_SUCCESS(表示交易已经成功结束，为高级即时到帐的交易状态成功标识);
        /// </summary>
        public ActionResult yjfreturnurl()
        {
            string tradeStatus = Request.QueryString["tradeStatus"];
            //生成Md5摘要
            var mysign = YJFService.GetMD5SignWithNotifyUrl(Request.QueryString);
            string sign = Request.QueryString["sign"];

            FillMoneyStatus status = FillMoneyStatus.Failed;

            if (mysign == sign && (tradeStatus == "trade_finished" || tradeStatus == "wait_buyer_pay"))   //验证支付发过来的消息，签名是否正确
            {
                //获取通知返回参数
                string success = Request.QueryString["success"];
                string tradeNo = Request.QueryString["tradeNo"]; //交易号
                string orderNo = Request.QueryString["orderNo"];
                string notifyTime = Request.QueryString["notifyTime"];//.Replace(" ","%")
                string signType = Request.QueryString["signType"];
                string money = Request.QueryString["money"];  //获取总金额
                string username = Request.QueryString["username"];

                if (tradeStatus == "wait_buyer_pay")//   判断支付状态_等待买家付款（文档中有枚举表可以参考）            
                {
                    status = FillMoneyStatus.Requesting;
                }
                else if (tradeStatus == "trade_finished" || tradeStatus == "trade_finished")//   判断支付状态_TRADE_FINISHED交易成功（文档中有枚举表可以参考）
                //如果trade_status的值是TRADE_SUCCESSED，是因为您用的测试帐号是返回的这个结果，改成您自己的支付宝帐号和PID以及KEY，则返回的值才是TRADE_FINISHED            
                {
                    status = FillMoneyStatus.Success;
                }

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(orderNo, status, decimal.Parse(money), tradeStatus, username, tradeNo, UserToken);
                    if (result.IsSuccess)
                    {
                        DelCacheUserBalance();
                    }
                    else
                    {
                        ViewBag.Message = "充值已完成。 ";
                    }
                    ViewBag.IsSuccess = result.IsSuccess;
                    ViewBag.Message = result.IsSuccess ? money : result.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.Message = "更新本地订单失败，请立即联系客服人员为你处理 - " + ex.Message;
                }
            }
            else
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = "验证失败";    //支付失败，提示信息
            }
            return View();
        }

        /// <summary>
        /// 功能：易极付主动通知调用的页面（通知页）
        /// 版本：3.0
        /// 日期：2010-05-28
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////页面功能说明///////////////////
        /// 创建该页面文件时，请留心该页面文件中无任何HTML代码及空格。
        /// 该页面不能在本机电脑测试，请到服务器上做测试。请确保外部可以访问该页面。
        /// 该页面调试工具请使用写文本函数log_result，该函数已被默认开启
        /// TRADE_FINISHED(表示交易已经成功结束，通用即时到帐反馈的交易状态成功标志);
        /// TRADE_SUCCESS(表示交易已经成功结束，高级即时到帐反馈的交易状态成功标志);
        /// 该通知页面主要功能是：对于返回页面（return_url.aspx）做补单处理。如果没有收到该页面返回的 success 信息，支付宝会在24小时内按一定的时间策略重发通知
        /// </summary>
        public void yjfnotifyurl()
        {
            ILogWriter logWriter = LogWriterGetter.GetLogWriter();

            //生成Md5摘要
            var mysign = YJFService.GetMD5SignWithNotifyUrl(Request.Form);
            string sign = Request.Form["sign"];

            //获取异步通知返回参数
            string executeStatus = Request.Form["executeStatus"];
            string tradeNo = Request.Form["tradeNo"];
            string orderNo = Request.Form["orderNo"];
            string notifyTime = Request.Form["notifyTime"];
            string signType = Request.Form["signType"];
            string tradeAction = Request.Form["tradeAction"];

            string money = Request.QueryString["money"];
            string username = Request.QueryString["username"];

            FillMoneyStatus status = FillMoneyStatus.Failed;

            if (mysign == sign && executeStatus.ToLower() == "true")   //验证支付发过来的消息，签名是否正确，只要成功进入这个判断里，则表示该页面已被支付宝服务器成功调用
            //但判断内出现自身编写的程序相关错误导致通知给支付宝并不是发送success的消息的情况或没有更新客户自身的数据库，请自身程序编写好应对措施，否则查明原因时困难之极
            {
                status = FillMoneyStatus.Success;

                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(orderNo, status, decimal.Parse(money), executeStatus, username, tradeNo, UserToken);
                    if (result.IsSuccess && status == FillMoneyStatus.Success)
                    {
                        DelCacheUserBalance();
                    }
                    //放入订单交易完成后的数据库更新程序代码，请务必保证response.Write出来的信息只有success
                    //为了保证不被重复调用，或重复执行数据库更新程序，请判断该笔交易状态是否是订单未处理状态
                    //logWriter.Write("FillMoney", "AlipayNotifyUrl", LogType.Information, "支付宝充值成功", "订单号：" + order_no + " 支付宝订单号：" + trade_no);
                    Response.Write("success");
                }
                catch (Exception ex)
                {
                    logWriter.Write("FillMoney", "yjfnotifyurl", ex);
                    Response.Write("fail");
                }

                //Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                //success与fail及其他字符的区别在于，支付宝的服务器若遇到success时，则不再发送请求通知（即不再调用该页面，让该页面再次运行起来），
                //若不是success，则支付宝默认没有收到成功的信息，则会反复不停地调用该页面直到失效，有效调用时间是24小时以内。
                //程序编写时，千万不要把这个当作是数据库执行成功后才通知给支付宝服务器是success，这个与数据库有关的程序代码工作及数据库更改成功与否无任何关系。
            }
            else
            {
                Response.Write("fail"); //支付失败，返回失败
                logWriter.Write("FillMoney", "yjfnotifyurl", LogType.Warning, "支付失败，签名验证失败", "MD5结果:mysign = " + mysign + " , sign = " + sign + " , request = " + Request.Url);
                //写TXT文件，以记录支付宝是否异步返回记录，比对md5计算结果（如网站不支持写txt文件，可改成写数据库）
                //string TOEXCELLR = "MD5结果:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt;

            }
        }

        #endregion

        #region 充值免手续费

        /// <summary>
        /// 支付宝充值免手续费
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult AlipayReturnUrlFeeFree()
        {
            //TradeNO 支付宝流水号
            //Remark 网站订单号
            //TradeMoney 最终回复金额
            //TradeNO={0}&Remark={1}&TradeMoney={2}&sign={3}
            string tradeNO = Request.Form["TradeNO"];
            string remark = Request.Form["ReMark"];
            //decimal tradeMoney = Convert.ToDecimal(Request.Form["TradeMoney"]);
            var tradeMoney = Convert.ToDecimal(Request.Form["TradeMoney"]).ToString("N2");
            tradeMoney = tradeMoney.Replace(",", "");
            //var alipayTime = Convert.ToDateTime(Request.Form["DateTime"]);
            var alipayTime = Request.Form["DateTime"];
            string sign = Request.Form["sign"];
            if (string.IsNullOrEmpty(remark) || string.IsNullOrEmpty(sign))
                return Content("充值失败：未接收到订单号或加密值");

            string webKey = WCFClients.GameClient.QueryConfigByKey("AlipayFillMoneyKey").ConfigValue;
            string mySign = Encipherment.MD5(string.Format("{0}_{1}_{2}_{3}_{4}", tradeNO, remark, tradeMoney, alipayTime, webKey));//5ae11a5bfeaa49a4bb6e6cf6cb63d41d
            if (mySign == sign)
            {
                try
                {
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrderFeeFree(remark, FillMoneyStatus.Success, Convert.ToDecimal(Request.Form["TradeMoney"]), "Success", "充值完成", tradeNO, Convert.ToDateTime(Convert.ToDateTime(alipayTime).ToString("yyyy-MM-dd HH:mm")), this.UserToken);
                    if (result.IsSuccess)
                    {
                        return Content("充值成功");
                    }
                }
                catch (Exception ex)
                {
                    return Content("充值失败:" + ex.Message);
                }
            }
            return Content("充值失败:加密不正确");
        }

        #endregion
        #region 环迅接口回调

        public ActionResult IPSReturnUrl()
        {
            ViewBag.Code = Request["errCode"];
            try
            {
                //接收数据
                string billno = Request["billno"];
                string amount = Request["amount"];
                string currency_type = Request["Currency_type"];
                string mydate = Request["date"];
                string succ = Request["succ"];
                string msg = Request["msg"];
                string attach = Request["attach"];
                string ipsbillno = Request["ipsbillno"];
                string retEncodeType = Request["retencodetype"];
                string signature = Request["signature"];
                string bankbillno = Request["bankbillno"];

                //签名原文
                //billno+【订单编号】+currencytype+【币种】+amount+【订单金额】+date+【订单日期】+succ+【成功标志】+ipsbillno+【IPS订单编号】+retencodetype +【交易返回签名方式】
                string content = "billno" + billno + "currencytype" + currency_type + "amount" + amount + "date" + mydate + "succ" + succ + "ipsbillno" + ipsbillno + "retencodetype" + retEncodeType;

                //签名是否正确
                Boolean verify = false;

                if (retEncodeType == "17")
                {
                    //登陆http://merchant.ips.com.cn/商户后台下载的商户证书内容
                    string merchant_key = base.IPS_Key;
                    //Md5摘要
                    string signature1 = FormsAuthentication.HashPasswordForStoringInConfigFile(content + merchant_key, "MD5").ToLower();
                    if (signature1 == signature)
                    {
                        verify = true;
                    }

                }

                //判断签名验证是否通过
                if (verify == true)
                {
                    var status = FillMoneyStatus.Failed;
                    //判断交易是否成功
                    if (succ != "Y")
                    {
                        status = FillMoneyStatus.Failed;
                    }
                    else
                    {
                        status = FillMoneyStatus.Success;
                    }
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(billno, status, decimal.Parse(amount), succ, "", ipsbillno, UserToken);
                    if (result.IsSuccess && status == FillMoneyStatus.Success)
                    {
                        DelCacheUserBalance();
                    }
                    ViewBag.ErrorMsg = status == FillMoneyStatus.Success ? "交易成功" : "交易失败";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
            }
            return View();
        }

        public ContentResult IPSNotifyUrl()
        {
            try
            {
                //接收数据
                string billno = Request["billno"];
                string amount = Request["amount"];
                string currency_type = Request["Currency_type"];
                string mydate = Request["date"];
                string succ = Request["succ"];
                string msg = Request["msg"];
                string attach = Request["attach"];
                string ipsbillno = Request["ipsbillno"];
                string retEncodeType = Request["retencodetype"];
                string signature = Request["signature"];
                string bankbillno = Request["bankbillno"];

                //签名原文
                //billno+【订单编号】+currencytype+【币种】+amount+【订单金额】+date+【订单日期】+succ+【成功标志】+ipsbillno+【IPS订单编号】+retencodetype +【交易返回签名方式】
                string content = "billno" + billno + "currencytype" + currency_type + "amount" + amount + "date" + mydate + "succ" + succ + "ipsbillno" + ipsbillno + "retencodetype" + retEncodeType;

                //签名是否正确
                Boolean verify = false;

                if (retEncodeType == "17")
                {
                    //登陆http://merchant.ips.com.cn/商户后台下载的商户证书内容
                    string merchant_key = base.IPS_Key;
                    //Md5摘要
                    string signature1 = FormsAuthentication.HashPasswordForStoringInConfigFile(content + merchant_key, "MD5").ToLower();
                    if (signature1 == signature)
                    {
                        verify = true;
                    }

                }

                //判断签名验证是否通过
                if (verify == true)
                {
                    var status = FillMoneyStatus.Failed;
                    //判断交易是否成功
                    if (succ != "Y")
                    {
                        status = FillMoneyStatus.Failed;
                    }
                    else
                    {
                        status = FillMoneyStatus.Success;
                    }
                    var result = WCFClients.GameFundClient.CompleteFillMoneyOrder(billno, status, decimal.Parse(amount), succ, "", ipsbillno, UserToken);
                    if (result.IsSuccess && status == FillMoneyStatus.Success)
                    {
                        DelCacheUserBalance();
                    }
                    return Content(status == FillMoneyStatus.Success ? "交易成功" : "交易失败");
                }

                return Content("签名不正确");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        #endregion

        private static ILogWriter log = Common.Log.LogWriterGetter.GetLogWriter();
        private void WriteLog(string txt)
        {
            log.Write("Member", DateTime.Now.ToString("yyyyMMddHHmmss"), LogType.Information, "Member", txt);
        }

        //[HttpPost]
        public ContentResult BBPayCallBack()
        {
            try
            {
                var logList = new List<string>();
                //logList.Add("Post");
                //foreach (string key in Request.Form.Keys)
                //{
                //    logList.Add(key + "-------" + Request.Form[key]);
                //}
                //this.WriteLog(string.Join(Environment.NewLine, logList.ToArray()));

                var logList2 = new List<string>();
                logList2.Add("GET");
                foreach (string key in Request.QueryString.Keys)
                {
                    logList2.Add(key + "-------" + Request.QueryString[key]);
                }
                this.WriteLog(string.Join(Environment.NewLine, logList2.ToArray()));

                string p1_md = Request["p1_md"];//网银
                string p2_sn = Request["p2_sn"];//银宝系统生成的订单号
                string p3_xn = Request["p3_xn"];//商户订单号
                string p4_amt = Request["p4_amt"];//支付金额(两位小数)
                string p5_ex = Request["p5_ex"];//商户自定义信息，原样返回
                string p6_pd = Request["p6_pd"];//支付方式
                string p7_st = Request["p7_st"];//成功=success,失败=faile
                string p8_reply = Request["p8_reply"];//1=通知,2=显示
                string sign = Request["sign"];//由以上字段加上商户KEY生成的MD5签名
                var bbPayKey = WCFClients.GameClient.QueryCoreConfigByKey("BiFuBao.Key").ConfigValue;
                Common.Gateway.BiFuBao.BBPayCallBackInfo info = new Common.Gateway.BiFuBao.BBPayCallBackInfo();
                info.p1_md = Convert.ToInt32(p1_md);
                info.p2_sn = p2_sn;
                info.p3_xn = p3_xn;
                info.p4_amt = Convert.ToDecimal(p4_amt);
                info.p5_ex = p5_ex;
                info.p6_pd = Convert.ToInt32(p6_pd);
                info.p7_st = p7_st;
                info.p8_reply = Convert.ToInt32(p8_reply);
                info.BBPayKey = bbPayKey;
                info.sign = sign;
                Common.Gateway.BiFuBao.BBPayAPI api = new Common.Gateway.BiFuBao.BBPayAPI();
                var paramsSign = api.GetBBPayCallBackSign(info);
                if (paramsSign.ToUpper() == sign.ToUpper())
                {
                    WCFClients.GameFundClient.CompleteFillMoneyOrder(info.p3_xn, FillMoneyStatus.Success, info.p4_amt, info.p7_st, info.p5_ex, info.p2_sn, UserToken);
                    if (info.p8_reply == 1)
                    {
                        if (Convert.ToDecimal(p4_amt) >= 1000M)
                        {
                            try
                            {
                                var _mobile_financial = WCFClients.GameClient.QueryCoreConfigByKey("Site.Financial.Mobile").ConfigValue;
                                foreach (var item in _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    WCFClients.GameClient.SendMsg(item, string.Format("财务人员请注意：充值订单{0}已成功充值{1:N}元，请注意出票平台账户余额。", p3_xn, p4_amt), IpManager.IPAddress, 3, string.Empty, info.p3_xn);
                                }
                            }
                            catch
                            {
                            }
                        }
                        return Content("success");
                    }
                }
                return Content("fail");
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                return Content("fail");
            }

        }
        /// <summary>
        /// 币币支付
        /// </summary>
        /// <returns></returns>
        public ContentResult NewBBPayCallBack()
        {
            try
            {
                ILogWriter logWriter = LogWriterGetter.GetLogWriter();
                var data = Request["data"];
                var encryptkey = Request["encryptkey"];
                var bbPayKey = WCFClients.GameClient.QueryCoreConfigByKey("NewBiFuBao.Key").ConfigValue;

                logWriter.Write("NewBBPayCallBack", "data", LogType.Information, "data - " + encryptkey, data);

                Common.Gateway.BBPay.BBPayAPI api = new Common.Gateway.BBPay.BBPayAPI();
                var info = api.GetBBPayCallBackSign(data, encryptkey, bbPayKey);
                logWriter.Write(info.order, "bbpaycallback", LogType.Warning, "验证签名", "MD5结果:sign=" + info.sign + ",金额:money=" + info.amount + ",币币订单:bborderid:" + info.bborderid + ",status=" + info.status + ",返回详细参数=" + data.ToString());
                if (info != null && info.status == 1)
                {
                    try
                    {
                        WCFClients.GameFundClient.CompleteFillMoneyOrder(info.order, FillMoneyStatus.Success, info.amount, info.status.ToString(), info.merrmk, info.bborderid, UserToken);
                    }
                    catch (Exception ex)
                    {
                        logWriter.Write(info.order, "bbpaycallback", ex);
                    }
                    if (Convert.ToDecimal(info.amount) >= 1000M)
                    {
                        try
                        {
                            var _mobile_financial = WCFClients.GameClient.QueryCoreConfigByKey("Site.Financial.Mobile").ConfigValue;
                            foreach (var item in _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                WCFClients.GameClient.SendMsg(item,
                                    string.Format("财务人员请注意：充值订单{0}已成功充值{1:N}元，请注意出票平台账户余额。", info.order,
                                        info.amount),
                                    IpManager.IPAddress, 3, string.Empty, info.order);
                            }
                        }
                        catch (Exception ex)
                        {
                            //logWriter.Write(info.order, "bbpaycallback", ex);
                        }
                    }
                    return Content("success");
                }
                return Content("");
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                return Content("");
            }
        }
        /// <summary>
        /// 币币支付返回前台页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BBPayReturnurl()
        {

            try
            {

                ILogWriter logWriter = LogWriterGetter.GetLogWriter();
                var data = Request["data"];
                var encryptkey = Request["encryptkey"];
                var bbPayKey = WCFClients.GameClient.QueryCoreConfigByKey("NewBiFuBao.Key").ConfigValue;

                logWriter.Write("NewBBPayCallBack", "data", LogType.Information, "data - " + encryptkey, data);

                Common.Gateway.BBPay.BBPayAPI api = new Common.Gateway.BBPay.BBPayAPI();
                var info = api.GetBBPayCallBackSign(data, encryptkey, bbPayKey);
                ViewBag.OrderId = info.order;
                logWriter.Write(info.order, "bbpaycallback", LogType.Warning, "验证签名", "MD5结果:sign=" + info.sign + ",金额:money=" + info.amount + ",币币订单:bborderid:" + info.bborderid + ",status=" + info.status + ",返回详细参数=" + data.ToString());

                if (info != null && info.status == 1)
                {
                    try
                    {
                        WCFClients.GameFundClient.CompleteFillMoneyOrder(info.order, FillMoneyStatus.Success, info.amount,
                            info.status.ToString(), info.merrmk, info.bborderid, UserToken);
                    }
                    catch (Exception)
                    {
                    }
                    if (Convert.ToDecimal(info.amount) >= 1000M)
                    {
                        try
                        {
                            var _mobile_financial =
                                WCFClients.GameClient.QueryCoreConfigByKey("Site.Financial.Mobile").ConfigValue;
                            foreach (
                                var item in
                                    _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                WCFClients.GameClient.SendMsg(item,
                                    string.Format("财务人员请注意：充值订单{0}已成功充值{1:N}元，请注意出票平台账户余额。", info.order,
                                        info.amount),
                                    IpManager.IPAddress, 3, string.Empty, info.order);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    ViewBag.Message = info.amount;
                    ViewBag.IsSuccess = true;

                }
                else
                {
                    ViewBag.Message = "验证失败";
                    ViewBag.IsSuccess = false;
                    logWriter.Write(info.order, "bbpaycallback", LogType.Error, "验证签名",
                        "MD5结果:sign=" + info.sign + ",金额:money=" + info.amount + ",币币订单:bborderid:" + info.bborderid +
                        ",status=" + info.status + ",返回详细参数=" + data.ToString());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.IsSuccess = false;
                ViewBag.OrderId = "不好意思，没有找到该订单";
            }
            return View();
        }
        #endregion

        #region 邮箱认证

        public ActionResult confirmemail()
        {
            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                Response.Redirect(SiteRoot);
            }
            else
            {
                string code = Request.QueryString["code"];
                try
                {
                    LoginInfo loginInfo = WCFClients.ExternalClient.ResponseAuthenticationEmail(code, SchemeSource.Web);
                    if (loginInfo.IsSuccess)
                    {
                        //CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                        ViewBag.CurrentUser = CurrentUser;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    ViewBag.CurrentUser = CurrentUser;
                }
            }
            return View();
        }

        #endregion

        #region 找回密码
        [HttpPost]
        public JsonResult FindPwd_Mobile()
        {
            try
            {
                string username = PreconditionAssert.IsNotEmptyString(Request.Form["userName"], "找回密码的用户名不能为空。");
                string mobile = PreconditionAssert.IsNotEmptyString(Request.Form["mobile"], "手机号码不能为空。");
                string uid = WCFClients.ExternalClient.GetUserIdByLoginName(username);
                var isAuthMobile = WCFClients.ExternalClient.CheckIsAuthenticatedUserMobile(uid, UserToken);
                PreconditionAssert.IsTrue(isAuthMobile, "用户未认证手机，无法使用手机找回密码。");

                var mobileinfo = WCFClients.ExternalClient.GetUserMobileInfo(uid, UserToken);
                PreconditionAssert.IsTrue(mobileinfo.Mobile == mobile, "认证手机不匹配，无法找回密码。");

                var result = WCFClients.ExternalClient.FindPassword(uid);
                string code = result.ReturnValue;

                #region 发送站内消息：手机短信或站内信

                if (!string.IsNullOrEmpty(code))
                {
                    var pwdArray = code.Split('|');
                    if (pwdArray.Length == 2)
                    {
                        var pList = new List<string>();
                        pList.Add(string.Format("{0}={1}", "[UserName]", username));
                        pList.Add(string.Format("{0}={1}", "[UserPassword]", pwdArray[0]));
                        pList.Add(string.Format("{0}={1}", "[UserPassword_2]", pwdArray[1]));
                        //发送短信
                        WCFClients.GameQueryClient.DoSendSiteMessage("", mobile, "ON_User_Find_Password", string.Join("|", pList.ToArray()));
                    }
                }
                #endregion


                return Json(new { IsSuccess = true, Message = mobile });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FindPwd_Email()
        {
            try
            {
                string username = PreconditionAssert.IsNotEmptyString(Request.Form["userName"], "找回密码的用户名不能为空。");
                string email = PreconditionAssert.IsNotEmptyString(Request.Form["email"], "认证邮箱地址不能为空。");
                string uid = WCFClients.ExternalClient.GetUserIdByLoginName(username);
                var isAuthEmail = WCFClients.ExternalClient.CheckIsAuthenticatedUserEmail(uid, UserToken);
                PreconditionAssert.IsTrue(isAuthEmail, "用户未认证邮箱，无法使用邮箱找回密码。");
                var emailinfo = WCFClients.ExternalClient.GetUserEmailInfo(uid, UserToken);
                PreconditionAssert.IsTrue(emailinfo.Email == email, "认证邮箱地址不匹配，无法找回密码。");

                var result = WCFClients.ExternalClient.FindPassword(uid);
                if (result.IsSuccess)
                {
                    string reciver = email;
                    string title = username + "，找回密码邮件";

                    ViewBag.UserName = ViewData["UserName"] = username;
                    ViewBag.NewPwd = ViewData["NewPwd"] = result.ReturnValue;

                    ViewResult v = View("MailFindPWD");
                    v.ExecuteResult(ControllerContext);
                    StringBuilder sb = new StringBuilder();
                    StringWriter textwriter = new StringWriter(sb);
                    HtmlTextWriter htmlwriter = new HtmlTextWriter(textwriter);
                    ViewContext viewContext = new ViewContext(ControllerContext, v.View, ViewData, TempData, htmlwriter);
                    v.View.Render(viewContext, htmlwriter);
                    string html = sb.ToString();

                    EmailSender sm = new EmailSender();
                    //{
                    //    Account = "97402576@qq.com",
                    //    Smtp = "smtp.qq.com",
                    //    DisplayName = "@(SiteString.getHZSiteName(Request))邮箱认证管理员",
                    //    Password = "iLikeMoney888"
                    //};
                    new Thread(() =>
                    {
                        try
                        {
                            sm.SendEmail(reciver, title, html);
                        }
                        catch (Exception ex)
                        {
                            LogWriterGetter.GetLogWriter().Write("Error", "FindPwd_Email", ex);
                        }
                    }).Start();
                }
                HttpContext.Response.Clear();
                return Json(result);
            }
            catch (Exception ex)
            {
                HttpContext.Response.Clear();
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        public JsonResult UserAuthInfo()
        {
            try
            {
                var userName = PreconditionAssert.IsNotEmptyString(Request.QueryString["uid"], "找回用户不能为空。");
                string uid = WCFClients.ExternalClient.GetUserIdByLoginName(userName);
                var email = WCFClients.ExternalClient.CheckIsAuthenticatedUserEmail(uid, UserToken);
                var mobile = WCFClients.ExternalClient.CheckIsAuthenticatedUserMobile(uid, UserToken);
                var realname = WCFClients.ExternalClient.CheckIsAuthenticatedUserRealName(uid, UserToken);

                return Json(new { code = true, msg = new { name = userName, email = email, mobile = mobile, realname = realname } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Forget()
        {
            return View();
        }

        #endregion

        #region 充值券充值相关

        //彩金券充值
        [UnionFilter]
        public ActionResult lotteryticket()
        {
            ViewBag.TotalBonusMoney = 0M;
            ViewBag.TotalUser = 0;
            try
            {
                ViewBag.TotalUser = WCFClients.ExternalClient.QueryTotalBonusUserCount(UserToken);
                ViewBag.TotalBonusMoney = WCFClients.ExternalClient.QueryTotalBonusMoney(UserToken);
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        //彩金券充值
        public JsonResult uselotteryticket()
        {
            try
            {
                var ticket = PreconditionAssert.IsNotEmptyString(Request["ticket"], "请输入彩金券编码。");
                var verifycode = PreconditionAssert.IsNotEmptyString(Request["verifycode"], "请输入验证码。");
                PreconditionAssert.IsTrue(VerifyCode(verifycode), "验证码错误");
                var result = WCFClients.ActivityClient.ExchangeCoupons(ticket, UserToken);
                return Json(new { state = result.IsSuccess ? 1 : -1, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { state = -1, msg = ex.Message });
            }
        }

        //充值券充值页面
        [UnionFilter]
        public ActionResult CouponPay()
        {
            if (!string.IsNullOrEmpty(Request["pid"]))
            {
                string pid = Request["pid"];
                int adid = string.IsNullOrEmpty(Request["adid"]) ? 0 : int.Parse(Request["adid"]);
                //RecordVisite();
            }

            CurrentUser = null;
            ViewBag.TotalBonusMoney = 0M;
            ViewBag.TotalUser = 0;
            try
            {
                ViewBag.TotalUser = WCFClients.ExternalClient.QueryTotalBonusUserCount(UserToken);
                ViewBag.TotalBonusMoney = WCFClients.ExternalClient.QueryTotalBonusMoney(UserToken);
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        //手机号码是否已被认证,true为已认证，false为未认证
        [HttpPost]
        public JsonResult MobileIsAuth()
        {
            try
            {
                string mobile = PreconditionAssert.IsNotEmptyString(Request.Form["mobile"], "手机号码不能为空。");
                return Json(WCFClients.ExternalClient.CheckIsMobileAuthenticated(mobile, UserToken));
            }
            catch
            {
                return Json(false);
            }
        }
        #endregion

        #region 支付宝快捷登录、物流信息查询

        #region 支付宝快捷登录(id为不同活动的url简写标识，用作支付宝登录后跳转)
        public ActionResult alipatloginfirst()
        {
            ViewBag.AlipayDic = Session["AlipayDic"];
            return View();
        }
        public ActionResult alipayloginsecond()
        {
            ViewBag.AlipayDic = Session["AlipayDic"];
            return View();
        }
        //支付宝登录参数跳转页
        public ActionResult alipaylogin()
        {
            Common.Gateway.Alipay.Login.Submit loginSubmit = new Common.Gateway.Alipay.Login.Submit(ali_Partner, ali_key);

            string ali_return_url = "/user/alipayback";
            Response.Redirect(loginSubmit.BuildRequestString(SiteRoot + ali_return_url));
            return View();
        }

        //支付宝快捷登录回调页面
        public ActionResult alipayback()
        {
            try
            {
                SortedDictionary<string, string> sPara = GetRequestGet();

                if (sPara.Count > 0)//判断是否有带返回参数
                {
                    Notify aliNotify = new Notify(ali_Partner, ali_key);
                    bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                    if (verifyResult)//验证成功
                    {
                        string user_id = Request["user_id"];//支付宝用户id

                        //  需要重新做支付宝共享信息接口
                        var isExist = WCFClients.ExternalClient.TryLoginAlipay(user_id, IpManager.IPAddress);

                        //如果账户已存在则直接登录并跳转到首页
                        if (isExist != null)
                        {
                            //  LoginInfo loginInfo = WCFClients.ExternalClient.LoginByAlipay(user_id);
                            if (isExist.IsSuccess)
                            {
                                //CurrentUser = new CurrentUserInfo() { LoginInfo = isExist };
                                //重新加载用户等级
                                LoadUserLeve();
                            }
                            Response.Redirect(SiteRoot);
                        }
                        else
                        {
                            //ViewBag.AlipayDic = sPara;
                            Session["AlipayDic"] = sPara;
                        }

                        ViewBag.IsSuccess = true;

                    }
                    else//验证失败
                    {
                        throw new Exception("验证失败");
                        //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));
                    }
                }
                else
                {
                    throw new Exception("参数错误");
                    //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        //支付宝账号绑定
        public ActionResult alipaybind()
        {
            try
            {
                //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                string token = Request["token"];	//授权令牌
                string real_name = Request["real_name"]; //用户姓名
                string email = Request["email"]; //用户支付宝账号
                string user_grade = Request["user_grade"]; //用户等级
                string user_grade_type = Request["user_grade_type"]; //用户等级类型
                Session["AlipayToken"] = token;//记录支付宝授权令牌

                var user_id = PreconditionAssert.IsNotEmptyString(Request["user_id"], "支付宝用户绑定ID不能为空"); //支付宝用户id
                var userName = PreconditionAssert.IsNotEmptyString(Request["alipayLoginName"], "注册用户名不能为空");
                var pwd = PreconditionAssert.IsNotEmptyString(Request["LoginPwd"], "账户密码不能为空");

                #region ------ 读取用户注册访问参数 -----
                string referrer = "";
                string userId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
                string userComeFrom = "";
                string agentId = "";

                //从Cookie里读取推广访问参数
                if (Request.Cookies["RegParamCookie"] != null)
                {
                    userComeFrom = Request.Cookies["RegParamCookie"]["UserComeFrom"];
                    userId = Request.Cookies["RegParamCookie"]["UserId"];
                    referrer = Request.Cookies["RegParamCookie"]["Referrer"];
                    agentId = Request.Cookies["RegParamCookie"]["AgentId"];
                }
                #endregion

                //注册或更新本地系统账户
                RegisterInfo_Local regInfo = new RegisterInfo_Local()
                {

                    // OpenId = user_id,
                    RegType = "ALIPAY",
                    LoginName = userName,
                    Password = pwd,
                    RegisterIp = IpManager.IPAddress,
                    ReferrerUrl = Session["RefferUrl"] == null ? "" : Session["RefferUrl"].ToString(),
                    Referrer = referrer,
                    AgentId = agentId,
                    CreateTime = DateTime.Now,
                    ComeFrom = "ALIPAY"
                };

                //如果用户来源不为空，则记录为注册类型
                if (!string.IsNullOrEmpty(userComeFrom))
                {
                    regInfo.RegType = userComeFrom;
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    var com = WCFClients.ExternalClient.BindAlipayExistUser(userId, user_id, IpManager.IPAddress);
                    if (com.IsSuccess)
                    {
                       // CurrentUser = new CurrentUserInfo() { LoginInfo = com };
                        LoadUserLeve();
                    }
                }
                else
                {
                    var com = WCFClients.ExternalClient.BindAlipayNewUser(regInfo, user_id);
                    if (com.IsSuccess)
                    {
                        LoginInfo logininfo = WCFClients.ExternalClient.LoginLocal(userName, pwd, IpManager.IPAddress);
                        if (logininfo.IsSuccess)
                        {
                           // CurrentUser = new CurrentUserInfo() { LoginInfo = logininfo };
                            LoadUserLeve();
                        }
                    }
                }

                ViewBag.CurrentUser = CurrentUser;
                ViewBag.IsSuccess = true;

                #region 实名认证
                //var real = Request["RealName"];
                //var idnum = Request["idnum"];
                //if (!string.IsNullOrEmpty(real))
                //{
                //    PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(real), "真实姓名不允许包含敏感词，如有疑问请联系客服。");

                //    PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(idnum), "请输入正确的身份证号码。");
                //    UserRealNameInfo realInfo = new UserRealNameInfo() { IdCardNumber = idnum, RealName = real };
                //    var result = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                //    DelCacheRealNameInfo();
                //}
                #endregion

                #region 设置资金密码
                //var paypassword = Request["paypassword"];
                //if (!string.IsNullOrEmpty(paypassword))
                //{
                //    PreconditionAssert.IsTrue(paypassword != pwd, "资金密码不能与登录密码相同");
                //    var result = WCFClients.GameFundClient.SetBalancePassword(paypassword, true, paypassword, UserToken);
                //}
                #endregion

                //ViewBag.Url = SiteRoot + UrlMapping.QueryMappingUrl("alipay", id);
                //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));



                #region 原支付宝更新信息
                //if (!bool.Parse(regres.ReturnValue))
                //{
                //    //如果是首次注册，则跳转到支付宝共享信息页面调用信息
                //    CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };

                //    //把请求参数打包成数组(用于物流地址信息查询)
                //    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                //    sParaTemp.Add("token", token);

                //    //构造快捷登录用户物流地址查询接口表单提交HTML数据，无需修改
                //    string ali_return_url = "/user/logisticscallback/" + id;

                //    Common.Gateway.Alipay.Logistics.Service ali = new Common.Gateway.Alipay.Logistics.Service(ali_Partner, ali_key);
                //    string sHtmlText = ali.User_logistics_address_query(sParaTemp, SiteRoot + ali_return_url);
                //    Response.Write(sHtmlText);
                //}
                //else
                //{

                //}

                //执行商户的业务程序

                ////etao专用,本网站暂时用不到
                //if (Request.QueryString["target_url"] != "")
                //{
                //    //程序自动跳转到target_url参数指定的url去
                //}

                //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        //支付宝快捷登录后，再次调用快捷物流回调页面
        //[ValidateInput(false)]
        //public ActionResult logisticscallback(string id)
        //{
        //    SortedDictionary<string, string> sPara = GetRequestPost();

        //    if (sPara.Count > 0)//判断是否有带返回参数
        //    {
        //        Common.Gateway.Alipay.Logistics.Notify aliNotify = new Common.Gateway.Alipay.Logistics.Notify(ali_Partner, ali_key);
        //        bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

        //        if (verifyResult)//验证成功
        //        {
        //            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //            //请在这里加上商户的业务逻辑程序代码

        //            //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
        //            //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
        //            //支付宝用户id
        //            string user_id = Request.Form["user_id"]; //授权用户的id。支付宝账号对应的支付宝唯一用户号。以2088开头的16位纯数字组成。
        //            string user_type = Request.Form["user_type"]; //用户类型（1/2）。 1代表公司账户 2代表个人账户
        //            string user_status = Request.Form["user_status"]; //用户状态（Q/T/B/W）。 Q代表快速注册用户 T代表已认证用户 B代表被冻结账户 W代表已注册，未激活的账户
        //            string firm_name = Request.Form["firm_name"]; //公司名称（用户类型是公司类型时公司名称才有此字段）。
        //            string real_name = Request.Form["real_name"]; //用户的真实姓名。
        //            string email = Request.Form["email"]; //用户支付宝账号绑定的邮箱地址
        //            string cert_no = Request.Form["cert_no"]; //证件号码。
        //            string gender = Request.Form["gender"]; //性别（F：女性；M：男性）。
        //            string phone = Request.Form["phone"]; //电话号码。
        //            string mobile = Request.Form["mobile"]; //手机号码。
        //            string is_certified = Request.Form["is_certified"]; //是否通过实名认证。
        //            string is_bank_auth = Request.Form["is_bank_auth"]; //T为是银行卡认证，F为非银行卡认证。
        //            string is_id_auth = Request.Form["is_id_auth"]; //T为是身份证认证，F为非身份证认证。
        //            string is_mobile_auth = Request.Form["is_mobile_auth"]; //T为是手机认证，F为非手机认证。
        //            string cert_type = Request.Form["cert_type"]; //证件类型，请参见“11.3 证件类型列表”。

        //            #region 用户收货地址
        //            //用户选择的收货地址
        //            string receive_address = Request.Form["receive_address"];

        //            string address = "", receivename = "", receivemobile = "";
        //            if (!string.IsNullOrEmpty(receive_address))
        //            {
        //                //对receive_address做XML解析，获得各节点信息
        //                XmlDocument xmlDoc = new XmlDocument();
        //                xmlDoc.LoadXml(receive_address);

        //                //获取地址
        //                if (xmlDoc.SelectSingleNode("/receiveAddress/address") != null)
        //                {
        //                    address = xmlDoc.SelectSingleNode("/receiveAddress/address").InnerText;
        //                }
        //                //获取收货人名称
        //                if (xmlDoc.SelectSingleNode("/receiveAddress/fullname") != null)
        //                {
        //                    receivename = xmlDoc.SelectSingleNode("/receiveAddress/fullname").InnerText;
        //                }
        //                //获取收货人手机号码
        //                if (xmlDoc.SelectSingleNode("/receiveAddress/mobile_phone") != null)
        //                {
        //                    receivemobile = xmlDoc.SelectSingleNode("/receiveAddress/mobile_phone").InnerText;
        //                }
        //            }
        //            #endregion

        //            try
        //            {
        //                //更新本地系统账户
        //                RegisterInfo_Alipay alipayRegInfo = new RegisterInfo_Alipay()
        //                {
        //                    LoginName = user_id,
        //                    CardNumber = cert_no,
        //                    CardType = cert_type,
        //                    Email = email,
        //                    IsAuthMobile = is_mobile_auth == "T" ? true : false,
        //                    IsAuthRealName = is_id_auth == "T" ? true : false,
        //                    RealName = real_name,
        //                    Mobile = mobile,
        //                    UserStatus = user_status
        //                };

        //                LoginInfo loginInfo = WCFClients.ExternalClient(alipayRegInfo, SchemeSource.Web, true);
        //                if (loginInfo.IsSuccess)
        //                {
        //                    CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
        //                }

        //            }
        //            catch
        //            {
        //            }
        //            //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));
        //            //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

        //            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //        }
        //        else//验证失败
        //        {
        //            //Response.Write("验证失败");
        //            //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));
        //        }
        //    }
        //    else
        //    {
        //        //Response.Write("无返回参数");
        //        //Response.Redirect(SiteRoot + UrlMapping.QueryMappingUrl("alipay", id));
        //    }
        //    return View();
        //}

        #endregion

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        #endregion

        #region QQ登录 API
        // 绑定已注册的用户名
        public ActionResult LoginFirst()
        {
            ViewBag.QQInfoDic = Session["userInfo"];
            return View();
        }
        //绑定新注册的用户名
        public ActionResult LoginSecond()
        {
            ViewBag.QQInfoDic = Session["userInfo"];
            return View();
        }
        // 绑定已注册的用户名
        public JsonResult CheckuaernameAndIdcard()
        {
            var backurl = string.IsNullOrEmpty(Request["backurl"]) ? SiteRoot : Request["backurl"];

            try
            {
                var loginName = PreconditionAssert.IsNotEmptyString(Request["LoginName"], "请输入用户名");
                var loginPwd = PreconditionAssert.IsNotEmptyString(Request["LoginPwd"], "请输入密码");
                var MobileAndIdCard = WCFClients.ExternalClient.QueryUserMobileAndIdCard(loginName, loginPwd);
                return Json(new { IsSucess = true, Msg = MobileAndIdCard, LoginName = loginName, Loginpwd = loginPwd });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });
            }

        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        //public JsonResult SendCode()
        //{
        //    try
        //    {
        //        var mobile = PreconditionAssert.IsNotEmptyString(Request["mobile"], "请输入手机号码");
        //        var code = WCFClients.ExternalClient.SendValidateCodeToUserMobile(mobile);
        //        var userName = Request["userName"];

        //        #region 发送站内消息：手机短信或站内信

        //        var pList = new List<string>();
        //        pList.Add(string.Format("{0}={1}", "[ValidNumber]", code));
        //        //发送短信
        //        WCFClients.GameQueryClient.DoSendSiteMessage("", mobile, "ON_User_Bind_Mobile_Before", string.Join("|", pList.ToArray()));

        //        #endregion

        //        if (code.IsSuccess)
        //        {
        //            Session["alipay-phone-code"] = code.ReturnValue;
        //            //return Json(new { Isscucess = true, Code = code.ReturnValue });
        //            return Json(new { Isscucess = true });
        //        }
        //        else
        //        {
        //            Session["alipay-phone-code"] = null;
        //            return Json(new { Isscucess = false, errMsg = "验证码错误" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Isscucess = false, Code = "", errMsg = ex.Message });
        //    }
        //}

        public ActionResult qqlogin()
        {
            //Request.Url.Authority
            var state = Guid.NewGuid().ToString().Replace("-", "");
            Session["state"] = state;
            var url = Request.Url.Authority;
            var callBack = string.Format("http://{0}{1}", url, qqlogin_CallBack);
            string login_url = "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id=" + qqlogin_ConsumerKey + "&state=" + state + "&redirect_uri=" + HttpUtility.UrlEncode(callBack) + "&scope=get_user_info,get_info,get_other_info";
            CurrentUser = null;//跳转到登录页面时，把当前用户Session置空
            Response.Redirect(login_url);
            return View();
        }

        public ActionResult qqback()
        {
            try
            {
                //临时变量，发送URL,接受返回结果
                string send_url = "", rezult = "";
                //用于第三方应用防止CSRF攻击，成功授权后回调时会原样带回。
                string state = "";
                //临时Authorization Code，官方提示10分钟过期
                string code = "";
                //通过Authorization Code返回结果获取到的Access Token
                string access_token = "";
                //expires_in是该Access Token的有效期，单位为秒
                string expires_in = "";
                //通过Access Token返回来的client_id 
                string new_client_id = "";
                //通过Access Token返回来的openid，QQ用户唯一值，可以与网站用户数据关联
                string openid = "";

                //取得state
                state = Request["state"];

                #region 判断是否初始化
                if (Session["state"] == null || Session["state"].ToString() == "")
                {
                    throw new Exception("state未初始化");
                }
                #endregion

                //如果返回state与之前发出的判断正确
                PreconditionAssert.IsTrue(state == Session["state"].ToString(), "state状态不匹配 - 返回结果state：" + state);

                //取得返回CODE
                code = Request["code"];
                var url = Request.Url.Authority;
                ////==============通过Authorization Code和基本资料获取Access Token=================
                var callBack = string.Format("http://{0}{1}", url, qqlogin_CallBack);
                send_url = "https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id=" + qqlogin_ConsumerKey + "&client_secret=" + qqlogin_ConsumerSecret + "&code=" + code + "&state=" + state + "&redirect_uri=" + HttpUtility.UrlEncode(callBack);

                ////发送并接受返回值
                rezult = PostManager.Get(send_url, Encoding.UTF8);

                //如果失败
                if (rezult.Contains("error"))
                {
                    //出错了
                    throw new Exception("出错了：" + rezult);
                }
                else
                {

                    //======================通过Access Token来获取用户的OpenID==============

                    string[] parm = rezult.Split('&');

                    //取得 access_token
                    access_token = parm[0].Split('=')[1];
                    //取得 过期时间
                    expires_in = parm[1].Split('=')[1];
                    //拼接url
                    send_url = "https://graph.qq.com/oauth2.0/me?access_token=" + access_token;

                    //发送并接受返回值
                    rezult = PostManager.Get(send_url, Encoding.UTF8);

                    //写日志
                    //Cs.Logs.logSave("第三步，发送 access_token：" + send_url);
                    //如果失败
                    if (rezult.Contains("error"))
                    {
                        //出错了
                        throw new Exception("出错了：" + rezult);
                    }

                    //写日志
                    //Cs.Logs.logSave("得到返回结果：" + rezult);

                    //取得文字出现
                    int str_start = rezult.IndexOf('(') + 1;
                    int str_last = rezult.LastIndexOf(')') - 1;

                    //取得JSON字符串
                    rezult = rezult.Substring(str_start, (str_last - str_start));

                    //反序列化JSON
                    Dictionary<string, string> _dic = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(rezult);
                    //Dictionary<string, string> _dic = JsonSerializer.Deserialize<Dictionary<string, string>>(rezult);


                    //取值
                    _dic.TryGetValue("client_id", out new_client_id);
                    _dic.TryGetValue("openid", out openid);

                    //储存获取数据用到的信息
                    Session["access_token"] = access_token;
                    Session["client_id"] = qqlogin_ConsumerKey;
                    Session["openid"] = openid;

                    //========继续您的业务逻辑编程==========================================

                    //取到 openId
                    //openId与您系统的user数据进行关联
                    //一个openid对应一个QQ，一个openid也要对应到您系统的一个账号：QQ--OpenId--User；
                    //这个时候有两种情况：
                    //【1】您让用户绑定系统已有的用户，那么让用户输入用户名密码，找到该用户，然后绑定OpenId
                    //【2】为用户生成一个系统用户，直接绑定OpenId

                    //上面完成之后，设置用户的登录状态，完整绑定和登录

                    //=============比如：通过Access Token和OpenID来使用get_user_info方法获取用户资料====
                    send_url = "https://graph.qq.com/user/get_user_info?access_token=" + access_token
                        + "&oauth_consumer_key=" + qqlogin_ConsumerKey + "&openid=" + openid;

                    //发送并接受返回值
                    rezult = PostManager.Get(send_url, Encoding.UTF8);
                    //写日志
                    //Cs.Logs.logSave("第四步，通过get_user_info方法获取数据：" + send_url);


                    //反序列化JSON
                    Dictionary<string, string> _dic2 = System.Web.Helpers.Json.Decode<Dictionary<string, string>>(rezult);
                    //Dictionary<string, string> _dic2 = JsonSerializer.Deserialize<Dictionary<string, string>>(rezult);
                    _dic2.Add("openid", openid);

                    string ret = "", msg = "", nickname = "", face = "", face1 = "",
                        face2 = "", sex = "", vip_level = "", qzone_level = "";

                    //取值
                    _dic2.TryGetValue("ret", out ret);
                    _dic2.TryGetValue("msg", out msg);

                    //如果失败
                    if (ret != "0")
                    {
                        //出错了
                        throw new Exception("出错了：" + rezult);
                    }

                    _dic2.TryGetValue("nickname", out nickname);
                    _dic2.TryGetValue("figureurl", out face);
                    _dic2.TryGetValue("figureurl_1", out face1);
                    _dic2.TryGetValue("figureurl_2", out face2);
                    _dic2.TryGetValue("gender", out sex);
                    _dic2.TryGetValue("vip", out vip_level);
                    _dic2.TryGetValue("level", out qzone_level);

                    //写日志
                    //Cs.Logs.logSave("得到返回结果" + rezult);

                    string newline = "<br>";
                    string str = "";
                    str += "openid：" + openid + newline;
                    str += "昵称：" + nickname + newline;
                    str += "性别：" + sex + newline;
                    str += "会员VIP等级：" + vip_level + newline;
                    str += "空间黄钻等级：" + qzone_level + newline;
                    str += "默认头像：" + face + newline;
                    str += "头像1：" + face1 + newline;
                    str += "头像2：" + face2 + newline;

                    var isExist = WCFClients.ExternalClient.TryLoginQQ(openid, IpManager.IPAddress);

                    //如果账户已存在则直接登录并跳转到首页
                    if (isExist != null)
                    {
                        //注册或更新本地系统账户
                        RegisterInfo_QQ qqRegInfo = new RegisterInfo_QQ()
                        {
                            OpenId = openid,
                        };
                        // LoginInfo loginInfo = WCFClients.ExternalClient.
                        if (isExist.IsSuccess)
                        {
                            //CurrentUser = new CurrentUserInfo() { LoginInfo = isExist };
                            //重新加载vip等级
                            LoadUserLeve();
                        }
                        Response.Redirect(SiteRoot);
                    }
                    else //否则直接注册，用以通过QQ登录审核
                    {
                        // ViewBag.QQInfoDic = _dic2;
                        Session["userInfo"] = _dic2;
                        //#region 注册

                        //#region ------ 读取用户注册访问参数 -----
                        //string referrer = "";
                        //string userId = "";
                        //string userComeFrom = "";
                        //string agentId = "";

                        ////从Session里读取推广访问参数
                        //if (Session["RegParam"] != null)
                        //{
                        //    Dictionary<string, string> RegParam = Session["RegParam"] as Dictionary<string, string>;
                        //    userComeFrom = RegParam["UserComeFrom"];
                        //    userId = RegParam["UserId"];
                        //    referrer = RegParam["Referrer"];
                        //    agentId = RegParam["AgentId"];
                        //}

                        ////从Cookie里读取推广访问参数
                        //if (Request.Cookies["RegParamCookie"] != null)
                        //{
                        //    userComeFrom = Request.Cookies["RegParamCookie"]["UserComeFrom"];
                        //    userId = Request.Cookies["RegParamCookie"]["UserId"];
                        //    referrer = Request.Cookies["RegParamCookie"]["Referrer"];
                        //    agentId = Request.Cookies["RegParamCookie"]["AgentId"];
                        //}


                        //#endregion

                        ////注册或更新本地系统账户
                        //RegisterInfo_QQ qqRegInfo = new RegisterInfo_QQ()
                        //{
                        //    RegType = "QQ",
                        //    RegisterIp = IpManager.IPAddress,
                        //    AgentId = agentId,
                        //    DisplayName = nickname,
                        //    OpenId = openid,
                        //    Referrer = referrer,
                        //    ReferrerUrl = Session["RefferUrl"] == null ? "" : Session["RefferUrl"].ToString(),
                        //    Password = "",
                        //    LoginName = openid,
                        //};
                        ////如果用户来源不为空，则记录为注册类型
                        //if (!string.IsNullOrEmpty(userComeFrom))
                        //{
                        //    qqRegInfo.RegType = userComeFrom;
                        //}

                        //LoginInfo loginInfo = WCFClients.ExternalClient.LoginQQ(qqRegInfo);
                        //if (loginInfo.IsSuccess)
                        //{
                        //    CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo, Figureurl = face, Figureurl_1 = face1, Figureurl_2 = face2, Sex = sex, QQVipLevel = vip_level };

                        //    Response.Redirect("/user/register2");
                        //}

                        //#endregion
                    }

                    ViewBag.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        //QQ登录绑定
        public ActionResult qqbind()
        {
            try
            {

                var face = Request["figureurl"];
                var face1 = Request["figureurl_1"];
                var face2 = Request["figureurl_2"];
                var sex = Request["gender"];
                var vip_level = Request["vip"];
                var openid = PreconditionAssert.IsNotEmptyString(Request["openid"], "QQ登录绑定ID不能为空");
                var nickname = Request["nickname"];
                //var userName = PreconditionAssert.IsNotEmptyString(Request["UserName"], "注册用户名不能为空");
                var userName = Request["LoginName"];
                //var pwd = PreconditionAssert.IsNotEmptyString(Request["password"], "账户密码不能为空");
                var pwd = Request["LoginPwd"];
                #region ------ 读取用户注册访问参数 -----
                string referrer = "";
                string userId = string.IsNullOrEmpty(Request["userId"]) ? "" : Request["userId"];
                string userComeFrom = "";
                string agentId = "";
                // 注册或更新本地系统账户

                RegisterInfo_Local info = new RegisterInfo_Local()
                {
                    RegType = "QQ",
                    RegisterIp = IpManager.IPAddress,
                    AgentId = agentId,
                    Referrer = referrer,
                    ReferrerUrl = Session["RefferUrl"] == null ? "" : Session["RefferUrl"].ToString(),
                    Password = pwd,
                    LoginName = userName,
                    CreateTime = DateTime.Now,
                    ComeFrom = "QQ"

                };
                ////如果用户来源不为空，则记录为注册类型
                if (!string.IsNullOrEmpty(userComeFrom))
                {
                    info.RegType = userComeFrom;
                }
                //绑定现有用户
                if (!string.IsNullOrEmpty(userId))
                {

                    var loginInfo = WCFClients.ExternalClient.BindQQExistUser(userId, openid, IpManager.IPAddress);
                    if (loginInfo.IsSuccess)
                    {
                       // CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo, QQVipLevel = vip_level };
                        LoadUserLeve();
                    }
                }
                else
                {
                    var comm = WCFClients.ExternalClient.BindQQNewUser(info, openid);
                    if (comm.IsSuccess)
                    {
                        LoginInfo loginInfo = WCFClients.ExternalClient.LoginLocal(userName, pwd, IpManager.IPAddress);
                        //CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo, QQVipLevel = vip_level };
                    }
                }

                ////从Session里读取推广访问参数
                //if (Session["RegParam"] != null)
                //{
                //    Dictionary<string, string> RegParam = Session["RegParam"] as Dictionary<string, string>;
                //    userComeFrom = RegParam["UserComeFrom"];
                //    userId = RegParam["UserId"];
                //    referrer = RegParam["Referrer"];
                //    agentId = RegParam["AgentId"];
                //}

                ////从Cookie里读取推广访问参数
                //if (Request.Cookies["RegParamCookie"] != null)
                //{
                //    userComeFrom = Request.Cookies["RegParamCookie"]["UserComeFrom"];
                //    userId = Request.Cookies["RegParamCookie"]["UserId"];
                //    referrer = Request.Cookies["RegParamCookie"]["Referrer"];
                //    agentId = Request.Cookies["RegParamCookie"]["AgentId"];
                //}


                #endregion

                //注册或更新本地系统账户
                //RegisterInfo_QQ qqRegInfo = new RegisterInfo_QQ()
                //{
                //    RegType = "QQ",
                //    RegisterIp = IpManager.IPAddress,
                //    AgentId = agentId,
                //    DisplayName = nickname,
                //    OpenId = openid,
                //    Referrer = referrer,
                //    ReferrerUrl = Session["RefferUrl"] == null ? "" : Session["RefferUrl"].ToString(),
                //    Password = pwd,
                //    LoginName = userName,
                //};
                ////如果用户来源不为空，则记录为注册类型
                //if (!string.IsNullOrEmpty(userComeFrom))
                //{
                //    qqRegInfo.RegType = userComeFrom;
                //}

                //LoginInfo loginInfo = WCFClients.ExternalClient.LoginQQ(qqRegInfo);
                //if (loginInfo.IsSuccess)
                //{
                //    CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo, QQVipLevel = vip_level };
                //}

                //#region 实名认证
                //var real = Request["RealName"];
                //var idnum = Request["idnum"];
                //if (!string.IsNullOrEmpty(real))
                //{
                //    PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(real), "真实姓名不允许包含敏感词，如有疑问请联系客服。");

                //    PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(idnum), "请输入正确的身份证号码。");
                //    UserRealNameInfo realInfo = new UserRealNameInfo() { IdCardNumber = idnum, RealName = real };
                //    var result = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                //    DelCacheRealNameInfo();
                //}
                //#endregion

                //#region 设置资金密码
                //var paypassword = Request["paypassword"];
                //if (!string.IsNullOrEmpty(paypassword))
                //{
                //    PreconditionAssert.IsTrue(paypassword != pwd, "资金密码不能与登录密码相同");
                //    var result = WCFClients.GameFundClient.SetBalancePassword(paypassword, true, paypassword, UserToken);
                //}
                //#endregion

                ViewBag.CurrentUser = CurrentUser;
                ViewBag.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = ex.Message;
            }

            return View();
        }
        #endregion

        #region 用户操作相关Ajax函数

        //private string bbsAddedUserLog = "bbsAddUser.txt";

        private string ReadFile(string fileFullName)
        {
            using (var sr = new StreamReader(fileFullName, Encoding.GetEncoding("gb2312")))
            {
                return sr.ReadToEnd();
            }
        }
        private void WriteFile(string fileFullName, string content)
        {
            StreamWriter sw = new StreamWriter(fileFullName, false, Encoding.GetEncoding("gb2312"));
            sw.Write(content);
            sw.Close();
        }

        private void AddToLogFile(string userName)
        {
            //if (string.IsNullOrEmpty(userName)) return;
            //var fileName = Path.Combine(Server.MapPath("~"), bbsAddedUserLog);
            //if (!System.IO.File.Exists(fileName))
            //{
            //    var list = new List<string>();
            //    list.Add(userName);
            //    WriteFile(fileName, JsonSerializer.Serialize(list));
            //}
            //else
            //{
            //    var oldList = JsonSerializer.Deserialize<List<string>>(ReadFile(fileName));
            //    if (!oldList.Contains(userName))
            //    {
            //        oldList.Add(userName);
            //        WriteFile(fileName, JsonSerializer.Serialize(oldList));
            //    }
            //}

        }
        private bool LocalLogContainsUserName(string userName)
        {
            return true;
            //if (string.IsNullOrEmpty(userName)) return false;
            //var fileName = Path.Combine(Server.MapPath("~"), bbsAddedUserLog);
            //if (!System.IO.File.Exists(fileName))
            //    return false;

            //var oldList = JsonSerializer.Deserialize<List<string>>(ReadFile(fileName));
            //return oldList.Contains(userName);
        }

        #region 注册的一笔
        /// <summary>
        /// 注册一个本地用户 注册第一步
        /// </summary>
        /// <param name="postForm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> LoacalUserRegisterAsync(FormCollection postForm)
        {
            try
            {
                
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["key"] = "BanRegistrFrequencyIPCount";

                var brfipCount = await serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");//限制IP注册次数时间分
                param.Clear();
                param["key"] = "BanRegistrFrequencyIPTime";
                var brfipTime = await serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");//限制分钟数
                param.Clear();
                param["key"] = "BanRegistrIP";
                var banRegistrIP = await serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");//禁止注册IP
                string localIP = IpManager.IPAddress;
                if (banRegistrIP.ConfigValue.Contains(localIP))
                {
                    return Json(new { IsSuccess = false, Message = "因检测到该IP地址异常，无法注册用户，请联系在线客服。" });
                }
                if (Convert.ToInt32(brfipCount.ConfigValue) > 0 && Convert.ToInt32(brfipTime) > 0)
                {
                    DateTime date = DateTime.Now.AddMinutes(-Convert.ToInt32(brfipTime));
                    param.Clear();
                    param["date"] = date;
                    param["localIP"] = localIP;
                    var count = await serviceProxyProvider.Invoke<int>(param, "api/user/GetTodayRegisterCount");//禁止注册IP
                    if (count > Convert.ToInt32(brfipCount))
                    {
                        return Json(new { IsSuccess = false, Message = string.Format("同一IP，在{0}分钟内只能注册{1}个账号", brfipTime.ConfigValue, brfipCount.ConfigValue) });
                    }
                }

                CommonActionResult result = null;

                if (Session["ValidateCode"] == null || Session["ValidateCode"].ToString() == "")
                {
                    return Json(new { Succuss = false, Msg = "验证码已过期！" }, JsonRequestBehavior.AllowGet);
                }

                string validateCode1 = PreconditionAssert.IsNotEmptyString(Request["verifyCode1"], "验证码不能为空！");
                string mobile = PreconditionAssert.IsNotEmptyString(Request["moblie"], "手机号码不能为空！");
                string validateCode = PreconditionAssert.IsNotEmptyString(Request["verifyCode"], "手机验证码不能为空！");
                var passWord = PreconditionAssert.IsNotEmptyString(Request["password"], "账户密码不能为空");
                var ipinfo = IpManager.GetIpDisplayname_Sina(localIP);
                string ipaddress = ipinfo == null ? "" : ipinfo.ToString();
                EntityModel.CoreModel.RegisterInfo_Local registerInfo = new EntityModel.CoreModel.RegisterInfo_Local()
                {
                    RegType = "LOCAL",
                    LoginName = mobile,
                    Password = passWord,
                    RegisterIp = string.Format("{0}({1})", localIP, ipaddress),
                    ComeFrom = "LOCAL",
                    ReferrerUrl = Session["RefferUrl"] == null ? "" : Session["RefferUrl"].ToString(),
                    Mobile = mobile
                };
                param.Clear();

                if (Session["pid"] != null)
                {
                    registerInfo.AgentId = Session["pid"].ToString();
                }
              
                if (Session["yqid"] != null)//yqid是普通会员推广
                {
                    string yqid = Session["yqid"].ToString();
                 }
                
                else if (Session["fxid"] != null)//fxid是分享链接推广
                {
                    string fxid = Session["fxid"].ToString();
                }
                param["validateCode"] = validateCode;
                param["mobile"] = mobile;
                param["source"] = EntityModel.Enum.SchemeSource.NewWeb;

                param["info"] = registerInfo;
                param["fxid"] = Session["fxid"];
                param["yqid"] = Session["yqid"];

                result = await serviceProxyProvider.Invoke<CommonActionResult>(param, "api/User/RegisterResponseMobile");

                if (result.IsSuccess)
                {
                    param.Clear();
                    param["loginName"] = registerInfo.LoginName;
                    param["password"] = passWord;
                    param["IPAddress"] = IpManager.IPAddress;
                    EntityModel.CoreModel.LoginInfo loginInfo = await serviceProxyProvider.Invoke<EntityModel.CoreModel.LoginInfo>(param, "api/user/user_login");
                    if (loginInfo.IsSuccess) 
                        CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                    else
                        return Json(new { IsSuccess = false, Message = loginInfo.Message, userToken = loginInfo.UserToken }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Succuss = result.IsSuccess, Msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        #endregion

        // 本地账号登录系统
        [HttpPost]
        public async Task<JsonResult> LoginUser123()
        {
            try
            {
                if (CurrentUser != null)
                    return Json(new { IsSuccess = true, Message = CurrentUser.LoginInfo.Message });

                string userName = PreconditionAssert.IsNotEmptyString(Request["userName"], "登录账号不能为空。");
                string passWord = PreconditionAssert.IsNotEmptyString(Request["passWord"], "登录密码不能为空。");
                //验证码
                //if (!Checkcaptcha())
                //{
                //    return Json(new { IsSuccess = false, Message = "请拖动验证滑块到正确位置!" });
                //}

                Session["userCurrentPassWord"] = passWord;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["loginName"] = userName;
                param["password"] = passWord;
                param["IPAddress"] = IpManager.IPAddress;
                var loginInfo = await serviceProxyProvider.Invoke<EntityModel.CoreModel.LoginInfo>(param, "api/User/User_Login");
                //LoginInfo loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                if (loginInfo.IsSuccess)
                {
                    #region 调用UCenter注册用户

                    //ThreadPool.UnsafeQueueUserWorkItem((u) =>
                    //{
                    //    try
                    //    {
                    //        string userNameCode = HttpContext.Server.UrlEncode(u.ToString());
                    //        var client = new DS.Web.UCenter.Client.UcClient();
                    //        var bbsInfo = client.UserInfo(userNameCode);
                    //        if (!bbsInfo.Success)
                    //        {
                    //            //查询用户信息失败，注册用户
                    //            //var email = "test@163.com";// string.Format("{0}@126.com", userName);
                    //            var email = "bbs@wancai.com";// string.Format("{0}@wancai.com", userName);
                    //            var r = client.UserRegister(userNameCode, passWord, email);
                    //            if (r.Result == DS.Web.UCenter.RegisterResult.Success)
                    //            {
                    //                //模拟登录激活用户
                    //                var url = "http://bbs.wancai.com/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1";
                    //                var a = PostManager.Post(url, string.Format("fastloginfield=username&username={0}&password={1}&quickforward=yes&handlekey=ls", userNameCode, passWord), Encoding.GetEncoding("gbk"));
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        log.Write("LoginUser", "CallUcenter", ex);
                    //    }
                    //}, userName);

                    #endregion

                    CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                    //本次登录的时间
                    Session["currentLoginTime"] = DateTime.Now;
                    return Json(new { IsSuccess = true, Message = loginInfo.Message });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = loginInfo.Message, UserId = loginInfo.UserId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }



        /// <summary>
        /// 第三方登录
        /// </summary>
        public JsonResult PartnerLogin()
        {
            try
            {
                string userName = PreconditionAssert.IsNotEmptyString(Request["userName"], "登录账号不能为空。");
                string passWord = PreconditionAssert.IsNotEmptyString(Request["passWord"], "登录密码不能为空。");
                LoginInfo loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                if (loginInfo.IsSuccess)
                {
                    var b = WCFClients.GameFundClient.QueryMyBalance(loginInfo.UserToken);
                    return Json(new
                    {
                        IsSuccess = true,
                        Message = loginInfo.Message,
                        IsVip = false,
                        VipLevel = 0,
                        //Balance = b.CommonBalance,
                        IsAlipay = loginInfo.LoginFrom == "ALIPAY",
                        // IsGoldAlipay = AlipayInfo != null ? AlipayInfo.IsGoldAccount : false,
                        DisplayName = HideUserName(loginInfo.DisplayName, loginInfo.HideDisplayNameCount),
                        UserId = loginInfo.UserId,
                        IsNeedBetPwd = b.CheckIsNeedPassword("Bet"),
                        BonusBalance = b.BonusBalance,
                        IsQQ = loginInfo.LoginFrom == "QQ",
                        Figureurl = string.Empty,
                        //UnReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(loginInfo.UserToken),
                        LoginFrom = loginInfo.LoginFrom,
                    });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = loginInfo.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        public string BBS_Url
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BBS_URL"];
            }
        }
        public ActionResult syncLogin()
        {
            ViewBag.BBS = BBS_Url;
            if (CurrentUser != null && CurrentUser.LoginInfo != null)
            {
                try
                {
                    //var client = new DS.Web.UCenter.Client.UcClient();
                    //string gbStr = System.Text.Encoding.GetEncoding("gbk").GetString(System.Text.Encoding.GetEncoding("gb2312").GetBytes(CurrentUser.LoginInfo.LoginName));
                    //var bbsInfo = client.UserInfo(gbStr);
                    //if (bbsInfo.Success)
                    //{
                    //    var html = client.UserSynlogin(bbsInfo.Uid);
                    //    ViewBag.LoginName = CurrentUser.LoginInfo.LoginName;
                    //    ViewBag.Password = Session["userCurrentPassWord"].ToString();
                    //    ViewBag.Html = html;

                    //    return View();

                    //}
                }
                catch (Exception)
                {

                }
            }
            ViewBag.Html = "";

            return View();
        }

        //返回用户是否已登录
        public JsonResult IsLogin()
        {
            return Json(CurrentUser != null, JsonRequestBehavior.AllowGet);
        }

        //退出用户登录
        public JsonResult LogOutUser()
        {
            try
            {
                CurrentUser = null;
                return Json(new { IsSuccess = true, Message = "退出成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }

        }

        //获取用户信息，返回Json
        public JsonResult VipInfo()
        {
            try
            {
                var cuser = CurrentUser;
                if (cuser == null)
                    return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
                var isAuthentication = false;
                if (Session["VipInfo"] == null)
                {
                    //var loginInfo = WCFClients.ExternalClient.LoginByUserId(CurrentUser.LoginInfo.UserId);
                    //cuser.LoginInfo.MaxLevelName = loginInfo.MaxLevelName;
                    isAuthentication = RealNameInfo != null;
                    cuser.IsAuthenticationRealName = isAuthentication;

                    Session["CurrentUserInfo"] = cuser;
                    Session["VipInfo"] = 1;
                }
                else
                {
                    isAuthentication = cuser.IsAuthenticationRealName;
                }
                var res = new
                {
                    IsSuccess = true,
                    vipLevel = cuser.LoginInfo.VipLevel,
                    //实名认证
                    isAuthentication = isAuthentication,
                    growthl = CurrentUserBalance.UserGrowth,
                    //现金金额=充值金额+奖金+返点+名家
                    balance = CurrentUserBalance.GetTotalCashMoney(),
                    //红包金额
                    rbbalance = CurrentUserBalance.RedBagBalance,
                    displayName = HideUserName(cuser.LoginInfo.DisplayName, cuser.LoginInfo.HideDisplayNameCount),
                    userId = cuser.LoginInfo.UserId,
                    isNeedBetPwd = CurrentUserBalance.CheckIsNeedPassword("Bet"),
                    //豆豆数量
                    doudou = CurrentUserBalance.CurrentDouDou,
                    //最高中奖中文
                    MaxLevelName = CurrentUser.LoginInfo.MaxLevelName,
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //获取用户信息，返回Json
        public JsonResult Info()
        {
            try
            {
                var cuser = CurrentUser;
                if (cuser != null)
                {
                    var res = new
                    {
                        IsSuccess = true,
                        isVip = false,
                        vipLevel = cuser.LoginInfo.VipLevel,
                        //IsRebate = cuser.LoginInfo.IsRebate,
                        //IsExperter = cuser.LoginInfo.IsExperter,
                        //IsBetHM = cuser.LoginInfo.IsBetHM,
                        //实名认证
                        isAuthentication = cuser.IsAuthenticationRealName,
                        //手机认证
                        //isAuthenticationMobile = CurrentUser.IsAuthenticationMobile,
                        //我的成长状态
                        //growthl = GrowthStatus(CurrentUser.UserBalance.UserGrowth),
                        growthl = CurrentUserBalance.UserGrowth,
                        //我的账户安全级别
                        accountl = AccountLevel(cuser),
                        //现金金额=充值金额+奖金+返点+名家
                        balance = CurrentUserBalance.GetTotalCashMoney(),
                        //红包金额
                        rbbalance = CurrentUserBalance.RedBagBalance,
                        //isAlipay = CurrentUser.LoginInfo.LoginFrom == "ALIPAY",
                        //isGoldAlipay = CurrentUser.AlipayInfo != null ? CurrentUser.AlipayInfo.IsGoldAccount : false,
                        displayName = HideUserName(cuser.LoginInfo.DisplayName, cuser.LoginInfo.HideDisplayNameCount),
                        userId = cuser.LoginInfo.UserId,
                        isNeedBetPwd = CurrentUserBalance.CheckIsNeedPassword("Bet"),
                        // isQQ = CurrentUser.LoginInfo.LoginFrom == "QQ",
                        //Figureurl = CurrentUser.LoginInfo.LoginFrom == "QQ" ? CurrentUser.Figureurl : string.Empty,
                        //unReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken),
                        MaxLevelName = CurrentUser.LoginInfo.MaxLevelName,

                    };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("用户未登录");
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //登录后如果刷新页面，则user.info里面的东西没有保存,所以重新请求一回
        public JsonResult ReloadUser()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["pid"]))
                    Session["pid"] = Request["pid"];

                if (!string.IsNullOrEmpty(Request["yqid"]))
                    Session["yqid"] = Request["yqid"];

                var currentUser = this.CurrentUser;
                if (currentUser != null)
                {
                    //LoadUserLeve();
                    LoadUerBalance();
                    var res = new
                    {
                        IsSuccess = true,
                        isVip = false,
                        vipLevel = currentUser.LoginInfo.VipLevel,
                        //IsRebate = currentUser.LoginInfo.IsRebate,
                        //我的成长状态
                        //  growthl = GrowthStatus(CurrentUser.UserBalance.UserGrowth),
                        growthl = CurrentUserBalance.UserGrowth,
                        isAuthentication = currentUser.IsAuthenticationRealName,
                        //我的账户安全级别
                        accountl = AccountLevel(currentUser),
                        //现金金额=充值金额+奖金+返点+名家
                        balance = CurrentUserBalance.GetTotalCashMoney(),
                        //红包金额
                        rbbalance = CurrentUserBalance.RedBagBalance,
                        isAlipay = currentUser.LoginInfo.LoginFrom == "ALIPAY",
                        // isGoldAlipay = currentUser.AlipayInfo != null ? currentUser.AlipayInfo.IsGoldAccount : false,
                        displayName = HideUserName(currentUser.LoginInfo.DisplayName, currentUser.LoginInfo.HideDisplayNameCount),
                        userId = currentUser.LoginInfo.UserId,
                        isNeedBetPwd = CurrentUserBalance.CheckIsNeedPassword("Bet"),
                        isQQ = currentUser.LoginInfo.LoginFrom == "QQ",
                        Figureurl = currentUser.LoginInfo.LoginFrom == "QQ" ? currentUser.Figureurl : string.Empty,
                        //unReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken),
                        doudou = CurrentUserBalance.CurrentDouDou,
                        MaxLevelName = currentUser.LoginInfo.MaxLevelName,
                        //IsExperter = currentUser.LoginInfo.IsExperter,
                        //IsBetHM = currentUser.LoginInfo.IsBetHM,

                    };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        //刷新余额
        public JsonResult Refreshub()
        {
            var canLoad = true;
            var lastRequestUserBalance = Session["LastRequestUserBalance"];
            if (lastRequestUserBalance == null)
            {
                //第一次
                canLoad = true;
                Session["LastRequestUserBalance"] = DateTime.Now;
            }
            else
            {
                //第N次
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(lastRequestUserBalance);
                canLoad = ts.TotalSeconds > 5;
                Session["LastRequestUserBalance"] = DateTime.Now;
            }
            if (canLoad)
            {
                var ub = CurrentUserBalance;
                LoadUerBalance();
                return Json(new
                {
                    fb = ub.GetTotalCashMoney(),
                    rb = ub.RedBagBalance
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { fb = -1, rb = -1 }, JsonRequestBehavior.AllowGet);
        }

        //提交意见反馈
        [HttpPost]
        public JsonResult SubmitFeed(FormCollection postForm)
        {
            try
            {
                var content = PreconditionAssert.IsNotEmptyString(postForm["feedContent"], "提交内容不能为空。");
                var category = string.IsNullOrEmpty(postForm["category"]) ? "normal" : postForm["category"];
                UserIdeaInfo_Add ideaInfo = new UserIdeaInfo_Add()
                {
                    Description = content,
                    Category = category,
                    IsAnonymous = CurrentUser == null ? true : false,
                    CreateUserId = CurrentUser == null ? "" : CurrentUser.LoginInfo.UserId,
                    CreateUserDisplayName = CurrentUser == null ? "" : CurrentUser.LoginInfo.DisplayName
                };
                var result = WCFClients.ExternalClient.SubmitUserIdea(ideaInfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        //UserToken登录
        public ActionResult l()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["token"]))
                {
                    var token = Request["token"];
                    var lInfo = WCFClients.ExternalClient.LoginByUserToken(token);
                    if (lInfo.IsSuccess)
                    {
                        //CurrentUser = new CurrentUserInfo() { LoginInfo = lInfo };
                        Response.Redirect("/");
                    }
                    else
                    {
                        throw new Exception(lInfo.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
            }
            return View();
        }

        private static List<string> _cache_UserIdIsExsite = new List<string>();
        //用户名是否存在
        public async Task<JsonResult> GetUserIdIsExsite()
        {
            try
            {
                if (Session["last_UserIdIsExsite"] != null)
                {
                    var lastDate = (DateTime)Session["last_UserIdIsExsite"];
                    if ((DateTime.Now - lastDate).TotalSeconds < 3)
                    {
                        return Json(new { code = false, msg = "请求过于频繁" }, JsonRequestBehavior.AllowGet);
                    }
                }
                Session["last_UserIdIsExsite"] = DateTime.Now;

                string userName = PreconditionAssert.IsNotEmptyString(Request["uid"], "用户名不能为空。");
                if (_cache_UserIdIsExsite.Exists(p => p == userName))
                    return Json(new { code = false, msg = "用户名不存在" }, JsonRequestBehavior.AllowGet);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["loginName"] = userName;

                string loginName = await serviceProxyProvider.Invoke<string>(param, "api/user/GetLoginNameIsExsite"); ;
                if (string.IsNullOrEmpty(loginName))
                {
                    _cache_UserIdIsExsite.Add(userName);
                    if (_cache_UserIdIsExsite.Count > 100)
                        _cache_UserIdIsExsite.RemoveAt(0);
                    return Json(new { code = false, msg = "用户名不存在" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { code = true, msg = "用户名存在" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //检查用户名合法性
        public JsonResult checkUserName(string id)
        {
            try
            {
                string userName = PreconditionAssert.IsNotEmptyString(id, "注册用户名不能为空。");

                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(userName), "用户名不允许包含敏感词，如有疑问请联系客服。");

                try
                {
                    string loginName = WCFClients.ExternalClient.GetUserIdByLoginName(userName);
                    if (!string.IsNullOrEmpty(loginName))
                    {
                        return Json(new { code = false, Msg = "用户已被注册" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                }

                return Json(new { code = true, Msg = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //检查是否身份证
        public JsonResult checkIsIdNum(string id)
        {
            try
            {
                string card = PreconditionAssert.IsNotEmptyString(id, "身份证号码不能为空。");

                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(card), "身份证号码格式错误，请重新输入。");

                return Json(new { code = true, Msg = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //检查手机是否绑定
        public JsonResult isMobileBinded()
        {
            try
            {

                string mobile = PreconditionAssert.IsNotEmptyString(Request["mobile"], "手机号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
                var result = WCFClients.ExternalClient.IsMobileBinded(CurrentUser.LoginInfo.UserId, mobile);
                if (result == true)
                {
                    return Json(new { code = true, Msg = "此手机号码不可用" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = false, Msg = "此手机号码可用" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                return Json(new { code = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //手机验证码（语音）
        //public JsonResult getTelCode()
        //{
        //    try
        //    {
        //        string mobile = PreconditionAssert.IsNotEmptyString(Request["bindMobile"], "手机号码不能为空。");
        //        PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
        //        var result = WCFClients.ExternalClient.RequestAuthenticationMobile(mobile, UserToken);
        //        string code = result.ReturnValue;//生成校验码

        //        #region 发送站内消息：手机短信或站内信

        //        var pList = new List<string>();
        //        pList.Add(string.Format("{0}={1}", "[ValidNumber]", code));
        //        //发送短信
        //        WCFClients.GameQueryClient.DoSendSiteMessage("", mobile, "ON_User_Bind_Mobile_Before", string.Join("|", pList.ToArray()));

        //        #endregion

        //        return Json(new { code = true, Msg = 1 }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { code = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //提交认证（手机，实名）
        public JsonResult bindMobileOrRealname()
        {

            try
            {
                //string mobileCode = PreconditionAssert.IsNotEmptyString(Request["mobileCode"], "校验码不能为空。");
                string mobileCode = string.IsNullOrEmpty(Request["mobileCode"]) ? "" : Request["mobileCode"];
                var realName = PreconditionAssert.IsNotEmptyString(Request["realName"], "真实姓名不能为空。");
                var cardNumber = PreconditionAssert.IsNotEmptyString(Request["idCardNumber"], "身份证号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(cardNumber), "请输入正确的身份证号码。");
                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(realName), "真实姓名不允许包含敏感词，如有疑问请联系客服。");
                UserRealNameInfo realInfo = new UserRealNameInfo() { IdCardNumber = cardNumber, RealName = realName };
                if (CurrentUser.IsAuthenticationMobile == false)
                {
                    var result_mobile = WCFClients.ExternalClient.ResponseAuthenticationMobile(mobileCode, SchemeSource.Web, UserToken);
                    if (!result_mobile.IsSuccess)
                    {
                        return Json(new { code = false, Msg = result_mobile.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (CurrentUser.IsAuthenticationRealName == false)
                {
                    var result_realName = WCFClients.ExternalClient.AuthenticateMyRealName(realInfo, SchemeSource.Web, UserToken);
                    if (!result_realName.IsSuccess)
                    {
                        return Json(new { code = false, Msg = result_realName.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { code = true, Msg = "完善信息成功。！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 验证码相关函数

        //生成验证码并返回一个结果
        public ValidateCodeGenerator CreateValidateCode()
        {
            var num = 0;
            string randomText = IsTest ? "8888" : SelectRandomNumber(5, out num);
            Session["ValidateCode"] = num;
            ValidateCodeGenerator vlimg = new ValidateCodeGenerator()
            {
                BackGroundColor = Color.FromKnownColor(KnownColor.LightGray),
                RandomWord = randomText,
                ImageHeight = 25,
                ImageWidth = 100,
                fontSize = 14,
            };
            return vlimg;
        }

        /// <summary>
        /// 内部校验验证码是否正确
        /// </summary>
        /// <param name="verifycode"></param>
        /// <returns></returns>
        private bool VerifyCode(string verifycode)
        {
            try
            {
                if (string.IsNullOrEmpty(verifycode) || Session["ValidateCode"] == null)
                {
                    return false;
                }
                if (verifycode.ToLower() == Session["ValidateCode"].ToString().ToLower())
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //Ajax校验验证码是否正确
        [HttpPost]
        public JsonResult ValidateCode(FormCollection post)
        {
            if (string.IsNullOrEmpty(post["VerifyCode"]) || Session["ValidateCode"] == null)
            {
                return Json(false);
            }
            if (post["VerifyCode"].ToLower() == Session["ValidateCode"].ToString().ToLower())
            {
                return Json(true);
            }
            return Json(false);
        }

        //选择随机字符
        private string SelectRandomWord(int numberOfChars)
        {
            if (numberOfChars > 26)
            {
                throw new InvalidOperationException("Random Word Charecters can not be greater than 26.");
            }
            // Creating an array of 26 characters  and 0-9 numbers
            char[] columns = new char[26];

            for (int charPos = 65; charPos < 65 + 26; charPos++)//取26个字符
                columns[charPos - 65] = (char)charPos;

            //for (int intPos = 48; intPos <= 57; intPos++)//取 0-9
            //    columns[26 + (intPos - 48)] = (char)intPos;

            StringBuilder randomBuilder = new StringBuilder();
            // pick charecters from random position
            Random randomSeed = new Random();
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(26)].ToString());//取26个字符里的任意一个

            }

            return randomBuilder.ToString();
        }

        //选择随机数字
        private string SelectRandomNumber(int numberOfChars, out int num)
        {
            num = 0;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            char[] columns = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                if (incr == 0 || incr == 2)
                {
                    var randomNum = columns[randomSeed.Next(10)].ToString();
                    randomBuilder.Append(randomNum);//取26个字符里的任意一个
                    num += int.Parse(randomNum);
                }
                if (incr == 1)
                {
                    randomBuilder.Append("+").ToString();
                }
                if (incr == 3)
                {
                    randomBuilder.Append("=").ToString();
                }
                if (incr == 4)
                {
                    randomBuilder.Append("?").ToString();
                }
            }
            return randomBuilder.ToString();
        }

        //选择随机字符+数字
        private string SelectRandomWordNumber(int numberOfChars)
        {
            if (numberOfChars > 36)
            {
                throw new InvalidOperationException("Random Word Charecters can not be greater than 26.");
            }
            // Creating an array of 26 characters  and 0-9 numbers
            char[] columns = new char[36];

            for (int charPos = 65; charPos < 65 + 26; charPos++)//取26个字符
                columns[charPos - 65] = (char)charPos;

            for (int intPos = 48; intPos <= 57; intPos++)//取 0-9
                columns[26 + (intPos - 48)] = (char)intPos;

            StringBuilder randomBuilder = new StringBuilder();
            // pick charecters from random position
            Random randomSeed = new Random();
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(36)].ToString());//取26个字符里的任意一个

            }

            return randomBuilder.ToString();
        }
        #endregion

        #region 生成短信验证码
        ///将数字转化为字符串
        ///数字+字母
        private const string RandowmChar = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="len"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<string> GetRandString(int len, int count)
        {
            var maxValue = Math.Pow(36, len);
            if (maxValue > long.MaxValue)
            {
                return null;
            }

            var allCount = (long)maxValue;
            var stepLong = allCount / count;
            if (stepLong > int.MaxValue)
            {
                return null;
            }
            var step = (int)stepLong;
            if (step < 3)
            {
                return null;
            }
            long begin = 0;
            var list = new List<string>();
            var rand = new Random();
            while (true)
            {
                var value = rand.Next(1, step) + begin;
                begin += step;
                list.Add(GetChart(len, value));
                if (list.Count == count)
                {
                    break;
                }
            }

            list = SortByRandom(list);

            return list;
        }

        /// <summary>
        /// 将数字转化成字符串
        /// </summary>
        /// <param name="len"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetChart(int len, long value)
        {
            var str = new StringBuilder();
            while (true)
            {
                str.Append(RandowmChar[(int)(value % 36)]);
                value = value / 36;
                if (str.Length == len)
                {
                    break;
                }
            }

            return str.ToString();
        }

        ///打乱数组顺序
        /// <summary>
        /// 随机排序
        /// </summary>
        /// <param name="charList"></param>
        /// <returns></returns>
        private List<string> SortByRandom(List<string> charList)
        {
            var rand = new Random();
            for (int i = 0; i < charList.Count; i++)
            {
                int index = rand.Next(0, charList.Count);
                string temp = charList[i];
                charList[i] = charList[index];
                charList[index] = temp;
            }

            return charList;
        }
        #endregion

        //#region 充值券充值相关

        ////彩金券充值
        //[UnionFilter]
        //public ActionResult lotteryticket()
        //{
        //    ViewBag.TotalBonusMoney = 0M;
        //    ViewBag.TotalUser = 0;
        //    try
        //    {
        //        ViewBag.TotalUser = WCFClients.ExternalClient.QueryTotalBonusUserCount(UserToken);
        //        ViewBag.TotalBonusMoney = WCFClients.ExternalClient.QueryTotalBonusMoney(UserToken);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return View();
        //}

        ////彩金券充值
        //public JsonResult uselotteryticket()
        //{
        //    try
        //    {
        //        var ticket = PreconditionAssert.IsNotEmptyString(Request["ticket"], "请输入彩金券编码。");
        //        var verifycode = PreconditionAssert.IsNotEmptyString(Request["verifycode"], "请输入验证码。");
        //        PreconditionAssert.IsTrue(VerifyCode(verifycode), "验证码错误");
        //        var result = WCFClients.ActivityClient.ExchangeCoupons(ticket, UserToken);
        //        return Json(new { state = result.IsSuccess ? 1 : -1, msg = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { state = -1, msg = ex.Message });
        //    }
        //}

        ////充值券充值页面
        //[UnionFilter]
        //public ActionResult CouponPay()
        //{
        //    if (!string.IsNullOrEmpty(Request["pid"]))
        //    {
        //        string pid = Request["pid"];
        //        int adid = string.IsNullOrEmpty(Request["adid"]) ? 0 : int.Parse(Request["adid"]);
        //        //RecordVisite();
        //    }

        //    CurrentUser = null;
        //    ViewBag.TotalBonusMoney = 0M;
        //    ViewBag.TotalUser = 0;
        //    try
        //    {
        //        ViewBag.TotalUser = WCFClients.ExternalClient.QueryTotalBonusUserCount(UserToken);
        //        ViewBag.TotalBonusMoney = WCFClients.ExternalClient.QueryTotalBonusMoney(UserToken);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return View();
        //}

        ////手机号码是否已被认证,true为已认证，false为未认证
        //[HttpPost]
        //public JsonResult MobileIsAuth()
        //{
        //    try
        //    {
        //        string mobile = PreconditionAssert.IsNotEmptyString(Request.Form["mobile"], "手机号码不能为空。");
        //        return Json(WCFClients.ExternalClient.CheckIsMobileAuthenticated(mobile, UserToken));
        //    }
        //    catch
        //    {
        //        return Json(false);
        //    }
        //}
        //#endregion

        //用支付宝账号登录，校验验证码是否正确
        public JsonResult ChkCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { IsSuccess = false, msg = "请输入验证码" }, JsonRequestBehavior.AllowGet);
            }
            var phoneCode = Session["alipay-phone-code"];
            if (phoneCode == null)
            {
                return Json(new { IsSuccess = false, msg = "您输入的验证码已过期" }, JsonRequestBehavior.AllowGet);
            }
            if (phoneCode.ToString() != code)
            {
                return Json(new { IsSuccess = false, msg = "您输入的验证码有误，请重新输入" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
        }

        #region 用户注册手机验证

        /// <summary>
        /// 注册后手机验证码认证
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterValidaMobile()
        {
            ViewBag.VerifyCode = CommonAPI.QueryCoreConfigByKey();
            var userId = Request["UserId"] == null ? string.Empty : Request["UserId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                Session["tempUserId"] = userId;
                Session["IsCode"] = false;
            }
            if (Session["tempUserId"] == null)
                return new RedirectResult("/statichtml/register.html");
            return View();
        }

        #region "20171108增加配置（禁止注册的手机号码）"
        /// <summary>
        /// 禁止注册的号码
        /// </summary>
        public string BanRegistrMobile
        {
            get
            {
                string defalutValue = "";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("BanRegistrMobile").ConfigValue;
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



        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> RegisterSendMobileCodeAsync()
        {
            try
            {
                //if (Session["TempRegisterInfo"] == null)
                //    throw new Exception("注册对象为空");

                //return Json(new { Succuss = true, Msg = string.Empty, Code = "321546" });
                string mobile = PreconditionAssert.IsNotEmptyString(Request["mobile"], "手机号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
                string code = PreconditionAssert.IsNotEmptyString(Request["code"], "验证码不能为空。");
                string vcode = Session["ValidateCode"] != null ? Session["ValidateCode"].ToString() : "";
                if (vcode != code)
                {
                    return Json(new { IsSuccess = false, Msg = "验证码不一致请重新输入" });
                }
                //Session["ValidateCode"] = num;
                #region "20171108增加配置（禁止注册的手机号码）"
                Dictionary<string, object> Keyparam = new Dictionary<string, object>();
                Keyparam["key"] = "BanRegistrMobile";

                var banRegistrMobile = await serviceProxyProvider.Invoke<C_Core_Config>(Keyparam, "api/user/QueryCoreConfigByKey");
                //string banRegistrMobile = BanRegistrMobile;
                if (banRegistrMobile.ConfigValue.Contains(mobile))
                {
                    return Json(new { IsSuccess = false, Msg = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。" });
                }
                if (mobile.StartsWith("1880000"))
                {
                    return Json(new { IsSuccess = false, Msg = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。" });
                }
                if (mobile.StartsWith("1770000"))
                {
                    return Json(new { IsSuccess = false, Msg = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。" });
                }
                foreach (var item in banRegistrMobile.ConfigValue.Split('|'))
                {
                    if (mobile.StartsWith(item))
                    {
                        return Json(new { IsSuccess = false, Msg = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。" });
                    }
                }
                #endregion

                //var userId = string.Empty;
                //if (Session["tempUserId"] != null) userId = Session["tempUserId"].ToString();
                //var result = WCFClients.ExternalClient.RegisterRequestMobile(mobile, userId);
                //string geetest = CommonAPI.QueryCoreConfigByKey();
                //if (geetest == "Default")
                //{
                //    string verifycode = PreconditionAssert.IsNotEmptyString(Request["verifycode"], "验证码不能为空。");
                //    PreconditionAssert.IsTrue(VerifyCode(verifycode), "请输入正确的验证码。");
                //}
                //else
                //{
                //    if (!Checkcaptcha())
                //    {
                //        return Json(new { Succuss = false, Msg = "请拖动验证滑块到正确位置!" }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                Keyparam.Clear();
                Keyparam["mobile"] = mobile;
                var result = await serviceProxyProvider.Invoke<CommonActionResult>(Keyparam, "api/user/RegisterRequestMobile");
                //var resutl = WCFClients.ExternalClient.RegisterRequestMobile(mobile);
                Session["IsCode"] = result.IsSuccess;
                Session["mobile"] = mobile;
                return Json(new { Succuss = result.IsSuccess, Msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 检查验证码 注册最后一步
        /// </summary>
        /// <returns></returns>
        public JsonResult RegisterResponseMobile()
        {
            try
            {
                string validateCode = PreconditionAssert.IsNotEmptyString(Request["validateCode"], "验证码不能为空！");
                string mobile = PreconditionAssert.IsNotEmptyString(Request["mobile"], "手机号码不能为空！");

                var passWord = string.Empty;
                var userName = string.Empty;
                var userId = string.Empty;

                var userInfo = new RegisterInfo_Local();
                if (Session["TempRegisterInfo"] == null)
                    return Json(new { Succuss = false, Msg = "注册失败，请重新注册！" }, JsonRequestBehavior.AllowGet);
                userInfo = Session["TempRegisterInfo"] as RegisterInfo_Local;
                userInfo.RegisterIp = IpManager.IPAddress;
                if (Session["pid"] != null)
                    userInfo.AgentId = Session["pid"].ToString();

                userName = userInfo.LoginName;
                passWord = userInfo.Password;
                Common.Communication.CommonActionResult result = null;
                if (Session["yqid"] != null)
                {
                    string yqid = Session["yqid"].ToString();
                    result = WCFClients.ExternalClient.RegisterResponseMobile_Spread(validateCode, mobile, SchemeSource.Web, userInfo, yqid);

                }
                else
                {
                    result = WCFClients.ExternalClient.RegisterResponseMobile(validateCode, mobile, SchemeSource.Web, userInfo);
                }

                if (result.IsSuccess)
                {
                    Session["IsCode"] = false;
                    Session["tempUserId"] = string.Empty;
                    Session["TempRegisterInfo"] = null;
                    LoginInfo loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                    if (loginInfo.IsSuccess) { }
                    //CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                    else
                        return Json(new { IsSuccess = false, Message = loginInfo.Message, userToken = loginInfo.UserToken }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Succuss = result.IsSuccess, Msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ClearSession()
        {
            try
            {
                Session["IsCode"] = false;
                return Json(new { Succuss = true, Msg = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region 验证码
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public string Getcaptcha()
        {
            GeetestLib geetest = new GeetestLib(GeetestConfig.publicKey, GeetestConfig.privateKey);
            //String userID = "iqucai";
            string userID = Common.Net.IpManager.IPAddress;
            Byte gtServerStatus = geetest.preProcess(userID);
            Session[GeetestLib.gtServerStatusSessionKey] = gtServerStatus;
            Session["iqucai"] = userID;
            return geetest.getResponseStr();
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        private bool Checkcaptcha()
        {
            GeetestLib geetest = new GeetestLib(GeetestConfig.publicKey, GeetestConfig.privateKey);
            Byte gt_server_status_code = (Byte)Session[GeetestLib.gtServerStatusSessionKey];
            String userID = (String)Session["iqucai"];
            int result = 0;
            String challenge = Request.Form.Get(GeetestLib.fnGeetestChallenge);
            String validate = Request.Form.Get(GeetestLib.fnGeetestValidate);
            String seccode = Request.Form.Get(GeetestLib.fnGeetestSeccode);
            if (gt_server_status_code == 1) result = geetest.enhencedValidateRequest(challenge, validate, seccode, userID);
            else result = geetest.failbackValidateRequest(challenge, validate, seccode);
            if (result == 1) return true;
            else return false;
        }
        #endregion
    }
}
