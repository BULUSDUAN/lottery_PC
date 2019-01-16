using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_ZHZSManager : DBbase
    {
        public void AddCQSSC_3X_ZHZS(CQSSC_3X_ZHZS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_ZHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_ZHZS QueryLastCQSSC_3X_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_ZHZS> QueryCQSSC_3X_ZHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_ZHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_ZHZS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_ZHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
