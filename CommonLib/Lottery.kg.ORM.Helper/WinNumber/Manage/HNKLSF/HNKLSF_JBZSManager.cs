using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_JBZSManager : DBbase
    {
        public void AddHNKLSF_JBZS(HNKLSF_JBZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_JBZS QueryLastHNKLSF_JBZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_JBZS> QueryHNKLSF_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_JBZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
