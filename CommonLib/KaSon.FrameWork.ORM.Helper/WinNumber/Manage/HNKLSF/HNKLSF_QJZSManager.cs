using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_QJZSManager : DBbase
    {
        public void AddHNKLSF_QJZS(HNKLSF_QJZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_QJZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_QJZS QueryLastHNKLSF_QJZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_QJZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_QJZS> QueryHNKLSF_QJZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_QJZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_QJZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_QJZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_QJZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
