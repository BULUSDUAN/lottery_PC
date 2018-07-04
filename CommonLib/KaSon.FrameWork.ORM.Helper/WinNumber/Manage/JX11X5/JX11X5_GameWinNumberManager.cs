using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JX11X5_GameWinNumberManager : DBbase
    {
        public void AddJX11X5_GameWinNumber(JX11X5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<JX11X5_GameWinNumber>().Add(entity);
        }

        public JX11X5_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<JX11X5_GameWinNumber> QueryJX11X5_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JX11X5_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<JX11X5_GameWinNumber> QueryJX11X5_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JX11X5_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<JX11X5_GameWinNumber> QueryJX11X5_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JX11X5_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
