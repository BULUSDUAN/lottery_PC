using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HC1_JBZSManager : DBbase
    {
        public void AddHC1_JBZS(HC1_JBZS entity)
        {
            LottertDataDB.GetDal<HC1_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HC1_JBZS QueryLastHC1_JBZS()
        {
             
            return LottertDataDB.CreateQuery<HC1_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HC1_JBZS> QueryHC1_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HC1_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HC1_JBZS本期是否生成
        /// </summary>
        public int QueryHC1_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HC1_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
