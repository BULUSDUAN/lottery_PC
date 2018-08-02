using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_Q2XTZSManager : DBbase
    {
        public void AddGD11X5_Q2XTZS(GD11X5_Q2XTZS entity)
        {
            LottertDataDB.GetDal<GD11X5_Q2XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_Q2XTZS QueryLastGD11X5_Q2XTZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q2XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_Q2XTZS> QueryGD11X5_Q2XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_Q2XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_Q2XTZS本期是否生成
        /// </summary>
        public int QueryGD11X5_Q2XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_Q2XTZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
