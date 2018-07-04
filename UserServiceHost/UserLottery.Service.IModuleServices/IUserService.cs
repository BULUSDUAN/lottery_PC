﻿
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

        [Service(Date = "2018-6-29", Director = "renjun", Name = "根据UserId查询用户信息")]
        Task<LoginInfo> GetLocalLoginByUserId(string userId);

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

        [Service(Date = "2018-6-19", Director = "renjun", Name = "注册验证手机")]
        Task<CommonActionResult> RegisterRequestMobile(string mobile);

        [Service(Date = "2018-6-19", Director = "renjun", Name = "手机号是否可注册")]
        Task<bool> HasMobile(string mobile);

        [Service(Date = "2018-6-20", Director = "renjun", Name = "配置项")]
        Task<C_Core_Config> QueryCoreConfigByKey(string key);

        [Service(Date = "2018-6-20", Director = "renjun", Name = "回复手机认证")]
        Task<CommonActionResult> ResponseAuthenticationMobile(string validateCode, SchemeSource source, string userToken);

        [Service(Date = "2018-6-27", Director = "renjun", Name = "判断找回密码验证码是否正确")]
        Task<bool> CheckValidateCodeByForgetPWD(string mobile, string validateCode);

        [Service(Date = "2018-6-27", Director = "renjun", Name = "根据用户名查询用户ID")]
        Task<string> GetUserIdByLoginName(string loginName);

        [Service(Date = "2018-6-27", Director = "renjun", Name = "找回密码")]
        Task<CommonActionResult> FindPassword(string userId);

        [Service(Date = "2018-6-27", Director = "renjun", Name = "某场景触发的发送站内消息")]
        Task<CommonActionResult> DoSendSiteMessage(string userId, string mobile, string sceneKey, string msgTemplateParams);

        [Service(Date = "2018-6-28", Director = "renjun", Name = "找回密码发送验证码")]
        Task<CommonActionResult> SendValidateCodeToUserMobileByForgetPWD(string mobile);

        [Service(Date = "2018-6-28", Director = "renjun", Name = " 检查是否和登录密码一至")]
        Task<CommonActionResult> CheckIsSame2LoginPassword(string newPassword, string userToken);

        [Service(Date = "2018-6-28", Director = "renjun", Name = " 设置资金密码")]
        Task<CommonActionResult> SetBalancePassword(string oldPassword, bool isSetPwd, string newPassword, string userToken);

        [Service(Date = "2018-6-28", Director = "renjun", Name = " 设置资金密码类型")]
        Task<CommonActionResult> SetBalancePasswordNeedPlace(string password, string placeList, string userToken);

        [Service(Date = "2018-6-29", Director = "renjun", Name = "实名认证")]
        Task<CommonActionResult> AuthenticateMyRealName(string IdCardNumber, string RealName, SchemeSource source, string userToken);

        [Service(Date = "2018-6-29", Director = "renjun", Name = "增加银行卡信息")]
        Task<CommonActionResult> AddBankCard(C_BankCard bankCard, string userToken);

        [Service(Date = "2018-7-02", Director = "renjun", Name = "提款确认")]
        Task<CheckWithdrawResult> RequestWithdraw_Step1(string userId, decimal requestMoney);

        [Service(Date = "2018-7-03", Director = "renjun", Name = "提款成功")]
        Task<CommonActionResult> RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string password);

        [Service(Date = "2018-7-03", Director = "renjun", Name = "提现记录")]
        Task<Withdraw_QueryInfoCollection> QueryMyWithdrawList(WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken);
    }
}
