using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Lottery;
using Common.JSON;
using GameBiz.Business.Domain.Managers.Ticket;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Core.Ticket;
using GameBiz.Domain.Entities;
using GameBiz.Domain.Managers;

namespace GameBiz.Business
{
    public partial class TicketGatewayAdmin
    {
        //public void AddMatchList_CTZQ(string gameCode, string gameType, string issuseNumber, string[] matchIdList)
        //{
        //    var matchInfoList = GetMatchList_CTZQ(gameCode, gameType, issuseNumber);
        //    var addList = matchInfoList.Where(m => matchIdList.Contains(m.Id));
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var matchManager = new CTZQ_MatchManager();
        //        foreach (var info in addList)
        //        {
        //            var entity = matchManager.GetMatch(gameCode, gameType, issuseNumber, info.OrderNumber);
        //            if (entity != null)
        //            {
        //                continue;
        //            }
        //            entity = new CTZQ_Match
        //            {
        //                Color = info.Color,
        //                GameType = info.GameType,
        //                GuestTeamHalfScore = info.GuestTeamHalfScore,
        //                GuestTeamId = info.GuestTeamId,
        //                GuestTeamName = info.GuestTeamName,
        //                GuestTeamScore = info.GuestTeamScore,
        //                GuestTeamStanding = info.GuestTeamStanding,
        //                HomeTeamHalfScore = info.HomeTeamHalfScore,
        //                HomeTeamId = info.HomeTeamId,
        //                HomeTeamName = info.HomeTeamName,
        //                HomeTeamScore = info.HomeTeamScore,
        //                HomeTeamStanding = info.HomeTeamStanding,
        //                Id = info.Id,
        //                IssuseNumber = info.IssuseNumber,
        //                MatchName = info.MatchName,
        //                MatchResult = info.MatchResult,
        //                MatchStartTime = DateTime.Parse(info.MatchStartTime),
        //                MatchState = info.MatchState,
        //                Mid = info.Mid,
        //                OrderNumber = info.OrderNumber,
        //                UpdateTime = DateTime.Parse(info.UpdateTime),
        //                GameCode = info.GameCode,
        //                MatchId = info.MatchId,
        //            };
        //            matchManager.AddCTZQ_Match(entity);
        //        }
        //        tran.CommitTran();
        //    }
        //}
        //public void AddMatchList_CTZQ_Service(string gameCode, string gameType)
        //{
        //    var issuseInfoList = GetIssuseList_CTZQ(gameCode, gameType);
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        foreach (var issuse in issuseInfoList)
        //        {
        //            var matchInfoList = GetMatchList_CTZQ(gameCode, gameType, issuse.IssuseNumber);

        //            var matchManager = new CTZQ_MatchManager();
        //            foreach (var info in matchInfoList)
        //            {
        //                var entity = matchManager.GetMatch(gameCode, gameType, issuse.IssuseNumber, info.OrderNumber);
        //                if (entity != null)
        //                    continue;
        //                entity = new CTZQ_Match
        //                {
        //                    Color = info.Color,
        //                    GameType = info.GameType,
        //                    GuestTeamHalfScore = info.GuestTeamHalfScore,
        //                    GuestTeamId = info.GuestTeamId,
        //                    GuestTeamName = info.GuestTeamName,
        //                    GuestTeamScore = info.GuestTeamScore,
        //                    GuestTeamStanding = info.GuestTeamStanding,
        //                    HomeTeamHalfScore = info.HomeTeamHalfScore,
        //                    HomeTeamId = info.HomeTeamId,
        //                    HomeTeamName = info.HomeTeamName,
        //                    HomeTeamScore = info.HomeTeamScore,
        //                    HomeTeamStanding = info.HomeTeamStanding,
        //                    Id = info.Id,
        //                    IssuseNumber = info.IssuseNumber,
        //                    MatchName = info.MatchName,
        //                    MatchResult = info.MatchResult,
        //                    MatchStartTime = DateTime.Parse(info.MatchStartTime),
        //                    MatchState = info.MatchState,
        //                    Mid = info.Mid,
        //                    OrderNumber = info.OrderNumber,
        //                    UpdateTime = DateTime.Parse(info.UpdateTime),
        //                    GameCode = info.GameCode,
        //                    MatchId = info.MatchId,
        //                };
        //                matchManager.AddCTZQ_Match(entity);
        //            }
        //        }
        //        tran.CommitTran();
        //    }
        //}
        //public void UpdateMatchList_CTZQ(string gameCode, string gameType, string issuseNumber, string[] matchIdList)
        //{
        //    var matchInfoList = GetMatchList_CTZQ(gameCode, gameType, issuseNumber);
        //    var addList = matchInfoList.Where(m => matchIdList.Contains(m.Id));
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var matchManager = new CTZQ_MatchManager();
        //        foreach (var info in addList)
        //        {
        //            var isAdd = false;
        //            var entity = matchManager.GetMatch(gameCode, gameType, issuseNumber, info.OrderNumber);
        //            if (entity == null)
        //            {
        //                entity = new CTZQ_Match();
        //                isAdd = true;
        //            }
        //            entity.Color = info.Color;
        //            entity.GameType = info.GameType;
        //            entity.GuestTeamHalfScore = info.GuestTeamHalfScore;
        //            entity.GuestTeamId = info.GuestTeamId;
        //            entity.GuestTeamName = info.GuestTeamName;
        //            entity.GuestTeamScore = info.GuestTeamScore;
        //            entity.GuestTeamStanding = info.GuestTeamStanding;
        //            entity.HomeTeamHalfScore = info.HomeTeamHalfScore;
        //            entity.HomeTeamId = info.HomeTeamId;
        //            entity.HomeTeamName = info.HomeTeamName;
        //            entity.HomeTeamScore = info.HomeTeamScore;
        //            entity.HomeTeamStanding = info.HomeTeamStanding;
        //            entity.Id = info.Id;
        //            entity.IssuseNumber = info.IssuseNumber;
        //            entity.MatchName = info.MatchName;
        //            entity.MatchResult = info.MatchResult;
        //            entity.MatchStartTime = DateTime.Parse(info.MatchStartTime);
        //            entity.MatchState = info.MatchState;
        //            entity.Mid = info.Mid;
        //            entity.OrderNumber = info.OrderNumber;
        //            entity.UpdateTime = DateTime.Parse(info.UpdateTime);
        //            entity.GameCode = gameCode;
        //            entity.MatchId = info.MatchId;

        //            if (isAdd)
        //            {
        //                matchManager.AddCTZQ_Match(entity);
        //            }
        //            else
        //            {
        //                matchManager.UpdateMatch(entity);
        //            }
        //        }
        //        tran.CommitTran();
        //    }
        //}
        //// 开启传统足球奖期
        //public void OpenIssuse_CTZQ(string gameCode, string gameType)
        //{
        //    var issuseInfoList = GetIssuseList_CTZQ(gameCode, gameType);
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();

        //        var issuseManager = new Ticket_IssuseManager();
        //        var list = new List<CTZQ_Issuse>();
        //        foreach (var info in issuseInfoList)
        //        {
        //            var entity = issuseManager.GetIssuse_CTZQ(gameType, info.IssuseNumber);
        //            if (entity != null)
        //            {
        //                entity.DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime).AddMinutes(10);
        //                //entity.BettingStopTime = GetBetStopTime_CTZQ(DateTime.Parse(info.StopBettingTime));
        //                entity.BettingStopTime = DateTime.Parse(info.StopBettingTime).AddMinutes(10);
        //                entity.OfficialStopTime = DateTime.Parse(info.OfficialStopTime);
        //                issuseManager.UpdateIssuse_CTZQ(entity);
        //                continue;
        //            }
        //            entity = new CTZQ_Issuse
        //            {
        //                IssuseId = info.Id,
        //                GameCode = gameCode,
        //                GameType = gameType,
        //                IssuseNumber = info.IssuseNumber,
        //                StartTime = DateTime.Parse(info.StartTime),
        //                OfficialStopTime = DateTime.Parse(info.OfficialStopTime),
        //                //BettingStopTime = GetBetStopTime_CTZQ(DateTime.Parse(info.StopBettingTime)),
        //                BettingStopTime = DateTime.Parse(info.StopBettingTime).AddMinutes(10),
        //                DSBettingStopTime = DateTime.Parse(info.DSStopBettingTime).AddMinutes(10),
        //                Status = IssuseStatus.Running,
        //                IssuseDate = DateTime.Today.Year.ToString(),
        //            };
        //            list.Add(entity);
        //        }
        //        issuseManager.AddIssuseList_CTZQ(list);

        //        List<OpenIssuseInfo> issuseList = new List<OpenIssuseInfo>();
        //        foreach (var item in list)
        //        {
        //            issuseList.Add(new OpenIssuseInfo
        //            {
        //                //Lottery_Id = new GatewayHandler_TicketMachine().ConvertGameType(item.GameCode,item.GameType),
        //                Issue = item.IssuseNumber,
        //                Start_Timestamp = item.StartTime,
        //                Print_Start_Timestamp = item.StartTime,
        //                End_Timestamp = item.BettingStopTime,
        //                Sys_End_Timestamp = item.OfficialStopTime,
        //            });
        //        }
        //        var content = JsonSerializer.Serialize(issuseList);

        //        tran.CommitTran();
        //    }
        //}
        //public void UpdateBonusPoolByInput_CTZQ(string gameCode, string gameType, string issuseNumber, CTZQ_BonusPoolCollection bonusPool)
        //{
        //    using (var tran = new TicketBusinessManagement())
        //    {
        //        tran.BeginTran();
        //        var bonusManager = new Ticket_BonusManager();
        //        foreach (var info in bonusPool)
        //        {
        //            string errMsg;
        //            var analyzer = AnalyzerFactory.GetWinNumberAnalyzer(gameCode, gameType);
        //            if (!analyzer.CheckWinNumber(info.MatchResult, out errMsg))
        //            {
        //                throw new FormatException(errMsg);
        //            }
        //            var entity = bonusManager.GetBonusPool(gameCode, gameType, issuseNumber, info.BonusLevel);
        //            if (entity != null)
        //            {
        //                entity.BonusCount = info.BonusCount;
        //                entity.BonusLevelDisplayName = info.BonusLevelDisplayName;
        //                entity.BonusMoney = info.BonusMoney;
        //                entity.WinNumber = info.MatchResult;
        //                entity.CreateTime = DateTime.Parse(info.CreateTime);
        //                bonusManager.UpdateBonusPool(entity);
        //            }
        //            else
        //            {
        //                entity = new Ticket_BonusPool
        //                {
        //                    Id = info.Id,
        //                    GameCode = gameCode,
        //                    GameType = gameType,
        //                    IssuseNumber = issuseNumber,
        //                    BonusLevel = info.BonusLevel,
        //                    BonusCount = info.BonusCount,
        //                    BonusLevelDisplayName = info.BonusLevelDisplayName,
        //                    BonusMoney = info.BonusMoney,
        //                    WinNumber = info.MatchResult,
        //                    CreateTime = DateTime.Parse(info.CreateTime),
        //                };
        //                bonusManager.AddBonusPool(entity);
        //            }
        //        }
        //        tran.CommitTran();
        //    }
        //}
        //private IList<CTZQ_IssuseInfo> GetIssuseList_CTZQ(string gameCode, string gameType)
        //{
        //    var fileName = string.Format(@"{2}\{0}\Match_{1}_Issuse_List.json", gameCode, gameType, _baseDir);
        //    //if (!File.Exists(fileName))
        //    //{
        //    //    throw new ArgumentException("奖期不存在或尚未开启奖期");
        //    //}
        //    var json = ReadFileString(fileName);
        //    var resultList = JsonSerializer.Deserialize<List<CTZQ_IssuseInfo>>(json);
        //    return resultList;
        //}
        //private IList<CTZQ_MatchInfo> GetMatchList_CTZQ(string gameCode, string gameType, string issuseNumber)
        //{
        //    var fileName = string.Format(@"{2}\{0}\{3}\Match_{1}_List.json", gameCode, gameType, _baseDir, issuseNumber);
        //    //if (!File.Exists(fileName))
        //    //{
        //    //    throw new ArgumentException("奖期不存在或尚未开启奖期");
        //    //}
        //    var json = ReadFileString(fileName);
        //    var resultList = JsonSerializer.Deserialize<List<CTZQ_MatchInfo>>(json);
        //    return resultList;
        //}

        //private int _betStopTimeOffsetSeconds_CTZQ = 120;
        //private DateTime GetBetStopTime_CTZQ(DateTime stopTime)
        //{
        //    return stopTime.AddSeconds(-_betStopTimeOffsetSeconds_CTZQ);
        //}
        //public CTZQ_MatchInfoCollection QueryCTZQ_MatchInfoList(string gameCode, string gameType, string issuseNumber)
        //{
        //    var collection = new CTZQ_MatchInfoCollection();
        //    var list = new CTZQ_MatchManager().GetCTZQ_MatchListInfo(gameCode, gameType, issuseNumber);
        //    foreach (var item in list)
        //    {
        //        collection.Add(item);
        //    }
        //    return collection;
        //}

        private IList<CTZQ_BonusPoolInfo> GetBonusPoolList_CTZQ(string gameCode, string gameType, string issuseNumber)
        {
            var fileName = string.Format(@"{3}\{0}\{2}\{0}_{1}_BonusLevel.json", gameCode, gameType, issuseNumber, _baseDir);
            //if (!File.Exists(fileName))
            //{
            //    throw new ArgumentException("奖池不存在或尚未开奖");
            //}
            var json = ReadFileString(fileName);
            var resultList = JsonSerializer.Deserialize<List<CTZQ_BonusPoolInfo>>(json);
            return resultList;
        }

        public string UpdateBonusPool_CTZQ(string gameCode, string gameType, string issuseNumber, out int totalBonusCount)
        {
            var winNumber = "";
            var bonusPoolList = GetBonusPoolList_CTZQ(gameCode, gameType, issuseNumber);
            totalBonusCount = bonusPoolList.Sum(b => b.BonusCount);
            using (var tran = new GameBizBusinessManagement())
            {
                tran.BeginTran();
                var bonusManager = new Ticket_BonusManager();
                foreach (var info in bonusPoolList)
                {
                    //string errMsg;
                    //var analyzer = AnalyzerFactory.GetWinNumberAnalyzer(gameCode, gameType);
                    //if (!analyzer.CheckWinNumber(info.MatchResult, out errMsg))
                    //{
                    //    throw new FormatException(errMsg);
                    //}
                    winNumber = info.MatchResult;
                    var entity = bonusManager.GetBonusPool(gameCode, gameType, issuseNumber, info.BonusLevel);
                    if (entity == null)
                    {
                        entity = new Ticket_BonusPool
                        {
                            Id = info.Id,
                            GameCode = gameCode,
                            GameType = gameType,
                            IssuseNumber = info.IssuseNumber,
                            BonusLevel = info.BonusLevel,
                            BonusCount = info.BonusCount,
                            BonusLevelDisplayName = info.BonusLevelDisplayName,
                            BonusMoney = info.BonusMoney,
                            WinNumber = info.MatchResult,
                            CreateTime = DateTime.Parse(info.CreateTime),
                        };
                        bonusManager.AddBonusPool(entity);
                    }
                    else
                    {
                        entity.BonusCount = info.BonusCount;
                        entity.BonusLevelDisplayName = info.BonusLevelDisplayName;
                        entity.BonusMoney = info.BonusMoney;
                        entity.WinNumber = info.MatchResult;
                        entity.CreateTime = DateTime.Parse(info.CreateTime);
                        bonusManager.UpdateBonusPool(entity);
                    }
                }
                tran.CommitTran();
            }
            return winNumber;
        }

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
                    SelectMatchId = selectMatchId,
                    TotalMoney = order.TotalMoney,
                });
            }

            return ctzqCodeList;
        }


    }
}
