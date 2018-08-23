using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CTZQ_T14C_GameWinNumberManager : DBbase
    {
        public void AddCTZQ_T14C_GameWinNumber(CTZQ_T14C_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T14C_GameWinNumber>().Add(entity);
        }

        public CTZQ_T14C_GameWinNumber QueryWinNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CTZQ_T14C_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public void UpdateCTZQ_T14C_GameWinNumber(CTZQ_T14C_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CTZQ_T14C_GameWinNumber>().Update(entity);
        }

        public List<CTZQ_T14C_GameWinNumber> QueryCTZQ_T14C_TR9_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {            
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T14C_GameWinNumber>()
                        where s.GameType == "T14C"                        
                        select s;
            totalCount = query.Count();
            return query.OrderByDescending(x=>x.IssuseNumber).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<CTZQ_T14C_GameWinNumber> QueryCTZQ_TR9_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T14C_GameWinNumber>()
                        where s.GameType == "TR9"
                        select s;
            totalCount = query.Count();
            return query.OrderByDescending(x => x.IssuseNumber).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<CTZQ_T14C_GameWinNumber> QueryCTZQ_T14C_TR9_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            var query = from s in LottertDataDB.CreateQuery<CTZQ_T14C_GameWinNumber>()
                        where s.GameType == "T14C"
                        && (s.CreateTime >= startTime && s.CreateTime < endTime)
                        select s;
            totalCount = query.Count();
            return query.OrderByDescending(x => x.IssuseNumber).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
