using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;

namespace Common.Lottery.WinNumberGetters
{
    internal class WinNumberGetter_ShiShiCai : WinNumberGetter
    {
        private static Dictionary<string, string> GameNameMapping = null;
        static WinNumberGetter_ShiShiCai()
        {
            GameNameMapping = new Dictionary<string, string>();
            GameNameMapping.Add("CQSSC", "4");
            GameNameMapping.Add("JXSSC", "5");
            GameNameMapping.Add("SD11X5", "16");
            GameNameMapping.Add("JX11X5", "23");
            GameNameMapping.Add("GD11X5", "24");
        }
        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            string gameCode = GameNameMapping[gameName];
            // string data = "{\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[4,3]}\"]}";
            //             {\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[4,12]}\"]}
            string data = "{\"CommandName\":\"SpeedAjax\",\"Parameters\":[\"{\\\"CommandName\\\":\\\"GetTop12Bonus\\\",\\\"Parameters\\\":[" + gameCode + "," + lastIssuseCount + "]}\"]}";
            string host = "www.shishicai.cn";
            int port = 80;
            Random r = new Random(DateTime.Now.Millisecond);
            string postPage = "/KAjax.ashx?method=SpeedAjax_GetTop12Bonus&randomStr=" + r.Next(10000, 99999);
            // data=%7B%22CommandName%22%3A%22SpeedAjax%22%2C%22Parameters%22%3A%5B%22%7B%5C%22CommandName%5C%22%3A%5C%22GetTop12Bonus%5C%22%2C%5C%22Parameters%5C%22%3A%5B4%2C6%5D%7D%22%5D%7D
            string arg = string.Format("data={0}", Uri.EscapeDataString(data));
            string result = PostManager.PostBySocket(host, port, postPage, arg);
            // string result = PostManager.Post("http://www.shishicai.cn/KAjax.ashx?method=SpeedAjax_GetTop12Bonus&randomStr=221853", Uri.EscapeDataString(data), Encoding.Default, 5);
            Dictionary<string, string> bonusList = AnalyzeWinNumbers(result);
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (string i in bonusList.Keys.Reverse())
            {
                string winCode = bonusList[i];
                switch (gameName)
                {
                    case "CQSSC":
                    case "JXSSC":
                        char[] cs = winCode.ToCharArray();
                        winCode = cs[0] + "," + cs[1] + "," + cs[2] + "," + cs[3] + "," + cs[4];
                        break;
                    case "JX11X5":
                    case "SD11X5":
                    case "GD11X5":
                        break;
                    default:
                        throw new Exception("不支持此玩法的分析中奖号码 - " + gameName);
                }
                string issuse = i;
                switch (gameName)
                {
                    case "JXSSC":
                        string[] items = issuse.Split('-');
                        int index = int.Parse(items[1]);
                        issuse = items[0] + "-" + index.ToString().PadLeft(2, '0');
                        break;
                }
                list.Add(issuse, winCode);
            }
            return list;
        }
        private Dictionary<string, string> AnalyzeWinNumbers(string postResult)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            string start = "GetDataBonusBack([\"";
            string end = "\"]);";
            string[] postList = postResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in postList)
            {
                if (row.StartsWith(start) && row.EndsWith(end))
                {
                    string allStr = row.Remove(row.IndexOf(end)).Remove(0, start.Length);
                    string[] bonusList = allStr.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in bonusList)
                    {
                        string[] sp = item.Split('|');
                        list.Add(sp[0], sp[1]);
                    }
                }
            }
            return list;
        }
    }
}
