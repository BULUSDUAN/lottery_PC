
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
    [ServiceBundle("creawball/{Service}")]
    public interface IBallCrawService: IServiceKey
    {


        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> CTZQMatchAndPool_Start(string Type,string gameName);
        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
         Task<string> JCZQMatch_Result_OZSP_Start(string Type);

        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> JCLQMatch_Start();
        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> BJDCMatch_OZSP_Start(string Type);

        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> CTZQMatchAndPool_Stop(string Type, string name);
    }
}
