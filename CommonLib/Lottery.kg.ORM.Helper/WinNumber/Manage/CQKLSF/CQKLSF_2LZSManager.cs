using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_2LZSManager : DBbase
    {
        public void AddCQKLSF_2LZS(CQKLSF_2LZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_2LZS QueryLastCQKLSF_2LZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_2LZS> QueryCQKLSF_2LZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_2LZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_2LZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_2LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
