using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JLK3_GameWinNumberManager : DBbase
    {
        public void AddJLK3_GameWinNumber(JLK3_GameWinNumber entity)
        {
            LottertDataDB.GetDal<JLK3_GameWinNumber>().Add(entity);
        }

        public JLK3_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }
        public List<JLK3_GameWinNumber> QueryJLK3_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JLK3_GameWinNumber>()
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
