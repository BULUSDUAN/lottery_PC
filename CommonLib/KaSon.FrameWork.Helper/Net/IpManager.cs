
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Helper.Net
{
    public static class IpManager
    {
        private static String ipUrl = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json";
        private static String China = "\u4e2d\u56fd";
        private static String Taiwan = "\u53f0\u6e7e";
        private static String HongKong = "\u9999\u6e2f";
        private static String Macao = "\u6fb3\u95e8";
        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址
        /// </summary>
        public static string IPAddress
        {
            get
            {
                //String srcIp = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                String srcIp = MyHttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"];
                if (srcIp == null)
                {
                    //回传IP地址
                    srcIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                // srcIp = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];
                else if (srcIp.Contains(","))
                {
                    srcIp = srcIp.Split(',')[0];
                }
                return srcIp;


               
            }
        }
    }
    }
