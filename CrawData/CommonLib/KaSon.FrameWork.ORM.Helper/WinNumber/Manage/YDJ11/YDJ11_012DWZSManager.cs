using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_012DWZSManager : DBbase
    {
        public void AddYDJ11_012DWZS(YDJ11_012DWZS entity)
        {
            LottertDataDB.GetDal<YDJ11_012DWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_012DWZS QueryLastYDJ11_012DWZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_012DWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_012DWZS> QueryYDJ11_012DWZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_012DWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_012DWZS本期是否生成
        /// </summary>
        public int QueryYDJ11_012DWZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_012DWZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
