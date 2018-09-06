using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters
{

    /// <summary>
    /// 务必初始化这个
    /// </summary>
    public class InitConfigInfo
    {

        public static string SZC_OPEN_URL { get; set; }
        public static string SZC_OPEN_URL_DAY { get; set; }
        //public static string SZC_OPEN_URL { get; set; }
        //public static string SZC_OPEN_URL { get; set; }
        //public static string SZC_OPEN_URL { get; set; }
        //public static string SZC_OPEN_URL { get; set; }

        public static ILoggerFactory logFactory { get; set; }
    }
}
