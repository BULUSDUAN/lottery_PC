using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_3X_DXZSManager : DBbase
    {
        public void AddCQSSC_3X_DXZS(CQSSC_3X_DXZS entity)
        {
            LottertDataDB.GetDal<CQSSC_3X_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_3X_DXZS QueryLastCQSSC_3X_DXZS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_3X_DXZS> QueryCQSSC_3X_DXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_3X_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_3X_DXZS本期是否生成
        /// </summary>
        public int QueryCQSSC_3X_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_3X_DXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
