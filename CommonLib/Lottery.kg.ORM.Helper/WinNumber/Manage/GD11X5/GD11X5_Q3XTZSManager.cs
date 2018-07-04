using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_Q3XTZSManager : DBbase
    {
        public void AddGD11X5_Q3XTZS(GD11X5_Q3XTZS entity)
        {
            LottertDataDB.GetDal<GD11X5_Q3XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_Q3XTZS QueryLastGD11X5_Q3XTZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q3XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_Q3XTZS> QueryGD11X5_Q3XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_Q3XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_Q3XTZS本期是否生成
        /// </summary>
        public int QueryGD11X5_Q3XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q3XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
