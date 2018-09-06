using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Net;

namespace Lottery.CrawGetters
{
    internal class WinNumberGetter_KaiJiangHao : WinNumberGetter
    {
        //重庆时时彩
        private const string urlCQSSC = @"http://www.kaijianghao.com/HF_History.do?lottytype=1120";

        //江西时时彩
        //private const string urlJXSSC = @"http://www.kaijianghao.com/HF_History.do?lottytype=1400";
        //排列3
        private const string urlPL3 = @"http://www.kaijianghao.com/history/1-1.html";

        //福彩3D
        private const string urlFC3D = @"http://www.kaijianghao.com/history/1113-1.html";

        //江西11选5
        private const string urlJX11X5 = @"http://www.kaijianghao.com/searchHistory.do?lottytype=1340";

        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var dic = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(gameName) || lastIssuseCount == 0)
                return dic;
            var url = string.Empty;
            var kaiJiangLe_URL = string.Empty;
            var requestString = string.Empty;
            switch (gameName)
            {
                case "FC3D":
                    url = urlFC3D;
                    break;
                case "PL3":
                    url = urlPL3;
                    break;
                case "CQSSC":
                    url = urlCQSSC;
                    break;
                //case "JXSSC":
                //    url = urlJXSSC;
                //    break;
                case "JX11X5":
                    url = urlJX11X5;
                    break;
            }

            var json = PostManagerWithProxy.Get(url, Encoding.UTF8, 0);
            if (string.IsNullOrEmpty(json))
                return dic;
            return SplitHtml(json, gameName, lastIssuseCount);
        }

        private Dictionary<string, string> SplitHtml(string json, string gameName, int lastIssuseCount)
        {
            var tableIndex =
                json.IndexOf(
                    "<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" bgcolor=\"#cccccc\">");
            json = json.Substring(tableIndex, json.Length - tableIndex);
            //json = json.Substring(0, json.LastIndexOf("</table>"));
            var arrRows = json.Split("\r\n".ToCharArray());
            return NeatenDic(gameName, lastIssuseCount, arrRows);
        }

        private Dictionary<string, string> NeatenDic(string gameName, int lastIssuseCount, string[] arrRows)
        {
            var dic = new Dictionary<string, string>();
            var issueNumber = string.Empty;
            //var winNumber = new List<string>();
            foreach (var item in arrRows)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                if (item.IndexOf("<td height=\"30\" align=\"center\" bgcolor=\"#FFFFFF\">") != -1)
                {
                    issueNumber = NeatenIssueNumber(gameName, item);
                    continue;
                }
                if (item.IndexOf("<div class=ball_yellow>") != -1)
                {
                    var win = NeatenWin(gameName, item);
                    dic.Add(issueNumber, win);
                }
            }
            return dic;
        }

        #region 整理奖号

        private string NeatenWin(string gameName, string str)
        {
            str = Regex.Replace(str.Substring(str.IndexOf(">") + 1), @"[^\d.\d]", "");
            switch (gameName)
            {
                case "FC3D":
                    str = Regex.Replace(str, @"(?<=\d)(?=\d)", ",");
                    break;
                case "PL3":
                    str = Regex.Replace(str, @"(?<=\d)(?=\d)", ",");
                    break;
                case "CQSSC":
                    str = Regex.Replace(str, @"(?<=\d)(?=\d)", ",");
                    break;
                case "JX11X5":
                    str = Regex.Replace(str, @"(?<=\d)(?=\d)", ",");
                    var arrTemp = str.Split(',');
                    var list = new List<string>();
                    for (var i = 0; i < arrTemp.Length; i += 2)
                    {
                        var arrStr = arrTemp.Skip(i).Take(2);
                        list.Add(string.Join("", arrStr.ToArray()));
                    }
                    str = string.Join(",", list.ToArray());
                    break;
            }
            return str;
        }

        #endregion

        #region 整理期号

        private string NeatenIssueNumber(string gameName, string item)
        {
            var issueNumber = string.Empty;
            issueNumber = IssueTreatingLeft(item);


            switch (gameName)
            {
                case "FC3D":
                    issueNumber = IssueTreatingLeft(issueNumber);
                    issueNumber = IssueTreatingRight(issueNumber);
                    issueNumber = issueNumber.Insert(2, "-");
                    issueNumber = "20" + issueNumber;
                    break;
                case "PL3":
                    issueNumber = IssueTreatingLeft(issueNumber);
                    issueNumber = IssueTreatingRight(issueNumber);
                    issueNumber = issueNumber.Insert(2, "-");
                    issueNumber = "20" + issueNumber;
                    break;
                case "CQSSC":
                    issueNumber = IssueTreatingRight(issueNumber);
                    issueNumber = issueNumber.Insert(8, "-");
                    break;
                case "JX11X5":
                    issueNumber = IssueTreatingLeft(issueNumber);
                    issueNumber = IssueTreatingRight(issueNumber);
                    issueNumber = "20" + issueNumber;
                    issueNumber = issueNumber.Insert(8, "-");
                    break;
            }

            #region

            //if (gameName == "FC3D" || gameName == "PL3" || gameName == "JX11X5")
            //{

            //}
            //issueNumber = issueNumber.Substring(0, issueNumber.IndexOf("<"));
            //issueNumber = Regex.Replace(issueNumber, @"[^\d]", "");

            //if (gameName == "FC3D" || gameName == "PL3")
            //{
            //    issueNumber = issueNumber.Insert(2, "-");
            //    issueNumber = "20" + issueNumber;
            //}
            //else
            //{
            //    //issueNumber = "20" + issueNumber;
            //    issueNumber = issueNumber.Insert(8, "-");
            //}

            #endregion

            return issueNumber;
        }

        private string IssueTreatingRight(string str)
        {
            str = str.Substring(0, str.IndexOf("<"));
            str = Regex.Replace(str, @"[^\d]", "");
            return str;
        }

        private string IssueTreatingLeft(string str)
        {
            return str.Substring(str.IndexOf('>') + 1, str.Length - (str.IndexOf('>') + 1));
        }

        #endregion
    }
}