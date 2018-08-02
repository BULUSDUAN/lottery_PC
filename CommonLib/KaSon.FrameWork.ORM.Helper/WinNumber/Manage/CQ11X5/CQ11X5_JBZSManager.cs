using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_JBZSManager : DBbase
    {
        public void AddCQ11X5_JBZS(CQ11X5_JBZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_JBZS QueryLastCQ11X5_JBZS()
        {            
            return LottertDataDB.CreateQuery<CQ11X5_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_JBZS> QueryCQ11X5_JBZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_JBZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_JBZSIssuseNumber(string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<CQ11X5_JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
