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
    }
}
