using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using Common.Utilities;

namespace GameBiz.Domain.Managers
{
    public class JCZQMatchManager : GameBizEntityManagement
    {
        public void AddJCZQ_Match(JCZQ_Match entity)
        {
            this.Add<JCZQ_Match>(entity);
        }
        public void AddJCZQ_OZBMatch(JCZQ_OZBMatch entity)
        {
            this.Add<JCZQ_OZBMatch>(entity);
        }
        public void AddJCZQ_MatchResult(JCZQ_MatchResult entity)
        {
            this.Add<JCZQ_MatchResult>(entity);
        }
        public void AddJCZQ_MatchResult_Prize(JCZQ_MatchResult_Prize entity)
        {
            this.Add<JCZQ_MatchResult_Prize>(entity);
        }
        public void AddJCZQ_SJBMatch(JCZQ_SJBMatch entity)
        {
            this.Add<JCZQ_SJBMatch>(entity);
        }

        public void UpdateJCZQ_Match(JCZQ_Match entity)
        {
            this.Update<JCZQ_Match>(entity);
        }
        public void UpdateJCZQ_OZBMatch(JCZQ_OZBMatch entity)
        {
            this.Update<JCZQ_OZBMatch>(entity);
        }
        public void UpdateJCZQ_SJBMatch(JCZQ_SJBMatch entity)
        {
            this.Update<JCZQ_SJBMatch>(entity);
        }
       

        public void UpdateJCZQ_MatchResult(JCZQ_MatchResult entity)
        {
            this.Update<JCZQ_MatchResult>(entity);
        }

        public void UpdateJCZQ_MatchResult_Prize(JCZQ_MatchResult_Prize entity)
        {
            this.Update<JCZQ_MatchResult_Prize>(entity);
        }

        public JCZQ_Match GetJCZQMatch(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCZQ_OZBMatch GetJCZQ_OZBMatch(string gameType, string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_OZBMatch>().FirstOrDefault(p => p.MatchId == matchId && p.GameType == gameType);
        }

        public List<JCZQ_OZBMatch> QueryJCZQ_OZBMatchList(string gameType, string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_OZBMatch>().Where(p => matchIdArray.Contains(p.MatchId) && p.GameType == gameType).ToList();
        }

        public IList<DisableMatchConfigInfo> QueryJCZQ_DisableMatchConfigList()
        {
            this.Session.Clear();
            var query = from c in Session.Query<JCZQ_Match>()
                        where c.StartDateTime > DateTime.Now
                        && c.PrivilegesType != null && c.PrivilegesType != ""
                        select new DisableMatchConfigInfo
                        {
                            MatchId = c.MatchId.ToString(),
                            GameCode = "JCZQ",
                            MatchStartTime = c.StartDateTime,
                            PrivilegesType = c.PrivilegesType,
                            IssuseNumber = ""
                        };
            return query.ToList();
        }

        public List<JCZQ_Match> QueryJCZQ_MatchListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_Match>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<JCZQ_MatchResult> QueryJCZQ_MatchResultListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_MatchResult>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<JCZQ_MatchResult_Prize> QueryJCZQ_MatchResult_PrizeListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_MatchResult_Prize>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }

        public JCZQ_MatchResult QueryJCZQ_MatchResultByMatchId(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCZQ_MatchResult_Prize QueryJCZQ_MatchResult_PrizeByMatchId(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_MatchResult_Prize>().FirstOrDefault(p => p.MatchId == matchId);
        }

        public List<CoreJCZQMatchInfo> QueryCurrentJCZQMatchInfo()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
                        where m.FSStopBettingTime >= DateTime.Now
                        select new CoreJCZQMatchInfo
                        {
                            FSStopBettingTime = m.FSStopBettingTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueColor = m.LeagueColor,
                            LeagueName = m.LeagueName,
                            MatchData = m.MatchData,
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            MatchNumber = m.MatchNumber,
                            StartDateTime = m.StartDateTime,
                            PrivilegesType = m.PrivilegesType,
                        };
            return query.ToList();
        }
        public List<JCZQMatchResult> QueryJCZQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<JCZQ_MatchResult>()
                        join m in this.Session.Query<JCZQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= startTime && r.CreateTime < endTime.AddDays(1)
                        && (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        orderby r.MatchId descending
                        select new JCZQMatchResult
                        {
                            MatchId = UsefullHelper.GetDbValue<string>(m.MatchId),
                            MatchIdName = UsefullHelper.GetDbValue<string>(m.MatchIdName),
                            StartTime = UsefullHelper.GetDbValue<DateTime>(m.StartDateTime),
                            LeagueId = UsefullHelper.GetDbValue<int>(m.LeagueId),
                            LeagueName = UsefullHelper.GetDbValue<string>(m.LeagueName),
                            LeagueColor = UsefullHelper.GetDbValue<string>(m.LeagueColor),
                            HomeTeamId = UsefullHelper.GetDbValue<int>(m.HomeTeamId),
                            HomeTeamName = UsefullHelper.GetDbValue<string>(m.HomeTeamName),
                            GuestTeamId = UsefullHelper.GetDbValue<int>(m.GuestTeamId),
                            GuestTeamName = UsefullHelper.GetDbValue<string>(m.GuestTeamName),
                            LetBall = UsefullHelper.GetDbValue<int>(m.LetBall),
                            WinOdds = UsefullHelper.GetDbValue<decimal>(m.WinOdds),
                            FlatOdds = UsefullHelper.GetDbValue<decimal>(m.FlatOdds),
                            LoseOdds = UsefullHelper.GetDbValue<decimal>(m.LoseOdds),
                            MatchState = UsefullHelper.GetDbValue<string>(r.MatchState),
                            HalfHomeTeamScore = UsefullHelper.GetDbValue<int>(r.HalfHomeTeamScore),
                            HalfGuestTeamScore = UsefullHelper.GetDbValue<int>(r.HalfGuestTeamScore),
                            FullHomeTeamScore = UsefullHelper.GetDbValue<int>(r.FullHomeTeamScore),
                            FullGuestTeamScore = UsefullHelper.GetDbValue<int>(r.FullGuestTeamScore),
                            SPF_Result = UsefullHelper.GetDbValue<string>(r.SPF_Result),
                            SPF_SP = UsefullHelper.GetDbValue<decimal>(r.SPF_SP),
                            BRQSPF_Result = UsefullHelper.GetDbValue<string>(r.BRQSPF_Result),
                            BRQSPF_SP = UsefullHelper.GetDbValue<decimal>(r.BRQSPF_SP),
                            ZJQ_Result = UsefullHelper.GetDbValue<string>(r.ZJQ_Result),
                            ZJQ_SP = UsefullHelper.GetDbValue<decimal>(r.ZJQ_SP),
                            BF_Result = UsefullHelper.GetDbValue<string>(r.BF_Result),
                            BF_SP = UsefullHelper.GetDbValue<decimal>(r.BF_SP),
                            BQC_Result = UsefullHelper.GetDbValue<string>(r.BQC_Result),
                            BQC_SP = UsefullHelper.GetDbValue<decimal>(r.BQC_SP),
                            CreateTime = UsefullHelper.GetDbValue<DateTime>(r.CreateTime),
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        public List<JCZQMatchResult> QueryJCZQMatchResult(DateTime time)
        {
            Session.Clear();
            var query = from r in this.Session.Query<JCZQ_MatchResult>()
                        join m in this.Session.Query<JCZQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= time && r.CreateTime < time.AddDays(1)
                        && (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        orderby r.MatchId descending
                        select new JCZQMatchResult
                        {
                            MatchId = UsefullHelper.GetDbValue<string>(m.MatchId),
                            MatchIdName = UsefullHelper.GetDbValue<string>(m.MatchIdName),
                            StartTime = UsefullHelper.GetDbValue<DateTime>(m.StartDateTime),
                            LeagueId = UsefullHelper.GetDbValue<int>(m.LeagueId),
                            LeagueName = UsefullHelper.GetDbValue<string>(m.LeagueName),
                            LeagueColor = UsefullHelper.GetDbValue<string>(m.LeagueColor),
                            HomeTeamId = UsefullHelper.GetDbValue<int>(m.HomeTeamId),
                            HomeTeamName = UsefullHelper.GetDbValue<string>(m.HomeTeamName),
                            GuestTeamId = UsefullHelper.GetDbValue<int>(m.GuestTeamId),
                            GuestTeamName = UsefullHelper.GetDbValue<string>(m.GuestTeamName),
                            LetBall = UsefullHelper.GetDbValue<int>(m.LetBall),
                            WinOdds = UsefullHelper.GetDbValue<decimal>(m.WinOdds),
                            FlatOdds = UsefullHelper.GetDbValue<decimal>(m.FlatOdds),
                            LoseOdds = UsefullHelper.GetDbValue<decimal>(m.LoseOdds),
                            MatchState = UsefullHelper.GetDbValue<string>(r.MatchState),
                            HalfHomeTeamScore = UsefullHelper.GetDbValue<int>(r.HalfHomeTeamScore),
                            HalfGuestTeamScore = UsefullHelper.GetDbValue<int>(r.HalfGuestTeamScore),
                            FullHomeTeamScore = UsefullHelper.GetDbValue<int>(r.FullHomeTeamScore),
                            FullGuestTeamScore = UsefullHelper.GetDbValue<int>(r.FullGuestTeamScore),
                            SPF_Result = UsefullHelper.GetDbValue<string>(r.SPF_Result),
                            SPF_SP = UsefullHelper.GetDbValue<decimal>(r.SPF_SP),
                            BRQSPF_Result = UsefullHelper.GetDbValue<string>(r.BRQSPF_Result),
                            BRQSPF_SP = UsefullHelper.GetDbValue<decimal>(r.BRQSPF_SP),
                            ZJQ_Result = UsefullHelper.GetDbValue<string>(r.ZJQ_Result),
                            ZJQ_SP = UsefullHelper.GetDbValue<decimal>(r.ZJQ_SP),
                            BF_Result = UsefullHelper.GetDbValue<string>(r.BF_Result),
                            BF_SP = UsefullHelper.GetDbValue<decimal>(r.BF_SP),
                            BQC_Result = UsefullHelper.GetDbValue<string>(r.BQC_Result),
                            BQC_SP = UsefullHelper.GetDbValue<decimal>(r.BQC_SP),
                            CreateTime = UsefullHelper.GetDbValue<DateTime>(r.CreateTime),
                        };
            return query.ToList();
        }

        public List<JCZQ_MatchResult> QueryJCZQMatchResultByDay(int day)
        {
            Session.Clear();
            var query = from r in this.Session.Query<JCZQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        select r;
            return query.ToList();
        }



        public JCZQ_SJBMatch GetJCZQ_SJBMatch(string gameType, string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_SJBMatch>().FirstOrDefault(p => p.MatchId == matchId && p.GameType == gameType);
        }

        public List<JCZQ_SJBMatch> QueryJCZQ_SJBMatchList(string gameType, string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_SJBMatch>().Where(p => matchIdArray.Contains(p.MatchId) && p.GameType == gameType).ToList();
        }
    }
}
