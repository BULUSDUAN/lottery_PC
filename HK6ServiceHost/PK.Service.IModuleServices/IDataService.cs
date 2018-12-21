﻿
using EntityModel;
using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Communication;
using EntityModel.CoreModel;

namespace PK.Service.IModuleServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ServiceBundle("PKDataService/{Service}")]
    public interface IDataService : IServiceKey
    {
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="winNum"></param>
        /// <returns></returns>
        [Service(Date = "2018-9-3", Director = "kason", Name = "充值")]
         Task<CommonActionResult> ReCharge(string userId, string userDisplayName, decimal Money);

        [Service(Date = "2018-9-3", Director = "kason", Name = "玩法分组")]
        Task<CommonActionResult> PlayCategory();

        [Service(Date = "2018-9-3", Director = "kason", Name = "充值记录")]
        Task<CommonActionResult> ReChargeRecord(string userId, int sType);

        [Service(Date = "2018-9-3", Director = "kason", Name = "提现")]
        Task<CommonActionResult> GameWithdraw(string userId, string userDisplayName, decimal Money);

        [Service(Date = "2018-9-3", Director = "kason", Name = "用户信息")]
         Task<CommonActionResult> UserInfo(string userId);
        [Service(Date = "2018-9-3", Director = "kason", Name = "订单信息")]
        Task<CommonActionResult> OrderInfo(string userId, int PageIndex);
        [Service(Date = "2018-9-3", Director = "kason", Name = "玩法")]
        Task<CommonActionResult> PlayInfo();
        [Service(Date = "2018-9-3", Director = "kason", Name = "获取期号")]
        Task<CommonActionResult> GetCurrentIssuseNo();
        [Service(Date = "2018-9-3", Director = "kason", Name = "获取期号")]
        Task<CommonActionResult> OrderDetail(string oId);
        [Service(Date = "2018-9-3", Director = "kason", Name = "获取期号")]
        Task<CommonActionResult> GameTransfer(string userId, int PageIndex);

        [Service(Date = "2018-9-3", Director = "kason", Name = "历史记录")]
        Task<CommonActionResult> HostoryData(string userId, int PageIndex);

        }
}
