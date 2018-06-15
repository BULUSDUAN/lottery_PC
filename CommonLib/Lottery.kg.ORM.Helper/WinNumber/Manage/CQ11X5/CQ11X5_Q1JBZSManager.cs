using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_Q1JBZSManager : DBbase
    {
        public void AddCQ11X5_Q1JBZS(CQ11X5_Q1JBZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_Q1JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_Q1JBZS QueryLastCQ11X5_Q1JBZS()
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q1JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_Q1JBZS> QueryCQ11X5_Q1JBZS(int index)
        {            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_Q1JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_Q1JBZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_Q1JBZSIssuseNumber(string issuseNumber)
        {            
            return LottertDataDB.CreateQuery<CQ11X5_Q1JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
