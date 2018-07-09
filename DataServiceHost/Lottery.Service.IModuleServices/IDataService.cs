
using EntityModel;
using EntityModel.CoreModel;
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
using Lottery.Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
    public interface IDataService: IServiceKey
    {
        ////[Authorization(AuthType = AuthorizationType.JWT)]
        //[Service(Date = "2018-06-05", Director = "lidi", Name = "根据key获取配置")]
        //// [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        //Task<CoreConfigInfo> QueryCoreConfigByKey(string key);

        [Service(Date = "2018-06-05", Director = "lidi", Name = "获取当前场次数据")]
        Task<Issuse_QueryInfo> QueryCurrentIssuseInfo(string gameCode);
        //GetUserListBy

        [Service(Date = "2018-06-05", Director = "lidi", Name = "查找C_Core_Config表中的配置信息")]
        Task<C_Core_Config> QueryCoreConfigByKey(string key);

        [Service(Date = "2018-06-05", Director = "lidi", Name = "从Redis中查询当前奖期")]
        Task<LotteryIssuse_QueryInfo> QueryNextIssuseListByLocalStopTime(string gameCode);
    }
}
