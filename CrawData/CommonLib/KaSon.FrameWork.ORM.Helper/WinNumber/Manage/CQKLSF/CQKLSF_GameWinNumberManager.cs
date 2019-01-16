using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_GameWinNumberManager : DBbase
    {
        public void AddCQKLSF_GameWinNumber(CQKLSF_GameWinNumber entity)
        {
            LottertDataDB.GetDal<CQKLSF_GameWinNumber>().Add(entity);
        }

        public CQKLSF_GameWinNumber QueryWinNumber(string issuseNumber)
        {
            
            return LottertDataDB.CreateQuery<CQKLSF_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public List<CQKLSF_GameWinNumber> QueryCQKLSF_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
