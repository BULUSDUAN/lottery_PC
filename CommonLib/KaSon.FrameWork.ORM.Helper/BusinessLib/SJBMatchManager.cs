using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SJBMatchManager :DBbase
    {
        public void AddSJB_Match(C_SJB_Match entity)
        {
            DB.GetDal<C_SJB_Match>().Add(entity);
        }
        public void UpdateSJB_Match(C_SJB_Match entity)
        {
            DB.GetDal<C_SJB_Match>().Update(entity);
        }

        public C_SJB_Match GetSJBMatch(string gameType, int matchId)
        {
            return DB.CreateQuery<C_SJB_Match>().Where(p => p.GameType == gameType && p.MatchId == matchId).FirstOrDefault();
        }

        public List<C_SJB_Match> QuerySJB_MatchListByMatchId(string[] matchIdArray)
        {
            return DB.CreateQuery<C_SJB_Match>().Where(p => matchIdArray.Contains(p.MatchId.ToString())).ToList();
        }
    }
}
