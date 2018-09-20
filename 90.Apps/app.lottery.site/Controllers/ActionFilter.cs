using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using GameBiz.Core;
using Common.Net;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using Common.Log;
using Common.XmlAnalyzer;
using System.Text.RegularExpressions;
using app.lottery.site.iqucai;

namespace app.lottery.site.Controllers
{
    /// <summary>
    /// Action执行Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ActionFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
    {
        #region IActionFilter 成员

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //step2

            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //step1

            //throw new NotImplementedException();
        }

        #endregion

        #region IResultFilter 成员

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //step4

            //throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //step3

            //throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// CC检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CCFilterAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        private const int checkSenconds = 10;
        private const int maxRequestCount = 10;
        private static ILogWriter writer = Common.Log.LogWriterGetter.GetLogWriter();
        private static string[] myServerIpArray = new string[] { "115.231.153.109", "118.122.37.118" };
        private static List<string> saveLog = new List<string>();
        private const int maxSaveLogCount = 1000;
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //try
            //{
            //    var checkCC = bool.Parse(ConfigurationManager.AppSettings["CheckCC"].ToString());
            //    if (checkCC)
            //    {
            //        var clientIp = GetClientIp(filterContext.HttpContext.Request);
            //        if (!myServerIpArray.Contains(clientIp))
            //        {
            //            //不是服务器ip
            //            var url = filterContext.HttpContext.Request.Path;
            //            var key = string.Format("{0}-->{1}", clientIp, url);
            //            writer.Write("CCFilter", "OnActionExecuting", LogType.Information, "访问ip", key);

            //            saveLog.Add(key);

            //            //单位时间内 ip+url 访问次数大于N次，重定向到404页面
            //            if (filterContext.HttpContext.Session["lastCheckTime"] == null)
            //                filterContext.HttpContext.Session["lastCheckTime"] = DateTime.Now;

            //            var senconds = (DateTime.Now - (DateTime)filterContext.HttpContext.Session["lastCheckTime"]).TotalSeconds;
            //            if (senconds >= checkSenconds)
            //            {
            //                //开始检查
            //                var hasKeyArray = saveLog.Where(p => p == key).ToArray();
            //                if (hasKeyArray.Length >= maxRequestCount)
            //                {
            //                    saveLog.RemoveAll((a) =>
            //                    {
            //                        return hasKeyArray.Contains(a);
            //                    });
            //                    filterContext.Result = new RedirectResult("/404/404.html");
            //                }
            //            }
            //            if (saveLog.Count >= maxSaveLogCount)
            //            {
            //                saveLog.RemoveAt(0);
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    filterContext.Result = new RedirectResult("/404/404.html");
            //}
        }
        public string GetClientIp(HttpRequestBase request)
        {
            //取CDN用户真实IP的方法
            //当用户使用代理时，取到的是代理IP
            string result = String.Empty;
            result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(result))
            {
                //可能有代理  
                if (result.IndexOf(".") == -1)
                {
                    result = string.Empty;
                }
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。
                        result = result.Replace("  ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (temparyip[i].Substring(0, 3) != "10."
                                && temparyip[i].Substring(0, 7) != "192.168"
                                && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                result = temparyip[i];
                            }
                        }
                        string[] str = result.Split(',');
                        if (str.Length > 0)
                            result = str[0].ToString().Trim();
                    }
                    else
                        return result;
                }
            }

            if (string.IsNullOrEmpty(result))
                result = request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = request.UserHostAddress;
            if (string.IsNullOrEmpty(result) || result == "::1")
                result = "127.0.0.1";

            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ErrorHandleAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //filterContext.Result = new RedirectResult("/404/404.html");
            //filterContext.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// 推广员访问参数记录Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class UnionFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
    {
        #region IActionFilter 成员
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //step2

            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region ------ 记录推广员访问信息  ------
            try
            {
                var pid = "";
                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request["pid"]))
                {
                    var _decodeUserId = filterContext.HttpContext.Request["pid"];
                    if (filterContext.HttpContext.Request["pid"] != null)
                    {
                        _decodeUserId = WCFClients.GameClient.DecodeBase64(filterContext.HttpContext.Request["pid"]);
                    }
                    pid = _decodeUserId;
                    //pid = filterContext.HttpContext.Request["pid"];
                }
                else
                {
                    //var domain = filterContext.HttpContext.Request.Url.AbsoluteUri.ToLower();
                    var domain = "http://" + filterContext.HttpContext.Request.Url.Host;
                    pid = WCFClients.ExternalClient.QueryAgentUserIdByCustomerDomain(domain);

                    //var logWriter = LogWriterGetter.GetLogWriter();
                    //if (logWriter != null)
                    //{
                    //    logWriter.Write("LOG", "UnionFilterAttribute", LogType.Information, "OnActionExecuting", string.Format("用户访问域名 ：{0}", domain));
                    //}
                    //pid = GetAgentIdByHostName(domain);
                }

                if (!string.IsNullOrEmpty(pid))
                {
                    //pid = RemoveNotNumber(pid);

                    //写Session
                    filterContext.HttpContext.Session["pid"] = pid;

                    //转到主页
                    //filterContext.Result = new RedirectResult(Common.Net.UrlHelper.GetSiteRoot(filterContext.RequestContext.HttpContext.Request));

                    //写Session
                    //Dictionary<string, string> RegParamDic = new Dictionary<string, string>();
                    //RegParamDic.Add("UserComeFrom", "PROMOTION");
                    //RegParamDic.Add("UserId", pid);
                    //RegParamDic.Add("Referrer", pid);
                    //RegParamDic.Add("AgentId", pid);
                    //filterContext.HttpContext.Session["RegParam"] = RegParamDic;

                    ////写Cookie
                    //HttpCookie RegParamCookie = new HttpCookie("RegParamCookie");
                    //RegParamCookie.Values.Add("UserComeFrom", "PROMOTION");
                    //RegParamCookie.Values.Add("UserId", pid);
                    //RegParamCookie.Values.Add("Referrer", pid);
                    //RegParamCookie.Values.Add("AgentId", pid);
                    //RegParamCookie.Expires = DateTime.Now.AddMonths(1);
                    //filterContext.HttpContext.Response.AppendCookie(RegParamCookie);
                }

                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request["yqid"]))
                {
                    var _yqid = WCFClients.GameClient.DecodeBase64(filterContext.HttpContext.Request["yqid"]);
                    filterContext.HttpContext.Session["yqid"] = _yqid;
                }
            }
            catch (Exception ex)
            {
                var source = filterContext.HttpContext.Request.Url.AbsolutePath + "WEB";
                if (!filterContext.HttpContext.Request.IsLocal)
                {
                    source += "-" + IpManager.IPAddress;
                }
                WriteException(source, ex);
            }
            #endregion
        }

        #region 功能函数
        private static void WriteException(string source, Exception ex)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                logWriter.Write("Error", source, ex);
            }
        }

        private string GetSiteRoot(ActionExecutingContext filterContext)
        {
            return Common.Net.UrlHelper.GetSiteRoot(filterContext.HttpContext.Request);
        }

        /// <summary>  
        /// 去掉字符串中的非数字  
        /// </summary>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        private static string RemoveNotNumber(string key)
        {
            return Regex.Replace(key, @"[^\d]*", "");
        }

        /// <summary>  
        /// 去掉字符串中的数字  
        /// </summary>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        private static string RemoveNumber(string key)
        {
            return Regex.Replace(key, @"\d", "");
        }

        private string GetAgentIdByHostName(string host)
        {
            var agentMapping = WCFClients.GameClient.QueryCoreConfigByKey("AgentMapping").ConfigValue;
            if (!string.IsNullOrEmpty(agentMapping))
            {
                var mappingArray = agentMapping.Split('#');
                string[] mappingItem;
                foreach (var m in mappingArray.Where(m => !string.IsNullOrEmpty(m)))
                {
                    //zyu.xinticai.com|100133|className|新体网
                    mappingItem = m.Split('|');
                    if (mappingItem.Any(item => item.Trim().ToUpper() == host.Trim().ToUpper()))
                    {
                        return mappingItem[1];
                    }
                }
            }
            return string.Empty;
        }

        #endregion

        #endregion

        #region IResultFilter 成员

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //step4

            //throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //step3

            //throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 屏蔽某个页面或功能，并跳转到首页Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class RedirectFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
    {
        #region IActionFilter 成员

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //step2

            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //step1

            //throw new NotImplementedException();

            filterContext.Result = new RedirectResult(string.Format("/Home/Index"));
        }

        #endregion

        #region IResultFilter 成员

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //step4

            //throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //step3

            //throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 用户是否登录检测
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckLoginAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["CurrentUser"] == null)
            {
                //filterContext.Result = new RedirectResult(string.Format("/home/default?backurl={0}", filterContext.RequestContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
                filterContext.Result = new RedirectResult("/");
            }
        }
    }

    /// <summary>
    /// 用户是否是代理用户
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckIsAgentAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.Session["CurrentUser"] as External.Core.Login.LoginInfo;
            if (user == null)
            {
                //filterContext.Result = new RedirectResult(string.Format("/home/default?backurl={0}", filterContext.RequestContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
                filterContext.Result = new RedirectResult("/");
                return;
            }
            if (!user.IsAgent)
            {
                filterContext.Result = new RedirectResult("/member/safe");
            }
        }
    }


    /// <summary>
    /// 用户是否是名家
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    //public class CheckIsExperAttribute : FilterAttribute, IActionFilter
    //{
    //    public void OnActionExecuted(ActionExecutedContext filterContext)
    //    {
    //    }

    //    public void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var user = filterContext.HttpContext.Session["CurrentUser"] as External.Core.Login.LoginInfo;
    //        if (user == null)
    //        {
    //            filterContext.Result = new RedirectResult(string.Format("/home/default?backurl={0}", filterContext.RequestContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
    //            return;
    //        }
    //        if (!user.IsExperter)
    //        {
    //            filterContext.Result = new RedirectResult("/member/safe");
    //        }
    //    }
    //}

    /// <summary>
    /// 检测用户是否为推广员，如果没有登录则跳转到登录页面
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckIsPromoterAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["CurrentUser"] == null)
            {
                //filterContext.Result = new RedirectResult(string.Format("/home/default?backurl={0}", filterContext.RequestContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
                filterContext.Result = new RedirectResult("/");
            }
            else
            {
                if (filterContext.HttpContext.Session["IsPromoter"] == null || !bool.Parse(filterContext.HttpContext.Session["IsPromoter"].ToString()))
                {
                    filterContext.Result = new RedirectResult(string.Format("/union/joinpromotion"));
                }
            }
        }
    }

    /// <summary>
    /// 校验访问来源是否为浏览器软件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckBrowserAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //验证是否是爬虫软件或者是否是浏览器软件
            if (!IsCrawler() && IsBrowserGet())
            {
                if (!IsDefault(filterContext.HttpContext.Request["devicetype"]) && MobileDevice())
                {
                    //  
                    //HttpBrowserCapabilities bc = new HttpBrowserCapabilities();
                    //bc = HttpContext.Current.Request.Browser;
                    //filterContext.HttpContext.Response.Write("<div>浏览器：" + bc.Browser + "<br>" +
                    //                                         "浏览器版本：" + bc.Version + "<br>" +
                    //                                         "操作系统：" + bc.Platform + "<br>" +
                    //                                         "是否是搜索引擎的网络爬虫：" + bc.Crawler + "<br>" +
                    //                                         "是否是移动设备：" + bc.IsMobileDevice + "<br></div>");
                    filterContext.Result = new RedirectResult("http://m.wancai.com");
                    //filterContext.HttpContext.Response.Write(GetBrowser());
                }
            }
            //else
            //{
            //}
            //filterContext.HttpContext.Response.Write(GetBrowser());
        }

        #region 用户环境
        #region 浏览器
        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            //if (HttpContext.Current.Request.IsLocal) return true;
            //else if (CheckLimitIp()) return true;
            //else
            //{
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox", "safari", "chrome" };
            string curBrowser = GetBrowser().ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT"]) && curBrowser.IndexOf(BrowserName[i]) >= 0) return true;
            }
            return false;
            //}
        }

        /// <summary>
        /// 是否IP白名单
        /// </summary>
        /// <returns></returns>
        private static bool CheckLimitIp()
        {
            string limitIpList = ConfigurationManager.AppSettings["LimitIpList"];
            if (!string.IsNullOrEmpty(limitIpList) && limitIpList != "*")
            {
                System.Collections.Generic.List<string> ipList = new System.Collections.Generic.List<string>(limitIpList.Split(','));
                string ip = Common.Net.IpManager.IPAddress;
                return ipList.Contains(ip);
            }
            return true;
        }

        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                string[] strArray = new string[] { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
                string str = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (str.IndexOf(strArray[i]) >= 0) return true;
                }
            }
            return false;
        }

        #endregion

        #region 取操作系统
        /// <summary>
        /// 取操作系统
        /// </summary>
        /// <returns>返回操作系统</returns>
        public static string GetOS()
        {
            HttpBrowserCapabilities bc = new HttpBrowserCapabilities();
            bc = System.Web.HttpContext.Current.Request.Browser;
            return bc.Platform;
        }
        #endregion

        #region 取游览器
        /// <summary>
        /// 取游览器
        /// </summary>
        /// <returns>返回游览器</returns>
        public static string GetBrowser()
        {
            HttpBrowserCapabilities bc = new HttpBrowserCapabilities();
            bc = System.Web.HttpContext.Current.Request.Browser;
            return bc.Browser;
        }
        #endregion
        #region 取移动设备
        /// <summary>
        /// 取移动设备
        /// </summary>
        /// <returns>返回是否是移动设备</returns>
        public static bool MobileDevice()
        {
            String[] mobileAgents = { "iphone", "android", "phone", "mobile", "wap", "netfront", "java", "opera mobi", "opera mini", "ucweb", "windows ce", "symbian", "series", "webos", "sony", "blackberry", "dopod", "nokia", "samsung", "palmsource", "xda", "pieplus", "meizu", "midp", "cldc", "motorola", "foma", "docomo", "up.browser", "up.link", "blazer", "helio", "hosin", "huawei", "novarra", "coolpad", "webos", "techfaith", "palmsource", "alcatel", "amoi", "ktouch", "nexian", "ericsson", "philips", "sagem", "wellcom", "bunjalloo", "maui", "smartphone", "iemobile", "spice", "bird", "zte-", "longcos", "pantech", "gionee", "portalmmm", "jig browser", "hiptop", "benq", "haier", "^lct", "320x320", "240x320", "176x220", "w3c ", "acs-", "alav", "alca", "amoi", "audi", "avan", "benq", "bird", "blac", "blaz", "brew", "cell", "cldc", "cmd-", "dang", "doco", "eric", "hipt", "inno", "ipaq", "java", "jigs", "kddi", "keji", "leno", "lg-c", "lg-d", "lg-g", "lge-", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-", "moto", "mwbp", "nec-", "newt", "noki", "oper", "palm", "pana", "pant", "phil", "play", "port", "prox", "qwap", "sage", "sams", "sany", "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-", "siem", "smal", "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-", "tosh", "tsm-", "upg1", "upsi", "vk-v", "voda", "wap-", "wapa", "wapi", "wapp", "wapr", "webc", "winw", "winw", "xda", "xda-", "Googlebot-Mobile" };
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent))
            {
                if (mobileAgents.Contains(HttpContext.Current.Request.UserAgent.ToLower()))
                    return true;
            }
            return false;
        }
        #endregion
        /// <summary>
        /// 是否是从触屏版点击pc版跳转至 
        /// </summary>
        /// <returns>false是跳转至触屏版，</returns>
        public static bool IsDefault(string devicetype)
        {
            if (!string.IsNullOrEmpty(devicetype) && devicetype.ToLower() == "pc")
                return true;
            else
                return false;
        }

        public static bool IsCrawler()
        {
            HttpBrowserCapabilities bc = new HttpBrowserCapabilities();
            bc = System.Web.HttpContext.Current.Request.Browser;
            return bc.Crawler;

        }
        #endregion
    }
    /// <summary>
    /// 判断网站，是否是二级域名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckRefererAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
            var host = HttpContext.Current.Request.Url.Host;
            var pid = filterContext.HttpContext.Request["pid"];
            var myhost = "www.wancai.com";
            if (host != myhost || !string.IsNullOrEmpty(pid))
                filterContext.Result = new RedirectResult("http://www.wancai.com/error");
        }
    }
}
