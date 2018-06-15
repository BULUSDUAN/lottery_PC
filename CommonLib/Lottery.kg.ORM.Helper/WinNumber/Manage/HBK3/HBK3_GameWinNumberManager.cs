using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class HBK3_GameWinNumberManager : DBbase
    {
        public void AddHBK3_GameWinNumber(HBK3_GameWinNumber entity)
        {
            LottertDataDB.GetDal<HBK3_GameWinNumber>().Add(entity);
        }

        public HBK3_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<HBK3_GameWinNumber> QueryHBK3_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HBK3_GameWinNumber>()
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
