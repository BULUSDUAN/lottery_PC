using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.Ticket;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
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

        public List<string> RequestTicket_CTZQSingleScheme(GatewayTicketOrder_SingleScheme order)
        {
            //var codeText = File.ReadAllText(order.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(order.FileBuffer);
            var allowCodeArray = order.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var ctzqMatchIdList = new List<string>();
            var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, order.GameType, allowCodeArray, out ctzqMatchIdList);
            var betCount = ctzqCodeList.Count;
            var totalMatchCount = ctzqMatchIdList.Count;
            var selectMatchIdArray = ctzqMatchIdList.ToArray();
            var selectMatchId = string.Join(",", selectMatchIdArray);

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
                    SelectMatchId = selectMatchId,
                    TotalMoney = order.TotalMoney,
                });
            }

            return ctzqCodeList;
        }
        public void PrizeBJDCTicket_OrderId(string orderId)
        {


                DB.Begin();

                var manager = new Sports_Manager();
                var ticketListInfo = manager.QueryPrizeTicket_OrderIdList("BJDC", orderId);
                var ticketStrSql = string.Empty;
                foreach (var ticket in ticketListInfo.SportsTicketList)
                {
                    if (ticket.TicketStatus != (int)TicketStatus.Ticketed) continue;

                    var preTaxBonusMoney = 0M;
                    var afterTaxBonusMoney = 0M;

                    var bonusCount = 0;
                    var collection = new GatewayAnteCodeCollection_Sport();
                    var codeList = new List<Ticket_AnteCode_Running>();
                    //100_3/101_1
                    foreach (var item in ticket.BetContent.Split('/'))
                    {
                        var oneMatch = item.Split('_');
                        codeList.Add(new Ticket_AnteCode_Running
                        {
                            MatchId = oneMatch[0],
                            IssuseNumber = ticket.IssuseNumber,
                            AnteNumber = oneMatch[1],
                            IsDan = false,
                            GameType = ticket.GameType,
                        });
                        collection.Add(new GatewayAnteCode_Sport
                        {
                            AnteCode = oneMatch[1],
                            MatchId = oneMatch[0],
                            GameType = ticket.GameType,
                            IsDan = false
                        });
                    }

                    var n = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[1]);
                    if (n > 1)
                    {
                        #region M串N
                        var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                        {
                            Amount = ticket.Amount,
                            AnteCodeList = collection,
                            Attach = string.Empty,
                            GameCode = ticket.GameCode,
                            GameType = ticket.GameType,
                            IssuseNumber = ticket.IssuseNumber,
                            IsVirtualOrder = false,
                            OrderId = ticket.SchemeId,
                            PlayType = ticket.PlayType.Replace("P", ""),
                            Price = 2,
                            UserId = string.Empty,
                            TotalMoney = ticket.BetMoney
                        }, "LOCAL", "agentId");

                        foreach (var ticket_cp in orderEntity.GetTicketList())
                        {
                            var matchIdL = (from c in ticket_cp.GetAnteCodeList() select c.MatchId).ToArray();
                            var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                            var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket_cp.GameCode, ticket_cp.GameType, int.Parse(ticket_cp.PlayType.Replace("P", "").Split('_')[0]));
                            var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), ticketListInfo.MatchList.ToArray());
                            if (bonusResult.IsWin)
                            {
                                bonusCount += bonusResult.BonusCount;
                                for (var i = 0; i < bonusResult.BonusCount; i++)
                                {
                                    var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                                    var sps = GetSPs(ticket_cp.GameCode, ticket_cp.GameType, ticket_cp.IssuseNumber, matchIdList);
                                    var oneBeforeBonusMoney = 2M;
                                    var isTrue = false;
                                    var num = 0;
                                    foreach (var item in sps)
                                    {
                                        if (item.Value == 1M)
                                        {
                                            num++;
                                            var entity = codeL.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
                                            var anteCodeCount = entity.AnteCode.Split(',').Count();
                                            oneBeforeBonusMoney *= anteCodeCount;
                                            if (sps.Count == 1) isTrue = true;
                                        }
                                        else
                                        {
                                            oneBeforeBonusMoney *= item.Value;
                                        }
                                    }
                                    if (!isTrue && num != sps.Count)
                                        oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
                                    oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
                                    var oneAfterBonusMoney = oneBeforeBonusMoney;

                                    //北单奖金小于2元的 按2元计算
                                    if (oneBeforeBonusMoney < 2M)
                                    {
                                        oneBeforeBonusMoney = 2M;
                                        oneAfterBonusMoney = 2M;
                                    }

                                    if (oneBeforeBonusMoney >= 10000)
                                    {
                                        oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                                    }
                                    oneBeforeBonusMoney *= ticket_cp.Amount;
                                    oneAfterBonusMoney *= ticket_cp.Amount;

                                    preTaxBonusMoney += oneBeforeBonusMoney;
                                    afterTaxBonusMoney += oneAfterBonusMoney;
                                }
                            }
                        }

                        //单票金额上限
                        if (afterTaxBonusMoney >= 5000000M)
                            afterTaxBonusMoney = 5000000M;

                        #endregion
                    }
                    else
                    {
                        #region M串1
                        var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]));
                        var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), ticketListInfo.MatchList.ToArray());
                        if (bonusResult.IsWin)
                        {
                            bonusCount += bonusResult.BonusCount;
                            for (var i = 0; i < bonusResult.BonusCount; i++)
                            {
                                var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                                var sps = GetSPs(ticket.GameCode, ticket.GameType, ticket.IssuseNumber, matchIdList);
                                var oneBeforeBonusMoney = 2M;
                                var isTrue = false;
                                var num = 0;
                                foreach (var item in sps)
                                {
                                    if (item.Value == 1M)
                                    {
                                        num++;
                                        var entity = codeList.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
                                        var anteCodeCount = entity.AnteCode.Split(',').Count();
                                        oneBeforeBonusMoney *= anteCodeCount;
                                        if (sps.Count == 1) isTrue = true;
                                    }
                                    else
                                    {
                                        oneBeforeBonusMoney *= item.Value;
                                    }
                                }
                                if (!isTrue && num != sps.Count)
                                    oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
                                oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
                                var oneAfterBonusMoney = oneBeforeBonusMoney;

                                //北单奖金小于2元的 按2元计算
                                if (oneBeforeBonusMoney < 2M)
                                {
                                    oneBeforeBonusMoney = 2M;
                                    oneAfterBonusMoney = 2M;
                                }

                                if (oneBeforeBonusMoney >= 10000)
                                {
                                    oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                                }
                                oneBeforeBonusMoney *= ticket.Amount;
                                oneAfterBonusMoney *= ticket.Amount;

                                //单票金额上限
                                if (oneAfterBonusMoney >= 5000000M)
                                    oneAfterBonusMoney = 5000000M;

                                preTaxBonusMoney += oneBeforeBonusMoney;
                                afterTaxBonusMoney += oneAfterBonusMoney;
                            }
                        }

                        #endregion
                    }

                    ticketStrSql += string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
                                , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
                }
                manager.ExecSql(ticketStrSql);

            DB.Commit();
            
        }

        private Dictionary<string, decimal> GetSPs(string gameCode, string gameType, string issuseNumber, string[] matchIdList)
        {
            var result = new Dictionary<string, decimal>();
            foreach (var id in matchIdList)
            {
                result.Add(id, ReturnSp(gameCode, gameType, issuseNumber, int.Parse(id)));
            }
            return result;
        }

        public decimal ReturnSp(string gameCode, string gameType, string issuseNumber, int id)
        {
            if (gameType.ToUpper().Equals("SF"))
            {
                var entity = new SFGGMatchManager().QuerySFGGMatch(issuseNumber, id);
                if (entity.MatchState != 2 || (entity.SF_Result != "-1" && entity.SF_SP == 1M))
                    throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                switch (gameType)
                {
                    case "SF":
                        return entity.SF_SP;
                    default:
                        throw new ArgumentException("获取比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                }
            }
            else
            {
                var entity = new BJDCMatchManager().QueryBJDC_MatchResult(issuseNumber, id);
                switch (gameType)
                {
                    case "SPF":
                        if (entity.MatchState != "2" || (entity.SPF_Result != "-1" && entity.SPF_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        return entity.SPF_SP;
                    case "ZJQ":
                        if (entity.MatchState != "2" || (entity.ZJQ_Result != "-1" && entity.ZJQ_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        return entity.ZJQ_SP;
                    case "SXDS":
                        if (entity.MatchState != "2" || (entity.SXDS_Result != "-1" && entity.SXDS_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        return entity.SXDS_SP;
                    case "BF":
                        if (entity.MatchState != "2" || (entity.BF_Result != "-1" && entity.BF_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        return entity.BF_SP;
                    case "BQC":
                        if (entity.MatchState != "2" || (entity.BQC_Result != "-1" && entity.BQC_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        return entity.BQC_SP;
                    default:
                        throw new ArgumentException("获取比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                }
            }
        }

        public void PrizeJCZQTicket_OrderId(string orderId)
        {

               DB.Begin();

                var manager = new Sports_Manager();
                var collection = manager.QueryPrizeTicket_OrderIdList("JCZQ", orderId);
                var ticketStrSql = string.Empty;
                foreach (var ticket in collection.TicketList)
                {
                    if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

                    var preTaxBonusMoney = 0M;
                    var afterTaxBonusMoney = 0M;
                    var bonusCount = 0;

                    ComputeJCZQTicketBonus(ticket.SchemeId, ticket.GameCode, ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, collection.MatchList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);

                    ticketStrSql += string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
                                , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
                }
                manager.ExecSql(ticketStrSql);

                DB.Commit();
            
        }

        private void ComputeJCZQTicketBonus(string orderId, string gameCode, string gameType, string betType, string locBetContent, string locOdds, int betAmount, IList<EntityModel.CoreModel.MatchInfo> matchResultList, decimal betMoney,
      out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        {
            bonusCount = 0;
            preTaxBonusMoney = 0M;
            afterTaxBonusMoney = 0M;
            #region odd 组合生成
            //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
            var locOddsL = locOdds.Split('/');
            var codeList = new List<Ticket_AnteCode_Running>();
            var collection = new GatewayAnteCodeCollection_Sport();
            var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
            foreach (var item in locBetContent.Split('/'))
            {
                var oneMatch = item.Split('_');
                if (gameType == "HH")
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[1],
                        AnteNumber = oneMatch[2],
                        IsDan = false,
                        GameType = oneMatch[0],
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[2],
                        MatchId = oneMatch[1],
                        GameType = oneMatch[0],
                        IsDan = false
                    });
                }
                else
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[0],
                        AnteNumber = oneMatch[1],
                        IsDan = false,
                        GameType = gameType,
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[1],
                        MatchId = oneMatch[0],
                        GameType = gameType,
                        IsDan = false
                    });
                }
            }
            #endregion

            var n = int.Parse(betType.Replace("P", "").Split('_')[1]);
            if (n > 1)
            {
                #region M串N
                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                {
                    Amount = betAmount,
                    AnteCodeList = collection,
                    Attach = string.Empty,
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = string.Empty,
                    IsVirtualOrder = false,
                    OrderId = orderId,
                    PlayType = betType.Replace("P", ""),
                    Price = 2,
                    UserId = string.Empty,
                    TotalMoney = betMoney
                }, "LOCAL", "agentId");

                foreach (var ticket in orderEntity.GetTicketList())
                {
                    var matchIdL = (from c in ticket.GetAnteCodeList() select c.MatchId).ToArray();
                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                    var baseCount = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]);
                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, baseCount);
                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), matchResultList.ToArray());
                    if (bonusResult.IsWin)
                    {
                        bonusCount = bonusResult.BonusCount;
                        for (var i = 0; i < bonusResult.BonusCount; i++)
                        {
                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                            var tmpAnteList = codeL.Where(a => matchIdList.Contains(a.MatchId));
                            var oneBeforeBonusMoney = 2M;
                            foreach (var item in tmpAnteList)
                            {
                                var odds = 1M;
                                var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                                var matchResult = result.GetMatchResult(gameCode, item.GameType);
                                odds = item.GetResultOdds(matchResult);

                                var anteCodeCount = item.AnteCode.Split(',').Count();
                                //if (baseCount == 1 && matchResult == "-1")
                                //{
                                //    preTaxBonusMoney += betMoney;
                                //    afterTaxBonusMoney += betMoney;
                                //    return;
                                //}
                                //else
                                if (anteCodeCount > 1 && matchResult == "-1")
                                {
                                    oneBeforeBonusMoney *= anteCodeCount;
                                }
                                else
                                {
                                    oneBeforeBonusMoney *= odds;
                                }
                            }
                            oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                            var oneAfterBonusMoney = oneBeforeBonusMoney;
                            if (oneBeforeBonusMoney >= 10000)
                            {
                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                            }

                            oneBeforeBonusMoney *= ticket.Amount;
                            oneAfterBonusMoney *= ticket.Amount;

                            preTaxBonusMoney += oneBeforeBonusMoney;
                            afterTaxBonusMoney += oneAfterBonusMoney;
                        }
                    }
                }

                //单票金额上限
                var m = int.Parse(betType.Replace("P", "").Split('_')[0]);
                if ((m == 2 || m == 3) && afterTaxBonusMoney >= 200000M)
                    afterTaxBonusMoney = 200000M;
                if ((m == 4 || m == 5) && afterTaxBonusMoney >= 500000M)
                    afterTaxBonusMoney = 500000M;
                if (m >= 6 && afterTaxBonusMoney >= 1000000M)
                    afterTaxBonusMoney = 1000000M;

                #endregion
            }
            else
            {
                #region M串1
                var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
                var record = string.Empty;
                if (bonusResult.IsWin)
                {
                    bonusCount = bonusResult.BonusCount;
                    for (var i = 0; i < bonusResult.BonusCount; i++)
                    {
                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                        var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
                        var oneBeforeBonusMoney = 2M;
                        foreach (var item in tmpAnteList)
                        {
                            var odds = 1M;

                            var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                            var matchResult = result.GetMatchResult(gameCode, item.GameType);
                            odds = item.GetResultOdds(matchResult);

                            var anteCodeCount = item.AnteCode.Split(',').Count();
                            if (baseCount == 1 && matchResult == "-1")
                            {
                                preTaxBonusMoney += betMoney;
                                afterTaxBonusMoney += betMoney;
                                return;
                            }
                            else if (anteCodeCount > 1 && matchResult == "-1")
                            {
                                oneBeforeBonusMoney *= anteCodeCount;
                            }
                            else
                            {
                                oneBeforeBonusMoney *= odds;
                            }
                        }
                        oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                        var oneAfterBonusMoney = oneBeforeBonusMoney;
                        if (oneBeforeBonusMoney >= 10000)
                        {
                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                        }
                        oneBeforeBonusMoney *= betAmount;
                        oneAfterBonusMoney *= betAmount;

                        //单票金额上限
                        if ((baseCount == 2 || baseCount == 3) && oneAfterBonusMoney >= 200000M)
                            oneAfterBonusMoney = 200000M;
                        if ((baseCount == 4 || baseCount == 5) && oneAfterBonusMoney >= 500000M)
                            oneAfterBonusMoney = 500000M;
                        if (baseCount >= 6 && oneAfterBonusMoney >= 1000000M)
                            oneAfterBonusMoney = 1000000M;

                        preTaxBonusMoney += oneBeforeBonusMoney;
                        afterTaxBonusMoney += oneAfterBonusMoney;
                    }
                }

                #endregion
            }
        }

        public void PrizeJCLQTicket_OrderId(string orderId)
        {

            DB.Begin();

                var manager = new Sports_Manager();
                var collection = manager.QueryPrizeTicket_OrderIdList("JCLQ", orderId);
                var ticketStrSql = string.Empty;
                foreach (var ticket in collection.TicketList)
                {
                    if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

                    var preTaxBonusMoney = 0M;
                    var afterTaxBonusMoney = 0M;
                    var bonusCount = 0;

                    ComputeJCLQTicketBonus(ticket.SchemeId, ticket.GameCode, ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, collection.MatchList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);

                    ticketStrSql += string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
                                , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
                }
                manager.ExecSql(ticketStrSql);

            DB.Commit();
            
        }

        private void ComputeJCLQTicketBonus(string orderId, string gameCode, string gameType, string betType, string locBetCount, string locOdds, int betAmount, IList<EntityModel.CoreModel.MatchInfo> matchResultList, decimal betMoney,
        out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        {
            bonusCount = 0;
            preTaxBonusMoney = 0M;
            afterTaxBonusMoney = 0M;
            //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
            var locOddsL = locOdds.Split('/');
            var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
            //查询JCLQ_AnteCode_Running表
            //140508010_1/140508011_1
            var codeList = new List<Ticket_AnteCode_Running>();
            var collection = new GatewayAnteCodeCollection_Sport();
            foreach (var item in locBetCount.Split('/'))
            {
                var oneMatch = item.Split('_');
                if (gameType == "HH")
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[1],
                        AnteNumber = oneMatch[2],
                        IsDan = false,
                        GameType = oneMatch[0],
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[2],
                        MatchId = oneMatch[1],
                        GameType = oneMatch[0],
                        IsDan = false
                    });
                }
                else
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[0],
                        AnteNumber = oneMatch[1],
                        IsDan = false,
                        GameType = gameType,
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[1],
                        MatchId = oneMatch[0],
                        GameType = gameType,
                        IsDan = false
                    });
                }
            }
            var n = int.Parse(betType.Replace("P", "").Split('_')[1]);
            if (n > 1)
            {
                #region M串N
                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                {
                    Amount = betAmount,
                    AnteCodeList = collection,
                    Attach = string.Empty,
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = string.Empty,
                    IsVirtualOrder = false,
                    OrderId = orderId,
                    PlayType = betType.Replace("P", ""),
                    Price = 2,
                    UserId = string.Empty,
                    TotalMoney = betMoney
                }, "LOCAL", "agentId");

                foreach (var ticket in orderEntity.GetTicketList())
                {
                    var matchIdL = (from c in ticket.GetAnteCodeList() select c.MatchId).ToArray();
                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                    var baseCount = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]);
                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, baseCount);
                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), matchResultList.ToArray());
                    if (bonusResult.IsWin)
                    {
                        bonusCount = bonusResult.BonusCount;
                        for (var i = 0; i < bonusResult.BonusCount; i++)
                        {
                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                            var tmpAnteList = codeL.Where(a => matchIdList.Contains(a.MatchId));
                            var oneBeforeBonusMoney = 2M;
                            foreach (var item in tmpAnteList)
                            {
                                var odds = 1M;

                                var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                                var offset = -1M;
                                switch (item.GameType.ToUpper())
                                {
                                    case "RFSF":
                                        offset = item.GetResultOdds("RF");
                                        break;
                                    case "DXF":
                                        offset = item.GetResultOdds("YSZF");
                                        break;
                                }
                                var matchResult = result.GetMatchResult(gameCode, item.GameType, offset);
                                odds = item.GetResultOdds(matchResult);
                                //}
                                var anteCodeCount = item.AnteCode.Split(',').Count();
                                //if (baseCount == 1 && matchResult == "-1")
                                //{
                                //    preTaxBonusMoney += betMoney;
                                //    afterTaxBonusMoney += betMoney;
                                //    return;
                                //}
                                if (anteCodeCount > 1 && matchResult == "-1")
                                {
                                    oneBeforeBonusMoney *= anteCodeCount;
                                }
                                else
                                {
                                    oneBeforeBonusMoney *= odds;
                                }
                            }
                            oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                            var oneAfterBonusMoney = oneBeforeBonusMoney;
                            if (oneBeforeBonusMoney >= 10000)
                            {
                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                            }
                            oneBeforeBonusMoney *= ticket.Amount;
                            oneAfterBonusMoney *= ticket.Amount;

                            preTaxBonusMoney += oneBeforeBonusMoney;
                            afterTaxBonusMoney += oneAfterBonusMoney;
                        }
                    }
                }

                //单票金额上限
                var m = int.Parse(betType.Replace("P", "").Split('_')[0]);
                if (m == 1 && afterTaxBonusMoney >= 100000M)
                    afterTaxBonusMoney = 100000M;
                if ((m == 2 || m == 3) && afterTaxBonusMoney >= 200000M)
                    afterTaxBonusMoney = 200000M;
                if ((m == 4 || m == 5) && afterTaxBonusMoney >= 500000M)
                    afterTaxBonusMoney = 500000M;
                if (m >= 6 && afterTaxBonusMoney >= 1000000M)
                    afterTaxBonusMoney = 1000000M;

                #endregion
            }
            else
            {
                #region M串1
                var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
                if (bonusResult.IsWin)
                {
                    bonusCount = bonusResult.BonusCount;
                    for (var i = 0; i < bonusResult.BonusCount; i++)
                    {
                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                        var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
                        var oneBeforeBonusMoney = 2M;
                        foreach (var item in tmpAnteList)
                        {
                            var odds = 1M;

                            var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                            var offset = -1M;
                            switch (item.GameType.ToUpper())
                            {
                                case "RFSF":
                                    offset = item.GetResultOdds("RF");
                                    break;
                                case "DXF":
                                    offset = item.GetResultOdds("YSZF");
                                    break;
                            }
                            var matchResult = result.GetMatchResult(gameCode, item.GameType, offset);
                            odds = item.GetResultOdds(matchResult);
                            //}
                            var anteCodeCount = item.AnteCode.Split(',').Count();
                            if (baseCount == 1 && matchResult == "-1")
                            {
                                preTaxBonusMoney += betMoney;
                                afterTaxBonusMoney += betMoney;
                                return;
                            }
                            else if (anteCodeCount > 1 && matchResult == "-1")
                            {
                                oneBeforeBonusMoney *= anteCodeCount;
                            }
                            else
                            {
                                oneBeforeBonusMoney *= odds;
                            }
                        }
                        oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                        var oneAfterBonusMoney = oneBeforeBonusMoney;
                        if (oneBeforeBonusMoney >= 10000)
                        {
                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                        }
                        oneBeforeBonusMoney *= betAmount;
                        oneAfterBonusMoney *= betAmount;

                        //单票金额上限
                        if (baseCount == 1 && oneAfterBonusMoney >= 100000M)
                            oneAfterBonusMoney = 100000M;
                        if ((baseCount == 2 || baseCount == 3) && oneAfterBonusMoney >= 200000M)
                            oneAfterBonusMoney = 200000M;
                        if ((baseCount == 4 || baseCount == 5) && oneAfterBonusMoney >= 500000M)
                            oneAfterBonusMoney = 500000M;
                        if (baseCount >= 6 && oneAfterBonusMoney >= 1000000M)
                            oneAfterBonusMoney = 1000000M;

                        preTaxBonusMoney += oneBeforeBonusMoney;
                        afterTaxBonusMoney += oneAfterBonusMoney;
                    }


                }
                #endregion
            }
        }
    }
}
