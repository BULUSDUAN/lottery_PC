using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderLottery.Service.IModuleServices
{
    [ServiceBundle("Order/{Service}")]
   public interface IOrderService: IServiceKey
    {
        [Service(Date = "2018-06-04", Director = "Debug", Name = "中奖查询")]
        BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "北京单场查询开奖结果")]
        BJDCMatchResultInfo_Collection QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "查询我的资金明细")]
        UserFundDetailCollection QueryMyFundDetailList(QueryUserFundDetailParam Model);

        [Service(Date = "2018-06-06", Director = "Debug", Name = "查询我的充值记录")]
        FillMoneyQueryInfoCollection QueryFillMoneyList(QueryFillMoneyListParam Model);
        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询我的投注记录")]
        MyBettingOrderInfoCollection QueryMyBettingOrderList(QueryMyBettingOrderParam Model);
        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询提款记录")]
        Withdraw_QueryInfoCollection QueryMyWithdrawList(QueryMyWithdrawParam Model);
    }
}
