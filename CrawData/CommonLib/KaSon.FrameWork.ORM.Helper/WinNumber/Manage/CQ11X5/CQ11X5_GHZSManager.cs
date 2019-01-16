using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_GHZSManager : DBbase
    {
        public void AddCQ11X5_GHZS(CQ11X5_GHZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_GHZS QueryLastCQ11X5_GHZS()
        {
            
            return LottertDataDB.CreateQuery<CQ11X5_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_GHZS> QueryCQ11X5_GHZS(int index)
        {
            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_GHZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_GHZSIssuseNumber(string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<CQ11X5_GHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
