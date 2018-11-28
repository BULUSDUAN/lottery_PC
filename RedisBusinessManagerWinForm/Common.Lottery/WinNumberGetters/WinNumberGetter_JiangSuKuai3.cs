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
    /// 江苏快三网
    /// </summary>
    internal class WinNumberGetter_JiangSuKuai3 : WinNumberGetter
    {
        //江苏快3
        private const string urlJSKS = @"http://www.jskuai3.com/opennum/index/t/1";


        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            string url = GetUrl(gameName);
            try
            {
                string html = PostManager.Post(url, string.Empty, Encoding.UTF8, 0);
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
                case "JSKS":
                    url = urlJSKS;
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

            //html只剩下table部分
            int rowIndex = html.IndexOf(styleCss[4]);
            html = html.Substring(rowIndex, html.Length - rowIndex);

            //html 只剩下 tr 部分
            var rows = html.Split(new string[] { styleCss[5] }, StringSplitOptions.RemoveEmptyEntries);

            int issuseLength = 0;//期号的长度 如CQSSC：20110129-023 ->12
            int issuseAfter_NumberLength = 0;//期号“-”后面的位数 如CQSSC:20110129-023 ->3
            int winNumberLength = 0;//一个号的长度 如CQSSC:1,2,3,4,5 ->1
            int winNumberCount = 0;//开奖号个数 如CQSSC ->5
            switch (gameName)
            {
                case "JSKS":
                    issuseLength = 9;
                    issuseAfter_NumberLength = 2;
                    winNumberLength = 1;
                    winNumberCount = 3;
                    break;
            }
            string issuseNumber = string.Empty;
            string win = string.Empty;
            bool isIssuseNumber = false;//该行是否为期号
            var winNumber = new List<string>();
            for (int i = rows.Length - 5; i > 0; i = i - 5)
            {
                //开奖号列  
                win = Regex.Replace(rows[i], @"[^\d]", "").PadLeft(winNumberLength, '0');
                //win江苏快3,例如win="335";
                for (int k = 0; k < winNumberCount; k++)
                {
                    winNumber.Add(win.Substring(k, 1));
                }


                i--;
                issuseNumber = Regex.Replace(rows[i], @"[^\d]", "");

                issuseNumber = issuseNumber.Insert(issuseNumber.Length - issuseAfter_NumberLength, "-");
                if (issuseNumber.Length != issuseLength)
                {
                    //1012230-16
                    issuseNumber = "20" + issuseNumber;
                    if (gameName == "JSKS")
                    {
                        issuseNumber = issuseNumber.Substring(0, 8) + issuseNumber.Substring(9);
                    }
                }
                if (winNumber.Count == winNumberCount)
                {
                    //取到了 winNumberCount 个开奖号  和   期号
                    dic.Add(issuseNumber, string.Join(",", winNumber.ToArray()));
                    if (dic.Count == resultLength)
                        break;
                    issuseNumber = string.Empty;
                    winNumber.Clear();
                    //i = i - 5;
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
                case "JSKS":
                    styleCss[0] = "<table class=\"detail-table\" width=\"948\"";
                    styleCss[1] = "";
                    styleCss[2] = "";
                    styleCss[3] = "</table>";
                    styleCss[4] = "<td>";
                    styleCss[5] = "<td>";
                    break;
            }
            return styleCss;
        }

    }
}