using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{ 
   public class BonusRuleManager : DBbase
    {
        public List<C_Bonus_Rule> QueryBonusRule(string gameCode, string gameType)
        {
            //Session.Clear();
            return this.DB.CreateQuery<C_Bonus_Rule>().Where(p => p.GameCode == gameCode && p.GameType == gameType).ToList();
        }

        public List<C_Bonus_Rule> QueryAllBonusRule()
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Bonus_Rule>().ToList();
        }

        public C_Bonus_Rule GetBonusRule(string gameCode, string gameType, int bonusLevel)
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Bonus_Rule>().FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.BonusGrade == bonusLevel);
        }
    }
}
