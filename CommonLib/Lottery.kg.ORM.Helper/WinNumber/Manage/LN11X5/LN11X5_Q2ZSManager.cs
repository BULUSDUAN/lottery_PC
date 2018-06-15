using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_Q2ZSManager : DBbase
    {
        public void AddLN11X5_Q2ZS(LN11X5_Q2ZS entity)
        {
            LottertDataDB.GetDal<LN11X5_Q2ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_Q2ZS QueryLastLN11X5_Q2ZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_Q2ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_Q2ZS> QueryLN11X5_Q2ZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_Q2ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_Q2ZS本期是否生成
        /// </summary>
        public int QueryLN11X5_Q2ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_Q2ZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
