using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_XTZSManager : DBbase
    {
        public void AddYDJ11_XTZS(YDJ11_XTZS entity)
        {
            LottertDataDB.GetDal<YDJ11_XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_XTZS QueryLastYDJ11_XTZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_XTZS> QueryYDJ11_XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_XTZS本期是否生成
        /// </summary>
        public int QueryYDJ11_XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_XTZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
