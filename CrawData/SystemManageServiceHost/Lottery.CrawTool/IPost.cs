using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lottery.CrawTool
{
    public interface IPostPriver
    {
        /// <summary>
        /// 获取Cookies
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
         CookieContainer GetCookie(string url);
    }
}
