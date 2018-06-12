
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
using UserLottery.Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EntityModel.RequestModel;
using EntityModel.Communication;
using EntityModel.CoreModel;

namespace UserLottery.Service.IModuleServices
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

        [Service(Date = "2018-6-06", Director = "renjun", Name = "登录")]
        Task<LoginInfo> User_Login(string loginName, string password,string IPAddress);

        [Service(Date = "2018-6-06", Director = "renjun", Name = "修改密码")]
        Task<CommonActionResult> ChangeMyPassword(string oldPassword, string newPassword, string userToken);

        [Service(Date = "2018-6-08", Director = "renjun", Name = "token登录")]
        Task<LoginInfo> LoginByUserToken(string userToken);


        [Service(Date = "2018-6-12", Director = "renjun", Name = "绑定信息")]
        Task<UserBindInfos> QueryUserBindInfos(string UserId);

        [Service(Date = "2018-6-12", Director = "renjun", Name = "查询余额信息")]
        Task<UserBalanceInfo> QueryMyBalance(string userToken);

        [Service(Date = "2018-6-12", Director = "renjun", Name = "查询银行卡信息")]
        Task<C_BankCard> QueryBankCard(string userToken);

        [Service(Date = "2018-6-12", Director = "renjun", Name = "获取用户站内信数量")]
        Task<int> GetMyUnreadInnerMailCount(string userToken);
    }
}
