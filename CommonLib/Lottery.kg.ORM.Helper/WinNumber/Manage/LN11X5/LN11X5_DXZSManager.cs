using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_DXZSManager : DBbase
    {
        public void AddLN11X5_DXZS(LN11X5_DXZS entity)
        {
            LottertDataDB.GetDal<LN11X5_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_DXZS QueryLastLN11X5_DXZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_DXZS> QueryLN11X5_DXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_DXZS本期是否生成
        /// </summary>
        public int QueryLN11X5_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_DXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
