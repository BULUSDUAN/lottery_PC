﻿using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace KaSon.FrameWork.Common.Net
{
    public class PostManagerWithProxy
    {
    //    private  readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ILogger<PostManagerWithProxy> logger = InitConfigInfo.logFactory.CreateLogger<PostManagerWithProxy>();

        public  static string buildProxyUrl(string url)
        {
            var proxy = InitConfigInfo.data_spider_proxy_url;
            if (string.IsNullOrEmpty(proxy))
                return url;
            var vsign = Encipherment.MD5(url + "aaa222");
            var tmp = proxy + "?url=" + HttpUtility.UrlEncode(url) + "&sign=" + vsign;

            return tmp;
        }

        public static string Post(string url, string requestString, Encoding encoding,
            Action<HttpWebRequest> requestHandler = null)
        {
            var content = PostManager.PostCustomer(buildProxyUrl(url), requestString, encoding, requestHandler);
            if (string.IsNullOrEmpty(content))
            {
                logger.LogError("下载失败:" + url);
                return string.Empty;
            }
            return content;
        }
        public static string Post_Head(string url, string requestString, Encoding encoding, 
            string contentType = "application/x-www-form-urlencoded", string refer = "", WebHeaderCollection Head = null)
        {
            var content = PostManager.Post_Head(buildProxyUrl(url), requestString, encoding, 0,null, contentType, refer, Head);
            if (string.IsNullOrEmpty(content))
            {
                logger.LogError("下载失败:" + url);
                return string.Empty;
            }
            return content;
        }
        

        public static string Get(string url, Encoding encoding, int timeout = 0,
            Action<HttpWebRequest> requestHandle = null)
        {
            var content = PostManager.Get(buildProxyUrl(url), encoding, timeout, requestHandle);
            if (string.IsNullOrEmpty(content))
            {
                logger.LogError("下载失败:" + url);
                return string.Empty;
            }
            return content;
        }
    }
}