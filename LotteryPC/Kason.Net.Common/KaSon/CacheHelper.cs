using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kason.Net.Common
{
    class CacheHelper
    {
        /// <summary>
        /// 设定绝对的过期时间
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="seconds">超过多少秒后过期</param>
        //public static void SetCacheDateTime(string CacheKey, object objObject, long Seconds)
        //{
        //    System.Web.Caching.Cache objCache = HttpRuntime.Cache;
        //    objCache.Insert(CacheKey, objObject, null, System.DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero);
        //}
        /// <summary>
        /// 设置当前应用程序指定包含相对过期时间Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="timeSpan">超过多少时间不调用就失效，单位是秒</param>
        //public static void SetCacheTimeSpan(string CacheKey, object objObject, long timeSpan)
        //{
        //    System.Web.Caching.Cache objCache = HttpRuntime.Cache;
        //    objCache.Insert(CacheKey, objObject, null, DateTime.MaxValue, TimeSpan.FromSeconds(timeSpan));
        //}
    }
}
