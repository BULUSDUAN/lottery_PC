using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Common.Lottery.WinNumberGetters
{
    internal class WinNumberGetter_Get_ShiShiCai : WinNumberGetter
    {
        //重庆时时彩
        private const string urlCQSSC = @"http://video.shishicai.cn/haoma/cqssc/list/120.aspx";
        //江西时时彩
        private const string urlJXSSC = @"http://video.shishicai.cn/haoma/jxssc/list/84.aspx";
        //山东11选5
        private const string urlSD11X5 = @"http://video.shishicai.cn/haoma/sd11x5/list/65.aspx";
        //广东11选5
        private const string urlGD11X5 = @"http://video.shishicai.cn/haoma/gd11x5/list/65.aspx";
        //江西11选5
        private const string urlJX11X5 = @"http://video.shishicai.cn/haoma/jx11x5/list/65.aspx";


        //广东快乐十分
        private const string urlGDKL10F = @"http://video.shishicai.cn/haoma/gdkl10/list/84.aspx";
        //广西快乐十分
        private const string urlGXKL10F = @"http://video.shishicai.cn/haoma/gxkl10/list/50.aspx";
        //山东群英会
        private const string urlSDQYH = @"http://video.shishicai.cn/haoma/sdqyh/list/40.aspx";


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(gameName) || lastIssuseCount == 0)
            {
                return dic;
            }
            string url = DatermineURL(gameName);
            string json = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
            {
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.3) Gecko/20100401 Firefox/3.6.3";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            });
            string strTemp = "var listIssue = ";
            int index = json.IndexOf(strTemp) + strTemp.Length;
            json = json.Substring(index, json.Length - index);
            index = json.IndexOf("];") + 1;
            json = json.Substring(0, index);
            var result = Deserialize(json);
            return DicResult(result, gameName, lastIssuseCount);
        }

        private Dictionary<string, string> DicResult(NumberInfo[] result, string gameName, int lastIssuseCount)
        {
            if (gameName.Equals("JXSSC") || gameName.Equals("SDQYH"))
            {
                return ErgodicResult(result, gameName, lastIssuseCount);
            }
            else
            {
                return ErgodicResult(result, lastIssuseCount);
            }
        }

        private Dictionary<string, string> ErgodicResult(NumberInfo[] result, string gameName, int lastIssuseCount)
        {
            var dic = new Dictionary<string, string>();
            foreach (NumberInfo item in result)
            {
                if (dic.Count == lastIssuseCount)
                    return dic;
                dic.Add(item.IssueNumber.Remove(9, 1), item.BonusNumberString.Substring(0, item.BonusNumberString.IndexOf("|")));
            }
            return dic;
        }

        private Dictionary<string, string> ErgodicResult(NumberInfo[] result, int lastIssuseCount)
        {
            var dic = new Dictionary<string, string>();
            foreach (NumberInfo item in result)
            {
                if (dic.Count == lastIssuseCount)
                    return dic;
                dic.Add(item.IssueNumber, item.BonusNumberString.Substring(0, item.BonusNumberString.IndexOf("|")));
            }
            return dic;
        }

        private string DatermineURL(string gameName)
        {
            string url = string.Empty;
            switch (gameName)
            {
                case "CQSSC":
                    url = urlCQSSC;
                    break;
                case "JXSSC":
                    url = urlJXSSC;
                    break;
                case "SD11X5":
                    url = urlSD11X5;
                    break;
                case "GD11X5":
                    url = urlGD11X5;
                    break;
                case "JX11X5":
                    url = urlJX11X5;
                    break;

                case "GDKLSF":
                    url = urlGDKL10F;
                    break;
                case "GXKLSF":
                    url = urlGXKL10F;
                    break;
                case "SDQYH":
                    url = urlSDQYH;
                    break;
            }
            return url;
        }

        private NumberInfo[] Deserialize(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NumberInfo[]));
                ms.Position = 0;
                var result = (NumberInfo[])ser.ReadObject(ms);
                return result;
            }
        }
    }
    [DataContract]
    public class NumberInfo
    {
        [DataMember]
        public string BonusNumberString { get; set; }
        [DataMember]
        public string BonusTime { get; set; }
        [DataMember]
        public string IssueNumber { get; set; }
    }

}
