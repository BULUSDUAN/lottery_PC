using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_2LZSManager : DBbase
    {
        public void AddGD11X5_2LZS(GD11X5_2LZS entity)
        {
            LottertDataDB.GetDal<GD11X5_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_2LZS QueryLastGD11X5_2LZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_2LZS> QueryGD11X5_2LZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_2LZS本期是否生成
        /// </summary>
        public int QueryGD11X5_2LZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_2LZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
