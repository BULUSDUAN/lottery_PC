using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_DLZSManager : DBbase
    {
        public void AddYDJ11_DLZS(YDJ11_DLZS entity)
        {
            LottertDataDB.GetDal<YDJ11_DLZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_DLZS QueryLastYDJ11_DLZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_DLZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_DLZS> QueryYDJ11_DLZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_DLZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_DLZS本期是否生成
        /// </summary>
        public int QueryYDJ11_DLZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_DLZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
