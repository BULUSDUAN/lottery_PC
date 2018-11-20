using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using NHibernate.Linq;

namespace GameBiz.Business.Domain.Managers
{
    public class SFGGMatchManager : GameBizEntityManagement
    {
        public void AddSFGGMatch(SFGG_Match entity)
        {
            this.Add<SFGG_Match>(entity);
        }
        public void UpdateSFGGMatch(SFGG_Match entity)
        {
            this.Update<SFGG_Match>(entity);
        }
        public List<SFGG_Match> QuerySFGGMatchList(string issuseNumber)
        {
            return this.Session.Query<SFGG_Match>().Where(s => s.IssuseNumber == issuseNumber).ToList();
        }
        public SFGG_Match QuerySFGGMatch(string issuseNumber, int matchOrderId)
        {
            return this.Session.Query<SFGG_Match>().Where(s => s.IssuseNumber == issuseNumber && s.MatchOrderId == matchOrderId).FirstOrDefault();
        }
    }
}
