using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HC1_GameWinNumberManager : DBbase
    {
        public void AddHC1_GameWinNumber(HC1_GameWinNumber entity)
        {
            LottertDataDB.GetDal<HC1_GameWinNumber>().Add(entity);
        }

        public HC1_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HC1_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public List<HC1_GameWinNumber> QueryHC1_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HC1_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
