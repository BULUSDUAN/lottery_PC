using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using GameBiz.Client;
using GameBiz.Core;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Xml;
using Common.Net;
using Common.Log;
using Common.Cryptography;
using System.Collections;
using System.IO;
using Common.Net.SMS;
using Common.Utilities;
using Common.XmlAnalyzer;
using External.Core.Login;
using External.Core.Authentication;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Common.Gateway.YiJiFu;
using app.lottery.site.Models;
using External.Core;
using Common.JSON;
using System.Globalization;
using app.lottery.site.iqucai;
using Common.Lottery.Redis;

namespace app.lottery.site.Controllers
{
    [CCFilter]
    [ErrorHandle]
    public class BaseController : Controller
    {

        #region Action执行前判断
        /// <summary>
        /// Action执行前判断
        /// </summary>
        /// <param name="filterContext"></param>
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string msg;
        //    string contextController;
        //    string contextActionName;

        //    contextController = filterContext.Controller.ToString();
        //    contextActionName = filterContext.ActionDescriptor.ActionName;

        //    StartProcessRequest(filterContext);
           
        //    base.OnActionExecuting(filterContext);
        //}

        #region SQL注入式攻击代码分析
        /**/
        /// <summary>
        /// 处理用户提交的请求
        /// </summary>
        public void StartProcessRequest(ActionExecutingContext filterContext)
        {

           
            try
            {
                string getkeys = "";
                if (System.Web.HttpContext.Current.Request.QueryString != null)
                {

                    for (int i = 0; i < System.Web.HttpContext.Current.Request.QueryString.Count; i++)
                    {
                        getkeys = System.Web.HttpContext.Current.Request.QueryString.Keys[i];
                        if (!ProcessSqlStr(System.Web.HttpContext.Current.Request.QueryString[getkeys], 0))
                        {
                            filterContext.Result = Content(string.Concat("<script>", string.Format("alert(\"请勿非法提交1\");;history.back();"), "</script>"), "text/html");
                            return;
                        }
                        else if (!isLegalNumber(System.Web.HttpContext.Current.Request.QueryString[getkeys]))
                        {
                            filterContext.Result = Content(string.Concat("<script>", string.Format("alert(\"请勿非法提交2\");;history.back();"), "</script>"), "text/html");
                            return;

                        }
                    }
                }
                if (System.Web.HttpContext.Current.Request.Form != null)
                {
                    for (int i = 0; i < System.Web.HttpContext.Current.Request.Form.Count; i++)
                    {
                        getkeys = System.Web.HttpContext.Current.Request.Form.Keys[i];
                        if (!ProcessSqlStr(System.Web.HttpContext.Current.Request.Form[getkeys], 1))
                        {
                            filterContext.Result = Content(string.Concat("<script>", string.Format("alert(\"请勿非法提交3\");;history.back();"), "</script>"), "text/html");
                            return;
                        }
                        else if (!isLegalNumber(System.Web.HttpContext.Current.Request.Form[getkeys]))
                        {
                            filterContext.Result = Content(string.Concat("<script>", string.Format("alert(\"请勿非法提交4\");;history.back();"), "</script>"), "text/html");
                            return;
                        }
                    }
                }
            }
            catch
            {
                // 错误处理: 处理用户提交信息!
            }
        }

        /**/
        /// <summary>
        /// 分析用户请求是否正常
        /// </summary>
        /// <param name="Str">传入用户提交数据</param>
        /// <returns>返回是否含有SQL注入式攻击代码</returns>
        private bool ProcessSqlStr(string Str, int type)
        {
            string SqlStr;

            if (type == 1)
                SqlStr = "exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
            else
                SqlStr = "'|and|exec|insert|select|delete|update|count|*|chr|mid|master|truncate|char|declare";

            bool ReturnValue = true;
            try
            {
                if (Str != "")
                {
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }
        #endregion

        #region 特殊字符
        /// <summary>
        /// 判断是否是非法字符
        /// </summary>
        /// <param name="str">判断是字符</param>
        /// <returns></returns>
        public Boolean isLegalNumber(string str)
        {
            char[] charStr = str.ToLower().ToCharArray();
            for (int i = 0; i < charStr.Length; i++)
            {
                int num = Convert.ToInt32(charStr[i]);
                if (!(IsChineseLetter(num)|| (num >= 48 && num <= 57) || (num >= 97 && num <= 123) || (num >= 65 && num <= 90) || num == 45))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 判断字符的Unicode值是否是汉字
        /// </summary>
        /// <param name="code">字符的Unicode</param>
        /// <returns></returns>
        protected bool IsChineseLetter(int code)
        {
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);

            if (code >= chfrom && code <= chend)
            {
                return true;     //当code在中文范围内返回true

            }
            else
            {
                return false;    //当code不在中文范围内返回false
            }

            return false;
        }
        #endregion
        #endregion



        #region 当前用户相关
        private static string _guestToken = "";
        //用户口令
        public string UserToken
        {
            get
            {
                if (Session["CurrentUser"] == null)
                {
                    try
                    {
                        _guestToken = string.IsNullOrEmpty(_guestToken) ? WCFClients.ExternalClient.GetGuestToken().ReturnValue : _guestToken;
                    }
                    catch
                    {
                        _guestToken = "";
                    }
                    return _guestToken;
                }
                return (Session["CurrentUser"] as LoginInfo).UserToken;
            }
        }

        public string CurrentUserId
        {
            get
            {
                if (Session["CurrentUser"] == null)
                {
                    return string.Empty;
                }
                return (Session["CurrentUser"] as LoginInfo).UserId;
            }
        }

        /// <summary>
        /// 当前用户对象，在Models里定义好属性，再在这里赋值
        /// </summary>
        protected CurrentUserInfo CurrentUser
        {
            get
            {
                if (Session["CurrentUserInfo"] == null)
                {
                    return null;
                }
                //user.UserBalance = UserBalance;
                //Session["CurrentUserInfo"] = user;
                var info = Session["CurrentUserInfo"] as CurrentUserInfo;
                //info.UserBalance = UserBalance;
                return info;
                //return Session["CurrentUserInfo"] == null ? null : Session["CurrentUserInfo"] as CurrentUserInfo;
            }
            set
            {
                if (value == null)
                {
                    Session["CurrentUser"] = null;
                    Session["CurrentUserInfo"] = null;
                    Session["MemeberInfo"] = null;//登录个人中心信息
                    Session["VipInfo"] = null; //vip登录信息
                    return;
                }
                Session["CurrentUser"] = value.LoginInfo;

                DelCacheRealNameInfo();
                DelCacheMobileInfo();
                DelCacheEmailInfo();
                DelCacheUBankCardInfo();
                DelCacheUserBalance();

                var cui = new CurrentUserInfo();
                cui.LoginInfo = value.LoginInfo;

                Session["CurrentUserInfo"] = cui;
            }
        }

        //我的成长状态
        protected int GrowthStatus(int userGrowth)
        {
            var level = 0;
            //成长状态
            if (userGrowth < 500)
                level = 1;
            else if (userGrowth >= 500 && userGrowth < 1000)
                level = 2;
            else if (userGrowth >= 1000 && userGrowth < 2000)
                level = 3;
            else if (userGrowth >= 2000 && userGrowth < 4000)
                level = 4;
            else if (userGrowth >= 4000 && userGrowth < 8000)
                level = 5;
            else if (userGrowth >= 8000 && userGrowth < 12000)
                level = 6;
            else if (userGrowth >= 12000 && userGrowth < 16000)
                level = 7;
            else if (userGrowth >= 16000 && userGrowth < 20000)
                level = 8;
            else if (userGrowth >= 20000)
                level = 9;
            return level;
        }

        protected int AccountLevel(CurrentUserInfo cuser)
        {
            //帐户安全级别
            var level = 0;
            if (CurrentUserBalance.IsSetPwd) { level++; }
            if (cuser.IsAuthenticationRealName) { level++; }
            if (cuser.IsAuthenticationMobile) { level++; }
            if (cuser.IsAuthenticationEmail && CurrentUser.IsBindBank) { level++; }
            return level;
        }

        /// <summary>
        /// 用户最后一次登录信息
        /// </summary>
        protected UserLoginHistoryInfo lastLoginInfo
        {
            get
            {
                try
                {
                    return WCFClients.GameClient.QueryCache_UserLastLoginInfo(UserToken);
                }
                catch (Exception ex)
                {
                    return new UserLoginHistoryInfo();
                }
            }
        }
        /// <summary>
        /// 完成任务后重新加载等级
        /// </summary>
        protected void LoadUserLeve(string flag = "")
        {
            var user = CurrentUser;
            if (user == null) return;

            LoginInfo loginInfo = WCFClients.ExternalClient.LoginByUserId(CurrentUser.LoginInfo.UserId);
            user.LoginInfo = loginInfo;

            switch (flag)
            {
                case "mobile":
                    user.MobileInfo = MobileInfo;
                    user.IsAuthenticationMobile = user.MobileInfo != null;
                    break;
                case "email":
                    user.EmailInfo = EmailInfo;
                    user.IsAuthenticationEmail = user.EmailInfo != null;
                    break;
                case "bankcard":
                    user.BankCardInfo = BankCardInfo;
                    user.IsBindBank = user.BankCardInfo != null;
                    break;
                case "realname":
                    user.RealNameInfo = RealNameInfo;
                    user.IsAuthenticationRealName = user.RealNameInfo != null;
                    break;
            }
            Session["CurrentUserInfo"] = user;
        }
        /// <summary>
        /// 购买后重新加载余额
        /// </summary>
        protected void LoadUerBalance()
        {
            //var user = CurrentUser;
            //if (user == null) return;

            //user.UserBalance = UserBalance;
            //Session["CurrentUserInfo"] = user;
        }

        //查询认证相关信息
        protected void LoadMemberInfo()
        {
            if (Session["MemeberInfo"] == null)
            {
                var cuser = CurrentUser;
                var info = WCFClients.ExternalClient.QueryUserBindInfos(CurrentUser.LoginInfo.UserId);
                if (info != null && cuser != null)
                {
                    cuser.LoginInfo.MaxLevelName = info.MaxLevelName;
                    cuser.LoginInfo.IsRebate = info.RebateCount > 0;
                    cuser.LoginInfo.IsAgent = info.IsAgent;
                    cuser.LoginInfo.IsUserType = info.IsUserType == 1 ? true : false;
                    cuser.RealNameInfo = new UserRealNameInfo { RealName = info.RealName, CardType = info.CardType, IdCardNumber = info.IdCardNumber };
                    cuser.MobileInfo = new UserMobileInfo { Mobile = info.Mobile };
                    cuser.EmailInfo = new UserEmailInfo { Email = info.Email };
                    cuser.BankCardInfo = new BankCardInfo
                    {
                        RealName = info.BankCardRealName,
                        ProvinceName = info.ProvinceName,
                        CityName = info.CityName,
                        BankName = info.BankName,
                        BankSubName = info.BankSubName,
                        BankCardNumber = info.BankCardNumber
                    };
                    cuser.IsAuthenticationRealName = !string.IsNullOrEmpty(info.RealName);
                    cuser.IsAuthenticationMobile = !string.IsNullOrEmpty(info.Mobile) && info.IsSettedMobile;
                    cuser.IsAuthenticationEmail = !string.IsNullOrEmpty(info.Email);
                    cuser.IsBindBank = !string.IsNullOrEmpty(info.BankCardNumber);
                    cuser.QQNumber = info.QQ;
                    cuser.AlipayInfo = info.AlipayAccount;
                    cuser.LastLoginInfo = new UserLoginHistoryInfo
                    {
                        //LoginFrom = info.LastLoginFrom,
                        //LoginIp = info.LastLoginIp,
                        //LoginTime = Convert.ToDateTime(info.LastLoginTime)
                    };

                    Session["CurrentUserInfo"] = cuser;
                    Session["MemeberInfo"] = 1;
                }
            }
        }

        #region 实名认证

        /// <summary>
        /// 是否已认证真实姓名
        /// </summary>
        private bool _isAuthenticationRealName;
        protected bool IsAuthenticationRealName
        {
            get
            {
                try
                {
                    _isAuthenticationRealName = WCFClients.ExternalClient.CheckIsAuthenticatedMyRealName(UserToken);
                }
                catch
                {
                    _isAuthenticationRealName = false;
                }
                return _isAuthenticationRealName;
            }
            set { _isAuthenticationRealName = value; }
        }

        /// <summary>
        /// 获取用户实名认证信息
        /// </summary>
        private UserRealNameInfo _realNameInfo;
        protected UserRealNameInfo RealNameInfo
        {
            get
            {
                try
                {
                    _realNameInfo = WCFClients.ExternalClient.GetMyRealNameInfo(UserToken);
                }
                catch
                {
                    _realNameInfo = new UserRealNameInfo();
                }
                return _realNameInfo;
            }
            set
            {
                _realNameInfo = value;
            }
        }

        /// <summary>
        /// 清除用户实名认证信息缓存
        /// </summary>
        public void DelCacheRealNameInfo()
        {
            //IsAuthenticationRealName = WCFClients.ExternalClient.CheckIsAuthenticatedMyRealName(UserToken);
            RealNameInfo = null;
        }

        #endregion

        #region 手机绑定

        /// <summary>
        /// 是否已手机认证
        /// </summary>
        private bool _isAuthenticationMobile;
        protected bool IsAuthenticationMobile
        {
            get
            {
                try
                {
                    _isAuthenticationMobile = WCFClients.ExternalClient.CheckIsAuthenticatedMyMobile(UserToken);
                }
                catch
                {
                    _isAuthenticationMobile = false;
                }
                return _isAuthenticationMobile;
            }
            set { _isAuthenticationMobile = value; }
        }

        /// <summary>
        /// 获取用户手机认证信息
        /// </summary>
        private UserMobileInfo _userMobile;
        protected UserMobileInfo MobileInfo
        {
            get
            {
                try
                {
                    _userMobile = WCFClients.ExternalClient.GetMyMobileInfo(UserToken);
                }
                catch
                {
                    _userMobile = new UserMobileInfo();
                }
                return _userMobile;
            }
            set
            {
                _userMobile = value;
            }
        }

        /// <summary>
        /// 清除用户手机认证信息缓存
        /// </summary>
        public void DelCacheMobileInfo()
        {
            //IsAuthenticationMobile = WCFClients.ExternalClient.CheckIsAuthenticatedMyMobile(UserToken);
            MobileInfo = null;
        }
        #endregion

        #region 邮箱认证

        /// <summary>
        /// 是否已认证邮箱
        /// </summary>
        private bool _isAuthenticationEmail;
        protected bool IsAuthenticationEmail
        {
            get
            {
                try
                {
                    _isAuthenticationEmail = WCFClients.ExternalClient.CheckIsAuthenticatedMyEmail(UserToken);

                }
                catch
                {
                    _isAuthenticationEmail = false;
                }
                return _isAuthenticationEmail;
            }
            set
            {
                _isAuthenticationEmail = value;
            }
        }

        /// <summary>
        /// 获取用户邮箱认证信息
        /// </summary>
        private UserEmailInfo _emailInfo;
        protected UserEmailInfo EmailInfo
        {
            get
            {
                try
                {
                    _emailInfo = WCFClients.ExternalClient.GetMyEmailInfo(UserToken);
                }
                catch
                {
                    _emailInfo = new UserEmailInfo();
                }
                return _emailInfo;
            }
            set
            {
                _emailInfo = value;
            }
        }

        /// <summary>
        /// 清除用户邮箱认证信息缓存
        /// </summary>
        public void DelCacheEmailInfo()
        {
            //IsAuthenticationEmail = WCFClients.ExternalClient.CheckIsAuthenticatedMyEmail(UserToken);
            EmailInfo = null;
        }
        #endregion

        #region 银行卡信息

        /// <summary>
        /// 是否已绑定银行卡
        /// </summary>
        private bool _isBindBank;
        protected bool IsBindBank
        {
            get
            {
                try
                {
                    var bankInfo = WCFClients.GameFundClient.QueryBankCard(UserToken);
                    _isBindBank = !string.IsNullOrEmpty(bankInfo.UserId);
                }
                catch
                {
                    _isBindBank = false;
                }
                return _isBindBank;
            }
            set
            {
                _isBindBank = value;
            }
        }

        /// <summary>
        /// 用户绑定的银行卡信息
        /// </summary>
        private BankCardInfo _bankCardInfo;
        protected BankCardInfo BankCardInfo
        {
            get
            {
                try
                {
                    _bankCardInfo = WCFClients.GameFundClient.QueryBankCard(UserToken);
                }
                catch
                {
                    _bankCardInfo = null;
                }
                return _bankCardInfo;
            }
            set
            {
                _bankCardInfo = value;
            }
        }

        /// <summary>
        /// 清除用户银行卡信息缓存
        /// </summary>
        public void DelCacheUBankCardInfo()
        {
            //Session["IsBindBank"] = null;
            BankCardInfo = null;
        }
        #endregion


        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        public void ClearUserBindInfoCache(string userId)
        {
            try
            {
                BankCardInfo = null;
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelper.DB_UserBindData;
                db.KeyDeleteAsync(fullKey);
            }
            catch (Exception)
            {
            }
        }

        #region 余额、彩豆余额

        /// <summary>
        /// 用户余额
        /// </summary>
        //private UserBalanceInfo _userBalance;
        protected UserBalanceInfo CurrentUserBalance
        {
            get
            {
                try
                {
                    return WebRedisHelper.QueryUserBalance(CurrentUserId);
                    //if (IsBetOrderToRedisList)
                    //{
                    //    return WebRedisHelper.QueryUserBalance(CurrentUserId);
                    //}
                    //else
                    //{
                    //    return WCFClients.GameFundClient.QueryMyBalance(UserToken);
                    //}
                }
                catch (Exception)
                {
                    //_userBalance = new UserBalanceInfo();
                    return new UserBalanceInfo(); ;
                }
                //return _userBalance;
            }
            //set
            //{
            //    _userBalance = value;
            //}
        }

        /// <summary>
        /// 清除用户余额缓存
        /// </summary>
        public void DelCacheUserBalance()
        {
            //UserBalance = null;
        }

        #endregion

        #endregion

        #region 系统配置信息

        /// <summary>
        /// 支付验证字符串
        /// </summary>
        public string payVerifyString
        {
            get { return "xinticaipay"; }
        }

        public bool IsTest
        {
            get
            {
                return Common.Utilities.UsefullHelper.IsInTest;
            }
        }

        /// <summary>
        /// 当前站点路径
        /// </summary>
        public string SiteRoot
        {
            get
            {
                return Common.Net.UrlHelper.GetSiteRoot(Request);
            }
        }

        /// <summary>
        /// 获取提现接口名称
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static string GetWithdrawAgentName(WithdrawAgentType agent)
        {
            switch (agent)
            {
                case WithdrawAgentType.Alipay:
                    return "支付宝";
                case WithdrawAgentType.BankCard:
                    return "银行卡";
                case WithdrawAgentType.Yeepay:
                    return "易宝";
                default: return agent.ToString();
            }
        }

        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public bool IsBrowserGet
        {
            get
            {
                string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox", "safari", "chrome" };
                string curBrowser = Request.Browser.Type.ToLower();
                for (int i = 0; i < BrowserName.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_ACCEPT"]) && curBrowser.IndexOf(BrowserName[i]) >= 0) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 判断当前访问是否来自Wap端浏览器
        /// </summary>
        /// <returns>当前访问是否来自Wap端,true表示是wap浏览器访问，false为PC浏览器访问</returns>
        public bool IsWapBrowser
        {
            get
            {
                string osPat = "mozilla|m3gate|winwap|openwave|Windows NT|Windows 3.1|95|Blackcomb|98|ME|X Window|Longhorn|ubuntu|AIX|Linux|AmigaOS|BEOS|HP-UX|OpenBSD|FreeBSD|NetBSD|OS/2|OSF1|SUN";

                string uAgent = Request.ServerVariables["HTTP_USER_AGENT"];

                Regex reg = new Regex(osPat);

                return !reg.IsMatch(uAgent);
            }
        }

        #endregion

        #region 彩种相关信息

        private static Dictionary<string, Dictionary<string, string>> _gameTypeList = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 彩种玩法信息
        /// </summary>
        public Dictionary<string, string> GameTypeList(string gameCode)
        {
            if (!_gameTypeList.ContainsKey(gameCode))
            {
                Dictionary<string, string> tmpDic = new Dictionary<string, string>();
                var gameTypeCollection = WCFClients.GameIssuseClient.QueryGameTypeList(gameCode, UserToken);
                foreach (var item in gameTypeCollection)
                {
                    tmpDic.Add(item.GameType, item.DisplayName);
                }
                _gameTypeList.Add(gameCode, tmpDic);
            }
            return _gameTypeList[gameCode];
        }

        private static GameInfoCollection _gameList = null;
        /// <summary>
        /// 彩种列表-服务器获取列表
        /// </summary>
        /// <returns></returns>
        public GameInfoCollection GameList
        {
            get
            {
                try
                {
                    if (_gameList == null)
                    {
                        _gameList = WCFClients.GameIssuseClient.QueryGameList(UserToken);
                    }
                }
                catch
                {
                    _gameList = new GameInfoCollection();
                }
                return _gameList;
            }
        }

        //获取对应彩种的最大期数
        protected int MaxIssuseCount(string gameCode)
        {
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                    return 120;
                case "JXSSC":
                    return 84;
                case "JX11X5":
                    return 84;
                case "SD11X5":
                    return 87;
                case "GD11X5":
                    return 84;
                case "FC3D":
                case "PL3":
                case "PL5":
                    return 358;
                case "SDQYH":
                    return 40;
                case "GDKLSF":
                    return 84;
                case "GXKLSF":
                    return 50;
                case "SSQ":
                case "DLT":
                    return 156;
                case "JSKS":
                    return 82;
                case "CTZQ":
                    return 50;
                case "SDKLPK3":
                    return 88;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 日期当天是否有奖期数据 （春节无奖期）
        /// </summary>
        private bool CheckIsOpenDay(DateTime date)
        {
            var calendar = new ChineseLunisolarCalendar();
            var lMonth = calendar.GetMonth(date);
            var lDay = calendar.GetDayOfMonth(date);
            if (lMonth == 1 && lDay < 7)
            {
                return false;
            }
            else
            {
                date = date.AddDays(1);
                lMonth = calendar.GetMonth(date);
                lDay = calendar.GetDayOfMonth(date);
                if (lMonth == 1 && lDay == 1)
                {
                    return false;
                }
            }
            return true;
        }

        //返回上一期期号，包含所有彩种
        protected string BuildLastIssuseNumber(string gameName, string currentIssuseNumber)
        {
            int maxIssuseCount = MaxIssuseCount(gameName);
            string[] issuseNumberArray = currentIssuseNumber.Split('-');
            int index = int.Parse(issuseNumberArray[1]);
            DateTime dtPart = DateTime.Now;
            switch (gameName.ToUpper())
            {
                case "CQSSC":
                case "JXSSC":
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                case "SDQYH":
                case "GDKLSF":
                case "JSKS":
                    dtPart = DateTime.ParseExact(issuseNumberArray[0], "yyyyMMdd", null);
                    if (index == 1)
                    {
                        dtPart = dtPart.AddDays(-1);
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }

                    //屏蔽春节7天奖期数据
                    if (!CheckIsOpenDay(dtPart))
                    {
                        dtPart = dtPart.AddDays(-1);
                    }
                    //while (dtPart >= minDate && dtPart <= maxDate)
                    //{
                    //    dtPart = dtPart.AddDays(-1);
                    //}

                    return string.Format("{0}-{1}", dtPart.ToString("yyyyMMdd"), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                case "GXKLSF":
                    int fech = int.Parse(issuseNumberArray[0]);
                    if (index == 1)
                    {
                        fech = fech - 1;
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }
                    return string.Format("{0}-{1}", fech.ToString(), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                case "FC3D":
                case "PL3":
                case "SSQ":
                case "DLT":
                    dtPart = DateTime.ParseExact(issuseNumberArray[0], "yyyy", null);
                    if (index == 1)
                    {
                        dtPart = dtPart.AddYears(-1);
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }
                    return string.Format("{0}-{1}", dtPart.ToString("yyyy"), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                default:
                    return currentIssuseNumber;
            }
        }

        /// <summary>
        /// 返回对应奖期的开奖号码
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="issuse">奖期</param>
        /// <returns>开奖号码</returns>
        protected string GetIssuseWinNumber(string gameCode, string issuse)
        {
            var logWriter = LogWriterGetter.GetLogWriter();

            try
            {
                var winNumberInfo = WCFClients.GameIssuseClient.QueryWinNumber(gameCode, issuse);
                if (winNumberInfo != null && !string.IsNullOrEmpty(winNumberInfo.WinNumber))
                {
                    return winNumberInfo.WinNumber;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                logWriter.Write("GetWinNumber", "GetLastWinNumber", ex);
                return string.Empty;
            }
        }

        private static Dictionary<string, Dictionary<string, string>> _hotLoseList = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 彩种遗漏冷热信息
        /// </summary>
        public Dictionary<string, string> HotLoseList(string gameCode)
        {
            var cur = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(gameCode);
            if (_hotLoseList.ContainsKey(gameCode))
            {
                if (!_hotLoseList[gameCode].ContainsKey("CurIssuse") || _hotLoseList[gameCode]["CurIssuse"] != cur.IssuseNumber)
                {
                    _hotLoseList.Remove(gameCode);
                }
                else
                {
                    return _hotLoseList[gameCode];
                }

            }
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("CurIssuse", cur.IssuseNumber);
            switch (gameCode.ToLower())
            {
                case "sd11x5":
                case "gd11x5":
                case "jx11x5":
                    #region 11选5
                    {
                        //var zxLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "zx", UserToken);
                        //var rxLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "rx", UserToken);

                        //var zxHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "zx", 20, UserToken);
                        //var rxHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "rx", 20, UserToken);
                        //ret.Add("ZXLose", zxLose);
                        //ret.Add("ZXHot", zxHot);
                        //ret.Add("RXLose", rxLose);
                        //ret.Add("RXHot", rxHot);
                    }
                    #endregion
                    break;
                case "cqssc":
                case "jxssc":
                    #region 时时彩
                    {
                        //var dxLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "dx", UserToken);
                        //var dxdsLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "dxds", UserToken);
                        //var zx2xLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "2xzx", UserToken);
                        //var zx3xLose = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "3xzx", UserToken);
                        //var dz3x = WCFClients.LotteryDataClient.GetLastLostDataString(gameCode, "3xdz", UserToken);

                        //ret.Add("DXLose", dxLose);
                        //ret.Add("DXDSLose", dxdsLose);
                        //ret.Add("ZX2XLose", zx2xLose);
                        //ret.Add("ZX3XLose", zx3xLose);
                        //ret.Add("DZ3X", dz3x);

                        //var dxHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "dx", 20, UserToken);
                        //var dxdsHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "dxds", 20, UserToken);
                        //var zx2xHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "2xzx", 20, UserToken);
                        //var zx3xHot = WCFClients.LotteryDataClient.GetHotDataString(gameCode, "3xzx", 20, UserToken);

                        //ret.Add("DXHot", dxHot);
                        //ret.Add("DXDSHot", dxdsHot);
                        //ret.Add("ZX2XHot", zx2xHot);
                        //ret.Add("ZX3XHot", zx3xHot);
                    }
                    #endregion
                    break;
            }

            #region old
            //switch (gameCode)
            //{
            //    case "SD11X5":
            //        #region 老11选5
            //        {
            //            var zxLose = WCFClients.ExternalClient.Get_SD11X5_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_SD11X5_ZX_Hot(10, UserToken);
            //            var rxLose = WCFClients.ExternalClient.Get_SD11X5_RX_Lose(UserToken);
            //            var rxHot = WCFClients.ExternalClient.Get_SD11X5_RX_Hot(10, UserToken);
            //            ret.Add("ZXLose", zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "2").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "3").LoseNumbers);
            //            ret.Add("ZXHot", zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "2").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "3").HotNumbers);
            //            ret.Add("RXLose", rxLose.LoseNumbers);
            //            ret.Add("RXHot", rxHot.HotNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "GD11X5":
            //        #region 新11选5
            //        {
            //            var zxLose = WCFClients.ExternalClient.Get_GD11X5_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_GD11X5_ZX_Hot(10, UserToken);
            //            var rxLose = WCFClients.ExternalClient.Get_GD11X5_RX_Lose(UserToken);
            //            var rxHot = WCFClients.ExternalClient.Get_GD11X5_RX_Hot(10, UserToken);
            //            ret.Add("ZXLose", zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "2").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "3").LoseNumbers);
            //            ret.Add("ZXHot", zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "2").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "3").HotNumbers);
            //            ret.Add("RXLose", rxLose.LoseNumbers);
            //            ret.Add("RXHot", rxHot.HotNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "JX11X5":
            //        #region 11选5
            //        {
            //            var zxLose = WCFClients.ExternalClient.Get_JX11X5_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_JX11X5_ZX_Hot(10, UserToken);
            //            var rxLose = WCFClients.ExternalClient.Get_JX11X5_RX_Lose(UserToken);
            //            var rxHot = WCFClients.ExternalClient.Get_JX11X5_RX_Hot(10, UserToken);
            //            ret.Add("ZXLose", zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "2").LoseNumbers + "#" + zxLose.FirstOrDefault(i => i.NumberPlace == "3").LoseNumbers);
            //            ret.Add("ZXHot", zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "2").HotNumbers + "#" + zxHot.FirstOrDefault(i => i.NumberPlace == "3").HotNumbers);
            //            ret.Add("RXLose", rxLose.LoseNumbers);
            //            ret.Add("RXHot", rxHot.HotNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "CQSSC":
            //        #region 时时彩
            //        {
            //            var dbLose = WCFClients.ExternalClient.Get_CQSSC_Double_Lose(UserToken);
            //            var zxLose = WCFClients.ExternalClient.Get_CQSSC_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_CQSSC_Hot(10, UserToken);
            //            var dsLose = WCFClients.ExternalClient.Get_CQSSC_DXDS_Lose(UserToken);
            //            var dsHot = WCFClients.ExternalClient.Get_CQSSC_DXDS_Hot(10, UserToken);
            //            var hz2xLose = WCFClients.ExternalClient.Get_CQSSC_2XHZ_Lose(UserToken);
            //            //var hz2xProb = WCFClients.ExternalClient.Get_CQSSC_2XHZ_Probability(10, UserToken);
            //            var hz3xLose = WCFClients.ExternalClient.Get_CQSSC_3XHZ_Lose(UserToken);
            //            //var hz3xProb = WCFClients.ExternalClient.Get_CQSSC_3XHZ_Probability(10, UserToken);

            //            ret.Add("DBLose", dbLose.LoseNumbers);

            //            var tmpzxLose = new List<string>();
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10000").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1000").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "100").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("ZXLose", string.Join("#", tmpzxLose));

            //            var tmpzxHot = new List<string>();
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10000").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1000").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "100").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("ZXHot", string.Join("#", tmpzxHot));

            //            var tmpdsLose = new List<string>();
            //            tmpdsLose.Add(dsLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpdsLose.Add(dsLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("DSLose", string.Join("#", tmpdsLose));

            //            var tmpdsHot = new List<string>();
            //            tmpdsHot.Add(dsHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpdsHot.Add(dsHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("DSHot", string.Join("#", tmpdsHot));

            //            ret.Add("HZ2XLose", hz2xLose.LoseNumbers);
            //            //ret.Add("HZ2XProb", hz2xProb.Probability);
            //            ret.Add("HZ3XLose", hz3xLose.LoseNumbers);
            //            //ret.Add("HZ3XProb", hz3xProb.Probability);
            //        }
            //        #endregion
            //        break;
            //    case "JXSSC":
            //        #region 新时时彩
            //        {
            //            var dbLose = WCFClients.ExternalClient.Get_JXSSC_Double_Lose(UserToken);
            //            var zxLose = WCFClients.ExternalClient.Get_JXSSC_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_JXSSC_Hot(10, UserToken);
            //            var dsLose = WCFClients.ExternalClient.Get_JXSSC_DXDS_Lose(UserToken);
            //            var dsHot = WCFClients.ExternalClient.Get_JXSSC_DXDS_Hot(10, UserToken);
            //            var hz2xLose = WCFClients.ExternalClient.Get_JXSSC_2XHZ_Lose(UserToken);
            //            //var hz2xProb = WCFClients.ExternalClient.Get_JXSSC_2XHZ_Probability(10, UserToken);
            //            var hz3xLose = WCFClients.ExternalClient.Get_JXSSC_3XHZ_Lose(UserToken);
            //            //var hz3xProb = WCFClients.ExternalClient.Get_JXSSC_3XHZ_Probability(10, UserToken);

            //            ret.Add("DBLose", dbLose.LoseNumbers);

            //            var tmpzxLose = new List<string>();
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10000").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1000").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "100").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("ZXLose", string.Join("#", tmpzxLose));

            //            var tmpzxHot = new List<string>();
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10000").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1000").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "100").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("ZXHot", string.Join("#", tmpzxHot));

            //            var tmpdsLose = new List<string>();
            //            tmpdsLose.Add(dsLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpdsLose.Add(dsLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("DSLose", string.Join("#", tmpdsLose));

            //            var tmpdsHot = new List<string>();
            //            tmpdsHot.Add(dsHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpdsHot.Add(dsHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("DSHot", string.Join("#", tmpdsHot));

            //            ret.Add("HZ2XLose", hz2xLose.LoseNumbers);
            //            //ret.Add("HZ2XProb", hz2xProb.Probability);
            //            ret.Add("HZ3XLose", hz3xLose.LoseNumbers);
            //            //ret.Add("HZ3XProb", hz3xProb.Probability);
            //        }
            //        #endregion
            //        break;
            //    case "SDQYH":
            //        #region 群英会
            //        {
            //            var lose = WCFClients.ExternalClient.Get_SDQYH_RX_Lose(UserToken);
            //            ret.Add("Lose", lose.LoseNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "GDKLSF":
            //        #region 广东快乐十分
            //        {
            //            var lose = WCFClients.ExternalClient.Get_GDKLSF_ZX_Lose(UserToken);
            //            ret.Add("Lose", lose.LoseNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "GXKLSF":
            //        #region 广西快乐十分
            //        {
            //            var lose = WCFClients.ExternalClient.Get_GXKLSF_ZX_Lose(UserToken);
            //            ret.Add("Lose", lose.LoseNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "FC3D":
            //        #region 福彩3D
            //        {
            //            var dbLose = WCFClients.ExternalClient.Get_FC3D_Double_Lose(UserToken);
            //            var zxLose = WCFClients.ExternalClient.Get_FC3D_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_FC3D_Hot(10, UserToken);
            //            ret.Add("DBLose", dbLose.LoseNumbers);

            //            var tmpzxLose = new List<string>();
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "100").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("ZXLose", string.Join("#", tmpzxLose));

            //            var tmpzxHot = new List<string>();
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "100").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("ZXHot", string.Join("#", tmpzxHot));
            //        }
            //        #endregion
            //        break;
            //    case "PL3":
            //        #region 排列三
            //        {
            //            var dbLose = WCFClients.ExternalClient.Get_PL3_Double_Lose(UserToken);
            //            var zxLose = WCFClients.ExternalClient.Get_PL3_ZX_Lose(UserToken);
            //            var zxHot = WCFClients.ExternalClient.Get_PL3_Hot(10, UserToken);
            //            ret.Add("DBLose", dbLose.LoseNumbers);
            //            var tmpzxLose = new List<string>();
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "100").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "10").LoseNumbers);
            //            tmpzxLose.Add(zxLose.FirstOrDefault(i => i.NumberPlace == "1").LoseNumbers);
            //            ret.Add("ZXLose", string.Join("#", tmpzxLose));

            //            var tmpzxHot = new List<string>();
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "100").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "10").HotNumbers);
            //            tmpzxHot.Add(zxHot.FirstOrDefault(i => i.NumberPlace == "1").HotNumbers);
            //            ret.Add("ZXHot", string.Join("#", tmpzxHot));
            //        }
            //        #endregion
            //        break;
            //    case "SSQ":
            //        #region 双色球
            //        {
            //            var lose = WCFClients.ExternalClient.Get_SSQ_Lose_Info(UserToken);
            //            ret.Add("Lose", lose.RedLoseInfo.LoseNumbers + "#" + lose.Blue_Lose_Info.LoseNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "DLT":
            //        #region 大乐透
            //        {
            //            var redlose = WCFClients.ExternalClient.Get_DLT_RedLose(UserToken);
            //            var bluelose = WCFClients.ExternalClient.Get_DLT_BlueLose(UserToken);
            //            ret.Add("Lose", redlose.LoseNumbers + "#" + bluelose.LoseNumbers);
            //        }
            //        #endregion
            //        break;
            //    case "JSKS":
            //        #region 江苏快三
            //        {

            //        }
            //        #endregion
            //        break;
            //}
            #endregion

            _hotLoseList.Add(gameCode, ret);
            return _hotLoseList[gameCode];
        }

        /// <summary>
        /// 是否为竞技彩
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public bool isJJC(string gameCode)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                case "jclq":
                case "ctzq": return true;
                default: return false;
            }
        }
        #endregion

        #region 统计信息

        private static Dictionary<DateTime, decimal> _totalBonusMoney = new Dictionary<DateTime, decimal>();
        /// <summary>
        /// @(SiteString.getHZSiteName(Request))累计发放中奖金额
        /// </summary>
        /// <param name="today">传入今天的时间，DateTime.Today</param>
        /// <returns>返回一共的发放中奖金额</returns>
        public decimal TotalBonusMoney(DateTime today)
        {
            if (!_totalBonusMoney.ContainsKey(today))
            {
                _totalBonusMoney.Clear();
                _totalBonusMoney.Add(today, WCFClients.ExternalClient.QueryTotalBonusMoney(UserToken));
            }
            return _totalBonusMoney[today];
        }

        #endregion

        #region 网站接口账号配置信息

        #region 支付宝配置信息
        /// <summary>
        /// 安全校验码，与partner是一组的
        /// </summary>
        public string ali_key
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.Alipay.AliKey").ConfigValue;
                //return SettingConfigAnalyzer.GetConfigValueByKey("Alipay", "AliKey");
            }
        }
        /// <summary>
        /// 商户ID，合用身份者ID，合用伙伴ID
        /// </summary>
        public string ali_Partner
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.Alipay.Partner").ConfigValue;
                //return SettingConfigAnalyzer.GetConfigValueByKey("Alipay", "Partner");
            }
        }
        /// <summary>
        /// 商家签约时的支付宝账号，即收款的支付宝账号
        /// </summary>
        public string ali_Seller_Email
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.Alipay.Seller_Email").ConfigValue;
                //return ConfigurationManager.AppSettings["Alipay_Seller_Email"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("Alipay", "Seller_Email");
            }
        }
        #endregion

        #region QQ登录配置信息
        /// <summary>
        /// qq登录ID
        /// </summary>
        public string qqlogin_ConsumerKey
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("QQLogin.ConsumerKey").ConfigValue;
                //return ConfigurationManager.AppSettings["QQLogin_ConsumerKey"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("QQLogin", "ConsumerKey");
            }
        }
        /// <summary>
        /// qq登录密钥
        /// </summary>
        public string qqlogin_ConsumerSecret
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("QQLogin.ConsumerSecret").ConfigValue;
                //return ConfigurationManager.AppSettings["QQLogin_ConsumerSecret"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("QQLogin", "ConsumerSecret");
            }
        }
        /// <summary>
        /// qq登录回调地址
        /// </summary>
        public string qqlogin_CallBack
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("QQLogin.CallBack").ConfigValue;
                //return ConfigurationManager.AppSettings["QQLogin_CallBack"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("QQLogin", "CallBack");
            }
        }
        #endregion

        #region 快钱支付配置信息
        /// <summary>
        /// 人民币网关账户号
        /// </summary>
        public string kq_MerchantAcctId
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("KuaiQian_MerchantAcctId").ConfigValue;
            }
        }
        /// <summary>
        /// 商户私钥密钥
        /// </summary>
        public string kq_CertificatePW
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("KuaiQian_CertificatePW").ConfigValue;
            }
        }
        /// <summary>
        /// 商户证书名称
        /// </summary>
        public string kq_PriKeyName
        {
            get
            {
                return ConfigurationManager.AppSettings["KuaiQian_PriKeyName"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("KuaiQian", "PriKeyName");
            }
        }
        /// <summary>
        /// 公钥证书名称
        /// </summary>
        public string kq_PubKeyName
        {
            get
            {
                return ConfigurationManager.AppSettings["KuaiQian_PubKeyName"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("KuaiQian", "PubKeyName");
            }
        }
        #endregion

        #region 财付通配置信息
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string tenpay_TenpayKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Tenpay_TenpayKey"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("Tenpay", "TenpayKey");
            }
        }
        /// <summary>
        /// 商户号
        /// </summary>
        public string tenpay_Partner
        {
            get
            {
                return ConfigurationManager.AppSettings["Tenpay_Partner"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("Tenpay", "Partner");
            }
        }
        #endregion

        #region 银联语音支付配置信息
        /// <summary>
        /// 商户号
        /// </summary>
        public string dnapay_MerchantNo
        {
            get
            {
                return ConfigurationManager.AppSettings["DNAPay_MerchantNo"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("DNAPay", "MerchantNo");
            }
        }
        /// <summary>
        /// 终端号
        /// </summary>
        public string dnapay_TerminalNo
        {
            get
            {
                return ConfigurationManager.AppSettings["DNAPay_TerminalNo"];
                //   return SettingConfigAnalyzer.GetConfigValueByKey("DNAPay", "TerminalNo");
            }
        }
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string dnapay_MerchantPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["DNAPay_MerchantPassword"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("DNAPay", "MerchantPassword");
            }
        }
        /// <summary>
        /// 服务接口地址
        /// </summary>
        public string dnapay_ServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["DNAPay_ServiceUrl"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("DNAPay", "ServiceUrl");
            }
        }
        #endregion

        #region 财务人员信息
        /// <summary>
        /// 财务手机号码
        /// </summary>
        public string[] fina_mobile
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("Site.Financial.Mobile").ConfigValue.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries); ;
                //return ConfigurationManager.AppSettings["Financial_Mobile"].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                //return SettingConfigAnalyzer.GetConfigValueByKey("Financial", "Mobile").Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        /// <summary>
        /// 财务电子邮箱
        public string fina_email
        {
            get
            {
                return ConfigurationManager.AppSettings["Financial_Email"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("Financial", "Email");
            }
        }
        #endregion

        #region 银宝配置信息
        /// <summary>
        /// 安全校验码，与partner是一组的
        /// </summary>
        public string yinbao_key
        {
            get
            {
                return ConfigurationManager.AppSettings["YinBao_YinBaoKey"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("YinBao", "YinBaoKey");
            }
        }
        /// <summary>
        /// 商户ID，合用身份者ID，合用伙伴ID
        /// </summary>
        public string yinbao_id
        {
            get
            {
                return ConfigurationManager.AppSettings["YinBao_YinBaoId"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("YinBao", "YinBaoId");
            }
        }
        #endregion

        #region 网银在线支付信息
        /// <summary>
        /// 商户号
        /// </summary>
        public string cb_mid
        {
            get
            {
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.ChinaBank.Mid").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return "23075914";
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return "23075914";
                }
            }
        }
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string cb_key
        {
            get
            {
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.ChinaBank.Key").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return "96763deb3f8dd25447d8f8c12648aad1";
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {

                    return "96763deb3f8dd25447d8f8c12648aad1";
                }

                // return ConfigurationManager.AppSettings["ChinaBank_ChinaBankKey"];
                //var key = WCFClients.GameClient.QueryCoreConfigByKey("ChinaBank_Mid");
                //if (key==null)
                //{

                //}
                //else
                //{
                //    return key.ConfigValue;
                //}
                //return SettingConfigAnalyzer.GetConfigValueByKey("ChinaBank", "ChinaBankKey");
            }
        }
        #endregion

        #region 易极付接口配置
        /// <summary>
        /// 易极付合作者身份ID PartnerId
        /// </summary>
        public string yjf_partnerid
        {
            get
            {
                return ConfigurationManager.AppSettings["YiJiFu_PartnerId"];
                // return SettingConfigAnalyzer.GetConfigValueByKey("YiJiFu", "PartnerId");
            }
        }

        /// <summary>
        /// 易极付密钥
        /// </summary>
        public string yjf_key
        {
            get
            {
                return ConfigurationManager.AppSettings["YiJiFu_YiJiFuKey"];
                //return SettingConfigAnalyzer.GetConfigValueByKey("YiJiFu", "YiJiFuKey");
            }
        }

        /// <summary>
        /// 易极付接口，集成易极付各交易接口函数
        /// 可根据这个接口来生成获取易极付交易接口url或进行易极付交易
        /// </summary>
        public YiJiFuService YJFService
        {
            get
            {
                var partnerid = ConfigurationManager.AppSettings["YiJiFu_PartnerId"]; //SettingConfigAnalyzer.GetConfigValueByKey("YiJiFu", "PartnerId");
                var key = ConfigurationManager.AppSettings["YiJiFu_YiJiFuKey"]; // SettingConfigAnalyzer.GetConfigValueByKey("YiJiFu", "YiJiFuKey");
                return new YiJiFuService(partnerid, key, IsTest);
            }
        }
        #endregion

        #region 环迅支付接口配置

        /// <summary>
        /// 环迅支付地址
        /// </summary>
        public string IPS_Url
        {
            get
            {
                string defalutValue = "http://pay.ips.net.cn/ipayment.aspx";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.IPS_Url").ConfigValue;
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
        /// 环迅支付商户号
        /// </summary>
        public string IPS_AgentId
        {
            get
            {
                string defalutValue = "000015";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.AgentId").ConfigValue;
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
        /// 环迅支付密钥
        /// </summary>
        public string IPS_Key
        {
            get
            {
                string defalutValue = "GDgLwwdK270Qj1w4xho8lyTpRQZV9Jm5x4NwWOTThUa4fMhEBK9jOXFrKRT6xhlJuU2FEa89ov0ryyjfJuuPkcGzO5CeVx5ZIrkkt1aBlZV36ySvHOMcNv8rncRiy3DQ";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.AgentKey").ConfigValue;
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
        /// 环迅支付跳转域名
        /// </summary>
        public string IPS_Jump_Url
        {
            get
            {
                string defalutValue = "http://z.jinxiangru.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("IPS_FillMoney.JumpUrl").ConfigValue;
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
        public string ZF_Jump_Url
        {
            get
            {
                string defalutValue = "http://pay.xsahedejs.cn";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("ZF_FillMoney.JumpUrl").ConfigValue;
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

        public string HC_Jump_Url
        {
            get
            {
                string defalutValue = "http://pay.xiacai.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("HC_FillMoney.JumpUrl").ConfigValue;
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

        public string YS_Jump_Url
        {
            get
            {
                string defalutValue = "http://pay.xiacai.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("YS_FillMoney.JumpUrl").ConfigValue;
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

        public string HC_NoCard_Jump_Url
        {
            get
            {
                string defalutValue = "http://pay.xiacai.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("HC_FillMoney.NoCard_Jump_Url").ConfigValue;
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
        /// 微信(优付)
        /// </summary>
        public string YF_WeiXin_Url
        {
            get
            {
                string defalutValue = "http://pay.szphlx.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("WXYF_PAY_REQREFERER").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        if (v.Contains("http"))
                        {
                            return v;
                        }
                        else
                        {
                            return string.Format("http://{0}", v);
                        }
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 中铁支付
        /// </summary>
        public string ZTPay_Url
        {
            get
            {
                string defalutValue = "http://pay.yongyecn.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("ZT_PAY_REQREFERER").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        if (v.Contains("http"))
                        {
                            return v;
                        }
                        else
                        {
                            return string.Format("http://{0}", v);
                        }
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// ka101支付
        /// </summary>
        public string PAY_101ka_URL_Url
        {
            get
            {
                string defalutValue = "http://pay1.denei.top";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("101ka_PAY_URL").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        if (v.Contains("http"))
                        {
                            return v;
                        }
                        else
                        {
                            return string.Format("http://{0}", v);
                        }
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 充值回调域名
        /// </summary>
        public string FillMoneyCallBackDomain
        {
            get
            {
                string defalutValue = "http://paytz.wancai.com";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoney.CallBackDomain").ConfigValue;
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
        /// 启用的充值接口
        /// </summary>
        public string FillMoney_Enable_GateWay
        {
            get
            {
                string defalutValue = "ZF_Bank|HC_Bank|IPS_Bank|IPS|Alipay|YS_Bank";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoney_Enable_GateWay").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return string.Empty;
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

        #region 微信支付接口配置

        /// <summary>
        /// 微信支付AppId
        /// </summary>
        public string WXPay_AppId
        {
            get
            {
                string defalutValue = "wx72f6b1a403cef120";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.WXPay_AppId").ConfigValue;
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
        /// 微信支付商户号
        /// </summary>
        public string WXPay_Mchid
        {
            get
            {
                string defalutValue = "1326803801";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.WXPay_Mchid").ConfigValue;
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
        /// 微信支付KEY
        /// </summary>
        public string WXPay_Key
        {
            get
            {
                string defalutValue = "Abcdabcd123443218888888800000000";
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("FillMoneyAgent.WXPay_Key").ConfigValue;
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

        public decimal MaxFillMoney
        {
            get
            {
                var defalutValue = 10000M;
                try
                {
                    var v = WCFClients.GameClient.QueryCoreConfigByKey("Max.FillMoney").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return decimal.Parse(v);
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        #endregion

        #region 功能函数
        /// <summary>
        /// 读取物理文件路径
        /// </summary>
        /// <param name="fileName">文件物理地址</param>
        /// <returns>文件内容</returns>
        public static string ReadFileString(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filename">需要保存的物理文件地址</param>
        /// <param name="saveName">保存的文件名</param>
        public void DownloadFile(string filename, string saveName)
        {
            FileInfo fi = new FileInfo(filename);
            string fileextname = fi.Extension;
            string DEFAULT_CONTENT_TYPE = "application/text-html";
            //RegistryKey regkey, fileextkey;
            string filecontenttype = DEFAULT_CONTENT_TYPE;
            //try
            //{
            //    regkey = Registry.ClassesRoot;
            //    fileextkey = regkey.OpenSubKey(fileextname);
            //    filecontenttype = fileextkey.GetValue("Content Type", DEFAULT_CONTENT_TYPE).ToString();
            //}
            //catch
            //{
            //    filecontenttype = DEFAULT_CONTENT_TYPE;
            //}


            Response.Clear();
            Response.Charset = "utf-8";
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            Response.AppendHeader("Content-Disposition", "attachment;filename=" + saveName);
            Response.ContentType = filecontenttype;

            Response.WriteFile(filename);
            Response.Flush();

            Response.End();
            Response.Close();
        }

        #region 隐藏字符串功能函数

        /// <summary>
        /// 加密字符串，隐藏指定位置用替换符代替
        /// </summary>
        /// <param name="objString">目标字符串</param>
        /// <param name="startIndex">起始隐藏位置</param>
        /// <param name="endIndex">结束隐藏位置</param>
        /// <param name="perch">替换符位数，默认3位</param>
        /// <param name="perchString">替换符号，默认*</param>
        /// <returns></returns>
        public static string EncodeString(string objString, int startIndex = 0, int endIndex = 0, int perch = 3, string perchString = "*")
        {
            if (endIndex > 0 && endIndex > startIndex && endIndex <= objString.Length)
            {
                var perchtmp = "";
                for (int i = 0; i < perch; i++)
                {
                    perchtmp += perchString;
                }
                return objString.Substring(0, startIndex) + perchtmp + objString.Substring(endIndex);
            }
            else
            {
                return objString;
            }
        }
        /// <summary>
        /// 隐藏用户名
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="showCount">显示长度</param>
        /// <returns>已隐藏部分用户名</returns>
        public static string HideUserName(string userName, int hideCount = 2)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }
            // var perchtmp = "***";
            hideCount = hideCount > userName.Length ? userName.Length : hideCount;
            //  return userName.Substring(0, 1) + perchtmp;
            return EncodeString(userName, userName.Length - hideCount, userName.Length, hideCount);
        }

        #endregion

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public void WriteException(string source, Exception ex)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                logWriter.Write("Error", source, ex);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public void WriteLog(string source, LogType logType, string logMsg, string detail)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                logWriter.Write("Site", source, logType, logMsg, detail);
            }
        }
        #endregion

        private static Dictionary<string, ThendLinkCollection> _thendLinkList = new Dictionary<string, ThendLinkCollection>();
        public ThendLinkCollection GetThendLinkCollection(string gameCode)
        {
            if (!_thendLinkList.Keys.Contains(gameCode))
                _thendLinkList.Add(gameCode, ThendLink(gameCode));
            return _thendLinkList[gameCode];
        }

        /// <summary>
        ///顶部彩种导航走势链接 
        /// </summary>
        private ThendLinkCollection ThendLink(string gameCode)
        {
            var collection = new ThendLinkCollection();
            #region switch 所有彩种
            switch (gameCode)
            {
                case "SSQ":
                    #region SSQ ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/staticHtml/lotterytrend/SSQ/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/Chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/DaXiao30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/HeZhi30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/JiOu30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "首尾跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_SW30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "一二跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_1230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二三跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_2330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三四跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_3430.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "四五跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_4530.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "五六跨度走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/KuaDu_5630.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/staticHtml/lotteryTrend/SSQ/ZhiHe30.html",
                    });
                    #endregion
                    break;
                case "DLT":
                    #region DLT ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/jiou30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/zhihe30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/hezhi30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "首尾跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/kuadu_sw30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "一二跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/kuadu_1230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二三跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/kuadu_2330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三四跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/kuadu_3430.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "四五跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DLT/kuadu_4530.html",
                    });
                    #endregion
                    break;
                case "CQSSC":
                    #region CQSSC ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3zxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小单双",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/dxds30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "一星走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s1zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s2hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s2zuxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s2zxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星除3余数",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3c3ys30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3kd30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3zhzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s3zuxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "五星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s5hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "五星基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQSSC/s5jbzs30.html",
                    });
                    #endregion
                    break;
                case "SDQYH":
                    #region SDQYH ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "顺选1走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/sx130.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "顺选2走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/sx230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "顺选3走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/sx330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/SDQYH/zh30.html",
                    });
                    #endregion
                    break;
                case "JSKS":
                    #region JSK3 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/JSKS/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JSKS/hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/JSKS/xt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组合走势",
                        LinkUrl = "/statichtml/lotteryTrend/JSKS/zh30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "综合走势",
                        LinkUrl = "/statichtml/lotteryTrend/JSKS/zhzs30.html",
                    });
                    #endregion
                    break;
                case "SDKLPK3":
                    #region SDKLPK3 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/zhxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "花色走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/hszs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/zhzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3余走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/c3yzs30.html",
                    }); 
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "类型走势",
                        LinkUrl = "/statichtml/lotteryTrend/sdklpk3/lxzs30.html",
                    });
                    #endregion
                    break;
                case "JLK3":
                    #region JLK3 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/staticHtml/lotteryTrend/JLK3/hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/staticHtml/lotteryTrend/JLK3/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/staticHtml/lotteryTrend/JLK3/xt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组合走势",
                        LinkUrl = "/staticHtml/lotteryTrend/JLK3/zh30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "综合走势",
                        LinkUrl = "/staticHtml/lotteryTrend/JLK3/zhzs30.html",
                    });
                    #endregion
                    break;
                case "HBK3":
                    #region HBK3 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/HBK3/hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/HBK3/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/HBK3/xt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组合走势",
                        LinkUrl = "/statichtml/lotteryTrend/HBK3/zh30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "综合走势",
                        LinkUrl = "/statichtml/lotteryTrend/HBK3/zhzs30.html",
                    });
                    #endregion
                    break;
                case "QXC":
                    #region QXC ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/QXC/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/QXC/chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/QXC/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/QXC/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/QXC/zh30.html",
                    });
                    #endregion
                    break;
                case "QLC":
                    #region QLC ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/QLC/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/QLC/chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/QLC/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/QLC/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/QLC/zh30.html",
                    });
                    #endregion
                    break;
                case "PL3":
                    #region PL3 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3一走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/chu3130.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3二走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/chu3230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3三走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/chu3330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小号码走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/dxhm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/hezhi30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和尾走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/hzhw30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值特征走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/hztz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶号码走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/johm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度百个走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/kdbg30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度百十走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/kdbs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度十个走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/kdsg30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和号码走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/zhhm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/zhihe30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL3/zxzs30.html",
                    });
                    #endregion
                    break;
                case "PL5":
                    #region PL5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/PL5/zh30.html",
                    });
                    #endregion
                    break;
                case "JX11X5":
                    #region JX11X5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxjbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前2和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q2hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前2组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q2zux30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前2直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q2zx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3chu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3zh30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3zx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前3组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/q3zux30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选1走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rx130.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选2走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rx230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选3走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rx330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选4走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rx430.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选5走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rx530.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选除3走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxchu330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxdx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxjo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选质和走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxzh30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "任选和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JX11X5/rxhz30.html",
                    });
                    #endregion
                    break;
                case "CQ11X5":
                    #region CQ11X5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "重号走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/chzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "多连走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/dlzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "定位走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/dwzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/kdzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "012路比走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/lzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q1jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q1xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q2jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q2xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q3jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/q3xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQ11X5/xtzs30.html",
                    });
                    #endregion
                    break;
                case "CQKLSF":
                    #region CQKLSF ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/q1zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/q3zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "区间走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/qjzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三连走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/slzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "同尾走势",
                        LinkUrl = "/statichtml/lotteryTrend/CQKLSF/twzs30.html",
                    });
                    #endregion
                    break;
                case "DF6J1":
                    #region DF6J1 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/kdzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/DF6J1/zhzs30.html",
                    });
                    #endregion
                    break;
                case "FC3D":
                    #region FC3D ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "直选走势",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/zxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3余数走势一",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/chu3130.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3余数走势二",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/chu3230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "除3余数走势三",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/chu3330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小号码分布",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/dxhm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小形态分布",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/dxxt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和尾走势",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/hwzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值分布",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/hzfb30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值特征",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/hztz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶号码分布",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/johm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶形态分布",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/joxt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "总跨度",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/kuadu30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "百十跨度",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/kuadu1230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "百个跨度",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/kuadu1330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "十个跨度",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/kuadu2330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合号码分布",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/zhhm30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合形态分布",
                        LinkUrl = "/statichtml/lotteryTrend/FC3D/zhxt30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "组选走势",
                        LinkUrl = "/staticHtml/lotteryTrend/FC3D/zuxzs30.html",
                    });
                    #endregion
                    break;
                case "GD11X5":
                    #region GD11X5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "重号走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/chzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "多连走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/dlzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "定位走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/dwzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/kdzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "012路比走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/lzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q1jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q1xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q2jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q2xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q3jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/q3xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/GD11X5/xtzs30.html",
                    });
                    #endregion
                    break;
                case "GDKLSF":
                    #region GDKLSF ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";

                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "定位一走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/dw130.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "定位二走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/dw230.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "定位三走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/dw330.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/GDKLSF/zh30.html",
                    });
                    #endregion
                    break;
                case "HC1":
                    #region HC1 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/HC1/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "生肖季节方位走势",
                        LinkUrl = "/statichtml/lotteryTrend/HC1/sxjjfwzs30.html",
                    });
                    #endregion
                    break;
                case "HD15X5":
                    #region HD15X5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "低频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "重号走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/chzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/dx30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/hz30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/jo30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "邻号走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/lhzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/HD15X5/zh30.html",
                    });
                    #endregion
                    break;
                case "HNKLSF":
                    #region HNKLSF ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/q1zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/q3zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "区间走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/qjzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三连走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/slzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "同尾走势",
                        LinkUrl = "/statichtml/lotteryTrend/HNKLSF/twzs30.html",
                    });
                    #endregion
                    break;
                case "JXSSC":
                    #region JXSSC ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3zxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小单双",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/dxds30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "一星走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s1zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s2hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s2zuxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二星直选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s2zxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星除3余数",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3c3ys30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3kd30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星质合走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3zhzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "三星组选走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s3zuxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "五星和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s5hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "五星基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/JXSSC/s5jbzs30.html",
                    });
                    #endregion
                    break;
                case "LN11X5":
                    #region LN11X5 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "重号走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/chzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "多连走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/dlzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "大小走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/dxzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "奇偶走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/jozs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/q1zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/q2zs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三走势",
                        LinkUrl = "/statichtml/lotteryTrend/LN11X5/q3zs30.html",
                    });
                    #endregion
                    break;
                case "SD11X5":
                    #region YDJ11 ThendLink
                    collection.GameCode = gameCode;
                    collection.GameName = GetGameName(gameCode);
                    collection.GDThend = "高频彩";
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "重号走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/chzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "多连走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/dlzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "012定位走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/dwzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "二连走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/elzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "隔号走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/ghzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "和值走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/hzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "跨度走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/kdzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "012路比走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/lzzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q1jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前一形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q1xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q2jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前二形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q2xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三基本走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q3jbzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "前三形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/q3xtzs30.html",
                    });
                    collection.LinkList.Add(new ThendLinkInfo
                    {
                        LinkName = "形态走势",
                        LinkUrl = "/statichtml/lotteryTrend/SD11X5/xtzs30.html",
                    });
                    #endregion
                    break;
                default:
                    break;
            }
            #endregion
            return collection;
        }

        /// <summary>
        /// 默认页面索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 默认页面条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return 30;
            }
        }
        public string GetGameName(string strGameCode)
        {
            switch (strGameCode)
            {
                case "CQSSC": return "重庆时时彩";
                case "DLT": return "大乐透";
                case "FC3D": return "福彩3D";
                case "GD11X5": return "广东十一选五";
                case "GDKLSF": return "广东快乐十分";
                case "GXKLSF": return "广西快乐十分";
                case "JSKS": return "江苏快3";
                case "JX11X5": return "江西十一选五";
                case "JXSSC": return "江西时时彩";
                case "PL3": return "排列三";
                case "QLC": return "七乐彩";
                case "QXC": return "七星彩";
                case "SD11X5": return "山东十一选五";
                case "SDQYH": return "山东群英会";
                case "SSQ": return "双色球";
                case "CQ11X5": return "重庆十一选五";
                case "CQKLSF": return "重庆快乐十分";
                case "DF6J1": return "东方6+1";
                case "HBK3": return "湖北快3";
                case "HC1": return "好彩一";
                case "HD15X5": return "华东十五选五";
                case "HNKLSF": return "湖南快乐十分";
                case "JLK3": return "新快3";
                case "JSK3": return "江苏快3";
                case "LN11X5": return "辽宁十一选五";
                case "PL5": return "排列五";
                case "BJDC": return "北京单场";
                case "JCZQ": return "竞彩足球";
                case "JCLQ": return "竞彩篮球";
                case "CTZQ": return "传统足球";
                default: return "所有";
            }
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        protected string SendMessage(string mobile, string msgContent, string str)
        {
            var agentName = WCFClients.GameClient.QueryCoreConfigByKey("SMSAgent.Name").ConfigValue;
            var attach = WCFClients.GameClient.QueryCoreConfigByKey("SMSAgent.Attach").ConfigValue;
            var password = WCFClients.GameClient.QueryCoreConfigByKey("SMSAgent.Password").ConfigValue;
            var userName = WCFClients.GameClient.QueryCoreConfigByKey("SMSAgent.UserId").ConfigValue;
            var returnted = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
            {
                AgentName = agentName,
                Attach = attach,
                Password = password,
                UserName = userName
            }).SendSMS(mobile, msgContent, attach);
            return returnted;
        }
        /// <summary>
        /// 控制网站部分功能是否显示
        /// </summary>
        /// <returns></returns>
        //public bool IsShowData()
        //{
        //    try
        //    {
        //        var result = WCFClients.GameClient.QueryCoreConfigByKey("IsShowData");
        //        if (result == null)
        //            return false;
        //        return Convert.ToBoolean(result.ConfigValue);
        //    }
        //    catch { return false; }
        //}

        //public bool IsInBetWhiteList()
        //{

        //    try
        //    {
        //        return WCFClients.ExternalClient.IsInBetWhiteList(CurrentUser.LoginInfo.UserId);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public string WithdrawMoney
        {
            get
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("WithdrawMoney").ConfigValue;
            }

        }

        public bool IsAddAgent
        {
            get
            {
                var needAgent = WCFClients.GameClient.QueryCoreConfigByKey("IsAddAgent").ConfigValue;
                return needAgent == "1" ? true : false;
            }
        }

        #region 接收后台通知和生成静态文件相关

        /// <summary>
        /// 静态文件根目录
        /// </summary>
        private const string _staticFileRootPath = "staticHtml";
        /// <summary>
        /// 幻灯版文件目录
        /// </summary>
        private const string _staticFileBannerPath = "Banners";
        private const string _staticFileBanner_Index = "indexBanner.html";
        private const string _staticFileBanner_CMS = "cmsBanner.html";

        /// <summary>
        /// 文章列表目录
        /// </summary>
        private const string _staticFileCMSDirectoryPath = "CMSDirectory";
        /// <summary>
        /// 文章详细目录
        /// </summary>
        private const string _staticFileCMSDetailPath = "WebCMSDetail";
        /// <summary>
        /// 神单排行目录
        /// </summary>
        private const string _staticFileWebSDTopPath = "WebSDTop";

        /// <summary>
        /// 接收生成静态文件通知
        /// </summary>
        //public void ReceiveBuildFileNotice(WebCacheNoticeCategory category, string key)
        //{
        //switch (category)
        //{
        //    case WebCacheNoticeCategory.WebCMSDetail:
        //        //网站资讯详细页
        //        break;
        //    case WebCacheNoticeCategory.WebCMSBanner:
        //        //Web站资讯页幻灯片
        //        BuildCMSBannerHtml(GetCMSBannerHtmlPath());
        //        break;
        //    case WebCacheNoticeCategory.WebCMSDirectory:
        //        //网站资讯目录列表
        //        break;
        //    case WebCacheNoticeCategory.WebIndexBanner:
        //        //Web站首页幻灯片
        //        BuildIndexBannerHtml(GetIndexBannerHtmlPath());
        //        break;
        //    case WebCacheNoticeCategory.WebNewOpenNotice:
        //        //网站最新开奖公告
        //        break;
        //    case WebCacheNoticeCategory.WebSDTop:
        //        //网站神单排行
        //        break;
        //    default:
        //        break;
        //}
        //}

        #region 首页幻灯片

        /// <summary>
        /// 获取首页幻灯片静态页路径
        /// </summary>
        public string TryGetIndexBannerHtmlWebPath()
        {
            try
            {
                var fileFullName = GetIndexBannerHtmlPath();
                if (!System.IO.File.Exists(fileFullName))
                    BuildIndexBannerHtml(fileFullName);
                return fileFullName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取首页幻灯片静态页路径
        /// </summary>
        public string GetIndexBannerHtmlPath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _staticFileRootPath, _staticFileBannerPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Path.Combine(path, _staticFileBanner_Index);
        }

        /// <summary>
        /// 生成首页幻灯片静态页面
        /// </summary>
        private void BuildIndexBannerHtml(string fileFullName)
        {
            //查询数据库数据
            var context = new ControllerContext(this.Request.RequestContext, new HomeController());
            context.Controller.ViewBag.Ads = WCFClients.ExternalClient.QuerySitemessageBanngerList_Web(BannerType.Index);
            BuildPartialViewHtml(context, "SlideBox", fileFullName);
        }

        #endregion

        #region 生成CMS幻灯片

        /// <summary>
        /// 获取首页幻灯片静态页路径
        /// </summary>
        public string TryGetCMSBannerHtmlWebPath()
        {
            try
            {
                var fileFullName = GetCMSBannerHtmlPath();
                if (!System.IO.File.Exists(fileFullName))
                    BuildCMSBannerHtml(fileFullName);
                return fileFullName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取cms幻灯片静态页路径
        /// </summary>
        private string GetCMSBannerHtmlPath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _staticFileRootPath, _staticFileBannerPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Path.Combine(path, _staticFileBanner_CMS);
        }

        /// <summary>
        /// 生成cms幻灯片静态页面
        /// </summary>
        private void BuildCMSBannerHtml(string fileFullName)
        {
            BuildPartialViewHtml(new ControllerContext(this.Request.RequestContext, new HomeController()), "SlideBox", fileFullName);
        }

        #endregion

        /// <summary>
        /// 生成完整html静态文件
        /// </summary>
        private void BuildViewHtml(Controller c, string actionName, string fileFullName)
        {
            try
            {
                var staticHtml = string.Empty;
                IView v = ViewEngines.Engines.FindView(c.ControllerContext, actionName, "").View;
                using (StringWriter sw = new StringWriter())
                {
                    ViewContext vc = new ViewContext(c.ControllerContext, v, c.ViewData, c.TempData, sw);
                    vc.View.Render(vc, sw);
                    staticHtml = sw.ToString();
                }
                //保存html
                System.IO.File.WriteAllText(fileFullName, staticHtml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 生成部分视图html静态文件
        /// </summary>
        private void BuildPartialViewHtml(ControllerContext context, string actionName, string fileFullName)
        {
            try
            {
                var staticHtml = string.Empty;
                IView v = ViewEngines.Engines.FindPartialView(context, actionName).View;
                using (StringWriter sw = new StringWriter())
                {
                    ViewContext vc = new ViewContext(context, v, context.Controller.ViewData, context.Controller.TempData, sw);
                    vc.View.Render(vc, sw);
                    staticHtml = sw.ToString();
                }
                //保存html
                System.IO.File.WriteAllText(fileFullName, staticHtml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
            }
        }










        /// <summary>
        /// 加载指定目录的html
        /// </summary>
        public string TryLoadHtml(string fileFullName)
        {
            try
            {
                //if (System.IO.File.Exists(fileFullName))
                //{
                //    return System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                //}
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 保存action视图内容为html
        /// </summary>
        public string TrySaveHtml(string actionName, string fileFullName)
        {
            try
            {
                var staticHtml = string.Empty;
                IView v = ViewEngines.Engines.FindView(this.ControllerContext, actionName, "").View;
                using (StringWriter sw = new StringWriter())
                {
                    ViewContext vc = new ViewContext(this.ControllerContext, v, this.ViewData, this.TempData, sw);
                    vc.View.Render(vc, sw);
                    staticHtml = sw.ToString();
                }
                //保存html
                System.IO.File.WriteAllText(fileFullName, staticHtml, Encoding.UTF8);
                return staticHtml;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string TrySaveContent(string fileFullName, string content)
        {
            try
            {
                //保存html
                System.IO.File.WriteAllText(fileFullName, content, Encoding.UTF8);
                return content;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 欧洲指数
        /// </summary>
        public ContentResult odds(string id)
        {
            var actionName = RouteData.Values["action"].ToString();
            var fileFullName = Server.MapPath(string.Format("~/staticHtmls/XTSportsInfo/{0}_{1}.html", actionName, id));

            //加载html
            var staticHtml = TryLoadHtml(fileFullName);
            if (!string.IsNullOrEmpty(staticHtml))
                return Content(staticHtml);


            ViewBag.MatchId = id;
            var url = "http://www.okooo.com/soccer/match/" + id + "/odds/";
            var html = PostManager.Get(url, Encoding.GetEncoding("gb2312")).Replace("/soccer/match/" + id + "/odds/stat/", "/XTSportsInfo/oddstatsChange?id=" + id + "&Cid=").Replace("/?type", "&type").Replace("/\" class=\"zstxt", "&Bid=" + id + "\" class=\"zstxt").Replace("/odds/change/", "&Cid=").Replace("http://www.okooo.com/soccer/match/", "/XTSportsInfo/change?id=");
            int s4 = html.LastIndexOf("<div  class=\"oddthfooter03\">");
            int e4 = html.LastIndexOf("class=\"yellowbtn_auto\" name=\"\"></div>");

            var bb = html.Substring(s4, e4 - s4) + "class=\"yellowbtn_auto\" name=\"\"></div>";

            int s = html.IndexOf("<div class=\"jfbox\">");
            int e = html.IndexOf("<div class=\" clearbox\">");

            ViewBag.Res = html.Substring(s, e - s).Replace("/style/img/weather/", "/xinti/images/xtSportsInfo/").Replace(bb, "").Replace("<a href=\"/soccer/match/", "<a href=\"/XTSportsInfo/odds/").Replace("/odds/\" class=", "\" class=").Replace("class=\"bluelink\">分布图", "class=\"bluelink\">").Replace("澳客", "").Replace("target=\"_blank\">走势</a>", "target=\"_blank\"></a>");

            int s1 = html.IndexOf("var data_str");
            int e1 = html.IndexOf("var checkAjaxDataOver = 0;");
            ViewBag.ResH = html.Substring(s1, e1 - s1);

            //int s2 = html.IndexOf("<script type=\"text/javascript\">");
            //int e2 = html.IndexOf("</script>");
            //ViewBag.ResJS1 = html.Substring(s2, e2 - s2);

            int s5 = html.IndexOf("var LotteryType ='WDL';");
            int e5 = html.IndexOf("<div class=\"lqnav\" style=\"position:relative;\">");
            ViewBag.ResId = html.Substring(s5, e5 - s5);


            //保存html
            staticHtml = TrySaveHtml(actionName, fileFullName);
            return Content(staticHtml);


            //return View();
        }

        #endregion

        //public LotteryIssuse_QueryInfoCollection LoadAllGameIssuse_RefreshByOfficialStopTime()
        //{
        //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);
        //    var filePath = Path.Combine(path, "game_new_issuse_list_official.json");
        //    LotteryIssuse_QueryInfoCollection data = LoadOldAllGameIssuseData(filePath);

        //    //列表中有结束的奖期
        //    if (data.Count == 0 || data.Where(p => p.OfficialStopTime < DateTime.Now).Count() > 0)
        //    {
        //        data = WCFClients.GameIssuseClient.QueryAllGameCurrentIssuse(true);
        //        if (System.IO.File.Exists(filePath))
        //            System.IO.File.Delete(filePath);
        //        var json = JsonSerializer.Serialize(data);
        //        System.IO.File.WriteAllText(filePath, json, Encoding.UTF8);
        //    }
        //    return data;
        //}

        //private LotteryIssuse_QueryInfoCollection LoadOldAllGameIssuseData(string filePath)
        //{
        //    //加载老数据
        //    var data = new LotteryIssuse_QueryInfoCollection();
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        var jsonData = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
        //        if (!string.IsNullOrEmpty(jsonData.Trim()))
        //        {
        //            data = JsonSerializer.Deserialize<LotteryIssuse_QueryInfoCollection>(jsonData);
        //        }
        //    }
        //    return data;
        //}

        //public LotteryIssuse_QueryInfoCollection LoadAllGameIssuse_RefreshByLocalStopTime()
        //{
        //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);
        //    var filePath = Path.Combine(path, "game_new_issuse_list_local.json");
        //    LotteryIssuse_QueryInfoCollection data = LoadOldAllGameIssuseData(filePath);

        //    //列表中有结束的奖期
        //    if (data.Count == 0 || data.Where(p => p.LocalStopTime < DateTime.Now).Count() > 0)
        //    {
        //        data = WCFClients.GameIssuseClient.QueryAllGameCurrentIssuse(false);
        //        if (System.IO.File.Exists(filePath))
        //            System.IO.File.Delete(filePath);
        //        var json = JsonSerializer.Serialize(data);
        //        System.IO.File.WriteAllText(filePath, json, Encoding.UTF8);
        //    }
        //    return data;
        //}

        /// <summary>
        /// 从Redis中查询当前奖期
        /// ByOfficialStopTime
        /// </summary>
        public LotteryIssuse_QueryInfo QueryCurrentIssuseByOfficialStopTime(string gameCode)
        {
            var list = WebRedisHelper.QueryNextIssuseListByOfficialStopTime(gameCode);
            if (list == null || list.Count <= 0)
                return null;
            return list.Where(p => p.OfficialStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault();
        }

        /// <summary>
        /// 从Redis中查询当前奖期
        /// ByLocalStopTime
        /// </summary>
        public LotteryIssuse_QueryInfo QueryCurrentIssuseByLocalStopTime(string gameCode)
        {
            var list = WebRedisHelper.QueryNextIssuseListByLocalStopTime(gameCode);
            if (list == null || list.Count <= 0)
                return null;
            return list.Where(p => p.LocalStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault();
        }

        /// <summary>
        /// 是否投注到Redis队列
        /// </summary>
        public bool IsBetOrderToRedisList
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["BetOrderToRedisList"]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        #region JsonData相关查询

        public void ClearChippedCache()
        {
            try
            {
                _chipped_hotuser.Clear();
                _chipped_list.List.Clear();
                _chipped_list = new Sports_TogetherSchemeQueryInfoCollection();
            }
            catch (Exception)
            {

            }
        }

        private static List<TogetherHotUserInfo> _chipped_hotuser = new List<TogetherHotUserInfo>();
        private static object _chippedLock2 = new object();
        /// <summary>
        /// 查询合买大厅红人
        /// </summary>
        /// <returns></returns>
        public List<TogetherHotUserInfo> QueryHotUserTogetherOrderList()
        {
            lock (_chippedLock2)
            {
                if (_chipped_hotuser != null && _chipped_hotuser.Count > 0)
                    return _chipped_hotuser;
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsondata", "chipped", "superList.json");
                if (!System.IO.File.Exists(path))
                    return new List<TogetherHotUserInfo>();

                var content = System.IO.File.ReadAllText(path, Encoding.UTF8);
                if (string.IsNullOrEmpty(content))
                    return new List<TogetherHotUserInfo>();

                _chipped_hotuser = JsonSerializer.Deserialize<TogetherHotUserInfoCollection>(content).OrderByDescending(p => p.WeeksWinMoney).ToList();
                return _chipped_hotuser;
            }
        }

        private static Sports_TogetherSchemeQueryInfoCollection _chipped_list = new Sports_TogetherSchemeQueryInfoCollection();
        private static object _chippedLock = new object();
        /// <summary>
        /// 查询合买大厅数据
        /// </summary>
        public Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherList(string key, string issuseNumber, string gameCode, string gameType,
            TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
            decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize)
        {
            lock (_chippedLock)
            {
                var seC = !security.HasValue ? -1 : (int)security.Value;
                var betC = !betCategory.HasValue ? -1 : (int)betCategory.Value;
                var strPro = !progressState.HasValue ? "10|20|30" : ((int)progressState.Value).ToString();
                var arrProg = strPro.Split('|');
                if (!string.IsNullOrEmpty(gameCode))
                    gameCode = gameCode.ToUpper();
                if (!string.IsNullOrEmpty(gameType))
                    gameType = gameType.ToUpper();

                if (_chipped_list == null || _chipped_list.List.Count <= 0)
                {
                    this.WriteLog("hemai", LogType.Information, "QuerySportsTogetherList", "内存缓存为空，读取文件");

                    //内存 缓存为空或条数为0
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "chipped", "hmList.json");
                    if (!System.IO.File.Exists(path))
                        return new Sports_TogetherSchemeQueryInfoCollection();

                    //读文件缓存
                    var content = System.IO.File.ReadAllText(path, Encoding.UTF8);
                    if (string.IsNullOrEmpty(content))
                        return new Sports_TogetherSchemeQueryInfoCollection();

                    _chipped_list = JsonSerializer.Deserialize<Sports_TogetherSchemeQueryInfoCollection>(content);
                }

                var query = from s in _chipped_list.List
                            where arrProg.Contains(Convert.ToInt32(s.ProgressStatus).ToString())
                            && (betC == -1 || Convert.ToInt32(s.SchemeBettingCategory) == betC)
                            && (issuseNumber == string.Empty || s.IssuseNumber == issuseNumber)
                            && (s.StopTime >= DateTime.Now)
                            && (gameCode == string.Empty || s.GameCode == gameCode)
                            && (gameType == string.Empty || s.GameType == gameType)
                            && (minMoney == -1 || s.TotalMoney >= minMoney)
                            && (maxMoney == -1 || s.TotalMoney <= maxMoney)
                            && (minProgress == -1 || s.Progress >= minProgress)
                            && (maxProgress == -1 || s.Progress <= maxProgress)
                            && (seC == -1 || Convert.ToInt32(s.Security) == seC)
                            && (key == string.Empty || s.CreateUserId == key || s.SchemeId == key || s.CreaterDisplayName == key)
                            select s;
                var cache = new Sports_TogetherSchemeQueryInfoCollection();
                cache.TotalCount = query.Count();
                cache.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                return cache;
            }
        }

        public void ClearGGTJCache()
        {
            try
            {
                _cache_ggtj.ReportItemList.Clear();
                _cache_ggtj = new SportsOrder_GuoGuanInfoCollection();
            }
            catch (Exception)
            {

            }
        }
        private static SportsOrder_GuoGuanInfoCollection _cache_ggtj = new SportsOrder_GuoGuanInfoCollection();
        private static object _ggtjdLock = new object();
        /// <summary>
        /// 过关统计查询
        /// </summary>
        public SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            lock (_ggtjdLock)
            {
                if (_cache_ggtj == null || _cache_ggtj.ReportItemList.Count <= 0)
                {
                    this.WriteLog("ggtj", LogType.Information, "QueryReportInfoList_GuoGuan", "内存缓存为空，读取文件");
                    //内存 缓存为空或条数为0
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "ggtj", gameCode, "ggtj.json");
                    if (!System.IO.File.Exists(path))
                        return new SportsOrder_GuoGuanInfoCollection();

                    //读文件缓存
                    var content = System.IO.File.ReadAllText(path, Encoding.UTF8);
                    if (string.IsNullOrEmpty(content))
                        return new SportsOrder_GuoGuanInfoCollection();

                    _cache_ggtj = JsonSerializer.Deserialize<SportsOrder_GuoGuanInfoCollection>(content);
                }

                var cache = new SportsOrder_GuoGuanInfoCollection();
                var query = from s in _cache_ggtj.ReportItemList
                            where s.IsVirtualOrder == isVirtualOrder
                            && (!category.HasValue || s.SchemeBettingCategory == category.Value)
                            && (key == string.Empty || s.UserDisplayName == key)
                            && s.GameCode == gameCode.ToUpper()
                            && (gameType == string.Empty || s.GameType == gameType)
                            && (issuseNumber == string.Empty || s.IssuseNumber == issuseNumber)
                            && (s.BetTime >= startTime && s.BetTime < endTime)
                            select s;
                cache.TotalCount = query.Count();
                cache.ReportItemList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                return cache;
            }
        }

        public ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, DateTime startTime, DateTime endTime)
        {
            ConcernedInfo info = new ConcernedInfo();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsondata", "baodan", "ConcernedInfo", string.Format("{0}.json", bdfxUserId));
            if (!System.IO.File.Exists(path))
            {
                BuildConcerned(bdfxUserId, currUserId, startTime, endTime);
            }
            else
            {
                var jsonData = System.IO.File.ReadAllText(path, Encoding.UTF8);
                info = JsonSerializer.Deserialize<ConcernedInfo>(jsonData);
                var time = ConfigurationManager.AppSettings["ConcernedTime"];
                var t = DateTime.Now - Convert.ToDateTime(info.FileCreateTime);
                if (t.TotalMilliseconds >= Convert.ToInt32(time))
                    BuildConcerned(bdfxUserId, currUserId, startTime, endTime);
            }

            return info;
        }
        /// <summary>
        /// 生成神单发起者相关信息
        /// </summary>
        public void BuildConcerned(string userIds, string currUserId, DateTime startTime, DateTime endTime)
        {
            var arrUser = userIds.Split('|');
            if (arrUser != null && arrUser.Length > 0)
            {
                foreach (var item in arrUser)
                {
                    var info = WCFClients.ExternalClient.QueryConcernedByUserId(item, currUserId, startTime.ToString("MM.dd"), endTime.ToString("MM.dd"));
                    info.FileCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "baodan", "ConcernedInfo");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path = Path.Combine(path, item + ".json");
                    var strJson = JsonSerializer.Serialize<ConcernedInfo>(info);
                    System.IO.File.WriteAllText(path, strJson);
                }
            }
        }

        /// <summary>
        /// 查询今日宝单
        /// </summary>
        public TotalSingleTreasure_Collection QueryTodayBDFXList(string userName, string strOrderBy, string currentUserId, int pageIndex, int pageSize)
        {
            TotalSingleTreasure_Collection collection = new TotalSingleTreasure_Collection();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "baodan", "redantoday", "redantoday.json");
            if (!System.IO.File.Exists(path))
            {
                collection = BuildRedanToday();
                BuildYesterdayNR();
            }
            else
            {
                var strJson = System.IO.File.ReadAllText(path, Encoding.UTF8);
                if (!string.IsNullOrEmpty(strJson))
                {
                    collection = JsonSerializer.Deserialize<TotalSingleTreasure_Collection>(strJson);
                    if (collection != null && collection.TotalCount > 0)
                    {
                        var query = from s in collection.TotalSingleTreasureList where (userName == string.Empty || s.UserName == userName) && (currentUserId == string.Empty || s.UserId == currentUserId) select s;
                        if (string.IsNullOrEmpty(strOrderBy))
                            query = query.OrderByDescending(s => s.BDFXCreateTime);
                        else
                        {
                            string orderBy = string.Empty;
                            string desc = string.Empty;
                            var array = strOrderBy.ToLower().Split('|');
                            if (array != null && array.Length > 1)
                            {
                                orderBy = array[0].ToString();
                                desc = array[1].ToString();
                            }
                            switch (orderBy)
                            {
                                case "expectedreturnrate":
                                    if (!string.IsNullOrEmpty(desc) && desc == "desc")
                                        query = query.OrderByDescending(s => s.ExpectedReturnRate);
                                    else
                                        query = query.OrderBy(s => s.ExpectedReturnRate);
                                    break;
                                case "totalbuymoney":
                                    if (!string.IsNullOrEmpty(desc) && desc == "desc")
                                        query = query.OrderByDescending(s => s.TotalBonusMoney);
                                    else
                                        query = query.OrderBy(s => s.TotalBonusMoney);
                                    break;
                                case "totalbuycount":
                                    if (!string.IsNullOrEmpty(desc) && desc == "desc")
                                        query = query.OrderByDescending(s => s.TotalBuyCount);
                                    else
                                        query = query.OrderBy(s => s.TotalBuyCount);
                                    break;
                                case "bdfxcreatetime":
                                    if (!string.IsNullOrEmpty(desc) && desc == "desc")
                                        query = query.OrderByDescending(s => s.BDFXCreateTime);
                                    else
                                        query = query.OrderBy(s => s.BDFXCreateTime);
                                    break;
                            }
                        }
                        collection.TotalSingleTreasureList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        var time = ConfigurationManager.AppSettings["RedanTodayTime"];
                        var ts = DateTime.Now - Convert.ToDateTime(collection.FileCreateTime);
                        if (ts.TotalMilliseconds >= Convert.ToInt32(time))
                        {
                            collection = BuildRedanToday();
                            BuildYesterdayNR();
                        }
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// 生成今日神单
        /// </summary>
        private TotalSingleTreasure_Collection BuildRedanToday()
        {
            int pageIndex = 0;
            int pageSize = 3000;
            TotalSingleTreasure_Collection collection = WCFClients.ExternalClient.QueryTodayBDFXList("", string.Empty, "", string.Empty, string.Empty, DateTime.Now, DateTime.Now, "", pageIndex, pageSize);
            if (collection != null && collection.TotalCount > 0)
            {
                collection.FileCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "baodan", "redantoday");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var strJson = JsonSerializer.Serialize<TotalSingleTreasure_Collection>(collection);
                path = Path.Combine(path, "redantoday.json");
                System.IO.File.WriteAllText(path, strJson, Encoding.UTF8);
            }
            return collection;
        }

        /// <summary>
        /// 查询昨日牛人
        /// </summary>
        /// <returns></returns>
        public string QueryYesterdayNR()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "baodan", "redantoday", "nr.json");
            if (!System.IO.File.Exists(path))
                return string.Empty;

            return System.IO.File.ReadAllText(path, Encoding.UTF8);
        }

        /// <summary>
        /// 生成昨日牛人
        /// </summary>
        private void BuildYesterdayNR()
        {
            var strNR = WCFClients.ExternalClient.QueryYesterdayNR(DateTime.Now, DateTime.Now, 3);
            if (!string.IsNullOrEmpty(strNR))
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "baodan", "redantoday");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var strJson = JsonSerializer.Serialize<string>(strNR);
                path = Path.Combine(path, "nr.json");
                System.IO.File.WriteAllText(path, strJson, Encoding.UTF8);
            }
        }

        #region 订单详情相关

        public void DoClearOrderCacheFile(string schemeId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            foreach (var fileName in Directory.GetFiles(path))
            {
                System.IO.File.Delete(fileName);
            }
        }

        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return new Sports_SchemeQueryInfo();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_1.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameClient.QuerySportsSchemeInfo(schemeId);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && schemeInfo.ProgressStatus == ProgressStatus.Complate && (schemeInfo.BonusStatus == BonusStatus.Lose || schemeInfo.BonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<Sports_SchemeQueryInfo>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }

                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<Sports_SchemeQueryInfo>(content);
            }
        }

        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return new BettingOrderInfo();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_2.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameQueryClient.QueryOrderDetailBySchemeId(schemeId);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && schemeInfo.ProgressStatus == ProgressStatus.Complate && (schemeInfo.BonusStatus == BonusStatus.Lose || schemeInfo.BonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<BettingOrderInfo>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }

                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<BettingOrderInfo>(content);
            }
        }

        public List<Sports_TicketQueryInfo> QuerySportsTicketList(string schemeId, int pageIndex, int pageSize, string userToken, out int totalCount)
        {
            if (string.IsNullOrEmpty(schemeId))
            {
                totalCount = 0;
                return new List<Sports_TicketQueryInfo>();
            }
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_3.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var ticketList = WCFClients.GameClient.QuerySportsTicketList(schemeId, 0, -1, userToken);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache)
                {
                    var noPrizeCount = ticketList.TicketList.Where(p => p.BonusStatus == BonusStatus.Waitting).Count();
                    if (ticketList.TicketList.Count > 0 && noPrizeCount <= 0)
                    {
                        var content = JsonSerializer.Serialize<Sports_TicketQueryInfoCollection>(ticketList);
                        System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                    }
                }

                totalCount = ticketList.TotalCount;
                return ticketList.TicketList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                var list = JsonSerializer.Deserialize<Sports_TicketQueryInfoCollection>(content);
                totalCount = list.TotalCount;
                return list.TicketList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        public BDFXCommisionInfo QueryBDFXCommision(string schemeId, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new BDFXCommisionInfo();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_4.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.ExternalClient.QueryBDFXCommision(schemeId);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<BDFXCommisionInfo>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<BDFXCommisionInfo>(content);
            }
        }

        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId, string userToken, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new Sports_AnteCodeQueryInfoCollection();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_5.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var list = WCFClients.GameClient.QuerySportsOrderAnteCodeList(schemeId, userToken);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && list != null && list.Count > 0 && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<Sports_AnteCodeQueryInfoCollection>(list);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return list;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<Sports_AnteCodeQueryInfoCollection>(content);
            }
        }

        public SingleScheme_AnteCodeQueryInfo QuerySingleSchemeFullFileName(string schemeId, string userToken, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new SingleScheme_AnteCodeQueryInfo();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_6.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameClient.QuerySingleSchemeFullFileName(schemeId, userToken);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<SingleScheme_AnteCodeQueryInfo>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<SingleScheme_AnteCodeQueryInfo>(content);
            }
        }

        public OrderSingleSchemeCollection QueryOrderSingleScheme(string schemeId, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new OrderSingleSchemeCollection();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("ordinary_{0}_7.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameQueryClient.QuerySingSchemeDetail(schemeId);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<OrderSingleSchemeCollection>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<OrderSingleSchemeCollection>(content);
            }
        }

        public Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return new Sports_TogetherSchemeQueryInfo();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, string.Format("hmdetail_{0}_1.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameClient.QuerySportsTogetherDetail(schemeId);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && (schemeInfo.BonusStatus == BonusStatus.Lose || schemeInfo.BonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<Sports_TogetherSchemeQueryInfo>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<Sports_TogetherSchemeQueryInfo>(content);
            }
        }

        public Sports_TogetherJoinInfoCollection QuerySportsTogetherJoinList(string schemeId, string userToken, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new Sports_TogetherJoinInfoCollection();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("hmdetail_{0}_2.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var list = WCFClients.GameClient.QuerySportsTogetherJoinList(schemeId, -1, -1, userToken);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && list != null && list.TotalCount > 0 && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<Sports_TogetherJoinInfoCollection>(list);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return list;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<Sports_TogetherJoinInfoCollection>(content);
            }
        }

        public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string schemeId, string userToken)
        {
            if (string.IsNullOrEmpty(schemeId)) return new BettingOrderInfoCollection();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("chase_{0}_1.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameQueryClient.QueryBettingOrderListByChaseKeyLine(schemeId, userToken);
                var isComplate = true;
                while (true)
                {
                    if (schemeInfo == null)
                    {
                        isComplate = false;
                        break;
                    }
                    foreach (var issuse in schemeInfo.OrderList)
                    {
                        if (issuse.ProgressStatus != ProgressStatus.Complate)
                        {
                            isComplate = false;
                            break;
                        }
                    }
                    break;
                }
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && isComplate)
                {
                    var content = JsonSerializer.Serialize<BettingOrderInfoCollection>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<BettingOrderInfoCollection>(content);
            }
        }

        public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId, string userToken, BonusStatus bonusStatus)
        {
            if (string.IsNullOrEmpty(schemeId)) return new BettingAnteCodeInfoCollection();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "scheme", CreateFileName(schemeId));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, string.Format("chase_{0}_3.json", schemeId));
            if (!System.IO.File.Exists(filePath))
            {
                //查数据
                var schemeInfo = WCFClients.GameQueryClient.QueryAnteCodeListBySchemeId(schemeId, userToken);
                //写文件
                var isBuildCache = bool.Parse(ConfigurationManager.AppSettings["IsBuildCache"].ToString());
                if (isBuildCache && schemeInfo != null && (bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Win))
                {
                    var content = JsonSerializer.Serialize<BettingAnteCodeInfoCollection>(schemeInfo);
                    System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                }
                return schemeInfo;
            }
            else
            {
                //读文件
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<BettingAnteCodeInfoCollection>(content);
            }
        }

        public string CreateFileName(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return string.Empty;
            if (schemeId.Length < 10)
                return schemeId;
            var fileName = schemeId.Substring(0, 10);
            return fileName;
        }

        #endregion

        #endregion
    }
}