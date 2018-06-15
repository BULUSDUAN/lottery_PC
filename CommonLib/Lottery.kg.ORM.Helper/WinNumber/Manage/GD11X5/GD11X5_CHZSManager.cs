using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_CHZSManager : DBbase
    {
        public void AddGD11X5_CHZS(GD11X5_CHZS entity)
        {
            LottertDataDB.GetDal<GD11X5_CHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_CHZS QueryLastGD11X5_CHZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_CHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_CHZS> QueryGD11X5_CHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_CHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_CHZS本期是否生成
        /// </summary>
        public int QueryGD11X5_CHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_CHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
