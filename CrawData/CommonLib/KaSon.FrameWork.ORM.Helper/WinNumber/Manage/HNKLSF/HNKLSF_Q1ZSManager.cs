using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_Q1ZSManager : DBbase
    {
        public void AddHNKLSF_Q1ZS(HNKLSF_Q1ZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_Q1ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_Q1ZS QueryLastHNKLSF_Q1ZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_Q1ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_Q1ZS> QueryHNKLSF_Q1ZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_Q1ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_Q1ZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_Q1ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_Q1ZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
