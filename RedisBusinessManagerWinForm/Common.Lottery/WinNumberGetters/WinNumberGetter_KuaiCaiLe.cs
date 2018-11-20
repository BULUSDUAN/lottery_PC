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
    /// 快彩乐
    /// </summary>
    internal class WinNumberGetter_KuaiCaiLe : WinNumberGetter
    {
        //江苏快3
        private const string urlJSKS = @"http://zst.kuaicaile.com/chart/k3/historyLotteryNumbers.jhtml";

        //福彩3D
        private const string urlFC3D = @"http://zst.kuaicaile.com/chart/3d/lotteryNumbers.jhtml";

        //广东11选5
        private const string urlGD11X5 = @"http://zst.kuaicaile.com/chart/gd11x5/todayLotteryNumbers.jhtml";

        //排列三
        private const string urlPL3 = @"http://zst.kuaicaile.com/chart/p3/lotteryNumbers.jhtml";

        //七乐彩
        private const string urlQLC = @"http://zst.kuaicaile.com/chart/qlc/lotteryNumbers.jhtml";

        //七星彩
        private const string urlQXC = @"http://zst.kuaicaile.com/chart/qxc/lotteryNumbers.jhtml";

        //山东11选5
        private const string urlSD11X5 = @"http://zst.kuaicaile.com/chart/sd11x5/todayLotteryNumbers.jhtml";

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
                case "JSKS":
                    url = urlJSKS;
                    break;
                case "FC3D":
                    url = urlFC3D;
                    break;
                case "GD11X5":
                    url = urlGD11X5;
                    break;
                case "PL3":
                    url = urlPL3;
                    break;
                case "QLC":
                    url = urlQLC;
                    break;
                case "QXC":
                    url = urlQXC;
                    break;
                case "SD11X5":
                    url = urlSD11X5;
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
                case "GD11X5":
                case "SD11X5":
                    issuseLength = 11;
                    issuseAfter_NumberLength = 2;
                    winNumberLength = 2;
                    winNumberCount = 5;
                    break;
                case "PL3":
                case "FC3D":
                    issuseLength = 8;
                    issuseAfter_NumberLength = 3;
                    winNumberLength = 1;
                    winNumberCount = 3;
                    break;
                case "QLC":
                    issuseLength = 8;
                    issuseAfter_NumberLength = 3;
                    winNumberLength = 2;
                    winNumberCount = 7;
                    break;
                case "QXC":
                    issuseLength = 8;
                    issuseAfter_NumberLength = 3;
                    winNumberLength = 1;
                    winNumberCount = 7;
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
                    //win江苏快3,例如win="335";
                    if (gameName == "GD11X5" || gameName == "QLC" || gameName == "QXC" || gameName == "SD11X5")
                    {
                        winNumber.Add(win);
                    }
                    else
                    {
                        for (int k = 0; k < winNumberCount; k++)
                        {
                            winNumber.Add(win.Substring(k, 1));
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
                    }
                }
                else if (rows[i].IndexOf(styleCss[2]) != -1)
                {
                    //期号列 如 <td align="center">101217111</td>
                    int td_Index = rows[i].IndexOf(">");
                    issuseNumber = rows[i].Substring(td_Index + 1).Replace("</td>", "").Replace("<td>", "");

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
                case "JSKS":
                    styleCss[0] = "<table id=\"chartsTable\" class=\"chartbox kjhistroybox\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1";
                    styleCss[1] = "<td align=\"center\" class=\"boldred";
                    styleCss[2] = "<td align=\"center";
                    styleCss[3] = "</table>";
                    styleCss[4] = "\r\n\t\t";
                    styleCss[5] = "\r\n\t\t";
                    break;
                case "FC3D":
                    styleCss[0] = "<tbody>";
                    styleCss[1] = "<td class=\"cfont2";
                    styleCss[2] = "<td class=\"t_tr1\">20";
                    styleCss[3] = "</tbody>";
                    styleCss[4] = "\r\n\t\t";
                    styleCss[5] = "\r\n\t\t";
                    break;
                case "GD11X5":
                case "SD11X5":
                    styleCss[0] = "<tbody>";
                    styleCss[1] = "\t";
                    styleCss[2] = "<td align=\"center\">20";
                    styleCss[3] = "</tbody>";
                    styleCss[4] = "\r\n\t\t\t\t\t\t";
                    styleCss[5] = "\r\n\t\t\t\t\t\t";
                    break;
                case "PL3":
                    styleCss[0] = "<tbody>";
                    styleCss[1] = "<td class=\"cfont2";
                    styleCss[2] = "<td class=\"t_tr1\">20";
                    styleCss[3] = "</tbody>";
                    styleCss[4] = "\r\n\t\t";
                    styleCss[5] = "\r\n\t\t";
                    break;
                case "QLC":
                    styleCss[0] = "<table width=\"100%\" class=\"chartbox kjhistroybox\" cellspacing=\"0\" cellpadding=\"1\" id=\"tablelist";
                    styleCss[1] = "<td class=\"t_cfont2\" align=\"center";
                    styleCss[2] = "<tr class=\"t_tr1\" align=\"center\"><td>";
                    styleCss[3] = "</table>";
                    styleCss[4] = "\r\n\t\t";
                    styleCss[5] = "\r\n\t\t";
                    break;
                case "QXC":
                    styleCss[0] = "<table  width=\"100%\" class=\"chartbox kjhistroybox\" cellspacing=\"0\" cellpadding=\"1\" id=\"tablelist";
                    styleCss[1] = "<td class=\"t_cfont2\" align=\"center";
                    styleCss[2] = "<tr class=\"t_tr1\" align=\"center\"><td>";
                    styleCss[3] = "</table>";
                    styleCss[4] = "\r\n\t\t";
                    styleCss[5] = "\r\n\t\t";
                    break;
                case "JXSSC":
                    styleCss[0] = "";
                    styleCss[1] = "";
                    break;
            }
            return styleCss;
        }

        private bool rowIsEnabled(string gameName, string row)
        {
            bool isEnabled = true;
            switch (gameName)
            {
                case "JSKS":
                    break;
                case "GD11X5":
                case "SD11X5":
                    var week = row.Substring("<td align=\"center\">".Length < row.Length ? "<td align=\"center\">".Length : 0, 1);
                    isEnabled = (week != "M" && week != "T" && week != "W" && week != "F" && week != "S") ? true : false;
                    break;
                case "PL3":
                case "FC3D":

                    break;
                case "QLC":
                case "QXC":
                    isEnabled = row.IndexOf("</td>\t\t\t\t") == -1 ? true : false;
                    break;
            }

            return isEnabled;
        }


    }
}
