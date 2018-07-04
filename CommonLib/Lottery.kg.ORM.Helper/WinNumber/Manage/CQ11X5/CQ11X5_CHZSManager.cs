using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_CHZSManager : DBbase
    {
        public void AddCQ11X5_CHZS(CQ11X5_CHZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_CHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_CHZS QueryLastCQ11X5_CHZS()
        {           
            return LottertDataDB.CreateQuery<CQ11X5_CHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_CHZS> QueryCQ11X5_CHZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_CHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_CHZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_CHZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_CHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
