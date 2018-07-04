using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CTZQ_T4CJQ_GameWinNumberManager : DBbase
    {
        public void AddCTZQ_T4CJQ_GameWinNumber(CTZQ_T4CJQ_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T4CJQ_GameWinNumber>().Add(entity);
        }

        public CTZQ_T4CJQ_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CTZQ_T4CJQ_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public void UpdateCTZQ_T4CJQ_GameWinNumber(CTZQ_T4CJQ_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T4CJQ_GameWinNumber>().Add(entity);
        }

        public List<CTZQ_T4CJQ_GameWinNumber> QueryCTZQ_T4CJQ_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T4CJQ_GameWinNumber>()
                        where s.GameType == "T4CJQ"
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<CTZQ_T4CJQ_GameWinNumber> QueryCTZQ_T4CJQ_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T4CJQ_GameWinNumber>()
                        where s.GameType == "T4CJQ"
                        && (s.CreateTime >= startTime && s.CreateTime < endTime)
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
