using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business.Domain.Entities.Ticket;
using Common.Database.ORM;
using NHibernate.Linq;

namespace GameBiz.Business.Domain.Managers.Ticket
{
    public class Ticket_BonusManager : GameBizEntityManagement
    {
        public void AddBonusPool(Ticket_BonusPool entity)
        {
            this.Add<Ticket_BonusPool>(entity);
        }
        public Ticket_BonusPool GetBonusPool(string gameCode, string gameType, string issuseNumber, string bonusLevel)
        {
            Session.Clear();
            return this.Session.Query<Ticket_BonusPool>().Where(p => p.GameCode == gameCode && p.GameType == gameType && p.IssuseNumber == issuseNumber && p.BonusLevel == bonusLevel).FirstOrDefault();
        }
        public void UpdateBonusPool(Ticket_BonusPool entity)
        {
            this.Update(entity);
        }

        public List<Ticket_BonusPool> GetBonusPool(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            var query = from p in this.Session.Query<Ticket_BonusPool>()
                        where p.GameCode == gameCode
                        && (gameType == "" || p.GameType == gameType)
                        && p.IssuseNumber == issuseNumber
                        select p;
            return query.ToList();

        }

        public List<Ticket_BonusPool> GetBonusPool(string gameCode, string gameType, int length)
        {
            Session.Clear();
            var query = from p in this.Session.Query<Ticket_BonusPool>()
                        where p.GameCode == gameCode
                        && (gameType == "" || p.GameType == gameType)
                        orderby p.CreateTime descending
                        select p;
            return query.Take(length).ToList();
        }


    }
}
