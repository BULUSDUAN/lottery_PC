using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;
using Common.Utilities;

namespace GameBiz.Domain.Managers
{
    public class JCLQMatchManager : GameBizEntityManagement
    {
        public void AddJCLQ_Match(JCLQ_Match entity)
        {
            this.Add<JCLQ_Match>(entity);
        }
        public void AddJCLQ_MatchResult(JCLQ_MatchResult entity)
        {
            this.Add<JCLQ_MatchResult>(entity);
        }
        public void AddJCLQ_MatchResult_Prize(JCLQ_MatchResult_Prize entity)
        {
            this.Add<JCLQ_MatchResult_Prize>(entity);
        }
        public void UpdateJCLQ_Match(JCLQ_Match entity)
        {
            this.Update<JCLQ_Match>(entity);
        }

        public void UpdateJCLQ_MatchResult(JCLQ_MatchResult entity)
        {
            this.Update<JCLQ_MatchResult>(entity);
        }

        public void UpdateJCLQ_MatchResult_Prize(JCLQ_MatchResult_Prize entity)
        {
            this.Update<JCLQ_MatchResult_Prize>(entity);
        }

        public JCLQ_Match GetJCLQMatch(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }

        public IList<DisableMatchConfigInfo> QueryJCLQ_DisableMatchConfigList()
        {
            this.Session.Clear();
            var query = from c in Session.Query<JCLQ_Match>()
                        where c.StartDateTime > DateTime.Now
                       && c.PrivilegesType != null && c.PrivilegesType != ""
                        select new DisableMatchConfigInfo
                        {
                            MatchId = c.MatchId.ToString(),
                            GameCode = "JCLQ",
                            MatchStartTime = c.StartDateTime,
                            PrivilegesType = c.PrivilegesType,
                            IssuseNumber = ""
                        };
            return query.ToList();
        }

        public List<JCLQ_Match> QueryJCLQ_MatchListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_Match>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<JCLQ_MatchResult> QueryJCLQ_MatchResultListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_MatchResult>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<JCLQ_MatchResult_Prize> QueryJCLQ_MatchResult_PrizeListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_MatchResult_Prize>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }

        public JCLQ_MatchResult QueryJCLQ_MatchResultByMatchId(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCLQ_MatchResult_Prize QueryJCLQ_MatchResult_PrizeByMatchId(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_MatchResult_Prize>().FirstOrDefault(p => p.MatchId == matchId);
        }

        public List<CoreJCLQMatchInfo> QueryCurrentJCLQMatchInfo()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
                        where m.FSStopBettingTime >= DateTime.Now
                        select new CoreJCLQMatchInfo
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
        public List<JCLQMatchResult> QueryJCLQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<JCLQ_MatchResult>()
                        join m in this.Session.Query<JCLQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= startTime && r.CreateTime < endTime.AddDays(1)
                        && (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M)
                        orderby r.MatchId descending
                        select new JCLQMatchResult
                        {
                            MatchId = UsefullHelper.GetDbValue<string>(m.MatchId),
                            MatchIdName = UsefullHelper.GetDbValue<string>(m.MatchIdName),
                            StartTime = UsefullHelper.GetDbValue<DateTime>(m.StartDateTime),
                            LeagueId = UsefullHelper.GetDbValue<int>(m.LeagueId),
                            LeagueName = UsefullHelper.GetDbValue<string>(m.LeagueName),
                            LeagueColor = UsefullHelper.GetDbValue<string>(m.LeagueColor),
                            HomeTeamName = UsefullHelper.GetDbValue<string>(m.HomeTeamName),
                            GuestTeamName = UsefullHelper.GetDbValue<string>(m.GuestTeamName),
                            MatchState = UsefullHelper.GetDbValue<string>(r.MatchState),
                            HomeTeamScore = UsefullHelper.GetDbValue<int>(r.HomeScore),
                            GuestTeamScore = UsefullHelper.GetDbValue<int>(r.GuestScore),

                            SF_Result = UsefullHelper.GetDbValue<string>(r.SF_Result),
                            SF_SP = UsefullHelper.GetDbValue<decimal>(r.SF_SP),
                            RFSF_Result = UsefullHelper.GetDbValue<string>(r.RFSF_Result),
                            RFSF_SP = UsefullHelper.GetDbValue<decimal>(r.RFSF_SP),
                            DXF_Result = UsefullHelper.GetDbValue<string>(r.DXF_Result),
                            DXF_SP = UsefullHelper.GetDbValue<decimal>(r.DXF_SP),
                            SFC_Result = UsefullHelper.GetDbValue<string>(r.SFC_Result),
                            SFC_SP = UsefullHelper.GetDbValue<decimal>(r.SFC_SP),
                            RFSF_Trend = UsefullHelper.GetDbValue<string>(r.RFSF_Trend),
                            DXF_Trend = UsefullHelper.GetDbValue<string>(r.DXF_Trend),
                            CreateTime = UsefullHelper.GetDbValue<DateTime>(r.CreateTime),
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<JCLQMatchResult> QueryJCLQMatchResult(DateTime time)
        {
            Session.Clear();
            var query = from r in this.Session.Query<JCLQ_MatchResult>()
                        join m in this.Session.Query<JCLQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= time && r.CreateTime < time.AddDays(1)
                        && (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M)
                        orderby r.MatchId descending
                        select new JCLQMatchResult
                        {
                            MatchId = UsefullHelper.GetDbValue<string>(m.MatchId),
                            MatchIdName = UsefullHelper.GetDbValue<string>(m.MatchIdName),
                            StartTime = UsefullHelper.GetDbValue<DateTime>(m.StartDateTime),
                            LeagueId = UsefullHelper.GetDbValue<int>(m.LeagueId),
                            LeagueName = UsefullHelper.GetDbValue<string>(m.LeagueName),
                            LeagueColor = UsefullHelper.GetDbValue<string>(m.LeagueColor),
                            HomeTeamName = UsefullHelper.GetDbValue<string>(m.HomeTeamName),
                            GuestTeamName = UsefullHelper.GetDbValue<string>(m.GuestTeamName),
                            MatchState = UsefullHelper.GetDbValue<string>(r.MatchState),
                            HomeTeamScore = UsefullHelper.GetDbValue<int>(r.HomeScore),
                            GuestTeamScore = UsefullHelper.GetDbValue<int>(r.GuestScore),

                            SF_Result = UsefullHelper.GetDbValue<string>(r.SF_Result),
                            SF_SP = UsefullHelper.GetDbValue<decimal>(r.SF_SP),
                            RFSF_Result = UsefullHelper.GetDbValue<string>(r.RFSF_Result),
                            RFSF_SP = UsefullHelper.GetDbValue<decimal>(r.RFSF_SP),
                            DXF_Result = UsefullHelper.GetDbValue<string>(r.DXF_Result),
                            DXF_SP = UsefullHelper.GetDbValue<decimal>(r.DXF_SP),
                            SFC_Result = UsefullHelper.GetDbValue<string>(r.SFC_Result),
                            SFC_SP = UsefullHelper.GetDbValue<decimal>(r.SFC_SP),
                            RFSF_Trend = UsefullHelper.GetDbValue<string>(r.RFSF_Trend),
                            DXF_Trend = UsefullHelper.GetDbValue<string>(r.DXF_Trend),
                            CreateTime = UsefullHelper.GetDbValue<DateTime>(r.CreateTime),
                        };
            return query.ToList();
        }

        public List<JCLQ_MatchResult> QueryJCLQMatchResultByDay(int day)
        {
            Session.Clear();
            var query = from r in this.Session.Query<JCLQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M )
                        select r;
            return query.ToList();
        }
    }
}
