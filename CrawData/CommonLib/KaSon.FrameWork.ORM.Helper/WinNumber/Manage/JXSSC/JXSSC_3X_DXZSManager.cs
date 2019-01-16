using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_DXZSManager : DBbase
    {
        public void AddJXSSC_3X_DXZS(JXSSC_3X_DXZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_DXZS QueryLastJXSSC_3X_DXZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_DXZS> QueryJXSSC_3X_DXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_DXZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_DXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
