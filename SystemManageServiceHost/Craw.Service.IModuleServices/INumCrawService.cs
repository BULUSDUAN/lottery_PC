
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


namespace Craw.Service.IModuleServices
{
    /// <summary>
    /// 采集服务
    /// </summary>
    [ServiceBundle("creawSer/{Service}")]
    public interface INumCrawService : IServiceKey
    {


        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> Login(string gameName);

        [Service(Date = "2018-9-17", Director = "kason", Name = "采集，开奖服务")]

        Task<string> NumLettory_WinNumber_Start(string gameName);

        [Service(Date = "2018-9-17", Director = "kason", Name = "采集，开奖服务")]

        Task<string> NumLettory_WinNumber_Stop(string gameName);

    }
}
