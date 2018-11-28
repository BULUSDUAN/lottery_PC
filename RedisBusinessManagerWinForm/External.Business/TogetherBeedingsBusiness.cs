using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using GameBiz.Business;
using Common.Lottery;
using GameBiz.Domain.Entities;

namespace FacaiExternal.Business
{
    /// <summary>
    /// 合买战绩更新
    /// </summary>
    public class TogetherBeedingsBusiness : IPrizeIssuse_AfterTranCommit
    {
        public void PrizeIssuse_AfterTranCommit(string gameCode, string issuseNumber, string winNumber, decimal prevTotalMoney, decimal afterTotalMoney)
        {

            var log = Common.Log.LogWriterGetter.GetLogWriter();
            log.Write("合买战绩更新-", "。。。", Common.Log.LogType.Information, "", "彩种：" + gameCode + "期号：" + issuseNumber);
            try
            {
                var manager = new SchemeManager();
                var bonusManager = new BonusManager();
                var lotteryManager = new LotteryGameManager();
                //主表SchemeId
                var togetherMain = manager.QueryTogetherMain(gameCode, issuseNumber);
                var business = new TogetherBusiness();

                //合买中奖数
                //成功提升战绩数
                var winCount = 0;
                var beedingsCount = 0;
                foreach (var item in togetherMain)
                {
                    var winMoney = 0M;
                    //查询战绩
                    var togetherBeedings = manager.QueryBeedingsByGame(item.CreateUser.UserId, item.Game.GameCode);
                    //
                //    log.Write("查询主表", "", Common.Log.LogType.Information, "", "FailCount:" + togetherBeedings.FailCount + ";FailWinCount:" + togetherBeedings.FailWinCount + ";FailBeedingsCount:" + togetherBeedings.FailWinCount + ";FailBeedings" + togetherBeedings.FailBeedings);
                    int amount = 0;
                    //得到倍数
                    var bettingIssuse = manager.QueryTempBettingIssuse(item.SchemeId, issuseNumber);
                    if (bettingIssuse == null)
                    {
                        amount = manager.QueryBettingIssuse(item.SchemeId, issuseNumber).Amount;
                    }
                    else
                    {
                        amount = bettingIssuse.Amount;
                    }
                    manager.QueryAnteCodeBySchemeId(item.SchemeId).ForEach(code =>
                    {
                        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, code.GameType);
                        var group = analyzer.CaculateBonus(code.AnteCode, winNumber).GroupBy(l => l);
                        foreach (var item1 in group)
                        {
                            var rule = bonusManager.QueryBonusRule(gameCode, code.GameType, item1.Key) ?? new BonusRule();
                            winMoney += rule.BonusMoney * item1.Count() * amount;
                        }
                    });
                    var beeding = 0;
                    //战绩金额
                    var beedingMoney = winMoney - item.TotalMoney;
                    if (winMoney != 0M)//中奖
                    {
                        winCount = 1;
                    }
                    if (beedingMoney > 100)//有战绩
                    {
                        //达到标准增加战绩
                        beeding = business.StatisticsBeedings(beedingMoney);
                        beedingsCount = 1;
                    }
                    if (item.State != TogetherState.Fail) //合买成功
                    {
                        togetherBeedings.FinishCount += 1;
                        togetherBeedings.WinCount += winCount;
                        togetherBeedings.BeedingsCount += beedingsCount;
                        togetherBeedings.Beedings += beeding;
                    }
                    else
                    {
                        togetherBeedings.FailCount += 1;
                        togetherBeedings.FailWinCount += winCount;
                        togetherBeedings.FailBeedingsCount += beedingsCount;
                        togetherBeedings.FailBeedings += beeding;
                    }
                    business.UpdateTogetherBeedingsGame(togetherBeedings);
                }
            }
            catch (Exception ex)
            {
                log.Write("合买战绩更新-", "异常出错", Common.Log.LogType.Information, "", ex.ToString());
            }
        }


        public object ExecPlugin(string type, object inputParam)
        {
            switch (type)
            {
                case "IPrizeIssuse_AfterTranCommit":
                    var arr = (object[])inputParam;
                    PrizeIssuse_AfterTranCommit((string)arr[0], (string)arr[1], (string)arr[2], (decimal)arr[3], (decimal)arr[4]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("不支持的接口类型 - " + type);
            }
            return null;
        }
    }
}
