using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using app.lottery.site.Controllers;

namespace app.lottery.site.iqucai.api
{
    public class CommonAPI
    {
        #region 是否开启新的验证码
        /// <summary>
        /// 是否开启新的验证码
        /// </summary>
        public static string QueryCoreConfigByKey()
        {
            try
            {
                var coreConfigInfo = WCFClients.GameClient.QueryCoreConfigByKey("ValidataCode.GateWay").ConfigValue;// WCFClients.GameClient.QueryCoreConfigByKey("ValidataCode.GateWay");
                return string.IsNullOrEmpty(coreConfigInfo) ? "Geetest" : coreConfigInfo;
            }
            catch
            {
                return "Geetest";//Default Geetestss
            }
        }
        #endregion


        /// <summary>
        /// 格式化IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string FormatIP(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return "";
            string[] arr = ip.Split('.');
            if (arr.Length == 4)
            {
                return string.Format("{0}.{1}.*.*", arr[0], arr[1]);
            }
            else
            {
                return ip;
            }

        }
    }
}