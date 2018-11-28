using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Common.Lottery.WinNumberGetters
{
    /// <summary>
    /// 广西福彩中心
    /// </summary>
    public class WinNumberGetter_GXFC : WinNumberGetter
    {
        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();

            var url = string.Empty;
            switch (gameCode)
            {
                case "GXKLSF":
                    url = "http://www.gxcaipiao.com.cn:8090/gxcams/xml/award_09.xml";
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return dic;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(url);


            XmlNode root = xmlDoc.GetElementsByTagName("lottery")[0];

            for (int i = 0; i < lastIssuseCount; i++)
            {
                XmlElement ele = root.ChildNodes[i] as XmlElement;
                string issuse = ele.Attributes["id"].Value;
                var winNumber = new List<string>();
                ele.Attributes["c"].Value.Split(',').ToList().ForEach(n =>
                {
                    winNumber.Add(n.PadLeft(2, '0'));
                });

                string winCode = string.Join(",", winNumber.ToArray());
                string part1 = issuse.Substring(0, 7);
                string part2 = issuse.Substring(7);
                issuse = string.Format("{0}-{1}", part1, part2);
                dic.Add(issuse, winCode);
            }
            return dic;
        }
    }
}
