using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public interface IPlugin
    {
        object ExecPlugin(string type, object inputParam);
    }

    #region 投注订单

    public interface IBettingSport_AfterTranCommit : IPlugin
    {
        void BettingSport_AfterTranCommit(string userId, Sports_BetingInfo bettingOrder, string schemeId);
    }
    public interface IBettingLottery_AfterTranCommit : IPlugin
    {
        void BettingLottery_AfterTranCommit(string userId, LotteryBettingInfo bettingOrder, string schemeId, string keyLine);
    }
    /// <summary>
    /// 投注完成20150509 dj
    /// </summary>
    public interface IComplateBetting_AfterTranCommit : IPlugin
    {
        void ComplateBetting(string userId, string schemeId, bool isSuccess);
    }

    #endregion

    #region 出票相关

    /// <summary>
    /// 出票完成
    /// </summary>
    public interface IComplateTicket : IPlugin
    {
        void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney);
    }

    /// <summary>
    /// 代理返点
    /// </summary>
    public interface IAgentPayIn : IPlugin
    {
        void AgentPayIn(string schemeId);
    }

    /// <summary>
    /// 退票
    /// </summary>
    public interface IPayBackTicket : IPlugin
    {
        void PayBack(string schemeId, string ticketId, decimal ticketMoney);
    }

    #endregion

    #region 合买订单

    public interface ICreateTogether_AfterTranCommit : IPlugin
    {
        void CreateTogether_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal totalMoney, DateTime stopTime);
    }
    public interface IJoinTogether_AfterTranCommit : IPlugin
    {
        void JoinTogether_AfterTranCommit(string userId, string schemeId, int buyCount, string gameCode, string gameType, string issuseNumber, decimal totalMoney, TogetherSchemeProgress progress);
    }

    #endregion

    #region 定制跟单

    public interface ITogetherFollow_AfterTranCommit : IPlugin
    {
        void TogetherFollow_AfterTranCommit(TogetherFollowerRuleInfo info);
    }
    public interface IExistTogetherFollow_AfterTranCommit : IPlugin
    {
        void ExistTogetherFollow_AfterTranCommit(string creatorUserId, string followUserId, string gameCode, string gameType);
    }

    #endregion

    #region 用户关注

    public interface IAttention_AfterTranCommit : IPlugin
    {
        void Attention_AfterTranCommit(string activeUserId, string passiveUserId);
    }
    public interface ICancelAttention_AfterTranCommit : IPlugin
    {
        void CancelAttention_AfterTranCommit(string activeUserId, string passiveUserId);
    }

    #endregion

    #region 订单开奖、派奖

    public interface IOrderPrize_AfterTranCommit : IPlugin
    {
        void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime);
    }

    public interface IOrderPrizeMoney_AfterTranCommit : IPlugin
    {
        void OrderPrizeMoney_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, decimal preTaxBonusMoney, decimal afterTaxBonusMoney);
    }

    #endregion

    #region 用户注册登录
    public interface IUser_AfterLogin : IPlugin
    {
        void User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime);
    }
    public interface IRegister_AfterTranCommit : IPlugin
    {
        void AfterRegisterTranCommit(string regType, string userId);
    }
    public interface IChangeHideDisplayNameCount_AfterTranCommit : IPlugin
    {
        void ChangeHideDisplayNameCount_AfterTranCommit(string userId, int hideDisplayNameCount);
    }
    public interface IyqidRegister_AfterTranCommit : IPlugin
    {
        void AfterRegisterTranCommit(string yqid);
    }
    #endregion

    #region 订单手工返钱

    public interface IManualPayForOrder : IPlugin
    {
        void ManualPayForOrderAfterTranCommit(string operators, string schemeId, decimal money, string msg);
    }

    #endregion

    #region 请求充值接口

    public interface IRequestFillMoney_BeforeTranBegin : IPlugin
    {
        void RequestFillMoney_BeforeTranBegin();
    }
    //public interface IRequestFillMoney_AfterTranBegin : IPlugin
    //{
    //    void RequestFillMoney_AfterTranBegin(UserFillMoneyAddInfo fillMoneyInfo, string userId, string createBy);
    //}
    //public interface IRequestFillMoney_BeforeTranCommit : IPlugin
    //{
    //    void RequestFillMoney_BeforeTranCommit(UserFillMoneyAddInfo fillMoneyInfo, string userId, string createBy);
    //}
    //public interface IRequestFillMoney_AfterTranCommit : IPlugin
    //{
    //    void RequestFillMoney_AfterTranCommit(UserFillMoneyAddInfo fillMoneyInfo, string userId, string createBy);
    //}
    //public interface IRequestFillMoney_OnError : IPlugin
    //{
    //    void RequestFillMoney_OnError(UserFillMoneyAddInfo fillMoneyInfo, string userId, string createBy, Exception ex);
    //}

    //#endregion

    //#region 转帐户
    //public interface ITransferMoney_AfterTranCommit : IPlugin
    //{
    //    void TransferMoney_AfterTranCommit(string userId, decimal transferMoney, AccountType transferFrom, AccountType transferTo);
    //}
    //#endregion

    //#region 完成充值接口

    //public interface ICompleteFillMoney_BeforeTranBegin : IPlugin
    //{
    //    void CompleteFillMoney_BeforeTranBegin(string orderId, FillMoneyStatus status, string userId);
    //}
    //public interface ICompleteFillMoney_AfterTranBegin : IPlugin
    //{
    //    void CompleteFillMoney_AfterTranBegin(string orderId, FillMoneyStatus status, string userId);
    //}
    //public interface ICompleteFillMoney_BeforeTranCommit : IPlugin
    //{
    //    void CompleteFillMoney_BeforeTranCommit(string orderId, FillMoneyStatus status, FillMoneyAgentType agentType, decimal fillMoney, string userId);
    //}
    public interface ICompleteFillMoney_AfterTranCommit : IPlugin
    {
        void CompleteFillMoney_AfterTranCommit(string orderId, FillMoneyStatus status, FillMoneyAgentType agentType, decimal fillMoney, string userId, int vipLevel);
    }
    public interface ICompleteFillMoney_OnError : IPlugin
    {
        void CompleteFillMoney_OnError(string orderId, FillMoneyStatus status, string userId, Exception ex);
    }

    //#endregion

    //#region 请求

    //接口

    //public interface IRequestWithdraw_BeforeTranBegin : IPlugin
    //{
    //    void RequestWithdraw_BeforeTranBegin(Withdraw_RequestInfo info, string userId);
    //}
    //public interface IRequestWithdraw_AfterTranBegin : IPlugin
    //{
    //    void RequestWithdraw_AfterTranBegin(Withdraw_RequestInfo info, string userId);
    //}
    //public interface IRequestWithdraw_BeforeTranCommit : IPlugin
    //{
    //    void RequestWithdraw_BeforeTranCommit(Withdraw_RequestInfo info, string orderId, string userId);
    //}
    //public interface IRequestWithdraw_AfterTranCommit : IPlugin
    //{
    //    void RequestWithdraw_AfterTranCommit(Withdraw_RequestInfo info, string orderId, string userId);
    //}
    //public interface IRequestWithdraw_OnError : IPlugin
    //{
    //    void RequestWithdraw_OnError(Withdraw_RequestInfo info, string userId, Exception ex);
    //}

    #endregion

    #region 完成提现接口

    public interface ICompleteWithdraw_BeforeTranBegin : IPlugin
    {
        void CompleteWithdraw_BeforeTranBegin(string orderId);
    }
    public interface ICompleteWithdraw_AfterTranBegin : IPlugin
    {
        void CompleteWithdraw_AfterTranBegin(string orderId);
    }
    public interface ICompleteWithdraw_BeforeTranCommit : IPlugin
    {
        void CompleteWithdraw_BeforeTranCommit(string orderId);
    }
    public interface ICompleteWithdraw_AfterTranCommit : IPlugin
    {
        void CompleteWithdraw_AfterTranCommit(string orderId);
    }
    public interface ICompleteWithdraw_OnError : IPlugin
    {
        void CompleteWithdraw_OnError(string orderId, Exception ex);
    }

    #endregion

    #region 拒绝提现接口

    public interface IRefuseWithdraw_BeforeTranBegin : IPlugin
    {
        void RefuseWithdraw_BeforeTranBegin(string orderId);
    }
    public interface IRefuseWithdraw_AfterTranBegin : IPlugin
    {
        void RefuseWithdraw_AfterTranBegin(string orderId);
    }
    public interface IRefuseWithdraw_BeforeTranCommit : IPlugin
    {
        void RefuseWithdraw_BeforeTranCommit(string orderId);
    }
    public interface IRefuseWithdraw_AfterTranCommit : IPlugin
    {
        void RefuseWithdraw_AfterTranCommit(string orderId);
    }
    public interface IRefuseWithdraw_OnError : IPlugin
    {
        void RefuseWithdraw_OnError(string orderId, Exception ex);
    }

    #endregion

    #region 用户认证
    public interface IResponseAuthentication_AfterTranCommit : IPlugin
    {
        void ResponseAuthentication_AfterTranCommit(string userId, string authenticationType, string authenticationInfo, SchemeSource source);
    }
    #endregion
    /// <summary>
    /// 绑定银行卡接口
    /// </summary>
    public interface IAddBankCard : IPlugin
    {
        void AddBankCard(string userId, string bankCardNumber, string bankCode, string bankName, string bankSubName, string cityName, string provinceName, string realName);
    }

    /// <summary>
    /// 资金密码接口
    /// </summary>
    public interface IBalancePassword : IPlugin
    {
        void AddBalancePassword(string userId, string oldPassword, bool isSetPwd, string newPassword);
    }

    /// <summary>
    /// 奖金优化投注
    /// </summary>
    public interface IBonusOptimize : IPlugin
    {
        void AddBonusOptimize(string userId, string schemeId);
    }

    //#endregion

    #region 验证用户可否投注

    public interface ICheckUserIsBetting_BeforeTranBegin : IPlugin
    {
        void CheckUserIsBetting(string userId, decimal orderMoney);
    }
    public interface ICheckUserIsBetting_BeforeTranCommit : IPlugin
    {
        void CheckUserIsBetting_End(string userId, decimal orderMoney, string schemeId, bool isSuccess);
    }

    #endregion

    #region 获取竞彩单关专场数据

    //public interface IGetJCSingleMatchData : IPlugin
    //{
    //    void GetJCSingleMatchData(JCZQ_MatchInfo_Collection collection);
    //}

    #endregion

}