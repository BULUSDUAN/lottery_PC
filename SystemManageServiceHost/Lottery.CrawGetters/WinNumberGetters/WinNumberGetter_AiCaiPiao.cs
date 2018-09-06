using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using Lottery.CrawTool.Net;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///     爱彩票
    /// </summary>
    public class WinNumberGetter_AiCaiPiao : WinNumberGetter
    {
        private const string url_jx11x5 = "http://kaijiang.aicai.com/jx11x5/";
        private const string url_gd11x5 = "http://kaijiang.aicai.com/gd11x5/";
        private const string url_cq11x5 = "http://kaijiang.aicai.com/y11x5/";
        private const string url_sd11x5 = "http://kaijiang.aicai.com/11ydj/";
        private const string url_ln11x5 = "http://kaijiang.aicai.com/ln11x5/";
        private const string url_hd15x5 = "http://kaijiang.aicai.com/15x5/";

        //private const string url_gxklsf = "http://kaijiang.2caipiao.com/allopenprized/historyprizedetail/highfrequencyHistoryByCalendar/311.html";

        private const string url_cqssc = "http://kaijiang.aicai.com/cqssc/";
        private const string url_jxssc = "http://kaijiang.aicai.com/jxssc/";

        private const string url_ssq = "http://kaijiang.aicai.com/fcssq/";
        private const string url_dlt = "http://zst.aicai.com/historyOpenDlt!openinfo.jhtml";
        private const string url_fc3d = "http://kaijiang.aicai.com/fc3d/";

        private const string url_jlk3 = "http://kaijiang.aicai.com/k3/";
        private const string url_jsk3 = "http://kaijiang.aicai.com/laok3/";
        private const string url_df6j1 = "http://kaijiang.aicai.com/df61/";
        private const string url_pl3 = "http://kaijiang.aicai.com/pl3/";
        private const string url_pl5 = "http://kaijiang.aicai.com/pl5/";
        private const string url_qlc = "http://kaijiang.aicai.com/qlc/";
        private const string url_qxc = "http://kaijiang.aicai.com/qxc/";

        public override Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber)
        {
            var dic = new Dictionary<string, string>();

            #region Url

            var url = string.Empty;
            switch (gameCode)
            {
                case "JX11X5":
                    url = url_jx11x5;
                    break;
                case "GD11X5":
                    url = url_gd11x5;
                    break;
                case "SD11X5":
                    url = url_sd11x5;
                    break;
                case "CQSSC":
                    url = url_cqssc;
                    break;
                case "JXSSC":
                    url = url_jxssc;
                    break;
                case "SSQ":
                    url = url_ssq;
                    break;
                case "DLT":
                    url = url_dlt;
                    break;
                case "PL3":
                    url = url_pl3;
                    break;
                case "PL5":
                    url = url_pl5;
                    break;
                case "FC3D":
                    url = url_fc3d;
                    break;
                case "JSKS":
                    url = url_jsk3;
                    break;
                case "CQ11X5":
                    url = url_cq11x5;
                    break;
                case "DF6J1":
                    url = url_df6j1;
                    break;
                case "HD15X5":
                    url = url_hd15x5;
                    break;
                case "JLK3":
                    url = url_jlk3;
                    break;
                case "LN11X5":
                    url = url_ln11x5;
                    break;
                case "QLC":
                    url = url_qlc;
                    break;
                case "QXC":
                    url = url_qxc;
                    break;
            }

            #endregion

            if (string.IsNullOrEmpty(url))
                return dic;
            var json = PostManager.Get(url, Encoding.UTF8, 0).Trim();
            var startIndex = -1;
            var endIndex = -1;
            switch (gameCode)
            {
                case "CQSSC":
                case "SD11X5":
                case "JSKS":
                case "LN11X5":
                case "JLK3":
                case "JX11X5":
                case "GD11X5":
                case "CQ11X5":
                case "JXSSC":
                    startIndex = json.IndexOf("<tbody id=\"jq_body_kc_result\">");
                    endIndex = json.IndexOf("<div class=\"lotboxright\">");
                    json = json.Substring(startIndex, endIndex - startIndex);
                    dic = GeGPWinNumber(gameCode, json, lastIssuseCount);
                    break;
                case "PL3":
                case "PL5":
                case "SSQ":
                case "DF6J1":
                case "QLC":
                case "QXC":
                case "FC3D":
                case "HD15X5":
                    startIndex = json.LastIndexOf("<p class=\"lot_kjqs\">");
                    endIndex = json.IndexOf("<span class=\"lot_text alink\">");
                    json = json.Substring(startIndex, endIndex - startIndex);
                    dic = GeDPWinNumber(gameCode, json);
                    break;
                case "DLT":
                    startIndex = json.IndexOf("<tr onmouseout=");
                    endIndex = json.IndexOf("</tbody>");
                    json = json.Substring(startIndex, endIndex - startIndex)
                        .Replace("<tbody id=\"jq_body_kc_result\">", "").Replace("</tbody>", "").Replace("</table>", "")
                        .Replace("</div>", "");
                    dic = GeGPWinNumber(gameCode, json, lastIssuseCount);
                    break;
                default:
                    break;
            }

            return dic;
        }

        /// <summary>
        ///     可以取多条数据
        /// </summary>
        public Dictionary<string, string> GeGPWinNumber(string gameCode, string json, int lastIssuseCount)
        {
            var dic = new Dictionary<string, string>();
            json = json.Replace("<tbody id=\"jq_body_kc_result\">", "").Replace("</tbody>", "").Replace("</table>", "")
                .Replace("</div>", "");

            var root = XElement.Parse("<root>" + json + "</root>");
            var trs = root.Elements("tr").Where(e => e.Elements("td").Count() != 1);
            var num = 0;

            foreach (var item in trs)
            {
                num++;
                if (num == lastIssuseCount)
                    return dic;
                var tds = item.Elements("td").ToList();
                var issuseNumber = string.Empty;
                var winNumber = string.Empty;
                switch (gameCode)
                {
                    case "JX11X5":
                    case "LN11X5":
                    case "GD11X5":
                        issuseNumber = tds[0].Value.Replace("期", "");
                        winNumber = tds[2].Value;
                        break;
                    case "SD11X5":
                    case "CQ11X5":
                        issuseNumber = "20" + tds[0].Value.Replace("期", "");
                        winNumber = tds[2].Value;
                        break;
                    case "CQSSC":
                        issuseNumber = tds[0].Value.Replace("期", "");
                        winNumber = tds[2].Value.Replace("|", ",");
                        break;
                    case "JXSSC":
                        issuseNumber = tds[0].Value.Replace("期", "");
                        issuseNumber = issuseNumber.Substring(0, 9) + issuseNumber.Substring(10, 2);
                        winNumber = tds[2].Value.Replace("|", ",");
                        break;
                    case "DLT":
                        issuseNumber = "20" + tds[0].Value.Insert(2, "-");
                        winNumber = string.Format("{0},{1},{2},{3},{4}|{5},{6}", tds[2].Value, tds[3].Value,
                            tds[4].Value, tds[5].Value, tds[6].Value, tds[7].Value, tds[8].Value);
                        break;
                    case "HD15X5":
                    case "PL3":
                    case "PL5":
                        issuseNumber = tds[1].Value;
                        winNumber = string.Format("{0},{1},{2}", tds[2].Value, tds[3].Value, tds[4].Value);
                        break;
                    case "QLC":
                    case "QXC":
                    case "FC3D":
                    case "JSKS":
                    case "JLK3":
                        issuseNumber = tds[0].Value.Replace("期", "");
                        winNumber = tds[2].Value;
                        break;
                }

                if (!dic.Keys.Contains(issuseNumber))
                    dic.Add(issuseNumber, winNumber);
            }
            return dic;
        }

        /// <summary>
        ///     只能取一条数据，
        /// </summary>
        public Dictionary<string, string> GeDPWinNumber(string gameCode, string json)
        {
            var dic = new Dictionary<string, string>();

            var issuseNumber = string.Empty;
            var winNumber = string.Empty;
            switch (gameCode)
            {
                case "SSQ":
                    issuseNumber = json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 7)
                        .Insert(4, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"),
                        "<i>07</i><i>09</i><i>13</i><i>17</i><i>21</i><i>22</i><i class=\"blue\">10".Length);
                    winNumber = winNumber.Replace("<i>", "").Replace("</i>", ",").Replace(",<i class=\"blue\">", "|");
                    break;
                case "PL3":
                    issuseNumber =
                        "20" + json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 5)
                            .Insert(2, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"), "<i>6</i><i>7</i><i>8".Length).Replace("<i>", "")
                        .Replace("</i>", ",");
                    break;
                case "PL5":
                    issuseNumber =
                        "20" + json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 5)
                            .Insert(2, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"), "<i>6</i><i>7</i><i>8</i><i>7</i><i>8".Length)
                        .Replace("<i>", "").Replace("</i>", ",");
                    break;
                case "QLC":
                    issuseNumber = json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 7)
                        .Insert(4, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"),
                        "<i>01</i><i>02</i><i>05</i><i>10</i><i>12</i><i>18</i><i>28</i><i class=\"blue\">25".Length);
                    winNumber = winNumber.Replace("<i>", "").Replace("</i>", ",").Replace(",<i class=\"blue\">", "|");
                    break;
                case "QXC":
                    issuseNumber =
                        "20" + json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 5)
                            .Insert(2, "-");
                    winNumber = json
                        .Substring(json.IndexOf("<i>"), "<i>6</i><i>7</i><i>8</i><i>7</i><i>8</i><i>7</i><i>8".Length)
                        .Replace("<i>", "").Replace("</i>", ",");
                    break;
                case "FC3D":
                    issuseNumber = json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 7)
                        .Insert(4, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"), "<i>6</i><i>7</i><i>8".Length).Replace("<i>", "")
                        .Replace("</i>", ",");
                    break;
                case "DF6J1":
                    issuseNumber = json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 7)
                        .Insert(4, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"),
                        "<i>7</i><i>8</i><i>2</i><i>0</i><i>7</i><i>2</i><i class=\"blue\">狗".Length);
                    winNumber = winNumber.Replace("<i>", "").Replace("</i>", ",").Replace(",<i class=\"blue\">", "|");
                    winNumber = winNumber.Replace("鼠", "1").Replace("牛", "2").Replace("虎", "3").Replace("兔", "4")
                        .Replace("龙", "5").Replace("蛇", "6").Replace("马", "7").Replace("羊", "8").Replace("猴", "9")
                        .Replace("鸡", "10").Replace("狗", "11").Replace("猪", "12");
                    break;
                case "HD15X5":
                    issuseNumber = json.Substring(json.IndexOf("<option value=\"") + "<option value=\"".Length, 7)
                        .Insert(4, "-");
                    winNumber = json.Substring(json.IndexOf("<i>"), "<i>03</i><i>08</i><i>13</i><i>14</i><i>15".Length)
                        .Replace("<i>", "").Replace("</i>", ",");
                    break;
                default:
                    break;
            }

            dic.Add(issuseNumber, winNumber);
            return dic;
        }

        private WinNumberInfo_AiCaiPiao[] Deserialize(string json)
        {
            return JsonHelper.Deserialize<WinNumberInfo_AiCaiPiao[]>(json);
        }
    }

    [DataContract]
    public class WinNumberInfo_AiCaiPiao
    {
        [DataMember]
        public string IssuseNumber { get; set; }

        [DataMember]
        public string WinNumber { get; set; }

        [DataMember]
        public string AwardTime { get; set; }
    }
}