using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_CHZSManager : DBbase
    {
        public void AddLN11X5_CHZS(LN11X5_CHZS entity)
        {
            LottertDataDB.GetDal<LN11X5_CHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_CHZS QueryLastLN11X5_CHZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_CHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_CHZS> QueryLN11X5_CHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_CHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_CHZS本期是否生成
        /// </summary>
        public int QueryLN11X5_CHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_CHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
