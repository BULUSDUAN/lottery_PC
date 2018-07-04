using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_JOZSManager : DBbase
    {
        public void AddDF6_1_JOZS(DF6_1_JOZS entity)
        {
            LottertDataDB.GetDal<DF6_1_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_JOZS QueryLastDF6_1_JOZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_JOZS> QueryDF6_1_JOZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_JOZS本期是否生成
        /// </summary>
        public int QueryDF6_1_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_JOZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
