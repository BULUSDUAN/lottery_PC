using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_Q3ZSManager : DBbase
    {
        public void AddHNKLSF_Q3ZS(HNKLSF_Q3ZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_Q3ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_Q3ZS QueryLastHNKLSF_Q3ZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_Q3ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_Q3ZS> QueryHNKLSF_Q3ZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_Q3ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_Q3ZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_Q3ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_Q3ZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
