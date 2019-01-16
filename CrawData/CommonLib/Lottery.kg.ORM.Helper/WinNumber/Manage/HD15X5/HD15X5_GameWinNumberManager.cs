using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HD15X5_GameWinNumberManager : DBbase
    {
        public void AddHD15X5_GameWinNumber(HD15X5_GameWinNumber entity)
        {
            LottertDataDB.GetDal<HD15X5_GameWinNumber>().Add(entity);
        }

        public HD15X5_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_GameWinNumber>().FirstOrDefault(p => p.IssuseNumber == issuseNumber);
        }
        public List<HD15X5_GameWinNumber> QueryHD15X5_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HD15X5_GameWinNumber>()
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
