using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using GameBiz.Core;
using Common.Utilities;
using Common.Net;
using System.Xml;
using Common.Net.SMS;
using Common.XmlAnalyzer;
using app.lottery.site.Models;
using External.Core.Login;
using System.IO;
using System.Text;
using Common.JSON;
using System.Text.RegularExpressions;
using External.Core.SiteMessage;
using Common.Communication;
using log4net;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;
using System.Threading.Tasks;

namespace app.lottery.site.Controllers
{
    [CheckReferer]
    [CheckBrowser]
    public class HomeController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public HomeController(IServiceProxyProvider _serviceProxyProvider, ILog log, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion
        public HomeController()
        {
        }
       

      
        //[UnionFilter]
        //public ActionResult Default()
        //{
        //    //咨询优化
        //    ViewBag.Cphot = new ArticleInfo_QueryCollection();// WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Hot", "", 0, 3);
        //    ViewBag.Gpc = new ArticleInfo_QueryCollection();//WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JX11X5|CQSSC", 0, 3);
        //    ViewBag.Scz = new ArticleInfo_QueryCollection();//WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT|PL3|FC3D", 0, 4);
        //    ViewBag.Jjc = new ArticleInfo_QueryCollection();//WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ|BJDC", 0, 4);
        //    ViewBag.CurrentUser = CurrentUser;

        //    //神单排行
        //    var now = DateTime.Now;
        //    ViewBag.RankList = WCFClients.ExternalClient.QueryGSRankList(now.ToString("MM.dd"), now.ToString("MM.dd"), "", "");
        //    //焦点新闻
        //    ViewBag.FocusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 10);
        //    //中奖新闻
        //    ViewBag.BonusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 10);
        //    //总注册人数
        //    ViewBag.TotalUserCount = WCFClients.ExternalClient.QueryUserRegisterCount();
        //    //神单排行（月榜/总榜）
        //    var MonthBeginTime = DateTime.Now.AddDays(-30);
        //    var TotalBeginTime = DateTime.Now.AddDays(-90);
        //    var endTime = DateTime.Now.AddDays(1);
        //    ViewBag.mRankList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(MonthBeginTime, endTime, "", "", 0, 10);
        //    ViewBag.tRankList = WCFClients.GameQueryClient.QueryRankReport_BettingProfit_Sport(TotalBeginTime, endTime, "", "", 0, 10);
        //    ViewBag.LotteryBonus = new LotteryNewBonusInfoCollection();// WCFClients.ExternalClient.QueryLotteryNewBonusInfoList(15);

        //    return View();
        //}

        public ActionResult default_app()
        {
            return View();
        }


        //首页幻灯
        public async Task<PartialViewResult> SlideBoxAsync()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["bannerType"] = BannerType.Index;
            param["returnRecord"] = 10;

            ViewBag.Ads = await serviceProxyProvider.Invoke<EntityModel.CoreModel.SiteMessageBannerInfo_Collection>(param, "api/data/QuerySitemessageBanngerList_Web");
            
            return PartialView();
        }

        /// <summary>
        /// 异步请求赛事资讯
        /// </summary>
        /// <returns></returns>
        public JsonResult News()
        {
            try
            {
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 11 : int.Parse(Request.QueryString["pageSize"]);
                var code = string.IsNullOrEmpty(Request["GameCode"]) ? "" : Request["GameCode"];

                ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList("", code, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
                return Json(new { msg = ViewBag.DataSource, Issucess = true });
            }
            catch (Exception ex)
            {

                return Json(new { msg = ex.Message, Issucess = false });
            }
        }
        public PartialViewResult Header()
        {
            //var url = "http://" + HttpContext.Request.Url.Host;
            //var agent = WCFClients.ExternalClient.QueryStoreIdByUrl(url);
            //ViewBag.Agent = agent;
            return PartialView();
        }
        public PartialViewResult HeaderTop()
        {
            var cuser = CurrentUser;
            ViewBag.CurrentUser = cuser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            if (cuser != null)
            {
                //我的成长状态
                ViewBag.GrowthLevel = CurrentUserBalance.UserGrowth;
                //我的账户安全级别
                ViewBag.AccountLevel = AccountLevel(cuser);
                //站内信
                //ViewBag.UnReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken);
            }
            return PartialView();
        }
        public PartialViewResult HeaderIndex()
        {
            var cuser = CurrentUser;
            ViewBag.CurrentUser = cuser;
            if (cuser != null)
            {
                //我的成长状态
                ViewBag.GrowthLevel = CurrentUserBalance.UserGrowth;
                //我的账户安全级别
                ViewBag.AccountLevel = AccountLevel(cuser);
                //站内信
                //ViewBag.UnReadMail = WCFClients.GameQueryClient.GetMyUnreadInnerMailCount(UserToken);
            }
            var url = "http://" + HttpContext.Request.Url.Host;
            var agent = WCFClients.ExternalClient.QueryStoreIdByUrl(url);
            ViewBag.Agent = agent;
            //首页幻灯
            ViewBag.Ads = WCFClients.ExternalClient.QuerySitemessageBanngerList_Web(BannerType.Index);
            return PartialView();
        }
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        public PartialViewResult FooterIndex()
        {
            return PartialView();
        }
        //关注
        public JsonResult SetFansOrNot(string flag, string uid, string fuid)
        {
            //uid操作的人,fuid被操作的人
            int f;
            int.TryParse(flag, out f);
            if (f != 0 && f != 1)
            {
                return Json(new { isSuccess = false, msg = "参数异常" }, JsonRequestBehavior.AllowGet); ;
            }
            if (string.IsNullOrEmpty(uid))
            {
                return Json(new { isSuccess = false, msg = "参数为空" }, JsonRequestBehavior.AllowGet); ;
            }
            if (string.IsNullOrEmpty(fuid))
            {
                return Json(new { isSuccess = false, msg = "参数为空" }, JsonRequestBehavior.AllowGet); ;
            }
            try
            {
                var result = new CommonActionResult();
                //取消关注此人0表示已关注,1表示未关注
                if (f == 0)
                {
                    result = WCFClients.ExternalClient.BDFXCancelAttention(uid, fuid);
                }//关注此人
                else if (f == 1)
                {
                    result = WCFClients.ExternalClient.BDFXAttention(uid, fuid);
                }
                return Json(new { isSuccess = result.IsSuccess, msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { isSuccess = false, msg = ex.Message });
            }
        }
        public PartialViewResult hemaiList()
        {
            try
            {
                ViewBag.User = CurrentUser;
                ViewBag.CurrentUserBalance = CurrentUserBalance;
                ViewBag.minMoney = string.IsNullOrEmpty(Request["minMoney"]) ? -1 : decimal.Parse(Request["minMoney"]);
                ViewBag.maxMoney = string.IsNullOrEmpty(Request["maxMoney"]) ? -1 : decimal.Parse(Request["maxMoney"]);
                ViewBag.minProgress = string.IsNullOrEmpty(Request["minProgress"]) ? -1 : decimal.Parse(Request["minProgress"]);
                ViewBag.maxProgress = string.IsNullOrEmpty(Request["maxProgress"]) ? -1 : decimal.Parse(Request["maxProgress"]);
                var ser = string.IsNullOrEmpty(Request["security"]) ? null : (TogetherSchemeSecurity?)int.Parse(Request["security"]);
                ViewBag.Security = ser == null ? "" : ((int)ser).ToString();
                ViewBag.pageNo = string.IsNullOrEmpty(Request.QueryString["pageNo"]) ? 0 : int.Parse(Request.QueryString["pageNo"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 12 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.orderByCategory = string.IsNullOrEmpty(Request["orderByCategory"]) ? 0 : int.Parse(Request["orderByCategory"]);
                ViewBag.orderByProperty = string.IsNullOrEmpty(Request["orderByProperty"]) ? 0 : int.Parse(Request["orderByProperty"]);
                // Sports_TogetherSchemeQueryInfoCollection togList = WCFClients.GameClient.QuerySportsTogetherList("", "", ViewBag.minMoney, ViewBag.maxMoney, (ViewBag.minProgress == -1 ? -1 : ViewBag.minProgress / 100), (ViewBag.maxProgress == -1 ? -1 : ViewBag.maxProgress / 100), ser, "", ViewBag.pageNo, ViewBag.PageSize, (QueryTogetherListOrderByProperty)ViewBag.orderByProperty, (OrderByCategory)ViewBag.orderByCategory, UserToken);
                // ViewBag.TogList = togList;
            }
            catch (Exception ex)
            {
                ViewBag.TogList = new Sports_TogetherSchemeQueryInfoCollection();
            }
            return PartialView();
        }

        public JsonResult QuickBuy(string id)
        {
            var lotteryInfos = new Dictionary<string, LotteryInfo>();
            var gameCodeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
            foreach (var gameCode in gameCodeArray)
            {
                var issuse = base.QueryCurrentIssuseByOfficialStopTime(gameCode);
                if (issuse == null) continue;

                var pre = "";
                if (issuse.GameCode == "SSQ" || issuse.GameCode == "DLT")
                    pre = BuildLastIssuseNumber(issuse.GameCode, issuse.IssuseNumber);
                lotteryInfos.Add(issuse.GameCode.ToLower(), new LotteryInfo
                {
                    issue = issuse.IssuseNumber,
                    pre = pre,
                    win = "",
                    sale = 1,
                    betTime = ((issuse.OfficialStopTime - DateTime.Now).TotalSeconds - issuse.GameDelaySecond).ToString("0"),
                    awardTime = (issuse.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0")
                });
            }
            //foreach (var item in app.lottery.site.iqucai.RedisHelper.QueryNextIssuseListByOfficialStopTime())
            //{

            //}
            //foreach (var item in base.LoadAllGameIssuse_RefreshByOfficialStopTime())
            //{
            //    var pre = "";
            //    if (item.GameCode == "SSQ" || item.GameCode == "DLT")
            //        pre = BuildLastIssuseNumber(item.GameCode, item.IssuseNumber);
            //    lotteryInfos.Add(item.GameCode.ToLower(), new LotteryInfo
            //    {
            //        issue = item.IssuseNumber,
            //        pre = pre,
            //        win = "",
            //        sale = 1,
            //        betTime = ((item.OfficialStopTime - DateTime.Now).TotalSeconds - item.GameDelaySecond).ToString("0"),
            //        awardTime = (item.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0")
            //    });
            //}

            return Json(new { data = lotteryInfos }, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult HeaderCms()
        {
            return PartialView();
        }
        public PartialViewResult FooterCms()
        {
            return PartialView();
        }

        /// <summary>
        /// 维护
        /// </summary>
        public ActionResult maintain()
        {

            return View();
        }
        public JsonResult SendExtensionMsg()
        {
            try
            {
                var phoneNumber = Request.Form["mobile"] ?? "";
                var msgTypet = Request.Form["msgTypet"] ?? "";
                var ip = IpManager.IPAddress;
                if (string.IsNullOrEmpty(phoneNumber))
                    throw new Exception("手机号码不能为空！");
                if (phoneNumber.Length != 11)
                {
                    throw new Exception("请输入11位手机号码！");
                }
                if (string.IsNullOrEmpty(msgTypet))
                    throw new Exception("传入数据异常！");
                if (string.IsNullOrEmpty(ip))
                    throw new Exception("非法IP地址！");

                var valideCode = Request["ValidCode"];
                if (string.IsNullOrEmpty(valideCode) || Session["ValidateCode"] == null)
                    throw new Exception("请输入验证码");
                if (valideCode.ToLower() != Session["ValidateCode"].ToString().ToLower())
                    throw new Exception("验证码输入错误");

                var android = WCFClients.GameClient.QueryAppConfigByAgentId("100000");
                var ios = WCFClients.GameClient.QueryAppConfigByAgentId("100001");
                var contentList = new List<string>();
                contentList.Add("欢迎您使用移动平台，");
                if (!string.IsNullOrEmpty(android.ConfigDownloadUrl))
                {
                    var androidUrl = string.Format("{0}/{1}_{2}", android.ConfigDownloadUrl, android.AgentName, android.ConfigVersion);
                    contentList.Add(string.Format("安卓下载地址{0} ", androidUrl));
                }
                if (!string.IsNullOrEmpty(ios.ConfigDownloadUrl))
                {
                    var iosUrl = ios.ConfigDownloadUrl;
                    contentList.Add(string.Format("苹果下载地址{0} ", iosUrl));
                }

                var content = string.Join("", contentList);
                var result = WCFClients.GameQueryClient.SendSMS(phoneNumber, content, ip);
                if (result.IsSuccess)
                    return Json(new { Succuss = true, Msg = "发送成功！" });
                else
                    return Json(new { Succuss = false, Msg = "发送失败！" });

            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 服务器时间
        /// </summary>
        /// <returns></returns>
        public JsonResult getServerTime()
        {
            var serverTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            return Json(new { servertime = serverTime }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 服务器时间
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServerTime2(string callback)
        {
            var serverTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            return new JsonpResult<object>(new { servertime = serverTime }, callback);
        }

        //活动手机认证6.20-7.31  发送验证码
        //public JsonResult addmobile(FormCollection postForm)
        //{
        //    try
        //    {
        //        string mobile = PreconditionAssert.IsNotEmptyString(postForm["mobile"], "手机号码不能为空。");
        //        string userName = PreconditionAssert.IsNotEmptyString(postForm["UserName"], "用户名不能为空。");
        //        PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
        //        var result = WCFClients.ExternalClient.RequestAuthenticationMobileIndex(mobile);
        //        if (result.IsSuccess)
        //        {
        //            string code = result.ReturnValue;//生成校验码

        //            #region 发送站内消息：手机短信或站内信

        //            var pList = new List<string>();
        //            pList.Add(string.Format("{0}={1}", "[ValidNumber]", code));
        //            //发送短信
        //            WCFClients.GameQueryClient.DoSendSiteMessage("", mobile, "ON_User_Bind_Mobile_Before", string.Join("|", pList.ToArray()));

        //            #endregion
        //        }
        //        else
        //        {
        //            throw new Exception(result.Message);
        //        }
        //        return Json(new { IsSucess = true, Message = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSucess = false, Message = ex.Message });
        //    }
        //}
        /// <summary>
        /// 首页活动  提交注册
        /// </summary>
        /// <returns></returns>
        public JsonResult ActivityRegist(FormCollection postForm)
        {
            var urls = Request.Url;
            ViewBag.Error = "";
            try
            {
                string userName = PreconditionAssert.IsNotEmptyString(postForm["userName"], "登录账号不能为空。");
                string passWord = PreconditionAssert.IsNotEmptyString(postForm["passWord"], "登录密码不能为空。");
                string realname = PreconditionAssert.IsNotEmptyString(postForm["realname"], "真实姓名不能为空。");
                string idcode = PreconditionAssert.IsNotEmptyString(postForm["idcode"], "身份证号码不能为空。");
                string mobile = PreconditionAssert.IsNotEmptyString(postForm["mobile"], "手机号码不能为空。");
                string auth_code = PreconditionAssert.IsNotEmptyString(postForm["auth_code"], "验证码不能为空。");
                PreconditionAssert.IsFalse(ValidateHelper.IsEmail(userName), "用户名不能是邮箱地址。");
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(idcode), "请输入正确的身份证号码。");

                RegisterInfo_Local_Index registerInfo = new RegisterInfo_Local_Index()
                {
                    RegType = "INDEX",
                    LoginName = userName,
                    Password = passWord,
                    RegisterIp = IpManager.IPAddress,
                    ReferrerUrl = urls.ToString(),
                    Realname = realname,
                    Idcode = idcode,
                    Mobile = mobile,
                    ComeFrom = "INDEX",
                };

                var result = WCFClients.ExternalClient.RegisterLoacal_Index(registerInfo, auth_code);
                if (result.IsSuccess)
                {
                    var loginInfo = WCFClients.ExternalClient.LoginLocal(userName, passWord, IpManager.IPAddress);
                    if (loginInfo.IsSuccess)
                    {
                        //CurrentUser = new CurrentUserInfo() { LoginInfo = loginInfo };
                    }
                }
                return Json(new { IsSucess = true, Message = "注册成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Message = ex.Message });
            }
        }

        //查参加活动条件
        public JsonResult QueryActivityCondition()
        {
            try
            {
                var userId = CurrentUser.LoginInfo.UserId;
                var userRegist = WCFClients.ExternalClient.QueryUserRegisterById(userId);
                if (!userRegist)
                {
                    throw new Exception("尊敬的用户，您不具备参加该活动的条件！");
                }
                return Json(new { IsSucess = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Message = ex.Message });
            }
        }

        public JsonResult QueryCardByIdCard(FormCollection postForm)
        {
            try
            {
                string idcode = PreconditionAssert.IsNotEmptyString(postForm["idcode"], "身份证号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(idcode), "请输入正确的身份证号码。");
                //var IsIdCard = WCFClients.ExternalClient.QueryCardByIdCard(idcode);
                //if (!IsIdCard)
                //{
                //    throw new Exception("身份证号码不可用");
                //}
                //return Json(new { IsSucess = true, Message = "" });
                return Json(new { IsSucess = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Message = ex.Message });
            }
        }
        public JsonResult QueryMobileByMobile(FormCollection postForm)
        {
            try
            {
                string Mobile = PreconditionAssert.IsNotEmptyString(postForm["Mobile"], "手机号码不能为空。");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(Mobile), "手机号码格式错误。");
                var IsMobile = WCFClients.ExternalClient.QueryMobileByMobile(Mobile);
                if (!IsMobile)
                {
                    throw new Exception("手机号码不可用");
                }
                return Json(new { IsSucess = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Message = ex.Message });
            }
        }


        public ContentResult testIp()
        {
            var list = new List<string>();
            //var ip = IPAddress;
            //var name = Common.Net.IpManager.GetIpDisplayname_Sina(ip);
            //list.Add(string.Format("ip:{0}  name:{1}", ip, name));

            //var ip2 = Common.Net.IpManager.IPAddress;
            //var name2 = Common.Net.IpManager.GetIpDisplayname_Sina(ip2);
            //list.Add(string.Format("ip:{0}  name:{1}", ip2, name2));

            //var x_forwarded = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //list.Add(string.Format("x_forwarded:{0}", x_forwarded));
            //var remote = Request.ServerVariables["REMOTE_ADDR"];
            //list.Add(string.Format("remote:{0}", remote));

            foreach (var item in Request.ServerVariables)
            {
                var key = item.ToString();
                var v = Request.ServerVariables[key];
                list.Add(string.Format("Key->{0}===>value->{1}", key, v));
            }
            string cnd_src_ip = Request.Headers["Cdn-Src-Ip"];
            list.Add(string.Format("Key->{0}===>value->{1}", "Cdn-Src-Ip", cnd_src_ip));


            String srcIp = Request.Headers["Cdn-Src-Ip"];
            if (srcIp == null)
            {
                srcIp = Request.UserHostAddress;
            }
            list.Add("通过 Cdn-Src-Ip 方式取到的是 " + srcIp);

            return Content(string.Join("<br/>", list.ToArray()));
        }


        private bool isIp(string ip)
        {
            string regexIp = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
            Regex ipReg = new Regex(regexIp);
            return ipReg.IsMatch(ip);
        }


        public string IPAddress
        {
            get
            {
                var x_forwarded = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                var remote = Request.ServerVariables["REMOTE_ADDR"];
                string result = string.Empty;
                if (!string.IsNullOrEmpty(x_forwarded))
                {
                    result = x_forwarded;
                    //可能有代理
                    if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式
                        result = string.Empty;
                    else
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。
                            result = result.Replace(" ", "").Replace("'", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (ValidateHelper.IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 6) != "172.16.")
                                {
                                    return temparyip[i];    //找到不是内网的地址
                                }
                            }
                        }

                        else if (ValidateHelper.IsIPAddress(result)) //代理即是IP格式 ,IsIPAddress判断是否是IP的方法,
                            return result;
                        else
                            result = string.Empty;    //代理中的内容 非IP，取IP
                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = remote;
                if (string.IsNullOrEmpty(result))
                    result = Request.UserHostAddress;
                if (string.IsNullOrEmpty(result) || result == "::1")
                    result = "127.0.0.1";

                return result;
            }
        }

        public ContentResult testip3()
        {
            var list = new List<string>();
            var Cdn_Src_Ip = Request.Headers["Cdn-Src-Ip"];
            var HTTP_X_Real_IP = Request.ServerVariables["HTTP_X_REAL_IP"];
            var HTTP_X_FORWARDED_FOR = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            list.Add(string.Format("HTTP_X_Real_IP:{0}", HTTP_X_Real_IP));
            list.Add(string.Format("HTTP_X_FORWARDED_FOR:{0}", HTTP_X_FORWARDED_FOR));
            list.Add(string.Format("IPAddress:{0}", Common.Net.IpManager.IPAddress));
            list.Add(string.Format("Cdn-Src-Ip:{0}", Cdn_Src_Ip));

            //Proxy-Client-IP
            //WL-Proxy-Client-IP
            //HTTP_CLIENT_IP

            return Content(string.Join("|", list.ToArray()));
        }


        public ContentResult testip2()
        {
            return Content(GetIp());
        }


        public ContentResult gethtml()
        {
            var myCode = Common.Cryptography.Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", "917"), System.Text.Encoding.UTF8);
            return Content(myCode);
        }

        /// <summary>
        /// 获取IP和地区详细
        /// </summary>
        /// <returns></returns>

        private string GetIp()
        {
            try
            {
                string url = "http://www.ip138.com/ips138.asp";
                string regStr = "(?<=<td\\s*align=\\\"center\\\">)[^<]*?(?=<br/><br/></td>)";
                string ipRegStr = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
                string ip = string.Empty;
                string html = GetHtml(url);
                Regex reg = new Regex(regStr, RegexOptions.None);
                Match ma = reg.Match(html);
                string context = ma.Value;
                reg = new Regex(ipRegStr, RegexOptions.None);
                ma = reg.Match(context);
                ip = ma.Value;
                return ip;
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// 获取网页HTML源码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHtml(string url)
        {
            Uri uri = new Uri(url);
            System.Net.WebRequest wr = System.Net.WebRequest.Create(uri);
            Stream s = wr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.Default);
            return sr.ReadToEnd();
        }

    }


    public class JsonpResult<T> : ActionResult
    {
        public T Obj { get; set; }
        public string CallbackName { get; set; }

        public JsonpResult(T obj, string callback)
        {
            this.Obj = obj;
            this.CallbackName = callback;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            var jsonp = this.CallbackName + "(" + js.Serialize(this.Obj) + ")";

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(jsonp);
        }
    }
}
