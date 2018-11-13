using EntityModel.BonusPool;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.CrawGetters.BonusPoolGetter
{
    /// <summary>
    /// 彩乐乐
    /// </summary>
    public class OpenDataGetter_CaiLeLe : OpenDataGetter
    {
        public override OpenDataInfo GetOpenData(string gameCode, string issuseNumber)
        {
            var url_ssq = "http://kjh.cailele.com/kj_ssq.shtml";
            var url_dlt = "http://kjh.cailele.com/kj_dlt.shtml";

            switch (gameCode)
            {
                case "SSQ":
                    var htmlSSQ = PostManager.GetCaiLeLe(url_ssq, Encoding.Default, 0).Trim();
                    //var htmlSSQ = PostManager.Post(url_ssq, string.Empty, Encoding.Default, 0);
                    return FomartSSQ(htmlSSQ);
                case "DLT":
                    var htmlDLT = PostManager.GetCaiLeLe(url_dlt, Encoding.Default, 0).Trim();
                    //var htmlDLT = PostManager.Post(url_dlt, string.Empty, Encoding.Default, 0);
                    return FomartDLT(htmlDLT);

                default:
                    break;
            }

            return new OpenDataInfo();
        }


        private OpenDataInfo FomartSSQ(string html)
        {
            var startIndex = html.LastIndexOf("<div class=\"drawing_list\">");
            var endIndex = html.IndexOf("<div class=\"soon_bets\">");
            html = html.Substring(startIndex, endIndex - startIndex);

            //取奖期
            var startIssuse = html.IndexOf("<p class=\"cz_name_period\">") + "<p class=\"cz_name_period\">".Length;
            var number = html.Substring(startIssuse, startIssuse + 7 - startIssuse);
            var issuseNumber = string.Format("{0}-{1}", number.Substring(0, 4), number.Substring(4));

            //取全国销量
            var saleStarindex = html.IndexOf("全国销量：<span class=\"FD3400_12_b\">") + "全国销量：<span class=\"FD3400_12_b\">".Length;
            var saleEndindex = html.IndexOf("</span>元<br>");
            var sale = html.Substring(saleStarindex, saleEndindex - saleStarindex);
            //取奖池滚存
            var poolStarindex = html.IndexOf("奖池奖金：<span class=\"FD3400_12_b\">") + "奖池奖金：<span class=\"FD3400_12_b\">".Length;
            var poolEndindex = html.LastIndexOf("</span>元");
            var pool = html.Substring(poolStarindex, poolEndindex - poolStarindex);
            //取开奖号
            var winNumberSIndex = html.IndexOf("<span class=\"red_ball\">") + "<span class=\"red_ball\">".Length;
            var winNumberEIndex = html.LastIndexOf("<div class=\"trend_chart\">");
            var winNumber = html.Substring(winNumberSIndex, winNumberEIndex - winNumberSIndex);
            winNumber = winNumber.Replace("<span class=\"red_ball\">", "").Replace("</span>", ",").Replace("</div>", "").Replace("<span class=\"blue_ball\">", "|").Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").Replace(",|", "|");
            winNumber = winNumber.Substring(0, winNumber.Length - 1);
            //取奖等
            var levelStarindex = html.IndexOf("<tbody>") + "<tbody>".Length;
            var levelEndindex = html.IndexOf("</tbody>");
            html = html.Substring(levelStarindex, levelEndindex - levelStarindex);

            var rows = html.Split(new string[] { "<th>" }, StringSplitOptions.RemoveEmptyEntries);
            var gradeList = new List<OpenGradeInfo>();

            var target = string.Empty;
            var index = 0;

            foreach (var row in rows)
            {
                var gradeName = string.Empty;
                var bonusMoney = 0M;
                var bonusCount = 0;
                var gradeIndex = row.IndexOf("</th>");
                if (gradeIndex == -1)
                    continue;
                gradeName = row.Substring(0, gradeIndex);
                var countstar = row.IndexOf("<span class=\"multiple\">") + "<span class=\"multiple\">".Length;
                var countend = row.IndexOf("</span></td>");
                bonusCount = int.Parse(row.Substring(countstar, countend - countstar).Replace("注", ""));
                var moneystar = row.IndexOf("<td><span>") + "<td><span>".Length;
                var moneyend = row.IndexOf("</span><span");
                var money = row.Substring(moneystar, moneyend - moneystar).Replace("万", "").Replace("元", "").Replace(",", "");
                var Mindex = money.IndexOf(".");
                bonusMoney = Mindex != -1 ? decimal.Parse(money) * 10000 : decimal.Parse(money);
                //bonusMoney = decimal.Parse();
                //var tds = row.Split(new string[] { "<td" }, StringSplitOptions.RemoveEmptyEntries);
                #region 奖等
                //for (int i = 0; i < tds.Length; i++)
                //{
                //    var td = tds[i];
                //    if (i == 1)
                //    {
                //        var targetstar = td.IndexOf(" height=\"24\" align=\"center\">") + " height=\"24\" align=\"center\">".Length;
                //        var targetend = td.IndexOf("</td>");
                //        gradeName = td.Substring(targetstar, targetend - targetstar);

                //    }
                //    if (i == 2)
                //    {
                //        //注数
                //        target = " align=\"center\">";
                //        index = td.IndexOf(target);
                //        bonusCount = int.Parse(td.Substring(index + target.Length).Replace(" 注</td>", string.Empty).Replace(",", ""));
                //    }
                //    if (i == 3)
                //    {
                //        //奖金
                //        target = " align=\"center\">";
                //        index = td.IndexOf(target);
                //        bonusMoney = decimal.Parse(td.Substring(index + target.Length).Replace(" 元</td>\r\n</tr>", "").Replace(",", "").Replace(" ", ""));
                //    }

                //}
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
            //gradeList = gradeList.OrderBy(s => s.Grade);

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
            var startIndex = html.LastIndexOf("<div class=\"drawing_list\">");
            var endIndex = html.IndexOf("<div class=\"soon_bets\">");
            html = html.Substring(startIndex, endIndex - startIndex);

            //取奖期
            var startIssuse = html.IndexOf("<p class=\"cz_name_period\">") + "<p class=\"cz_name_period\">".Length;
            var number = html.Substring(startIssuse, startIssuse + 5 - startIssuse);
            var issuseNumber = string.Format("20{0}-{1}", number.Substring(0, 2), number.Substring(2));

            //取全国销量
            var saleStarindex = html.IndexOf("全国销量：<span class=\"FD3400_12_b\">") + "全国销量：<span class=\"FD3400_12_b\">".Length;
            var saleEndindex = html.IndexOf("</span>元<br>");
            var sale = html.Substring(saleStarindex, saleEndindex - saleStarindex);
            //取奖池滚存
            var poolStarindex = html.IndexOf("奖池奖金：<span class=\"FD3400_12_b\">") + "奖池奖金：<span class=\"FD3400_12_b\">".Length;
            var poolEndindex = html.LastIndexOf("</span>元");
            var pool = html.Substring(poolStarindex, poolEndindex - poolStarindex);
            //取开奖号
            var winNumberSIndex = html.IndexOf("<span class=\"red_ball\">") + "<span class=\"red_ball\">".Length;
            var winNumberEIndex = html.LastIndexOf("<div class=\"trend_chart\">");
            var winNumber = html.Substring(winNumberSIndex, winNumberEIndex - winNumberSIndex);
            winNumber = winNumber.Replace("<span class=\"red_ball\">", "").Replace("</span>", ",").Replace("</div>", "").Replace("<span class=\"blue_ball\">", "|").Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
            winNumber = winNumber.Substring(0, winNumber.Length - 1).Replace(",|01,|", ",|01,").Replace(",|02,|", ",|02,").Replace(",|03,|", ",|03,").Replace(",|04,|", ",|04,").Replace(",|05,|", ",|05,").Replace(",|06,|", ",|06,").Replace(",|07,|", ",|07,").Replace(",|08,|", ",|08,").Replace(",|09,|", ",|09,").Replace(",|10,|", ",|10,").Replace(",|11,|", ",|11,").Replace(",|12,|", ",|12,");
            //取奖等   
            var levelStarindex = html.IndexOf("<tbody>") + "<tbody>".Length;
            var levelEndindex = html.IndexOf("</tbody>");
            html = html.Substring(levelStarindex, levelEndindex - levelStarindex);

            var rows = html.Split(new string[] { "<th" }, StringSplitOptions.RemoveEmptyEntries);
            var gradeList = new List<OpenGradeInfo>();

            var target = string.Empty;
            var index = 0;
            foreach (var row in rows)
            {
                var gradeName = string.Empty;
                var bonusMoney = 0M;
                var bonusCount = 0;

                //var gradeIndex = row.IndexOf("</th>");
                //if (gradeIndex == -1)
                //    continue;

                //gradeName = row.Substring(0, gradeIndex);
                //var countstar = row.IndexOf("<span class=\"multiple\">") + "<span class=\"multiple\">".Length;
                //var countend = row.IndexOf("</span></td>");
                //bonusCount = int.Parse(row.Substring(countstar, countend - countstar).Replace("注", ""));
                //var moneystar = row.IndexOf("<td><span>") + "<td><span>".Length;
                //var moneyend = row.IndexOf("</span><span");
                //var money = row.Substring(moneystar, moneyend - moneystar).Replace("万", "").Replace("元", "").Replace(",", "");
                //var Mindex = money.IndexOf(".");
                //bonusMoney = Mindex != -1 ? decimal.Parse(money) * 10000 : decimal.Parse(money);

                var tds = row.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in tds)
                {
                    var jb = item.IndexOf("基本");
                    var zj = item.IndexOf("追加");
                    if (jb == -1 && zj == -1)
                        continue;

                    var gradeSIndex = item.IndexOf("rowspan=\"2\">") + "rowspan=\"2\">".Length;
                    var gradeEIndex = item.IndexOf("</th>");
                    if (gradeSIndex > 11)
                        gradeName = item.Substring(gradeSIndex, gradeEIndex - gradeSIndex);
                    var countstar = item.IndexOf("<span class=\"multiple\">") + "<span class=\"multiple\">".Length;
                    var countend = item.IndexOf("</span></td>");
                    bonusCount = int.Parse(item.Substring(countstar, countend - countstar).Replace("注", ""));
                    var moneystar = item.IndexOf("<td><span>") + "<td><span>".Length;
                    var moneyend = item.IndexOf("</span><span");
                    var money = item.Substring(moneystar, moneyend - moneystar).Replace("万", "").Replace("元", "").Replace(",", "");
                    var wan = item.IndexOf("万");
                    var Mindex = money.IndexOf(".");
                    bonusMoney = Mindex != -1 ? decimal.Parse(money) * 10000 : (wan != -1 ? decimal.Parse(money) * 10000 : decimal.Parse(money));


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

                    if (zj != -1)
                    {
                        attr = OpenGradeAttr.append;
                        gradeName = gradeName + "追加";
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
                //for (int i = 0; i < tds.Length; i++)
                //{
                //    var td = tds[i];
                //    if (i == 0)
                //    {
                //        //奖等
                //        target = "\r\n<td height=\"24\" align=\"center\">";
                //        index = td.IndexOf(target);
                //        gradeName = td.Substring(index + target.Length);
                //    }
                //    if (i == 1)
                //    {
                //        //注数
                //        target = "\r\n<td align=\"center\">";
                //        index = td.IndexOf(target);
                //        bonusCount = int.Parse(td.Substring(index + target.Length).Replace("注", string.Empty).Replace(",", string.Empty));
                //    }
                //    if (i == 2)
                //    {
                //        //奖金
                //        if (gradeName.Contains("附加12选2"))
                //            target = "<td>";
                //        else
                //            target = "\r\n<td align=\"center\">";
                //        index = td.IndexOf(target);
                //        var money = td.Substring(index + target.Length).Replace(" 元", string.Empty).Replace(",", string.Empty);
                //        bonusMoney = decimal.Parse(money);
                //    }
                //}

                //var grade = -1;
                //var attr = string.Empty;
                //if (gradeName.Contains("一等奖"))
                //    grade = 1;
                //if (gradeName.Contains("二等奖"))
                //    grade = 2;
                //if (gradeName.Contains("三等奖"))
                //    grade = 3;
                //if (gradeName.Contains("四等奖"))
                //    grade = 4;
                //if (gradeName.Contains("五等奖"))
                //    grade = 5;
                //if (gradeName.Contains("六等奖"))
                //    grade = 6;
                //if (gradeName.Contains("七等奖"))
                //    grade = 7;
                //if (gradeName.Contains("八等奖"))
                //    grade = 8;
                //if (gradeName.Contains("幸运奖"))
                //    grade = 9;

                //if (gradeName.Contains("追加"))
                //    attr = OpenGradeAttr.append;
                //else if (gradeName.Contains("钻石"))
                //    attr = OpenGradeAttr.diamond;
                //else if (gradeName.Contains("宝石"))
                //    attr = OpenGradeAttr.stone;
                //else
                //    attr = OpenGradeAttr.general;

                //gradeList.Add(new OpenGradeInfo
                //{
                //    GradeName = gradeName,
                //    BonusMoney = bonusMoney,
                //    BonusCount = bonusCount,
                //    Grade = grade,
                //    GradeIndex = grade,
                //    Attr = attr,
                //});
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
