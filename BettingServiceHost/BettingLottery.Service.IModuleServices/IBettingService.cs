
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
    public interface IBettingService : IServiceKey
    {
        [Service(Date = "2018-7-15", Director = "lidi", Name = "北单，竞彩投注")]
        //
        Task<CommonActionResult> Sports_Betting(Sports_BetingInfo info, string password, decimal redBagMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "足彩投注,用户保存的订单")]
        Task<CommonActionResult> SaveOrderSportsBetting(Sports_BetingInfo info, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "数字彩投注")]
        //
        Task<CommonActionResult> LotteryBetting(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "保存用户未购买订单")]
        Task<CommonActionResult> SaveOrderLotteryBetting(LotteryBettingInfo info, string userToken);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "发起合买")]
        Task<CommonActionResult> CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "发起合买，保存订单")]
        Task<CommonActionResult> SaveOrder_CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "奖金优化合买")]
        Task<CommonActionResult> CreateYouHuaSchemeTogether(Sports_TogetherSchemeInfo info, string balancePassword, decimal realTotalMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "虚拟奖金优化投注")]
        Task<CommonActionResult> VirtualOrderYouHuaBet(Sports_BetingInfo info, decimal realTotalMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "优化投注")]
        Task<CommonActionResult> YouHuaBet(Sports_BetingInfo info, string password, decimal realTotalMoney, decimal redBagMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "参与合买")]
        Task<CommonActionResult> JoinSportsTogether(string schemeId, int buyCount, string joinPwd, string balancePassword, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "编辑合买跟单")]
        Task<CommonActionResult> EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "定制合买跟单")]
        Task<CommonActionResult> CustomTogetherFollower(TogetherFollowerRuleInfo info, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "退订跟单")]
        Task<CommonActionResult> ExistTogetherFollower(long followerId, string userid);


        [Service(Date = "2018-7-15", Director = "lidi", Name = "宝单分享-创建宝单")]
        Task<CommonActionResult> SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "宝单分享-抄单")]
        Task<CommonActionResult> Sports_BettingAndChase_BDFX(Sports_BetingInfo info, string password, string userId);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "世界杯投注")]
        Task<CommonActionResult> BetSJB(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userid);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "日志")]
        Task<string> ReadSqlTimeLog(string FileName);

        [Service(Date = "2018-7-15", Director = "kason", Name = "日志")]
        Task<string> ReadLog(string DicName, string ApiDicTypeName);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "查询合买大厅")]
        Task<List<Sports_TogetherSchemeQueryInfo>> QueryTogetherHall();

        [Service(Date = "2018-10-24", Director = "lidi", Name = "查询必要数据")]
        Task<string> GetAllConfigValue();

        [Service(Date = "2018-10-29", Director = "renjun", Name = "购买用户保存订单")]
        Task<CommonActionResult> BettingUserSavedOrder(string schemeId, string balancePassword, decimal redBagMoney, string userId);
        [Service(Date = "2018-10-29", Director = "renjun", Name = "发起单式合买")]
        Task<CommonActionResult> CreateSingleSchemeTogether(SingleScheme_TogetherSchemeInfo info, string balancePassword, string userId);
        [Service(Date = "2018-10-30", Director = "renjun", Name = "单式投注和追号")]
        Task<CommonActionResult> SingleSchemeBettingAndChase(SingleSchemeInfo info, string password, decimal redBagMoney, string userId);
    }
}
