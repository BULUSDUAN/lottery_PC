using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CTZQ_T6BQC_GameWinNumberManager : DBbase
    {
        public void AddCTZQ_T6BQC_GameWinNumber(CTZQ_T6BQC_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T6BQC_GameWinNumber>().Add(entity);
        }

        public CTZQ_T6BQC_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CTZQ_T6BQC_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }

        public void AddCTZQ_T4CJQ_GameWinNumber(CTZQ_T4CJQ_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T4CJQ_GameWinNumber>().Add(entity);
        }

        public void UpdateCTZQ_T6BQC_GameWinNumber(CTZQ_T6BQC_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T6BQC_GameWinNumber>().Add(entity);
        }

        public List<CTZQ_T6BQC_GameWinNumber> QueryCTZQ_T6BQC_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T6BQC_GameWinNumber>()
                        where s.GameType == "T6BQC"
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<CTZQ_T6BQC_GameWinNumber> QueryCTZQ_T6BQC_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T6BQC_GameWinNumber>()
                        where s.GameType == "T6BQC"
                        && (s.CreateTime >= startTime && s.CreateTime < endTime)
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
