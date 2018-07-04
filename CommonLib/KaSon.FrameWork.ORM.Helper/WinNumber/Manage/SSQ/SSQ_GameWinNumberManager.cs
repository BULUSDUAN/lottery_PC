using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SSQ_GameWinNumberManager : DBbase
    {
        public void AddSSQ_GameWinNumber(SSQ_GameWinNumber entity)
        {
            LottertDataDB.GetDal<SSQ_GameWinNumber>().Add(entity);
        }

        public SSQ_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public List<SSQ_GameWinNumber> QuerySSQ_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<SSQ_GameWinNumber> QuerySSQ_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
