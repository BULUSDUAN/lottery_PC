using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SDQYH_GameWinNumberManager : DBbase
    {
        public void AddSDQYH_GameWinNumber(SDQYH_GameWinNumber entity)
        {
            LottertDataDB.GetDal<SDQYH_GameWinNumber>().Add(entity);
        }

        public SDQYH_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public List<SDQYH_GameWinNumber> QuerySDQYH_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SDQYH_GameWinNumber>()
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
