using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_Q3JBZSManager : DBbase
    {
        public void AddGD11X5_Q3JBZS(GD11X5_Q3JBZS entity)
        {
            LottertDataDB.GetDal<GD11X5_Q3JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_Q3JBZS QueryLastGD11X5_Q3JBZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q3JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_Q3JBZS> QueryGD11X5_Q3JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_Q3JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_Q3JBZS本期是否生成
        /// </summary>
        public int QueryGD11X5_Q3JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q3JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
