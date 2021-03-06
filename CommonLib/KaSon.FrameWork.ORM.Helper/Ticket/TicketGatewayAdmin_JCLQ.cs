﻿using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Ticket;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// BJDC
    /// </summary>
    public partial class TicketGatewayAdmin
    {
        //public string PrizeBJDCTicket(int num)
        //{
        //    var successCount = 0;
        //    var failCount = 0;
        //    var log = new List<string>();

        //    try
        //    {
        //        var manager = new Sports_Manager();
        //        var ticketListInfo = manager.QueryPrizeTicketList("BJDC", num);
        //        var prizeList = new List<TicketBatchPrizeInfo>();
        //        //var ticketStrSql = string.Empty;
        //        foreach (var ticket in ticketListInfo.TicketList)
        //        {
        //            try
        //            {
        //                if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

        //                var preTaxBonusMoney = 0M;
        //                var afterTaxBonusMoney = 0M;
        //                var bonusCount = 0;
        //                var codeList = new List<Ticket_AnteCode_Running>();
        //                var collection = new GatewayAnteCodeCollection_Sport();
        //                //100_3/101_1
        //                foreach (var item in ticket.BetContent.Split('/'))
        //                {
        //                    var oneMatch = item.Split('_');
        //                    codeList.Add(new C_Ticket_AnteCode_Running
        //                    {
        //                        MatchId = oneMatch[0],
        //                        IssuseNumber = ticket.IssuseNumber,
        //                        AnteNumber = oneMatch[1],
        //                        IsDan = false,
        //                        GameType = ticket.GameType,
        //                    });
        //                    collection.Add(new GatewayAnteCode_Sport
        //                    {
        //                        AnteCode = oneMatch[1],
        //                        MatchId = oneMatch[0],
        //                        GameType = ticket.GameType,
        //                        IsDan = false
        //                    });
        //                }
        //                var n = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[1]);
        //                if (n > 1)
        //                {
        //                    #region M串N
        //                    var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
        //                    {
        //                        Amount = ticket.Amount,
        //                        AnteCodeList = collection,
        //                        Attach = string.Empty,
        //                        GameCode = ticket.GameCode,
        //                        GameType = ticket.GameType,
        //                        IssuseNumber = ticket.IssuseNumber,
        //                        IsVirtualOrder = false,
        //                        OrderId = ticket.SchemeId,
        //                        PlayType = ticket.PlayType.Replace("P", ""),
        //                        Price = 2,
        //                        UserId = string.Empty,
        //                        TotalMoney = ticket.BetMoney
        //                    }, "LOCAL", "agentId");

        //                    foreach (var ticket_cp in orderEntity.GetTicketList())
        //                    {
        //                        var matchIdL = (from c in ticket_cp.GetAnteCodeList() select c.MatchId).ToArray();
        //                        var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

        //                        var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket_cp.GameCode, ticket_cp.GameType, int.Parse(ticket_cp.PlayType.Replace("P", "").Split('_')[0]));
        //                        var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), ticketListInfo.MatchList.ToArray());
        //                        if (bonusResult.IsWin)
        //                        {
        //                            bonusCount += bonusResult.BonusCount;
        //                            for (var i = 0; i < bonusResult.BonusCount; i++)
        //                            {
        //                                var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                                var sps = GetSPs(ticket_cp.GameCode, ticket_cp.GameType, ticket_cp.IssuseNumber, matchIdList);
        //                                var oneBeforeBonusMoney = 2M;
        //                                var isTrue = false;
        //                                var num_q = 0;
        //                                foreach (var item in sps)
        //                                {
        //                                    if (item.Value == 1M)
        //                                    {
        //                                        num_q++;
        //                                        var entity = codeL.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
        //                                        var anteCodeCount = entity.AnteCode.Split(',').Count();
        //                                        oneBeforeBonusMoney *= anteCodeCount;
        //                                        if (sps.Count == 1) isTrue = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        oneBeforeBonusMoney *= item.Value;
        //                                    }
        //                                }
        //                                if (!isTrue && num_q != sps.Count)
        //                                    oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
        //                                oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
        //                                var oneAfterBonusMoney = oneBeforeBonusMoney;

        //                                //北单奖金小于2元的 按2元计算
        //                                if (oneBeforeBonusMoney < 2M)
        //                                {
        //                                    oneBeforeBonusMoney = 2M;
        //                                    oneAfterBonusMoney = 2M;
        //                                }

        //                                if (oneBeforeBonusMoney >= 10000)
        //                                {
        //                                    oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
        //                                }
        //                                oneBeforeBonusMoney *= ticket_cp.Amount;
        //                                oneAfterBonusMoney *= ticket_cp.Amount;

        //                                preTaxBonusMoney += oneBeforeBonusMoney;
        //                                afterTaxBonusMoney += oneAfterBonusMoney;
        //                            }
        //                        }
        //                    }

        //                    //单票金额上限
        //                    if (afterTaxBonusMoney >= 5000000M)
        //                        afterTaxBonusMoney = 5000000M;

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region M串1
        //                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]));
        //                    var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), ticketListInfo.MatchList.ToArray());
        //                    if (bonusResult.IsWin)
        //                    {
        //                        bonusCount += bonusResult.BonusCount;
        //                        for (var i = 0; i < bonusResult.BonusCount; i++)
        //                        {
        //                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                            var sps = GetSPs(ticket.GameCode, ticket.GameType, ticket.IssuseNumber, matchIdList);
        //                            var oneBeforeBonusMoney = 2M;
        //                            var isTrue = false;
        //                            var num_q = 0;
        //                            foreach (var item in sps)
        //                            {
        //                                if (item.Value == 1M)
        //                                {
        //                                    num_q++;
        //                                    var entity = codeList.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
        //                                    var anteCodeCount = entity.AnteCode.Split(',').Count();
        //                                    oneBeforeBonusMoney *= anteCodeCount;
        //                                    if (sps.Count == 1) isTrue = true;
        //                                }
        //                                else
        //                                {
        //                                    oneBeforeBonusMoney *= item.Value;
        //                                }
        //                            }
        //                            if (!isTrue && num_q != sps.Count)
        //                                oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
        //                            oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
        //                            var oneAfterBonusMoney = oneBeforeBonusMoney;

        //                            //北单奖金小于2元的 按2元计算
        //                            if (oneBeforeBonusMoney < 2M)
        //                            {
        //                                oneBeforeBonusMoney = 2M;
        //                                oneAfterBonusMoney = 2M;
        //                            }

        //                            if (oneBeforeBonusMoney >= 10000)
        //                            {
        //                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
        //                            }
        //                            oneBeforeBonusMoney *= ticket.Amount;
        //                            oneAfterBonusMoney *= ticket.Amount;

        //                            //单票金额上限
        //                            if (oneAfterBonusMoney >= 5000000M)
        //                                oneAfterBonusMoney = 5000000M;

        //                            preTaxBonusMoney += oneBeforeBonusMoney;
        //                            afterTaxBonusMoney += oneAfterBonusMoney;
        //                        }
        //                    }

        //                    #endregion
        //                }

        //                //更新票数据sql
        //                prizeList.Add(new TicketBatchPrizeInfo
        //                {
        //                    //Id = item.Id,
        //                    TicketId = ticket.TicketId,
        //                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
        //                    PreMoney = preTaxBonusMoney,
        //                    AfterMoney = afterTaxBonusMoney,
        //                });

        //                //var ticketStrSql = string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
        //                //          , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
        //                //manager.ExecSql(ticketStrSql);
        //                successCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                failCount++;
        //                log.Add(ticket.TicketId + "派奖出错 - " + ex.Message);
        //            }
        //        }

        //        //批量更新数据库
        //        BusinessHelper.UpdateTicketBonus(prizeList);

        //        log.Insert(0, string.Format("总查询到{0}张票,成功派奖票：{1}条，失败派奖票：{2}条", ticketListInfo.TicketList.Count, successCount, failCount));
        //    }
        //    catch (Exception ex)
        //    {
        //        return "派奖票数据出错 - " + ex.Message;
        //    }

        //    return string.Join(Environment.NewLine, log.ToArray());
        //}

        //public void PrizeBJDCTicket_OrderId(string orderId)
        //{
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var manager = new Sports_Manager();
        //        var ticketListInfo = manager.QueryPrizeTicket_OrderIdList("BJDC", orderId);
        //        var ticketStrSql = string.Empty;
        //        foreach (var ticket in ticketListInfo.TicketList)
        //        {
        //            if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

        //            var preTaxBonusMoney = 0M;
        //            var afterTaxBonusMoney = 0M;

        //            var bonusCount = 0;
        //            var collection = new GatewayAnteCodeCollection_Sport();
        //            var codeList = new List<Ticket_AnteCode_Running>();
        //            //100_3/101_1
        //            foreach (var item in ticket.BetContent.Split('/'))
        //            {
        //                var oneMatch = item.Split('_');
        //                codeList.Add(new Ticket_AnteCode_Running
        //                {
        //                    MatchId = oneMatch[0],
        //                    IssuseNumber = ticket.IssuseNumber,
        //                    AnteNumber = oneMatch[1],
        //                    IsDan = false,
        //                    GameType = ticket.GameType,
        //                });
        //                collection.Add(new GatewayAnteCode_Sport
        //                {
        //                    AnteCode = oneMatch[1],
        //                    MatchId = oneMatch[0],
        //                    GameType = ticket.GameType,
        //                    IsDan = false
        //                });
        //            }

        //            var n = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[1]);
        //            if (n > 1)
        //            {
        //                #region M串N
        //                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
        //                {
        //                    Amount = ticket.Amount,
        //                    AnteCodeList = collection,
        //                    Attach = string.Empty,
        //                    GameCode = ticket.GameCode,
        //                    GameType = ticket.GameType,
        //                    IssuseNumber = ticket.IssuseNumber,
        //                    IsVirtualOrder = false,
        //                    OrderId = ticket.SchemeId,
        //                    PlayType = ticket.PlayType.Replace("P", ""),
        //                    Price = 2,
        //                    UserId = string.Empty,
        //                    TotalMoney = ticket.BetMoney
        //                }, "LOCAL", "agentId");

        //                foreach (var ticket_cp in orderEntity.GetTicketList())
        //                {
        //                    var matchIdL = (from c in ticket_cp.GetAnteCodeList() select c.MatchId).ToArray();
        //                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

        //                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket_cp.GameCode, ticket_cp.GameType, int.Parse(ticket_cp.PlayType.Replace("P", "").Split('_')[0]));
        //                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), ticketListInfo.MatchList.ToArray());
        //                    if (bonusResult.IsWin)
        //                    {
        //                        bonusCount += bonusResult.BonusCount;
        //                        for (var i = 0; i < bonusResult.BonusCount; i++)
        //                        {
        //                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                            var sps = GetSPs(ticket_cp.GameCode, ticket_cp.GameType, ticket_cp.IssuseNumber, matchIdList);
        //                            var oneBeforeBonusMoney = 2M;
        //                            var isTrue = false;
        //                            var num = 0;
        //                            foreach (var item in sps)
        //                            {
        //                                if (item.Value == 1M)
        //                                {
        //                                    num++;
        //                                    var entity = codeL.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
        //                                    var anteCodeCount = entity.AnteCode.Split(',').Count();
        //                                    oneBeforeBonusMoney *= anteCodeCount;
        //                                    if (sps.Count == 1) isTrue = true;
        //                                }
        //                                else
        //                                {
        //                                    oneBeforeBonusMoney *= item.Value;
        //                                }
        //                            }
        //                            if (!isTrue && num != sps.Count)
        //                                oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
        //                            oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
        //                            var oneAfterBonusMoney = oneBeforeBonusMoney;

        //                            //北单奖金小于2元的 按2元计算
        //                            if (oneBeforeBonusMoney < 2M)
        //                            {
        //                                oneBeforeBonusMoney = 2M;
        //                                oneAfterBonusMoney = 2M;
        //                            }

        //                            if (oneBeforeBonusMoney >= 10000)
        //                            {
        //                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
        //                            }
        //                            oneBeforeBonusMoney *= ticket_cp.Amount;
        //                            oneAfterBonusMoney *= ticket_cp.Amount;

        //                            preTaxBonusMoney += oneBeforeBonusMoney;
        //                            afterTaxBonusMoney += oneAfterBonusMoney;
        //                        }
        //                    }
        //                }

        //                //单票金额上限
        //                if (afterTaxBonusMoney >= 5000000M)
        //                    afterTaxBonusMoney = 5000000M;

        //                #endregion
        //            }
        //            else
        //            {
        //                #region M串1
        //                var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]));
        //                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), ticketListInfo.MatchList.ToArray());
        //                if (bonusResult.IsWin)
        //                {
        //                    bonusCount += bonusResult.BonusCount;
        //                    for (var i = 0; i < bonusResult.BonusCount; i++)
        //                    {
        //                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                        var sps = GetSPs(ticket.GameCode, ticket.GameType, ticket.IssuseNumber, matchIdList);
        //                        var oneBeforeBonusMoney = 2M;
        //                        var isTrue = false;
        //                        var num = 0;
        //                        foreach (var item in sps)
        //                        {
        //                            if (item.Value == 1M)
        //                            {
        //                                num++;
        //                                var entity = codeList.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
        //                                var anteCodeCount = entity.AnteCode.Split(',').Count();
        //                                oneBeforeBonusMoney *= anteCodeCount;
        //                                if (sps.Count == 1) isTrue = true;
        //                            }
        //                            else
        //                            {
        //                                oneBeforeBonusMoney *= item.Value;
        //                            }
        //                        }
        //                        if (!isTrue && num != sps.Count)
        //                            oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
        //                        oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
        //                        var oneAfterBonusMoney = oneBeforeBonusMoney;

        //                        //北单奖金小于2元的 按2元计算
        //                        if (oneBeforeBonusMoney < 2M)
        //                        {
        //                            oneBeforeBonusMoney = 2M;
        //                            oneAfterBonusMoney = 2M;
        //                        }

        //                        if (oneBeforeBonusMoney >= 10000)
        //                        {
        //                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
        //                        }
        //                        oneBeforeBonusMoney *= ticket.Amount;
        //                        oneAfterBonusMoney *= ticket.Amount;

        //                        //单票金额上限
        //                        if (oneAfterBonusMoney >= 5000000M)
        //                            oneAfterBonusMoney = 5000000M;

        //                        preTaxBonusMoney += oneBeforeBonusMoney;
        //                        afterTaxBonusMoney += oneAfterBonusMoney;
        //                    }
        //                }

        //                #endregion
        //            }

        //            ticketStrSql += string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
        //                        , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
        //        }
        //        manager.ExecSql(ticketStrSql);

        //        biz.CommitTran();
        //    }
        //}

        //private Dictionary<string, decimal> GetSPs(string gameCode, string gameType, string issuseNumber, string[] matchIdList)
        //{
        //    var result = new Dictionary<string, decimal>();
        //    foreach (var id in matchIdList)
        //    {
        //        result.Add(id, ReturnSp(gameCode, gameType, issuseNumber, int.Parse(id)));
        //    }
        //    return result;
        //}

        //public decimal ReturnSp(string gameCode, string gameType, string issuseNumber, int id)
        //{
        //    if (gameType.ToUpper().Equals("SF"))
        //    {
        //        var entity = new SFGGMatchManager().QuerySFGGMatch(issuseNumber, id);
        //        if (entity.MatchState != 2 || (entity.SF_Result != "-1" && entity.SF_SP == 1M))
        //            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //        switch (gameType)
        //        {
        //            case "SF":
        //                return entity.SF_SP;
        //            default:
        //                throw new ArgumentException("获取比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
        //        }
        //    }
        //    else
        //    {
        //        var entity = new BJDCMatchManager().QueryBJDC_MatchResult(issuseNumber, id);
        //        switch (gameType)
        //        {
        //            case "SPF":
        //                if (entity.MatchState != "2" || (entity.SPF_Result != "-1" && entity.SPF_SP == 1M))
        //                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //                return entity.SPF_SP;
        //            case "ZJQ":
        //                if (entity.MatchState != "2" || (entity.ZJQ_Result != "-1" && entity.ZJQ_SP == 1M))
        //                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //                return entity.ZJQ_SP;
        //            case "SXDS":
        //                if (entity.MatchState != "2" || (entity.SXDS_Result != "-1" && entity.SXDS_SP == 1M))
        //                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //                return entity.SXDS_SP;
        //            case "BF":
        //                if (entity.MatchState != "2" || (entity.BF_Result != "-1" && entity.BF_SP == 1M))
        //                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //                return entity.BF_SP;
        //            case "BQC":
        //                if (entity.MatchState != "2" || (entity.BQC_Result != "-1" && entity.BQC_SP == 1M))
        //                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
        //                return entity.BQC_SP;
        //            default:
        //                throw new ArgumentException("获取比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
        //        }
        //    }
        //}

        public IMongoDatabase MonDB { get; set; }
        //private List<BJDCIssuseInfo> LoadBJDCIssuseLIst()
        //{
        //    //var fileName = string.Format(@"{0}\{1}\Match_IssuseNumber_List.json", _baseDir, "BJDC");
        //    //var json = ReadFileString(fileName);
        //    //if (string.IsNullOrEmpty(json))
        //    //    return new List<MatchBiz.Core.BJDC_IssuseInfo>();
        //    //   return JsonSerializer.Deserialize<List<MatchBiz.Core.BJDC_IssuseInfo>>(json);

        //    var coll = MonDB.GetCollection<BJDCIssuseInfo>("BJDCIssuseInfo");
        //    var builder = Builders<BJDCIssuseInfo>.Filter;
        //    // DateTime startTime = DateTime.Now.AddMonths(1);
        //    //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

        //    var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
        //    var documents = coll.Find<BJDCIssuseInfo>(filter).ToList();

        //    return documents;
        //}

        //    public void UpdateOddsList_JCLQ<TInfo, TEntity>(string gameCode, string gameType, string[] matchIdList, bool isDS)
        //where TInfo : JingCaiMatchBase, I_JingCai_Odds
        //where TEntity : JingCai_Odds, new()
        //    {
        //        var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
        //        using (var tran = new GameBizBusinessManagement())
        //        {
        //            tran.BeginTran();
        //            var oddsManager = new JCLQ_OddsManager();
        //            foreach (var id in matchIdList)
        //            {
        //                var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
        //                if (odds == null)
        //                {
        //                    continue;
        //                }
        //                if (!odds.CheckIsValidate())
        //                {
        //                    continue;
        //                }
        //                var entity = oddsManager.GetLastOdds<TEntity>(gameType, id, isDS);
        //                //if (entity == null || !entity.Equals(odds))
        //                //{
        //                //    entity = new TEntity
        //                //    {
        //                //        MatchId = id,
        //                //        CreateTime = DateTime.Now,
        //                //    };
        //                //    entity.SetOdds(odds);
        //                //    oddsManager.AddOdds<TEntity>(entity);
        //                //}
        //                if (entity == null)
        //                {

        //                    entity = new TEntity
        //                    {
        //                        MatchId = id,
        //                        CreateTime = DateTime.Now,
        //                    };
        //                    entity.SetOdds(odds);
        //                    oddsManager.AddOdds<TEntity>(entity);
        //                }
        //                else if (!entity.Equals(odds))
        //                {
        //                    entity.CreateTime = DateTime.Now;
        //                    entity.SetOdds(odds);
        //                    oddsManager.UpdateOdds<TEntity>(entity);
        //                }
        //            }
        //            tran.CommitTran();
        //        }
        //    }
        //竞猜足球 5个重载方法
        #region
        public void UpdateOddsList_JCZQ<TInfo, TEntity>(string gameCode, string gameType, string[] matchIdList, bool isDS)
               where TInfo : JingCaiMatchBase, I_JingCai_Odds
            where TEntity : EntityModel.CoreModel. JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<TEntity>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new TEntity
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {
                    oddsManager.DB.Rollback();
                    throw;
                }
            }
        }

        public void UpdateOddsList_JCLQ<TInfo, TEntity>(string tableName,string gameCode, string gameType, string[] matchIdList, bool isDS)
            where TInfo : JingCaiMatchBase, I_JingCai_Odds
         where TEntity : EntityModel.CoreModel.JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCaiZQ<TInfo>(tableName,gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<TEntity>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new TEntity
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {
                    oddsManager.DB.Rollback();
                    throw;
                }
            }
        }
        public void UpdateOddsList_JCZQ_SPF(string tabname,string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCaiZQ<JCLQ_SF_SPInfo>(tabname, gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_SPF>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCZQ_Odds_SPF
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCZQ_BRQSPF(string tbname, string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCaiZQ<JCZQ_BRQSPF_SPInfo>(tbname, gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BRQSPF>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCZQ_Odds_BRQSPF
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCZQ_ZJQ(string tbname, string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCaiZQ<JCZQ_ZJQ_SPInfo>(tbname, gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_ZJQ>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCZQ_Odds_ZJQ
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCZQ_BQC(string tbname, string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCaiZQ<JCZQ_BQC_SPInfo>(tbname, gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BQC>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCZQ_Odds_BQC
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCZQ_BF(string tbname, string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCaiZQ<JCZQ_BF_SPInfo>(tbname,gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BF>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCZQ_Odds_BF
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        #endregion


        public void UpdateOddsList_JCLQ_SF(string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCai<JCLQ_SF_SPInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                foreach (var id in matchIdList)
                {
                    var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                    if (odds == null)
                    {
                        continue;
                    }
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_SF>(gameType, id, isDS);
                    //if (entity == null || !entity.Equals(odds))
                    //{
                    //    entity = new TEntity
                    //    {
                    //        MatchId = id,
                    //        CreateTime = DateTime.Now,
                    //    };
                    //    entity.SetOdds(odds);
                    //    oddsManager.AddOdds<TEntity>(entity);

                    //}

                    if (entity == null)
                    {
                        entity = new T_JCLQ_Odds_SF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds(entity);
                    }
                    //if (entity == null)
                    //{

                    //    entity = new JCLQ_Odds_SF
                    //    {
                    //        MatchId = id,
                    //        CreateTime = DateTime.Now,
                    //    };
                    //    entity.SetOdds(odds);
                    //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                    //}
                    //else if (!entity.Equals(odds))
                    //{
                    //    entity.CreateTime = DateTime.Now;
                    //    entity.SetOdds(odds);
                    //    oddsManager.UpdateOdds<TEntity>(entity);
                    //}
                }
                oddsManager.DB.Commit();

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCLQ_RFSFF(string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCai<JCLQ_RFSF_SPInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_RFSF>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCLQ_Odds_RFSF
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {
                    oddsManager.DB.Rollback();
                    throw;
                }
            }
        }



        public void UpdateOddsList_JCLQ_SFC(string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCai<JCLQ_SFC_SPInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_SFC>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCLQ_Odds_SFC
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {
                    oddsManager.DB.Rollback();

                    throw;
                }
            }
        }

        public void UpdateOddsList_JCLQ_DXF(string gameCode, string gameType, string[] matchIdList, bool isDS)
        {
            var oddsList = GetOddsList_JingCai<JCLQ_DXF_SPInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            using (oddsManager.DB)
            {
                oddsManager.DB.Begin();
                try
                {

                    foreach (var id in matchIdList)
                    {
                        var odds = oddsList.FirstOrDefault(o => o.MatchId.Equals(id));
                        if (odds == null)
                        {
                            continue;
                        }
                        if (!odds.CheckIsValidate())
                        {
                            continue;
                        }
                        var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_DXF>(gameType, id, isDS);
                        //if (entity == null || !entity.Equals(odds))
                        //{
                        //    entity = new TEntity
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<TEntity>(entity);

                        //}

                        if (entity == null)
                        {
                            entity = new T_JCLQ_Odds_DXF
                            {
                                MatchId = odds.MatchId,
                                CreateTime = DateTime.Now,
                            };
                            entity.SetOdds(odds);
                            oddsManager.AddOdds(entity);
                        }
                        else if (!entity.Equals(odds))
                        {
                            entity.CreateTime = DateTime.Now;
                            entity.SetOdds(odds);
                            oddsManager.UpdateOdds(entity);
                        }
                        //if (entity == null)
                        //{

                        //    entity = new JCLQ_Odds_SF
                        //    {
                        //        MatchId = id,
                        //        CreateTime = DateTime.Now,
                        //    };
                        //    entity.SetOdds(odds);
                        //    oddsManager.AddOdds<JCLQ_Odds_SF>(entity);
                        //}
                        //else if (!entity.Equals(odds))
                        //{
                        //    entity.CreateTime = DateTime.Now;
                        //    entity.SetOdds(odds);
                        //    oddsManager.UpdateOdds<TEntity>(entity);
                        //}
                    }
                    oddsManager.DB.Commit();

                }
                catch (Exception)
                {
                    oddsManager.DB.Rollback();
                    throw;
                }
            }
        }

        private IList<T> GetOddsList_JingCai<T>(string gameCode, string gameType, string flag)
        {//T_JCLQ_Odds_SF
         //T_JCLQ_Odds_RFSF
            string name = typeof(T).Name;
            var coll = MonDB.GetCollection<T>(name);
            var documents = coll.Find<T>(Builders<T>.Filter.Empty).ToList();
            return documents;
        }
        private IList<T> GetOddsList_JingCaiZQ<T>(string tbName,string gameCode, string gameType, string flag)
        {//T_JCLQ_Odds_SF
         //T_JCLQ_Odds_RFSF
          //  string name = typeof(T).Name;
            var coll = MonDB.GetCollection<T>(tbName);
            var documents = coll.Find<T>(Builders<T>.Filter.Empty).ToList();
            return documents;
        }
        public void UpdateOddsList_JCLQ_Manual_SF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCLQ_Odds_SF>(gameCode, gameType, string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            try
            {
                oddsManager.DB.Begin();
              
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_SF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {
                        entity = new T_JCLQ_Odds_SF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds(entity);
                    }
                }
                oddsManager.DB.Commit();
            }
            catch (Exception ex)
            {
                oddsManager.DB.Rollback();
                throw;
            }

        }
        public void UpdateOddsList_JCLQ_Manual_RFSF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCLQ_Odds_RFSF>(gameCode, gameType, string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            try
            {
                oddsManager.DB.Begin();
             
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_RFSF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {
                        entity = new T_JCLQ_Odds_RFSF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds(entity);
                    }
                }
                oddsManager.DB.Commit();
            }
            catch (Exception ex)
            {
                oddsManager.DB.Rollback();
                throw;
            }

        }
        public void UpdateOddsList_JCLQ_Manual_SFC(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCLQ_Odds_SFC>(gameCode, gameType, string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            try
            {
                oddsManager. DB.Begin();
              
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_SFC>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {
                        entity = new T_JCLQ_Odds_SFC
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds(entity);
                    }
                }
                oddsManager.DB.Commit();
            }
            catch (Exception ex)
            {
                oddsManager.DB.Rollback();
                throw;
            }

        }
        public void UpdateOddsList_JCLQ_Manual_DXF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCLQ_Odds_DXF>(gameCode, gameType, string.Empty);
            var oddsManager = new JCLQ_OddsManager();
            try
            {
                oddsManager.DB.Begin();
              
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<T_JCLQ_Odds_DXF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {
                        entity = new T_JCLQ_Odds_DXF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds(entity);
                    }
                }
                oddsManager.DB.Commit();
            }
            catch (Exception ex)
            {
                oddsManager.DB.Rollback();
                throw;
            }

        }

        public List<string> RequestTicket_JCLQSingleScheme(GatewayTicketOrder_SingleScheme order, out List<string> realMatchIdArray)
        {
            var selectMatchIdArray = order.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = order.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //var codeText = File.ReadAllText(order.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(order.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, order.PlayType, order.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
            var totalMoney = codeList.Count * 2M * order.Amount;
            if (totalMoney != order.TotalMoney)
                throw new Exception(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。订单号：{2}", totalMoney, order.TotalMoney, order.OrderId));
            realMatchIdArray = order.ContainsMatchId ? matchIdList : selectMatchIdArray.ToList();

            if (!new JCZQ_OddsManager().CheckAllMatchUpdatedOdds(order.GameCode, order.GameType, realMatchIdArray.ToArray()))
            {
                throw new ArgumentException("订单中有比赛未开出赔率");
            }
            var manager = new Sports_Manager();
            if (manager.QuerySingleSchemeOrder(order.OrderId) == null)
            {
                manager.AddSingleSchemeOrder(new T_SingleScheme_Order
                {
                    AllowCodes = order.AllowCodes,
                    Amount = order.Amount,
                    FileBuffer = Encoding.UTF8.GetString(order.FileBuffer),
                    //AnteCodeFullFileName = order.AnteCodeFullFileName,
                    ContainsMatchId = order.ContainsMatchId,
                    CreateTime = DateTime.Now,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    IssuseNumber = order.IssuseNumber,
                    IsVirtualOrder = order.IsVirtualOrder,
                    OrderId = order.OrderId,
                    PlayType = order.PlayType,
                    SelectMatchId = order.SelectMatchId,
                    TotalMoney = order.TotalMoney,
                });
            }

            return codeList;
        }
    }
}
