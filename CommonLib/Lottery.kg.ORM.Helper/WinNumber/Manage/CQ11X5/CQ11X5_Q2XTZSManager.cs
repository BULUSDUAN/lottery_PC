using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_Q2XTZSManager : DBbase
    {
        public void AddCQ11X5_Q2XTZS(CQ11X5_Q2XTZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_Q2XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_Q2XTZS QueryLastCQ11X5_Q2XTZS()
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q2XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_Q2XTZS> QueryCQ11X5_Q2XTZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_Q2XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_Q2XTZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_Q2XTZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q2XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
