using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using Common.Utilities;
using Common.Log;
using Common.JSON;

namespace Common.Net
{
    public static class IpManager
    {
        private static String ipUrl = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json";
        private static String China = "\u4e2d\u56fd";
        private static String Taiwan = "\u53f0\u6e7e";
        private static String HongKong = "\u9999\u6e2f";
        private static String Macao = "\u6fb3\u95e8";


        /// <summary>
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址
        /// </summary>
        public static string IPAddress
        {
            get
            {
                //String srcIp = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                String srcIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (srcIp == null)
                {
                    srcIp = HttpContext.Current.Request.UserHostAddress;
                }
                // srcIp = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];
                else if (srcIp.Contains(","))
                {
                    srcIp = srcIp.Split(',')[0];
                }
                return srcIp;


                //取CDN用户真实IP的方法
                //当用户使用代理时，取到的是代理IP
                string result = String.Empty;
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
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
                                if (ValidateHelper.IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
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
                        else if (ValidateHelper.IsIPAddress(result))
                            return result;
                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;
                if (string.IsNullOrEmpty(result) || result == "::1")
                    result = "127.0.0.1";

                return result;
            }
        }

        /// <summary>
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址
        /// </summary>
        //public static string IPAddress
        //{
        //    get
        //    {
        //        var x_forwarded = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //        var remote = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        string result = string.Empty;
        //        if (!string.IsNullOrEmpty(x_forwarded))
        //        {
        //            result = x_forwarded;
        //            //可能有代理
        //            if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式
        //                result = string.Empty;
        //            else
        //            {
        //                if (result.IndexOf(",") != -1)
        //                {
        //                    //有“,”，估计多个代理。取第一个不是内网的IP。
        //                    result = result.Replace(" ", "").Replace("'", "");
        //                    string[] temparyip = result.Split(",;".ToCharArray());
        //                    for (int i = 0; i < temparyip.Length; i++)
        //                    {
        //                        if (ValidateHelper.IsIPAddress(temparyip[i])
        //                            && temparyip[i].Substring(0, 3) != "10."
        //                            && temparyip[i].Substring(0, 7) != "192.168"
        //                            && temparyip[i].Substring(0, 7) != "172.16.")
        //                        {
        //                            return temparyip[i];    //找到不是内网的地址
        //                        }
        //                    }
        //                }
        //                else if (ValidateHelper.IsIPAddress(result)) //代理即是IP格式 ,IsIPAddress判断是否是IP的方法,
        //                    return result;
        //                else
        //                    result = string.Empty;    //代理中的内容 非IP，取IP
        //            }
        //        }

        //        if (string.IsNullOrEmpty(result))
        //            result = remote;
        //        if (string.IsNullOrEmpty(result))
        //            result = HttpContext.Current.Request.UserHostAddress;
        //        if (string.IsNullOrEmpty(result) || result == "::1")
        //            result = "127.0.0.1";

        //        return result;
        //    }
        //}

        /// <summary>
        /// 获取ip的名称
        /// </summary>
        public static IpInfo GetIpDisplayname_Sina(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return new IpInfo();
            var json = Common.Net.PostManager.Get(string.Format("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip={0}", ip), Encoding.UTF8);
            try
            {
                //json = json.Substring(json.IndexOf('{')).TrimEnd(';');
                var obj = JsonSerializer.Deserialize<IpInfo>(json);
                return obj;
            }
            catch (Exception ex)
            {
                ex = new Exception("获取IP出错 - IP:" + ip + ";JSON结果:" + json, ex);
                LogWriterGetter.GetLogWriter().Write("Error", "GetIpDisplayname_Sina", ex);
            }
            return null;
        }

        /// <summary>
        /// 是否为中国IP
        /// </summary>
        public static bool IpIsChinaInland(string ip)
        {
            var info = GetIpDisplayname_Sina(ip);
            if (info == null) return false;
            if (string.IsNullOrEmpty(info.country))
                return false;
            if (!info.country.Equals(China))
                return false;
            //if (string.IsNullOrEmpty(info.province))
            //    return false;
            //if (!info.province.Equals(Taiwan) && !info.province.Equals(HongKong) && !info.province.Equals(Macao))
            //    return false;

            return true;
        }
    }

    /// <summary>
    /// IP地址对象
    /// </summary>
    public class IpInfo
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", country, province, city);
        }
    }
}
