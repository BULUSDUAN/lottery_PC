using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GDKLSF_GameWinNumberManager : DBbase
    {
        public void AddGDKLSF_GameWinNumber(GDKLSF_GameWinNumber entity)
        {
            LottertDataDB.GetDal<GDKLSF_GameWinNumber>().Add(entity);
        }

        public GDKLSF_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<GDKLSF_GameWinNumber> QueryGDKLSF_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GDKLSF_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<GDKLSF_GameWinNumber> QueryGDKLSF_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GDKLSF_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<GDKLSF_GameWinNumber> QueryGDKLSF_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GDKLSF_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
