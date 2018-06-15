using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_JBZSManager : DBbase
    {
        public void AddGD11X5_JBZS(GD11X5_JBZS entity)
        {
            LottertDataDB.GetDal<GD11X5_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_JBZS QueryLastGD11X5_JBZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_JBZS> QueryGD11X5_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_JBZS本期是否生成
        /// </summary>
        public int QueryGD11X5_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
