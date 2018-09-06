using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Lottery.CrawGetters
{
    internal class WinNumberGetter_CaiLeLe : WinNumberGetter
    {
        //重庆时时彩
        private const string urlCQSSC = @"http://www.cailele.com/static/termInfo/150.txt?_={0}";

        //江西时时彩
        private const string urlJXSSC = @"http://www.cailele.com/static/termInfo/153.txt?_={0}";

        //山东11选5
        private const string urlSD11X5 = @"http://www.cailele.com/static/termInfo/50.txt?_={0}";

        //广东11选5
        private const string urlGD11X5 = @"http://www.cailele.com/static/termInfo/53.txt?_={0}";

        //江西11选5
        private const string urlJX11X5 = @"http://www.cailele.com/static/termInfo/52.txt?_={0}";


        //广东快乐十分
        private const string urlGDKL10F = @"http://www.cailele.com/static/termInfo/152.txt?_={0}";

        //todo:广西快乐十分
        private const string urlGXKL10F = @"http://kjh.cailele.com/history_gxklsf.aspx?_={0}";

        //山东群英会
        private const string urlSDQYH = @"http://www.cailele.com/static/termInfo/154.txt?_={0}";

        //福彩3D
        private const string urlFC3D = @"http://www.cailele.com/static/termInfo/102.txt?v={0}";

        //排列三
        private const string urlPL3 = @"http://www.cailele.com/static/termInfo/3.txt?v={0}";

        //双色球
        private const string urlSSQ = @"http://www.cailele.com/static/termInfo/100.txt?v={0}";

        //大乐透
        private const string urlDLT = @"http://www.cailele.com/static/termInfo/1.txt?v={0}";

        //todo:上海时时乐
        private const string urlSHSSL = @"http://kjh.cailele.com/history_shssl.aspx?menu=mcl_4&_={0}";

        //江苏快3
        private const string urlJSK3 = @"http://www.cailele.com/static/termInfo/157.txt?_={0}";

        //重庆11选5
        private const string urlCQ11X5 = @"http://www.cailele.com/static/termInfo/54.txt?_={0}";

        //辽宁11选5
        private const string urlLN11X5 = @"http://www.cailele.com/static/termInfo/55.txt?_={0}";


        //重庆快乐十分
        private const string urlCQKL10F = @"http://www.cailele.com/static/termInfo/158.txt?_={0}";

        //湖南快乐十分
        private const string urlHNKL10F = @"http://www.cailele.com/static/termInfo/160.txt?_={0}";

        //新快3
        private const string urlJLK3 = @"http://www.cailele.com/static/termInfo/159.txt?_={0}";

        //湖北快3
        private const string urlHBK3 = @"http://www.cailele.com/static/termInfo/161.txt?_={0}";

        //排列五
        private const string urlPL5 = @"http://www.cailele.com/static/termInfo/4.txt?v={0}";

        //七乐彩
        private const string urlQLC = @"http://www.cailele.com/static/termInfo/101.txt?v={0}";

        //七星彩
        private const string urlQXC = @"http://www.cailele.com/static/termInfo/P2.txt?v={0}";

        //华东15选5
        private const string url15X5 = @"http://www.cailele.com/static/termInfo/103.txt?v={0}";

        //好彩1
        private const string urlHC1 = @"http://www.cailele.com/static/termInfo/106.txt?v={0}";

        //东方6+1
        private const string urlDF6J1 = @"http://www.cailele.com/static/termInfo/105.txt?v={0}";


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var url = GetUrl(gameName);

            var xml = PostManager.Get(url, Encoding.UTF8, 0,
                request => { request.Headers.Add("Upgrade-Insecure-Requests", "1"); });

            var info = JsonHelper.Deserialize<CaiLeleOpenInfo>(xml);
            dic.Add(FomartIssuseNumber(gameName, info.openTerm), FomartWinNumber(gameName, info.openResult));

            return dic;
        }

        private string FomartIssuseNumber(string gameCode, string issuseNumber)
        {
            switch (gameCode)
            {
                case "CQSSC":
                case "JX11X5":
                case "CQ11X5":
                case "LN11X5":

                case "GDKLSF":
                case "CQKLSF":
                    return issuseNumber.Insert(8, "-");
                case "FC3D":
                case "SSQ":
                case "QLC":
                case "HD15X5":
                case "HC1":
                case "DF6J1":
                    return issuseNumber.Insert(4, "-");
                case "PL3":
                case "PL5":
                case "QXC":
                case "DLT":
                    return "20" + issuseNumber.Insert(2, "-");
                case "HNKLSF":
                    return string.Format("{0}-{1}", issuseNumber.Substring(0, 8), issuseNumber.Substring(9, 2));
                case "GD11X5":
                    return string.Format("{0}-{1}", issuseNumber.Substring(0, 8), issuseNumber.Substring(8, 2));
                case "JXSSC":
                    return string.Format("{0}-{1}", issuseNumber.Substring(0, 8), issuseNumber.Substring(9, 2));
                case "SD11X5":
                    return string.Format("20{0}-{1}", issuseNumber.Substring(0, 6), issuseNumber.Substring(6, 2));
                case "JSKS":
                case "JLK3":
                case "HBK3":
                    return string.Format("20{0}-{1}", issuseNumber.Substring(0, 6), issuseNumber.Substring(7, 2));
                case "SDQYH":
                    return string.Format("20{0}-{1}", issuseNumber.Substring(0, 6), issuseNumber.Substring(8, 2));
                default:
                    break;
            }
            return issuseNumber;
        }

        private string FomartWinNumber(string gameCode, string winNumber)
        {
            switch (gameCode)
            {
                case "CQSSC":
                case "JXSSC":

                case "CQ11X5":
                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                case "LN11X5":

                case "GDKLSF":
                case "CQKLSF":
                case "SDQYH":
                case "HNKLSF":
                case "JSKS":
                case "JLK3":
                case "HBK3":

                case "FC3D":
                case "PL3":
                case "PL5":
                case "HD15X5":
                case "QXC":
                    break;
                case "HC1":
                    var hc1Array = winNumber.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    return hc1Array[0];
                case "SSQ":
                    var ssqArray = winNumber.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (ssqArray.Length != 7)
                        throw new LogicException("双色球应该是由 , 分隔的7位数");
                    var ssqQian = string.Join(",", ssqArray.Take(6).ToArray());
                    var ssqHou = ssqArray[6];
                    return string.Join("|", ssqQian, ssqHou);
                case "DF6J1":
                    var df6j1Array = winNumber.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (df6j1Array.Length != 7)
                        throw new LogicException("东方六加一应该是由 , 分隔的7位数");
                    var df6j1Qian = string.Join(",", df6j1Array.Take(6).ToArray());
                    var df6j1Hou = df6j1Array[6];
                    return string.Join("|", df6j1Qian, df6j1Hou);
                case "QLC":
                    var qlcArray = winNumber.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (qlcArray.Length != 8)
                        throw new LogicException("七乐彩应该是由 , 分隔的8位数");
                    var qlcQian = string.Join(",", qlcArray.Take(7).ToArray());
                    var qlcHou = qlcArray[7];
                    return string.Join("|", qlcQian, qlcHou);
                case "DLT":
                    var dltArray = winNumber.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (dltArray.Length != 7)
                        throw new LogicException("双色球应该是由 , 分隔的7位数");
                    var dltQian = string.Join(",", dltArray.Take(5).ToArray());
                    var dltHou = string.Join(",", dltArray.Skip(5).Take(2).ToArray());
                    return string.Join("|", dltQian, dltHou);
                default:
                    break;
            }
            return winNumber;
        }

        private string GetUrl(string gameName)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;

            var url = string.Empty;
            switch (gameName)
            {
                case "CQSSC":
                    url = string.Format(urlCQSSC, tt);
                    break;
                case "JXSSC":
                    url = string.Format(urlJXSSC, tt);
                    break;
                case "SD11X5":
                    url = string.Format(urlSD11X5, tt);
                    break;
                case "GD11X5":
                    url = string.Format(urlGD11X5, tt);
                    break;
                case "JX11X5":
                    url = string.Format(urlJX11X5, tt);
                    break;
                case "GDKLSF":
                    url = string.Format(urlGDKL10F, tt);
                    break;
                case "GXKLSF":
                    url = string.Format(urlGXKL10F, tt);
                    break;
                case "SDQYH":
                    url = string.Format(urlSDQYH, tt);
                    break;
                case "FC3D":
                    url = string.Format(urlFC3D, tt);
                    break;
                case "PL3":
                    url = string.Format(urlPL3, tt);
                    break;
                case "SSQ":
                    url = string.Format(urlSSQ, tt);
                    break;
                case "DLT":
                    url = string.Format(urlDLT, tt);
                    break;
                case "SHSSL":
                    url = string.Format(urlSHSSL, tt);
                    break;
                case "JSKS":
                    url = string.Format(urlJSK3, tt);
                    break;

                case "CQ11X5":
                    url = urlCQ11X5;
                    break;
                case "LN11X5":
                    url = urlLN11X5;
                    break;
                case "CQKLSF":
                    url = urlCQKL10F;
                    break;
                case "HNKLSF":
                    url = urlHNKL10F;
                    break;
                case "PL5":
                    url = urlPL5;
                    break;
                case "JLK3":
                    url = urlJLK3;
                    break;
                case "HBK3":
                    url = urlHBK3;
                    break;
                case "QLC":
                    url = urlQLC;
                    break;
                case "QXC":
                    url = urlQXC;
                    break;
                case "HD15X5":
                    url = url15X5;
                    break;
                case "HC1":
                    url = urlHC1;
                    break;
                case "DF6J1":
                    url = urlDF6J1;
                    break;
            }
            return url;
        }
    }

    [DataContract]
    internal class CaiLeleOpenInfo
    {
        [DataMember]
        public string openTerm { get; set; }

        [DataMember]
        public string openResult { get; set; }
    }
}