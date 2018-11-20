using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using Common.Net;
using System.Xml.Linq;

namespace Common.Lottery.OpenDataGetters
{
    /// <summary>
    /// 爱彩
    /// </summary>
    public class OpenDataGetter_AiCai : OpenDataGetter
    {
        public override OpenDataInfo GetOpenData(string gameCode, string issuseNumber)
        {
            var url_ssq = "http://kaijiang.aicai.com/fcssq/";
            var url_dlt = "http://kaijiang.aicai.com/tcdlt/";

            switch (gameCode)
            {
                case "SSQ":
                    var htmlSSQ = PostManager.Get(url_ssq, Encoding.UTF8, 0).Trim();
                    return FomartSSQ(htmlSSQ);
                case "DLT":
                    var htmlDLT = PostManager.Get(url_dlt, Encoding.UTF8, 0).Trim();
                    return FomartDLT(htmlDLT);

                default:
                    break;
            }

            return new OpenDataInfo();
        }


        private OpenDataInfo FomartSSQ(string html)
        {
            //var startIndex = html.LastIndexOf("<div class=\"lot_kjmub\">");
            var startIndex = html.LastIndexOf("<div class=\"lot_js\">");
            var endIndex = html.IndexOf("<div class=\"lotboxright\">");
            html = html.Substring(startIndex, endIndex - startIndex);
            //取奖期
            var startIssuse = html.IndexOf("<option");
            var endIssuse = html.IndexOf("</option>");
            var number = html.Substring(startIssuse, endIssuse - startIssuse);
            startIssuse = number.IndexOf(">");
            var issuseNumber = number.Substring(startIssuse).Replace(">", "");
            issuseNumber = string.Format("{0}-{1}", issuseNumber.Substring(0, 4), issuseNumber.Substring(4));
            #region 重新改

            #endregion


            //取全国销量
            int saleStarindex1 = html.IndexOf("全国销量：￥<i id=\"jq_saleValue\" class=\"red fs18 ari\">");
            int saleStarindex2 = "全国销量：￥<i id=\"jq_saleValue\" class=\"red fs18 ari\">".Length;
            int saleStarindex = saleStarindex1 + saleStarindex2;
            var saleEndindex = html.IndexOf("</i>元  <span class=\"lot_text\">");
            var sale = html.Substring(saleStarindex, saleEndindex - saleStarindex);
            //取奖池滚存
            int poolStarindex1 = html.IndexOf("奖池滚存：￥<i id=\"jq_poolsValue\">");
            int poolStarindex2 = "奖池滚存：￥<i id=\"jq_poolsValue\">".Length;
            int poolStarindex = poolStarindex1 + poolStarindex2;
            int poolEndindex = html.IndexOf("</i>元</span></p>");
            var pool = html.Substring(poolStarindex, poolEndindex - poolStarindex);
            //取开奖号
            var winNumber = html.Substring(html.IndexOf("<i>"), "<i>07</i><i>09</i><i>13</i><i>17</i><i>21</i><i>22</i><i class=\"blue\">10".Length);
            winNumber = winNumber.Replace("<i>", "").Replace("</i>", ",").Replace(",<i class=\"blue\">", "|");
            //取奖等
            var levelStarindex = html.IndexOf("<tbody id=\"jq_tbody_prizeLevel\">") + "<tbody id=\"jq_tbody_prizeLevel\">\r\n                      ".Length;
            var levelEndindex = html.IndexOf("</tbody>");
            html = html.Substring(levelStarindex, levelEndindex - levelStarindex);

            var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            var gradeList = new OpenGradeInfoCollection();

            var target = string.Empty;
            var index = 0;

            foreach (var row in rows)
            {
                var gradeName = string.Empty;
                var bonusMoney = 0M;
                var bonusCount = 0;

                var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                if (tds.Length < 3)
                    continue;
                #region 奖等
                for (int i = 0; i < tds.Length; i++)
                {
                    var td = tds[i];
                    if (i == 0)
                    {
                        //奖等
                        target = "<td>";
                        index = td.IndexOf(target);
                        gradeName = td.Substring(index + target.Length);
                    }
                    if (i == 1)
                    {
                        //奖金
                        target = "<i class=\"red\">";
                        index = td.IndexOf(target);
                        bonusMoney = decimal.Parse(td.Substring(index + target.Length).Replace("</i>元", string.Empty).Replace(",", string.Empty));
                    }
                    if (i == 2)
                    {
                        target = "<td>";
                        index = td.IndexOf(target);
                        bonusCount = int.Parse(td.Substring(index + target.Length).Replace("注", string.Empty).Replace(",", ""));
                    }

                }

                var grade = -1;
                var attr = string.Empty;
                if (gradeName.Contains("一等奖"))
                    grade = 1;
                if (gradeName.Contains("二等奖"))
                    grade = 2;
                if (gradeName.Contains("三等奖"))
                    grade = 3;
                if (gradeName.Contains("四等奖"))
                    grade = 4;
                if (gradeName.Contains("五等奖"))
                    grade = 5;
                if (gradeName.Contains("六等奖"))
                    grade = 6;

                attr = OpenGradeAttr.general;

                gradeList.Add(new OpenGradeInfo
                {
                    GradeName = gradeName,
                    BonusMoney = bonusMoney,
                    BonusCount = bonusCount,
                    Grade = grade,
                    GradeIndex = grade,
                    Attr = attr,
                });
                #endregion
            }

            return new OpenDataInfo
            {
                IssuseNumber = issuseNumber,
                GameCode = "SSQ",
                Source = "aicaipiao",
                WinNumber = winNumber,
                TotalSellMoney = decimal.Parse(sale.Replace(",", string.Empty)),
                TotalPrizePoolMoney = decimal.Parse(pool.Replace(",", string.Empty)),
                GradeList = gradeList,
                TotalBonusCount = gradeList.Sum(g => g.BonusCount),
                TotalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney),
            };
        }

        private OpenDataInfo FomartDLT(string html)
        {
            //var startIndex = html.LastIndexOf("<div class=\"lot_kjmub\">");
            var startIndex = html.LastIndexOf("<div class=\"lot_js\">");
            var endIndex = html.IndexOf("</tbody>");
            html = html.Substring(startIndex, endIndex - startIndex);

            //取奖期
            var startIssuse = html.IndexOf("<option");
            var endIssuse = html.IndexOf("</option>");
            var number = html.Substring(startIssuse, endIssuse - startIssuse);
            startIssuse = number.IndexOf(">");
            var issuseNumber = number.Substring(startIssuse).Replace(">", "");
            issuseNumber = string.Format("20{0}-{1}", issuseNumber.Substring(0, 2), issuseNumber.Substring(2));

            //取开奖号
            var winNumber = html.Substring(html.IndexOf("<i>"), "<i>09</i><i>10</i><i>24</i><i>28</i><i>29</i><i class=\"blue\">10</i><i class=\"blue\">12".Length);
            winNumber = winNumber.Replace("<i>", "").Replace("</i>", ",").Replace(",<i class=\"blue\">", ",");
            winNumber = string.Format("{0}|{1}", winNumber.Substring(0, 14), winNumber.Substring(15, 5));

            //取全国销量
            var saleStarindex = html.IndexOf("全国销量：￥<i id=\"jq_saleValue\" class=\"red\">") + "全国销量：￥<i id=\"jq_saleValue\" class=\"red\">".Length;
            var saleEndindex = html.IndexOf("</i>元");
            var sale = html.Substring(saleStarindex, saleEndindex - saleStarindex);
            //取奖池滚存
            var poolStarindex = html.IndexOf("<i id=\"jq_poolsValue\" class=\"red\">") + "<i id=\"jq_poolsValue\" class=\"red\">".Length;
            var poolEndindex = html.IndexOf("</i>元</span>");
            var pool = html.Substring(poolStarindex, poolEndindex - poolStarindex);
            //取奖等
            var levelStarindex = html.IndexOf("<tbody id=\"jq_tbody_prizeLevel\">") + "<tbody id=\"jq_tbody_prizeLevel\">".Length;
            html = html.Substring(levelStarindex, html.Length - levelStarindex).Replace("\r\n                      \r\n              ", "");

            var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            var gradeList = new OpenGradeInfoCollection();

            var target = string.Empty;
            var index = 0;
            foreach (var row in rows)
            {
                var gradeName = string.Empty;
                var bonusMoney = 0M;
                var bonusCount = 0;

                var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < tds.Length; i++)
                {
                    var td = tds[i];
                    if (i == 0)
                    {
                        //奖等
                        target = "<td>";
                        index = td.IndexOf(target);
                        gradeName = td.Substring(index + target.Length);                        
                    }
                    if (i == 1)
                    {
                        //奖金
                        target = "<i class=\"red\">";
                        index = td.IndexOf(target);
                        var money = td.Substring(index + target.Length).Replace("</i>元", string.Empty).Replace(",", string.Empty);
                        var moneyIndex = money.IndexOf("(");
                        if (moneyIndex != -1)
                            money = money.Substring(0, moneyIndex);
                        bonusMoney = decimal.Parse(money);
                    }
                    if (i == 2)
                    {
                        target = "<td>";
                        index = td.IndexOf(target);
                        bonusCount = int.Parse(td.Substring(index + target.Length).Replace("注", string.Empty).Replace(",", string.Empty));
                    }
                }

                var grade = -1;
                var attr = string.Empty;
                if (gradeName.Contains("一等奖"))
                    grade = 1;
                if (gradeName.Contains("二等奖"))
                    grade = 2;
                if (gradeName.Contains("三等奖"))
                    grade = 3;
                if (gradeName.Contains("四等奖"))
                    grade = 4;
                if (gradeName.Contains("五等奖"))
                    grade = 5;
                if (gradeName.Contains("六等奖"))
                    grade = 6;
                if (gradeName.Contains("七等奖"))
                    grade = 7;
                if (gradeName.Contains("八等奖"))
                    grade = 8;
                if (gradeName.Contains("幸运奖"))
                    grade = 9;
                if (gradeName.Contains("12选2"))
                    grade = 10;

                if (grade == -1)
                {
                    continue;
                }
                if (gradeName.Contains("追加"))
                {
                    continue;
                    //attr = OpenGradeAttr.append;
                }
                else if (gradeName.Contains("钻石"))
                    attr = OpenGradeAttr.diamond;
                else if (gradeName.Contains("宝石"))
                    attr = OpenGradeAttr.stone;
                else
                    attr = OpenGradeAttr.general;

                gradeList.Add(new OpenGradeInfo
                {
                    GradeName = gradeName,
                    BonusMoney = bonusMoney,
                    BonusCount = bonusCount,
                    Grade = grade,
                    GradeIndex = grade,
                    Attr = attr,
                });
            }

            return new OpenDataInfo
            {
                IssuseNumber = issuseNumber,
                GameCode = "DLT",
                Source = "aicaipiao",
                WinNumber = winNumber,
                TotalSellMoney = decimal.Parse(sale.Replace(",", string.Empty)),
                TotalPrizePoolMoney = decimal.Parse(pool.Replace(",", string.Empty)),
                GradeList = gradeList,
                TotalBonusCount = gradeList.Sum(g => g.BonusCount),
                TotalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney),
            };
        }

    }
}
