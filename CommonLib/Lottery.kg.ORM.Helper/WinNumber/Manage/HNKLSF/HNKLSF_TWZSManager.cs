using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_TWZSManager : DBbase
    {
        public void AddHNKLSF_TWZS(HNKLSF_TWZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_TWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_TWZS QueryLastHNKLSF_TWZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_TWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_TWZS> QueryHNKLSF_TWZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_TWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_TWZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_TWZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_TWZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
