using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_HZZSManager : DBbase
    {
        public void AddCQ11X5_HZZS(CQ11X5_HZZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_HZZS QueryLastCQ11X5_HZZS()
        {
            return LottertDataDB.CreateQuery<CQ11X5_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_HZZS> QueryCQ11X5_HZZS(int index)
        {            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_HZZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_HZZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
