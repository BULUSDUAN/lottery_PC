using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;
using GameBiz.Business;

namespace GameBiz.Domain.Managers
{
    public class BJDCMatchManager : GameBizEntityManagement
    {
        public void AddBJDC_Match(params BJDC_Match[] array)
        {
            this.Add<BJDC_Match>(array);
        }
        public void AddBJDC_MatchResult(params BJDC_MatchResult[] array)
        {
            this.Add<BJDC_MatchResult>(array);
        }
        public void AddBJDC_MatchResult_Prize(params BJDC_MatchResult_Prize[] array)
        {
            this.Add<BJDC_MatchResult_Prize>(array);
        }
        public void AddBJDC_Issuse(params BJDC_Issuse[] array)
        {
            this.Add<BJDC_Issuse>(array);
        }
        public void UpdateBJDC_Match(params BJDC_Match[] entity)
        {
            this.Update<BJDC_Match>(entity);
        }
        public void UpdateBJDC_MatchResult(params BJDC_MatchResult[] entity)
        {
            this.Update<BJDC_MatchResult>(entity);
        }
        public void UpdateBJDC_MatchResult_Prize(params BJDC_MatchResult_Prize[] entity)
        {
            this.Update<BJDC_MatchResult_Prize>(entity);
        }
        public void UpdateBJDC_Issuse(BJDC_Issuse entity)
        {
            this.Update(entity);
        }

        public BJDC_Issuse QueryBJDC_Issuse(string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<BJDC_Issuse>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public List<BJDC_Match> QueryBJDC_MatchList(string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<BJDC_Match>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public List<BJDC_MatchResult> QueryBJDC_MatchResultList(string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public List<BJDC_MatchResult_Prize> QueryBJDC_MatchResult_PrizeList(string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult_Prize>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public BJDC_MatchResult QueryBJDC_MatchResult(string issuseNumber, int matchOrderId)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == matchOrderId).FirstOrDefault();
        }
        public BJDC_MatchResult_Prize QueryBJDC_MatchResult_Prize(string issuseNumber, int matchOrderId)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult_Prize>().Where(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == matchOrderId).FirstOrDefault();
        }
        public BJDC_Match GetBJDCMatchById(string Id)
        {
            Session.Clear();
            return this.Session.Query<BJDC_Match>().Where(b => b.Id == Id).FirstOrDefault();
        }
        public IList<DisableMatchConfigInfo> QueryBJDC_DisableMatchConfigList()
        {
            this.Session.Clear();
            var query = from c in Session.Query<BJDC_Match>()
                        where c.MatchStartTime > DateTime.Now
                        && c.PrivilegesType != null && c.PrivilegesType != ""
                        select new DisableMatchConfigInfo
                        {
                            MatchId = c.MatchOrderId.ToString(),
                            GameCode = "BJDC",
                            MatchStartTime = c.MatchStartTime,
                            PrivilegesType = c.PrivilegesType,
                            IssuseNumber = c.IssuseNumber
                        };
            return query.ToList();
        }
        public List<BJDC_MatchResult> QueryBJDC_MatchResultList(string issuseNumber, string[] matchOrderIdArray)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber
                && matchOrderIdArray.Contains(p.MatchOrderId.ToString())
                && p.SPF_Result != "" && p.ZJQ_Result != "" && p.SXDS_Result != "" && p.BF_Result != "" && p.BQC_Result != ""
                ).ToList();
        }
        public List<CoreBJDCMatchInfo> QueryCurrentBJDCMatchInfo()
        {
            Session.Clear();
            var query = from m in this.Session.Query<BJDC_Match>()
                        where m.LocalStopTime >= DateTime.Now
                        select new CoreBJDCMatchInfo
                        {
                            Id = m.Id,
                            MatchOrderId = m.MatchOrderId != null ? m.MatchOrderId.ToString().Trim() : "",
                            FSStopBettingTime = m.LocalStopTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueColor = m.MatchColor,
                            LeagueName = m.MatchName,
                            MatchData = m.IssuseNumber,
                            MatchId = m.MatchId != null ? m.MatchId.ToString().Trim() : "",
                            MatchIdName = "",
                            MatchNumber = m.MatchOrderId != null ? m.MatchOrderId.ToString().Trim() : "",
                            StartDateTime = m.MatchStartTime,
                            PrivilegesType = m.PrivilegesType != null ? m.PrivilegesType : "",
                        };
            return query.ToList();
        }
        public List<BJDCMatchResultInfo> QueryBJDC_MatchResultListByissuseNumber(string issuseNumber)
        {
            Session.Clear();
            var query = from r in this.Session.Query<BJDC_MatchResult>()
                        join m in this.Session.Query<BJDC_Match>() on r.Id equals m.Id
                        where r.IssuseNumber == issuseNumber
                        orderby r.Id descending
                        select new BJDCMatchResultInfo
                        {
                            BF_Result = r.BF_Result == null ? "" : r.BF_Result,
                            BF_SP = r.BF_SP == null ? 0 : r.BF_SP,
                            BQC_Result = r.BQC_Result == null ? "" : r.BQC_Result,
                            BQC_SP = r.BQC_SP == null ? 0 : r.BQC_SP,
                            CreateTime = r.CreateTime,
                            FlatOdds = m.FlatOdds == null ? 0 : m.FlatOdds,
                            GuestFull_Result = r.GuestFull_Result == null ? "" : r.GuestFull_Result,
                            GuestHalf_Result = r.GuestHalf_Result == null ? "" : r.GuestHalf_Result,
                            GuestTeamName = m.GuestTeamName,
                            HomeFull_Result = r.HomeFull_Result == null ? "" : r.HomeFull_Result,
                            HomeHalf_Result = r.HomeHalf_Result == null ? "" : r.HomeHalf_Result,
                            HomeTeamName = m.HomeTeamName,
                            Id = r.Id,
                            IssuseNumber = r.IssuseNumber,
                            LetBall = m.LetBall,
                            LoseOdds = m.LoseOdds,
                            MatchColor = m.MatchColor,
                            MatchName = m.MatchName,
                            MatchOrderId = r.MatchOrderId,
                            MatchStartTime = m.MatchStartTime,
                            MatchState = r.MatchState,
                            SPF_Result = r.SPF_Result == null ? "" : r.SPF_Result,
                            SPF_SP = r.SPF_SP,
                            SXDS_Result = r.SXDS_Result == null ? "" : r.SXDS_Result,
                            SXDS_SP = r.SXDS_SP,
                            WinOdds = m.WinOdds,
                            ZJQ_Result = r.ZJQ_Result == null ? "" : r.ZJQ_Result,
                            ZJQ_SP = r.ZJQ_SP
                        };
            if (query != null)
                return query.ToList();
            return new List<BJDCMatchResultInfo>();

        }
        public List<BJDCMatchResultInfo> QueryBJDC_MatchResultList(string issuseNumber, int pageIndex, int pageSize)
        {
            Session.Clear();
            var query = from r in this.Session.Query<BJDC_MatchResult>()
                        join m in this.Session.Query<BJDC_Match>() on r.Id equals m.Id
                        where r.IssuseNumber == issuseNumber
                        orderby r.Id descending
                        select new BJDCMatchResultInfo
                        {
                            BF_Result = r.BF_Result == null ? "" : r.BF_Result,
                            BF_SP = r.BF_SP == null ? 0 : r.BF_SP,
                            BQC_Result = r.BQC_Result == null ? "" : r.BQC_Result,
                            BQC_SP = r.BQC_SP == null ? 0 : r.BQC_SP,
                            CreateTime = r.CreateTime,
                            FlatOdds = m.FlatOdds == null ? 0 : m.FlatOdds,
                            GuestFull_Result = r.GuestFull_Result == null ? "" : r.GuestFull_Result,
                            GuestHalf_Result = r.GuestHalf_Result == null ? "" : r.GuestHalf_Result,
                            GuestTeamName = m.GuestTeamName,
                            HomeFull_Result = r.HomeFull_Result == null ? "" : r.HomeFull_Result,
                            HomeHalf_Result = r.HomeHalf_Result == null ? "" : r.HomeHalf_Result,
                            HomeTeamName = m.HomeTeamName,
                            Id = r.Id,
                            IssuseNumber = r.IssuseNumber,
                            LetBall = m.LetBall,
                            LoseOdds = m.LoseOdds,
                            MatchColor = m.MatchColor,
                            MatchName = m.MatchName,
                            MatchOrderId = r.MatchOrderId,
                            MatchStartTime = m.MatchStartTime,
                            MatchState = r.MatchState,
                            SPF_Result = r.SPF_Result == null ? "" : r.SPF_Result,
                            SPF_SP = r.SPF_SP,
                            SXDS_Result = r.SXDS_Result == null ? "" : r.SXDS_Result,
                            SXDS_SP = r.SXDS_SP,
                            WinOdds = m.WinOdds,
                            ZJQ_Result = r.ZJQ_Result == null ? "" : r.ZJQ_Result,
                            ZJQ_SP = r.ZJQ_SP
                        };
            if (query != null)
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return new List<BJDCMatchResultInfo>();

        }

        public MatchBiz.Core.BJDC_IssuseInfo QueryBJDCCurrentIssuse()
        {
            this.Session.Clear();
            var query = from b in this.Session.Query<BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select new MatchBiz.Core.BJDC_IssuseInfo
                        {
                            IssuseNumber = b.IssuseNumber,
                            MinLocalStopTime = b.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            MinMatchStartTime = b.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
            return query.FirstOrDefault();
        }
        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            this.Session.Clear();
            var query = from b in this.Session.Query<BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select new BJDCIssuseInfo
                        {
                            IssuseNumber = b.IssuseNumber,
                            MinLocalStopTime = b.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            MinMatchStartTime = b.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
            return query.FirstOrDefault();
        }
        public List<BJDCMatchResultInfo> GetBJDCIssuse(int count)
        {
            var query = from s in Session.Query<BJDC_MatchResult>()
                        group s by s.IssuseNumber into g
                        select new BJDCMatchResultInfo
                        {
                            IssuseNumber = g.Key,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList().OrderByDescending(s => s.IssuseNumber).Take(count).ToList();
            return new List<BJDCMatchResultInfo>();
        }
        public string[] QueryBJDCLastIssuseNumber(int count)
        {
            Session.Clear();
            var query = from i in this.Session.Query<BJDC_Issuse>()
                        orderby i.IssuseNumber descending
                        select i.IssuseNumber;
            return query.Take(count).ToArray();
        }

        public List<BJDC_MatchResult_Prize> QueryBJDCMatchResultByIssuse(int issuseCount)
        {
            Session.Clear();
            //查最近N期的期号
            var queryIssuse = from b in this.Session.Query<BJDC_Issuse>()
                              orderby b.IssuseNumber descending
                              select b.IssuseNumber;
            var issuseList = queryIssuse.Take(issuseCount).ToList();
            //查期号中有结果的比赛
            var query = from r in this.Session.Query<BJDC_MatchResult_Prize>()
                        where issuseList.Contains(r.IssuseNumber)
                        && r.MatchState == "2"
                        select r;
            return query.ToList();
        }

    }
}
