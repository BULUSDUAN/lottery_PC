
using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_012DWZSManager : DBbase
    {
        public void AddCQ11X5_012DWZS(CQ11X5_012DWZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_012DWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_012DWZS QueryLastCQ11X5_012DWZS()
        {
            return DB.CreateQuery<CQ11X5_012DWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_012DWZS> QueryCQ11X5_012DWZS(int index)
        {
          
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_012DWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_012DWZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_012DWZSIssuseNumber(string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<CQ11X5_012DWZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
