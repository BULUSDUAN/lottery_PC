using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_2X_ZXZSManager : DBbase
    {
        public void AddJXSSC_2X_ZXZS(JXSSC_2X_ZXZS entity)
        {
            LottertDataDB.GetDal<JXSSC_2X_ZXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_2X_ZXZS QueryLastJXSSC_2X_ZXZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_ZXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_2X_ZXZS> QueryJXSSC_2X_ZXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_2X_ZXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_2X_ZXZS本期是否生成
        /// </summary>
        public int QueryJXSSC_2X_ZXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_ZXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
