using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_GHZSManager : DBbase
    {
        public void AddHNKLSF_GHZS(HNKLSF_GHZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_GHZS QueryLastHNKLSF_GHZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_GHZS> QueryHNKLSF_GHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_GHZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_GHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_GHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
