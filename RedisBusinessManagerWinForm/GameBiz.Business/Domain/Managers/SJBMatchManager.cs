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
    public class SJBMatchManager : GameBizEntityManagement
    {
        public void AddSJB_Match(SJB_Match entity)
        {
            this.Add<SJB_Match>(entity);
        }
        public void UpdateSJB_Match(SJB_Match entity)
        {
            this.Update<SJB_Match>(entity);
        }

        public SJB_Match GetSJBMatch(string gameType, int matchId)
        {
            Session.Clear();
            return this.Session.Query<SJB_Match>().FirstOrDefault(p => p.GameType == gameType && p.MatchId == matchId);
        }

        public List<SJB_Match> QuerySJB_MatchListByMatchId(string[] matchIdArray)
        {
            Session.Clear();
            return this.Session.Query<SJB_Match>().Where(p => matchIdArray.Contains(p.MatchId.ToString())).ToList();
        }
    }
}
