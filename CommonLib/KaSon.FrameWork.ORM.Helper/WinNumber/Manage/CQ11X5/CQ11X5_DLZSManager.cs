using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_DLZSManager : DBbase
    {
        public void AddCQ11X5_DLZS(CQ11X5_DLZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_DLZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_DLZS QueryLastCQ11X5_DLZS()
        {         
            return LottertDataDB.CreateQuery<CQ11X5_DLZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_DLZS> QueryCQ11X5_DLZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_DLZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_DLZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_DLZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_DLZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
