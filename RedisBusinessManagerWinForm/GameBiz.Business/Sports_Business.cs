using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using Common.Lottery;
using Common.Expansion;
using GameBiz.Domain.Entities;
using Common;
using GameBiz.Auth.Domain.Managers;
using Common.Cryptography;
using System.Configuration;
using Common.Communication;
using System.IO;
using System.Dynamic;
using Common.XmlAnalyzer;
using Common.Net.SMS;
using Common.Algorithms;
using Common.Utilities;
using Common.Net;
using GameBiz.Business.Domain.Managers;
using Common.Log;
using System.Threading;
using Common.JSON;
using GameBiz.Core.Ticket;
using GameBiz.Business.Domain.Entities.Ticket;
using Common.Lottery.Gateway.ZhongMin;
using GameBiz.Business.Domain.Managers.Ticket;
using Common.Lottery.Objects;
using System.Diagnostics;
using System.Data;
using Common.Lottery.Redis;


namespace GameBiz.Business
{
    public class Sports_Business
    {
        private ILogWriter writer = Common.Log.LogWriterGetter.GetLogWriter();

        #region 足彩普通订单处理

        /// <summary>
        /// 足彩普通投注
        /// </summary>
        public string SportsBetting(Sports_BetingInfo info, string userId, string password, string place, int totalCount, DateTime stopTime, decimal redBagMoney)
        {
            string schemeId = info.SchemeId;
            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;
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
                schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
            var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                     info.IssuseNumber, info.Amount, totalCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                     SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                     canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    var entityAnteCode = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                    sportsManager.AddSports_AnteCode(entityAnteCode);
                }


                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BusinessHelper.FormatGameCode(info.GameCode));
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

                biz.CommitTran();
            }

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writer.Write("SportsBetting", "SQL", LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

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
                writer.Write("SportsBetting", "Redis", LogType.Warning, "拆票", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
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
            schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);
            var sportsManager = new Sports_Manager();
            //验证比赛是否还可以投注
            var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            // 检查订单金额是否匹配
            var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                    info.SchemeSource, info.Security, SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                //用户的订单保存
                sportsManager.AddUserSaveOrder(new UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = SchemeType.SaveScheme,
                    SchemeSource = info.SchemeSource,
                    SchemeBettingCategory = info.BettingCategory,
                    ProgressStatus = ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                });

                biz.CommitTran();
            }
            return schemeId;
        }

        /// <summary>
        ///购买用户保存订单
        /// </summary>
        public void BettingUserSavedOrder(string schemeId, string userId, string balancePassword, decimal redBagMoney)
        {
            var canChase = true;
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new Exception("当前订单不存在");
            if (order.CanChase)
                throw new Exception("当前订单已付款，请勿重复操作");
            var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            //runningOrder = order;
            //删除保存信息
            var saveOrder = sportsManager.QuerySaveOrder(schemeId);
            if (saveOrder == null)
                throw new Exception("订单不存在或已投注");
            if (saveOrder.UserId != userId)
                throw new Exception("订单不属于该用户");
            if (saveOrder.ProgressStatus != ProgressStatus.Waitting)
                throw new Exception("订单订单已作废");
            if (DateTime.Now > saveOrder.StopTime)
                throw new Exception("订单投注已过期");
            var schemeManager = new SchemeManager();
            var orderDetail = schemeManager.QueryOrderDetail(schemeId);
            //decimal percent = 0;
            if (redBagMoney > 0M)
            {
                var fundManager = new FundManager();
                decimal percent = fundManager.QueryRedBagUseConfig(order.GameCode);
                var maxUseMoney = order.TotalMoney * percent / 100;
                if (redBagMoney > maxUseMoney)
                    throw new Exception(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
            }
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                //biz.CommitTran();               
                //删除保存信息
                sportsManager.DeleteUserSaveOrder_Sports(saveOrder);

                var isHM = false;
                var isHMRenGou = false;
                if (schemeId.StartsWith("TSM"))//处理合买订单
                {
                    isHM = true;
                    BettingSavedCreateTogether(schemeId, balancePassword, out canChase, out isHMRenGou);
                }
                else
                    isHMRenGou = true;

                //更新running表
                order.IsVirtualOrder = isHMRenGou ? false : true;
                order.CanChase = canChase;
                order.SchemeType = isHM ? SchemeType.TogetherBetting : SchemeType.GeneralBetting;
                order.RedBagMoney = redBagMoney;
                order.TicketLog += "|" + DateTime.Now + "用户购买保存订单|";
                sportsManager.UpdateSports_Order_Running(order);

                orderDetail.SchemeType = isHM ? SchemeType.TogetherBetting : SchemeType.GeneralBetting;
                orderDetail.IsVirtualOrder = isHMRenGou ? false : true;
                orderDetail.RedBagMoney = redBagMoney;
                schemeManager.UpdateOrderDetail(orderDetail);


                if (!isHM)
                {
                    // 消费资金
                    var noIssuseGameCode = new string[] { "JCZQ", "JCLQ" };
                    string msg = noIssuseGameCode.Contains(order.GameCode) ? string.Format("{0} 投注", BusinessHelper.FormatGameCode(order.GameCode))
                                                        : string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(order.GameCode), order.IssuseNumber);
                    if (redBagMoney > 0M)
                    {
                        //红包支付
                        BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, msg, "Bet", balancePassword);
                    }
                    BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, order.TotalMoney - redBagMoney, msg, "Bet", balancePassword);
                }
                biz.CommitTran();
            }
            #region 拆票
            if (RedisHelper.EnableRedis)
            {
                var redisWaitOrder = new RedisWaitTicketOrder
                {
                    AnteCodeList = anteCodeList,
                    RunningOrder = order
                };
                RedisOrderBusiness.AddOrderToRedis(order.GameCode, redisWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(redisWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }
            #endregion
            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);
        }

        /// <summary>
        /// 查询足彩方案信息
        /// </summary>
        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            var manager = new SchemeManager();
            var sportsManager = new Sports_Manager();
            var orderDetail = manager.QueryOrderDetail(schemeId);
            if (orderDetail == null) return null;
            var info = (orderDetail.ProgressStatus == ProgressStatus.Complate
                || orderDetail.ProgressStatus == ProgressStatus.Aborted
                || orderDetail.ProgressStatus == ProgressStatus.AutoStop) ? sportsManager.QuerySports_Order_ComplateInfo(schemeId) : sportsManager.QuerySports_Order_RunningInfo(schemeId);
            if (info == null)
                throw new Exception(string.Format("没有查询到方案{0}的信息", schemeId));
            return info;
        }
        /// <summary>
        /// 合买中奖分配
        /// </summary>
        public Sports_ComplateInfo QuerSportsComplateInfo(string schemeId)
        {
            if (schemeId.IndexOf("TSM") == -1) return new Sports_ComplateInfo();
            var manager = new SchemeManager();
            var sportsManager = new Sports_Manager();

            Sports_ComplateInfo sports_ComplateInfo = new Sports_ComplateInfo();

            var info = sportsManager.QuerySports_Order_ComplateInfo(schemeId);
            if (info == null)
                return new Sports_ComplateInfo();

            //中奖金额 + 加奖金额
            var bonusMoney = info.AfterTaxBonusMoney + info.AddMoney;
            var game = new LotteryGameManager().LoadGame(info.GameCode);
            var userManager = new UserManager();
            if (info.SchemeType == SchemeType.TogetherBetting)
            {
                var main = sportsManager.QuerySports_Together(schemeId);
                //加奖金额平均分配
                bonusMoney = info.DistributionWay == AddMoneyDistributionWay.Average ? info.AfterTaxBonusMoney + info.AddMoney : info.AfterTaxBonusMoney;

                //提成
                var deductMoney = 0M;
                if (bonusMoney > main.TotalMoney)
                    deductMoney = (bonusMoney - main.TotalMoney) * main.BonusDeduct / 100;
                if (deductMoney > 0M)
                {
                    sports_ComplateInfo.UserId = info.UserId;
                    sports_ComplateInfo.PromoterCommission = deductMoney;
                    sports_ComplateInfo.Allocation = AddMoneyDistributionWay.Average;
                    sports_ComplateInfo.AwardMoney = info.AddMoney;
                    sports_ComplateInfo.TotalMoney = bonusMoney - deductMoney;
                    foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                    {
                        if (item.JoinType == TogetherJoinType.SystemGuarantees) continue;
                        Sports_ComplateInfoList ComplateInfo = new Sports_ComplateInfoList();
                        ComplateInfo.UserId = item.JoinUserId;
                        ComplateInfo.BuyMoney = item.TotalMoney;
                        ComplateInfo.WinMoney = (bonusMoney - deductMoney) / main.TotalCount * item.RealBuyCount;
                        ComplateInfo.AddMoney = info.AddMoney / main.TotalCount * item.RealBuyCount;
                        sports_ComplateInfo.SingleDetailList.Add(ComplateInfo);
                    }
                    return sports_ComplateInfo;
                }
                //加奖金额分配给发起者
                if (info.DistributionWay == AddMoneyDistributionWay.CreaterOnly)
                {
                    if (info.AddMoney > 0)
                    {
                        sports_ComplateInfo.PromoterCommission = deductMoney;
                        sports_ComplateInfo.Allocation = AddMoneyDistributionWay.CreaterOnly;
                        sports_ComplateInfo.AwardMoney = info.AddMoney;
                        sports_ComplateInfo.TotalMoney = info.AfterTaxBonusMoney;
                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            if (item.JoinType == TogetherJoinType.SystemGuarantees) continue;
                            Sports_ComplateInfoList ComplateInfo = new Sports_ComplateInfoList();
                            ComplateInfo.UserId = item.JoinUserId;
                            ComplateInfo.BuyMoney = item.TotalMoney;
                            ComplateInfo.WinMoney = (bonusMoney - deductMoney) / main.TotalCount * item.RealBuyCount;
                            ComplateInfo.AddMoney = 0;
                            sports_ComplateInfo.SingleDetailList.Add(ComplateInfo);
                        }
                        return sports_ComplateInfo;
                    }
                }

                //加奖金额分配给参与者
                var singleMoney = info.DistributionWay == AddMoneyDistributionWay.JoinerOnly ?
                    (bonusMoney + info.AddMoney - deductMoney) / main.TotalCount :
                    (bonusMoney - deductMoney) / main.TotalCount;

                sports_ComplateInfo.PromoterCommission = deductMoney;
                sports_ComplateInfo.Allocation = AddMoneyDistributionWay.JoinerOnly;
                sports_ComplateInfo.AwardMoney = info.AddMoney;
                sports_ComplateInfo.TotalMoney = bonusMoney + info.AddMoney - deductMoney;
                foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                {
                    if (item.JoinType == TogetherJoinType.SystemGuarantees) continue;
                    Sports_ComplateInfoList ComplateInfo = new Sports_ComplateInfoList();
                    ComplateInfo.UserId = item.JoinUserId;
                    ComplateInfo.BuyMoney = item.TotalMoney;
                    ComplateInfo.WinMoney = (bonusMoney - deductMoney) / main.TotalCount * item.RealBuyCount;
                    ComplateInfo.AddMoney = info.AddMoney / main.TotalCount * item.RealBuyCount;
                    sports_ComplateInfo.SingleDetailList.Add(ComplateInfo);
                }
                return sports_ComplateInfo;
            }
            return sports_ComplateInfo;
        }

        /// <summary>
        /// 查询投注号码信息
        /// </summary>
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId)
        {
            var result = new Sports_AnteCodeQueryInfoCollection();
            var sportsManager = new Sports_Manager();
            var sjbManager = new SJBMatchManager();
            var lotteryManager = new LotteryGameManager();
            var issuseList = new List<GameIssuse>();
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
                        BonusStatus = item.BonusStatus,
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
                        BonusStatus = item.BonusStatus,
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
                        BonusStatus = item.BonusStatus,
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
                        BonusStatus = item.BonusStatus,
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
                        BonusStatus = item.BonusStatus,
                        GameType = item.GameType,
                        StartTime = DateTime.Now,
                    });
                    continue;
                }

                var c = issuseList.FirstOrDefault(p => p.GameCode == item.GameCode && p.IssuseNumber == item.IssuseNumber);
                if (c == null)
                {
                    c = lotteryManager.QueryGameIssuse(item.GameCode, item.IssuseNumber);
                    issuseList.Add(c);
                }
                result.Add(new Sports_AnteCodeQueryInfo
                {
                    AnteCode = item.AnteCode,
                    IssuseNumber = item.IssuseNumber,
                    BonusStatus = item.BonusStatus,
                    CurrentSp = item.Odds,
                    IsDan = item.IsDan,
                    GameType = item.GameType,
                    WinNumber = c == null ? string.Empty : string.IsNullOrEmpty(c.WinNumber) ? string.Empty : c.WinNumber,
                    StartTime = DateTime.Now,
                });
            }
            return result;
        }

        /// <summary>
        /// 查询等待追号的足彩订单
        /// </summary>
        public string QuerySportsWaitForChaseSchemeIdArray(string gameCode, string issuseNumber, int returnCount)
        {
            return string.Join(",", new Sports_Manager().QuerySports_Order_Running_SchemeId_List(gameCode, issuseNumber, returnCount));
        }

        /// <summary>
        /// 足彩追号
        /// </summary>
        public bool SportsChase(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new Exception(string.Format("查不到方案{0}的Order_Running信息", schemeId));
            if (order.TicketStatus != TicketStatus.Waitting)
                throw new LogicException(string.Format("订单{0}出票状态应是TicketStatus.Waitting,实际是{1}", schemeId, order.TicketStatus));

            #region 发送站内消息：手机短信或站内信

            var userManager = new UserBalanceManager();
            var user = userManager.QueryUserRegister(order.UserId);
            //当订单为追号订单时
            if (order.SchemeType == SchemeType.ChaseBetting)
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

            var ticketStatus = TicketStatus.Waitting;
            var progressStatus = ProgressStatus.Waitting;

            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 修改订单相关信息

                order.TicketStatus = ticketStatus;
                order.ProgressStatus = progressStatus;
                order.TicketId = ticketId;
                order.TicketProgress = 0;
                order.TicketLog = ticketLog;
                order.BetTime = DateTime.Now;
                sportsManager.UpdateSports_Order_Running(order);

                orderDetail.ProgressStatus = progressStatus;
                orderDetail.CurrentBettingMoney = order.IsVirtualOrder ? 0M : (ticketStatus == TicketStatus.Ticketing ? order.TotalMoney : 0M);
                orderDetail.CurrentIssuseNumber = order.IssuseNumber;
                orderDetail.TicketStatus = ticketStatus;
                orderDetail.BetTime = DateTime.Now;
                manager.UpdateOrderDetail(orderDetail);

                #endregion

                #region 请求出票失败后，移动订单数据

                if (ticketStatus == TicketStatus.Error)
                {
                    //移动订单数据
                    OrderFailToEnd(schemeId, sportsManager, order);
                }

                #endregion

                #region 请求出票失败后，退还投注资金

                if (!order.IsVirtualOrder && ticketStatus == TicketStatus.Error)
                {
                    // 返还资金
                    if (order.SchemeType == SchemeType.GeneralBetting)
                    {
                        if (order.TotalMoney > 0)
                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, schemeId, order.TotalMoney
                                , string.Format("{0} 出票失败，返还资金￥{1:N2}。 ", BusinessHelper.FormatGameCode(order.GameCode), order.TotalMoney));
                    }
                    if (order.SchemeType == SchemeType.ChaseBetting)
                    {
                        var chaseOrder = sportsManager.QueryLotteryScheme(order.SchemeId);
                        if (chaseOrder != null)
                        {
                            if (order.TotalMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, chaseOrder.KeyLine, order.TotalMoney
                                , string.Format("订单{0} 出票失败，返还资金￥{1:N2}。 ", order.SchemeId, order.TotalMoney));
                        }
                    }
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        //失败
                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            item.JoinSucess = false;
                            item.JoinLog += "出票失败";
                            sportsManager.UpdateSports_TogetherJoin(item);

                            if (item.JoinType == TogetherJoinType.SystemGuarantees)
                                continue;

                            var t = string.Empty;
                            var realBuyCount = item.RealBuyCount;
                            switch (item.JoinType)
                            {
                                case TogetherJoinType.Subscription:
                                    t = "认购";
                                    break;
                                case TogetherJoinType.FollowerJoin:
                                    t = "订制跟单";
                                    break;
                                case TogetherJoinType.Join:
                                    t = "参与";
                                    break;
                                case TogetherJoinType.Guarantees:
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

                if (ticketStatus == TicketStatus.Error)
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

                biz.CommitTran();
            }

            if (ticketStatus == TicketStatus.Error)
                return false;
            return true;

        }

        /// <summary>
        /// 订单失败，处理订单数据
        /// </summary>
        private void OrderFailToEnd(string schemeId, Sports_Manager sportsManager, Sports_Order_Running order)
        {
            var complateOrder = new Sports_Order_Complate
            {
                SchemeId = order.SchemeId,
                GameCode = order.GameCode,
                GameType = order.GameType,
                PlayType = order.PlayType,
                IssuseNumber = order.IssuseNumber,
                TotalMoney = order.TotalMoney,
                Amount = order.Amount,
                TotalMatchCount = order.TotalMatchCount,
                TicketStatus = Core.TicketStatus.Error,// order.TicketStatus,
                BonusStatus = order.BonusStatus,
                AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                CanChase = order.CanChase,
                IsVirtualOrder = order.IsVirtualOrder,
                CreateTime = order.CreateTime,
                PreTaxBonusMoney = order.PreTaxBonusMoney,
                ProgressStatus = Core.ProgressStatus.Aborted, // order.ProgressStatus,
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
                DistributionWay = AddMoneyDistributionWay.Average,
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
                TicketTime = order.TicketTime,
                RedBagMoney = order.RedBagMoney,
                IsPayRebate = order.IsPayRebate,
                MaxBonusMoney = order.MaxBonusMoney,
                MinBonusMoney = order.MinBonusMoney,
                QueryTicketStopTime = order.QueryTicketStopTime,
                RealPayRebateMoney = order.RealPayRebateMoney,
                TotalPayRebateMoney = order.TotalPayRebateMoney,
                IsSplitTickets = order.IsSplitTickets,
            };
            sportsManager.AddSports_Order_Complate(complateOrder);
            sportsManager.DeleteSports_Order_Running(order);

            //if (order.SchemeType == SchemeType.ChaseBetting)
            //    MoveChaseBrotherOrder(schemeId);
            if (order.SchemeType == SchemeType.TogetherBetting)
            {
                var together = sportsManager.QuerySports_Together(schemeId);
                if (together != null)
                {
                    together.ProgressStatus = TogetherSchemeProgress.Cancel;

                    //处理退保
                    if (!together.IsPayBackGuarantees && together.ProgressStatus == TogetherSchemeProgress.Finish)
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

        /// <summary>
        /// 查询订单票数据
        /// </summary>
        public Sports_TicketQueryInfoCollection QuerySchemeTicketList(string schemeId, int pageIndex, int pageSize)
        {
            var result = new Sports_TicketQueryInfoCollection();

            var totalCount = 0;
            var sportManager = new Sports_Manager();
            var list = sportManager.QueryTicketInfoList(schemeId, pageIndex, pageSize, out totalCount);
            if (list.Count <= 0)
            {
                //可能已移动到历史数据表
                var detail = new SchemeManager().QueryOrderDetail(schemeId);
                if (detail != null && detail.ProgressStatus == ProgressStatus.Complate)
                    list = sportManager.QueryTicketHisgoryInfoList(schemeId, pageIndex, pageSize, out totalCount);
            }
            result.TotalCount = totalCount;
            result.TicketList = list;

            return result;
        }

        /// <summary>
        /// 手工修改中奖状态
        /// </summary>
        public void UpdateSchemeTicket(string ticketId, BonusStatus bonusStatus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney)
        {
            var sportManager = new Sports_Manager();
            var entity = sportManager.QueryTicket(ticketId);
            entity.BonusStatus = bonusStatus;
            entity.PreTaxBonusMoney = preTaxBonusMoney;
            entity.AfterTaxBonusMoney = afterTaxBonusMoney;
            sportManager.UpdateSports_Ticket(entity);
        }
        public void Test_WriteLog(string category, string source, string logMsg, string detail)
        {
            try
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();

                writer.Write(category, source, Common.Log.LogType.Information, logMsg, detail);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 出票错误退钱
        /// </summary>
        private void TicketErrorPayBackMoney(Sports_Manager sportsManager, string schemeId, SchemeType schemeType, decimal money, string userId, bool isAllFail = false)
        {
            if (money <= 0) return;

            switch (schemeType)
            {
                case SchemeType.GeneralBetting:
                    //直接退钱给用户
                    if (money > 0)
                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, userId, schemeId, money
                            , string.Format("订单{0}，出票失败{1:N2}元。", schemeId, money));
                    break;
                case SchemeType.ChaseBetting:
                    //直接退钱给用户
                    var chaseOrder = sportsManager.QueryLotteryScheme(schemeId);
                    if (chaseOrder != null && money > 0)
                    {
                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, userId, chaseOrder.KeyLine, money
                        , string.Format("订单{0} 出票失败，返还资金￥{1:N2}。 ", schemeId, money));
                    }
                    break;
                case SchemeType.TogetherBetting:
                    //退钱给所有参与人
                    var together = sportsManager.QuerySports_Together(schemeId);
                    if (together == null)
                        throw new Exception(string.Format("查不到订单{0}的合买信息", schemeId));
                    var joinList = sportsManager.QuerySports_TogetherSucessJoin(schemeId);

                    //出票失败百分比
                    var errorPersent = money / together.TotalMoney;
                    foreach (var joiner in joinList)
                    {
                        if (joiner.JoinType == TogetherJoinType.SystemGuarantees) continue;

                        var realBuyCount = joiner.RealBuyCount;
                        if (isAllFail)
                        {
                            if (joiner.JoinType == TogetherJoinType.Guarantees)
                                realBuyCount = joiner.BuyCount;
                        }
                        var oneErrorMoney = joiner.Price * realBuyCount * errorPersent;

                        //var oneErrorMoney = joiner.Price * joiner.RealBuyCount * errorPersent;
                        if (oneErrorMoney < 0.01M) continue;

                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, joiner.JoinUserId, string.Format("{0}_{1}", schemeId, joiner.Id)
                            , oneErrorMoney, string.Format("订单{0}，出票失败{1:N2}元，退还{2:N2}元。", schemeId, money, oneErrorMoney));
                    }

                    break;
                case SchemeType.ExperterScheme:
                    break;
                case SchemeType.SaveScheme:
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 退票
        /// </summary>
        public QueryTicketInfo PayBackTicket(string ticketId)
        {
            var info = new QueryTicketInfo { };

            var ticketArray = ticketId.Split('|');
            if (ticketArray.Length != 3)
                return info;
            var schemeId = ticketArray[1];
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new Exception(string.Format("方案{0}不存在.", schemeId));

            var ticket = sportsManager.QueryTicket(ticketId);
            if (ticket == null)
                throw new Exception(string.Format("找不到票号:{0}", ticketId));
            if (ticket.TicketStatus == TicketStatus.Abort)
                throw new Exception(string.Format("票{0}已退票", ticketId));
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                ticket.TicketStatus = TicketStatus.Abort;
                sportsManager.UpdateSports_Ticket(ticket);

                order.SuccessMoney -= ticket.BetMoney;
                sportsManager.UpdateSports_Order_Running(order);
                //出票失败返钱
                TicketErrorPayBackMoney(sportsManager, schemeId, order.SchemeType, ticket.BetMoney, order.UserId);

                var successTicketList = sportsManager.QuerySuccessTicketList(schemeId);
                if (successTicketList.Count == 0)
                {
                    //订单全部失败
                    OrderFailToEnd(schemeId, sportsManager, order);
                }
                info.SchemeId = schemeId;
                info.TicketId = ticketId;
                info.TotalMoney = ticket.BetMoney;

                biz.CommitTran();
            }
            return info;
        }

        #endregion

        /// <summary>
        /// 传统足球、数字彩开奖
        /// </summary>
        public void LotteryOpen(string gameCode, string issuseNumber)
        {
            var lotteryManager = new LotteryGameManager();
            var issuseEntity = lotteryManager.QueryGameIssuseByKey(gameCode, string.Empty, issuseNumber);
            if (issuseEntity == null)
                throw new Exception(string.Format("奖期{0}.{1} 不存在", gameCode, issuseNumber));
            if (issuseEntity.Status == IssuseStatus.Awarded)
                throw new Exception("奖期已开奖");
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                issuseEntity.Status = IssuseStatus.Awarded;
                lotteryManager.UpdateGameIssuse(issuseEntity);

                var sportsManager = new Sports_Manager();
                var runningOrder = sportsManager.QueryWaitForOpenRunningOrder(gameCode, issuseNumber);
                foreach (var order in runningOrder)
                {
                    order.BonusStatus = BonusStatus.Awarding;
                    sportsManager.UpdateSports_Order_Running(order);
                }

                biz.CommitTran();
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
            if (issuseEntity.Status == IssuseStatus.Stopped)
                throw new Exception(string.Format("奖期{0}.{1}.{2}奖期已派奖", gameCode, gameType, issuseNumber));

            issuseEntity.Status = IssuseStatus.Stopped;
            issuseEntity.AwardTime = DateTime.Now;
            issuseEntity.WinNumber = winNumber;
            lotteryManager.UpdateGameIssuse(issuseEntity);

            //更新Redis缓存
            RedisMatchBusiness.LoadSZCWinNumber(gameCode);
        }

        private static List<BonusRule> _bonusRuleList = new List<BonusRule>();
        private BonusRule GetBonusRule(string gameCode, string gameType, int bonusLevel)
        {
            if (_bonusRuleList.Count == 0)
                _bonusRuleList = new BonusRuleManager().QueryAllBonusRule();

            var rule = (from b in _bonusRuleList where b.GameCode == gameCode && b.GameType == gameType && b.BonusGrade == bonusLevel select b).FirstOrDefault();
            if (rule == null)
                throw new Exception("未定义此中奖规则：" + gameCode + "-" + gameType + " - " + bonusLevel);
            return rule;
        }

        /// <summary>
        /// 足彩派奖
        /// </summary>
        public void SportsPrize(string schemeId, BonusStatus bonusStatus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney)
        {
            Sports_SchemeQueryInfo schemeInfo = new Sports_SchemeQueryInfo();
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new LogicException(string.Format("找不到方案{0}的Order_Running订单信息", schemeId));
            else if (order.ProgressStatus == ProgressStatus.Waitting)
                throw new Exception(string.Format("派奖出错:订单{0}状态异常,当前进度状态:{1},当前票状态{2}。", schemeId, order.ProgressStatus, order.TicketStatus));

            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);
            if (orderDetail == null)
                throw new Exception(string.Format("找不到方案{0}的OrderDetail订单信息", schemeId));
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                schemeInfo.UserId = order.UserId;
                schemeInfo.GameCode = order.GameCode;
                schemeInfo.GameType = order.GameType;
                schemeInfo.IssuseNumber = order.IssuseNumber;
                schemeInfo.TotalMoney = order.SuccessMoney;
                schemeInfo.IsVirtualOrder = order.IsVirtualOrder;
                schemeInfo.SchemeType = order.SchemeType;

                #region 处理名家推荐

                if (order.SchemeType == SchemeType.ExperterScheme && order.IsVirtualOrder)
                {
                    var experManager = new ExperterSchemeManager();
                    var scheme = experManager.QueryExperterSchemeId(order.SchemeId);
                    if (scheme != null)
                    {
                        scheme.BonusStatus = bonusStatus;
                        scheme.BonusMoney = afterTaxBonusMoney;
                        experManager.UpdateExperterScheme(scheme);
                    }
                }

                #endregion

                #region 更新票数据

                //var localTicketList = sportsManager.QueryTicketList(schemeId);
                //var bonusTicketList = BusinessHelper.QueryHistogryTicketBonusList(order.GameCode, schemeId);
                //foreach (var item in localTicketList)
                //{
                //    var bonusTicket = bonusTicketList.FirstOrDefault(p => p.TicketId == item.TicketId);
                //    item.BonusStatus = bonusTicket == null ? BonusStatus.Lose : BonusStatus.Win;
                //    item.PreTaxBonusMoney = bonusTicket == null ? 0M : bonusTicket.PreTaxBonusMoney;
                //    item.AfterTaxBonusMoney = bonusTicket == null ? 0M : bonusTicket.AfterTaxBonusMoney;
                //    sportsManager.UpdateSports_Ticket(item);
                //}

                #endregion

                #region 处理订单

                order.BonusStatus = bonusStatus;
                order.TicketStatus = TicketStatus.Ticketed;
                order.ProgressStatus = ProgressStatus.Complate;
                order.PreTaxBonusMoney = preTaxBonusMoney;
                order.AfterTaxBonusMoney = afterTaxBonusMoney;
                //sportsManager.UpdateSports_Order_Running(order);

                orderDetail.BonusStatus = bonusStatus;
                orderDetail.ComplateTime = DateTime.Now;
                orderDetail.PreTaxBonusMoney = preTaxBonusMoney;
                orderDetail.AfterTaxBonusMoney = afterTaxBonusMoney;
                orderDetail.ProgressStatus = ProgressStatus.Complate;
                orderDetail.TicketStatus = TicketStatus.Ticketed;
                manager.UpdateOrderDetail(orderDetail);

                var complateOrder = new Sports_Order_Complate
                {
                    SchemeId = order.SchemeId,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    PlayType = order.PlayType,
                    IssuseNumber = order.IssuseNumber,
                    TotalMoney = order.TotalMoney,
                    Amount = order.Amount,
                    TotalMatchCount = order.TotalMatchCount,
                    TicketStatus = order.TicketStatus,
                    BonusStatus = order.BonusStatus,
                    AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                    CanChase = order.CanChase,
                    IsVirtualOrder = order.IsVirtualOrder,
                    CreateTime = order.CreateTime,
                    PreTaxBonusMoney = order.PreTaxBonusMoney,
                    ProgressStatus = order.ProgressStatus,
                    SchemeType = order.SchemeType,
                    TicketGateway = order.TicketGateway,
                    TicketProgress = order.TicketProgress,
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
                    IsPayRebate = order.IsPayRebate,
                    TotalPayRebateMoney = order.TotalPayRebateMoney,
                    RealPayRebateMoney = order.RealPayRebateMoney,
                    MaxBonusMoney = order.MaxBonusMoney,
                    MinBonusMoney = order.MinBonusMoney,
                    RightCount = order.RightCount,
                    Error1Count = order.Error1Count,
                    Error2Count = order.Error2Count,
                    AddMoney = 0M,
                    DistributionWay = AddMoneyDistributionWay.Average,
                    AddMoneyDescription = string.Empty,
                    Security = order.Security,
                    BetTime = order.BetTime,
                    SuccessMoney = order.SuccessMoney,
                    ExtensionOne = order.ExtensionOne,
                    BonusCount = order.BonusCount,
                    HitMatchCount = order.HitMatchCount,
                    Attach = order.Attach,
                    IsAppend = order.IsAppend,
                    TicketTime = order.TicketTime,
                    RedBagMoney = order.RedBagMoney,
                    QueryTicketStopTime = order.QueryTicketStopTime,
                    BonusCountDescription = "",
                    BonusCountDisplayName = "",
                    IsSplitTickets = order.IsSplitTickets,
                };
                sportsManager.AddSports_Order_Complate(complateOrder);
                sportsManager.DeleteSports_Order_Running(order);

                #endregion

                #region 处理追号订单

                while (true)
                {
                    if (order.SchemeType != SchemeType.ChaseBetting)
                        break;
                    if (bonusStatus == BonusStatus.Win && orderDetail.StopAfterBonus)
                    {
                        BusinessHelper.MoveChaseBrotherOrder(schemeId);
                        break;
                    }
                    var setNextOrderSql = string.Format(@"update C_Lottery_Scheme set IsComplate='true' where SchemeId='{0}' 
                                        update C_Sports_Order_Running
                                        set CanChase='true'
                                        where schemeid=
                                        (select top 1 schemeid 
                                        from C_Lottery_Scheme
                                        where Keyline=
	                                        (select KeyLine 
	                                        from C_Lottery_Scheme
	                                        where schemeid='{0}')
                                        and  IsComplate='false'
                                        order by OrderIndex ASC)", schemeId);
                    sportsManager.ExecSql(setNextOrderSql);
                    break;
                }

                #endregion

                #region 处理合买

                if (order.SchemeType == SchemeType.TogetherBetting)
                {
                    var main = sportsManager.QuerySports_Together(schemeId);
                    if (main.ProgressStatus != TogetherSchemeProgress.AutoStop && main.ProgressStatus != TogetherSchemeProgress.Cancel)
                        main.ProgressStatus = TogetherSchemeProgress.Completed;
                    sportsManager.UpdateSports_Together(main);

                    //处理订制跟单
                    var sql = bonusStatus == BonusStatus.Win
                        ? string.Format("UPDATE C_Together_FollowerRule SET  NotBonusSchemeCount=0 WHERE CreaterUserId='{0}' AND GameCode='{1}' AND GameType='{2}'", order.UserId, order.GameCode, order.GameType)
                        : string.Format("UPDATE C_Together_FollowerRule SET  NotBonusSchemeCount=NotBonusSchemeCount+1 WHERE CreaterUserId='{0}' AND GameCode='{1}' AND GameType='{2}'", order.UserId, order.GameCode, order.GameType);
                    sportsManager.ExecSql(sql);

                    if (bonusStatus == BonusStatus.Win && afterTaxBonusMoney > 0)
                    {
                        //提成
                        var deductMoney = 0M;
                        if (afterTaxBonusMoney > main.TotalMoney)
                            deductMoney = (afterTaxBonusMoney - main.TotalMoney) * main.BonusDeduct / 100;
                        var totalMoney = afterTaxBonusMoney - deductMoney;
                        //var singleMoney = Math.Truncate((totalMoney / main.TotalCount) * 100) / 100;
                        var singleMoney = totalMoney / main.TotalCount;
                        foreach (var join in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            join.PreTaxBonusMoney = join.RealBuyCount * singleMoney;
                            join.AfterTaxBonusMoney = join.RealBuyCount * singleMoney;
                            sportsManager.UpdateSports_TogetherJoin(join);
                            //订制跟单奖金更新
                            if (join.JoinType == TogetherJoinType.FollowerJoin)
                            {
                                //修改定制跟单记录
                                var f = sportsManager.QueryFollowerRecordBySchemeId(main.SchemeId, join.JoinUserId);
                                if (f == null) continue;
                                f.BonusMoney = join.RealBuyCount * singleMoney;
                                sportsManager.UpdateTogetherFollowerRecord(f);

                                //修改定制跟单规则
                                var followRule = sportsManager.QueryTogetherFollowerRule(main.CreateUserId, join.JoinUserId, order.GameCode, order.GameType);
                                if (followRule == null) continue;
                                followRule.TotalBonusOrderCount++;
                                followRule.TotalBonusMoney += join.AfterTaxBonusMoney;
                                sportsManager.UpdateTogetherFollowerRule(followRule);
                            }
                        }
                    }
                }

                #endregion

                //if (order.BonusStatus == BonusStatus.Win)
                //    InitUserBeedingAndBounsPercent(sportsManager, order.UserId, order.GameCode, order.GameType);

                biz.CommitTran();
            }
            try
            {
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IOrderPrize_AfterTranCommit>(new object[] { schemeInfo.UserId, schemeId, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, bonusStatus == BonusStatus.Win, preTaxBonusMoney, afterTaxBonusMoney, schemeInfo.IsVirtualOrder, DateTime.Now });
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("ERROR_SportsPrize", "_SportsPrize", Common.Log.LogType.Error, "订单派奖执行插件失败", ex.ToString());
            }
        }

        private void InitUserBeedingAndBounsPercent(Sports_Manager sportsManager, string userId, string gameCode, string gameType)
        {
            return;

            var allowBeedingGameCodeArray = new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!allowBeedingGameCodeArray.Contains(gameCode))
                return;

            switch (gameCode)
            {
                case "SSQ":
                case "DLT":
                case "FC3D":
                case "PL3":
                case "CQSSC":
                case "JX11X5":
                    gameType = string.Empty;
                    break;
                default:
                    break;
            }

            var beeding = sportsManager.QueryUserBeedings(userId, gameCode, gameType);
            if (beeding == null)
            {
                sportsManager.AddUserBeedings(new UserBeedings
                {
                    UserId = userId,
                    UpdateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    BeFollowedTotalMoney = 0M,
                    BeFollowerUserCount = 0,
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
            var bonusPercent = sportsManager.QueryUserBonusPercent(userId, gameCode, gameType);
            if (bonusPercent == null)
            {
                sportsManager.AddUserBonusPercent(new UserBonusPercent
                {
                    BonusPercent = 0M,
                    CreateTime = DateTime.Now,
                    CurrentDate = DateTime.Now.ToString("yyyyMM"),
                    GameCode = gameCode,
                    GameType = gameType,
                    UserId = userId,
                    BonusOrderCount = 0,
                    TotalOrderCount = 0,
                });
            }
        }

        /// <summary>
        /// 查询等待派钱的订单列表
        /// </summary>
        public Sports_SchemeQueryInfoCollection QueryWaitForPrizeMoneyOrderList(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            var result = new Sports_SchemeQueryInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryWaitForPrizeMoneyOrderList(startTime, endTime, gameCode, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }
        /// <summary>
        /// 足彩派钱
        /// </summary>
        public List<Sports_SchemeQueryInfo> SportsPrizeMoney(string[] schemeIdArray)
        {
            var list = new List<Sports_SchemeQueryInfo>();
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                foreach (var schemeId in schemeIdArray)
                {
                    var order = sportsManager.QuerySports_Order_Complate(schemeId);
                    if (order == null) continue;
                    if (order.BonusStatus != BonusStatus.Win) continue;
                    if (order.IsPrizeMoney) continue;

                    order.IsPrizeMoney = true;
                    sportsManager.UpdateSports_Order_Complate(order);
                    if (order.IsVirtualOrder) continue;

                    var game = new LotteryGameManager().LoadGame(order.GameCode);
                    var userManager = new UserManager();

                    //if (order.SchemeType == SchemeType.GeneralBetting || order.SchemeType == SchemeType.ChaseBetting || order.SchemeType == SchemeType.SaveScheme)
                    if (order.SchemeType == SchemeType.GeneralBetting || order.SchemeType == SchemeType.ChaseBetting || order.SchemeType == SchemeType.SingleCopy)
                    {
                        if (order.AfterTaxBonusMoney > 0)
                        {
                            if (order.SchemeType == SchemeType.SingleCopy)//抄单订单，派奖时需减去奖金提成的金额
                            {
                                var bdfxManager = new TotalSingleTreasureManager();
                                var bdfxRecorSingleEntity = bdfxManager.QueryBDFXRecordSingleCopyBySchemeId(schemeId);
                                var realBonusMoney = order.AfterTaxBonusMoney;
                                var commissionMoney = 0M;
                                if (bdfxRecorSingleEntity != null)
                                {
                                    var BDFXEntity = bdfxManager.QueryTotalSingleTreasureBySchemeId(bdfxRecorSingleEntity.BDXFSchemeId);
                                    if (BDFXEntity != null)
                                    {
                                        //计算提成金额
                                        if ((order.AfterTaxBonusMoney - order.TotalMoney) > 0)
                                        {
                                            commissionMoney = (order.AfterTaxBonusMoney - order.TotalMoney) * BDFXEntity.Commission / 100M;
                                            commissionMoney = Math.Truncate(commissionMoney * 100) / 100M;
                                            realBonusMoney = order.AfterTaxBonusMoney - commissionMoney;
                                            //返提成
                                            if (commissionMoney > 0)
                                            {
                                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_BDFXCommissionMoney, BDFXEntity.UserId, schemeId, commissionMoney,
                                                    string.Format("抄单订单{0}中奖{1:N2}元,提成{2:N0}%,获得奖金盈利提成金额{3:N2}元.", schemeId, order.AfterTaxBonusMoney, BDFXEntity.Commission, commissionMoney));
                                            }
                                        }
                                    }
                                }
                                //返奖金
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, realBonusMoney,
                                        string.Format("抄单订单{0}中奖{1:N2}元,扣除奖金盈利提成金额{2:N2}元,实得奖金{3:N2}元.", schemeId, order.AfterTaxBonusMoney, commissionMoney, realBonusMoney));
                            }
                            else
                            {
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AfterTaxBonusMoney,
                                    string.Format("中奖奖金{0:N2}元.", order.AfterTaxBonusMoney));
                            }
                        }

                        if (order.AddMoney > 0)
                            BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AddMoney,
                                                        string.Format("活动赠送{0:N2}元.", order.AddMoney));
                    }
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        var main = sportsManager.QuerySports_Together(schemeId);
                        //提成
                        var deductMoney = 0M;
                        if (order.AfterTaxBonusMoney > main.TotalMoney)
                            deductMoney = (order.AfterTaxBonusMoney - main.TotalMoney) * main.BonusDeduct / 100;
                        //提成金额，只能给合买发起者
                        if (deductMoney > 0M)
                            BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Deduct, order.UserId, schemeId, deductMoney,
                                string.Format("订单{0}， 佣金{1:N2}元。", schemeId, deductMoney));


                        //中奖金额,分发到所有参与合买的用户的奖金账户
                        var bonusMoney = order.AfterTaxBonusMoney - deductMoney;
                        var singleMoney = bonusMoney / main.TotalCount;
                        foreach (var join in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            if (join.JoinType == TogetherJoinType.SystemGuarantees) continue;
                            //发参与奖金
                            var joinMoney = join.RealBuyCount * singleMoney;
                            //派钱
                            if (joinMoney > 0M)
                                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, join.JoinUserId, schemeId, joinMoney,
                                    string.Format("中奖分成，奖金￥{0:N2}元。", joinMoney));
                        }
                        if (order.AddMoney > 0M)
                        {
                            //加奖金额分配给发起者
                            if (order.DistributionWay == AddMoneyDistributionWay.CreaterOnly)
                            {
                                //加奖
                                if (order.AddMoney > 0)
                                    BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, order.UserId, schemeId, order.AddMoney,
                                        string.Format("活动赠送{0:N2}元。", order.AddMoney), RedBagCategory.Activity);
                            }
                            //处理加奖
                            if (order.DistributionWay == AddMoneyDistributionWay.Average)
                            {
                                var addMonesinglePrice = order.AddMoney / main.TotalCount;
                                foreach (var join in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                                {
                                    if (join.JoinType == TogetherJoinType.SystemGuarantees) continue;
                                    //发参与奖金
                                    var joinMoney = join.RealBuyCount * addMonesinglePrice;
                                    //派钱
                                    BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, join.JoinUserId, schemeId, joinMoney,
                                        string.Format("订单{0}活动赠送{1:N2}元。", schemeId, joinMoney), RedBagCategory.Activity);
                                }
                            }
                            //加奖金额分配给发起者
                            if (order.DistributionWay == AddMoneyDistributionWay.JoinerOnly)
                            {
                                //订单发起者没有加奖
                                var joinList = sportsManager.QuerySports_TogetherSucessJoin(schemeId);
                                var createrList = joinList.Where(p => p.JoinUserId == order.UserId).ToList();
                                var createJoinCount = createrList.Count == 0 ? 0 : createrList.Sum(p => p.RealBuyCount);
                                var addMonesinglePrice = order.AddMoney / (main.TotalCount - createJoinCount);
                                foreach (var join in joinList)
                                {
                                    if (join.JoinType == TogetherJoinType.SystemGuarantees) continue;
                                    if (join.JoinUserId == order.UserId) continue;
                                    //发参与奖金
                                    var joinMoney = join.RealBuyCount * addMonesinglePrice;
                                    //派钱
                                    BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Activity, join.JoinUserId, schemeId, joinMoney,
                                        string.Format("订单{0}活动赠送{1:N2}元。", schemeId, joinMoney), RedBagCategory.Activity);
                                }
                            }
                        }
                    }

                    list.Add(new Sports_SchemeQueryInfo
                    {
                        UserId = order.UserId,
                        SchemeId = schemeId,
                        GameCode = order.GameCode,
                        GameType = order.GameType,
                        IssuseNumber = order.IssuseNumber,
                        TotalMoney = order.TotalMoney,
                        PreTaxBonusMoney = order.PreTaxBonusMoney,
                        AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                    });
                }

                biz.CommitTran();
            }
            return list;
        }

        public UserCurrentOrderInfoCollection QueryUserCurrentOrderList(string userId, string gameCode, int pageIndex, int pageSize)
        {
            var sportsManager = new Sports_Manager();
            int totalCount = 0;
            var list = sportsManager.QueryUserCurrentOrderList(userId, gameCode, pageIndex, pageSize, out  totalCount);
            var result = new UserCurrentOrderInfoCollection();
            result.TotalCount = totalCount;
            result.List.AddRange(list);
            return result;
        }
        public UserCurrentOrderInfoCollection QueryUserCurrentTogetherOrderList(string userId, string gameCode, int pageIndex, int pageSize)
        {
            var result = new UserCurrentOrderInfoCollection();
            int totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserCurrentTogetherOrderList(userId, gameCode, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        private void CheckSportAnteCode(string gameCode, string gameType, string[] anteCode)
        {
            var allowCodeList = new string[] { };
            switch (gameCode)
            {
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负
                        case "BRQSPF": //不让球胜平负
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":     // 胜负
                        case "RFSF":     // 让分胜负
                        case "DXF":     // 大小分
                            allowCodeList = "3,0".Split(',');
                            break;
                        case "SFC":      // 胜分差
                            allowCodeList = "01,02,03,04,05,06,11,12,13,14,15,16".Split(',');
                            break;
                    }
                    break;
                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负 
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "SXDS":    // 上下单双。上单 3；上双 2；下单 1；下双 0；
                            allowCodeList = "SD,SS,XD,XS".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                        case "SF":
                            allowCodeList = "3,0".Split(',');
                            break;
                    }
                    break;
                default:
                    break;
            }
            foreach (var item in anteCode)
            {
                if (!allowCodeList.Contains(item))
                    throw new Exception(string.Format("投注号码{0}为非法字符", item));
            }
        }
        public int CheckBettingOrderMoney(Sports_AnteCodeInfoCollection codeList, string gameCode, string gameType, string playType, int amount, decimal schemeTotalMoney, DateTime stopTime, bool isAllow = false, string userId = "")
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
                CheckSportAnteCode(gameCode, string.IsNullOrEmpty(item.GameType) ? gameType : item.GameType.ToUpper(), oneCodeArray);
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
        public DateTime CheckGeneralBettingMatch(Sports_Manager sportsManager, string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
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
                    CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                    return SFGGMatchList.Min(m => m.BetStopTime.Value);
                }
                else
                {

                    if (matchList.Count != matchIdArray.Length)
                        throw new LogicException("所选比赛中有停止销售的比赛。");
                    CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
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

                CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);

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

                CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);

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
        //验证 不支持的玩法
        private void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<JCZQ_Match> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 10：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<JCLQ_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<BJDC_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        #region 数字彩票相关,包括传统足球

        private void CheckSJBMatch(string gameType, int matchId)
        {
            var entity = new SJBMatchManager().GetSJBMatch(gameType, matchId);
            if (entity == null)
                throw new Exception("投注场次错误");

            if (entity.BetState != "开售")
                throw new Exception(string.Format("比赛{0}停止销售", matchId));
        }

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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                        schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(keyLine))
                            schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                        else
                            schemeId = keyLine;
                    }
                    lock (schemeId)
                    {
                        var anteCodeList = new List<Sports_AnteCode>();
                        var gameTypeList = new List<GameTypeInfo>();
                        foreach (var item in info.AnteCodeList)
                        {
                            var codeEntity = new Sports_AnteCode
                            {
                                AnteCode = item.AnteCode,
                                BonusStatus = BonusStatus.Waitting,
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
                            sportsManager.AddLotteryScheme(new LotteryScheme
                            {
                                OrderIndex = orderIndex,
                                KeyLine = keyLine,
                                SchemeId = schemeId,
                                CreateTime = DateTime.Now,
                                IsComplate = false,
                                IssuseNumber = issuse.IssuseNumber,
                            });
                        }
                        var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
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

                biz.CommitTran();
            }
            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writer.Write("LotteryBetting", "SQL", LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));


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
                this.writer.Write("LotteryBetting", "Redis", LogType.Information, "投注耗时记录", string.Format("订单{0}总用时{1}毫秒", keyLine, watch.Elapsed.TotalMilliseconds));

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return keyLine;
        }

        private void SerializChaseOrder(LotteryBettingInfo order, string chaseOrderId)
        {
            try
            {
                var json = JsonSerializer.Serialize(order);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "ChaseOrder", DateTime.Now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileName = Path.Combine(path, string.Format("{0}.json", chaseOrderId));
                File.WriteAllText(fileName, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                this.writer.Write("LotteryBetting", "SerializChaseOrder", LogType.Information, "序列化失败", ex.ToString());
            }
        }

        /// <summary>
        /// 写入追号订单数据到数据库
        /// </summary>
        public string WriteChaseOrderToDb()
        {
            var logList = new List<string>();
            logList.Add("<---------开始写入追号订单数据到数据库");
            var maxDay = 5;
            for (int i = 0; i < maxDay; i++)
            {
                var now = DateTime.Today.AddDays(-i);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "ChaseOrder", now.ToString("yyyy-MM-dd"));
                logList.Add(string.Format("查询目录:{0}", path));
                if (!Directory.Exists(path))
                    continue;

                var sportsManager = new Sports_Manager();
                var schemeManager = new SchemeManager();
                var gameTypes = new LotteryGameManager().QueryEnableGameTypes();
                //日期下面只有一级文件
                var fileArray = Directory.GetFiles(path);
                logList.Add(string.Format("文件数：{0}个", fileArray.Length));
                foreach (var item in fileArray)
                {
                    var json = File.ReadAllText(item, Encoding.UTF8);
                    var chaseOrderId = item.Substring(item.LastIndexOf("\\") + 1).Replace(".json", "");
                    var order = JsonSerializer.Deserialize<LotteryBettingInfo>(json);

                    order.IssuseNumberList.Sort((x, y) =>
                    {
                        return x.IssuseNumber.CompareTo(y.IssuseNumber);
                    });

                    logList.Add(string.Format("开始处理{0}", chaseOrderId));
                    //一个追号订单，保存到数据库
                    try
                    {
                        var chaseSchemeList = sportsManager.QueryAllLotterySchemeByKeyLine(chaseOrderId);
                        var schemeIdArray = chaseSchemeList.Select(p => p.SchemeId).ToArray();
                        //查询三个订单表数据
                        var orderDetailList = schemeManager.QueryOrderDetailListBySchemeId(schemeIdArray);
                        var orderRunningList = sportsManager.QueryOrderRunningBySchemeIdArray(schemeIdArray);
                        var orderComplateList = sportsManager.QueryOrderComplateBySchemeIdArray(schemeIdArray);

                        if (chaseSchemeList.Count == orderDetailList.Count && chaseSchemeList.Count == orderRunningList.Count + orderComplateList.Count)
                        {
                            //订单数据正常，删除订单文件
                            logList.Add("订单数据正常，删除订单文件");
                            File.Delete(item);
                            continue;
                        }

                        var gameTypeList = new List<GameTypeInfo>();
                        foreach (var code in order.AnteCodeList)
                        {
                            var t = gameTypes.FirstOrDefault(a => a.Game.GameCode == order.GameCode && a.GameType == code.GameType.ToUpper());
                            if (t != null && !gameTypeList.Contains(t))
                            {
                                gameTypeList.Add(t);
                            }
                        }
                        var gameType = string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray());

                        //计算OrderDetail表没有的数据并写入
                        foreach (var scheme in chaseSchemeList)
                        {
                            var orderDetail = orderDetailList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            if (orderDetail == null)
                            {
                                logList.Add("写入orderDetail");
                                var currentIssuse = order.IssuseNumberList.FirstOrDefault(p => p.IssuseNumber == scheme.IssuseNumber);

                                schemeManager.AddOrderDetail(new OrderDetail
                                {
                                    SchemeId = scheme.SchemeId,
                                    AddMoney = 0M,
                                    AfterTaxBonusMoney = 0M,
                                    AgentId = string.Empty,
                                    Amount = currentIssuse == null ? 1 : currentIssuse.Amount,
                                    BetTime = DateTime.Now,
                                    BonusAwardsMoney = 0M,
                                    BonusStatus = BonusStatus.Waitting,
                                    ComplateTime = null,
                                    CreateTime = DateTime.Now,
                                    CurrentBettingMoney = 0M,
                                    CurrentIssuseNumber = currentIssuse == null ? string.Empty : currentIssuse.IssuseNumber,
                                    GameCode = order.GameCode,
                                    GameType = gameType,
                                    GameTypeName = BusinessHelper.FormatGameType(order.GameCode, gameType),
                                    IsAppend = order.IsAppend,
                                    IsVirtualOrder = false,
                                    PlayType = string.Empty,
                                    PreTaxBonusMoney = 0M,
                                    ProgressStatus = ProgressStatus.Waitting,
                                    RealPayRebateMoney = 0M,
                                    RedBagAwardsMoney = 0M,
                                    RedBagMoney = 0M,
                                    SchemeBettingCategory = SchemeBettingCategory.GeneralBetting,
                                    SchemeSource = order.SchemeSource,
                                    SchemeType = SchemeType.ChaseBetting,
                                    StartIssuseNumber = string.Empty,
                                    StopAfterBonus = order.StopAfterBonus,
                                    TicketStatus = TicketStatus.Waitting,
                                    TicketTime = null,
                                    TotalIssuseCount = order.IssuseNumberList.Count,
                                    TotalMoney = currentIssuse.IssuseTotalMoney,
                                    TotalPayRebateMoney = 0M,
                                    UserId = order.UserId,
                                });
                            }
                        }

                        //计算OrderRunning表没有的数据并写入
                        foreach (var scheme in chaseSchemeList)
                        {
                            var runningOrder = orderRunningList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            var comlateOrder = orderComplateList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            if (runningOrder == null && comlateOrder == null)
                            {
                                logList.Add("写入runningOrder");
                                var currentIssuse = order.IssuseNumberList.FirstOrDefault(p => p.IssuseNumber == scheme.IssuseNumber);
                                sportsManager.AddSports_Order_Running(new Sports_Order_Running
                                {
                                    AfterTaxBonusMoney = 0M,
                                    AgentId = string.Empty,
                                    Amount = currentIssuse == null ? 1 : currentIssuse.Amount,
                                    Attach = string.Empty,

                                    BonusStatus = BonusStatus.Waitting,
                                    CanChase = false,
                                    IsVirtualOrder = false,
                                    IsPayRebate = false,
                                    RealPayRebateMoney = 0M,
                                    TotalPayRebateMoney = 0M,
                                    CreateTime = DateTime.Now,
                                    GameCode = order.GameCode,
                                    GameType = gameType,
                                    IssuseNumber = currentIssuse == null ? string.Empty : currentIssuse.IssuseNumber,
                                    PlayType = string.Empty,
                                    PreTaxBonusMoney = 0M,
                                    ProgressStatus = ProgressStatus.Waitting,
                                    SchemeId = scheme.SchemeId,
                                    SchemeType = SchemeType.ChaseBetting,
                                    SchemeBettingCategory = SchemeBettingCategory.GeneralBetting,
                                    TicketId = string.Empty,
                                    TicketLog = string.Empty,
                                    TicketStatus = TicketStatus.Waitting,
                                    TotalMatchCount = 0,
                                    TotalMoney = currentIssuse.IssuseTotalMoney,
                                    SuccessMoney = 0M,
                                    UserId = order.UserId,
                                    StopTime = DateTime.Now,
                                    SchemeSource = SchemeSource.Web,
                                    BetCount = 0,
                                    BonusCount = 0,
                                    HitMatchCount = 0,
                                    RightCount = 0,
                                    Error1Count = 0,
                                    Error2Count = 0,
                                    MaxBonusMoney = 0,
                                    MinBonusMoney = 0,
                                    Security = TogetherSchemeSecurity.Public,
                                    TicketGateway = string.Empty,
                                    TicketProgress = 1M,
                                    BetTime = DateTime.Now,
                                    ExtensionOne = string.Format("{0}{1}", "3X1_", (int)order.ActivityType),
                                    QueryTicketStopTime = DateTime.Now.AddMinutes(1).ToString("yyyyMMddHHmm"),
                                    IsAppend = false,
                                    RedBagMoney = 0,
                                    IsSplitTickets = false,
                                    TicketTime = null,
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("保存追号订单数据失败:{0}", ex.ToString()));
                    }
                }
            }
            logList.Add("本次处理全部完成---------->");
            return string.Join(Environment.NewLine, logList.ToArray());
        }

        /// <summary>
        /// 修复追号订单，用于追号订单卡死、不能继续的情况
        /// 查询redis中数字彩开奖号，然后查询 running 表里面未结算订单
        /// 做拆票、派奖操作
        /// </summary>
        public string RepairChaseOrder(string gameCode, int maxCount)
        {
            var logList = new List<string>();
            logList.Add(string.Format("<--------开始修复{0}，最大{1}条", gameCode, maxCount));
            var winNumber = RedisMatchBusiness.QuerySZCWinNumber(gameCode).OrderBy(p => p.Key).ToArray();
            var bonusPool = RedisMatchBusiness.QuserySZCBonusPool(gameCode);
            foreach (var win in winNumber)
            {
                var sportsManager = new Sports_Manager();
                var orderList = sportsManager.QueryErrorChaseOrderRunning(gameCode, win.Key, maxCount);
                logList.Add(string.Format("彩种：{0} 第{1}期开奖号为{2}，异常订单{3}条", gameCode, win.Key, win.Value, orderList.Count));
                foreach (var order in orderList)
                {
                    //处理一个订单
                    try
                    {
                        //step 1:拆票
                        logList.Add(string.Format("订单{0}拆票", order.SchemeId));
                        var log = DoSplitOrderTicketsWithNoThread(order.SchemeId, order);
                        logList.Add(log);
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("拆票异常：{0}", ex.ToString()));
                        continue;
                    }

                    try
                    {
                        //step 2:派奖
                        #region 派奖

                        logList.Add(string.Format("订单{0}开始派奖", order.SchemeId));
                        var watch = new Stopwatch();
                        watch.Start();
                        var ticketList = sportsManager.QueryTicketList(order.SchemeId);
                        watch.Stop();
                        logList.Add(string.Format("查询所有票数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                        watch.Restart();
                        var prizeList = new List<TicketBatchPrizeInfo>();
                        var totalPreBonusMoney = 0M;
                        var totalAfterBonusMoney = 0M;
                        foreach (var ticket in ticketList)
                        {
                            var preTaxBonusMoney = 0M;
                            var afterTaxBonusMoney = 0M;

                            DoComputeTicketBonusMoney(ticket.TicketId, ticket.GameCode, ticket.GameType, ticket.BetContent, ticket.Amount, ticket.IsAppend,
                                ticket.IssuseNumber, win.Value, bonusPool, out preTaxBonusMoney, out afterTaxBonusMoney);

                            //更新票数据sql
                            prizeList.Add(new TicketBatchPrizeInfo
                            {
                                TicketId = ticket.TicketId,
                                BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                                PreMoney = preTaxBonusMoney,
                                AfterMoney = afterTaxBonusMoney,
                            });
                        }

                        prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
                        totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
                        totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
                        //更新到数据库
                        BusinessHelper.UpdateTicketBonus(prizeList);
                        if (totalPreBonusMoney < 0M)
                            totalPreBonusMoney = -1;
                        if (totalAfterBonusMoney < 0M)
                            totalAfterBonusMoney = -1;
                        watch.Stop();
                        logList.Add(string.Format("票数据派奖用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                        watch.Restart();
                        BusinessHelper.DoOrderPrize(order.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
                        watch.Stop();
                        logList.Add(string.Format("订单数据派奖用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                        #endregion

                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("派奖异常：{0}", ex.ToString()));
                    }
                }
            }
            return string.Join(Environment.NewLine, logList.ToArray());
        }

        /// <summary>
        /// 取消追号订单
        /// </summary>
        public void CancelChaseOrder(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId))
                return;
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new Exception(string.Format("查不到方案{0}的Order_Running信息", schemeId));
            if (order.TicketStatus != TicketStatus.Waitting)
                throw new LogicException("订单状态已更新，不能取消追号");
            if (order.SchemeType != SchemeType.ChaseBetting)
                throw new Exception("订单不是追号订单");

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 修改订单相关信息

                order.TicketStatus = TicketStatus.Abort;
                order.ProgressStatus = ProgressStatus.Aborted;
                order.BonusStatus = BonusStatus.Error;
                order.TicketId = "";
                order.TicketProgress = 0;
                order.TicketLog = "用户取消追号";
                order.BetTime = DateTime.Now;
                sportsManager.UpdateSports_Order_Running(order);

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                orderDetail.ProgressStatus = ProgressStatus.Aborted;
                orderDetail.BonusStatus = BonusStatus.Error;
                orderDetail.CurrentBettingMoney = 0M;
                orderDetail.CurrentIssuseNumber = order.IssuseNumber;
                orderDetail.TicketStatus = TicketStatus.Abort;
                orderDetail.BetTime = DateTime.Now;
                manager.UpdateOrderDetail(orderDetail);

                var complateOrder = new Sports_Order_Complate
                {
                    SchemeId = order.SchemeId,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    PlayType = order.PlayType,
                    IssuseNumber = order.IssuseNumber,
                    TotalMoney = order.TotalMoney,
                    Amount = order.Amount,
                    TotalMatchCount = order.TotalMatchCount,
                    TicketStatus = Core.TicketStatus.Error,// order.TicketStatus,
                    BonusStatus = order.BonusStatus,
                    AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                    CanChase = order.CanChase,
                    IsVirtualOrder = order.IsVirtualOrder,
                    CreateTime = order.CreateTime,
                    PreTaxBonusMoney = order.PreTaxBonusMoney,
                    ProgressStatus = Core.ProgressStatus.Aborted, // order.ProgressStatus,
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
                    DistributionWay = AddMoneyDistributionWay.Average,
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
                    RedBagMoney = order.RedBagMoney,
                    IsAppend = order.IsAppend,
                    IsPayRebate = order.IsPayRebate,
                    MaxBonusMoney = order.MaxBonusMoney,
                    MinBonusMoney = order.MinBonusMoney,
                    QueryTicketStopTime = order.QueryTicketStopTime,
                    RealPayRebateMoney = order.RealPayRebateMoney,
                    TicketTime = order.TicketTime,
                    TotalPayRebateMoney = order.TotalPayRebateMoney,
                    IsSplitTickets = order.IsSplitTickets,
                };
                sportsManager.AddSports_Order_Complate(complateOrder);
                sportsManager.DeleteSports_Order_Running(order);

                var chaseOrder = sportsManager.QueryLotteryScheme(schemeId);
                chaseOrder.IsComplate = true;
                sportsManager.UpdateLotteryScheme(chaseOrder);

                #endregion

                //清理冻结
                BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.UserId, chaseOrder.KeyLine, "取消追号，清除冻结资金", order.TotalMoney);

                //返还投注资金
                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_ChaseBack, order.UserId, chaseOrder.KeyLine, order.TotalMoney, string.Format("取消追号，返还投注资金{0:N2}元", order.TotalMoney));

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询未成功的追号订单
        /// </summary>
        public string QueryNoChaseOrder()
        {
            return new Sports_Manager().QueryNoChaseOrder();
        }

        public string QueryPrizedIssuseList(string gameCode, string gameType, int length)
        {
            return new LotteryGameManager().QueryPrizedIssuseList(gameCode, gameType, length);
        }
        public string QueryStopIssuseList(string gameCode, string gameType, int length)
        {
            return new LotteryGameManager().QueryStopIssuseList(gameCode, gameType, length);
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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                    var schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        sportsManager.AddSports_AnteCode(new Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = BonusStatus.Waitting,
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
                        sportsManager.AddLotteryScheme(new LotteryScheme
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

                    sportsManager.AddUserSaveOrder(new UserSaveOrder
                    {
                        SchemeId = schemeId,
                        UserId = userId,
                        GameCode = info.GameCode,
                        GameType = string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                        PlayType = string.Empty,
                        SchemeType = SchemeType.SaveScheme,
                        SchemeSource = info.SchemeSource,
                        SchemeBettingCategory = info.BettingCategory,
                        ProgressStatus = ProgressStatus.Waitting,
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

                biz.CommitTran();
            }
            return keyLine;
        }

        private void CheckOZBMatch(string gameType, string anteCode)
        {
            var matchIdArray = anteCode.Split(',');
            var matchList = new JCZQMatchManager().QueryJCZQ_OZBMatchList(gameType, matchIdArray);
            if (matchList.Where(p => p.BetState != "开售").Count() > 0)
                throw new Exception("比赛中有包括未开售或过期的比赛");
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

        private DateTime GetOZB_StopBetTime(string gameType)
        {
            switch (gameType.ToUpper())
            {
                case "GJ":
                    return new DateTime(2016, 7, 11, 0, 0, 0);
                case "GYJ":
                    return new DateTime(2016, 7, 8, 0, 0, 0);
                default:
                    break;
            }
            return DateTime.Now;
        }

        /// <summary>
        /// 欧洲杯投注
        /// </summary>
        public string BetOZB(LotteryBettingInfo info, string userId, string balancePassword, string place, decimal redBagMoney)
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

            if (info.GameCode != "OZB")
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
                    CheckOZBMatch(item.GameType, item.AnteCode);
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
                if (currentIssuse.Status != IssuseStatus.OnSale)
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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();

                var totalBetMoney = 0M;
                var issuse = info.IssuseNumberList[0];
                if (string.IsNullOrEmpty(schemeId))
                    schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                lock (schemeId)
                {
                    var anteCodeList = new List<Sports_AnteCode>();
                    var gameTypeList = new List<string>();
                    foreach (var item in info.AnteCodeList)
                    {
                        var codeEntity = new Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = BonusStatus.Waitting,
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

                    var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                    var entity = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", gameTypeList.ToArray()),
                          string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, GetOZB_StopBetTime(gameType), info.SchemeSource, info.Security,
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

                biz.CommitTran();
            }

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


        private List<JCZQ_SJBMatch> CheckSJBMatch(string gameType, string anteCode)
        {
            var matchIdArray = anteCode.Split(',');
            var matchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(gameType, matchIdArray);
            return matchList;
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
                if (currentIssuse.Status != IssuseStatus.OnSale)
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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();

                var totalBetMoney = 0M;
                var issuse = info.IssuseNumberList[0];
                if (string.IsNullOrEmpty(schemeId))
                    schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                lock (schemeId)
                {
                    var anteCodeList = new List<Sports_AnteCode>();
                    var gameTypeList = new List<string>();
                    foreach (var item in info.AnteCodeList)
                    {
                        var codeEntity = new Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = BonusStatus.Waitting,
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

                    var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
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

                biz.CommitTran();
            }

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
        #endregion

        #region 单式上传相关

        /// <summary>
        /// 单式上传
        /// </summary>
        public string SingleScheme(SingleSchemeInfo info, string userId, string balancePassword, decimal redBagMoney)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            SingleScheme_AnteCode anteCodeSingle = null;
            Sports_Order_Running runningOrder = null;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            var selectMatchIdArray = info.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = info.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //if (!File.Exists(info.AnteCodeFullFileName))
            //    throw new Exception("文件不存在： " + info.AnteCodeFullFileName);
            //var codeText = File.ReadAllText(info.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(info.FileBuffer);
            //投注注数
            var betCount = 0;
            //所选比赛场数
            var totalMatchCount = 0;
            //所选比赛编号
            var selectMatchId = string.Empty;
            var jcCodeList = new List<string>();
            switch (info.GameCode)
            {
                case "JCZQ":
                case "JCLQ":
                case "BJDC":
                    var matchIdList = new List<string>();
                    jcCodeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, info.PlayType, info.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
                    if (jcCodeList.Count * 2M * info.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = jcCodeList.Count;
                    totalMatchCount = info.ContainsMatchId ? matchIdList.Count : selectMatchIdArray.Length;
                    selectMatchId = info.ContainsMatchId ? string.Join(",", matchIdList.ToArray()) : info.SelectMatchId;
                    selectMatchIdArray = info.ContainsMatchId ? matchIdList.ToArray() : selectMatchIdArray;
                    break;
                case "CTZQ":
                    var ctzqMatchIdList = new List<string>();
                    var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, info.GameType, allowCodeArray, out ctzqMatchIdList);
                    if (ctzqCodeList.Count * 2M * info.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = ctzqCodeList.Count;
                    totalMatchCount = ctzqMatchIdList.Count;
                    selectMatchIdArray = ctzqMatchIdList.ToArray();
                    selectMatchId = string.Join(",", selectMatchIdArray);
                    break;
            }


            //获取投注结束时间
            var stopTime = DateTime.Now;
            var sportsManager = new Sports_Manager();

            #region 计算投注结束时间

            switch (info.GameCode)
            {
                case "BJDC":
                    var matchIdArray = (from l in selectMatchIdArray select string.Format("{0}|{1}", info.IssuseNumber, l)).ToArray();
                    var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                    if (matchList.Count != matchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (matchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = matchList.Min(m => m.LocalStopTime);
                    break;
                case "JCZQ":
                    var jczqDsMatchList = sportsManager.QueryJCZQDSSaleMatchCount(selectMatchIdArray);
                    if (jczqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jczqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCZQ_Single(info.GameCode, info.GameType, jcCodeList, jczqDsMatchList);


                    stopTime = jczqDsMatchList.Min(m => m.DSStopBettingTime);
                    break;
                case "JCLQ":
                    var jclqDsMatchList = sportsManager.QueryJCLQDSSaleMatchCount(selectMatchIdArray);
                    if (jclqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jclqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCLQ_Single(info.GameCode, info.GameType, jcCodeList, jclqDsMatchList);

                    stopTime = jclqDsMatchList.Min(m => m.DSStopBettingTime);
                    break;
                case "CTZQ":
                    var issuse = new LotteryGameManager().QueryGameIssuseByKey(info.GameCode, info.GameType, info.IssuseNumber);
                    if (issuse == null)
                        throw new Exception(string.Format("{0},{1}奖期{2}不存在", info.GameCode, info.GameType, info.IssuseNumber));
                    if (issuse.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("{0},{1}奖期{2}结束时间为{3}", info.GameCode, info.GameType, info.IssuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    stopTime = issuse.LocalStopTime;
                    break;
            }

            #endregion

            var schemeId = info.SchemeId;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                if (string.IsNullOrEmpty(schemeId))
                    schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                //var buffer = Encoding.UTF8.GetBytes(codeText);
                anteCodeSingle = new SingleScheme_AnteCode
                {
                    SchemeId = schemeId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    IssuseNumber = info.IssuseNumber,
                    CreateTime = DateTime.Now,
                    //AnteCodeFullFileName = info.AnteCodeFullFileName,
                    FileBuffer = info.FileBuffer,
                    AllowCodes = info.AllowCodes,
                    ContainsMatchId = info.ContainsMatchId,
                    SelectMatchId = selectMatchId,
                };
                sportsManager.AddSingleScheme_AnteCode(anteCodeSingle);

                #region 存入号码表

                foreach (var item in selectMatchIdArray)
                {
                    //获取过滤订单的母单信息
                    var code = info.AnteCodeList.Where(p => p.MatchId == item && p.AnteCode != "*").Select(s => s.AnteCode);
                    if (code == null || code.ToList().Count() <= 0)
                        continue;
                    string anteCode = string.Join(",", code.ToList());

                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        GameCode = info.GameCode,
                        GameType = info.GameType,
                        PlayType = info.PlayType,
                        BonusStatus = BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        IssuseNumber = info.IssuseNumber,
                        SchemeId = schemeId,
                        MatchId = item,
                        Odds = string.Empty,
                        //AnteCode = code == null ? string.Empty : anteCode,
                        AnteCode = string.Empty,//单式上传不记录当前字段
                        IsDan = false,
                    });
                }

                #endregion

                //var userManager = new UserBalanceManager();
                //var user = userManager.LoadUserRegister(userId);

                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount,
                   totalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security, SchemeType.GeneralBetting, true, false, user.UserId,
                   user.AgentId, info.CurrentBetTime, info.ActivityType, "", false, redBagMoney, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                if (redBagMoney > 0M)
                {
                    var fundManager = new FundManager();
                    var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                    var maxUseMoney = info.TotalMoney * percent / 100;
                    if (redBagMoney > maxUseMoney)
                        throw new Exception(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
                    //红包支付
                    BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, string.Format("{0}单式上传消费{1:N2}元", BusinessHelper.FormatGameCode(info.GameCode), redBagMoney), "Bet", balancePassword);
                }
                // 消费资金
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney - redBagMoney,
                     string.Format("{0}单式上传消费{1:N2}元", BusinessHelper.FormatGameCode(info.GameCode), info.TotalMoney - redBagMoney), "Bet", balancePassword);

                biz.CommitTran();
            }

            #region 拆票

            if (RedisHelper.EnableRedis)
            {
                var reidsWaitOrder = new RedisWaitTicketOrderSingle
                {
                    AnteCode = anteCodeSingle,
                    RunningOrder = runningOrder,
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, reidsWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(reidsWaitOrder);
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

        private void CheckPrivilegesType_JCZQ_Single(string gameCode, string gameType, List<string> jcCodeList, List<JCZQ_Match> jczqDsMatchList)
        {
            //PrivilegesType
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 10：不让球胜平负过关
            foreach (var code in jcCodeList)
            {
                var _array = code.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in _array)
                {
                    var oneCode = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (oneCode.Length != 3) continue;
                    var _matchId = oneCode[0];
                    var _playType = oneCode[2];

                    var privileType = string.Empty;
                    switch (gameType)
                    {
                        case "SPF":
                            privileType = _playType == "1_1" ? "1" : "5";
                            break;
                        case "BRQSPF":
                            privileType = _playType == "1_1" ? "9" : "10";
                            break;
                        case "BF":
                            privileType = _playType == "1_1" ? "2" : "6";
                            break;
                        case "ZJQ":
                            privileType = _playType == "1_1" ? "3" : "7";
                            break;
                        case "BQC":
                            privileType = _playType == "1_1" ? "4" : "8";
                            break;
                        default:
                            break;
                    }

                    var temp = jczqDsMatchList.FirstOrDefault(p => p.MatchId == _matchId);
                    var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                        throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), _playType == "1_1" ? "单关" : "过关"));
                }
            }
        }
        private void CheckPrivilegesType_JCLQ_Single(string gameCode, string gameType, List<string> jcCodeList, List<JCLQ_Match> jclqDsMatchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in jcCodeList)
            {
                var _array = code.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in _array)
                {
                    var oneCode = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (oneCode.Length != 3) continue;
                    var _matchId = oneCode[0];
                    var _playType = oneCode[2];

                    var privileType = string.Empty;
                    switch (gameType)
                    {
                        case "SF":
                            privileType = _playType == "1_1" ? "1" : "5";
                            break;
                        case "RFSF":
                            privileType = _playType == "1_1" ? "2" : "6";
                            break;
                        case "SFC":
                            privileType = _playType == "1_1" ? "3" : "7";
                            break;
                        case "DXF":
                            privileType = _playType == "1_1" ? "4" : "8";
                            break;
                        default:
                            break;
                    }

                    var temp = jclqDsMatchList.FirstOrDefault(p => p.MatchId == _matchId);
                    var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                        throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), _playType == "1_1" ? "单关" : "过关"));
                }
            }
        }

        /// <summary>
        /// 创建单式上传的合买
        /// </summary>
        public string CreateSingleSchemeTogether(SingleScheme_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop
            , out bool canChase, out DateTime stopTime, ref Sports_BetingInfo schemeInfo)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            SingleScheme_AnteCode anteCodeSingle = null;
            Sports_Order_Running runningOrder = null;

            info.BettingInfo.GameCode = info.BettingInfo.GameCode.ToUpper();
            info.BettingInfo.GameType = info.BettingInfo.GameType.ToUpper();
            info.BettingInfo.PlayType = info.BettingInfo.PlayType.ToUpper();

            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new Exception("方案拆分不正确");
            if (info.Subscription < 1)
                throw new Exception("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new Exception("发起者认购份数和保底份数不能超过总份数");

            var selectMatchIdArray = info.BettingInfo.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = info.BettingInfo.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //if (!File.Exists(info.BettingInfo.AnteCodeFullFileName))
            //    throw new Exception("文件不存在： " + info.BettingInfo.AnteCodeFullFileName);
            //var codeText = File.ReadAllText(info.BettingInfo.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(info.BettingInfo.FileBuffer);
            //投注注数
            var betCount = 0;
            //所选比赛场数
            var totalMatchCount = 0;
            //所选比赛编号
            var selectMatchId = string.Empty;
            var jcCodeList = new List<string>();
            switch (info.BettingInfo.GameCode)
            {
                case "JCZQ":
                case "JCLQ":
                case "BJDC":
                    var matchIdList = new List<string>();
                    jcCodeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, info.BettingInfo.PlayType, info.BettingInfo.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
                    if (jcCodeList.Count * 2M * info.BettingInfo.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = jcCodeList.Count;
                    totalMatchCount = info.BettingInfo.ContainsMatchId ? matchIdList.Count : selectMatchIdArray.Length;
                    selectMatchId = info.BettingInfo.ContainsMatchId ? string.Join(",", matchIdList.ToArray()) : info.BettingInfo.SelectMatchId;
                    selectMatchIdArray = info.BettingInfo.ContainsMatchId ? matchIdList.ToArray() : selectMatchIdArray;
                    break;
                case "CTZQ":
                    var ctzqMatchIdList = new List<string>();
                    var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, info.BettingInfo.GameType, allowCodeArray, out ctzqMatchIdList);
                    if (ctzqCodeList.Count * 2M * info.BettingInfo.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = ctzqCodeList.Count;
                    totalMatchCount = ctzqMatchIdList.Count;
                    selectMatchIdArray = ctzqMatchIdList.ToArray();
                    selectMatchId = string.Join(",", selectMatchIdArray);
                    break;
            }

            var schemeId = string.IsNullOrEmpty(info.BettingInfo.SchemeId) ? BusinessHelper.GetTogetherBettingSchemeId() : info.BettingInfo.SchemeId;
            var sportsManager = new Sports_Manager();

            var issuseNumberOrTime = info.BettingInfo.IssuseNumber;
            #region 计算投注结束时间

            stopTime = DateTime.Now;
            switch (info.BettingInfo.GameCode)
            {
                case "BJDC":
                    var matchIdArray = (from l in selectMatchIdArray select string.Format("{0}|{1}", info.BettingInfo.IssuseNumber, l)).ToArray();
                    var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                    if (matchList.Count != matchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (matchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = matchList.Min(m => m.LocalStopTime);
                    break;
                case "JCZQ":
                    var jczqDsMatchList = sportsManager.QueryJCZQDSSaleMatchCount(selectMatchIdArray);
                    if (jczqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jczqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCZQ_Single(info.BettingInfo.GameCode, info.BettingInfo.GameType, jcCodeList, jczqDsMatchList);

                    stopTime = jczqDsMatchList.Min(m => m.DSStopBettingTime);
                    issuseNumberOrTime = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case "JCLQ":
                    var jclqDsMatchList = sportsManager.QueryJCLQDSSaleMatchCount(selectMatchIdArray);
                    if (jclqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jclqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCLQ_Single(info.BettingInfo.GameCode, info.BettingInfo.GameType, jcCodeList, jclqDsMatchList);

                    stopTime = jclqDsMatchList.Min(m => m.DSStopBettingTime);

                    issuseNumberOrTime = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case "CTZQ":
                    var issuse = new LotteryGameManager().QueryGameIssuseByKey(info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber);
                    if (issuse == null)
                        throw new Exception(string.Format("{0},{1}奖期{2}不存在", info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber));
                    if (issuse.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("{0},{1}奖期{2}结束时间为{3}", info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    stopTime = issuse.LocalStopTime;
                    break;
            }

            #endregion

            //var userManager = new UserBalanceManager();
            //var user = userManager.QueryUserRegister(userId);
            //if (!user.IsEnable)
            //    throw new Exception("用户已禁用");


            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                //添加合买信息
                var main = AddTogetherInfo(info, schemeId, info.TotalCount, info.TotalMoney, info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.PlayType, info.BettingInfo.SchemeSource, info.BettingInfo.Security,
                    selectMatchIdArray.Length, stopTime, true, schemeDeduct, user.UserId, user.AgentId,
                    balancePassword, sysGuarantees, isTop, SchemeBettingCategory.SingleBetting, issuseNumberOrTime, out canChase);
                //schemeId = main.SchemeId;

                //添加订单信息
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingInfo.BettingCategory, info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.PlayType, true, info.BettingInfo.IssuseNumber,
                    info.BettingInfo.Amount, betCount, selectMatchIdArray.Length, info.TotalMoney, stopTime, info.BettingInfo.SchemeSource, info.BettingInfo.Security, SchemeType.TogetherBetting,
                    canChase, false, user.UserId, user.AgentId, info.BettingInfo.CurrentBetTime, info.BettingInfo.ActivityType, "", false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                //添加投注号码信息
                //var buffer = Encoding.UTF8.GetBytes(codeText);
                anteCodeSingle = new SingleScheme_AnteCode
                {
                    SchemeId = schemeId,
                    GameCode = info.BettingInfo.GameCode,
                    GameType = info.BettingInfo.GameType,
                    PlayType = info.BettingInfo.PlayType,
                    IssuseNumber = info.BettingInfo.IssuseNumber,
                    CreateTime = DateTime.Now,
                    //AnteCodeFullFileName = info.BettingInfo.AnteCodeFullFileName,
                    FileBuffer = info.BettingInfo.FileBuffer,
                    AllowCodes = info.BettingInfo.AllowCodes,
                    ContainsMatchId = info.BettingInfo.ContainsMatchId,
                    SelectMatchId = selectMatchId,
                };
                sportsManager.AddSingleScheme_AnteCode(anteCodeSingle);

                #region 存入号码表

                //if (info.BettingInfo.GameCode == "JCZQ" || info.BettingInfo.GameCode == "JCLQ")
                //{
                //foreach (var item in selectMatchIdArray)
                //{
                //    //获取过滤订单的母单信息
                //    var code = info.BettingInfo.AnteCodeList.FirstOrDefault(p => p.MatchId == item);
                //    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                //    {
                //        GameCode = info.BettingInfo.GameCode,
                //        GameType = info.BettingInfo.GameType,
                //        PlayType = info.BettingInfo.PlayType,
                //        BonusStatus = BonusStatus.Waitting,
                //        CreateTime = DateTime.Now,
                //        IssuseNumber = info.BettingInfo.IssuseNumber,
                //        SchemeId = schemeId,
                //        MatchId = item,
                //        Odds = string.Empty,
                //        AnteCode = code == null ? string.Empty : code.AnteCode,
                //        IsDan = false,
                //    });
                //}
                //}

                foreach (var item in selectMatchIdArray)
                {
                    //获取过滤订单的母单信息
                    var code = info.BettingInfo.AnteCodeList.Where(p => p.MatchId == item && p.AnteCode != "*").Select(s => s.AnteCode);
                    if (code == null || code.ToList().Count() <= 0)
                        continue;
                    string anteCode = string.Join(",", code.ToList());

                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        GameCode = info.BettingInfo.GameCode,
                        GameType = info.BettingInfo.GameType,
                        PlayType = info.BettingInfo.PlayType,
                        BonusStatus = BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        IssuseNumber = info.BettingInfo.IssuseNumber,
                        SchemeId = schemeId,
                        MatchId = item,
                        Odds = string.Empty,
                        AnteCode = code == null ? string.Empty : anteCode,
                        IsDan = false,
                    });
                }

                #endregion

                schemeInfo.GameCode = info.BettingInfo.GameCode;
                schemeInfo.GameType = info.BettingInfo.GameType;
                schemeInfo.IssuseNumber = info.BettingInfo.IssuseNumber;
                schemeInfo.TotalMoney = info.TotalMoney;
                schemeInfo.SoldCount = main.SoldCount;
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }

            #region 拆票

            if (canChase)
            {
                if (RedisHelper.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrderSingle
                    {
                        AnteCode = anteCodeSingle,
                        RunningOrder = runningOrder,
                    };
                    RedisOrderBusiness.AddOrderToRedis(info.BettingInfo.GameCode, redisWaitOrder);
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

        /// <summary>
        /// 查询单式上传的订单投注号码
        /// </summary>
        public string QuerySingleSchemeAnteCode(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var entity = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (entity == null) return string.Empty;
            return string.Empty;
            //return File.ReadAllText(entity.AnteCodeFullFileName, Encoding.UTF8);
        }
        /// <summary>
        /// 查询单式上传的文件路径
        /// </summary>
        public SingleScheme_AnteCodeQueryInfo QuerySingleSchemeFullFileName(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var entity = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (entity == null) return new SingleScheme_AnteCodeQueryInfo();
            return new SingleScheme_AnteCodeQueryInfo
            {
                AllowCodes = entity.AllowCodes,
                PlayType = entity.PlayType,
                //AnteCodeFullFileName = entity.AnteCodeFullFileName,
                ContainsMatchId = entity.ContainsMatchId,
                CreateTime = entity.CreateTime,
                SchemeId = entity.SchemeId,
                SelectMatchId = entity.SelectMatchId,
                FileBuffer = entity.FileBuffer,

            };
        }

        /// <summary>
        /// 保存用户未投注订单
        /// </summary>
        public string SaveOrderSingleScheme(SingleSchemeInfo info, string userId)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            var selectMatchIdArray = info.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = info.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //if (!File.Exists(info.AnteCodeFullFileName))
            //    throw new Exception("文件不存在： " + info.AnteCodeFullFileName);
            //var codeText = File.ReadAllText(info.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(info.FileBuffer);
            //投注注数
            var betCount = 0;
            //所选比赛场数
            var totalMatchCount = 0;
            //所选比赛编号
            var selectMatchId = string.Empty;
            switch (info.GameCode)
            {
                case "JCZQ":
                case "JCLQ":
                case "BJDC":
                    var matchIdList = new List<string>();
                    var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, info.PlayType, info.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
                    if (codeList.Count * 2M * info.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = codeList.Count;
                    totalMatchCount = info.ContainsMatchId ? matchIdList.Count : selectMatchIdArray.Length;
                    selectMatchId = info.ContainsMatchId ? string.Join(",", matchIdList.ToArray()) : info.SelectMatchId;
                    selectMatchIdArray = info.ContainsMatchId ? matchIdList.ToArray() : selectMatchIdArray;
                    break;
                case "CTZQ":
                    var ctzqMatchIdList = new List<string>();
                    var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, info.GameType, allowCodeArray, out ctzqMatchIdList);
                    if (ctzqCodeList.Count * 2M * info.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = ctzqCodeList.Count;
                    totalMatchCount = ctzqMatchIdList.Count;
                    selectMatchIdArray = ctzqMatchIdList.ToArray();
                    selectMatchId = string.Join(",", selectMatchIdArray);
                    break;
            }
            var schemeId = string.Empty;
            //开启事务


            //获取投注结束时间
            var stopTime = DateTime.Now;
            var sportsManager = new Sports_Manager();

            #region 计算投注结束时间

            switch (info.GameCode)
            {
                case "BJDC":
                    var matchIdArray = (from l in selectMatchIdArray select string.Format("{0}|{1}", info.IssuseNumber, l)).ToArray();
                    var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                    if (matchList.Count != matchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (matchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = matchList.Min(m => m.LocalStopTime);
                    break;
                case "JCZQ":
                    var jczqDsMatchList = sportsManager.QueryJCZQDSSaleMatchCount(selectMatchIdArray);
                    if (jczqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jczqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = jczqDsMatchList.Min(m => m.DSStopBettingTime);
                    break;
                case "JCLQ":
                    var jclqDsMatchList = sportsManager.QueryJCLQDSSaleMatchCount(selectMatchIdArray);
                    if (jclqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jclqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = jclqDsMatchList.Min(m => m.DSStopBettingTime);
                    break;
                case "CTZQ":
                    var issuse = new LotteryGameManager().QueryGameIssuseByKey(info.GameCode, info.GameType, info.IssuseNumber);
                    if (issuse == null)
                        throw new Exception(string.Format("{0},{1}奖期{2}不存在", info.GameCode, info.GameType, info.IssuseNumber));
                    if (issuse.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("{0},{1}奖期{2}结束时间为{3}", info.GameCode, info.GameType, info.IssuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    stopTime = issuse.LocalStopTime;
                    break;
            }

            #endregion

            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();


                schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                //var buffer = Encoding.UTF8.GetBytes(codeText);
                sportsManager.AddSingleScheme_AnteCode(new SingleScheme_AnteCode
                {
                    SchemeId = schemeId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    IssuseNumber = info.IssuseNumber,
                    CreateTime = DateTime.Now,
                    //AnteCodeFullFileName = info.AnteCodeFullFileName,
                    FileBuffer = info.FileBuffer,
                    AllowCodes = info.AllowCodes,
                    ContainsMatchId = info.ContainsMatchId,
                    SelectMatchId = selectMatchId,
                });

                #region 存入号码表

                //if (info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
                //{
                foreach (var item in selectMatchIdArray)
                {
                    //获取过滤订单的母单信息
                    var code = info.AnteCodeList.FirstOrDefault(p => p.MatchId == item);
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        GameCode = info.GameCode,
                        GameType = info.GameType,
                        PlayType = info.PlayType,
                        BonusStatus = BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        IssuseNumber = info.IssuseNumber,
                        SchemeId = schemeId,
                        MatchId = item,
                        Odds = string.Empty,
                        AnteCode = code == null ? string.Empty : code.AnteCode,
                        IsDan = false,
                    });
                }
                //}

                #endregion

                //var userManager = new UserBalanceManager();
                //var user = userManager.LoadUserRegister(userId);

                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount,
                   totalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security, SchemeType.SaveScheme, false, true,
                   user.UserId, user.AgentId, stopTime, info.ActivityType, "", false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                sportsManager.AddUserSaveOrder(new UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = SchemeType.SaveScheme,
                    SchemeSource = info.SchemeSource,
                    SchemeBettingCategory = info.BettingCategory,
                    ProgressStatus = ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                });

                biz.CommitTran();
            }
            return schemeId;
        }

        #endregion

        #region 奖金优化相关

        /// <summary>
        /// 奖金优化投注
        /// </summary>
        public string YouHuaBet(Sports_BetingInfo info, string userId, string password, decimal realTotalMoney, int betCount, DateTime stopTime, decimal redBagMoney)
        {
            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;
            if (string.IsNullOrEmpty(info.Attach))
                throw new Exception("投注内容不完整");
            if (info.TotalMoney % 2 != 0 || realTotalMoney % 2 != 0)
                throw new AggregateException("订单金额不正确，应该为2的倍数");

            schemeId = string.IsNullOrEmpty(info.SchemeId) ? BusinessHelper.GetSportsBettingSchemeId(gameCode) : info.SchemeId;

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new Exception("用户已禁用");

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, 1, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    var entity = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BusinessHelper.FormatGameCode(info.GameCode));
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

                biz.CommitTran();
            }

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
                        if (!BusinessHelper.CheckAnteCode(matchArray[1], matchArray[2]))
                            throw new Exception("投注内容格式不正确");
                    }
                }
            }
            if (codeMoney != realTotalMoney)
                throw new Exception(string.Format("优化金额不正确，应为{0}，实际为{1}", codeMoney, realTotalMoney));
        }

        private DateTime CheckYouHuaBetInfo(YouHuaBetInfo info)
        {
            var allowGameCode = new string[] { "JCZQ" };
            if (!allowGameCode.Contains(info.GameCode))
                throw new Exception("该彩种暂时不支持奖金优化投注");

            var totalMoney = 0M;
            Sports_Manager sportsManager = new Sports_Manager();
            var ac = new ArrayCombination();
            var c = new Combination();
            var stopTime = DateTime.Now;
            foreach (var item in info.AnteCodeList)
            {
                //一注号，如：帕纳[负胜]x特拉[让胜]=201.6
                var a = int.Parse(item.PlayType.Split('_')[0]);
                var b = int.Parse(item.PlayType.Split('_')[1]);
                if (b != 1)
                    throw new Exception("过关方式不正确");
                if (a != item.MatchList.Count)
                    throw new Exception("过关方式与所选场次数不匹配");

                if (info.GameCode == "JCZQ")
                {
                    #region JCZQ验证

                    var matchIdArray = (from m in item.MatchList select m.MatchId).ToArray();
                    var matchList = sportsManager.QueryJCZQSaleMatchCount(matchIdArray);
                    if (matchList.Count != matchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    //var matchResultList = sportsManager.QueryJCZQMatchResult(matchIdArray);
                    //if (matchResultList.Count > 0)
                    //    throw new ArgumentException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

                    //验证 不支持的玩法
                    foreach (var match in item.MatchList)
                    {
                        var privileType = string.Empty;
                        switch (match.GameType.ToUpper())
                        {
                            case "SPF":
                                privileType = item.PlayType == "1_1" ? "1" : "5";
                                break;
                            case "BRQSPF":
                                privileType = item.PlayType == "1_1" ? "9" : "10";
                                break;
                            case "BF":
                                privileType = item.PlayType == "1_1" ? "2" : "6";
                                break;
                            case "ZJQ":
                                privileType = item.PlayType == "1_1" ? "3" : "7";
                                break;
                            case "BQC":
                                privileType = item.PlayType == "1_1" ? "4" : "8";
                                break;
                            default:
                                break;
                        }
                        var temp = matchList.FirstOrDefault(p => p.MatchId == match.MatchId);
                        if (temp == null) continue;
                        var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                            throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(info.GameCode, match.GameType), item.PlayType == "1_1" ? "单关" : "过关"));

                        //检查投注号码
                        var msg = string.Empty;
                        var zhu = AnalyzerFactory.GetSportAnteCodeChecker(info.GameCode, match.GameType).CheckAntecodeNumber(new Sports_AnteCodeInfo
                        {
                            AnteCode = match.BetContent,
                            GameType = match.GameType,
                            IsDan = false,
                            MatchId = match.MatchId,
                            PlayType = item.PlayType,
                        }, out msg);
                        if (!string.IsNullOrEmpty(msg))
                            throw new Exception(msg);

                        stopTime = matchList.Min(m => m.FSStopBettingTime);

                    }

                    #endregion
                }
                totalMoney += item.Amount * 2M;
            }
            if (totalMoney * info.Amount != info.TotalMoney)
                throw new Exception("投注金额不正确");

            return stopTime;
        }

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

            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            //if (info.TotalCount * info.Price != info.TotalMoney)
            //    throw new Exception("方案拆分不正确");
            if (info.Subscription < 1)
                throw new Exception("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new Exception("发起者认购份数和保底份数不能超过总份数");

            var schemeId = string.IsNullOrEmpty(info.SchemeId) ? BusinessHelper.GetTogetherBettingSchemeId() : info.SchemeId;

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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                    var entity = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }


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

        #region 名家发布推荐

        public string PublishExperterScheme(Sports_BetingInfo info, string userId)
        {
            var schemeId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                info.GameCode = info.GameCode.ToUpper();
                info.GameType = info.GameType.ToUpper();
                var gameCode = info.GameCode;

                var experterScheme = new ExperterSchemeManager();
                var schemeCount = experterScheme.QueryExperterCurrentTimeScheme(userId, DateTime.Now.ToString("yyyy-MM-dd"));
                if (schemeCount >= 2)
                    throw new Exception("每天最多只能发起两个方案");

                #region 订单相关

                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                if (!user.IsEnable)
                    throw new Exception("用户已禁用");

                var experManager = new ExperterManager();
                var exper = experManager.QueryExperterById(userId);
                if (exper == null)
                    throw new Exception("当前用户不是名家，不能发布推荐");

                schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);

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
                var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);

                // 检查订单金额是否匹配
                var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime);
                if (betCount > 2)
                    throw new Exception("名家方案只能发布2注以下的推荐");
                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.ExperterScheme, true, true, user.UserId, user.AgentId, DateTime.Now, info.ActivityType, "", false, 0M, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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

                #endregion

                #region 推荐相关

                experterScheme.AddExperterScheme(new ExperterScheme
                {
                    UserId = userId,
                    Against = 0,
                    Support = 0,
                    GuestTeamComments = "",
                    HomeTeamComments = "",
                    SchemeId = schemeId,
                    ExperterType = exper.ExperterType,
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    CreateTime = DateTime.Now,
                    StopTime = stopTime,
                    BonusStatus = BonusStatus.Waitting,
                    BetMoney = info.TotalMoney,
                    BonusMoney = 0M,
                });

                #endregion

                biz.CommitTran();
            }

            return schemeId;
        }

        #endregion

        #region 合买订单处理

        private Sports_Together AddTogetherInfo(TogetherSchemeBase info, string schemeId, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
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
            sportsManager.AddTemp_Together(new Temp_Together
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                SchemeId = schemeId,
                StopTime = stopTime.ToString("yyyyMMddHHmm"),
            });

            //合买信息
            var main = new Sports_Together();
            main.BonusDeduct = info.BonusDeduct;
            main.SchemeDeduct = schemeDeduct;
            main.CreateTime = DateTime.Now;
            main.CreateUserId = userId;
            main.SchemeSource = schemeSource;
            main.SchemeBettingCategory = category;
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
            main.Security = security;
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

            var subItem = new Sports_TogetherJoin
            {
                AfterTaxBonusMoney = 0M,
                BuyCount = info.Subscription,
                RealBuyCount = info.Subscription,
                CreateTime = DateTime.Now,
                JoinType = TogetherJoinType.Subscription,
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
                var joinItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.FollowerJoin,
                    JoinUserId = item.FollowerUserId,
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = realBuyMoney,
                    JoinSucess = true,
                    JoinLog = "跟单参与合买",
                };
                sportsManager.AddSports_TogetherJoin(joinItem);

                //添加跟单记录
                sportsManager.AddTogetherFollowerRecord(new TogetherFollowerRecord
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
            main.ProgressStatus = TogetherSchemeProgress.SalesIn;

            if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Standard;
            if (main.SoldCount == main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Finish;

            #region 发起人保底

            var guaranteeMoney = info.Guarantees * info.Price;
            //扣钱
            if (guaranteeMoney > 0)
            {
                var minGuarantees = main.TotalCount - main.SoldCount;
                var guaranteeItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = info.Guarantees,
                    RealBuyCount = minGuarantees <= info.Guarantees ? minGuarantees : info.Guarantees,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.Guarantees,
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
                sportsManager.AddSports_TogetherJoin(new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = main.SystemGuarantees,
                    RealBuyCount = tempSystemGuarantees,
                    CreateTime = DateTime.Now,
                    JoinLog = "网站保底参与合买",
                    JoinSucess = false,
                    JoinType = TogetherJoinType.SystemGuarantees,
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
        private void SetTogetherIsTop(Sports_Together main)
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

        public Sports_Order_Running AddRunningOrderAndOrderDetail(string schemeId, SchemeBettingCategory category,
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
            var order = new Sports_Order_Running
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                Amount = amount,
                BonusStatus = BonusStatus.Waitting,
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
                ProgressStatus = progressStatus,
                SchemeId = schemeId,
                SchemeType = schemeType,
                SchemeBettingCategory = category,
                TicketId = string.Empty,
                TicketLog = string.Empty,
                TicketStatus = ticketStatus,
                TotalMatchCount = totalMatchCount,
                TotalMoney = totalMoney,
                SuccessMoney = totalMoney,
                UserId = userId,
                StopTime = stopTime,
                SchemeSource = schemeSource,
                BetCount = betCount,
                BonusCount = 0,
                HitMatchCount = 0,
                RightCount = 0,
                Error1Count = 0,
                Error2Count = 0,
                MaxBonusMoney = 0,
                MinBonusMoney = 0,
                Security = security,
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
            sportsManager.AddSports_Order_Running(order);

            //订单总表信息
            var orderDetail = new OrderDetail
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                BonusStatus = BonusStatus.Waitting,
                ComplateTime = null,
                CreateTime = createtime,
                CurrentBettingMoney = ticketStatus == TicketStatus.Ticketed ? totalMoney : 0M,
                GameCode = gameCode,
                GameType = gameType,
                GameTypeName = BusinessHelper.FormatGameType(gameCode, gameType),
                PreTaxBonusMoney = 0M,
                ProgressStatus = progressStatus,
                SchemeId = schemeId,
                SchemeSource = schemeSource,
                SchemeType = schemeType,
                SchemeBettingCategory = category,
                StartIssuseNumber = issuseNumber,
                StopAfterBonus = stopAfterBonus,
                TicketStatus = ticketStatus,
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
            new SchemeManager().AddOrderDetail(orderDetail);
            return order;
        }

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

            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            info.PlayType = info.PlayType.ToUpper();

            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new LogicException("方案拆分不正确");
            if (info.Subscription < 1)
                throw new LogicException("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new LogicException("发起者认购份数和保底份数不能超过总份数");

            var schemeId = string.IsNullOrEmpty(info.SchemeId) ? BusinessHelper.GetTogetherBettingSchemeId() : info.SchemeId;
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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                    var entity = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }


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
        /// 用户是否已经参与了合买
        /// </summary>
        public bool IsUserJoinSportsTogether(string schemeId, string userId)
        {
            return new Sports_Manager().IsUserJoinTogether(schemeId, userId);
        }

        public Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherList(string key, string issuseNumber, string gameCode, string gameType,
            TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
            decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize)
        {
            var sportsManager = new Sports_Manager();
            var totalCount = 0;
            var list = sportsManager.QuerySportsTogetherList(key, issuseNumber, gameCode, gameType, security, betCategory, progressState, minMoney, maxMoney, minProgress, maxProgress, orderBy, pageIndex, pageSize, out totalCount);
            var result = new Sports_TogetherSchemeQueryInfoCollection();
            result.TotalCount = totalCount;
            result.List.AddRange(list);
            return result;
        }

        public Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId)
        {
            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(schemeId);
            if (orderDetail == null)
                throw new Exception(string.Format("没有查询到方案{0}的orderDetail信息", schemeId));

            var sportsManager = new Sports_Manager();
            //var info = sportsManager.QueryRunningSportsTogetherDetail(schemeId);
            //if (info == null)
            //    info = sportsManager.QueryComplateSportsTogetherDetail(schemeId);
            var info = (orderDetail.ProgressStatus == ProgressStatus.Complate
              || orderDetail.ProgressStatus == ProgressStatus.Aborted
              || orderDetail.ProgressStatus == ProgressStatus.AutoStop) ? sportsManager.QueryComplateSportsTogetherDetail(schemeId) : sportsManager.QueryRunningSportsTogetherDetail(schemeId);
            if (info == null)
                throw new Exception(string.Format("没有查询到方案{0}的信息", schemeId));
            return info;
        }

        public Sports_TogetherJoinInfoCollection QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize)
        {
            var result = new Sports_TogetherJoinInfoCollection();
            var totalCount = 0;
            var sportsManager = new Sports_Manager();
            var list = sportsManager.QuerySportsTogetherJoinList(schemeId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            result.List.AddRange(list);
            return result;
        }
        public Sports_TogetherJoinInfoCollection QueryUserSportsTogetherJoinList(string schemeId, string userId)
        {
            var result = new Sports_TogetherJoinInfoCollection();
            var sportsManager = new Sports_Manager();
            var list = sportsManager.QueryUserSportsTogetherJoinList(schemeId, userId);
            result.List.AddRange(list);
            return result;
        }

        public Sports_TogetherJoinInfoCollection QueryNewBonusTogetherJoiner(int count)
        {
            var result = new Sports_TogetherJoinInfoCollection();
            var sportsManager = new Sports_Manager();
            var list = sportsManager.QueryNewBonusTogetherJoiner(count);
            result.TotalCount = list.Count;
            result.List.AddRange(list);
            return result;
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
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                sportsManager.AddUserSaveOrder(new UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = SchemeType.SaveScheme,
                    SchemeSource = info.SchemeSource,
                    SchemeBettingCategory = info.BettingCategory,
                    ProgressStatus = ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    //StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),//20160225 根据比赛截止时间处理保存订单，出票中心发现是保存订单不做过期撤单处理
                    StrStopTime = stopTime.ToString("yyyyMMddHHmm"),
                });

                biz.CommitTran();
            }
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

            Sports_Order_Running runningOrder = null;
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
            if (main.ProgressStatus != TogetherSchemeProgress.SalesIn && main.ProgressStatus != TogetherSchemeProgress.Standard) throw new Exception("合买已完成，不能参与");
            if (!string.IsNullOrEmpty(main.JoinPwd) && (string.IsNullOrEmpty(joinPwd) || Encipherment.MD5(joinPwd) != main.JoinPwd))
                throw new Exception("参与密码不正确");
            var surplusCount = main.TotalCount - main.SoldCount;
            if (surplusCount < buyCount)
                throw new Exception(string.Format("方案剩余份数不足{0}份", buyCount));

            var buyMoney = main.Price * buyCount;
            if (buyMoney < 1)
                throw new Exception("参与金额最少为1元");

            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                main.SoldCount += buyCount;
                main.JoinUserCount += sportsManager.IsUserJoinTogether(schemeId, userId) ? 0 : 1;
                main.Progress = (decimal)main.SoldCount / main.TotalCount;
                if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Standard;
                if (main.SoldCount == main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Finish;
                //不需要系统保底
                //if (main.SoldCount + main.Guarantees >= main.TotalCount)
                //    main.SystemGuarantees = 0;

                var joinItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    TotalMoney = buyMoney,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    Price = main.Price,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.Join,
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
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }

            #region 拆票

            if (canChase)
            {
                if (RedisHelper.EnableRedis)
                {
                    if (runningOrder.SchemeBettingCategory == SchemeBettingCategory.SingleBetting || runningOrder.SchemeBettingCategory == SchemeBettingCategory.XianFaQiHSC)
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

        /// <summary>
        /// 退出合买
        /// </summary>
        public void ExitTogether(string schemeId, int joinId, string joinUserId)
        {
            var sportsManager = new Sports_Manager();
            var main = sportsManager.QuerySports_Together(schemeId);
            if (DateTime.Now >= main.StopTime)
                throw new Exception(string.Format("合买结束时间是{0}，现在不能退出合买。", main.StopTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if ((main.SoldCount + main.Guarantees) / main.TotalCount >= 0.8M)
                throw new Exception("合买进度已超过80%，不能退出合买");

            var sportsOrder = sportsManager.QuerySports_Order_Running(schemeId);
            if (sportsOrder.TicketStatus == TicketStatus.Ticketed)
                throw new Exception("已出票订单不能撤销");

            var join = sportsManager.QuerySports_TogetherJoin(schemeId, joinId);
            if (join == null)
                throw new Exception("参与记录为空");
            if (join.JoinUserId != joinUserId)
                throw new Exception("参与记录不是此用户的数据");
            if (!join.JoinSucess)
                throw new Exception("已成功退出合买");
            if (join.JoinType == TogetherJoinType.Guarantees)
                throw new Exception("保底不能撤销");
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();


                //if (join.JoinType == TogetherJoinType.FollowerJoin)
                //    throw new Exception("订制跟单不能撤销");

                var existList = new List<Sports_TogetherJoin>();

                bool IsCancel = false;//是否撤销合买
                if (join.JoinType == TogetherJoinType.Subscription)
                {
                    //整个订单撤销
                    existList.AddRange(sportsManager.QuerySports_TogetherSucessJoin(schemeId));
                    IsCancel = true;
                }
                else
                {
                    existList.Add(join);
                }

                if (existList.Count == 0)
                    throw new Exception("没有要退出的参与记录");

                foreach (var item in existList)
                {
                    if (item.JoinType == TogetherJoinType.SystemGuarantees) continue;
                    //撤销参与记录
                    item.JoinSucess = false;
                    item.JoinLog += " 退出合买";
                    sportsManager.UpdateSports_TogetherJoin(item);
                    //真实参与金额
                    var joinMoney = item.RealBuyCount * item.Price;
                    //退钱
                    if (joinMoney > 0)
                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_CancelOrder, item.JoinUserId, string.Format("{0}_{1}", schemeId, item.Id),
                            joinMoney, string.Format("退出合买。返还参与资金{0:N2}元", joinMoney));
                }

                var joinUserCount = sportsManager.QueryTogetherJoinUserCount(schemeId);
                main.JoinUserCount = joinUserCount;
                main.SoldCount -= existList.Sum(p => p.RealBuyCount);

                if (existList.Count > 1 || IsCancel)
                {
                    //全部撤销
                    main.ProgressStatus = TogetherSchemeProgress.Cancel;
                    main.Progress = 0M;

                    var temp = sportsManager.QueryTemp_Together(schemeId);
                    if (temp != null)
                        sportsManager.DeleteTemp_Together(temp);

                    var manager = new SchemeManager();
                    var orderDetail = manager.QueryOrderDetail(schemeId);
                    orderDetail.ProgressStatus = ProgressStatus.Aborted;
                    orderDetail.ComplateTime = DateTime.Now;
                    orderDetail.BonusStatus = BonusStatus.Lose;
                    orderDetail.TicketStatus = TicketStatus.Error;
                    orderDetail.CurrentBettingMoney = 0M;
                    orderDetail.IsVirtualOrder = true;
                    manager.UpdateOrderDetail(orderDetail);

                    sportsOrder.BonusStatus = BonusStatus.Lose;
                    sportsOrder.CanChase = false;
                    sportsOrder.IsVirtualOrder = true;
                    sportsOrder.TicketLog = "用户撤单" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //sportsManager.UpdateSports_Order_Running(sportsOrder);

                    OrderFailToEnd(schemeId, sportsManager, sportsOrder);

                    //var manager = new SchemeManager();
                    //var orderDetail = manager.QueryOrderDetail(schemeId);
                    //orderDetail.ProgressStatus = ProgressStatus.Running;
                    //orderDetail.ComplateTime = DateTime.Now;
                    //orderDetail.BonusStatus = BonusStatus.Lose;
                    //orderDetail.TicketStatus = TicketStatus.Waitting;
                    //orderDetail.CurrentBettingMoney = 0M;
                    //orderDetail.IsVirtualOrder = true;
                    //manager.UpdateOrderDetail(orderDetail);

                    //var sportsOrder = sportsManager.QuerySports_Order_Running(schemeId);
                    //sportsOrder.BonusStatus = BonusStatus.Lose;
                    //sportsOrder.CanChase = true;
                    //sportsOrder.ProgressStatus = ProgressStatus.Running;
                    //sportsOrder.TicketStatus = TicketStatus.Waitting;
                    //sportsOrder.IsVirtualOrder = true;
                    //sportsOrder.TicketLog = "用户撤单" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //sportsManager.UpdateSports_Order_Running(sportsOrder);
                }
                else
                {
                    main.Progress = (decimal)main.SoldCount / main.TotalCount;
                    if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                        main.ProgressStatus = TogetherSchemeProgress.Standard;
                    if (main.SoldCount == main.TotalCount)
                        main.ProgressStatus = TogetherSchemeProgress.Finish;
                }
                sportsManager.UpdateSports_Together(main);

                biz.CommitTran();
            }

            #region 发送站内消息：手机短信或站内信

            var _userManager = new UserBalanceManager();
            var _user = _userManager.QueryUserRegister(main.CreateUserId);
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "[UserName]", _user.DisplayName));
            pList.Add(string.Format("{0}={1}", "[SchemeId]", schemeId));
            //发送短信
            new SiteMessageControllBusiness().DoSendSiteMessage(_user.UserId, "", "ON_User_Exit_Together", pList.ToArray());

            #endregion
        }

        /// <summary>
        /// 撤销合买
        /// </summary>
        public void CancelTogether(string schemeId, string userId)
        {
            var sportsManager = new Sports_Manager();
            var main = sportsManager.QuerySports_Together(schemeId);
            if (main.CreateUserId != userId)
                throw new Exception("合买发起人不是该用户");
            if ((main.SoldCount + main.Guarantees) / main.TotalCount >= 0.75M)
                throw new Exception("合买进度已超过75%，不能撤销合买方案");
            if (main.ProgressStatus == TogetherSchemeProgress.Cancel)
                throw new Exception("此订单已被撤销");

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                main.ProgressStatus = TogetherSchemeProgress.Cancel;
                sportsManager.UpdateSports_Together(main);
                foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                {
                    item.JoinSucess = false;
                    item.JoinLog += " 合买发起人撤销合买方案。";
                    sportsManager.UpdateSports_TogetherJoin(item);
                    if (item.JoinType == TogetherJoinType.SystemGuarantees)
                        continue;

                    var t = string.Empty;
                    var realBuyCount = item.RealBuyCount;
                    switch (item.JoinType)
                    {
                        case TogetherJoinType.Subscription:
                            t = "认购";
                            break;
                        case TogetherJoinType.FollowerJoin:
                            t = "订制跟单";
                            break;
                        case TogetherJoinType.Join:
                            t = "参与";
                            break;
                        case TogetherJoinType.Guarantees:
                            realBuyCount = item.BuyCount;
                            t = "保底";
                            break;
                    }
                    //退钱
                    //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_CancelOrder, schemeId, schemeId, false, string.Empty,
                    //    string.Empty, item.JoinUserId, AccountType.Common, item.TotalMoney, string.Format("合买发起人撤销合买方案，返还{0}资金{1:N2}元", t, item.TotalMoney));
                    var money = item.Price * realBuyCount;
                    if (money > 0)
                        BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_CancelOrder, item.JoinUserId,
                            string.Format("{0}_{1}", schemeId, item.Id), item.TotalMoney, string.Format("合买发起人撤销合买方案，返还{0}资金{1:N2}元", t, money));
                }

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                orderDetail.ProgressStatus = ProgressStatus.Running;
                orderDetail.TicketStatus = TicketStatus.Waitting;
                orderDetail.IsVirtualOrder = true;
                orderDetail.CurrentBettingMoney = 0M;
                manager.UpdateOrderDetail(orderDetail);

                var sportsOrder = sportsManager.QuerySports_Order_Running(schemeId);
                sportsOrder.BonusStatus = BonusStatus.Lose;
                sportsOrder.CanChase = true;
                sportsOrder.ProgressStatus = ProgressStatus.Running;
                sportsOrder.TicketStatus = TicketStatus.Waitting;
                sportsOrder.TicketLog = " 合买发起人撤销合买方案。" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sportsOrder.IsVirtualOrder = true;
                sportsManager.UpdateSports_Order_Running(sportsOrder);
                //var temp = sportsManager.QueryTemp_Together(schemeId);
                //if (temp != null)
                //    sportsManager.DeleteTemp_Together(temp);

                biz.CommitTran();
            }

            #region 发送站内消息：手机短信或站内信

            var _userManager = new UserBalanceManager();
            var _user = _userManager.QueryUserRegister(main.CreateUserId);
            var pList = new List<string>();
            pList.Add(string.Format("{0}={1}", "[UserName]", _user.DisplayName));
            pList.Add(string.Format("{0}={1}", "[SchemeId]", schemeId));
            //发送短信
            new SiteMessageControllBusiness().DoSendSiteMessage(_user.UserId, "", "ON_User_Cancel_Together", pList.ToArray());

            #endregion
        }

        public string QueryWaitToProcessingTogetherSchemeId(string gameCode, string stopTime)
        {
            var array = new Sports_Manager().QueryWaitToProcessingTogetherIdList(gameCode, stopTime);
            return string.Join("|", array);
        }
        public string QueryWaitToProcessingXianFaQiHSCOrderList(string gameCode, string stopTime)
        {
            var array = new Sports_Manager().QueryWaitToProcessingXianFaQiHSCOrderList(gameCode, stopTime);
            return string.Join("|", array);
        }

        /// <summary>
        /// 分析合买
        /// </summary>
        public void AnalysisSchemeTogether(string schemeId)
        {
            Sports_Order_Running runningOrder = null;
            string errorMsg = string.Empty;
            var sportsManager = new Sports_Manager();
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsOrder = sportsManager.QuerySports_Order_Running(schemeId);
                if (sportsOrder == null)
                    throw new Exception("查询方案信息Order_Running失败");
                runningOrder = sportsOrder;
                var main = sportsManager.QuerySports_Together(schemeId);
                if (main == null)
                    throw new Exception("查询方案信息Together失败");
                //if (main.IsPayBackGuarantees)
                //    throw new Exception("订单已退还用户保底");
                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception("查询方案orderDetail 失败");
                var temp = sportsManager.QueryTemp_Together(schemeId);
                if (temp == null)
                    throw new Exception("订单已执行过分析");
                if (main.ProgressStatus == TogetherSchemeProgress.Cancel)
                    throw new Exception("订单已撤单");
                var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
                while (true)
                {
                    if (main.ProgressStatus != TogetherSchemeProgress.Finish && main.ProgressStatus != TogetherSchemeProgress.Standard)
                    {
                        errorMsg = string.Format("合买失败，截止{0}，方案奖态为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), main.ProgressStatus);
                        break;
                    }
                    if (main.SoldCount + main.Guarantees + main.SystemGuarantees < main.TotalCount)
                    {
                        errorMsg = string.Format("合买失败，截止{0}，方案销售份数、发起人保底份数、系统保底份数未达到条件。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        break;
                    }
                    if (anteCodeList == null || anteCodeList.Count <= 0)
                    {
                        errorMsg = string.Format("当前方案未上传投注内容且已过系统保存日期，截止日期{0}", temp.StopTime);
                        break;
                    }
                    break;
                }

                while (true)
                {
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        //失败
                        #region 合买失败

                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            item.JoinSucess = false;
                            item.JoinLog += errorMsg;
                            sportsManager.UpdateSports_TogetherJoin(item);
                            if (item.JoinType == TogetherJoinType.SystemGuarantees)
                                continue;

                            var t = string.Empty;
                            var realBuyCount = item.RealBuyCount;
                            switch (item.JoinType)
                            {
                                case TogetherJoinType.Subscription:
                                    t = "认购";
                                    break;
                                case TogetherJoinType.FollowerJoin:
                                    t = "订制跟单";
                                    break;
                                case TogetherJoinType.Join:
                                    t = "参与";
                                    break;
                                case TogetherJoinType.Guarantees:
                                    realBuyCount = item.BuyCount;
                                    t = "保底";
                                    break;
                            }
                            //var joinMoney = item.Price * item.RealBuyCount;
                            var joinMoney = item.Price * realBuyCount;
                            //退钱
                            if (joinMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TogetherFail, item.JoinUserId, string.Format("{0}_{1}", schemeId, item.Id)
                                    , joinMoney, string.Format("合买失败，返还{0}资金{1:N2}元", t, joinMoney));
                        }

                        //分析合买失败后撤单
                        main.IsPayBackGuarantees = true;
                        main.ProgressStatus = TogetherSchemeProgress.AutoStop;
                        sportsManager.UpdateSports_Together(main);

                        orderDetail.ProgressStatus = ProgressStatus.Aborted;
                        orderDetail.ComplateTime = DateTime.Now;
                        orderDetail.BonusStatus = BonusStatus.Lose;
                        orderDetail.TicketStatus = TicketStatus.Error;
                        orderDetail.CurrentBettingMoney = 0M;
                        orderDetail.IsVirtualOrder = true;
                        manager.UpdateOrderDetail(orderDetail);

                        sportsOrder.IsPayRebate = true;
                        sportsOrder.BonusStatus = BonusStatus.Lose;
                        sportsOrder.CanChase = false;
                        sportsOrder.ProgressStatus = ProgressStatus.Aborted;
                        sportsOrder.TicketStatus = TicketStatus.Error;
                        sportsOrder.IsVirtualOrder = true;
                        sportsOrder.TicketLog = errorMsg + "|分析合买失败撤单|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        sportsManager.UpdateSports_Order_Running(sportsOrder);

                        OrderFailToEnd(schemeId, sportsManager, sportsOrder);

                        #endregion
                        break;
                    }

                    //成功
                    //系统保底
                    #region 合买成功

                    //退还用户保底
                    var joinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                    if (joinEntity != null)
                    {
                        var tooMuchMoney = (joinEntity.BuyCount - joinEntity.RealBuyCount) * joinEntity.Price;
                        if (tooMuchMoney > 0M)
                        {
                            var summary = string.Format("计划保底{0}份，实际参与保底{1}份，返还保底资金{2:N2}元", joinEntity.BuyCount, joinEntity.RealBuyCount, tooMuchMoney);
                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_ReturnGuarantees, joinEntity.JoinUserId
                              , string.Format("{0}_{1}", schemeId, joinEntity.Id), tooMuchMoney, summary);
                        }
                    }
                    //系统保底
                    var systemGuaranteesEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                    if (systemGuaranteesEntity != null && systemGuaranteesEntity.RealBuyCount > 0)
                    {
                        systemGuaranteesEntity.JoinSucess = true;
                        sportsManager.UpdateSports_TogetherJoin(systemGuaranteesEntity);
                    }

                    //main.SystemGuarantees = (sysGuarantees > 0 ? sysGuarantees : 0);
                    main.Progress = 1M;
                    main.ProgressStatus = TogetherSchemeProgress.Finish;
                    main.SoldCount = main.TotalCount;
                    main.IsPayBackGuarantees = true;
                    sportsManager.UpdateSports_Together(main);

                    //处理参与信息
                    foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                    {
                        item.JoinLog += "  合买成功。";
                        sportsManager.UpdateSports_TogetherJoin(item);
                    }
                    if (orderDetail.TicketStatus != TicketStatus.Ticketed)
                        orderDetail.IsVirtualOrder = false;
                    manager.UpdateOrderDetail(orderDetail);
                    if (sportsOrder.TicketStatus != TicketStatus.Ticketed)
                        sportsOrder.IsVirtualOrder = false;
                    sportsManager.UpdateSports_Order_Running(sportsOrder);

                    #endregion

                    break;
                }

                //清除临时表数据
                if (temp != null)
                    sportsManager.DeleteTemp_Together(temp);

                biz.CommitTran();
            }

            if (string.IsNullOrEmpty(errorMsg))
            {
                if (RedisHelper.EnableRedis)
                {
                    if (runningOrder.SchemeBettingCategory == SchemeBettingCategory.SingleBetting || runningOrder.SchemeBettingCategory == SchemeBettingCategory.XianFaQiHSC)
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
        }

        /// <summary>
        /// 查询未退保的订单
        /// </summary>
        public string QueryFinishTogether()
        {
            var manager = new Sports_Manager();
            var pageIndex = 0;
            var pageSize = 100;
            var totalCount = 0;
            var list = new List<Sports_Together>();
            var temp = manager.QueryFinishTogether(pageIndex, pageSize, out totalCount);
            list.AddRange(temp);
            pageIndex++;
            while (pageIndex * pageSize < totalCount)
            {
                temp = manager.QueryFinishTogether(pageIndex, pageSize, out totalCount);
                list.AddRange(temp);
                pageIndex++;
            }

            return string.Join("|", list.Select(p => p.SchemeId).ToArray());
        }

        /// <summary>
        /// 处理订单退保
        /// </summary>
        public void DoPayBackGuarantees(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var together = sportsManager.QuerySports_Together(schemeId);
            if (together == null)
                throw new Exception(string.Format("打不到订单 {0}", schemeId));
            if (together.IsPayBackGuarantees)
                throw new Exception(string.Format("订单 {0}，已经完成退保", schemeId));
            if (together.ProgressStatus != TogetherSchemeProgress.Finish)
                throw new Exception(string.Format("订单 {0}，状态不正确", schemeId));

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
            sportsManager.UpdateSports_Together(together);
        }

        #endregion

        #region 用户关系

        /// <summary>
        /// 查询用户个人战绩
        /// </summary>
        public UserBeedingListInfoCollection QueryBonusUserBeedingList(string userId)
        {
            var list = new UserBeedingListInfoCollection();
            list.List.AddRange(new Sports_Manager().QueryBonusUserBeedingList(userId));
            return list;
        }

        /// <summary>
        /// 查询用户指定彩种的战绩
        /// </summary>
        public UserBeedingListInfo QueryUserBeedingListInfo(string userId, string gameCode, string gameType)
        {
            return new Sports_Manager().QueryUserBeedingListInfo(userId, gameCode, gameType);
        }

        /// <summary>
        /// 查询网站彩种战绩，自动跟单列表用
        /// </summary>
        public UserBeedingListInfoCollection QueryUserBeedingList(string gameCode, string gameType, string userId, string userDisplayName, int pageIndex, int pageSize, QueryUserBeedingListOrderByProperty property, OrderByCategory category)
        {
            if (string.IsNullOrEmpty(gameCode))
                gameCode = string.Empty;
            if (string.IsNullOrEmpty(gameType))
                gameType = string.Empty;
            if (string.IsNullOrEmpty(userDisplayName))
                userDisplayName = string.Empty;
            var result = new UserBeedingListInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserBeedingList(gameCode, gameType, userId, userDisplayName, pageIndex, pageSize, property, category, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 订制合买跟单
        /// </summary>
        public void CustomTogetherFollower(TogetherFollowerRuleInfo info)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                sportsManager.AddTogetherFollowerRule(new TogetherFollowerRule
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
                    sportsManager.AddUserBeedings(new UserBeedings
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

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 查询跟单信息
        /// </summary>
        public TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createUserId, string followerUserId, string gameCode, string gameType)
        {
            using (var sportsManager = new Sports_Manager())
            {
                return sportsManager.QueryTogetherFollowerRuleInfo(createUserId, followerUserId, gameCode, gameType);

            }
        }

        /// <summary>
        /// 用户成功订制跟单记录
        /// </summary>
        public TogetherFollowRecordInfoCollection QuerySucessFolloweRecord(string userId, long ruleId, string gameCode, int pageIndex, int pageSize)
        {
            var result = new TogetherFollowRecordInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QuerySucessFolloweRecord(userId, ruleId, gameCode, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询我的定制  或 定制我的
        /// </summary>
        public TogetherFollowerRuleQueryInfoCollection QueryUserFollowRule(bool byFollower, string userId, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            var result = new TogetherFollowerRuleQueryInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserFollowRule(byFollower, userId, gameCode, gameType, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 向上调
        /// </summary>
        public void FollowRuleMoveUp(string createrUserId, string gameCode, string gameType, long ruleId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var list = sportsManager.QuerySportsTogetherFollowerList(createrUserId, gameCode, gameType);
                if (list.Count == 0)
                    throw new Exception("没有查到相应的定制跟单记录");
                if (list[0].Id == ruleId)
                    throw new Exception("已经是第一位了");
                for (int i = 0; i < list.Count; i++)
                {
                    var current = list[i];
                    if (current.Id != ruleId)
                        continue;
                    var lastOne = list[i - 1];
                    var currentIndex = current.FollowerIndex;
                    current.FollowerIndex = lastOne.FollowerIndex;
                    sportsManager.UpdateTogetherFollowerRule(current);
                    lastOne.FollowerIndex = currentIndex;
                    sportsManager.UpdateTogetherFollowerRule(lastOne);
                    break;
                }

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 向下调
        /// </summary>
        public void FollowRuleMoveDown(string createrUserId, string gameCode, string gameType, long ruleId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var list = sportsManager.QuerySportsTogetherFollowerList(createrUserId, gameCode, gameType);
                if (list.Count == 0)
                    throw new Exception("没有查到相应的定制跟单记录");
                if (list[list.Count - 1].Id == ruleId)
                    throw new Exception("已经是最末位了");
                for (int i = 0; i < list.Count; i++)
                {
                    var current = list[i];
                    if (current.Id != ruleId)
                        continue;
                    var nextOne = list[i + 1];
                    var currentIndex = current.FollowerIndex;
                    current.FollowerIndex = nextOne.FollowerIndex;
                    sportsManager.UpdateTogetherFollowerRule(current);
                    nextOne.FollowerIndex = currentIndex;
                    sportsManager.UpdateTogetherFollowerRule(nextOne);
                    break;
                }

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 置顶
        /// </summary>
        public void FollowRuleSetTop(string createrUserId, string gameCode, string gameType, long ruleId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var list = sportsManager.QuerySportsTogetherFollowerList(createrUserId, gameCode, gameType);
                if (list.Count == 0)
                    throw new Exception("没有查到相应的定制跟单记录");
                var current = list.FirstOrDefault(p => p.Id == ruleId);
                if (current == null)
                    throw new Exception(string.Format("没有查询到记录{0}", ruleId));
                current.FollowerIndex = 1;
                sportsManager.UpdateTogetherFollowerRule(current);
                list.Remove(current);
                for (int i = 0; i < list.Count; i++)
                {
                    var c = list[i];
                    c.FollowerIndex = i + 2;
                    sportsManager.UpdateTogetherFollowerRule(c);
                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询定制我的跟单
        /// </summary>
        public TogetherFollowMeInfoCollection QueryUserBeFollowedReport(string userId, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            var result = new TogetherFollowMeInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserBeFollowedReport(userId, gameCode, gameType, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 编辑跟单
        /// </summary>
        public void EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 退订跟单
        /// </summary>
        public TogetherFollowerRule ExistTogetherFollower(long followerId, string followerUserId)
        {
            TogetherFollowerRule entity = null;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
            return entity;
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        public void AttentionUser(string currentUserId, string beAttentionUserId)
        {
            if (currentUserId == beAttentionUserId)
                throw new LogicException("不能关注自己");
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var entity = sportsManager.QueryUserAttention(currentUserId, beAttentionUserId);
                if (entity != null)
                    throw new LogicException("已对该用户发起过关注");

                sportsManager.AddUserAttention(new UserAttention
                {
                    BeAttentionUserId = beAttentionUserId,
                    FollowerUserId = currentUserId,
                    CreateTime = DateTime.Now,
                });

                var currentEntity = sportsManager.QueryUserAttentionSummary(currentUserId);
                if (currentEntity == null)
                {
                    sportsManager.AddUserAttentionSummary(new UserAttentionSummary
                    {
                        UserId = currentUserId,
                        FollowerUserCount = 1,
                        BeAttentionUserCount = 0,
                        UpdateTime = DateTime.Now,
                    });
                }
                else
                {
                    currentEntity.UpdateTime = DateTime.Now;
                    currentEntity.FollowerUserCount++;
                    sportsManager.UpdateUserAttentionSummary(currentEntity);
                }

                var beAttenEntity = sportsManager.QueryUserAttentionSummary(beAttentionUserId);
                if (beAttenEntity == null)
                {
                    sportsManager.AddUserAttentionSummary(new UserAttentionSummary
                    {
                        UserId = beAttentionUserId,
                        FollowerUserCount = 0,
                        BeAttentionUserCount = 1,
                        UpdateTime = DateTime.Now,
                    });
                }
                else
                {
                    beAttenEntity.UpdateTime = DateTime.Now;
                    beAttenEntity.BeAttentionUserCount++;
                    sportsManager.UpdateUserAttentionSummary(beAttenEntity);
                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        public void CancelAttentionUser(string currentUserId, string beAttentionUserId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var entity = sportsManager.QueryUserAttention(currentUserId, beAttentionUserId);
                if (entity == null)
                    throw new Exception("没有关注过该用户");
                sportsManager.DeleteUserAttention(entity);

                var currentEntity = sportsManager.QueryUserAttentionSummary(currentUserId);
                if (currentEntity == null)
                {
                    sportsManager.AddUserAttentionSummary(new UserAttentionSummary
                    {
                        UserId = currentUserId,
                        FollowerUserCount = 0,
                        BeAttentionUserCount = 0,
                        UpdateTime = DateTime.Now,
                    });
                }
                else
                {
                    currentEntity.UpdateTime = DateTime.Now;
                    currentEntity.FollowerUserCount--;
                    sportsManager.UpdateUserAttentionSummary(currentEntity);
                }

                var beAttenEntity = sportsManager.QueryUserAttentionSummary(beAttentionUserId);
                if (beAttenEntity == null)
                {
                    sportsManager.AddUserAttentionSummary(new UserAttentionSummary
                    {
                        UserId = beAttentionUserId,
                        FollowerUserCount = 0,
                        BeAttentionUserCount = 0,
                        UpdateTime = DateTime.Now,
                    });
                }
                else
                {
                    beAttenEntity.UpdateTime = DateTime.Now;
                    beAttenEntity.BeAttentionUserCount--;
                    sportsManager.UpdateUserAttentionSummary(beAttenEntity);
                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询是否有关注
        /// </summary>
        public bool QueryIsAttention(string currentUserId, string beAttentionUserId)
        {
            var sportsManager = new Sports_Manager();
            var entity = sportsManager.QueryUserAttention(currentUserId, beAttentionUserId);
            if (entity == null)
                return false;
            return true;
        }
        /// <summary>
        /// 根据用户Id，查询我的关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public UserAttention_Collection QueryMyAttentionListByUserId(string userId, int pageIndex, int pageSize)
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryMyAttentionListByUserId(userId, pageIndex, pageSize);
            }
        }
        /// <summary>
        /// 查询用户关注排行
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryUserAttentionSummaryRank(int pageIndex, int pageSize)
        {
            var result = new UserAttentionSummaryInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserAttentionSummaryRank(pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询用户关注的列表
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryUserAttentionList(string userId, int pageIndex, int pageSize)
        {
            var result = new UserAttentionSummaryInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryUserAttentionList(userId, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询关注用户(被关注)的列表
        /// </summary>
        public UserAttentionSummaryInfoCollection QueryAttentionUserList(string userId, int pageIndex, int pageSize)
        {
            var result = new UserAttentionSummaryInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new Sports_Manager().QueryAttentionUserList(userId, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        public TogetherFollowerRuleQueryInfo QueryMyTogetherFollowerRuleById(long ruleId)
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryMyTogetherFollowerRuleById(ruleId);
            }
        }

        #endregion

        #region 计算用户战绩

        /// <summary>
        /// 计算用户战绩，一天一次
        /// </summary>
        public void ComputeUserBeedings(string complateDate)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                //最新战绩计算方式，以订单为单位计算
                var sportsManager = new Sports_Manager();
                var updateBeedingList = new List<UserBeedings>();
                var totalOrderList = sportsManager.QuerySports_Order_ComplateByComplateDate(complateDate);
                //totalOrderList = totalOrderList.Where(s => s.SchemeId == "JCZQ14082458").ToList();//测试
                var gameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ", "CTZQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                foreach (var gameCode in gameCodeArray)
                {
                    var gameTypeArray = GetGameTypeArray(gameCode);
                    foreach (var gameType in gameTypeArray)
                    {
                        if (totalOrderList.Count == 0) continue;
                        foreach (var order in totalOrderList)
                        {
                            if (order.GameCode == gameCode && order.GameType == gameType)
                            {
                                var beeding = sportsManager.QueryUserBeedings(order.UserId, gameCode, gameType);
                                if (beeding == null) continue;
                                var gainMoney = order.AfterTaxBonusMoney - order.TotalMoney;
                                beeding.UpdateTime = DateTime.Now;
                                beeding.TotalBonusTimes += order.BonusStatus == BonusStatus.Win ? 1 : 0;
                                beeding.TotalBonusMoney += order.AfterTaxBonusMoney;
                                beeding.TotalOrderCount += 1;
                                beeding.TotalBetMoney += order.TotalMoney;
                                //计算战绩
                                ComputeOneUserBeeding(beeding, gainMoney, order.IsVirtualOrder);
                                sportsManager.UpdateUserBeedings(beeding);
                            }
                        }
                    }
                }

                #region 20150918 屏蔽

                //var sportsManager = new Sports_Manager();
                //var updateBeedingList = new List<UserBeedings>();
                //var totalOrderList = sportsManager.QuerySports_Order_ComplateByComplateDate(complateDate);
                //var gameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ", "CTZQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
                //foreach (var gameCode in gameCodeArray)
                //{
                //    var gameTypeArray = GetGameTypeArray(gameCode);
                //    if (gameTypeArray.Length == 0)
                //    {
                //        //数字彩
                //        var g = totalOrderList.Where(p => p.GameCode == gameCode).GroupBy(p => p.UserId);
                //        foreach (var item in g)
                //        {
                //            var currentOrder = totalOrderList.Where(p => p.UserId == item.Key && p.GameCode == gameCode).ToList();
                //            if (currentOrder.Count == 0) continue;

                //            var beeding = sportsManager.QueryUserBeedings(item.Key, gameCode, string.Empty);
                //            if (beeding == null) continue;

                //            foreach (var order in currentOrder)
                //            {
                //                var gainMoney = order.AfterTaxBonusMoney - order.TotalMoney;
                //                beeding.UpdateTime = DateTime.Now;
                //                beeding.TotalBonusTimes += order.BonusStatus == BonusStatus.Win ? 1 : 0;
                //                beeding.TotalBonusMoney += order.AfterTaxBonusMoney;
                //                beeding.TotalOrderCount += currentOrder.Count;
                //                beeding.TotalBetMoney += order.TotalMoney;
                //                //计算战绩
                //                ComputeOneUserBeeding(beeding, gainMoney, order.IsVirtualOrder);
                //            }
                //            sportsManager.UpdateUserBeedings(beeding);
                //        }
                //        continue;
                //    }
                //    //足彩
                //    foreach (var gameType in gameTypeArray)
                //    {
                //        var g = totalOrderList.Where(p => p.GameCode == gameCode && p.GameType == gameType).GroupBy(p => p.UserId);
                //        foreach (var item in g)
                //        {
                //            var currentOrder = totalOrderList.Where(p => p.UserId == item.Key && p.GameCode == gameCode && p.GameType == gameType).ToList();
                //            if (currentOrder.Count == 0) continue;

                //            var beeding = sportsManager.QueryUserBeedings(item.Key, gameCode, gameType);
                //            if (beeding == null) continue;

                //            foreach (var order in currentOrder)
                //            {
                //                var gainMoney = order.AfterTaxBonusMoney - order.TotalMoney;
                //                beeding.UpdateTime = DateTime.Now;
                //                beeding.TotalBonusTimes += order.BonusStatus == BonusStatus.Win ? 1 : 0;
                //                beeding.TotalBonusMoney += order.AfterTaxBonusMoney;
                //                beeding.TotalOrderCount += currentOrder.Count;
                //                beeding.TotalBetMoney += order.TotalMoney;
                //                //计算战绩
                //                ComputeOneUserBeeding(beeding, gainMoney, order.IsVirtualOrder);
                //            }
                //            sportsManager.UpdateUserBeedings(beeding);
                //        }
                //    }
                //} 

                #endregion

                biz.CommitTran();
            }
        }

        private void ComputeOneUserBeeding(UserBeedings beeding, decimal gainMoney, bool isVirtualOrder)
        {
            var lotteryArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            var sportsArray = new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ" };

            if (lotteryArray.Contains(beeding.GameCode))
            {
                //数字彩计算规则
                #region 数字彩

                if (gainMoney < 500)
                    return;
                if (gainMoney >= 500 && gainMoney < 1000)
                {
                    //一枚金星
                    if (isVirtualOrder)
                    {
                        beeding.SilverStarCount++;
                    }
                    else
                    {
                        beeding.GoldStarCount++;
                    }
                }
                if (gainMoney >= 1000 && gainMoney < 10000)
                {
                    //奖励N枚金星（N枚金星=盈利金额÷1000）
                    int count = (int)Math.Floor(gainMoney / 1000);
                    if (isVirtualOrder)
                    {
                        beeding.SilverStarCount += count;
                    }
                    else
                    {
                        beeding.GoldStarCount += count;
                    }
                }
                if (gainMoney >= 10000 && gainMoney < 10000 * 10)
                {
                    //奖励N座金钻（N枚金钻=盈利金额÷1万）；
                    int count = (int)Math.Floor(gainMoney / 10000);
                    if (isVirtualOrder)
                    {
                        beeding.SilverDiamondsCount += count;
                    }
                    else
                    {
                        beeding.GoldDiamondsCount += count;
                    }
                }
                if (gainMoney >= 10000 * 10 && gainMoney < 10000 * 100)
                {
                    //奖励N顶金奖杯（N枚金奖杯=盈利金额÷10万）
                    int count = (int)Math.Floor(gainMoney / (10 * 10000));
                    if (isVirtualOrder)
                    {
                        beeding.SilverCupCount += count;
                    }
                    else
                    {
                        beeding.GoldCupCount += count;
                    }
                }
                if (gainMoney >= 10000 * 100)
                {
                    //1个金冠
                    if (isVirtualOrder)
                    {
                        beeding.SilverCrownCount++;
                    }
                    else
                    {
                        beeding.GoldCrownCount++;
                    }
                }

                #endregion
            }
            if (sportsArray.Contains(beeding.GameCode))
            {
                //足彩计算规则
                #region 足彩

                if (gainMoney < 1000)
                    return;
                if (gainMoney >= 1000 && gainMoney < 10000)
                {
                    //一枚金星
                    if (isVirtualOrder)
                    {
                        beeding.SilverStarCount++;
                    }
                    else
                    {
                        beeding.GoldStarCount++;
                    }
                }
                if (gainMoney >= 10000 && gainMoney < 10000 * 10)
                {
                    //奖励N颗金星（N颗金星=盈利金额÷5000）；
                    int count = (int)Math.Floor(gainMoney / 5000);
                    if (isVirtualOrder)
                    {
                        beeding.SilverStarCount += count;
                    }
                    else
                    {
                        beeding.GoldStarCount += count;
                    }
                }
                if (gainMoney >= 10000 * 10 && gainMoney < 10000 * 100)
                {
                    //奖励N座金钻（N座金钻=盈利金额÷10万）；
                    int count = (int)Math.Floor(gainMoney / (10000 * 10));
                    if (isVirtualOrder)
                    {
                        beeding.SilverDiamondsCount += count;
                    }
                    else
                    {
                        beeding.GoldDiamondsCount += count;
                    }
                }
                if (gainMoney >= 10000 * 100)
                {
                    //（N座金奖杯=盈利金额÷100万）
                    int count = (int)Math.Floor(gainMoney / (10000 * 100));
                    if (isVirtualOrder)
                    {
                        beeding.SilverCupCount += count;
                    }
                    else
                    {
                        beeding.GoldCupCount += count;
                    }
                }
                #endregion
            }

            #region 计算是否需要进位

            //金
            if (beeding.GoldStarCount >= 5)
            {
                int jinWei = beeding.GoldStarCount / 5;
                var spare = beeding.GoldStarCount - (5 * jinWei); ;
                beeding.GoldStarCount = spare;
                beeding.GoldDiamondsCount += jinWei;
            }
            if (beeding.GoldDiamondsCount >= 5)
            {
                int jinWei = beeding.GoldDiamondsCount / 5;
                var spare = beeding.GoldDiamondsCount - (5 * jinWei); ;
                beeding.GoldDiamondsCount = spare;
                beeding.GoldCupCount += jinWei;
            }
            if (beeding.GoldCupCount >= 5)
            {
                int jinWei = beeding.GoldCupCount / 5;
                var spare = beeding.GoldCupCount - (5 * jinWei); ;
                beeding.GoldCupCount = spare;
                beeding.GoldCrownCount += jinWei;
            }

            //银
            if (beeding.SilverStarCount >= 5)
            {
                int jinWei = beeding.SilverStarCount / 5;
                var spare = beeding.SilverStarCount - (5 * jinWei); ;
                beeding.SilverStarCount = spare;
                beeding.SilverDiamondsCount += jinWei;
            }
            if (beeding.SilverDiamondsCount >= 5)
            {
                int jinWei = beeding.SilverDiamondsCount / 5;
                var spare = beeding.SilverDiamondsCount - (5 * jinWei); ;
                beeding.SilverDiamondsCount = spare;
                beeding.SilverCupCount += jinWei;
            }
            if (beeding.SilverCupCount >= 5)
            {
                int jinWei = beeding.SilverCupCount / 5;
                var spare = beeding.SilverCupCount - (5 * jinWei); ;
                beeding.SilverCupCount = spare;
                beeding.SilverCrownCount += jinWei;
            }
            #endregion
        }

        /// <summary>
        /// 计算幸运用户，计算当天
        /// </summary>
        public void ComputeLucyUser()
        {
            DateTime startTime = DateTime.Now.AddDays(-1);
            DateTime endTime = DateTime.Now.AddDays(1);
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var gameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ", "CTZQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JXSSQ", "JX11X5", "SD11X5", "GD11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                foreach (var gameCode in gameCodeArray)
                {
                    var gameTypeArray = GetGameTypeArray(gameCode);
                    foreach (var gameType in gameTypeArray)
                    {
                        foreach (var beeding in sportsManager.QueryUserBeedingsList(gameCode, gameType))
                        {
                            var orderList = sportsManager.QuerySports_Order_ComplateByComplateTime(beeding.UserId, gameCode, gameType, startTime, endTime);
                            //beeding.ShowLuck = false;
                            while (true)
                            {
                                if (orderList.Count < 10)
                                    break;
                                //if (orderList.Count(p => p.AfterTaxBonusMoney >= p.TotalMoney * 2) > 0)
                                //    beeding.ShowLuck = true;
                                break;
                            }
                            sportsManager.UpdateUserBeedings(beeding);
                        }
                    }
                    if (gameTypeArray.Length == 0)
                    {
                        foreach (var beeding in sportsManager.QueryUserBeedingsList(gameCode, string.Empty))
                        {
                            var orderList = sportsManager.QuerySports_Order_ComplateByComplateTime(beeding.UserId, gameCode, string.Empty, startTime, endTime);
                            //beeding.ShowLuck = false;
                            while (true)
                            {
                                if (orderList.Count < 10)
                                    break;
                                //if (orderList.Count(p => p.AfterTaxBonusMoney >= p.TotalMoney * 2) > 0)
                                //    beeding.ShowLuck = true;
                                break;
                            }
                            sportsManager.UpdateUserBeedings(beeding);
                        }
                    }
                }

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 计算中奖概率，每天一次，但是计算最一个月的订单数据
        /// </summary>
        public void ComputeBonusPercent()
        {
            DateTime startTime = DateTime.Now.AddDays(-7);
            DateTime endTime = DateTime.Now.AddDays(1);

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var gameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ", "CTZQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
                var currentDate = endTime.ToString("yyyyMMdd");

                var totalOrderList = sportsManager.QuerySports_Order_ComplateByComplateTime(startTime, endTime);
                foreach (var gameCode in gameCodeArray)
                {
                    var currentOrder = new List<Sports_Order_Complate>();
                    var gameTypeArray = GetGameTypeArray(gameCode);
                    if (gameTypeArray.Length == 0)
                    {
                        //数字彩
                        currentOrder = totalOrderList.Where(p => p.GameCode == gameCode).ToList();
                        foreach (var u in currentOrder.GroupBy(p => p.UserId))
                        {
                            var userTotalOrderList = currentOrder.Where(p => p.UserId == u.Key).ToList();
                            //加上此行，如果近一月没有订单，也不会清空中奖概率
                            //if (userTotalOrderList.Count == 0) continue;
                            var userWinOrderList = userTotalOrderList.Where(p => p.BonusStatus == BonusStatus.Win).ToList();
                            var bounsPercent = userTotalOrderList.Count == 0 ? 0M : (decimal)userWinOrderList.Count / userTotalOrderList.Count;
                            //查询用户中奖概率数据
                            var current = sportsManager.QueryUserBonusPercent(u.Key, gameCode, string.Empty);
                            if (current == null) continue;

                            current.TotalOrderCount = userTotalOrderList.Count;
                            current.BonusOrderCount = userWinOrderList.Count;
                            current.CurrentDate = currentDate;
                            current.BonusPercent = bounsPercent;
                            sportsManager.UpdateUserBonusPercent(current);
                        }

                        continue;
                    }
                    //足彩
                    foreach (var gameType in gameTypeArray)
                    {
                        currentOrder = totalOrderList.Where(p => p.GameCode == gameCode && p.GameType == gameType).ToList();
                        foreach (var u in currentOrder.GroupBy(p => p.UserId))
                        {
                            var userTotalOrderList = currentOrder.Where(p => p.UserId == u.Key).ToList();
                            //加上此行，如果近一月没有订单，也不会清空中奖概率
                            //if (userTotalOrderList.Count == 0) continue;
                            var userWinOrderList = userTotalOrderList.Where(p => p.BonusStatus == BonusStatus.Win).ToList();
                            var bounsPercent = userTotalOrderList.Count == 0 ? 0M : (decimal)userWinOrderList.Count / userTotalOrderList.Count;
                            //查询用户中奖概率数据
                            var current = sportsManager.QueryUserBonusPercent(u.Key, gameCode, gameType);
                            if (current == null) continue;

                            current.TotalOrderCount = userTotalOrderList.Count;
                            current.BonusOrderCount = userWinOrderList.Count;
                            current.CurrentDate = currentDate;
                            current.BonusPercent = bounsPercent;
                            sportsManager.UpdateUserBonusPercent(current);
                        }
                    }
                }

                //更新合买红人
                var hotManager = new TogetherHotUserManager();
                var list = hotManager.QueryTogetherHotUserList();
                foreach (var item in list)
                {
                    var orderList = sportsManager.QueryWinSports_Order_ComplateByComplateTime(item.UserId, DateTime.Now.AddDays(-7), DateTime.Now);
                    var sum = orderList.Sum(p => p.AfterTaxBonusMoney);
                    if (sum <= 0) continue;

                    item.WeeksWinMoney = sum;
                    hotManager.UpdateTogetherHotUser(item);
                }

                //更新名家命中率
                //var experManager = new ExperterSchemeManager();
                //foreach (var exper in experManager.QueryAllExperter())
                //{
                //    //总（6个月）
                //    var totalSchemeList = experManager.QueryExperterSchemeList(exper.UserId, DateTime.Now.AddMonths(-12), DateTime.Now);
                //    var totalWinSchemeList = totalSchemeList.Where(p => p.BonusStatus == BonusStatus.Win).ToList();

                //    //月
                //    var monthSchemeList = totalSchemeList.Where(p => p.CreateTime >= DateTime.Now.AddMonths(-1) && p.CreateTime < DateTime.Now).ToList();
                //    var monthWinSchemeList = monthSchemeList.Where(p => p.BonusStatus == BonusStatus.Win).ToList();

                //    //周
                //    var weekSchemeList = monthSchemeList.Where(p => p.CreateTime >= DateTime.Now.AddDays(-7) && p.CreateTime < DateTime.Now).ToList();
                //    var weekWinSchemeList = weekSchemeList.Where(p => p.BonusStatus == BonusStatus.Win).ToList();

                //    //命中率
                //    exper.TotalShooting = totalSchemeList.Count == 0 ? 0M : (decimal)totalWinSchemeList.Count / totalSchemeList.Count;
                //    exper.MonthShooting = monthSchemeList.Count == 0 ? 0M : (decimal)monthWinSchemeList.Count / monthSchemeList.Count;
                //    exper.WeekShooting = weekSchemeList.Count == 0 ? 0M : (decimal)weekWinSchemeList.Count / weekSchemeList.Count;
                //    //回报率
                //    exper.TotalRate = totalSchemeList.Count == 0 || totalSchemeList.Sum(p => p.BetMoney) == 0M ? 0M : totalSchemeList.Sum(p => p.BonusMoney) / totalSchemeList.Sum(p => p.BetMoney);
                //    exper.MonthRate = monthSchemeList.Count == 0 || monthSchemeList.Sum(p => p.BetMoney) == 0M ? 0M : monthSchemeList.Sum(p => p.BonusMoney) / monthSchemeList.Sum(p => p.BetMoney);
                //    exper.WeekRate = weekSchemeList.Count == 0 || weekSchemeList.Sum(p => p.BetMoney) == 0M ? 0M : weekSchemeList.Sum(p => p.BonusMoney) / weekSchemeList.Sum(p => p.BetMoney);

                //    experManager.UpdateExperter(exper);
                //}

                biz.CommitTran();
            }
        }

        private string[] GetGameTypeArray(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return new string[] { "SPF", "ZJQ", "SXDS", "BF", "BQC" };
                case "JCZQ":
                    return new string[] { "SPF", "BRQSPF", "BF", "ZJQ", "BQC", "HH" };
                case "JCLQ":
                    return new string[] { "SF", "RFSF", "SFC", "DXF", "HH" };
                case "CTZQ":
                    return new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
                //todo  其它彩种
            }
            return new string[] { };
        }

        public void UpdateUserBeeding(string userId, string gameCode, string gameType, decimal successBettingMoney, decimal successAndBonusMoney, int successAndBonusCount,
            decimal failBettingMoney, decimal failAndBonusMoney, int failAndBonusCount)
        {
            if (successBettingMoney < 0) return;
            if (successAndBonusMoney < 0) return;
            if (failBettingMoney < 0) return;
            if (failAndBonusMoney < 0) return;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                gameCode = gameCode.ToUpper();
                gameType = gameType.ToUpper();
                var sportsManager = new Sports_Manager();
                var entity = sportsManager.QueryUserBeedings(userId, gameCode, gameType);
                if (entity == null)
                {
                    sportsManager.AddUserBeedings(new UserBeedings
                    {
                        UpdateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = gameType,
                        UserId = userId,
                        //TogetherSchemeBettingMoney = successBettingMoney + failBettingMoney,
                        //TogetherSchemeBonusMoney = successAndBonusMoney + failAndBonusMoney,
                        //TogetherSchemeCount = successAndBonusCount + failAndBonusCount,
                        //TogetherSchemeSuccessAndBonusCount = successAndBonusCount,
                        //TogetherSchemeFailAndBonusCount = failAndBonusCount,

                        //TogetherSchemeSuccessBettingMoney = successBettingMoney,
                        //TogetherSchemeSuccessAndBonusMoney = successAndBonusMoney,
                        //TogetherSchemeSuccessGainMoney = successAndBonusMoney - successBettingMoney > 0 ? successAndBonusMoney - successBettingMoney : 0,

                        //TogetherSchemeFailBettingMoney = failBettingMoney,
                        //TogetherSchemeFailAndBonusMoney = failAndBonusMoney,
                        //TogetherSchemeFailGainMoney = failAndBonusMoney - failBettingMoney > 0 ? failAndBonusMoney - failBettingMoney : 0,
                    });
                }
                else
                {
                    //entity.TogetherSchemeBettingMoney = successBettingMoney + failBettingMoney;
                    //entity.TogetherSchemeBonusMoney = successAndBonusMoney + failAndBonusMoney;
                    //entity.TogetherSchemeCount = successAndBonusCount + failAndBonusCount;
                    //entity.TogetherSchemeSuccessAndBonusCount = successAndBonusCount;
                    //entity.TogetherSchemeFailAndBonusCount = failAndBonusCount;

                    //entity.TogetherSchemeSuccessBettingMoney = successBettingMoney;
                    //entity.TogetherSchemeSuccessAndBonusMoney = successAndBonusMoney;
                    //entity.TogetherSchemeSuccessGainMoney = successAndBonusMoney - successBettingMoney > 0 ? successAndBonusMoney - successBettingMoney : 0;
                    //entity.TogetherSchemeFailBettingMoney = failBettingMoney;
                    //entity.TogetherSchemeFailAndBonusMoney = failAndBonusMoney;
                    //entity.TogetherSchemeFailGainMoney = failAndBonusMoney - failBettingMoney > 0 ? failAndBonusMoney - failBettingMoney : 0;
                    entity.UpdateTime = DateTime.Now;
                    sportsManager.UpdateUserBeedings(entity);
                }

                biz.CommitTran();
            }
        }

        #endregion

        #region 手工处理订单

        private static void SetTogetherOrder(Sports_Manager sportsManager, string schemeId, decimal bonusMoney)
        {
            var main = sportsManager.QuerySports_Together(schemeId);
            main.ProgressStatus = TogetherSchemeProgress.Completed;
            sportsManager.UpdateSports_Together(main);

            //提成
            var deductMoney = 0M;
            if (bonusMoney > main.TotalMoney)
                deductMoney = bonusMoney * main.BonusDeduct / 100;
            var totalMoney = bonusMoney - deductMoney;
            var singleMoney = totalMoney / main.TotalCount;
            foreach (var join in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
            {
                join.PreTaxBonusMoney = join.BuyCount * singleMoney;
                join.AfterTaxBonusMoney = join.BuyCount * singleMoney;
                sportsManager.UpdateSports_TogetherJoin(join);
                //订制跟单奖金更新
                if (join.JoinType == TogetherJoinType.FollowerJoin)
                {
                    var f = sportsManager.QueryFollowerRecordBySchemeId(main.SchemeId, join.JoinUserId);
                    if (f == null) continue;
                    f.BonusMoney = join.BuyCount * singleMoney;
                    sportsManager.UpdateTogetherFollowerRecord(f);
                }
            }
        }

        /// <summary>
        /// 移动运行中的订单数据到完成订单数据
        /// </summary>
        public void MoveRunningOrderToComplateOrder(string schemeId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception(string.Format("找不到订单{0}的Running的信息，可能是订单已移动到Complate", schemeId));

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception(string.Format("找不到方案{0}的orderDetail信息", schemeId));

                order.TicketStatus = TicketStatus.Ticketed;
                order.ProgressStatus = ProgressStatus.Complate;
                var complateOrder = new Sports_Order_Complate
                {
                    SchemeId = order.SchemeId,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    PlayType = order.PlayType,
                    IssuseNumber = order.IssuseNumber,
                    TotalMoney = order.TotalMoney,
                    Amount = order.Amount,
                    TotalMatchCount = order.TotalMatchCount,
                    TicketStatus = order.TicketStatus,
                    BonusStatus = order.BonusStatus,
                    AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                    CanChase = order.CanChase,
                    IsVirtualOrder = order.IsVirtualOrder,
                    CreateTime = order.CreateTime,
                    PreTaxBonusMoney = order.PreTaxBonusMoney,
                    ProgressStatus = order.ProgressStatus,
                    SchemeType = order.SchemeType,
                    TicketId = order.TicketId,
                    TicketLog = order.TicketLog + "|后台手工移动订单",
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
                    HitMatchCount = order.HitMatchCount,
                    RightCount = order.RightCount,
                    Error1Count = order.Error1Count,
                    Error2Count = order.Error2Count,
                    AddMoney = 0M,
                    DistributionWay = AddMoneyDistributionWay.Average,
                    AddMoneyDescription = string.Empty,
                    BonusCountDescription = string.Empty,
                    BonusCountDisplayName = string.Empty,
                    Security = order.Security,
                    BetTime = order.BetTime,
                    SuccessMoney = order.SuccessMoney,
                    ExtensionOne = order.ExtensionOne,
                    TicketGateway = order.TicketGateway,
                    TicketProgress = order.TicketProgress,
                    Attach = order.Attach,
                    IsAppend = order.IsAppend,
                    RedBagMoney = order.RedBagMoney,
                    TicketTime = order.TicketTime,
                    TotalPayRebateMoney = order.TotalPayRebateMoney,
                    RealPayRebateMoney = order.RealPayRebateMoney,
                    QueryTicketStopTime = order.QueryTicketStopTime,
                    MinBonusMoney = order.MinBonusMoney,
                    MaxBonusMoney = order.MaxBonusMoney,
                    IsPayRebate = order.IsPayRebate,
                    IsSplitTickets = order.IsSplitTickets,
                };
                sportsManager.UpdateSports_Order_Running(order);
                sportsManager.AddSports_Order_Complate(complateOrder);
                sportsManager.DeleteSports_Order_Running(order);

                orderDetail.TicketStatus = order.TicketStatus;
                orderDetail.ProgressStatus = order.ProgressStatus;
                orderDetail.ComplateTime = DateTime.Now;
                orderDetail.BonusStatus = order.BonusStatus;
                orderDetail.CurrentBettingMoney = order.IsVirtualOrder ? 0 : orderDetail.TotalMoney;
                orderDetail.PreTaxBonusMoney = order.PreTaxBonusMoney;
                orderDetail.AfterTaxBonusMoney = order.AfterTaxBonusMoney;
                manager.UpdateOrderDetail(orderDetail);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 手工设置订单中奖数据
        /// </summary>
        public void ManualSetOrderBonusMoney(string schemeId, decimal bonusMoney, int bonusCount, int hitMatchCount, string bonusCountDescription, string bonusCountDisplayName)
        {
            Sports_Order_Complate order = null;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                order = sportsManager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new Exception(string.Format("找不到订单{0}的QuerySports_Order_Complate信息", schemeId));
                //if (order.SchemeType == SchemeType.ChaseBetting)
                //    throw new Exception("此方法不能用于【追号订单】");

                var schemeManager = new SchemeManager();
                var orderDetail = schemeManager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception(string.Format("找不到订单{0}的orderDetail的信息", schemeId));

                order.IsPrizeMoney = false;
                order.BonusStatus = BonusStatus.Win;
                order.PreTaxBonusMoney = bonusMoney;
                order.AfterTaxBonusMoney = bonusMoney;
                if (bonusCount != -1)
                    order.BonusCount = bonusCount;
                if (hitMatchCount != -1)
                    order.HitMatchCount = hitMatchCount;
                if (!string.IsNullOrEmpty(bonusCountDescription))
                    order.BonusCountDescription = bonusCountDescription;
                if (!string.IsNullOrEmpty(bonusCountDisplayName))
                    order.BonusCountDisplayName = bonusCountDisplayName;
                sportsManager.UpdateSports_Order_Complate(order);

                orderDetail.PreTaxBonusMoney = bonusMoney;
                orderDetail.AfterTaxBonusMoney = bonusMoney;
                orderDetail.BonusStatus = BonusStatus.Win;
                schemeManager.UpdateOrderDetail(orderDetail);

                if (order.SchemeType == SchemeType.TogetherBetting)
                    SetTogetherOrder(sportsManager, schemeId, order.AfterTaxBonusMoney);

                biz.CommitTran();
            }
            try
            {
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IOrderPrize_AfterTranCommit>(new object[] { order.UserId, schemeId, order.GameCode, order.GameType, order.IssuseNumber, order.TotalMoney, true, order.PreTaxBonusMoney, order.AfterTaxBonusMoney, order.IsVirtualOrder, DateTime.Now });
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 手工设置订单为不中奖
        /// </summary>
        public void ManualSetOrderNotBonus(string schemeId, int hitMatchCount)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var order = sportsManager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new Exception(string.Format("找不到订单{0}的QuerySports_Order_Complate信息", schemeId));
                if (order.SchemeType == SchemeType.ChaseBetting)
                    throw new Exception("此方法不能用于【追号订单】");
                if (order.IsPrizeMoney)
                    throw new Exception("订单已派发奖金");

                var schemeManager = new SchemeManager();
                var orderDetail = schemeManager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception(string.Format("找不到订单{0}的orderDetail的信息", schemeId));

                order.BonusStatus = BonusStatus.Lose;
                order.PreTaxBonusMoney = 0M;
                order.AfterTaxBonusMoney = 0M;
                order.BonusCount = 0;
                order.HitMatchCount = hitMatchCount;
                order.BonusCountDescription = string.Empty;
                order.BonusCountDisplayName = string.Empty;
                sportsManager.UpdateSports_Order_Complate(order);

                orderDetail.PreTaxBonusMoney = 0M;
                orderDetail.AfterTaxBonusMoney = 0M;
                orderDetail.BonusStatus = BonusStatus.Lose;
                schemeManager.UpdateOrderDetail(orderDetail);

                if (order.SchemeType == SchemeType.TogetherBetting)
                    SetTogetherOrder(sportsManager, schemeId, 0M);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 修改订单状态为已派奖
        /// </summary>
        public void UpdateOrderPrizeMoney(string schemeId)
        {
            using (var manager = new Sports_Manager())
            {
                var order = manager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new Exception("未查询到方案信息！");
                if (order.ProgressStatus != ProgressStatus.Complate || order.BonusStatus != BonusStatus.Win || order.IsVirtualOrder)
                    throw new Exception("当前订单状态不能执行此项操作！");
                if (order.IsPrizeMoney)
                    throw new Exception("当前订单已经为已派奖状态！");
                order.IsPrizeMoney = true;
                manager.UpdateSports_Order_Complate(order);
            }
        }

        #endregion

        public System.Data.DataSet QueryBettingAnteCode(DateTime startTime, DateTime endTime)
        {
            return new SchemeManager().QueryBettingAnteCode(startTime, endTime);
        }

        /// <summary>
        /// 查询用户保存的订单信息
        /// </summary>
        public SaveOrder_LotteryBettingInfoCollection QuerySaveOrderLottery(string userId)
        {
            var result = new SaveOrder_LotteryBettingInfoCollection();
            result.AddRange(new Sports_Manager().QuerySaveOrderLottery(userId));
            return result;
        }

        /// <summary>
        /// 查询未投注的订单
        /// </summary>
        public string QueryUnBetOrder(string time)
        {
            var manager = new Sports_Manager();
            return manager.QueryUnBetOrderByTime(time);
        }

        /// <summary>
        /// 自动投注用户保存的订单
        /// </summary>
        public void AutoBetUserSaveOrder(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            //删除保存信息
            var saveOrder = sportsManager.QuerySaveOrder(schemeId);
            if (saveOrder == null)
                return;
            saveOrder.ProgressStatus = ProgressStatus.Aborted;
            saveOrder.SchemeType = SchemeType.GeneralBetting;
            sportsManager.UpdateUserSaveOrder(saveOrder);

            //撤单
            BetFail(schemeId);
        }

        /// <summary>
        /// 订单投注失败，返钱
        /// </summary>
        public void BetFail(string schemeId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                if (schemeId.StartsWith("CHASE"))
                    throw new Exception("追号订单暂时不能处理");

                var sportsManager = new Sports_Manager();
                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception(string.Format("订单{0}不存在", schemeId));

                #region 处理订单数据

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                orderDetail.ProgressStatus = ProgressStatus.Aborted;
                orderDetail.CurrentBettingMoney = 0M;
                orderDetail.CurrentIssuseNumber = order.IssuseNumber;
                orderDetail.TicketStatus = TicketStatus.Error;
                manager.UpdateOrderDetail(orderDetail);

                //移动订单数据
                OrderFailToEnd(schemeId, sportsManager, order);

                #endregion

                #region 请求出票失败后，退还投注资金

                if (!order.IsVirtualOrder)
                {
                    // 返还资金
                    if (order.SchemeType == SchemeType.GeneralBetting)
                    {
                        if (order.TotalMoney > 0)
                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, schemeId, order.TotalMoney
                               , string.Format("{0} 出票失败，返还资金￥{1:N2}。 ", BusinessHelper.FormatGameCode(order.GameCode), order.TotalMoney));
                    }
                    if (order.SchemeType == SchemeType.ChaseBetting)
                    {
                        var chaseOrder = sportsManager.QueryLotteryScheme(order.SchemeId);
                        if (chaseOrder != null)
                        {
                            if (order.TotalMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TicketFailed, order.UserId, chaseOrder.KeyLine, order.TotalMoney
                                , string.Format("订单{0} 出票失败，返还资金￥{1:N2}。 ", order.SchemeId, order.TotalMoney));
                        }
                    }
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        //失败
                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(schemeId))
                        {
                            item.JoinSucess = false;
                            item.JoinLog += "出票失败";
                            sportsManager.UpdateSports_TogetherJoin(item);
                            if (item.JoinType == TogetherJoinType.SystemGuarantees)
                                continue;

                            var t = string.Empty;
                            var realBuyCount = item.RealBuyCount;
                            switch (item.JoinType)
                            {
                                case TogetherJoinType.Subscription:
                                    t = "认购";
                                    break;
                                case TogetherJoinType.FollowerJoin:
                                    t = "订制跟单";
                                    break;
                                case TogetherJoinType.Join:
                                    t = "参与";
                                    break;
                                case TogetherJoinType.Guarantees:
                                    realBuyCount = item.BuyCount;
                                    t = "保底";
                                    break;
                            }
                            var money = item.Price * realBuyCount;
                            //退钱
                            if (money > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_TogetherFail, item.JoinUserId,
                                  string.Format("{0}_{1}", schemeId, item.Id), item.TotalMoney, string.Format("合买失败，返还{0}资金{1:N2}元", t, money));
                        }
                    }
                }

                #endregion

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 添加通知
        /// </summary>
        public void AddReceiveNoticeLog(ReceiveNoticeLogInfo info)
        {
            if (info == null)
                return;

            var entity = new ReceiveNoticeLog();
            ObjectConvert.ConverInfoToEntity(info, ref entity);
            new Sports_Manager().AddReceiveNoticeLog(entity);
        }

        public void UpdateReceiveNoticeLog(ReceiveNoticeLog notice)
        {
            new Sports_Manager().UpdateReceiveNoticeLog(notice);
        }

        /// <summary>
        /// 从历史表添加通知到临时表
        /// </summary>
        public void MoveComplateNotice(long noticeId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new Sports_Manager();
                var notice = manager.QueryReceiveNoticeLog_Complate(noticeId);
                if (notice == null)
                    throw new Exception(string.Format("找不到通知{0}", noticeId));
                manager.AddReceiveNoticeLog(new ReceiveNoticeLog
                {
                    AgentId = notice.AgentId,
                    CreateTime = DateTime.Now,
                    Sign = notice.Sign,
                    SendTimes = 0,
                    Remark = "接收成功",
                    ReceiveUrlRoot = notice.ReceiveUrlRoot,
                    ReceiveDataString = notice.ReceiveDataString,
                    NoticeType = notice.NoticeType,
                });


                biz.CommitTran();
            }

        }

        /// <summary>
        /// 通知移动历史表
        /// </summary>
        public void MoveNoticeToComplate(ReceiveNoticeLog notice)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new Sports_Manager();
                manager.AddReceiveNoticeLog_Complate(new ReceiveNoticeLog_Complate
                {
                    AgentId = notice.AgentId,
                    ComplateTime = DateTime.Now,
                    CreateTime = notice.CreateTime,
                    NoticeType = notice.NoticeType,
                    ReceiveDataString = notice.ReceiveDataString,
                    ReceiveNoticeId = notice.ReceiveNoticeId,
                    ReceiveUrlRoot = notice.ReceiveUrlRoot,
                    Remark = notice.Remark,
                    SendTimes = notice.SendTimes,
                    Sign = notice.Sign,
                });
                manager.DeleteReceiveNoticeLog(notice);

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 查询通知
        /// </summary>
        public string QueryReceiveNoticeList(int returnRecord = 0)
        {
            using (var sportManager = new Sports_Manager())
            {
                return sportManager.QueryReceiveNoticeList(returnRecord);
            }
        }

        public ReceiveNoticeLog QueryReceiveNoticeByReceiveId(string receiveId)
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryReceiveNoticeByReceiveId(receiveId);
            }
        }

        public ReceiveNoticeLogInfo_Collection QueryReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new Sports_Manager())
            {
                int totalCount;
                ReceiveNoticeLogInfo_Collection colleciton = new ReceiveNoticeLogInfo_Collection();
                colleciton.ListInfo = manager.QueryReceiveNoticeLogList(noticeType, startTiem, endTime, pageIndex, pageSize, out totalCount);
                colleciton.TotalCount = totalCount;
                return colleciton;
            }
        }

        public ReceiveNoticeLogInfo_Collection QueryComplateReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new Sports_Manager())
            {
                int totalCount;
                ReceiveNoticeLogInfo_Collection colleciton = new ReceiveNoticeLogInfo_Collection();
                colleciton.ListInfo = manager.QueryComplateReceiveNoticeLogList(noticeType, startTiem, endTime, pageIndex, pageSize, out totalCount);
                colleciton.TotalCount = totalCount;
                return colleciton;
            }
        }
        public CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber)
        {
            using (var manager = new CTZQMatchManager())
            {
                CTZQMatchInfo_Collection collection = new CTZQMatchInfo_Collection();
                collection.ListInfo = manager.QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber);
                return collection;
            }
        }

        public string QueryWaitingTicket(int returnRecord)
        {
            return new Sports_Manager().QueryWaitingTicket(returnRecord);
        }

        public string QueryWaitForTicketOrderId(string gameCode, string dateTime)
        {
            return new Sports_Manager().QueryWaitForTicketOrderId(gameCode, dateTime);
        }

        #region 红人相关

        /// <summary>
        /// 查询红人的合买订单
        /// </summary>
        public TogetherHotUserInfoCollection QueryHotUserTogetherOrderList()
        {
            TogetherHotUserInfoCollection colleciton = new TogetherHotUserInfoCollection();

            var manager = new TogetherHotUserManager();
            var userList = manager.QueryTogetherHotUserInfo();
            var orderList = manager.QueryTogetherHotUserOrderInfo(userList.Select(p => p.UserId).ToArray());
            foreach (var item in userList)
            {
                var l = new TogetherHotUserOrderInfoCollection();
                l.AddRange(orderList.Where(p => p.CreateUserId == item.UserId).ToList());
                item.OrderList = l;
            }
            colleciton.AddRange(userList);
            return colleciton;
        }

        /// <summary>
        /// 添加红人
        /// </summary>
        public void AddTogetherHotUser(string userId)
        {
            var manager = new TogetherHotUserManager();
            var existCount = manager.QueryTogether(userId);
            if (existCount >= 1)
                throw new Exception("该用户已经是红人！");
            var userManager = new UserBalanceManager();
            var user = userManager.QueryUserRegister(userId);
            if (user == null)
                throw new Exception(string.Format("不存在的用户:{0}", userId));

            var entity = new TogetherHotUser()
            {
                UserId = userId,
                WeeksWinMoney = 0,
                CreateTime = DateTime.Now,
            };
            manager.AddTogetherHotUser(entity);
        }

        public void DeleteTogetherHotUser(string userId)
        {
            using (var manager = new TogetherHotUserManager())
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("用户编号不能为空！");
                var entity = manager.TogetherHotUserById(userId);
                if (entity == null || string.IsNullOrEmpty(entity.UserId))
                    throw new Exception("未查询到当前用户信息！");
                manager.DeleteTogetherHotUser(entity);

            }
        }

        #endregion

        #region 方案快照查询 2014.11.24 dj

        public OrderSnapshotDetailInfo_JC_Collection QueryJCBettingSnapshotInfo(string schemeId, string gameCode)
        {
            using (var manger = new Sports_Manager())
            {
                return manger.QueryJCBettingSnapshotInfo(schemeId, gameCode);
            }
        }

        public OrderSnapshotDetailInfo_PT_Collection QueryPTBettingSnapshotInfo(string schemeId)
        {
            using (var manger = new Sports_Manager())
            {
                return manger.QueryPTBettingSnapshotInfo(schemeId);
            }
        }

        public OrderSnapshotDetailInfo_PT_Collection QueryChaseBettingSnapshotInfo(string KeyLine)
        {
            using (var manger = new Sports_Manager())
            {
                return manger.QueryChaseBettingSnapshotInfo(KeyLine);
            }
        }

        public OrderSnapshotDetailInfo_Together_Collection QueryTogetherBettingSnapshotInfo(string schemeId)
        {
            using (var manger = new Sports_Manager())
            {
                return manger.QueryTogetherBettingSnapshotInfo(schemeId);
            }
        }

        #endregion

        //private static List<Sports_Order_Complate> CTZQBonusList = new List<Sports_Order_Complate>();
        public int GetHitMatchCount(string gameCode, string gameType, string issuseNumber, int hitMatch)
        {
            using (var manager = new Sports_Manager())
            {
                //if (CTZQBonusList == null || CTZQBonusList.Count <= 0)
                //{
                //    var result = manager.GetHitMatchCount(gameCode, gameType, issuseNumber, hitMatch);
                //    foreach (var item in result)
                //    {
                //        CTZQBonusList.Add(item);
                //    }
                //}
                //var listInfo = CTZQBonusList.Where(o => o.GameCode == gameCode && o.GameType == gameType && o.IssuseNumber == issuseNumber && o.HitMatchCount == hitMatch);
                //if (listInfo != null && listInfo.Count() > 0)
                //    return listInfo.Count();
                //return 0;
                var result = manager.GetHitMatchCount(gameCode, gameType, issuseNumber, hitMatch);
                if (result == null || result.Count <= 0)
                    return 0;
                return result.Count();
            }
        }

        #region 投注测试函数


        /// <summary>
        /// (测试)足彩普通投注
        /// </summary>
        public string Test_SportsBetting(Sports_BetingInfo info, string userId, bool isVirtualOrder)
        {
            string schemeId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                info.GameCode = info.GameCode.ToUpper();
                info.GameType = info.GameType.ToUpper();
                var gameCode = info.GameCode;

                schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);

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
                var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);

                // 检查订单金额是否匹配
                var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime);

                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                if (!user.IsEnable)
                    throw new Exception("用户已禁用");
                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.GeneralBetting, true, isVirtualOrder, user.UserId, user.AgentId, DateTime.Now, info.ActivityType, info.Attach, false, 0M,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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

                biz.CommitTran();
            }

            return schemeId;
        }
        public string Test_LotteryBetting(LotteryBettingInfo info, string userId, bool isVirtualOrder, out string keyLine)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 数据验证

                info.GameCode = info.GameCode.ToUpper();
                //info.GameType = info.GameType.ToUpper();

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
                        if (new string[] { "JCSJBGJ", "JCYJ" }.Contains(info.GameCode))
                            CheckSJBMatch(info.GameCode, int.Parse(item.AnteCode));

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

                var gameInfo = lotteryManager.LoadGame(info.GameCode);
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();
                keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
                var orderIndex = 1;
                var totalBetMoney = 0M;
                foreach (var issuse in info.IssuseNumberList)
                {
                    var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                    if (currentIssuseNumber == null)
                        throw new Exception(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                    if (currentIssuseNumber.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));

                    var schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        sportsManager.AddSports_AnteCode(new Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = BonusStatus.Waitting,
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
                        var gameType = lotteryManager.QueryGameType(info.GameCode, item.GameType);
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
                        sportsManager.AddLotteryScheme(new LotteryScheme
                        {
                            OrderIndex = orderIndex,
                            KeyLine = keyLine,
                            SchemeId = schemeId,
                            CreateTime = DateTime.Now,
                            IsComplate = false,
                            IssuseNumber = issuse.IssuseNumber,
                        });
                    }
                    var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                    AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                        string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                        info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting, orderIndex == 1, isVirtualOrder, user.UserId, user.AgentId,
                        orderIndex == 1 ? DateTime.Now : currentIssuseNumber.StartTime, info.ActivityType, "", info.IsAppend, 0M, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                    totalBetMoney += currentIssuseMoney;
                    orderIndex++;
                }

                biz.CommitTran();
            }
            return keyLine;
        }
        private Sports_Together Test_AddTogetherInfo(TogetherSchemeBase info, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
            SchemeSource schemeSource, TogetherSchemeSecurity security, int totalMatchCount, DateTime stopTime, bool isUploadAnteCode,
            decimal schemeDeduct, string userId, string userAgent, string balancePassword, int sysGuarantees, bool isTop, SchemeBettingCategory category, string issuseNumber, out bool canChase)
        {
            canChase = false;
            stopTime = stopTime.AddMinutes(-5);

            //if (DateTime.Now >= stopTime)
            //    throw new Exception(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                gameType = string.Empty;

            var schemeId = BusinessHelper.GetTogetherBettingSchemeId();
            var sportsManager = new Sports_Manager();

            //存入临时合买表
            sportsManager.AddTemp_Together(new Temp_Together
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                SchemeId = schemeId,
                StopTime = stopTime.ToString("yyyyMMddHHmm"),
            });

            //合买信息
            var main = new Sports_Together();
            main.BonusDeduct = info.BonusDeduct;
            main.SchemeDeduct = schemeDeduct;
            main.CreateTime = DateTime.Now;
            main.CreateUserId = userId;
            main.SchemeSource = schemeSource;
            main.SchemeBettingCategory = category;
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
            main.Security = security;
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

            var subItem = new Sports_TogetherJoin
            {
                AfterTaxBonusMoney = 0M,
                BuyCount = info.Subscription,
                RealBuyCount = info.Subscription,
                CreateTime = DateTime.Now,
                JoinType = TogetherJoinType.Subscription,
                JoinUserId = userId,
                Price = info.Price,
                SchemeId = schemeId,
                TotalMoney = info.Subscription * info.Price,
                JoinSucess = true,
                JoinLog = "认购合买",
            };
            sportsManager.AddSports_TogetherJoin(subItem);


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
                    if (item.SchemeCount == 0)
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
                var joinItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.FollowerJoin,
                    JoinUserId = item.FollowerUserId,
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = realBuyMoney,
                    JoinSucess = true,
                    JoinLog = "跟单参与合买",
                };
                sportsManager.AddSports_TogetherJoin(joinItem);

                //添加跟单记录
                sportsManager.AddTogetherFollowerRecord(new TogetherFollowerRecord
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
            main.ProgressStatus = TogetherSchemeProgress.SalesIn;

            if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Standard;
            if (main.SoldCount == main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Finish;

            #region 发起人保底

            var guaranteeMoney = info.Guarantees * info.Price;
            //扣钱
            if (guaranteeMoney > 0)
            {
                var minGuarantees = main.TotalCount - main.SoldCount;
                var guaranteeItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = info.Guarantees,
                    RealBuyCount = minGuarantees <= info.Guarantees ? minGuarantees : info.Guarantees,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.Guarantees,
                    JoinUserId = userId,
                    Price = info.Price,
                    SchemeId = schemeId,
                    TotalMoney = guaranteeMoney,
                    JoinSucess = true,
                    JoinLog = "合买保底",
                };
                sportsManager.AddSports_TogetherJoin(guaranteeItem);
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
                //记录系统保底
                sportsManager.AddSports_TogetherJoin(new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = main.SystemGuarantees,
                    RealBuyCount = systemGuarantees <= main.SystemGuarantees ? systemGuarantees : main.SystemGuarantees,
                    CreateTime = DateTime.Now,
                    JoinLog = "网站保底参与合买",
                    JoinSucess = false,
                    JoinType = TogetherJoinType.SystemGuarantees,
                    JoinUserId = "xtadmin",
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = systemGuarantees * main.Price,
                });
            }
            SetTogetherIsTop(main);
            sportsManager.AddSports_Together(main);

            return main;
        }
        /// <summary>
        /// 创建合买
        /// </summary>
        public string Test_CreateSportsTogether(Sports_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop
            , bool isVirtualOrder, out bool canChase, out DateTime stopTime, ref Sports_BetingInfo schemeInfo)
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
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                stopTime = CheckGeneralBettingMatch(sportsManager, info.GameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
                var betCount = 0;

                #region 计算注数
                if (info.GameCode == "BJDC" || info.GameCode == "JCZQ" || info.GameCode == "JCLQ")
                {
                    betCount = CheckBettingOrderMoney(info.AnteCodeList, info.GameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime);
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

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(userId);

                var issuseNumberOrTime = (info.GameCode == "JCZQ" || info.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.IssuseNumber;
                //添加合买信息
                var main = Test_AddTogetherInfo(info, info.TotalCount, info.TotalMoney, info.GameCode, info.GameType, info.PlayType, info.SchemeSource, info.Security, info.TotalMatchCount,
                    stopTime, true, schemeDeduct, user.UserId, user.AgentId, balancePassword,
                    sysGuarantees, isTop, SchemeBettingCategory.GeneralBetting, issuseNumberOrTime, out canChase);
                schemeId = main.SchemeId;

                //添加订单信息
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber,
                    info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.TogetherBetting, canChase, isVirtualOrder, user.UserId, user.AgentId, stopTime, info.ActivityType, info.Attach, info.IsAppend, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                //添加投注号码信息
                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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

                schemeInfo.GameCode = info.GameCode;
                schemeInfo.GameType = info.GameType;
                schemeInfo.IssuseNumber = info.IssuseNumber;
                schemeInfo.TotalMoney = info.TotalMoney;
                schemeInfo.SoldCount = main.SoldCount;
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }
            return schemeId;
        }

        #endregion

        #region 先发起后上传

        /// <summary>
        /// 先发起后上传_发起合买
        /// </summary>
        public string XianFaQiHSC_CreateSportsTogether(SingleScheme_TogetherSchemeInfo info, decimal schemeDeduct, string userId, string balancePassword, int sysGuarantees, bool isTop
            , out bool canChase, out DateTime stopTime, ref Sports_BetingInfo schemeInfo)
        {
            info.BettingInfo.GameCode = info.BettingInfo.GameCode.ToUpper();
            info.BettingInfo.GameType = info.BettingInfo.GameType.ToUpper();
            info.BettingInfo.PlayType = info.BettingInfo.PlayType.ToUpper();

            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new Exception("方案拆分不正确");
            if (info.Subscription < 1)
                throw new Exception("发起者至少认购1份");
            if (info.Subscription + info.Guarantees > info.TotalCount)
                throw new Exception("发起者认购份数和保底份数不能超过总份数");

            var schemeId = string.IsNullOrEmpty(info.BettingInfo.SchemeId) ? BusinessHelper.GetTogetherBettingSchemeId() : info.BettingInfo.SchemeId;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var betCount = 0;

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(userId);
                stopTime = Convert.ToDateTime(DateTime.Now.AddDays(4).Date.ToString("yyyy/MM/dd") + " 01:00:00");
                var issuseNumberOrTime = (info.BettingInfo.GameCode == "JCZQ" || info.BettingInfo.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.BettingInfo.IssuseNumber;
                //添加合买信息
                var main = AddTogetherInfo(info, schemeId, info.TotalCount, info.TotalMoney, info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.PlayType, info.BettingInfo.SchemeSource, info.BettingInfo.Security, 0,
                    stopTime, true, schemeDeduct, user.UserId, user.AgentId, balancePassword,
                    sysGuarantees, isTop, SchemeBettingCategory.XianFaQiHSC, issuseNumberOrTime, out canChase);
                schemeId = main.SchemeId;

                //添加订单信息
                AddRunningOrderAndOrderDetail(schemeId, info.BettingInfo.BettingCategory, info.BettingInfo.GameCode, info.BettingInfo.GameType, string.Empty, true, info.BettingInfo.IssuseNumber,
                    0, betCount, 0, info.TotalMoney, stopTime, info.BettingInfo.SchemeSource, info.BettingInfo.Security,
                    SchemeType.TogetherBetting, false, false, user.UserId, user.AgentId, info.BettingInfo.CurrentBetTime, info.BettingInfo.ActivityType, "", false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                biz.CommitTran();
            }

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }

        /// <summary>
        /// 先发起后上传_参与合买
        /// </summary>
        public bool XianFaQiHSC_JoinSportsTogether(string schemeId, int buyCount, string userId, string joinPwd, string balancePassword
            , ref Sports_BetingInfo schemeInfo)
        {
            Sports_Order_Running runningOrder = null;
            var canChase = false;
            var sportsManager = new Sports_Manager();
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var main = sportsManager.QuerySports_Together(schemeId);
                if (main == null) throw new Exception("合买订单不存在");
                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception(string.Format("查不到{0}的orderDetail 信息", schemeId));


                BusinessHelper.CheckDisableGame(orderDetail.GameCode, orderDetail.GameType);
                var orderAnteCode = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
                bool IsAnteCode = false;//通过投注内容判断用户是否已上传方案
                if (orderAnteCode != null && orderAnteCode.Count > 0)
                {
                    IsAnteCode = true;
                    if (DateTime.Now >= main.StopTime)
                        throw new Exception(string.Format("合买结束时间是{0}，现在不能参与合买。", main.StopTime.ToString("yyyy-MM-dd HH:mm:ss")));
                    if (main.ProgressStatus != TogetherSchemeProgress.SalesIn && main.ProgressStatus != TogetherSchemeProgress.Standard) throw new Exception("合买已完成，不能参与");
                }
                if (!string.IsNullOrEmpty(main.JoinPwd) && Encipherment.MD5(joinPwd) != main.JoinPwd)
                    throw new Exception("参与密码不正确");
                var surplusCount = main.TotalCount - main.SoldCount;
                if (surplusCount < buyCount)
                    throw new Exception(string.Format("方案剩余份数不足{0}份", buyCount));

                var buyMoney = main.Price * buyCount;
                if (buyMoney < 1)
                    throw new Exception("参与金额最少为1元");

                main.SoldCount += buyCount;
                main.JoinUserCount += sportsManager.IsUserJoinTogether(schemeId, userId) ? 0 : 1;
                main.Progress = (decimal)main.SoldCount / main.TotalCount;
                if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Standard;
                if (main.SoldCount == main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Finish;
                //不需要系统保底
                //if (main.SoldCount + main.Guarantees >= main.TotalCount)
                //    main.SystemGuarantees = 0;

                var joinItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    TotalMoney = buyMoney,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    Price = main.Price,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.Join,
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


                //计算用户真实保底
                surplusCount = main.TotalCount - main.SoldCount;
                var joinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                if (joinEntity != null && joinEntity.RealBuyCount > 0)
                {
                    if (surplusCount >= joinEntity.BuyCount)
                    {
                        //剩余份数 大于 用户保底数
                    }
                    if (surplusCount < joinEntity.BuyCount)
                    {
                        joinEntity.RealBuyCount = surplusCount;
                        joinEntity.TotalMoney = surplusCount * joinEntity.Price;
                        main.Guarantees = surplusCount;
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
                        if (joinEntity != null)
                            joinEntity.TotalMoney = surplusCount * joinEntity.Price;
                        main.SystemGuarantees = surplusCount;
                        sportsManager.UpdateSports_TogetherJoin(sysJoinEntity);
                    }
                }
                sportsManager.UpdateSports_Together(main);

                var order = sportsManager.QuerySports_Order_Running(schemeId);
                runningOrder = order;
                if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount && !order.CanChase)
                {
                    if (IsAnteCode)
                    {
                        canChase = true;
                        order.CanChase = true;
                    }
                    order.IsVirtualOrder = false;
                    sportsManager.UpdateSports_Order_Running(order);

                    orderDetail.IsVirtualOrder = false;
                    manager.UpdateOrderDetail(orderDetail);
                }

                schemeInfo.GameCode = order.GameCode;
                schemeInfo.GameType = order.GameType;
                schemeInfo.IssuseNumber = order.IssuseNumber;
                schemeInfo.TotalMoney = order.TotalMoney;
                schemeInfo.SchemeProgress = main.ProgressStatus;

                biz.CommitTran();
            }

            #region 拆票

            if (canChase)
            {
                if (RedisHelper.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrderSingle
                    {
                        RunningOrder = runningOrder,
                        AnteCode = new Sports_Manager().QuerySingleScheme_AnteCode(schemeId)
                    };
                    RedisOrderBusiness.AddOrderToRedis(runningOrder.GameCode, redisWaitOrder);
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

            return canChase;

        }

        /// <summary>
        /// 追加保底
        /// </summary>
        public bool XianFaQi_AddGuarantees(string schemeId, int buyCount, string userId, string balancePassword)
        {
            bool isCanChase = false;
            Sports_Order_Running runningOrder = null;
            var sportManager = new Sports_Manager();
            var order = sportManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                throw new Exception("未查询到订单信息");
            runningOrder = order;
            var main = sportManager.QuerySports_Together(schemeId);
            if (main == null)
                throw new Exception("未查询到合买信息");
            var surplusguarantees = main.TotalCount - main.SoldCount - main.Guarantees;//剩余保底份数
            if (buyCount > surplusguarantees)
                throw new Exception("保底份数不能大于剩余份数");
            var orderAnteCode = sportManager.QuerySportsAnteCodeBySchemeId(schemeId);
            bool IsAnteCode = false;//通过投注内容判断用户是否已上传方案
            if (orderAnteCode != null && orderAnteCode.Count > 0)
            {
                IsAnteCode = true;
                if (DateTime.Now >= main.StopTime)
                    throw new Exception(string.Format("合买结束时间是{0}，现在不能参与合买。", main.StopTime.ToString("yyyy-MM-dd HH:mm:ss")));
                if (main.ProgressStatus != TogetherSchemeProgress.SalesIn && main.ProgressStatus != TogetherSchemeProgress.Standard) throw new Exception("合买已完成，不能参与");
            }
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                main.Guarantees += buyCount;
                sportManager.UpdateSports_Together(main);

                if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Standard;

                var guaranteesInfo = sportManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                long joinId = 0;
                if (guaranteesInfo == null)
                {
                    var minGuarantees = main.TotalCount - main.SoldCount;
                    var guaranteeItem = new Sports_TogetherJoin
                    {
                        AfterTaxBonusMoney = 0M,
                        BuyCount = buyCount,
                        RealBuyCount = buyCount,
                        CreateTime = DateTime.Now,
                        JoinType = TogetherJoinType.Guarantees,
                        JoinUserId = userId,
                        Price = main.Price,
                        SchemeId = schemeId,
                        TotalMoney = buyCount * main.Price,
                        JoinSucess = true,
                        JoinLog = "合买保底",
                    };
                    sportManager.AddSports_TogetherJoin(guaranteeItem);
                    guaranteesInfo = sportManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                    joinId = guaranteeItem.Id;
                }
                else
                {
                    guaranteesInfo.BuyCount += buyCount;
                    guaranteesInfo.RealBuyCount += buyCount;
                    guaranteesInfo.TotalMoney += buyCount * guaranteesInfo.Price;
                    sportManager.UpdateSports_TogetherJoin(guaranteesInfo);
                    joinId = guaranteesInfo.Id;
                }
                var buyMoney = buyCount * main.Price;
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, string.Format("{0}_{1}", schemeId, joinId), buyMoney,
                        string.Format("订单{0}追加保底，支出{1:N2}元", schemeId, buyMoney), "Bet", balancePassword);


                var sysJoinEntity = sportManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                var surplusCount = main.TotalCount - main.SoldCount;
                surplusCount -= guaranteesInfo.RealBuyCount;
                if (surplusCount < 0)
                    surplusCount = 0;
                if (sysJoinEntity != null)
                {
                    if (surplusCount < main.SystemGuarantees)
                    {
                        sysJoinEntity.RealBuyCount = surplusCount;
                        sportManager.UpdateSports_TogetherJoin(sysJoinEntity);
                    }
                }

                if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount && !order.CanChase)
                {
                    if (IsAnteCode)
                    {
                        order.CanChase = true;
                        isCanChase = true;
                    }
                    sportManager.UpdateSports_Order_Running(order);
                }
                sportManager.UpdateSports_Together(main);

                biz.CommitTran();
            }

            #region 拆票

            if (isCanChase)
            {
                if (RedisHelper.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrderSingle
                    {
                        RunningOrder = runningOrder,
                        AnteCode = new Sports_Manager().QuerySingleScheme_AnteCode(schemeId)
                    };
                    RedisOrderBusiness.AddOrderToRedis(runningOrder.GameCode, redisWaitOrder);
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

            return isCanChase;
        }

        /// <summary>
        /// 上传号码
        /// </summary>
        public bool XianFaQi_UpLoadScheme(string schemeId, SingleScheme_TogetherSchemeInfo info)
        {
            Sports_Order_Running runningOrder = null;
            SingleScheme_AnteCode anteCodeSingle = null;
            info.BettingInfo.GameCode = info.BettingInfo.GameCode.ToUpper();
            info.BettingInfo.GameType = info.BettingInfo.GameType.ToUpper();
            info.BettingInfo.PlayType = info.BettingInfo.PlayType.ToUpper();

            var sportsManager = new Sports_Manager();
            var AnteCode = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            if (AnteCode != null && AnteCode.Count > 0)
                throw new Exception("您已经上传过方案");
            if (info.TotalCount * info.Price != info.TotalMoney)
                throw new Exception("方案拆分不正确");

            var selectMatchIdArray = info.BettingInfo.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = info.BettingInfo.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var codeText = Encoding.UTF8.GetString(info.BettingInfo.FileBuffer);
            //投注注数
            var betCount = 0;
            //所选比赛场数
            var totalMatchCount = 0;
            //所选比赛编号
            var selectMatchId = string.Empty;
            var jcCodeList = new List<string>();
            switch (info.BettingInfo.GameCode)
            {
                case "JCZQ":
                case "JCLQ":
                case "BJDC":
                    var matchIdList = new List<string>();
                    jcCodeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, info.BettingInfo.PlayType, info.BettingInfo.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
                    if (jcCodeList.Count * 2M * info.BettingInfo.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = jcCodeList.Count;
                    totalMatchCount = info.BettingInfo.ContainsMatchId ? matchIdList.Count : selectMatchIdArray.Length;
                    selectMatchId = info.BettingInfo.ContainsMatchId ? string.Join(",", matchIdList.ToArray()) : info.BettingInfo.SelectMatchId;
                    selectMatchIdArray = info.BettingInfo.ContainsMatchId ? matchIdList.ToArray() : selectMatchIdArray;
                    break;
                case "CTZQ":
                    var ctzqMatchIdList = new List<string>();
                    var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, info.BettingInfo.GameType, allowCodeArray, out ctzqMatchIdList);
                    if (ctzqCodeList.Count * 2M * info.BettingInfo.Amount != info.TotalMoney)
                        throw new Exception("投注金额不正确");
                    betCount = ctzqCodeList.Count;
                    totalMatchCount = ctzqMatchIdList.Count;
                    selectMatchIdArray = ctzqMatchIdList.ToArray();
                    selectMatchId = string.Join(",", selectMatchIdArray);
                    break;
            }
            bool isCanChase = false;
            var manager = new SchemeManager();
            var Order = sportsManager.QuerySports_Order_Running(schemeId);
            runningOrder = Order;
            var OrderDetail = manager.QueryOrderDetail(schemeId);
            var togetherInfo = sportsManager.QuerySports_Together(schemeId);

            if (Order == null || OrderDetail == null)
                throw new Exception("未查询到订单信息");
            var floatMoney = Math.Round(Order.TotalMoney * 0.3M);
            var lowerMoney = Order.TotalMoney - floatMoney;
            var UpperMoney = Order.TotalMoney + floatMoney;
            if (info.TotalMoney <= lowerMoney || info.TotalMoney >= UpperMoney)
                throw new Exception(string.Format("上传金额应在{0}到{1}之间", lowerMoney, UpperMoney));

            if (info.TotalCount < togetherInfo.TotalCount && togetherInfo.SoldCount > info.TotalCount)
                throw new Exception("上传金额不能小于已被认购的金额");
            var issuseNumberOrTime = (info.BettingInfo.GameCode == "JCZQ" || info.BettingInfo.GameCode == "JCLQ") ? DateTime.Now.ToString("yyyy-MM-dd") : info.BettingInfo.IssuseNumber;

            #region 计算投注结束时间

            var stopTime = DateTime.Now;
            switch (info.BettingInfo.GameCode)
            {
                case "BJDC":
                    var matchIdArray = (from l in selectMatchIdArray select string.Format("{0}|{1}", info.BettingInfo.IssuseNumber, l)).ToArray();
                    var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                    if (matchList.Count != matchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (matchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");
                    stopTime = matchList.Min(m => m.LocalStopTime);
                    break;
                case "JCZQ":
                    var jczqDsMatchList = sportsManager.QueryJCZQDSSaleMatchCount(selectMatchIdArray);
                    if (jczqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jczqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCZQ_Single(info.BettingInfo.GameCode, info.BettingInfo.GameType, jcCodeList, jczqDsMatchList);

                    stopTime = jczqDsMatchList.Min(m => m.DSStopBettingTime);
                    issuseNumberOrTime = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case "JCLQ":
                    var jclqDsMatchList = sportsManager.QueryJCLQDSSaleMatchCount(selectMatchIdArray);
                    if (jclqDsMatchList.Count != selectMatchIdArray.Length)
                        throw new ArgumentException("所选比赛中有停止销售的比赛。");
                    if (jclqDsMatchList.Count == 0)
                        throw new Exception("参数中没有包含场次信息");

                    //检查不支持的玩法
                    CheckPrivilegesType_JCLQ_Single(info.BettingInfo.GameCode, info.BettingInfo.GameType, jcCodeList, jclqDsMatchList);

                    stopTime = jclqDsMatchList.Min(m => m.DSStopBettingTime);

                    issuseNumberOrTime = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case "CTZQ":
                    var issuse = new LotteryGameManager().QueryGameIssuseByKey(info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber);
                    if (issuse == null)
                        throw new Exception(string.Format("{0},{1}奖期{2}不存在", info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber));
                    if (issuse.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("{0},{1}奖期{2}结束时间为{3}", info.BettingInfo.GameCode, info.BettingInfo.GameType, info.BettingInfo.IssuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                    stopTime = issuse.LocalStopTime;
                    break;
            }



            #endregion

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 修改订单信息

                var togetherTemp = sportsManager.QueryTemp_Together(schemeId);
                var joinGuaranteesEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.Guarantees);
                var sysJoinEntity = sportsManager.QuerySports_TogetherJoin(schemeId, TogetherJoinType.SystemGuarantees);
                if (info.TotalMoney != Order.TotalMoney)
                {
                    if (info.TotalMoney < Order.TotalMoney)//当上传金额，小于投注金额的时候
                    {
                        var currGuarantees = info.TotalCount - togetherInfo.SoldCount;//当前最大保底
                        if (joinGuaranteesEntity != null && joinGuaranteesEntity.RealBuyCount > currGuarantees)//如果原保底份数大于当前上传总份数时，需退还多余保底
                        {
                            var returnGuarantees = joinGuaranteesEntity.RealBuyCount - currGuarantees;//退还多余保底

                            togetherInfo.Guarantees = currGuarantees;
                            joinGuaranteesEntity.BuyCount = currGuarantees;
                            joinGuaranteesEntity.RealBuyCount = currGuarantees;
                            joinGuaranteesEntity.TotalMoney = currGuarantees * togetherInfo.Price;


                            var currSysGuarantees = currGuarantees - joinGuaranteesEntity.RealBuyCount;
                            if (currSysGuarantees < 0)
                                currSysGuarantees = 0;

                            if (sysJoinEntity != null)
                            {
                                if (currSysGuarantees < togetherInfo.SystemGuarantees)
                                {
                                    sysJoinEntity.RealBuyCount = currSysGuarantees;
                                    sportsManager.UpdateSports_TogetherJoin(sysJoinEntity);
                                }
                            }
                            var summary = string.Format("先发起后上传返还多余保底资金{0:N2}元", returnGuarantees * togetherInfo.Price);
                            if ((returnGuarantees * togetherInfo.Price) > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_ReturnGuarantees, joinGuaranteesEntity.JoinUserId
                                  , string.Format("{0}_{1}", schemeId, joinGuaranteesEntity.Id), returnGuarantees * togetherInfo.Price, summary);

                        }
                        else
                        {
                            var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));
                            sysGuarantees = (info.TotalCount * sysGuarantees / 100);
                            if (togetherInfo.SystemGuarantees > sysGuarantees)
                            {
                                togetherInfo.SystemGuarantees = sysGuarantees;
                                if (sysJoinEntity != null)
                                {
                                    sysJoinEntity.BuyCount = sysGuarantees;
                                    sysJoinEntity.RealBuyCount = sysGuarantees;
                                    sysJoinEntity.TotalMoney = sysGuarantees * togetherInfo.Price;
                                    sportsManager.UpdateSports_TogetherJoin(sysJoinEntity);
                                }
                            }

                        }
                    }
                    else if (info.TotalMoney > Order.TotalMoney)
                    {
                        var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));
                        sysGuarantees = (info.TotalCount * sysGuarantees / 100);

                        togetherInfo.SystemGuarantees = sysGuarantees;
                        if (sysJoinEntity != null)
                        {
                            sysJoinEntity.BuyCount = sysGuarantees;
                            sysJoinEntity.RealBuyCount = sysGuarantees;
                            sysJoinEntity.TotalMoney = sysGuarantees * togetherInfo.Price;
                            sportsManager.UpdateSports_TogetherJoin(sysJoinEntity);
                        }
                    }
                    if (joinGuaranteesEntity != null)
                        sportsManager.UpdateSports_TogetherJoin(joinGuaranteesEntity);
                    Order.TotalMoney = info.TotalMoney;
                    OrderDetail.TotalMoney = info.TotalMoney;
                    togetherInfo.TotalMoney = info.TotalMoney;
                    togetherInfo.TotalCount = info.TotalCount;
                }

                if (togetherInfo.SoldCount + togetherInfo.Guarantees + togetherInfo.SystemGuarantees >= togetherInfo.TotalCount)
                {
                    togetherInfo.ProgressStatus = TogetherSchemeProgress.Standard;
                    Order.CanChase = true;
                    isCanChase = true;
                }
                else
                {
                    togetherInfo.ProgressStatus = TogetherSchemeProgress.SalesIn;
                    Order.CanChase = false;
                    isCanChase = false;
                }
                if (Order.SchemeType == SchemeType.TogetherBetting)
                {
                    if (togetherInfo == null || togetherTemp == null)
                        throw new Exception("未查询到合买信息");
                    var togetherStopTime = stopTime.AddMinutes(-5);
                    //if (DateTime.Now >= stopTime)
                    //    throw new Exception(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

                    togetherTemp.StopTime = togetherStopTime.ToString("yyyyMMddHHmm");
                    sportsManager.UpdateTempTogether(togetherTemp);

                    togetherInfo.TotalMatchCount = selectMatchIdArray.Length;
                    togetherInfo.StopTime = togetherStopTime;
                    togetherInfo.CreateTimeOrIssuseNumber = issuseNumberOrTime;
                    togetherInfo.PlayType = info.BettingInfo.PlayType;
                    sportsManager.UpdateSports_Together(togetherInfo);
                }
                else
                {
                    //if (DateTime.Now >= stopTime)
                    //    throw new Exception(string.Format("订单结束时间是{0}，订单不能投注。", stopTime.ToString("yyyy-MM-dd HH:mm")));
                }


                Order.Amount = info.BettingInfo.Amount;
                Order.BetCount = betCount;
                Order.PlayType = info.BettingInfo.PlayType;
                Order.TotalMatchCount = selectMatchIdArray.Length;
                Order.StopTime = stopTime;
                Order.BetTime = stopTime;
                Order.QueryTicketStopTime = stopTime.AddMinutes(1).ToString("yyyyMMddHHmm");
                Order.IssuseNumber = issuseNumberOrTime;
                sportsManager.UpdateSports_Order_Running(Order);

                OrderDetail.Amount = info.BettingInfo.Amount;
                OrderDetail.PlayType = info.BettingInfo.PlayType;
                OrderDetail.CurrentIssuseNumber = issuseNumberOrTime;
                OrderDetail.StartIssuseNumber = issuseNumberOrTime;
                manager.UpdateOrderDetail(OrderDetail);

                #endregion

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(Order.UserId);

                #region 存入号码表
                anteCodeSingle = new SingleScheme_AnteCode
                {
                    SchemeId = schemeId,
                    GameCode = info.BettingInfo.GameCode,
                    GameType = info.BettingInfo.GameType,
                    PlayType = info.BettingInfo.PlayType,
                    IssuseNumber = info.BettingInfo.IssuseNumber,
                    CreateTime = DateTime.Now,
                    //AnteCodeFullFileName = info.BettingInfo.AnteCodeFullFileName,
                    FileBuffer = info.BettingInfo.FileBuffer,
                    AllowCodes = info.BettingInfo.AllowCodes,
                    ContainsMatchId = info.BettingInfo.ContainsMatchId,
                    SelectMatchId = selectMatchId,
                };
                sportsManager.AddSingleScheme_AnteCode(anteCodeSingle);

                foreach (var item in selectMatchIdArray)
                {
                    //获取过滤订单的母单信息
                    var code = info.BettingInfo.AnteCodeList.Where(p => p.MatchId == item && p.AnteCode != "*").Select(s => s.AnteCode);
                    if (code == null || code.ToList().Count() <= 0)
                        continue;
                    string anteCode = string.Join(",", code.ToList());

                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        GameCode = info.BettingInfo.GameCode,
                        GameType = info.BettingInfo.GameType,
                        PlayType = info.BettingInfo.PlayType,
                        BonusStatus = BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        IssuseNumber = info.BettingInfo.IssuseNumber,
                        SchemeId = schemeId,
                        MatchId = item,
                        Odds = string.Empty,
                        //AnteCode = code == null ? string.Empty : anteCode,
                        AnteCode = string.Empty,//不记录当前字段
                        IsDan = false,
                    });
                }

                #endregion

                biz.CommitTran();
            }

            #region 拆票

            if (isCanChase)
            {
                if (RedisHelper.EnableRedis)
                {
                    var redisWaitOrder = new RedisWaitTicketOrderSingle
                    {
                        RunningOrder = runningOrder,
                        AnteCode = anteCodeSingle,
                    };
                    RedisOrderBusiness.AddOrderToRedis(runningOrder.GameCode, redisWaitOrder);
                    //RedisOrderBusiness.AddOrderToWaitSplitList();
                }
                else
                {
                    DoSplitOrderTickets(schemeId);
                }
            }

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(Order.UserId);

            return isCanChase;
        }


        #endregion

        #region APP相关函数


        public BettingOrderInfoCollection QueryOrderListByBonusState(string strSate, string userId, string strSchemeType, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryOrderListByBonusState(strSate, userId, strSchemeType, startTime, endTime, pageIndex, pageSize);
            }
        }

        #endregion

        #region 赢家平台投注

        /// <summary>
        /// 赢家平台_足彩普通投注
        /// </summary>
        public string WinnerModel_SportsBetting(Sports_BetingInfo info, string userId, string password, string place, bool canChase, bool isVirtualOrder, bool isFrozen, bool isDeduction, decimal totalBettingMoeny, string modelKeyLine)
        {
            try
            {
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            string schemeId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                info.GameCode = info.GameCode.ToUpper();
                info.GameType = info.GameType.ToUpper();
                var gameCode = info.GameCode;

                schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);

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
                var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);

                // 检查订单金额是否匹配
                var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime);

                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                if (!user.IsEnable)
                    throw new Exception("用户已禁用");
                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.GeneralBetting, canChase, isVirtualOrder, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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


                if (!isFrozen)
                {
                    // 消费资金
                    string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                        : string.Format("{0} 投注", BusinessHelper.FormatGameCode(info.GameCode));

                    BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney, msg, place, password);
                }
                else if (isFrozen && isDeduction)
                {
                    //追号投注
                    BusinessHelper.Payout_To_Frozen(BusinessHelper.FundCategory_Betting, userId, modelKeyLine, totalBettingMoeny
                        , string.Format("模型追号订单{0}投注", schemeId), place, password);
                }

                if (isFrozen)//清理冻结资金
                    BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId,
                    string.Format("模型追号订单完成投注，扣除冻结资金:{0:N2}元", info.TotalMoney), info.TotalMoney);


                biz.CommitTran();
            }

            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }
        /// <summary>
        /// 赢家平台_数字彩投注
        /// </summary>
        public string WinnerModel_LotteryBetting(LotteryBettingInfo info, string userId, string balancePassword, string place, out string keyLine, bool canChase, bool isVirtualOrder, bool isFrozen, bool isDeduction, decimal totalBettingMoney, string modelKeyLine)
        {
            try
            {
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 数据验证

                info.GameCode = info.GameCode.ToUpper();
                //info.GameType = info.GameType.ToUpper();

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
                        if (new string[] { "JCSJBGJ", "JCYJ" }.Contains(info.GameCode))
                            CheckSJBMatch(info.GameCode, int.Parse(item.AnteCode));

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
                    var currentMoney = item.Amount * totalNumberZhu * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);
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

                var gameInfo = lotteryManager.LoadGame(info.GameCode);
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                var schemeManager = new SchemeManager();
                var sportsManager = new Sports_Manager();
                keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
                foreach (var issuse in info.IssuseNumberList)
                {
                    var currentIssuseNumber = lotteryManager.QueryGameIssuseByKey(info.GameCode, info.GameCode.ToUpper() == "CTZQ" ? info.AnteCodeList[0].GameType.ToUpper() : string.Empty, issuse.IssuseNumber);
                    if (currentIssuseNumber == null)
                        throw new Exception(string.Format("奖期{0}不存在", issuse.IssuseNumber));
                    if (currentIssuseNumber.LocalStopTime < DateTime.Now)
                        throw new Exception(string.Format("奖期{0}结束时间为{1}", issuse.IssuseNumber, currentIssuseNumber.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));

                    var schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
                    var gameTypeList = new List<GameTypeInfo>();
                    foreach (var item in info.AnteCodeList)
                    {
                        sportsManager.AddSports_AnteCode(new Sports_AnteCode
                        {
                            AnteCode = item.AnteCode,
                            BonusStatus = BonusStatus.Waitting,
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
                        var gameType = lotteryManager.QueryGameType(info.GameCode, item.GameType);
                        if (!gameTypeList.Contains(gameType))
                        {
                            gameTypeList.Add(gameType);
                        }
                    }
                    var currentIssuseMoney = totalNumberZhu * issuse.Amount * ((info.IsAppend && info.GameCode == "DLT") ? 3M : 2M);

                    if (info.IssuseNumberList.Count == 1)
                    {
                        keyLine = schemeId;
                    }
                    var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                    AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray()),
                        string.Empty, info.StopAfterBonus, issuse.IssuseNumber, issuse.Amount, totalNumberZhu, 0, currentIssuseMoney, currentIssuseNumber.OfficialStopTime, info.SchemeSource, info.Security,
                        info.IssuseNumberList.Count == 1 ? SchemeType.GeneralBetting : SchemeType.ChaseBetting, canChase, isVirtualOrder, user.UserId, user.AgentId,
                        info.CurrentBetTime, info.ActivityType, "", info.IsAppend, 0M, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                }

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
                    if (!isFrozen)
                    {
                        //普通投注
                        BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, info.TotalMoney
                            , string.Format("{0}第{1}期投注", gameInfo.DisplayName, info.IssuseNumberList[0].IssuseNumber), place, balancePassword);
                    }
                    else if (isFrozen && isDeduction)
                    {
                        //追号投注
                        BusinessHelper.Payout_To_Frozen(BusinessHelper.FundCategory_Betting, userId, modelKeyLine, totalBettingMoney
                            , string.Format("模型追号订单{0}投注", keyLine), place, balancePassword);
                    }

                    if (isFrozen)//清理冻结资金
                        BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine,
                        string.Format("模型追号订单完成投注，扣除冻结资金:{0:N2}元", info.TotalMoney), info.TotalMoney);
                }


                biz.CommitTran();
            }

            BusinessHelper.RefreshRedisUserBalance(userId);

            return keyLine;
        }

        #endregion

        #region

        /// <summary>
        /// 数字彩投注
        /// </summary>
        public string Extensions_LotteryBetting(LotteryBettingInfo info, string userId, string balancePassword, string place, decimal redBagMoney)
        {
            var schemeId = this.LotteryBetting(info, userId, balancePassword, place, redBagMoney);
            return schemeId;
        }
        /// <summary>
        /// 竞彩投注
        /// </summary>
        public string Extensions_SportsBetting(Sports_BetingInfo info, string userId, string password, string place, decimal redBagMoney)
        {
            //检查投注内容,并获取投注注数
            var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
            //检查投注的比赛，并获取最早结束时间
            var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);

            string schemeId = new Sports_Business().SportsBetting(info, userId, password, place, totalCount, stopTime, redBagMoney);

            return schemeId;
        }

        #endregion

        public string QueryTicketAbnormalOrderId()
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryTicketAbnormalOrderId();
            }
        }

        public void UpdateCoreConfigInfo(string configKey, string configValue)
        {
            //var manager=new UserIntegralManager();
            //var config = new CacheDataBusiness().QueryCoreConfigByKey("IsPauseBet");
            //if (config != null)
            //{
            //    config.ConfigValue=""
            //    new CacheDataBusiness().UpdateCoreConfigInfo(config);
            //    //config.ConfigValue = configKey;
            //    //var entity = manager.QueryCoreConfig(config.Id);
            //    //entity.ConfigValue=confi
            //    //manager.UpdateActivityPrizeConfig(configKey);
            //}
            var config = new CacheDataBusiness().QueryCoreConfigByKey(configKey);
            if (config != null)
            {
                config.ConfigKey = configKey;
                config.ConfigValue = configValue;
                config.CreateTime = DateTime.Now;
                //CoreConfigInfo info = new CoreConfigInfo();
                //info.ConfigKey = configKey;
                //info.ConfigValue = configValue;
                //info.ConfigName = "是否暂停投注";
                new CacheDataBusiness().UpdateCoreConfigInfo(config);
            }

        }
        public CoreConfigInfo QueryConfigByKey(string key)
        {
            return new CacheDataBusiness().QueryCoreConfigByKey(key);
        }

        #region 优化函数

        public HitMatchInfo GetHitMatchCount_YouHua(string gameCode, string gameType, int length)
        {
            HitMatchInfo info = new HitMatchInfo();
            var issuseNumber = new LotteryGameManager().QueryPrizedIssuseList(gameCode, gameType, length);
            if (string.IsNullOrEmpty(issuseNumber))
                return info;
            var hitMatchList = new Sports_Manager().GetHitMatchCount_YouHua(gameCode, string.Empty, issuseNumber);
            if (hitMatchList != null && hitMatchList.Count > 0)
            {
                var HitInfo_R9 = hitMatchList.Where(s => s.GameType == "TR9" && s.HitMatchCount == 9);
                if (HitInfo_R9 != null)
                    info.HitMatch_R9 = HitInfo_R9.Count();
                var HitInfo_14 = hitMatchList.Where(s => s.GameType == "T14C" && s.HitMatchCount == 14);
                if (HitInfo_14 != null)
                    info.HitMatch_14 = HitInfo_14.Count();
            }
            return info;
        }

        #endregion

        /// <summary>
        /// 订单打票成功
        /// </summary>
        public void SchemePrintTicket(string schemeId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var sportsManager = new Sports_Manager();
                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(schemeId);
                if (orderDetail == null)
                    throw new Exception(string.Format("订单{0}不存在.", schemeId));
                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception(string.Format("方案{0}不存在.", schemeId));
                if (order.TicketStatus == TicketStatus.Ticketing)
                {
                    order.TicketStatus = TicketStatus.PrintTicket;
                    sportsManager.UpdateSports_Order_Running(order);
                }
                if (orderDetail.TicketStatus == TicketStatus.Ticketing)
                {
                    orderDetail.TicketStatus = TicketStatus.PrintTicket;
                    manager.UpdateOrderDetail(orderDetail);
                }

                biz.CommitTran();
            }
        }

        #region 宝单分享

        /// <summary>
        /// 宝单分享-创建宝单
        /// </summary>
        public string SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId)
        {
            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;

            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;

            schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);
            var sportsManager = new Sports_Manager();
            //验证比赛是否还可以投注
            var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            // 检查订单金额是否匹配
            var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(userId);
                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                    info.SchemeSource, info.Security, SchemeType.SingleTreasure, true, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M,
                    canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);
                foreach (var item in info.AnteCodeList)
                {
                    var codeEntity = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                sportsManager.AddUserSaveOrder(new UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = SchemeType.SingleTreasure,
                    SchemeSource = info.SchemeSource,
                    SchemeBettingCategory = info.BettingCategory,
                    ProgressStatus = ProgressStatus.Waitting,
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

                biz.CommitTran();
            }


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
            var anteCodeList = new List<Sports_AnteCode>();
            Sports_Order_Running runningOrder = null;
            string schemeId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                info.GameCode = info.GameCode.ToUpper();
                info.GameType = info.GameType.ToUpper();
                var gameCode = info.GameCode;

                schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);

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
                var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.SingleCopy, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);


                foreach (var item in info.AnteCodeList)
                {
                    var codeEntity = new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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
                    BDFXRecordSingleCopy entity = new BDFXRecordSingleCopy();
                    entity.BDXFSchemeId = info.BDFXSchemeId;
                    entity.SingleCopySchemeId = schemeId;
                    entity.CreateTime = DateTime.Now;
                    BDFXManager.AddBDFXRecordSingleCopy(entity);
                }

                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BusinessHelper.FormatGameCode(info.GameCode));

                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney, msg, place, password);

                biz.CommitTran();
            }

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
        public Sports_TicketQueryInfoCollection QueryTicketListBySchemeId(string schemeId)
        {
            using (var manager = new Sports_Manager())
            {
                Sports_TicketQueryInfoCollection collection = new Sports_TicketQueryInfoCollection();
                collection.TotalCount = 0;
                var ticketList = manager.QueryTicketList(schemeId);
                if (ticketList == null || ticketList.Count < 0)
                    ticketList = manager.QueryTicketListHistory(schemeId);
                if (ticketList == null || ticketList.Count < 0)
                    return collection;
                collection.TotalCount = ticketList.Count;
                collection.TicketList = new List<Sports_TicketQueryInfo>();
                foreach (var item in ticketList)
                {
                    Sports_TicketQueryInfo info = new Sports_TicketQueryInfo();
                    info.AfterTaxBonusMoney = item.AfterTaxBonusMoney;
                    info.Amount = item.Amount;
                    info.BetMoney = item.BetMoney;
                    info.BetUnits = item.BetUnits;
                    info.BonusStatus = item.BonusStatus;
                    info.CreateTime = item.CreateTime;
                    info.GameCode = item.GameCode;
                    info.GameType = item.GameType;
                    info.IssuseNumber = item.IssuseNumber;
                    info.PlayType = item.PlayType;
                    info.PreTaxBonusMoney = item.PreTaxBonusMoney;
                    info.BarCode = item.BarCode;
                    info.PrintNumber1 = item.PrintNumber1;
                    info.PrintNumber2 = item.PrintNumber2;
                    info.PrintNumber3 = item.PrintNumber3;
                    info.SchemeId = item.SchemeId;
                    info.TicketId = item.TicketId;
                    info.TicketStatus = item.TicketStatus;
                    info.BetContent = item.BetContent;
                    info.LocOdds = item.LocOdds;
                    info.PrintDateTime = item.PrintDateTime;
                    collection.TicketList.Add(info);
                }
                return collection;
            }
        }


        #endregion

        #region 新版流程设计

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

            schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);

            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new Exception("用户已禁用");

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                    info.IssuseNumber, 1, betCount, info.TotalMatchCount, realTotalMoney, stopTime, info.SchemeSource, info.Security,
                    SchemeType.GeneralBetting, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);

                //用户的订单保存
                sportsManager.AddUserSaveOrder(new UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = SchemeType.SaveScheme,
                    SchemeSource = info.SchemeSource,
                    SchemeBettingCategory = info.BettingCategory,
                    ProgressStatus = ProgressStatus.Waitting,
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
                    sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = BonusStatus.Waitting,
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

                biz.CommitTran();
            }

            return schemeId;
        }

        /// <summary>
        /// 合买保存订单；注意：合买保存订单时不做扣款,但需要记录售出份数
        /// </summary>
        private Sports_Together SavaOrder_AddTogetherInfo(TogetherSchemeBase info, int totalCount, decimal totalMoney, string gameCode, string gameType, string playType,
            SchemeSource schemeSource, TogetherSchemeSecurity security, int totalMatchCount, DateTime stopTime, bool isUploadAnteCode,
            decimal schemeDeduct, string userId, string userAgent, string balancePassword, int sysGuarantees, bool isTop, SchemeBettingCategory category, string issuseNumber)
        {
            var canChase = false;
            stopTime = stopTime.AddMinutes(-5);

            if (DateTime.Now >= stopTime)
                throw new Exception(string.Format("订单结束时间是{0}，合买订单必须提前5分钟发起。", stopTime.ToString("yyyy-MM-dd HH:mm")));

            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" }.Contains(gameCode))
                gameType = string.Empty;

            var schemeId = BusinessHelper.GetTogetherBettingSchemeId();
            var sportsManager = new Sports_Manager();

            //存入临时合买表
            sportsManager.AddTemp_Together(new Temp_Together
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                SchemeId = schemeId,
                StopTime = stopTime.ToString("yyyyMMddHHmm"),
            });

            //合买信息
            var main = new Sports_Together();
            main.BonusDeduct = info.BonusDeduct;
            main.SchemeDeduct = schemeDeduct;
            main.CreateTime = DateTime.Now;
            main.CreateUserId = userId;
            main.SchemeSource = schemeSource;
            main.SchemeBettingCategory = category;
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
            main.Security = security;
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

            var subItem = new Sports_TogetherJoin
            {
                AfterTaxBonusMoney = 0M,
                BuyCount = info.Subscription,
                RealBuyCount = info.Subscription,
                CreateTime = DateTime.Now,
                JoinType = TogetherJoinType.Subscription,
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
                var joinItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = buyCount,
                    RealBuyCount = buyCount,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.FollowerJoin,
                    JoinUserId = item.FollowerUserId,
                    Price = main.Price,
                    SchemeId = schemeId,
                    TotalMoney = realBuyMoney,
                    JoinSucess = false,
                    JoinLog = "跟单参与合买",
                };
                sportsManager.AddSports_TogetherJoin(joinItem);

                //添加跟单记录
                sportsManager.AddTogetherFollowerRecord(new TogetherFollowerRecord
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
            main.ProgressStatus = TogetherSchemeProgress.SalesIn;

            if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Standard;
            if (main.SoldCount == main.TotalCount)
                main.ProgressStatus = TogetherSchemeProgress.Finish;

            #region 发起人保底

            var guaranteeMoney = info.Guarantees * info.Price;
            //扣钱
            if (guaranteeMoney > 0)
            {
                var minGuarantees = main.TotalCount - main.SoldCount;
                var guaranteeItem = new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = info.Guarantees,
                    RealBuyCount = minGuarantees <= info.Guarantees ? minGuarantees : info.Guarantees,
                    CreateTime = DateTime.Now,
                    JoinType = TogetherJoinType.Guarantees,
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
                sportsManager.AddSports_TogetherJoin(new Sports_TogetherJoin
                {
                    AfterTaxBonusMoney = 0M,
                    BuyCount = main.SystemGuarantees,
                    RealBuyCount = tempSystemGuarantees,
                    CreateTime = DateTime.Now,
                    JoinLog = "网站保底参与合买",
                    JoinSucess = false,
                    JoinType = TogetherJoinType.SystemGuarantees,
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

        /// <summary>
        /// 合买扣款
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="balancePassword"></param>
        public void BettingSavedCreateTogether(string schemeId, string balancePassword, out bool canChase, out bool isHMRenGou)
        {
            canChase = false;
            isHMRenGou = false;
            try
            {
                var sportsManager = new Sports_Manager();
                var main = sportsManager.QuerySports_Together(schemeId);
                var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception("未查询到当前订单");

                #region 扣除认购和保底金额

                var joinTogetherList = sportsManager.QuerySports_JoinTogetherList(schemeId);
                if (joinTogetherList != null && joinTogetherList.Count > 0)
                {
                    foreach (var item in joinTogetherList)
                    {
                        if (item.JoinType == TogetherJoinType.Subscription)
                        {
                            BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, main.CreateUserId, string.Format("{0}_{1}", schemeId, item.Id), main.Subscription * main.Price,
                                 string.Format("发起合买认购{0:N2}元", main.Subscription * main.Price), "Bet", balancePassword);
                            item.JoinSucess = true;
                            sportsManager.UpdateSports_TogetherJoin(item);
                            isHMRenGou = true;
                        }
                        else if (item.JoinType == TogetherJoinType.FollowerJoin)//自动跟单扣款
                        {
                            try
                            {
                                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, item.JoinUserId, string.Format("{0}_{1}", schemeId, item.Id), item.RealBuyCount * item.Price,
                               string.Format("跟单参与合买{0:N2}元", item.RealBuyCount * item.Price), string.Empty, string.Empty);
                                item.JoinSucess = true;
                                sportsManager.UpdateSports_TogetherJoin(item);
                            }
                            catch (Exception ex)
                            {
                                main.SoldCount = main.SoldCount - item.RealBuyCount;
                                main.JoinUserCount--;
                                item.JoinSucess = false;
                                item.RealBuyCount = 0;
                                item.TotalMoney = 0M;
                                item.JoinLog += "|合买扣款异常:" + ex.Message + "";
                                sportsManager.UpdateSports_TogetherJoin(item);//20160826
                                //sportsManager.DeleteTogetherJoin(item);
                                var strMsg = string.Format("失败原因：{0},跟单用户编号：{1},订单号：{2},参与Id：{3},参与份数：{4},实际参与份数：{5}", ex.ToString(), item.JoinUserId, item.SchemeId, item.Id, item.TotalMoney, item.RealBuyCount);
                                writer.Write("跟单合买扣款异常-BettingSavedCreateTogether", "跟单合买扣款异常-BettingSavedCreateTogether", LogType.Information, strMsg, strMsg);
                            }
                        }
                        else if (item.JoinType == TogetherJoinType.Guarantees)
                        {
                            var guaranteeMoney = main.Guarantees * main.Price;//此处扣除实际保底金额，这样在分析合买或者订单失败后退还保底才不会出错
                            BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, main.CreateUserId, string.Format("{0}_{1}", schemeId, item.Id), guaranteeMoney,
                            string.Format("发起合买保底{0:N2}元", guaranteeMoney), "Bet", balancePassword);
                            item.JoinSucess = true;
                            sportsManager.UpdateSports_TogetherJoin(item);
                        }
                    }
                }

                #endregion

                var surplusCount = main.TotalCount - main.SoldCount;
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

                main.Progress = (decimal)main.SoldCount / main.TotalCount;
                main.ProgressStatus = TogetherSchemeProgress.SalesIn;

                if (main.SoldCount + main.Guarantees >= main.TotalCount || main.SoldCount + main.Guarantees + main.SystemGuarantees >= main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Standard;
                if (main.SoldCount == main.TotalCount)
                    main.ProgressStatus = TogetherSchemeProgress.Finish;


                sportsManager.UpdateSports_Together(main);

                //是否出票
                if ((main.SoldCount + main.Guarantees + main.SystemGuarantees) >= main.TotalCount)
                    canChase = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #endregion

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
            var poolList = new List<Ticket_BonusPool>();
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

        /// <summary>
        /// 计算票中奖数据
        /// </summary>
        private void DoComputeTicketBonusMoney(Sports_Ticket ticket, string winNumber, out decimal totalPreMoney, out decimal totalAfterMoney)
        {
            totalPreMoney = 0M;
            totalAfterMoney = 0M;
            var ruleManaager = new BonusRuleManager();
            var bonusManager = new Ticket_BonusManager();
            foreach (var item in ticket.BetContent.Split('/'))
            {
                var ticketPreMoney = 0M;
                var ticketAfterMoney = 0M;
                var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(ticket.GameCode, ticket.GameType);
                var bonusLevelList = analyzer.CaculateBonus(item, winNumber);
                //HCount += AnalyzerFactory.GetHitCount(item, winNumber);
                foreach (var level in bonusLevelList)
                {
                    //取中奖规则
                    var rule = ruleManaager.GetBonusRule(ticket.GameCode, ticket.GameType, level);
                    if (rule == null)
                        throw new Exception(string.Format("票{0}找不到对应的奖金规则{1}", ticket.TicketId, level));
                    var money = rule.BonusMoney;
                    if (money == -1M)
                    {
                        var pool = bonusManager.GetBonusPool(ticket.GameCode, (ticket.GameCode.ToUpper() == "SSQ" || ticket.GameCode.ToUpper() == "DLT") ? "" : ticket.GameType, ticket.IssuseNumber, level.ToString());
                        if (pool == null)
                        {
                            throw new Exception("未更新奖池数据 - " + ticket.GameCode + "." + ticket.GameType);
                        }
                        //if (pool.BonusCount <= 0)
                        //{
                        //    throw new Exception("此奖等无人中奖 - " + ticket.GameCode + "." + ticket.GameType);
                        //}
                        money = pool.BonusMoney;
                    }

                    //追加投注
                    var appendMoney = 0M;
                    if (level <= 3 && ticket.IsAppend)
                        appendMoney = money * 0.6M;
                    else if (level > 3 && level <= 5 && ticket.IsAppend)
                        appendMoney = money * 0.5M;

                    var bonusMoney = (money + appendMoney) * ticket.Amount;
                    ticketPreMoney += bonusMoney;
                    var taxMoney = 0M;
                    //扣税
                    if ((money + appendMoney) >= 10000M)
                    {
                        taxMoney = (money + appendMoney) * 0.2M * ticket.Amount;
                    }
                    ticketAfterMoney += bonusMoney - taxMoney;
                }

                totalPreMoney += ticketPreMoney;
                totalAfterMoney += ticketAfterMoney;
            }
        }

        private void DoComputeTicketBonusMoney(string ticketId, string gameCode, string gameType, string betContent, int amount, bool isAppend, string issuseNumber, string winNumber, List<Ticket_BonusPool> poolList, out decimal totalPreMoney, out decimal totalAfterMoney)
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
        /// 订单手工派奖,只处理票数据
        /// </summary>
        public void ManualPrizeOrder(string orderId)
        {
            var orderDetail = new SchemeManager().QueryOrderDetail(orderId);
            if (orderDetail == null)
                throw new Exception(string.Format("找不到方案{0}的OrderDetail订单信息", orderId));

            if (new string[] { "BJDC", "JCZQ", "JCLQ" }.Contains(orderDetail.GameCode))
            {
                //竞彩和北单
                switch (orderDetail.GameCode)
                {
                    case "BJDC":
                        new TicketGatewayAdmin().PrizeBJDCTicket_OrderId(orderId);
                        break;
                    case "JCZQ":
                        new TicketGatewayAdmin().PrizeJCZQTicket_OrderId(orderId);
                        break;
                    case "JCLQ":
                        new TicketGatewayAdmin().PrizeJCLQTicket_OrderId(orderId);
                        break;
                    default:
                        throw new Exception("派奖彩种异常" + orderDetail.GameCode);
                }
                string str = DoOrderPrize(orderId);
                return;
            }
            //传统足球和数字彩
            var poolManager = new Ticket_BonusManager();
            var manager = new Sports_Manager();

            var ticketList = manager.QueryTicketList(orderId);
            if (ticketList.Count <= 0)
                throw new Exception("订单无票数据");
            var poolList = new List<Ticket_BonusPool>();
            var issuseEntity = new LotteryGameManager().QueryIssuse(orderDetail.GameCode, orderDetail.GameCode == "CTZQ" ? orderDetail.GameType : "", orderDetail.CurrentIssuseNumber);
            if (issuseEntity == null || string.IsNullOrEmpty(issuseEntity.WinNumber))
                throw new Exception("无开奖信息");
            foreach (var item in ticketList)
            {
                try
                {
                    //查询奖池
                    if (new string[] { "SSQ", "DLT", "CTZQ" }.Contains(orderDetail.GameCode))
                    {
                        if (poolList.Count <= 0)
                        {
                            poolList.AddRange(poolManager.GetBonusPool(item.GameCode, orderDetail.GameCode == "CTZQ" ? orderDetail.GameType : "", item.IssuseNumber));
                        }
                    }

                    var totalPreMoney = 0M;
                    var totalAfterMoney = 0M;
                    //计算中奖金额
                    DoComputeTicketBonusMoney(item.TicketId, item.GameCode.ToUpper(), item.GameType.ToUpper(), item.BetContent, item.Amount, item.IsAppend, item.IssuseNumber, issuseEntity.WinNumber, poolList, out totalPreMoney, out totalAfterMoney);

                    //更新票数据sql

                    var updateSql = string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where ticketId='{3}' "
                        , totalPreMoney, totalAfterMoney, totalAfterMoney > 0M ? 20 : 30, item.TicketId);
                    manager.ExecSql(updateSql);
                }
                catch (Exception ex)
                {
                    //log.Add(string.Format("票{0} 派奖异常：{1}", item.TicketId, ex.Message));
                }
            }

            string str2 = DoOrderPrize(orderId);
        }

        /// <summary>
        /// 查询订单状态和票状态不一至的订单
        /// </summary>
        public string QueryErrorTicketOrder(int count)
        {
            return new Sports_Manager().QueryErrorTicketOrder(count);
        }

        /// <summary>
        /// 手工出票（拆票），用于订单状态和票状态不一至的时候
        /// </summary>
        public void ManualBet(string orderId)
        {
            var schemeManager = new SchemeManager();
            //var orderDetail = schemeManager.QueryOrderDetail(orderId);
            //if (orderDetail == null)
            //    throw new Exception(string.Format("找不到方案{0}的OrderDetail订单信息", orderId));

            var manager = new Sports_Manager();
            var order = manager.QuerySports_Order_Running(orderId);
            if (order == null)
                throw new Exception("找不到订单的Order_Running数据，可能订单已派奖");


            //var complate_order = manager.QuerySports_Order_Complate(orderId);
            //if (complate_order != null)
            //{

            //}


            order.TicketTime = DateTime.Now;
            order.TicketStatus = TicketStatus.Ticketed;
            order.ProgressStatus = ProgressStatus.Running;
            order.IsSplitTickets = false;


            //orderDetail.CurrentBettingMoney = orderDetail.TotalMoney;
            //orderDetail.ProgressStatus = ProgressStatus.Running;
            //orderDetail.TicketStatus = TicketStatus.Ticketed;
            //orderDetail.TicketTime = DateTime.Now;

            //manager.UpdateSports_Order_Running(order);
            //schemeManager.UpdateOrderDetail(orderDetail);
            //拆票
            DoSplitOrderTicketsWithNoThread(orderId, order);





        }

        /// <summary>
        /// 查询未派奖的订单并派奖
        /// </summary>
        public string QueryUnPrizeOrderAndDoPrize(string gameCode, int count)
        {
            if (string.IsNullOrEmpty(gameCode))
                throw new Exception("彩种不能为空");
            //if (string.IsNullOrEmpty(issuseNumber))
            //    throw new Exception("期号不能为空");

            var watch = new Stopwatch();
            var log = new List<string>();
            var manager = new Sports_Manager();
            watch.Start();
            var schemeIdList = manager.QueryUnPrizeOrder(gameCode, count);
            watch.Stop();
            log.Add(string.Format("查询{0}-{1}，用时{2}毫秒", gameCode, count, watch.Elapsed.TotalMilliseconds));
            foreach (var item in schemeIdList)
            {
                watch.Reset();
                string str = DoOrderPrize(item);
                watch.Stop();
                log.Add(string.Format("订单{0}派奖用时{1}毫秒，结果：{2}", item, watch.Elapsed.TotalMilliseconds, str));
            }

            return string.Join(Environment.NewLine, log.ToArray());
        }

        public string DoOrderPrize(string schemeId)
        {
            var manager = new Sports_Manager();
            try
            {
                var ticketList = manager.QueryTicketList(schemeId).Distinct(new Sports_TicketComparer()).ToList();
                if (ticketList.Count <= 0)
                    return string.Format("订单{0}没有生成票数据", schemeId);
                //订单的票中包括未派奖的票
                if (ticketList.Where(p => p.BonusStatus != BonusStatus.Lose && p.BonusStatus != BonusStatus.Win).Count() > 0)
                    return string.Format("订单{0}包括未派奖的票数据", schemeId);

                //订单派奖
                var preMoney = ticketList.Sum(p => p.PreTaxBonusMoney);
                var afterMoney = ticketList.Sum(p => p.AfterTaxBonusMoney);
                //数字彩奖期无开奖号的情况
                if (preMoney < 0M)
                    preMoney = -1M;
                if (afterMoney < 0M)
                    afterMoney = -1M;

                //执行订单派奖
                BusinessHelper.DoOrderPrize(schemeId, afterMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, preMoney, afterMoney);
                return "派奖完成";
            }
            catch (Exception ex)
            {
                return string.Format("订单{0} 派奖异常：{1}", schemeId, ex.ToString());
            }
        }


        /// <summary>
        /// 自动服务更新订单票状态
        /// </summary>
        //public void UpdateOrderAndTicketStatus(string noChaseGameCodeStr, int afterSeconds)
        //{
        //    var noChaseGameCodeArray = noChaseGameCodeStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //    var sqlCondition = string.Empty;
        //    if (noChaseGameCodeArray.Length > 0)
        //    {
        //        var t = noChaseGameCodeArray.Select(p => string.Format("'{0}'", p)).ToArray();
        //        sqlCondition = string.Format(" and gameCode not in ({0})", string.Join(",", t));
        //    }
        //    var sportsManager = new Sports_Manager();
        //    var sql = string.Empty;
        //    sql += string.Format(" update [C_Sports_Order_Running] set TicketStatus=90 where TicketStatus=10 and CanChase=1 and DATEADD(second,{0}, CreateTime)<getdate() {1} {2}", afterSeconds, sqlCondition, Environment.NewLine);
        //    sql += string.Format(" update C_OrderDetail set TicketStatus=90 where TicketStatus=10 and  DATEADD(second,{0}, CreateTime)<getdate() {1} {2}", afterSeconds, sqlCondition, Environment.NewLine);
        //    sql += string.Format(" update C_Sports_Ticket set TicketStatus=90 where TicketStatus=10 and  DATEADD(second,{0}, CreateTime)<getdate() {1} ", afterSeconds, sqlCondition);
        //    sportsManager.ExecSql(sql);
        //}

        /// <summary>
        /// 查询未拆票的订单
        /// </summary>
        public string QueryUnSplitTicketsOrder(int count)
        {
            var allGameCodeArray = new string[] { "CQSSC", "JX11X5", "FC3D", "PL3", "SSQ", "DLT", "CTZQ", "BJDC", "JCZQ", "JCLQ", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            var unTicketGameCode = new List<string>();
            foreach (var item in allGameCodeArray)
            {
                if (!BusinessHelper.CanRequestBet(item))
                    unTicketGameCode.Add(item);
            }

            var log = new List<string>();
            var sportsManager = new Sports_Manager();
            var schemeIdList = sportsManager.QueryUnSplitTicketsOrder(unTicketGameCode, count);
            foreach (var item in schemeIdList)
            {
                try
                {
                    log.Add(DoSplitOrderTicketsWithNoThread(item));
                }
                catch (Exception ex)
                {
                    log.Add(string.Format("订单{0}拆票异常：{1}", item, ex.ToString()));
                }
            }
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
                    this.writer.Write("DoSplitOrderTickets", "DpSplitOrderTicketsWithNoThread", ex);
                }
            }, schemeId);

            //new Thread(() =>
            //{


            //}).Start();

            //return "拆分票成功";
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

            if (!BusinessHelper.CanRequestBet(order.GameCode))
                //return;
                return string.Format("彩种{0}暂时不能出票", order.GameCode);
            var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            if (anteCodeList.Count <= 0)
                //return;
                return string.Format("订单{0}无投注内容", schemeId);

            if (order.SchemeType == SchemeType.ChaseBetting)
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
                if (order.SchemeBettingCategory == SchemeBettingCategory.SingleBetting || order.SchemeBettingCategory == SchemeBettingCategory.XianFaQiHSC)
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
                            var json = JsonSerializer.Serialize(betInfo);
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

                        RequestTicket(betInfo, "", true, order.SchemeType);

                        //new Thread(() =>
                        //{

                        try
                        {
                            //生成文件
                            var json = JsonSerializer.Serialize(betInfo);
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
            order.TicketStatus = TicketStatus.Ticketed;
            order.ProgressStatus = ProgressStatus.Running;
            sportsManager.UpdateSports_Order_Running(order);

            var detailManager = new SchemeManager();
            var detail = detailManager.QueryOrderDetail(schemeId);
            if (detail != null)
            {
                if (!detail.TicketTime.HasValue)
                    detail.TicketTime = DateTime.Now;
                detail.TicketStatus = TicketStatus.Ticketed;
                detail.ProgressStatus = ProgressStatus.Running;
                detail.CurrentBettingMoney = detail.TotalMoney;
                detailManager.UpdateOrderDetail(detail);
            }
            watch.Stop();
            log.Add("3)拆分和修改订单数据 " + watch.Elapsed.TotalMilliseconds);
            //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));
            return string.Join(Environment.NewLine, log);
        }

        /// <summary>
        /// 拆票（无线程）
        /// </summary>
        public string DoSplitOrderTicketsWithNoThread(string schemeId, Sports_Order_Running order)
        {
            var log = new List<string>();
            try
            {
                log.Add(string.Format("----------->>>>>>开始对订单{0}做票拆分", schemeId));
                var watch = new Stopwatch();
                watch.Start();
                var sportsManager = new Sports_Manager();
                //var order = sportsManager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    //return;
                    return string.Format("找不到订单{0}的订单数据", schemeId);

                if (!BusinessHelper.CanRequestBet(order.GameCode))
                    //return;
                    return string.Format("彩种{0}暂时不能出票", order.GameCode);
                var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
                if (anteCodeList.Count <= 0)
                    //return;
                    return string.Format("订单{0}无投注内容", schemeId);

                if (order.SchemeType == SchemeType.ChaseBetting)
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
                    if (order.SchemeBettingCategory == SchemeBettingCategory.SingleBetting || order.SchemeBettingCategory == SchemeBettingCategory.XianFaQiHSC)
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
                                var json = JsonSerializer.Serialize(betInfo);
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


                            //写入redis派奖
                            //RequestTicket(betInfo, "", true, order.SchemeType);
                            RequestTicket2(betInfo, "", true, order.SchemeType);
                            //new Thread(() =>
                            //{

                            try
                            {
                                //生成文件
                                var json = JsonSerializer.Serialize(betInfo);
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
                order.TicketStatus = TicketStatus.Ticketed;
                order.ProgressStatus = ProgressStatus.Running;


                //var detailManager = new SchemeManager();
                ////var detail = detailManager.QueryOrderDetail(schemeId);
                //if (detail != null)
                //{
                //    if (!detail.TicketTime.HasValue)
                //        detail.TicketTime = DateTime.Now;
                //    detail.TicketStatus = TicketStatus.Ticketed;
                //    detail.ProgressStatus = ProgressStatus.Running;
                //    detail.CurrentBettingMoney = detail.TotalMoney;
                //    //detailManager.UpdateOrderDetail(detail);
                //}
                //sportsManager.UpdateSports_Order_Running(order);


                //更新订单状态
                var sql = string.Format(@"update [C_Sports_Order_Running] set TicketStatus=90,ProgressStatus=10,TicketTime=getdate(),IsSplitTickets=1 where [SchemeId]='{0}'
                                    update C_OrderDetail set CurrentBettingMoney=TotalMoney,TicketStatus=90,ProgressStatus=10,TicketTime=getdate() where [SchemeId]='{0}'", schemeId);
                sportsManager.ExecSql(sql);

                watch.Stop();
                log.Add("3)拆分和修改订单数据 " + watch.Elapsed.TotalMilliseconds);
                //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));
                //return string.Join(Environment.NewLine, log);
            }
            catch (Exception exp)
            {
                this.writer.Write("DoSplitOrderTicketsError", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()) + "/r/nError:" + exp.Message);
            }
            return string.Join(Environment.NewLine, log);
        }

        /// <summary>
        /// 自动更新可追号的订单状态
        /// </summary>
        public string UpdateCanChaseOrderStatus(string noChaseGameCodeStr, int afterSeconds)
        {
            var log = new List<string>();
            var successCount = 0;
            var failCount = 0;
            var noChaseGameCodeArray = noChaseGameCodeStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var sportsManager = new Sports_Manager();
            var orderList = sportsManager.QueryWaitTicketOrder(noChaseGameCodeArray, afterSeconds);
            var updateSchemeIdStr = string.Join(",", orderList.Select(p => string.Format("'{0}'", p.SchemeId)).ToArray());
            if (orderList.Count > 0)
            {
                var sql = string.Format(@"--更新
                                      update C_OrderDetail set TicketStatus=90,ProgressStatus=10,[CurrentBettingMoney]=[TotalMoney]  where SchemeId in ({0})
                                      update C_Sports_Order_Running set TicketStatus=90,ProgressStatus=10 where SchemeId in ({0})
                                      update C_Sports_Ticket set TicketStatus=90 where SchemeId in ({0}) ", updateSchemeIdStr);
                sportsManager.ExecSql(sql);

                //foreach (var order in orderList)
                //{
                //    try
                //    {
                //        BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.UserId, order.SchemeId, order.TotalMoney, order.TotalMoney });
                //        successCount++;
                //    }
                //    catch (Exception ex)
                //    {
                //        failCount++;
                //        log.Add(string.Format("订单{0}执行插件异常：{1}", order.SchemeId, ex.Message));
                //    }
                //}
            }
            log.Insert(0, string.Format("成功：{0}条，失败：{1}条", successCount, failCount));
            return string.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        /// 未开出对应彩种奖期开奖号，按订单本金归还
        /// 更新奖期状态
        /// </summary>
        public string QueryOrderAndFundOrder(string gameCode, string issuse)
        {
            try
            {
                //var manager = new Sports_Manager();
                //manager.SetNotOpenTickets(gameCode, issuse);
                //return "更新票数据完成";
                var manager = new LotteryGameManager();
                var entity = manager.QueryGameIssuse(gameCode, issuse);
                if (entity == null)
                    throw new Exception(string.Format("找不到奖期信息：{0}--{1}", gameCode, issuse));
                if (entity.Status == IssuseStatus.Stopped)
                    throw new Exception("奖期已派奖");
                entity.Status = IssuseStatus.Stopped;
                entity.WinNumber = string.Empty;
                entity.AwardTime = DateTime.Now;
                manager.UpdateGameIssuse(entity);
                return "更新奖期数据完成";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }



            //var log = new List<string>();
            //var ticketList = manager.QueryTicketList(gameCode, issuse);
            //if (ticketList.Count <= 0) return string.Empty;

            //try
            //{
            //    manager.UpdateTicketList(ticketList.Select(p => p.TicketId).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("更新票数据出错 - " + ex.Message);
            //}

            //var orderList = ticketList.GroupBy(p => p.SchemeId).Select(o => o.Key).ToList();
            //foreach (var item in orderList)
            //{
            //    try
            //    {
            //        //处理订单并退钱
            //        DoOrderPrize(item, BonusStatus.Lose, -1, -1);
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Add(string.Format("订单{0}派奖异常：{1}", item, ex.Message));
            //    }
            //}

            //return string.Format("未开出对应彩种奖期开奖号，按订单本金归还处理异常订单。{0}", string.Join(Environment.NewLine, log.ToArray()));
        }

        /// <summary>
        /// 移动已完成的票数据
        /// </summary>
        public void MoveComplateTicket(int count)
        {
            var manager = new Sports_Manager();
            var ticketList = manager.QueryComplateTicketListTop(count);
            if (ticketList.Count <= 0) return;

            var idList = new List<long>();
            var ticketTable = GetTicketHistoryTable();
            foreach (var item in ticketList)
            {
                DataRow r = ticketTable.NewRow();
                r["Id"] = item.Id;
                r["SchemeId"] = item.SchemeId;
                r["TicketId"] = item.TicketId;
                r["GameCode"] = item.GameCode;
                r["GameType"] = item.GameType;
                r["PlayType"] = item.PlayType;
                r["MatchIdList"] = item.MatchIdList;
                r["IssuseNumber"] = item.IssuseNumber;
                r["BetUnits"] = item.BetUnits;
                r["Amount"] = item.Amount;
                r["BetMoney"] = item.BetMoney;
                r["BetContent"] = item.BetContent;
                r["LocOdds"] = item.LocOdds;
                r["TicketStatus"] = item.TicketStatus;
                r["TicketLog"] = item.TicketLog;
                r["PartnerId"] = "";
                r["Palmid"] = "";
                r["PrintNumber1"] = item.PrintNumber1;
                r["PrintNumber2"] = item.PrintNumber2;
                r["PrintNumber3"] = item.PrintNumber3;
                r["BarCode"] = item.BarCode;
                r["PrintOdd"] = "";
                r["PrintUnOdd"] = "";
                r["BonusStatus"] = item.BonusStatus;
                r["PreTaxBonusMoney"] = item.PreTaxBonusMoney;
                r["AfterTaxBonusMoney"] = item.AfterTaxBonusMoney;
                if (item.PrintDateTime.HasValue)
                    r["PrintDateTime"] = item.PrintDateTime;
                else
                    r["PrintDateTime"] = DBNull.Value;
                r["Gateway"] = string.IsNullOrEmpty(item.Gateway) ? string.Empty : item.Gateway;
                if (item.CreateTime != DateTime.MinValue)
                    r["CreateTime"] = item.CreateTime;
                else
                    r["CreateTime"] = DBNull.Value;
                r["IsAppend"] = item.IsAppend;
                if (item.PrizeDateTime.HasValue)
                    r["PrizeDateTime"] = item.PrizeDateTime;
                else
                    r["PrizeDateTime"] = DBNull.Value;
                ticketTable.Rows.Add(r);

                idList.Add(item.Id);
            }

            //批量插入票表
            manager.SqlBulkAddTable(ticketTable);

            var deleteSql = string.Format("delete [C_Sports_Ticket] where id in ({0})", string.Join(",", idList));
            manager.ExecSql(deleteSql);
        }

        private DataTable GetTicketHistoryTable()
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
            ticketTable.Columns.Add("PrizeDateTime", typeof(DateTime));
            return ticketTable;
        }

        #endregion

        #region 原ticket操作

        #region 北单、竞彩投注拆票

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

        private string CreateInsertSql(GatewayTicketOrder orderInfo, Ticket ticket, InnerOrderInfo info, string locOdds)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"INSERT INTO dbo.C_Sports_Ticket (SchemeId, TicketId, GameCode, GameType, PlayType,  IssuseNumber, BetUnits, Amount, BetMoney, BetContent, LocOdds, TicketStatus, PrintNumber3, BonusStatus, PreTaxBonusMoney, AfterTaxBonusMoney, PrintDateTime, CreateTime, IsAppend)
VALUES ('{0}','{1}','{2}','{3}','{4}', '{5}',{6}, {7}, {8}, '{9}', '{10}', {11}, '{12}', {13}, {14}, {15}, '{16}', '{17}', {18})", orderInfo.OrderId, info.OrderId, orderInfo.GameCode.ToUpper(), ticket.GameType.ToUpper(), info.BetType, orderInfo.IssuseNumber, info.BetUnits, info.Multiple, info.BetMoney, info.BetContent, locOdds, 90, Guid.NewGuid().ToString("N").ToUpper(), 0, 0, 0, DateTime.Now, DateTime.Now, orderInfo.IsAppend == true ? 1 : 0);
            return sql.ToString();
        }

        private DataTable GetNewSportsAnteCodeTable()
        {
            var sportsAnteCodeTable = new DataTable("C_Sports_AnteCode");
            sportsAnteCodeTable.Columns.Add("Id", typeof(long));
            sportsAnteCodeTable.Columns.Add("SchemeId", typeof(string));
            sportsAnteCodeTable.Columns.Add("GameCode", typeof(string));
            sportsAnteCodeTable.Columns.Add("GameType", typeof(string));
            sportsAnteCodeTable.Columns.Add("PlayType", typeof(string));
            sportsAnteCodeTable.Columns.Add("IssuseNumber", typeof(string));
            sportsAnteCodeTable.Columns.Add("MatchId", typeof(string));
            sportsAnteCodeTable.Columns.Add("AnteCode", typeof(string));
            sportsAnteCodeTable.Columns.Add("IsDan", typeof(bool));
            sportsAnteCodeTable.Columns.Add("Odds", typeof(string));
            sportsAnteCodeTable.Columns.Add("BonusStatus", typeof(int));
            sportsAnteCodeTable.Columns.Add("CreateTime", typeof(DateTime));
            sportsAnteCodeTable.PrimaryKey = new DataColumn[] { sportsAnteCodeTable.Columns["Id"] };
            return sportsAnteCodeTable;
        }

        public void RequestTicket_Sport(IEntityOrder order, bool isTrue, Dictionary<string, string> matchIdOddsList, string[] matchIdArray)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var matchIdList = new List<string>();

            var manager = new Sports_Manager();
            var ticketTable = GetNewTicketTable();
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
                            throw new Exception("投注比赛内容出错 - " + item);
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
                DataRow r = ticketTable.NewRow();
                r["Id"] = count;
                r["GameCode"] = ticket.GameCode.ToUpper();
                r["GameType"] = ticket.GameType.ToUpper();
                r["SchemeId"] = ticket.OrderId;
                r["TicketId"] = ticket.Id;
                r["IssuseNumber"] = ticket.IssuseNumber;
                r["PlayType"] = ticket.PlayType;
                r["BetContent"] = betContent;
                r["LocOdds"] = locOddStr;
                r["TicketStatus"] = 90;
                r["Amount"] = ticket.Amount;
                r["BetUnits"] = ticket.BetCount;
                r["BetMoney"] = ticket.TotalMoney;
                r["MatchIdList"] = ticket.ToAnteString_zhongminToMatchId();
                r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                r["IsAppend"] = false;
                r["BonusStatus"] = 0;
                r["PreTaxBonusMoney"] = 0M;
                r["AfterTaxBonusMoney"] = 0M;
                r["PrintDateTime"] = DateTime.Now;
                r["CreateTime"] = DateTime.Now;

                ticketTable.Rows.Add(r);

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
            if (RedisHelper.EnableRedis)
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

        #endregion

            if (!result.TicketList.Sum(t => t.TotalMoney).Equals(order.TotalMoney))
            {
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", result.TicketList.Sum(t => t.TotalMoney), order.TotalMoney));
            }
            return result;
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
                    throw new Exception("优化投注拆票有错！");

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
                //    throw new Exception("优化投注拆票有错！");
                var anteList = arrayList[0].Split('|');
                foreach (var ante in anteList)
                {
                    var anteArray = ante.Split('_');
                    if (order.AnteCodeList.Where(p => p.MatchId.Contains(anteArray[0])).Count() <= 0)
                        throw new Exception("优化投注拆票有错！");
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
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", result.TicketList.Sum(t => t.TotalMoney), order.TotalMoney));
            }
            return result;
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

        public TOrder AnalyzeOrder_Sport_Prize<TOrder, TTicket, TAnteCode>(GatewayTicketOrder_Sport order, string gateway, string agentId)
            where TOrder : I_Sport_Order<TTicket, TAnteCode>, new()
            where TTicket : I_Sport_Ticket<TAnteCode>, new()
            where TAnteCode : I_Sport_AnteCode, new()
        {
            var maxAmount = GetMaxTicketAmount(order.GameCode);
            var result = new TOrder
            {
                AgentId = agentId,
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
                            foreach (var count in countList)
                            {
                                var analyzer = AnalyzerFactory.GetSportAnalyzer(order.GameCode, order.GameType, count);
                                //M串1
                                c.Calculate(match, count, (arr) =>
                                {
                                    index = AddToTicketList<TOrder, TTicket, TAnteCode>(order, gateway, agentId, maxAmount, result, index, danList, count, analyzer, arr);
                                });
                            }
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
                                    index = AddToTicketList<TOrder, TTicket, TAnteCode>(order, gateway, agentId, maxAmount, result, index, danList, count, analyzer, tuoArr);
                                }

                                #endregion
                            });
                        });
                    }
                }
            }
            if (!result.TicketList.Sum(t => t.TotalMoney).Equals(order.TotalMoney))
            {
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", result.TicketList.Sum(t => t.TotalMoney), order.TotalMoney));
            }
            return result;
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

        //根据playType 1_1判断是单关  区DS下面
        public string GetOddsToMatchId(string locBetContent, string gameCode, string gameType)
        {
            var strList = new List<string>();
            var matchL = locBetContent.Split('/');
            foreach (var content in matchL)
            {
                var contentL = content.Split('_');
                var type = gameType.ToUpper() == "HH" ? contentL[0] : gameType;
                var matchId = gameType.ToUpper() == "HH" ? contentL[1] : contentL[0];
                if (gameCode.ToUpper() == "JCZQ")
                {
                    var oddManager = new JCZQ_OddsManager();
                    switch (type.ToUpper())
                    {
                        case "SPF":
                            var entity_SPF = oddManager.GetLastOdds<JCZQ_Odds_SPF>(type.ToUpper(), matchId, false);
                            if (entity_SPF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_SPF.GetOddsString()));
                            break;
                        case "BRQSPF":
                            var entity_BRQSPF = oddManager.GetLastOdds<JCZQ_Odds_BRQSPF>(type.ToUpper(), matchId, false);
                            if (entity_BRQSPF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_BRQSPF.GetOddsString()));
                            break;
                        case "BF":
                            var entity_BF = oddManager.GetLastOdds<JCZQ_Odds_BF>(type.ToUpper(), matchId, false);
                            if (entity_BF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_BF.GetOddsString()));
                            break;
                        case "ZJQ":
                            var entity_ZJQ = oddManager.GetLastOdds<JCZQ_Odds_ZJQ>(type.ToUpper(), matchId, false);
                            if (entity_ZJQ == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_ZJQ.GetOddsString()));
                            break;
                        case "BQC":
                            var entity_BQC = oddManager.GetLastOdds<JCZQ_Odds_BQC>(type.ToUpper(), matchId, false);
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
                    switch (type.ToUpper())
                    {
                        case "SF":
                            var entity_SF = oddManager.GetLastOdds<JCLQ_Odds_SF>(type.ToUpper(), matchId, false);
                            if (entity_SF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_SF.GetOddsString()));
                            break;
                        case "RFSF":
                            var entity_RFSF = oddManager.GetLastOdds<JCLQ_Odds_RFSF>(type.ToUpper(), matchId, false);
                            if (entity_RFSF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_RFSF.GetOddsString()));
                            break;
                        case "SFC":
                            var entity_SFC = oddManager.GetLastOdds<JCLQ_Odds_SFC>(type.ToUpper(), matchId, false);
                            if (entity_SFC == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_SFC.GetOddsString()));
                            break;
                        case "DXF":
                            var entity_DXF = oddManager.GetLastOdds<JCLQ_Odds_DXF>(type.ToUpper(), matchId, false);
                            if (entity_DXF == null)
                                throw new Exception("订单中有比赛未开出赔率" + matchId);
                            strList.Add(string.Format("{0}_{1}", matchId, entity_DXF.GetOddsString()));
                            break;
                        default:
                            break;
                    }
                }
            }
            return string.Join("/", strList);
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
                        var entity_SPF = oddManager.GetLastOdds<JCZQ_Odds_SPF>(gameType.ToUpper(), matchId, false);
                        if (entity_SPF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SPF.GetOddsString()));
                        break;
                    case "BRQSPF":
                        var entity_BRQSPF = oddManager.GetLastOdds<JCZQ_Odds_BRQSPF>(gameType.ToUpper(), matchId, false);
                        if (entity_BRQSPF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BRQSPF.GetOddsString()));
                        break;
                    case "BF":
                        var entity_BF = oddManager.GetLastOdds<JCZQ_Odds_BF>(gameType.ToUpper(), matchId, false);
                        if (entity_BF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BF.GetOddsString()));
                        break;
                    case "ZJQ":
                        var entity_ZJQ = oddManager.GetLastOdds<JCZQ_Odds_ZJQ>(gameType.ToUpper(), matchId, false);
                        if (entity_ZJQ == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_ZJQ.GetOddsString()));
                        break;
                    case "BQC":
                        var entity_BQC = oddManager.GetLastOdds<JCZQ_Odds_BQC>(gameType.ToUpper(), matchId, false);
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
                        var entity_SF = oddManager.GetLastOdds<JCLQ_Odds_SF>(gameType.ToUpper(), matchId, false);
                        if (entity_SF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SF.GetOddsString()));
                        break;
                    case "RFSF":
                        var entity_RFSF = oddManager.GetLastOdds<JCLQ_Odds_RFSF>(gameType.ToUpper(), matchId, false);
                        if (entity_RFSF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_RFSF.GetOddsString()));
                        break;
                    case "SFC":
                        var entity_SFC = oddManager.GetLastOdds<JCLQ_Odds_SFC>(gameType.ToUpper(), matchId, false);
                        if (entity_SFC == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SFC.GetOddsString()));
                        break;
                    case "DXF":
                        var entity_DXF = oddManager.GetLastOdds<JCLQ_Odds_DXF>(gameType.ToUpper(), matchId, false);
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

        #endregion

        #region 数字彩、传统足球投注拆票

        /// <summary>
        /// 拆票，数字彩、传统足球
        /// </summary>
        public void RequestTicket(GatewayTicketOrder orderInfo, string keyLine, bool stopAfterBonus, SchemeType schemeType)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var gameType = string.Empty;
            var ticketList = AnalyzeOrder(orderInfo);
            var ticketTable = GetNewTicketTable();
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
                var info = new InnerOrderInfo
                {
                    OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                    BetType = ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
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
                var ticketTable = GetNewTicketTable();
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
                    var info = new InnerOrderInfo
                    {
                        OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                        BetType = ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
                        IssueNumber = orderInfo.IssuseNumber,
                        BetUnits = ticket.BetCount,
                        Multiple = ticket.Amount,
                        BetMoney = ticket.TicketMoney,
                        IsAppend = orderInfo.IsAppend ? 1 : 0,
                        BetContent = locBetContent,
                    };

                    //sql.Append(CreateInsertSql(orderInfo, ticket, info, locOdds));
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
                try
                {
                    //批量插入票表
                    manager.SqlBulkAddTable(ticketTable);
                }
                catch (Exception exp)
                {
                    this.writer.Write("SqlBulkAddTableError", orderInfo.OrderId, LogType.Information, "SqlBulkAddTable", exp.Message + "/r/n");
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
                this.writer.Write("RequestTicketError", orderInfo.OrderId, LogType.Information, "保存票数据报错", exp.Message + "/r/n" + sql.ToString());
            }
            //watch.Stop();
            //this.writer.Write("RequestTicket", orderInfo.OrderId, LogType.Information, "保存票数据计时", "用时 " + watch.Elapsed.TotalMilliseconds);
        }

        public TicketCollection AnalyzeOrder(GatewayTicketOrder order)
        {
            Common.Lottery.Objects.Order o = new Common.Lottery.Objects.Order()
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
            var ticketList = AnalyzeTickets(o);
            ticketList.AnalyzeOrder(order.GameCode, order.Price);
            if (ticketList.TotalMoney != order.TotalMoney)
            {
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", ticketList.TotalMoney, order.TotalMoney));
            }
            return ticketList;
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
            new Combination().Calculate(numberIndexList.ToArray(), numberIndexList.Count - 9, (p) =>
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

        private TicketCollection AnalyzeTickets(Common.Lottery.Objects.Order order)
        {
            // 将所有号码，按照玩法进行分组
            var groupAntecodes = order.AntecodeList.GroupBy((item) => item.GameType);
            var ticketList = new TicketCollection();
            foreach (var group in groupAntecodes)
            {
                var gameType = group.Key;
                // 获取玩法一张票最多可以携带的号码数量
                var maxCount = GetMaxAntecodeCountEachTicket(order.GameCode, gameType);
                // 解析此玩法所有号码，并返回多张票
                var innerTicketList = GetTicketsByAntecodes(group.ToArray(), maxCount, GetMaxTicketAmount(order.GameCode), () => new Ticket() { GameType = gameType, Amount = order.Amount, });
                ticketList.AddRange(innerTicketList);
            }
            return ticketList;
        }

        private int GetMaxAntecodeCountEachTicket(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    return 1;
                case "ssq":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return 5;
                        default:
                            return 1;
                    }
                case "dlt":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return 5;
                        default:
                            return 1;
                    }
                default:
                    return 1;
            }
        }

        private IEnumerable<Ticket> GetTicketsByAntecodes(IEnumerable<Antecode> antecodeList, int maxCount, int maxAmount, Func<Ticket> createTicketHandler)
        {
            var ticketList = new List<Ticket>();
            var tmpTicket = createTicketHandler();
            var tmpAmount = tmpTicket.Amount;
            while (tmpAmount > 0)
            {
                var currentAmount = maxAmount;
                if (tmpAmount <= maxAmount)
                {
                    currentAmount = tmpAmount;
                }
                tmpAmount -= maxAmount;
                tmpTicket.Amount = currentAmount;
                foreach (var antecode in antecodeList)
                {
                    // 添加号码到票
                    tmpTicket.AnteCodeList.Add(antecode);
                    // 如果票包含的号码数量达到上限，则将票添加到列表，并重建票对象
                    if (tmpTicket.AnteCodeList.Count >= maxCount)
                    {
                        ticketList.Add(tmpTicket);
                        tmpTicket = createTicketHandler();
                        tmpTicket.Amount = currentAmount;
                    }
                }
                if (tmpTicket.AnteCodeList.Count > 0)
                {
                    ticketList.Add(tmpTicket);
                    tmpTicket = createTicketHandler();
                    tmpTicket.Amount = currentAmount;
                }
            }
            return ticketList.ToArray();
        }

        private string ConvertGameCode(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            return "JCJQS";
                        case "hh":
                            return "JCZQFH";
                        default:
                            return "JC" + gameType;
                    }
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sfc":
                            return "JCFC";
                        case "hh":
                            return "JCLQFH";
                        default:
                            return "JC" + gameType;
                    }
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            return "JQS";
                        default:
                            return gameType;
                    }
                case "ctzq":
                    switch (gameType.ToLower())
                    {
                        case "t14c":
                            return "14CSF";
                        case "tr9":
                            return "SFR9";
                        case "t6bqc":
                            return "6CBQ";
                        case "t4cjq":
                            return "4CJQ";
                        default:
                            return "";
                    }
                case "fc3d":
                    return "3D";
                case "cqssc":
                    return "ZQSSC";
                default:
                    return gameCode;
            }
        }

        public string ConvertBetType(string gameCode, string betType)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                case "jclq":
                case "bjdc":
                    if (!betType.StartsWith("P"))
                    {
                        return "P" + betType;
                    }
                    return betType;
                default:
                    return betType;
            }
        }

        public string ConvertGameType(string gameCode, string gameType, string betType, int betCount)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    return betType;
                //case "dlt":
                //    return betCount > 1 ? "FS" : "DS";
                //switch (gameType.ToLower())
                //{
                //    case "dt":
                //        return "FS";
                //    default:
                //        return gameType;
                //}
                case "cqssc":
                    switch (gameType.ToLower())
                    {
                        case "1xdx":
                            return "ZQSSC_1X_DS";
                        case "2xdx":
                            return string.Format("ZQSSC_2X_{0}", betType == "DS" ? "DS" : "FS");
                        case "3xdx":
                            return string.Format("ZQSSC_3X_{0}", betType == "DS" ? "DS" : "FS");
                        case "5xdx":
                            return string.Format("ZQSSC_5X_{0}", betType == "DS" ? "DS" : "FS");
                        case "2xzxfs":
                            return string.Format("ZQSSC_2XZX{0}", betType == "DS" ? "_DS" : "ZH");
                        case "2xhz":
                            return "ZQSSC_2XHZ";
                        case "2xzxfw":
                            return "ZQSSC_2XZXFZ";
                        case "2xbaodan":
                            return "ZQSSC_2XZX_BD";
                        case "3xzxzh":
                            if (betCount == 6)
                            {
                                return string.Format("ZQSSC_3XZH");
                            }
                            else
                            {
                                return string.Format("ZQSSC_3XZH_FS");
                            }
                        case "3xbaodan":
                            return "ZQSSC_3XZX_BD";
                        case "3xhz":
                            return "ZQSSC_3XHZ";
                        case "zx3ds":
                            return "ZQSSC_3XZ3_DS";
                        case "zx3fs":
                            return "ZQSSC_3XZ3_FS";
                        case "3xzxhz":
                            return "ZQSSC_3XZXHZ";
                        case "5xtx":
                            return "ZQSSC_5XTX";
                        case "dxds":
                            return "ZQSSC_DXDS";
                        case "zx6":
                            return string.Format("ZQSSC_3XZ6_{0}", betType == "DS" ? "DS" : "FS");
                        default:
                            return gameType;
                    }
                case "jx11x5":
                    switch (gameType.ToLower())
                    {
                        case "rx1":
                            return "11_RX1";
                        case "rx2":
                            return "11_RX2";
                        case "rx3":
                            return "11_RX3";
                        case "rx4":
                            return "11_RX4";
                        case "rx5":
                            return "11_RX5";
                        case "rx6":
                            return "11_RX6";
                        case "rx7":
                            return "11_RX7";
                        case "rx8":
                            return "11_RX8";
                        case "q2zhix":
                            return string.Format("11_ZXQ2_{0}", betType == "DS" ? "D" : "F");
                        case "q3zhix":
                            return string.Format("11_ZXQ3_{0}", betType == "DS" ? "D" : "F");
                        case "q2zux":
                            return "11_ZXQ2";
                        case "q3zux":
                            return "11_ZXQ3";
                        default:
                            return gameType;
                    }
                case "fc3d":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "fs":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "hz":
                            return "ZXHZ";
                        case "zx3ds":
                            return "ZX_DS";
                        case "zx3fs":
                            return "Z3FS";
                        case "zx6":
                            return string.Format("Z{0}", betType == "DS" ? "X_DS" : "6FS"); ;

                        default:
                            return gameType;
                    }
                case "pl3":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "fs":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "hz":
                            return "ZXHZ";
                        case "zx3ds":
                            return "ZX_DS";
                        case "zx3fs":
                            return "ZXZ3";
                        case "zx6":
                            if (betType == "DS")
                            {
                                return "ZX_DS";
                            }
                            else
                            {
                                return "ZXZ6";
                            }
                        default:
                            return gameType;
                    }
                default:
                    return gameType;
            }
        }

        #endregion

        #region 单式上传

        /// <summary>
        /// 拆票，单式上传
        /// </summary>
        public void RequestTicketByGateway_SingleScheme_New(GatewayTicketOrder_SingleScheme order)
        {
            List<string> codeList = new List<string>();
            var admin = new TicketGatewayAdmin();
            var matchIdList = new List<string>();
            var matchIdOddsList = new Dictionary<string, string>();

            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();

                #region 保存订单、票数据

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

                if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                {
                    foreach (var item in matchIdList)
                    {
                        matchIdOddsList.Add(string.Format("{0}_{1}", order.GameType.ToUpper(), item), GetOddsToMatchId_New(item, order.GameCode.ToUpper(), order.GameType.ToUpper()));
                    }
                }

                #endregion

                tran.CommitTran();
            }

            //拆票
            RequestTicket_SingleSport(order, codeList, order.Amount, matchIdOddsList, matchIdList.Distinct().ToList());

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
                var ticketTable = GetNewTicketTable();
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
                    var maxAmount = GetMaxTicketAmount(order.GameCode);
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
                Common.Log.LogWriterGetter.GetLogWriter().Write("GatewayHandler_Shop", "RequestTicket_SingleSport", Common.Log.LogType.Information, "test", ex.ToString());
                throw new Exception(ex.Message);
            }
        }

        #endregion

        /// <summary>
        ///  比赛数据添加、更新
        /// </summary>
        public void UpdateLocalData(string text, string param, NoticeType noticeType, string innerKey)
        {
            switch (noticeType)
            {
                #region 北京单场
                case NoticeType.BJDC_Issuse:
                    new IssuseBusiness().Update_BJDC_IssuseList();
                    break;
                case NoticeType.BJDC_Match:
                    var matchParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var matchIdArray = matchParamArray[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_BJDC_MatchList(matchParamArray[0], matchIdArray);
                    break;
                case NoticeType.BJDC_Match_SFGG:
                    var SFGGParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var SFGGdMatchIdArray = SFGGParamArray[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_SFGG_MatchList(SFGGParamArray[0], SFGGdMatchIdArray);
                    break;
                case NoticeType.BJDC_MatchResult:
                    var matchResultParamArray = text.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var matchResultIdArray = matchResultParamArray[1].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var bjdzBiz = new IssuseBusiness();
                    var str_array = from s in matchResultIdArray select s.Split('_')[0];

                    bjdzBiz.Update_BJDC_MatchResultList(matchResultParamArray[0], str_array.ToArray());
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
                    var SFGGbjdzBiz = new IssuseBusiness();
                    var SFGGstr_array = from s in SFGGmatchResultIdArray select s.Split('_')[0];

                    SFGGbjdzBiz.Update_SFGG_MatchResultList(SFGGmatchResultParamArray[0], SFGGstr_array.ToArray());
                    break;
                #endregion

                #region 传统足球
                case NoticeType.CTZQ_Issuse:
                    var p = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    var ctzq_GameCode = p[0].Split('|')[1];
                    new IssuseBusiness().Update_CTZQ_GameIssuse(ctzq_GameCode, p);
                    break;
                case NoticeType.CTZQ_Match:
                    var ctzqMatchArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = ctzqMatchArray[0].Split('|');
                    var ctzq_gameCode = arr[1];
                    var issuseNumber = arr[2];
                    new IssuseBusiness().Update_CTZQ_MatchList(ctzq_gameCode, issuseNumber, ctzqMatchArray);
                    break;
                case NoticeType.CTZQ_MatchPool:
                    int totalBonusCount;
                    var tmpCTZQ_MatchPool = param.Split('^');
                    var winNumber = new TicketGatewayAdmin().UpdateBonusPool_CTZQ("CTZQ", tmpCTZQ_MatchPool[0], tmpCTZQ_MatchPool[1], out totalBonusCount);
                    if (new TicketGatewayAdmin().CanPrize_CTZQ(tmpCTZQ_MatchPool[0], totalBonusCount, winNumber))
                    {
                        QueryUnPrizeTicketAndDoPrizeByGameCode("CTZQ", tmpCTZQ_MatchPool[0], -1);
                        LotteryIssusePrize("CTZQ", tmpCTZQ_MatchPool[0], tmpCTZQ_MatchPool[1], winNumber);
                    }
                    break;
                #endregion

                #region 竞彩足球
                case NoticeType.JCZQ_Match:
                    var jczq_array = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCZQ_MatchList(jczq_array);
                    break;
                case NoticeType.JCZQ_MatchResult:
                    var jczq_match_result_array = text.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var str_Array = from s in jczq_match_result_array select s.Split('_')[0];
                    var jczq_biz = new IssuseBusiness();
                    jczq_biz.Update_JCZQ_MatchResultList(str_Array.ToArray());
                    new Thread(() =>
                    {
                        Common.Utilities.UsefullHelper.TryDoAction(() =>
                        {
                            jczq_biz.Update_JCZQ_HitCount(str_Array.ToArray());
                        });
                    }).Start();
                    break;
                //过关
                case NoticeType.JCZQ_SPF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_SPF_SPInfo, JCZQ_Odds_SPF>("JCZQ", "SPF", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_BRQSPF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_BRQSPF_SPInfo, JCZQ_Odds_BRQSPF>("JCZQ", "BRQSPF", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_ZJQ_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_ZJQ_SPInfo, JCZQ_Odds_ZJQ>("JCZQ", "ZJQ", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_BQC_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_BQC_SPInfo, JCZQ_Odds_BQC>("JCZQ", "BQC", text.Split('_'), false);
                    break;
                case NoticeType.JCZQ_BF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCZQ<JCZQ_BF_SPInfo, JCZQ_Odds_BF>("JCZQ", "BF", text.Split('_'), false);
                    break;
                #endregion

                #region 竞彩篮球
                case NoticeType.JCLQ_Match:
                    var jclq_array = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCLQ_MatchList(jclq_array);
                    break;
                case NoticeType.JCLQ_MatchResult:
                    var jclq_match_result_array = text.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    var jclq_result_array = from s in jclq_match_result_array select s.Split('_')[0];
                    var jclqBiz = new IssuseBusiness();
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
                    new TicketGatewayAdmin().UpdateOddsList_JCLQ<JCLQ_SF_SPInfo, JCLQ_Odds_SF>("JCLQ", "SF", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_RFSF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCLQ<JCLQ_RFSF_SPInfo, JCLQ_Odds_RFSF>("JCLQ", "RFSF", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_SFC_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCLQ<JCLQ_SFC_SPInfo, JCLQ_Odds_SFC>("JCLQ", "SFC", text.Split('_'), false);
                    break;
                case NoticeType.JCLQ_DXF_SP:
                    new TicketGatewayAdmin().UpdateOddsList_JCLQ<JCLQ_DXF_SPInfo, JCLQ_Odds_DXF>("JCLQ", "DXF", text.Split('_'), false);
                    break;
                #endregion

                #region 数字彩奖池：双色球、大乐透
                case NoticeType.SZC_MatchPool:
                    var tmpSZC_MatchPool = text.Split('_');
                    new TicketGatewayAdmin().UpdateBonusPool_SZC(tmpSZC_MatchPool[0], tmpSZC_MatchPool[1]);
                    break;
                #endregion

                #region 欧洲杯

                case NoticeType.JCOZB_GJ:
                    var gjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCZQ_OZB_GJ(gjMatchIdArray);
                    break;
                case NoticeType.JCOZB_GYJ:
                    var gyjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCZQ_OZB_GYJ(gyjMatchIdArray);
                    break;

                #endregion

                #region 世界杯
                case NoticeType.JCSJB_GJ:
                    var sjbgjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCZQ_SJB_GJ(sjbgjMatchIdArray);
                    break;
                case NoticeType.JCSJB_GYJ:
                    var sjbgyjMatchIdArray = text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    new IssuseBusiness().Update_JCZQ_SJB_GYJ(sjbgyjMatchIdArray);
                    break;
                #endregion

                default:
                    break;
            }

        }

        /// <summary>
        /// 根据彩种更新比赛为取消
        /// </summary>
        public void DoMatchCancel(string gameCode, string matchId, string issuse)
        {
            switch (gameCode.ToUpper())
            {
                case "JCZQ":
                    #region jczq
                    var manager_JCZQ = new JCZQMatchManager();
                    var old_JCZQ = manager_JCZQ.QueryJCZQ_MatchResultByMatchId(matchId);
                    var oldResult_JCZQ = manager_JCZQ.QueryJCZQ_MatchResult_PrizeByMatchId(matchId);
                    if (old_JCZQ == null)
                    {
                        manager_JCZQ.AddJCZQ_MatchResult(new JCZQ_MatchResult
                        {
                            HalfGuestTeamScore = -1,
                            HalfHomeTeamScore = -1,
                            FullGuestTeamScore = -1,
                            FullHomeTeamScore = -1,
                            MatchState = "2",
                            SPF_Result = "-1",
                            SPF_SP = 1M,
                            BRQSPF_Result = "-1",
                            BRQSPF_SP = 1M,
                            BF_Result = "-1",
                            BF_SP = 1M,
                            ZJQ_Result = "-1",
                            ZJQ_SP = 1M,
                            BQC_Result = "-1",
                            BQC_SP = 1M,
                            CreateTime = DateTime.Now,
                            MatchData = matchId.Substring(0, 6),
                            MatchId = matchId,
                            MatchNumber = matchId.Substring(6),
                        });
                    }
                    else
                    {
                        old_JCZQ.HalfGuestTeamScore = -1;
                        old_JCZQ.HalfHomeTeamScore = -1;
                        old_JCZQ.FullGuestTeamScore = -1;
                        old_JCZQ.FullHomeTeamScore = -1;
                        old_JCZQ.MatchState = "2";
                        old_JCZQ.SPF_Result = "-1";
                        old_JCZQ.SPF_SP = 1M;
                        old_JCZQ.BRQSPF_Result = "-1";
                        old_JCZQ.BRQSPF_SP = 1M;
                        old_JCZQ.BF_Result = "-1";
                        old_JCZQ.BF_SP = 1M;
                        old_JCZQ.ZJQ_Result = "-1";
                        old_JCZQ.ZJQ_SP = 1M;
                        old_JCZQ.BQC_Result = "-1";
                        old_JCZQ.BQC_SP = 1M;
                        manager_JCZQ.UpdateJCZQ_MatchResult(old_JCZQ);
                    }

                    if (oldResult_JCZQ == null)
                    {
                        manager_JCZQ.AddJCZQ_MatchResult_Prize(new JCZQ_MatchResult_Prize
                        {
                            HalfGuestTeamScore = -1,
                            HalfHomeTeamScore = -1,
                            FullGuestTeamScore = -1,
                            FullHomeTeamScore = -1,
                            MatchState = "2",
                            SPF_Result = "-1",
                            SPF_SP = 1M,
                            BRQSPF_Result = "-1",
                            BRQSPF_SP = 1M,
                            BF_Result = "-1",
                            BF_SP = 1M,
                            ZJQ_Result = "-1",
                            ZJQ_SP = 1M,
                            BQC_Result = "-1",
                            BQC_SP = 1M,
                            CreateTime = DateTime.Now,
                            MatchData = matchId.Substring(0, 6),
                            MatchId = matchId,
                            MatchNumber = matchId.Substring(6)
                        });
                    }
                    else
                    {
                        oldResult_JCZQ.HalfGuestTeamScore = -1;
                        oldResult_JCZQ.HalfHomeTeamScore = -1;
                        oldResult_JCZQ.FullGuestTeamScore = -1;
                        oldResult_JCZQ.FullHomeTeamScore = -1;
                        oldResult_JCZQ.MatchState = "2";
                        oldResult_JCZQ.SPF_Result = "-1";
                        oldResult_JCZQ.SPF_SP = 1M;
                        oldResult_JCZQ.BRQSPF_Result = "-1";
                        oldResult_JCZQ.BRQSPF_SP = 1M;
                        oldResult_JCZQ.BF_Result = "-1";
                        oldResult_JCZQ.BF_SP = 1M;
                        oldResult_JCZQ.ZJQ_Result = "-1";
                        oldResult_JCZQ.ZJQ_SP = 1M;
                        oldResult_JCZQ.BQC_Result = "-1";
                        oldResult_JCZQ.BQC_SP = 1M;
                        manager_JCZQ.UpdateJCZQ_MatchResult_Prize(oldResult_JCZQ);
                    }
                    #endregion
                    break;
                case "JCLQ":
                    #region jclq
                    var manager_JCLQ = new JCLQMatchManager();
                    var old_JCLQ = manager_JCLQ.QueryJCLQ_MatchResultByMatchId(matchId);
                    var oldResult_JCLQ = manager_JCLQ.QueryJCLQ_MatchResult_PrizeByMatchId(matchId);
                    if (old_JCLQ == null)
                    {
                        manager_JCLQ.AddJCLQ_MatchResult(new JCLQ_MatchResult
                        {
                            HomeScore = -1,
                            GuestScore = -1,
                            MatchState = "2",
                            RFSF_Trend = "1",
                            DXF_Trend = "1",
                            SF_Result = "-1",
                            SF_SP = 1M,
                            RFSF_Result = "-1",
                            RFSF_SP = 1M,
                            SFC_Result = "-1",
                            SFC_SP = 1M,
                            DXF_Result = "-1",
                            DXF_SP = 1M,
                            CreateTime = DateTime.Now,
                            MatchData = matchId.Substring(0, 6),
                            MatchId = matchId,
                            MatchNumber = matchId.Substring(6),
                        });
                    }
                    else
                    {
                        old_JCLQ.HomeScore = -1;
                        old_JCLQ.GuestScore = -1;
                        old_JCLQ.MatchState = "2";
                        old_JCLQ.RFSF_Trend = "1";
                        old_JCLQ.DXF_Trend = "1";
                        old_JCLQ.SF_Result = "-1";
                        old_JCLQ.SF_SP = 1M;
                        old_JCLQ.RFSF_Result = "-1";
                        old_JCLQ.RFSF_SP = 1M;
                        old_JCLQ.SFC_Result = "-1";
                        old_JCLQ.SFC_SP = 1M;
                        old_JCLQ.DXF_Result = "-1";
                        old_JCLQ.DXF_SP = 1M;
                        manager_JCLQ.UpdateJCLQ_MatchResult(old_JCLQ);
                    }

                    if (oldResult_JCLQ == null)
                    {
                        manager_JCLQ.AddJCLQ_MatchResult_Prize(new JCLQ_MatchResult_Prize
                        {

                            HomeScore = -1,
                            GuestScore = -1,
                            MatchState = "2",
                            RFSF_Trend = "1",
                            DXF_Trend = "1",
                            SF_Result = "-1",
                            SF_SP = 1M,
                            RFSF_Result = "-1",
                            RFSF_SP = 1M,
                            SFC_Result = "-1",
                            SFC_SP = 1M,
                            DXF_Result = "-1",
                            DXF_SP = 1M,
                            CreateTime = DateTime.Now,
                            MatchData = matchId.Substring(0, 6),
                            MatchId = matchId,
                            MatchNumber = matchId.Substring(6),
                        });
                    }
                    else
                    {
                        oldResult_JCLQ.HomeScore = -1;
                        oldResult_JCLQ.GuestScore = -1;
                        oldResult_JCLQ.MatchState = "2";
                        oldResult_JCLQ.RFSF_Trend = "1";
                        oldResult_JCLQ.DXF_Trend = "1";
                        oldResult_JCLQ.SF_Result = "-1";
                        oldResult_JCLQ.SF_SP = 1M;
                        oldResult_JCLQ.RFSF_Result = "-1";
                        oldResult_JCLQ.RFSF_SP = 1M;
                        oldResult_JCLQ.SFC_Result = "-1";
                        oldResult_JCLQ.SFC_SP = 1M;
                        oldResult_JCLQ.DXF_Result = "-1";
                        oldResult_JCLQ.DXF_SP = 1M;
                        manager_JCLQ.UpdateJCLQ_MatchResult_Prize(oldResult_JCLQ);
                    }
                    #endregion
                    break;
                case "BJDC":
                    #region bjdc
                    var manager_BJDC = new BJDCMatchManager();
                    var old_BJDC = manager_BJDC.QueryBJDC_MatchResult(issuse, int.Parse(matchId));
                    var oldResult_BJDC = manager_BJDC.QueryBJDC_MatchResult_Prize(issuse, int.Parse(matchId));
                    if (old_BJDC == null)
                    {
                        manager_BJDC.AddBJDC_MatchResult(new BJDC_MatchResult
                        {
                            CreateTime = DateTime.Now,
                            Id = string.Format("{0}|{1}", issuse, matchId),
                            IssuseNumber = issuse,
                            BF_Result = "-1",
                            BF_SP = 1M,
                            BQC_Result = "-1",
                            BQC_SP = 1M,
                            SPF_Result = "-1",
                            SPF_SP = 1M,
                            SXDS_Result = "-1",
                            SXDS_SP = 1M,
                            ZJQ_Result = "-1",
                            ZJQ_SP = 1M,
                            GuestFull_Result = "-1",
                            GuestHalf_Result = "-1",
                            HomeFull_Result = "-1",
                            HomeHalf_Result = "-1",
                            MatchOrderId = int.Parse(matchId),
                            MatchState = "2",
                        });
                    }
                    else
                    {
                        old_BJDC.BF_Result = "-1";
                        old_BJDC.BF_SP = 1M;
                        old_BJDC.BQC_Result = "-1";
                        old_BJDC.BQC_SP = 1M;
                        old_BJDC.GuestFull_Result = "-1";
                        old_BJDC.GuestHalf_Result = "-1";
                        old_BJDC.HomeFull_Result = "-1";
                        old_BJDC.HomeHalf_Result = "-1";
                        old_BJDC.MatchState = "2";
                        old_BJDC.SPF_Result = "-1";
                        old_BJDC.SPF_SP = 1M;
                        old_BJDC.SXDS_Result = "-1";
                        old_BJDC.SXDS_SP = 1M;
                        old_BJDC.ZJQ_Result = "-1";
                        old_BJDC.ZJQ_SP = 1M;
                        manager_BJDC.UpdateBJDC_MatchResult(old_BJDC);
                    }

                    if (oldResult_BJDC == null)
                    {
                        manager_BJDC.AddBJDC_MatchResult_Prize(new BJDC_MatchResult_Prize
                        {
                            CreateTime = DateTime.Now,
                            Id = string.Format("{0}|{1}", issuse, matchId),
                            IssuseNumber = issuse,
                            BF_Result = "-1",
                            BF_SP = 1M,
                            BQC_Result = "-1",
                            BQC_SP = 1M,
                            SPF_Result = "-1",
                            SPF_SP = 1M,
                            SXDS_Result = "-1",
                            SXDS_SP = 1M,
                            ZJQ_Result = "-1",
                            ZJQ_SP = 1M,
                            GuestFull_Result = "-1",
                            GuestHalf_Result = "-1",
                            HomeFull_Result = "-1",
                            HomeHalf_Result = "-1",
                            MatchOrderId = int.Parse(matchId),
                            MatchState = "2",
                        });
                    }
                    else
                    {
                        oldResult_BJDC.BF_Result = "-1";
                        oldResult_BJDC.BF_SP = 1M;
                        oldResult_BJDC.BQC_Result = "-1";
                        oldResult_BJDC.BQC_SP = 1M;
                        oldResult_BJDC.GuestFull_Result = "-1";
                        oldResult_BJDC.GuestHalf_Result = "-1";
                        oldResult_BJDC.HomeFull_Result = "-1";
                        oldResult_BJDC.HomeHalf_Result = "-1";
                        oldResult_BJDC.MatchState = "2";
                        oldResult_BJDC.SPF_Result = "-1";
                        oldResult_BJDC.SPF_SP = 1M;
                        oldResult_BJDC.SXDS_Result = "-1";
                        oldResult_BJDC.SXDS_SP = 1M;
                        oldResult_BJDC.ZJQ_Result = "-1";
                        oldResult_BJDC.ZJQ_SP = 1M;
                        manager_BJDC.UpdateBJDC_MatchResult_Prize(oldResult_BJDC);
                    }
                    #endregion
                    break;
                default:
                    throw new Exception("传入彩种异常 - " + gameCode);
            }
        }

        #region 首页队伍信息

        public IndexMatch_Collection AddIndexMatch(string indexMatchList)
        {
            var collection = new IndexMatch_Collection();

            var matchIdList = JsonSerializer.Deserialize<List<IndexMatchInfo>>(indexMatchList);
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

        public IndexMatchInfo QueryIndexMatchInfo(int id)
        {
            using (var manager = new Sports_Manager())
            {
                IndexMatchInfo info = new IndexMatchInfo();
                var entiy = manager.QueryIndexMatchById(id);
                if (entiy != null)
                {
                    info.Id = entiy.Id;
                    info.ImgPath = entiy.ImgPath;
                    info.MatchId = entiy.MatchId;
                    info.MatchName = entiy.MatchName;
                    info.CreateTime = entiy.CreateTime;
                }
                return info;
            }
        }
        public void UpdateIndexMatch(int Id, string imgPath)
        {
            using (var manager = new Sports_Manager())
            {
                if (Id <= 0)
                    throw new Exception("编号不能为空");
                var entiy = manager.QueryIndexMatchById(Id);
                if (entiy == null) throw new Exception("未查询到相关比赛信息");
                entiy.ImgPath = imgPath;
                manager.UpdateIndexMatch(entiy);
            }
        }
        public IndexMatch_Collection QueryIndexMatchCollection(string matchId, string hasImg, int pageIndex, int pageSize)
        {
            using (var manager = new Sports_Manager())
            {
                return manager.QueryIndexMatchCollection(matchId, hasImg, pageIndex, pageSize);
            }
        }

        #endregion
    }
}