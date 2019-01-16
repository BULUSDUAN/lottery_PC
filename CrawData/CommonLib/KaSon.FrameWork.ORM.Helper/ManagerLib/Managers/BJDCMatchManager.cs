using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{ 

    public class BJDCMatchManager : DBbase
    {
        public void AddBJDC_Match(params C_BJDC_Match[] array)
        {
            DB.GetDal<C_BJDC_Match>().Add(array);
        }
        public void AddBJDC_MatchResult(params C_BJDC_MatchResult[] array)
        {
            DB.GetDal<C_BJDC_MatchResult>().Add(array);
        }
        public void AddBJDC_MatchResult_Prize(params C_BJDC_MatchResult_Prize[] array)
        {
            DB.GetDal<C_BJDC_MatchResult_Prize>().Add(array);
        }
        public void AddBJDC_Issuse(params C_BJDC_Issuse[] array)
        {
            DB.GetDal<C_BJDC_Issuse>().Add(array);
        }
        public void UpdateBJDC_Match(params C_BJDC_Match[] entity)
        {
            DB.GetDal<C_BJDC_Match>().Update(entity);
        }
        public void UpdateBJDC_MatchResult(params C_BJDC_MatchResult[] entity)
        {
            DB.GetDal<C_BJDC_MatchResult>().Update(entity);
        }
        public void UpdateBJDC_MatchResult_Prize(params C_BJDC_MatchResult_Prize[] entity)
        {
            DB.GetDal<C_BJDC_MatchResult_Prize>().Update(entity);
        }
        public void UpdateBJDC_Issuse(C_BJDC_Issuse entity)
        {
            DB.GetDal<C_BJDC_Issuse>().Update(entity);
        }

        public C_BJDC_Issuse QueryBJDC_Issuse(string issuseNumber)
        {
         
            return DB.CreateQuery<C_BJDC_Issuse>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public List<C_BJDC_Match> QueryBJDC_MatchList(string issuseNumber)
        {
           
            return DB.CreateQuery<C_BJDC_Match>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public List<C_BJDC_MatchResult> QueryBJDC_MatchResultList(string issuseNumber)
        {
            
            return DB.CreateQuery<C_BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public List<C_BJDC_MatchResult_Prize> QueryBJDC_MatchResult_PrizeList(string issuseNumber)
        {
           
            return DB.CreateQuery<C_BJDC_MatchResult_Prize>().Where(p => p.IssuseNumber == issuseNumber).ToList();
        }
        public C_BJDC_MatchResult QueryBJDC_MatchResult(string issuseNumber, int matchOrderId)
        {
            
            return DB.CreateQuery<C_BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == matchOrderId).FirstOrDefault();
        }
        public C_BJDC_MatchResult_Prize QueryBJDC_MatchResult_Prize(string issuseNumber, int matchOrderId)
        {
          
            return DB.CreateQuery<C_BJDC_MatchResult_Prize>().Where(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == matchOrderId).FirstOrDefault();
        }
        public C_BJDC_Match GetBJDCMatchById(string Id)
        {
          
            return DB.CreateQuery<C_BJDC_Match>().Where(b => b.Id == Id).FirstOrDefault();
        }
        public IList<DisableMatchConfigInfo> QueryBJDC_DisableMatchConfigList()
        {
          
            var query = from c in DB.CreateQuery<C_BJDC_Match>()
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
        public List<C_BJDC_MatchResult> QueryBJDC_MatchResultList(string issuseNumber, string[] matchOrderIdArray)
        {
           
            return DB.CreateQuery<C_BJDC_MatchResult>().Where(p => p.IssuseNumber == issuseNumber
                && matchOrderIdArray.Contains(p.MatchOrderId.ToString())
                && p.SPF_Result != "" && p.ZJQ_Result != "" && p.SXDS_Result != "" && p.BF_Result != "" && p.BQC_Result != ""
                ).ToList();
        }
        public List<CoreBJDCMatchInfo> QueryCurrentBJDCMatchInfo()
        {
       
            var query = from m in DB.CreateQuery<C_BJDC_Match>()
                        where m.LocalStopTime >= DateTime.Now
                        select new CoreBJDCMatchInfo
                        {
                            Id = m.Id,
                            MatchOrderId = m.MatchOrderId != 0 ? m.MatchOrderId.ToString().Trim() : "",
                            FSStopBettingTime = m.LocalStopTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueColor = m.MatchColor,
                            LeagueName = m.MatchName,
                            MatchData = m.IssuseNumber,
                            MatchId = m.MatchId != 0 ? m.MatchId.ToString().Trim() : "",
                            MatchIdName = "",
                            MatchNumber = m.MatchOrderId != 0 ? m.MatchOrderId.ToString().Trim() : "",
                            StartDateTime = m.MatchStartTime,
                            PrivilegesType = m.PrivilegesType != null ? m.PrivilegesType : "",
                        };
            return query.ToList();
        }
        public List<BJDCMatchResultInfo> QueryBJDC_MatchResultListByissuseNumber(string issuseNumber)
        {
           
            var query = from r in DB.CreateQuery<C_BJDC_MatchResult>()
                        join m in DB.CreateQuery<C_BJDC_Match>() on r.Id equals m.Id
                        where r.IssuseNumber == issuseNumber
                        orderby r.Id descending
                        select new BJDCMatchResultInfo
                        {
                            BF_Result = r.BF_Result == null ? "" : r.BF_Result,
                            BF_SP = r.BF_SP == 0 ? 0 : r.BF_SP,
                            BQC_Result = r.BQC_Result == null ? "" : r.BQC_Result,
                            BQC_SP = r.BQC_SP == 0 ? 0 : r.BQC_SP,
                            CreateTime = r.CreateTime,
                            FlatOdds = m.FlatOdds == 0 ? 0 : m.FlatOdds,
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

            var query = from r in DB.CreateQuery<C_BJDC_MatchResult>()
                        join m in DB.CreateQuery<C_BJDC_Match>() on r.Id equals m.Id
                        where r.IssuseNumber == issuseNumber
                        orderby r.Id descending
                        select new BJDCMatchResultInfo
                        {
                            BF_Result = r.BF_Result == null ? "" : r.BF_Result,
                            BF_SP = r.BF_SP == 0 ? 0 : r.BF_SP,
                            BQC_Result = r.BQC_Result == null ? "" : r.BQC_Result,
                            BQC_SP = r.BQC_SP == 0 ? 0 : r.BQC_SP,
                            CreateTime = r.CreateTime,
                            FlatOdds = m.FlatOdds == 0 ? 0 : m.FlatOdds,
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

        public C_BJDC_Issuse QueryBJDCCurrentIssuse()
        {
          
            var query = from b in DB.CreateQuery<C_BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select new C_BJDC_Issuse
                        {
                            IssuseNumber = b.IssuseNumber,
                            MinLocalStopTime = Convert.ToDateTime(b.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            MinMatchStartTime = Convert.ToDateTime(b.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                        };
            return query.FirstOrDefault();
        }
        public C_BJDC_Issuse QueryBJDCCurrentIssuseInfo()
        {
           
            var query = from b in DB.CreateQuery<C_BJDC_Issuse>()
                        where b.MinLocalStopTime >= DateTime.Now
                        orderby b.MinLocalStopTime ascending
                        select new C_BJDC_Issuse
                        {
                            IssuseNumber = b.IssuseNumber,
                            MinLocalStopTime = Convert.ToDateTime(b.MinLocalStopTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            MinMatchStartTime = Convert.ToDateTime(b.MinMatchStartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                        };
            return query.FirstOrDefault();
        }
        public List<BJDCMatchResultInfo> GetBJDCIssuse(int count)
        {
            var query = from s in DB.CreateQuery<C_BJDC_MatchResult>()
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
          
            var query = from i in DB.CreateQuery<C_BJDC_Issuse>()
                        orderby i.IssuseNumber descending
                        select i.IssuseNumber;
            return query.Take(count).ToArray();
        }

        public List<C_JCLQ_MatchResult> QueryJCLQMatchResultByDay(int day)
        {
            // Session.Clear();
            var query = from r in DB.CreateQuery<C_JCLQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M )
                        select r;
            return query.ToList();
        }

        public List<C_BJDC_MatchResult_Prize> QueryBJDCMatchResultByIssuse(int issuseCount)
        {
            // Session.Clear();
            //查最近N期的期号
            var queryIssuse = from b in DB.CreateQuery<C_BJDC_Issuse>()
                              orderby b.IssuseNumber descending
                              select b.IssuseNumber;
            var issuseList = queryIssuse.Take(issuseCount).ToList();
            //查期号中有结果的比赛
            var query = from r in DB.CreateQuery<C_BJDC_MatchResult_Prize>()
                        where issuseList.Contains(r.IssuseNumber)
                        && r.MatchState == "2"
                        select r;
            return query.ToList();
        }

    }
}
