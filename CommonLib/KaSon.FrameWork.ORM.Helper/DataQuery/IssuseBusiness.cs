﻿using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Net;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using MongoDB.Driver;

namespace KaSon.FrameWork.ORM.Helper
{
    public class IssuseBusiness:DBbase
    {
        private static string _baseDir;
        public void SetMatchConfigBaseDir(string dir)
        {
            _baseDir = dir;
        }
        public C_Game_Issuse QueryWinNumberByIssuseNumber(string gameCode, string gameType, string issuseNumber)
        {
            var query = from b in DB.CreateQuery<C_Game_Issuse>()
                        where b.GameCode == gameCode
                        && (gameType == null || gameType == "" || b.GameType == gameType)
                        && b.IssuseNumber == issuseNumber
                        select b;
            return query.FirstOrDefault();
        }

        public WinNumber_QueryInfoCollection QueryWinNumber(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var gameType = string.Empty;
            if (gameCode.IndexOf("_") >= 0)
            {
                var array = gameCode.Split('_');
                gameCode = array[0].ToUpper();
                gameType = array[1].ToUpper();
            }
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from i in DB.CreateQuery<C_Game_Issuse>()
                        join g in DB.CreateQuery<C_Lottery_Game>() on i.GameCode equals g.GameCode
                        orderby i.IssuseNumber descending
                        where i.GameCode == gameCode && (gameType == string.Empty || i.GameType == gameType) && i.WinNumber != string.Empty && i.WinNumber != null
                        && i.AwardTime >= startTime && i.AwardTime < endTime
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = i.AwardTime,
                            GameCode = g.GameCode,
                            DisplayName = g.DisplayName,
                            IssuseNumber = i.IssuseNumber,
                            WinNumber = i.WinNumber,
                            GameType = i.GameType,
                        };
            var Result = new WinNumber_QueryInfoCollection();
            Result.TotalCount = query.Count();
            Result.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return Result;
        }

        public WinNumber_QueryInfoCollection QueryWinNumber(string gameCode, int count)
        {
            var query = from i in DB.CreateQuery<C_Game_Issuse>()
                        join g in DB.CreateQuery<C_Lottery_Game>() on i.GameCode equals g.GameCode
                        orderby i.IssuseNumber descending
                        where i.GameCode == gameCode && i.WinNumber != string.Empty && i.WinNumber != null
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = i.AwardTime,
                            GameCode = g.GameCode,
                            DisplayName = g.DisplayName,
                            IssuseNumber = i.IssuseNumber,
                            WinNumber = i.WinNumber,
                        };
            var Result = new WinNumber_QueryInfoCollection();
            Result.List= query.Take(count).ToList();
            return Result;
        }

        public JCZQMatchResult_Collection QueryJCZQMatchResult(DateTime time)
        {
            var result = new JCZQMatchResult_Collection();
            var manager = new JCZQMatchManager();
            result.MatchResultList = manager.QueryJCZQMatchResult(time);
            return result;
        }

        public JCLQMatchResult_Collection QueryJCLQMatchResult(DateTime time)
        {
            var result = new JCLQMatchResult_Collection();
            var manager = new JCLQMatchManager();
            result.MatchResultList = manager.QueryJCLQMatchResult(time);
            return result;
        }

        public string QueryBJDCLastIssuseNumber(int count)
        {
            var manager = new BJDCMatchManager();
            var array = manager.QueryBJDCLastIssuseNumber(count);
            return string.Join("|", array);
        }

        public BJDCMatchResultInfo_Collection QueryBJDC_MatchResultList(string issuseNumber)
        {
            var manager = new BJDCMatchManager();
            {
                BJDCMatchResultInfo_Collection collection = new BJDCMatchResultInfo_Collection();
                collection.ListInfo = manager.QueryBJDC_MatchResultListByissuseNumber(issuseNumber);
                return collection;
            }
        }



        public void ManualUpdate_BJDC_MatchList(string issuseNumber)
        {
            List<C_BJDC_Match> matchInfoList = new List<C_BJDC_Match>();

            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                matchInfoList = LoadBJDCMatchList_Mg(issuseNumber);
            }
            else
            {
                matchInfoList = LoadBJDCMatchList(issuseNumber);
            }
           

            var matchIdArray = matchInfoList.Select(p => p.MatchOrderId.ToString()).ToArray();
            UpdateBJDCMatch(issuseNumber, matchIdArray, matchInfoList);
            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentBJDCMatch();
        }
        public void ManualUpdate_BJDC_MatchResultList(string issuseNumber)
        {
            var matchResultList = LoadBJDCMatchResultList(issuseNumber);
            var matchIdArray = matchResultList.Select(p => p.MatchOrderId.ToString()).ToArray();
             Update_BJDC_MatchResultList(issuseNumber, matchIdArray);
        }
        public void ManualUpdate_JCZQ_MatchList()
        {
            var matchInfoList = LoadJCZQMatchList();
            var matchIdArray = matchInfoList.Select(p => p.MatchId).ToArray();
            if (matchIdArray != null && matchIdArray.Count() > 0)
            {
                UpdateJCZQMatch(matchIdArray, matchInfoList);
            }
            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCZQMatch();
        }
        public void ManualUpdate_JCLQ_MatchList()
        {
            var matchInfoList = LoadJCLQMatchList();
            var matchIdArray = matchInfoList.Select(p => p.MatchId).ToArray();
            if (matchIdArray != null && matchIdArray.Count() > 0)
            {
                UpdateJCLQMatch(matchIdArray, matchInfoList);
            }
            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCLQMatch();
        }
        public void ManualUpdate_CTZQ_MatchList(string gameCode, string issuseNumber)
        {
            var list = LoadCTZQMatchList(issuseNumber, gameCode);
            var matchIdArray = list.Select(p => p.Id).ToArray();
            if (matchIdArray != null && matchIdArray.Count() > 0)
            {
                UpdateCTZQMatchList(matchIdArray, list);
            }
        }
#region 私有方法
        private List<C_BJDC_Match> LoadBJDCMatchList(string issuseNumber)
        {
            var fileName = string.Format(@"{2}\{0}\{1}\Match_List.json", "BJDC", issuseNumber, _baseDir);
            var json = ReadFileString(fileName);
            if (string.IsNullOrEmpty(json))
                return new List<C_BJDC_Match>();
            return JsonSerializer.Deserialize<List<C_BJDC_Match>>(json);
        }
        private List<C_BJDC_Match> LoadBJDCMatchList_Mg(string issuseNumber)
        {
          return  MgMatchDataHelper.BJDC_Match_List_ByIssuseSales(issuseNumber);
        }

        private void UpdateBJDCMatch(string issuseNumber, string[] matchIdList, List<C_BJDC_Match> matchInfoList)
        {
            //开启事务
                DB.Begin();
            try
            {
                var manager = new BJDCMatchManager();
                var oldList = manager.QueryBJDC_MatchList(issuseNumber);
                var oldResultList = manager.QueryBJDC_MatchResult_PrizeList(issuseNumber);
                foreach (var matchId in matchIdList)
                {
                    string id = string.Format("{0}|{1}", issuseNumber, matchId);
                    var old = oldList.FirstOrDefault(p => p.Id == id);
                    var oldResult = oldResultList.FirstOrDefault(p => p.Id == id);
                    var current = matchInfoList.FirstOrDefault(p => p.Id == id);
                    if (current == null)
                        continue;
                    //重新添加
                    if (old == null)
                    {
                        manager.AddBJDC_Match(new C_BJDC_Match
                        {
                            CreateTime = current.CreateTime,
                            FlatOdds = current.FlatOdds,
                            GuestTeamName = current.GuestTeamName,
                            GuestTeamSort = current.GuestTeamSort,
                            HomeTeamName = current.HomeTeamName,
                            HomeTeamSort = current.HomeTeamSort,
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            LetBall = current.LetBall,
                            LocalStopTime =current.LocalStopTime,
                            LoseOdds = current.LoseOdds,
                            MatchColor = current.MatchColor,
                            MatchName = current.MatchName,
                            MatchOrderId = current.MatchOrderId,
                            MatchStartTime =current.MatchStartTime,
                            MatchState = (int)current.MatchState,
                            WinOdds = current.WinOdds,
                            MatchId = current.MatchId,
                            HomeTeamId = current.HomeTeamId,
                            GuestTeamId = current.GuestTeamId,
                            Mid = current.Mid,
                        });
                    }
                    else
                    {
                        //更新
                        old.FlatOdds = current.FlatOdds;
                        old.GuestTeamName = current.GuestTeamName;
                        old.GuestTeamSort = current.GuestTeamSort;
                        old.HomeTeamName = current.HomeTeamName;
                        old.HomeTeamSort = current.HomeTeamSort;
                        old.LetBall = current.LetBall;
                        old.LocalStopTime = (current.LocalStopTime);
                        old.LoseOdds = current.LoseOdds;
                        old.MatchColor = current.MatchColor;
                        old.MatchName = current.MatchName;
                        old.MatchStartTime = (current.MatchStartTime);
                        old.MatchState = (int)current.MatchState;
                        old.WinOdds = current.WinOdds;
                        manager.UpdateBJDC_Match(old);
                    }

                    if (oldResult == null)
                    {
                        manager.AddBJDC_MatchResult_Prize(new C_BJDC_MatchResult_Prize
                        {
                            CreateTime = (current.CreateTime),
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            MatchState = "0",
                            MatchOrderId = current.MatchOrderId,
                            HomeFull_Result = "-",
                            HomeHalf_Result = "-",
                            GuestFull_Result = "-",
                            GuestHalf_Result = "-",
                            BQC_Result = "-",
                            BF_Result = "-",
                            SPF_Result = "-",
                            SXDS_Result = "-",
                            ZJQ_Result = "-",
                            BF_SP = 0M,
                            SXDS_SP = 0M,
                            BQC_SP = 0M,
                            SPF_SP = 0M,
                            ZJQ_SP = 0M,
                        });
                    }

                }

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
        private string ReadFileString(string fullUrl)
        {
            try
            {
                string strResult = PostManager.Get(fullUrl, Encoding.UTF8);
                if (strResult == "404") return string.Empty;

                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToLower().StartsWith("var"))
                    {
                        string[] strArray = strResult.Split('=');
                        if (strArray != null && strArray.Length == 2)
                        {
                            if (strArray[1].ToString().Trim().EndsWith(";"))
                            {
                                return strArray[1].ToString().Trim().TrimEnd(';');
                            }
                            return strArray[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        private List<C_BJDC_MatchResult> LoadBJDCMatchResultList(string issuseNumber)
        {
            List<C_BJDC_MatchResult> result = new List<C_BJDC_MatchResult>();
            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                result = MgMatchDataHelper.BJDC_MatchResult_List_ByIssuse(issuseNumber);
            }
            else
            {

                var fileName = string.Format(@"{2}\{0}\{1}\MatchResult_List.json", "BJDC", issuseNumber, _baseDir);
                var json = ReadFileString(fileName);
                if (string.IsNullOrEmpty(json))
                    return result;
                result = JsonSerializer.Deserialize<List<C_BJDC_MatchResult>>(json);
            }






            return result;
        }
     
        private void Update_BJDC_MatchResultList(string issuseNumber, string[] matchResultIdArray)
        {
            var matchResultList = LoadBJDCMatchResultList(issuseNumber);
            //开启事务
            DB.Begin();
            try
            {
                var manager = new BJDCMatchManager();
                var oldList = manager.QueryBJDC_MatchResultList(issuseNumber);
                var oldResultList = manager.QueryBJDC_MatchResult_PrizeList(issuseNumber);
                foreach (var matchResultId in matchResultIdArray)
                {
                    string id = string.Format("{0}|{1}", issuseNumber, matchResultId);
                    var old = oldList.FirstOrDefault(p => p.Id == id);
                    var oldResult = oldResultList.FirstOrDefault(p => p.Id == id);
                    var current = matchResultList.FirstOrDefault(p => p.Id == id);
                    if (current == null)
                        continue;

                    var state = "1";
                    if (current.SPF_SP > 1M && current.SXDS_SP > 1M && current.ZJQ_SP > 1M && current.BF_SP > 1M && current.BQC_SP > 1M && current.MatchState == "Finish")
                        state = "2";
                    //重新添加
                    if (old == null)
                    {
                        manager.AddBJDC_MatchResult(new C_BJDC_MatchResult
                        {
                            CreateTime =(current.CreateTime),
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            SXDS_Result = current.SXDS_Result,
                            SXDS_SP = current.SXDS_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            GuestFull_Result = current.GuestFull_Result,
                            GuestHalf_Result = current.GuestHalf_Result,
                            HomeFull_Result = current.HomeFull_Result,
                            HomeHalf_Result = current.HomeHalf_Result,
                            MatchOrderId = current.MatchOrderId,
                            MatchState = state,
                        });
                    }
                    else
                    {
                        if (old.MatchState != "2")
                        {
                            //更新
                            old.BF_Result = current.BF_Result;
                            old.BF_SP = current.BF_SP;
                            old.BQC_Result = current.BQC_Result;
                            old.BQC_SP = current.BQC_SP;
                            old.GuestFull_Result = current.GuestFull_Result;
                            old.GuestHalf_Result = current.GuestHalf_Result;
                            old.HomeFull_Result = current.HomeFull_Result;
                            old.HomeHalf_Result = current.HomeHalf_Result;
                            old.MatchState = state;
                            old.SPF_Result = current.SPF_Result;
                            old.SPF_SP = current.SPF_SP;
                            old.SXDS_Result = current.SXDS_Result;
                            old.SXDS_SP = current.SXDS_SP;
                            old.ZJQ_Result = current.ZJQ_Result;
                            old.ZJQ_SP = current.ZJQ_SP;
                            manager.UpdateBJDC_MatchResult(old);
                        }
                    }


                    if (oldResult == null)
                    {
                        manager.AddBJDC_MatchResult_Prize(new C_BJDC_MatchResult_Prize
                        {
                            CreateTime = (current.CreateTime),
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            SXDS_Result = current.SXDS_Result,
                            SXDS_SP = current.SXDS_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            GuestFull_Result = current.GuestFull_Result,
                            GuestHalf_Result = current.GuestHalf_Result,
                            HomeFull_Result = current.HomeFull_Result,
                            HomeHalf_Result = current.HomeHalf_Result,
                            MatchOrderId = current.MatchOrderId,
                            MatchState = state,
                        });
                    }
                    else
                    {
                        if (oldResult.MatchState != "2")
                        {
                            //更新
                            oldResult.BF_Result = current.BF_Result;
                            oldResult.BF_SP = current.BF_SP;
                            oldResult.BQC_Result = current.BQC_Result;
                            oldResult.BQC_SP = current.BQC_SP;
                            oldResult.GuestFull_Result = current.GuestFull_Result;
                            oldResult.GuestHalf_Result = current.GuestHalf_Result;
                            oldResult.HomeFull_Result = current.HomeFull_Result;
                            oldResult.HomeHalf_Result = current.HomeHalf_Result;
                            oldResult.MatchState = state;
                            oldResult.SPF_Result = current.SPF_Result;
                            oldResult.SPF_SP = current.SPF_SP;
                            oldResult.SXDS_Result = current.SXDS_Result;
                            oldResult.SXDS_SP = current.SXDS_SP;
                            oldResult.ZJQ_Result = current.ZJQ_Result;
                            oldResult.ZJQ_SP = current.ZJQ_SP;
                            manager.UpdateBJDC_MatchResult_Prize(oldResult);
                        }
                    }

                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
        private List<JCZQ_MatchInfo> LoadJCZQMatchList()
        {
            List<JCZQ_MatchInfo> result = new List<JCZQ_MatchInfo>();
            var fileName = string.Format(@"{1}\{0}\Match_List_FB.json", "JCZQ", _baseDir);


            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                result = MgMatchDataHelper.JCZQ_Match_List_FB();
            }
            else
            {
                var json = ReadFileString(fileName);
                if (string.IsNullOrEmpty(json))
                    return result;
                result = JsonSerializer.Deserialize<List<JCZQ_MatchInfo>>(json);
            }







            return result;
        }
        private void UpdateJCZQMatch(string[] matchIdArray, List<JCZQ_MatchInfo> matchInfoList)
        {
            //开启事务
            DB.Begin();
            try
            {
                var manager = new JCZQMatchManager();
                var oldList = manager.QueryJCZQ_MatchListByMatchId(matchIdArray);
                var oldResultList = manager.QueryJCZQ_MatchResult_PrizeListByMatchId(matchIdArray);
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;

                    if (old == null)
                    {
                        //重新添加
                        //var entity = new JCZQ_Match();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.JCZQ_MatchInfo, JCZQ_Match>(current, ref entity);
                        manager.AddJCZQ_Match(new C_JCZQ_Match
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            StartDateTime = DateTime.Parse(current.StartDateTime),
                            DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime),
                            FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime),
                            WinOdds = current.WinOdds,
                            FlatOdds = current.FlatOdds,
                            LoseOdds = current.LoseOdds,
                            LetBall = current.LetBall,
                            LeagueColor = current.LeagueColor,
                            LeagueId = current.LeagueId,
                            LeagueName = current.LeagueName,
                            HomeTeamId = current.HomeTeamId,
                            HomeTeamName = current.HomeTeamName,
                            GuestTeamId = current.GuestTeamId,
                            GuestTeamName = current.GuestTeamName,
                            PrivilegesType = current.PrivilegesType,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchIdName = current.MatchIdName,
                            MatchNumber = current.MatchNumber,
                            Mid = current.Mid,
                            MatchStopDesc = current.MatchStopDesc,
                        });
                    }
                    else
                    {
                        old.StartDateTime = DateTime.Parse(current.StartDateTime);
                        old.DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime);
                        old.FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime);
                        old.WinOdds = current.WinOdds;
                        old.FlatOdds = current.FlatOdds;
                        old.LoseOdds = current.LoseOdds;
                        old.LetBall = current.LetBall;
                        old.LeagueColor = current.LeagueColor;
                        old.LeagueId = current.LeagueId;
                        old.LeagueName = current.LeagueName;
                        old.HomeTeamId = current.HomeTeamId;
                        old.HomeTeamName = current.HomeTeamName;
                        old.GuestTeamId = current.GuestTeamId;
                        old.GuestTeamName = current.GuestTeamName;
                        //old.PrivilegesType = current.PrivilegesType;
                        old.MatchStopDesc = current.MatchStopDesc;
                        manager.UpdateJCZQ_Match(old);
                    }

                    if (oldResult == null)
                    {
                        manager.AddJCZQ_MatchResult_Prize(new C_JCZQ_MatchResult_Prize
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                            FullGuestTeamScore = 0,
                            FullHomeTeamScore = 0,
                            HalfHomeTeamScore = 0,
                            HalfGuestTeamScore = 0,
                            BF_Result = "-",
                            BQC_Result = "-",
                            BRQSPF_Result = "-",
                            SPF_Result = "-",
                            ZJQ_Result = "-",
                            BF_SP = 0M,
                            BQC_SP = 0M,
                            BRQSPF_SP = 0M,
                            SPF_SP = 0M,
                            ZJQ_SP = 0M,
                            MatchState = "0",
                        });
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
        private List<JCLQ_MatchInfo> LoadJCLQMatchList()
        {

            List<JCLQ_MatchInfo> result = new List<JCLQ_MatchInfo>();
            var fileName = string.Format(@"{1}\{0}\Match_List.json", "JCLQ", _baseDir);

            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                result = MgMatchDataHelper.JCLQ_MatchList(type: "", matchDate: "");
            }
            else
            {
                var json = ReadFileString(fileName);
                if (!string.IsNullOrEmpty(json))
                    result = JsonSerializer.Deserialize<List<JCLQ_MatchInfo>>(json);
           

            }


            return result;

        }
        private void UpdateJCLQMatch(string[] matchIdArray, List<JCLQ_MatchInfo> matchInfoList)
        {
            //开启事务
            DB.Begin();
            try
            {
                var manager = new JCLQMatchManager();
                var oldList = manager.QueryJCLQ_MatchListByMatchId(matchIdArray);
                var oldResultList = manager.QueryJCLQ_MatchResult_PrizeListByMatchId(matchIdArray);
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        manager.AddJCLQ_Match(new C_JCLQ_Match
                        {
                            StartDateTime = DateTime.Parse(current.StartDateTime),
                            DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime),
                            FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime),
                            AverageLose = current.AverageLose,
                            AverageWin = current.AverageWin,
                            GuestTeamName = current.GuestTeamName,
                            HomeTeamName = current.HomeTeamName,
                            MatchState = current.MatchState,
                            PrivilegesType = current.PrivilegesType,
                            CreateTime = DateTime.Parse(current.CreateTime),
                            GuestTeamId = current.GuestTeamId,
                            HomeTeamId = current.HomeTeamId,
                            LeagueColor = current.LeagueColor,
                            LeagueId = current.LeagueId,
                            LeagueName = current.LeagueName,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchIdName = current.MatchIdName,
                            MatchNumber = current.MatchNumber,
                            Mid = current.Mid,
                        });
                    }
                    else
                    {
                        old.StartDateTime = DateTime.Parse(current.StartDateTime);
                        old.DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime);
                        old.FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime);
                        old.AverageLose = current.AverageLose;
                        old.AverageWin = current.AverageWin;
                        old.GuestTeamName = current.GuestTeamName;
                        old.HomeTeamName = current.HomeTeamName;
                        old.MatchState = current.MatchState;
                        //old.PrivilegesType = current.PrivilegesType;
                        manager.UpdateJCLQ_Match(old);
                    }

                    if (oldResult == null)
                    {
                        manager.AddJCLQ_MatchResult_Prize(new C_JCLQ_MatchResult_Prize
                        {
                            MatchState = "0",
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                            GuestScore = 0,
                            HomeScore = 0,
                            SFC_Result = "-",
                            DXF_Result = "-",
                            RFSF_Result = "-",
                            SF_Result = "-",
                            DXF_Trend = "-",
                            RFSF_Trend = "-",
                            DXF_SP = 0M,
                            RFSF_SP = 0M,
                            SF_SP = 0M,
                            SFC_SP = 0M,
                        });
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
        private List<CTZQ_MatchInfo> LoadCTZQMatchList(string issuseNumber, string gameType)
        {
            List<CTZQ_MatchInfo> result = new List<CTZQ_MatchInfo>();


            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                result = MgMatchDataHelper.CTZQ_Match_List(gameType, issuseNumber);
            }
            else
            {
                var fileName = string.Format(@"{3}\{0}\{1}\Match_{2}_List.json", "CTZQ", issuseNumber, gameType, _baseDir);
            var json = ReadFileString(fileName);
            if (!string.IsNullOrEmpty(json))
                result = JsonSerializer.Deserialize<List<CTZQ_MatchInfo>>(json);
            }


            return result; ;
        }
        private void UpdateCTZQMatchList(string[] array, List<CTZQ_MatchInfo> list)
        {
            //开启事务
                DB.Begin();
            try
            {
                var manager = new CTZQMatchManager();
                var oldList = manager.QueryCTZQMatchListById(array);
                foreach (var item in array)
                {
                    var old = oldList.FirstOrDefault(p => p.Id == item);
                    var current = list.FirstOrDefault(p => p.Id == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        manager.AddCTZQ_Match(new C_CTZQ_Match
                        {
                            Color = current.Color,
                            GuestTeamHalfScore = current.GuestTeamHalfScore,
                            GuestTeamScore = current.GuestTeamScore,
                            GuestTeamStanding = int.Parse(current.GuestTeamStanding),
                            GuestTeamId = current.GuestTeamId,
                            GuestTeamName = current.GuestTeamName,
                            HomeTeamHalfScore = current.HomeTeamHalfScore,
                            HomeTeamId = current.HomeTeamId,
                            HomeTeamName = current.HomeTeamName,
                            HomeTeamScore = current.HomeTeamScore,
                            HomeTeamStanding = int.Parse(current.HomeTeamStanding),
                            MatchResult = current.MatchResult,
                            MatchStartTime = DateTime.Parse(current.MatchStartTime),
                            MatchState = (int)current.MatchState,
                            UpdateTime = DateTime.Parse(current.UpdateTime),
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            MatchId = current.MatchId,
                            MatchName = current.MatchName,
                            Mid = current.Mid,
                            OrderNumber = current.OrderNumber,
                        });
                        continue;
                    }

                    old.Color = current.Color;
                    old.GuestTeamHalfScore = current.GuestTeamHalfScore;
                    old.GuestTeamScore = current.GuestTeamScore;
                    old.GuestTeamStanding = int.Parse(current.GuestTeamStanding);
                    old.GuestTeamId = current.GuestTeamId;
                    old.GuestTeamName = current.GuestTeamName;
                    old.HomeTeamHalfScore = current.HomeTeamHalfScore;
                    old.HomeTeamId = current.HomeTeamId;
                    old.HomeTeamName = current.HomeTeamName;
                    old.HomeTeamScore = current.HomeTeamScore;
                    old.HomeTeamStanding = int.Parse(current.HomeTeamStanding);
                    old.MatchResult = current.MatchResult;
                    old.MatchStartTime = DateTime.Parse(current.MatchStartTime);
                    old.MatchState = (int)current.MatchState;
                    old.UpdateTime = DateTime.Parse(current.UpdateTime);
                    manager.UpdateCTZQ_Match(old);
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                DB.Dispose();
                throw new Exception("操作失败" + "●" + ex.Message, ex);
            }
        }
#endregion
        public CoreJCZQMatchInfoCollection QueryCurrentJCZQMatchInfo()
        {
            var collection = new CoreJCZQMatchInfoCollection();
            var manager = new JCZQMatchManager();
            collection.AddRange(manager.QueryCurrentJCZQMatchInfo());
            return collection;
        }
        public CoreJCLQMatchInfoCollection QueryCurrentJCLQMatchInfo()
        {
            var collection = new CoreJCLQMatchInfoCollection();
            var manager = new JCLQMatchManager();
            collection.AddRange(manager.QueryCurrentJCLQMatchInfo());
            return collection;
        }
        public CoreBJDCMatchInfoCollection QueryCurrentBJDCMatchInfo()
        {
            var collection = new CoreBJDCMatchInfoCollection();
            var manager = new BJDCMatchManager();
            collection.AddRange(manager.QueryCurrentBJDCMatchInfo());
            return collection;
        }
        public void UpdateJCZQMatchInfo(string matchId, string privilegesType)
        {
            var manager = new JCZQMatchManager();
            var entity = manager.GetJCZQMatch(matchId);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", matchId));
            entity.PrivilegesType = privilegesType;
            manager.UpdateJCZQ_Match(entity);
        }
        public void UpdateJCLQMatchInfo(string matchId, string privilegesType)
        {
            var manager = new JCLQMatchManager();
            var entity = manager.GetJCLQMatch(matchId);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", matchId));
            entity.PrivilegesType = privilegesType;
            manager.UpdateJCLQ_Match(entity);
        }
        public void UpdateBJDCMatchInfo(string Id, string privilegesType)
        {
            var manager = new BJDCMatchManager();
            var entity = manager.GetBJDCMatchById(Id);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", Id));
            entity.PrivilegesType = privilegesType;
            manager.UpdateBJDC_Match(entity);
        }


        /// <summary>
        /// 更新单个订单的过关统计
        /// </summary>
        public void Update_CTZQ_HitCountBySchemeId(string schemeId)
        {
            //暂停业务
            return;

            //var sportsManager = new Sports_Manager();
            //var order = sportsManager.QuerySports_Order_Running(schemeId);
            //if (order == null)
            //    throw new Exception(string.Format("没有找到{0}的Order_Running数据", schemeId));

            //var poolKey = string.Format("{0}|{1}|{2}|{3}", "CTZQ", order.GameType, order.IssuseNumber, "1");
            //var pool = new CTZQMatchManager().QueryCTZQ_MatchPoolByKey(poolKey);
            //if (pool == null)
            //    throw new Exception(string.Format("没有找到Key为{0}的奖池数据", poolKey));
            //if (string.IsNullOrEmpty(pool.MatchResult))
            //    throw new Exception(string.Format("Key为{0}的开奖结果为空", poolKey));
            //var bonusCodeArray = pool.MatchResult.Split(',');

            ////普通投注
            //if (order.SchemeBettingCategory == SchemeBettingCategory.GeneralBetting)
            //{
            //    var code = sportsManager.QueryOneSportsAnteCodeBySchemeId(schemeId);
            //    Update_CTZQ_General_HitCount(order.GameType, order.SchemeId, code.AnteCode, bonusCodeArray);
            //    return;
            //}
            ////单式上传 和 过滤投注
            //var singleCode = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            //Update_CTZQ_Single_HitCount(singleCode, bonusCodeArray);
        }

        // 批量开启高频彩奖期
        public void OpenIssuseBatch_Fast(string gameCode, DateTime date, int bettingOffset, Func<DateTime, bool> checkIsOpenDay, Dictionary<int, double> phases, string issuseFormat, Func<int, DateTime, DateTime> eachIssuseOffsetHander = null, int dayIndex = 0)
        {
            var i = 1;
            var collection = new LocalIssuse_AddInfoCollection();
            var offset = date.Date;
            foreach (var phase in phases)
            {
                if (phase.Key <= 0)
                {
                    offset = offset.AddMinutes(phase.Value);
                    continue;
                }
                for (; i <= phase.Key; i++)
                {
                    if (checkIsOpenDay(date))
                    {
                        if (eachIssuseOffsetHander != null)
                        {
                            offset = eachIssuseOffsetHander(i, offset);
                        }
                        var issuseNumber = string.Format(issuseFormat, date, i, dayIndex);
                        var info = new LocalIssuse_AddInfo
                        {
                            GameCode = gameCode,
                            IssuseNumber = issuseNumber,
                        };
                        info.StartTime = offset;
                        offset = offset.AddMinutes(phase.Value);
                        info.OfficialStopTime = offset;
                        info.BettingStopTime = offset;
                        collection.Add(info);
                    }
                }
            }
            var delay = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.GameDelay." + gameCode.ToUpper()));
            OpenIssuse(collection, delay);
        }
        /// <summary>
        /// 开启奖期
        /// </summary>
        public void OpenIssuse(LocalIssuse_AddInfoCollection list, int localAdvanceSeconds)
        {
            var manager = new LotteryGameManager();
            foreach (var item in list)
            {
                var issuse = manager.QueryGameIssuse(item.GameCode, item.IssuseNumber);
                if (issuse == null)
                {
                    issuse = new C_Game_Issuse
                    {
                        CreateTime = DateTime.Now,
                        GameCode = item.GameCode,
                        GameCode_IssuseNumber = string.Format("{0}|{1}", item.GameCode, item.IssuseNumber),
                        IssuseNumber = item.IssuseNumber,
                        StartTime = item.StartTime,
                        WinNumber = string.Empty,
                        Status = (int)IssuseStatus.OnSale,
                        GatewayStopTime = item.BettingStopTime,
                        OfficialStopTime = item.OfficialStopTime,
                        LocalStopTime = item.BettingStopTime.AddSeconds(-localAdvanceSeconds),
                    };
                    manager.AddGameIssuse(issuse);
                }
                else
                {
                    if (issuse.LocalStopTime != item.BettingStopTime.AddSeconds(-localAdvanceSeconds))
                    {
                        issuse.OfficialStopTime = item.OfficialStopTime;
                        issuse.GatewayStopTime = item.BettingStopTime;
                        issuse.LocalStopTime = item.BettingStopTime.AddSeconds(-localAdvanceSeconds);
                        manager.UpdateGameIssuse(issuse);
                    }
                }
            }
        }

        // 批量开启低频彩以及每日彩奖期，如 双色球、福彩3D
        public void OpenIssuseBatch_Slow(string gameCode, int year, string issuseFormat, Func<DateTime, bool> checkIsOpenDay, Func<DateTime, DateTime> getOfficialStopTime, int bettingStopTimeOffsetMinutes)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
            var i = 1;
            var startTime = startDate;
            var collection = new LocalIssuse_AddInfoCollection();
            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                if (checkIsOpenDay(date))
                {
                    var issuseNumber = string.Format(issuseFormat, date, i++);
                    var info = new LocalIssuse_AddInfo
                    {
                        GameCode = gameCode,
                        IssuseNumber = issuseNumber,
                    };
                    info.StartTime = startTime;
                    info.OfficialStopTime = getOfficialStopTime(date);
                    info.BettingStopTime = info.OfficialStopTime;
                    collection.Add(info);
                    //startTime = info.OfficialStopTime.AddHours(3);
                }
            }
            var delay = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.GameDelay." + gameCode.ToUpper()));
            OpenIssuse(collection, delay);
        }

    }
}
