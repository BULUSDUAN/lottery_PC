using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_C3YSManager : DBbase
    {
        public void AddCQSSC_3X_C3YS(CQSSC_3X_C3YS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_C3YS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_C3YS QueryLastCQSSC_3X_C3YS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_C3YS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_C3YS> QueryCQSSC_3X_C3YS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_C3YS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_C3YS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_C3YSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_C3YS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
