using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_JOZSManager : DBbase
    {
        public void AddCQSSC_3X_JOZS(CQSSC_3X_JOZS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_JOZS QueryLastCQSSC_3X_JOZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_JOZS> QueryCQSSC_3X_JOZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_JOZS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_JOZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
