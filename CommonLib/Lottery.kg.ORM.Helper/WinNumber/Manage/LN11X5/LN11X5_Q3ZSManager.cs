using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_Q3ZSManager : DBbase
    {
        public void AddLN11X5_Q3ZS(LN11X5_Q3ZS entity)
        {
            LottertDataDB.GetDal<LN11X5_Q3ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_Q3ZS QueryLastLN11X5_Q3ZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_Q3ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_Q3ZS> QueryLN11X5_Q3ZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_Q3ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_Q3ZS本期是否生成
        /// </summary>
        public int QueryLN11X5_Q3ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_Q3ZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
