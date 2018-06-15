using System;
using System.Collections.Generic;
using System.Linq;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_GameWinNumberManager : DBbase
    {
        public void AddCQ11X5_GameWinNumber(CQ11X5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CQ11X5_GameWinNumber>().Add(entity);
        }

        public CQ11X5_GameWinNumber QueryWinNumber(string issuseNumber)
        { 
            return LottertDataDB.CreateQuery<CQ11X5_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public List<CQ11X5_GameWinNumber> QueryCQ11X5_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {           
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
