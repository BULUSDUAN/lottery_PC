using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_Q1ZSManager : DBbase
    {
        public void AddCQKLSF_Q1ZS(CQKLSF_Q1ZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_Q1ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_Q1ZS QueryLastCQKLSF_Q1ZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_Q1ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_Q1ZS> QueryCQKLSF_Q1ZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_Q1ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_JBZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_Q1ZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_Q1ZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
