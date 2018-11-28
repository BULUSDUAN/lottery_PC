using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Net;
using System.Text.RegularExpressions;

namespace Common.Lottery.WinNumberGetters
{
    /// <summary>
    /// 爱彩乐
    /// </summary>
    internal class WinNumberGetter_AiCaiLe : WinNumberGetter
    {
        //山东11选5
        private const string urlSD11X5 = @"http://pub.icaile.com/kjgg.php?lottery_type=502";

        //广东11选5
        private const string urlGD11X5 = @"http://pub.icaile.com/kjgg.php?lottery_type=504";

        //江西11选5
        private const string urlJX11X5 = @"http://pub.icaile.com/kjgg.php?lottery_type=505";

        //重庆时时彩
        private const string urlCQSSC = @"http://pub.icaile.com/kjgg.php?lottery_type=503";

        //江西时时彩
        private const string urlJXSSC = @"http://pub.icaile.com/kjgg.php?lottery_type=501";


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            string url = GetUrl(gameName);
            try
            {
                string html = PostManager.Post(url, string.Empty, Encoding.Default, 0);
                if (string.IsNullOrEmpty(html))
                    return dic;
                return SplitHtml(html, gameName.ToUpper(), lastIssuseCount);
            }
            catch
            {
                return dic;
            }

        }

        private string GetUrl(string gameName)
        {
            string url = string.Empty;
            switch (gameName)
            {
                //高频彩
                case "GD11X5":
                    url = urlGD11X5;
                    break;
                case "SD11X5":
                    url = urlSD11X5;
                    break;
                case "JX11X5":
                    url = urlJX11X5;
                    break;
                case "CQSSC":
                    url = urlCQSSC;
                    break;
                case "JXSSC":
                    url = urlJXSSC;
                    break;
            }
            return url;
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

            //ml只剩下table部分
            int rowIndex = html.IndexOf(styleCss[4]);
            html = html.Substring(rowIndex, html.Length - rowIndex);

            //ml 只剩下 tr 部分
            var rows = html.Split(new string[] { styleCss[5] }, StringSplitOptions.RemoveEmptyEntries);


            int issuseLength = 0;//期号的长度 如CQSSC：20110129-023 ->12
            int issuseAfter_NumberLength = 0;//期号“-”后面的位数 如CQSSC:20110129-023 ->3
            int winNumberLength = 0;//一个号的长度 如CQSSC:1,2,3,4,5 ->1
            int winNumberCount = 0;//开奖号个数 如CQSSC ->5
            switch (gameName)
            {
                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                    issuseLength = 12;
                    issuseAfter_NumberLength = 2;
                    winNumberLength = 2;
                    winNumberCount = 5;
                    break;
                case "CQSSC":
                case "JXSSC":
                    issuseLength = 12;
                    issuseAfter_NumberLength = 3;
                    winNumberLength = 1;
                    winNumberCount = 5;
                    break;
            }
            string issuseNumber = string.Empty;
            var winNumber = new List<string>();
            for (int i = 0; i < rows.Length; i++)
            {
                //个别彩种rows是否循环
                if (!rowIsEnabled(gameName, rows[i]))
                {
                    continue;
                }
                if (rows[i].IndexOf(styleCss[1]) != -1)
                {
                    //开奖号列  
                    string win = Regex.Replace(rows[i].Substring(rows[i].IndexOf(">") + 1), @"[^\d]", "").PadLeft(winNumberLength, '0');
                    for (int k = 0; k < winNumberCount * winNumberLength; k = k + winNumberLength)
                    {
                        winNumber.Add(win.Substring(k, winNumberLength));
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
                }
                else if (rows[i].IndexOf(styleCss[2]) != -1)
                {
                    //期号列 如 <td align="center">101217111</td>
                    issuseNumber = Regex.Replace(rows[i].Substring(rows[i].IndexOf(">") + 1), @"[^\d]", "");
                    issuseNumber = issuseNumber.Insert(issuseNumber.Length - issuseAfter_NumberLength, "-");
                    if (issuseNumber.Length != issuseLength)
                    {
                        //1012230-16
                        issuseNumber = "20" + issuseNumber;
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
                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                case "CQSSC":
                case "JXSSC":
                    styleCss[0] = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"1";
                    styleCss[1] = " class=\"im\"";
                    styleCss[2] = ">";
                    styleCss[3] = "</table>";
                    styleCss[4] = "<td";
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
                    isEnabled = row.IndexOf(">2012") == -1 ? true : false;
                    break;
            }
            return isEnabled;
        }
    }
}
