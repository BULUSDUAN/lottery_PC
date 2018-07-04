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
   public class JCLQMatchManager : DBbase
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

    }
}
