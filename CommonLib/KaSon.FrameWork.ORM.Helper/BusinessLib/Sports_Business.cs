using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.UserHelper;
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
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using EntityModel.Communication;

namespace KaSon.FrameWork.ORM.Helper
{
    public class Sports_Business: DBbase
    {
        private static Log4Log writerLog = new Log4Log();
      
        public int CheckBettingOrderMoney(List<Sports_AnteCodeInfo> codeList, string gameCode, string gameType, string playType, int amount, decimal schemeTotalMoney, DateTime stopTime, bool isAllow = false, string userId = "")
        {
            //验证投注号码
            if (stopTime < DateTime.Now)
                throw new Exception("投注结束时间不能小于当前时间");

            //验证投注内容是否合法，或是否重复
            foreach (var item in codeList)
            {
                var oneCodeArray = item.AnteCode.Split(',');
                if (oneCodeArray.Distinct().Count() != oneCodeArray.Length)
                    throw new Exception(string.Format("投注号码{0}中包括重复的内容", item.AnteCode));
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
                                    //            throw new Exception("您好，根据您投注的内容将产生多张彩票，每张彩票金额不足50元，请您增加倍数以达到出票条件。");
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
                    throw new Exception("任9玩法选9场时不能设胆");
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
            var ticketList = BettingHelper. AnalyzeTickets(o);
            ticketList.AnalyzeOrder(order.GameCode, order.Price);
            if (ticketList.TotalMoney != order.TotalMoney)
            {
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", ticketList.TotalMoney, order.TotalMoney));
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
                var ticketTable = BettingHelper.GetNewTicketTable();
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
                    System.Data.DataRow r = ticketTable.NewRow();
                    r["Id"] = index;
                    r["GameCode"] = orderInfo.GameCode.ToUpper();
                    r["GameType"] = ticket.GameType.ToUpper();
                    r["SchemeId"] = orderInfo.OrderId;
                    r["TicketId"] = info.OrderId;
                    r["IssuseNumber"] = orderInfo.IssuseNumber;
                    r["PlayType"] = info.BetType;
                    r["BetContent"] = info.BetContent;
                    r["TicketStatus"] = 90;
                    r["Amount"] = info.Multiple;
                    r["BetUnits"] = info.BetUnits;
                    r["BetMoney"] = info.BetMoney;
                    r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                    r["IsAppend"] = orderInfo.IsAppend;
                    r["BonusStatus"] = 0;
                    r["PreTaxBonusMoney"] = 0M;
                    r["AfterTaxBonusMoney"] = 0M;
                    r["PrintDateTime"] = DateTime.Now;
                    r["CreateTime"] = DateTime.Now;
                    r["LocOdds"] = locOdds;

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
                    ticketTable.Rows.Add(r);
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

                    writerLog.WriteLog("SqlBulkAddTableError", orderInfo.OrderId, (int)LogType.Information, "SqlBulkAddTable", exp.Message + "/r/n");
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
                if (RedisHelper.EnableRedis)
                    RedisOrderBusiness.AddToRunningOrder_SZC(schemeType, orderInfo.GameCode.ToUpper(), gameType, orderInfo.OrderId, keyLine, stopAfterBonus, orderInfo.IssuseNumber, redisTicketList);

            }
            catch (Exception exp)
            {
                writerLog.WriteLog("RequestTicketError", orderInfo.OrderId, (int)LogType.Information, "保存票数据报错", exp.Message + "/r/n" + sql.ToString());
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
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SPF.GetOddsString()));
                        break;
                    case "BRQSPF":
                        var entity_BRQSPF = oddManager.GetLastOdds<T_JCZQ_Odds_BRQSPF>(gameType.ToUpper(), matchId, false);
                        if (entity_BRQSPF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BRQSPF.GetOddsString()));
                        break;
                    case "BF":
                        var entity_BF = oddManager.GetLastOdds<T_JCZQ_Odds_BF>(gameType.ToUpper(), matchId, false);
                        if (entity_BF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BF.GetOddsString()));
                        break;
                    case "ZJQ":
                        var entity_ZJQ = oddManager.GetLastOdds<T_JCZQ_Odds_ZJQ>(gameType.ToUpper(), matchId, false);
                        if (entity_ZJQ == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_ZJQ.GetOddsString()));
                        break;
                    case "BQC":
                        var entity_BQC = oddManager.GetLastOdds<T_JCZQ_Odds_BQC>(gameType.ToUpper(), matchId, false);
                        if (entity_BQC == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
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
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SF.GetOddsString()));
                        break;
                    case "RFSF":
                        var entity_RFSF = oddManager.GetLastOdds<T_JCLQ_Odds_RFSF>(gameType.ToUpper(), matchId, false);
                        if (entity_RFSF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_RFSF.GetOddsString()));
                        break;
                    case "SFC":
                        var entity_SFC = oddManager.GetLastOdds<T_JCLQ_Odds_SFC>(gameType.ToUpper(), matchId, false);
                        if (entity_SFC == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SFC.GetOddsString()));
                        break;
                    case "DXF":
                        var entity_DXF = oddManager.GetLastOdds<T_JCLQ_Odds_DXF>(gameType.ToUpper(), matchId, false);
                        if (entity_DXF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
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

            //var order = string.IsNullOrEmpty(orderInfo.Attach) ?
            //    AnalyzeOrder_Sport<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL")
            //   : AnalyzeOrder_Sport_YH<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL", orderInfo.Attach);


            //RequestTicket_Sport(order, orderInfo.IsRunningTicket, matchIdOddsList, matchIdArray);
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
            var watch = new Stopwatch();
            watch.Start();
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            if (string.IsNullOrEmpty(schemeId))
                schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);

            //var key = string.Format("{0}_{1}", info.GameCode, "StopTicketing");
            //string AppSetting = DBbase.GlobalConfig[key].ToString();
            //if (BettingHelper.CanRequestBet(AppSetting, gameCode))
            var canTicket = BettingHelper.CanRequestBet(info.GameCode);
            //开启事务
            using ( DB)
            {
                
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                     info.IssuseNumber, info.Amount, totalCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                     SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                     canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

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
                    DB.GetDal<C_Sports_AnteCode>().Add(entityAnteCode);
                    //sportsManager.AddSports_AnteCode(entityAnteCode);
                }


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

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("SportsBetting", "SQL", (int)LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #region 拆票

            watch.Restart();
            if (RedisHelper.EnableRedis)
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

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("SportsBetting", "Redis",(int) LogType.Warning, "拆票", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
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
                }
                catch (Exception EX)
                {
                    DB.Rollback();
                    throw EX;
                }
               

                #endregion

                DB.Commit();
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
                var ticketTable = BettingHelper.GetNewTicketTable();
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

                        DataRow r = ticketTable.NewRow();
                        r["Id"] = ticketNum;
                        r["GameCode"] = order.GameCode.ToUpper();
                        r["GameType"] = order.GameType.ToUpper();
                        r["SchemeId"] = order.OrderId;
                        r["TicketId"] = ticketId;
                        r["IssuseNumber"] = ConvertIssuseNumber(order.GameCode, order.IssuseNumber);
                        r["PlayType"] = string.Format("P{0}", betType);
                        r["BetContent"] = string.Join("/", locBetContentList);
                        r["TicketStatus"] = 90;
                        r["Amount"] = currentAmount;
                        r["BetUnits"] = 1;
                        r["BetMoney"] = ticketMoney;
                        r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                        r["MatchIdList"] = string.Join(",", matchIdList.ToArray());
                        r["LocOdds"] = locOddStr;
                        r["IsAppend"] = false;
                        r["BonusStatus"] = 0;
                        r["PreTaxBonusMoney"] = 0M;
                        r["AfterTaxBonusMoney"] = 0M;
                        r["PrintDateTime"] = DateTime.Now;
                        r["CreateTime"] = DateTime.Now;

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

                        ticketTable.Rows.Add(r);

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
                if (RedisHelper.EnableRedis)
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
            var ticketTable = BettingHelper.GetNewTicketTable();
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

                DataRow r = ticketTable.NewRow();
                r["Id"] = index;
                r["GameCode"] = orderInfo.GameCode.ToUpper();
                r["GameType"] = ticket.GameType.ToUpper();
                r["SchemeId"] = orderInfo.OrderId;
                r["TicketId"] = info.OrderId;
                r["IssuseNumber"] = orderInfo.IssuseNumber;
                r["PlayType"] = info.BetType;
                r["BetContent"] = info.BetContent;
                r["TicketStatus"] = 90;
                r["Amount"] = info.Multiple;
                r["BetUnits"] = info.BetUnits;
                r["BetMoney"] = info.BetMoney;
                r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                r["IsAppend"] = orderInfo.IsAppend;
                r["BonusStatus"] = 0;
                r["PreTaxBonusMoney"] = 0M;
                r["AfterTaxBonusMoney"] = 0M;
                r["PrintDateTime"] = DateTime.Now;
                r["CreateTime"] = DateTime.Now;
                r["LocOdds"] = locOdds;
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

                ticketTable.Rows.Add(r);
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
            if (RedisHelper.EnableRedis)
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
            //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));
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
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicketsWithNoThread(o.ToString());
                }
                catch (Exception ex)
                {
                    writerLog.ErrrorLog("DoSplitOrderTickets-DpSplitOrderTicketsWithNoThread", ex);
                }
            }, schemeId);

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
                ProgressStatus =(int) progressStatus,
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
                if (RedisHelper.EnableRedis)
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
            if (main == null) throw new Exception("合买订单不存在");
            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);
            if (orderDetail == null)
                throw new Exception(string.Format("查不到{0}的orderDetail 信息", schemeId));
            else if (orderDetail.IsVirtualOrder)
                throw new Exception("当前订单还未付款不能参与合买");
            BusinessHelper.CheckDisableGame(orderDetail.GameCode, orderDetail.GameType);
            if (DateTime.Now >= main.StopTime)
                throw new Exception(string.Format("合买结束时间是{0}，现在不能参与合买。", main.StopTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if (main.ProgressStatus != (int)TogetherSchemeProgress.SalesIn && main.ProgressStatus != (int)TogetherSchemeProgress.Standard) throw new Exception("合买已完成，不能参与");
            if (!string.IsNullOrEmpty(main.JoinPwd) && (string.IsNullOrEmpty(joinPwd) || Encipherment.MD5(joinPwd) != main.JoinPwd))
                throw new Exception("参与密码不正确");
            var surplusCount = main.TotalCount - main.SoldCount;
            if (surplusCount < buyCount)
                throw new Exception(string.Format("方案剩余份数不足{0}份", buyCount));

            var buyMoney = main.Price * buyCount;
            if (buyMoney < 1)
                throw new Exception("参与金额最少为1元");

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
                    throw new Exception("未查询到订单信息");
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
                if (RedisHelper.EnableRedis)
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
                throw new Exception(string.Format("查不到方案{0}的Order_Running信息", schemeId));
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
            var watch = new Stopwatch();
            watch.Start();

            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");
            info.UserId = userId;

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
                if (item.GameType == "TR9" && info.SchemeSource == SchemeSource.Android && !string.IsNullOrEmpty(item.AnteCode))//app投注任九时，去掉后面的|
                    item.AnteCode = item.AnteCode.Trim().TrimEnd('|');
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

            var lotteryManager = new LotteryGameManager();
            string ctzqGameType = string.Empty;
            if (!string.IsNullOrEmpty(info.GameCode) && info.GameCode.ToUpper() == "CTZQ")
                ctzqGameType = info.AnteCodeList[0].GameType.ToUpper();
            //var currentIssuse = BusinessHelper.QueryCurentIssuse(info.GameCode, ctzqGameType);
            //if (currentIssuse == null)
            //    throw new LogicException("订单期号不存在，请联系客服");
            //if (info.IssuseNumberList.First().IssuseNumber.CompareTo(currentIssuse.IssuseNumber) < 0)
            //    throw new LogicException("投注订单期号已过期或未开售");

            #endregion
            var gameTypes = lotteryManager.QueryEnableGameTypes();
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
                DB.Begin();
            try
            {
                var gameInfo = BusinessHelper.QueryLotteryGame(info.GameCode);
                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();
                if (string.IsNullOrEmpty(keyLine))
                    keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
                var orderIndex = 1;
                var totalBetMoney = 0M;
                foreach (var issuse in info.IssuseNumberList)
                {
                    //var IsEnableLimitBetAmount = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableLimitBetAmount").ConfigValue);
                    //if (IsEnableLimitBetAmount)//开启限制用户单倍投注
                    //{
                    //    if (issuse.Amount == 1 && totalNumberZhu > 50)
                    //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                    //    else if (issuse.Amount > 0 && issuse.IssuseTotalMoney / issuse.Amount > 100)
                    //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                    //}
                    var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                    if (currentIssuseNumber == null)
                        throw new LogicException(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                    if (!string.IsNullOrEmpty(currentIssuseNumber.WinNumber))
                        throw new LogicException("奖期已开出开奖号");
                    if (info.CurrentBetTime > currentIssuseNumber.LocalStopTime)
                        throw new LogicException(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));

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
                    lock (schemeId)
                    {
                        var anteCodeList = new List<C_Sports_AnteCode>();
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
                            sportsManager.AddSports_AnteCode(codeEntity);
                            //var gameType = lotteryManager.QueryGameType(info.GameCode, item.GameType);
                            var gameType = gameTypes.FirstOrDefault(a => a.Game.GameCode == info.GameCode && a.GameType == item.GameType.ToUpper());
                            if (gameType != null && !gameTypeList.Contains(gameType))
                            {
                                gameTypeList.Add(gameType);
                            }
                        }

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
                            sportsManager.AddLotteryScheme(new C_Lottery_Scheme
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
                        var entity = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                              string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                              info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting, orderIndex == 1, false, user.UserId, user.AgentId,
                              orderIndex == 1 ? info.CurrentBetTime : currentIssuseNumber.StartTime, info.ActivityType, "", info.IsAppend, redBagMoney,
                              (orderIndex == 1 ? (canTicket ? ProgressStatus.Running : ProgressStatus.Waitting) : ProgressStatus.Waitting),
                              (orderIndex == 1 ? (canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting) : TicketStatus.Waitting));
                        totalBetMoney += currentIssuseMoney;

                        //启用了Redis
                        if (RedisHelper.EnableRedis)
                        {
                            var runningOrder = new RedisWaitTicketOrder
                            {
                                AnteCodeList = anteCodeList,
                                RunningOrder = entity,
                                KeyLine = keyLine,
                                StopAfterBonus = info.StopAfterBonus,
                                SchemeType = info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting
                            };
                            //追号方式 存入Redis订单列表
                            redisOrderList.OrderList.Add(runningOrder);
                        }
                    }
                    orderIndex++;
                }

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
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }

                
            //}
            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("LotteryBetting", "SQL", (int)LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));


            watch.Reset();
            if (RedisHelper.EnableRedis)
            {
                if (info.IssuseNumberList.Count > 1)
                {
                    //追号
                    redisOrderList.KeyLine = keyLine;
                    redisOrderList.StopAfterBonus = info.StopAfterBonus;
                    RedisOrderBusiness.AddOrderToWaitSplitList(info.GameCode, redisOrderList);
                    //序列化订单到文件
                    SerializChaseOrder(info, keyLine);
                }
                else
                {
                    //普通投注
                    if (redisOrderList.OrderList.Count > 0)
                        RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisOrderList.OrderList[0]);
                }
            }

            //拆票
            if (!RedisHelper.EnableRedis)
                DoSplitOrderTickets(firstSchemeId);

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("LotteryBetting", "Redis", (int)LogType.Information, "投注耗时记录", string.Format("订单{0}总用时{1}毫秒", keyLine, watch.Elapsed.TotalMilliseconds));

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return keyLine;
        }

        private void CheckSJBMatch(string gameType, int matchId)
        {
            var entity = new SJBMatchManager().GetSJBMatch(gameType, matchId);
            if (entity == null)
                throw new Exception("投注场次错误");

            if (entity.BetState != "开售")
                throw new Exception(string.Format("比赛{0}停止销售", matchId));
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
                writerLog.WriteLog("LotteryBetting", "SerializChaseOrder", (int)LogType.Information, "序列化失败", ex.ToString());
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
                    throw new Exception("投注号码出错 - " + ex.Message);
                }
            }
            var codeMoney = 0M;
            info.IssuseNumberList.ForEach(item =>
            {
                if (item.Amount < 1)
                    throw new Exception("倍数不能小于1");
                var currentMoney = item.Amount * totalNumberZhu * 2M;
                if (currentMoney != item.IssuseTotalMoney)
                    throw new Exception(string.Format("第{0}期投注金额应该是{1},实际是{2}", item.IssuseNumber, currentMoney, item.IssuseTotalMoney));
                codeMoney += currentMoney;
            });
            if (codeMoney != info.TotalMoney)
                throw new Exception("投注期号总金额与方案总金额不匹配");
            var lotteryManager = new LotteryGameManager();
            var currentIssuse = lotteryManager.QueryCurrentIssuse(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty);
            if (currentIssuse == null)
                throw new Exception("订单期号不存在，请联系客服");
            if (info.IssuseNumberList.First().IssuseNumber.CompareTo(currentIssuse.IssuseNumber) < 0)
                throw new Exception("投注订单期号已过期");
            #endregion
            var gameTypes = lotteryManager.QueryEnableGameTypes();
            //using (var biz = new GameBizBusinessManagement())
            //{
                DB.Begin();
            try
            {
                var gameInfo = lotteryManager.LoadGame(info.GameCode);
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();
                keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
                if (info.IssuseNumberList.Count > 1)
                    throw new Exception("保存的订单只能投注一期");
                var orderIndex = 1;
                foreach (var issuse in info.IssuseNumberList)
                {
                    //var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.AnteCodeList[0].GameType, issuse.IssuseNumber);
                    var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                    if (currentIssuseNumber == null)
                        throw new Exception(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                    if (currentIssuseNumber.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    var schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);
                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        sportsManager.AddSports_AnteCode(new C_Sports_AnteCode
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
                        sportsManager.AddLotteryScheme(new C_Lottery_Scheme
                        {
                            OrderIndex = orderIndex,
                            KeyLine = keyLine,
                            SchemeId = schemeId,
                            CreateTime = DateTime.Now,
                            IsComplate = false,
                            IssuseNumber = issuse.IssuseNumber,
                        });
                    }
                    AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                        string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                        SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, "", info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                    sportsManager.AddUserSaveOrder(new C_UserSaveOrder
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

                    orderIndex++;
                }

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
                throw new Exception("方案拆分不正确");
            if (info.Subscription < 1)
                throw new Exception("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new Exception("发起者认购份数和保底份数不能超过总份数");
            var schemeId = string.Empty;

            var sportsManager = new Sports_Manager();
            var stopTime = CheckGeneralBettingMatch(sportsManager, info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            var betCount = 0;

            #region 计算注数
            if (info.GameCode == "BJDC" || info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
            {
                betCount = CheckBettingOrderMoney(info.AnteCodeList, info.GameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
                //if (betCount > BusinessHelper.GetMaxBetCount())
                //    throw new Exception("您好！单票注数不能大于一万注");
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
                        //if (zhu > BusinessHelper.GetMaxBetCount())
                        //    throw new Exception("您好！单票注数不能大于一万注");
                        betCount += zhu;
                        codeMoney += zhu * info.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("投注号码出错 - " + ex.Message);
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
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                //}


                //if (betCount > BusinessHelper.GetMaxBetCount())
                //    throw new Exception("您好！单票注数不能大于一万注");
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

        /// <summary>
        /// 合买保存订单；注意：合买保存订单时不做扣款,但需要记录售出份数
        /// </summary>
        private C_Sports_Together SavaOrder_AddTogetherInfo(TogetherSchemeBase info, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
            SchemeSource schemeSource, TogetherSchemeSecurity security, int totalMatchCount, DateTime stopTime, bool isUploadAnteCode,
            decimal schemeDeduct, string userId, string userAgent, string balancePassword, int sysGuarantees, bool isTop, SchemeBettingCategory category, string issuseNumber)
        {
            var canChase = false;
            stopTime = stopTime.AddMinutes(-5);

            if (DateTime.Now >= stopTime)
                throw new Exception(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

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
            //    throw new Exception("方案拆分不正确");
            if (info.Subscription < 1)
                throw new Exception("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new Exception("发起者认购份数和保底份数不能超过总份数");

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
                        throw new Exception("投注号码出错 - " + ex.Message);
                    }
                }

                if (codeMoney != info.TotalMoney)
                    throw new Exception("投注期号总金额与方案总金额不匹配");
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
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
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
                if (RedisHelper.EnableRedis)
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
                throw new Exception("附加信息不能为空");
            var codeMoney = 0M;
            var attachArray = attach.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in attachArray)
            {
                var itemArray = item.Split('^');
                if (itemArray.Length != 2) continue;
                var amount = decimal.Parse(itemArray[1]);
                if (amount <= 0)
                    throw new Exception("注数格式不正确");
                var isTrue = System.Text.RegularExpressions.Regex.IsMatch(itemArray[1].ToString(), "^([0-9]{1,})$");
                if (!isTrue)
                    throw new Exception("注数格式不正确");

                codeMoney += amount * 2;
                foreach (var oneMatch in itemArray[0].Split('|'))
                {
                    var matchArray = oneMatch.Split('_');
                    if (matchArray.Length != 3)
                        throw new Exception("投注内容不正确");
                    if (bettingCategory == SchemeBettingCategory.YouHua)//如果投注类别为奖金优化
                    {
                        //if (!new string[] { "SPF", "BRQSPF" }.Contains(matchArray[1]))
                        //    throw new Exception("奖金优化只支持胜平负玩法");
                        //if (!new string[] { "3", "1", "0" }.Contains(matchArray[2]))
                        //    throw new Exception("投注内容格式不正确");

                        if (!new string[] { "SPF", "BRQSPF", "ZJQ", "BQC", "BF" }.Contains(matchArray[1]))
                            throw new Exception("奖金优化只支持胜平负玩法");
                        if (!BettingHelper.CheckAnteCode(matchArray[1], matchArray[2]))
                            throw new Exception("投注内容格式不正确");
                    }
                }
            }
            if (codeMoney != realTotalMoney)
                throw new Exception(string.Format("优化金额不正确，应为{0}，实际为{1}", codeMoney, realTotalMoney));
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
                throw new Exception("投注内容不完整");

            schemeId = BettingHelper.GetSportsBettingSchemeId(gameCode);

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new Exception("用户已禁用");

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
                throw new Exception("投注内容不完整");
            if (info.TotalMoney % 2 != 0 || realTotalMoney % 2 != 0)
                throw new AggregateException("订单金额不正确，应该为2的倍数");

            schemeId = string.IsNullOrEmpty(info.SchemeId) ? BettingHelper.GetSportsBettingSchemeId(gameCode) : info.SchemeId;

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new Exception("用户已禁用");

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
                        throw new Exception(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
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

            if (RedisHelper.EnableRedis)
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
                DB.Begin();
            try
            {
                var sportsManager = new Sports_Manager();
                var entity = sportsManager.QueryTogetherFollowerRule(ruleId);
                if (entity == null)
                    throw new Exception("找不到相关的订制跟单");
                if (entity.FollowerUserId != info.FollowerUserId)
                    throw new Exception("跟单规则不是此用户所订制");

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
        /// 订制合买跟单
        /// </summary>
        public void CustomTogetherFollower(TogetherFollowerRuleInfo info)
        {
            //开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
                DB.Begin();

            try
            {
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
                    throw new Exception("找不到相关的订制跟单");
                if (entity.FollowerUserId != followerUserId)
                    throw new Exception("跟单规则不是此用户所订制");

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
            if (RedisHelper.EnableRedis)
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
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                //    else if (info.Amount > 0 && info.TotalMoney / info.Amount > 100)
                //        throw new Exception("对不起，暂时不支持多串过关单倍金额超过100元。");
                //}
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                if (!user.IsEnable)
                    throw new Exception("用户已禁用");
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

                throw ex;
            }
            //}

            #region 拆票

            if (RedisHelper.EnableRedis)
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
                throw new Exception("彩种编码不正确");
            if (info.IssuseNumberList == null || info.IssuseNumberList.Count != 1)
                throw new Exception("期号信息不能为空");
            if (info.AnteCodeList == null || info.AnteCodeList.Count <= 0)
                throw new Exception("投注号码不能为空");

            var lotteryManager = new LotteryGameManager();
            var totalNumberZhu = 0;
            foreach (var item in info.AnteCodeList)
            {
                try
                {
                    //检查投注内容
                    var matchList = CheckSJBMatch(item.GameType, item.AnteCode);
                    if (matchList.Where(p => p.BetState != "开售").Count() > 0)
                        throw new Exception("比赛中有包括未开售或过期的比赛");
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
                    throw new Exception("当前无奖期数据");
                if (currentIssuse.Status != (int)IssuseStatus.OnSale)
                    throw new Exception("投注已截止");
                if (currentIssuse.IssuseNumber != item.IssuseNumber)
                    throw new Exception(string.Format("当前期应该是{0}，实际是{1}", currentIssuse.IssuseNumber, item.IssuseNumber));

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
                DB.Begin();

            try
            {
                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();

                var totalBetMoney = 0M;
                var issuse = info.IssuseNumberList[0];
                if (string.IsNullOrEmpty(schemeId))
                    schemeId = BettingHelper.GetSportsBettingSchemeId(info.GameCode);
                lock (schemeId)
                {
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
                        sportsManager.AddSports_AnteCode(codeEntity);
                        var gameTypeName = item.GameType.ToUpper() == "GJ" ? "冠军" : "冠亚军";
                        if (!gameTypeList.Contains(gameTypeName))
                        {
                            gameTypeList.Add(gameTypeName);
                        }
                    }

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
                    if (RedisHelper.EnableRedis)
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

            if (RedisHelper.EnableRedis)
            {
                //普通投注
                if (redisOrderList.OrderList.Count > 0)
                    RedisOrderBusiness.AddOrderToRedis(info.GameCode, redisOrderList.OrderList[0]);
            }

            //拆票
            if (!RedisHelper.EnableRedis)
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
    }
}
