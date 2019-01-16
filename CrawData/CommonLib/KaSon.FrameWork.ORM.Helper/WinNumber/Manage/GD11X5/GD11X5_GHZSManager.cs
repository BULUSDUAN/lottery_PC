using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_GHZSManager : DBbase
    {
        public void AddGD11X5_GHZS(GD11X5_GHZS entity)
        {
            LottertDataDB.GetDal<GD11X5_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_GHZS QueryLastGD11X5_GHZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_GHZS> QueryGD11X5_GHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_GHZS本期是否生成
        /// </summary>
        public int QueryGD11X5_GHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_GHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
