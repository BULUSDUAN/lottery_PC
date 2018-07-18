using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
  

    public class JCLQMatchManager : DBbase
    {
        public void AddJCLQ_Match(C_JCLQ_Match entity)
        {
            DB.GetDal<C_JCLQ_Match>().Add(entity);
        }
        public void AddJCLQ_MatchResult(C_JCLQ_MatchResult entity)
        {
            DB.GetDal<C_JCLQ_MatchResult>().Add(entity);
        }
        public void AddJCLQ_MatchResult_Prize(C_JCLQ_MatchResult_Prize entity)
        {
            DB.GetDal<C_JCLQ_MatchResult_Prize>().Add(entity);
        }
        public void UpdateJCLQ_Match(C_JCLQ_Match entity)
        {
            DB.GetDal<C_JCLQ_Match>().Update(entity);
        }

        public void UpdateJCLQ_MatchResult(C_JCLQ_MatchResult entity)
        {
            DB.GetDal<C_JCLQ_MatchResult>().Update(entity);
        }

        public void UpdateJCLQ_MatchResult_Prize(C_JCLQ_MatchResult_Prize entity)
        {
            DB.GetDal<C_JCLQ_MatchResult_Prize>().Update(entity);
        }

        public C_JCLQ_Match GetJCLQMatch(string matchId)
        {
          
            return DB.CreateQuery<C_JCLQ_Match>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }

        public IList<DisableMatchConfigInfo> QueryJCLQ_DisableMatchConfigList()
        {
           
            var query = from c in DB.CreateQuery<C_JCLQ_Match>()
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

        public List<C_JCLQ_Match> QueryJCLQ_MatchListByMatchId(string[] matchIdArray)
        {
           
            return DB.CreateQuery<C_JCLQ_Match>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<C_JCLQ_MatchResult> QueryJCLQ_MatchResultListByMatchId(string[] matchIdArray)
        {
           
            return DB.CreateQuery<C_JCLQ_MatchResult>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<C_JCLQ_MatchResult_Prize> QueryJCLQ_MatchResult_PrizeListByMatchId(string[] matchIdArray)
        {
           
            return DB.CreateQuery<C_JCLQ_MatchResult_Prize>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }

        public C_JCLQ_MatchResult QueryJCLQ_MatchResultByMatchId(string matchId)
        {
          
            return DB.CreateQuery<C_JCLQ_MatchResult>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCLQ_MatchResult_Prize QueryJCLQ_MatchResult_PrizeByMatchId(string matchId)
        {
          
            return DB.CreateQuery<C_JCLQ_MatchResult_Prize>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }

        public List<C_JCLQ_Match> QueryCurrentJCLQMatchInfo()
        {
           
            var query = from m in DB.CreateQuery<C_JCLQ_Match>()
                        where m.FSStopBettingTime >= DateTime.Now
                        select new C_JCLQ_Match
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
          
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in DB.CreateQuery<C_JCLQ_MatchResult>()
                        join m in DB.CreateQuery<C_JCLQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= startTime && r.CreateTime < endTime.AddDays(1)
                        && (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M)
                        orderby r.MatchId descending
                        select new JCLQMatchResult
                        {
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            StartTime = m.StartDateTime,
                            LeagueId = m.LeagueId,
                            LeagueName = m.LeagueName,
                            LeagueColor = m.LeagueColor,
                            HomeTeamName = m.HomeTeamName,
                            GuestTeamName = m.GuestTeamName,
                            MatchState = r.MatchState,
                            HomeTeamScore = r.HomeScore,
                            GuestTeamScore = r.GuestScore,

                            SF_Result = r.SF_Result,
                            SF_SP = r.SF_SP,
                            RFSF_Result = r.RFSF_Result,
                            RFSF_SP = r.RFSF_SP,
                            DXF_Result = r.DXF_Result,
                            DXF_SP = r.DXF_SP,
                            SFC_Result = r.SFC_Result,
                            SFC_SP = r.SFC_SP,
                            RFSF_Trend = r.RFSF_Trend,
                            DXF_Trend = r.DXF_Trend,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<JCLQMatchResult> QueryJCLQMatchResult(DateTime time)
        {
         
            var query = from r in DB.CreateQuery<C_JCLQ_MatchResult>()
                        join m in DB.CreateQuery<C_JCLQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= time && r.CreateTime < time.AddDays(1)
                        && (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M)
                        orderby r.MatchId descending
                        select new JCLQMatchResult
                        {
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            StartTime = m.StartDateTime,
                            LeagueId = m.LeagueId,
                            LeagueName = m.LeagueName,
                            LeagueColor = m.LeagueColor,
                            HomeTeamName = m.HomeTeamName,
                            GuestTeamName = m.GuestTeamName,
                            MatchState = r.MatchState,
                            HomeTeamScore = r.HomeScore,
                            GuestTeamScore = r.GuestScore,

                            SF_Result = r.SF_Result,
                            SF_SP = r.SF_SP,
                            RFSF_Result = r.RFSF_Result,
                            RFSF_SP = r.RFSF_SP,
                            DXF_Result = r.DXF_Result,
                            DXF_SP = r.DXF_SP,
                            SFC_Result = r.SFC_Result,
                            SFC_SP = r.SFC_SP,
                            RFSF_Trend = r.RFSF_Trend,
                            DXF_Trend = r.DXF_Trend,
                            CreateTime = r.CreateTime,
                        };
            return query.ToList();
        }

        public List<C_JCLQ_MatchResult> QueryJCLQMatchResultByDay(int day)
        {

            var query = from r in DB.CreateQuery<C_JCLQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M )
                        select r;
            return query.ToList();
        }
    }
}
