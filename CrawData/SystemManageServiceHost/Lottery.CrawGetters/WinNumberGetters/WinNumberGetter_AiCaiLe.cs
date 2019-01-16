using KaSon.FrameWork.Common.Net;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Lottery.CrawGetters
{
    /// <summary>
    ///     爱彩乐
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


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var url = GetUrl(gameName);

            var html = PostManagerWithProxy.Post(url, string.Empty, Encoding.Default, null);
            if (string.IsNullOrEmpty(html))
                return dic;
            return SplitHtml(html, gameName.ToUpper(), lastIssuseCount);
        }

        private string GetUrl(string gameName)
        {
            var url = string.Empty;
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
            var dic = new Dictionary<string, string>();
            //开始截取html标志,中奖号码，期号，结束截取标志，rowIndex,移除部分
            var styleCss = SetHtml(gameName);

            var tableIndex = html.IndexOf(styleCss[0]);
            html = html.Substring(tableIndex, html.Length - tableIndex);

            var endTableIndex = html.IndexOf(styleCss[3]);
            html = html.Substring(0, endTableIndex);

            //ml只剩下table部分
            var rowIndex = html.IndexOf(styleCss[4]);
            html = html.Substring(rowIndex, html.Length - rowIndex);

            //ml 只剩下 tr 部分
            var rows = html.Split(new[] {styleCss[5]}, StringSplitOptions.RemoveEmptyEntries);


            var issuseLength = 0; //期号的长度 如CQSSC：20110129-023 ->12
            var issuseAfter_NumberLength = 0; //期号“-”后面的位数 如CQSSC:20110129-023 ->3
            var winNumberLength = 0; //一个号的长度 如CQSSC:1,2,3,4,5 ->1
            var winNumberCount = 0; //开奖号个数 如CQSSC ->5
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
            var issuseNumber = string.Empty;
            var winNumber = new List<string>();
            for (var i = 0; i < rows.Length; i++)
            {
                //个别彩种rows是否循环
                if (!rowIsEnabled(gameName, rows[i]))
                    continue;
                if (rows[i].IndexOf(styleCss[1]) != -1)
                {
                    //开奖号列  
                    var win = Regex.Replace(rows[i].Substring(rows[i].IndexOf(">") + 1), @"[^\d]", "")
                        .PadLeft(winNumberLength, '0');
                    for (var k = 0; k < winNumberCount * winNumberLength; k = k + winNumberLength)
                        winNumber.Add(win.Substring(k, winNumberLength));

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
                        issuseNumber = "20" + issuseNumber;
                }
            }
            return dic;
        }


        private string[] SetHtml(string gameName)
        {
            //开始截取html标志,中奖号码，期号，结束截取标志，rowIndex,移除部分
            var styleCss = new string[6];
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
            var isEnabled = true;
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