using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_5X_HZZSManager : DBbase
    {
        public void AddJXSSC_5X_HZZS(JXSSC_5X_HZZS entity)
        {
            LottertDataDB.GetDal<JXSSC_5X_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_5X_HZZS QueryLastJXSSC_5X_HZZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_5X_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_5X_HZZS> QueryJXSSC_5X_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_5X_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_5X_HZZS本期是否生成
        /// </summary>
        public int QueryJXSSC_5X_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_5X_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
