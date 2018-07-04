using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_Q1JBZSManager : DBbase
    {
        public void AddGD11X5_Q1JBZS(GD11X5_Q1JBZS entity)
        {
            LottertDataDB.GetDal<GD11X5_Q1JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_Q1JBZS QueryLastGD11X5_Q1JBZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q1JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_Q1JBZS> QueryGD11X5_Q1JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_Q1JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_Q1JBZS本期是否生成
        /// </summary>
        public int QueryGD11X5_Q1JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q1JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
