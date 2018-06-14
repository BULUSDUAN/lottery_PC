
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
using EntityModel.Enum;

namespace UserLottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
    public interface IUserService: IServiceKey
    {
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

        [Service(Date = "2018-6-14", Director = "renjun", Name = "注册")]
        Task<CommonActionResult> RegisterResponseMobile(string validateCode, string mobile, SchemeSource source, RegisterInfo_Local info);

    }
}
