using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_KDZSManager : DBbase
    {
        public void AddGD11X5_KDZS(GD11X5_KDZS entity)
        {
            LottertDataDB.GetDal<GD11X5_KDZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_KDZS QueryLastGD11X5_KDZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_KDZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_KDZS> QueryGD11X5_KDZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_KDZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_KDZS本期是否生成
        /// </summary>
        public int QueryGD11X5_KDZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_KDZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
