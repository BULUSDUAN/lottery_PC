using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_HZZSManager : DBbase
    {
        public void AddCQSSC_3X_HZZS(CQSSC_3X_HZZS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_HZZS QueryLastCQSSC_3X_HZZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_HZZS> QueryCQSSC_3X_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_HZZS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
