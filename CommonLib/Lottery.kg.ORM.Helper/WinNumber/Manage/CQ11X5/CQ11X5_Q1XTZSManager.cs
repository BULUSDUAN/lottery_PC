using System;
using System.Collections.Generic;
using System.Linq;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_Q1XTZSManager : DBbase
    {
        public void AddCQ11X5_Q1XTZS(CQ11X5_Q1XTZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_Q1XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_Q1XTZS QueryLastCQ11X5_Q1XTZS()
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q1XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_Q1XTZS> QueryCQ11X5_Q1XTZS(int index)
        {
            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_Q1XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_Q1XTZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_Q1XTZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q1XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
