using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
        public static string ZHM_ServiceUrl { get; set; }
        
        public static string ZHM_Key { get; set; }
        public static string ZHM_PartnerId { get; set; }
        public static string ZHM_Version { get; set; }

        public static string SZC_OPEN_MIRROR_URL { get; set; }

        public static ILoggerFactory logFactory { get; set; }

        /// <summary>
        /// 通知站点构建静态数据
        /// </summary>
        public static string BuildStaticFileSendUrl { get; set; }

        public static void Init(JToken cf) {

            SZC_OPEN_URL=cf["SZC_OPEN_URL"] == null ? "" : cf["SZC_OPEN_URL"].ToString();
            SZC_OPEN_URL_DAY = cf["SZC_OPEN_URL_DAY"] == null ? "" : cf["SZC_OPEN_URL_DAY"].ToString();

            ZHM_ServiceUrl = cf["ZHM_ServiceUrl"] == null ? "" : cf["ZHM_ServiceUrl"].ToString();
            ZHM_Key = cf["ZHM_Key"] == null ? "" : cf["ZHM_Key"].ToString();
            ZHM_PartnerId = cf["ZHM_PartnerId"] == null ? "" : cf["ZHM_PartnerId"].ToString();
            ZHM_Version = cf["ZHM_Version"] == null ? "" : cf["ZHM_Version"].ToString();
            SZC_OPEN_MIRROR_URL = cf["SZC_OPEN_MIRROR_URL"] == null ? "" : cf["SZC_OPEN_MIRROR_URL"].ToString();
         

            //  SZC_OPEN_URL = cf["SZC_OPEN_URL"] ??"";
        }
    }
}
