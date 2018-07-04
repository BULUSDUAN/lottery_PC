using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_2LZSManager : DBbase
    {
        public void AddHNKLSF_2LZS(HNKLSF_2LZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_2LZS QueryLastHNKLSF_2LZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_2LZS> QueryHNKLSF_2LZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_2LZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_2LZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_2LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
