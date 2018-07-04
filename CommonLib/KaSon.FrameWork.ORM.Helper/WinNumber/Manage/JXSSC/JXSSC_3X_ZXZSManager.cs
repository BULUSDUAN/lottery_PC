using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_ZXZSManager : DBbase
    {
        public void AddJXSSC_3X_ZXZS(JXSSC_3X_ZXZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_ZXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_ZXZS QueryLastJXSSC_3X_ZXZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_ZXZS> QueryJXSSC_3X_ZXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_ZXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_ZXZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_ZXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
