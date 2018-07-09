
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Net
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
        /// 获取客户Ip
        /// </summary>
        public static string GetClientUserIp(this HttpContext context)
        {

            //String srcIp = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;

        }
    }
    }
