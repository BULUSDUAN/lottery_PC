using System;
using System.Collections.Generic;
using System.Linq;
using Common.Communication;
using Common.Net;

namespace Lottery.CrawGetters
{
    internal class WinNumberGetter_ShiShiCai : WinNumberGetter
    {
        private static readonly Dictionary<string, string> GameNameMapping;

        static WinNumberGetter_ShiShiCai()
        {
            GameNameMapping = new Dictionary<string, string>();
            GameNameMapping.Add("CQSSC", "4");
            GameNameMapping.Add("JXSSC", "5");
            GameNameMapping.Add("SD11X5", "16");
            GameNameMapping.Add("JX11X5", "23");
            GameNameMapping.Add("GD11X5", "24");
        }

        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var gameCode = GameNameMapping[gameName];
            // string data = "{\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[4,3]}\"]}";
            //             {\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[4,12]}\"]}
            var data =
                "{\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[" +
                gameCode + "," + lastIssuseCount + "]}\"]}";
            var host = "www.shishicai.cn";
            var port = 80;
            var r = new Random(DateTime.Now.Millisecond);
            var postPage = "/KAjax.ashx?method=SpeedAjax_GetTop12Bonus&randomStr=" + r.Next(10000, 99999);
            // data=%7B%22CommandName%22%3A%22SpeedAjax%22%2C%22Parameters%22%3A%5B%22%7B%5C%22CommandName%5C%22%3A%5C%22GetTop12Bonus%5C%22%2C%5C%22Parameters%5C%22%3A%5B4%2C6%5D%7D%22%5D%7D
            var arg = string.Format("data={0}", Uri.EscapeDataString(data));
            var result = PostManager.PostBySocket(host, port, postPage, arg);
            // string result = PostManager.Post("http://www.shishicai.cn/KAjax.ashx?method=SpeedAjax_GetTop12Bonus&randomStr=221853", Uri.EscapeDataString(data), Encoding.Default, 5);
            var bonusList = AnalyzeWinNumbers(result);
            var list = new Dictionary<string, string>();
            foreach (var i in bonusList.Keys.Reverse())
            {
                var winCode = bonusList[i];
                switch (gameName)
                {
                    case "CQSSC":
                    case "JXSSC":
                        var cs = winCode.ToCharArray();
                        winCode = cs[0] + "," + cs[1] + "," + cs[2] + "," + cs[3] + "," + cs[4];
                        break;
                    case "JX11X5":
                    case "SD11X5":
                    case "GD11X5":
                        break;
                    default:
                        throw new LogicException("不支持此玩法的分析中奖号码 - " + gameName);
                }
                var issuse = i;
                switch (gameName)
                {
                    case "JXSSC":
                        var items = issuse.Split('-');
                        var index = int.Parse(items[1]);
                        issuse = items[0] + "-" + index.ToString().PadLeft(2, '0');
                        break;
                }
                list.Add(issuse, winCode);
            }
            return list;
        }

        private Dictionary<string, string> AnalyzeWinNumbers(string postResult)
        {
            var list = new Dictionary<string, string>();
            var start = "GetDataBonusBack([\"";
            var end = "\"]);";
            var postList = postResult.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var row in postList)
                if (row.StartsWith(start) && row.EndsWith(end))
                {
                    var allStr = row.Remove(row.IndexOf(end)).Remove(0, start.Length);
                    var bonusList = allStr.Split(new[] {"\",\""}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in bonusList)
                    {
                        var sp = item.Split('|');
                        list.Add(sp[0], sp[1]);
                    }
                }
            return list;
        }
    }
}