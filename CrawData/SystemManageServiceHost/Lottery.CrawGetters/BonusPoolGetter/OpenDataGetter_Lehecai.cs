using EntityModel.BonusPool;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.CrawGetters.BonusPoolGetter
{
    /// <summary>
    /// 乐和彩采集
    /// </summary>
    public class OpenDataGetter_Lehecai : OpenDataGetter
    {
        private const string baseUrlFormat = "http://baidu.lehecai.com/lottery/draw/view/{0}";

        public override OpenDataInfo GetOpenData(string gameCode, string issuseNumber)
        {
            var url = string.Empty;
            switch (gameCode.ToUpper())
            {
                case "DLT":
                    url = string.Format(baseUrlFormat, "1");
                    break;
                case "SSQ":
                    url = string.Format(baseUrlFormat, "50");
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return new OpenDataInfo();
            string html = string.Empty;
            try
            {
                html = PostManager.Get(url, Encoding.UTF8, requestHandler: (h) =>
                {
                    h.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
                });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("请求网页{0}出错：{1}", url, ex.Message), ex);
            }

            var json = string.Empty;
            try
            {
                //取json数据
                var phaseDataIndex = html.IndexOf("phaseData");
                var end = html.LastIndexOf("var result_config_arr");
                html = html.Substring(phaseDataIndex, end - phaseDataIndex).Replace("}};", "}}");
                //var oneLineIndex = html.IndexOf(";\n");}};
                //json = html.Substring(0, oneLineIndex).Substring(html.IndexOf("=") + 2);
                json = html.Substring(html.IndexOf("=") + 2);

                switch (gameCode.ToUpper())
                {
                    case "DLT":
                        return DecodeJson_DLT(json);
                    case "SSQ":
                        return DecodeJson_SSQ(json);
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("解析json{0}出错：{1}", json, ex.Message), ex);
            }

            return new OpenDataInfo();
        }
        //严重性	代码	说明	项目	文件	行	禁止显示状态
        //错误 CS0656  缺少编译器要求的成员“Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create”	Lottery.CrawGetters E:\wokerFiler\Lettery\Lottery_02\SystemManageServiceHost\Lottery.CrawGetters\BonusPoolGetter\OpenDataGetter_Lehecai.cs	86	活动的

        private OpenDataInfo DecodeJson_DLT(string json)
        {
            var jsonDic = JsonHelper.Decode(json);
            var gradeList = new List<OpenGradeInfo>();
            var saleAmount = 0M;
            var poolAmount = 0M;
            var winNumber = string.Empty;
            var issuseNumber = string.Empty;

            //json 数据处理 todo

            //foreach (var item in jsonDic)
            //{
            //    foreach (var prize in item.Value.list)
            //    {
            //        var grade = -1;
            //        string gradeName = prize.Value.name;
            //        var attr = string.Empty;
            //        if (gradeName.Contains("一等奖"))
            //            grade = 1;
            //        if (gradeName.Contains("二等奖"))
            //            grade = 2;
            //        if (gradeName.Contains("三等奖"))
            //            grade = 3;
            //        if (gradeName.Contains("四等奖"))
            //            grade = 4;
            //        if (gradeName.Contains("五等奖"))
            //            grade = 5;
            //        if (gradeName.Contains("六等奖"))
            //            grade = 6;
            //        if (gradeName.Contains("七等奖"))
            //            grade = 7;
            //        if (gradeName.Contains("八等奖"))
            //            grade = 8;
            //        if (gradeName.Contains("幸运奖"))
            //            grade = 9;

            //        if (gradeName.Contains("追加"))
            //            attr = OpenGradeAttr.append;
            //        else if (gradeName.Contains("钻石"))
            //            attr = OpenGradeAttr.diamond;
            //        else if (gradeName.Contains("宝石"))
            //            attr = OpenGradeAttr.stone;
            //        else
            //            attr = OpenGradeAttr.general;

            //        gradeList.Add(new OpenGradeInfo
            //        {
            //            GradeIndex = int.Parse(prize.Value.key.Replace("prize", "")),
            //            Grade = grade,
            //            GradeName = prize.Value.name,
            //            BonusCount = int.Parse(prize.Value.bet),
            //            BonusMoney = decimal.Parse(prize.Value.prize),
            //            Attr = attr,
            //        });
            //    }
            //    issuseNumber = string.Format("{0}-{1}", DateTime.Now.Year, item.Key.Substring(2));
            //    saleAmount = decimal.Parse(item.Value.formatSaleAmount);
            //    poolAmount = decimal.Parse(item.Value.formatPoolAmount);
            //    winNumber = string.Join(",", item.Value.result.red) + "|" + string.Join(",", item.Value.result.blue);
            //    break;
            //}

            return new OpenDataInfo
            {
                GameCode = "DLT",
                Source = "lehecai",
                IssuseNumber = issuseNumber,
                TotalPrizePoolMoney = poolAmount,
                TotalSellMoney = saleAmount,
                WinNumber = winNumber,
                TotalBonusCount = gradeList.Sum(g => g.BonusCount),
                TotalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney),
                GradeList = gradeList,
            };
        }
        private OpenDataInfo DecodeJson_SSQ(string json)
        {
            var jsonDic = JsonHelper.Decode(json);
            var gradeList = new List<OpenGradeInfo>();
            var saleAmount = 0M;
            var poolAmount = 0M;
            var winNumber = string.Empty;
            var issuseNumber = string.Empty;
            var time_draw = string.Empty;
            foreach (var item in jsonDic)
            {
                foreach (var prize in item.Value.list)
                {
                    var grade = -1;
                    string gradeName = prize.Value.name;
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


                    if (gradeName.Contains("追加"))
                        attr = OpenGradeAttr.append;
                    else if (gradeName.Contains("钻石"))
                        attr = OpenGradeAttr.diamond;
                    else if (gradeName.Contains("宝石"))
                        attr = OpenGradeAttr.stone;
                    else
                        attr = OpenGradeAttr.general;

                    gradeList.Add(new OpenGradeInfo
                    {
                        GradeIndex = int.Parse(prize.Value.key.Replace("prize", "")),
                        Grade = grade,
                        GradeName = prize.Value.name,
                        BonusCount = int.Parse(prize.Value.bet),
                        BonusMoney = decimal.Parse(prize.Value.prize),
                        Attr = attr,
                    });
                }
                issuseNumber = item.Key.Insert(4, "-");
                //string.Format("{0}-{1}", DateTime.Now.Year, item.Key.Substring(2));
                saleAmount = decimal.Parse(item.Value.formatSaleAmount);
                poolAmount = decimal.Parse(item.Value.formatPoolAmount);
                winNumber = string.Join(",", item.Value.result.red) + "|" + string.Join(",", item.Value.result.blue);
                break;
            }

            return new OpenDataInfo
            {
                GameCode = "SSQ",
                Source = "lehecai",
                IssuseNumber = issuseNumber,
                TotalPrizePoolMoney = poolAmount,
                TotalSellMoney = saleAmount,
                WinNumber = winNumber,
                TotalBonusCount = gradeList.Sum(g => g.BonusCount),
                TotalBonusMoney = gradeList.Sum(g => g.BonusCount * g.BonusMoney),
                GradeList = gradeList,
            };
        }
    }
}
