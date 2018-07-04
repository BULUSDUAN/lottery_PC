using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_2LZSManager : DBbase
    {
        public void AddLN11X5_2LZS(LN11X5_2LZS entity)
        {
            LottertDataDB.GetDal<LN11X5_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_2LZS QueryLastLN11X5_2LZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_2LZS> QueryLN11X5_2LZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_2LZS本期是否生成
        /// </summary>
        public int QueryLN11X5_2LZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_2LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
