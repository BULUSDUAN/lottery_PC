using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_GameWinNumberManager : DBbase
    {
        public void AddHNKLSF_GameWinNumber(HNKLSF_GameWinNumber entity)
        {
            LottertDataDB.GetDal<HNKLSF_GameWinNumber>().Add(entity);
        }

        public HNKLSF_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<HNKLSF_GameWinNumber> QueryHNKLSF_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
