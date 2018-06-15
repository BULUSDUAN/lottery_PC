using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_2X_HZZSManager : DBbase
    {
        public void AddJXSSC_2X_HZZS(JXSSC_2X_HZZS entity)
        {
            LottertDataDB.GetDal<JXSSC_2X_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_2X_HZZS QueryLastJXSSC_2X_HZZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_2X_HZZS> QueryJXSSC_2X_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_2X_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_2X_HZZS本期是否生成
        /// </summary>
        public int QueryJXSSC_2X_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
