using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_ZuXZSManager : DBbase
    {
        public void AddCQSSC_3X_ZuXZS(CQSSC_3X_ZuXZS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_ZuXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_ZuXZS QueryLastCQSSC_3X_ZuXZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_ZuXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_ZuXZS> QueryCQSSC_3X_ZuXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_ZuXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_ZuXZS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_ZuXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_ZuXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
