using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
  

    public class JCZQMatchManager : DBbase
    {
        public List<C_JCZQ_MatchResult> QueryJCZQMatchResultByDay(int day)
        {
            // Session.Clear();
            var query = from r in DB.CreateQuery<C_JCZQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        select r;
            return query.ToList();
        }
        public List<C_JCZQ_OZBMatch> QueryJCZQ_OZBMatchList(string gameType, string[] matchIdArray)
        {
            //  Session.Clear();
            return this.DB.CreateQuery<C_JCZQ_OZBMatch>().Where(p => matchIdArray.Contains(p.MatchId) && p.GameType == gameType).ToList();
        }
        public List<C_JCZQ_SJBMatch> QueryJCZQ_SJBMatchList(string gameType, string[] matchIdArray)
        {
            //Session.Clear();
            return this.DB.CreateQuery<C_JCZQ_SJBMatch>().Where(p => matchIdArray.Contains(p.MatchId) && p.GameType == gameType).ToList();
        }
        public void AddJCZQ_Match(C_JCZQ_Match entity)
        {

            DB.GetDal<C_JCZQ_Match>().Add(entity);
        }
        public void AddJCZQ_OZBMatch(C_JCZQ_OZBMatch entity)
        {
            DB.GetDal<C_JCZQ_OZBMatch>().Add(entity);
        }
        public void AddJCZQ_MatchResult(C_JCZQ_MatchResult entity)
        {
            DB.GetDal<C_JCZQ_MatchResult>().Add(entity);
        }
        public void AddJCZQ_MatchResult_Prize(C_JCZQ_MatchResult_Prize entity)
        {
            DB.GetDal<C_JCZQ_MatchResult_Prize>().Add(entity);
        }
        public void AddJCZQ_SJBMatch(C_JCZQ_SJBMatch entity)
        {
            DB.GetDal<C_JCZQ_SJBMatch>().Add(entity);
        }

        public void UpdateJCZQ_Match(C_JCZQ_Match entity)
        {
            DB.GetDal<C_JCZQ_Match>().Update(entity);
        }
        public void UpdateJCZQ_OZBMatch(C_JCZQ_OZBMatch entity)
        {
            DB.GetDal<C_JCZQ_OZBMatch>().Update(entity);
        }
        public void UpdateJCZQ_SJBMatch(C_JCZQ_SJBMatch entity)
        {
            DB.GetDal<C_JCZQ_SJBMatch>().Update(entity);
        }


        public void UpdateJCZQ_MatchResult(C_JCZQ_MatchResult entity)
        {
            DB.GetDal<C_JCZQ_MatchResult>().Update(entity);
        }

        public void UpdateJCZQ_MatchResult_Prize(C_JCZQ_MatchResult_Prize entity)
        {
            DB.GetDal<C_JCZQ_MatchResult_Prize>().Update(entity);
        }

        public C_JCZQ_Match GetJCZQMatch(string matchId)
        {
          
            return DB.CreateQuery<C_JCZQ_Match>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCZQ_OZBMatch GetJCZQ_OZBMatch(string gameType, string matchId)
        {
           
            return DB.CreateQuery<C_JCZQ_OZBMatch>().Where(p => p.MatchId == matchId && p.GameType == gameType).FirstOrDefault();
        }

     

        public IList<DisableMatchConfigInfo> QueryJCZQ_DisableMatchConfigList()
        {
         
            var query = from c in DB.CreateQuery<C_JCZQ_Match>()
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

        public List<C_JCZQ_Match> QueryJCZQ_MatchListByMatchId(string[] matchIdArray)
        {
          
            return DB.CreateQuery<C_JCZQ_Match>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<C_JCZQ_MatchResult> QueryJCZQ_MatchResultListByMatchId(string[] matchIdArray)
        {
          
            return DB.CreateQuery<C_JCZQ_MatchResult>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }
        public List<C_JCZQ_MatchResult_Prize> QueryJCZQ_MatchResult_PrizeListByMatchId(string[] matchIdArray)
        {
           
            return DB.CreateQuery<C_JCZQ_MatchResult_Prize>().Where(p => matchIdArray.Contains(p.MatchId)).ToList();
        }

        public C_JCZQ_MatchResult QueryJCZQ_MatchResultByMatchId(string matchId)
        {
           
            return DB.CreateQuery<C_JCZQ_MatchResult>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCZQ_MatchResult_Prize QueryJCZQ_MatchResult_PrizeByMatchId(string matchId)
        {
            
            return DB.CreateQuery<C_JCZQ_MatchResult_Prize>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }

        public List<C_JCZQ_Match> QueryCurrentJCZQMatchInfo()
        {
          
            var query = from m in DB.CreateQuery<C_JCZQ_Match>()
                        where m.FSStopBettingTime >= DateTime.Now
                        select new C_JCZQ_Match
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
          
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in DB.CreateQuery<C_JCZQ_MatchResult>()
                        join m in DB.CreateQuery<C_JCZQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= startTime && r.CreateTime < endTime.AddDays(1)
                        && (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        orderby r.MatchId descending
                        select new JCZQMatchResult
                        {
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            StartTime =m.StartDateTime,
                            LeagueId = m.LeagueId,
                            LeagueName = m.LeagueName,
                            LeagueColor =m.LeagueColor,
                            HomeTeamId = m.HomeTeamId,
                            HomeTeamName = m.HomeTeamName,
                            GuestTeamId = m.GuestTeamId,
                            GuestTeamName = m.GuestTeamName,
                            LetBall = m.LetBall,
                            WinOdds =m.WinOdds,
                            FlatOdds = m.FlatOdds,
                            LoseOdds = m.LoseOdds,
                            MatchState = r.MatchState,
                            HalfHomeTeamScore =r.HalfHomeTeamScore,
                            HalfGuestTeamScore = r.HalfGuestTeamScore,
                            FullHomeTeamScore = r.FullHomeTeamScore,
                            FullGuestTeamScore = r.FullGuestTeamScore,
                            SPF_Result = r.SPF_Result,
                            SPF_SP = r.SPF_SP,
                            BRQSPF_Result = r.BRQSPF_Result,
                            BRQSPF_SP = r.BRQSPF_SP,
                            ZJQ_Result = r.ZJQ_Result,
                            ZJQ_SP = r.ZJQ_SP,
                            BF_Result = r.BF_Result,
                            BF_SP = r.BF_SP,
                            BQC_Result = r.BQC_Result,
                            BQC_SP =r.BQC_SP,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        public List<JCZQMatchResult> QueryJCZQMatchResult(DateTime time)
        {
         
            var query = (from r in DB.CreateQuery<C_JCZQ_MatchResult>()
                        join m in DB.CreateQuery<C_JCZQ_Match>() on r.MatchId equals m.MatchId
                        where r.CreateTime >= time && r.CreateTime < time.AddDays(1)
                        && (r.SPF_SP != 1M && r.BRQSPF_SP != 1M && r.ZJQ_SP != 1M && r.BF_SP != 1M && r.BQC_SP != 1M)
                        orderby r.MatchId descending
                        select new {r,m }).ToList().Select(p=> new JCZQMatchResult
                        {
                            MatchId = p.m.MatchId,
                            MatchIdName = p.m.MatchIdName,
                            StartTime = p.m.StartDateTime,
                            LeagueId = p.m.LeagueId,
                            LeagueName = p.m.LeagueName,
                            LeagueColor = p.m.LeagueColor,
                            HomeTeamId = p.m.HomeTeamId,
                            HomeTeamName = p.m.HomeTeamName,
                            GuestTeamId = p.m.GuestTeamId,
                            GuestTeamName = p.m.GuestTeamName,
                            LetBall = p.m.LetBall,
                            WinOdds = p.m.WinOdds,
                            FlatOdds = p.m.FlatOdds,
                            LoseOdds = p.m.LoseOdds,
                            MatchState = p.r.MatchState,
                            HalfHomeTeamScore = p.r.HalfHomeTeamScore,
                            HalfGuestTeamScore = p.r.HalfGuestTeamScore,
                            FullHomeTeamScore = p.r.FullHomeTeamScore,
                            FullGuestTeamScore = p.r.FullGuestTeamScore,
                            SPF_Result = p.r.SPF_Result,
                            SPF_SP = p.r.SPF_SP,
                            BRQSPF_Result = p.r.BRQSPF_Result,
                            BRQSPF_SP = p.r.BRQSPF_SP,
                            ZJQ_Result = p.r.ZJQ_Result,
                            ZJQ_SP = p.r.ZJQ_SP,
                            BF_Result = p.r.BF_Result,
                            BF_SP = p.r.BF_SP,
                            BQC_Result = p.r.BQC_Result,
                            BQC_SP = p.r.BQC_SP,
                            CreateTime = p.r.CreateTime,
                        });
            return query.ToList();
        }

    



        public C_JCZQ_SJBMatch GetJCZQ_SJBMatch(string gameType, string matchId)
        {
         
            return DB.CreateQuery<C_JCZQ_SJBMatch>().Where(p => p.MatchId == matchId && p.GameType == gameType).FirstOrDefault();
        }

      
    }
}
