using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using Common.JSON;
using System.Runtime.Serialization;

namespace Common.Lottery.WinNumberGetters
{
    /// <summary>
    /// 开彩网
    /// 支持彩种：重庆时时彩 江西时时彩 江苏快三 上海11选5 山东11选5(十一运夺金) 江西11选5(多乐彩) 广东11选5
    /// </summary>
    public class WinNumberGetter_KaiCaiWang : WinNumberGetter
    {
        /// <summary>
        /// 采集地址
        /// </summary>
        //private const string _url = "http://c.apiplus.net/newly.do?token=f9e18eb66b794d91&code={0}&format=json";
        private const string _url = "http://101.37.126.96/newly.do?token=f9e18eb66b794d91&code={0}&format=json";
        //private const string _url = "http://d.apiplus.net:8888/daily.do?token=f9e18eb66b794d91&code={0}&format=json";

        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount, string issuseNumber = "")
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var url = string.Format(_url, FormatGameCode(gameCode));
                var json = PostManager.Get(url, Encoding.UTF8);
                var r = JsonSerializer.Deserialize<KaiCaiWangResult>(json);
                if (r.data != null && r.data.Length > 0)
                {
                    foreach (var item in r.data)
                    {
                        dic.Add(FormatIssuseNumber(gameCode, item.expect), FormatWinNumber(gameCode, item.opencode));
                    }
                }
            }
            catch (Exception)
            {
                return dic;
            }
            return dic;
        }

        /// <summary>
        /// 格式化彩种编码
        /// </summary>
        private string FormatGameCode(string gameCode)
        {
            switch (gameCode.ToUpper())
            {
                case "JSKS":
                    return "jsk3";
                default:
                    break;
            }
            return gameCode.ToLower();
        }

        /// <summary>
        /// 格式化开奖号码
        /// </summary>
        private string FormatWinNumber(string gameCode, string oldCode)
        {
            var winNumber = oldCode;
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                case "GDKLSF":
                case "JSKS":
                case "SDKLPK3":
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
        /// 格式化期号
        /// </summary>
        private string FormatIssuseNumber(string gameCode, string oldIssuseNumber)
        {
            var issuseNumber = oldIssuseNumber;
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                case "SDKLPK3":
                    issuseNumber = issuseNumber.Insert(8, "-");
                    break;

                case "JSKS":
                case "GDKLSF":
                    issuseNumber = issuseNumber.Insert(8, "-");
                    var t = issuseNumber.Split('-');
                    if (t.Length == 2)
                    {
                        issuseNumber = t[0] + "-" + t[1].Substring(1);
                    }
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
