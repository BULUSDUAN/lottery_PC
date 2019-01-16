using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_Q3JBZSManager : DBbase
    {
        public void AddCQ11X5_Q3JBZS(CQ11X5_Q3JBZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_Q3JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_Q3JBZS QueryLastCQ11X5_Q3JBZS()
        {
            return LottertDataDB.CreateQuery<CQ11X5_Q3JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_Q3JBZS> QueryCQ11X5_Q3JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_Q3JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_Q3JBZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_Q3JBZSIssuseNumber(string issuseNumber)
        {
            
            return LottertDataDB.CreateQuery<CQ11X5_Q3JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
