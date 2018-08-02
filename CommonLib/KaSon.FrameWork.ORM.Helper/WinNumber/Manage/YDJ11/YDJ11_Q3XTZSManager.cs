using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q3XTZSManager : DBbase
    {
        public void AddYDJ11_Q3XTZS(YDJ11_Q3XTZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q3XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q3XTZS QueryLastYDJ11_Q3XTZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q3XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q3XTZS> QueryYDJ11_Q3XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q3XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q3XTZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q3XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q3XTZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
