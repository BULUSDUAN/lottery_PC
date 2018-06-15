using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaSon.FrameWork.Helper;
using KaSon.FrameWork.Helper.Net;
using Microsoft.AspNetCore.Mvc;


namespace Lottery.Base.Controllers
{
 
   // [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class BaseController : Controller
    {

        IKgLog log = null;
        public BaseController() {
            log = new Log4Log();
        }
        protected JsonResult JsonEx(object data)
        {
            JsonResult result = new JsonResult(data);
            //result.Data = data;
            //result.ContentType = contentType;
            //result.ContentEncoding = contentEncoding;
            //result.JsonRequestBehavior = behavior;
            return result;
        }
        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        public  void Log(string name,Exception ex)
        {
            log.Log(name,ex);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public  void Log(string msg)
        {
            log.Log(msg);
        }
        public static string GetJsonData(string url)
        {
            try
            {
                var domain = ConfigHelper.ConfigInfo["SelfDomain"] ?? "http://www.iqucai.com";
                url = domain + url;
                if (string.IsNullOrEmpty(url))
                    return string.Empty;
                var result = KaSon.FrameWork.Helper.PostManager.Get(url, Encoding.UTF8);
                if (result == "404") return string.Empty;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
