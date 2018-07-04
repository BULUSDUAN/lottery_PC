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
   public class Ticket_BonusManager : DBbase
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
        public void AddBonusPool(T_Ticket_BonusPool entity)
        {
            DB.GetDal<T_Ticket_BonusPool>().Add(entity);
        }
        public T_Ticket_BonusPool GetBonusPool(string gameCode, string gameType, string issuseNumber, string bonusLevel)
        {
           // Session.Clear();
            return DB.CreateQuery<T_Ticket_BonusPool>().Where(p => p.GameCode == gameCode && p.GameType == gameType && p.IssuseNumber == issuseNumber && p.BonusLevel == bonusLevel).FirstOrDefault();
        }
        public void UpdateBonusPool(T_Ticket_BonusPool entity)
        {
            DB.GetDal<T_Ticket_BonusPool>().Update(entity);
        }

        public List<T_Ticket_BonusPool> GetBonusPool(string gameCode, string gameType, string issuseNumber)
        {
        //  ..  Session.Clear();
            var query = from p in this.DB.CreateQuery<T_Ticket_BonusPool>()
                        where p.GameCode == gameCode
                        && (gameType == "" || p.GameType == gameType)
                        && p.IssuseNumber == issuseNumber
                        select p;
            return query.ToList();

        }

        public List<T_Ticket_BonusPool> GetBonusPool(string gameCode, string gameType, int length)
        {
            //Session.Clear();
            var query = from p in this.DB.CreateQuery<T_Ticket_BonusPool>()
                        where p.GameCode == gameCode
                        && (gameType == "" || p.GameType == gameType)
                        orderby p.CreateTime descending
                        select p;
            return query.Take(length).ToList();
        }
    }
}
