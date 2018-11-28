using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using Common.Business;

namespace Common.Lottery.OpenDataGetters
{
    /// <summary>
    /// 从500wan.com获取开奖信息
    /// </summary>
    public class OpenDataGetter_500wan : OpenDataGetter
    {
        /// <summary>
        /// 从500wan.com获取开奖信息
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <returns></returns>
        public override OpenDataInfo GetOpenData(string gameCode, string issuseNumber)
        {
            var url = "http://kaijiang.500wan.com/";

            switch (gameCode.ToUpper())
            {
                case "DLT":
                    url += "dlt.shtml";
                    break;

                case "SSQ":
                    url += "ssq.shtml";
                    break;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(url))
                return null;

            var html = PostManager.Get(url, Encoding.GetEncoding("gb2312"));

            OpenDataInfo result = null;

            switch (gameCode.ToUpper())
            {
                case "DLT":
                    result = getDLTInfo(html);
                    break;

                case "SSQ":
                    result = getSSQInfo(html);
                    break;

                default:
                    break;
            }

            return result;
        }

        private OpenDataInfo getSSQInfo(string html)
        {
            string issuseNumber = getDataFromHtml(html, "", "<font class=\"cfont2\"><strong>", "</strong></font>");
            issuseNumber = "20" + issuseNumber;
            issuseNumber = issuseNumber.Insert(4, "-");
            string winNumber = getDataFromHtml(html, "<div class=\"ball_box01\">", "<ul>", "</ul>");
            winNumber = winNumber.Replace("<li class=\"ball_red\">", ",").Replace("</li>", "").Replace("<li class=\"ball_blue\">", "|").TrimStart(',');
            decimal totalPrizePoolMoney = Convert.ToDecimal(getDataFromHtml(html, "奖池滚存", "class=\"cfont1 \">", "元</span>").Replace(",", ""));
            decimal totalSellMoney = Convert.ToDecimal(getDataFromHtml(html, "本期销量", "<span class=\"cfont1\">", "元</span>").Replace(",", ""));
            int totalBonusCount = 0;
            decimal totalBonusMoney = 0M;

            int grade = 0;
            string gradeName = "";
            string bonusData = "";
            int bonusCount = 0;
            decimal bonusMoney = 0M;

            var gradeList = new OpenGradeInfoCollection();

            #region 获取开奖详细

            //一等奖
            grade = 1;
            gradeName = "一等奖";

            bonusData = getDataFromHtml(html, "一等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 1,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //二等奖
            grade = 2;
            gradeName = "二等奖";

            bonusData = getDataFromHtml(html, "二等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 2,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //三等奖
            grade = 3;
            gradeName = "三等奖";

            bonusData = getDataFromHtml(html, "三等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 3,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //四等奖
            grade = 4;
            gradeName = "四等奖";

            bonusData = getDataFromHtml(html, "四等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 4,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //五等奖
            grade = 5;
            gradeName = "五等奖";

            bonusData = getDataFromHtml(html, "五等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 5,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //六等奖
            grade = 6;
            gradeName = "六等奖";

            bonusData = getDataFromHtml(html, "六等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 6,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            #endregion

            totalBonusCount = gradeList.Sum(g => g.BonusCount);
            totalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney);

            return new OpenDataInfo
            {
                GameCode = "SSQ",
                Source = "500wan",
                IssuseNumber = issuseNumber,
                TotalPrizePoolMoney = totalPrizePoolMoney,
                TotalSellMoney = totalSellMoney,
                WinNumber = winNumber,
                TotalBonusCount = totalBonusCount,
                TotalBonusMoney = totalBonusMoney,
                GradeList = gradeList,
            };
        }

        private OpenDataInfo getDLTInfo(string html)
        {
            string issuseNumber = getDataFromHtml(html, "", "<font class=\"cfont2\"><strong>", "</strong></font>");
            issuseNumber = "20" + issuseNumber;
            issuseNumber = issuseNumber.Insert(4, "-");
            string winNumber = getDataFromHtml(html, "<div class=\"ball_box01\">", "<ul>", "</ul>");
            winNumber = winNumber.Replace("<li class=\"ball_red\">", ",").Replace("</li>", "").Replace("<li class=\"ball_blue\">", "|").TrimStart(',');
            winNumber = winNumber.Substring(0, 17) + "," + winNumber.Substring(18);
            decimal totalPrizePoolMoney = Convert.ToDecimal(getDataFromHtml(html, "奖池滚存", "class=\"cfont1\">", "元</span>").Replace(",", ""));
            decimal totalSellMoney = Convert.ToDecimal(getDataFromHtml(html, "本期销量", "<span class=\"cfont1\">", "元</span>".Replace(",", "")));
            int totalBonusCount = 0;
            decimal totalBonusMoney = 0M;

            int grade = 0;
            string gradeName = "";
            string bonusData = "";
            int bonusCount = 0;
            decimal bonusMoney = 0M;

            var gradeList = new OpenGradeInfoCollection();

            #region 获取开奖详细

            //一等奖
            grade = 1;
            gradeName = "一等奖";

            bonusData = getDataFromHtml(html, "一等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            var bsMoney = bonusData.Split('|')[2].Replace(",", "");
            bsMoney = bsMoney.Substring(0, bsMoney.IndexOf("元(基本)"));
            bonusMoney = Convert.ToDecimal(bsMoney);

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 1,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //一等奖 追加
            grade = 1;
            gradeName = "一等奖追加";

            bonusData = getDataFromHtml(html, "一等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            var bszjMoney = bonusData.Split('|')[1].Replace(",", "");
            bszjMoney = bszjMoney.Substring(0, bszjMoney.IndexOf("元(基本)"));
            bonusMoney = Convert.ToDecimal(bszjMoney);

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 2,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });


            //二等奖
            grade = 2;
            gradeName = "二等奖";

            bonusData = getDataFromHtml(html, "二等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 3,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //二等奖 追加
            grade = 2;
            gradeName = "二等奖追加";

            bonusData = getDataFromHtml(html, "二等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 4,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });

            //三等奖
            grade = 3;
            gradeName = "三等奖";

            bonusData = getDataFromHtml(html, "三等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 5,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //三等奖 追加
            grade = 3;
            gradeName = "三等奖追加";

            bonusData = getDataFromHtml(html, "三等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 6,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });



            //四等奖
            grade = 4;
            gradeName = "四等奖";

            bonusData = getDataFromHtml(html, "四等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 7,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //四等奖 追加
            grade = 4;
            gradeName = "四等奖追加";

            bonusData = getDataFromHtml(html, "四等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 8,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });



            //五等奖
            grade = 5;
            gradeName = "五等奖";

            bonusData = getDataFromHtml(html, "五等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 9,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //五等奖 追加
            grade = 5;
            gradeName = "五等奖追加";

            bonusData = getDataFromHtml(html, "五等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 10,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });

            //六等奖
            grade = 6;
            gradeName = "六等奖";

            bonusData = getDataFromHtml(html, "六等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 11,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //六等奖 追加
            grade = 6;
            gradeName = "六等奖追加";

            bonusData = getDataFromHtml(html, "六等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 12,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });

            //七等奖
            grade = 7;
            gradeName = "七等奖";

            bonusData = getDataFromHtml(html, "七等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[1]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[2].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 13,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            //七等奖 追加
            grade = 7;
            gradeName = "七等奖追加";

            bonusData = getDataFromHtml(html, "七等奖", "追加", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 14,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.append,
            });

            //八等奖
            grade = 8;
            gradeName = "八等奖";

            bonusData = getDataFromHtml(html, "八等奖", "<td>", "</tr>");
            bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            gradeList.Add(new OpenGradeInfo
            {
                GradeIndex = 15,
                Grade = grade,
                GradeName = gradeName,
                BonusCount = bonusCount,
                BonusMoney = bonusMoney,
                Attr = OpenGradeAttr.general,
            });

            ////九等奖
            //grade = 9;
            //gradeName = "幸运奖";

            //bonusData = getDataFromHtml(html, "开奖详情", "12选2", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 16,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.general,
            //});
            #endregion

            #region
            //一等奖 钻石
            //grade = 1;
            //gradeName = "钻石一等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "一等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 17,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.diamond,
            //});

            ////二等奖 钻石
            //grade = 2;
            //gradeName = "钻石二等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "二等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 18,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.diamond,
            //});

            ////三等奖 钻石
            //grade = 3;
            //gradeName = "钻石三等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "三等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 19,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.diamond,
            //});

            ////四等奖 钻石
            //grade = 4;
            //gradeName = "钻石四等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "四等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 20,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.diamond,
            //});


            ////-----------------------------

            ////一等奖 宝石
            //grade = 1;
            //gradeName = "宝石一等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "一等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 21,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.stone,
            //});

            ////二等奖 宝石
            //grade = 2;
            //gradeName = "宝石二等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "二等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 22,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.stone,
            //});

            ////三等奖 宝石
            //grade = 3;
            //gradeName = "宝石三等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "三等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 23,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.stone,
            //});

            ////四等奖 宝石
            //grade = 4;
            //gradeName = "宝石四等奖";

            //bonusData = getDataFromHtml(html, "宝石钻石", "四等奖", "<td>", "</tr>");
            //bonusData = bonusData.Replace("<td>", "|").Replace("</td>", "");

            //bonusCount = Convert.ToInt32(bonusData.Split('|')[0]);
            //bonusMoney = Convert.ToDecimal(bonusData.Split('|')[1].Replace(",", ""));

            //gradeList.Add(new OpenGradeInfo
            //{
            //    GradeIndex = 24,
            //    Grade = grade,
            //    GradeName = gradeName,
            //    BonusCount = bonusCount,
            //    BonusMoney = bonusMoney,
            //    Attr = OpenGradeAttr.stone,
            //});

            #endregion

            totalBonusCount = gradeList.Sum(g => g.BonusCount);
            totalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney);

            return new OpenDataInfo
            {
                GameCode = "DLT",
                Source = "500wan",
                IssuseNumber = issuseNumber,
                TotalPrizePoolMoney = totalPrizePoolMoney,
                TotalSellMoney = totalSellMoney,
                WinNumber = winNumber,
                TotalBonusCount = totalBonusCount,
                TotalBonusMoney = totalBonusMoney,
                GradeList = gradeList,
            };
        }

        private string getDataFromHtml(string html, string startTag, string preTag, string lastTag)
        {
            var startIndex = html.IndexOf(startTag);
            var preIndex = html.IndexOf(preTag, startIndex) + preTag.Length;
            var lastIndex = html.IndexOf(lastTag, preIndex);
            var result = html.Substring(preIndex, lastIndex - preIndex);

            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            return result;
        }

        private string getDataFromHtml(string html, string startTag1, string startTag2, string preTag, string lastTag)
        {
            var startIndex1 = html.IndexOf(startTag1);
            var startIndex2 = html.IndexOf(startTag2, startIndex1);
            var preIndex = html.IndexOf(preTag, startIndex2) + preTag.Length;
            var lastIndex = html.IndexOf(lastTag, preIndex);
            var result = html.Substring(preIndex, lastIndex - preIndex);

            result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            return result;
        }
    }
}
