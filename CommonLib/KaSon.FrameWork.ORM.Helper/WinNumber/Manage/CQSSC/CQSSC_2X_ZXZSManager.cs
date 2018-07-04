using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_2X_ZXZSManager : DBbase
    {
        public void AddCQSSC_2X_ZXZS(CQSSC_2X_ZXZS entity)
        {
            LottertDataDB.GetDal<CQSSC_2X_ZXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_2X_ZXZS QueryLastCQSSC_2X_ZXZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_2X_ZXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_2X_ZXZS> QueryCQSSC_2X_ZXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_2X_ZXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_2X_ZXZS本期是否生成
        /// </summary>
        public int QueryCQSSC_2X_ZXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_2X_ZXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
