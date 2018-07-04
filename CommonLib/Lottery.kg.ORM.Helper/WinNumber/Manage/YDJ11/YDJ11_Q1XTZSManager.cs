using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q1XTZSManager : DBbase
    {
        public void AddYDJ11_Q1XTZS(YDJ11_Q1XTZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q1XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q1XTZS QueryLastYDJ11_Q1XTZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q1XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q1XTZS> QueryYDJ11_Q1XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q1XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q1XTZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q1XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q1XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
