
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using static EntityModel.CoreModel.ReportInfo;

namespace KaSon.FrameWork.ORM.Helper
{
   public class SqlQueryBusiness:DBbase
    {
        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            var manager = new SqlQueryManager();
            
             return manager.QueryYqidRegisterByAgentId(AgentId);
            
        }

        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
           var manager = new SqlQueryManager();
            
          return manager.QueryOrderDetailBySchemeId(schemeId);
            
        }

        public int QueryTogetherFollowerCount(string createUserId)
        {
             var manager = new SqlQueryManager();
            
             return manager.QueryTogetherFollowerCount(createUserId);
            
        }

        #region 过关统计

        /// <summary>
        /// 查询过关统计
        /// </summary>
        public SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool? isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new SportsOrder_GuoGuanInfoCollection();
            var totalCount = 0;
            result.ReportItemList.AddRange(new SqlQueryManager().QueryReportInfoList_GuoGuan(isVirtualOrder, category, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize));
            result.TotalCount = totalCount;
            return result;
        }

        #endregion
    }
}
