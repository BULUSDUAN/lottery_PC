using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class FC3D_GameWinNumberManager : DBbase
    {
        public void AddFC3D_GameWinNumber(FC3D_GameWinNumber entity)
        {
            LottertDataDB.GetDal<FC3D_GameWinNumber>().Add(entity);
        }

        public FC3D_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<FC3D_GameWinNumber> QueryFC3D_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<FC3D_GameWinNumber> QueryFC3D_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
