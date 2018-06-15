using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_GameWinNumberManager : DBbase
    {
        public void AddGD11X5_GameWinNumber(GD11X5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<GD11X5_GameWinNumber>().Add(entity);
        }

        public GD11X5_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<GD11X5_GameWinNumber> QueryGD11X5_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<GD11X5_GameWinNumber> QueryGD11X5_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<GD11X5_GameWinNumber> QueryGD11X5_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
