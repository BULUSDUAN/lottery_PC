using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_HZZSManager : DBbase
    {
        public void AddLN11X5_HZZS(LN11X5_HZZS entity)
        {
            LottertDataDB.GetDal<LN11X5_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_HZZS QueryLastLN11X5_HZZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_HZZS> QueryLN11X5_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_HZZS本期是否生成
        /// </summary>
        public int QueryLN11X5_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
