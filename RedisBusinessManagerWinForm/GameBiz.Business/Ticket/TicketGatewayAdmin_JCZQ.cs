using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Lottery;
using System.IO;
using Common.JSON;
using Common.Algorithms;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Business.Domain.Managers.Ticket;
using GameBiz.Core.Ticket;
using GameBiz.Domain.Managers;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using System.Diagnostics;

namespace GameBiz.Business
{
    public partial class TicketGatewayAdmin
    {
        //      public void AddMatchList_JCZQ(string gameCode, string[] matchIdList)
        //      {
        //          var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //          var addList = matchInfoList.Where(m => matchIdList.Contains(m.MatchId));
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var matchManager = new JCZQ_MatchManager();
        //              foreach (var info in addList)
        //              {
        //                  var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                  if (entity != null)
        //                  {
        //                      continue;
        //                  }
        //                  entity = new JCZQ_Match
        //                  {
        //                      CompositeId = gameCode + "|" + info.MatchId,
        //                      GameCode = gameCode,
        //                      MatchId = info.MatchId,
        //                      MatchIdName = info.MatchIdName,
        //                      MatchNumber = info.MatchNumber,
        //                      MatchDate = info.MatchData,
        //                      DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime),
        //                      FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime),

        //                      CreateTime = DateTime.Parse(info.CreateTime),
        //                      UpdateTime = DateTime.Now,
        //                      OpenTime = null,

        //                      LeagueId = info.LeagueId.ToString(),
        //                      LeagueName = info.LeagueName,

        //                      HomeTeamId = info.HomeTeamId.ToString(),
        //                      HomeTeamName = info.HomeTeamName,
        //                      HomeTeamRankName = "",
        //                      GuestTeamId = info.GuestTeamId.ToString(),
        //                      GuestTeamName = info.GuestTeamName,
        //                      GuestTeamRankName = "",
        //                      LetBall = info.LetBall,

        //                      MatchStartTime = DateTime.Parse(info.StartDateTime),
        //                      MatchState = MatchState.Waiting,
        //                      HomeTeamHalfScore = "-",
        //                      HomeTeamScore = "-",
        //                      GuestTeamHalfScore = "-",
        //                      GuestTeamScore = "-",

        //                      SPF_Result = "-",
        //                      BRQSPF_Result = "-",
        //                      ZJQ_Result = "-",
        //                      BF_Result = "-",
        //                      BQC_Result = "-",
        //                  };
        //                  matchManager.AddJCZQ_Match(entity);
        //              }
        //              tran.CommitTran();
        //          }
        //      }
        //      public void AddMatchList_JCZQ_Service(string gameCode)
        //      {
        //          var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var matchManager = new JCZQ_MatchManager();
        //              foreach (var info in matchInfoList)
        //              {
        //                  var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                  if (entity != null)
        //                  {
        //                      continue;
        //                  }
        //                  entity = new JCZQ_Match
        //                  {
        //                      CompositeId = gameCode + "|" + info.MatchId,
        //                      GameCode = gameCode,
        //                      MatchId = info.MatchId,
        //                      MatchIdName = info.MatchIdName,
        //                      MatchNumber = info.MatchNumber,
        //                      MatchDate = info.MatchData,
        //                      DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime),
        //                      FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime),

        //                      CreateTime = DateTime.Parse(info.CreateTime),
        //                      UpdateTime = DateTime.Now,
        //                      OpenTime = null,

        //                      LeagueId = info.LeagueId.ToString(),
        //                      LeagueName = info.LeagueName,

        //                      HomeTeamId = info.HomeTeamId.ToString(),
        //                      HomeTeamName = info.HomeTeamName,
        //                      HomeTeamRankName = "",
        //                      GuestTeamId = info.GuestTeamId.ToString(),
        //                      GuestTeamName = info.GuestTeamName,
        //                      GuestTeamRankName = "",
        //                      LetBall = info.LetBall,

        //                      MatchStartTime = DateTime.Parse(info.StartDateTime),
        //                      MatchState = MatchState.Waiting,
        //                      HomeTeamHalfScore = "-",
        //                      HomeTeamScore = "-",
        //                      GuestTeamHalfScore = "-",
        //                      GuestTeamScore = "-",

        //                      SPF_Result = "-",
        //                      BRQSPF_Result = "-",
        //                      ZJQ_Result = "-",
        //                      BF_Result = "-",
        //                      BQC_Result = "-",
        //                  };
        //                  matchManager.AddJCZQ_Match(entity);
        //              }
        //              tran.CommitTran();
        //          }
        //      }
        //      public void UpdateMatchList_JCZQ(string gameCode, string[] matchIdList)
        //      {
        //          var matchInfoList = GetMatchList_JingCai(gameCode, "");
        //          var addList = matchInfoList.Where(m => matchIdList.Contains(m.MatchId));
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var matchManager = new JCZQ_MatchManager();
        //              foreach (var info in addList)
        //              {
        //                  var isAdd = false;
        //                  var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                  if (entity == null)
        //                  {
        //                      entity = new JCZQ_Match();
        //                      entity.OpenTime = null;
        //                      entity.MatchState = MatchState.Waiting;
        //                      entity.HomeTeamHalfScore = "-";
        //                      entity.HomeTeamScore = "-";
        //                      entity.GuestTeamHalfScore = "-";
        //                      entity.GuestTeamScore = "-";
        //                      entity.SPF_Result = "-";
        //                      entity.BRQSPF_Result = "-";
        //                      entity.ZJQ_Result = "-";
        //                      entity.BF_Result = "-";
        //                      entity.BQC_Result = "-";

        //                      isAdd = true;
        //                  }
        //                  entity.CompositeId = gameCode + "|" + info.MatchId;
        //                  entity.GameCode = gameCode;
        //                  entity.MatchId = info.MatchId;
        //                  entity.MatchIdName = info.MatchIdName;
        //                  entity.MatchNumber = info.MatchNumber;
        //                  entity.MatchDate = info.MatchData;
        //                  entity.DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime);
        //                  entity.FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime);

        //                  entity.CreateTime = DateTime.Parse(info.CreateTime);
        //                  entity.UpdateTime = DateTime.Now;

        //                  entity.LeagueId = info.LeagueId.ToString();
        //                  entity.LeagueName = info.LeagueName;

        //                  entity.HomeTeamId = info.HomeTeamId.ToString();
        //                  entity.HomeTeamName = info.HomeTeamName;
        //                  entity.HomeTeamRankName = "";
        //                  entity.GuestTeamId = info.GuestTeamId.ToString();
        //                  entity.GuestTeamName = info.GuestTeamName;
        //                  entity.GuestTeamRankName = "";
        //                  entity.LetBall = info.LetBall;

        //                  entity.MatchStartTime = DateTime.Parse(info.StartDateTime);

        //                  if (isAdd)
        //                  {
        //                      matchManager.AddJCZQ_Match(entity);
        //                  }
        //                  else
        //                  {
        //                      matchManager.UpdateMatch(entity);
        //                  }
        //              }
        //              tran.CommitTran();
        //          }
        //      }
        //      public void UpdateOddsList_JCZQ_Service<TInfo, TEntity>(string gameCode, string gameType, bool isDS)
        //          where TInfo : JingCaiMatchBase, I_JingCai_Odds
        //          where TEntity : JingCai_Odds, new()
        //      {
        //          var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var oddsManager = new JCZQ_OddsManager();
        //              foreach (var odds in oddsList)
        //              {
        //                  //todo:
        //                  var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, isDS);
        //                  if (entity == null)
        //                  {

        //                      entity = new TEntity
        //                      {
        //                          MatchId = odds.MatchId,
        //                          CreateTime = DateTime.Now,
        //                      };
        //                      entity.SetOdds(odds);
        //                      oddsManager.AddOdds<TEntity>(entity);
        //                  }
        //                  else if (!entity.Equals(odds))
        //                  {
        //                      entity.CreateTime = DateTime.Now;
        //                      entity.SetOdds(odds);
        //                      oddsManager.UpdateOdds<TEntity>(entity);
        //                  }
        //              }
        //              tran.CommitTran();
        //          }
        //      }
        //      public void AutoUpdateOdds_JCZQ<TInfo, TEntity>(string gameCode, string gameType, bool isDS)
        //          where TInfo : JingCaiMatchBase, I_JingCai_Odds
        //          where TEntity : JingCai_Odds, new()
        //      {
        //          var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var oddsManager = new JCZQ_OddsManager();
        //              foreach (var odds in oddsList)
        //              {
        //                  if (!odds.CheckIsValidate())
        //                  {
        //                      continue;
        //                  }
        //                  var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, isDS);
        //                  if (entity == null || !entity.Equals(odds))
        //                  {
        //                      entity = new TEntity
        //                      {
        //                          MatchId = odds.MatchId,
        //                          CreateTime = DateTime.Now,
        //                      };
        //                      entity.SetOdds(odds);
        //                      oddsManager.AddOdds<TEntity>(entity);
        //                  }
        //              }
        //              tran.CommitTran();
        //          }
        //      }
        //      public IList<JCZQ_MatchQueryInfo> QueryMatchList_JCZQToMatchId(string matchId)
        //      {
        //          var issuseManager = new JCZQ_MatchManager();
        //          return issuseManager.QueryMatchListToMatchId(matchId);
        //      }
        //      public IList<JCZQ_MatchQueryInfo> QueryWaitPrizeMatchList_JCZQ(string gameCode, string matchDate)
        //      {
        //          var issuseManager = new JCZQ_MatchManager();
        //          return issuseManager.QueryWaitPrizeMatchList(gameCode, matchDate);
        //      }
        //      public void UpdateMatchPrized_JCZQ(string fid, MatchState matchState, bool isPrized)
        //      {
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var issuseManager = new JCZQ_MatchManager();
        //              var f = fid.Split('_');
        //              var entity = issuseManager.GetMatch(f[1].ToString(), f[0].ToString());
        //              if (entity == null)
        //              {
        //                  throw new ArgumentException(string.Format("比赛{0}-{1}不存在", f[1].ToString(), f[0].ToString()));
        //              }
        //              entity.IsPrized = isPrized;
        //              entity.MatchState = matchState;
        //              issuseManager.UpdateMatch(entity);

        //              tran.CommitTran();
        //          }
        //      }
        //      public void UpdateMatchCancel_JCZQ(string fid)
        //      {
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var issuseManager = new JCZQ_MatchManager();
        //              var f = fid.Split('_');
        //              var entity = issuseManager.GetMatch(f[1].ToString(), f[0].ToString());
        //              if (entity == null)
        //              {
        //                  throw new ArgumentException(string.Format("比赛{0}-{1}不存在", f[1].ToString(), f[0].ToString()));
        //              }
        //              entity.HomeTeamHalfScore = "-1";
        //              entity.HomeTeamScore = "-1";
        //              entity.GuestTeamHalfScore = "-1";
        //              entity.GuestTeamScore = "-1";
        //              entity.SPF_Result = "-1";
        //              entity.BRQSPF_Result = "-1";
        //              entity.BF_Result = "-1";
        //              entity.ZJQ_Result = "-1";
        //              entity.BQC_Result = "-1";
        //              entity.SPF_SP = 1M;
        //              entity.BRQSPF_SP = 1M;
        //              entity.BF_SP = 1M;
        //              entity.ZJQ_SP = 1M;
        //              entity.BQC_SP = 1M;
        //              entity.MatchState = MatchState.Oddsed;
        //              issuseManager.UpdateMatch(entity);

        //              tran.CommitTran();
        //          }
        //      }
        //      public void UpdateMatchResultList_JCZQ(string gameCode, string[] matchIdList, out  List<string> matchList)
        //      {
        //          var resultList = GetMatchResultList_JingCai(gameCode, "");
        //          matchList = new List<string>();
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var matchManager = new JCZQ_MatchManager();
        //              var updateList = resultList.Where(r => matchIdList.Contains(r.MatchId));
        //              foreach (var result in updateList)
        //              {
        //                  var match = matchManager.GetMatch(gameCode, result.MatchId);
        //                  if (match == null) { continue; }

        //                  if (result.MatchState.Equals("2") && match.MatchState != MatchState.Oddsed)
        //                  {
        //                      match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                      match.HomeTeamScore = result.FullHomeTeamScore.ToString();
        //                      match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                      match.GuestTeamScore = result.FullGuestTeamScore.ToString();
        //                      match.SPF_Result = result.SPF_Result;
        //                      match.BRQSPF_Result = result.BRQSPF_Result;
        //                      match.ZJQ_Result = result.ZJQ_Result;
        //                      match.BF_Result = result.BF_Result;
        //                      match.BQC_Result = result.BQC_Result;
        //                      match.SPF_SP = result.SPF_SP;
        //                      match.BRQSPF_SP = result.BRQSPF_SP;
        //                      match.ZJQ_SP = result.ZJQ_SP;
        //                      match.BF_SP = result.BF_SP;
        //                      match.BQC_SP = result.BQC_SP;
        //                      match.OpenTime = DateTime.Now;

        //                      if (result.SPF_SP == 1M && result.BRQSPF_SP == 1M && result.ZJQ_SP == 1M && result.BF_SP == 1M && result.BQC_SP == 1M) continue;

        //                      if ((result.SPF_SP > 1M || result.SPF_SP == 0M) && (result.BRQSPF_SP > 1M || result.BRQSPF_SP == 0M) && (result.ZJQ_SP > 1M || result.ZJQ_SP == 0M) && (result.BF_SP > 1M || result.BF_SP == 0M) && (result.BQC_SP > 1M || result.BQC_SP == 0M))
        //                      {
        //                          match.MatchState = MatchState.Oddsed;
        //                          matchList.Add(match.MatchId);
        //                      }
        //                      else
        //                      {
        //                          match.MatchState = MatchState.Finish;
        //                      }
        //                      matchManager.UpdateMatch(match);
        //                  }
        //                  else if (result.MatchState.Equals("3"))
        //                  {
        //                      match.MatchState = MatchState.Oddsed;
        //                      match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                      match.HomeTeamScore = result.FullHomeTeamScore.ToString();
        //                      match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                      match.GuestTeamScore = result.FullGuestTeamScore.ToString();
        //                      match.SPF_Result = "-1";
        //                      match.BRQSPF_Result = "-1";
        //                      match.ZJQ_Result = "-1";
        //                      match.BF_Result = "-1";
        //                      match.BQC_Result = "-1";
        //                      match.SPF_SP = 1M;
        //                      match.BRQSPF_SP = 1M;
        //                      match.ZJQ_SP = 1M;
        //                      match.BF_SP = 1M;
        //                      match.BQC_SP = 1M;
        //                      match.OpenTime = DateTime.Now;
        //                      matchManager.UpdateMatch(match);
        //                  }
        //                  else if (result.MatchState.Equals("4"))
        //                  {
        //                      match.MatchState = MatchState.Late;
        //                      matchManager.UpdateMatch(match);
        //                  }
        //              }
        //              tran.CommitTran();
        //          }

        //      }
        //      public OrderId_QueryInfoCollection QueryCanAwardOrderListByMatch_JCZQ(string matchId)
        //      {
        //          var orderManager = new JCZQ_OrderManager();
        //          return orderManager.QueryCanAwardOrderListByMatch(matchId);
        //      }

        //      public string QueryJCZQCanAwardOrderListByMatchId(string matchIdList)
        //      {
        //          var strList = new List<string>();
        //          var order_JCZQManager = new JCZQ_OrderManager();
        //          var order_JCLQManager = new JCLQ_OrderManager();
        //          var array = matchIdList.Split('^');
        //          foreach (var matchId in array)
        //          {
        //              var gameCodeMatchId = matchId.Split('|');
        //              if (gameCodeMatchId.Length != 2)
        //                  return string.Join(",", strList);
        //              if (gameCodeMatchId[0].ToUpper() == "JCZQ")
        //              {
        //                  if (string.IsNullOrEmpty(gameCodeMatchId[1]))
        //                      continue;
        //                  var orderList = order_JCZQManager.QueryCanAwardOrderList(gameCodeMatchId[1]);
        //                  foreach (var item in orderList)
        //                  {
        //                      if (!strList.Contains(item.OrderId))
        //                          strList.Add(item.OrderId);
        //                  }
        //              }
        //              else if (gameCodeMatchId[0].ToUpper() == "JCLQ")
        //              {
        //                  if (string.IsNullOrEmpty(gameCodeMatchId[1]))
        //                      continue;
        //                  var orderList = order_JCLQManager.QueryCanAwardOrderList(gameCodeMatchId[1]);
        //                  foreach (var item in orderList)
        //                  {
        //                      if (!strList.Contains(item.OrderId))
        //                          strList.Add(item.OrderId);
        //                  }
        //              }
        //          }
        //          return string.Join(",", strList);
        //      }

        //      public IList<Order_QueryInfo> QueryRunningOrderListById_JCZQ(string orderIdList, out int totalCount, out decimal totalMoney)
        //      {
        //          var orderManager = new JCZQ_OrderManager();
        //          return orderManager.QueryRunningOrderListById_JCZQ(orderIdList, out totalCount, out totalMoney);
        //      }
        //      public void AutoUpdateMatch_JCZQ(string gameCode, string matchData)
        //      {
        //          var matchInfoList = GetMatchList_JingCai(gameCode, matchData);
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();
        //              var matchManager = new JCZQ_MatchManager();
        //              foreach (var info in matchInfoList)
        //              {
        //                  var isAdd = false;
        //                  var entity = matchManager.GetMatch(gameCode, info.MatchId);
        //                  if (entity == null)
        //                  {
        //                      entity = new JCZQ_Match();
        //                      entity.OpenTime = null;
        //                      entity.MatchState = MatchState.Waiting;
        //                      entity.HomeTeamHalfScore = "-";
        //                      entity.HomeTeamScore = "-";
        //                      entity.GuestTeamHalfScore = "-";
        //                      entity.GuestTeamScore = "-";
        //                      entity.SPF_Result = "-";
        //                      entity.BRQSPF_Result = "-";
        //                      entity.ZJQ_Result = "-";
        //                      entity.BF_Result = "-";
        //                      entity.BQC_Result = "-";

        //                      isAdd = true;
        //                  }
        //                  entity.CompositeId = gameCode + "|" + info.MatchId;
        //                  entity.GameCode = gameCode;
        //                  entity.MatchId = info.MatchId;
        //                  entity.MatchIdName = info.MatchIdName;
        //                  entity.MatchNumber = info.MatchNumber;
        //                  entity.MatchDate = info.MatchData;
        //                  entity.DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime);
        //                  entity.FSBettingStopTime = DateTime.Parse(info.FSStopBettingTime);

        //                  entity.CreateTime = DateTime.Parse(info.CreateTime);
        //                  entity.UpdateTime = DateTime.Now;

        //                  entity.LeagueId = info.LeagueId.ToString();
        //                  entity.LeagueName = info.LeagueName;

        //                  entity.HomeTeamId = info.HomeTeamId.ToString();
        //                  entity.HomeTeamName = info.HomeTeamName;
        //                  entity.HomeTeamRankName = "";
        //                  entity.GuestTeamId = info.GuestTeamId.ToString();
        //                  entity.GuestTeamName = info.GuestTeamName;
        //                  entity.GuestTeamRankName = "";
        //                  entity.LetBall = info.LetBall;

        //                  entity.MatchStartTime = DateTime.Parse(info.StartDateTime);

        //                  if (isAdd)
        //                  {
        //                      matchManager.AddJCZQ_Match(entity);
        //                  }
        //                  else
        //                  {
        //                      matchManager.UpdateMatch(entity);
        //                  }
        //              }
        //              tran.CommitTran();
        //          }
        //      }

        //      public string[] AutoUpdateMatchResult_JCZQ(string gameCode, string matchData)
        //      {
        //          var resultList = GetMatchResultList_JingCai(gameCode, matchData);
        //          if (resultList.Count() <= 0)
        //              throw new Exception("比赛结果跟新失败！");
        //          var matchIdList = new List<string>();
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();

        //              var matchManager = new JCZQ_MatchManager();
        //              foreach (var result in resultList)
        //              {
        //                  var match = matchManager.GetMatch(gameCode, result.MatchId);
        //                  if (match == null) { continue; }

        //                  if (result.MatchState.Equals("2"))
        //                  {
        //                      match.MatchState = MatchState.Finish;
        //                      match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                      match.HomeTeamScore = result.FullHomeTeamScore.ToString();
        //                      match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                      match.GuestTeamScore = result.FullGuestTeamScore.ToString();
        //                      match.SPF_Result = result.SPF_Result;
        //                      match.BRQSPF_Result = result.BRQSPF_Result;
        //                      match.ZJQ_Result = result.ZJQ_Result;
        //                      match.BF_Result = result.BF_Result;
        //                      match.BQC_Result = result.BQC_Result;
        //                      match.SPF_SP = result.SPF_SP;
        //                      match.BRQSPF_SP = result.BRQSPF_SP;
        //                      match.ZJQ_SP = result.ZJQ_SP;
        //                      match.BF_SP = result.BF_SP;
        //                      match.BQC_SP = result.BQC_SP;
        //                      match.OpenTime = DateTime.Now;
        //                      matchManager.UpdateMatch(match);
        //                      if (!match.IsPrized)
        //                      {
        //                          matchIdList.Add(result.MatchId);
        //                      }
        //                  }
        //                  else if (result.MatchState.Equals("3"))
        //                  {
        //                      match.MatchState = MatchState.Finish;
        //                      match.HomeTeamHalfScore = result.HalfHomeTeamScore.ToString();
        //                      match.HomeTeamScore = result.FullHomeTeamScore.ToString();
        //                      match.GuestTeamHalfScore = result.HalfGuestTeamScore.ToString();
        //                      match.GuestTeamScore = result.FullGuestTeamScore.ToString();
        //                      match.SPF_Result = result.SPF_Result;
        //                      match.BRQSPF_Result = result.BRQSPF_Result;
        //                      match.ZJQ_Result = result.ZJQ_Result;
        //                      match.BF_Result = result.BF_Result;
        //                      match.BQC_Result = result.BQC_Result;
        //                      match.SPF_SP = result.SPF_SP;
        //                      match.BRQSPF_SP = result.BRQSPF_SP;
        //                      match.ZJQ_SP = result.ZJQ_SP;
        //                      match.BF_SP = result.BF_SP;
        //                      match.BQC_SP = result.BQC_SP;
        //                      match.OpenTime = DateTime.Now;
        //                      matchManager.UpdateMatch(match);
        //                      if (!match.IsPrized)
        //                      {
        //                          matchIdList.Add(result.MatchId);
        //                      }
        //                  }
        //                  else if (result.MatchState.Equals("4"))
        //                  {
        //                      match.MatchState = MatchState.Late;
        //                      matchManager.UpdateMatch(match);
        //                  }
        //              }

        //              tran.CommitTran();
        //          }
        //          return matchIdList.ToArray();
        //      }
        //      private string ConvertAnteCode(string gameCode, string gameType, string ante)
        //      {
        //          //ante = ante.Replace("[", "").Replace("]", "");
        //          switch (gameCode.ToLower())
        //          {
        //              case "jczq":
        //                  switch (gameType.ToLower())
        //                  {
        //                      case "spf":
        //                      case "brqspf":
        //                          return ante.Replace("胜", "3").Replace("平", "1").Replace("负", "0");
        //                      case "zjq":
        //                      case "jqs":
        //                          return ante.Replace("7+", "7");
        //                      //5-001:[4:1,4:2]/5-002:[1:4]
        //                      case "bf": return ante.Replace("1:0", "10").Replace("2:0", "20").Replace("2:1", "21").Replace("3:0", "30").Replace("3:1", "31").Replace("3:2", "32").Replace("4:0", "40").Replace("4:1", "41").Replace("4:2", "42").Replace("5:0", "50").Replace("5:1", "51").Replace("5:2", "52").Replace("胜其他", "X0")
        //                              .Replace("0:0", "00").Replace("1:1", "11").Replace("2:2", "22").Replace("3:3", "33").Replace("平其他", "XX")
        //                              .Replace("0:1", "01").Replace("0:2", "02").Replace("1:2", "12").Replace("0:3", "03").Replace("1:3", "13").Replace("2:3", "23").Replace("0:4", "04").Replace("1:4", "14").Replace("2:4", "24").Replace("0:5", "05").Replace("1:5", "15").Replace("2:5", "25").Replace("负其他", "0X");
        //                      case "bqc":
        //                          return ante.Replace("胜-胜", "33").Replace("胜-平", "31").Replace("胜-负", "30")
        //                              .Replace("平-胜", "13").Replace("平-平", "11").Replace("平-负", "10")
        //                              .Replace("负-胜", "03").Replace("负-平", "01").Replace("负-负", "00");
        //                  }
        //                  return ante;
        //              case "jclq":
        //                  switch (gameType.ToLower())
        //                  {
        //                      case "sf":
        //                      case "rfsf":
        //                          return ante.Replace("主胜", "3").Replace("客胜", "0");
        //                      case "dxf":
        //                          return ante.Replace("大", "3").Replace("小", "0");
        //                      case "sfc":
        //                      case "fc":
        //                          return ante
        //                              .Replace("胜A", "01").Replace("胜B", "02").Replace("胜C", "03").Replace("胜D", "04").Replace("胜E", "05").Replace("胜F", "06")
        //                              .Replace("负A", "11").Replace("负B", "12").Replace("负C", "13").Replace("负D", "14").Replace("负E", "15").Replace("负F", "16")
        //                              //.Replace("胜1-5", "胜A").Replace("胜6-10", "胜B").Replace("胜11-15", "胜C").Replace("胜16-20", "胜D").Replace("胜21-25", "胜E").Replace("胜26+", "胜F")
        //                              //.Replace("负1-5", "负A").Replace("负6-10", "负B").Replace("负11-15", "负C").Replace("负16-20", "负D").Replace("负21-25", "负E").Replace("负26+", "负F")
        //                              .Replace("胜1-5", "01").Replace("胜6-10", "02").Replace("胜11-15", "03").Replace("胜16-20", "04").Replace("胜21-25", "05").Replace("胜26+", "06")
        //                              .Replace("负1-5", "11").Replace("负6-10", "12").Replace("负11-15", "13").Replace("负16-20", "14").Replace("负21-25", "15").Replace("负26+", "16");
        //                  }
        //                  return ante;
        //              default:
        //                  break;
        //          }

        //          return ante;
        //      }
        //      public List<string> PrizeOrder_JCZQ(string agentId, string orderId, IList<MatchInfo> matchResultList, decimal prizedMaxMoney)
        //      {
        //          var noticeIdList = new List<string>();
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();

        //              var orderManager = new JCZQ_OrderManager();
        //              var matchManager = new JCZQ_MatchManager();
        //              var userManager = new Common_UserManager();
        //              var noticeManager = new Common_NoticeManager();
        //              var historyManager = new Common_GatewayHistoryManager();
        //              var prizeOrder = new PrizeOrderInfo();

        //              #region 验证订单数据

        //              // 查询还未出票的出票订单
        //              var runningOrder = orderManager.QueryRunningOrderByOrderId(orderId);
        //              if (runningOrder == null)
        //              {
        //                  throw new ArgumentException(string.Format("指定订单\"{0}\"不存在或已完成", orderId));
        //              }
        //              if (runningOrder.TicketStatus != OrderStatus.Successful)
        //              {
        //                  throw new ArgumentException(string.Format("指定订单\"{0}\"未出票，不能派奖 - {1}", orderId, runningOrder.TicketStatus));
        //              }
        //              //if (!runningOrder.TicketGateway.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
        //              //{
        //              //    throw new ArgumentException(string.Format("指定订单\"{0}\"本地派奖 - {1}", orderId, runningOrder.TicketGateway));
        //              //}

        //              #endregion

        //              var isBonus = false;
        //              switch (runningOrder.BettingCategory)
        //              {
        //                  case SchemeBettingCategory.GeneralBetting:
        //                  case SchemeBettingCategory.SingleBetting:
        //                      isBonus = PrizeJCZQOrder_GeneralBetting(historyManager, orderManager, matchManager, userManager, matchResultList, runningOrder, agentId, prizedMaxMoney, out prizeOrder);
        //                      break;
        //                  default:
        //                      break;
        //              }

        //              #region 更新票中奖数据

        //              //Prize_ShopTicket_JC(tran, runningOrder.TicketGateway, runningOrder.OrderId, matchResultList);

        //              #endregion

        //              #region 添加通知
        //              //金额超过指定奖金不发通知
        //              if (prizeOrder.AfterTaxBonusMoney < prizedMaxMoney || runningOrder.TicketGateway == "LOCAL")
        //              {
        //                  var agent = userManager.GetUser(agentId);
        //                  if (agent.NoticeStatus == EnableStatus.Enable)
        //                  {
        //                      //添加通知数据
        //                      var num = 100;
        //                      var count = 1;
        //                      count = prizeOrder.PrizeTicketListInfo.Count / num;
        //                      if (prizeOrder.PrizeTicketListInfo.Count % num > 0)
        //                          count++;
        //                      for (int i = 0; i < count; i++)
        //                      {
        //                          var str = JsonSerializer.Serialize<PrizeOrderInfo>(new PrizeOrderInfo
        //                          {
        //                              OrderId = prizeOrder.OrderId,
        //                              AfterTaxBonusMoney = prizeOrder.AfterTaxBonusMoney,
        //                              PreTaxBonusMoney = prizeOrder.PreTaxBonusMoney,
        //                              TicketCount = prizeOrder.PrizeTicketListInfo.Count,
        //                              PrizeTicketListInfo = prizeOrder.PrizeTicketListInfo.Skip(num * i).Take(num).ToList()
        //                          });
        //                          //添加通知数据
        //                          noticeIdList = AddNotice(noticeManager, NoticeType.BonusNotice_Order, str, agent, runningOrder.OrderId);
        //                      }

        //                      //var text = string.Format("{0}_{1}_{2}_{3}_{4}", runningOrder.GameCode, runningOrder.OrderId, isBonus.ToString().ToLower());
        //                      //noticeIdList = AddNotice(noticeManager, NoticeType.BonusNotice_JCZQ, text, agent, orderId);
        //                  }
        //              }
        //              #endregion

        //              tran.CommitTran();
        //          }
        //          return noticeIdList;
        //      }

        //      private bool PrizeJCZQOrder_GeneralBetting(Common_GatewayHistoryManager historyManager, JCZQ_OrderManager orderManager, JCZQ_MatchManager matchManager, Common_UserManager userManager,
        //IList<MatchInfo> matchResultList, JCZQ_Order_Running runningOrder, string agentId, decimal prizedMaxMoney, out PrizeOrderInfo prizeOrder)
        //      {
        //          var isBonus = false;
        //          var totalBeforeTaxBonusMoeny = 0M;
        //          var totalAfterTaxBonusMoeny = 0M;
        //          var totalBonusCount = 0;
        //          prizeOrder = new PrizeOrderInfo();
        //          var prizeTicketList = new List<PrizeTicketListInfo>();

        //          //数字彩票数据
        //          IList<GatewayHistory> zhmTicketList = new List<GatewayHistory>();
        //          IList<GetWay_LocHistory> locTicketList = new List<GetWay_LocHistory>();
        //          if (runningOrder.TicketGateway.Split('|').Contains("LOCAL"))
        //          {
        //              locTicketList = historyManager.GetLocHistoryByOrderId(runningOrder.OrderId, string.Empty);
        //          }
        //          runningOrder.TicketList = orderManager.QueryRunningTicketListByOrderId(agentId, runningOrder.OrderId);

        //          foreach (var ticket in runningOrder.TicketList)
        //          {
        //              ticket.AnteCodeList = orderManager.QueryRunningAnteCodeListByTicketId(ticket.Id);
        //              ticket.BonusTime = DateTime.Now;
        //          }

        //          #region 计算订单派奖

        //          #region 虚拟订单处理
        //          foreach (var ticket in locTicketList)
        //          {
        //              if (ticket.TicketStatus != TicketStatus.Successful) continue;

        //              var preTaxBonusMoney = 0M;
        //              var afterTaxBonusMoney = 0M;
        //              var bonusCount = 0;
        //              //var codeList = orderManager.QueryRunningAnteCodeListByOrderId(ticket.OrderId, ticket.MatchIdList.Split(','));
        //              ComputeJCZQTicketBonus(ticket.OrderId, ticket.GameCode, ticket.GameType, ticket.BetType, ticket.LocBetContent, ticket.LocOdds, ticket.Multiple, matchResultList, matchManager, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);
        //              totalBeforeTaxBonusMoeny += preTaxBonusMoney;
        //              totalAfterTaxBonusMoeny += afterTaxBonusMoney;
        //              totalBonusCount += bonusCount;

        //              ticket.BonusMoneyPrevTax = preTaxBonusMoney;
        //              ticket.BonusMoneyAfterTax = afterTaxBonusMoney;
        //              ticket.BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //              historyManager.UpdateGatewayLocHistory(ticket);
        //              prizeTicketList.Add(new PrizeTicketListInfo
        //              {
        //                  TicketId = ticket.TicketId,
        //                  AfterTaxBonusMoney_Ticket = afterTaxBonusMoney,
        //                  PreTaxBonusMoney_Ticket = preTaxBonusMoney
        //              });
        //          }
        //          #endregion

        //          prizeOrder.OrderId = runningOrder.OrderId;
        //          prizeOrder.PreTaxBonusMoney = totalBeforeTaxBonusMoeny;
        //          prizeOrder.AfterTaxBonusMoney = totalAfterTaxBonusMoeny;
        //          prizeOrder.PrizeTicketListInfo = prizeTicketList;

        //          runningOrder.BonusTime = DateTime.Now;
        //          runningOrder.BonusMoneyBeforeTax = totalBeforeTaxBonusMoeny;
        //          runningOrder.BonusMoneyAfterTax = totalAfterTaxBonusMoeny;
        //          runningOrder.BonusStatus = totalAfterTaxBonusMoeny > 0M ? BonusStatus.Win : BonusStatus.Lose;
        //          runningOrder.BonusCount = totalBonusCount;
        //          isBonus = totalAfterTaxBonusMoeny > 0M;
        //          var prizedOrder = new JCZQ_Order_Prized();
        //          prizedOrder.LoadByRunning(runningOrder);
        //          //prizedOrder.IsHandle = totalAfterTaxBonusMoeny >= prizedMaxMoney ? false : true;
        //          prizedOrder.IsHandle = (totalAfterTaxBonusMoeny < prizedMaxMoney || runningOrder.TicketGateway == "LOCAL") ? true : false;
        //          orderManager.AddPrizedOrder(prizedOrder);
        //          orderManager.DeleteRunningOrder(runningOrder);

        //          var orderAllLManager = new Common_OrderManager();
        //          var orderALEntity = orderAllLManager.GetRunningOrder(runningOrder.OrderId);
        //          orderALEntity.PrizedTime = DateTime.Now;
        //          orderALEntity.BonusMoneyPrevTax = totalBeforeTaxBonusMoeny;
        //          orderALEntity.BonusMoneyAfterTax = totalAfterTaxBonusMoeny;
        //          orderAllLManager.UpdateCommon_OrderAllList(orderALEntity);
        //          #endregion

        //          return isBonus;
        //      }

        //      private void ComputeJCZQTicketBonus(string orderId, string gameCode, string gameType, string betType, string locBetContent, string locOdds, int betAmount, IList<MatchInfo> matchResultList, JCZQ_MatchManager matchManager, decimal betMoney,
        //         out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        //      {
        //          bonusCount = 0;
        //          preTaxBonusMoney = 0M;
        //          afterTaxBonusMoney = 0M;
        //          #region odd 组合生成
        //          //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
        //          var locOddsL = locOdds.Split('/');
        //          var codeList = new List<JCZQ_AnteCode_Running>();
        //          var collection = new TicketBiz.Core.TicketGateway.GatewayAnteCodeCollection_Sport();
        //          var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
        //          foreach (var item in locBetContent.Split('/'))
        //          {
        //              var oneMatch = item.Split('_');
        //              if (gameType == "HH")
        //              {
        //                  var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
        //                  codeList.Add(new JCZQ_AnteCode_Running
        //                  {
        //                      MatchId = oneMatch[1],
        //                      AnteNumber = oneMatch[2],
        //                      IsDan = false,
        //                      GameType = oneMatch[0],
        //                      Odds = SP[0][1].ToString()
        //                  });
        //                  collection.Add(new TicketBiz.Core.TicketGateway.GatewayAnteCode_Sport
        //                  {
        //                      AnteCode = oneMatch[2],
        //                      MatchId = oneMatch[1],
        //                      GameType = oneMatch[0],
        //                      IsDan = false
        //                  });
        //              }
        //              else
        //              {
        //                  var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
        //                  codeList.Add(new JCZQ_AnteCode_Running
        //                  {
        //                      MatchId = oneMatch[0],
        //                      AnteNumber = oneMatch[1],
        //                      IsDan = false,
        //                      GameType = gameType,
        //                      Odds = SP[0][1].ToString()
        //                  });
        //                  collection.Add(new TicketBiz.Core.TicketGateway.GatewayAnteCode_Sport
        //                  {
        //                      AnteCode = oneMatch[1],
        //                      MatchId = oneMatch[0],
        //                      GameType = gameType,
        //                      IsDan = false
        //                  });
        //              }
        //          }
        //          #endregion

        //          var n = int.Parse(betType.Replace("P", "").Split('_')[1]);
        //          if (n > 1)
        //          {
        //              #region M串N
        //              var orderEntity = new GatewayHandler_LOCAL().AnalyzeOrder_Sport_Prize<BJDC_Order_Running, BJDC_Ticket_Running, BJDC_AnteCode_Running>(new TicketBiz.Core.TicketGateway.GatewayTicketOrder_Sport
        //              {
        //                  Amount = betAmount,
        //                  AnteCodeList = collection,
        //                  Attach = string.Empty,
        //                  GameCode = gameCode,
        //                  GameType = gameType,
        //                  IssuseNumber = string.Empty,
        //                  IsVirtualOrder = false,
        //                  OrderId = orderId,
        //                  PlayType = betType.Replace("P", ""),
        //                  Price = 2,
        //                  UserId = string.Empty,
        //                  TotalMoney = betMoney
        //              }, "LOCAL", "agentId");

        //              foreach (var ticket in orderEntity.GetTicketList())
        //              {
        //                  var matchIdL = (from c in ticket.GetAnteCodeList() select c.MatchId).ToArray();
        //                  var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

        //                  var baseCount = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]);
        //                  var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, baseCount);
        //                  var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), matchResultList.ToArray());
        //                  if (bonusResult.IsWin)
        //                  {
        //                      bonusCount = bonusResult.BonusCount;
        //                      for (var i = 0; i < bonusResult.BonusCount; i++)
        //                      {
        //                          var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                          var tmpAnteList = codeL.Where(a => matchIdList.Contains(a.MatchId));
        //                          var oneBeforeBonusMoney = 2M;
        //                          foreach (var item in tmpAnteList)
        //                          {
        //                              var odds = 1M;
        //                              var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
        //                              odds = item.GetResultOdds(result.GetMatchResult(gameCode, item.GameType));
        //                              oneBeforeBonusMoney *= odds;
        //                          }
        //                          oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
        //                          var oneAfterBonusMoney = oneBeforeBonusMoney;
        //                          if (oneBeforeBonusMoney >= _taxBaseMoney_Sport)
        //                          {
        //                              oneAfterBonusMoney = oneBeforeBonusMoney * (1M - _taxRatio_Sport);
        //                          }

        //                          oneBeforeBonusMoney *= ticket.Amount;
        //                          oneAfterBonusMoney *= ticket.Amount;

        //                          preTaxBonusMoney += oneBeforeBonusMoney;
        //                          afterTaxBonusMoney += oneAfterBonusMoney;
        //                      }
        //                  }
        //              }
        //              #endregion
        //          }
        //          else
        //          {
        //              #region M串1
        //              var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
        //              var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
        //              var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
        //              var record = string.Empty;
        //              if (bonusResult.IsWin)
        //              {
        //                  //1
        //                  record += "1-" + bonusResult.IsWin;
        //                  //2
        //                  record += ",2-" + bonusResult.BonusCount;
        //                  bonusCount = bonusResult.BonusCount;
        //                  for (var i = 0; i < bonusResult.BonusCount; i++)
        //                  {
        //                      var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
        //                      //3
        //                      record += ",3-" + string.Join("^", matchIdList);
        //                      var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
        //                      var oneBeforeBonusMoney = 2M;
        //                      foreach (var item in tmpAnteList)
        //                      {
        //                          var odds = 1M;

        //                          var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
        //                          var matchResult = result.GetMatchResult(gameCode, item.GameType);
        //                          odds = item.GetResultOdds(matchResult);
        //                          //}
        //                          //4
        //                          record += ",4-" + odds;
        //                          if (baseCount == 1 && matchResult == "-1")
        //                          {
        //                              preTaxBonusMoney += betMoney;
        //                              afterTaxBonusMoney += betMoney;
        //                              return;
        //                          }
        //                          else
        //                          {
        //                              oneBeforeBonusMoney *= odds;
        //                          }
        //                          //5
        //                          record += ",5-" + oneBeforeBonusMoney;
        //                      }
        //                      //6
        //                      record += ",6-" + oneBeforeBonusMoney;
        //                      oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
        //                      var oneAfterBonusMoney = oneBeforeBonusMoney;
        //                      if (oneBeforeBonusMoney >= _taxBaseMoney_Sport)
        //                      {
        //                          oneAfterBonusMoney = oneBeforeBonusMoney * (1M - _taxRatio_Sport);
        //                      }
        //                      record += ",7-" + betAmount;
        //                      oneBeforeBonusMoney *= betAmount;
        //                      record += ",8-" + oneBeforeBonusMoney;
        //                      oneAfterBonusMoney *= betAmount;
        //                      record += ",9-" + oneAfterBonusMoney;

        //                      preTaxBonusMoney += oneBeforeBonusMoney;
        //                      record += ",10-" + preTaxBonusMoney;
        //                      afterTaxBonusMoney += oneAfterBonusMoney;
        //                      record += ",11-" + afterTaxBonusMoney;
        //                  }
        //              }

        //              #endregion
        //          }
        //      }

        //      #region 接受成功票号保存
        //      private void GetrderInfoList(string content, string fileName)
        //      {
        //          var fileFullName = BuildFileFullName(fileName);
        //          File.AppendAllText(fileFullName, content, Encoding.UTF8);
        //      }
        //      /// <summary>
        //      /// 创建文件全路径
        //      /// </summary>
        //      private string BuildFileFullName(string fileName)
        //      {
        //          var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrizedJCZQ", DateTime.Now.ToString("yyyyMMddHH"));
        //          //var path = Path.Combine("E:\\XT_Lottery\\trunk\\90.Apps\\app.lottery.wcf.ticketN", "TicketMachine", DateTime.Now.ToString("yyyyMMddHH"));
        //          if (!Directory.Exists(path))
        //              Directory.CreateDirectory(path);
        //          return Path.Combine(path, fileName);
        //      }
        //      #endregion

        //      public void UpdateMatchPrized_JCZQ(string gameCode, string matchId)
        //      {
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();

        //              var issuseManager = new JCZQ_MatchManager();
        //              var entity = issuseManager.GetMatch(gameCode, matchId);
        //              if (entity == null)
        //              {
        //                  throw new ArgumentException(string.Format("比赛{0}-{1}不存在", gameCode, matchId));
        //              }
        //              entity.IsPrized = true;
        //              entity.PrizeTime = DateTime.Now;
        //              issuseManager.UpdateMatch(entity);

        //              tran.CommitTran();
        //          }
        //      }
        //      public JCZQ_MatchQueryCollection QueryJCZQ_MatchInfoList(string gameCode, string matchDate)
        //      {
        //          var collection = new JCZQ_MatchQueryCollection();
        //          var list = new JCZQ_MatchManager().QueryMatchDateMatchInfoList(gameCode, matchDate);
        //          foreach (var item in list)
        //          {
        //              collection.Add(item);
        //          }
        //          return collection;
        //      }
        //      #region 自动服务派奖相关
        //      public string QueryPrizeOrder_JCZQ()
        //      {
        //          var list = new List<string>();
        //          using (var tran = new TicketBusinessManagement())
        //          {
        //              tran.BeginTran();

        //              var manager = new JCZQ_OrderManager();
        //              var orderList = manager.QueryRunningOrderList();
        //              if (orderList == null)
        //                  throw new Exception("没有查询到进行中的订单。");
        //              foreach (var item in orderList)
        //              {
        //                  //var anteCodeList = manager.QueryRunningAnteCodeList(item.OrderId);
        //                  //if (anteCodeList == null)
        //                  //    throw new Exception(string.Format("该订单{0}下面没有找到投注内容数据。", item.OrderId));
        //                  //if (IsPrizeOrder_JCZQ(anteCodeList))
        //                  list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //              }

        //              tran.CommitTran();
        //          }
        //          return string.Join("_", list);
        //      }
        //      public MatchInfoCollection IsPrizeOrder_JCZQ(string orderId, string playType)
        //      {
        //          var collection = new MatchInfoCollection();
        //          var manager = new JCZQ_OrderManager();
        //          var anteCodeList = manager.QueryRunningAnteCodeList(orderId);
        //          if (anteCodeList == null)
        //              throw new Exception(string.Format("该订单{0}下面没有找到投注内容数据。", orderId));
        //          var matchList = new List<string>();
        //          var matchManager = new JCZQ_MatchManager();
        //          foreach (var item in anteCodeList)
        //          {
        //              var match = matchManager.GetMatch(item.GameCode, item.MatchId);
        //              if (match == null)
        //                  throw new Exception("没有找到对应比赛。");
        //              if (match.MatchState != MatchState.Oddsed)
        //                  return new MatchInfoCollection();
        //              //var result = QueryCanAwardOrderListByMatch_JCZQ("JCZQ", item.MatchId);
        //              //var result=
        //              if (matchList.Contains(item.MatchId))
        //                  continue;
        //              matchList.Add(item.MatchId);
        //              if (playType == "1c1")
        //              {
        //                  if (match.MatchState != MatchState.Oddsed)
        //                      return new MatchInfoCollection();
        //              }
        //              collection.Add(new MatchInfo
        //              {
        //                  GameCode = match.GameCode,
        //                  MatchId = match.MatchId,
        //                  MatchIdName = match.MatchIdName,
        //                  MatchNumber = match.MatchNumber,
        //                  MatchDate = match.MatchDate,
        //                  LeagueId = match.LeagueId,
        //                  LeagueName = match.LeagueName,
        //                  HomeTeamId = match.HomeTeamId,
        //                  HomeTeamName = match.HomeTeamName,
        //                  HomeTeamRankName = match.HomeTeamRankName,
        //                  GuestTeamId = match.GuestTeamId,
        //                  GuestTeamName = match.GuestTeamName,
        //                  GuestTeamRankName = match.GuestTeamRankName,
        //                  LetBall = match.LetBall,
        //                  MatchStartTime = match.MatchStartTime,
        //                  MatchState = match.MatchState,
        //                  HomeTeamHalfScore = match.HomeTeamHalfScore,
        //                  HomeTeamScore = match.HomeTeamScore,
        //                  GuestTeamHalfScore = match.GuestTeamHalfScore,
        //                  GuestTeamScore = match.GuestTeamScore,
        //                  SPF_Result = match.SPF_Result,
        //                  BRQSPF_Result = match.BRQSPF_Result,
        //                  ZJQ_Result = match.ZJQ_Result,
        //                  BF_Result = match.BF_Result,
        //                  BQC_Result = match.BQC_Result,
        //                  BettingStopTime = match.FSBettingStopTime,
        //              });
        //          }
        //          return collection;
        //      }

        //      public string QueryPrizeOrder_JCZQ(string orderId)
        //      {
        //          var list = new List<string>();
        //          var manager = new JCZQ_OrderManager();
        //          var order = manager.QueryRunningOrderList_OrderId(orderId);
        //          if (order == null)
        //              throw new Exception("没有查询到进行中的订单。");
        //          //list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //          return string.Format("{0}|{1}|{2}", order.OrderId, order.AgentId, order.PlayType.Replace("_", "c"));
        //      }

        //      public string ManualPrizeOrder_JCZQ(string orderId)
        //      {
        //          var list = new List<string>();
        //          var manager = new JCZQ_OrderManager();
        //          var order = manager.QueryRunningOrderList_OrderId(orderId);
        //          if (order == null)
        //              order = HandlePrizeOrder_JCZQ(orderId);
        //          if (order == null)
        //              throw new Exception("没有查询到进行中的订单。");
        //          //list.Add(item.OrderId + "|" + item.AgentId + "|" + item.PlayType.Replace("_", "c"));
        //          return string.Format("{0}|{1}|{2}", order.OrderId, order.AgentId, order.PlayType.Replace("_", "c"));
        //      }


        //      public void PrizeOrder_JCZQ()
        //      {
        //          var manager = new JCZQ_OrderManager();
        //          var matchIdList = manager.QueryDoPrizeOrderToMatchIdList();
        //          foreach (var matchId in matchIdList)
        //          {
        //              try
        //              {
        //                  PrizeOrder_JCZQ_MatchId(matchId);
        //              }
        //              catch (Exception)
        //              {
        //              }
        //          }
        //      }
        //      #endregion

        //      #region 根据比赛派奖

        //      public void PrizeOrder_JCZQ_MatchId(string matchId)
        //      {
        //          var result = QueryCanAwardOrderListByMatch_JCZQ(matchId);
        //          foreach (var item in result.OrderList)
        //          {
        //              try
        //              {
        //                  PrizeOrder_JCZQ(item.AgentId, item.OrderId, result.MatchList, _prizedMaxMoney);
        //              }
        //              catch (Exception)
        //              {
        //              }
        //          }
        //      }

        //      #endregion

        public void UpdateOddsList_JCZQ<TInfo, TEntity>(string gameCode, string gameType, string[] matchIdList, bool isDS)
            where TInfo : JingCaiMatchBase, I_JingCai_Odds
            where TEntity : JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, isDS ? "_DS" : string.Empty);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();
                var oddsManager = new JCZQ_OddsManager();
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
                    //todo:
                    var entity = oddsManager.GetLastOdds<TEntity>(gameType, id, isDS);
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
        public void UpdateOddsList_JCZQ_Manual()
        {
            UpdateOddsList_JCZQ_Manual<JCZQ_SPF_SPInfo, JCZQ_Odds_SPF>("JCZQ", "SPF");
            UpdateOddsList_JCZQ_Manual<JCZQ_BRQSPF_SPInfo, JCZQ_Odds_BRQSPF>("JCZQ", "BRQSPF");
            UpdateOddsList_JCZQ_Manual<JCZQ_ZJQ_SPInfo, JCZQ_Odds_ZJQ>("JCZQ", "ZJQ");
            UpdateOddsList_JCZQ_Manual<JCZQ_BQC_SPInfo, JCZQ_Odds_BQC>("JCZQ", "BQC");
            UpdateOddsList_JCZQ_Manual<JCZQ_BF_SPInfo, JCZQ_Odds_BF>("JCZQ", "BF");
        }

        public void UpdateOddsList_JCZQ_Manual<TInfo, TEntity>(string gameCode, string gameType)
            where TInfo : JingCaiMatchBase, I_JingCai_Odds
            where TEntity : JingCai_Odds, new()
        {
            var oddsList = GetOddsList_JingCai<TInfo>(gameCode, gameType, string.Empty);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();
                var oddsManager = new JCZQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<TEntity>(gameType, odds.MatchId, false);
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


        public string PrizeJCZQTicket(int num)
        {
            var writer = Common.Log.LogWriterGetter.GetLogWriter();

            var successCount = 0;
            var failCount = 0;
            var log = new List<string>();

            try
            {
                var manager = new Sports_Manager();
                //var watch = new Stopwatch();
                //watch.Start();
                var collection = manager.QueryPrizeTicketList("JCZQ", num);
                //watch.Stop();
                //writer.Write("PrizeJCZQTicket", "PrizeJCZQTicket", Common.Log.LogType.Information, "执行查询", "用时 " + watch.Elapsed.TotalMilliseconds + " 毫秒");


                //watch.Restart();
                //var ticketStrSql = string.Empty;
                var prizeList = new List<TicketBatchPrizeInfo>();
                foreach (var ticket in collection.TicketList)
                {
                    if (ticket.TicketStatus != TicketStatus.Ticketed) continue;

                    var preTaxBonusMoney = 0M;
                    var afterTaxBonusMoney = 0M;
                    var bonusCount = 0;

                    try
                    {
                        //watch.Restart();

                        ComputeJCZQTicketBonus(ticket.SchemeId, ticket.GameCode, ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, collection.MatchList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);

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
                        //              , preTaxBonusMoney, afterTaxBonusMoney, afterTaxBonusMoney > 0M ? Convert.ToInt32(BonusStatus.Win) : Convert.ToInt32(BonusStatus.Lose), ticket.TicketId, Environment.NewLine);

                        //manager.ExecSql(ticketStrSql);
                        //watch.Stop();
                        //writer.Write("PrizeJCZQTicket", "PrizeJCZQTicket", Common.Log.LogType.Information, "执行票数据更新", "用时 " + watch.Elapsed.TotalMilliseconds + " 毫秒");

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
                //watch.Stop();

                //writer.Write("PrizeJCZQTicket", "PrizeJCZQTicket", Common.Log.LogType.Information, "票奖金计算完成", "用时 " + watch.Elapsed.TotalMilliseconds + " 毫秒");

                log.Insert(0, string.Format("总查询到{0}张票,成功派奖票：{1}条，失败派奖票：{2}条", collection.TicketList.Count, successCount, failCount));
            }
            catch (Exception ex)
            {
                return "派奖票数据出错 - " + ex.Message;
            }

            return string.Join(Environment.NewLine, log.ToArray());
        }

        public void PrizeJCZQTicket_OrderId(string orderId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
        }

        private void ComputeJCZQTicketBonus(string orderId, string gameCode, string gameType, string betType, string locBetContent, string locOdds, int betAmount, IList<MatchInfo> matchResultList, decimal betMoney,
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

        public List<string> RequestTicket_JCZQSingleScheme(GatewayTicketOrder_SingleScheme order, out List<string> realMatchIdArray)
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
