using System;
using System.Collections.Generic;
using System.Xml;

namespace Lottery.CrawGetters
{
    internal class WinNumberGetter_500Wan : WinNumberGetter
    {
        private static readonly Dictionary<string, string> UrlMapping;

        static WinNumberGetter_500Wan()
        {
            //http://kaijiang.500.com/ssq.shtml
            UrlMapping = new Dictionary<string, string>();
            UrlMapping.Add("CQSSC", "http://kaijiang.500.com/static/public/ssc/xml/qihaoxml/{0:yyyyMMdd}.xml"); //重庆时时彩
            UrlMapping.Add("FC3D", "http://jk.trade.500.com/static/info/kaijiang/xml/sd/list{0}.xml");
            UrlMapping.Add("SSQ", "http://jk.trade.500.com/static/info/kaijiang/xml/ssq/list{0}.xml"); //双色球
            UrlMapping.Add("DLT", "http://jk.trade.500.com/static/info/kaijiang/xml/dlt/list{0}.xml"); //大乐透
            UrlMapping.Add("JX11X5", "http://jk.trade.500.com/static/public/dlc/xml/newlyopenlist.xml"); //江西11选5
            UrlMapping.Add("PL3", "http://jk.trade.500.com/static/info/kaijiang/xml/pls/list{0}.xml");
            UrlMapping.Add("SDQYH", "http://kaijiang.500wan.com/static/info/kaijiang/xml/qyh/{0:yyyyMMdd}.xml"); //山东群英会
            UrlMapping.Add("QXC", "http://trade.500wan.com/static/info/kaijiang/xml/qxc/list{0}.xml"); //七星彩
            UrlMapping.Add("QLC", "http://trade.500wan.com/static/info/kaijiang/xml/qlc/list{0}.xml"); //七乐彩
            UrlMapping.Add("PL5", "http://kaijiang.500.com/static/info/kaijiang/xml/plw/list{0}.xml"); //排列5
            UrlMapping.Add("SHSSL", "http://kaijiang.500.com/static/public/ssl/xml/qihaoxml/{0:yyyyMMdd}.xml"); //排列5
            UrlMapping.Add("GD11X5",
                "http://kaijiang.500.com/static/info/kaijiang/xml/gdsyxw/{0:yyyyMMdd}.xml"); //广东11选5 
            UrlMapping.Add("GDKLSF", "http://kaijiang.500.com/static/info/kaijiang/xml/gdklsf/{0:yyyyMMdd}.xml");

            //UrlMapping.Add("GD11X5", "24");
        }

        private string GetUrl(string gameName, int lastIssuseCount)
        {
            var xmlUrl = string.Empty;
            switch (gameName)
            {
                case "CQSSC":
                case "SHSSL":
                case "GD11X5":
                case "SDQYH":
                case "GDKLSF":
                    xmlUrl = UrlMapping[gameName];
                    xmlUrl = string.Format(xmlUrl, DateTime.Today);
                    break;
                case "SSQ":
                case "QXC":
                case "QLC":
                case "DLT":
                case "JX11X5":
                case "FC3D":
                case "PL3":
                case "PL5":
                    xmlUrl = UrlMapping[gameName];
                    xmlUrl = string.Format(xmlUrl, lastIssuseCount);
                    break;
            }
            return xmlUrl;
        }

        /// <summary>
        ///     500万采集号码
        /// </summary>
        /// <param name="gameName">彩种</param>
        /// <param name="lastIssuseCount">int值：10，20，30，50，100</param>
        /// <returns></returns>
        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var xmlUrl = GetUrl(gameName, lastIssuseCount);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlUrl);
            var result = new Dictionary<string, string>();

            switch (gameName)
            {
                case "SDQYH":
                case "GDKLSF":
                case "CQSSC":
                case "GD11X5":
                case "SHSSL":
                    result = GetWinNumber(xmlDoc, lastIssuseCount, gameName);
                    break;
                default:
                    break;
            }
            switch (gameName)
            {
                case "SSQ":
                case "QXC":
                case "QLC":
                case "DLT":
                case "PL3":
                case "PL5":
                case "JX11X5":
                case "FC3D":
                    result = GetWinNumber1(xmlDoc, lastIssuseCount, gameName);
                    break;
                default:
                    break;
            }
            return result;
        }

        private Dictionary<string, string> GetWinNumber(XmlDocument xmlDoc, int lastIssuseCount, string gameCode)
        {
            var result = new Dictionary<string, string>();
            var root = xmlDoc.GetElementsByTagName("xml")[0];
            var start = root.ChildNodes.Count - 1;

            for (; start >= 0; start--)
            {
                var ele = root.ChildNodes[start] as XmlElement;
                var issuse = ele.Attributes["expect"].Value;
                var winCode = ele.Attributes["opencode"].Value;
                switch (gameCode)
                {
                    case "SDQYH":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(8, 2));
                        break;
                    case "GDKLSF":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 2));
                        break;
                    case "CQSSC":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 3));
                        break;
                    case "GD11X5":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(6, 2));
                        break;
                    case "SHSSL":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 3));
                        //winCode = winCode.Split(',');//没有逗号分隔
                        break;
                    default:
                        break;
                }
                result.Add(issuse, winCode);

                if (result.Count >= lastIssuseCount)
                    break;
            }
            return result;
        }


        private Dictionary<string, string> GetWinNumber1(XmlDocument xmlDoc, int lastIssuseCount, string gameCode)
        {
            var result = new Dictionary<string, string>();
            var root = xmlDoc.GetElementsByTagName("xml")[0];
            //int start = root.ChildNodes.Count - 1;

            var name = xmlDoc.DocumentElement.Name;
            foreach (XmlElement item in xmlDoc[name].ChildNodes)
            {
                var winCode = item.Attributes["opencode"].Value;
                var issuse = item.Attributes["expect"].Value;
                switch (gameCode)
                {
                    case "SSQ":
                    case "QXC":
                    case "QLC":
                    case "DLT":
                        issuse = string.Format("{0}-{1}", DateTime.Now.Year, issuse.Substring(2, issuse.Length - 2));
                        break;
                    case "PL3":
                    case "PL5":
                        issuse = string.Format("{0}-{1}", DateTime.Now.Year, issuse.Substring(2, issuse.Length - 2));
                        break;
                    case "FC3D":
                        issuse = issuse.Insert(4, "-");
                        break;
                    case "JX11X5":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(6));
                        break;
                    default:
                        break;
                }
                result.Add(issuse, winCode);
            }
            return result;
        }
    }
}