using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_JBZSManager : DBbase
    {
        public void AddDF6_1_JBZS(DF6_1_JBZS entity)
        {
            LottertDataDB.GetDal<DF6_1_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_JBZS QueryLastDF6_1_JBZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_JBZS> QueryDF6_1_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_JBZS本期是否生成
        /// </summary>
        public int QueryDF6_1_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
