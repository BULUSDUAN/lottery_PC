using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_ZHZSManager : DBbase
    {
        public void AddDF6_1_ZHZS(DF6_1_ZHZS entity)
        {
            LottertDataDB.GetDal<DF6_1_ZHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_ZHZS QueryLastDF6_1_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_ZHZS> QueryDF6_1_ZHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_ZHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_ZHZS本期是否生成
        /// </summary>
        public int QueryDF6_1_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_ZHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
