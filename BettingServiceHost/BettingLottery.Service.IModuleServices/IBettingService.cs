
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
using BettingLottery.Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Communication;
using EntityModel.CoreModel;

namespace BettingLottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
    public interface IBettingService: IServiceKey
    {


        // // [Authorization(AuthType = AuthorizationType.JWT)]
        // [Service(Date = "2018-5-15", Director = "kason", Name = "获取用户")]
        //// [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        // Task<int> GetUserId(string userName);


        // //[Authorization(AuthType = AuthorizationType.JWT)]
        // [Service(Date = "2018-5-15", Director = "kason", Name = "根据id查找用户是否存在")]
        // // [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        // Task<UserModel> GetUser(UserModel user);

        //[Service(Date = "2018-5-15", Director = "kason", Name = "获取用户列表")]
        //Task<List<object>> Betting(string userName);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "北单，竞彩投注")]
        //
        Task<CommonActionResult> Sports_Betting(Sports_BetingInfo info, string password, decimal redBagMoney, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "足彩投注,用户保存的订单")]
        Task<CommonActionResult> SaveOrderSportsBetting(Sports_BetingInfo info, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "数字彩投注")]
        //
        Task<CommonActionResult> LotteryBetting(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "保存用户未购买订单")]
        Task<CommonActionResult> SaveOrderLotteryBetting(LotteryBettingInfo info, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "发起合买")]
        Task<CommonActionResult> CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "发起合买，保存订单")]
        Task<CommonActionResult> SaveOrder_CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "奖金优化合买")]
        Task<CommonActionResult> CreateYouHuaSchemeTogether(Sports_TogetherSchemeInfo info, string balancePassword, decimal realTotalMoney, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "虚拟奖金优化投注")]
        Task<CommonActionResult> VirtualOrderYouHuaBet(Sports_BetingInfo info, decimal realTotalMoney, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "优化投注")]
        Task<CommonActionResult> YouHuaBet(Sports_BetingInfo info, string password, decimal realTotalMoney, decimal redBagMoney, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "参与合买")]
        Task<CommonActionResult> JoinSportsTogether(string schemeId, int buyCount, string joinPwd, string balancePassword, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "编辑合买跟单")]
        Task<CommonActionResult> EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "定制合买跟单")]
        Task<CommonActionResult> CustomTogetherFollower(TogetherFollowerRuleInfo info, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "退订跟单")]
        Task<CommonActionResult> ExistTogetherFollower(long followerId, string userToken);


        [Service(Date = "2018-7-15", Director = "lidi", Name = "宝单分享-创建宝单")] 
        Task<CommonActionResult> SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "宝单分享-抄单")] 
        Task<CommonActionResult> Sports_BettingAndChase_BDFX(Sports_BetingInfo info, string password, string userId);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "世界杯投注")]
        Task<CommonActionResult> BetSJB(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken);
    }
}
