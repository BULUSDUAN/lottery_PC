using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_DXZSManager : DBbase
    {
        public void AddHNKLSF_DXZS(HNKLSF_DXZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_DXZS QueryLastHNKLSF_DXZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_DXZS> QueryHNKLSF_DXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_DXZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_DXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
