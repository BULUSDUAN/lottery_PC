using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_KDManager : DBbase
    {
        public void AddCQSSC_3X_KD(CQSSC_3X_KD entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_KD>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_KD QueryLastCQSSC_3X_KD()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_KD>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_KD> QueryCQSSC_3X_KD(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_KD>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_KD本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_KDIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_KD>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
