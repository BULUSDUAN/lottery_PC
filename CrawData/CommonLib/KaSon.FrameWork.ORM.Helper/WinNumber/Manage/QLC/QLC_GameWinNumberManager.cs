using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class QLC_GameWinNumberManager : DBbase
    {
        public void AddQLC_GameWinNumber(QLC_GameWinNumber entity)
        {
            LottertDataDB.GetDal<QLC_GameWinNumber>().Add(entity);
        }

        public QLC_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }
        public List<QLC_GameWinNumber> QueryQLC_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<QLC_GameWinNumber>()
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
