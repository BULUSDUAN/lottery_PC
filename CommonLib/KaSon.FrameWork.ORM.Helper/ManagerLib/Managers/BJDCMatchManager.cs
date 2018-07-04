using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
    /// <summary>
    /// kason
    /// </summary>
   public class BJDCMatchManager : DBbase
    {
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
