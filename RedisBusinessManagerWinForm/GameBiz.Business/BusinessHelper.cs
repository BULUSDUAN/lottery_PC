using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using GameBiz.Domain.Managers;
using Common.Database.NHibernate;
using GameBiz.Core;
using Common;
using NHibernate;
using Common.Cryptography;
using Common.Utilities;
using Common.Gateway.SXF;
using System.Threading;
using System.Configuration;
using System.Net;
using Common.Algorithms;
using Common.Lottery;
using Common.Communication;
using Common.Log;
using Common.Net;
using StackExchange.Redis;
using Common.JSON;
using GameBiz.Business.Domain.Entities.Ticket;
using GameBiz.Core.Ticket;
using GameBiz.Business.Domain.Managers.Ticket;
using System.IO;
using System.Diagnostics;
using Common.Lottery.Redis;

namespace GameBiz.Business
{
    public class FundInfo
    {
        /// <summary>
        /// 订单号
        /// SchemeId|IssuseNumber 格式
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public PayType PayType { get; set; }
        /// <summary>
        /// 支出金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public AccountType AccountType { get; set; }
    }
    /// <summary>
    /// 订单资金数据
    /// </summary>
    public class OrderFundInfo
    {
        public OrderFundInfo()
        {
            FundList = new List<FundInfo>();
        }
        /// <summary>
        /// 线索编号
        /// </summary>
        public string KeyLine { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 是否需要资金密码
        /// </summary>
        public bool IsNeedPwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// 用户Key
        /// </summary>
        public string Password { get; set; }

        public IList<FundInfo> FundList { get; set; }
        /// <summary>
        /// 操作员Id
        /// </summary>
        public string OperatorId { get; set; }
    }

    internal class IdGen
    {
        public long Index { get; set; }
        public DateTime DateTime { get; set; }
    }

    /// <summary>
    /// 资金收入/支出明细
    /// </summary>
    public class PayDetail
    {
        public AccountType AccountType { get; set; }
        public PayType PayType { get; set; }
        public decimal PayMoney { get; set; }
    }


    public static class BusinessHelper
    {
        private const string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        private static ILogWriter writer = Common.Log.LogWriterGetter.GetLogWriter();
        public const int MaxPageSize = 200;

        #region 资金明细分类

        /// <summary>
        /// 资金分类——购彩
        /// </summary>
        public const string FundCategory_Betting = "购彩";
        /// <summary>
        /// 资金分类——出票失败
        /// </summary>
        public const string FundCategory_TicketFailed = "出票失败";
        /// <summary>
        /// 资金分类——无开奖结果退款
        /// </summary>
        public const string FundCategory_PayBack_BetMoney = "无开奖结果退款";
        /// <summary>
        /// 资金分类--奖金
        /// </summary>
        public const string FundCategory_Bonus = "奖金";
        /// <summary>
        /// 资金分类--追号返还
        /// </summary>
        public const string FundCategory_ChaseBack = "追号返还";
        /// <summary>
        /// 资金分类--合买提成
        /// </summary>
        public const string FundCategory_Deduct = "合买奖金盈利提成";//之前为"合买佣金"
        /// <summary>
        /// 资金分类--方案返利
        /// </summary>
        public const string FundCategory_SchemeDeduct = "方案返利";
        /// <summary>
        /// 资金分类--方案分红
        /// </summary>
        public const string FundCategory_SchemeBonus = "方案分红";
        /// <summary>
        /// 资金分类--返还保底资金
        /// </summary>
        public const string FundCategory_ReturnGuarantees = "返还保底";
        /// <summary>
        /// 资金分类--合买失败
        /// </summary>
        public const string FundCategory_TogetherFail = "合买失败";
        /// <summary>
        /// 资金分类--撤单
        /// </summary>
        public const string FundCategory_CancelOrder = "撤单";
        /// <summary>
        /// 资金分类--用户充值
        /// </summary>
        public const string FundCategory_UserFillMoney = "充值";
        /// <summary>
        /// 资金分类--充值专员充值
        /// </summary>
        public const string FundCategory_UserFillMoneyByCzzy = "充值专员充值";
        /// <summary>
        /// 资金分类--手工充值
        /// </summary>
        public const string FundCategory_ManualFillMoney = "手工充值";
        /// <summary>
        /// 资金分类--手工扣款
        /// </summary>
        public const string FundCategory_ManualDeductMoney = "手工扣款";
        /// <summary>
        /// 资金分类--手工打款
        /// </summary>
        public const string FundCategory_ManualRemitMoney = "手工打款";
        /// <summary>
        /// 资金分类--申请提现
        /// </summary>
        public const string FundCategory_RequestWithdraw = "申请提现";
        /// <summary>
        /// 资金分类--提现手续费
        /// </summary>
        public const string FundCategory_RequestWithdrawCounterFee = "提现手续费";
        /// <summary>
        /// 资金分类--完成提现
        /// </summary>
        public const string FundCategory_CompleteWithdraw = "完成提现";
        /// <summary>
        /// 资金分类--拒绝提现，返还资金
        /// </summary>
        public const string FundCategory_RefusedWithdraw = "拒绝提现";
        /// <summary>
        /// 资金分类--资金转出
        /// </summary>
        public const string FundCategory_TransferFrom = "资金转出";
        /// <summary>
        /// 资金分类--资金转入
        /// </summary>
        public const string FundCategory_TransferTo = "资金转入";
        /// <summary>
        /// 资金分类--提取佣金
        /// </summary>
        public const string FundCategory_Commission = "提取佣金";
        /// <summary>
        /// 资金分类--活动赠送
        /// </summary>
        public const string FundCategory_Activity = "活动赠送";
        /// <summary>
        /// 资金分类--预定专家方案
        /// </summary>
        public const string FundCategory_Booking = "预定专家方案";
        /// <summary>
        /// 资金分类--购买专家方案
        /// </summary>
        public const string FundCategory_BuyExpScheme = "购买名家方案";
        /// <summary>
        /// 资金分类--投注增加成长值
        /// </summary>
        public const string FundCategory_BuyGrowthValue = "投注增加成长值";
        /// <summary>
        /// 资金分类--投注增加豆豆
        /// </summary>
        public const string FundCategory_BuyDouDou = "投注增加豆豆";
        /// <summary>
        /// 资金分类--豆豆兑换奖品
        /// </summary>
        public const string FundCategory_ExchangeDouDou = "豆豆兑换奖品";
        /// <summary>
        /// 资金分类--用户等级提升
        /// </summary>
        public const string FundCategory_UserLevelUp = "用户等级提升";
        /// <summary>
        /// 资金分类--赢家平台模型转入先行赔付
        /// </summary>
        public const string FundCategory_WinnerModelPayIn = "模型转入先行赔付金";
        /// <summary>
        /// 资金分类--赢家平台模型转出先行赔付
        /// </summary>
        public const string FundCategory_WinnerModelPayOut = "模型转出先行赔付金";
        /// <summary>
        /// 资金分类-赢家平台用户追号详情停止
        /// </summary>
        public const string FundCategory_WinnerModelStopChase = "停止追号计划";
        /// <summary>
        /// 资金分类-赢家平台返退还者先行赔付金
        /// </summary>
        public const string FundCategory_WinnerModelPayBackFirstPayMoney = "退还先行赔付金";
        /// <summary>
        /// 资金分类-赢家平台豆豆模型竞价排行
        /// </summary>
        public const string FundCategory_WinnerModelBidding = "豆豆模型竞价排行";
        /// <summary>
        /// 资金分类-申请票样
        /// </summary>
        public const string FundCategory_ApplyTicket = "申请票样";
        /// <summary>
        /// 资金分类-拒绝申请票样
        /// </summary>
        public const string FundCategory_ApplyTicketRefuse = "拒绝申请票样";
        /// <summary>
        /// 资金分类-返点转入
        /// </summary>
        public const string FundCategory_IntergralPayOut = "返点转入";
        /// <summary>
        /// 资金分类-渠道充值
        /// </summary>
        public const string FundCategory_IntegralFillMoney = "渠道充值";
        /// <summary>
        /// 资金分类-渠道充值
        /// </summary>
        public const string FundCategory_CPSFillMoney = "CPS充值";
        /// <summary>
        /// 资金分类--宝单分享奖金提成
        /// </summary>
        public const string FundCategory_BDFXCommissionMoney = "宝单分享奖金盈利提成";
        ///// <summary>
        ///// 资金分类--宝单分享扣除奖金提成
        ///// </summary>
        //public const string FundCategory_BDFXDeductCommissionMoney = "宝单分享扣除奖金提成";

        /// <summary>
        /// 资金分类-CPS返点转入
        /// </summary>
        public const string FundCategory_CPSPayOut = "CPS返点转入";

        #region 代理平台

        /// <summary>
        /// 申请提取积分
        /// </summary>
        public const string FundCategory_IntegralRequestWithdraw = "申请提取积分";
        /// <summary>
        /// 资金分类--方案提成
        /// </summary>
        public const string FundCategory_IntegralSchemeDeduct = "方案返积分";
        /// <summary>
        /// 资金分类--完成提现
        /// </summary>
        public const string FundCategory_IntegralCompleteWithdraw = "完成提取积分";
        /// <summary>
        /// 资金分类--拒绝提现，返还资金
        /// </summary>
        public const string FundCategory_IntegralRefusedWithdraw = "拒绝提取积分";
        /// <summary>
        /// 资金分类--手工扣款
        /// </summary>
        public const string FundCategory_IntegralManualDeductMoney = "手工扣除积分";
        /// <summary>
        /// 资金分类--手工打款
        /// </summary>
        public const string FundCategory_IntegralManualRemitMoney = "手工增加积分";

        /// <summary>
        /// 申请提取返点-CPS
        /// </summary>
        public const string FundCategory_CPSRequestWithdraw = "CPS申请提取返点";

        #endregion

        /// <summary>
        /// 资金分类——购彩
        /// </summary>
        public const string FundCategory_SettlementBonus = "结算分红";


        #endregion

        #region 自动生成各类订单编号

        /// <summary>
        /// 普通投注方案编号
        /// </summary>
        public static string GetGeneralBettingSchemeId()
        {
            string prefix = "GSM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 追号投注方案编号
        /// </summary>
        public static string GetChaseBettingSchemeId()
        {
            string prefix = "CSM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 合买投注方案编号
        /// </summary>
        public static string GetTogetherBettingSchemeId()
        {
            string prefix = "TSM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 参与合买编号
        /// </summary>
        public static string GetTogetherJoinId()
        {
            string prefix = "JSM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 中奖编号
        /// </summary>
        public static string GetBonusId()
        {
            string prefix = "BSM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 充值编号
        /// </summary>
        public static string GetUserFillMoneyId()
        {
            string prefix = "UFM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 手工充值编号
        /// </summary>
        public static string GetManualFillMoneyId()
        {
            string prefix = "MFM";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 申请提现编号
        /// </summary>
        public static string GetWithdrawId()
        {
            string prefix = "MWD";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 预定专家方案编号
        /// </summary>
        public static string GetBookingId()
        {
            string prefix = "Book";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 专家方案编号
        /// </summary>
        public static string GetExperterSchemeId()
        {
            string prefix = "EXP";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 购买专家方案 订单编号
        /// </summary>
        public static string GetBuyExperterSchemeId()
        {
            string prefix = "BUYEXP";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 模型编号
        /// </summary>
        public static string GetModelSchemeId()
        {
            string prefix = "MODEL";
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 索赔索票
        /// </summary>
        public static string GetSampleTicketsSchemeId()
        {
            string prefix = "ST";
            return prefix + UsefullHelper.UUID();
        }

        /// <summary>
        /// 北京单场方案编号
        /// </summary>
        public static string GetSportsBettingSchemeId(string gameCode)
        {
            string prefix = gameCode;
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 追号订单的KeyLine
        /// </summary>
        public static string GetChaseLotterySchemeKeyLine(string gameCode)
        {
            while (true)
            {
                string prefix = "CHASE" + gameCode;
                var keyLine = prefix + UsefullHelper.UUID();
                var count = new Sports_Manager().QueryKeyLineCount(keyLine);
                if (count > 0)
                    continue;
                return keyLine;
            }
        }
        /// <summary>
        /// 普通数字彩KeyLine
        /// </summary>
        public static string GetGeneralLotterySchemeKeyLine(string gameCode)
        {
            string prefix = "GENERAL" + gameCode;
            return prefix + UsefullHelper.UUID();
        }

        /// <summary>
        /// 转账编号
        /// </summary>
        /// <returns></returns>
        public static string GetTransferId()
        {
            string prefix = "TAM";
            return prefix + UsefullHelper.UUID();
        }

        /// <summary>
        /// 分析编号
        /// </summary>
        /// <returns></returns>
        public static string GetAnalysisId()
        {
            string prefix = "XSTJ";
            return prefix + UsefullHelper.UUID();
        }

        /// <summary>
        /// 文章编号
        /// </summary>
        /// <returns></returns>
        public static string GetArticleId()
        {
            string prefix = "ZX";
            return prefix + UsefullHelper.UUID();
        }

        #endregion


        #region 资金操作通用函数

        #region 钱相关

        /// <summary>
        /// 用户支出，自动扣除用户金额，非冻结
        /// 用于购彩、发起合买、参与合买、跟单合买等
        /// </summary>
        public static void Payout_To_End(string category, string userId, string orderId, decimal payoutMoney, string summary, string place, string password)
        {
            if (payoutMoney <= 0M)
                throw new LogicException("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new LogicException("资金密码输入错误");
                    }
                }
            }

            var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance;
            if (totalMoney < payoutMoney)
                throw new LogicException(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            //消费顺序：充值金额=>奖金=>佣金=>名家
            #region 按顺序消费用户余额

            var currentPayout = 0M;
            var payDetailList = new List<PayDetail>();
            if (userBalance.FillMoneyBalance > 0M && payoutMoney > 0M)
            {
                //充值金额参与支出
                currentPayout = userBalance.FillMoneyBalance >= payoutMoney ? payoutMoney : userBalance.FillMoneyBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance -= currentPayout;
            }
            if (userBalance.BonusBalance > 0M && payoutMoney > 0M)
            {
                //奖金参与支付
                currentPayout = userBalance.BonusBalance >= payoutMoney ? payoutMoney : userBalance.BonusBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance -= currentPayout;
            }
            if (userBalance.CommissionBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CommissionBalance >= payoutMoney ? payoutMoney : userBalance.CommissionBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance -= currentPayout;
            }
            if (userBalance.ExpertsBalance > 0M && payoutMoney > 0M)
            {
                //名家参与支付
                currentPayout = userBalance.ExpertsBalance >= payoutMoney ? payoutMoney : userBalance.ExpertsBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance -= currentPayout;
            }
            if (userBalance.CPSBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CPSBalance >= payoutMoney ? payoutMoney : userBalance.CPSBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.CPS,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.CPS,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CPSBalance,
                    AfterBalance = userBalance.CPSBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.CPSBalance -= currentPayout;
            }
            if (payoutMoney > 0M)
                throw new LogicException("用户余额不足");

            #endregion

            //修改余额
            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出，仅扣除用户红包余额，非冻结
        /// 用于购彩、发起合买、参与合买、跟单合买等
        /// </summary>
        public static void Payout_RedBag_To_End(string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string password)
        {
            if (payoutMoney <= 0M)
                throw new LogicException("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new LogicException("资金密码输入错误");
                    }
                }
            }

            var totalMoney = userBalance.RedBagBalance;
            if (totalMoney < payoutMoney)
                throw new LogicException(string.Format("用户红包小于 {0:N2}元。", payoutMoney));

            var currentPayout = 0M;
            var payDetailList = new List<PayDetail>();
            if (userBalance.RedBagBalance > 0M && payoutMoney > 0M)
            {
                //红包参与支付
                currentPayout = userBalance.RedBagBalance >= payoutMoney ? payoutMoney : userBalance.RedBagBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance -= currentPayout;
            }

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出
        /// 指定帐户支出
        /// </summary>
        public static void Payout_To_End(AccountType accountType, string category, string userId, string orderId, decimal payoutMoney, string summary, string operatorId = "")
        {
            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            #region 扣除账户金额

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payoutMoney,
                PayType = PayType.Payout,
            });

            switch (accountType)
            {
                case AccountType.Bonus:
                    if (userBalance.BonusBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.BonusBalance,
                        AfterBalance = userBalance.BonusBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.BonusBalance -= payoutMoney;
                    break;
                case AccountType.Freeze:
                    if (userBalance.FreezeBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.FreezeBalance,
                        AfterBalance = userBalance.FreezeBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.FreezeBalance -= payoutMoney;
                    break;
                case AccountType.Commission:
                    if (userBalance.CommissionBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.CommissionBalance,
                        AfterBalance = userBalance.CommissionBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.CommissionBalance -= payoutMoney;
                    break;
                case AccountType.FillMoney:
                    if (userBalance.FillMoneyBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.FillMoneyBalance,
                        AfterBalance = userBalance.FillMoneyBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.FillMoneyBalance -= payoutMoney;
                    break;
                case AccountType.Experts:
                    if (userBalance.ExpertsBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.ExpertsBalance,
                        AfterBalance = userBalance.ExpertsBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.ExpertsBalance -= payoutMoney;
                    break;
                case AccountType.RedBag:
                    if (userBalance.RedBagBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.RedBagBalance,
                        AfterBalance = userBalance.RedBagBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.RedBagBalance -= payoutMoney;
                    break;
                case AccountType.CPS:
                    if (userBalance.CPSBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.CPSBalance,
                        AfterBalance = userBalance.CPSBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.CPSBalance -= payoutMoney;
                    break;
                default:
                    break;
            }

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出-CPS
        /// 指定帐户支出
        /// </summary>
        public static void Payout_To_End_CPS(AccountType accountType, string category, string userId, string orderId, decimal payoutMoney, string summary, string operatorId = "")
        {
            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            #region 扣除账户金额
            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payoutMoney,
                PayType = PayType.Payout,
            });
            switch (accountType)
            {
                case AccountType.Bonus:
                    if (userBalance.BonusBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.BonusBalance,
                        AfterBalance = userBalance.BonusBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.BonusBalance -= payoutMoney;
                    break;
                case AccountType.Freeze:
                    if (userBalance.FreezeBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.FreezeBalance,
                        AfterBalance = userBalance.FreezeBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.FreezeBalance -= payoutMoney;
                    break;
                case AccountType.Commission:
                    if (userBalance.CommissionBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.CommissionBalance,
                        AfterBalance = userBalance.CommissionBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.CommissionBalance -= payoutMoney;
                    break;
                case AccountType.FillMoney:
                    if (userBalance.FillMoneyBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.FillMoneyBalance,
                        AfterBalance = userBalance.FillMoneyBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.FillMoneyBalance -= payoutMoney;
                    break;
                case AccountType.Experts:
                    if (userBalance.ExpertsBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.ExpertsBalance,
                        AfterBalance = userBalance.ExpertsBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.ExpertsBalance -= payoutMoney;
                    break;
                case AccountType.RedBag:
                    if (userBalance.RedBagBalance < payoutMoney)
                        throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.RedBagBalance,
                        AfterBalance = userBalance.RedBagBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.RedBagBalance -= payoutMoney;
                    break;
                case AccountType.CPS:
                    //if (userBalance.CPSBalance < payoutMoney)
                    //    throw new Exception("账户余额不足");
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = accountType,
                        PayMoney = payoutMoney,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.CPSBalance,
                        AfterBalance = userBalance.CPSBalance - payoutMoney,
                        OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
                    });
                    //userBalance.CPSBalance -= payoutMoney;
                    break;
                default:
                    break;
            }

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());

        }

        /// <summary>
        /// 用户退款,用于用户支出后，因业务原因退款
        /// 调用前提：必须有已支付过的订单，按订单支付的相应账号退还金额
        /// </summary>
        public static void Payback_To_Balance(string category, string userId, string orderId, decimal payBackMoney, string summary)
        {
            if (payBackMoney <= 0M)
                throw new Exception("退款金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var fundList = fundManager.QueryFundDetailList(orderId, userId);
            if (fundList == null || fundList.Count == 0)
                throw new Exception(string.Format("未查询到用户{0}的订单{1}的支付明细", userId, orderId));
            //if (fundList.Sum(p => p.PayMoney) < payBackMoney)
            //    throw new Exception("退款金额大于订单总支付金额");

            //退款顺序：名家=>佣金=>奖金=>红包=>充值金额
            #region 按顺序退款

            var currentPayBack = 0M;
            var payDetailList = new List<PayDetail>();
            var expertFund = fundList.Where(p => p.AccountType == AccountType.Experts).ToList();
            if (expertFund != null && expertFund.Count > 0 && payBackMoney > 0M)
            {
                //名家金额参与支付，退款到名家金额
                currentPayBack = payBackMoney >= expertFund.Sum(p => p.PayMoney) ? expertFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance += currentPayBack;
            }

            var commisionFund = fundList.Where(p => p.AccountType == AccountType.Commission).ToList();
            if (commisionFund != null && commisionFund.Count > 0 && payBackMoney > 0M)
            {
                //佣金金额参与支付，退款到佣金金额
                currentPayBack = payBackMoney >= commisionFund.Sum(p => p.PayMoney) ? commisionFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance += currentPayBack;
            }

            var bonusFund = fundList.Where(p => p.AccountType == AccountType.Bonus).ToList();
            if (bonusFund != null && bonusFund.Count > 0 && payBackMoney > 0M)
            {
                //奖金金额参与支付，退款到奖金金额
                currentPayBack = payBackMoney >= bonusFund.Sum(p => p.PayMoney) ? bonusFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance += currentPayBack;
            }

            var redBagFund = fundList.Where(p => p.AccountType == AccountType.RedBag).ToList();
            if (redBagFund != null && redBagFund.Count > 0 && payBackMoney > 0M)
            {
                //红包金额参与支付，退款到红包金额
                currentPayBack = payBackMoney >= redBagFund.Sum(p => p.PayMoney) ? redBagFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance += currentPayBack;
            }

            var fillFund = fundList.Where(p => p.AccountType == AccountType.FillMoney).ToList();
            if (fillFund != null && fillFund.Count > 0 && payBackMoney > 0M)
            {
                //充值金额参与支付，退款到充值金额
                currentPayBack = payBackMoney >= fillFund.Sum(p => p.PayMoney) ? fillFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance += currentPayBack;
            }
            //if (payBackMoney > 0M)
            //    throw new Exception("退款金额大于总支付金额");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出，金额转到冻结金额
        /// 暂时只用于追号
        /// </summary>
        public static void Payout_To_Frozen(string category, string userId, string orderId, decimal payoutMoney, string summary, string place, string password)
        {
            if (payoutMoney <= 0M)
                throw new LogicException("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new LogicException("资金密码输入错误");
                    }
                }
            }
            var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance + userBalance.RedBagBalance;
            if (totalMoney < payoutMoney)
                throw new LogicException(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.Freeze,
                PayType = PayType.Payin,
                PayMoney = payoutMoney,
            });
            //冻结资金明细
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance + payoutMoney,
                OperatorId = userId,
            });

            //userBalance.FreezeBalance += payoutMoney;

            #region 按顺序消费用户余额

            var currentPayout = 0M;
            if (userBalance.FillMoneyBalance > 0M && payoutMoney > 0M)
            {
                //充值金额参与支出
                currentPayout = userBalance.FillMoneyBalance >= payoutMoney ? payoutMoney : userBalance.FillMoneyBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance -= currentPayout;
            }
            if (userBalance.BonusBalance > 0M && payoutMoney > 0M)
            {
                //奖金参与支付
                currentPayout = userBalance.BonusBalance >= payoutMoney ? payoutMoney : userBalance.BonusBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance -= currentPayout;
            }
            if (userBalance.CommissionBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CommissionBalance >= payoutMoney ? payoutMoney : userBalance.CommissionBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance -= currentPayout;
            }
            if (userBalance.ExpertsBalance > 0M && payoutMoney > 0M)
            {
                //名家参与支付
                currentPayout = userBalance.ExpertsBalance >= payoutMoney ? payoutMoney : userBalance.ExpertsBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance -= currentPayout;
            }
            if (payoutMoney > 0M)
                throw new LogicException("用户余额不足");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出，仅红包金额转到冻结金额
        /// 暂时只用于追号
        /// </summary>
        public static void Payout_RedBag_To_Frozen(string category, string userId, string orderId, decimal payoutMoney, string summary, string place, string password)
        {
            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new Exception("资金密码输入错误");
                    }
                }
            }
            var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance + userBalance.RedBagBalance;
            if (totalMoney < payoutMoney)
                throw new Exception(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            //去掉冻结记录
            //加入冻结记录
            //var freeze = balanceManager.GetUserBalanceFreezeByOrder(userId, orderId);
            //if (freeze != null)
            //    throw new ArgumentException("订单金额已冻结 - " + orderId);
            //freeze = new UserBalanceFreeze
            //{
            //    UserId = userId,
            //    OrderId = orderId,
            //    FreezeMoney = payoutMoney,
            //    Category = frozenCategory,
            //    Description = summary,
            //    CreateTime = DateTime.Now,
            //};
            //balanceManager.AddUserBalanceFreeze(freeze);

            var payDetailList = new List<PayDetail>();
            //冻结资金明细
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance + payoutMoney,
                OperatorId = userId,
            });

            //userBalance.FreezeBalance += payoutMoney;

            #region 按顺序消费用户余额

            var currentPayout = 0M;
            if (userBalance.RedBagBalance > 0M && payoutMoney > 0M)
            {
                //红包参与支付
                currentPayout = userBalance.RedBagBalance >= payoutMoney ? payoutMoney : userBalance.RedBagBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance -= currentPayout;
            }
            if (payoutMoney > 0M)
                throw new Exception("用户余额不足");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        ///  用户支出，申请提现
        /// </summary>
        public static WithdrawCategory Payout_To_Frozen_Withdraw(string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string password, out  decimal responseMoney)
        {
            var requestMoney = payoutMoney;
            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new Exception("资金密码输入错误");
                    }
                }
            }
            var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance;
            if (totalMoney < payoutMoney)
                throw new Exception(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
            });
            //冻结资金明细
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance + payoutMoney,
                OperatorId = userId,
            });
            //userBalance.FreezeBalance += payoutMoney;

            #region 正常提现

            //奖金+佣金+名家
            var currentPayout = 0M;
            if (userBalance.BonusBalance > 0M && payoutMoney > 0M)
            {
                //奖金参与支付
                currentPayout = userBalance.BonusBalance >= payoutMoney ? payoutMoney : userBalance.BonusBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance -= currentPayout;
            }
            if (userBalance.CommissionBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CommissionBalance >= payoutMoney ? payoutMoney : userBalance.CommissionBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance -= currentPayout;
            }
            if (userBalance.ExpertsBalance > 0M && payoutMoney > 0M)
            {
                //名家参与支付
                currentPayout = userBalance.ExpertsBalance >= payoutMoney ? payoutMoney : userBalance.ExpertsBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance -= currentPayout;
            }

            #endregion

            responseMoney = requestMoney;
            var payCategory = WithdrawCategory.Compulsory;
            if (payoutMoney <= 0M)
            {
                payCategory = WithdrawCategory.General;
            }
            else
            {
                //使用充值金额扣款
                if (userBalance.FillMoneyBalance < payoutMoney)
                    throw new Exception("可用充值金额不足");

                #region 异常提现

                //收取5%手续费
                var percent = decimal.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("WithdrawAboutFillMoney.CutPercent"));
                var counterFee = payoutMoney * percent / 100;
                //到帐金额
                responseMoney = requestMoney - counterFee;

                //手续费明细
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = BusinessHelper.FundCategory_RequestWithdrawCounterFee,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = counterFee,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance - counterFee,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance -= counterFee;

                //到帐金额
                var resMoney = payoutMoney - counterFee;
                //写充值金额的扣款资金明细
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = resMoney,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance - resMoney,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance -= resMoney;

                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = counterFee + resMoney,
                    PayType = PayType.Payout,
                });

                #endregion
            }
            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());

            return payCategory;
        }

        /// <summary>
        /// 用户收入，冻结资金 还原到对应的账户
        /// 调用前提：必须要有之前加入冻结的订单
        /// </summary>
        public static void Payin_FrozenBack(string category, string userId, string orderId, decimal payMoney, string summary)
        {
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var fundList = fundManager.QueryFundDetailList(orderId, userId);
            if (fundList == null || fundList.Count == 0)
                throw new Exception(string.Format("未查询到用户{0}的订单{0}的支付明细", userId, orderId));

            //退款顺序：名家=>佣金=>奖金=>红包=>充值金额
            #region 按顺序退款

            var payDetailList = new List<PayDetail>();
            var payBackMoney = payMoney;
            var currentPayBack = 0M;
            var expertFund = fundList.Where(p => p.AccountType == AccountType.Experts).ToList();
            if (expertFund != null && expertFund.Count > 0 && payBackMoney > 0M)
            {
                //名家金额参与支付，退款到名家金额
                currentPayBack = payBackMoney >= expertFund.Sum(p => p.PayMoney) ? expertFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance += currentPayBack;
            }

            var commisionFund = fundList.Where(p => p.AccountType == AccountType.Commission).ToList();
            if (commisionFund != null && commisionFund.Count > 0 && payBackMoney > 0M)
            {
                //佣金金额参与支付，退款到佣金金额
                currentPayBack = payBackMoney >= commisionFund.Sum(p => p.PayMoney) ? commisionFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance += currentPayBack;
            }

            var bonusFund = fundList.Where(p => p.AccountType == AccountType.Bonus).ToList();
            if (bonusFund != null && bonusFund.Count > 0 && payBackMoney > 0M)
            {
                //奖金金额参与支付，退款到奖金金额
                currentPayBack = payBackMoney >= bonusFund.Sum(p => p.PayMoney) ? bonusFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance += currentPayBack;
            }

            var redBagFund = fundList.Where(p => p.AccountType == AccountType.RedBag).ToList();
            if (redBagFund != null && redBagFund.Count > 0 && payBackMoney > 0M)
            {
                //红包金额参与支付，退款到红包金额
                currentPayBack = payBackMoney >= redBagFund.Sum(p => p.PayMoney) ? redBagFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance += currentPayBack;
            }

            var fillFund = fundList.Where(p => p.AccountType == AccountType.FillMoney).ToList();
            if (fillFund != null && fillFund.Count > 0 && payBackMoney > 0M)
            {
                //充值金额参与支付，退款到充值金额
                currentPayBack = payBackMoney >= fillFund.Sum(p => p.PayMoney) ? fillFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance += currentPayBack;
            }
            if (payBackMoney > 0M)
                throw new Exception("退款金额大于总支付金额");

            #endregion

            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = payMoney,
                PayType = PayType.Payout,
                Summary = summary,
                UserId = userId,
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance - payMoney,
                OperatorId = userId,
            });
            //userBalance.FreezeBalance -= payMoney;
            if (userBalance.FreezeBalance > 0)
            {
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Freeze,
                    PayMoney = payMoney,
                    PayType = PayType.Payout,
                });
                //userBalance.FreezeBalance -= payMoney;
            }

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 从指定账户转到冻结
        /// </summary>
        public static void Payout_To_Frozen(AccountType accountType, string category, string userId, string orderId, decimal payoutMoney, string summary)
        {
            if (accountType == AccountType.Freeze)
                throw new Exception("冻结的账户不能是冻结账户");

            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            //加入冻结记录
            //var freeze = balanceManager.GetUserBalanceFreezeByOrder(userId, orderId);
            //if (freeze != null)
            //    throw new ArgumentException("订单金额已冻结 - " + orderId);
            //freeze = new UserBalanceFreeze
            //{
            //    UserId = userId,
            //    OrderId = orderId,
            //    FreezeMoney = payoutMoney,
            //    Category = FrozenCategory.Manual,
            //    Description = summary,
            //    CreateTime = DateTime.Now,
            //};
            //balanceManager.AddUserBalanceFreeze(freeze);

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
            });
            //冻结资金明细
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance + payoutMoney,
                OperatorId = userId,
            });

            //userBalance.FreezeBalance += payoutMoney;

            //扣除账户余额，写资金明细
            var before = 0M;
            var after = 0M;
            switch (accountType)
            {
                case AccountType.Bonus:
                    if (userBalance.BonusBalance < payoutMoney)
                        throw new Exception(string.Format("奖金账户不足{0:N2}元。", payoutMoney));
                    before = userBalance.BonusBalance;
                    after = userBalance.BonusBalance - payoutMoney;
                    //userBalance.BonusBalance = after;
                    break;
                case AccountType.Commission:
                    if (userBalance.CommissionBalance < payoutMoney)
                        throw new Exception(string.Format("佣金账户不足{0:N2}元。", payoutMoney));
                    before = userBalance.CommissionBalance;
                    after = userBalance.CommissionBalance - payoutMoney;
                    //userBalance.CommissionBalance = after;
                    break;
                case AccountType.FillMoney:
                    if (userBalance.FillMoneyBalance < payoutMoney)
                        throw new Exception(string.Format("充值账户不足{0:N2}元。", payoutMoney));
                    before = userBalance.FillMoneyBalance;
                    after = userBalance.FillMoneyBalance - payoutMoney;
                    //userBalance.FillMoneyBalance = after;
                    break;
                case AccountType.Experts:
                    if (userBalance.ExpertsBalance < payoutMoney)
                        throw new Exception(string.Format("名家账户不足{0:N2}元。", payoutMoney));
                    before = userBalance.ExpertsBalance;
                    after = userBalance.ExpertsBalance - payoutMoney;
                    //userBalance.ExpertsBalance = after;
                    break;
                case AccountType.RedBag:
                    if (userBalance.RedBagBalance < payoutMoney)
                        throw new Exception(string.Format("红包账户不足{0:N2}元。", payoutMoney));
                    before = userBalance.RedBagBalance;
                    after = userBalance.RedBagBalance - payoutMoney;
                    //userBalance.RedBagBalance = after;
                    break;
                default:
                    throw new ArgumentException("不支持的账户类型 - " + accountType);
            }
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = accountType,
                PayMoney = payoutMoney,
                PayType = PayType.Payout,
                Summary = summary,
                UserId = userId,
                BeforeBalance = before,
                AfterBalance = after,
                OperatorId = userId,
            });
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payoutMoney,
                PayType = PayType.Payout,
            });

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出，冻结到结束，清理冻结
        /// 用于完成提现，追号完成
        /// </summary>
        public static void Payout_Frozen_To_End(string category, string userId, string orderId, string summary, decimal clearMoney)
        {
            var fundManager = new FundManager();
            var balanceManager = new UserBalanceManager();

            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var old = fundManager.QueryUserClearChaseRecord(orderId, userId);
            if (old != null)
                return;

            var befor = userBalance.FreezeBalance;
            var after = userBalance.FreezeBalance - clearMoney;
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = AccountType.Freeze,
                PayMoney = clearMoney,
                PayType = PayType.Payout,
                Summary = summary,
                UserId = userId,
                BeforeBalance = befor,
                AfterBalance = after < 0M ? 0M : after,
                OperatorId = userId,
            });
            userBalance.FreezeBalance -= clearMoney;
            if (userBalance.FreezeBalance <= 0)
                userBalance.FreezeBalance = 0M;

            balanceManager.UpdateUserBalance(userBalance);
        }

        /// <summary>
        /// 用户收入，转到指定账户
        /// </summary>
        public static void Payin_To_Balance(AccountType accountType, string category, string userId, string orderId, decimal payMoney, string summary, RedBagCategory redBag = RedBagCategory.FillMoney, string operatorId = "")
        {
            //if (accountType == AccountType.Freeze)
            //    throw new Exception("退款账户不能为冻结账户");

            if (payMoney <= 0M)
                return;
            //throw new Exception("转入金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payMoney,
                PayType = PayType.Payin,
            });
            var before = 0M;
            var after = 0M;
            switch (accountType)
            {
                case AccountType.Bonus:
                    before = userBalance.BonusBalance;
                    after = userBalance.BonusBalance + payMoney;
                    //userBalance.BonusBalance = after;
                    break;
                case AccountType.Commission:
                    before = userBalance.CommissionBalance;
                    after = userBalance.CommissionBalance + payMoney;
                    //userBalance.CommissionBalance = after;
                    break;
                case AccountType.FillMoney:
                    before = userBalance.FillMoneyBalance;
                    after = userBalance.FillMoneyBalance + payMoney;
                    //userBalance.FillMoneyBalance = after;
                    break;
                case AccountType.Experts:
                    before = userBalance.ExpertsBalance;
                    after = userBalance.ExpertsBalance + payMoney;
                    //userBalance.ExpertsBalance = after;
                    break;
                case AccountType.RedBag:
                    before = userBalance.RedBagBalance;
                    after = userBalance.RedBagBalance + payMoney;
                    //userBalance.RedBagBalance = after;
                    fundManager.AddRedBagDetail(new RedBagDetail
                    {
                        CreateTime = DateTime.Now,
                        OrderId = orderId,
                        RedBagCategory = redBag,
                        RedBagMoney = payMoney,
                        UserId = userId,
                    });
                    break;
                case AccountType.Freeze:
                    before = userBalance.FreezeBalance;
                    after = userBalance.FreezeBalance + payMoney;
                    //userBalance.FreezeBalance = after;
                    break;
                case AccountType.CPS:
                    before = userBalance.CPSBalance;
                    after = userBalance.CPSBalance + payMoney;
                    //userBalance.CPSBalance = after;
                    break;
                default:
                    throw new ArgumentException("不支持的账户类型 - " + accountType);
            }
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = accountType,
                PayMoney = payMoney,
                PayType = PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = before,
                AfterBalance = after,
                OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
            });

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 充值专员支出 转到指定的账户
        /// </summary>
        /// <param name="accountType"></param>
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="payMoney"></param>
        /// <param name="summary"></param>
        /// <param name="redBag"></param>
        /// <param name="operatorId"></param>
        public static void PayOut_To_BalanceByCzzy(AccountType accountType, string category, string userId, string orderId, decimal payMoney, string summary, RedBagCategory redBag = RedBagCategory.FillMoney, string operatorId = "")
        {
            //if (accountType == AccountType.Freeze)
            //    throw new Exception("退款账户不能为冻结账户");

            if (payMoney <= 0M)
                return;
            //throw new Exception("转入金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }


            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payMoney,
                PayType = PayType.Payout,
            });
            var before = 0M;
            var after = 0M;
            switch (accountType)
            {
                case AccountType.Bonus:
                    before = userBalance.BonusBalance;
                    after = userBalance.BonusBalance - payMoney;
                    if (userBalance.BonusBalance < payMoney)
                    { throw new Exception("奖金帐户余额不足"); }
                    //userBalance.BonusBalance = after;
                    break;
                case AccountType.Commission:
                    before = userBalance.CommissionBalance;
                    after = userBalance.CommissionBalance - payMoney;
                    //userBalance.CommissionBalance = after;
                    break;
                case AccountType.FillMoney:
                    before = userBalance.FillMoneyBalance;
                    after = userBalance.FillMoneyBalance - payMoney;
                    if (userBalance.FillMoneyBalance < payMoney)
                    { throw new Exception("充值账户余额不足"); }
                    //userBalance.FillMoneyBalance = after;
                    break;
                case AccountType.Experts:
                    before = userBalance.ExpertsBalance;
                    after = userBalance.ExpertsBalance - payMoney;
                    //userBalance.ExpertsBalance = after;
                    break;
                case AccountType.RedBag:
                    before = userBalance.RedBagBalance;
                    after = userBalance.RedBagBalance - payMoney;
                    if (userBalance.RedBagBalance < payMoney)
                    { throw new Exception("红包帐户余额不足"); }
                    //userBalance.RedBagBalance = after;
                    fundManager.AddRedBagDetail(new RedBagDetail
                    {
                        CreateTime = DateTime.Now,
                        OrderId = orderId,
                        RedBagCategory = redBag,
                        RedBagMoney = payMoney,
                        UserId = userId,
                    });
                    break;
                case AccountType.Freeze:
                    before = userBalance.FreezeBalance;
                    after = userBalance.FreezeBalance - payMoney;
                    //userBalance.FreezeBalance = after;
                    break;
                case AccountType.CPS:
                    before = userBalance.CPSBalance;
                    after = userBalance.CPSBalance - payMoney;
                    //userBalance.CPSBalance = after;
                    break;
                default:
                    throw new ArgumentException("不支持的账户类型 - " + accountType);
            }
            fundManager.AddFundDetail(new FundDetail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = accountType,
                PayMoney = payMoney,
                PayType = PayType.Payout,
                Summary = summary,
                UserId = userId,
                BeforeBalance = before,
                AfterBalance = after,
                OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
            });

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 用户支出，自动扣除用户金额，非冻结
        /// 用于赢家平台模型转入先行赔付金；20150226 dj
        /// </summary>
        public static void Payout_To_End_Model(string category, string userId, string orderId, decimal payoutMoney, string summary, string place, string password, bool containRedBag = true)
        {
            if (payoutMoney <= 0M)
                throw new Exception("消费金额不能小于0");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            {
                if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                {
                    password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                    if (!userBalance.Password.ToUpper().Equals(password))
                    {
                        throw new Exception("资金密码输入错误");
                    }
                }
            }

            var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance + userBalance.RedBagBalance;
            if (totalMoney < payoutMoney)
                throw new Exception(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            var payDetailList = new List<PayDetail>();
            //消费顺序：红包=>佣金=>奖金=>充值=>名家
            #region 按顺序消费用户余额

            var currentPayout = 0M;
            if (containRedBag)
            {
                if (userBalance.RedBagBalance > 0M && payoutMoney > 0M)
                {
                    //充值金额参与支出
                    currentPayout = userBalance.RedBagBalance >= payoutMoney ? payoutMoney : userBalance.RedBagBalance;
                    payoutMoney -= currentPayout;
                    payDetailList.Add(new PayDetail
                    {
                        AccountType = AccountType.RedBag,
                        PayMoney = currentPayout,
                        PayType = PayType.Payout,
                    });
                    fundManager.AddFundDetail(new FundDetail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = AccountType.RedBag,
                        PayMoney = currentPayout,
                        PayType = PayType.Payout,
                        Summary = summary,
                        UserId = userId,
                        BeforeBalance = userBalance.RedBagBalance,
                        AfterBalance = userBalance.RedBagBalance - currentPayout,
                        OperatorId = userId,
                    });
                    //userBalance.RedBagBalance -= currentPayout;
                }
            }
            if (userBalance.CommissionBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CommissionBalance >= payoutMoney ? payoutMoney : userBalance.CommissionBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance -= currentPayout;
            }
            if (userBalance.BonusBalance > 0M && payoutMoney > 0M)
            {
                //奖金参与支付
                currentPayout = userBalance.BonusBalance >= payoutMoney ? payoutMoney : userBalance.BonusBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance -= currentPayout;
            }
            if (userBalance.FillMoneyBalance > 0M && payoutMoney > 0M)
            {
                //红包参与支付
                currentPayout = userBalance.FillMoneyBalance >= payoutMoney ? payoutMoney : userBalance.FillMoneyBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance -= currentPayout;
            }

            if (userBalance.ExpertsBalance > 0M && payoutMoney > 0M)
            {
                //名家参与支付
                currentPayout = userBalance.ExpertsBalance >= payoutMoney ? payoutMoney : userBalance.ExpertsBalance;
                payoutMoney -= currentPayout;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = PayType.Payout,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance - currentPayout,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance -= currentPayout;
            }
            if (payoutMoney > 0M)
                throw new Exception("用户余额不足");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }
        /// <summary>
        /// 用户收入，转到指定账户
        /// 用于赢家平台模型转出先行赔付金；20150226 dj
        /// </summary>
        public static void Payback_To_Balance_Model(string category, string userId, string orderId, decimal payBackMoney, string summary)
        {
            if (payBackMoney <= 0M)
                throw new Exception("退款金额不能小于0");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var fundList = fundManager.QueryFundDetailList(orderId, userId);
            if (fundList == null || fundList.Count == 0)
                throw new Exception(string.Format("未查询到用户{0}的订单{1}的支付明细", userId, orderId));
            if (fundList.Sum(p => p.PayMoney) < payBackMoney)
                throw new Exception("退款金额大于订单总支付金额");

            //退款顺序：佣金=>奖金=>充值=>红包
            #region 按顺序退款

            var currentPayBack = 0M;
            var payDetailList = new List<PayDetail>();

            var commisionFund = fundList.Where(p => p.AccountType == AccountType.Commission).ToList();
            if (commisionFund != null && commisionFund.Count > 0 && payBackMoney > 0M)
            {
                //佣金金额参与支付，退款到佣金金额
                currentPayBack = payBackMoney >= commisionFund.Sum(p => p.PayMoney) ? commisionFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance += currentPayBack;
            }

            var bonusFund = fundList.Where(p => p.AccountType == AccountType.Bonus).ToList();
            if (bonusFund != null && bonusFund.Count > 0 && payBackMoney > 0M)
            {
                //奖金金额参与支付，退款到奖金金额
                currentPayBack = payBackMoney >= bonusFund.Sum(p => p.PayMoney) ? bonusFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance += currentPayBack;
            }

            var fillFund = fundList.Where(p => p.AccountType == AccountType.FillMoney).ToList();
            if (fillFund != null && fillFund.Count > 0 && payBackMoney > 0M)
            {
                //充值金额参与支付，退款到充值金额
                currentPayBack = payBackMoney >= fillFund.Sum(p => p.PayMoney) ? fillFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance += currentPayBack;
            }

            var redBagFund = fundList.Where(p => p.AccountType == AccountType.RedBag).ToList();
            if (redBagFund != null && redBagFund.Count > 0 && payBackMoney > 0M)
            {
                //红包金额参与支付，退款到红包金额
                currentPayBack = payBackMoney >= redBagFund.Sum(p => p.PayMoney) ? redBagFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance += currentPayBack;
            }

            var expertFund = fundList.Where(p => p.AccountType == AccountType.Experts).ToList();
            if (expertFund != null && expertFund.Count > 0 && payBackMoney > 0M)
            {
                //名家金额参与支付，退款到名家金额
                currentPayBack = payBackMoney >= expertFund.Sum(p => p.PayMoney) ? expertFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance += currentPayBack;
            }
            if (payBackMoney > 0M)
                throw new Exception("退款金额大于总支付金额");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        #endregion

        #region 成长值 和 澳彩豆豆

        /// <summary>
        /// 收入 --添加用户成长值
        /// 返回用户vip等级
        /// </summary>
        public static int Payin_UserGrowth(string category, string orderId, string userId, int userGrowth, string summary)
        {
            if (userGrowth <= 0) return 0;

            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            var user = balanceManager.QueryUserRegister(userId);
            var userBalance = balanceManager.QueryUserBalance(userId);

            fundManager.AddUserGrowthDetail(new UserGrowthDetail
            {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = userBalance.UserGrowth,
                PayMoney = userGrowth,
                PayType = PayType.Payin,
                Summary = summary,
                AfterBalance = userBalance.UserGrowth + userGrowth,
            });
            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.UserGrowth,
                PayMoney = userGrowth,
                PayType = PayType.Payin,
            });

            var vipLevel = GetUserVipLevel(userBalance.UserGrowth + userGrowth);
            //更新成长值
            //userBalance.UserGrowth += userGrowth;
            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
            if (user.VipLevel < vipLevel)
            {
                for (int i = user.VipLevel + 1; i <= vipLevel; i++)
                {
                    //达到相应等级赠送红包
                    if (new int[] { 3, 4, 5, 6 }.Contains(i))
                    {
                        var redBag = 0M;
                        switch (i)
                        {
                            case 3:
                                redBag = 2M;
                                break;
                            case 4:
                                redBag = 10M;
                                break;
                            case 5:
                                redBag = 20M;
                                break;
                            case 6:
                                redBag = 88M;
                                break;
                            default:
                                break;
                        }
                        if (redBag > 0M)
                            Payin_To_Balance(AccountType.RedBag, FundCategory_UserLevelUp, userId, orderId, redBag, string.Format("用户等级提升到{0}级", i), RedBagCategory.UserUpLevel);
                    }
                }
                //修改vip等级
                user.VipLevel = vipLevel;
                balanceManager.UpdateUserRegister(user);
            }

            return user.VipLevel;
        }

        /// <summary>
        /// 计算用户等级
        /// </summary>
        private static int GetUserVipLevel(int userGrowth)
        {
            if (userGrowth >= 20000)
                return 9;
            if (userGrowth >= 16000)
                return 8;
            if (userGrowth >= 12000)
                return 7;
            if (userGrowth >= 8000)
                return 6;
            if (userGrowth >= 4000)
                return 5;
            if (userGrowth >= 2000)
                return 4;
            if (userGrowth >= 1000)
                return 3;
            if (userGrowth >= 500)
                return 2;
            return 0;
        }

        /// <summary>
        /// 收入 --添加用户豆豆值
        /// </summary>
        public static void Payin_OCDouDou(string category, string orderId, string userId, int doudou, string summary)
        {
            if (doudou <= 0) return;

            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            var user = balanceManager.QueryUserBalance(userId);
            fundManager.AddOCDouDouDetail(new OCDouDouDetail
            {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = user.CurrentDouDou,
                PayMoney = doudou,
                PayType = PayType.Payin,
                Summary = summary,
                AfterBalance = user.CurrentDouDou + doudou,
            });
            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.DouDou,
                PayMoney = doudou,
                PayType = PayType.Payin,
            });

            //user.CurrentDouDou += doudou;
            //balanceManager.UpdateUserBalance(user);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        /// 支出 --使用豆豆
        /// </summary>
        public static void Payout_OCDouDou(string category, string orderId, string userId, int doudou, string summary)
        {
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            var user = balanceManager.QueryUserBalance(userId);
            if (user.CurrentDouDou < doudou)
                throw new Exception("豆豆不足" + doudou);

            fundManager.AddOCDouDouDetail(new OCDouDouDetail
            {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = user.CurrentDouDou,
                PayMoney = doudou,
                PayType = PayType.Payout,
                Summary = summary,
                AfterBalance = user.CurrentDouDou - doudou,
            });
            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.DouDou,
                PayMoney = doudou,
                PayType = PayType.Payout,
            });

            //user.CurrentDouDou -= doudou;
            //balanceManager.UpdateUserBalance(user);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
        }


        #endregion

        #endregion

        public static string GetAccountName(AccountType type)
        {
            switch (type)
            {
                case AccountType.Bonus:
                    return "奖金账户";
                case AccountType.Freeze:
                    return "冻结账户";
                case AccountType.Commission:
                    return "佣金账户";
                case AccountType.FillMoney:
                    return "充值账户";
                case AccountType.Experts:
                    return "名家账户";
                case AccountType.RedBag:
                    return "红包账户";
                default:
                    throw new ArgumentException("不支持的账户类型 - " + type);
            }
        }

        /// <summary>
        /// 解析彩种为中文名称
        /// </summary>
        public static string FormatGameCode(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return "北京单场";
                case "JCZQ":
                    return "竞彩足球";
                case "JCLQ":
                    return "竞彩篮球";
                case "CTZQ":
                    return "传统足球";
                case "SSQ":
                    return "双色球";
                case "DLT":
                    return "大乐透";
                case "FC3D":
                    return "福彩3D";
                case "PL3":
                    return "排列3";
                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
                case "CQSSC":
                    return "重庆时时彩";
                case "JX11X5":
                    return "江西11选五";
                case "SD11X5":
                    return "山东11选5";
                case "GD11X5":
                    return "广东11选5";
                case "GDKLSF":
                    return "广东快乐十分";
                case "JSKS":
                    return "江苏快三";
                case "SDKLPK3":
                    return "山东快乐扑克3";

            }
            return gameCode;
        }

        /// <summary>
        /// 解析玩法为中文名称
        /// </summary>
        public static string FormatGameType(string gameCode, string gameType)
        {
            var nameList = new List<string>();
            var typeList = gameType.Split(',', '|');
            foreach (var t in typeList)
            {
                nameList.Add(FormatGameType_Each(gameCode, t));
            }
            return string.Join(",", nameList.ToArray());
        }

        public static string FormatGameType_Each(string gameCode, string gameType)
        {
            switch (gameCode)
            {
                #region 足彩

                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":
                            return "胜平负";
                        case "ZJQ":
                            return "总进球";
                        case "SXDS":
                            return "上下单双";
                        case "BF":
                            return "比分";
                        case "BQC":
                            return "半全场";
                    }
                    break;
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return "让球胜平负";
                        case "BRQSPF":
                            return "胜平负";
                        case "BF":
                            return "比分";
                        case "ZJQ":
                            return "总进球";
                        case "BQC":
                            return "半全场";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return "胜负";
                        case "RFSF":
                            return "让分胜负";
                        case "SFC":
                            return "胜分差";
                        case "DXF":
                            return "大小分";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return "胜负14场";
                        case "TR9":
                            return "任选9";
                        case "T6BQC":
                            return "6场半全场";
                        case "T4CJQ":
                            return "4场进球";
                    }
                    break;

                #endregion

                #region 重庆时时彩

                case "CQSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "3XHZ":    // 三星和值
                            return "三星和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XBAODAN":   // 二星组选包胆
                            return "二星组选包胆";
                        case "3XBAODAN":   // 三星组选包胆
                            return "三星组选包胆";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "3XBAODIAN":   // 三星组选包点
                            return "三星组选包点";
                        case "2XZXFS":   // 二星组选复式
                            return "二星组选复式";
                        case "2XZXFW":   // 二星组选分位
                            return "二星组选分位";
                        case "3XZXZH":   // 三星直选组合
                            return "三星直选组合";
                    }
                    break;

                #endregion

                #region 江西时时彩

                case "JXSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "4XDX":
                            return "四星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "2XZX":   // 二星组选
                            return "二星组选";
                        case "RX1":   // 任选一
                            return "任选一";
                        case "RX2":   // 任选二
                            return "任选二";
                    }
                    break;

                #endregion

                #region 山东十一选五、广东十一选五、江西十一选五

                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                    switch (gameType)
                    {
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "RX7":
                            return "任选七";
                        case "RX8":
                            return "任选八";
                        case "Q2ZHIX":
                            return "前二直选";
                        case "Q3ZHIX":
                            return "前三直选";
                        case "Q2ZUX":
                            return "前二组选";
                        case "Q3ZUX":
                            return "前三组选";
                    }
                    break;

                #endregion

                #region 广东快乐十分

                case "GDKLSF":
                    switch (gameType)
                    {
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "X1HT":
                            return "选一红投";
                        case "X1ST":
                            return "选一数投";
                        case "X2LZHI":
                            return "选二连直";
                        case "X2LZU":
                            return "选二连组";
                        case "X3QZHI":
                            return "选三连直";
                        case "X3QZU":
                            return "选三连组";
                    }
                    break;

                #endregion

                #region 江苏快三

                case "JSKS":
                    switch (gameType)
                    {
                        case "2BTH":
                            return "二不同号";
                        case "2BTHDT":
                            return "二不同号单选";
                        case "2THDX":
                            return "二同号单选";
                        case "2THFX":
                            return "二同号复选";
                        case "3BTH":
                            return "三不同号";
                        case "3BTHDT":
                            return "三不同号单选";
                        case "3LHTX":
                            return "三连号通选";
                        case "3THDX":
                            return "三同号单选";
                        case "3THTX":
                            return "三同号通选";
                        case "HZ":
                            return "和值";
                    }
                    break;

                #endregion

                #region 山东快乐扑克3

                case "SDKLPK3":
                    switch (gameType)
                    {
                        case "BZ":
                            return "豹子";
                        case "DZ":
                            return "对子";
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "SZ":
                            return "顺子";
                        case "TH":
                            return "同花";
                        case "THS":
                            return "同花顺";
                    }
                    break;

                #endregion


                #region 福彩3D、排列三

                case "FC3D":
                case "PL3":
                    switch (gameType)
                    {
                        case "FS":
                            return "复式";
                        case "HZ":
                            return "和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                    }
                    break;

                #endregion

                #region 双色球

                case "SSQ":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                    }
                    break;

                #endregion

                #region 大乐透

                case "DLT":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                        case "12X2DS":
                            return "12生肖";
                        case "12X2FS":
                            return "12生肖";
                    }
                    break;


                #endregion

                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
            }
            return gameType;
        }

        #region 插件相关

        private static List<PluginClass> _enablePluginClass = new List<PluginClass>();

        public static void ClearPlugin()
        {
            if (_enablePluginClass != null)
                _enablePluginClass.Clear();
        }

        public static void ExecPlugin<T>(object inputParam)
                   where T : class, IPlugin
        {
            if (_enablePluginClass == null || _enablePluginClass.Count == 0)
                _enablePluginClass = new PluginClassManager().QueryPluginClass(true);


            foreach (var plugin in _enablePluginClass)
            {
                try
                {
                    if (typeof(T).FullName != plugin.InterfaceName) continue;

                    if (plugin.StartTime.HasValue && plugin.StartTime.Value > DateTime.Now) continue;//未开始
                    if (plugin.EndTime.HasValue && plugin.EndTime.Value < DateTime.Now) continue;//已结束

                    var fullName = plugin.ClassName + "," + plugin.AssemblyFileName;
                    var type = Type.GetType(fullName);
                    if (type == null)
                    {
                        throw new ArgumentNullException("类型在当前域中不存在，或对应组件未加载：" + fullName);
                    }
                    var i = Type.GetType(fullName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as T;
                    if (i == null)
                    {
                        throw new ArgumentNullException("无法实例化对象：" + fullName);
                    }
                    //new Thread(() =>
                    //{
                    try
                    {
                        i.ExecPlugin(typeof(T).Name, inputParam);
                    }
                    catch (AggregateException ex)
                    {
                        throw new AggregateException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        writer.Write("ERROR_ExecPlugin", "_ExecPlugin", Common.Log.LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                    }
                    //}).Start();
                }
                catch (AggregateException ex)
                {
                    throw new AggregateException(ex.Message);
                }
                catch (Exception ex)
                {
                    var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    writer.Write("ERROR_ExecPlugin", "_ExecPlugin", Common.Log.LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                }
            }

        }

        #endregion

        #region 订单撤单后通知用户

        public static string GetTogetherStatusName(TogetherSchemeProgress status)
        {
            switch (status)
            {
                case TogetherSchemeProgress.SalesIn:
                    return "销售中";
                case TogetherSchemeProgress.Standard:
                    return "达到目标";
                case TogetherSchemeProgress.Finish:
                    return "满员";
                case TogetherSchemeProgress.Cancel:
                    return "撤销";
                case TogetherSchemeProgress.Completed:
                    return "已完成";
                case TogetherSchemeProgress.AutoStop:
                    return "自动停止";

            }
            return string.Empty;
        }

        #endregion

        #region 发送短信

        public static bool SendMsg(string mobile, string content, string ip, int msgType, string userId, string schemeId, int msgId = 0)
        {
            string status = string.Empty;
            string errorMsg = string.Empty;
            SendMsgHistoryRecordInfo info = new SendMsgHistoryRecordInfo();
            try
            {
                var agentName = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Name");
                var attach = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Attach");
                var password = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Password");
                var userName = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.UserId");
                var result = Common.Net.SMS.SMSSenderFactory.GetSMSSenderInstance(new Common.Net.SMS.SMSConfigInfo
                {
                    AgentName = agentName,
                    Attach = attach,
                    Password = password,
                    UserName = userName
                }).SendSMS(mobile, content, attach);

                var resultInfo = AnalyticalResult(result);
                if (resultInfo != null)
                {
                    info.SMSId = resultInfo.smsid;
                    status = resultInfo.code;
                }
                if (status == "2")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                status = "-1";
                errorMsg = ex.Message;
                return false;
            }
            finally
            {
                string statusName = GetVeeSingStatusName(status);
                info.IP = ip;
                info.MsgContent = content;
                info.PhoneNumber = mobile;
                info.MsgType = msgType;
                info.MsgId = msgId;
                info.MsgResultStatus = status;
                info.MsgContent = content;
                info.UserId = userId;
                info.SchemeId = schemeId;
                if (status == "-1")
                    info.MsgStatusDesc = errorMsg;
                else
                    info.MsgStatusDesc = statusName;
                SaveSMSHistoryRecord(info);
            }
        }
        public static void SaveSMSHistoryRecord(SendMsgHistoryRecordInfo info)
        {
            using (var manager = new UserBalanceManager())
            {
                if (info != null)
                {
                    if (info.MsgId > 0)//后台列表手工发送时修改回执状态即可
                    {
                        var entity = manager.QueryMsgHistoryRecordByMsgId(info.MsgId);
                        if (entity != null)
                        {
                            entity.SendTime = DateTime.Now;
                            entity.MsgStatusDesc = info.MsgStatusDesc;
                            entity.MsgResultStatus = info.MsgResultStatus;
                            entity.SMSId = info.SMSId;
                            entity.SendNumber += 1;
                            manager.UpdateMsgHistoryRecord(entity);
                        }
                    }
                    else
                    {
                        info.CreateTime = DateTime.Now;
                        info.SendTime = DateTime.Now;
                        info.SendNumber = 0;
                        SendMsgHistoryRecord entity = new SendMsgHistoryRecord();
                        ObjectConvert.ConverInfoToEntity(info, ref entity);
                        manager.AddSendMsgHistoryRecord(entity);
                    }
                }
            }
        }
        public static ResultVeeSingInfo AnalyticalResult(string result)
        {
            ResultVeeSingInfo info = new ResultVeeSingInfo();
            if (!string.IsNullOrEmpty(result))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(result);
                var node = doc.ChildNodes;
                if (node != null && node.Count >= 2)
                {
                    for (int i = 0; i < node[1].ChildNodes.Count; i++)
                    {
                        if (node[1].ChildNodes[i].Name.ToLower() == "code")
                            info.code = node[1].ChildNodes[i].InnerText;
                        else if (node[1].ChildNodes[i].Name.ToLower() == "msg")
                            info.msg = node[1].ChildNodes[i].InnerText;
                        else if (node[1].ChildNodes[i].Name.ToLower() == "smsid")
                            info.smsid = node[1].ChildNodes[i].InnerText;
                        else if (node[1].ChildNodes[i].Name.ToLower() == "num")
                            info.num = node[1].ChildNodes[i].InnerText;
                    }
                    //info.code = doc.SelectSingleNode("code").Value;
                    //info.msg = doc.SelectSingleNode("msg").Value;
                    //info.smsid = doc.SelectSingleNode("smsid").Value;
                }
            }
            return info;
        }
        public static string GetVeeSingStatusName(string status)
        {
            switch (status)
            {
                case "-1"://自定义状态，接口异常情况
                    return "发送信息接口异常";
                case "0":
                    return "提交失败";
                case "2":
                    return "提交成功";
                case "400":
                    return "非法ip访问";
                case "401":
                    return "帐号不能为空";
                case "4010":
                    return "通道限制：每个号码1分钟内只能发1条";
                case "402":
                    return "密码不能为空";
                case "403":
                    return "手机号码不能为空";
                case "4030":
                    return "手机号码已被列入黑名单";
                case "404":
                    return "短信内容不能为空";
                case "405":
                    return "用户名或密码不正确";
                case "4050":
                    return "账号被冻结";
                case "4051":
                    return "剩余条数不足";
                case "4052":
                    return "访问ip与备案ip不符";
                case "406":
                    return "手机格式不正确";
                case "407":
                    return "短信内容含有敏感字符";
                case "4070":
                    return "签名格式不正确";
                case "4071":
                    return "没有提交备案模板";
                case "4072":
                    return "短信内容与模板不匹配";
                case "4073":
                    return "短信内容超出长度限制";
                case "408":
                    return "同一手机号码一分钟之内发送频率超过10条，系统将冻结你的帐号";
                case "4080":
                    return "同一手机号码同一秒钟不能超过2条";
                case "4081":
                    return "同一手机号码一分钟之内发送频率超不能超过1条";
                case "4082":
                    return "同一手机号码一天之内发送频率超不能超过5条";
                case "4083":
                    return "同内容每分钟限制：1条";
                case "4084":
                    return "同内容每日限制：5条";
                case "4085":
                    return "同一手机号码验证码短信发送量超出5条";
                case "4086":
                    return "提交失败，同一个手机号码发送频率太频繁";
                default:
                    return "发送信息接口异常";
            }
        }
        /// <summary>
        /// 查询短信余额
        /// </summary>
        /// <returns></returns>
        public static string GetSMSBalance()
        {
            var agentName = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Name");
            var attach = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Attach");
            var password = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.Password");
            var userName = new CacheDataBusiness().QueryCoreConfigFromRedis("SMSAgent.UserId");
            var result = Common.Net.SMS.SMSSenderFactory.GetSMSSenderInstance(new Common.Net.SMS.SMSConfigInfo
            {
                AgentName = agentName,
                Attach = attach,
                Password = password,
                UserName = userName
            }).GetBalance();
            var info = AnalyticalResult(result);
            return info.num;
        }

        #endregion

        public static void CheckUserRealName(string idCardNumber)
        {
            if (string.IsNullOrEmpty(idCardNumber))
                throw new Exception("用户身份证信息不完整，不能购买彩票");
            if (idCardNumber.Length < 18)
            {
                if (idCardNumber.Length != 15)
                    throw new Exception("用户身份证号格式不正确，不能购买彩票");
            }
            else if (idCardNumber.Length > 15)
            {
                if (idCardNumber.Length != 18)
                    throw new Exception("用户身份证号格式不正确，不能购买彩票");
            }

            var birth = string.Empty;
            int year = 0;
            int month = 0;
            int day = 0;
            if (idCardNumber.Length == 18)
                birth = idCardNumber.Substring(6, 8);
            if (idCardNumber.Length == 15)
                birth = string.Format("19{0}", idCardNumber.Substring(6, 6));
            if (birth.Length != 8)
                throw new Exception("用户身份证号格式不正确，不能购买彩票");

            year = int.Parse(birth.Substring(0, 4));
            month = int.Parse(birth.Substring(4, 2));
            day = int.Parse(birth.Substring(6, 2));

            var diffYear = DateTime.Now.Year - year;
            if (diffYear > 18)
                return;
            if (diffYear < 18)
                throw new Exception("用户未满18岁，不能购买彩票");
            if (diffYear == 18)
            {
                if (DateTime.Now.Month < month)
                    throw new Exception("用户未满18岁，不能购买彩票");
                else if (DateTime.Now.Month == month)
                {
                    if (DateTime.Now.Day < day)
                        throw new Exception("用户未满18岁，不能购买彩票");
                }
            }
        }

        public static void CheckDisableGame(string gameCode, string gameType)
        {
            var status = new GameBusiness().LotteryGameToStatus(gameCode);
            if (status != Common.EnableStatus.Enable)
                throw new Exception("彩种暂时不能投注");
        }

        #region 网站各种限制

        public static void CheckBalance(string userId, string password = "", string place = "")
        {
            var balanceManager = new UserBalanceManager();
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null)
                throw new Exception("未查询到您的账户信息");
            else if ((userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance) <= 0)
                throw new Exception("对不起！您当前的余额不足");
            //资金密码判断
            if (!string.IsNullOrEmpty(password))
            {
                if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
                {
                    if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
                    {
                        password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                        if (!userBalance.Password.ToUpper().Equals(password))
                        {
                            throw new Exception("资金密码输入错误");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 所有彩种
        /// </summary>
        private static List<LotteryGame> _cacheAllGameList = new List<LotteryGame>();
        /// <summary>
        /// 查询彩种
        /// </summary>
        public static LotteryGame QueryLotteryGame(string gameCode)
        {
            if (_cacheAllGameList == null || _cacheAllGameList.Count <= 0)
            {
                var manager = new LotteryGameManager();
                _cacheAllGameList.AddRange(manager.QueryAllGame());
            }
            return _cacheAllGameList.FirstOrDefault(p => p.GameCode == gameCode);
        }

        /// <summary>
        /// 重新加载彩种
        /// </summary>
        public static void ReloadLotteryGame()
        {
            if (_cacheAllGameList == null)
                _cacheAllGameList = new List<LotteryGame>();
            _cacheAllGameList.Clear();
            _cacheAllGameList.AddRange(new LotteryGameManager().QueryAllGame());
        }

        /// <summary>
        /// 检查用户是否实名
        /// </summary>
        public static bool IsUserValidateRealName(string userId)
        {
            return new SqlQueryManager().IsUserValidateRealName(userId);
        }

        /// <summary>
        /// 检查彩种是否开启
        /// </summary>
        public static void CheckGameEnable(string gameCode)
        {
            var game = QueryLotteryGame(gameCode);
            if (game == null)
                throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
            if (game.EnableStatus != EnableStatus.Enable)
                throw new LogicException(string.Format("{0} 暂停销售", game.DisplayName));
        }

        public static void CheckGameCodeAndType(string gameCode, string gameType)
        {
            if (gameCode.ToUpper() == "BJDC")
            {
                if (new string[] { "ZJQ", "SXDS", "BQC", "BF" }.Contains(gameType.ToUpper()))
                    throw new LogicException("该玩法暂停销售");
            }
        }

        /// <summary>
        /// 验证投注号码 返回实际投注注数 by dzq
        /// </summary>
        public static int CheckBetCode(string userId, string gameCode, string gameType, SchemeSource schemeSource, string playType, int amount, decimal schemeTotalMoney, Sports_AnteCodeInfoCollection anteCodeList)
        {
            if (anteCodeList == null || anteCodeList.Count <= 0)
                throw new LogicException("投注内容不能为空");

            var gameTypeList = new List<string>();
            gameTypeList.Add(gameType);
            //验证投注内容是否合法，或是否重复
            foreach (var item in anteCodeList)
            {
                var oneCodeArray = item.AnteCode.Split(',');
                if (oneCodeArray.Distinct().Count() != oneCodeArray.Length)
                    throw new LogicException(string.Format("投注号码{0}中包括重复的内容", item.AnteCode));

                //检查投注内容
                //CheckSportAnteCode(gameCode, string.IsNullOrEmpty(item.GameType) ? gameType : item.GameType.ToUpper(), oneCodeArray);
                var error = string.Empty;
                AnalyzerFactory.GetSportAnteCodeChecker(gameCode, item.GameType).CheckAntecodeNumber(item, out error);
                if (!string.IsNullOrEmpty(error))
                    throw new LogicException(error);

                gameTypeList.Add(item.GameType);
            }

            var totalMoney = 0M;
            var totalCount = 0;
            var tmp = playType.Split('|');
            foreach (var chuan in tmp)
            {
                var chuanArray = chuan.Split('_');
                if (chuanArray.Length != 2)
                    throw new LogicException(string.Format("串关方式：{0}  不正确", chuan));
                var m = int.Parse(chuanArray[0]);
                var n = int.Parse(chuanArray[1]);

                //检查串关方式 是否正确
                CheckPlayType(gameCode, gameTypeList, m);

                #region 计算注数

                //串关包括的真实串数
                var c = new Combination();
                var countList = SportAnalyzer.AnalyzeChuan(m, n);
                if (n > 1)
                {
                    //3_3类型
                    c.Calculate(anteCodeList.ToArray(), m, (arr) =>
                    {
                        //m场比赛
                        if (arr.Select(p => p.MatchId).Distinct().Count() == m)
                        {
                            foreach (var count in countList)
                            {
                                //M串1
                                c.Calculate(arr, count, (a1) =>
                                {
                                    var cCount = 1;
                                    foreach (var t in a1)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                });
                            }
                        }
                    });
                }
                else
                {
                    var ac = new ArrayCombination();
                    var danList = anteCodeList.Where(a => a.IsDan).ToList();
                    var totalCodeList = new List<Sports_AnteCodeInfo[]>();
                    foreach (var g in anteCodeList.GroupBy(p => p.MatchId))
                    {
                        totalCodeList.Add(anteCodeList.Where(p => p.MatchId == g.Key).ToArray());
                    }
                    //3_1类型
                    foreach (var count in countList)
                    {
                        c.Calculate(totalCodeList.ToArray(), count, (arr2) =>
                        {
                            ac.Calculate(arr2, (tuoArr) =>
                            {
                                #region 拆分组合票

                                var isContainsDan = true;
                                foreach (var dan in danList)
                                {
                                    var con = tuoArr.FirstOrDefault(p => p.MatchId == dan.MatchId);
                                    if (con == null)
                                    {
                                        isContainsDan = false;
                                        break;
                                    }
                                }

                                if (isContainsDan)
                                {
                                    var cCount = 1;
                                    foreach (var t in tuoArr)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                }

                                #endregion
                            });
                        });
                    }
                }

                #endregion
            }
            totalMoney = totalCount * amount * 2M;

            if (totalMoney != schemeTotalMoney)
                throw new LogicException(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。", totalMoney, schemeTotalMoney));

            return totalCount;
        }

        /// <summary>
        /// 检查串关方式
        /// </summary>
        private static void CheckPlayType(string gameCode, List<string> gameTypeList, int m)
        {
            if (gameCode.ToUpper() != "JCZQ" && gameCode.ToUpper() != "JCLQ")
                return;
            //竞彩足球 BF、BQC 最高串关4，ZJQ最高串关6
            //竞彩篮球 SFC 最高串关4
            if (gameTypeList.Contains("BF") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("BQC") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("ZJQ") && m > 6)
                throw new LogicException("总进球过关方式最大为6串");
            if (gameTypeList.Contains("SFC") && m > 4)
                throw new LogicException("胜分差过关方式最大为4串");
        }

        /// <summary>
        /// 检查投注内容
        /// </summary>
        private static void CheckSportAnteCode(string gameCode, string gameType, string[] anteCode)
        {
            var allowCodeList = new string[] { };
            switch (gameCode)
            {
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负
                        case "BRQSPF": //不让球胜平负
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":     // 胜负
                        case "RFSF":     // 让分胜负
                        case "DXF":     // 大小分
                            allowCodeList = "3,0".Split(',');
                            break;
                        case "SFC":      // 胜分差
                            allowCodeList = "01,02,03,04,05,06,11,12,13,14,15,16".Split(',');
                            break;
                    }
                    break;
                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负 
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "SXDS":    // 上下单双。上单 3；上双 2；下单 1；下双 0；
                            allowCodeList = "SD,SS,XD,XS".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                        case "SF":
                            allowCodeList = "3,0".Split(',');
                            break;
                    }
                    break;
                default:
                    break;
            }
            foreach (var item in anteCode)
            {
                if (!allowCodeList.Contains(item))
                    throw new Exception(string.Format("投注号码{0}为非法字符", item));
            }
        }

        #region 内存缓存竞彩数据

        ///// <summary>
        ///// 竞彩足球当前可投注的比赛缓存
        ///// </summary>
        //private static List<JCZQ_Match> _cacheCurrentJCZQMatch = new List<JCZQ_Match>();
        ///// <summary>
        ///// 查询竞彩足球当前可投注的比赛
        ///// </summary>
        //public static List<JCZQ_Match> QueryCurrentJCZQMatchList()
        //{
        //    if (_cacheCurrentJCZQMatch == null)
        //        _cacheCurrentJCZQMatch = new List<JCZQ_Match>();
        //    if (_cacheCurrentJCZQMatch.Count <= 0)
        //        _cacheCurrentJCZQMatch = new Sports_Manager().QueryJCZQCurrentMatchList();
        //    return _cacheCurrentJCZQMatch;
        //}
        ///// <summary>
        ///// 重新加载竞彩足球当前比赛缓存
        ///// </summary>
        //public static void ReloadCurrentJCZQMatch()
        //{
        //    if (_cacheCurrentJCZQMatch == null)
        //        _cacheCurrentJCZQMatch = new List<JCZQ_Match>();
        //    _cacheCurrentJCZQMatch.Clear();
        //    _cacheCurrentJCZQMatch = new Sports_Manager().QueryJCZQCurrentMatchList();
        //}

        //private static List<JCZQ_OZBMatch> _cacheCurrentJCZQ_OZBMatch = new List<JCZQ_OZBMatch>();
        ///// <summary>
        ///// 查询竞彩足球当前可投注的比赛
        ///// </summary>
        //public static List<JCZQ_OZBMatch> QueryCurrentJCZQ_OZBMatchList()
        //{
        //    if (_cacheCurrentJCZQ_OZBMatch == null)
        //        _cacheCurrentJCZQ_OZBMatch = new List<JCZQ_OZBMatch>();
        //    if (_cacheCurrentJCZQ_OZBMatch.Count <= 0)
        //        _cacheCurrentJCZQ_OZBMatch = new Sports_Manager().QueryJCZQ_OZBCurrentMatchList();
        //    return _cacheCurrentJCZQ_OZBMatch;
        //}
        ///// <summary>
        ///// 重新加载竞彩足球欧洲杯比赛缓存
        ///// </summary>
        //public static void ReloadCurrentJCZQ_OZBMatch()
        //{
        //    if (_cacheCurrentJCZQ_OZBMatch == null)
        //        _cacheCurrentJCZQ_OZBMatch = new List<JCZQ_OZBMatch>();
        //    _cacheCurrentJCZQ_OZBMatch.Clear();
        //    _cacheCurrentJCZQ_OZBMatch = new Sports_Manager().QueryJCZQ_OZBCurrentMatchList();
        //}

        ///// <summary>
        ///// 竞彩篮球当前可投注的比赛缓存
        ///// </summary>
        //private static List<JCLQ_Match> _cacheCurrentJCLQMatch = new List<JCLQ_Match>();
        ///// <summary>
        ///// 查询竞彩篮球当前可投注的比赛
        ///// </summary>
        //public static List<JCLQ_Match> QueryCurrentJCLQMatchList()
        //{
        //    if (_cacheCurrentJCLQMatch == null)
        //        _cacheCurrentJCLQMatch = new List<JCLQ_Match>();
        //    if (_cacheCurrentJCLQMatch.Count <= 0)
        //        _cacheCurrentJCLQMatch = new Sports_Manager().QueryJCLQCurrentMatchList();
        //    return _cacheCurrentJCLQMatch;
        //}
        ///// <summary>
        ///// 重新加载竞彩篮球当前比赛缓存
        ///// </summary>
        //public static void ReloadCurrentJCLQMatch()
        //{
        //    if (_cacheCurrentJCLQMatch == null)
        //        _cacheCurrentJCLQMatch = new List<JCLQ_Match>();
        //    _cacheCurrentJCLQMatch.Clear();
        //    _cacheCurrentJCLQMatch = new Sports_Manager().QueryJCLQCurrentMatchList();
        //}

        ///// <summary>
        ///// 北京单场当前可投注的比赛缓存
        ///// </summary>
        //private static List<BJDC_Match> _cacheCurrentBJDCMatch = new List<BJDC_Match>();
        ///// <summary>
        ///// 查询北京单场当前可投注的比赛
        ///// </summary>
        //public static List<BJDC_Match> QueryCurrentBJDCMatchList()
        //{
        //    if (_cacheCurrentBJDCMatch == null)
        //        _cacheCurrentBJDCMatch = new List<BJDC_Match>();
        //    if (_cacheCurrentBJDCMatch.Count <= 0)
        //        _cacheCurrentBJDCMatch = new Sports_Manager().QueryBJDCCurrentMatchList();
        //    return _cacheCurrentBJDCMatch;
        //}
        ///// <summary>
        ///// 重新加载北京单场比赛缓存
        ///// </summary>
        //public static void ReloadCurrentBJDCMatch()
        //{
        //    if (_cacheCurrentBJDCMatch == null)
        //        _cacheCurrentBJDCMatch = new List<BJDC_Match>();
        //    _cacheCurrentBJDCMatch.Clear();
        //    _cacheCurrentBJDCMatch = new Sports_Manager().QueryBJDCCurrentMatchList();
        //}

        ///// <summary>
        ///// 数字彩当前奖期缓存
        ///// </summary>
        //private static List<GameIssuse> _cacheCurrentNumberIssuse = new List<GameIssuse>();
        ///// <summary>
        ///// 查询数字彩当前奖期
        ///// </summary>
        //public static GameIssuse QueryCurentIssuse(string gameCode)
        //{
        //    var manager = new LotteryGameManager();
        //    if (_cacheCurrentNumberIssuse == null)
        //        _cacheCurrentNumberIssuse = new List<GameIssuse>();
        //    var current = _cacheCurrentCTZQIssuse.FirstOrDefault(p => p.GameCode == gameCode);
        //    if (current == null || current.LocalStopTime < DateTime.Now)
        //    {
        //        _cacheCurrentNumberIssuse.RemoveAll((i) =>
        //        {
        //            return i.GameCode == gameCode;
        //        });
        //        current = manager.QueryCurrentIssuse(gameCode);
        //        _cacheCurrentNumberIssuse.Add(current);
        //    }
        //    return current;
        //}

        ///// <summary>
        ///// 数字彩当前奖期缓存
        ///// </summary>
        //private static List<GameIssuse> _currentNumberIssuse = new List<GameIssuse>();
        ///// <summary>
        ///// 查询数字彩当前奖期
        ///// </summary>
        //public static GameIssuse QueryCurentIssuse(string gameCode, string gameType)
        //{
        //    var manager = new LotteryGameManager();
        //    if (_currentNumberIssuse == null)
        //        _currentNumberIssuse = new List<GameIssuse>();
        //    var current = _currentNumberIssuse.FirstOrDefault(p => p.GameCode == gameCode && (gameType == string.Empty || p.GameType == gameType));
        //    if (current == null || current.LocalStopTime < DateTime.Now)
        //    {
        //        _currentNumberIssuse.RemoveAll((i) =>
        //        {
        //            return i.GameCode == gameCode;
        //        });
        //        current = manager.QueryCurrentIssuse(gameCode, gameType);
        //        if (current != null)
        //            _currentNumberIssuse.Add(current);
        //    }
        //    return current;
        //}

        ///// <summary>
        ///// 传统足球当前奖期缓存
        ///// </summary>
        //private static List<GameIssuse> _cacheCurrentCTZQIssuse = new List<GameIssuse>();
        ///// <summary>
        ///// 查询传统足球当前奖期
        ///// </summary>
        //public static GameIssuse QueryCurrentCTZQIssuse(string gameType)
        //{
        //    var manager = new LotteryGameManager();
        //    if (_cacheCurrentCTZQIssuse == null)
        //        _cacheCurrentCTZQIssuse = new List<GameIssuse>();
        //    if (_cacheCurrentCTZQIssuse.Count == 0)
        //    {
        //        _cacheCurrentCTZQIssuse.Add(manager.QueryCTZQCurrentIssuse("T14C"));
        //        _cacheCurrentCTZQIssuse.Add(manager.QueryCTZQCurrentIssuse("TR9"));
        //        _cacheCurrentCTZQIssuse.Add(manager.QueryCTZQCurrentIssuse("T6BQC"));
        //        _cacheCurrentCTZQIssuse.Add(manager.QueryCTZQCurrentIssuse("T4CJQ"));

        //        return _cacheCurrentCTZQIssuse.FirstOrDefault(p => p.GameType == gameType);
        //    }
        //    var current = _cacheCurrentCTZQIssuse.FirstOrDefault(p => p.GameType == gameType);
        //    if (current == null || current.LocalStopTime < DateTime.Now)
        //    {
        //        _cacheCurrentCTZQIssuse.RemoveAll((i) =>
        //        {
        //            return i.GameType == gameType;
        //        });
        //        current = manager.QueryCTZQCurrentIssuse(gameType);
        //        _cacheCurrentCTZQIssuse.Add(current);
        //    }
        //    return current;
        //}

        #endregion



        /// <summary>
        /// 检查订单基本信息
        /// </summary>
        public static void CheckSports_BetingInfo(Sports_BetingInfo info)
        {
            var gameCodeArray = new string[] { "JCZQ", "JCLQ", "BJDC" };
            if (!gameCodeArray.Contains(info.GameCode.ToUpper()))
                throw new Exception("彩种编码不正确");
            if (info.AnteCodeList.Count <= 0)
                throw new Exception("投注号码不能为空");
            if (info.Amount <= 0)
                throw new Exception("投注倍数不正确");
            if (info.TotalMoney <= 0M)
                throw new Exception("投注金额不正确");

            if (info.GameType.ToUpper() != "HH")
            {
                foreach (var code in info.AnteCodeList)
                {
                    if (info.GameType.ToUpper() != info.GameType.ToUpper())
                        throw new Exception("投注玩法不正确");
                }
            }
        }

        /// <summary>
        /// 检查投注比赛是否可投注，并返回最早结束的比赛时间
        /// </summary>
        //public static DateTime CheckGeneralBettingMatch(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        //{
        //    var sportsManager = new Sports_Manager();
        //    if (gameCode == "BJDC")
        //    {
        //        var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
        //        var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
        //        if (gameType.ToUpper() == "SF")
        //        {
        //            var SFGGMatchList = sportsManager.QuerySFGGSaleMatchCount(matchIdArray);

        //            if (SFGGMatchList.Count != matchIdArray.Length)
        //                throw new LogicException("所选比赛中有停止销售的比赛。");
        //            CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
        //            return SFGGMatchList.Min(m => m.BetStopTime.Value);
        //        }
        //        else
        //        {

        //            if (matchList.Count != matchIdArray.Length)
        //                throw new LogicException("所选比赛中有停止销售的比赛。");
        //            CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
        //            return matchList.Min(m => m.LocalStopTime);
        //        }
        //    }
        //    if (gameCode == "JCZQ")
        //    {
        //        var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
        //        var matchList = QueryCurrentJCZQMatchList().Where(p => matchIdArray.Contains(p.MatchId) && p.FSStopBettingTime > DateTime.Now).ToList();
        //        if (matchList.Count != matchIdArray.Length)
        //            throw new LogicException("所选比赛中有停止销售的比赛。");
        //        //var matchResultList = sportsManager.QueryJCZQMatchResult(matchIdArray);
        //        //if (matchResultList.Count > 0)
        //        //    throw new ArgumentException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

        //        CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);

        //        //if (playType == "1_1")
        //        if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
        //            return matchList.Min(m => m.DSStopBettingTime);
        //        return matchList.Min(m => m.FSStopBettingTime);
        //    }
        //    if (gameCode == "JCLQ")
        //    {
        //        var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
        //        var matchList = QueryCurrentJCLQMatchList().Where(p => matchIdArray.Contains(p.MatchId) && p.FSStopBettingTime > DateTime.Now).ToList();
        //        if (matchList.Count != matchIdArray.Length)
        //            throw new LogicException("所选比赛中有停止销售的比赛。");
        //        //var matchResultList = sportsManager.QueryJCLQMatchResult(matchIdArray);
        //        //if (matchResultList.Count > 0)
        //        //    throw new ArgumentException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

        //        CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);

        //        //if (playType == "1_1")
        //        if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
        //            return matchList.Min(m => m.DSStopBettingTime);
        //        return matchList.Min(m => m.FSStopBettingTime);
        //    }
        //    throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
        //}

        //验证 不支持的玩法
        private static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<JCZQ_Match> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 0：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<JCLQ_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<BJDC_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckHHPlayType(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection anteCodeList)
        {
            if (anteCodeList != null && anteCodeList.Count > 0)
            {
                if (gameCode.ToLower() == "jczq" || gameCode.ToLower() == "jclq")
                {
                    var bfCount = 0;
                    var bqcCount = 0;
                    var sfcCount = 0;
                    var zjqCount = 0;
                    foreach (var itemCode in anteCodeList)
                    {
                        if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf" || gameType.ToLower() == "zjq"))
                        {
                            if (gameCode.ToLower() == "jczq")
                            {
                                if (itemCode.GameType.ToLower() == "bf")
                                {
                                    bfCount++;
                                }
                                else if (itemCode.GameType.ToLower() == "bqc")
                                {
                                    bqcCount++;
                                }
                                else if (itemCode.GameType.ToLower() == "zjq")
                                {
                                    zjqCount++;
                                }
                            }
                            else if (gameCode.ToLower() == "jclq")
                            {
                                if (itemCode.GameType.ToLower() == "sfc")
                                    sfcCount++;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(playType))
                    {
                        var tempMatchCount = playType.Split('|').Select(s => s.Split('_')[0]).Where(s => int.Parse(s) > 4).Count();
                        if (bfCount > 0 || bqcCount > 0)
                        {
                            if (tempMatchCount > 0)
                                throw new Exception("竞彩足球-包含比分或半全场玩法投注，串关方式最多不能超过四串!");
                        }
                        else if (sfcCount > 0)
                        {
                            if (tempMatchCount > 0)
                                throw new Exception("竞彩篮球-包含胜分差玩法投注，串关方式最多不能超过四串!");
                        }
                        else if (zjqCount > 0)
                        {
                            var tempMatchCount_zjq = playType.Split('|').Select(s => s.Split('_')[0]).Where(s => int.Parse(s) > 6).Count();
                            if (tempMatchCount_zjq > 0)
                                throw new Exception("竞彩足球-包含总进球玩法的投注，串关方式最多不能超过六串!");
                        }

                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 检查sql条件中特殊字符
        /// </summary>
        /// <param name="sqlCondition"></param>
        /// <returns></returns>
        public static bool CheckSQLCondition(string sqlCondition)
        {
            var charList = new List<string>();

            #region 特殊字符

            charList.Add("'");
            charList.Add("\"");
            charList.Add("&");
            charList.Add("<");
            charList.Add(">");
            charList.Add("delete");
            charList.Add("update");
            charList.Add("insert");
            charList.Add("'");
            charList.Add(";");
            charList.Add("(");
            charList.Add(")");
            charList.Add("Exec");
            charList.Add("Execute");
            charList.Add("xp_");
            charList.Add("sp_");
            charList.Add("0x");
            charList.Add("?");
            charList.Add("<");
            charList.Add(">");
            charList.Add("(");
            charList.Add(")");
            charList.Add("@");
            charList.Add("=");
            charList.Add("+");
            charList.Add("*");
            charList.Add("&");
            charList.Add("#");
            charList.Add("%");
            charList.Add("$");
            charList.Add("and");
            charList.Add("net user");
            charList.Add("or");
            charList.Add("net");
            charList.Add("drop");
            charList.Add("script");
            charList.Add(";");
            charList.Add("*/");
            charList.Add("\r\n");

            #endregion

            if (charList.Contains(sqlCondition))
                return true;
            return false;
        }

        /// <summary>
        /// 字符串64位编码
        /// </summary>
        public static string EncodeBase64(string source)
        {
            string enstring = "";
            Encoding encode = Encoding.UTF8;
            try
            {
                byte[] bytes = encode.GetBytes(source);
                enstring = Convert.ToBase64String(bytes);
            }
            catch
            {
                enstring = source;
            }
            return enstring;
        }
        /// <summary>
        /// 字符串解码
        /// </summary>
        public static string DecodeBase64(string result)
        {
            string decode = "";
            Encoding encode = Encoding.UTF8;
            try
            {
                byte[] bytes = Convert.FromBase64String(result);
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
        public static bool CheckAnteCode(string gameType, string anteCode)
        {
            if (string.IsNullOrEmpty(gameType))
                return false;
            switch (gameType.ToLower())
            {
                case "spf":
                case "brqspf":
                    if (!new string[] { "3", "1", "0" }.Contains(anteCode))
                        return false;
                    break;
                case "zjq":
                    if (!new string[] { "0", "1", "2", "3", "4", "5", "6", "7" }.Contains(anteCode))
                        return false;
                    break;
                case "bqc":
                    if (!new string[] { "33", "31", "30", "13", "11", "10", "03", "01", "00" }.Contains(anteCode))
                        return false;
                    break;
                case "bf":
                    if (!new string[] { "00", "01", "02", "03", "10", "11", "12", "13", "20", "21", "22", "23", "30", "31", "32", "33", "40", "41", "42", "04", "14", "24", "50", "51", "52", "05", "15", "25", "X0", "XX", "0X" }.Contains(anteCode))
                        return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 发送生成静态文件事件通知
        /// </summary>
        public static void SendBuildStaticFileNotice(WebBuildStaticFileEventCategory category, string data)
        {
            //try
            //{
            //    var code = Encipherment.MD5(string.Format("Home_BuildStaticPageEvent_{0}", (int)category), Encoding.UTF8);
            //    var urlArray = ConfigurationManager.AppSettings["BuildStaticFileSendUrl"].Split('|');
            //    foreach (var item in urlArray)
            //    {
            //        var fullUrl = string.Format("{0}/StaticHtml/BuildStaticPageEvent?pageType={1}&code={2}&data={3}", item, (int)category, code, data);
            //        var result = PostManager.Get(fullUrl, Encoding.UTF8);
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }
        /// <summary>
        /// 发送生成JsonData文件通知
        /// </summary>
        public static string BuildJsonDataNotice(string pageType, string data = "")
        {
            try
            {
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var strUrl = ConfigurationManager.AppSettings["BuildStaticFileSendUrl"] ?? "http://www.iqucai.com";
                var arrUrl = strUrl.Split('|');
                foreach (var item in arrUrl)
                {
                    try
                    {
                        var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", item, "StaticHtml/BuildSpecificPage", pageType, code, data);
                        var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                        return result;
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }


        private static Dictionary<string, string> _stopTime = new Dictionary<string, string>();
        /// <summary>
        /// 查询指定彩是否可以出票
        /// </summary>
        public static bool CanRequestBet(string gameCode)
        {
            try
            {
                var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
                if (!_stopTime.ContainsKey(key))
                {
                    _stopTime.Add(key, ConfigurationManager.AppSettings[key]);
                }
                var stopTime = _stopTime[key];
                if (string.IsNullOrEmpty(stopTime))
                    return true;

                var szArray = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
                if (szArray.Contains(gameCode))
                {
                    var szTimeArray = stopTime.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    var szStartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[0]);
                    var szEndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[1]);

                    if (DateTime.Now > szStartTime && DateTime.Now < szEndTime)
                    {
                        if (gameCode == "FC3D" || gameCode == "PL3")
                        {
                            return false;
                        }
                        var szDayIndex = (int)DateTime.Now.DayOfWeek;
                        switch (szDayIndex)
                        {
                            case 0:
                                return gameCode != "SSQ";
                            case 1:
                                return gameCode != "DLT";
                            case 2:
                                return gameCode != "SSQ";
                            case 3:
                                return gameCode != "DLT";
                            case 4:
                                return gameCode != "SSQ";
                            case 5:
                                break;
                            case 6:
                                return gameCode != "DLT";
                            default:
                                break;
                        }
                    }
                    return true;
                }

                var array = stopTime.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 7)
                    return true;
                var dayIndex = (int)DateTime.Now.DayOfWeek;
                var timeArray = array[dayIndex].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                var gameCodeArrary = new string[] { "JCZQ", "JCLQ", "OZB", "SJB" };
                var ozbTime = new string[] { "160611", "160612", "160613", "160614"
                                        , "160615", "160616", "160617", "160618", "160619", "160620" , "160621", "160622"
                                        , "160623", "160625", "160626", "160627", "160628", "160701", "160702", "160703",
                                        "160704", "160707", "160708", "160711"};
                var sjbTime = new string[] { "180614", "180615", "180616", "180617", "180618", "180619", "180620", "180621",
                                            "180621","180622","180623","180624","180625","180626","180627","180628","180629",
                                            "180630","180701","180702","180703","180704","180705","180706","180707","180708",
                                            "180709","180710","180711","180712","180713","180714","180715"};
                var date = DateTime.Now.ToString("yyyyMMdd").Substring(2);
                var isOzb = false;
                var isSJB = false;
                if (sjbTime.Contains(date))
                    isSJB = true;
                if (ozbTime.Contains(date))
                    isOzb = true;
                if (gameCodeArrary.Contains(gameCode) && isOzb)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                if (gameCodeArrary.Contains(gameCode) && isSJB)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                var startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[0]);
                var endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[1]);

                if (startTime > endTime)
                    endTime = endTime.AddDays(1);

                if (DateTime.Now > startTime && DateTime.Now < endTime)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// 更新数据库票数据
        /// </summary>
        public static void UpdateTicketBonus(List<TicketBatchPrizeInfo> list)
        {
            if (list.Count <= 0) return;

            //查找出未中奖的票并更新
            foreach (var item in list.GroupBy(p => new { Pre = p.PreMoney, After = p.AfterMoney }))
            {
                //查询相同奖金的票数据
                var currentList = list.Where(p => p.AfterMoney == item.Key.After && p.PreMoney == item.Key.Pre).ToArray();
                if (currentList.Length <= 0) continue;

                var pageIndex = 0;
                var pageSize = 100;
                while (true)
                {
                    var pageList = currentList.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                    DoUpdateTicketList(pageList, item.Key.Pre, item.Key.After);

                    if (pageList.Count < pageSize)
                        break;

                    pageIndex++;
                }
            }
        }

        private static void DoUpdateTicketList(List<TicketBatchPrizeInfo> currentList, decimal pre, decimal after)
        {
            if (currentList.Count == 0) return;
            var manager = new Sports_Manager();
            var sqlList = new List<string>();

            if (after <= 0)
            {
                //未中奖
                if (currentList.Count == 1)
                {
                    sqlList.Add(string.Format("update C_Sports_Ticket set BonusStatus=30,PrizeDateTime=getdate() where ticketId='{0}' and BonusStatus=0 ", currentList[0].TicketId));
                }
                else
                {
                    sqlList.Add(string.Format("update C_Sports_Ticket set BonusStatus=30,PrizeDateTime=getdate() where ticketId in ({0})  and BonusStatus=0", string.Join(",", currentList.Select(p => string.Format("'{0}'", p.TicketId)).ToArray())));
                }
            }
            else
            {
                //已中奖
                if (currentList.Count == 1)
                {
                    sqlList.Add(string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2},PrizeDateTime=getdate() where ticketId='{3}' and BonusStatus=0 "
                                , pre, after, after > 0M ? "20" : "30", currentList[0].TicketId));
                }
                else
                {
                    sqlList.Add(string.Format("update C_Sports_Ticket set PreTaxBonusMoney={0},AfterTaxBonusMoney={1},BonusStatus={2},PrizeDateTime=getdate() where ticketId in ({3}) and BonusStatus=0 "
                                , pre, after, after > 0M ? "20" : "30", string.Join(",", currentList.Select(p => string.Format("'{0}'", p.TicketId)).ToArray())));
                }
            }

            if (sqlList.Count > 0)
            {
                //sqlList.Insert(0, "BEGIN TRANSACTION--开始事务");
                //sqlList.Add("COMMIT TRANSACTION--事务提交语句");
                manager.ExecSql(string.Join(Environment.NewLine, sqlList.ToArray()));
            }
        }

        /// <summary>
        /// 执行订单派奖
        /// 奖期未开出开奖号，订单按本金归还，订单中奖状态为未中奖，中奖金额传-1
        /// </summary>
        public static void DoOrderPrize(string orderId, BonusStatus bonusStatus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(orderId);
            if (order == null)
                return;
            //throw new Exception(string.Format("找不到方案{0}的Sports_Order_Running订单信息", orderId));
            var manager = new SchemeManager();
            var orderDetail = manager.QueryOrderDetail(orderId);
            //if (orderDetail == null)
            //    throw new Exception(string.Format("找不到方案{0}的OrderDetail订单信息", orderId));

            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                #region 处理订单

                order.BonusStatus = bonusStatus;
                order.TicketStatus = TicketStatus.Ticketed;
                order.ProgressStatus = ProgressStatus.Complate;
                order.PreTaxBonusMoney = preTaxBonusMoney;
                order.AfterTaxBonusMoney = afterTaxBonusMoney;
                //sportsManager.UpdateSports_Order_Running(order);

                if (orderDetail != null)
                {
                    orderDetail.BonusStatus = bonusStatus;
                    orderDetail.ComplateTime = DateTime.Now;
                    orderDetail.PreTaxBonusMoney = preTaxBonusMoney;
                    orderDetail.AfterTaxBonusMoney = afterTaxBonusMoney;
                    orderDetail.ProgressStatus = ProgressStatus.Complate;
                    orderDetail.TicketStatus = TicketStatus.Ticketed;
                    manager.UpdateOrderDetail(orderDetail);
                }

                var complateOrder = new Sports_Order_Complate
                {
                    SchemeId = order.SchemeId,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    PlayType = order.PlayType,
                    IssuseNumber = order.IssuseNumber,
                    TotalMoney = order.TotalMoney,
                    Amount = order.Amount,
                    TotalMatchCount = order.TotalMatchCount,
                    TicketStatus = order.TicketStatus,
                    BonusStatus = order.BonusStatus,
                    AfterTaxBonusMoney = order.AfterTaxBonusMoney,
                    CanChase = order.CanChase,
                    IsVirtualOrder = order.IsVirtualOrder,
                    CreateTime = order.CreateTime,
                    PreTaxBonusMoney = order.PreTaxBonusMoney,
                    ProgressStatus = order.ProgressStatus,
                    SchemeType = order.SchemeType,
                    TicketGateway = order.TicketGateway,
                    TicketProgress = order.TicketProgress,
                    TicketId = order.TicketId,
                    TicketLog = order.TicketLog,
                    UserId = order.UserId,
                    AgentId = order.AgentId,
                    SchemeSource = order.SchemeSource,
                    SchemeBettingCategory = order.SchemeBettingCategory,
                    StopTime = order.StopTime,
                    ComplateDate = DateTime.Now.ToString("yyyyMMdd"),
                    ComplateDateTime = DateTime.Now,
                    BetCount = order.BetCount,
                    IsPrizeMoney = false,
                    IsPayRebate = order.IsPayRebate,
                    TotalPayRebateMoney = order.TotalPayRebateMoney,
                    RealPayRebateMoney = order.RealPayRebateMoney,
                    MaxBonusMoney = order.MaxBonusMoney,
                    MinBonusMoney = order.MinBonusMoney,
                    RightCount = order.RightCount,
                    Error1Count = order.Error1Count,
                    Error2Count = order.Error2Count,
                    AddMoney = 0M,
                    DistributionWay = AddMoneyDistributionWay.Average,
                    AddMoneyDescription = string.Empty,
                    Security = order.Security,
                    BetTime = order.BetTime,
                    SuccessMoney = order.SuccessMoney,
                    ExtensionOne = order.ExtensionOne,
                    BonusCount = order.BonusCount,
                    HitMatchCount = order.HitMatchCount,
                    Attach = order.Attach,
                    IsAppend = order.IsAppend,
                    TicketTime = order.TicketTime,
                    RedBagMoney = order.RedBagMoney,
                    QueryTicketStopTime = order.QueryTicketStopTime,
                    BonusCountDescription = "",
                    BonusCountDisplayName = "",
                    IsSplitTickets = order.IsSplitTickets,
                };
                sportsManager.AddSports_Order_Complate(complateOrder);
                sportsManager.DeleteSports_Order_Running(order);

                #endregion

                #region 处理追号订单

                while (true)
                {
                    if (order.SchemeType != SchemeType.ChaseBetting)
                        break;
                    if (bonusStatus == BonusStatus.Win && (orderDetail != null && orderDetail.StopAfterBonus))
                    {
                        MoveChaseBrotherOrder(order.SchemeId);
                        break;
                    }
                    var setNextOrderSql = string.Format(@"update C_Lottery_Scheme set IsComplate='true' where SchemeId='{0}' 
                                        update C_Sports_Order_Running
                                        set CanChase='true'
                                        where schemeid=
                                        (select top 1 schemeid 
                                        from C_Lottery_Scheme
                                        where Keyline=
	                                        (select KeyLine 
	                                        from C_Lottery_Scheme
	                                        where schemeid='{0}')
                                        and  IsComplate='false'
                                        order by OrderIndex ASC)", order.SchemeId);
                    sportsManager.ExecSql(setNextOrderSql);
                    break;
                }

                #endregion

                #region 处理合买

                if (order.SchemeType == SchemeType.TogetherBetting)
                {
                    var main = sportsManager.QuerySports_Together(order.SchemeId);
                    if (main.ProgressStatus != TogetherSchemeProgress.AutoStop && main.ProgressStatus != TogetherSchemeProgress.Cancel)
                        main.ProgressStatus = TogetherSchemeProgress.Completed;
                    sportsManager.UpdateSports_Together(main);

                    //处理订制跟单
                    var sql = bonusStatus == BonusStatus.Win
                        ? string.Format("UPDATE C_Together_FollowerRule SET  NotBonusSchemeCount=0 WHERE CreaterUserId='{0}' AND GameCode='{1}' AND GameType='{2}'", order.UserId, order.GameCode, order.GameType)
                        : string.Format("UPDATE C_Together_FollowerRule SET  NotBonusSchemeCount=NotBonusSchemeCount+1 WHERE CreaterUserId='{0}' AND GameCode='{1}' AND GameType='{2}'", order.UserId, order.GameCode, order.GameType);
                    sportsManager.ExecSql(sql);

                    if (bonusStatus == BonusStatus.Win && afterTaxBonusMoney > 0)
                    {
                        //提成
                        var deductMoney = 0M;
                        if (afterTaxBonusMoney > main.TotalMoney)
                            deductMoney = (afterTaxBonusMoney - main.TotalMoney) * main.BonusDeduct / 100;
                        var totalMoney = afterTaxBonusMoney - deductMoney;
                        //var singleMoney = Math.Truncate((totalMoney / main.TotalCount) * 100) / 100;
                        var singleMoney = totalMoney / main.TotalCount;
                        foreach (var join in sportsManager.QuerySports_TogetherSucessJoin(order.SchemeId))
                        {
                            join.PreTaxBonusMoney = join.RealBuyCount * singleMoney;
                            join.AfterTaxBonusMoney = join.RealBuyCount * singleMoney;
                            sportsManager.UpdateSports_TogetherJoin(join);
                            //订制跟单奖金更新
                            if (join.JoinType == TogetherJoinType.FollowerJoin)
                            {
                                //修改定制跟单记录
                                var f = sportsManager.QueryFollowerRecordBySchemeId(main.SchemeId, join.JoinUserId);
                                if (f == null) continue;
                                f.BonusMoney = join.RealBuyCount * singleMoney;
                                sportsManager.UpdateTogetherFollowerRecord(f);

                                //修改定制跟单规则
                                var followRule = sportsManager.QueryTogetherFollowerRule(main.CreateUserId, join.JoinUserId, order.GameCode, order.GameType);
                                if (followRule == null) continue;
                                followRule.TotalBonusOrderCount++;
                                followRule.TotalBonusMoney += join.AfterTaxBonusMoney;
                                sportsManager.UpdateTogetherFollowerRule(followRule);
                            }
                        }
                    }
                }

                #endregion

                #region 数字彩无开奖结果处理

                if (afterTaxBonusMoney < 0M && !order.IsVirtualOrder)
                {
                    //无开奖结果，退钱给用户
                    // 返还资金
                    if (order.SchemeType == SchemeType.GeneralBetting)
                    {
                        if (order.TotalMoney > 0)
                            BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_PayBack_BetMoney, order.UserId, order.SchemeId, order.TotalMoney
                                , string.Format("{0} 无开奖结果，返还资金￥{1:N2}。 ", BusinessHelper.FormatGameCode(order.GameCode), order.TotalMoney));
                    }
                    if (order.SchemeType == SchemeType.ChaseBetting)
                    {
                        var chaseOrder = sportsManager.QueryLotteryScheme(order.SchemeId);
                        if (chaseOrder != null)
                        {
                            if (order.TotalMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_PayBack_BetMoney, order.UserId, chaseOrder.KeyLine, order.TotalMoney
                                , string.Format("订单{0} 无开奖结果，返还资金￥{1:N2}。 ", order.SchemeId, order.TotalMoney));
                        }
                    }
                    if (order.SchemeType == SchemeType.TogetherBetting)
                    {
                        //失败
                        foreach (var item in sportsManager.QuerySports_TogetherSucessJoin(order.SchemeId))
                        {
                            item.JoinSucess = false;
                            item.JoinLog += "无开奖结果";
                            sportsManager.UpdateSports_TogetherJoin(item);

                            if (item.JoinType == TogetherJoinType.SystemGuarantees)
                                continue;

                            var t = string.Empty;
                            var realBuyCount = item.RealBuyCount;
                            switch (item.JoinType)
                            {
                                case TogetherJoinType.Subscription:
                                    t = "认购";
                                    break;
                                case TogetherJoinType.FollowerJoin:
                                    t = "订制跟单";
                                    break;
                                case TogetherJoinType.Join:
                                    t = "参与";
                                    break;
                                case TogetherJoinType.Guarantees:
                                    realBuyCount = item.BuyCount;//为解决用户发起合买后有自动跟单情况，然后投注时直接失败，这时会出现退还保底金额少
                                    t = "保底";
                                    break;
                            }
                            //var joinMoney = item.Price * item.RealBuyCount;
                            var joinMoney = item.Price * realBuyCount;
                            //退钱
                            if (joinMoney > 0)
                                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_PayBack_BetMoney, item.JoinUserId,
                                  string.Format("{0}_{1}", order.SchemeId, item.Id), joinMoney, string.Format("无开奖结果，返还{0}资金{1:N2}元", t, joinMoney));
                        }
                    }
                }

                #endregion

                biz.CommitTran();
            }

            #region 发送站内消息：手机短信或站内信

            if (afterTaxBonusMoney > 0M)
            {
                var _userManager = new UserBalanceManager();
                var _user = _userManager.QueryUserRegister(order.UserId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", _user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[SchemeId]", order.SchemeId));
                pList.Add(string.Format("{0}={1}", "[BonusMoney]", afterTaxBonusMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(_user.UserId, "", "ON_User_Scheme_Bonus", pList.ToArray());
            }

            #endregion

            try
            {
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IOrderPrize_AfterTranCommit>(new object[] { order.UserId, order.SchemeId, order.GameCode, order.GameType, order.IssuseNumber, order.TotalMoney, bonusStatus == BonusStatus.Win, preTaxBonusMoney, afterTaxBonusMoney, order.IsVirtualOrder, DateTime.Now });
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("ERROR_OrderPrize", "_DoOrderPrize", Common.Log.LogType.Error, "订单派奖执行插件失败", ex.ToString());
            }
        }

        /// <summary>
        /// 移动订单数据
        /// </summary>
        public static void MoveChaseBrotherOrder(string schemeId)
        {
            Sports_Manager sportsManager = new Sports_Manager();
            SchemeManager manager = new SchemeManager();
            var chaseOrder = sportsManager.QueryLotteryScheme(schemeId);
            var brotherList = sportsManager.QueryBrotherSports_Order_Running(schemeId);
            if (brotherList.Count == 0) return;
            var userId = string.Empty;
            var totalMoney = 0M;
            foreach (var item in brotherList)
            {
                if (item.TicketStatus == TicketStatus.Waitting)
                {
                    totalMoney += item.TotalMoney;
                }
                item.CanChase = false;
                item.IsVirtualOrder = true;
                item.BonusStatus = BonusStatus.Error;
                item.ProgressStatus = ProgressStatus.AutoStop;
                item.TicketStatus = TicketStatus.Skipped;
                item.BetTime = DateTime.Now;
                item.PreTaxBonusMoney = 0M;
                item.AfterTaxBonusMoney = 0M;
                sportsManager.UpdateSports_Order_Running(item);

                if (string.IsNullOrEmpty(userId))
                    userId = item.UserId;

                var chaseOrderDetail = manager.QueryOrderDetail(item.SchemeId);
                if (chaseOrderDetail == null)
                    throw new Exception(string.Format("找不到方案{0}的订单信息", item.SchemeId));
                chaseOrderDetail.ProgressStatus = ProgressStatus.AutoStop;
                chaseOrderDetail.TicketStatus = TicketStatus.Skipped;
                chaseOrderDetail.IsVirtualOrder = true;
                chaseOrderDetail.CurrentBettingMoney = 0M;
                chaseOrderDetail.ComplateTime = DateTime.Now;
                manager.UpdateOrderDetail(chaseOrderDetail);

                var chaseComplateOrder = new Sports_Order_Complate
                {
                    SchemeId = item.SchemeId,
                    GameCode = item.GameCode,
                    GameType = item.GameType,
                    PlayType = item.PlayType,
                    IssuseNumber = item.IssuseNumber,
                    TotalMoney = item.TotalMoney,
                    Amount = item.Amount,
                    TotalMatchCount = item.TotalMatchCount,
                    TicketStatus = item.TicketStatus,
                    BonusStatus = item.BonusStatus,
                    AfterTaxBonusMoney = item.AfterTaxBonusMoney,
                    CanChase = item.CanChase,
                    IsVirtualOrder = item.IsVirtualOrder,
                    CreateTime = item.CreateTime,
                    PreTaxBonusMoney = item.PreTaxBonusMoney,
                    ProgressStatus = item.ProgressStatus,
                    SchemeType = item.SchemeType,
                    TicketId = item.TicketId,
                    TicketLog = item.TicketLog,
                    UserId = item.UserId,
                    AgentId = item.AgentId,
                    SchemeSource = item.SchemeSource,
                    SchemeBettingCategory = item.SchemeBettingCategory,
                    StopTime = item.StopTime,
                    ComplateDate = DateTime.Now.ToString("yyyyMMdd"),
                    ComplateDateTime = DateTime.Now,
                    BetCount = item.BetCount,
                    IsPrizeMoney = false,
                    HitMatchCount = 0,
                    RightCount = 0,
                    Error1Count = 0,
                    Error2Count = 0,
                    BonusCount = 0,
                    BonusCountDescription = string.Empty,
                    BonusCountDisplayName = string.Empty,
                    Security = item.Security,
                    AddMoney = 0M,
                    DistributionWay = AddMoneyDistributionWay.Average,
                    AddMoneyDescription = string.Empty,
                    BetTime = item.BetTime,
                    SuccessMoney = item.SuccessMoney,
                    TicketGateway = item.TicketGateway,
                    TicketProgress = item.TicketProgress,
                    ExtensionOne = item.ExtensionOne,
                    Attach = item.Attach,
                    IsAppend = item.IsAppend,
                    RedBagMoney = item.RedBagMoney,
                    IsPayRebate = item.IsPayRebate,
                    MaxBonusMoney = item.MaxBonusMoney,
                    MinBonusMoney = item.MinBonusMoney,
                    QueryTicketStopTime = item.QueryTicketStopTime,
                    RealPayRebateMoney = item.RealPayRebateMoney,
                    TotalPayRebateMoney = item.TotalPayRebateMoney,
                    TicketTime = item.TicketTime,
                    IsSplitTickets = item.IsSplitTickets,
                };
                sportsManager.AddSports_Order_Complate(chaseComplateOrder);
                sportsManager.DeleteSports_Order_Running(item);
            }

            if (chaseOrder != null && totalMoney > 0M)
            {
                //清理冻结
                BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, userId, chaseOrder.KeyLine, "停止追号，清除冻结资金", totalMoney);

                //返还投注资金
                BusinessHelper.Payback_To_Balance(BusinessHelper.FundCategory_ChaseBack, userId, chaseOrder.KeyLine, totalMoney, string.Format("停止追号，返还投注资金{0:N2}元", totalMoney));
            }
        }

        /// <summary>
        /// 刷新Redis中用户余额
        /// </summary>
        public static void RefreshRedisUserBalance(string userId)
        {
            try
            {
                var db = RedisHelper.DB_UserBalance;
                string key = string.Format("UserBalance_{0}", userId);
                var fund = new FundBusiness();
                var userBalance = fund.QueryUserBalance(userId);
                var json = JsonSerializer.Serialize(userBalance);
                db.StringSetAsync(key, json, TimeSpan.FromSeconds(60 * 2));
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("BusinessHelper", "RefreshRedisUserBalance", ex);
            }
        }
    }


    /// <summary>
    /// Redis等待出票订单（普通投注方式）
    /// </summary>
    public class RedisWaitTicketOrder
    {
        public RedisWaitTicketOrder()
        {
            AnteCodeList = new List<Sports_AnteCode>();
        }

        public string KeyLine { get; set; }
        public SchemeType SchemeType { get; set; }
        public bool StopAfterBonus { get; set; }
        public Sports_Order_Running RunningOrder { get; set; }
        public List<Sports_AnteCode> AnteCodeList { get; set; }
    }

    /// <summary>
    /// Redis等待出票订单列表（普通投注方式，追号方式）
    /// </summary>
    public class RedisWaitTicketOrderList
    {
        public RedisWaitTicketOrderList()
        {
            OrderList = new List<RedisWaitTicketOrder>();
        }

        public string KeyLine { get; set; }
        public bool StopAfterBonus { get; set; }

        public List<RedisWaitTicketOrder> OrderList { get; set; }
    }

    /// <summary>
    /// Redis等待出票订单（单式上传投注方式）
    /// </summary>
    public class RedisWaitTicketOrderSingle
    {
        public Sports_Order_Running RunningOrder { get; set; }
        public SingleScheme_AnteCode AnteCode { get; set; }

    }

    /// <summary>
    /// Redis比赛相关业务
    /// </summary>
    public static class RedisMatchBusiness
    {
        private static ILogWriter writer = Common.Log.LogWriterGetter.GetLogWriter();

        #region 检查订单是否可投注

        /// <summary>
        /// 检查投注比赛是否可投注，并返回最早结束的比赛时间
        /// </summary>
        public static DateTime CheckGeneralBettingMatch(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            var sportsManager = new Sports_Manager();
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = QueryBJDCMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                return matchList.Min(m => m.LocalStopTime);
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCZQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "JCLQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = QueryJCLQMatch(matchIdArray);
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);
                if (bettingCategory.HasValue && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
        }

        //验证 不支持的玩法
        private static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCZQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 0：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCLQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        private static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<Cache_BJDC_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        #endregion


        #region Redis缓存北京单场当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentBJDCMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryBJDC_Current_CacheMatchList();
                var json = JsonSerializer.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_BJDC_MatchInfo> QueryBJDCMatch(string[] matchIdArray)
        {
            //try
            //{
            //    var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
            //    var db = RedisHelper.DB_Match;
            //    var json = db.StringGetAsync(fullKey).Result;
            //    var matchList = JsonSerializer.Deserialize<List<Cache_BJDC_MatchInfo>>(json);
            //    return matchList.Where(p => matchIdArray.Contains(p.Id) && p.LocalStopTime <= DateTime.Now).ToList();
            //}
            //catch (Exception)
            //{
            //    return new List<Cache_BJDC_MatchInfo>();
            //}

            try
            {
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_BJDC_MatchInfo>>(json);
                var t = matchList.Where(p => p.LocalStopTime > DateTime.Now).ToList();
                return t.Where(p => matchIdArray.Contains(p.Id)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_BJDC_MatchInfo>();
            }

        }

        #endregion

        #region Redis缓存竞彩足球当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentJCZQMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryJCZQ_Current_CacheMatchList();
                var json = JsonSerializer.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_JCZQ_MatchInfo> QueryJCZQMatch(string[] matchIdArray)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_JCZQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCZQ_MatchInfo>();
            }
        }

        #endregion

        #region Redis缓存竞彩篮球当前比赛

        /// <summary>
        /// 重新加载比赛数据
        /// </summary>
        public static void ReloadCurrentJCLQMatch()
        {
            try
            {
                var matchList = new Sports_Manager().QueryJCLQ_Current_CacheMatchList();
                var json = JsonSerializer.Serialize(matchList);
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                db.StringSetAsync(fullKey, json);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用matchId查询缓存中的比赛
        /// </summary>
        public static List<Cache_JCLQ_MatchInfo> QueryJCLQMatch(string[] matchIdArray)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Match_List);
                var db = RedisHelper.DB_Match;
                var json = db.StringGetAsync(fullKey).Result;
                var matchList = JsonSerializer.Deserialize<List<Cache_JCLQ_MatchInfo>>(json);
                return matchList.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
            }
            catch (Exception)
            {
                return new List<Cache_JCLQ_MatchInfo>();
            }
        }

        #endregion

        #region Redis缓存竞彩足球比赛结果数据

        /// <summary>
        /// 加载竞彩足球比赛结果
        /// </summary>
        public static void LoadJCZQMatchResult()
        {
            //查询sql中近三天的比赛结果
            var manager = new JCZQMatchManager();
            var resultList = manager.QueryJCZQMatchResultByDay(-30);
            //写入redis库
            var json = JsonSerializer.Serialize<List<JCZQ_MatchResult>>(resultList);
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩足球比赛结果
        /// </summary>
        public static List<JCZQ_MatchResult> QueryJCZQMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<JCZQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QueryJCZQMatchResult", ex);
                return new List<JCZQ_MatchResult>();
            }
        }

        #endregion

        #region Redis缓存竞彩篮球比赛结果数据

        /// <summary>
        /// 加载竞彩篮球比赛结果
        /// </summary>
        public static void LoadJCLQMatchResult()
        {
            //查询sql中近三天的比赛结果
            var manager = new JCLQMatchManager();
            var resultList = manager.QueryJCLQMatchResultByDay(-5);
            //写入redis库
            var json = JsonSerializer.Serialize<List<JCLQ_MatchResult>>(resultList);
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询竞彩篮球比赛结果
        /// </summary>
        public static List<JCLQ_MatchResult> QueryJCLQMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<JCLQ_MatchResult>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QueryJCLQMatchResult", ex);
                return new List<JCLQ_MatchResult>();
            }
        }

        #endregion #region Redis缓存竞彩篮球比赛结果数据

        #region Redis缓存北京单场比赛结果数据

        /// <summary>
        /// 加载北京单场比赛结果
        /// </summary>
        public static void LoadBJDCMatchResult()
        {
            //查询sql中近两期的比赛结果
            var manager = new BJDCMatchManager();
            var resultList = manager.QueryBJDCMatchResultByIssuse(2);
            //写入redis库
            var json = JsonSerializer.Serialize<List<BJDC_MatchResult_Prize>>(resultList);
            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询北京单场比赛结果
        /// </summary>
        public static List<BJDC_MatchResult_Prize> QueryBJDCMatchResult()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "BJDC", RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<BJDC_MatchResult_Prize>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QueryBJDCMatchResult", ex);
                return new List<BJDC_MatchResult_Prize>();
            }
        }

        #endregion

        #region Redis缓存传统足球比赛结果及奖池数据

        /// <summary>
        /// 加载传统足球比赛结果
        /// </summary>
        public static void LoadCTZQBonusPool()
        {
            //查询sql中近三期的比赛结果
            var manager = new Ticket_BonusManager();
            var gameTypeArray = new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
            foreach (var gameType in gameTypeArray)
            {
                var poolList = manager.GetBonusPool("CTZQ", gameType, 30);
                //写入redis库
                var json = JsonSerializer.Serialize<List<Ticket_BonusPool>>(poolList);
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                db.StringSetAsync(fullKey, json);
            }
        }

        /// <summary>
        /// 查询传统足球比赛结果
        /// </summary>
        public static List<Ticket_BonusPool> QuseryCTZQBonusPool(string gameType)
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("CTZQ_{0}_{1}", gameType, RedisKeys.Key_MatchResult_List);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QuseryCTZQBonusPool_" + gameType, ex);
                return new List<Ticket_BonusPool>();
            }
        }

        #endregion

        #region Redis缓存数字彩奖池数据

        /// <summary>
        /// 加载数字彩奖池数据
        /// </summary>
        public static void LoadSZCBonusPool()
        {
            //查询sql中近三期的比赛结果
            var manager = new Ticket_BonusManager();
            var gameCodeArray = new string[] { "SSQ", "DLT" };
            foreach (var gameCode in gameCodeArray)
            {
                var poolList = manager.GetBonusPool(gameCode, "", 30);
                //写入redis库
                var json = JsonSerializer.Serialize<List<Ticket_BonusPool>>(poolList);
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                db.StringSetAsync(fullKey, json);
            }
        }

        /// <summary>
        /// 查询数字彩奖池数据
        /// </summary>
        public static List<Ticket_BonusPool> QuserySZCBonusPool(string gameCode)
        {
            try
            {
                var gameCodeArray = new string[] { "SSQ", "DLT" };
                if (!gameCodeArray.Contains(gameCode))
                    return new List<Ticket_BonusPool>();

                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_SZC_BonusPool);
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<Ticket_BonusPool>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QuserySZCBonusPool_" + gameCode, ex);
                return new List<Ticket_BonusPool>();
            }
        }

        #endregion

        #region Redis缓存数字彩中奖规则

        /// <summary>
        /// 加载数字彩中奖规则
        /// </summary>
        public static void LoadSZCBonusRule()
        {
            //查询所有数字彩中奖规则
            var list = new BonusRuleManager().QueryAllBonusRule();
            //写入redis库
            var json = JsonSerializer.Serialize<List<BonusRule>>(list);
            var db = RedisHelper.DB_Match;
            var fullKey = "SZC_Bonus_Rule_List";
            db.StringSetAsync(fullKey, json);
        }

        /// <summary>
        /// 查询数字彩中奖规则
        /// </summary>
        public static List<BonusRule> QuerySZCBonusRule()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = "SZC_Bonus_Rule_List";
                var json = db.StringGetAsync(fullKey).Result;
                var resultList = JsonSerializer.Deserialize<List<BonusRule>>(json);
                return resultList;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QuerySZCBonusRule", ex);
                return new List<BonusRule>();
            }
        }

        #endregion

        #region Redis缓存数字彩开奖号

        /// <summary>
        /// 加载数字彩开奖号码
        /// </summary>
        public static void LoadSZCWinNumber()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "OZB", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadSZCWinNumber(gameCode);
            }
        }

        /// <summary>
        /// 加载数字彩开奖号码
        /// </summary>
        public static void LoadSZCWinNumber(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "OZB", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;

            if (gameCode == "OZB")
            {
                LoadOZBWinNumber();
            }
            else
            {
                var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse(gameCode, 100);
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                //清空Key对应的value值
                db.KeyDeleteAsync(fullKey);

                //写入redis库
                //格式：期号^开奖号
                var array = lastOpenIssuse.Select(p => (RedisValue)string.Format("{0}^{1}", p.IssuseNumber, p.WinNumber)).ToArray();
                db.ListRightPushAsync(fullKey, array);
            }
        }

        /// <summary>
        /// 查询数字彩开奖号码
        /// </summary>
        public static Dictionary<string, string> QuerySZCWinNumber(string gameCode)
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QuerySZCWinNumber_" + gameCode, ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region 加载欧洲杯开奖数据

        public static void LoadOZBWinNumber()
        {
            var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse("OZB", 2);
            var gjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GJ");
            var gyjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GYJ");

            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.KeyDeleteAsync(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
        }

        public static Dictionary<string, string> QueryOZBWinNumber()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "OZB", RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QueryOZBWinNumber", ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region 加载世界杯开奖数据

        public static void LoadSJBWinNumber()
        {
            var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse("SJB", 2);
            var gjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GJ");
            var gyjIssuse = lastOpenIssuse.FirstOrDefault(p => p.GameType == "GYJ");

            var db = RedisHelper.DB_Match;
            var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
            //清空Key对应的value值
            db.KeyDeleteAsync(fullKey);
            //写入redis库
            //格式：玩法^开奖号
            if (gjIssuse != null && !string.IsNullOrEmpty(gjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GJ", gjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
            if (gyjIssuse != null && !string.IsNullOrEmpty(gyjIssuse.WinNumber))
            {
                var v = string.Format("{0}^{1}", "GYJ", gyjIssuse.WinNumber);
                db.ListRightPushAsync(fullKey, v);
            }
        }

        public static Dictionary<string, string> QuerySJBWinNumber()
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", "SJB", RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                writer.Write("RedisMatchBusiness", "QuerySJBWinNumber", ex);
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region Redis缓存数字彩当前奖期信息

        public static void LoadCurrentIssuseByOfficialStopTime()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadCurrentIssuseByOfficialStopTime(gameCode);
            }
        }

        public static void LoadCurrentIssuseByOfficialStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(true);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonSerializer.Serialize(item);
                db.StringSetAsync(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByOfficialStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelper.DB_CoreCacheData;
            var json = db.StringGetAsync(key).Result;
            return JsonSerializer.Deserialize<LotteryIssuse_QueryInfo>(json);
        }

        public static void LoadCurrentIssuseByLocalStopTime()
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            foreach (var gameCode in gameCodeArray)
            {
                LoadCurrentIssuseByLocalStopTime(gameCode);
            }
        }

        public static void LoadCurrentIssuseByLocalStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            var issuseList = new LotteryGameManager().QueryAllGameCurrentIssuse(false);
            foreach (var item in issuseList)
            {
                string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, item.GameCode);
                var json = JsonSerializer.Serialize(item);
                db.StringSetAsync(key, json);
            }
        }

        public static LotteryIssuse_QueryInfo QueryCurrentIssuseByLocalStopTime(string gameCode)
        {
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_CurrentIssuse, gameCode);
            var db = RedisHelper.DB_CoreCacheData;
            var json = db.StringGetAsync(key).Result;
            return JsonSerializer.Deserialize<LotteryIssuse_QueryInfo>(json);
        }

        #endregion

        #region Redis缓存未来奖期列表

        public static void LoadNextIssuseListByOfficialStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            string key = string.Format("{0}_ByOfficialStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(true, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.KeyDeleteAsync(key);
            foreach (var item in issuseList)
            {
                var json = JsonSerializer.Serialize(item);
                db.ListRightPushAsync(key, json);
            }
        }

        public static void LoadNextIssuseListByLocalStopTime(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" };
            if (!gameCodeArray.Contains(gameCode))
                return;
            var db = RedisHelper.DB_CoreCacheData;
            string key = string.Format("{0}_ByLocalStopTime_{1}", RedisKeys.Key_NextIssuse_List, gameCode);
            var issuseList = new LotteryGameManager().QueryNextIssuseList(false, gameCode, 10);
            if (issuseList.Count <= 0)
                return;
            db.KeyDeleteAsync(key);
            foreach (var item in issuseList)
            {
                var json = JsonSerializer.Serialize(item);
                db.ListRightPushAsync(key, json);
            }
        }

        #endregion

    }


    /// <summary>
    /// Redis订单相关业务
    /// </summary>
    public static class RedisOrderBusiness
    {
        private static ILogWriter writer = Common.Log.LogWriterGetter.GetLogWriter();

        #region 订单投注后加入待拆票列表

        /// <summary>
        /// 查询SQL中等待拆票的订单号
        /// </summary>
        public static string QueryUnSplitTicketsOrder()
        {
            return string.Join("|", new Sports_Manager().QueryUnSplitTicketsOrder());
        }

        /// <summary>
        /// 把SQL中的订单加入Redis待拆票库中
        /// </summary>
        public static string AddOrderToWaitSplitList(string schemeId)
        {
            var logList = new List<string>();
            try
            {
                logList.Add(string.Format("开始处理订单{0}", schemeId));
                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception("订单数据为空");
                var codeList = manager.QuerySportsAnteCodeBySchemeId(schemeId);
                if (codeList == null || codeList.Count <= 0)
                    throw new Exception("订单投注号码为空");

                if (order.SchemeType == SchemeType.ChaseBetting)
                {
                    logList.Add("订单是【追号订单】");

                    var detail = new SchemeManager().QueryOrderDetail(schemeId);
                    if (detail == null)
                        throw new Exception("OrderDetail数据为空");
                    var scheme = manager.QueryLotteryScheme(schemeId);
                    if (scheme == null)
                        throw new Exception("LotteryScheme数据为空");

                    //检查keyline在Redis库中存不存在
                    var db = RedisHelper.DB_Chase_Order;
                    var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, order.GameCode);
                    var chaseKeyLineArray = db.ListRangeAsync(fullKey).Result;
                    foreach (var k in chaseKeyLineArray)
                    {
                        if (!k.HasValue)
                            continue;
                        if (k.ToString() == scheme.KeyLine)
                        {
                            logList.Add("追号列表中已存在KeyLine.");
                            //取出所有追号列表
                            var chaseList = db.ListRangeAsync(scheme.KeyLine).Result;
                            //清空key
                            db.KeyDeleteAsync(scheme.KeyLine);
                            //修改canchase为true后，添加key
                            var orderList2 = new RedisWaitTicketOrderList();
                            orderList2.KeyLine = scheme.KeyLine;
                            orderList2.StopAfterBonus = detail.StopAfterBonus;
                            var findCurrentOrder = false;
                            foreach (var item in chaseList)
                            {
                                var orderJson = item.ToString();
                                var chaseOrder = JsonSerializer.Deserialize<RedisWaitTicketOrder>(orderJson);
                                if (chaseOrder.RunningOrder != null)
                                {
                                    chaseOrder.RunningOrder.CanChase = !findCurrentOrder;
                                    if (chaseOrder.RunningOrder.SchemeId == schemeId)
                                        findCurrentOrder = true;
                                    orderList2.OrderList.Add(chaseOrder);
                                }
                            }
                            AddOrderToWaitSplitList(order.GameCode, orderList2);
                            logList.Add("重新添加成功。");
                            return string.Join(Environment.NewLine, logList);
                        }
                        //throw new Exception("追号列表中已存在KeyLine");
                    }

                    var schemeList = manager.QueryLotterySchemeByKeyLine(scheme.KeyLine);
                    if (schemeList == null || schemeList.Count <= 0)
                        throw new Exception("schemeList为空");

                    var orderList = new RedisWaitTicketOrderList();
                    orderList.KeyLine = scheme.KeyLine;
                    orderList.StopAfterBonus = detail.StopAfterBonus;
                    foreach (var item in schemeList)
                    {
                        var running = manager.QuerySports_Order_Running(item.SchemeId);
                        if (running == null) continue;

                        orderList.OrderList.Add(new RedisWaitTicketOrder
                        {
                            KeyLine = scheme.KeyLine,
                            StopAfterBonus = detail.StopAfterBonus,
                            SchemeType = order.SchemeType,
                            AnteCodeList = codeList,
                            RunningOrder = running
                        });
                    }
                    if (orderList.OrderList == null || orderList.OrderList.Count <= 0)
                        throw new Exception("OrderList数据为空");
                    orderList.OrderList[0].RunningOrder.CanChase = true;
                    AddOrderToWaitSplitList(order.GameCode, orderList);
                }
                else if (order.SchemeBettingCategory == SchemeBettingCategory.SingleBetting)
                {
                    logList.Add("订单是【单式上传订单】");
                    var singleCode = manager.QuerySingleScheme_AnteCode(schemeId);
                    if (singleCode == null)
                        throw new Exception("单式号码数据为空");

                    AddOrderToWaitSplitList(new RedisWaitTicketOrderSingle
                    {
                        RunningOrder = order,
                        AnteCode = singleCode,
                    });
                }
                else
                {
                    logList.Add("订单是【普通订单】");
                    AddOrderToWaitSplitList(new RedisWaitTicketOrder
                    {
                        KeyLine = order.SchemeId,
                        AnteCodeList = codeList,
                        RunningOrder = order,
                        SchemeType = order.SchemeType,
                        StopAfterBonus = true,
                    });
                }

                logList.Add("添加完成");
            }
            catch (Exception ex)
            {
                logList.Add(ex.Message);
            }
            return string.Join(Environment.NewLine, logList);
        }

        /// <summary>
        /// 获取 待拆票 可用的Key
        /// </summary>
        private static string GetWaitingOrderUsableKey(string gameCode)
        {
            try
            {
                var count = int.Parse(ConfigurationManager.AppSettings["WaitingOrderListCount"]);
                var db = RedisHelper.DB_NoTicket_Order;
                var key = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", gameCode.ToUpper());
                var currentIndexKey = string.Format("{0}_Current", key);
                var indexValue = db.StringGetAsync(currentIndexKey).Result;
                var index = 0;
                if (indexValue.HasValue)
                {
                    //获取索引
                    index = int.Parse(indexValue.ToString());
                    index = index >= count ? 0 : index + 1;
                }
                db.StringSetAsync(currentIndexKey, index);
                return string.Format("{0}_{1}", key, index);
            }
            catch (Exception)
            {
                return string.Format("{0}_{1}_{2}_0", RedisKeys.Key_Waiting_Order_List, "General", gameCode.ToUpper());
            }
        }

        /// <summary>
        /// 订单投注后加入Redis待拆票列表(普通投注)
        /// </summary>
        public static void AddOrderToWaitSplitList(RedisWaitTicketOrder order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCodeList.Count <= 0)
                return;

            //var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", order.RunningOrder.GameCode.ToUpper());
            var fullKey = GetWaitingOrderUsableKey(order.RunningOrder.GameCode);
            var json = JsonSerializer.Serialize<RedisWaitTicketOrder>(order);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
        }

        /// <summary>
        /// 订单投注后加入Redis待拆票列表(追号订单)
        /// </summary>
        public static void AddOrderToWaitSplitList(string gameCode, RedisWaitTicketOrderList orderList)
        {
            if (orderList == null || orderList.OrderList.Count <= 0 || string.IsNullOrEmpty(orderList.KeyLine))
                return;

            //把追号订单以keyline为key存入
            var db = RedisHelper.DB_Chase_Order;
            foreach (var item in orderList.OrderList)
            {
                var json = JsonSerializer.Serialize<RedisWaitTicketOrder>(item);
                db.ListRightPushAsync(orderList.KeyLine, json);
            }
            //把keyline存入Waiting_Chase_Order_List
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, gameCode);
            db.ListRightPushAsync(fullKey, orderList.KeyLine);
        }

        /// <summary>
        /// 订单投注后加入Redis待拆票列表(单式投注)
        /// </summary>
        public static void AddOrderToWaitSplitList(RedisWaitTicketOrderSingle order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCode == null)
                return;

            var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "Single", order.RunningOrder.GameCode.ToUpper());
            var json = JsonSerializer.Serialize<RedisWaitTicketOrderSingle>(order);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
        }

        /// <summary>
        /// 更新订单状态为已出票
        /// </summary>
        private static void UpdateOrderTicketStatus(string schemeId)
        {
            var sql = string.Format(@"update [C_Sports_Order_Running] set TicketStatus=90,ProgressStatus=10,TicketTime=getdate() where [SchemeId]='{0}'
                                    update C_OrderDetail set CurrentBettingMoney=TotalMoney,TicketStatus=90,ProgressStatus=10,TicketTime=getdate() where [SchemeId]='{0}'", schemeId);
            var sportsManager = new Sports_Manager();
            sportsManager.ExecSql(sql);
        }

        /// <summary>
        /// 订单拆票(追号投注)
        /// </summary>
        public static string SplitChaseOrderTicket(string gameCode)
        {
            if (!BusinessHelper.CanRequestBet(gameCode))
                return string.Format("{0}暂时不能出票", gameCode);

            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, gameCode);
            //var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3" , "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3"};
            var db = RedisHelper.DB_Chase_Order;
            var keyLineList = db.ListRangeAsync(fullKey).Result;
            logList.Add(string.Format("当前追号订单KeyLine:{0}一共有{1}条", fullKey, keyLineList.Length));
            var manager = new SchemeManager();
            foreach (var item in keyLineList)
            {
                if (!item.HasValue)
                    continue;

                var keyLine = item.ToString();
                logList.Add(string.Format("开始处理追号订单{0}", keyLine));
                //取出追号订单列表中的第一条
                var first = db.ListLeftPopAsync(keyLine).Result;//  .ListRange(keyLine.ToString(), 0, 0);
                if (!first.HasValue)
                {
                    //说明追号订单已运行完成，删除
                    db.KeyDeleteAsync(keyLine);
                    db.ListRemoveAsync(fullKey, keyLine);
                    logList.Add(string.Format("追号订单{0}已完成", keyLine));
                    continue;
                }
                var order = JsonSerializer.Deserialize<RedisWaitTicketOrder>(first.ToString());
                //-------------------------------------------------------------------------------------------------------------
                //redis DB_Chase_Order库中有冗余状态为已完成的追号订单  这里增加判断移除 20171202
                //拆票逻辑是先从DB_Chase_Order中 比如这个键中Waiting_Chase_Order_List_CQSSC（存放追号订单号的key）取出所有的追号订单号 再循环处理每一个追号单 
                //ListLeftPopAsync取出追号订单列表中的第一条并删除 每次都取出左边的第一条并删除 直到订单完成
                var orderDetail = manager.QueryOrderDetail(order.RunningOrder.SchemeId);
                if (orderDetail == null || orderDetail.ProgressStatus == ProgressStatus.Complate || orderDetail.ProgressStatus == ProgressStatus.Aborted || orderDetail.ProgressStatus == ProgressStatus.AutoStop || orderDetail.BonusStatus == BonusStatus.Lose || orderDetail.BonusStatus == BonusStatus.Win || orderDetail.BonusStatus == BonusStatus.Awarding)
                {
                    //中奖后停止移除DB_Chase_Order中的追号单 追号订单已运行完成
                    if (order.StopAfterBonus && orderDetail.BonusStatus == BonusStatus.Win)
                    {
                        db.KeyDeleteAsync(keyLine);
                        db.ListRemoveAsync(fullKey, keyLine);
                        logList.Add(string.Format("追号订单{0}中奖后停止", keyLine));
                        continue;
                    }
                    //db.ListRemoveAsync(fullKey, keyLine);//从DB_Chase_Order中 比如这个键中Waiting_Chase_Order_List_CQSSC（存放追号订单号的key）移除当前循环的 追号订单号
                    db.ListRemoveAsync(keyLine, order.ToString());//从DB_Chase_Order中 具体的某个追号订单号 中移除当前已完成订单
                    logList.Add(string.Format("订单{0} orderDetail数据为空或者已完成、已中断、自动停止,从redis {1}中移除：{2}", order.RunningOrder.SchemeId, fullKey, order.ToString()));
                    continue;
                }
                //-------------------------------------------------------------------------------------------------------------
                if (!order.RunningOrder.CanChase)
                {
                    //订单暂时不能拆票
                    db.ListLeftPushAsync(keyLine, first);
                    logList.Add(string.Format("追号订单{0}暂时不能出票", order.RunningOrder.SchemeId));
                    continue;
                }

                try
                {
                    //拆票
                    DoSplitOrderTicket(order);
                    logList.Add(string.Format("追号订单{0}拆票完成", order.RunningOrder.SchemeId));
                }
                catch (Exception ex)
                {
                    db.ListLeftPushAsync(keyLine, first);
                    logList.Add(string.Format("订单{0}拆票异常{1}", order.RunningOrder.SchemeId, ex.ToString()));
                }
            }

            watch.Stop();
            logList.Add(string.Format("本次总用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        /// <summary>
        /// 订单拆票(普通投注)
        /// </summary>
        public static string SplitOrderTicket(string gameCode, int doMaxOrderCount)
        {
            var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", gameCode);
            return SplitOrderTicket(gameCode, fullKey, doMaxOrderCount);
        }

        public static string SplitOrderTicket(string gameCode, string fullKey, int doMaxOrderCount)
        {
            if (!BusinessHelper.CanRequestBet(gameCode))
                return string.Format("{0}暂时不能出票", gameCode);

            var logList = new List<string>();
            var watch = new Stopwatch();
            var db = RedisHelper.DB_NoTicket_Order;
            if (db.KeyExistsAsync(fullKey).Result)
            {
                for (int i = 0; i < doMaxOrderCount; i++)
                {
                    watch.Start();
                    var orderJson = db.ListLeftPopAsync(fullKey).Result;
                    if (!orderJson.HasValue)
                        break;
                    watch.Stop();
                    logList.Add(string.Format("从Redis中取出JSON数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                    watch.Restart();
                    //反序列化json
                    var order = JsonSerializer.Deserialize<RedisWaitTicketOrder>(orderJson);
                    watch.Stop();
                    logList.Add(string.Format("反序列化JSON数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                    watch.Restart();
                    try
                    {
                        DoSplitOrderTicket(order);
                        logList.Add(string.Format("订单{0}拆票完成", order.RunningOrder.SchemeId));
                    }
                    catch (Exception ex)
                    {
                        db.ListRightPushAsync(fullKey, orderJson);
                        logList.Add(string.Format("订单{0}拆票异常{1}", order.RunningOrder.SchemeId, ex.ToString()));
                    }
                    watch.Stop();
                    logList.Add(string.Format("执行订单{0}的票拆分用时{1}毫秒", order.RunningOrder.SchemeId, watch.Elapsed.TotalMilliseconds));
                }
            }
            return string.Join(Environment.NewLine, logList);

        }



        /// <summary>
        /// 多线程执行拆票（普通投注）
        /// </summary>
        public static void DoSplitOrderTicketWithThread(RedisWaitTicketOrder order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCodeList == null || order.AnteCodeList.Count <= 0)
                return;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicket(o as RedisWaitTicketOrder);
                }
                catch (Exception ex)
                {
                    writer.Write("Redis_DoSplitOrderTicket", "DoSplitOrderTicketWithThread", ex);
                }
            }, order);
        }

        /// <summary>
        /// 执行拆票(普通投注)
        /// </summary>
        public static void DoSplitOrderTicket(RedisWaitTicketOrder order)
        {
            //if (!BusinessHelper.CanRequestBet(order.RunningOrder.GameCode))
            //    return;

            try
            {
                var sportsManager = new Sports_Manager();
                var oldCount = sportsManager.QueryTicketCount(order.RunningOrder.SchemeId);
                if (oldCount <= 0)
                {
                    //清理冻结
                    if (order.RunningOrder.SchemeType == SchemeType.ChaseBetting)
                        BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.RunningOrder.UserId, order.RunningOrder.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney), order.RunningOrder.TotalMoney);

                    //普通投注
                    var jcGameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                    if (jcGameCodeArray.Contains(order.RunningOrder.GameCode))
                    {
                        //竞彩
                        #region 拆票

                        var betInfo = new GatewayTicketOrder_Sport
                        {
                            Amount = order.RunningOrder.Amount,
                            Attach = order.RunningOrder.Attach,
                            GameCode = order.RunningOrder.GameCode,
                            GameType = order.RunningOrder.GameType,
                            IssuseNumber = order.RunningOrder.IssuseNumber,
                            IsVirtualOrder = order.RunningOrder.IsVirtualOrder,
                            OrderId = order.RunningOrder.SchemeId,
                            PlayType = order.RunningOrder.PlayType,
                            UserId = order.RunningOrder.UserId,
                            TotalMoney = order.RunningOrder.TotalMoney,
                            Price = 2M,
                            IsRunningTicket = true,
                        };
                        foreach (var code in order.AnteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode_Sport
                            {
                                AnteCode = code.AnteCode,
                                GameType = code.GameType,
                                IsDan = code.IsDan,
                                MatchId = code.MatchId,
                            });
                        }
                        new Sports_Business().RequestTicket_Sport(betInfo);

                        //new Thread(() =>
                        //{
                        try
                        {
                            //生成文件
                            var json = JsonSerializer.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }
                        //}).Start();
                        #endregion
                    }
                    else
                    {
                        //数字彩、传统足球
                        #region 拆票

                        var betInfo = new GatewayTicketOrder
                        {
                            Amount = order.RunningOrder.Amount,
                            GameCode = order.RunningOrder.GameCode,
                            IssuseNumber = order.RunningOrder.IssuseNumber,
                            OrderId = order.RunningOrder.SchemeId,
                            Price = ((order.RunningOrder.IsAppend && order.RunningOrder.GameCode == "DLT") ? 3M : 2M),
                            TotalMoney = order.RunningOrder.TotalMoney,
                            IsVirtualOrder = false,
                            Attach = "",
                            IsAppend = order.RunningOrder.IsAppend,
                            UserId = order.RunningOrder.UserId,
                            IsRunningTicket = true,
                        };
                        foreach (var item in order.AnteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode
                            {
                                AnteNumber = item.AnteCode,
                                GameType = item.GameType,
                            });
                        }

                        //new Sports_Business().RequestTicket(betInfo, order.KeyLine, order.StopAfterBonus, order.SchemeType);

                        new Sports_Business().RequestTicket2(betInfo, order.KeyLine, order.StopAfterBonus, order.SchemeType);
                        //new Thread(() =>
                        //{

                        try
                        {
                            //生成文件
                            var json = JsonSerializer.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }

                        //}).Start();

                        #endregion
                    }

                    //更新订单状态
                    UpdateOrderTicketStatus(order.RunningOrder.SchemeId);

                    //触发出票完成接口
                    BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.RunningOrder.UserId, order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney, order.RunningOrder.TotalMoney });


                }
            }
            catch (Exception exp)
            {
                writer.Write("追号订单自动拆票任务", "DoSplitOrderTicket", LogType.Information, "追号订单自动拆票任务日志", exp.Message);

            }
        }

        /// <summary>
        /// 订单拆票(单式投注)
        /// </summary>
        public static string SplitOrderTicket_Single(string gameCode, int doMaxOrderCount)
        {
            if (!BusinessHelper.CanRequestBet(gameCode))
                return string.Format("{0}暂时不能出票", gameCode);

            var logList = new List<string>();
            var watch = new Stopwatch();
            var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "Single", gameCode);
            var db = RedisHelper.DB_NoTicket_Order;
            if (db.KeyExistsAsync(fullKey).Result)
            {
                for (int i = 0; i < doMaxOrderCount; i++)
                {
                    watch.Start();
                    var orderJson = db.ListLeftPopAsync(fullKey).Result;
                    if (orderJson.IsNullOrEmpty)
                        break;
                    watch.Stop();
                    logList.Add(string.Format("从Redis中取出JSON数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                    watch.Restart();
                    //反序列化json
                    var order = JsonSerializer.Deserialize<RedisWaitTicketOrderSingle>(orderJson);
                    watch.Stop();
                    logList.Add(string.Format("反序列化JSON数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

                    watch.Restart();
                    try
                    {
                        DoSplitOrderTicket_Single(order);
                        logList.Add(string.Format("订单{0}拆票完成", order.RunningOrder.SchemeId));
                    }
                    catch (Exception ex)
                    {
                        db.ListRightPushAsync(fullKey, orderJson);
                        logList.Add(string.Format("订单{0}拆票异常{1}", order.RunningOrder.SchemeId, ex.ToString()));
                    }
                    watch.Stop();
                    logList.Add(string.Format("执行订单{0}的票拆分用时{1}毫秒", order.RunningOrder.SchemeId, watch.Elapsed.TotalMilliseconds));
                }
            }
            return string.Join(Environment.NewLine, logList);
        }

        /// <summary>
        /// 多线程执行拆票（单式投注）
        /// </summary>
        public static void DoSplitOrderTicketWithThread_Single(RedisWaitTicketOrderSingle order)
        {
            if (order == null || order.AnteCode == null)
                return;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicket_Single(o as RedisWaitTicketOrderSingle);
                }
                catch (Exception ex)
                {
                    writer.Write("Redis_DoSplitOrderTicket_Single", "DoSplitOrderTicketWithThread_Single", ex);
                }
            }, order);
        }

        /// <summary>
        /// 执行拆票（单式投注）
        /// </summary>
        public static void DoSplitOrderTicket_Single(RedisWaitTicketOrderSingle order)
        {
            //if (!BusinessHelper.CanRequestBet(order.RunningOrder.GameCode))
            //    return;

            var sportsManager = new Sports_Manager();
            var oldCount = sportsManager.QueryTicketCount(order.RunningOrder.SchemeId);
            if (oldCount <= 0)
            {
                //清理冻结
                if (order.RunningOrder.SchemeType == SchemeType.ChaseBetting)
                    BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.RunningOrder.UserId, order.RunningOrder.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney), order.RunningOrder.TotalMoney);

                #region 拆票

                new Sports_Business().RequestTicketByGateway_SingleScheme_New(new GatewayTicketOrder_SingleScheme
                {
                    AllowCodes = order.AnteCode.AllowCodes,
                    Amount = order.RunningOrder.Amount,
                    ContainsMatchId = order.AnteCode.ContainsMatchId,
                    FileBuffer = order.AnteCode.FileBuffer,
                    GameCode = order.RunningOrder.GameCode,
                    GameType = order.RunningOrder.GameType,
                    IsRunningTicket = true,
                    IssuseNumber = order.RunningOrder.IssuseNumber,
                    IsVirtualOrder = false,
                    OrderId = order.RunningOrder.SchemeId,
                    PlayType = order.RunningOrder.PlayType,
                    SelectMatchId = order.AnteCode.SelectMatchId,
                    TotalMoney = order.RunningOrder.TotalMoney,
                    UserId = order.RunningOrder.UserId,
                });

                //new Thread(() =>
                //{
                try
                {
                    //生成文件
                    var json = Encoding.UTF8.GetString(order.AnteCode.FileBuffer);
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                    File.WriteAllText(fileName, json, Encoding.UTF8);
                }
                catch (Exception)
                {
                }
                //}).Start();


                #endregion

                //更新订单状态
                UpdateOrderTicketStatus(order.RunningOrder.SchemeId);

                //触发出票完成接口
                BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.RunningOrder.UserId, order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney, order.RunningOrder.TotalMoney });
            }
        }

        /// <summary>
        /// 添加订单到Redis库，本方法决定拆票或不拆票
        /// </summary>
        public static void AddOrderToRedis(string gameCode, RedisWaitTicketOrder order)
        {
            if (BusinessHelper.CanRequestBet(gameCode))
            {
                //可以拆票
                DoSplitOrderTicketWithThread(order);
            }
            else
            {
                //不能拆票
                AddOrderToWaitSplitList(order);
            }
        }

        /// <summary>
        /// 添加订单(单式)到Redis库，本方法决定拆票或不拆票
        /// </summary>
        public static void AddOrderToRedis(string gameCode, RedisWaitTicketOrderSingle order)
        {
            if (BusinessHelper.CanRequestBet(gameCode))
            {
                //可以拆票
                DoSplitOrderTicketWithThread_Single(order);
            }
            else
            {
                //不能拆票
                AddOrderToWaitSplitList(order);
            }
        }

        #endregion

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        /// <summary>
        /// 获取可用的队列Key
        /// </summary>
        public static string GetUsableList(string key)
        {
            try
            {
                var db = RedisHelper.DB_NoTicket_Order;
                var currentIndexKey = string.Format("{0}_Current", key);
                var indexValue = db.StringGetAsync(currentIndexKey).Result;
                var index = 0;
                if (indexValue.HasValue)
                {
                    //获取索引
                    index = int.Parse(indexValue.ToString());
                    index = index >= Max_PrizeListCount ? 0 : index + 1;
                }
                db.StringSetAsync(currentIndexKey, index);
                return string.Format("{0}_{1}", key, index);
            }
            catch (Exception)
            {
                return key;
            }
        }

        /// <summary>
        /// 拆票后,保存竞彩、北单订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_JC(string gameCode, string orderId, string[] matchIdArray, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;

            //把订单号和订单的比赛编号存入List
            var db = RedisHelper.DB_Running_Order_JC;
            //竞彩足球、竞彩篮球格式：比赛编号1,比赛编号2_订单号
            //北京单场格式：期号|比赛编号,期号|比赛编号_订单号
            var orderValue = string.Format("{0}_{1}", string.Join(",", matchIdArray), orderId);
            var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);
            fullKey = GetUsableList(fullKey);
            db.ListRightPushAsync(fullKey, orderValue);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                TicketList = ticketList,
            };
            var json = JsonSerializer.Serialize<RedisOrderInfo>(orderInfo);
            //以订单号为key 订单内容为value保存
            db.StringSetAsync(orderId, json);
        }

        /// <summary>
        /// 拆票后,北单订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_BJDC(string orderId, string[] matchIdArray, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;
            var db = RedisHelper.DB_Running_Order_BJDC;
            string issuseNumber = ticketList[0].IssuseNumber;
            var fullKey = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, issuseNumber);
            //fullKey = GetUsableList(fullKey);
            var orderValue = string.Format("{0}_{1}", string.Join(",", matchIdArray), orderId);
            db.ListRightPushAsync(fullKey, orderValue);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                TicketList = ticketList,
            };
            var json = JsonSerializer.Serialize<RedisOrderInfo>(orderInfo);
            //以订单号为key 订单内容为value保存
            db.StringSetAsync(orderId, json);
        }

        /// <summary>
        /// 拆票后,保存数字彩订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_SZC(SchemeType schemeType, string gameCode, string gameType, string orderId, string keyLine, bool stopAfterBonus, string issuseNumber, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;
            //把彩种、玩法、期号为Key,订单json存入List
            IDatabase db = null;// RedisHelper.DB_Running_Order;
            if (gameCode == "CTZQ")
                db = RedisHelper.DB_Running_Order_CTZQ;
            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "OZB" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_DP;
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_GP;

            if (db == null)
                throw new Exception(string.Format("找不到{0}对应的库", gameCode));

            var fullKey = (gameCode.ToUpper() == "CTZQ" || gameCode.ToUpper() == "OZB") ? string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, RedisKeys.Key_Running_Order_List, issuseNumber) :
                                                    string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
            //fullKey = GetUsableList(fullKey);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                KeyLine = keyLine,
                StopAfterBonus = stopAfterBonus,
                TicketList = ticketList,
                SchemeType = schemeType,
            };
            var json = JsonSerializer.Serialize<RedisOrderInfo>(orderInfo);
            db.ListRightPushAsync(fullKey, json);
        }

        /// <summary>
        /// 查询已出票未派奖的订单
        /// </summary>
        public static string QueryUnPrizeOrder()
        {
            return string.Join("|", new Sports_Manager().QueryUnPrizeOrder());
        }

        /// <summary>
        /// 添加SQL订单数据到Redis库中
        /// </summary>
        public static string AddToRunningOrder(string schemeId)
        {
            var logList = new List<string>();
            try
            {
                logList.Add(string.Format("开始处理订单{0}", schemeId));
                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception("订单数据为空");
                var ticketList = manager.QueryTicketList(schemeId);
                if (ticketList == null && ticketList.Count <= 0)
                    throw new Exception("订单无票数据");
                //取比赛编号，Redis票对象
                var matchIdList = new List<string>();
                var redisTicketList = new List<RedisTicketInfo>();
                foreach (var ticket in ticketList)
                {
                    if (!string.IsNullOrEmpty(ticket.MatchIdList))
                        matchIdList.AddRange(ticket.MatchIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                    redisTicketList.Add(new RedisTicketInfo
                    {
                        AfterBonusMoney = ticket.AfterTaxBonusMoney,
                        Amount = ticket.Amount,
                        BetContent = ticket.BetContent,
                        BetMoney = ticket.BetMoney,
                        BetUnits = ticket.BetUnits,
                        BonusStatus = ticket.BonusStatus,
                        GameCode = ticket.GameCode,
                        GameType = ticket.GameType,
                        IsAppend = ticket.IsAppend,
                        IssuseNumber = ticket.IssuseNumber,
                        LocOdds = ticket.LocOdds,
                        MatchIdList = ticket.MatchIdList,
                        PlayType = ticket.PlayType,
                        PreBonusMoney = ticket.PreTaxBonusMoney,
                        SchemeId = ticket.SchemeId,
                        TicketId = ticket.TicketId,
                    });
                }
                var matchIdArray = matchIdList.Distinct().ToArray();
                //竞彩或北单
                if (order.GameCode == "BJDC")
                {
                    //北单
                    AddToRunningOrder_BJDC(schemeId, matchIdArray, redisTicketList);
                }
                else if (new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode))
                {
                    //竞彩
                    AddToRunningOrder_JC(order.GameCode, schemeId, matchIdArray, redisTicketList);
                }
                else
                {
                    //传统足球或数字彩
                    var keyLine = schemeId;
                    var stopAfterBonus = true;
                    if (order.SchemeType == SchemeType.ChaseBetting)
                    {
                        var scheme = manager.QueryLotteryScheme(schemeId);
                        if (scheme == null)
                            throw new Exception("LotteryScheme数据为空");
                        keyLine = scheme.KeyLine;

                        var detail = new SchemeManager().QueryOrderDetail(schemeId);
                        if (detail == null)
                            throw new Exception("OrderDetail数据为空");
                        stopAfterBonus = detail.StopAfterBonus;
                    }
                    AddToRunningOrder_SZC(order.SchemeType, order.GameCode, order.GameType, schemeId, keyLine, stopAfterBonus, order.IssuseNumber, redisTicketList);
                }
                logList.Add("添加成功");
            }
            catch (Exception ex)
            {
                logList.Add(ex.Message);
            }
            return string.Join(Environment.NewLine, logList);
        }

        #region 竞彩足球派奖

        /// <summary>
        /// 订单手工派奖
        /// </summary>
        public static void ManualPrizeOrder_JCZQ(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) throw new Exception("订单号不能为空");

            var db = RedisHelper.DB_Running_Order_JC;
            var json = db.StringGetAsync(schemeId).Result;
            if (!json.HasValue)
                throw new Exception("订单数据为空");

            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
            var matchIdList = new List<string>();
            foreach (var item in orderInfo.TicketList)
            {
                matchIdList.AddRange(item.MatchIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            var matchIdArray = matchIdList.Distinct().ToArray();
            var jczqMatchResult = RedisMatchBusiness.QueryJCZQMatchResult();
            //查找比赛结果
            var query = from r in jczqMatchResult
                        where matchIdArray.Contains(r.MatchId)
                        select new MatchInfo
                        {
                            BF_Result = r.BF_Result,
                            BQC_Result = r.BQC_Result,
                            BRQSPF_Result = r.BRQSPF_Result,
                            SPF_Result = r.SPF_Result,
                            ZJQ_Result = r.ZJQ_Result,
                            CreateTime = r.CreateTime,
                            GameCode = "JCZQ",
                            GuestTeamHalfScore = r.HalfGuestTeamScore.ToString(),
                            GuestTeamScore = r.FullGuestTeamScore,
                            HomeTeamHalfScore = r.HalfHomeTeamScore.ToString(),
                            HomeTeamScore = r.FullHomeTeamScore,
                            MatchDate = r.MatchData,
                            MatchId = r.MatchId,
                            IssuseNumber = r.MatchNumber,
                            MatchState = r.MatchState,
                        };
            var orderMatchResultList = query.ToList();
            if (matchIdArray.Length != orderMatchResultList.Count)
                throw new Exception("彩果与投注比赛不一至，暂时不能派奖");

            PrizeOrder_JCZQ(orderInfo, orderMatchResultList);
            //删除订单json数据
            db.KeyDeleteAsync(schemeId);
        }

        /// <summary>
        /// 竞彩足球的订单派奖
        /// </summary>
        public static string PrizeOrder_JCZQ(int pageSize, string fullKey)
        {
            var watch_total = new Stopwatch();
            var watch = new Stopwatch();
            var logList = new List<string>();
            watch_total.Start();
            watch.Start();
            //查找比赛结果
            var jczqMatchResult = RedisMatchBusiness.QueryJCZQMatchResult();
            watch.Stop();
            logList.Add(string.Format("查询竞彩足球比赛结果数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            //var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Order_List);

            var db = RedisHelper.DB_Running_Order_JC;
            //找出竞彩足球运行中的订单
            var pageIndex = 0;
            var doOrderCount = 0;
            var manager = new SchemeManager();
            while (true)
            {
                //watch.Restart();
                logList.Add(string.Format("派奖Key:{0}", fullKey));
                var runingOrderList = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                logList.Add(string.Format("当前页订单{0}条", runingOrderList.Length));
                //watch.Stop();
                //logList.Add(string.Format("查询{0}-{1}条订单用时{2}毫秒", "JCZQ", pageSize, watch.Elapsed.TotalMilliseconds));

                #region 处理一页订单

                foreach (var item in runingOrderList)
                {
                    watch.Restart();
                    var orderId = string.Empty;
                    try
                    {
                        if (!item.HasValue)
                        {
                            //删除竞彩足球运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            continue;
                        }
                        //找出订单的比赛
                        //竞彩足球、竞彩篮球格式：比赛编号1,比赛编号2_订单号
                        var itemArray = item.ToString().Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);//比赛编号1,比赛编号2_订单号
                        if (itemArray.Length != 2)
                            continue;
                        var matchIdArray = itemArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                        orderId = itemArray[1];

                        //查找比赛结果
                        var query = from r in jczqMatchResult
                                    where matchIdArray.Contains(r.MatchId)
                                    select new MatchInfo
                                    {
                                        BF_Result = r.BF_Result,
                                        BQC_Result = r.BQC_Result,
                                        BRQSPF_Result = r.BRQSPF_Result,
                                        SPF_Result = r.SPF_Result,
                                        ZJQ_Result = r.ZJQ_Result,
                                        CreateTime = r.CreateTime,
                                        GameCode = "JCZQ",
                                        GuestTeamHalfScore = r.HalfGuestTeamScore.ToString(),
                                        GuestTeamScore = r.FullGuestTeamScore,
                                        HomeTeamHalfScore = r.HalfHomeTeamScore.ToString(),
                                        HomeTeamScore = r.FullHomeTeamScore,
                                        MatchDate = r.MatchData,
                                        MatchId = r.MatchId,
                                        IssuseNumber = r.MatchNumber,
                                        MatchState = r.MatchState,
                                    };
                        var orderMatchResultList = query.ToList();
                        if (matchIdArray.Length != orderMatchResultList.Count)
                        {
                            watch.Stop();
                            logList.Add(string.Format("订单{0}暂时不能派奖，用时{1}毫秒,订单{2}条，比赛结果{3}条", orderId, watch.Elapsed.TotalMilliseconds, matchIdArray.Length, orderMatchResultList.Count));
                            continue;
                        }

                        //订单对应的比赛已有比赛结果，执行订单派奖
                        var json = db.StringGetAsync(orderId).Result;
                        var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                        if (orderInfo == null)
                            throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                        logList.Add(PrizeOrder_JCZQ(orderInfo, orderMatchResultList));
                        //删除竞彩足球运行中的订单
                        db.ListRemoveAsync(fullKey, item);
                        //删除订单json数据
                        db.KeyDeleteAsync(orderId);
                        doOrderCount++;
                    }
                    catch (Exception ex)
                    {
                        //反序列化json为空的时候重新添加下
                        //RedisOrderBusiness.AddToRunningOrder(orderId);
                        //----------------------------------------------------------------------------------------------------------------------------
                        // 已完成的订单在try中会抛异常 再此判断移除 20171202
                        var orderDetail = manager.QueryOrderDetail(orderId);
                        if (orderDetail == null || orderDetail.ProgressStatus == ProgressStatus.Complate || orderDetail.ProgressStatus == ProgressStatus.Aborted || orderDetail.ProgressStatus == ProgressStatus.AutoStop || orderDetail.BonusStatus == BonusStatus.Win || orderDetail.BonusStatus == BonusStatus.Awarding)
                        {
                            logList.Add(string.Format("订单{0} orderDetail数据为空或者已完成、已中断、自动停止,从redis JCZQ_Running_Order_List中移除。", orderId));
                            //删除竞彩足球运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                        }
                        //----------------------------------------------------------------------------------------------------------------------------
                        logList.Add(item + ex.ToString());
                    }
                }

                #endregion

                pageIndex++;
                //最后一页
                if (runingOrderList.Length < pageSize)
                    break;
                //if (doOrderCount > doMaxCount)
                //    break;
            }
            logList.Add(string.Format("本次共派奖订单{0}条，总用时{1}毫秒", doOrderCount, watch_total.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        public static string PrizeOrder_JCZQ(RedisOrderInfo info, List<MatchInfo> orderMatchResultList)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            logList.Add(string.Format("开始计算{0}的中奖奖金", info.SchemeId));
            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var bonusCount = 0;
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;
                ComputeTicketBonus_JCZQ(info.SchemeId, "JCZQ", ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, orderMatchResultList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);
                //totalPreBonusMoney += preTaxBonusMoney;
                //totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }

            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
            logList.Add(string.Format("票数据：{0}条，奖金：{1}元", prizeList.Count, totalAfterBonusMoney));
            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //批量更新数据库
            logList.Add("批量更新数据库");
            BusinessHelper.UpdateTicketBonus(prizeList);
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //订单派奖
            logList.Add("执行订单派奖");
            //订单派奖
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        public static void ComputeTicketBonus_JCZQ(string orderId, string gameCode, string gameType, string betType, string locBetContent, string locOdds, int betAmount, IList<MatchInfo> matchResultList, decimal betMoney,
         out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        {
            bonusCount = 0;
            preTaxBonusMoney = 0M;
            afterTaxBonusMoney = 0M;
            #region odd 组合生成
            //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
            var locOddsL = locOdds.Split('/');
            var codeList = new List<Ticket_AnteCode_Running>();
            var collection = new GatewayAnteCodeCollection_Sport();
            var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
            foreach (var item in locBetContent.Split('/'))
            {
                var oneMatch = item.Split('_');
                if (gameType == "HH")
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[1],
                        AnteNumber = oneMatch[2],
                        IsDan = false,
                        GameType = oneMatch[0],
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[2],
                        MatchId = oneMatch[1],
                        GameType = oneMatch[0],
                        IsDan = false
                    });
                }
                else
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[0],
                        AnteNumber = oneMatch[1],
                        IsDan = false,
                        GameType = gameType,
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[1],
                        MatchId = oneMatch[0],
                        GameType = gameType,
                        IsDan = false
                    });
                }
            }
            #endregion

            var n = int.Parse(betType.Replace("P", "").Split('_')[1]);
            if (n > 1)
            {
                #region M串N
                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                {
                    Amount = betAmount,
                    AnteCodeList = collection,
                    Attach = string.Empty,
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = string.Empty,
                    IsVirtualOrder = false,
                    OrderId = orderId,
                    PlayType = betType.Replace("P", ""),
                    Price = 2,
                    UserId = string.Empty,
                    TotalMoney = betMoney
                }, "LOCAL", "agentId");

                foreach (var ticket in orderEntity.GetTicketList())
                {
                    var matchIdL = (from c in ticket.GetAnteCodeList() select c.MatchId).ToArray();
                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                    var baseCount = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]);
                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, baseCount);
                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), matchResultList.ToArray());
                    if (bonusResult.IsWin)
                    {
                        bonusCount = bonusResult.BonusCount;
                        for (var i = 0; i < bonusResult.BonusCount; i++)
                        {
                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                            var tmpAnteList = codeL.Where(a => matchIdList.Contains(a.MatchId));
                            var oneBeforeBonusMoney = 2M;
                            foreach (var item in tmpAnteList)
                            {
                                var odds = 1M;
                                var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                                var matchResult = result.GetMatchResult(gameCode, item.GameType);
                                odds = item.GetResultOdds(matchResult);

                                var anteCodeCount = item.AnteCode.Split(',').Count();
                                //if (baseCount == 1 && matchResult == "-1")
                                //{
                                //    preTaxBonusMoney += betMoney;
                                //    afterTaxBonusMoney += betMoney;
                                //    return;
                                //}
                                //else
                                if (anteCodeCount > 1 && matchResult == "-1")
                                {
                                    oneBeforeBonusMoney *= anteCodeCount;
                                }
                                else
                                {
                                    oneBeforeBonusMoney *= odds;
                                }
                            }
                            oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                            var oneAfterBonusMoney = oneBeforeBonusMoney;
                            if (oneBeforeBonusMoney >= 10000)
                            {
                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                            }

                            oneBeforeBonusMoney *= ticket.Amount;
                            oneAfterBonusMoney *= ticket.Amount;

                            preTaxBonusMoney += oneBeforeBonusMoney;
                            afterTaxBonusMoney += oneAfterBonusMoney;
                        }
                    }
                }

                //单票金额上限
                var m = int.Parse(betType.Replace("P", "").Split('_')[0]);
                if ((m == 2 || m == 3) && afterTaxBonusMoney >= 200000M)
                    afterTaxBonusMoney = 200000M;
                if ((m == 4 || m == 5) && afterTaxBonusMoney >= 500000M)
                    afterTaxBonusMoney = 500000M;
                if (m >= 6 && afterTaxBonusMoney >= 1000000M)
                    afterTaxBonusMoney = 1000000M;

                #endregion
            }
            else
            {
                #region M串1
                var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
                var record = string.Empty;
                if (bonusResult.IsWin)
                {
                    bonusCount = bonusResult.BonusCount;
                    for (var i = 0; i < bonusResult.BonusCount; i++)
                    {
                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                        var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
                        var oneBeforeBonusMoney = 2M;
                        foreach (var item in tmpAnteList)
                        {
                            var odds = 1M;

                            var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                            var matchResult = result.GetMatchResult(gameCode, item.GameType);
                            odds = item.GetResultOdds(matchResult);

                            var anteCodeCount = item.AnteCode.Split(',').Count();
                            if (baseCount == 1 && matchResult == "-1")
                            {
                                preTaxBonusMoney += betMoney;
                                afterTaxBonusMoney += betMoney;
                                return;
                            }
                            else if (anteCodeCount > 1 && matchResult == "-1")
                            {
                                oneBeforeBonusMoney *= anteCodeCount;
                            }
                            else
                            {
                                oneBeforeBonusMoney *= odds;
                            }
                        }
                        oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                        var oneAfterBonusMoney = oneBeforeBonusMoney;
                        if (oneBeforeBonusMoney >= 10000)
                        {
                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                        }
                        oneBeforeBonusMoney *= betAmount;
                        oneAfterBonusMoney *= betAmount;

                        //单票金额上限
                        if ((baseCount == 2 || baseCount == 3) && oneAfterBonusMoney >= 200000M)
                            oneAfterBonusMoney = 200000M;
                        if ((baseCount == 4 || baseCount == 5) && oneAfterBonusMoney >= 500000M)
                            oneAfterBonusMoney = 500000M;
                        if (baseCount >= 6 && oneAfterBonusMoney >= 1000000M)
                            oneAfterBonusMoney = 1000000M;

                        preTaxBonusMoney += oneBeforeBonusMoney;
                        afterTaxBonusMoney += oneAfterBonusMoney;
                    }
                }

                #endregion
            }
        }

        #endregion

        #region 竞彩篮球派奖

        /// <summary>
        /// 订单手工派奖
        /// </summary>
        public static void ManualPrizeOrder_JCLQ(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) throw new Exception("订单号不能为空");

            var db = RedisHelper.DB_Running_Order_JC;
            var json = db.StringGetAsync(schemeId).Result;
            if (!json.HasValue)
                throw new Exception("订单数据为空");

            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
            var matchIdList = new List<string>();
            foreach (var item in orderInfo.TicketList)
            {
                matchIdList.AddRange(item.MatchIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            var matchIdArray = matchIdList.Distinct().ToArray();
            var jclqMatchResult = RedisMatchBusiness.QueryJCLQMatchResult();
            //查找比赛结果
            var query = from r in jclqMatchResult
                        where matchIdArray.Contains(r.MatchId)
                        select new MatchInfo
                        {
                            SF_Result = r.SF_Result,
                            RFSF_Result = r.RFSF_Result,
                            SFC_Result = r.SFC_Result,
                            DXF_Result = r.DXF_Result,
                            HomeTeamScore = r.HomeScore,
                            GuestTeamScore = r.GuestScore,
                            CreateTime = r.CreateTime,
                            GameCode = "JCLQ",
                            MatchDate = r.MatchData,
                            MatchId = r.MatchId,
                            IssuseNumber = r.MatchNumber,
                            MatchState = r.MatchState,
                        };
            var orderMatchResultList = query.ToList();
            if (matchIdArray.Length != orderMatchResultList.Count)
                throw new Exception("彩果与投注比赛不一至，暂时不能派奖");

            PrizeOrder_JCLQ(orderInfo, orderMatchResultList);
            //删除订单json数据
            db.KeyDeleteAsync(schemeId);
        }

        public static string PrizeOrder_JCLQ(int pageSize, string fullKey)
        {
            var watch_total = new Stopwatch();
            var watch = new Stopwatch();
            var logList = new List<string>();
            watch_total.Start();
            watch.Start();
            //查找比赛结果
            var jclqMatchResult = RedisMatchBusiness.QueryJCLQMatchResult();
            watch.Stop();
            logList.Add(string.Format("查询竞彩篮球比赛结果数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            //var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Order_List);
            var db = RedisHelper.DB_Running_Order_JC;
            //找出竞彩篮球运行中的订单
            var pageIndex = 0;
            var doOrderCount = 0;
            var manager = new SchemeManager();
            while (true)
            {
                #region 处理一页订单

                //watch.Restart();
                logList.Add(string.Format("派奖Key:{0}", fullKey));
                var runingOrderList = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                logList.Add(string.Format("当前页订单{0}条", runingOrderList.Length));
                //watch.Stop();
                //logList.Add(string.Format("查询{0}-{1}条订单用时{2}毫秒", "JCLQ", pageSize, watch.Elapsed.TotalMilliseconds));
                foreach (var item in runingOrderList)
                {
                    watch.Restart();
                    var orderId = string.Empty;
                    try
                    {
                        if (!item.HasValue)
                        {
                            //删除竞彩篮球运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            continue;
                        }
                        //找出订单的比赛
                        //竞彩足球、竞彩篮球格式：比赛编号1,比赛编号2_订单号
                        var itemArray = item.ToString().Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);//比赛编号1,比赛编号2_订单号
                        if (itemArray.Length != 2)
                            continue;
                        var matchIdArray = itemArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                        orderId = itemArray[1];

                        //var orderDetail = manager.QueryOrderDetail(orderId);
                        //if (orderDetail == null || orderDetail.ProgressStatus == ProgressStatus.Complate || orderDetail.ProgressStatus == ProgressStatus.Aborted || orderDetail.ProgressStatus == ProgressStatus.AutoStop)
                        //{
                        //    logList.Add(string.Format("订单{0} orderDetail数据为空或者已完成、已中断、自动停止,从redis JCZQ_Running_Order_List中移除。", orderId));
                        //    //删除竞彩足球运行中的订单
                        //    db.ListRemoveAsync(fullKey, item);
                        //    continue;
                        //}
                        //查找比赛结果
                        var query = from r in jclqMatchResult
                                    where matchIdArray.Contains(r.MatchId)
                                    select new MatchInfo
                                    {
                                        SF_Result = r.SF_Result,
                                        RFSF_Result = r.RFSF_Result,
                                        SFC_Result = r.SFC_Result,
                                        DXF_Result = r.DXF_Result,
                                        HomeTeamScore = r.HomeScore,
                                        GuestTeamScore = r.GuestScore,
                                        CreateTime = r.CreateTime,
                                        GameCode = "JCLQ",
                                        MatchDate = r.MatchData,
                                        MatchId = r.MatchId,
                                        IssuseNumber = r.MatchNumber,
                                        MatchState = r.MatchState,
                                    };
                        var orderMatchResultList = query.ToList(); //jczqMatchResult.Where(p => matchIdArray.Contains(p.MatchId)).ToList();
                        if (matchIdArray.Length != orderMatchResultList.Count)
                        {
                            watch.Stop();
                            logList.Add(string.Format("订单{0}暂时不能派奖，用时{1}毫秒，订单{2}条，比赛结果{3}条", orderId, watch.Elapsed.TotalMilliseconds, matchIdArray.Length, orderMatchResultList.Count));
                            continue;
                        }

                        //订单对应的比赛已有比赛结果，执行订单派奖
                        var json = db.StringGetAsync(orderId).Result;
                        var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                        if (orderInfo == null)
                            throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                        logList.Add(PrizeOrder_JCLQ(orderInfo, orderMatchResultList));
                        //删除竞彩篮球运行中的订单
                        db.ListRemoveAsync(fullKey, item);
                        //删除订单json数据
                        db.KeyDeleteAsync(orderId);
                        doOrderCount++;

                    }
                    catch (Exception ex)
                    {
                        logList.Add(ex.ToString());
                    }

                }

                #endregion
                pageIndex++;
                //最后一页
                if (runingOrderList.Length < pageSize)
                    break;
                //if (doOrderCount > doMaxCount)
                //    break;
            }
            logList.Add(string.Format("本次共派奖订单{0}条，总用时{1}毫秒", doOrderCount, watch_total.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_JCLQ(RedisOrderInfo info, List<MatchInfo> orderMatchResultList)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            logList.Add(string.Format("开始计算{0}的中奖奖金", info.SchemeId));
            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var bonusCount = 0;
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;
                ComputeTicketBonus_JCLQ(info.SchemeId, "JCLQ", ticket.GameType, ticket.PlayType, ticket.BetContent, ticket.LocOdds, ticket.Amount, orderMatchResultList, ticket.BetMoney, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);
                //totalPreBonusMoney += preTaxBonusMoney;
                //totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }

            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
            logList.Add(string.Format("票数据：{0}条，奖金：{1}元", prizeList.Count, totalAfterBonusMoney));
            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //批量更新数据库
            logList.Add("批量更新数据库");
            BusinessHelper.UpdateTicketBonus(prizeList);
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //订单派奖
            logList.Add("执行订单派奖");
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        private static void ComputeTicketBonus_JCLQ(string orderId, string gameCode, string gameType, string betType, string locBetCount, string locOdds, int betAmount, IList<MatchInfo> matchResultList, decimal betMoney,
           out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        {
            bonusCount = 0;
            preTaxBonusMoney = 0M;
            afterTaxBonusMoney = 0M;
            //140605051_3|1.9200,1|3.6500,0|3.0000/140605052_3|3.8000,1|4.0500,0|1.6200
            var locOddsL = locOdds.Split('/');
            var arrayOdds = locOddsL.Select(s => s.Split('_')).ToArray();
            //查询JCLQ_AnteCode_Running表
            //140508010_1/140508011_1
            var codeList = new List<Ticket_AnteCode_Running>();
            var collection = new GatewayAnteCodeCollection_Sport();
            foreach (var item in locBetCount.Split('/'))
            {
                var oneMatch = item.Split('_');
                if (gameType == "HH")
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[1])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[1],
                        AnteNumber = oneMatch[2],
                        IsDan = false,
                        GameType = oneMatch[0],
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[2],
                        MatchId = oneMatch[1],
                        GameType = oneMatch[0],
                        IsDan = false
                    });
                }
                else
                {
                    var SP = arrayOdds.Where(s => s.Contains(oneMatch[0])).ToArray();
                    codeList.Add(new Ticket_AnteCode_Running
                    {
                        MatchId = oneMatch[0],
                        AnteNumber = oneMatch[1],
                        IsDan = false,
                        GameType = gameType,
                        Odds = SP[0][1].ToString()
                    });
                    collection.Add(new GatewayAnteCode_Sport
                    {
                        AnteCode = oneMatch[1],
                        MatchId = oneMatch[0],
                        GameType = gameType,
                        IsDan = false
                    });
                }
            }
            var n = int.Parse(betType.Replace("P", "").Split('_')[1]);
            if (n > 1)
            {
                #region M串N
                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                {
                    Amount = betAmount,
                    AnteCodeList = collection,
                    Attach = string.Empty,
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = string.Empty,
                    IsVirtualOrder = false,
                    OrderId = orderId,
                    PlayType = betType.Replace("P", ""),
                    Price = 2,
                    UserId = string.Empty,
                    TotalMoney = betMoney
                }, "LOCAL", "agentId");

                foreach (var ticket in orderEntity.GetTicketList())
                {
                    var matchIdL = (from c in ticket.GetAnteCodeList() select c.MatchId).ToArray();
                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                    var baseCount = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]);
                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, baseCount);
                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), matchResultList.ToArray());
                    if (bonusResult.IsWin)
                    {
                        bonusCount = bonusResult.BonusCount;
                        for (var i = 0; i < bonusResult.BonusCount; i++)
                        {
                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                            var tmpAnteList = codeL.Where(a => matchIdList.Contains(a.MatchId));
                            var oneBeforeBonusMoney = 2M;
                            foreach (var item in tmpAnteList)
                            {
                                var odds = 1M;

                                var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                                var offset = -1M;
                                switch (item.GameType.ToUpper())
                                {
                                    case "RFSF":
                                        offset = item.GetResultOdds("RF");
                                        break;
                                    case "DXF":
                                        offset = item.GetResultOdds("YSZF");
                                        break;
                                }
                                var matchResult = result.GetMatchResult(gameCode, item.GameType, offset);
                                odds = item.GetResultOdds(matchResult);
                                var anteCodeCount = item.AnteCode.Split(',').Count();
                                if (anteCodeCount > 1 && matchResult == "-1")
                                {
                                    oneBeforeBonusMoney *= anteCodeCount;
                                }
                                else
                                {
                                    oneBeforeBonusMoney *= odds;
                                }
                            }
                            oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                            var oneAfterBonusMoney = oneBeforeBonusMoney;
                            if (oneBeforeBonusMoney >= 10000)
                            {
                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                            }
                            oneBeforeBonusMoney *= ticket.Amount;
                            oneAfterBonusMoney *= ticket.Amount;

                            preTaxBonusMoney += oneBeforeBonusMoney;
                            afterTaxBonusMoney += oneAfterBonusMoney;
                        }
                    }
                }

                //单票金额上限
                var m = int.Parse(betType.Replace("P", "").Split('_')[0]);
                if (m == 1 && afterTaxBonusMoney >= 100000M)
                    afterTaxBonusMoney = 100000M;
                if ((m == 2 || m == 3) && afterTaxBonusMoney >= 200000M)
                    afterTaxBonusMoney = 200000M;
                if ((m == 4 || m == 5) && afterTaxBonusMoney >= 500000M)
                    afterTaxBonusMoney = 500000M;
                if (m >= 6 && afterTaxBonusMoney >= 1000000M)
                    afterTaxBonusMoney = 1000000M;

                #endregion
            }
            else
            {
                #region M串1
                var baseCount = int.Parse(betType.Replace("P", "").Split('_')[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer(gameCode, gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), matchResultList.ToArray());
                if (bonusResult.IsWin)
                {
                    bonusCount = bonusResult.BonusCount;
                    for (var i = 0; i < bonusResult.BonusCount; i++)
                    {
                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                        var tmpAnteList = codeList.Where(a => matchIdList.Contains(a.MatchId));
                        var oneBeforeBonusMoney = 2M;
                        foreach (var item in tmpAnteList)
                        {
                            var odds = 1M;

                            var result = matchResultList.Single(r => r.GetMatchId(gameCode).Equals(item.MatchId));
                            var offset = -1M;
                            switch (item.GameType.ToUpper())
                            {
                                case "RFSF":
                                    offset = item.GetResultOdds("RF");
                                    break;
                                case "DXF":
                                    offset = item.GetResultOdds("YSZF");
                                    break;
                            }
                            var matchResult = result.GetMatchResult(gameCode, item.GameType, offset);
                            odds = item.GetResultOdds(matchResult);
                            var anteCodeCount = item.AnteCode.Split(',').Count();
                            if (baseCount == 1 && matchResult == "-1")
                            {
                                preTaxBonusMoney += betMoney;
                                afterTaxBonusMoney += betMoney;
                                return;
                            }
                            else if (anteCodeCount > 1 && matchResult == "-1")
                            {
                                oneBeforeBonusMoney *= anteCodeCount;
                            }
                            else
                            {
                                oneBeforeBonusMoney *= odds;
                            }
                        }
                        oneBeforeBonusMoney = new SMGBonus().FourToSixHomesInFive(oneBeforeBonusMoney);
                        var oneAfterBonusMoney = oneBeforeBonusMoney;
                        if (oneBeforeBonusMoney >= 10000)
                        {
                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                        }
                        oneBeforeBonusMoney *= betAmount;
                        oneAfterBonusMoney *= betAmount;

                        //单票金额上限
                        if (baseCount == 1 && oneAfterBonusMoney >= 100000M)
                            oneAfterBonusMoney = 100000M;
                        if ((baseCount == 2 || baseCount == 3) && oneAfterBonusMoney >= 200000M)
                            oneAfterBonusMoney = 200000M;
                        if ((baseCount == 4 || baseCount == 5) && oneAfterBonusMoney >= 500000M)
                            oneAfterBonusMoney = 500000M;
                        if (baseCount >= 6 && oneAfterBonusMoney >= 1000000M)
                            oneAfterBonusMoney = 1000000M;

                        preTaxBonusMoney += oneBeforeBonusMoney;
                        afterTaxBonusMoney += oneAfterBonusMoney;
                    }


                }
                #endregion
            }
        }

        #endregion

        #region 北京单场派奖

        /// <summary>
        /// 订单手工派奖
        /// </summary>
        public static void ManualPrizeOrder_BJDC(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) throw new Exception("订单号不能为空");

            var db = RedisHelper.DB_Running_Order_BJDC;
            var json = db.StringGetAsync(schemeId).Result;
            if (!json.HasValue)
                throw new Exception("订单数据为空");

            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
            var matchIdList = new List<string>();
            foreach (var item in orderInfo.TicketList)
            {
                matchIdList.AddRange(item.MatchIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            var matchIdArray = matchIdList.Distinct().ToArray();
            var matchResult = RedisMatchBusiness.QueryBJDCMatchResult();
            //查找比赛结果
            var query = from r in matchResult
                        where r.IssuseNumber == orderInfo.TicketList[0].IssuseNumber
                        && matchIdArray.Contains(r.MatchOrderId.ToString())
                        select new MatchInfo
                        {
                            SPF_Result = r.SPF_Result,
                            ZJQ_Result = r.ZJQ_Result,
                            SXDS_Result = r.SXDS_Result,
                            BF_Result = r.BF_Result,
                            BQC_Result = r.BQC_Result,
                            SPF_SP = r.SPF_SP,
                            ZJQ_SP = r.ZJQ_SP,
                            SXDS_SP = r.SXDS_SP,
                            BF_SP = r.BF_SP,
                            BQC_SP = r.BQC_SP,
                            CreateTime = r.CreateTime,
                            GameCode = "BJDC",
                            MatchIndex = r.MatchOrderId,
                            IssuseNumber = r.IssuseNumber,
                            MatchState = r.MatchState,
                        };
            var orderMatchResultList = query.ToList();
            if (matchIdArray.Length != orderMatchResultList.Count)
                throw new Exception("彩果与投注比赛不一至，暂时不能派奖");

            PrizeOrder_BJDC(orderInfo, orderMatchResultList);
            //删除订单json数据
            db.KeyDeleteAsync(schemeId);
        }

        public static string PrizeOrder_BJDC(int pageSize, int doMaxCount)
        {
            var watch_total = new Stopwatch();
            var watch = new Stopwatch();
            var logList = new List<string>();
            watch_total.Start();
            watch.Start();
            //查找比赛结果
            var matchResult = RedisMatchBusiness.QueryBJDCMatchResult();
            watch.Stop();
            logList.Add(string.Format("查询北京单场比赛结果数据用时{0}毫秒", watch.Elapsed.TotalMilliseconds));

            var db = RedisHelper.DB_Running_Order_BJDC;
            //按期号GroupBy
            var resultGroup = matchResult.GroupBy(p => p.IssuseNumber);
            foreach (var g in resultGroup)
            {
                var issuseNumber = g.Key;
                var currentResult = g.ToList();

                #region 对指定期的订单派奖

                var pageIndex = 0;
                var doOrderCount = 0;
                var fullKey = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, issuseNumber);
                while (true)
                {
                    //找出北京单场运行中的订单
                    //watch.Restart();
                    logList.Add(string.Format("派奖Key:{0}", fullKey));
                    var runingOrderList = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                    logList.Add(string.Format("当前页订单{0}条", runingOrderList.Length));
                    //watch.Stop();
                    //logList.Add(string.Format("查询{0}{1}期-{2}条订单用时{3}毫秒", "BJDC", issuseNumber, pageSize, watch.Elapsed.TotalMilliseconds));
                    foreach (var item in runingOrderList)
                    {
                        var orderId = string.Empty;
                        try
                        {
                            #region 对一条订单进行处理

                            if (!item.HasValue)
                            {
                                //删除北京单场运行中的订单
                                db.ListRemoveAsync(fullKey, item);
                                continue;
                            }

                            //北京单场格式：比赛编号1,比赛编号2_订单号
                            var itemArray = item.ToString().Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            if (itemArray.Length != 2)
                                continue;

                            watch.Restart();
                            orderId = itemArray[1];
                            var orderMatchIdArray = itemArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                            //查找比赛结果
                            var query = from r in currentResult
                                        where orderMatchIdArray.Contains(r.MatchOrderId.ToString())
                                        select new MatchInfo
                                        {
                                            SPF_Result = r.SPF_Result,
                                            ZJQ_Result = r.ZJQ_Result,
                                            SXDS_Result = r.SXDS_Result,
                                            BF_Result = r.BF_Result,
                                            BQC_Result = r.BQC_Result,
                                            SPF_SP = r.SPF_SP,
                                            ZJQ_SP = r.ZJQ_SP,
                                            SXDS_SP = r.SXDS_SP,
                                            BF_SP = r.BF_SP,
                                            BQC_SP = r.BQC_SP,
                                            CreateTime = r.CreateTime,
                                            GameCode = "BJDC",
                                            MatchIndex = r.MatchOrderId,
                                            IssuseNumber = r.IssuseNumber,
                                            MatchState = r.MatchState,
                                        };
                            var orderMatchResultList = query.ToList();
                            if (orderMatchIdArray.Length != orderMatchResultList.Count)
                            {
                                watch.Stop();
                                logList.Add(string.Format("订单{0}暂时不能派奖，用时{1}毫秒,订单有{2}条，比赛结果有{3}条", orderId, watch.Elapsed.TotalMilliseconds, orderMatchIdArray.Length, orderMatchResultList.Count));
                                continue;
                            }

                            //订单对应的比赛已有比赛结果，执行订单派奖
                            var json = db.StringGetAsync(orderId).Result;
                            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                            if (orderInfo == null)
                                throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                            logList.Add(PrizeOrder_BJDC(orderInfo, orderMatchResultList));
                            //删除北京单场运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            //删除订单json数据
                            db.KeyDeleteAsync(orderId);
                            doOrderCount++;

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            logList.Add(string.Format("订单{0}派奖异常：{1}", orderId, ex.ToString()));
                        }
                    }
                    pageIndex++;
                    //最后一页
                    if (runingOrderList.Length < pageSize)
                        break;
                    if (doOrderCount > doMaxCount)
                        break;
                }

                #endregion
            }

            watch_total.Stop();
            logList.Add(string.Format("本次共派奖订单，总用时{0}毫秒", watch_total.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_BJDC(RedisOrderInfo info, List<MatchInfo> orderMatchResultList)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            logList.Add(string.Format("开始计算{0}的中奖奖金", info.SchemeId));
            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var bonusCount = 0;
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;

                ComputeTicketBonus_BJDC(ticket, orderMatchResultList, out bonusCount, out preTaxBonusMoney, out afterTaxBonusMoney);

                //totalPreBonusMoney += preTaxBonusMoney;
                //totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }
            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
            logList.Add(string.Format("票数据：{0}条，奖金：{1}元", prizeList.Count, totalAfterBonusMoney));
            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));


            watch.Restart();
            //批量更新数据库
            logList.Add("批量更新数据库");
            BusinessHelper.UpdateTicketBonus(prizeList);
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            //订单派奖
            watch.Restart();
            //订单派奖
            logList.Add("执行订单派奖");
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        private static void ComputeTicketBonus_BJDC(RedisTicketInfo ticket, List<MatchInfo> orderMatchResultList, out int bonusCount, out decimal preTaxBonusMoney, out decimal afterTaxBonusMoney)
        {
            bonusCount = 0;
            preTaxBonusMoney = 0M;
            afterTaxBonusMoney = 0M;

            var collection = new GatewayAnteCodeCollection_Sport();
            var codeList = new List<Ticket_AnteCode_Running>();
            //100_3/101_1
            foreach (var item in ticket.BetContent.Split('/'))
            {
                var oneMatch = item.Split('_');
                codeList.Add(new Ticket_AnteCode_Running
                {
                    MatchId = oneMatch[0],
                    IssuseNumber = ticket.IssuseNumber,
                    AnteNumber = oneMatch[1],
                    IsDan = false,
                    GameType = ticket.GameType,
                });
                collection.Add(new GatewayAnteCode_Sport
                {
                    AnteCode = oneMatch[1],
                    MatchId = oneMatch[0],
                    GameType = ticket.GameType,
                    IsDan = false
                });
            }

            var n = int.Parse(ticket.PlayType.Replace("P", "").Split('_')[1]);
            if (n > 1)
            {
                #region M串N

                var orderEntity = new Sports_Business().AnalyzeOrder_Sport_Prize<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(new GatewayTicketOrder_Sport
                {
                    Amount = ticket.Amount,
                    AnteCodeList = collection,
                    Attach = string.Empty,
                    GameCode = ticket.GameCode,
                    GameType = ticket.GameType,
                    IssuseNumber = ticket.IssuseNumber,
                    IsVirtualOrder = false,
                    OrderId = ticket.SchemeId,
                    PlayType = ticket.PlayType.Replace("P", ""),
                    Price = 2,
                    UserId = string.Empty,
                    TotalMoney = ticket.BetMoney
                }, "LOCAL", "agentId");

                foreach (var ticket_cp in orderEntity.GetTicketList())
                {
                    var matchIdL = (from c in ticket_cp.GetAnteCodeList() select c.MatchId).ToArray();
                    var codeL = codeList.Where(p => matchIdL.Contains(p.MatchId)).ToArray();

                    var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket_cp.GameCode, ticket_cp.GameType, int.Parse(ticket_cp.PlayType.Replace("P", "").Split('_')[0]));
                    var bonusResult = analyzer.CaculateBonus(codeL.ToArray(), orderMatchResultList.ToArray());
                    if (bonusResult.IsWin)
                    {
                        bonusCount += bonusResult.BonusCount;
                        for (var i = 0; i < bonusResult.BonusCount; i++)
                        {
                            var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                            var sps = GetSPs(ticket_cp.GameType, ticket_cp.IssuseNumber, matchIdList, orderMatchResultList);
                            if (sps.Count != matchIdList.Length)
                                throw new Exception("比赛与SP条数不匹配");
                            var oneBeforeBonusMoney = 2M;
                            var isTrue = false;
                            var num = 0;
                            foreach (var item in sps)
                            {
                                if (item.Value == 1M)
                                {
                                    num++;
                                    var entity = codeL.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
                                    var anteCodeCount = entity.AnteCode.Split(',').Count();
                                    oneBeforeBonusMoney *= anteCodeCount;
                                    if (sps.Count == 1) isTrue = true;
                                }
                                else
                                {
                                    oneBeforeBonusMoney *= item.Value;
                                }
                            }
                            if (!isTrue && num != sps.Count)
                                oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
                            oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
                            var oneAfterBonusMoney = oneBeforeBonusMoney;

                            //北单奖金小于2元的 按2元计算
                            if (oneBeforeBonusMoney < 2M)
                            {
                                oneBeforeBonusMoney = 2M;
                                oneAfterBonusMoney = 2M;
                            }

                            if (oneBeforeBonusMoney >= 10000)
                            {
                                oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                            }
                            oneBeforeBonusMoney *= ticket_cp.Amount;
                            oneAfterBonusMoney *= ticket_cp.Amount;

                            preTaxBonusMoney += oneBeforeBonusMoney;
                            afterTaxBonusMoney += oneAfterBonusMoney;
                        }
                    }
                }

                //单票金额上限
                if (afterTaxBonusMoney >= 5000000M)
                    afterTaxBonusMoney = 5000000M;

                #endregion
            }
            else
            {
                #region M串1

                var analyzer = AnalyzerFactory.GetSportAnalyzer(ticket.GameCode, ticket.GameType, int.Parse(ticket.PlayType.Replace("P", "").Split('_')[0]));
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), orderMatchResultList.ToArray());
                if (bonusResult.IsWin)
                {
                    bonusCount += bonusResult.BonusCount;
                    for (var i = 0; i < bonusResult.BonusCount; i++)
                    {
                        var matchIdList = bonusResult.BonusHitMatchIdListCollection[i];
                        var sps = GetSPs(ticket.GameType, ticket.IssuseNumber, matchIdList, orderMatchResultList);
                        if (sps.Count != matchIdList.Length)
                            throw new Exception("比赛与SP条数不匹配");
                        var oneBeforeBonusMoney = 2M;
                        var isTrue = false;
                        var num = 0;
                        foreach (var item in sps)
                        {
                            if (item.Value == 1M)
                            {
                                num++;
                                var entity = codeList.Where(p => p.MatchIndex == int.Parse(item.Key)).FirstOrDefault();
                                var anteCodeCount = entity.AnteCode.Split(',').Count();
                                oneBeforeBonusMoney *= anteCodeCount;
                                if (sps.Count == 1) isTrue = true;
                            }
                            else
                            {
                                oneBeforeBonusMoney *= item.Value;
                            }
                        }
                        if (!isTrue && num != sps.Count)
                            oneBeforeBonusMoney *= 0.65M;    // 官方扣点 - 65%
                        oneBeforeBonusMoney = Math.Truncate(oneBeforeBonusMoney * 100) / 100;
                        var oneAfterBonusMoney = oneBeforeBonusMoney;

                        //北单奖金小于2元的 按2元计算
                        if (oneBeforeBonusMoney < 2M)
                        {
                            oneBeforeBonusMoney = 2M;
                            oneAfterBonusMoney = 2M;
                        }

                        if (oneBeforeBonusMoney >= 10000)
                        {
                            oneAfterBonusMoney = oneBeforeBonusMoney * (1M - 0.2M);
                        }
                        oneBeforeBonusMoney *= ticket.Amount;
                        oneAfterBonusMoney *= ticket.Amount;

                        //单票金额上限
                        if (oneAfterBonusMoney >= 5000000M)
                            oneAfterBonusMoney = 5000000M;

                        preTaxBonusMoney += oneBeforeBonusMoney;
                        afterTaxBonusMoney += oneAfterBonusMoney;
                    }
                }

                #endregion
            }

        }

        private static Dictionary<string, decimal> GetSPs(string gameType, string issuseNumber, string[] matchIdList, List<MatchInfo> matchInfo)
        {
            var result = new Dictionary<string, decimal>();
            foreach (var id in matchIdList)
            {
                var entity = matchInfo.FirstOrDefault(p => p.IssuseNumber == issuseNumber && p.MatchIndex == int.Parse(id));
                if (entity == null) continue;

                switch (gameType)
                {
                    case "SPF":
                        if (entity.MatchState != "2" || (entity.SPF_Result != "-1" && entity.SPF_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        result.Add(id, entity.SPF_SP);
                        break;
                    case "ZJQ":
                        if (entity.MatchState != "2" || (entity.ZJQ_Result != "-1" && entity.ZJQ_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        result.Add(id, entity.ZJQ_SP);
                        break;
                    case "SXDS":
                        if (entity.MatchState != "2" || (entity.SXDS_Result != "-1" && entity.SXDS_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        result.Add(id, entity.SXDS_SP);
                        break;
                    case "BF":
                        if (entity.MatchState != "2" || (entity.BF_Result != "-1" && entity.BF_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        result.Add(id, entity.BF_SP);
                        break;
                    case "BQC":
                        if (entity.MatchState != "2" || (entity.BQC_Result != "-1" && entity.BQC_SP == 1M))
                            throw new ArgumentException("获取比赛结果，当场比赛有异常" + issuseNumber + " - " + id);
                        result.Add(id, entity.BQC_SP);
                        break;
                    default:
                        throw new ArgumentException("获取比赛结果，不支持的玩法 -" + gameType);
                }

            }
            return result;
        }

        #endregion

        #region 传统足球派奖

        public static string PrizeOrder_CTZQ(string gameType, int pageSize, int doMaxCount)
        {
            var watch_total = new Stopwatch();
            var watch = new Stopwatch();
            var logList = new List<string>();
            watch_total.Start();
            watch.Start();
            //查找比赛结果
            var matchResult = RedisMatchBusiness.QuseryCTZQBonusPool(gameType);
            watch.Stop();
            logList.Add(string.Format("查询{0}奖池数据用时{1}毫秒", gameType, watch.Elapsed.TotalMilliseconds));

            //按期号GroupBy
            var resultGroup = matchResult.GroupBy(p => p.IssuseNumber);
            foreach (var g in resultGroup)
            {
                var issuseNumber = g.Key;
                var currentResult = g.ToList();

                //查找比赛结果
                var poolList = currentResult.Where(p => p.BonusMoney > 0).ToList();
                if (gameType == "T14C")
                {
                    if (poolList.Count != 2)
                    {
                        watch.Stop();
                        logList.Add(string.Format("{0}期号{1}奖池数据不完整，用时{1}毫秒", gameType, issuseNumber, watch.Elapsed.TotalMilliseconds));
                        continue;
                    }
                }
                if (poolList.Count < 1)
                {
                    watch.Stop();
                    logList.Add(string.Format("{0}期号{1}奖池数据为0，用时{1}毫秒", gameType, issuseNumber, watch.Elapsed.TotalMilliseconds));
                    continue;
                }

                #region 对指定期的订单派奖

                var doOrderCount = 0;
                var fullKey = string.Format("{0}_{1}_{2}_{3}", "CTZQ", gameType, RedisKeys.Key_Running_Order_List, issuseNumber);
                logList.Add(string.Format("派奖Key:{0}", fullKey));
                var db = RedisHelper.DB_Running_Order_CTZQ;
                var pageIndex = 0;
                //找出传统足球运行中的订单
                while (true)
                {
                    #region 处理一页订单

                    //watch.Restart();
                    var runingOrderList = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                    //watch.Stop();
                    //logList.Add(string.Format("查询{0}{1}期-{2}条订单用时{3}毫秒", gameType, issuseNumber, pageSize, watch.Elapsed.TotalMilliseconds));
                    logList.Add(string.Format("当前页订单{0}条", runingOrderList.Length));
                    foreach (var item in runingOrderList)
                    {
                        try
                        {
                            if (!item.HasValue)
                            {
                                //删除传统足球运行中的订单
                                db.ListRemoveAsync(fullKey, item);
                                continue;
                            }

                            var json = item.ToString();
                            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                            if (orderInfo == null)
                                throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                            logList.Add(PrizeOrder_CTZQ(orderInfo, poolList[0].WinNumber, poolList));
                            //删除运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            doOrderCount++;
                        }
                        catch (Exception ex)
                        {
                            logList.Add(ex.ToString());
                            //writer.Write("RedisOrderBusiness", "PrizeOrder_CTZQ " + gameType, ex);
                        }
                    }

                    #endregion

                    pageIndex++;
                    //最后一页
                    if (runingOrderList.Length < pageSize)
                        break;
                    if (doOrderCount > doMaxCount)
                        break;
                }

                #endregion
            }

            watch_total.Stop();
            logList.Add(string.Format("本次共派奖订单,总用时{0}毫秒", watch_total.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_CTZQ(RedisOrderInfo info, string winNumber, List<Ticket_BonusPool> pool)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            logList.Add(string.Format("开始计算{0}的中奖奖金", info.SchemeId));
            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;

                DoComputeTicketBonusMoney(ticket.TicketId, ticket.GameCode, ticket.GameType, ticket.BetContent, ticket.Amount, ticket.IsAppend, ticket.IssuseNumber, winNumber, pool
                    , out preTaxBonusMoney, out afterTaxBonusMoney);

                //totalPreBonusMoney += preTaxBonusMoney;
                //totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }
            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
            logList.Add(string.Format("票数据：{0}条，奖金：{1}元", prizeList.Count, totalAfterBonusMoney));

            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));


            watch.Restart();
            //批量更新数据库
            logList.Add("批量更新数据库");
            BusinessHelper.UpdateTicketBonus(prizeList);
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            //订单派奖
            watch.Restart();
            //订单派奖
            logList.Add("执行订单派奖");
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        #endregion

        #region 数字彩派奖

        public static string PrizeOrder_SZC(string gameCode, int pageSize, int doMaxCount)
        {
            IDatabase db = null;
            if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_DP;
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_GP;
            if (db == null)
                return string.Format("找不到{0}对应的库", gameCode);

            var watch_total = new Stopwatch();
            var watch = new Stopwatch();
            var logList = new List<string>();
            watch_total.Start();
            watch.Start();
            //查找比赛结果
            var szcMatchResult = RedisMatchBusiness.QuserySZCBonusPool(gameCode);
            var szcWinNumber = RedisMatchBusiness.QuerySZCWinNumber(gameCode);
            watch.Stop();
            logList.Add(string.Format("查询{0}中奖规则、开奖号用时{1}毫秒", gameCode, watch.Elapsed.TotalMilliseconds));
            var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);

            //找出数字彩运行中的订单
            var pageIndex = 0;
            //取出近N期的开奖号
            foreach (var winNumberItem in szcWinNumber)
            {
                var currentIssuseNumber = winNumberItem.Key;
                var currentWinNumber = winNumberItem.Value;
                if (string.IsNullOrEmpty(currentWinNumber))
                    continue;

                //查找奖池结果
                var poolList = szcMatchResult.Where(p => p.IssuseNumber == currentIssuseNumber && p.BonusMoney > 0).ToList();
                //彩种_Key_期号 完整Key值
                var currentKey = string.Format("{0}_{1}", fullKey, currentIssuseNumber);
                logList.Add(string.Format("对{0}第{1}期的订单派奖,key:{2}", gameCode, currentIssuseNumber, currentKey));
                var doOrderCount = 0;
                while (true)
                {
                    #region 处理一页的订单

                    //var runingOrderListJson = db.ListRangeAsync(currentKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                    var runingOrderListJson = db.ListRangeAsync(currentKey).Result;
                    //if (runingOrderListJson.Length <= 0)
                    //    break;
                    logList.Add(string.Format("当前页订单{0}条", runingOrderListJson.Length));
                    foreach (var item in runingOrderListJson)
                    {
                        try
                        {
                            if (!item.HasValue)
                            {
                                //删除数字彩运行中的订单
                                db.ListRemoveAsync(currentKey, item);
                                continue;
                            }

                            var json = item.ToString();
                            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                            if (orderInfo == null)
                                throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                            logList.Add(PrizeOrder_SZC(orderInfo, currentWinNumber, poolList));
                            //删除数字彩运行中的订单
                            db.ListRemoveAsync(currentKey, item);
                            doOrderCount++;
                        }
                        catch (Exception ex)
                        {
                            //writer.Write("RedisOrderBusiness", "PrizeOrder_SZC  " + gameCode, ex);
                            logList.Add(ex.ToString());
                        }
                    }

                    #endregion

                    pageIndex++;
                    //最后一页
                    if (runingOrderListJson.Length < pageSize)
                        break;
                    if (doOrderCount > doMaxCount)
                        break;
                }
            }

            watch_total.Stop();
            logList.Add(string.Format("本次共派奖订单，总用时{0}毫秒", watch_total.Elapsed.TotalMilliseconds));
            return string.Join(Environment.NewLine, logList);
        }

        public static string PrizeOrder_SZC_ByKey(string gameCode, string currentIssuseNumber, string fullKey)
        {
            IDatabase db = null;
            if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_DP;
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_GP;
            if (db == null)
                return string.Format("找不到{0}对应的库", gameCode);

            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();
            //查找比赛结果
            var szcMatchResult = RedisMatchBusiness.QuserySZCBonusPool(gameCode);
            var szcWinNumber = RedisMatchBusiness.QuerySZCWinNumber(gameCode);
            //watch.Stop();
            //查找奖池结果
            var poolList = szcMatchResult.Where(p => p.IssuseNumber == currentIssuseNumber && p.BonusMoney > 0).ToList();
            //查找开奖号
            var currentWinNumber = szcWinNumber.Where(p => p.Key == currentIssuseNumber).FirstOrDefault().Value;
            if (string.IsNullOrEmpty(currentWinNumber))
                return string.Format("{0}第{1}期开奖号为空", gameCode, currentIssuseNumber);

            //logList.Add(string.Format("查询{0}中奖规则、开奖号用时{1}毫秒", gameCode, watch.Elapsed.TotalMilliseconds));

            var runingOrderListJson = db.ListRangeAsync(fullKey).Result;
            var orderCount = runingOrderListJson.Length;
            logList.Add(string.Format("对{0}第{1}期的订单派奖,key:{2}，订单{3}条", gameCode, currentIssuseNumber, fullKey, orderCount));

            #region 处理列表中的订单

            foreach (var item in runingOrderListJson)
            {
                try
                {
                    if (!item.HasValue)
                    {
                        //删除数字彩运行中的订单
                        db.ListRemoveAsync(fullKey, item);
                        continue;
                    }

                    var json = item.ToString();
                    var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                    if (orderInfo == null)
                        throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                    logList.Add(PrizeOrder_SZC(orderInfo, currentWinNumber, poolList));
                    //删除数字彩运行中的订单
                    db.ListRemoveAsync(fullKey, item);
                }
                catch (Exception ex)
                {
                    logList.Add(ex.ToString());
                }
            }

            #endregion

            watch.Stop();
            logList.Add(string.Format("共派奖订单{0}条，用时{1}毫秒", orderCount, watch.Elapsed.TotalMilliseconds));

            return string.Join(Environment.NewLine, logList);
        }

        public static string PrizeOrder_SZC_Page(string gameCode, string currentIssuseNumber, string fullKey, string runingOrderList)
        {
            IDatabase db = null;
            if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_DP;
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_GP;
            if (db == null)
                return string.Format("找不到{0}对应的库", gameCode);

            //查找比赛结果
            var szcMatchResult = RedisMatchBusiness.QuserySZCBonusPool(gameCode);
            var szcWinNumber = RedisMatchBusiness.QuerySZCWinNumber(gameCode);
            //查找奖池结果
            var poolList = szcMatchResult.Where(p => p.IssuseNumber == currentIssuseNumber && p.BonusMoney > 0).ToList();
            //查找开奖号
            var currentWinNumber = szcWinNumber.Where(p => p.Key == currentIssuseNumber).FirstOrDefault().Value;
            if (string.IsNullOrEmpty(currentWinNumber))
                return string.Format("{0}第{1}期开奖号为空", gameCode, currentIssuseNumber);

            var runingOrderListJson = runingOrderList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var orderCount = runingOrderListJson.Length;
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();
            foreach (var item in runingOrderListJson)
            {
                try
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        //删除数字彩运行中的订单
                        db.ListRemoveAsync(fullKey, item);
                        continue;
                    }

                    var json = item.ToString();
                    var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(json);
                    if (orderInfo == null)
                        throw new Exception(string.Format("json:{0}在反序列化后为空", json));
                    logList.Add(PrizeOrder_SZC(orderInfo, currentWinNumber, poolList));
                    //删除数字彩运行中的订单
                    db.ListRemoveAsync(fullKey, item);
                }
                catch (Exception ex)
                {
                    logList.Add(ex.ToString());
                }
            }
            watch.Stop();
            logList.Add(string.Format("共派奖订单{0}条，用时{1}毫秒", orderCount, watch.Elapsed.TotalMilliseconds));

            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_SZC(RedisOrderInfo info, string winNumber, List<Ticket_BonusPool> pool)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            logList.Add(string.Format("开始计算{0}的中奖奖金,keyLine为：{1}", info.SchemeId, info.KeyLine));
            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            var gamecode = string.Empty;
            foreach (var ticket in info.TicketList)
            {
                gamecode = ticket.GameCode;
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;

                DoComputeTicketBonusMoney(ticket.TicketId, ticket.GameCode, ticket.GameType, ticket.BetContent, ticket.Amount, ticket.IsAppend, ticket.IssuseNumber, winNumber, pool
                    , out preTaxBonusMoney, out afterTaxBonusMoney);

                //totalPreBonusMoney += preTaxBonusMoney;
                //totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }

            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);
            //logList.Add(string.Format("票数据：{0}条，奖金：{1}元", prizeList.Count, totalAfterBonusMoney));

            //watch.Stop();
            //logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            //watch.Restart();
            //批量更新数据库
            //logList.Add("批量更新数据库");
            BusinessHelper.UpdateTicketBonus(prizeList);
            if (totalPreBonusMoney < 0M)
                totalPreBonusMoney = -1;
            if (totalAfterBonusMoney < 0M)
                totalAfterBonusMoney = -1;
            //watch.Stop();
            //logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            //watch.Restart();
            //订单派奖
            //logList.Add("执行订单派奖");
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);
            //处理追号
            //if (!string.IsNullOrEmpty(info.KeyLine))
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, gamecode);
            if (info.SchemeType == SchemeType.ChaseBetting)
            {
                logList.Add(string.Format("订单{0}是追号订单,keyLine为:{1}", info.SchemeId, info.KeyLine));

                var db = RedisHelper.DB_Chase_Order;
                //取出追号订单列表中的下一个订单
                var nextOrder = db.ListLeftPopAsync(info.KeyLine).Result;
                if (!nextOrder.HasValue)
                {
                    logList.Add("追号已完成,已删除追号库中key:" + info.KeyLine);
                    //说明追号订单已运行完成，删除
                    db.KeyDeleteAsync(info.KeyLine);
                }
                else
                {
                    var order = JsonSerializer.Deserialize<RedisWaitTicketOrder>(nextOrder);
                    if (order.StopAfterBonus && totalAfterBonusMoney > 0M)
                    {
                        //中奖后停止
                        db.KeyDeleteAsync(info.KeyLine);
                        //------------------------------------------------------------------------------
                        //20171203修改 单独的RedisKeys.Key_Waiting_Chase_Order_List redis2号库中是没有的
                        //db.ListRemoveAsync(RedisKeys.Key_Waiting_Chase_Order_List, info.KeyLine);
                        db.ListRemoveAsync(fullKey, info.KeyLine);
                        //------------------------------------------------------------------------------
                        logList.Add("中奖后停止,已删除追号库中key和" + fullKey + "中key:" + info.KeyLine);
                    }
                    else
                    {
                        //继续追号
                        order.RunningOrder.CanChase = true;
                        var json = JsonSerializer.Serialize<RedisWaitTicketOrder>(order);
                        db.ListLeftPushAsync(info.KeyLine, json);
                        logList.Add("中奖后继续追号");
                    }
                }
            }

            watch.Stop();
            logList.Add(string.Format("执行订单{0}派奖用时：{1}毫秒", info.SchemeId, watch.Elapsed.TotalMilliseconds));

            return string.Join(Environment.NewLine, logList);
        }

        #endregion

        /// <summary>
        /// 传统足球、数字彩票派奖
        /// </summary>
        public static void DoComputeTicketBonusMoney(string ticketId, string gameCode, string gameType, string betContent, int amount, bool isAppend, string issuseNumber, string winNumber, List<Ticket_BonusPool> poolList, out decimal totalPreMoney, out decimal totalAfterMoney)
        {
            totalPreMoney = 0M;
            totalAfterMoney = 0M;
            //开奖号为空或开奖号为-，说明当期无开奖结果
            if (string.IsNullOrEmpty(winNumber) || winNumber == "-")
            {
                totalPreMoney = -1M;
                totalAfterMoney = -1M;
                return;
            }
            var bonusRuleList = RedisMatchBusiness.QuerySZCBonusRule();
            foreach (var item in betContent.Split('/'))
            {
                var ticketPreMoney = 0M;
                var ticketAfterMoney = 0M;
                var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, gameType);
                var bonusLevelList = analyzer.CaculateBonus(item, winNumber);
                //HCount += AnalyzerFactory.GetHitCount(item, winNumber);
                foreach (var level in bonusLevelList)
                {
                    //取中奖规则
                    var rule = bonusRuleList.FirstOrDefault(p => p.GameCode == gameCode && p.GameType == gameType && p.BonusGrade == level);
                    if (rule == null)
                        throw new Exception(string.Format("票{0}找不到对应的奖金规则{1}", ticketId, level));
                    var money = rule.BonusMoney;
                    if (money == -1M)
                    {
                        var pGameType = (gameCode == "SSQ" || gameCode == "DLT") ? "" : gameType;
                        var pool = poolList.FirstOrDefault(p => p.GameCode == gameCode
                                                        && (pGameType == "" || p.GameType == pGameType)
                                                        && p.IssuseNumber == issuseNumber
                                                        && p.BonusLevel == level.ToString());
                        if (pool == null)
                            throw new Exception("未更新奖池数据 - " + gameCode + "." + gameType);
                        money = pool.BonusMoney;
                    }
                    if (money <= 0M)
                        throw new Exception(string.Format("{0} {1}奖金为0", gameCode, gameType));

                    //追加投注
                    var appendMoney = 0M;
                    if (level <= 3 && isAppend)
                        appendMoney = money * 0.6M;
                    else if (level > 3 && level <= 5 && isAppend)
                        appendMoney = money * 0.5M;

                    //var dltAddMoneyIssuseNumber = new string[] { "2016-057", "2016-058", "2016-059", "2016-060", "2016-061", "2016-062" };
                    //if (dltAddMoneyIssuseNumber.Contains(issuseNumber))
                    //{
                    //    //加奖活动
                    //    if (isAppend)
                    //    {
                    //        if (level == 4)
                    //        {
                    //            money += 100M;
                    //        }
                    //        if (level == 5 || level == 6)
                    //        {
                    //            money += 5M;
                    //        }
                    //    }
                    //}

                    var bonusMoney = (money + appendMoney) * amount;
                    ticketPreMoney += bonusMoney;
                    var taxMoney = 0M;
                    //扣税
                    if ((money + appendMoney) >= 10000M)
                    {
                        taxMoney = (money + appendMoney) * 0.2M * amount;
                    }
                    ticketAfterMoney += bonusMoney - taxMoney;
                }

                totalPreMoney += ticketPreMoney;
                totalAfterMoney += ticketAfterMoney;
            }
        }

        #region 欧洲杯派奖

        public static string PrizeOrder_OZB(int pageSize, int doMaxCount)
        {
            var logList = new List<string>();
            var db = RedisHelper.DB_Running_Order_SCZ_DP;
            var ozbWinNumber = RedisMatchBusiness.QueryOZBWinNumber();
            var key_GJ = ozbWinNumber.FirstOrDefault(p => p.Key == "GJ");
            var key_GYJ = ozbWinNumber.FirstOrDefault(p => p.Key == "GYJ");
            var gjWinNumber = key_GJ.Value;
            var gyjWinNumber = key_GYJ.Value;
            if (!string.IsNullOrEmpty(gjWinNumber))
            {
                //冠军派奖
                var key1 = string.Format("OZB_{0}_Running_Order_List_2016", "GJ");
                logList.Add(DoPrizeOrder_OZB_By_Key(key1, pageSize, gjWinNumber, doMaxCount));
                var key2 = string.Format("OZB_{0}_Running_Order_List_2016", "冠军");
                logList.Add(DoPrizeOrder_OZB_By_Key(key2, pageSize, gjWinNumber, doMaxCount));
            }
            if (!string.IsNullOrEmpty(gyjWinNumber))
            {
                //冠亚军派奖
                var key1 = string.Format("OZB_{0}_Running_Order_List_2016", "GYJ");
                logList.Add(DoPrizeOrder_OZB_By_Key(key1, pageSize, gyjWinNumber, doMaxCount));
                var key2 = string.Format("OZB_{0}_Running_Order_List_2016", "冠亚军");
                logList.Add(DoPrizeOrder_OZB_By_Key(key2, pageSize, gyjWinNumber, doMaxCount));
            }

            //foreach (var item in szcWinNumber)
            //{
            //    var key = string.Format("OZB_{0}_Running_Order_List_2016", item.Key);
            //    var winNumber = item.Value;
            //    if (string.IsNullOrEmpty(winNumber))
            //        continue;

            //    logList.Add(DoPrizeOrder_OZB_By_Key(key, pageSize, winNumber, doMaxCount));
            //}

            return string.Join(Environment.NewLine, logList);
        }

        private static string DoPrizeOrder_OZB_By_Key(string fullKey, int pageSize, string winNumber, int doMaxCount)
        {
            var logList = new List<string>();
            var db = RedisHelper.DB_Running_Order_SCZ_DP;
            var pageIndex = 0;
            var watch = new Stopwatch();
            var doOrderCount = 0;
            while (true)
            {
                #region 处理一页订单

                var runingOrderListJson = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                if (runingOrderListJson.Length <= 0)
                    break;
                foreach (var item in runingOrderListJson)
                {
                    var orderId = string.Empty;
                    try
                    {
                        if (!item.HasValue)
                        {
                            //删除数字彩运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            continue;
                        }
                        try
                        {
                            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(item);
                            orderId = orderInfo.SchemeId;
                            watch.Restart();
                            logList.Add(PrizeOrder_OZB(orderInfo, winNumber));
                        }
                        catch (Exception ex)
                        {
                            logList.Add(ex.ToString());
                        }
                        finally
                        {
                            watch.Stop();
                            logList.Add(string.Format("订单{0}派奖用时{1}毫秒", orderId, watch.Elapsed.TotalMilliseconds));

                            //删除数字彩运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            doOrderCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        logList.Add(ex.ToString());
                    }
                }

                #endregion
                pageIndex++;
                //最后一页
                if (runingOrderListJson.Length < pageSize)
                    break;
                if (doOrderCount > doMaxCount)
                    break;
            }
            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_OZB(RedisOrderInfo info, string winNumber)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;

                DoComputeOZBTicketBonusMoney(ticket.GameCode, ticket.GameType, ticket.BetContent, ticket.LocOdds, ticket.Amount, winNumber, out preTaxBonusMoney, out afterTaxBonusMoney);

                totalPreBonusMoney += preTaxBonusMoney;
                totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }

            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);

            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //批量更新数据库
            BusinessHelper.UpdateTicketBonus(prizeList);
            if (totalPreBonusMoney < 0M)
                totalPreBonusMoney = -1;
            if (totalAfterBonusMoney < 0M)
                totalAfterBonusMoney = -1;
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //订单派奖
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);

            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            return string.Join(Environment.NewLine, logList);
        }

        private static void DoComputeOZBTicketBonusMoney(string gameCode, string gameType, string betContent, string locOdds, int amount, string winNumber,
            out decimal totalPreMoney, out decimal totalAfterMoney)
        {
            totalPreMoney = 0M;
            totalAfterMoney = 0M;
            //开奖号为空或开奖号为-，说明当期无开奖结果
            if (string.IsNullOrEmpty(winNumber) || winNumber == "-")
            {
                totalPreMoney = -1M;
                totalAfterMoney = -1M;
                return;
            }
            var bonusManager = new Ticket_BonusManager();
            var ticketPreMoney = 0M;
            var ticketAfterMoney = 0M;
            var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, gameType);
            var bonusLevelList = analyzer.CaculateBonus(betContent, winNumber);
            if (bonusLevelList.Count > 0)
            {
                //中奖
                var oddArray = locOdds.Split('/');
                var tag = string.Format("{0}|", winNumber);
                var bonusOddStr = oddArray.FirstOrDefault(p => p.StartsWith(tag));
                if (!string.IsNullOrEmpty(bonusOddStr))
                {
                    var odds = decimal.Parse(bonusOddStr.Replace(tag, string.Empty));
                    ticketPreMoney = new SMGBonus().FourToSixHomesInFive(odds * 2M);
                    ticketAfterMoney = ticketPreMoney;
                    if (ticketAfterMoney >= 10000)
                    {
                        ticketAfterMoney = ticketAfterMoney * (1M - 0.2M);
                    }
                    ticketPreMoney *= amount;
                    ticketAfterMoney *= amount;
                }
            }
            totalPreMoney += ticketPreMoney;
            totalAfterMoney += ticketAfterMoney;
        }


        #endregion


        #region 世界杯派奖

        public static string PrizeOrder_SJB(int pageSize, int doMaxCount)
        {
            var logList = new List<string>();
            var db = RedisHelper.DB_Running_Order_SCZ_DP;
            var sjbWinNumber = RedisMatchBusiness.QuerySJBWinNumber();
            var key_GJ = sjbWinNumber.FirstOrDefault(p => p.Key == "GJ");
            var key_GYJ = sjbWinNumber.FirstOrDefault(p => p.Key == "GYJ");
            var gjWinNumber = key_GJ.Value;
            var gyjWinNumber = key_GYJ.Value;
            if (!string.IsNullOrEmpty(gjWinNumber))
            {
                //冠军派奖
                var key1 = string.Format("SJB_{0}_Running_Order_List_2018", "GJ");
                logList.Add(DoPrizeOrder_SJB_By_Key(key1, pageSize, gjWinNumber, doMaxCount));
                var key2 = string.Format("SJB_{0}_Running_Order_List_2018", "冠军");
                logList.Add(DoPrizeOrder_SJB_By_Key(key2, pageSize, gjWinNumber, doMaxCount));
            }
            if (!string.IsNullOrEmpty(gyjWinNumber))
            {
                //冠亚军派奖
                var key1 = string.Format("SJB_{0}_Running_Order_List_2018", "GYJ");
                logList.Add(DoPrizeOrder_SJB_By_Key(key1, pageSize, gyjWinNumber, doMaxCount));
                var key2 = string.Format("SJB_{0}_Running_Order_List_2018", "冠亚军");
                logList.Add(DoPrizeOrder_SJB_By_Key(key2, pageSize, gyjWinNumber, doMaxCount));
            }

            //foreach (var item in szcWinNumber)
            //{
            //    var key = string.Format("OZB_{0}_Running_Order_List_2016", item.Key);
            //    var winNumber = item.Value;
            //    if (string.IsNullOrEmpty(winNumber))
            //        continue;

            //    logList.Add(DoPrizeOrder_OZB_By_Key(key, pageSize, winNumber, doMaxCount));
            //}

            return string.Join(Environment.NewLine, logList);
        }

        private static string DoPrizeOrder_SJB_By_Key(string fullKey, int pageSize, string winNumber, int doMaxCount)
        {
            var logList = new List<string>();
            var db = RedisHelper.DB_Running_Order_SCZ_DP;
            var pageIndex = 0;
            var watch = new Stopwatch();
            var doOrderCount = 0;
            while (true)
            {
                #region 处理一页订单

                var runingOrderListJson = db.ListRangeAsync(fullKey, pageIndex * pageSize, (pageIndex + 1) * pageSize - 1).Result;
                if (runingOrderListJson.Length <= 0)
                    break;
                foreach (var item in runingOrderListJson)
                {
                    var orderId = string.Empty;
                    try
                    {
                        if (!item.HasValue)
                        {
                            //删除数字彩运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            continue;
                        }
                        try
                        {
                            var orderInfo = JsonSerializer.Deserialize<RedisOrderInfo>(item);
                            orderId = orderInfo.SchemeId;
                            watch.Restart();
                            logList.Add(PrizeOrder_SJB(orderInfo, winNumber));
                        }
                        catch (Exception ex)
                        {
                            logList.Add(ex.ToString());
                        }
                        finally
                        {
                            watch.Stop();
                            logList.Add(string.Format("订单{0}派奖用时{1}毫秒", orderId, watch.Elapsed.TotalMilliseconds));

                            //删除数字彩运行中的订单
                            db.ListRemoveAsync(fullKey, item);
                            doOrderCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        logList.Add(ex.ToString());
                    }
                }

                #endregion
                pageIndex++;
                //最后一页
                if (runingOrderListJson.Length < pageSize)
                    break;
                if (doOrderCount > doMaxCount)
                    break;
            }
            return string.Join(Environment.NewLine, logList);
        }

        private static string PrizeOrder_SJB(RedisOrderInfo info, string winNumber)
        {
            var logList = new List<string>();
            var watch = new Stopwatch();
            watch.Start();

            var prizeList = new List<TicketBatchPrizeInfo>();
            var totalPreBonusMoney = 0M;
            var totalAfterBonusMoney = 0M;
            foreach (var ticket in info.TicketList)
            {
                var preTaxBonusMoney = 0M;
                var afterTaxBonusMoney = 0M;

                DoComputeSJBTicketBonusMoney(ticket.GameCode, ticket.GameType, ticket.BetContent, ticket.LocOdds, ticket.Amount, winNumber, out preTaxBonusMoney, out afterTaxBonusMoney);

                totalPreBonusMoney += preTaxBonusMoney;
                totalAfterBonusMoney += afterTaxBonusMoney;
                //更新票数据sql
                prizeList.Add(new TicketBatchPrizeInfo
                {
                    TicketId = ticket.TicketId,
                    BonusStatus = afterTaxBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose,
                    PreMoney = preTaxBonusMoney,
                    AfterMoney = afterTaxBonusMoney,
                });
            }

            prizeList = prizeList.Distinct(new TicketBatchPrizeInfo_Comparer()).ToList();
            totalPreBonusMoney = prizeList.Sum(p => p.PreMoney);
            totalAfterBonusMoney = prizeList.Sum(p => p.AfterMoney);

            watch.Stop();
            logList.Add(string.Format("执行票派奖计算用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //批量更新数据库
            BusinessHelper.UpdateTicketBonus(prizeList);
            if (totalPreBonusMoney < 0M)
                totalPreBonusMoney = -1;
            if (totalAfterBonusMoney < 0M)
                totalAfterBonusMoney = -1;
            watch.Stop();
            logList.Add(string.Format("更新票中奖数据用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            watch.Restart();
            //订单派奖
            BusinessHelper.DoOrderPrize(info.SchemeId, totalAfterBonusMoney > 0M ? BonusStatus.Win : BonusStatus.Lose, totalPreBonusMoney, totalAfterBonusMoney);

            watch.Stop();
            logList.Add(string.Format("执行订单派奖用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            return string.Join(Environment.NewLine, logList);
        }

        private static void DoComputeSJBTicketBonusMoney(string gameCode, string gameType, string betContent, string locOdds, int amount, string winNumber,
            out decimal totalPreMoney, out decimal totalAfterMoney)
        {
            totalPreMoney = 0M;
            totalAfterMoney = 0M;
            //开奖号为空或开奖号为-，说明当期无开奖结果
            if (string.IsNullOrEmpty(winNumber) || winNumber == "-")
            {
                totalPreMoney = -1M;
                totalAfterMoney = -1M;
                return;
            }
            var bonusManager = new Ticket_BonusManager();
            var ticketPreMoney = 0M;
            var ticketAfterMoney = 0M;
            var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gameCode, gameType);
            var bonusLevelList = analyzer.CaculateBonus(betContent, winNumber);
            if (bonusLevelList.Count > 0)
            {
                //中奖
                var oddArray = locOdds.Split('/');
                var tag = string.Format("{0}|", winNumber);
                var bonusOddStr = oddArray.FirstOrDefault(p => p.StartsWith(tag));
                if (!string.IsNullOrEmpty(bonusOddStr))
                {
                    var odds = decimal.Parse(bonusOddStr.Replace(tag, string.Empty));
                    ticketPreMoney = new SMGBonus().FourToSixHomesInFive(odds * 2M);
                    ticketAfterMoney = ticketPreMoney;
                    if (ticketAfterMoney >= 10000)
                    {
                        ticketAfterMoney = ticketAfterMoney * (1M - 0.2M);
                    }
                    ticketPreMoney *= amount;
                    ticketAfterMoney *= amount;
                }
            }
            totalPreMoney += ticketPreMoney;
            totalAfterMoney += ticketAfterMoney;
        }


        #endregion
    }

}
