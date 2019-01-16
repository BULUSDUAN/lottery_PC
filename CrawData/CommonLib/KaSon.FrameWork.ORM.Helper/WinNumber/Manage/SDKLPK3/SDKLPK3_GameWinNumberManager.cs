using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SDKLPK3_GameWinNumberManager : DBbase
    {
        public void AddSDKLPK3_GameWinNumber(SDKLPK3_GameWinNumber entity)
        {
            LottertDataDB.GetDal<SDKLPK3_GameWinNumber>().Add(entity);
        }

        public SDKLPK3_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public List<SDKLPK3_GameWinNumber> QuerySDKLPK3_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SDKLPK3_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<SDKLPK3_GameWinNumber> QuerySDKLPK3_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SDKLPK3_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<SDKLPK3_GameWinNumber> QuerySDKLPK3_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SDKLPK3_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
