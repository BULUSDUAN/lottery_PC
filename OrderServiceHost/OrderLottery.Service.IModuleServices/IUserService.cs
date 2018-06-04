
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
using OrderLottery.Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderLottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
    public interface IUserService: IServiceKey
    {


        // [Authorization(AuthType = AuthorizationType.JWT)]
        [Service(Date = "2018-5-15", Director = "kason", Name = "获取用户")]
       // [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        Task<int> GetUserId(string userName);


        //[Authorization(AuthType = AuthorizationType.JWT)]
        [Service(Date = "2018-5-15", Director = "kason", Name = "根据id查找用户是否存在")]
        // [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        Task<UserModel> GetUser(UserModel user);

        [Service(Date = "2018-5-15", Director = "kason", Name = "获取用户列表")]
        Task<List<User>> GetUserList(string userName);
        //GetUserListBy
    
    }
}
