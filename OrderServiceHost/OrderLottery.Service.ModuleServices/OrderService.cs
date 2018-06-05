using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Helper;
using Lottery.Kg.ORM.Helper;
using OrderLottery.Service.IModuleServices;
using OrderLottery.Service.ModuleBaseServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace OrderLottery.Service.ModuleServices
{
    [ModuleName("Order")]
    public class OrderService:DBbase, IOrderService
    {
        IKgLog log = null;
        public OrderService()
        {
            log = new Log4Log();
        }
        /// <summary>
        /// 中奖查询
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model)
        {
            Model.pageIndex = Model.pageIndex < 0 ? 0 : Model.pageIndex;
            Model.pageSize = Model.pageSize > Model.MaxPageSize ? Model.MaxPageSize : Model.pageSize;
            try
            {
                string sql = SqlModule.UserSystemModule.FirstOrDefault(p => p.Key == "P_Order_QueryBonusOrderList").SQL;
                var query = DB.CreateSQLQuery(sql)
                    .SetString("@UserId", Model.userId)
                    .SetString("@GameCode", Model.gameCode)
                    .SetString("@GameType", Model.gameType)
                    .SetString("@IssuseNumber", Model.issuseNumber)
                    .SetInt("@CompleteData", Model.completeData)
                    .SetString("@Key_UID_UName_SchemeId", Model.key)
                    .SetInt("@PageIndex", Model.pageIndex)
                    .SetInt("@PageSize", Model.pageSize)
                    .SetInt("@TotalCount", 0);
                return query as BonusOrderInfoCollection;
            }
            catch (Exception ex)
            {
                return null;                
            }
            
        }

    }
}
