using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_3LZSManager : DBbase
    {
        public void AddCQKLSF_3LZS(CQKLSF_3LZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_3LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_3LZS QueryLastCQKLSF_3LZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_3LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_3LZS> QueryCQKLSF_3LZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_3LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_3LZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_3LZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_3LZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
