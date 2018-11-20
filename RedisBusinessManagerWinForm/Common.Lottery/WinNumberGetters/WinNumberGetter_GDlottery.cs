using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using System.Text.RegularExpressions;


namespace Common.Lottery.WinNumberGetters
{
    public class WinNumberGetter_GDlottery : WinNumberGetter
    {

        //广东11选5
        private const string urlGD11X5 = @"http://www.gdlottery.cn/lot11x5.do?method=to11x5kjggzst&date={0}";
        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var url = "";
            switch (gameCode.ToUpper())
            {
                case "GD11X5":
                    url = string.Format(urlGD11X5, DateTime.Now.ToString("yyyy-MM-dd"));
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return dic;
            try
            {
                string html = PostManager.Post(url, string.Empty, Encoding.Default, 0);
                if (string.IsNullOrEmpty(html))
                    return dic;
                return SplitHtml(html, gameCode.ToUpper(), lastIssuseCount);
            }
            catch
            {
                return dic;
            }

        }


        private Dictionary<string, string> SplitHtml(string html, string gameName, int resultLength)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //开始截取html标志,中奖号码，期号，结束截取标志，rowIndex,移除部分
            string[] styleCss = SetHtml(gameName);

            int tableIndex = html.IndexOf(styleCss[0]);
            html = html.Substring(tableIndex, html.Length - tableIndex);

            int endTableIndex = html.IndexOf(styleCss[3]);
            html = html.Substring(0, endTableIndex);

            //html只剩下table部分
            int rowIndex = html.IndexOf(styleCss[4]);
            html = html.Substring(rowIndex, html.Length - rowIndex);
            //去掉select下拉框
            html = html.Replace(html.Substring(html.IndexOf("<tr bgcolor=\"#99cc99\">\r\n"), html.IndexOf("</tr>") - html.IndexOf("<tr bgcolor=\"#99cc99\">\r\n") + "</tr>".Length), "");

            //html 只剩下 tr 部分
            var rows = html.Split(new string[] { styleCss[5] }, StringSplitOptions.RemoveEmptyEntries);
            System.Collections.ArrayList al = new System.Collections.ArrayList(rows);

            int issuseLength = 0;//期号的长度 如CQSSC：20110129-023 ->12
            int issuseAfter_NumberLength = 0;//期号“-”后面的位数 如CQSSC:20110129-023 ->3
            int winNumberLength = 0;//一个号的长度 如CQSSC:1,2,3,4,5 ->1
            int winNumberCount = 0;//开奖号个数 如CQSSC ->5
            switch (gameName)
            {
                case "GD11X5":
                    issuseLength = 11;
                    issuseAfter_NumberLength = 2;
                    winNumberLength = 2;
                    winNumberCount = 5;
                    break;
            }
            string issuseNumber = string.Empty;
            string win = string.Empty;
            var winNumber = new List<string>();
            for (int i = rows.Length - 1; i > 0 - 1; i--)
            {

                //个别彩种rows是否循环
                if (!rowIsEnabled(gameName, rows[i]))
                {
                    continue;
                }
                if (rows[i].IndexOf(styleCss[1]) != -1)
                {
                    //开奖号列  
                    win = Regex.Replace(rows[i].Substring(rows[i].IndexOf("strong>") + 1), @"[^\d]", "").PadLeft(winNumberLength, '0');
                    //win江苏快3,例如win="335";

                    for (int k = 0; k < winNumberCount * winNumberLength; k = k + winNumberLength)
                    {
                        winNumber.Add(win.Substring(k, 2));
                    }
                }
                else if (rows[i].IndexOf(styleCss[2]) != -1)
                {
                    //期号列 如 <td align="center">101217111</td>
                    int td_Index = rows[i].IndexOf(">");
                    issuseNumber = rows[i].Substring(td_Index + 1).Replace("</td>", "").Replace("<td>", "");
                    issuseNumber = Regex.Replace(issuseNumber, @"[^\d]", "");
                    issuseNumber = issuseNumber.Insert(issuseNumber.Length - issuseAfter_NumberLength, "-");
                    if (issuseNumber.Length != issuseLength)
                    {
                        //1012230-16
                        issuseNumber = "20" + issuseNumber;
                    }
                    if (winNumber.Count == winNumberCount)
                    {
                        //取到了 winNumberCount 个开奖号  和   期号
                        dic.Add(issuseNumber, string.Join(",", winNumber.ToArray()));
                        if (dic.Count == resultLength)
                            break;
                        issuseNumber = string.Empty;
                        winNumber.Clear();
                    }
                    continue;
                }

            }
            return dic;
        }

        private string[] SetHtml(string gameName)
        {
            //开始截取html标志,中奖号码，期号，结束截取标志，rowIndex,移除部分
            string[] styleCss = new string[6];
            switch (gameName.ToUpper())
            {
                case "GD11X5":
                    styleCss[0] = "<table width=\"100%\" border=\"0\" cellspacing=\"1\" bordercolorlight=\"#008000\" bordercolordark=\"#FFFFFF";
                    styleCss[1] = "  height=\"20\"  width=\"154\" align=\"center\" bgcolor=\"#FFFF99\">\r\n";
                    styleCss[2] = " height=\"20\" align=\"center\" bgcolor=\"#FFFFFF\"";
                    styleCss[3] = "</table>";
                    styleCss[4] = "\r\n";
                    styleCss[5] = "<td";
                    break;

            }
            return styleCss;
        }

        private bool rowIsEnabled(string gameName, string row)
        {
            bool isEnabled = true;
            switch (gameName)
            {
                case "GD11X5":

                    isEnabled = row.IndexOf(" height=\"20\" align=\"center\" bgcolor=\"#FFFFFF\" width=") == -1 ? true : false;
                    break;
            }
            return isEnabled;
        }

    }
}
