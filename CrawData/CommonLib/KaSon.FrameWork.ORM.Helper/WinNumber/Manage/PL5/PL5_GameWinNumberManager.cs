using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class PL5_GameWinNumberManager : DBbase
    {
        public void AddPL5_GameWinNumber(PL5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<PL5_GameWinNumber>().Add(entity);
        }

        public PL5_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
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
