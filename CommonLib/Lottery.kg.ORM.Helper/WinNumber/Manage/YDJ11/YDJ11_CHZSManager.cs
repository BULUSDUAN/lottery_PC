using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_CHZSManager : DBbase
    {
        public void AddYDJ11_CHZS(YDJ11_CHZS entity)
        {
            LottertDataDB.GetDal<YDJ11_CHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_CHZS QueryLastYDJ11_CHZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_CHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_CHZS> QueryYDJ11_CHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_CHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_CHZS本期是否生成
        /// </summary>
        public int QueryYDJ11_CHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_CHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
