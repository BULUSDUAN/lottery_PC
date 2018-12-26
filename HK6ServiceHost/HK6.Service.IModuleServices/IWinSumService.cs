
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

namespace HK6.ModuleBaseServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ServiceBundle("HK6WinSum/{Service}")]
    public interface IWinSumService : IServiceKey
    {
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="winNum"></param>
        /// <returns></returns>
        [Service(Date = "2018-9-3", Director = "kason", Name = "管理登陆")]
        Task<CommonActionResult> Sum(string userId, string date, string IssueNo, string winNum);


    }
}
