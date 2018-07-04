using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_3LZSManager : DBbase
    {
        public void AddHNKLSF_3LZS(HNKLSF_3LZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_3LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_3LZS QueryLastHNKLSF_3LZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_3LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_3LZS> QueryHNKLSF_3LZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_3LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_3LZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_3LZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_3LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
