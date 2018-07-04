using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_HZZSManager : DBbase
    {
        public void AddDF6_1_HZZS(DF6_1_HZZS entity)
        {
            LottertDataDB.GetDal<DF6_1_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_HZZS QueryLastDF6_1_HZZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_HZZS> QueryDF6_1_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_HZZS本期是否生成
        /// </summary>
        public int QueryDF6_1_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
