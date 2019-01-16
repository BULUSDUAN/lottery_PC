using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_2LZSManager : DBbase
    {
        public void AddYDJ11_2LZS(YDJ11_2LZS entity)
        {
            LottertDataDB.GetDal<YDJ11_2LZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_2LZS QueryLastYDJ11_2LZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_2LZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_2LZS> QueryYDJ11_2LZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_2LZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_2LZS本期是否生成
        /// </summary>
        public int QueryYDJ11_2LZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_2LZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
