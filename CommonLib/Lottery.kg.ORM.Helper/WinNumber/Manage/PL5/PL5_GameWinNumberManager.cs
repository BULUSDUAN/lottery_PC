using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class PL5_GameWinNumberManager : DBbase
    {
        public void AddPL5_GameWinNumber(PL5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<PL5_GameWinNumber>().Add(entity);
        }

        public PL5_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<PL5_GameWinNumber> QueryPL5_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<PL5_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
