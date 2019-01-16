using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_Q3XTZSManager : DBbase
    {
        public void AddCQ11X5_Q3XTZS(CQ11X5_Q3XTZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_Q3XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_Q3XTZS QueryLastCQ11X5_Q3XTZS()
        {           
            return LottertDataDB.CreateQuery<CQ11X5_Q3XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_Q3XTZS> QueryCQ11X5_Q3XTZS(int index)
        {
           
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_Q3XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_Q3XTZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_Q3XTZSIssuseNumber(string issuseNumber)
        {
            
            return LottertDataDB.CreateQuery<CQ11X5_Q3XTZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
