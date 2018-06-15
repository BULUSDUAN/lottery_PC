using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_JOZSManager : DBbase
    {
        public void AddLN11X5_JOZS(LN11X5_JOZS entity)
        {
            LottertDataDB.GetDal<LN11X5_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_JOZS QueryLastLN11X5_JOZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_JOZS> QueryLN11X5_JOZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_JOZS本期是否生成
        /// </summary>
        public int QueryLN11X5_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_JOZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
