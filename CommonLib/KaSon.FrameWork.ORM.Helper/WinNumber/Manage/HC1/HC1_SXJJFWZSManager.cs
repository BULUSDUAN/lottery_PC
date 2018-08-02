using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HC1_SXJJFWZSManager : DBbase
    {
        public void AddHC1_SXJJFWZS(HC1_SXJJFWZS entity)
        {
            LottertDataDB.GetDal<HC1_SXJJFWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HC1_SXJJFWZS QueryLastHC1_SXJJFWZS()
        {
             
            return LottertDataDB.CreateQuery<HC1_SXJJFWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HC1_SXJJFWZS> QueryHC1_SXJJFWZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HC1_SXJJFWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HC1_SXJJFWZS本期是否生成
        /// </summary>
        public int QueryHC1_SXJJFWZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HC1_SXJJFWZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
