using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderLottery.Service.IModuleServices
{
    [ServiceBundle("Order/{Service}")]
   public interface IOrderService: IServiceKey
    {
        [Service(Date = "2018-06-04", Director = "Debug", Name = "中奖查询")]
        BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model);
    }
}
