using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Lottery;
using System.IO;
using Common.JSON;
using Common.Algorithms;
using GameBiz.Core.Ticket;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Business.Domain.Managers.Ticket;
using GameBiz.Domain.Managers;
using GameBiz.Core;
using GameBiz.Domain.Entities;

namespace GameBiz.Business
{
    public partial class TicketGatewayAdmin
    {
        //       public void AddMatchList_JCLQ(string gameCode, string[] matchIdList)
        //       {
        //           var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //           //var addList = matchInfoList.Where(m => matchIdList.Contains(m.MatchId));
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               foreach (var info in matchInfoList)
        //               {
        //                   var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                   if (entity != null)
        //                   {
        //                       continue;
        //                   }
        //                   entity = new JCLQ_Match
        //                   {
        //                       CompositeId = gameCode + "|" + info.MatchId,
        //                       GameCode = gameCode,
        //                       MatchId = info.MatchId,
        //                       MatchIdName = info.MatchIdName,
        //                       MatchNumber = info.MatchNumber,
        //                       MatchDate = info.MatchData,
        //                       DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime),
        //                       FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime),

        //                       CreateTime = DateTime.Parse(info.CreateTime),
        //                       UpdateTime = DateTime.Now,
        //                       OpenTime = null,

        //                       LeagueId = info.LeagueId.ToString(),
        //                       LeagueName = info.LeagueName,

        //                       HomeTeamId = info.HomeTeamId.ToString(),
        //                       HomeTeamName = info.HomeTeamName,
        //                       HomeTeamRankName = "",
        //                       GuestTeamId = info.GuestTeamId.ToString(),
        //                       GuestTeamName = info.GuestTeamName,
        //                       GuestTeamRankName = "",
        //                       LetBall = info.LetBall,

        //                       MatchStartTime = DateTime.Parse(info.StartDateTime),
        //                       MatchState = MatchState.Waiting,
        //                       HomeTeamHalfScore = "-",
        //                       HomeTeamScore = "-",
        //                       GuestTeamHalfScore = "-",
        //                       GuestTeamScore = "-",

        //                       SF_Result = "-",
        //                       RFSF_Result = "-",
        //                       SFC_Result = "-",
        //                       DXF_Result = "-",
        //                   };
        //                   matchManager.AddJCLQ_Match(entity);
        //               }
        //               tran.CommitTran();
        //           }
        //       }
        //       public void AddMatchList_JCLQ_Service(string gameCode)
        //       {
        //           var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               foreach (var info in matchInfoList)
        //               {
        //                   var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                   if (entity != null)
        //                   {
        //                       continue;
        //                   }
        //                   entity = new JCLQ_Match
        //                   {
        //                       CompositeId = gameCode + "|" + info.MatchId,
        //                       GameCode = gameCode,
        //                       MatchId = info.MatchId,
        //                       MatchIdName = info.MatchIdName,
        //                       MatchNumber = info.MatchNumber,
        //                       MatchDate = info.MatchData,
        //                       DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime),
        //                       FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime),

        //                       CreateTime = DateTime.Parse(info.CreateTime),
        //                       UpdateTime = DateTime.Now,
        //                       OpenTime = null,

        //                       LeagueId = info.LeagueId.ToString(),
        //                       LeagueName = info.LeagueName,

        //                       HomeTeamId = info.HomeTeamId.ToString(),
        //                       HomeTeamName = info.HomeTeamName,
        //                       HomeTeamRankName = "",
        //                       GuestTeamId = info.GuestTeamId.ToString(),
        //                       GuestTeamName = info.GuestTeamName,
        //                       GuestTeamRankName = "",
        //                       LetBall = info.LetBall,

        //                       MatchStartTime = DateTime.Parse(info.StartDateTime),
        //                       MatchState = MatchState.Waiting,
        //                       HomeTeamHalfScore = "-",
        //                       HomeTeamScore = "-",
        //                       GuestTeamHalfScore = "-",
        //                       GuestTeamScore = "-",

        //                       SF_Result = "-",
        //                       RFSF_Result = "-",
        //                       SFC_Result = "-",
        //                       DXF_Result = "-",
        //                   };
        //                   matchManager.AddJCLQ_Match(entity);
        //               }
        //               tran.CommitTran();
        //           }
        //       }
        //       public void UpdateMatchList_JCLQ(string gameCode, string[] matchIdList)
        //       {
        //           var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //           var addList = matchInfoList.Where(m => matchIdList.Contains(m.MatchId));
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               foreach (var info in addList)
        //               {
        //                   var isAdd = false;
        //                   var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                   if (entity == null)
        //                   {
        //                       entity = new JCLQ_Match();
        //                       entity.OpenTime = null;
        //                       entity.MatchState = MatchState.Waiting;
        //                       entity.HomeTeamHalfScore = "-";
        //                       entity.HomeTeamScore = "-";
        //                       entity.GuestTeamHalfScore = "-";
        //                       entity.GuestTeamScore = "-";
        //                       entity.SF_Result = "-";
        //                       entity.RFSF_Result = "-";
        //                       entity.SFC_Result = "-";
        //                       entity.DXF_Result = "-";

        //                       isAdd = true;
        //                   }
        //                   entity.CompositeId = gameCode + "|" + info.MatchId;
        //                   entity.GameCode = gameCode;
        //                   entity.MatchId = info.MatchId;
        //                   entity.MatchIdName = info.MatchIdName;
        //                   entity.MatchNumber = info.MatchNumber;
        //                   entity.MatchDate = info.MatchData;
        //                   entity.DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime);
        //                   entity.FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime);

        //                   entity.CreateTime = DateTime.Parse(info.CreateTime);
        //                   entity.UpdateTime = DateTime.Now;

        //                   entity.LeagueId = info.LeagueId.ToString();
        //                   entity.LeagueName = info.LeagueName;

        //                   entity.HomeTeamId = info.HomeTeamId.ToString();
        //                   entity.HomeTeamName = info.HomeTeamName;
        //                   entity.HomeTeamRankName = "";
        //                   entity.GuestTeamId = info.GuestTeamId.ToString();
        //                   entity.GuestTeamName = info.GuestTeamName;
        //                   entity.GuestTeamRankName = "";
        //                   entity.LetBall = info.LetBall;

        //                   entity.MatchStartTime = DateTime.Parse(info.StartDateTime);

        //                   if (isAdd)
        //                   {
        //                       matchManager.AddJCLQ_Match(entity);
        //                   }
        //                   else
        //                   {
        //                       matchManager.UpdateMatch(entity);
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //       }

        //       public void UpdateOddsList_JCLQ_Service<TInfo, TEntity>(string gameCode, string gameType, bool isDS)
        //           where TInfo : JingCaiMatchBase, I_JingCai_Odds
        //           where TEntity : JingCai_Odds, new()
        //       {
        //           var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var oddsManager = new JCLQ_OddsManager();
        //               foreach (var odds in oddsList)
        //               {
        //                   var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, isDS);
        //                   if (entity == null)
        //                   {

        //                       entity = new TEntity
        //                       {
        //                           MatchId = odds.MatchId,
        //                           CreateTime = DateTime.Now,
        //                       };
        //                       entity.SetOdds(odds);
        //                       oddsManager.AddOdds<TEntity>(entity);
        //                   }
        //                   else if (!entity.Equals(odds))
        //                   {
        //                       entity.CreateTime = DateTime.Now;
        //                       entity.SetOdds(odds);
        //                       oddsManager.UpdateOdds<TEntity>(entity);
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //       }
        //       public void AutoUpdateOdds_JCLQ<TInfo, TEntity>(string gameCode, string gameType, bool isDS)
        //           where TInfo : JingCaiMatchBase, I_JingCai_Odds
        //           where TEntity : JingCai_Odds, new()
        //       {
        //           var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var oddsManager = new JCLQ_OddsManager();
        //               foreach (var odds in oddsList)
        //               {
        //                   if (!odds.CheckIsValidate())
        //                   {
        //                       continue;
        //                   }
        //                   var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, isDS);
        //                   if (entity == null || !entity.Equals(odds))
        //                   {
        //                       entity = new TEntity
        //                       {
        //                           MatchId = odds.MatchId,
        //                           CreateTime = DateTime.Now,
        //                       };
        //                       entity.SetOdds(odds);
        //                       oddsManager.AddOdds<TEntity>(entity);
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //       }

        //       public IList<JCLQ_MatchQueryInfo> QueryMatchList_JCLQToMatchId(string matchId)
        //       {
        //           var issuseManager = new JCLQ_MatchManager();
        //           return issuseManager.QueryMatchListToMatchId(matchId);
        //       }
        //       public IList<JCLQ_MatchQueryInfo> QueryWaitPrizeMatchList_JCLQ(string gameCode, string matchDate)
        //       {
        //           var issuseManager = new JCLQ_MatchManager();
        //           return issuseManager.QueryWaitPrizeMatchList(gameCode, matchDate);
        //       }
        //       public void UpdateMatchPrized_JCLQ(string fid, MatchState matchState, bool isPrized)
        //       {
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var issuseManager = new JCLQ_MatchManager();
        //               var f = fid.Split('_');
        //               var entity = issuseManager.GetMatch(f[1].ToString(), f[0].ToString());
        //               if (entity == null)
        //               {
        //                   throw new ArgumentException(string.Format("比赛{0}-{1}不存在", f[1].ToString(), f[0].ToString()));
        //               }
        //               entity.IsPrized = isPrized;
        //               entity.MatchState = matchState;
        //               issuseManager.UpdateMatch(entity);

        //               tran.CommitTran();
        //           }
        //       }
        //       public void UpdateMatchCancel_JCLQ(string fid)
        //       {
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var issuseManager = new JCLQ_MatchManager();
        //               var f = fid.Split('_');
        //               var entity = issuseManager.GetMatch(f[1].ToString(), f[0].ToString());
        //               if (entity == null)
        //               {
        //                   throw new ArgumentException(string.Format("比赛{0}-{1}不存在", f[1].ToString(), f[0].ToString()));
        //               }
        //               entity.HomeTeamHalfScore = "-1";
        //               entity.HomeTeamScore = "-1";
        //               entity.GuestTeamHalfScore = "-1";
        //               entity.GuestTeamScore = "-1";
        //               entity.DXF_Result = "-1";
        //               entity.RFSF_Result = "-1";
        //               entity.SF_Result = "-1";
        //               entity.SFC_Result = "-1";
        //               entity.DXF_SP = 1M;
        //               entity.RFSF_SP = 1M;
        //               entity.SF_SP = 1M;
        //               entity.SFC_SP = 1M;
        //               entity.MatchState = MatchState.Oddsed;
        //               issuseManager.UpdateMatch(entity);

        //               tran.CommitTran();
        //           }
        //       }
        //       public void UpdateMatchResultList_JCLQ(string gameCode, string[] matchIdList, out List<string> matchList)
        //       {
        //           matchList = new List<string>();
        //           var resultList = GetMatchResultList_JingCai(gameCode, "");
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               var updateList = resultList.Where(r => matchIdList.Contains(r.MatchId));
        //               foreach (var result in updateList)
        //               {
        //                   var match = matchManager.GetMatch(gameCode, result.MatchId);
        //                   if (match == null) { continue; }

        //                   if (result.MatchState.Equals("2"))
        //                   {
        //                       //match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                       match.HomeTeamScore = result.HomeScore.ToString();
        //                       //match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                       match.GuestTeamScore = result.GuestScore.ToString();
        //                       match.SF_Result = result.SF_Result;
        //                       match.RFSF_Result = result.RFSF_Result;
        //                       match.SFC_Result = result.SFC_Result;
        //                       match.DXF_Result = result.DXF_Result;
        //                       match.SF_SP = result.SF_SP;
        //                       match.RFSF_SP = result.RFSF_SP;
        //                       match.SFC_SP = result.SFC_SP;
        //                       match.DXF_SP = result.DXF_SP;
        //                       match.OpenTime = DateTime.Now;
        //                       if ((result.SF_SP > 1M || result.SF_SP == 0M) && (result.RFSF_SP > 1M || result.RFSF_SP == 0M) && (result.SFC_SP > 1M || result.SFC_SP == 0M) && (result.DXF_SP > 1M || result.DXF_SP == 0M))
        //                       {
        //                           match.MatchState = MatchState.Oddsed;
        //                           matchList.Add(match.MatchId);
        //                       }
        //                       else
        //                       {
        //                           match.MatchState = MatchState.Finish;
        //                       }
        //                       matchManager.UpdateMatch(match);
        //                   }
        //                   else if (result.MatchState.Equals("3"))
        //                   {
        //                       match.MatchState = MatchState.Oddsed;
        //                       //match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                       match.HomeTeamScore = result.HomeScore.ToString();
        //                       //match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                       match.GuestTeamScore = result.GuestScore.ToString();
        //                       match.SF_Result = result.SF_Result;
        //                       match.RFSF_Result = result.RFSF_Result;
        //                       match.SFC_Result = result.SFC_Result;
        //                       match.DXF_Result = result.DXF_Result;
        //                       match.SF_SP = result.SF_SP;
        //                       match.RFSF_SP = result.RFSF_SP;
        //                       match.SFC_SP = result.SFC_SP;
        //                       match.DXF_SP = result.DXF_SP;
        //                       match.OpenTime = DateTime.Now;
        //                       matchManager.UpdateMatch(match);
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //       }
        //       public OrderId_QueryInfoCollection QueryCanAwardOrderListByMatch_JCLQ(string matchId)
        //       {
        //           var orderManager = new JCLQ_OrderManager();
        //           return orderManager.QueryCanAwardOrderListByMatch(matchId);
        //       }
        //       public string QueryJCLQCanAwardOrderListByMatchId(string matchIdList)
        //       {
        //           var strList = new List<string>();
        //           var orderManager = new JCLQ_OrderManager();
        //           var array = matchIdList.Split(',');
        //           foreach (var matchId in array)
        //           {
        //               var orderList = orderManager.QueryCanAwardOrderListByMatch(matchId);
        //               foreach (var item in orderList.OrderList)
        //               {
        //                   if (!strList.Contains(item.OrderId))
        //                       strList.Add(item.OrderId);
        //               }
        //           }
        //           return string.Join(",", strList);
        //       }
        //       public IList<Order_QueryInfo> QueryRunningOrderListById_JCLQ(string orderIdList, out int totalCount, out decimal totalMoney)
        //       {
        //           var orderManager = new JCLQ_OrderManager();
        //           return orderManager.QueryRunningOrderListById_JCLQ(orderIdList, out totalCount, out totalMoney);
        //       }
        //       public void AutoUpdateMatch_JCLQ(string gameCode, string matchData)
        //       {
        //           var matchInfoList = GetMatchList_JingCai(gameCode, matchData);
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               foreach (var info in matchInfoList)
        //               {
        //                   var isAdd = false;
        //                   var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                   if (entity == null)
        //                   {
        //                       entity = new JCLQ_Match();
        //                       entity.OpenTime = null;
        //                       entity.MatchState = MatchState.Waiting;
        //                       entity.HomeTeamHalfScore = "-";
        //                       entity.HomeTeamScore = "-";
        //                       entity.GuestTeamHalfScore = "-";
        //                       entity.GuestTeamScore = "-";
        //                       entity.SF_Result = "-";
        //                       entity.RFSF_Result = "-";
        //                       entity.SFC_Result = "-";
        //                       entity.DXF_Result = "-";

        //                       isAdd = true;
        //                   }
        //                   entity.CompositeId = gameCode + "|" + info.MatchId;
        //                   entity.GameCode = gameCode;
        //                   entity.MatchId = info.MatchId;
        //                   entity.MatchIdName = info.MatchIdName;
        //                   entity.MatchNumber = info.MatchNumber;
        //                   entity.MatchDate = info.MatchData;
        //                   entity.DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime);
        //                   entity.FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime);

        //                   entity.CreateTime = DateTime.Parse(info.CreateTime);
        //                   entity.UpdateTime = DateTime.Now;

        //                   entity.LeagueId = info.LeagueId.ToString();
        //                   entity.LeagueName = info.LeagueName;

        //                   entity.HomeTeamId = info.HomeTeamId.ToString();
        //                   entity.HomeTeamName = info.HomeTeamName;
        //                   entity.HomeTeamRankName = "";
        //                   entity.GuestTeamId = info.GuestTeamId.ToString();
        //                   entity.GuestTeamName = info.GuestTeamName;
        //                   entity.GuestTeamRankName = "";
        //                   entity.LetBall = info.LetBall;

        //                   entity.MatchStartTime = DateTime.Parse(info.StartDateTime);

        //                   if (isAdd)
        //                   {
        //                       matchManager.AddJCLQ_Match(entity);
        //                   }
        //                   else
        //                   {
        //                       matchManager.UpdateMatch(entity);
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //       }

        //       public string[] AutoUpdateMatchResult_JCLQ(string gameCode, string matchData)
        //       {
        //           var resultList = GetMatchResultList_JingCai(gameCode, matchData);
        //           var matchIdList = new List<string>();
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var matchManager = new JCLQ_MatchManager();
        //               foreach (var result in resultList)
        //               {
        //                   var match = matchManager.GetMatch(gameCode, result.MatchId);
        //                   if (match == null) { continue; }

        //                   if (result.MatchState.Equals("2"))
        //                   {
        //                       match.MatchState = MatchState.Finish;
        //                       //match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                       match.HomeTeamScore = result.HomeScore.ToString();
        //                       //match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                       match.GuestTeamScore = result.GuestScore.ToString();
        //                       match.SF_Result = result.SF_Result;
        //                       match.RFSF_Result = result.RFSF_Result;
        //                       match.SFC_Result = result.SFC_Result;
        //                       match.DXF_Result = result.DXF_Result;
        //                       match.SF_SP = result.SF_SP;
        //                       match.RFSF_SP = result.RFSF_SP;
        //                       match.SFC_SP = result.SFC_SP;
        //                       match.DXF_SP = result.DXF_SP;
        //                       match.OpenTime = DateTime.Now;
        //                       matchManager.UpdateMatch(match);
        //                       if (!match.IsPrized)
        //                       {
        //                           matchIdList.Add(result.MatchId);
        //                       }
        //                   }
        //                   else if (result.MatchState.Equals("3"))
        //                   {
        //                       match.MatchState = MatchState.Finish;
        //                       //match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                       match.HomeTeamScore = result.HomeScore.ToString();
        //                       //match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                       match.GuestTeamScore = result.GuestScore.ToString();
        //                       match.SF_Result = result.SF_Result;
        //                       match.RFSF_Result = result.RFSF_Result;
        //                       match.SFC_Result = result.SFC_Result;
        //                       match.DXF_Result = result.DXF_Result;
        //                       match.SF_SP = result.SF_SP;
        //                       match.RFSF_SP = result.RFSF_SP;
        //                       match.SFC_SP = result.SFC_SP;
        //                       match.DXF_SP = result.DXF_SP;
        //                       match.OpenTime = DateTime.Now;
        //                       matchManager.UpdateMatch(match);
        //                       if (!match.IsPrized)
        //                       {
        //                           matchIdList.Add(result.MatchId);
        //                       }
        //                   }
        //               }
        //               tran.CommitTran();
        //           }
        //           return matchIdList.ToArray();
        //       }

        //       public List<string> PrizeOrder_JCLQ(string agentId, string orderId, IList<MatchInfo> matchResultList, decimal prizedMaxMoney)
        //       {
        //           var noticeIdList = new List<string>();
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var orderManager = new JCLQ_OrderManager();
        //               var matchManager = new JCLQ_MatchManager();
        //               var userManager = new Common_UserManager();
        //               var noticeManager = new Common_NoticeManager();
        //               var historyManager = new Common_GatewayHistoryManager();
        //               var prizeOrder = new PrizeOrderInfo();

        //               #region 验证订单数据

        //               // 查询还未出票的出票订单
        //               var runningOrder = orderManager.QueryRunningOrderByOrderId(orderId);
        //               if (runningOrder == null)
        //               {
        //                   throw new ArgumentException(string.Format("指定订单\"{0}\"不存在或已完成", orderId));
        //               }
        //               if (runningOrder.TicketStatus != OrderStatus.Successful)
        //               {
        //                   throw new ArgumentException(string.Format("指定订单\"{0}\"未出票，不能派奖 - {1}", orderId, runningOrder.TicketStatus));
        //               }
        //               //if (!runningOrder.TicketGateway.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
        //               //{
        //               //    throw new ArgumentException(string.Format("指定订单\"{0}\"本地派奖 - {1}", orderId, runningOrder.TicketGateway));
        //               //}

        //               #endregion


        //               var isBonus = false;
        //               switch (runningOrder.BettingCategory)
        //               {
        //                   case SchemeBettingCategory.GeneralBetting:
        //                   case SchemeBettingCategory.SingleBetting:
        //                       isBonus = PrizeJCLQOrder_GeneralBetting(historyManager, orderManager, matchManager, userManager, matchResultList, runningOrder, agentId, prizedMaxMoney, out prizeOrder);
        //                       break;
        //                   default:
        //                       break;
        //               }

        //               #region 添加通知
        //               //金额超过指定奖金不发通知
        //               if (prizeOrder.AfterTaxBonusMoney < prizedMaxMoney || runningOrder.TicketGateway == "LOCAL")
        //               {
        //                   var agent = userManager.GetUser(agentId);
        //                   if (agent.NoticeStatus == EnableStatus.Enable)
        //                   {
        //                       //添加通知数据
        //                       var num = 100;
        //                       var count = 1;
        //                       count = prizeOrder.PrizeTicketListInfo.Count / num;
        //                       if (prizeOrder.PrizeTicketListInfo.Count % num > 0)
        //                           count++;
        //                       for (int i = 0; i < count; i++)
        //                       {
        //                           var str = JsonSerializer.Serialize<PrizeOrderInfo>(new PrizeOrderInfo
        //                           {
        //                               OrderId = prizeOrder.OrderId,
        //                               AfterTaxBonusMoney = prizeOrder.AfterTaxBonusMoney,
        //                               PreTaxBonusMoney = prizeOrder.PreTaxBonusMoney,
        //                               TicketCount = prizeOrder.PrizeTicketListInfo.Count,
        //                               PrizeTicketListInfo = prizeOrder.PrizeTicketListInfo.Skip(num * i).Take(num).ToList()
        //                           });
        //                           //添加通知数据
        //                           noticeIdList = AddNotice(noticeManager, NoticeType.BonusNotice_Order, str, agent, runningOrder.OrderId);
        //                       }
        //                   }
        //               }
        //               #endregion

        //               tran.CommitTran();
        //           }
        //           return noticeIdList;
        //       }

        //       private bool PrizeJCLQOrder_GeneralBetting(Common_GatewayHistoryManager historyManager, JCLQ_OrderManager orderManager, JCLQ_MatchManager matchManager, Common_UserManager userManager,
        //IList<MatchInfo> matchResultList, JCLQ_Order_Running runningOrder, string agentId, decimal prizedMaxMoney, out PrizeOrderInfo prizeOrder)
        //       {
        //           var isBonus = false;
        //           var totalBeforeTaxBonusMoeny = 0M;
        //           var totalAfterTaxBonusMoeny = 0M;
        //           var totalBonusCount = 0;
        //           prizeOrder = new PrizeOrderInfo();
        //           var prizeTicketList = new List<PrizeTicketListInfo>();

        //           //数字彩票数据
        //           IList<GatewayHistory> zhmTicketList = new List<GatewayHistory>();
        //           IList<GetWay_LocHistory> locTicketList = new List<GetWay_LocHistory>();
        //           if (runningOrder.TicketGateway.Split('|').Contains("LOCAL"))
        //           {
        //               locTicketList = historyManager.GetLocHistoryByOrderId(runningOrder.OrderId, string.Empty);
        //           }

        //           runningOrder.TicketList = orderManager.QueryRunningTicketListByOrderId(agentId, runningOrder.OrderId);
        //           foreach (var ticket in runningOrder.TicketList)
        //           {
        //               ticket.AnteCodeList = orderManager.QueryRunningAnteCodeListByTicketId(ticket.Id);
        //               ticket.BonusTime = DateTime.Now;
        //           }

        //           runningOrder.HitDanMatchIdList = string.Empty;
        //           runningOrder.HitTuoMatchIdList = string.Empty;
        //           runningOrder.HitTotalMatchIdList = string.Empty;

        //           #region 计算订单派奖



        //           #region 虚拟订单处理
        //           foreach (var ticket in locTicketList)
        //           {
        //               if (ticket.TicketStatus != TicketStatus.Successful) continue;

        //               var preTaxBonusMoney = 0M;
        //               var afterTaxBonusMoney = 0M;
        //               var bonusCount = 0;

        //               //var codeList = orderManager.QueryRunningAnteCodeListByOrderId(ticket.OrderId, ticket.MatchIdList.Split(','));
        //               ComputeJCLQTicketBonus(ticket.GameCode, ticket.GameType, ticket.BetType, ticket.LocBetContent, ticket.LocOdds, ticket.Multiple, matchResultList, matchManager, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);
        //               totalBeforeTaxBonusMoeny += preTaxBonusMoney;
        //               totalAfterTaxBonusMoeny += afterTaxBonusMoney;
        //               totalBonusCount += bonusCount;

        //               ticket.BonusMoneyPrevTax = preTaxBonusMoney;
        //               ticket.BonusMoneyAfterTax = afterTaxBonusMoney;
        //               ticket.BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //               historyManager.UpdateGatewayLocHistory(ticket);
        //               prizeTicketList.Add(new PrizeTicketListInfo
        //               {
        //                   TicketId = ticket.TicketId,
        //                   AfterTaxBonusMoney_Ticket = afterTaxBonusMoney,
        //                   PreTaxBonusMoney_Ticket = preTaxBonusMoney
        //               });
        //           }
        //           #endregion


        //           prizeOrder.OrderId = runningOrder.OrderId;
        //           prizeOrder.PreTaxBonusMoney = totalBeforeTaxBonusMoeny;
        //           prizeOrder.AfterTaxBonusMoney = totalAfterTaxBonusMoeny;
        //           prizeOrder.PrizeTicketListInfo = prizeTicketList;

        //           runningOrder.BonusTime = DateTime.Now;
        //           runningOrder.BonusMoneyBeforeTax = totalBeforeTaxBonusMoeny;
        //           runningOrder.BonusMoneyAfterTax = totalAfterTaxBonusMoeny;
        //           runningOrder.BonusStatus = totalAfterTaxBonusMoeny > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //           runningOrder.BonusCount = totalBonusCount;
        //           isBonus = totalAfterTaxBonusMoeny > 0M;
        //           var prizedOrder = new JCLQ_Order_Prized();
        //           prizedOrder.LoadByRunning(runningOrder);
        //           //prizedOrder.IsHandle = totalAfterTaxBonusMoeny >= prizedMaxMoney ? false : true;
        //           prizedOrder.IsHandle = (totalAfterTaxBonusMoeny < prizedMaxMoney || runningOrder.TicketGateway == "LOCAL") ? true : false;
        //           orderManager.AddPrizedOrder(prizedOrder);
        //           orderManager.DeleteRunningOrder(runningOrder);

        //           var orderAllLManager = new Common_OrderManager();
        //           var orderALEntity = orderAllLManager.GetRunningOrder(runningOrder.OrderId);
        //           orderALEntity.PrizedTime = DateTime.Now;
        //           orderALEntity.BonusMoneyPrevTax = totalBeforeTaxBonusMoeny;
        //           orderALEntity.BonusMoneyAfterTax = totalAfterTaxBonusMoeny;
        //           orderAllLManager.UpdateCommon_OrderAllList(orderALEntity);
        //           #endregion

        //           return isBonus;
        //       }


        //       private void ComputeJCLQTicketBonus(string gameCode, string gameType, string betType, string locBetCount, string locOdds, int betAmount, IList<MatchInfo> matchResultList, JCLQ_MatchManager matchManager, decimal betMoney,
        //          out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        //       {
        //           bonusCount = 0;
        //           preTaxBonusMoney = 0M;
        //           afterTaxBonusMoney = 0M;
        //           //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
        //           var locOddsL = locOdds.Split('/');
        //           var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
        //           //查询JCLQ_AnteCode_Running表
        //           //140508010_1/140508011_1
        //           var codeList = new List<JCLQ_AnteCode_Running>();
        //           foreach (var item in locBetCount.Split('/'))
        //           {
        //               var oneMatch = item.Split('_');
        //               if (gameType == "HH")
        //               {
        //                   var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
        //                   codeList.Add(new JCLQ_AnteCode_Running
        //                   {
        //                       MatchId = oneMatch[1],
        //                       AnteNumber = oneMatch[2],
        //                       IsDan = false,
        //                       GameType = oneMatch[0],
        //                       Odds = SP[0][1].ToString()
        //                   });
        //               }
        //               else
        //               {
        //                   var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
        //                   codeList.Add(new JCLQ_AnteCode_Running
        //                   {
        //                       MatchId = oneMatch[0],
        //                       AnteNumber = oneMatch[1],
        //                       IsDan = false,
        //                       GameType = gameType,
        //                       Odds = SP[0][1].ToString()
        //                   });
        //               }
        //           }

        //           var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
        //           var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
        //           var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
        //           if (bonusResult.IsWin)
        //           {
        //               bonusCount = bonusResult.BonusCount;
        //               for (var i = 0; i < bonusResult.BonusCount; i++)
        //               {
        //                   var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                   var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
        //                   var oneBeforeBonusMoney = 2M;
        //                   foreach (var item in tmpAnteList)
        //                   {
        //                       var odds = 1M;

        //                       var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
        //                       var offset = -1M;
        //                       switch (item.GameType.ToUpper())
        //                       {
        //                           case "RFSF":
        //                               offset = item.GetResultOdds("RF");
        //                               break;
        //                           case "DXF":
        //                               offset = item.GetResultOdds("YSZF");
        //                               break;
        //                       }
        //                       var matchResult = result.GetMatchResult(gameCode, item.GameType, offset);
        //                       odds = item.GetResultOdds(matchResult);
        //                       //}
        //                       if (baseCount == 1 && matchResult == "-1")
        //                       {
        //                           preTaxBonusMoney += betMoney;
        //                           afterTaxBonusMoney += betMoney;
        //                           return;
        //                       }
        //                       else
        //                       {
        //                           oneBeforeBonusMoney *= odds;
        //                       }
        //                   }
        //                   oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
        //                   var oneAfterBonusMoney = oneBeforeBonusMoney;
        //                   if (oneBeforeBonusMoney >= _taxBaseMoney_Sport)
        //                   {
        //                       oneAfterBonusMoney = oneBeforeBonusMoney * (1M - _taxRatio_Sport);
        //                   }
        //                   oneBeforeBonusMoney *= betAmount;
        //                   oneAfterBonusMoney *= betAmount;

        //                   preTaxBonusMoney += oneBeforeBonusMoney;
        //                   afterTaxBonusMoney += oneAfterBonusMoney;
        //               }
        //           }
        //       }

        //       public void UpdateMatchPrized_JCLQ(string gameCode, string matchId)
        //       {
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               var issuseManager = new JCLQ_MatchManager();
        //               var entity = issuseManager.GetMatch(gameCode, matchId);
        //               if (entity == null)
        //               {
        //                   throw new ArgumentException(string.Format("比赛{0}-{1}不存在", gameCode, matchId));
        //               }
        //               entity.IsPrized = true;
        //               entity.PrizeTime = DateTime.Now;
        //               issuseManager.UpdateMatch(entity);

        //               tran.CommitTran();
        //           }
        //       }

        //       public JCLQ_MatchQueryCollection QueryJCLQ_MatchInfoList(string gameCode, string matchDate)
        //       {
        //           var collection = new JCLQ_MatchQueryCollection();
        //           var list = new JCLQ_MatchManager().QueryMatchDateMatchInfoList(gameCode, matchDate);
        //           foreach (var item in list)
        //           {
        //               collection.Add(item);
        //           }
        //           return collection;
        //       }

        //       #region 自动服务派奖相关
        //       public string QueryPrizeOrder_JCLQ()
        //       {
        //           var list = new List<string>();
        //           using (var tran = new TicketBusinessManagement())
        //           {
        //               tran.BeginTran();
        //               var manager = new JCLQ_OrderManager();
        //               var orderList = manager.QueryRunningOrderList();
        //               if (orderList == null)
        //                   throw new Exception("没有查询到进行中的订单。");
        //               foreach (var item in orderList)
        //               {
        //                   //var anteCodeList = manager.QueryRunningAnteCodeList(item.OrderId);
        //                   //if (anteCodeList == null)
        //                   //    throw new Exception(string.Format("该订单{0}下面没有找到投注内容数据。", item.OrderId));
        //                   //if (IsPrizeOrder_JCLQ(anteCodeList))
        //                   list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //               }
        //               tran.CommitTran();
        //           }
        //           return string.Join("_", list);
        //       }
        //       public MatchInfoCollection IsPrizeOrder_JCLQ(string orderId, string playType)
        //       {
        //           var collection = new MatchInfoCollection();
        //           var manager = new JCLQ_OrderManager();
        //           var anteCodeList = manager.QueryRunningAnteCodeList(orderId);
        //           if (anteCodeList == null)
        //               throw new Exception(string.Format("该订单{0}下面没有找到投注内容数据。", orderId));
        //           var matchList = new List<string>();
        //           var matchManager = new JCLQ_MatchManager();
        //           foreach (var item in anteCodeList)
        //           {
        //               var match = matchManager.GetMatch(item.GameCode, item.MatchId);
        //               if (match == null)
        //                   throw new Exception("没有找到对应比赛。");
        //               if (match.MatchState != MatchState.Oddsed)
        //                   return new MatchInfoCollection();
        //               var result = QueryCanAwardOrderListByMatch_JCLQ(item.MatchId);
        //               if (matchList.Contains(item.MatchId))
        //                   continue;
        //               matchList.Add(item.MatchId);
        //               if (playType == "1c1")
        //               {
        //                   if (match.MatchState != MatchState.Oddsed)
        //                       return new MatchInfoCollection();
        //               }
        //               collection.AddRange(result.MatchList.Where(p => p.MatchId == item.MatchId));
        //           }
        //           return collection;
        //       }

        //       public string QueryPrizeOrder_JCLQ(string orderId)
        //       {
        //           var list = new List<string>();
        //           var manager = new JCLQ_OrderManager();
        //           var order = manager.QueryRunningOrderList_OrderId(orderId);
        //           if (order == null)
        //               throw new Exception("没有查询到进行中的订单。");
        //           //list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //           return string.Format("{0}|{1}|{2}", order.OrderId, order.AgentId, order.PlayType.Replace("_", "c"));
        //       }

        //       public string ManualPrizeOrder_JCLQ(string orderId)
        //       {
        //           var list = new List<string>();
        //           var manager = new JCLQ_OrderManager();
        //           var order = manager.QueryRunningOrderList_OrderId(orderId);
        //           if (order == null)
        //               order = HandlePrizeOrder_JCLQ(orderId);
        //           if (order == null)
        //               throw new Exception("没有查询到进行中的订单。");
        //           //list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //           return string.Format("{0}|{1}|{2}", order.OrderId, order.AgentId, order.PlayType.Replace("_", "c"));
        //       }


        //       public void PrizeOrder_JCLQ()
        //       {
        //           var manager = new JCLQ_OrderManager();
        //           var matchIdList = manager.QueryDoPrizeOrderToMatchIdList();
        //           foreach (var matchId in matchIdList)
        //           {
        //               try
        //               {
        //                   PrizeOrder_JCLQ_MatchId(matchId);
        //               }
        //               catch (Exception)
        //               {
        //               }
        //           }
        //       }
        //       #endregion

        //       #region 根据比赛派奖

        //       public void PrizeOrder_JCLQ_MatchId(string matchId)
        //       {
        //           var result = QueryCanAwardOrderListByMatch_JCLQ(matchId);
        //           foreach (var item in result.OrderList)
        //           {
        //               try
        //               {
        //                   PrizeOrder_JCLQ(item.AgentId, item.OrderId, result.MatchList, _prizedMaxMoney);
        //               }
        //               catch (Exception)
        //               {
        //               }
        //           }
        //       }

        //       #endregion


        public void UpdateOddsList_JCLQ<TInfo, TEntity>(string gameCode, string gameType, string[] matchIdList, bool isDS)
            where TInfo : JingCaiMatchBase, I_JingCai_Odds
            where TEntity : JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();
                var oddsManager = new JCLQ_OddsManager();
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
                            MatchId = id,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds<TEntity>(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds<TEntity>(entity);
                    }
                }
                tran.CommitTran();
            }
        }

        public void UpdateOddsList_JCLQ_Manual()
        {
            UpdateOddsList_JCLQ_Manual<JCLQ_SF_SPInfo, JCLQ_Odds_SF>("JCLQ", "SF");
            UpdateOddsList_JCLQ_Manual<JCLQ_RFSF_SPInfo, JCLQ_Odds_RFSF>("JCLQ", "RFSF");
            UpdateOddsList_JCLQ_Manual<JCLQ_SFC_SPInfo, JCLQ_Odds_SFC>("JCLQ", "SFC");
            UpdateOddsList_JCLQ_Manual<JCLQ_DXF_SPInfo, JCLQ_Odds_DXF>("JCLQ", "DXF");
        }

        public void UpdateOddsList_JCLQ_Manual<TInfo, TEntity>(string gameCode, string gameType)
            where TInfo : JingCaiMatchBase, I_JingCai_Odds
            where TEntity : JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, string.Empty);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();
                var oddsManager = new JCLQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                    {
                        continue;
                    }
                    var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, false);
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
                        oddsManager.AddOdds<TEntity>(entity);
                    }
                    else if (!entity.Equals(odds))
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.SetOdds(odds);
                        oddsManager.UpdateOdds<TEntity>(entity);
                    }
                }
                tran.CommitTran();
            }
        }

        public string PrizeJCLQTicket(int num)
        {
            var successCount = 0;
            var failCount = 0;
            var log = new List<string>();

            try
            {
                var manager = new Sports_Manager();
                var collection = manager.QueryPrizeTicketList("JCLQ", num);
                var prizeList = new List<TicketBatchPrizeInfo>();
                //var ticketStrSql = string.Empty;
                foreach (var ticket in collection.TicketList)
                {
                    if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

                    var preTaxBonusMoney = 0M;
                    var afterTaxBonusMoney = 0M;
                    var bonusCount = 0;

                    try
                    {
                        ComputeJCLQTicketBonus(ticket.SchemeId, ticket.GameCode, ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, collection.MatchList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);
                        //更新票数据sql
                        prizeList.Add(new TicketBatchPrizeInfo
                        {
                            //Id = item.Id,
                            TicketId = ticket.TicketId,
                            BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                            PreMoney = preTaxBonusMoney,
                            AfterMoney = afterTaxBonusMoney,
                        });

                        //var ticketStrSql = string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2} where TicketId='{3}' {4}"
                        //             , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);
                        //manager.ExecSql(ticketStrSql);

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        log.Add(ticket.TicketId + "派奖出错 - " + ex.Message);
                    }

                }
                //批量更新数据库
                BusinessHelper.UpdateTicketBonus(prizeList);

                log.Insert(0, string.Format("总查询到{0}张票,成功派奖票：{1}条，失败派奖票：{2}条", collection.TicketList.Count, successCount, failCount));
            }
            catch (Exception ex)
            {
                return "派奖票数据出错 - " + ex.Message;
            }

            return string.Join(Environment.NewLine, log.ToArray());
        }

        public void PrizeJCLQTicket_OrderId(string orderId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
        }

        private void ComputeJCLQTicketBonus(string orderId, string gameCode, string gameType, string betType, string locBetCount, string locOdds, int betAmount, IList<MatchInfo> matchResultList, decimal betMoney,
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
                manager.AddSingleSchemeOrder(new SingleSchemeOrder
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
