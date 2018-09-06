using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Common.Communication;
using Common.JSON;
using Common.Net;
using Common.Utilities;
using log4net;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///     开彩网
    ///     支持彩种：重庆时时彩 江西时时彩 江苏快三 上海11选5 山东11选5(十一运夺金) 江西11选5(多乐彩) 广东11选5
    /// </summary>
    public class WinNumberGetter_KaiCaiWang : WinNumberGetter
    {
        /// <summary>
        ///     采集地址
        /// </summary>
        private static readonly string API_URL = AppSettingsHelper.GetString("SZC_OPEN_URL")
            ; //  "http://c.apiplus.cn/newly.do?token=f9e18eb66b794d91&code={0}&format=json&random={1}";

        private static readonly string API_URL_DAY = AppSettingsHelper.GetString("SZC_OPEN_URL_DAY")
            ; //"http://c.apiplus.cn/daily.do?token=f9e18eb66b794d91&code={0}&format=json&random={1}&date={2}";

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //private readonly static Cache cache = new Cache(new Common.Utilities.ThreadPool());
        private static DateTime lastTime = DateTime.Now; // = new ConcurrentDictionary<string, DateTime>();

        private static readonly object lockObject = new object();


        private static Cache cache => HttpWcfClient.DefaultCache;


        private int Parse(string gameCode, string json, Dictionary<string, string> dic)
        {
            var r = JsonHelper.Deserialize<KaiCaiWangResult>(json);
            var cnt = 0;
            if (r != null && r.data != null && r.data.Length > 0)
                foreach (var item in r.data)
                {
                    var k = FormatIssuseNumber(gameCode, item.expect);
                    var v = FormatWinNumber(gameCode, item.opencode);
                    if (dic.ContainsKey(k)) continue;
                    dic[k] = v;

                    cnt++;
                }
            return cnt;
        }

        private void deay()
        {
            while (true)
            {
                TimeSpan span;
                lock (lockObject)
                {
                    span = DateTime.Now - lastTime;
                    if (span > TimeSpan.FromSeconds(5))
                    {
                        lastTime = DateTime.Now;
                        return;
                    }
                }
                Thread.Sleep(span);
            }
        }

        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber = "")
        {
            deay();
            var dic = new Dictionary<string, string>();
            //add by giant：添加参数防止服务器缓存结果
            //TODO:防止采集过快
            //?code=cqssc&date=2016-11-19
            var url = string.Format(API_URL, gameCode.ToLower(), DateTime.Now.Ticks);
            if (Parse(gameCode, PostManagerWithProxy.Get(url, Encoding.UTF8), dic) == 0)
                return dic;
            //21分钟采集一次
            if (cache.ExistKey(gameCode, TimeSpan.FromMinutes(49))) //, TimeSpan.FromMinutes(49), 0))
                return dic;
            var succeed = true;
            //自动补3天的数据
            foreach (var dt in new[]
            {
                DateTime.Now,
                DateTime.Now - TimeSpan.FromDays(1)
            })
            {
                url = string.Format(API_URL_DAY, gameCode.ToLower(), DateTime.Now.Ticks, dt.ToString("yyyy-MM-dd"));
                Thread.Sleep(TimeSpan.FromSeconds(5));
                deay();
                if (Parse(gameCode, PostManagerWithProxy.Get(url, Encoding.UTF8), dic) == 0)
                    succeed = false;
            }
            if (succeed)
                cache.SetKey(gameCode, TimeSpan.FromMinutes(49), 0);
            return dic;
        }

        /// <summary>
        ///     格式化开奖号码
        /// </summary>
        private string FormatWinNumber(string gameCode, string oldCode)
        {
            var winNumber = oldCode;
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                case "JX11X5":
                case "FC3D":
                case "PL3":
                    break;
                case "SSQ":
                case "DLT":
                    winNumber = winNumber.Replace("+", "|");
                    break;

                default:
                    break;
            }
            return winNumber;
        }


        /// <summary>
        ///     格式化期号
        /// </summary>
        private string FormatIssuseNumber(string gameCode, string oldIssuseNumber)
        {
            var issuseNumber = oldIssuseNumber;
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                case "JX11X5":
                case "GD11X5":
                case "SD11X5":
                    issuseNumber = issuseNumber.Insert(8, "-");
                    break;
                case "SSQ":
                case "DLT":
                case "FC3D":
                case "PL3":
                    issuseNumber = issuseNumber.Insert(4, "-");
                    break;
                default:
                    break;
            }
            return issuseNumber;
        }

        [DataContract]
        internal class KaiCaiWangInfo
        {
            [DataMember]
            public string expect { get; set; }

            [DataMember]
            public string opencode { get; set; }

            [DataMember]
            public string opentime { get; set; }

            [DataMember]
            public string opentimestamp { get; set; }
        }

        [DataContract]
        internal class KaiCaiWangResult
        {
            [DataMember]
            public string rows { get; set; }

            [DataMember]
            public string code { get; set; }

            [DataMember]
            public string remain { get; set; }

            [DataMember]
            public KaiCaiWangInfo[] data { get; set; }
        }
    }
}