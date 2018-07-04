using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_2LZSManager : DBbase
    {
        public void AddCQ11X5_2LZS(CQ11X5_2LZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_2LZS QueryLastCQ11X5_2LZS()
        {
           
            return LottertDataDB.CreateQuery<CQ11X5_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_2LZS> QueryCQ11X5_2LZS(int index)
        {
            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_2LZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_2LZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_2LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
