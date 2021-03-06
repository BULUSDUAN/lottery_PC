﻿
using KaSon.FrameWork.Common.JSON;
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

        /// <summary>
        /// 获取ip的名称
        /// </summary>
        public static IpInfo GetIpDisplayname_Sina(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return new IpInfo();
            var json = PostManager.Get(string.Format("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip={0}", ip), Encoding.UTF8);
            try
            {
                //json = json.Substring(json.IndexOf('{')).TrimEnd(';');
                var obj = JsonSerializer.Deserialize<IpInfo>(json);
                return obj;
            }
            catch (Exception ex)
            {
                //ex = new Exception("获取IP出错 - IP:" + ip + ";JSON结果:" + json, ex);
                //LogWriterGetter.GetLogWriter().Write("Error", "GetIpDisplayname_Sina", ex);
            }
            return null;
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
    }
