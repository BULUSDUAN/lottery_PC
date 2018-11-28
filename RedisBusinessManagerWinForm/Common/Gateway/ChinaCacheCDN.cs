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
using Common.Net;
using System.Xml;

namespace Common.Gateway.CDN
{
    public static class ChinaCacheCDN
    {
        private static string username = "username";
        private static string psd = "password";
        private static string postUrl3 = "http://ccms.chinacache.com/index.jsp";
        /// <summary>
        /// 刷新cdn接口
        /// </summary>
        /// <param name="urls">需要刷新的url路径，可同时提交多条，每条请用换行隔开。每小时最多3000条，每天最多72000条。</param>
        /// <param name="dirs">需要刷新的目录路径，可同时提交多条，每条请用换行隔开。每10分钟最多10条目录刷新。</param>
        /// <returns></returns>
        public static string UpdateCDN(string urls, string dirs)
        {
            try
            {
                if (string.IsNullOrEmpty(urls) && string.IsNullOrEmpty(dirs))
                {
                    throw new Exception("请输入需要刷新的url地址或目录");
                }

                //第三版本刷新CDN
                string task = (string.IsNullOrEmpty(urls) ? string.Empty : "urls=" + urls) + (string.IsNullOrEmpty(dirs) ? string.Empty : (string.IsNullOrEmpty(urls) ? string.Empty : "&") + "dirs=" + dirs);
                string paraString = "ok=ok&user=" + username + "&pswd=" + psd + "&" + task;

                string postResult = PostManager.Post(postUrl3, paraString, Encoding.UTF8);
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(postResult);
                if (xdoc.SelectSingleNode("result").InnerText == "failed")
                {
                    throw new Exception("刷新CDN失败，请稍候再试");
                }
                var url = xdoc.SelectNodes("result/url");
                var dir = xdoc.SelectNodes("result/dir");
                return string.Format("CDN刷新成功。成功URL：[{0}]，成功目录：[{1}]", url, dir);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
