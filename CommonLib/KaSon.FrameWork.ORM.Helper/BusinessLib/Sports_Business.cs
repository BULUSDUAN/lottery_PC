﻿using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using EntityModel;
using EntityModel.ExceptionExtend;
using System.Diagnostics;
using EntityModel.Enum;
using System.Linq;
using KaSon.FrameWork.Common.Redis;
using EntityModel.Redis;
using EntityModel.Ticket;
using System.Threading;
using EntityModel.GameBiz.Core;
using System.IO;
using System.Data;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Algorithms;
using KaSon.FrameWork.Common.ExtensionFn;
using KaSon.FrameWork.Common.Sport;
using EntityModel.Communication;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using EntityModel.Interface;
using KaSon.FrameWork.Analyzer;

using GameBiz.Domain.Entities;
using System.Threading.Tasks;
using MongoDB.Driver;
using EntityModel.Domain.Entities;

namespace KaSon.FrameWork.ORM.Helper
{
    public class Sports_Business : DBbase
    {
        // ILogger<Sports_Business> _Log;
        // public class Sports_Business(ILogger<BettingRepository>)
        // private static Log4Log writerLog = new Log4Log();

        private IMongoDatabase mDB;
        public Sports_Business(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
        public Sports_Business()
        {

        }
        public void Test()
        {
            string sql = @"SELECT * FROM T_JCLQ_Odds_SF O INNER JOIN (SELECT MAX(Id) [MaxId] " +
                "FROM T_JCLQ_Odds_SF WHERE [MatchId] ='181031301' ) T ON O.[Id]=T.[MaxId]";
            var one = DB.CreateSQLQuery(sql).First<T_JCLQ_Odds_SF>();
            Console.WriteLine(one);
        }
        public IndexMatch_Collection AddIndexMatch(string indexMatchList)
        {
            var collection = new IndexMatch_Collection();

            var matchIdList = JsonHelper.Deserialize<List<IndexMatchInfo>>(indexMatchList);
            if (matchIdList.Count == 0 || matchIdList == null)
                return collection;

            using (var manager = new Sports_Manager())
            {
                try
                {
                    foreach (var item in matchIdList)
                    {
                        var entity = manager.QueryIndexMatchByMatchId(item.MatchId);
                        if (entity == null)
                        {
                            //entity = new IndexMatch();
                            //ObjectConvert.ConverInfoToEntity(item, ref entity);
                            manager.AddIndexMatch(new IndexMatch
                            {
                                CreateTime = item.CreateTime,
                                ImgPath = item.ImgPath,
                                MatchId = item.MatchId,
                                MatchName = item.MatchName
                            });
                            continue;
                        }
                        collection.IndexMatchList.Add(new IndexMatchInfo
                        {
                            MatchId = entity.MatchId,
                            ImgPath = entity.ImgPath,
                        });
                    }
                }
                catch
                {
                    return collection;
                }
            }
            return collection;
        }
        /// <summary>
        ///  比赛数据添加、更新
        /// </summary>
        public void UpdateLocalData(string text, string param, NoticeType noticeType, string innerKey)
        {
            switch (noticeType)
            {
                #region 北京单场
                case NoticeType.BJDC_Issuse:
                    new IssuseBusiness(mDB).Update_BJDC_IssuseList("BJDC_Match_IssuseNumber_List");
                    break;
                case NoticeType.BJDC_Match:
                    var matchParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var matchIdArray = matchParamArray[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_BJDC_MatchList("BJDC_Match_List", matchParamArray[0], matchIdArray);
                    break;
                case NoticeType.BJDC_Match_SFGG:
                    var SFGGParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var SFGGdMatchIdArray = SFGGParamArray[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_SFGG_MatchList("BJDC_Match_SFGG_List", SFGGParamArray[0], SFGGdMatchIdArray);
                    break;
                case NoticeType.BJDC_MatchResult:
                    var matchResultParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var matchResultIdArray = matchResultParamArray[1].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var bjdzBiz = new IssuseBusiness();
                    var str_array = from s in matchResultIdArray select s.Split('_')[0];

                    bjdzBiz.Update_BJDC_MatchResultList("BJDC_MatchResult_List", matchResultParamArray[0], str_array.ToArray());
                    new Thread(() =>
                    {
                        Common.Utilities.UsefullHelper.TryDoAction(() =>
                        {
                            bjdzBiz.Update_BJDC_HitCount(matchResultParamArray[0], str_array.ToArray());
                        });
                    }).Start();
                    break;
                case NoticeType.BJDC_MatchResult_SFGG:
                    var SFGGmatchResultParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var SFGGmatchResultIdArray = SFGGmatchResultParamArray[1].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var SFGGbjdzBiz = new IssuseBusiness(mDB);
                    var SFGGstr_array = from s in SFGGmatchResultIdArray select s.Split('_')[0];

                    SFGGbjdzBiz.Update_SFGG_MatchResultList("BJDC_MatchResult_SFGG_List", SFGGmatchResultParamArray[0], SFGGstr_array.ToArray());
                    break;
                #endregion

                #region 传统足球
                case NoticeType.CTZQ_Issuse:
                    var p = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    var ctzq_GameCode = p[0].Split('|')[1];
                    new IssuseBusiness(mDB).Update_CTZQ_GameIssuse(ctzq_GameCode, p);
                    break;
                case NoticeType.CTZQ_Match:
                    var ctzqMatchArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = ctzqMatchArray[0].Split('|');
                    var ctzq_gameCode = arr[1];
                    var issuseNumber = arr[2];
                    new IssuseBusiness(mDB).Update_CTZQ_MatchList(ctzq_gameCode, issuseNumber, ctzqMatchArray);
                    break;
                case NoticeType.CTZQ_MatchPool:
                    int totalBonusCount;
                    var tmpCTZQ_MatchPool = param.Split('^');
                    var winNumber = new IssuseBusiness(mDB).UpdateBonusPool_CTZQ("CTZQ", tmpCTZQ_MatchPool[0], tmpCTZQ_MatchPool[1], out totalBonusCount);
                    if (new IssuseBusiness(mDB).CanPrize_CTZQ(tmpCTZQ_MatchPool[0], totalBonusCount, winNumber))
                    {
                        QueryUnPrizeTicketAndDoPrizeByGameCode("CTZQ", tmpCTZQ_MatchPool[0], -1);
                        LotteryIssusePrize("CTZQ", tmpCTZQ_MatchPool[0], tmpCTZQ_MatchPool[1], winNumber);
                    }
                    break;
                #endregion

                #region 竞彩足球
                case NoticeType.JCZQ_Match:
                    var jczq_array = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCZQ_MatchList("JCZQ_Match_List", jczq_array);
                    break;
                case NoticeType.JCZQ_MatchResult:
                    var jczq_match_result_array = text.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var str_Array = from s in jczq_match_result_array select s.Split('_')[0];
                    var jczq_biz = new IssuseBusiness(mDB);
                    jczq_biz.Update_JCZQ_MatchResultList("JCZQ_Match_Result_List", str_Array.ToArray());
                    //new Thread(() =>
                    //{
                    //    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    //    {
                    //        jczq_biz.Update_JCZQ_HitCount(str_Array.ToArray());
                    //    });
                    //}).Start();
                    break;
                //过关
                case NoticeType.JCZQ_SPF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ_SPF("JCZQ_SPF_SP", "JCZQ", "SPF", text.Split('_'), false);
                    //  new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_SPF_SPInfo, JCZQ_Odds_SPF>("JCZQ", "SPF", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_BRQSPF_SP:
                    //UpdateOddsList_JCLQ_RFSFF
                    // new TicketGatewayAdmin().UpdateOddsList_JCLQ_RFSFF<JCZQ_BRQSPF_SPInfo, JCZQ_Odds_BRQSPF>("JCZQ", "BRQSPF", text.Split('_'), false);
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ_BRQSPF("JCZQ_BRQSPF_SP", "JCZQ", "BRQSPF", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_ZJQ_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ_ZJQ("JCZQ_ZJQ_SP", "JCZQ", "ZJQ", text.Split('_'), false);
                    // new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_ZJQ_SPInfo, JCZQ_Odds_ZJQ>("JCZQ", "ZJQ", text.Split('_'), false);


                    break;
                case NoticeType.JCZQ_BQC_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ_BQC("JCZQ_BQC_SP", "JCZQ", "BQC", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_BF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ_BF("JCZQ_BF_SP", "JCZQ", "BF", text.Split('_'), false);
                    break;
                #endregion

                #region 竞彩篮球
                case NoticeType.JCLQ_Match:
                    var jclq_array = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCLQ_MatchList("JCLQ_Match_List", jclq_array);
                    break;
                case NoticeType.JCLQ_MatchResult:
                    var jclq_match_result_array = text.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var jclq_result_array = from s in jclq_match_result_array select s.Split('_')[0];
                    var jclqBiz = new IssuseBusiness(mDB);
                    jclqBiz.Update_JCLQ_MatchResultList(jclq_result_array.ToArray());
                    new Thread(() =>
                    {
                        Common.Utilities.UsefullHelper.TryDoAction(() =>
                        {
                            jclqBiz.Update_JCLQ_HitCount(jclq_result_array.ToArray());
                        });
                    }).Start();
                    break;
                //过关
                case NoticeType.JCLQ_SF_SP:
                    new TicketGatewayAdmin() { MonDB = mDB }.UpdateOddsList_JCLQ<JCLQ_SF_SPInfo, T_JCLQ_Odds_SF>("JCLQ_DXF_SP", "JCLQ", "SF", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_RFSF_SP:
                    new TicketGatewayAdmin() { MonDB = mDB }.UpdateOddsList_JCLQ<JCLQ_RFSF_SPInfo, T_JCLQ_Odds_RFSF>("JCLQ_RFSF_SP", "JCLQ", "RFSF", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_SFC_SP:
                    new TicketGatewayAdmin() { MonDB = mDB }.UpdateOddsList_JCLQ<JCLQ_SFC_SPInfo, T_JCLQ_Odds_SFC>("JCLQ_SFC_SP", "JCLQ", "SFC", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_DXF_SP:
                    new TicketGatewayAdmin() { MonDB = mDB }.UpdateOddsList_JCLQ<JCLQ_DXF_SPInfo, T_JCLQ_Odds_DXF>("JCLQ_DXF_SP", "JCLQ", "DXF", text.Split('_'), false);
                    break;
                #endregion

                #region 数字彩奖池：双色球、大乐透
                case NoticeType.SZC_MatchPool:
                    var tmpSZC_MatchPool = text.Split('_');
                    new IssuseBusiness(mDB).UpdateBonusPool_SZC(tmpSZC_MatchPool[0], tmpSZC_MatchPool[1]);
                    break;
                #endregion

                #region 欧洲杯

                case NoticeType.JCOZB_GJ:
                    var gjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCZQ_OZB_GJ(gjMatchIdArray);
                    break;
                case NoticeType.JCOZB_GYJ:
                    var gyjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCZQ_OZB_GYJ(gyjMatchIdArray);
                    break;

                #endregion

                #region 世界杯
                case NoticeType.JCSJB_GJ:
                    var sjbgjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCZQ_SJB_GJ(sjbgjMatchIdArray);
                    break;
                case NoticeType.JCSJB_GYJ:
                    var sjbgyjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness(mDB).Update_JCZQ_SJB_GYJ(sjbgyjMatchIdArray);
                    break;
                #endregion

                default:
                    break;
            }

        }

        /// <summary>
        /// 按彩种查询未派奖的票并执行派奖
        /// </summary>
        public string QueryUnPrizeTicketAndDoPrizeByGameCode(string gameCode, string gameType, int count)
        {
            if (string.IsNullOrEmpty(gameCode))
                throw new Exception("彩种不能为空");

            var watch = new Stopwatch();

            var successCount = 0;
            var failCount = 0;
            var log = new List<string>();
            var manager = new Sports_Manager();
            var poolManager = new Ticket_BonusManager();

            var szcArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            var poolList = new List<T_Ticket_BonusPool>();
            var ticketList = new List<TicketPrizeInfo>();
            watch.Start();
            if (szcArray.Contains(gameCode.ToUpper()))
            {
                //数字彩
                ticketList.AddRange(manager.QuerySZCUnPrizeTicket(gameCode, count));
            }
            if (gameCode == "CTZQ")
            {
                //传统足球
                ticketList.AddRange(manager.QueryCTZQUnPrizeticket(gameType, count));
            }
            watch.Stop();
            log.Add(string.Format("查询{0}-{1}-{2}，用时{3}毫秒", gameCode, gameType, count, watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            var prizeList = new List<TicketBatchPrizeInfo>();
            foreach (var item in ticketList)
            {
                try
                {
                    //查询奖池
                    if (new string[] { "SSQ", "DLT", "CTZQ" }.Contains(gameCode))
                    {
                        var pCount = poolList.Where(p => p.GameCode == item.GameCode && (gameType == "" || p.GameType == gameType) && p.IssuseNumber == item.IssuseNumber).Count();
                        if (pCount <= 0)
                        {
                            poolList.AddRange(poolManager.GetBonusPool(item.GameCode, (gameType == "" ? "" : item.GameType), item.IssuseNumber));
                        }
                    }

                    var totalPreMoney = 0M;
                    var totalAfterMoney = 0M;
                    //计算中奖金额
                    DoComputeTicketBonusMoney(item.TicketId, item.GameCode.ToUpper(), item.GameType.ToUpper(), item.BetContent, item.Amount, item.IsAppend, item.IssuseNumber, item.WinNumber, poolList, out totalPreMoney, out totalAfterMoney);

                    //更新票数据sql
                    prizeList.Add(new TicketBatchPrizeInfo
                    {
                        //Id = item.Id,
                        TicketId = item.TicketId,
                        BonusStatus = totalAfterMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                        PreMoney = totalPreMoney,
                        AfterMoney = totalAfterMoney,
                    });

                    //var updateSql = string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where ticketId='{3}' "
                    //    , totalPreMoney, totalAfterMoney, totalAfterMoney > 0M ? 20 : 30, item.TicketId);
                    //manager.ExecSql(updateSql);
                    //sqlList.Add(updateSql);

                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    log.Add(string.Format("票{0} 派奖异常：{1}", item.TicketId, ex.Message));
                }
            }
            watch.Stop();
            log.Add(string.Format("计算票中奖数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));


            watch.Restart();
            //批量更新数据库
            BusinessHelper.UpdateTicketBonus(prizeList);
            watch.Stop();
            log.Add(string.Format("执行更新票数据，用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            //if (sqlList.Count > 0)
            //{
            //    //执行更新sql
            //    //manager.ExecSql(string.Join(Environment.NewLine, sqlList));
            //}

            log.Insert(0, string.Format("成功派奖票：{0}条，失败派奖票：{1}条", successCount, failCount));
            return string.Join(Environment.NewLine, log.ToArray());
        }
        private void DoComputeTicketBonusMoney(string ticketId, string gameCode, string gameType, string betContent, int amount, bool isAppend, string issuseNumber, string winNumber, List<T_Ticket_BonusPool> poolList, out decimal totalPreMoney, out decimal totalAfterMoney)
        {
            totalPreMoney = 0M;
            totalAfterMoney = 0M;
            if (winNumber == "-")
            {
                totalPreMoney = -1M;
                totalAfterMoney = -1M;
                return;
            }
            var ruleManaager = new BonusRuleManager();
            var bonusManager = new Ticket_BonusManager();
            foreach (var item in betContent.Split('/'))
            {
                var ticketPreMoney = 0M;
                var ticketAfterMoney = 0M;
                var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, gameType);
                var bonusLevelList = analyzer.CaculateBonus(item, winNumber);
                //HCount += AnalyzerFactory.GetHitCount(item, winNumber);
                foreach (var level in bonusLevelList)
                {
                    //取中奖规则
                    var rule = ruleManaager.GetBonusRule(gameCode, gameType, level);
                    if (rule == null)
                        throw new Exception(string.Format("票{0}找不到对应的奖金规则{1}", ticketId, level));
                    var money = rule.BonusMoney;
                    if (money == -1M)
                    {
                        var pGameType = (gameCode == "SSQ" || gameCode == "DLT") ? "" : gameType;
                        var pool = poolList.FirstOrDefault(p => p.GameCode == gameCode && (pGameType == "" || p.GameType == pGameType) && p.IssuseNumber == issuseNumber && p.BonusLevel == level.ToString());
                        if (pool == null)
                        {
                            throw new Exception("未更新奖池数据 - " + gameCode + "." + gameType);
                        }
                        money = pool.BonusMoney;
                    }

                    //追加投注
                    var appendMoney = 0M;
                    if (level <= 3 && isAppend)
                        appendMoney = money * 0.6M;
                    else if (level > 3 && level <= 5 && isAppend)
                        appendMoney = money * 0.5M;

                    var bonusMoney = (money + appendMoney) * amount;
                    ticketPreMoney += bonusMoney;
                    var taxMoney = 0M;
                    //扣税
                    if ((money + appendMoney) >= 10000M)
                    {
                        taxMoney = (money + appendMoney) * 0.2M * amount;
                    }
                    ticketAfterMoney += bonusMoney - taxMoney;
                }

                totalPreMoney += ticketPreMoney;
                totalAfterMoney += ticketAfterMoney;
            }
        }

        /// <summary>
        /// 传统足球、数字彩奖期派奖
        /// </summary>
        public void LotteryIssusePrize(string gameCode, string gameType, string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber))
                throw new Exception("期号不能为空");
            if (string.IsNullOrEmpty(winNumber))
                throw new Exception("开奖号不能为空");

            var lotteryManager = new LotteryGameManager();
            var issuseEntity = lotteryManager.QueryGameIssuseByKey(gameCode, gameType, issuseNumber);
            if (issuseEntity == null)
                throw new Exception(string.Format("奖期{0}.{1}.{2}不存在", gameCode, gameType, issuseNumber));
            if (issuseEntity.OfficialStopTime > DateTime.Now)
                throw new Exception(string.Format("奖期{0}销售未结束，不能派奖", issuseNumber));
            //if (issuseEntity.Status == IssuseStatus.Awarded)
            //    throw new Exception(string.Format("奖期{0}.{1}.{2}奖期状态不正确", gameCode, gameType, issuseNumber));
            if (issuseEntity.Status == (int)IssuseStatus.Stopped)
                throw new Exception(string.Format("奖期{0}.{1}.{2}奖期已派奖", gameCode, gameType, issuseNumber));

            issuseEntity.Status = (int)IssuseStatus.Stopped;
            issuseEntity.AwardTime = DateTime.Now;
            issuseEntity.WinNumber = winNumber;
            lotteryManager.UpdateGameIssuse(issuseEntity);

            //更新Redis缓存
            RedisMatchBusiness.LoadSZCWinNumber(gameCode);
        }

        public int CheckBettingOrderMoney(List<Sports_AnteCodeInfo> codeList, string gameCode, string gameType, string playType, int amount, decimal schemeTotalMoney, DateTime stopTime, bool isAllow = false, string userId = "")
        {
            //验证投注号码
            if (stopTime < DateTime.Now)
                throw new LogicException("投注结束时间不能小于当前时间");

            //验证投注内容是否合法，或是否重复
            foreach (var item in codeList)
            {
                var oneCodeArray = item.AnteCode.Split(',');
                if (oneCodeArray.Distinct().Count() != oneCodeArray.Length)
                    throw new LogicException(string.Format("投注号码{0}中包括重复的内容", item.AnteCode));
                BettingHelper.CheckSportAnteCode(gameCode, string.IsNullOrEmpty(item.GameType) ? gameType : item.GameType.ToUpper(), oneCodeArray);
            }

            var tmp = playType.Split('|');
            var totalMoney = 0M;
            var totalCount = 0;

            var c = new Combination();
            foreach (var chuan in tmp)
            {
                var chuanArray = chuan.Split('_');
                if (chuanArray.Length != 2) continue;

                var m = int.Parse(chuanArray[0]);
                var n = int.Parse(chuanArray[1]);

                //串关包括的真实串数
                var countList = SportAnalyzer.AnalyzeChuan(m, n);
                if (n > 1)
                {
                    //3_3类型
                    c.Calculate(codeList.ToArray(), m, (arr) =>
                    {
                        //m场比赛
                        if (arr.Select(p => p.MatchId).Distinct().Count() == m)
                        {
                            foreach (var count in countList)
                            {
                                //M串1
                                c.Calculate(arr, count, (a1) =>
                                {
                                    var cCount = 1;
                                    foreach (var t in a1)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                });
                            }
                        }

                    });
                }
                else
                {
                    var ac = new ArrayCombination();
                    var danList = codeList.Where(a => a.IsDan).ToList();
                    //var tuoList = codeList.Where(a => !a.IsDan).ToList();
                    var totalCodeList = new List<Sports_AnteCodeInfo[]>();
                    foreach (var g in codeList.GroupBy(p => p.MatchId))
                    {
                        totalCodeList.Add(codeList.Where(p => p.MatchId == g.Key).ToArray());
                    }
                    //3_1类型
                    foreach (var count in countList)
                    {
                        //c.Calculate(totalCodeList.ToArray(), count - danList.Count, (arr2) =>
                        c.Calculate(totalCodeList.ToArray(), count, (arr2) =>
                        {
                            ac.Calculate(arr2, (tuoArr) =>
                            {
                                #region 拆分组合票

                                var isContainsDan = true;
                                foreach (var dan in danList)
                                {
                                    var con = tuoArr.FirstOrDefault(p => p.MatchId == dan.MatchId);
                                    if (con == null)
                                    {
                                        isContainsDan = false;
                                        break;
                                    }
                                }

                                if (isContainsDan)
                                {
                                    var cCount = 1;
                                    foreach (var t in tuoArr)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                    //if (!isAllow && BusinessHelper.IsSiteLimit(gameCode))
                                    //{
                                    //    if (!string.IsNullOrEmpty(userId) && !BusinessHelper.IsSpecificUser(userId))//如果是特定用户，则不限制投注
                                    //    {
                                    //        if ((cCount * amount * 2M) < 50)
                                    //            throw new LogicException("您好，根据您投注的内容将产生多张彩票，每张彩票金额不足50元，请您增加倍数以达到出票条件。");
                                    //    }
                                    //}
                                }

                                #endregion
                            });
                        });
                    }
                }
            }
            totalMoney = totalCount * amount * 2M;

            if (totalMoney != schemeTotalMoney) throw new ArgumentException(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。", totalMoney, schemeTotalMoney));
            return totalCount;
        }

        public DateTime CheckGeneralBettingMatch(Sports_Manager sportsManager, string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                if (gameType.ToUpper() == "SF")
                {
                    var SFGGMatchList = sportsManager.QuerySFGGSaleMatchCount(matchIdArray);

                    if (SFGGMatchList.Count != matchIdArray.Length)
                        throw new LogicException("所选比赛中有停止销售的比赛。");
                    BettingHelper.CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                    return SFGGMatchList.Min(m => m.BetStopTime);
                }
                else
                {

                    if (matchList.Count != matchIdArray.Length)
                        throw new LogicException("所选比赛中有停止销售的比赛。");
                    BettingHelper.CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                    return matchList.Min(m => m.LocalStopTime);
                }
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = sportsManager.QueryJCZQSaleMatchCount(matchIdArray);
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                //var matchResultList = sportsManager.QueryJCZQMatchResult(matchIdArray);
                //if (matchResultList.Count > 0)
                //    throw new ArgumentException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

                BettingHelper.CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);

                //if (playType == "1_1")
                if (bettingCategory != null && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "JCLQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = sportsManager.QueryJCLQSaleMatchCount(matchIdArray);
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                var matchResultList = sportsManager.QueryJCLQMatchResult(matchIdArray);
                if (matchResultList.Count > 0)
                    throw new LogicException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

                BettingHelper.CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);

                //if (playType == "1_1")
                if (bettingCategory != null && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "CTZQ")
            {
                var issuse = new LotteryGameManager().QueryGameIssuseByKey(gameCode, gameType, issuseNumber);
                if (issuse == null)
                    throw new LogicException(string.Format("{0},{1}奖期{2}不存在", gameCode, gameType, issuseNumber));
                if (issuse.LocalStopTime < DateTime.Now)
                    throw new LogicException(string.Format("{0},{1}奖期{2}结束时间为{3}", gameCode, gameType, issuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                return issuse.LocalStopTime;
            }
            //其它数字彩
            var currentIssuse = new LotteryGameManager().QueryGameIssuseByKey(gameCode, gameCode == "CTZQ" ? gameType : string.Empty, issuseNumber);
            if (currentIssuse == null)
                throw new LogicException(string.Format("{0},{1}奖期{2}不存在", gameCode, gameType, issuseNumber));
            if (currentIssuse.LocalStopTime < DateTime.Now)
                throw new LogicException(string.Format("{0},{1}奖期{2}结束时间为{3}", gameCode, gameType, issuseNumber, currentIssuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
            return currentIssuse.LocalStopTime;
        }

        /// <summary>
        /// 保存用户未投注订单
        /// </summary>
        public string SaveOrderSportsBetting(Sports_BetingInfo info, string userId)
        {
            string schemeId = string.Empty;
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;
            schemeId = BettingHelper.GetSportsBettingSchemeId(gameCode);
            var sportsManager = new Sports_Manager();
            //验证比赛是否还可以投注
            var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            // 检查订单金额是否匹配
            var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            //开启事务
            using (DB)
            {
                try
                {
                    DB.Begin();
                    AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                        info.SchemeSource, info.Security, SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                    foreach (var item in info.AnteCodeList)
                    {
                        //sportsManager.AddSports_AnteCode(new Sports_AnteCode
                        //{
                        //    SchemeId = schemeId,
                        //    AnteCode = item.AnteCode,
                        //    BonusStatus = BonusStatus.Waitting,
                        //    CreateTime = DateTime.Now,
                        //    GameCode = gameCode,
                        //    GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        //    IsDan = item.IsDan,
                        //    IssuseNumber = info.IssuseNumber,
                        //    MatchId = item.MatchId,
                        //    PlayType = info.PlayType,
                        //    Odds = string.Empty,
                        //});
                        var entity = new C_Sports_AnteCode
                        {
                            SchemeId = schemeId,
                            AnteCode = item.AnteCode,
                            BonusStatus = (int)BonusStatus.Waitting,
                            CreateTime = DateTime.Now,
                            GameCode = gameCode,
                            GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                            IsDan = item.IsDan,
                            IssuseNumber = info.IssuseNumber,
                            MatchId = item.MatchId,
                            PlayType = info.PlayType,
                            Odds = string.Empty,
                        };
                        DB.GetDal<C_Sports_AnteCode>().Add(entity);

                    }
                    //用户的订单保存
                    //sportsManager.AddUserSaveOrder(new UserSaveOrder
                    //{
                    //    SchemeId = schemeId,
                    //    UserId = userId,
                    //    GameCode = info.GameCode,
                    //    GameType = info.GameType,
                    //    PlayType = info.PlayType,
                    //    SchemeType = SchemeType.SaveScheme,
                    //    SchemeSource = info.SchemeSource,
                    //    SchemeBettingCategory = info.BettingCategory,
                    //    ProgressStatus = ProgressStatus.Waitting,
                    //    IssuseNumber = info.IssuseNumber,
                    //    Amount = info.Amount,
                    //    BetCount = betCount,
                    //    TotalMoney = info.TotalMoney,
                    //    StopTime = stopTime,
                    //    CreateTime = DateTime.Now,
                    //    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                    //});
                    var entity_U = new C_UserSaveOrder
                    {
                        SchemeId = schemeId,
                        UserId = userId,
                        GameCode = info.GameCode,
                        GameType = info.GameType,
                        PlayType = info.PlayType,
                        SchemeType = (int)SchemeType.SaveScheme,
                        SchemeSource = (int)info.SchemeSource,
                        SchemeBettingCategory = (int)info.BettingCategory,
                        ProgressStatus = (int)ProgressStatus.Waitting,
                        IssuseNumber = info.IssuseNumber,
                        Amount = info.Amount,
                        BetCount = betCount,
                        TotalMoney = info.TotalMoney,
                        StopTime = stopTime,
                        CreateTime = DateTime.Now,
                        StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                    };
                    DB.Commit();
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }

                // biz.CommitTran();
            }
            return schemeId;
        }

        public List<Antecode> FomartTR9AnteCodeList(GatewayAnteCode code)
        {
            var list = new List<Antecode>();
            //var tr9 = "3,1,0,31,11,10,30,*,*,*,*,*,1,1|0,1,2,3,4";
            var source = code.AnteNumber.Split('|');
            var array = source[0].Split(',');
            var danArray = source.Length == 1 ? new string[] { } : source[1].Split(',');

            //找出数字的索引
            var numberIndexList = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != "*")
                    numberIndexList.Add(i);
            }
            if (numberIndexList.Count == 9)
            {
                if (danArray.Length > 0)
                    throw new LogicException("任9玩法选9场时不能设胆");
                list.Add(new Antecode
                {
                    AnteNumber = code.AnteNumber,
                    GameType = code.GameType,
                });
                return list;
            }

            //计算应该替换为*的数组索引的组合
            var replaceIndexList = new List<int[]>();
            new Common.Algorithms.Combination().Calculate(numberIndexList.ToArray(), numberIndexList.Count - 9, (p) =>
            {
                var containDan = false;
                if (danArray.Length > 0)
                {
                    foreach (var item in p)
                    {
                        if (danArray.Contains(item.ToString()))
                        {
                            containDan = true;
                            break;
                        }
                    }
                }
                if (!containDan)
                {
                    replaceIndexList.Add(p);
                    var t = string.Join(",", p);
                }
            });

            //按索引替换
            foreach (var indexArray in replaceIndexList)
            {
                var tempCode = source[0].Split(',');
                foreach (var index in indexArray)
                {
                    tempCode[index] = "*";
                }
                var replaceCode = string.Join(",", tempCode);
                list.Add(new Antecode
                {
                    AnteNumber = replaceCode,
                    GameType = code.GameType,
                });
            }

            return list;
        }



        public TicketCollection AnalyzeOrder(GatewayTicketOrder order)
        {
            Order o = new Order()
            {
                GameCode = order.GameCode,
                Amount = order.Amount,
            };
            foreach (var a in order.AnteCodeList)
            {
                if (order.GameCode == "CTZQ" && a.GameType == "TR9")
                {
                    o.AntecodeList.AddRange(FomartTR9AnteCodeList(a));
                }
                else
                {
                    o.AntecodeList.Add(new Antecode()
                    {
                        AnteNumber = a.AnteNumber,
                        GameType = a.GameType,
                    });
                }
            }
            var ticketList = BettingHelper.AnalyzeTickets(o);
            ticketList.AnalyzeOrderEx(order.GameCode, order.Price);
            if (ticketList.TotalMoney != order.TotalMoney)
            {
                throw new LogicException(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", ticketList.TotalMoney, order.TotalMoney));
            }
            return ticketList;
        }
        /// <summary>
        /// 拆票，数字彩、传统足球
        /// </summary>
        public void RequestTicket2(GatewayTicketOrder orderInfo, string keyLine, bool stopAfterBonus, SchemeType schemeType)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                var redisTicketList = new List<RedisTicketInfo>();
                var gameType = string.Empty;
                var ticketList = AnalyzeOrder(orderInfo);
                //var ticketTable = BettingHelper.GetNewTicketTable();
                var ticketTable = new List<C_Sports_Ticket>();
                var index = 1;
                var locOdds = string.Empty;
                foreach (var ticket in ticketList)
                {
                    var locBetContent = ticket.ToAnteString_Localhost();
                    if (orderInfo.GameCode == "OZB")
                    {
                        //欧洲杯特殊处理
                        var locOddsList = new List<string>();
                        var betArray = locBetContent.Split(',');
                        var ozbMatchList = new JCZQMatchManager().QueryJCZQ_OZBMatchList(ticket.GameType, betArray);
                        foreach (var c in betArray)
                        {
                            var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                            if (m == null) continue;
                            locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                        }
                        locOdds = string.Join("/", locOddsList);
                    }
                    if (orderInfo.GameCode == "SJB")
                    {
                        //欧洲杯特殊处理
                        var locOddsList = new List<string>();
                        var betArray = locBetContent.Split(',');
                        var ozbMatchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(ticket.GameType, betArray);
                        foreach (var c in betArray)
                        {
                            var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                            if (m == null) continue;
                            locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                        }
                        locOdds = string.Join("/", locOddsList);
                    }

                    var betType = ticket.BetCount <= 1 ? "DS" : "FS";
                    var info = new
                    {
                        OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                        BetType = BettingHelper.ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
                        IssueNumber = orderInfo.IssuseNumber,
                        BetUnits = ticket.BetCount,
                        Multiple = ticket.Amount,
                        BetMoney = ticket.TicketMoney,
                        IsAppend = orderInfo.IsAppend ? 1 : 0,
                        BetContent = locBetContent,
                    };

                    //sql.Append(CreateInsertSql(orderInfo, ticket, info, locOdds));
                    //System.Data.DataRow r = ticketTable.NewRow();
                    var item = new C_Sports_Ticket();
                    //r["Id"] = index;
                    //r["GameCode"] = orderInfo.GameCode.ToUpper();
                    //r["GameType"] = ticket.GameType.ToUpper();
                    //r["SchemeId"] = orderInfo.OrderId;
                    //r["TicketId"] = info.OrderId;
                    //r["IssuseNumber"] = orderInfo.IssuseNumber;
                    //r["PlayType"] = info.BetType;
                    //r["BetContent"] = info.BetContent;
                    //r["TicketStatus"] = 90;
                    //r["Amount"] = info.Multiple;
                    //r["BetUnits"] = info.BetUnits;
                    //r["BetMoney"] = info.BetMoney;
                    //r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                    //r["IsAppend"] = orderInfo.IsAppend;
                    //r["BonusStatus"] = 0;
                    //r["PreTaxBonusMoney"] = 0M;
                    //r["AfterTaxBonusMoney"] = 0M;
                    //r["PrintDateTime"] = DateTime.Now;
                    //r["CreateTime"] = DateTime.Now;
                    //r["LocOdds"] = locOdds;

                    item.Id = index;
                    item.GameCode = orderInfo.GameCode.ToUpper();
                    item.GameType = ticket.GameType.ToUpper();
                    item.SchemeId = orderInfo.OrderId;
                    item.TicketId = info.OrderId;
                    item.IssuseNumber = orderInfo.IssuseNumber;
                    item.PlayType = info.BetType;
                    item.BetContent = info.BetContent;
                    item.TicketStatus = 90;
                    item.Amount = info.Multiple;
                    item.BetUnits = info.BetUnits;
                    item.BetMoney = info.BetMoney;
                    item.PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper();
                    item.IsAppend = orderInfo.IsAppend;
                    item.BonusStatus = 0;
                    item.PreTaxBonusMoney = 0M;
                    item.AfterTaxBonusMoney = 0M;
                    item.PrintDateTime = DateTime.Now;
                    item.CreateTime = DateTime.Now;
                    item.LocOdds = locOdds;

                    if (string.IsNullOrEmpty(gameType))
                        gameType = ticket.GameType.ToUpper();
                    redisTicketList.Add(new RedisTicketInfo
                    {
                        AfterBonusMoney = 0M,
                        Amount = info.Multiple,
                        BetContent = info.BetContent,
                        BetMoney = info.BetMoney,
                        BetUnits = ticket.BetCount,
                        BonusStatus = BonusStatus.Waitting,
                        GameCode = orderInfo.GameCode.ToUpper(),
                        GameType = ticket.GameType.ToUpper(),
                        IsAppend = orderInfo.IsAppend,
                        IssuseNumber = orderInfo.IssuseNumber,
                        LocOdds = locOdds,
                        MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                        PlayType = "",
                        PreBonusMoney = 0M,
                        SchemeId = orderInfo.OrderId,
                        TicketId = info.OrderId,
                    });
                    ticketTable.Add(item);
                }

                //var watch = new Stopwatch();
                //watch.Start();
                var manager = new Sports_Manager();
                try
                {
                    //批量插入票表
                    manager.SqlBulkAddTable(ticketTable);
                }
                catch (Exception exp)
                {

                    Log4Log.Error("SqlBulkAddTableError-" + orderInfo.OrderId, exp);
                    return;
                }
                //manager.ExecSql(sql.ToString());
                //Common.Database.DbDapper.DapperHelper.ExecuteSql(sql.ToString());
                if (orderInfo.GameCode == "OZB")
                {
                    //更新号码表比赛SP
                    var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                    foreach (var item in codeList)
                    {
                        if (!string.IsNullOrEmpty(item.Odds))
                            continue;

                        item.Odds = locOdds;
                        manager.UpdateSports_AnteCode(item);
                    }
                }
                if (orderInfo.GameCode == "SJB")
                {
                    //更新号码表比赛SP
                    var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                    foreach (var item in codeList)
                    {
                        if (!string.IsNullOrEmpty(item.Odds))
                            continue;

                        item.Odds = locOdds;
                        manager.UpdateSports_AnteCode(item);
                    }
                }

                //调用Redis保存
                if (RedisHelperEx.EnableRedis)
                    RedisOrderBusiness.AddToRunningOrder_SZC(schemeType, orderInfo.GameCode.ToUpper(), gameType, orderInfo.OrderId, keyLine, stopAfterBonus, orderInfo.IssuseNumber, redisTicketList);

            }
            catch (Exception exp)
            {
                Log4Log.Error("RequestTicketError-" + orderInfo.OrderId + " 保存票数据报错" + sql.ToString(), exp);
            }
            //watch.Stop();
            //this.writer.Write("RequestTicket", orderInfo.OrderId, LogType.Information, "保存票数据计时", "用时 " + watch.Elapsed.TotalMilliseconds);
        }

        //根据playType 1_1判断是单关  区DS下面
        public string GetOddsToMatchId_New(string matchId, string gameCode, string gameType)
        {
            var strList = new List<string>();
            if (gameCode.ToUpper() == "JCZQ")
            {
                var oddManager = new JCZQ_OddsManager();
                switch (gameType.ToUpper())
                {
                    case "SPF":
                        var entity_SPF = oddManager.GetLastOdds<T_JCZQ_Odds_SPF>(gameType.ToUpper(), matchId, false);
                        if (entity_SPF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SPF.GetOddsString()));
                        break;
                    case "BRQSPF":
                        var entity_BRQSPF = oddManager.GetLastOdds<T_JCZQ_Odds_BRQSPF>(gameType.ToUpper(), matchId, false);
                        if (entity_BRQSPF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BRQSPF.GetOddsString()));
                        break;
                    case "BF":
                        var entity_BF = oddManager.GetLastOdds<T_JCZQ_Odds_BF>(gameType.ToUpper(), matchId, false);
                        if (entity_BF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BF.GetOddsString()));
                        break;
                    case "ZJQ":
                        var entity_ZJQ = oddManager.GetLastOdds<T_JCZQ_Odds_ZJQ>(gameType.ToUpper(), matchId, false);
                        if (entity_ZJQ == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_ZJQ.GetOddsString()));
                        break;
                    case "BQC":
                        var entity_BQC = oddManager.GetLastOdds<T_JCZQ_Odds_BQC>(gameType.ToUpper(), matchId, false);
                        if (entity_BQC == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BQC.GetOddsString()));
                        break;
                    default:
                        break;
                }
            }
            if (gameCode.ToUpper() == "JCLQ")
            {
                var oddManager = new JCLQ_OddsManager();
                switch (gameType.ToUpper())
                {
                    case "SF":
                        var entity_SF = oddManager.GetLastOdds<T_JCLQ_Odds_SF>(gameType.ToUpper(), matchId, false);
                        if (entity_SF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SF.GetOddsString()));
                        break;
                    case "RFSF":
                        var entity_RFSF = oddManager.GetLastOdds<T_JCLQ_Odds_RFSF>(gameType.ToUpper(), matchId, false);
                        if (entity_RFSF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_RFSF.GetOddsString()));
                        break;
                    case "SFC":
                        var entity_SFC = oddManager.GetLastOdds<T_JCLQ_Odds_SFC>(gameType.ToUpper(), matchId, false);
                        if (entity_SFC == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SFC.GetOddsString()));
                        break;
                    case "DXF":
                        var entity_DXF = oddManager.GetLastOdds<T_JCLQ_Odds_DXF>(gameType.ToUpper(), matchId, false);
                        if (entity_DXF == null)
                            throw new LogicException("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_DXF.GetOddsString()));
                        break;
                    default:
                        break;
                }
            }
            return string.Join("/", strList);
        }

        /// <summary>
        /// 拆票，北单、竞彩
        /// </summary>
        public void RequestTicket_Sport(GatewayTicketOrder_Sport orderInfo)
        {
            var matchIdArray = new string[] { };
            var matchIdOddsList = new Dictionary<string, string>();
            if (orderInfo.GameCode.ToUpper() == "JCZQ" || orderInfo.GameCode.ToUpper() == "JCLQ" || orderInfo.GameCode.ToUpper() == "BJDC")
            {
                matchIdArray = orderInfo.AnteCodeList.Select(p => p.MatchId).Distinct().ToArray();

                foreach (var item in orderInfo.AnteCodeList)
                {
                    matchIdOddsList.Add(string.Format("{0}_{1}", item.GameType.ToUpper(), item.MatchId), GetOddsToMatchId_New(item.MatchId, orderInfo.GameCode.ToUpper(), item.GameType.ToUpper()));
                }
            }

            var order = string.IsNullOrEmpty(orderInfo.Attach) ?
                AnalyzeOrder_Sport<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL")
               : AnalyzeOrder_Sport_YH<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL", orderInfo.Attach);


            RequestTicket_Sport(order, orderInfo.IsRunningTicket, matchIdOddsList, matchIdArray);
        }


        /// <summary>
        /// 优化投注运用
        /// </summary>
        public TOrder AnalyzeOrder_Sport_YH<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway, string attch)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            var maxAmount = GetMaxTicketAmount(order.GameCode);
            var result = new TOrder
            {
                AgentId = "agentId",
                OrderId = order.OrderId,
                TicketGateway = gateway,
                GameCode = order.GameCode,
                GameType = order.GameType,
                PlayType = order.PlayType,
                IssuseNumber = order.IssuseNumber,
                TotalMoney = order.TotalMoney,
                Amount = order.Amount,
                Attach = order.Attach,
                Price = order.Price,
                RequestTime = DateTime.Now,
                TicketStatus = ProgressStatus.Running,
                TicketTime = null,
                TotalMatchCount = order.AnteCodeList.Count,

                TicketList = new List<TTicket>(),
            };
            //          140915018_brqspf_3|140915019_spf_3^3
            //        ,140915018_spf_3|140915019_spf_3^1
            var strList = attch.Split(',');
            var num = 1;
            var gameType = "HH";
            //if (order.AnteCodeList.GroupBy(an => an.GameType).Count() == 1)
            //{
            //    gameType = order.AnteCodeList.GroupBy(an => an.GameType).First().Key ?? order.GameType;
            //}
            foreach (var item in strList)
            {
                //item   140915018_brqspf_3|140915019_spf_3^3
                var arrayList = item.Split('^');
                if (arrayList.Length != 2)
                    throw new LogicException("优化投注拆票有错！");

                if (item.Split('|').Count() > 1)
                {
                    var checkGameTypeArray = item.Split('|');
                    var strL = new List<string>();
                    foreach (var check in checkGameTypeArray)
                    {
                        var arry = check.Split('_');
                        if (arry != null && arry.Length == 3)
                            strL.Add(arry[1]);
                    }
                    if (strL.Distinct().Count() > 1)
                        gameType = "HH";
                    else
                        gameType = strL.FirstOrDefault().ToUpper();
                }
                else
                {
                    gameType = item.Split('_')[1];
                }
                //var checkGameTypeArray = item.Split('_');
                //if (checkGameTypeArray[1] == checkGameTypeArray[3])
                //    gameType = checkGameTypeArray[1].ToUpper();
                //else
                //    gameType = "HH";

                //if (anteList.Length != 2)
                //    throw new LogicException("优化投注拆票有错！");
                var anteList = arrayList[0].Split('|');
                foreach (var ante in anteList)
                {
                    var anteArray = ante.Split('_');
                    if (order.AnteCodeList.Where(p => p.MatchId.Contains(anteArray[0])).Count() <= 0)
                        throw new LogicException("优化投注拆票有错！");
                }

                var tmpAmount = int.Parse(arrayList[1]);
                while (tmpAmount > 0)
                {
                    var currentAmount = maxAmount;
                    if (tmpAmount <= maxAmount)
                    {
                        currentAmount = tmpAmount;
                    }
                    tmpAmount -= maxAmount;
                    var tmpAmountList = GetAmountList(1, currentAmount);
                    foreach (var checkedAmount in tmpAmountList)
                    {
                        var ticket = new TTicket
                        {
                            Id = order.OrderId + "|" + (num++).ToString("D3"),
                            AgentId = "agentId",
                            OrderId = order.OrderId,
                            TicketGateway = gateway.ToUpper(),
                            GameCode = order.GameCode,
                            GameType = gameType,
                            BaseCount = anteList.Length,
                            PlayType = "P" + anteList.Length + "_1",
                            IssuseNumber = order.IssuseNumber,
                            TotalMoney = order.Price * checkedAmount,
                            Amount = checkedAmount,
                            BetCount = 1,
                            Attach = order.Attach,
                            Price = order.Price,
                            RequestTime = DateTime.Now,
                            TicketStatus = TicketStatus.Ticketing,
                            TicketTime = null,
                            TotalMatchCount = anteList.Length,
                            AnteCodeList = new List<TAnteCode>(),
                        };
                        foreach (var code in anteList)
                        {
                            var ante = new TAnteCode
                            {
                                Id = Guid.NewGuid().ToString().ToUpper(),
                                AgentId = "agentId",
                                TicketId = ticket.Id,
                                OrderId = order.OrderId,
                                GameCode = order.GameCode,
                                GameType = code.Split('_')[1].ToUpper(),
                                IssuseNumber = order.IssuseNumber,
                                MatchId = code.Split('_')[0],
                                AnteNumber = code.Split('_')[2],
                                IsDan = false,
                                BonusStatus = BonusStatus.Waitting,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now,
                            };
                            ticket.AnteCodeList.Add(ante);
                        }
                        result.TotalBetCount += ticket.BetCount;
                        result.TicketList.Add(ticket);
                    }
                }
            }
            if (!result.TicketList.Sum(t => t.TotalMoney).Equals(order.TotalMoney))
            {
                throw new LogicException(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", result.TicketList.Sum(t => t.TotalMoney), order.TotalMoney));
            }
            return result;
        }

        public TOrder AnalyzeOrder_Sport<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            var maxAmount = GetMaxTicketAmount(order.GameCode);
            var result = new TOrder
            {
                AgentId = "agentId",
                OrderId = order.OrderId,
                TicketGateway = gateway,
                GameCode = order.GameCode,
                GameType = order.GameType,
                PlayType = order.PlayType,
                IssuseNumber = order.IssuseNumber,
                TotalMoney = order.TotalMoney,
                Amount = order.Amount,
                Attach = order.Attach,
                Price = order.Price,
                RequestTime = DateTime.Now,
                TicketStatus = ProgressStatus.Running,
                TicketTime = null,
                TotalMatchCount = order.AnteCodeList.Count,

                TicketList = new List<TTicket>(),
            };
            var index = 1;
            var tmp = order.PlayType.Split('|');
            var ac = new ArrayCombination();
            var c = new Combination();
            var danList = order.AnteCodeList.Where(a => a.IsDan).ToList();
            //var tuoList = order.AnteCodeList.Where(a => !a.IsDan).ToList();
            //按比赛编号组成二维数组
            var totalCodeList = new List<GatewayAnteCode_Sport[]>();
            foreach (var g in order.AnteCodeList.GroupBy(p => p.MatchId))
            {
                totalCodeList.Add(order.AnteCodeList.Where(p => p.MatchId == g.Key).ToArray());
            }

            foreach (var chuan in tmp)
            {
                var a = int.Parse(chuan.Split('_')[0]);
                var b = int.Parse(chuan.Split('_')[1]);
                //串关包括的真实串数
                var countList = SportAnalyzer.AnalyzeChuan(a, b);
                if (b > 1)
                {
                    //3_3类型
                    c.Calculate(order.AnteCodeList.ToArray(), a, (match) =>
                    {
                        //m场比赛
                        if (match.Select(p => p.MatchId).Distinct().Count() == a)
                        {
                            var zhu = 0;
                            foreach (var count in countList)
                            {
                                var analyzer = AnalyzerFactory.GetSportAnalyzer(order.GameCode, order.GameType, count);
                                //M串1
                                c.Calculate(match, count, (arr) =>
                                {
                                    zhu += analyzer.AnalyzeAnteCode(arr);
                                });
                            }
                            index = AddToTicketListM_N<TOrder, TTicket, TAnteCode>(order, gateway, maxAmount, result, index, danList, a, b, zhu, match.ToArray());
                        }
                    });
                }
                else
                {
                    //3_1类型
                    foreach (var count in countList)
                    {
                        var analyzer = AnalyzerFactory.GetSportAnalyzer(order.GameCode, order.GameType, count);

                        c.Calculate(totalCodeList.ToArray(), count, (arr2) =>
                        {
                            ac.Calculate(arr2, (tuoArr) =>
                            {
                                #region 拆分组合票

                                var isContainsDan = true;
                                foreach (var dan in danList)
                                {
                                    var con = tuoArr.FirstOrDefault(p => p.MatchId == dan.MatchId);
                                    if (con == null)
                                    {
                                        isContainsDan = false;
                                        break;
                                    }
                                }

                                if (isContainsDan)
                                {
                                    index = AddToTicketList<TOrder, TTicket, TAnteCode>(order, gateway, maxAmount, result, index, danList, count, analyzer, tuoArr);
                                }

                                #endregion
                            });
                        });
                    }
                }

            }


            if (!result.TicketList.Sum(t => t.TotalMoney).Equals(order.TotalMoney))
            {
                throw new LogicException(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", result.TicketList.Sum(t => t.TotalMoney), order.TotalMoney));
            }
            return result;
        }

        private int AddToTicketList<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway, int maxAmount, TOrder result, int index, List<GatewayAnteCode_Sport> danList, int count, IAntecodeAnalyzable_Sport analyzer, GatewayAnteCode_Sport[] tuoArr)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            //var list = new List<GatewayAnteCode_Sport>(danList);
            var list = new List<GatewayAnteCode_Sport>();
            list.AddRange(tuoArr);
            var arr = list.ToArray();
            var gameType = "HH";
            if (arr.GroupBy(an => an.GameType).Count() == 1)
            {
                gameType = arr.GroupBy(an => an.GameType).First().Key ?? order.GameType;
            }
            var betCount = analyzer.AnalyzeAnteCode(arr);
            //if (betCount > 10000)
            //{
            //    throw new ArgumentException("超出单票金额限制 - " + (10000 * 2));
            //}
            var tmpAmount = order.Amount;
            while (tmpAmount > 0)
            {
                var currentAmount = maxAmount;
                if (tmpAmount <= maxAmount)
                {
                    currentAmount = tmpAmount;
                }
                tmpAmount -= maxAmount;
                var tmpAmountList = GetAmountList(betCount, currentAmount);
                foreach (var checkedAmount in tmpAmountList)
                {
                    var ticket = new TTicket
                    {
                        Id = order.OrderId + "|" + (index++).ToString("D3"),
                        AgentId = "agentId",
                        OrderId = order.OrderId,
                        TicketGateway = gateway,
                        GameCode = order.GameCode,
                        GameType = gameType,
                        BaseCount = count,
                        PlayType = "P" + count + "_1",
                        IssuseNumber = order.IssuseNumber,
                        TotalMoney = betCount * order.Price * checkedAmount,
                        Amount = checkedAmount,
                        BetCount = betCount,
                        Attach = order.Attach,
                        Price = order.Price,
                        RequestTime = DateTime.Now,
                        TicketStatus = TicketStatus.Ticketing,
                        TicketTime = null,
                        TotalMatchCount = count,

                        AnteCodeList = new List<TAnteCode>(),
                    };
                    foreach (var code in arr)
                    {
                        var ante = new TAnteCode
                        {
                            Id = Guid.NewGuid().ToString().ToUpper(),
                            AgentId = "agentId",
                            TicketId = ticket.Id,
                            OrderId = order.OrderId,
                            GameCode = order.GameCode,
                            GameType = code.GameType,
                            IssuseNumber = order.IssuseNumber,
                            MatchId = code.MatchId,
                            AnteNumber = code.AnteCode,
                            IsDan = code.IsDan,
                            BonusStatus = BonusStatus.Waitting,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        };
                        ticket.AnteCodeList.Add(ante);
                    }
                    result.TotalBetCount += ticket.BetCount;
                    result.TicketList.Add(ticket);
                }
            }
            return index;
        }

        private int AddToTicketListM_N<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway, int maxAmount, TOrder result, int index, List<GatewayAnteCode_Sport> danList, int count, int chuan, int zhu, GatewayAnteCode_Sport[] tuoArr)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            var list = new List<GatewayAnteCode_Sport>();
            list.AddRange(tuoArr);
            var arr = list.ToArray();
            var gameType = "HH";
            if (arr.GroupBy(an => an.GameType).Count() == 1)
            {
                gameType = arr.GroupBy(an => an.GameType).First().Key ?? order.GameType;
            }
            var betCount = zhu;
            //if (betCount > 10000)
            //{
            //    throw new ArgumentException("超出单票金额限制 - 20000");
            //}
            var tmpAmount = order.Amount;
            while (tmpAmount > 0)
            {
                var currentAmount = maxAmount;
                if (tmpAmount <= maxAmount)
                {
                    currentAmount = tmpAmount;
                }
                tmpAmount -= maxAmount;
                var tmpAmountList = GetAmountList(betCount, currentAmount);
                foreach (var checkedAmount in tmpAmountList)
                {
                    var ticket = new TTicket
                    {
                        Id = order.OrderId + "|" + (index++).ToString("D3"),
                        AgentId = "agentId",
                        OrderId = order.OrderId,
                        TicketGateway = gateway,
                        GameCode = order.GameCode,
                        GameType = gameType,
                        BaseCount = count,
                        PlayType = "P" + count + "_" + chuan,
                        IssuseNumber = order.IssuseNumber,
                        TotalMoney = betCount * order.Price * checkedAmount,
                        Amount = checkedAmount,
                        BetCount = betCount,
                        Attach = order.Attach,
                        Price = order.Price,
                        RequestTime = DateTime.Now,
                        TicketStatus = TicketStatus.Ticketing,
                        TicketTime = null,
                        TotalMatchCount = count,

                        AnteCodeList = new List<TAnteCode>(),
                    };
                    foreach (var code in arr)
                    {
                        var ante = new TAnteCode
                        {
                            Id = Guid.NewGuid().ToString().ToUpper(),
                            AgentId = "agentId",
                            TicketId = ticket.Id,
                            OrderId = order.OrderId,
                            GameCode = order.GameCode,
                            GameType = code.GameType,
                            IssuseNumber = order.IssuseNumber,
                            MatchId = code.MatchId,
                            AnteNumber = code.AnteCode,
                            IsDan = code.IsDan,
                            BonusStatus = BonusStatus.Waitting,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        };
                        ticket.AnteCodeList.Add(ante);
                    }
                    result.TotalBetCount += ticket.BetCount;
                    result.TicketList.Add(ticket);
                }
            }
            return index;
        }

        private List<int> GetAmountList(int betCount, int amount)
        {
            var list = new List<int>();
            var flag = amount;
            while (betCount * flag > 10000)
            {
                flag--;
            }
            while (amount > 0)
            {
                list.Add(System.Math.Min(flag, amount));
                amount -= flag;
            }
            return list;
        }

        public int GetMaxTicketAmount(string gameCode)
        {
            switch (gameCode.ToLower())
            {
                case "ssq":
                case "fc3d":
                    return 50;
                default:
                    return 99;
            }
        }
        /// <summary>
        /// 足彩普通投注
        /// </summary>
        public string SportsBetting(Sports_BetingInfo info, string userId, string password, string place, int totalCount, DateTime stopTime, decimal redBagMoney)
        {
            string schemeId = info.SchemeId;
            var anteCodeList = new List<EntityModel.C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;
            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            if (redBagMoney > 0M)
            {
                var fundManager = new FundManager();
                var percent = 0M;
                //JCZQ单关配置了单独的红包比例
                if (info.PlayType == "1_1" && info.GameCode.ToUpper() == "JCZQ")
                {
                    percent = fundManager.QueryRedBagUseConfig("jczqdg");
                }
                else
                {
                    percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                }
                //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);

                //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                var maxUseMoney = info.TotalMoney * percent / 100;
                if (redBagMoney > maxUseMoney)
                    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
            }

            //投注流程计时
            //   var watch = new Stopwatch();
            //  watch.Start();
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            if (string.IsNullOrEmpty(schemeId))
                schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);

            //var key = string.Format("{0}_{1}", info.GameCode, "StopTicketing");
            //string AppSetting = DBbase.GlobalConfig[key].ToString();
            //if (BettingHelper.CanRequestBet(AppSetting, gameCode))
            var canTicket = BettingHelper.CanRequestBet(info.GameCode);
            //开启事务
            DB.Begin();
            try
            {


                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                     info.IssuseNumber, info.Amount, totalCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                     SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                     canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);
                //   IList<C_Sports_AnteCode> anteList = new List<C_Sports_AnteCode>();


                foreach (var item in info.AnteCodeList)
                {
                    var entityAnteCode = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    anteCodeList.Add(entityAnteCode);

                    //sportsManager.AddSports_AnteCode(entityAnteCode);
                }
                //kson 批量优化录入
                DB.GetDal<C_Sports_AnteCode>().BulkAdd(anteCodeList);
                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BettingHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BettingHelper.FormatGameCode(info.GameCode));
                if (redBagMoney > 0M)
                {
                    //var fundManager = new FundManager();
                    //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                    //var maxUseMoney = info.TotalMoney * percent / 100;
                    //if (redBagMoney > maxUseMoney)
                    //    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
                    //红包支付
                    BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, msg, "Bet", password);
                }
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney - redBagMoney, msg, place, password);

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            //watch.Stop();
            //if (watch.Elapsed.TotalMilliseconds > 1000)
            //    writerLog.WriteLog("SportsBetting", "SQL", (int)LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #region 拆票

            //  watch.Restart();
            if (RedisHelperEx.EnableRedis)
            {
                var reidsWaitOrder = new RedisWaitTicketOrder
                {
                    RunningOrder = runningOrder,
                    AnteCodeList = anteCodeList
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, reidsWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(reidsWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }

            //watch.Stop();
            //if (watch.Elapsed.TotalMilliseconds > 1000)
            //    writerLog.WriteLog("SportsBetting", "Redis",(int) LogType.Warning, "拆票", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }

        private int AddToTicketList<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway, string agentId, int maxAmount, TOrder result, int index, List<GatewayAnteCode_Sport> danList, int count, IAntecodeAnalyzable_Sport analyzer, GatewayAnteCode_Sport[] tuoArr)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            //var list = new List<GatewayAnteCode_Sport>(danList);
            var list = new List<GatewayAnteCode_Sport>();
            list.AddRange(tuoArr);
            var arr = list.ToArray();
            var gameType = "HH";
            if (arr.GroupBy(an => an.GameType).Count() == 1)
            {
                gameType = arr.GroupBy(an => an.GameType).First().Key ?? order.GameType;
            }
            var betCount = analyzer.AnalyzeAnteCode(arr);
            //if (betCount > 10000)
            //{
            //    throw new ArgumentException("超出单票金额限制 - " + (10000 * 2));
            //}
            var tmpAmount = order.Amount;
            while (tmpAmount > 0)
            {
                var currentAmount = maxAmount;
                if (tmpAmount <= maxAmount)
                {
                    currentAmount = tmpAmount;
                }
                tmpAmount -= maxAmount;
                var tmpAmountList = GetAmountList(betCount, currentAmount);
                foreach (var checkedAmount in tmpAmountList)
                {
                    var ticket = new TTicket
                    {
                        Id = agentId + "|" + order.OrderId + "|" + (index++).ToString("D3"),
                        AgentId = agentId,
                        OrderId = order.OrderId,
                        TicketGateway = gateway,
                        GameCode = order.GameCode,
                        GameType = gameType,
                        BaseCount = count,
                        PlayType = "P" + count + "_1",
                        IssuseNumber = order.IssuseNumber,
                        TotalMoney = betCount * order.Price * checkedAmount,
                        Amount = checkedAmount,
                        BetCount = betCount,
                        Attach = order.Attach,
                        Price = order.Price,
                        RequestTime = DateTime.Now,
                        TicketStatus = TicketStatus.Ticketing,
                        TicketTime = null,
                        TotalMatchCount = count,

                        AnteCodeList = new List<TAnteCode>(),
                    };
                    foreach (var code in arr)
                    {
                        var ante = new TAnteCode
                        {
                            Id = Guid.NewGuid().ToString().ToUpper(),
                            AgentId = agentId,
                            TicketId = ticket.Id,
                            OrderId = order.OrderId,
                            GameCode = order.GameCode,
                            GameType = code.GameType,
                            IssuseNumber = order.IssuseNumber,
                            MatchId = code.MatchId,
                            AnteNumber = code.AnteCode,
                            IsDan = code.IsDan,
                            BonusStatus = BonusStatus.Waitting,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        };
                        ticket.AnteCodeList.Add(ante);
                    }
                    result.TotalBetCount += ticket.BetCount;
                    result.TicketList.Add(ticket);
                }
            }
            return index;
        }


        /// <summary>
        /// 拆票，单式上传
        /// </summary>
        public void RequestTicketByGateway_SingleScheme_New(GatewayTicketOrder_SingleScheme order)
        {
            List<string> codeList = new List<string>();
            var admin = new TicketGatewayAdmin();
            var matchIdList = new List<string>();
            var matchIdOddsList = new Dictionary<string, string>();

            using (DB)
            {

                DB.Begin();

                #region 保存订单、票数据
                try
                {

                    switch (order.GameCode.ToUpper())
                    {
                        case "BJDC":
                            codeList = admin.RequestTicket_BJDCSingleScheme(order, out matchIdList);
                            break;
                        case "JCZQ":
                            codeList = admin.RequestTicket_JCZQSingleScheme(order, out matchIdList);
                            break;
                        case "JCLQ":
                            codeList = admin.RequestTicket_JCLQSingleScheme(order, out matchIdList);
                            break;
                        case "CTZQ":
                            codeList = admin.RequestTicket_CTZQSingleScheme(order);
                            break;
                        default:
                            throw new ArgumentException("请求出票不支持的订单彩种 - " + order.GameCode);
                    }
                    //throw new NotImplementedException("请求出票不支持的订单彩种 - " + order.GameCode);
                    if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                    {
                        foreach (var item in matchIdList)
                        {
                            matchIdOddsList.Add(string.Format("{0}_{1}", order.GameType.ToUpper(), item), GetOddsToMatchId_New(item, order.GameCode.ToUpper(), order.GameType.ToUpper()));
                        }
                    }
                    DB.Commit();
                }
                catch (Exception EX)
                {
                    DB.Rollback();
                    throw EX;
                }


                #endregion


            }

            //拆票
            RequestTicket_SingleSport(order, codeList, order.Amount, matchIdOddsList, matchIdList.Distinct().ToList());

        }
        public string ConvertIssuseNumber(string gameCode, string issuseNumber)
        {
            return issuseNumber;

            //switch (gameCode.ToLower())
            //{
            //    case "ssq":
            //    case "fc3d":
            //    case "cqssc":
            //        return issuseNumber.Replace("-", "");
            //    case "dlt":
            //    case "pl3":
            //    case "jx11x5":
            //        return issuseNumber.Substring(2).Replace("-", "");
            //    default:
            //        return issuseNumber;
            //}
        }
        public void RequestTicket_SingleSport(GatewayTicketOrder_SingleScheme order, List<string> codeList, int amount, Dictionary<string, string> matchIdOddsList, List<string> orderMatchIdList)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var ticketNum = 0;
            var hisManager = new Sports_Manager();
            //key:比赛编号_玩法  value:赔率
            var oddDic = new Dictionary<string, string>();
            try
            {
                //var ticketTable = BettingHelper.GetNewTicketTable();
                var ticketTable = new List<C_Sports_Ticket>();
                var dictionary = new Dictionary<string, int>();
                foreach (var item in codeList)
                {
                    //[0]: "158|3|5_1#159|1|5_1#160|1|5_1#166|0|5_1#169|1|5_1"
                    //[1]: "158|0|5_1#159|1|5_1#160|1|5_1#166|0|5_1#169|3|5_1"
                    #region 组合一注号码

                    var matchIdList = new List<string>();
                    var betType = string.Empty;
                    //var strList = new List<string>();
                    var locBetContentList = new List<string>();
                    foreach (var matchCode in item.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        //158|0|5_1
                        var array = matchCode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != 3) continue;
                        if (string.IsNullOrEmpty(array[1]) || array[1] == "*" || array[1] == "**")
                            continue;

                        betType = array[2];
                        var matchId = ObjectExtension.ConverMatchId(order.GameCode, array[0]);
                        matchIdList.Add(array[0]);
                        //strList.Add(string.Format("{0}{1}^", ObjectExtension.ConverMatchId(order.GameCode, array[0]), ObjectExtension.ConvertAnteCode(order.GameCode, order.GameType, array[1])));
                        if (order.GameCode.ToUpper() == "BJDC" && !string.IsNullOrEmpty(order.GameType) && order.GameType.ToUpper() == "SXDS")
                            locBetContentList.Add(string.Format("{0}_{1}", array[0], array[1].Replace("0", "SD").Replace("1", "SS").Replace("2", "XD").Replace("3", "XS")));
                        else
                            locBetContentList.Add(string.Format("{0}_{1}", array[0], array[1]));
                    }

                    var locOdds = new List<string>();
                    if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                    {
                        foreach (var match in matchIdList)
                        {
                            locOdds.Add(matchIdOddsList.FirstOrDefault(p => p.Key == string.Format("{0}_{1}", order.GameType.ToUpper(), match)).Value);
                        }
                    }
                    var locOddStr = string.Join("/", locOdds);

                    #endregion

                    //最大倍数
                    var maxAmount = BettingHelper.GetMaxTicketAmount(order.GameCode);
                    var tmpAmount = amount;
                    while (tmpAmount > 0)
                    {
                        var currentAmount = maxAmount;
                        if (tmpAmount <= maxAmount)
                        {
                            currentAmount = tmpAmount;
                        }
                        tmpAmount -= maxAmount;
                        var ticketMoney = 2M * currentAmount;
                        ticketNum++;
                        var ticketId = string.Format("{0}|{1}", order.OrderId, ticketNum.ToString().PadLeft(3, '0'));
                        //var locOdds =GetOddsToMatchId(string.Join("/", locBetContentList), order.GameCode, order.GameType);
                        //var history = new Sports_Ticket
                        //{
                        //    GameCode = order.GameCode.ToUpper(),
                        //    GameType = order.GameType.ToUpper(),
                        //    SchemeId = order.OrderId,
                        //    TicketId = ticketId,
                        //    IssuseNumber = ConvertIssuseNumber(order.GameCode, order.IssuseNumber),
                        //    CreateTime = DateTime.Now,
                        //    TicketStatus = order.IsRunningTicket ? TicketStatus.Ticketing : TicketStatus.Waitting,
                        //    PlayType = string.Format("P{0}", betType),
                        //    Amount = currentAmount,
                        //    BetUnits = 1,
                        //    BetMoney = ticketMoney,
                        //    BetContent = string.Join("/", locBetContentList),
                        //    MatchIdList = string.Join(",", matchIdList.ToArray()),
                        //    LocOdds = GetOddsToMatchId(string.Join("/", locBetContentList), order.GameCode, order.GameType),
                        //    PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper(),
                        //};
                        //hisManager.AddSports_Ticket(history);

                        C_Sports_Ticket r = new C_Sports_Ticket();
                        //r["Id"] = ticketNum;
                        //r["GameCode"] = order.GameCode.ToUpper();
                        //r["GameType"] = order.GameType.ToUpper();
                        //r["SchemeId"] = order.OrderId;
                        //r["TicketId"] = ticketId;
                        //r["IssuseNumber"] = ConvertIssuseNumber(order.GameCode, order.IssuseNumber);
                        //r["PlayType"] = string.Format("P{0}", betType);
                        //r["BetContent"] = string.Join("/", locBetContentList);
                        //r["TicketStatus"] = 90;
                        //r["Amount"] = currentAmount;
                        //r["BetUnits"] = 1;
                        //r["BetMoney"] = ticketMoney;
                        //r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                        //r["MatchIdList"] = string.Join(",", matchIdList.ToArray());
                        //r["LocOdds"] = locOddStr;
                        //r["IsAppend"] = false;
                        //r["BonusStatus"] = 0;
                        //r["PreTaxBonusMoney"] = 0M;
                        //r["AfterTaxBonusMoney"] = 0M;
                        //r["PrintDateTime"] = DateTime.Now;
                        //r["CreateTime"] = DateTime.Now;

                        r.GameCode = order.GameCode.ToUpper();
                        r.GameType = order.GameType.ToUpper();
                        r.SchemeId = order.OrderId;
                        r.TicketId = ticketId;
                        r.IssuseNumber = ConvertIssuseNumber(order.GameCode, order.IssuseNumber);
                        r.PlayType = string.Format("P{0}", betType);
                        r.BetContent = string.Join("/", locBetContentList);
                        r.TicketStatus = 90;
                        r.Amount = currentAmount;
                        r.BetUnits = 1;
                        r.BetMoney = ticketMoney;
                        r.PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper();
                        r.MatchIdList = string.Join(",", matchIdList.ToArray());
                        r.LocOdds = locOddStr;
                        r.IsAppend = false;
                        r.BonusStatus = 0;
                        r.PreTaxBonusMoney = 0M;
                        r.AfterTaxBonusMoney = 0M;
                        r.PrintDateTime = DateTime.Now;
                        r.CreateTime = DateTime.Now;

                        redisTicketList.Add(new RedisTicketInfo
                        {
                            AfterBonusMoney = 0M,
                            Amount = currentAmount,
                            BetContent = string.Join("/", locBetContentList),
                            BetMoney = ticketMoney,
                            BetUnits = 1,
                            BonusStatus = BonusStatus.Waitting,
                            GameCode = order.GameCode.ToUpper(),
                            GameType = order.GameType.ToUpper(),
                            IsAppend = false,
                            IssuseNumber = ConvertIssuseNumber(order.GameCode, order.IssuseNumber),
                            LocOdds = locOddStr,
                            MatchIdList = string.Join(",", matchIdList.ToArray()),
                            PlayType = string.Format("P{0}", betType),
                            PreBonusMoney = 0M,
                            SchemeId = order.OrderId,
                            TicketId = ticketId,
                        });

                        ticketTable.Add(r);

                        #region 号码表更新对应比赛sp

                        var oddArray = locOddStr.Split('/');
                        foreach (var content in locBetContentList)
                        {
                            var contentArray = content.Split('_');
                            var gameType = string.Empty;
                            var matchId = string.Empty;
                            if (contentArray.Length == 3)
                            {
                                //混合
                                gameType = contentArray[0];
                                matchId = contentArray[1];
                            }
                            if (contentArray.Length == 2)
                            {
                                //普通
                                gameType = order.GameType.ToUpper();
                                matchId = contentArray[0];
                            }
                            if (string.IsNullOrEmpty(gameType) || string.IsNullOrEmpty(matchId)) continue;

                            var matchIdStr = string.Format("{0}_", matchId);
                            var oddItem = oddArray.FirstOrDefault(p => p.StartsWith(matchIdStr));
                            if (string.IsNullOrEmpty(oddItem)) continue;
                            var matchIdOdd = oddItem.Replace(matchIdStr, string.Empty);
                            var key = string.Format("{0}_{1}", matchId, gameType).ToUpper();
                            if (!oddDic.ContainsKey(key))
                                oddDic.Add(key, matchIdOdd);
                        }

                        #endregion
                    }
                }

                //批量插入票表
                hisManager.SqlBulkAddTable(ticketTable);
                //调用Redis保存
                if (RedisHelperEx.EnableRedis)
                {
                    if (new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode.ToUpper()))
                    {
                        RedisOrderBusiness.AddToRunningOrder_JC(order.GameCode, order.OrderId, orderMatchIdList.ToArray(), redisTicketList);
                    }
                    if (order.GameCode == "BJDC")
                    {
                        RedisOrderBusiness.AddToRunningOrder_BJDC(order.OrderId, orderMatchIdList.ToArray(), redisTicketList);
                    }
                    if (order.GameCode == "CTZQ")
                    {
                        RedisOrderBusiness.AddToRunningOrder_SZC(SchemeType.GeneralBetting, order.GameCode, order.GameType, order.OrderId, "", true, order.IssuseNumber, redisTicketList);
                    }
                }

                //更新号码表比赛SP
                var anteCodeList = hisManager.QuerySportsAnteCodeBySchemeId(order.OrderId);
                foreach (var item in anteCodeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    var key = string.Format("{0}_{1}", item.MatchId, item.GameType).ToUpper();
                    if (oddDic.ContainsKey(key))
                    {
                        var odd = oddDic[key] + ",-1|1";
                        item.Odds = odd;
                        hisManager.UpdateSports_AnteCode(item);
                    }
                }
            }
            catch (Exception ex)
            {
                //Common.Log.LogWriterGetter.GetLogWriter().Write("GatewayHandler_Shop", "RequestTicket_SingleSport", Common.Log.LogType.Information, "test", ex.ToString());
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 拆票，数字彩、传统足球
        /// </summary>
        public void RequestTicket(GatewayTicketOrder orderInfo, string keyLine, bool stopAfterBonus, SchemeType schemeType)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var gameType = string.Empty;
            var ticketList = AnalyzeOrder(orderInfo);
            //var ticketTable = BettingHelper.GetNewTicketTable();
            var ticketTable = new List<C_Sports_Ticket>();
            var index = 1;
            var locOdds = string.Empty;
            foreach (var ticket in ticketList)
            {
                var locBetContent = ticket.ToAnteString_Localhost();
                if (orderInfo.GameCode == "OZB")
                {
                    //欧洲杯特殊处理
                    var locOddsList = new List<string>();
                    var betArray = locBetContent.Split(',');
                    var ozbMatchList = new JCZQMatchManager().QueryJCZQ_OZBMatchList(ticket.GameType, betArray);
                    foreach (var c in betArray)
                    {
                        var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                        if (m == null) continue;
                        locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                    }
                    locOdds = string.Join("/", locOddsList);
                }
                if (orderInfo.GameCode == "SJB")
                {
                    //欧洲杯特殊处理
                    var locOddsList = new List<string>();
                    var betArray = locBetContent.Split(',');
                    var ozbMatchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(ticket.GameType, betArray);
                    foreach (var c in betArray)
                    {
                        var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                        if (m == null) continue;
                        locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                    }
                    locOdds = string.Join("/", locOddsList);
                }

                var betType = ticket.BetCount <= 1 ? "DS" : "FS";
                var info = new
                {
                    OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                    BetType = BettingHelper.ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
                    IssueNumber = orderInfo.IssuseNumber,
                    BetUnits = ticket.BetCount,
                    Multiple = ticket.Amount,
                    BetMoney = ticket.TicketMoney,
                    IsAppend = orderInfo.IsAppend ? 1 : 0,
                    BetContent = locBetContent,
                };

                //DataRow r = ticketTable.NewRow();
                C_Sports_Ticket r = new C_Sports_Ticket();
                //r["Id"] = index;
                //r["GameCode"] = orderInfo.GameCode.ToUpper();
                //r["GameType"] = ticket.GameType.ToUpper();
                //r["SchemeId"] = orderInfo.OrderId;
                //r["TicketId"] = info.OrderId;
                //r["IssuseNumber"] = orderInfo.IssuseNumber;
                //r["PlayType"] = info.BetType;
                //r["BetContent"] = info.BetContent;
                //r["TicketStatus"] = 90;
                //r["Amount"] = info.Multiple;
                //r["BetUnits"] = info.BetUnits;
                //r["BetMoney"] = info.BetMoney;
                //r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                //r["IsAppend"] = orderInfo.IsAppend;
                //r["BonusStatus"] = 0;
                //r["PreTaxBonusMoney"] = 0M;
                //r["AfterTaxBonusMoney"] = 0M;
                //r["PrintDateTime"] = DateTime.Now;
                //r["CreateTime"] = DateTime.Now;
                //r["LocOdds"] = locOdds;


                r.GameCode = orderInfo.GameCode.ToUpper();
                r.GameType = ticket.GameType.ToUpper();
                r.SchemeId = orderInfo.OrderId;
                r.TicketId = info.OrderId;
                r.IssuseNumber = orderInfo.IssuseNumber;
                r.PlayType = info.BetType;
                r.BetContent = info.BetContent;
                r.TicketStatus = 90;
                r.Amount = info.Multiple;
                r.BetUnits = info.BetUnits;
                r.BetMoney = info.BetMoney;
                r.PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper();
                r.IsAppend = orderInfo.IsAppend;
                r.BonusStatus = 0;
                r.PreTaxBonusMoney = 0M;
                r.AfterTaxBonusMoney = 0M;
                r.PrintDateTime = DateTime.Now;
                r.CreateTime = DateTime.Now;
                r.LocOdds = locOdds;
                if (string.IsNullOrEmpty(gameType))
                    gameType = ticket.GameType.ToUpper();
                redisTicketList.Add(new RedisTicketInfo
                {
                    AfterBonusMoney = 0M,
                    Amount = info.Multiple,
                    BetContent = info.BetContent,
                    BetMoney = info.BetMoney,
                    BetUnits = ticket.BetCount,
                    BonusStatus = BonusStatus.Waitting,
                    GameCode = orderInfo.GameCode.ToUpper(),
                    GameType = ticket.GameType.ToUpper(),
                    IsAppend = orderInfo.IsAppend,
                    IssuseNumber = orderInfo.IssuseNumber,
                    LocOdds = locOdds,
                    MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                    PlayType = "",
                    PreBonusMoney = 0M,
                    SchemeId = orderInfo.OrderId,
                    TicketId = info.OrderId,
                });

                ticketTable.Add(r);
            }

            //var watch = new Stopwatch();
            //watch.Start();
            var manager = new Sports_Manager();
            //批量插入票表
            manager.SqlBulkAddTable(ticketTable);
            if (orderInfo.GameCode == "OZB")
            {
                //更新号码表比赛SP
                var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                foreach (var item in codeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    item.Odds = locOdds;
                    manager.UpdateSports_AnteCode(item);
                }
            }
            if (orderInfo.GameCode == "SJB")
            {
                //更新号码表比赛SP
                var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                foreach (var item in codeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    item.Odds = locOdds;
                    manager.UpdateSports_AnteCode(item);
                }
            }

            //调用Redis保存
            if (RedisHelperEx.EnableRedis)
                RedisOrderBusiness.AddToRunningOrder_SZC(schemeType, orderInfo.GameCode.ToUpper(), gameType, orderInfo.OrderId, keyLine, stopAfterBonus, orderInfo.IssuseNumber, redisTicketList);

            //watch.Stop();
            //this.writer.Write("RequestTicket", orderInfo.OrderId, LogType.Information, "保存票数据计时", "用时 " + watch.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// 拆票（无线程）
        /// </summary>
        public string DoSplitOrderTicketsWithNoThread(string schemeId)
        {
            var log = new List<string>();
            log.Add(string.Format("----------->>>>>>开始对订单{0}做票拆分", schemeId));
            var watch = new Stopwatch();
            watch.Start();
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                //return;
                return string.Format("找不到订单{0}的订单数据", schemeId);
            //读取配置文件
            //var key = string.Format("{0}_{1}", order.GameCode.ToUpper(), "StopTicketing");
            //string AppSetting = DBbase.GlobalConfig[key].ToString();
            if (BettingHelper.CanRequestBet(order.GameCode))
                // if (!BusinessHelper.CanRequestBet(order.GameCode))
                //return;
                return string.Format("彩种{0}暂时不能出票", order.GameCode);
            var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            if (anteCodeList.Count <= 0)
                //return;
                return string.Format("订单{0}无投注内容", schemeId);

            if (order.SchemeType == (int)SchemeType.ChaseBetting)
            {
                //清理冻结
                BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.UserId, order.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.SchemeId, order.TotalMoney), order.TotalMoney);
            }
            var oldCount = sportsManager.QueryTicketCount(schemeId);
            watch.Stop();
            log.Add("1)查询订单和号码用时 " + watch.Elapsed.TotalMilliseconds);

            watch.Restart();
            if (oldCount <= 0)
            {
                //保存投注内容到文件
                if (order.SchemeBettingCategory == (int)SchemeBettingCategory.SingleBetting || order.SchemeBettingCategory == (int)SchemeBettingCategory.XianFaQiHSC)
                {
                    //单式
                    var singleCode = sportsManager.QuerySingleScheme_AnteCode(schemeId);
                    if (singleCode == null)
                        //return;
                        return string.Format("单式订单{0}找不到上传的号码数据", schemeId);

                    #region 拆票

                    RequestTicketByGateway_SingleScheme_New(new GatewayTicketOrder_SingleScheme
                    {
                        AllowCodes = singleCode.AllowCodes,
                        Amount = order.Amount,
                        ContainsMatchId = singleCode.ContainsMatchId,
                        FileBuffer = singleCode.FileBuffer,
                        GameCode = order.GameCode,
                        GameType = order.GameType,
                        IsRunningTicket = true,
                        IssuseNumber = order.IssuseNumber,
                        IsVirtualOrder = false,
                        OrderId = schemeId,
                        PlayType = order.PlayType,
                        SelectMatchId = singleCode.SelectMatchId,
                        TotalMoney = order.TotalMoney,
                        UserId = order.UserId,
                    });


                    //new Thread(() =>
                    //{
                    try
                    {
                        //生成文件
                        var json = Encoding.UTF8.GetString(singleCode.FileBuffer);
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                        File.WriteAllText(fileName, json, Encoding.UTF8);
                    }
                    catch (Exception)
                    {
                    }
                    //}).Start();


                    #endregion
                }
                else
                {
                    //普通投注
                    var jcGameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                    if (jcGameCodeArray.Contains(order.GameCode))
                    {
                        //竞彩
                        #region 拆票

                        var betInfo = new GatewayTicketOrder_Sport
                        {
                            Amount = order.Amount,
                            Attach = order.Attach,
                            GameCode = order.GameCode,
                            GameType = order.GameType,
                            IssuseNumber = order.IssuseNumber,
                            IsVirtualOrder = order.IsVirtualOrder,
                            OrderId = schemeId,
                            PlayType = order.PlayType,
                            UserId = order.UserId,
                            TotalMoney = order.TotalMoney,
                            Price = 2M,
                            IsRunningTicket = true,
                        };
                        foreach (var code in anteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode_Sport
                            {
                                AnteCode = code.AnteCode,
                                GameType = code.GameType,
                                IsDan = code.IsDan,
                                MatchId = code.MatchId,
                            });
                        }
                        RequestTicket_Sport(betInfo);

                        //new Thread(() =>
                        //{
                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }
                        //}).Start();
                        #endregion
                    }
                    else
                    {
                        //数字彩、传统足球
                        #region 拆票

                        var betInfo = new GatewayTicketOrder
                        {
                            Amount = order.Amount,
                            GameCode = order.GameCode,
                            IssuseNumber = order.IssuseNumber,
                            OrderId = schemeId,
                            Price = ((order.IsAppend && order.GameCode == "DLT") ? 3M : 2M),
                            TotalMoney = order.TotalMoney,
                            IsVirtualOrder = false,
                            Attach = "",
                            IsAppend = order.IsAppend,
                            UserId = order.UserId,
                            IsRunningTicket = true,
                        };
                        foreach (var item in anteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode
                            {
                                AnteNumber = item.AnteCode,
                                GameType = item.GameType,
                            });
                        }

                        RequestTicket(betInfo, "", true, (SchemeType)order.SchemeType);

                        //new Thread(() =>
                        //{

                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }

                        //}).Start();

                        #endregion
                    }
                }

                //触发出票完成接口
                BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.UserId, order.SchemeId, order.TotalMoney, order.TotalMoney });
            }
            watch.Stop();
            log.Add("2)执行拆分用时 " + watch.Elapsed.TotalMilliseconds);
            //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));

            watch.Restart();
            order.IsSplitTickets = true;
            if (!order.TicketTime.HasValue)
                order.TicketTime = DateTime.Now;
            order.TicketStatus = (int)TicketStatus.Ticketed;
            order.ProgressStatus = (int)ProgressStatus.Running;
            sportsManager.UpdateSports_Order_Running(order);

            var detailManager = new SchemeManager();
            var detail = detailManager.QueryOrderDetail(schemeId);
            if (detail != null)
            {
                if (!detail.TicketTime.HasValue)
                    detail.TicketTime = DateTime.Now;
                detail.TicketStatus = (int)TicketStatus.Ticketed;
                detail.ProgressStatus = (int)ProgressStatus.Running;
                detail.CurrentBettingMoney = detail.TotalMoney;
                detailManager.UpdateOrderDetail(detail);
            }
            watch.Stop();
            log.Add("3)拆分和修改订单数据 " + watch.Elapsed.TotalMilliseconds);
            Log4Log.Fatal("DoSplitOrderTickets-" + schemeId + "," + string.Join(Environment.NewLine, log.ToArray()));
            return string.Join(Environment.NewLine, log);
        }

        /// <summary>
        /// 订单拆票
        /// </summary>
        public void DoSplitOrderTickets(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId))
                return;
            //return "订单号不能为空";
            Task.Factory.StartNew((o) =>
            {
                try
                {
                    DoSplitOrderTicketsWithNoThread(o.ToString());
                }
                catch (Exception ex)
                {
                    Log4Log.Error("DoSplitOrderTickets-DpSplitOrderTicketsWithNoThread", ex);
                }
            }, schemeId);
            //ThreadPool.QueueUserWorkItem((o) =>
            //{

            //}, schemeId);

            //new Thread(() =>
            //{


            //}).Start();

            //return "拆分票成功";
        }
        public C_Sports_Order_Running AddRunningOrderAndOrderDetail(string schemeId, SchemeBettingCategory category,
            string gameCode, string gameType, string playType, bool stopAfterBonus,
            string issuseNumber, int amount, int betCount, int totalMatchCount, decimal totalMoney, DateTime stopTime,
            SchemeSource schemeSource, TogetherSchemeSecurity security, SchemeType schemeType, bool canChase, bool isVirtualOrder,
            string userId, string userAgent, DateTime betTime, ActivityType activityType, string attach, bool isAppend, decimal redBagMoney, ProgressStatus progressStatus, TicketStatus ticketStatus)
        {
            //if (DateTime.Now >= stopTime)
            if (betTime >= stopTime)
                throw new LogicException(string.Format("订单结束时间是{0}，订单不能投注。", stopTime.ToString("yyyy-MM-dd HH:mm")));
            var sportsManager = new Sports_Manager();
            if (new string[] { "JCZQ", "JCLQ" }.Contains(gameCode))
                issuseNumber = DateTime.Now.ToString("yyyy-MM-dd");

            DateTime? ticketTime = null;
            if (ticketStatus == TicketStatus.Ticketed)
                ticketTime = DateTime.Now;
            DateTime createtime = DateTime.Now;
            if (betTime != null && betTime.ToString() != "0001/1/1 0:00:00")
            {
                createtime = betTime;
            }
            else
            {
                betTime = DateTime.Now;
            }
            betTime = createtime;
            var order = new C_Sports_Order_Running
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                Amount = amount,
                BonusStatus = (int)BonusStatus.Waitting,
                CanChase = canChase,
                IsVirtualOrder = isVirtualOrder,
                IsPayRebate = false,
                RealPayRebateMoney = 0M,
                TotalPayRebateMoney = 0M,
                CreateTime = createtime,
                GameCode = gameCode,
                GameType = gameType,
                IssuseNumber = issuseNumber,
                PlayType = playType,
                PreTaxBonusMoney = 0M,
                ProgressStatus = (int)progressStatus,
                SchemeId = schemeId,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                TicketId = string.Empty,
                TicketLog = string.Empty,
                TicketStatus = (int)ticketStatus,
                TotalMatchCount = totalMatchCount,
                TotalMoney = totalMoney,
                SuccessMoney = totalMoney,
                UserId = userId,
                StopTime = stopTime,
                SchemeSource = (int)schemeSource,
                BetCount = betCount,
                BonusCount = 0,
                HitMatchCount = 0,
                RightCount = 0,
                Error1Count = 0,
                Error2Count = 0,
                MaxBonusMoney = 0,
                MinBonusMoney = 0,
                Security = (int)security,
                TicketGateway = string.Empty,
                TicketProgress = 1M,
                BetTime = betTime,
                ExtensionOne = string.Format("{0}{1}", "3X1_", (int)activityType),
                Attach = string.IsNullOrEmpty(attach) ? string.Empty : attach.ToUpper(),
                QueryTicketStopTime = stopTime.AddMinutes(1).ToString("yyyyMMddHHmm"),
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                IsSplitTickets = ticketStatus == TicketStatus.Ticketed,
                TicketTime = ticketTime,
            };
            // sportsManager.AddSports_Order_Running(order);
            DB.GetDal<C_Sports_Order_Running>().Add(order);
            //订单总表信息
            var orderDetail = new C_OrderDetail
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                BonusStatus = (int)BonusStatus.Waitting,
                ComplateTime = null,
                CreateTime = createtime,
                CurrentBettingMoney = ticketStatus == TicketStatus.Ticketed ? totalMoney : 0M,
                GameCode = gameCode,
                GameType = gameType,
                GameTypeName = BettingHelper.FormatGameType(gameCode, gameType),
                PreTaxBonusMoney = 0M,
                ProgressStatus = (int)progressStatus,
                SchemeId = schemeId,
                SchemeSource = (int)schemeSource,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                StartIssuseNumber = issuseNumber,
                StopAfterBonus = stopAfterBonus,
                TicketStatus = (int)ticketStatus,
                TotalIssuseCount = 1,
                TotalMoney = totalMoney,
                UserId = userId,
                CurrentIssuseNumber = issuseNumber,
                IsVirtualOrder = isVirtualOrder,
                PlayType = playType,
                Amount = amount,
                AddMoney = 0M,
                BetTime = betTime,
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                BonusAwardsMoney = 0M,
                RealPayRebateMoney = 0M,
                RedBagAwardsMoney = 0M,
                TotalPayRebateMoney = 0M,
                TicketTime = ticketTime,
            };
            //   new SchemeManager().AddOrderDetail(orderDetail);
            DB.GetDal<C_OrderDetail>().Add(orderDetail);
            return order;
        }


        public Order_Together AddRunningOrderAndOrderDetail_BackList(string schemeId, SchemeBettingCategory category,
         string gameCode, string gameType, string playType, bool stopAfterBonus,
         string issuseNumber, int amount, int betCount, int totalMatchCount, decimal totalMoney, DateTime stopTime,
         SchemeSource schemeSource, TogetherSchemeSecurity security, SchemeType schemeType, bool canChase, bool isVirtualOrder,
         string userId, string userAgent, DateTime betTime, ActivityType activityType, string attach, bool isAppend, decimal redBagMoney, ProgressStatus progressStatus, TicketStatus ticketStatus)
        {
            //if (DateTime.Now >= stopTime)
            if (betTime >= stopTime)
                throw new LogicException(string.Format("订单结束时间是{0}，订单不能投注。", stopTime.ToString("yyyy-MM-dd HH:mm")));
            var sportsManager = new Sports_Manager();
            if (new string[] { "JCZQ", "JCLQ" }.Contains(gameCode))
                issuseNumber = DateTime.Now.ToString("yyyy-MM-dd");

            DateTime? ticketTime = null;
            if (ticketStatus == TicketStatus.Ticketed)
                ticketTime = DateTime.Now;
            DateTime createtime = DateTime.Now;
            if (betTime != null && betTime.ToString() != "0001/1/1 0:00:00")
            {
                createtime = betTime;
            }
            else
            {
                betTime = DateTime.Now;
            }
            betTime = createtime;
            var order = new C_Sports_Order_Running
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                Amount = amount,
                BonusStatus = (int)BonusStatus.Waitting,
                CanChase = canChase,
                IsVirtualOrder = isVirtualOrder,
                IsPayRebate = false,
                RealPayRebateMoney = 0M,
                TotalPayRebateMoney = 0M,
                CreateTime = createtime,
                GameCode = gameCode,
                GameType = gameType,
                IssuseNumber = issuseNumber,
                PlayType = playType,
                PreTaxBonusMoney = 0M,
                ProgressStatus = (int)progressStatus,
                SchemeId = schemeId,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                TicketId = string.Empty,
                TicketLog = string.Empty,
                TicketStatus = (int)ticketStatus,
                TotalMatchCount = totalMatchCount,
                TotalMoney = totalMoney,
                SuccessMoney = totalMoney,
                UserId = userId,
                StopTime = stopTime,
                SchemeSource = (int)schemeSource,
                BetCount = betCount,
                BonusCount = 0,
                HitMatchCount = 0,
                RightCount = 0,
                Error1Count = 0,
                Error2Count = 0,
                MaxBonusMoney = 0,
                MinBonusMoney = 0,
                Security = (int)security,
                TicketGateway = string.Empty,
                TicketProgress = 1M,
                BetTime = betTime,
                ExtensionOne = string.Format("{0}{1}", "3X1_", (int)activityType),
                Attach = string.IsNullOrEmpty(attach) ? string.Empty : attach.ToUpper(),
                QueryTicketStopTime = stopTime.AddMinutes(1).ToString("yyyyMMddHHmm"),
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                IsSplitTickets = ticketStatus == TicketStatus.Ticketed,
                TicketTime = ticketTime,
            };
            // sportsManager.AddSports_Order_Running(order);
            //  DB.GetDal<C_Sports_Order_Running>().Add(order);
            //订单总表信息
            var orderDetail = new C_OrderDetail
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                BonusStatus = (int)BonusStatus.Waitting,
                ComplateTime = null,
                CreateTime = createtime,
                CurrentBettingMoney = ticketStatus == TicketStatus.Ticketed ? totalMoney : 0M,
                GameCode = gameCode,
                GameType = gameType,
                GameTypeName = BettingHelper.FormatGameType(gameCode, gameType),
                PreTaxBonusMoney = 0M,
                ProgressStatus = (int)progressStatus,
                SchemeId = schemeId,
                SchemeSource = (int)schemeSource,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                StartIssuseNumber = issuseNumber,
                StopAfterBonus = stopAfterBonus,
                TicketStatus = (int)ticketStatus,
                TotalIssuseCount = 1,
                TotalMoney = totalMoney,
                UserId = userId,
                CurrentIssuseNumber = issuseNumber,
                IsVirtualOrder = isVirtualOrder,
                PlayType = playType,
                Amount = amount,
                AddMoney = 0M,
                BetTime = betTime,
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                BonusAwardsMoney = 0M,
                RealPayRebateMoney = 0M,
                RedBagAwardsMoney = 0M,
                TotalPayRebateMoney = 0M,
                TicketTime = ticketTime,
            };
            //   new SchemeManager().AddOrderDetail(orderDetail);
            //  DB.GetDal<C_OrderDetail>().Add(orderDetail);
            Order_Together OT = new Order_Together();
            OT.Order_Running = order;
            OT.OrderDetail = orderDetail;
            return OT;
        }


        #region 创建合买，参与合买
        /// <summary>
        /// 创建合买
        /// </summary>
        public string CreateSportsTogether(Sports_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop
            , out bool canChase, out DateTime stopTime, ref Sports_BetingInfo schemeInfo)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            var anteCodeList = new List<C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new LogicException("方案拆分不正确");
            if (info.Subscription < 1)
                throw new LogicException("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new LogicException("发起者认购份数和保底份数不能超过总份数");

            var schemeId = string.IsNullOrEmpty(info.SchemeId) ? BettingHelper.GetTogetherBettingSchemeId() : info.SchemeId;
            var sportsManager = new Sports_Manager();
            stopTime = CheckGeneralBettingMatch(sportsManager, info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            var betCount = 0;

            #region 计算注数
            if (info.GameCode == "BJDC" || info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
            {
                betCount = CheckBettingOrderMoney(info.AnteCodeList, info.GameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
                //检查投注号码
                foreach (var item in info.AnteCodeList)
                {
                    var msg = string.Empty;
                    var zhu = AnalyzerFactory.GetSportAnteCodeChecker(info.GameCode, item.GameType).CheckAntecodeNumber(item, out msg);
                    if (!string.IsNullOrEmpty(msg))
                        throw new LogicException(msg);
                }
            }
            else
            {
                var codeMoney = 0M;
                foreach (var item in info.AnteCodeList)
                {
                    try
                    {
                        var type = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper();
                        var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, type).AnalyzeAnteCode(item.AnteCode);
                        betCount += zhu;
                        codeMoney += zhu * info.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
                    }
                    catch (Exception ex)
                    {
                        throw new LogicException("投注号码出错 - " + ex.Message);
                    }
                }

                if (codeMoney != info.TotalMoney)
                    throw new LogicException("投注期号总金额与方案总金额不匹配");
            }
            #endregion

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                //var userManager = new UserBalanceManager();
                //var user = userManager.QueryUserRegister(userId);
                var issuseNumberOrTime = (info.GameCode == "JCZQ" || info.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.IssuseNumber;
                //添加合买信息
                var main = AddTogetherInfo(info, schemeId, info.TotalCount, info.TotalMoney, info.GameCode, info.GameType, info.PlayType, info.SchemeSource, info.Security, info.TotalMatchCount,
                    stopTime, true, schemeDeduct, user.UserId, user.AgentId, balancePassword,
                    sysGuarantees, isTop, SchemeBettingCategory.GeneralBetting, issuseNumberOrTime, out canChase);
                //schemeId = main.SchemeId;

                //添加订单信息
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber,
                    info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.TogetherBetting, canChase, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                //添加投注号码信息
                foreach (var item in info.AnteCodeList)
                {
                    var entity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode.ToUpper(),
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = string.IsNullOrEmpty(info.PlayType) ? string.Empty : info.PlayType.ToUpper(),
                        Odds = string.Empty,
                    };
                    sportsManager.AddSports_AnteCode(entity);
                    anteCodeList.Add(entity);
                }

                schemeInfo.GameCode = info.GameCode;
                schemeInfo.GameType = info.GameType;
                schemeInfo.IssuseNumber = info.IssuseNumber;
                schemeInfo.TotalMoney = info.TotalMoney;
                schemeInfo.SoldCount = main.SoldCount;
                schemeInfo.SchemeProgress = (TogetherSchemeProgress)main.ProgressStatus;
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }


            //}


            #region 拆票

            if (canChase)
            {
                if (RedisHelperEx.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrder
                    {
                        AnteCodeList = anteCodeList,
                        RunningOrder = runningOrder,
                    };
                    RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisWaitOrder);
                    //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
                }
                else
                {
                    DoSplitOrderTickets(schemeId);
                }
            }

            #endregion

            #region 发送站内消息：手机短信或站内信

            var _userManager = new UserBalanceManager();
            var _user = _userManager.QueryUserRegister(userId);
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "[UserName]", _user.DisplayName));
            pList.Add(string.Format("{0}={1}", "[SchemeId]", schemeId));
            pList.Add(string.Format("{0}={1}", "[SchemeTotalMoney]", info.TotalMoney));
            //发送短信
            new SiteMessageControllBusiness().DoSendSiteMessage(_user.UserId, "", "ON_User_Create_Together", pList.ToArray());

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }

        /// <summary>
        /// 参与合买
        /// </summary>
        public bool JoinSportsTogether(string schemeId, int buyCount, string userId, string joinPwd, string balancePassword
            , ref Sports_BetingInfo schemeInfo)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            C_Sports_Order_Running runningOrder = null;
            //开启事务
            var canChase = false;

            var sportsManager = new Sports_Manager();
            var main = sportsManager.QuerySports_Together(schemeId);
            if (main == null) throw new LogicException("合买订单不存在");
            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);
            if (orderDetail == null)
                throw new LogicException(string.Format("查不到{0}的orderDetail 信息", schemeId));
            else if (orderDetail.IsVirtualOrder)
                throw new LogicException("当前订单还未付款不能参与合买");
            BusinessHelper.CheckDisableGame(orderDetail.GameCode, orderDetail.GameType);
            if (DateTime.Now >= main.StopTime)
                throw new LogicException(string.Format("合买结束时间是{0}，现在不能参与合买。", main.StopTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if (main.ProgressStatus != (int)TogetherSchemeProgress.SalesIn && main.ProgressStatus != (int)TogetherSchemeProgress.Standard) throw new LogicException("合买已完成，不能参与");
            if (!string.IsNullOrEmpty(main.JoinPwd) && (string.IsNullOrEmpty(joinPwd) || Encipherment.MD5(joinPwd) != main.JoinPwd))
                throw new LogicException("参与密码不正确");
            var surplusCount = main.TotalCount - main.SoldCount;
            if (surplusCount < buyCount)
                throw new LogicException(string.Format("方案剩余份数不足{0}份", buyCount));

            var buyMoney = main.Price * buyCount;
            if (buyMoney < 1)
                throw new LogicException("参与金额最少为1元");

            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                main.SoldCount += buyCount;
                main.JoinUserCount += sportsManager.IsUserJoinTogether(schemeId, userId) ? 0 : 1;
                main.Progress = (decimal)main.SoldCount / main.TotalCount;
                if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                    main.ProgressStatus = (int)TogetherSchemeProgress.Standard;
                if (main.SoldCount == main.TotalCount)
                    main.ProgressStatus = (int)TogetherSchemeProgress.Finish;
                //不需要系统保底
                //if (main.SoldCount + main.Guarantees >= main.TotalCount)
                //    main.SystemGuarantees = 0;

                var joinItem = new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    TotalMoney = buyMoney,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    Price = main.Price,
                    CreateTime = DateTime.Now,
                    JoinType = (int)TogetherJoinType.Join,
                    SchemeId = main.SchemeId,
                    JoinUserId = userId,
                    JoinSucess = true,
                    JoinLog = "参与合买",
                    PreTaxBonusMoney = 0M,
                };
                sportsManager.AddSports_TogetherJoin(joinItem);
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, joinItem.Id), buyMoney,
                    string.Format("参与订单{0}合买，支出{1:N2}元", schemeId, buyMoney), "Bet", balancePassword);

                SetTogetherIsTop(main);

                sportsManager.UpdateSports_Together(main);

                //计算用户真实保底
                surplusCount = main.TotalCount - main.SoldCount;
                var joinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                if (joinEntity != null)
                {
                    if (surplusCount >= joinEntity.BuyCount)
                    {
                        //剩余份数 大于 用户保底数
                    }
                    if (surplusCount < joinEntity.BuyCount)
                    {
                        joinEntity.RealBuyCount = surplusCount;
                        sportsManager.UpdateSports_TogetherJoin(joinEntity);
                    }
                    //剩余份数 
                    surplusCount -= joinEntity.RealBuyCount;
                    if (surplusCount < 0)
                        surplusCount = 0;
                }
                if (sysJoinEntity != null)
                {
                    if (surplusCount < main.SystemGuarantees)
                    {
                        sysJoinEntity.RealBuyCount = surplusCount;
                        sportsManager.UpdateSports_TogetherJoin(sysJoinEntity);
                    }
                }

                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new LogicException("未查询到订单信息");
                runningOrder = order;
                if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount && !order.CanChase)
                {
                    canChase = true;
                    order.CanChase = true;
                    order.IsVirtualOrder = false;
                    sportsManager.UpdateSports_Order_Running(order);

                    orderDetail.IsVirtualOrder = false;
                    manager.UpdateOrderDetail(orderDetail);
                }

                schemeInfo.GameCode = order.GameCode;
                schemeInfo.GameType = order.GameType;
                schemeInfo.IssuseNumber = order.IssuseNumber;
                schemeInfo.TotalMoney = order.TotalMoney;
                schemeInfo.SchemeProgress = (TogetherSchemeProgress)main.ProgressStatus;

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            #region 拆票

            if (canChase)
            {
                if (RedisHelperEx.EnableRedis)
                {
                    if (runningOrder.SchemeBettingCategory == (int)SchemeBettingCategory.SingleBetting || runningOrder.SchemeBettingCategory == (int)SchemeBettingCategory.XianFaQiHSC)
                    {
                        //单式上传方式投注的订单
                        var redisWaitOrder_Single = new RedisWaitTicketOrderSingle
                        {
                            RunningOrder = runningOrder,
                            AnteCode = new Sports_Manager().QuerySingleScheme_AnteCode(schemeId)
                        };
                        RedisOrderBusiness.AddOrderToRedis(runningOrder.GameCode, redisWaitOrder_Single);
                        //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder_Single);
                    }
                    else
                    {
                        //普通投注方式的订单
                        var redisWaitOrder = new RedisWaitTicketOrder
                        {
                            RunningOrder = runningOrder,
                            AnteCodeList = new Sports_Manager().QuerySportsAnteCodeBySchemeId(schemeId)
                        };
                        RedisOrderBusiness.AddOrderToRedis(runningOrder.GameCode, redisWaitOrder);
                        //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
                    }
                }
                else
                {
                    DoSplitOrderTickets(schemeId);
                }
            }

            #endregion

            #region 发送站内消息：手机短信或站内信

            var _userManager = new UserBalanceManager();
            var _user = _userManager.QueryUserRegister(userId);
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "[UserName]", _user.DisplayName));
            pList.Add(string.Format("{0}={1}", "[SchemeId]", schemeId));
            pList.Add(string.Format("{0}={1}", "[JoinMoney]", buyMoney));
            //发送短信
            new SiteMessageControllBusiness().DoSendSiteMessage(_user.UserId, "", "ON_User_Join_Together", pList.ToArray());

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return canChase;
        }
        #endregion


        #region 足彩追号
        /// <summary>
        /// 足彩追号
        /// </summary>
        public bool SportsChase(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new LogicException(string.Format("查不到方案{0}的Order_Running信息", schemeId));
            if (order.TicketStatus != (int)TicketStatus.Waitting)
                throw new LogicException(string.Format("订单{0}出票状态应是TicketStatus.Waitting,实际是{1}", schemeId, (TicketStatus)order.TicketStatus));

            #region 发送站内消息：手机短信或站内信

            var userManager = new UserBalanceManager();
            var user = userManager.QueryUserRegister(order.UserId);
            //当订单为追号订单时
            if (order.SchemeType == (int)SchemeType.ChaseBetting)
            {
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[SchemeId]", order.SchemeId));
                pList.Add(string.Format("{0}={1}", "[SchemeTotalMoney]", order.TotalMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_ChaseScheme_Chased", pList.ToArray());
            }

            #endregion

            var ticketId = string.Empty;
            var ticketLog = string.Empty;

            var ticketStatus = (int)TicketStatus.Waitting;
            var progressStatus = (int)ProgressStatus.Waitting;

            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();
            try
            {
                #region 修改订单相关信息

                order.TicketStatus = ticketStatus;
                order.ProgressStatus = progressStatus;
                order.TicketId = ticketId;
                order.TicketProgress = 0;
                order.TicketLog = ticketLog;
                order.BetTime = DateTime.Now;
                sportsManager.UpdateSports_Order_Running(order);
                orderDetail.ProgressStatus = progressStatus;
                orderDetail.CurrentBettingMoney = order.IsVirtualOrder ? 0M : (ticketStatus == (int)TicketStatus.Ticketing ? order.TotalMoney : 0M);
                orderDetail.CurrentIssuseNumber = order.IssuseNumber;
                orderDetail.TicketStatus = ticketStatus;
                orderDetail.BetTime = DateTime.Now;
                manager.UpdateOrderDetail(orderDetail);
                #endregion

                #region 请求出票失败后，移动订单数据
                if (ticketStatus == (int)TicketStatus.Error)
                {
                    //移动订单数据
                    OrderFailToEnd(schemeId, sportsManager, order);
                }
                #endregion

                #region 请求出票失败后，退还投注资金

                if (!order.IsVirtualOrder && ticketStatus == (int)TicketStatus.Error)
                {
                    // 返还资金
                    if (order.SchemeType == (int)SchemeType.GeneralBetting)
                    {
                        if (order.TotalMoney > 0)
                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, schemeId, order.TotalMoney
                                , string.Format("{0} 出票失败，返还资金￥{1:N2}。 ", BettingHelper.FormatGameCode(order.GameCode), order.TotalMoney));
                    }
                    if (order.SchemeType == (int)SchemeType.ChaseBetting)
                    {
                        var chaseOrder = sportsManager.QueryLotteryScheme(order.SchemeId);
                        if (chaseOrder != null)
                        {
                            if (order.TotalMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, chaseOrder.KeyLine, order.TotalMoney
                                , string.Format("订单{0} 出票失败，返还资金￥{1:N2}。 ", order.SchemeId, order.TotalMoney));
                        }
                    }
                    if (order.SchemeType == (int)SchemeType.TogetherBetting)
                    {
                        //失败
                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            item.JoinSucess = false;
                            item.JoinLog += "出票失败";
                            sportsManager.UpdateSports_TogetherJoin(item);

                            if (item.JoinType == (int)TogetherJoinType.SystemGuarantees)
                                continue;

                            var t = string.Empty;
                            var realBuyCount = item.RealBuyCount;
                            switch (item.JoinType)
                            {
                                case (int)TogetherJoinType.Subscription:
                                    t = "认购";
                                    break;
                                case (int)TogetherJoinType.FollowerJoin:
                                    t = "订制跟单";
                                    break;
                                case (int)TogetherJoinType.Join:
                                    t = "参与";
                                    break;
                                case (int)TogetherJoinType.Guarantees:
                                    realBuyCount = item.BuyCount;//为解决用户发起合买后有自动跟单情况，然后投注时直接失败，这时会出现退还保底金额少
                                    t = "保底";
                                    break;
                            }
                            //var joinMoney = item.Price * item.RealBuyCount;
                            var joinMoney = item.Price * realBuyCount;
                            //退钱
                            if (joinMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TogetherFail, item.JoinUserId,
                                  string.Format("{0}_{1}", schemeId, item.Id), joinMoney, string.Format("合买失败，返还{0}资金{1:N2}元", t, joinMoney));
                        }
                    }
                }

                #endregion

                #region 发送站内消息：手机短信或站内信

                if (ticketStatus == (int)TicketStatus.Error)
                {
                    var pList = new List<string>();
                    pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                    pList.Add(string.Format("{0}={1}", "[SchemeId]", order.SchemeId));
                    pList.Add(string.Format("{0}={1}", "[SchemeTotalMoney]", order.TotalMoney));
                    pList.Add(string.Format("{0}={1}", "[SchemeErrorMoney]", order.TotalMoney));
                    //发送短信
                    new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Scheme_Ticket_Error", pList.ToArray());
                }
                #endregion
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}
            if (ticketStatus == (int)TicketStatus.Error)
                return false;
            return true;
        }
        #endregion

        /// <summary>
        /// 订单失败，处理订单数据
        /// </summary>
        private void OrderFailToEnd(string schemeId, Sports_Manager sportsManager, C_Sports_Order_Running order)
        {
            var complateOrder = new C_Sports_Order_Complate
            {
                SchemeId = order.SchemeId,
                GameCode = order.GameCode,
                GameType = order.GameType,
                PlayType = order.PlayType,
                IssuseNumber = order.IssuseNumber,
                TotalMoney = order.TotalMoney,
                Amount = order.Amount,
                TotalMatchCount = order.TotalMatchCount,
                TicketStatus = (int)TicketStatus.Error,// order.TicketStatus,
                BonusStatus = order.BonusStatus,
                AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                CanChase = order.CanChase,
                IsVirtualOrder = order.IsVirtualOrder,
                CreateTime = order.CreateTime,
                PreTaxBonusMoney = order.PreTaxBonusMoney,
                ProgressStatus = (int)ProgressStatus.Aborted, // order.ProgressStatus,
                SchemeType = order.SchemeType,
                TicketId = order.TicketId,
                TicketLog = order.TicketLog,
                UserId = order.UserId,
                AgentId = order.AgentId,
                SchemeSource = order.SchemeSource,
                SchemeBettingCategory = order.SchemeBettingCategory,
                StopTime = order.StopTime,
                ComplateDate = DateTime.Now.ToString("yyyyMMdd"),
                ComplateDateTime = DateTime.Now,
                BetCount = order.BetCount,
                IsPrizeMoney = false,
                BonusCount = 0,
                HitMatchCount = 0,
                RightCount = 0,
                Error1Count = 0,
                Error2Count = 0,
                AddMoney = 0M,
                DistributionWay = (int)AddMoneyDistributionWay.Average,
                AddMoneyDescription = string.Empty,
                BonusCountDescription = string.Empty,
                BonusCountDisplayName = string.Empty,
                Security = order.Security,
                BetTime = order.BetTime,
                SuccessMoney = order.SuccessMoney,
                TicketGateway = order.TicketGateway,
                TicketProgress = order.TicketProgress,
                ExtensionOne = order.ExtensionOne,
                Attach = order.Attach,
                IsAppend = order.IsAppend,
                TicketTime = order.TicketTime.Value,
                RedBagMoney = order.RedBagMoney,
                IsPayRebate = order.IsPayRebate,
                MaxBonusMoney = order.MaxBonusMoney,
                MinBonusMoney = order.MinBonusMoney,
                //QueryTicketStopTime = order.QueryTicketStopTime,
                RealPayRebateMoney = order.RealPayRebateMoney,
                TotalPayRebateMoney = order.TotalPayRebateMoney,
                IsSplitTickets = order.IsSplitTickets,
            };
            sportsManager.AddSports_Order_Complate(complateOrder);
            sportsManager.DeleteSports_Order_Running(order);

            //if (order.SchemeType == SchemeType.ChaseBetting)
            //    MoveChaseBrotherOrder(schemeId);
            if (order.SchemeType == (int)SchemeType.TogetherBetting)
            {
                var together = sportsManager.QuerySports_Together(schemeId);
                if (together != null)
                {
                    together.ProgressStatus = (int)TogetherSchemeProgress.Cancel;

                    //处理退保
                    if (!together.IsPayBackGuarantees && together.ProgressStatus == (int)TogetherSchemeProgress.Finish)
                    {
                        var joinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                        if (joinEntity != null)
                        {
                            var tooMuchMoney = (together.Guarantees - joinEntity.RealBuyCount) * together.Price;
                            //返还保底资金
                            if (tooMuchMoney > 0M)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_ReturnGuarantees, joinEntity.JoinUserId
                                                  , string.Format("{0}_{1}", schemeId, joinEntity.Id), tooMuchMoney, string.Format("返还保底资金{0:N2}元", tooMuchMoney));
                        }
                        together.IsPayBackGuarantees = true;
                    }
                    sportsManager.UpdateSports_Together(together);
                }
            }
        }

        #region 处理合买订单
        private C_Sports_Together AddTogetherInfo(TogetherSchemeBase info, string schemeId, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
            SchemeSource schemeSource, TogetherSchemeSecurity security, int totalMatchCount, DateTime stopTime, bool isUploadAnteCode,
            decimal schemeDeduct, string userId, string userAgent, string balancePassword, int sysGuarantees, bool isTop, SchemeBettingCategory category, string issuseNumber, out bool canChase)
        {
            canChase = false;
            stopTime = stopTime.AddMinutes(-5);

            //if (DateTime.Now >= stopTime)
            //    throw new LogicException(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                gameType = string.Empty;

            //var schemeId = BusinessHelper.GetTogetherBettingSchemeId();
            var sportsManager = new Sports_Manager();

            //存入临时合买表
            sportsManager.AddTemp_Together(new C_Temp_Together
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                SchemeId = schemeId,
                StopTime = stopTime.ToString("yyyyMMddHHmm"),
            });

            //合买信息
            var main = new C_Sports_Together();
            main.BonusDeduct = info.BonusDeduct;
            main.SchemeDeduct = schemeDeduct;
            main.CreateTime = DateTime.Now;
            main.CreateUserId = userId;
            main.SchemeSource = (int)schemeSource;
            main.SchemeBettingCategory = (int)category;
            main.AgentId = userAgent;
            main.Description = info.Description;
            main.Guarantees = info.Guarantees;
            main.IsTop = isTop;
            main.IsUploadAnteCode = isUploadAnteCode;
            main.JoinPwd = string.IsNullOrEmpty(info.JoinPwd) ? string.Empty : Encipherment.MD5(info.JoinPwd);
            main.JoinUserCount = 1;
            main.Price = info.Price;
            main.SoldCount = 0;
            main.SchemeId = schemeId;
            main.Security = (int)security;
            main.Subscription = info.Subscription;
            main.SystemGuarantees = info.TotalCount * sysGuarantees / 100;
            main.Title = info.Title;
            main.TotalCount = totalCount;
            main.TotalMoney = totalMoney;
            main.TotalMatchCount = totalMatchCount;
            main.StopTime = stopTime;
            main.GameCode = gameCode;
            main.GameType = gameType;
            main.PlayType = playType;
            main.CreateTimeOrIssuseNumber = issuseNumber;

            #region 处理认购

            var subItem = new C_Sports_TogetherJoin
            {
                AfterTaxBonusMoney = 0M,
                BuyCount = info.Subscription,
                RealBuyCount = info.Subscription,
                CreateTime = DateTime.Now,
                JoinType = (int)TogetherJoinType.Subscription,
                JoinUserId = userId,
                Price = info.Price,
                SchemeId = schemeId,
                TotalMoney = info.Subscription * info.Price,
                JoinSucess = true,
                JoinLog = "认购合买",
            };
            sportsManager.AddSports_TogetherJoin(subItem);

            BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, subItem.Id), info.Subscription * info.Price,
                 string.Format("发起合买认购{0:N2}元", info.Subscription * info.Price), "Bet", balancePassword);

            main.SoldCount += info.Subscription;

            #endregion

            #region 处理自动跟单

            var surplusCount = main.TotalCount - main.SoldCount;
            var surplusPercent = (decimal)surplusCount / main.TotalCount * 100;
            var balanceManager = new UserBalanceManager();
            foreach (var item in sportsManager.QuerySportsTogetherFollowerList(userId, gameCode, gameType))
            {
                if (surplusCount == 0) continue;
                if (!item.IsEnable) continue;
                //跟单人 本彩种是否还能继续跟单 其中 -1 不无限跟单
                if (item.SchemeCount != -1)
                {
                    //为0后表示已跟单数完成，不再继续跟单
                    if (item.SchemeCount == 0 || item.SchemeCount < -1)
                        continue;
                }
                //跟单人 资金余额是否达到 订制的最小值
                var b = balanceManager.QueryUserBalance(item.FollowerUserId);
                if (b == null)
                    continue;
                if (b.GetTotalEnableMoney() <= item.StopFollowerMinBalance)
                    continue;

                //方案最小金额
                if (item.MinSchemeMoney != -1 && info.TotalMoney < item.MinSchemeMoney)
                    continue;
                //方案最大金额
                if (item.MaxSchemeMoney != -1 && info.TotalMoney > item.MaxSchemeMoney)
                    continue;

                // 当方案剩余份数/百分比不足时 是否跟单
                if (surplusCount < item.FollowerCount && !item.CancelWhenSurplusNotMatch)
                    continue;
                if (surplusPercent < item.FollowerPercent && !item.CancelWhenSurplusNotMatch)
                    continue;

                //计算真实应该买的份数
                var buyCount = (item.FollowerCount != -1) ? (surplusCount <= item.FollowerCount ? surplusCount : item.FollowerCount) :
                    (item.FollowerPercent == -1) ? 0 : (surplusPercent <= item.FollowerPercent ? surplusCount : (int)(item.FollowerPercent * main.TotalCount / 100));
                var realBuyMoney = buyCount * main.Price;
                if (realBuyMoney < 1)
                    continue;
                //用户资金余额 小于 实际购买总金额
                if (b.GetTotalEnableMoney() < realBuyMoney)
                    continue;

                //连续X个方案未中奖则停止跟单
                if (item.CancelNoBonusSchemeCount != -1 && item.NotBonusSchemeCount >= item.CancelNoBonusSchemeCount)
                    continue;

                //添加参与合买记录
                var joinItem = new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    CreateTime = DateTime.Now,
                    JoinType = (int)TogetherJoinType.FollowerJoin,
                    JoinUserId = item.FollowerUserId,
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = realBuyMoney,
                    JoinSucess = true,
                    JoinLog = "跟单参与合买",
                };
                sportsManager.AddSports_TogetherJoin(joinItem);

                //添加跟单记录
                sportsManager.AddTogetherFollowerRecord(new C_Together_FollowerRecord
                {
                    RuleId = item.Id,
                    RecordKey = string.Format("{0}_{1}_{2}_{3}", userId, item.FollowerUserId, gameCode, gameType),
                    BuyCount = buyCount,
                    BuyMoney = realBuyMoney,
                    CreaterUserId = main.CreateUserId,
                    CreateTime = DateTime.Now,
                    FollowerUserId = item.FollowerUserId,
                    GameCode = main.GameCode,
                    GameType = main.GameType,
                    Price = main.Price,
                    BonusMoney = 0,
                    SchemeId = schemeId,
                });
                //修改跟单规则记录
                item.TotalBetMoney += realBuyMoney;
                item.TotalBetOrderCount++;
                item.SchemeCount--;
                sportsManager.UpdateTogetherFollowerRule(item);

                try
                {
                    //扣钱
                    BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, joinItem.JoinUserId, string.Format("{0}_{1}", schemeId, joinItem.Id), realBuyMoney,
                    string.Format("跟单参与合买{0:N2}元", realBuyMoney), string.Empty, string.Empty);
                }
                catch (Exception)
                {
                    continue;
                }
                main.JoinUserCount++;
                main.SoldCount += buyCount;
                surplusCount = main.TotalCount - main.SoldCount;
                surplusPercent = (decimal)surplusCount / main.TotalCount * 100;

                //处理战绩
                var beed = sportsManager.QueryUserBeedings(main.CreateUserId, main.GameCode, main.GameType);
                if (beed != null)
                {
                    beed.BeFollowedTotalMoney += realBuyMoney;
                    sportsManager.UpdateUserBeedings(beed);
                }
            }

            #endregion

            main.Progress = (decimal)main.SoldCount / main.TotalCount;
            main.ProgressStatus = (int)TogetherSchemeProgress.SalesIn;

            if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                main.ProgressStatus = (int)TogetherSchemeProgress.Standard;
            if (main.SoldCount == main.TotalCount)
                main.ProgressStatus = (int)TogetherSchemeProgress.Finish;

            #region 发起人保底

            var guaranteeMoney = info.Guarantees * info.Price;
            //扣钱
            if (guaranteeMoney > 0)
            {
                var minGuarantees = main.TotalCount - main.SoldCount;
                var guaranteeItem = new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = info.Guarantees,
                    RealBuyCount = minGuarantees <= info.Guarantees ? minGuarantees : info.Guarantees,
                    CreateTime = DateTime.Now,
                    JoinType = (int)TogetherJoinType.Guarantees,
                    JoinUserId = userId,
                    Price = info.Price,
                    SchemeId = schemeId,
                    TotalMoney = guaranteeMoney,
                    JoinSucess = true,
                    JoinLog = "合买保底",
                };
                sportsManager.AddSports_TogetherJoin(guaranteeItem);

                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, guaranteeItem.Id), guaranteeMoney,
                string.Format("发起合买保底{0:N2}元", guaranteeMoney), "Bet", balancePassword);
            }

            #endregion

            //if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) / main.TotalCount >= 1M)
            //    canChase = true;
            //是否能出票
            if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount)
                canChase = true;

            //系统实际保底
            int systemGuarantees = main.TotalCount - main.SoldCount - main.Guarantees;
            if (systemGuarantees < 0)
                systemGuarantees = 0;

            if (systemGuarantees > 0)
            {
                var tempSystemGuarantees = systemGuarantees <= main.SystemGuarantees ? systemGuarantees : main.SystemGuarantees;
                //记录系统保底
                sportsManager.AddSports_TogetherJoin(new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = main.SystemGuarantees,
                    RealBuyCount = tempSystemGuarantees,
                    CreateTime = DateTime.Now,
                    JoinLog = "网站保底参与合买",
                    JoinSucess = false,
                    JoinType = (int)TogetherJoinType.SystemGuarantees,
                    JoinUserId = "xtadmin",
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = tempSystemGuarantees * main.Price,
                });
            }
            SetTogetherIsTop(main);
            sportsManager.AddSports_Together(main);
            return main;
        }

        //设置合买订单是否置顶
        private void SetTogetherIsTop(C_Sports_Together main)
        {
            //数字彩 金额大于200，进度大于20%，则设置为置顶
            if (new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(main.GameCode))
            {
                if (main.TotalMoney >= 200M && main.Progress >= 0.2M)
                    main.IsTop = true;
            }
            //竞彩 金额大于600，进度大于20%，则设置为置顶
            if (new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ" }.Contains(main.GameCode))
            {
                if (main.TotalMoney >= 600M && main.Progress >= 0.2M)
                    main.IsTop = true;
            }
        }
        #endregion


        #region 数字彩投注(单期或追号)
        /// <summary>
        /// 数字彩投注(单期或追号)
        /// </summary>
        public string LotteryBetting(LotteryBettingInfo info, string userId, string balancePassword, string place, decimal redBagMoney)
        {
            //时间记录变量
            long businessDT = 0, orderDT = 0, dataDT = 0, gametypesDT = 0, IssuseDT = 0, userDT = 0, oneDataDt = 0, oneDataDt1 = 0, oneDataDt2 = 0;
            var watch = new Stopwatch();
            watch.Start();
            //UserRegister 
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");
            info.UserId = userId;
            //查询用户用时
            userDT = watch.ElapsedMilliseconds;
            // 
            //Redis订单列表
            var redisOrderList = new RedisWaitTicketOrderList();

            var keyLine = info.SchemeId;
            var firstSchemeId = string.Empty;
            var firstMoney = 0M;

            #region 数据验证

            info.GameCode = info.GameCode.ToUpper();

            //排序
            info.IssuseNumberList.Sort((x, y) =>
            {
                return x.IssuseNumber.CompareTo(y.IssuseNumber);
            });

            var totalNumberZhu = 0;
            foreach (var item in info.AnteCodeList)
            {
                //if (item.GameType == "TR9" && info.SchemeSource == SchemeSource.Android && !string.IsNullOrEmpty(item.AnteCode))//app投注任九时，去掉后面的|
                //    item.AnteCode = item.AnteCode.Trim().TrimEnd('|');
                try
                {
                    if (new string[] { "JCSJBGJ", "JCYJ" }.Contains(info.GameCode))
                        CheckSJBMatch(info.GameCode, int.Parse(item.AnteCode));

                    var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, item.GameType).AnalyzeAnteCode(item.AnteCode);
                    totalNumberZhu += zhu;
                }
                catch (Exception ex)
                {
                    throw new LogicException("投注号码出错 - " + ex.Message);
                }
            }
            var codeMoney = 0M;
            info.IssuseNumberList.ForEach(item =>
            {
                if (item.Amount < 1)
                    throw new LogicException("倍数不能小于1");
                var currentMoney = item.Amount * totalNumberZhu * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
                if (currentMoney != item.IssuseTotalMoney)
                    throw new LogicException(string.Format("第{0}期投注金额应该是{1},实际是{2}", item.IssuseNumber, currentMoney, item.IssuseTotalMoney));
                codeMoney += currentMoney;
            });

            if (codeMoney != info.TotalMoney)
                throw new LogicException("投注期号总金额与方案总金额不匹配");


            string ctzqGameType = string.Empty;
            if (!string.IsNullOrEmpty(info.GameCode) && info.GameCode.ToUpper() == "CTZQ")
                ctzqGameType = info.AnteCodeList[0].GameType.ToUpper();
            //var currentIssuse = BusinessHelper.QueryCurentIssuse(info.GameCode, ctzqGameType);
            //if (currentIssuse == null)
            //    throw new LogicException("订单期号不存在，请联系客服");
            //if (info.IssuseNumberList.First().IssuseNumber.CompareTo(currentIssuse.IssuseNumber) < 0)
            //    throw new LogicException("投注订单期号已过期或未开售");

            #endregion
            //数据验证用时
            dataDT = watch.ElapsedMilliseconds;
            //


            var lotteryManager = new LotteryGameManager();
            var gameTypes = lotteryManager.QueryEnableGameTypes();
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            var gameInfo = BusinessHelper.QueryLotteryGame(info.GameCode);
            //   var schemeManager = new SchemeManager();
            // var sportsManager = new Sports_Manager();
            if (string.IsNullOrEmpty(keyLine))
                keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
            var orderIndex = 1;
            var totalBetMoney = 0M;
            var currentIssuseNumberList = new List<C_Game_Issuse>();
            //查询彩种用时
            gametypesDT = watch.ElapsedMilliseconds;
            //  
            //期号处理
            foreach (var issuse in info.IssuseNumberList)
            {
                var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                if (currentIssuseNumber == null)
                    throw new LogicException(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                if (!string.IsNullOrEmpty(currentIssuseNumber.WinNumber))
                    throw new LogicException("奖期已开出开奖号");
                if (info.CurrentBetTime > currentIssuseNumber.LocalStopTime)
                    throw new LogicException(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                currentIssuseNumberList.Add(currentIssuseNumber);
            }
            //期号处理用时
            IssuseDT = watch.ElapsedMilliseconds;
            //  
            try
            {
                var anteCodeList = new List<C_Sports_AnteCode>();
                IList<C_Sports_Order_Running> Order_Running_List = new List<C_Sports_Order_Running>();
                IList<C_OrderDetail> OrderDetail_List = new List<C_OrderDetail>();
                IList<C_Lottery_Scheme> Scheme_List = new List<C_Lottery_Scheme>();
                foreach (var issuse in info.IssuseNumberList)
                {
                    //var IsEnableLimitBetAmount = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableLimitBetAmount").ConfigValue);
                    //if (IsEnableLimitBetAmount)//开启限制用户单倍投注
                    //{
                    //    if (issuse.Amount == 1 && totalNumberZhu > 50)
                    //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                    //    else if (issuse.Amount > 0 && issuse.IssuseTotalMoney / issuse.Amount > 100)
                    //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                    //}
                    //var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                    //if (currentIssuseNumber == null)
                    //    throw new LogicException(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                    //if (!string.IsNullOrEmpty(currentIssuseNumber.WinNumber))
                    //    throw new LogicException("奖期已开出开奖号");
                    //if (info.CurrentBetTime > currentIssuseNumber.LocalStopTime)
                    //    throw new LogicException(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    var currentIssuseNumber = currentIssuseNumberList.FirstOrDefault(p => p.IssuseNumber == issuse.IssuseNumber);
                    var schemeId = string.Empty;
                    if (info.IssuseNumberList.Count > 1)
                    {
                        schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(keyLine))
                            schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);
                        else
                            schemeId = keyLine;
                    }


                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        var codeEntity = new C_Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = (int)BonusStatus.Waitting,
                            CreateTime = info.CurrentBetTime,// DateTime.Now,
                            GameCode = info.GameCode,
                            GameType = item.GameType.ToUpper(),
                            IsDan = item.IsDan,
                            IssuseNumber = issuse.IssuseNumber,
                            MatchId = string.Empty,
                            Odds = string.Empty,
                            PlayType = string.Empty,
                            SchemeId = schemeId,
                        };
                        anteCodeList.Add(codeEntity);
                        //sportsManager.AddSports_AnteCode(codeEntity);
                        //var gameType = lotteryManager.QueryGameType(info.GameCode, item.GameType);
                        var gameType = gameTypes.FirstOrDefault(a => a.Game.GameCode == info.GameCode && a.GameType == item.GameType.ToUpper());
                        if (gameType != null && !gameTypeList.Contains(gameType))
                        {
                            gameTypeList.Add(gameType);
                        }
                    }






                    //kason测试 赋值
                    info.SchemeId = schemeId;
                    var currentIssuseMoney = totalNumberZhu * issuse.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
                    if (string.IsNullOrEmpty(firstSchemeId))
                    {
                        firstSchemeId = schemeId;
                    }
                    if (firstMoney == 0M)
                    {
                        firstMoney = currentIssuseMoney;
                    }
                    if (string.IsNullOrEmpty(keyLine))
                    {
                        keyLine = schemeId;
                    }
                    else
                    {
                        Scheme_List.Add(new C_Lottery_Scheme
                        {
                            OrderIndex = orderIndex,
                            KeyLine = keyLine,
                            SchemeId = schemeId,
                            CreateTime = DateTime.Now,
                            IsComplate = false,
                            IssuseNumber = issuse.IssuseNumber,
                        });
                    }
                    var canTicket = BettingHelper.CanRequestBet(info.GameCode);
                    var entity = AddRunningOrderAndOrderDetail_BackList(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                          string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                          info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting, orderIndex == 1, false, user.UserId, user.AgentId,
                          orderIndex == 1 ? info.CurrentBetTime : currentIssuseNumber.StartTime, info.ActivityType, "", info.IsAppend, redBagMoney,
                          (orderIndex == 1 ? (canTicket ? ProgressStatus.Running : ProgressStatus.Waitting) : ProgressStatus.Waitting),
                          (orderIndex == 1 ? (canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting) : TicketStatus.Waitting));
                    totalBetMoney += currentIssuseMoney;

                    //启用了Redis
                    if (RedisHelperEx.EnableRedis)
                    {
                        var runningOrder = new RedisWaitTicketOrder
                        {
                            AnteCodeList = anteCodeList,
                            RunningOrder = entity.Order_Running,
                            KeyLine = keyLine,
                            StopAfterBonus = info.StopAfterBonus,
                            SchemeType = info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting
                        };
                        //追号方式 存入Redis订单列表
                        redisOrderList.OrderList.Add(runningOrder);
                    }

                    Order_Running_List.Add(entity.Order_Running);
                    OrderDetail_List.Add(entity.OrderDetail);
                    orderIndex++;
                }

                //订单构建用时
                orderDT = watch.ElapsedMilliseconds;
                //   
                DB.Begin();
                try
                {
                    DB.GetDal<C_Sports_AnteCode>().BulkAdd(anteCodeList);

                    DB.GetDal<C_Lottery_Scheme>().BulkAdd(Scheme_List);
                    //kason 批量录入录入订单信息
                    DB.GetDal<C_OrderDetail>().BulkAdd(OrderDetail_List);

                    oneDataDt = watch.ElapsedMilliseconds;
                    //
                    DB.GetDal<C_Sports_Order_Running>().BulkAdd(Order_Running_List);
                    oneDataDt1 = watch.ElapsedMilliseconds;
                    //

                    if (info.IssuseNumberList.Count > 1)
                    {
                        #region 发送站内消息：手机短信或站内信

                        var pList = new List<string>();
                        pList.Add(string.Format("{0}={1}", "[Chase_Id]", keyLine));
                        pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                        pList.Add(string.Format("{0}={1}", "[IssuseCount]", info.IssuseNumberList.Count));
                        pList.Add(string.Format("{0}={1}", "[SchemeTotalMoney]", totalBetMoney));
                        //发送短信
                        new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Create_ChaseScheme", pList.ToArray());

                        #endregion
                    }
                    oneDataDt2 = watch.ElapsedMilliseconds;
                    #region 支付

                    //摇钱树订单，不扣用户的钱，扣代理商余额
                    if (info.SchemeSource != SchemeSource.YQS
                        && info.SchemeSource != SchemeSource.YQS_Advertising
                        && info.SchemeSource != SchemeSource.NS_Bet
                        && info.SchemeSource != SchemeSource.YQS_Bet
                        && info.SchemeSource != SchemeSource.Publisher_0321
                        && info.SchemeSource != SchemeSource.WX_GiveLottery
                        && info.SchemeSource != SchemeSource.Web_GiveLottery
                        && info.SchemeSource != SchemeSource.LuckyDraw)
                    {
                        // 消费资金
                        //BusinessHelper.Payout_2End(BusinessHelper.FundCategory_Betting, schemeId, schemeId, true, "Bet", balancePassword, userId, AccountType.Common, currentIssuseMoney
                        //    , string.Format("{0}第{1}期投注", gameInfo.DisplayName, issuse.IssuseNumber));
                        if (info.IssuseNumberList.Count == 1)
                        {
                            //普通投注
                            var msg = string.Format("{0}第{1}期投注", gameInfo.DisplayName, info.IssuseNumberList[0].IssuseNumber);
                            if (redBagMoney > 0M)
                            {
                                var fundManager = new FundManager();
                                var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                                var maxUseMoney = info.TotalMoney * percent / 100;
                                if (redBagMoney > maxUseMoney)
                                    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0:N2}%，即{1:N2}元", percent, maxUseMoney));
                                //红包支付
                                BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, redBagMoney, msg, "Bet", balancePassword);
                            }
                            //其它账户支付
                            BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, totalBetMoney - redBagMoney
                                , msg, "Bet", balancePassword);
                        }
                        else
                        {
                            //追号投注
                            var msg = string.Format("追号订单{0}投注", keyLine);
                            if (redBagMoney > 0M)
                            {
                                var fundManager = new FundManager();
                                var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                                var maxUseMoney = info.TotalMoney * percent / 100;
                                if (redBagMoney > maxUseMoney)
                                    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
                                //红包支付
                                BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, redBagMoney, msg, "Bet", balancePassword);
                            }
                            //其它账户支付
                            BusinessHelper.Payout_To_Frozen(BusinessHelper.FundCategory_Betting, userId, keyLine, totalBetMoney
                                , msg, "Bet", balancePassword);
                        }
                    }

                    #endregion


                    DB.Commit();
                    //扣款录入订单用时间
                    businessDT = watch.ElapsedMilliseconds;
                    //  
                }
                catch (Exception ex1)
                {
                    DB.Rollback();
                    throw ex1;
                }
            }
            catch (Exception ex)
            {
                // DB.Rollback();
                watch.Stop();
                throw ex;
            }

            try
            {



                //}
                // watch.Stop();
                //if (watch.Elapsed.TotalMilliseconds > 1000)
                //    writerLog.WriteLog("LotteryBetting", "SQL", (int)LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));


                //
                if (RedisHelperEx.EnableRedis)
                {
                    if (info.IssuseNumberList.Count > 1)
                    {
                        //追号
                        redisOrderList.KeyLine = keyLine;
                        redisOrderList.StopAfterBonus = info.StopAfterBonus;
                        RedisOrderBusiness.AddOrderToWaitSplitList(info.GameCode, redisOrderList);
                        //序列化订单到文件
                        // Task.Factory.StartNew();
                        SerializChaseOrder(info, keyLine);
                    }
                    else
                    {
                        //普通投注
                        if (redisOrderList.OrderList.Count > 0)
                            RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisOrderList.OrderList[0]);
                    }
                }
                long redisDT = watch.ElapsedMilliseconds;
                //  
                //拆票
                if (!RedisHelperEx.EnableRedis)
                    DoSplitOrderTickets(firstSchemeId);

                //watch.Stop();
                if (watch.Elapsed.TotalMilliseconds > 300)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("LotteryBetting + Redis + 投注耗时记录 \r\n");
                    sb.Append("userDT 查询用户信息：" + userDT.ToString() + " \r\n");
                    sb.Append("dataDT  数据验证时间：" + (dataDT - userDT).ToString() + " \r\n");
                    sb.Append("gametypesDT 彩种处理时间：" + (gametypesDT - dataDT).ToString() + " \r\n");
                    sb.Append("IssuseDT 期号处理时间：" + (IssuseDT - gametypesDT).ToString() + " \r\n");

                    sb.Append("orderDT 订单构建用时：" + (orderDT - IssuseDT).ToString() + " \r\n");

                    sb.Append("oneDataDt C_OrderDetail录入订单处理时间：" + (oneDataDt - orderDT).ToString() + " \r\n");
                    sb.Append("oneDataDt1 C_Sports_Order_Running录入订单处理时间：" + (oneDataDt1 - oneDataDt).ToString() + " \r\n");

                    sb.Append("发送短信录处理时间：" + (oneDataDt2 - oneDataDt1).ToString() + " \r\n");

                    sb.Append("扣款用时 ：" + (businessDT - oneDataDt2).ToString() + " \r\n");




                    sb.Append("redisDT redis录入订单处理时间：" + (redisDT - businessDT).ToString() + " \r\n");


                    sb.Append(keyLine + ",订单总用时毫秒:" + watch.Elapsed.TotalMilliseconds.ToString() + " \r\n");
                    //录入跟踪信息
                    Log4Log.Fatal(sb.ToString());
                    //  Console.WriteLine(sb.ToString());
                }
                //刷新用户在Redis中的余额
                BusinessHelper.RefreshRedisUserBalance(userId);
            }
            catch (Exception ex)
            {
                watch.Stop();
                var p = ex;

            }
            watch.Stop();
            return keyLine;
        }

        private void CheckSJBMatch(string gameType, int matchId)
        {
            var entity = new SJBMatchManager().GetSJBMatch(gameType, matchId);
            if (entity == null)
                throw new LogicException("投注场次错误");

            if (entity.BetState != "开售")
                throw new LogicException(string.Format("比赛{0}停止销售", matchId));
        }

        private List<C_JCZQ_SJBMatch> CheckSJBMatch(string gameType, string anteCode)
        {
            var matchIdArray = anteCode.Split(',');
            var matchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(gameType, matchIdArray);
            return matchList;
        }
        #endregion

        private void SerializChaseOrder(LotteryBettingInfo order, string chaseOrderId)
        {
            try
            {
                var json = JsonHelper.Serialize(order);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "ChaseOrder", DateTime.Now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileName = Path.Combine(path, string.Format("{0}.json", chaseOrderId));
                File.WriteAllText(fileName, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log4Log.Error("LotteryBetting-SerializChaseOrder-序列化失败", ex);
            }
        }

        /// <summary>
        /// 保存用户未投注订单
        /// </summary>
        public string SaveOrderLotteryBetting(LotteryBettingInfo info, string userId, out string keyLine)
        {
            //开启事务
            #region 数据验证
            info.GameCode = info.GameCode.ToUpper();
            //排序
            info.IssuseNumberList.Sort((x, y) =>
            {
                return x.IssuseNumber.CompareTo(y.IssuseNumber);
            });
            var totalNumberZhu = 0;
            foreach (var item in info.AnteCodeList)
            {
                try
                {
                    var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, item.GameType).AnalyzeAnteCode(item.AnteCode);
                    totalNumberZhu += zhu;
                }
                catch (Exception ex)
                {
                    throw new LogicException("投注号码出错 - " + ex.Message);
                }
            }
            var codeMoney = 0M;
            info.IssuseNumberList.ForEach(item =>
            {
                if (item.Amount < 1)
                    throw new LogicException("倍数不能小于1");
                var currentMoney = item.Amount * totalNumberZhu * 2M;
                if (currentMoney != item.IssuseTotalMoney)
                    throw new LogicException(string.Format("第{0}期投注金额应该是{1},实际是{2}", item.IssuseNumber, currentMoney, item.IssuseTotalMoney));
                codeMoney += currentMoney;
            });
            if (codeMoney != info.TotalMoney)
                throw new LogicException("投注期号总金额与方案总金额不匹配");
            var lotteryManager = new LotteryGameManager();
            var currentIssuse = lotteryManager.QueryCurrentIssuse(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty);
            if (currentIssuse == null)
                throw new LogicException("订单期号不存在，请联系客服");
            if (info.IssuseNumberList.First().IssuseNumber.CompareTo(currentIssuse.IssuseNumber) < 0)
                throw new LogicException("投注订单期号已过期");
            #endregion
            var gameTypes = lotteryManager.QueryEnableGameTypes();
            //using (var biz = new GameBizBusinessManagement())
            //{

            var gameInfo = lotteryManager.LoadGame(info.GameCode);
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            var schemeManager = new SchemeManager();
            var sportsManager = new Sports_Manager();
            keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
            if (info.IssuseNumberList.Count > 1)
                throw new LogicException("保存的订单只能投注一期");
            var orderIndex = 1;
            var IssuseNumberList = new List<C_Game_Issuse>();
            foreach (var issuse in info.IssuseNumberList)
            {
                var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                if (currentIssuseNumber == null)
                    throw new LogicException(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                if (currentIssuseNumber.LocalStopTime < DateTime.Now)
                    throw new LogicException(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                IssuseNumberList.Add(currentIssuseNumber);
            }
            DB.Begin();
            try
            {
                //修改批量录入
                IList<C_OrderDetail> orderDetailList = new List<C_OrderDetail>();
                IList<C_Sports_Order_Running> orderRunningList = new List<C_Sports_Order_Running>();
                IList<C_UserSaveOrder> userSaveOrderList = new List<C_UserSaveOrder>();
                IList<C_Sports_AnteCode> Sports_AnteCodeList = new List<C_Sports_AnteCode>();
                IList<C_Lottery_Scheme> Lottery_SchemeList = new List<C_Lottery_Scheme>();

                //  C_UserSaveOrder
                foreach (var issuse in info.IssuseNumberList)
                {
                    //var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.AnteCodeList[0].GameType, issuse.IssuseNumber);

                    var currentIssuseNumber = IssuseNumberList.FirstOrDefault(p => p.IssuseNumber == issuse.IssuseNumber);
                    var schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);
                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        Sports_AnteCodeList.Add(new C_Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = (int)BonusStatus.Waitting,
                            CreateTime = DateTime.Now,
                            GameCode = info.GameCode,
                            GameType = item.GameType.ToUpper(),
                            IsDan = item.IsDan,
                            IssuseNumber = issuse.IssuseNumber,
                            MatchId = string.Empty,
                            Odds = string.Empty,
                            PlayType = string.Empty,
                            SchemeId = schemeId,
                        });
                        //var gameType = lotteryManager.QueryGameType(info.GameCode, item.GameType);
                        var gameType = gameTypes.FirstOrDefault(a => a.Game.GameCode == info.GameCode && a.GameType == item.GameType.ToUpper());
                        if (!gameTypeList.Contains(gameType))
                        {
                            gameTypeList.Add(gameType);
                        }
                    }
                    var currentIssuseMoney = totalNumberZhu * issuse.Amount * 2M;
                    if (info.IssuseNumberList.Count == 1)
                    {
                        keyLine = schemeId;
                    }
                    else
                    {
                        Lottery_SchemeList.Add(new C_Lottery_Scheme
                        {
                            OrderIndex = orderIndex,
                            KeyLine = keyLine,
                            SchemeId = schemeId,
                            CreateTime = DateTime.Now,
                            IsComplate = false,
                            IssuseNumber = issuse.IssuseNumber,
                        });
                    }
                    var model = AddRunningOrderAndOrderDetail_BackList(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                         string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                         SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, "", info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                    // sportsManager.AddUserSaveOrder();
                    userSaveOrderList.Add(new C_UserSaveOrder
                    {
                        SchemeId = schemeId,
                        UserId = userId,
                        GameCode = info.GameCode,
                        GameType = string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                        PlayType = string.Empty,
                        SchemeType = (int)SchemeType.SaveScheme,
                        SchemeSource = (int)info.SchemeSource,
                        SchemeBettingCategory = (int)info.BettingCategory,
                        ProgressStatus = (int)ProgressStatus.Waitting,
                        IssuseNumber = issuse.IssuseNumber,
                        Amount = issuse.Amount,
                        BetCount = totalNumberZhu,
                        TotalMoney = info.TotalMoney,
                        StopTime = currentIssuseNumber.LocalStopTime,
                        CreateTime = DateTime.Now,
                        StrStopTime = currentIssuseNumber.LocalStopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                    });
                    //修改批量录入
                    //IList<C_OrderDetail> orderDetailList = new List<C_OrderDetail>();
                    //IList<C_Sports_Order_Running> orderRunningList = new List<C_Sports_Order_Running>();
                    //IList<C_UserSaveOrder> userSaveOrderList = new List<C_UserSaveOrder>();
                    //IList<C_Sports_AnteCode> Sports_AnteCodeList = new List<C_Sports_AnteCode>();
                    //IList<C_Lottery_Scheme> Lottery_SchemeList = new List<C_Lottery_Scheme>();

                    orderDetailList.Add(model.OrderDetail);
                    orderRunningList.Add(model.Order_Running);
                    orderIndex++;
                }
                //KSON 优化批量录入

                DB.GetDal<C_Sports_AnteCode>().BulkAdd(Sports_AnteCodeList);
                DB.GetDal<C_Lottery_Scheme>().BulkAdd(Lottery_SchemeList);
                DB.GetDal<C_Sports_Order_Running>().BulkAdd(orderRunningList);
                DB.GetDal<C_OrderDetail>().BulkAdd(orderDetailList);

                DB.GetDal<C_UserSaveOrder>().BulkAdd(userSaveOrderList);

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return keyLine;
        }

        /// <summary>
        /// 创建合买_保存订单
        /// </summary>
        public string SaveCreateSportsTogether(Sports_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop)
        {
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new LogicException("方案拆分不正确");
            if (info.Subscription < 1)
                throw new LogicException("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new LogicException("发起者认购份数和保底份数不能超过总份数");
            var schemeId = string.Empty;

            var sportsManager = new Sports_Manager();
            var stopTime = CheckGeneralBettingMatch(sportsManager, info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            var betCount = 0;

            #region 计算注数
            if (info.GameCode == "BJDC" || info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
            {
                betCount = CheckBettingOrderMoney(info.AnteCodeList, info.GameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
                //if (betCount > BusinessHelper.GetMaxBetCount())
                //    throw new LogicException("您好！单票注数不能大于一万注");
                //检查投注号码
                foreach (var item in info.AnteCodeList)
                {
                    var msg = string.Empty;
                    var zhu = AnalyzerFactory.GetSportAnteCodeChecker(info.GameCode, item.GameType).CheckAntecodeNumber(item, out msg);
                    if (!string.IsNullOrEmpty(msg))
                        throw new LogicException(msg);
                }
            }
            else
            {
                var codeMoney = 0M;
                foreach (var item in info.AnteCodeList)
                {
                    try
                    {
                        var type = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper();
                        var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, type).AnalyzeAnteCode(item.AnteCode);
                        //if (zhu > BusinessHelper.GetMaxBetCount())
                        //    throw new LogicException("您好！单票注数不能大于一万注");
                        betCount += zhu;
                        codeMoney += zhu * info.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
                    }
                    catch (Exception ex)
                    {
                        throw new LogicException("投注号码出错 - " + ex.Message);
                    }
                }

            }
            #endregion

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                //var IsEnableLimitBetAmount = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableLimitBetAmount").ConfigValue);
                //if (IsEnableLimitBetAmount)//开启限制用户单倍投注
                //{
                //    if (info.Amount == 1 && betCount > 50)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //}


                //if (betCount > BusinessHelper.GetMaxBetCount())
                //    throw new LogicException("您好！单票注数不能大于一万注");
                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(userId);

                var issuseNumberOrTime = (info.GameCode == "JCZQ" || info.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.IssuseNumber;
                //添加合买信息
                var main = SavaOrder_AddTogetherInfo(info, info.TotalCount, info.TotalMoney, info.GameCode, info.GameType, info.PlayType, info.SchemeSource, info.Security, info.TotalMatchCount,
                    stopTime, true, schemeDeduct, user.UserId, user.AgentId, balancePassword,
                    sysGuarantees, isTop, SchemeBettingCategory.GeneralBetting, issuseNumberOrTime);
                schemeId = main.SchemeId;

                //添加订单信息
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber,
                    info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.TogetherBetting, false, true, user.UserId, user.AgentId, DateTime.Now, info.ActivityType, info.Attach, info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                //添加投注号码信息
                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode.ToUpper(),
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = string.IsNullOrEmpty(info.PlayType) ? string.Empty : info.PlayType.ToUpper(),
                        Odds = string.Empty,
                    });
                }

                //用户的订单保存
                sportsManager.AddUserSaveOrder(new C_UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = (int)SchemeType.SaveScheme,
                    SchemeSource = (int)info.SchemeSource,
                    SchemeBettingCategory = (int)info.BettingCategory,
                    ProgressStatus = (int)ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    //StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),//20160225 根据比赛截止时间处理保存订单，出票中心发现是保存订单不做过期撤单处理
                    StrStopTime = stopTime.ToString("yyyyMMddHHmm"),
                });

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}
            return schemeId;
        }


        public void RequestTicket_Sport(IEntityOrder order, bool isTrue, Dictionary<string, string> matchIdOddsList, string[] matchIdArray)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var matchIdList = new List<string>();

            var manager = new Sports_Manager();
            //var ticketTable = GetNewTicketTable();
            var ticketTable = new List<C_Sports_Ticket>();
            var count = 0L;
            //key:比赛编号_玩法  value:赔率
            var oddDic = new Dictionary<string, string>();
            foreach (var ticket in order.GetTicketList())
            {
                count++;
                var betContent = ticket.ToAnteString_LocalhostShop();
                var betMatchList = betContent.Split('/');
                var locOdds = new List<string>();
                if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                {
                    foreach (var item in betMatchList)
                    {
                        var matchL = item.Split('_');
                        if (matchL.Length < 2)
                            throw new LogicException("投注比赛内容出错 - " + item);
                        if (matchL.Length == 2)
                            locOdds.Add(matchIdOddsList.FirstOrDefault(p => p.Key == string.Format("{0}_{1}", ticket.GameType.ToUpper(), matchL[0])).Value);
                        else if (matchL.Length == 3 && ticket.GameType.ToUpper() == "HH")
                            locOdds.Add(matchIdOddsList.FirstOrDefault(p => p.Key == string.Format("{0}_{1}", matchL[0].ToUpper(), matchL[1])).Value);
                    }
                }
                var locOddStr = string.Join("/", locOdds);

                //var history = new Sports_Ticket
                //{
                //    GameCode = ticket.GameCode.ToUpper(),
                //    GameType = ticket.GameType.ToUpper(),
                //    SchemeId = ticket.OrderId,
                //    TicketId = ticket.Id,
                //    IssuseNumber = ticket.IssuseNumber,
                //    CreateTime = DateTime.Now,
                //    TicketStatus = isTrue ? TicketStatus.Ticketing : TicketStatus.Waitting,
                //    PlayType = ticket.PlayType,
                //    Amount = ticket.Amount,
                //    BetUnits = ticket.BetCount,
                //    BetMoney = ticket.TotalMoney,
                //    BetContent = betContent,
                //    MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                //    LocOdds = string.Join("/", locOdds),
                //    PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper(),
                //    IsAppend = false,
                //};
                //manager.AddSports_Ticket(history);

                //替换为批量插入票表
                //DataRow r = ticketTable.NewRow();
                C_Sports_Ticket r = new C_Sports_Ticket();
                //r["Id"] = count;
                //r["GameCode"] = ticket.GameCode.ToUpper();
                //r["GameType"] = ticket.GameType.ToUpper();
                //r["SchemeId"] = ticket.OrderId;
                //r["TicketId"] = ticket.Id;
                //r["IssuseNumber"] = ticket.IssuseNumber;
                //r["PlayType"] = ticket.PlayType;
                //r["BetContent"] = betContent;
                //r["LocOdds"] = locOddStr;
                //r["TicketStatus"] = 90;
                //r["Amount"] = ticket.Amount;
                //r["BetUnits"] = ticket.BetCount;
                //r["BetMoney"] = ticket.TotalMoney;
                //r["MatchIdList"] = ticket.ToAnteString_zhongminToMatchId();
                //r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                //r["IsAppend"] = false;
                //r["BonusStatus"] = 0;
                //r["PreTaxBonusMoney"] = 0M;
                //r["AfterTaxBonusMoney"] = 0M;
                //r["PrintDateTime"] = DateTime.Now;
                //r["CreateTime"] = DateTime.Now;

                r.GameCode = ticket.GameCode.ToUpper();
                r.GameType = ticket.GameType.ToUpper();
                r.SchemeId = ticket.OrderId;
                r.TicketId = ticket.Id;
                r.IssuseNumber = ticket.IssuseNumber;
                r.PlayType = ticket.PlayType;
                r.BetContent = betContent;
                r.LocOdds = locOddStr;
                r.TicketStatus = 90;
                r.Amount = ticket.Amount;
                r.BetUnits = ticket.BetCount;
                r.BetMoney = ticket.TotalMoney;
                r.MatchIdList = ticket.ToAnteString_zhongminToMatchId();
                r.PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper();
                r.IsAppend = false;
                r.BonusStatus = 0;
                r.PreTaxBonusMoney = 0M;
                r.AfterTaxBonusMoney = 0M;
                r.PrintDateTime = DateTime.Now;
                r.CreateTime = DateTime.Now;

                ticketTable.Add(r);

                redisTicketList.Add(new RedisTicketInfo
                {
                    AfterBonusMoney = 0M,
                    Amount = ticket.Amount,
                    BetContent = betContent,
                    BetMoney = ticket.TotalMoney,
                    BetUnits = ticket.BetCount,
                    BonusStatus = BonusStatus.Waitting,
                    GameCode = ticket.GameCode.ToUpper(),
                    GameType = ticket.GameType.ToUpper(),
                    IsAppend = false,
                    IssuseNumber = ticket.IssuseNumber,
                    LocOdds = locOddStr,
                    MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                    PlayType = ticket.PlayType,
                    PreBonusMoney = 0M,
                    SchemeId = ticket.OrderId,
                    TicketId = ticket.Id,
                });

                #region 号码表更新对应比赛sp

                var betContentArray = betContent.Split('/');
                foreach (var content in betContentArray)
                {
                    var contentArray = content.Split('_');
                    var gameType = string.Empty;
                    var matchId = string.Empty;
                    if (contentArray.Length == 3)
                    {
                        //混合
                        gameType = contentArray[0];
                        matchId = contentArray[1];
                    }
                    if (contentArray.Length == 2)
                    {
                        //普通
                        gameType = ticket.GameType.ToUpper();
                        matchId = contentArray[0];
                    }
                    if (string.IsNullOrEmpty(gameType) || string.IsNullOrEmpty(matchId)) continue;

                    var matchIdStr = string.Format("{0}_", matchId);
                    var oddItem = locOdds.FirstOrDefault(p => p.StartsWith(matchIdStr));
                    if (string.IsNullOrEmpty(oddItem)) continue;
                    var matchIdOdd = oddItem.Replace(matchIdStr, string.Empty);
                    var key = string.Format("{0}_{1}", matchId, gameType).ToUpper();
                    if (!oddDic.ContainsKey(key))
                        oddDic.Add(key, matchIdOdd);
                }

                #endregion
            }

            //批量插入票表
            manager.SqlBulkAddTable(ticketTable);
            //调用Redis保存
            if (RedisHelperEx.EnableRedis)
            {
                if (order.GameCode == "BJDC")
                {
                    RedisOrderBusiness.AddToRunningOrder_BJDC(order.OrderId, matchIdArray, redisTicketList);
                }
                if (new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode))
                {
                    RedisOrderBusiness.AddToRunningOrder_JC(order.GameCode, order.OrderId, matchIdArray, redisTicketList);
                }
            }

            //更新号码表比赛SP
            var codeList = manager.QuerySportsAnteCodeBySchemeId(order.OrderId);
            foreach (var item in codeList)
            {
                if (!string.IsNullOrEmpty(item.Odds))
                    continue;

                var key = string.Format("{0}_{1}", item.MatchId, item.GameType).ToUpper();
                if (oddDic.ContainsKey(key))
                {
                    var odd = oddDic[key] + ",-1|1";
                    item.Odds = odd;
                    manager.UpdateSports_AnteCode(item);
                }
            }
        }

        /// <summary>
        /// 合买保存订单；注意：合买保存订单时不做扣款,但需要记录售出份数
        /// </summary>
        private C_Sports_Together SavaOrder_AddTogetherInfo(TogetherSchemeBase info, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
            SchemeSource schemeSource, TogetherSchemeSecurity security, int totalMatchCount, DateTime stopTime, bool isUploadAnteCode,
            decimal schemeDeduct, string userId, string userAgent, string balancePassword, int sysGuarantees, bool isTop, SchemeBettingCategory category, string issuseNumber)
        {
            // var canChase = false;
            stopTime = stopTime.AddMinutes(-5);

            if (DateTime.Now >= stopTime)
                throw new LogicException(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" }.Contains(gameCode))
                gameType = string.Empty;

            var schemeId = BettingHelper.GetTogetherBettingSchemeId();
            var sportsManager = new Sports_Manager();

            //存入临时合买表
            sportsManager.AddTemp_Together(new C_Temp_Together
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                SchemeId = schemeId,
                StopTime = stopTime.ToString("yyyyMMddHHmm"),
            });

            //合买信息
            var main = new C_Sports_Together();
            main.BonusDeduct = info.BonusDeduct;
            main.SchemeDeduct = schemeDeduct;
            main.CreateTime = DateTime.Now;
            main.CreateUserId = userId;
            main.SchemeSource = (int)schemeSource;
            main.SchemeBettingCategory = (int)category;
            main.AgentId = userAgent;
            main.Description = info.Description;
            main.Guarantees = info.Guarantees;
            main.IsTop = isTop;
            main.IsUploadAnteCode = isUploadAnteCode;
            main.JoinPwd = string.IsNullOrEmpty(info.JoinPwd) ? string.Empty : Encipherment.MD5(info.JoinPwd);
            main.JoinUserCount = 1;
            main.Price = info.Price;
            main.SoldCount = 0;
            main.SchemeId = schemeId;
            main.Security = (int)security;
            main.Subscription = info.Subscription;
            main.SystemGuarantees = info.TotalCount * sysGuarantees / 100;
            main.Title = info.Title;
            main.TotalCount = totalCount;
            main.TotalMoney = totalMoney;
            main.TotalMatchCount = totalMatchCount;
            main.StopTime = stopTime;
            main.GameCode = gameCode;
            main.GameType = gameType;
            main.PlayType = playType;
            main.CreateTimeOrIssuseNumber = issuseNumber;

            #region 处理认购

            var subItem = new C_Sports_TogetherJoin
            {
                AfterTaxBonusMoney = 0M,
                BuyCount = info.Subscription,
                RealBuyCount = info.Subscription,
                CreateTime = DateTime.Now,
                JoinType = (int)TogetherJoinType.Subscription,
                JoinUserId = userId,
                Price = info.Price,
                SchemeId = schemeId,
                TotalMoney = info.Subscription * info.Price,
                JoinSucess = false,
                JoinLog = "认购合买",
            };
            sportsManager.AddSports_TogetherJoin(subItem);

            //BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, subItem.Id), info.Subscription * info.Price,
            //     string.Format("发起合买认购{0:N2}元", info.Subscription * info.Price), "Bet", balancePassword);

            main.SoldCount += info.Subscription;

            #endregion

            #region 处理自动跟单

            var surplusCount = main.TotalCount - main.SoldCount;
            var surplusPercent = (decimal)surplusCount / main.TotalCount * 100;
            var balanceManager = new UserBalanceManager();
            foreach (var item in sportsManager.QuerySportsTogetherFollowerList(userId, gameCode, gameType))
            {
                if (!string.IsNullOrEmpty(main.JoinPwd)) break;
                if (surplusCount == 0) continue;
                if (!item.IsEnable) continue;
                //跟单人 本彩种是否还能继续跟单 其中 -1 不无限跟单
                if (item.SchemeCount != -1)
                {
                    //为0后表示已跟单数完成，不再继续跟单
                    if (item.SchemeCount == 0 || item.SchemeCount < -1)
                        continue;
                }
                //跟单人 资金余额是否达到 订制的最小值
                var b = balanceManager.QueryUserBalance(item.FollowerUserId);
                if (b == null)
                    continue;
                if (b.GetTotalEnableMoney() <= item.StopFollowerMinBalance)
                    continue;

                //方案最小金额
                if (item.MinSchemeMoney != -1 && info.TotalMoney < item.MinSchemeMoney)
                    continue;
                //方案最大金额
                if (item.MaxSchemeMoney != -1 && info.TotalMoney > item.MaxSchemeMoney)
                    continue;

                // 当方案剩余份数/百分比不足时 是否跟单
                if (surplusCount < item.FollowerCount && !item.CancelWhenSurplusNotMatch)
                    continue;
                if (surplusPercent < item.FollowerPercent && !item.CancelWhenSurplusNotMatch)
                    continue;

                //计算真实应该买的份数
                var buyCount = (item.FollowerCount != -1) ? (surplusCount <= item.FollowerCount ? surplusCount : item.FollowerCount) :
                    (item.FollowerPercent == -1) ? 0 : (surplusPercent <= item.FollowerPercent ? surplusCount : (int)(item.FollowerPercent * main.TotalCount / 100));
                var realBuyMoney = buyCount * main.Price;
                if (realBuyMoney < 1)
                    continue;
                //用户资金余额 小于 实际购买总金额
                if (b.GetTotalEnableMoney() < realBuyMoney)
                    continue;

                //连续X个方案未中奖则停止跟单
                if (item.CancelNoBonusSchemeCount != -1 && item.NotBonusSchemeCount >= item.CancelNoBonusSchemeCount)
                    continue;

                //添加参与合买记录
                var joinItem = new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    CreateTime = DateTime.Now,
                    JoinType = (int)TogetherJoinType.FollowerJoin,
                    JoinUserId = item.FollowerUserId,
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = realBuyMoney,
                    JoinSucess = false,
                    JoinLog = "跟单参与合买",
                };
                sportsManager.AddSports_TogetherJoin(joinItem);

                //添加跟单记录
                sportsManager.AddTogetherFollowerRecord(new C_Together_FollowerRecord
                {
                    RuleId = item.Id,
                    RecordKey = string.Format("{0}_{1}_{2}_{3}", userId, item.FollowerUserId, gameCode, gameType),
                    BuyCount = buyCount,
                    BuyMoney = realBuyMoney,
                    CreaterUserId = main.CreateUserId,
                    CreateTime = DateTime.Now,
                    FollowerUserId = item.FollowerUserId,
                    GameCode = main.GameCode,
                    GameType = main.GameType,
                    Price = main.Price,
                    BonusMoney = 0,
                    SchemeId = schemeId,
                });
                //修改跟单规则记录
                item.TotalBetMoney += realBuyMoney;
                item.TotalBetOrderCount++;
                if (item.SchemeCount > 0)
                    item.SchemeCount--;
                sportsManager.UpdateTogetherFollowerRule(item);

                try
                {
                    //扣钱
                    //BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, joinItem.JoinUserId, string.Format("{0}_{1}", schemeId, joinItem.Id), realBuyMoney,
                    //string.Format("跟单参与合买{0:N2}元", realBuyMoney), string.Empty, string.Empty);
                }
                catch (Exception)
                {
                    continue;
                }
                main.JoinUserCount++;
                main.SoldCount += buyCount;
                surplusCount = main.TotalCount - main.SoldCount;
                surplusPercent = (decimal)surplusCount / main.TotalCount * 100;

                //处理战绩
                var beed = sportsManager.QueryUserBeedings(main.CreateUserId, main.GameCode, main.GameType);
                if (beed != null)
                {
                    beed.BeFollowedTotalMoney += realBuyMoney;
                    sportsManager.UpdateUserBeedings(beed);
                }
            }

            #endregion

            main.Progress = (decimal)main.SoldCount / main.TotalCount;
            main.ProgressStatus = (int)TogetherSchemeProgress.SalesIn;

            if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                main.ProgressStatus = (int)TogetherSchemeProgress.Standard;
            if (main.SoldCount == main.TotalCount)
                main.ProgressStatus = (int)TogetherSchemeProgress.Finish;

            #region 发起人保底

            var guaranteeMoney = info.Guarantees * info.Price;
            //扣钱
            if (guaranteeMoney > 0)
            {
                var minGuarantees = main.TotalCount - main.SoldCount;
                var guaranteeItem = new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = info.Guarantees,
                    RealBuyCount = minGuarantees <= info.Guarantees ? minGuarantees : info.Guarantees,
                    CreateTime = DateTime.Now,
                    JoinType = (int)TogetherJoinType.Guarantees,
                    JoinUserId = userId,
                    Price = info.Price,
                    SchemeId = schemeId,
                    TotalMoney = guaranteeMoney,
                    JoinSucess = false,
                    JoinLog = "合买保底",
                };
                sportsManager.AddSports_TogetherJoin(guaranteeItem);

                //BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, guaranteeItem.Id), guaranteeMoney,
                //string.Format("发起合买保底{0:N2}元", guaranteeMoney), "Bet", balancePassword);
            }

            #endregion

            //if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) / main.TotalCount >= 1M)
            //    canChase = true;
            ////是否能出票
            //if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount)
            //    canChase = true;

            //系统实际保底
            int systemGuarantees = main.TotalCount - main.SoldCount - main.Guarantees;
            if (systemGuarantees < 0)
                systemGuarantees = 0;

            if (systemGuarantees > 0)
            {
                var tempSystemGuarantees = systemGuarantees <= main.SystemGuarantees ? systemGuarantees : main.SystemGuarantees;
                //记录系统保底
                sportsManager.AddSports_TogetherJoin(new C_Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = main.SystemGuarantees,
                    RealBuyCount = tempSystemGuarantees,
                    CreateTime = DateTime.Now,
                    JoinLog = "网站保底参与合买",
                    JoinSucess = false,
                    JoinType = (int)TogetherJoinType.SystemGuarantees,
                    JoinUserId = "xtadmin",
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = tempSystemGuarantees * main.Price,
                });
            }
            SetTogetherIsTop(main);
            sportsManager.AddSports_Together(main);

            return main;
        }

        #region 优化合买
        /// <summary>
        /// 奖金优化合买
        /// </summary>
        public string CreateYouHuaTogether(Sports_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop, decimal realTotalMoney
            , out bool canChase, out DateTime stopTime, ref Sports_BetingInfo schemeInfo)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            var anteCodeList = new List<C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            //if (info.TotalCount * info.Price != info.TotalMoney)
            //    throw new LogicException("方案拆分不正确");
            if (info.Subscription < 1)
                throw new LogicException("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new LogicException("发起者认购份数和保底份数不能超过总份数");

            var schemeId = string.IsNullOrEmpty(info.SchemeId) ? BettingHelper.GetTogetherBettingSchemeId() : info.SchemeId;

            //对附件字符做验证
            CheckYouHuaBetAttach(info.Attach, realTotalMoney, info.BettingCategory);

            var sportsManager = new Sports_Manager();
            stopTime = CheckGeneralBettingMatch(sportsManager, info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            var betCount = 0;

            #region 计算注数
            if (info.GameCode == "BJDC" || info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
            {
                betCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //betCount = CheckBettingOrderMoney(info.AnteCodeList, info.GameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, true, userId);
                //检查投注号码
                foreach (var item in info.AnteCodeList)
                {
                    var msg = string.Empty;
                    var zhu = AnalyzerFactory.GetSportAnteCodeChecker(info.GameCode, item.GameType).CheckAntecodeNumber(item, out msg);
                    if (!string.IsNullOrEmpty(msg))
                        throw new Exception(msg);
                }
            }
            else
            {
                var codeMoney = 0M;
                foreach (var item in info.AnteCodeList)
                {
                    try
                    {
                        var type = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper();
                        var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, type).AnalyzeAnteCode(item.AnteCode);
                        betCount += zhu;
                        codeMoney += zhu * info.Amount * 2M;
                    }
                    catch (Exception ex)
                    {
                        throw new LogicException("投注号码出错 - " + ex.Message);
                    }
                }

                if (codeMoney != info.TotalMoney)
                    throw new LogicException("投注期号总金额与方案总金额不匹配");
            }
            #endregion

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                //var IsEnableLimitBetAmount = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableLimitBetAmount").ConfigValue);
                //if (IsEnableLimitBetAmount)//开启限制用户单倍投注
                //{
                //    if (info.Amount == 1 && betCount > 50)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //}

                //var userManager = new UserBalanceManager();
                //var user = userManager.QueryUserRegister(userId);

                var issuseNumberOrTime = (info.GameCode == "JCZQ" || info.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.IssuseNumber;
                //添加合买信息
                var main = AddTogetherInfo(info, schemeId, (int)realTotalMoney, realTotalMoney, info.GameCode, info.GameType, info.PlayType, info.SchemeSource, info.Security, info.TotalMatchCount,
                    stopTime, true, schemeDeduct, user.UserId, user.AgentId, balancePassword,
                    sysGuarantees, isTop, SchemeBettingCategory.YouHua, issuseNumberOrTime, out canChase);
                //schemeId = main.SchemeId;

                //添加订单信息
                //AddRunningOrderAndOrderDetail(schemeId, SchemeBettingCategory.YouHua, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber,
                //    info.Amount, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
                //    SchemeType.TogetherBetting, canChase, false, user.UserId, user.AgentId, stopTime, info.ActivityType, info.Attach, info.IsAppend);
                //优化投注倍数始终为1倍
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber,
                    1, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.TogetherBetting, canChase, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                //添加投注号码信息
                foreach (var item in info.AnteCodeList)
                {
                    var entity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode.ToUpper(),
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = string.IsNullOrEmpty(info.PlayType) ? string.Empty : info.PlayType.ToUpper(),
                        Odds = string.Empty,
                    };
                    sportsManager.AddSports_AnteCode(entity);
                    anteCodeList.Add(entity);
                }

                schemeInfo.GameCode = info.GameCode;
                schemeInfo.GameType = info.GameType;
                schemeInfo.IssuseNumber = info.IssuseNumber;
                schemeInfo.TotalMoney = info.TotalMoney;
                schemeInfo.SoldCount = main.SoldCount;
                schemeInfo.SchemeProgress = (TogetherSchemeProgress)main.ProgressStatus;

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}


            #region 拆票

            if (canChase)
            {
                if (RedisHelperEx.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrder
                    {
                        RunningOrder = runningOrder,
                        AnteCodeList = anteCodeList
                    };
                    RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisWaitOrder);
                    //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
                }
                else
                {
                    DoSplitOrderTickets(schemeId);
                }
            }

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }
        #endregion

        public void CheckYouHuaBetAttach(string attach, decimal realTotalMoney, SchemeBettingCategory bettingCategory)
        {
            if (string.IsNullOrEmpty(attach))
                throw new LogicException("附加信息不能为空");
            var codeMoney = 0M;
            var attachArray = attach.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in attachArray)
            {
                var itemArray = item.Split('^');
                if (itemArray.Length != 2) continue;
                var amount = decimal.Parse(itemArray[1]);
                if (amount <= 0)
                    throw new LogicException("注数格式不正确");
                var isTrue = System.Text.RegularExpressions.Regex.IsMatch(itemArray[1].ToString(), "^([0-9]{1,})$");
                if (!isTrue)
                    throw new LogicException("注数格式不正确");

                codeMoney += amount * 2;
                foreach (var oneMatch in itemArray[0].Split('|'))
                {
                    var matchArray = oneMatch.Split('_');
                    if (matchArray.Length != 3)
                        throw new LogicException("投注内容不正确");
                    if (bettingCategory == SchemeBettingCategory.YouHua)//如果投注类别为奖金优化
                    {
                        //if (!new string[] { "SPF", "BRQSPF" }.Contains(matchArray[1]))
                        //    throw new LogicException("奖金优化只支持胜平负玩法");
                        //if (!new string[] { "3", "1", "0" }.Contains(matchArray[2]))
                        //    throw new LogicException("投注内容格式不正确");

                        if (!new string[] { "SPF", "BRQSPF", "ZJQ", "BQC", "BF" }.Contains(matchArray[1]))
                            throw new LogicException("奖金优化只支持胜平负玩法");
                        if (!BettingHelper.CheckAnteCode(matchArray[1], matchArray[2]))
                            throw new LogicException("投注内容格式不正确");
                    }
                }
            }
            if (codeMoney != realTotalMoney)
                throw new LogicException(string.Format("优化金额不正确，应为{0}，实际为{1}", codeMoney, realTotalMoney));
        }

        #region IOS相关投注(由于IOS是以虚拟订单的方式投注，所以重新添加优化函数)
        /// <summary>
        /// 虚拟奖金优化投注
        /// </summary>
        public string VirtualOrderYouHuaBet(Sports_BetingInfo info, string userId, decimal realTotalMoney, int betCount, DateTime stopTime)
        {
            string schemeId = string.Empty;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;
            if (string.IsNullOrEmpty(info.Attach))
                throw new LogicException("投注内容不完整");

            schemeId = BettingHelper.GetSportsBettingSchemeId(gameCode);

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
            info.IssuseNumber, 1, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
            SchemeType.GeneralBetting, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                //用户的订单保存
                sportsManager.AddUserSaveOrder(new C_UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = (int)SchemeType.SaveScheme,
                    SchemeSource = (int)info.SchemeSource,
                    SchemeBettingCategory = (int)info.BettingCategory,
                    ProgressStatus = (int)ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                });

                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    });
                }

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            return schemeId;
        }


        /// <summary>
        /// 奖金优化投注
        /// </summary>
        public string YouHuaBet(Sports_BetingInfo info, string userId, string password, decimal realTotalMoney, int betCount, DateTime stopTime, decimal redBagMoney)
        {
            var anteCodeList = new List<C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;
            if (string.IsNullOrEmpty(info.Attach))
                throw new LogicException("投注内容不完整");
            if (info.TotalMoney % 2 != 0 || realTotalMoney % 2 != 0)
                throw new AggregateException("订单金额不正确，应该为2的倍数");

            schemeId = string.IsNullOrEmpty(info.SchemeId) ? BettingHelper.GetSportsBettingSchemeId(gameCode) : info.SchemeId;

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                var canTicket = BettingHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, 1, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    var entity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    sportsManager.AddSports_AnteCode(entity);
                    anteCodeList.Add(entity);
                }

                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BettingHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BettingHelper.FormatGameCode(info.GameCode));
                if (redBagMoney > 0M)
                {
                    var fundManager = new FundManager();
                    var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                    var maxUseMoney = realTotalMoney * percent / 100;
                    if (redBagMoney > maxUseMoney)
                        throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
                    //红包支付
                    BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, msg, "Bet", password);
                }
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, realTotalMoney - redBagMoney, msg, "Bet", password);

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            #region 拆票

            if (RedisHelperEx.EnableRedis)
            {
                var redisWaitOrder = new RedisWaitTicketOrder
                {
                    AnteCodeList = anteCodeList,
                    RunningOrder = runningOrder,
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }
        #endregion

        /// <summary>
        /// 编辑跟单
        /// </summary>
        public void EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId)
        {
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            //    DB.Begin();
            //try
            //{
            var sportsManager = new Sports_Manager();
            var entity = sportsManager.QueryTogetherFollowerRule(ruleId);
            if (entity == null)
                throw new LogicException("找不到相关的订制跟单");
            if (entity.FollowerUserId != info.FollowerUserId)
                throw new LogicException("跟单规则不是此用户所订制");

            entity.CancelNoBonusSchemeCount = info.CancelNoBonusSchemeCount;
            entity.CancelWhenSurplusNotMatch = info.CancelWhenSurplusNotMatch;
            entity.FollowerCount = info.FollowerCount;
            entity.FollowerPercent = info.FollowerPercent;
            entity.IsEnable = info.IsEnable;
            entity.MaxSchemeMoney = info.MaxSchemeMoney;
            entity.MinSchemeMoney = info.MinSchemeMoney;
            entity.SchemeCount = info.SchemeCount;
            entity.StopFollowerMinBalance = info.StopFollowerMinBalance;
            sportsManager.UpdateTogetherFollowerRule(entity);

            //    DB.Commit();
            //}
            //catch (Exception ex)
            //{
            //    DB.Rollback();
            //    throw ex;
            //}
            //}
        }

        /// <summary>
        /// 订制合买跟单
        /// </summary>
        public void CustomTogetherFollower(TogetherFollowerRuleInfo info)
        {
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            if (info.CreaterUserId == info.FollowerUserId)
                throw new LogicException("用户不能定制跟单自己。");
            var numberGameCode = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
            if (numberGameCode.Contains(info.GameCode))
            {
                if (string.IsNullOrEmpty(info.CreaterUserId) || string.IsNullOrEmpty(info.FollowerUserId) || string.IsNullOrEmpty(info.GameCode))
                    throw new LogicException("请输入必填项");
            }
            else
            {
                if (string.IsNullOrEmpty(info.CreaterUserId) || string.IsNullOrEmpty(info.FollowerUserId) || string.IsNullOrEmpty(info.GameCode) || string.IsNullOrEmpty(info.GameType))
                    throw new LogicException("请输入必填项");
            }
            if (info.SchemeCount < 1 && info.SchemeCount != -1)
                throw new LogicException("跟单方案数只能等于-1或大于0");
            if (info.MinSchemeMoney < 1 && info.MinSchemeMoney != -1)
                throw new LogicException("最小方案金额只能等于-1或大于0");
            if (info.MaxSchemeMoney < 1 && info.MaxSchemeMoney != -1)
                throw new LogicException("最大方案金额只能等于-1或大于0");
            if ((info.FollowerCount == -1 && info.FollowerPercent == -1) || (info.FollowerCount != -1 && info.FollowerPercent != -1))
                throw new LogicException("跟单份数和跟单百分比必须设置一个");
            if (info.CancelNoBonusSchemeCount < 1 && info.CancelNoBonusSchemeCount != -1)
                throw new LogicException("连续X个方案未中奖则停止跟单配置只能等于-1或大于0");
            if (info.StopFollowerMinBalance < 1 && info.StopFollowerMinBalance != -1)
                throw new LogicException("当用户金额小于X时停止跟单配置只能等于-1或大于0");

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            DB.Begin();

            try
            {


                var sportsManager = new Sports_Manager();
                var entity = sportsManager.QueryTogetherFollowerRule(info.CreaterUserId, info.FollowerUserId, info.GameCode, info.GameType);
                if (entity != null)
                    throw new LogicException("用户已经订制了跟单信息。");
                var followedCount = sportsManager.QueryTogetherFollowerRuleCount(info.CreaterUserId, info.GameCode, info.GameType);
                sportsManager.AddTogetherFollowerRule(new C_Together_FollowerRule
                {
                    IsEnable = info.IsEnable,
                    CreaterUserId = info.CreaterUserId,
                    CreateTime = DateTime.Now,
                    FollowerUserId = info.FollowerUserId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    SchemeCount = info.SchemeCount,
                    StopFollowerMinBalance = info.StopFollowerMinBalance,
                    CancelNoBonusSchemeCount = info.CancelNoBonusSchemeCount,
                    CancelWhenSurplusNotMatch = info.CancelWhenSurplusNotMatch,
                    FollowerCount = info.FollowerCount,
                    FollowerPercent = info.FollowerPercent,
                    MaxSchemeMoney = info.MaxSchemeMoney,
                    MinSchemeMoney = info.MinSchemeMoney,
                    NotBonusSchemeCount = 0,
                    FollowerIndex = followedCount + 1,
                });

                var beeding = sportsManager.QueryUserBeedings(info.CreaterUserId, info.GameCode, info.GameType);
                if (beeding == null)
                {
                    sportsManager.AddUserBeedings(new C_User_Beedings
                    {
                        UserId = info.CreaterUserId,
                        UpdateTime = DateTime.Now,
                        GameCode = info.GameCode,
                        GameType = info.GameType,
                        BeFollowerUserCount = 1,
                        BeFollowedTotalMoney = 0M,
                        GoldCrownCount = 0,
                        GoldCupCount = 0,
                        GoldDiamondsCount = 0,
                        GoldStarCount = 0,
                        SilverCrownCount = 0,
                        SilverCupCount = 0,
                        SilverDiamondsCount = 0,
                        SilverStarCount = 0,
                    });
                }
                else
                {
                    beeding.BeFollowerUserCount++;
                    sportsManager.UpdateUserBeedings(beeding);
                }

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}
        }

        /// <summary>
        /// 退订跟单
        /// </summary>
        public C_Together_FollowerRule ExistTogetherFollower(long followerId, string followerUserId)
        {
            C_Together_FollowerRule entity = null;
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();
            try
            {
                var sportsManager = new Sports_Manager();
                entity = sportsManager.QueryTogetherFollowerRule(followerId);
                if (entity == null)
                    throw new LogicException("找不到相关的订制跟单");
                if (entity.FollowerUserId != followerUserId)
                    throw new LogicException("跟单规则不是此用户所订制");

                sportsManager.DeleteTogetherFollowerRule(entity);

                var beeding = sportsManager.QueryUserBeedings(entity.CreaterUserId, entity.GameCode, entity.GameType);
                if (beeding != null && beeding.BeFollowerUserCount > 0)
                {
                    beeding.BeFollowerUserCount--;
                    sportsManager.UpdateUserBeedings(beeding);
                }

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}
            return entity;
        }

        private DataTable GetNewTicketTable()
        {
            var ticketTable = new DataTable("C_Sports_Ticket");
            ticketTable.Columns.Add("Id", typeof(long));
            ticketTable.Columns.Add("SchemeId", typeof(string));
            ticketTable.Columns.Add("TicketId", typeof(string));
            ticketTable.Columns.Add("GameCode", typeof(string));
            ticketTable.Columns.Add("GameType", typeof(string));
            ticketTable.Columns.Add("PlayType", typeof(string));
            ticketTable.Columns.Add("MatchIdList", typeof(string));
            ticketTable.Columns.Add("IssuseNumber", typeof(string));
            ticketTable.Columns.Add("BetUnits", typeof(int));
            ticketTable.Columns.Add("Amount", typeof(int));
            ticketTable.Columns.Add("BetMoney", typeof(decimal));
            ticketTable.Columns.Add("BetContent", typeof(string));
            ticketTable.Columns.Add("LocOdds", typeof(string));
            ticketTable.Columns.Add("TicketStatus", typeof(int));
            ticketTable.Columns.Add("TicketLog", typeof(string));
            ticketTable.Columns.Add("PartnerId", typeof(string));
            ticketTable.Columns.Add("Palmid", typeof(string));
            ticketTable.Columns.Add("PrintNumber1", typeof(string));
            ticketTable.Columns.Add("PrintNumber2", typeof(string));
            ticketTable.Columns.Add("PrintNumber3", typeof(string));
            ticketTable.Columns.Add("BarCode", typeof(string));
            ticketTable.Columns.Add("PrintOdd", typeof(string));
            ticketTable.Columns.Add("PrintUnOdd", typeof(string));
            ticketTable.Columns.Add("BonusStatus", typeof(int));
            ticketTable.Columns.Add("PreTaxBonusMoney", typeof(decimal));
            ticketTable.Columns.Add("AfterTaxBonusMoney", typeof(decimal));
            ticketTable.Columns.Add("PrintDateTime", typeof(DateTime));
            ticketTable.Columns.Add("Gateway", typeof(string));
            ticketTable.Columns.Add("CreateTime", typeof(DateTime));
            ticketTable.Columns.Add("IsAppend", typeof(bool));
            ticketTable.PrimaryKey = new DataColumn[] { ticketTable.Columns["Id"] };
            return ticketTable;
        }

        #region 宝单分享

        /// <summary>
        /// 宝单分享-创建宝单
        /// </summary>
        public string SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId)
        {
            var anteCodeList = new List<C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;

            schemeId = BettingHelper.GetSportsBettingSchemeId(gameCode);
            var sportsManager = new Sports_Manager();
            //验证比赛是否还可以投注
            var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            // 检查订单金额是否匹配
            var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();

            try
            {
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                var canTicket = BettingHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                    info.SchemeSource, info.Security, SchemeType.SingleTreasure, true, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);
                foreach (var item in info.AnteCodeList)
                {
                    var codeEntity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    sportsManager.AddSports_AnteCode(codeEntity);
                    anteCodeList.Add(codeEntity);
                }

                //用户的订单保存
                sportsManager.AddUserSaveOrder(new C_UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = (int)SchemeType.SingleTreasure,
                    SchemeSource = (int)info.SchemeSource,
                    SchemeBettingCategory = (int)info.BettingCategory,
                    ProgressStatus = (int)ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                    SingleTreasureDeclaration = info.SingleTreasureDeclaration,
                    BDFXCommission = info.BDFXCommission,
                });

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}


            #region 拆票
            if (RedisHelperEx.EnableRedis)
            {
                var redisWaitOrder = new RedisWaitTicketOrder
                {
                    RunningOrder = runningOrder,
                    AnteCodeList = anteCodeList,
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }

            #endregion

            return schemeId;
        }


        /// <summary>
        /// 宝单分享-抄单
        /// </summary>
        public string SportsBetting_BDFX(Sports_BetingInfo info, string userId, string password, string place)
        {
            var anteCodeList = new List<C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            DB.Begin();
            try
            {
                info.GameCode = info.GameCode.ToUpper();
                info.GameType = info.GameType.ToUpper();
                var gameCode = info.GameCode;

                schemeId = BettingHelper.GetSportsBettingSchemeId(gameCode);

                //检查投注号码
                foreach (var item in info.AnteCodeList)
                {
                    var error = string.Empty;
                    var zhu = AnalyzerFactory.GetSportAnteCodeChecker(info.GameCode, item.GameType).CheckAntecodeNumber(item, out error);
                    if (!string.IsNullOrEmpty(error))
                        throw new Exception(error);
                }

                var sportsManager = new Sports_Manager();
                //验证比赛是否还可以投注
                var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);

                // 检查订单金额是否匹配
                var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);

                //var IsEnableLimitBetAmount = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableLimitBetAmount").ConfigValue);
                //if (IsEnableLimitBetAmount)//开启限制用户单倍投注
                //{
                //    if (info.Amount == 1 && betCount > 50)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new LogicException("对不起，暂时不支持多串过关单倍金额超过100元。");
                //}
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                if (!user.IsEnable)
                    throw new LogicException("用户已禁用");
                var canTicket = BettingHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.SingleCopy, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);


                foreach (var item in info.AnteCodeList)
                {
                    var codeEntity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    sportsManager.AddSports_AnteCode(codeEntity);
                    anteCodeList.Add(codeEntity);
                }
                var BDFXManager = new TotalSingleTreasureManager();
                var BDFXRecordSingleCopyEntity = BDFXManager.QueryBDFXRecordSingleCopyBySchemeId(schemeId);
                if (BDFXRecordSingleCopyEntity == null)
                {
                    C_BDFX_RecordSingleCopy entity = new C_BDFX_RecordSingleCopy();
                    entity.BDXFSchemeId = info.BDFXSchemeId;
                    entity.SingleCopySchemeId = schemeId;
                    entity.CreateTime = DateTime.Now;
                    BDFXManager.AddBDFXRecordSingleCopy(entity);
                }

                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BettingHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BettingHelper.FormatGameCode(info.GameCode));

                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney, msg, place, password);

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            #region 拆票

            if (RedisHelperEx.EnableRedis)
            {
                var redisWaitOrder = new RedisWaitTicketOrder
                {
                    RunningOrder = runningOrder,
                    AnteCodeList = anteCodeList
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }

            #endregion

            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }
        //public Sports_TicketQueryInfoCollection QueryTicketListBySchemeId(string schemeId)
        //{
        //    using (var manager = new Sports_Manager())
        //    {
        //        Sports_TicketQueryInfoCollection collection = new Sports_TicketQueryInfoCollection();
        //        collection.TotalCount = 0;
        //        var ticketList = manager.QueryTicketList(schemeId);
        //        if (ticketList == null || ticketList.Count < 0)
        //            ticketList = manager.QueryTicketListHistory(schemeId);
        //        if (ticketList == null || ticketList.Count < 0)
        //            return collection;
        //        collection.TotalCount = ticketList.Count;
        //        collection.TicketList = new List<Sports_TicketQueryInfo>();
        //        foreach (var item in ticketList)
        //        {
        //            Sports_TicketQueryInfo info = new Sports_TicketQueryInfo();
        //            info.AfterTaxBonusMoney = item.AfterTaxBonusMoney;
        //            info.Amount = item.Amount;
        //            info.BetMoney = item.BetMoney;
        //            info.BetUnits = item.BetUnits;
        //            info.BonusStatus = item.BonusStatus;
        //            info.CreateTime = item.CreateTime;
        //            info.GameCode = item.GameCode;
        //            info.GameType = item.GameType;
        //            info.IssuseNumber = item.IssuseNumber;
        //            info.PlayType = item.PlayType;
        //            info.PreTaxBonusMoney = item.PreTaxBonusMoney;
        //            info.BarCode = item.BarCode;
        //            info.PrintNumber1 = item.PrintNumber1;
        //            info.PrintNumber2 = item.PrintNumber2;
        //            info.PrintNumber3 = item.PrintNumber3;
        //            info.SchemeId = item.SchemeId;
        //            info.TicketId = item.TicketId;
        //            info.TicketStatus = item.TicketStatus;
        //            info.BetContent = item.BetContent;
        //            info.LocOdds = item.LocOdds;
        //            info.PrintDateTime = item.PrintDateTime;
        //            collection.TicketList.Add(info);
        //        }
        //        return collection;
        //    }
        //}


        #endregion

        #region 世界杯投注
        /// <summary>
        /// 世界杯投注
        /// </summary>
        public string BetSJB(LotteryBettingInfo info, string userId, string balancePassword, string place, decimal redBagMoney)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");
            var gameType = string.Empty;
            //Redis订单列表
            var redisOrderList = new RedisWaitTicketOrderList();
            var schemeId = info.SchemeId;

            #region 数据验证

            if (info.GameCode != "SJB")
                throw new LogicException("彩种编码不正确");
            if (info.IssuseNumberList == null || info.IssuseNumberList.Count != 1)
                throw new LogicException("期号信息不能为空");
            if (info.AnteCodeList == null || info.AnteCodeList.Count <= 0)
                throw new LogicException("投注号码不能为空");

            var lotteryManager = new LotteryGameManager();
            var totalNumberZhu = 0;
            foreach (var item in info.AnteCodeList)
            {
                try
                {
                    //检查投注内容
                    var matchList = CheckSJBMatch(item.GameType, item.AnteCode);
                    if (matchList.Where(p => p.BetState != "开售").Count() > 0)
                        throw new LogicException("比赛中有包括未开售或过期的比赛");
                    gameType = item.GameType.ToUpper();

                    var zhu = AnalyzerFactory.GetAntecodeAnalyzer(info.GameCode, item.GameType).AnalyzeAnteCode(item.AnteCode);
                    totalNumberZhu += zhu;
                }
                catch (Exception ex)
                {
                    throw new LogicException("投注号码出错 - " + ex.Message);
                }
            }
            var codeMoney = 0M;
            info.IssuseNumberList.ForEach(item =>
            {
                //检查期号
                var currentIssuse = lotteryManager.QueryCurrentIssuse(info.GameCode, gameType);
                if (currentIssuse == null)
                    throw new LogicException("当前无奖期数据");
                if (currentIssuse.Status != (int)IssuseStatus.OnSale)
                    throw new LogicException("投注已截止");
                if (currentIssuse.IssuseNumber != item.IssuseNumber)
                    throw new LogicException(string.Format("当前期应该是{0}，实际是{1}", currentIssuse.IssuseNumber, item.IssuseNumber));

                if (item.Amount < 1)
                    throw new LogicException("倍数不能小于1");
                var currentMoney = item.Amount * totalNumberZhu * 2M;
                if (currentMoney != item.IssuseTotalMoney)
                    throw new LogicException(string.Format("第{0}期投注金额应该是{1},实际是{2}", item.IssuseNumber, currentMoney, item.IssuseTotalMoney));
                codeMoney += currentMoney;
            });

            if (codeMoney != info.TotalMoney)
                throw new LogicException("投注期号总金额与方案总金额不匹配");

            #endregion

            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{

            if (string.IsNullOrEmpty(schemeId))
                schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);

            DB.Begin();

            try
            {
                //var schemeManager = new SchemeManager();
                //var sportsManager = new Sports_Manager();

                var totalBetMoney = 0M;
                var issuse = info.IssuseNumberList[0];


                var anteCodeList = new List<C_Sports_AnteCode>();
                var gameTypeList = new List<string>();
                foreach (var item in info.AnteCodeList)
                {
                    var codeEntity = new C_Sports_AnteCode
                    {
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode,
                        GameType = item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = issuse.IssuseNumber,
                        MatchId = string.Empty,
                        Odds = string.Empty,
                        PlayType = string.Empty,
                        SchemeId = schemeId,
                    };
                    anteCodeList.Add(codeEntity);
                    //sportsManager.AddSports_AnteCode(codeEntity);
                    var gameTypeName = item.GameType.ToUpper() == "GJ" ? "冠军" : "冠亚军";
                    if (!gameTypeList.Contains(gameTypeName))
                    {
                        gameTypeList.Add(gameTypeName);
                    }
                }
                //录入投注号码
                DB.GetDal<C_Sports_AnteCode>().BulkAdd(anteCodeList);

                var currentIssuseMoney = totalNumberZhu * issuse.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);

                var canTicket = BettingHelper.CanRequestBet(info.GameCode);
                var entity = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", gameTypeList.ToArray()),
                      string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, GetSJB_StopBetTime(gameType), info.SchemeSource, info.Security,
                      info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting, true, false, user.UserId, user.AgentId,
                       info.CurrentBetTime, info.ActivityType, "", info.IsAppend, redBagMoney,
                      (canTicket ? ProgressStatus.Running : ProgressStatus.Waitting),
                      (canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting));
                totalBetMoney += currentIssuseMoney;

                //启用了Redis
                if (RedisHelperEx.EnableRedis)
                {
                    var runningOrder = new RedisWaitTicketOrder
                    {
                        AnteCodeList = anteCodeList,
                        RunningOrder = entity,
                        KeyLine = string.Empty,
                        StopAfterBonus = info.StopAfterBonus,
                        SchemeType = info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting
                    };
                    //追号方式 存入Redis订单列表
                    redisOrderList.OrderList.Add(runningOrder);
                }


                #region 支付

                //摇钱树订单，不扣用户的钱，扣代理商余额
                if (info.SchemeSource != SchemeSource.YQS
                    && info.SchemeSource != SchemeSource.YQS_Advertising
                    && info.SchemeSource != SchemeSource.NS_Bet
                    && info.SchemeSource != SchemeSource.YQS_Bet
                    && info.SchemeSource != SchemeSource.Publisher_0321
                    && info.SchemeSource != SchemeSource.WX_GiveLottery
                    && info.SchemeSource != SchemeSource.Web_GiveLottery
                    && info.SchemeSource != SchemeSource.LuckyDraw)
                {
                    // 消费资金
                    //普通投注
                    var msg = string.Format("{0}第{1}期投注", GetOZB_DisplayName(gameType), info.IssuseNumberList[0].IssuseNumber);
                    if (redBagMoney > 0M)
                    {
                        var fundManager = new FundManager();
                        var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                        var maxUseMoney = info.TotalMoney * percent / 100;
                        if (redBagMoney > maxUseMoney)
                            throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0:N2}%，即{1:N2}元", percent, maxUseMoney));
                        //红包支付
                        BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, msg, "Bet", balancePassword);
                    }
                    //其它账户支付
                    BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, totalBetMoney - redBagMoney
                        , msg, "Bet", balancePassword);
                }

                #endregion

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            //}

            if (RedisHelperEx.EnableRedis)
            {
                //普通投注
                if (redisOrderList.OrderList.Count > 0)
                    RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisOrderList.OrderList[0]);
            }

            //拆票
            if (!RedisHelperEx.EnableRedis)
                DoSplitOrderTickets(schemeId);

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }

        private string GetOZB_DisplayName(string gameType)
        {
            switch (gameType)
            {
                case "GJ":
                    return "冠军";
                case "GYJ":
                    return "冠亚军";
                default:
                    break;
            }
            return gameType;
        }

        private DateTime GetSJB_StopBetTime(string gameType)
        {
            switch (gameType.ToUpper())
            {
                case "GJ":
                    return new DateTime(2018, 7, 15, 0, 0, 0);
                case "GYJ":
                    return new DateTime(2018, 7, 15, 0, 0, 0);
                default:
                    break;
            }
            return DateTime.Now;
        }
        #endregion

        /// <summary>
        /// 查询投注号码信息
        /// </summary>
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId)
        {
            var result = new Sports_AnteCodeQueryInfoCollection();
            var sportsManager = new Sports_Manager();
            var sjbManager = new SJBMatchManager();
            var lotteryManager = new LotteryGameManager();
            var issuseList = new List<C_Game_Issuse>();
            var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            foreach (var item in codeList)
            {
                if (item.GameCode == "BJDC")
                {
                    #region BJDC

                    var match = sportsManager.QueryBJDC_Match(string.Format("{0}|{1}", item.IssuseNumber, item.MatchId));
                    var matchResult = sportsManager.QueryBJDC_MatchResult(string.Format("{0}|{1}", item.IssuseNumber, item.MatchId));
                    //matchResult.MatchState
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HomeHalf_Result, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeFull_Result, matchResult.GuestFull_Result);
                        matchState = matchResult.MatchState;
                        switch (item.GameType)
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "SXDS":
                                caiguo = matchResult.SXDS_Result;
                                matchResultSp = matchResult.SXDS_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = match.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = match.MatchName,
                        LeagueColor = match.MatchColor,
                        MatchId = item.MatchId,
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.MatchStartTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCZQ")
                {
                    #region JCZQ

                    var match = sportsManager.QueryJCZQ_Match(item.MatchId);
                    var matchResult = sportsManager.QueryJCZQ_MatchResult(item.MatchId);
                    //matchResult .MatchState
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HalfHomeTeamScore, matchResult.HalfGuestTeamScore);
                        fullResult = string.Format("{0}:{1}", matchResult.FullHomeTeamScore, matchResult.FullGuestTeamScore);
                        matchState = matchResult.MatchState;
                        switch (item.GameType.ToUpper())
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "BRQSPF":
                                caiguo = matchResult.BRQSPF_Result;
                                matchResultSp = matchResult.BRQSPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = match.HomeTeamId.ToString(),
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = match.GuestTeamId.ToString(),
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        XmlHeader = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCLQ")
                {
                    #region JCLQ

                    var match = sportsManager.QueryJCLQ_Match(item.MatchId);
                    var matchResult = sportsManager.QueryJCLQ_MatchResult(item.MatchId);
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        //halfResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestScore);
                        matchState = matchResult.MatchState;
                        switch (item.GameType.ToUpper())
                        {
                            case "SF":
                                caiguo = matchResult.SF_Result;
                                matchResultSp = matchResult.SF_SP;
                                break;
                            case "RFSF":
                                caiguo = matchResult.RFSF_Result;
                                matchResultSp = matchResult.RFSF_SP;
                                break;
                            case "SFC":
                                caiguo = matchResult.SFC_Result;
                                matchResultSp = matchResult.SFC_SP;
                                break;
                            case "DXF":
                                caiguo = matchResult.DXF_Result;
                                matchResultSp = matchResult.DXF_SP;
                                break;
                        }
                    }
                    result.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = item.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        MatchState = matchState,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "CTZQ")
                {
                    #region CTZQ

                    result.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = item.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = string.Empty,
                        LeagueColor = string.Empty,
                        MatchId = string.Empty,
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = string.Empty,
                        GuestTeamId = string.Empty,
                        GuestTeamName = string.Empty,
                        IsDan = item.IsDan,
                        StartTime = DateTime.Now,
                        HalfResult = string.Empty,
                        FullResult = string.Empty,
                        MatchResult = string.Empty,
                        MatchResultSp = 0M,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        WinNumber = string.Empty,
                    });

                    #endregion
                    continue;
                }
                if (item.GameCode == "JCSJBGJ" || item.GameCode == "JCYJ")
                {
                    var match = sjbManager.GetSJBMatch(item.GameCode, int.Parse(item.AnteCode));
                    result.Add(new Sports_AnteCodeQueryInfo
                    {
                        AnteCode = item.AnteCode,
                        IssuseNumber = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.Team,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.Team,
                        IsDan = item.IsDan,
                        CurrentSp = item.Odds,
                        BonusStatus = (BonusStatus)item.BonusStatus,
                        GameType = item.GameType,
                        StartTime = DateTime.Now,
                    });
                    continue;
                }

                var c = issuseList.Where(p => p.GameCode == item.GameCode && p.IssuseNumber == item.IssuseNumber).FirstOrDefault();
                if (c == null)
                {
                    c = lotteryManager.QueryGameIssuse(item.GameCode, item.IssuseNumber);
                    issuseList.Add(c);
                }
                result.Add(new Sports_AnteCodeQueryInfo
                {
                    AnteCode = item.AnteCode,
                    IssuseNumber = item.IssuseNumber,
                    BonusStatus = (BonusStatus)item.BonusStatus,
                    CurrentSp = item.Odds,
                    IsDan = item.IsDan,
                    GameType = item.GameType,
                    WinNumber = c == null ? string.Empty : string.IsNullOrEmpty(c.WinNumber) ? string.Empty : c.WinNumber,
                    StartTime = DateTime.Now,
                });
            }
            return result;
        }
    }
}
