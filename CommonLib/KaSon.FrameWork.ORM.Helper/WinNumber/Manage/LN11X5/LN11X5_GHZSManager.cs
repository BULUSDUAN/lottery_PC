using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_GHZSManager : DBbase
    {
        public void AddLN11X5_GHZS(LN11X5_GHZS entity)
        {
            LottertDataDB.GetDal<LN11X5_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_GHZS QueryLastLN11X5_GHZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_GHZS> QueryLN11X5_GHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_GHZS本期是否生成
        /// </summary>
        public int QueryLN11X5_GHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_GHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
