using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using GameBiz.Business;
using GameBiz.Domain.Entities;

namespace GameBiz.Domain.Managers
{
    public class BonusRuleManager : GameBizEntityManagement
    {
        public List<BonusRule> QueryBonusRule(string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<BonusRule>().Where(p => p.GameCode == gameCode && p.GameType == gameType).ToList();
        }

        public List<BonusRule> QueryAllBonusRule()
        {
            Session.Clear();
            return this.Session.Query<BonusRule>().ToList();
        }

        public BonusRule GetBonusRule(string gameCode, string gameType, int bonusLevel)
        {
            Session.Clear();
            return this.Session.Query<BonusRule>().FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.BonusGrade == bonusLevel);
        }

    }
}
