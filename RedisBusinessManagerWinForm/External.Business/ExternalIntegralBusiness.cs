using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using GameBiz.Core;
using Common;
using External.Business.Domain.Managers.Agent;
using GameBiz.Domain.Managers;
using GameBiz.Domain.Entities;
using Common.Cryptography;
using External.Core.Login;
using Common.Net;
using Common.Utilities;
using External.Domain.Managers.Login;
using External.Domain.Managers.Authentication;

namespace External.Business
{
    /// <summary>
    /// 代理积分业务
    /// </summary>
    public class ExternalIntegralBusiness
    {
        /// <summary>
        /// 代理转入积分
        /// </summary>
        public string AgentFillMoney(string userId, decimal fillMoney)
        {
            var orderId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                orderId = BusinessHelper.GetUserFillMoneyId();

                var fundManager = new FundManager();
                var balanceManager = new UserBalanceManager();
                var userBalance = balanceManager.QueryUserBalance(userId);
                if (userBalance == null)
                    throw new Exception("用户不存在");
                if (userBalance.CommissionBalance < fillMoney)
                    throw new Exception("用户返点不足");

                #region 返点消费
                var payDetailList = new List<PayDetail>();
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.Commission,
                    PayMoney = fillMoney,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = BusinessHelper.FundCategory_IntergralPayOut,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.Commission,
                    PayMoney = fillMoney,
                    PayType = PayType.Payout,
                    Summary = string.Format("返点转入{0:N2}", fillMoney),
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance - fillMoney,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance -= fillMoney;

                #endregion

                #region 充值账户充值

                fundManager.AddFillMoney(new FillMoney
                {
                    AgentId = string.Empty,
                    DeliveryAddress = string.Empty,
                    FillMoneyAgent = FillMoneyAgentType.QuDao,
                    GoodsDescription = string.Empty,
                    GoodsName = string.Empty,
                    GoodsType = string.Empty,
                    IsNeedDelivery = string.Empty,
                    NotifyUrl = string.Empty,
                    OrderId = orderId,
                    OuterFlowId = string.Empty,
                    PayMoney = fillMoney,
                    RequestBy = userId,
                    RequestExtensionInfo = string.Empty,
                    RequestMoney = fillMoney,
                    RequestTime = DateTime.Now,
                    ResponseBy = userId,
                    ResponseCode = string.Empty,
                    ResponseMessage = BusinessHelper.FundCategory_IntegralFillMoney,
                    ResponseMoney = fillMoney,
                    ResponseTime = DateTime.Now,
                    ReturnUrl = string.Empty,
                    SchemeSource = SchemeSource.Web,
                    ShowUrl = string.Empty,
                    UserId = userId,
                    Status = FillMoneyStatus.Success
                });

                fundManager.AddFundDetail(new FundDetail
                {
                    AccountType = AccountType.FillMoney,
                    Category = BusinessHelper.FundCategory_IntegralFillMoney,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AgentId = string.Empty,
                    OperatorId = string.Empty,
                    PayMoney = fillMoney,
                    PayType = PayType.Payin,
                    Summary = string.Format("渠道充值{0:N2}元", fillMoney),
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + fillMoney
                });
                //userBalance.FillMoneyBalance += fillMoney;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = fillMoney,
                    PayType = PayType.Payin,
                });

                #endregion

                //balanceManager.UpdateUserBalance(userBalance);
                balanceManager.PayToUserBalance(userId, payDetailList.ToArray());

                biz.CommitTran();
            }

            return orderId;
        }

        /// <summary>
        /// 代理转入返点
        /// </summary>
        public string AgentCPSFillMoney(string userId, decimal fillMoney)
        {
            var orderId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                orderId = BusinessHelper.GetUserFillMoneyId();

                var fundManager = new FundManager();
                var balanceManager = new UserBalanceManager();
                var userBalance = balanceManager.QueryUserBalance(userId);
                if (userBalance == null)
                    throw new Exception("用户不存在");
                if (userBalance.CPSBalance < fillMoney)
                    throw new Exception("用户返点不足");

                var payDetailList = new List<PayDetail>();

                #region 返点消费

                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.CPS,
                    PayMoney = fillMoney,
                    PayType = PayType.Payout,
                });
                fundManager.AddFundDetail(new FundDetail
                {
                    Category = BusinessHelper.FundCategory_CPSPayOut,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = AccountType.CPS,
                    PayMoney = fillMoney,
                    PayType = PayType.Payout,
                    Summary = string.Format("返点转入{0:N2}", fillMoney),
                    UserId = userId,
                    BeforeBalance = userBalance.CPSBalance,
                    AfterBalance = userBalance.CPSBalance - fillMoney,
                    OperatorId = userId,
                });
                //userBalance.CPSBalance -= fillMoney;

                #endregion

                #region 充值账户充值

                fundManager.AddFillMoney(new FillMoney
                {
                    AgentId = string.Empty,
                    DeliveryAddress = string.Empty,
                    FillMoneyAgent = FillMoneyAgentType.QuDao,
                    GoodsDescription = string.Empty,
                    GoodsName = string.Empty,
                    GoodsType = string.Empty,
                    IsNeedDelivery = string.Empty,
                    NotifyUrl = string.Empty,
                    OrderId = orderId,
                    OuterFlowId = string.Empty,
                    PayMoney = fillMoney,
                    RequestBy = userId,
                    RequestExtensionInfo = string.Empty,
                    RequestMoney = fillMoney,
                    RequestTime = DateTime.Now,
                    ResponseBy = userId,
                    ResponseCode = string.Empty,
                    ResponseMessage = BusinessHelper.FundCategory_CPSFillMoney,
                    ResponseMoney = fillMoney,
                    ResponseTime = DateTime.Now,
                    ReturnUrl = string.Empty,
                    SchemeSource = SchemeSource.Web,
                    ShowUrl = string.Empty,
                    UserId = userId,
                    Status = FillMoneyStatus.Success
                });

                fundManager.AddFundDetail(new FundDetail
                {
                    AccountType = AccountType.FillMoney,
                    Category = BusinessHelper.FundCategory_CPSFillMoney,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AgentId = string.Empty,
                    OperatorId = string.Empty,
                    PayMoney = fillMoney,
                    PayType = PayType.Payin,
                    Summary = string.Format("CPS充值{0:N2}元", fillMoney),
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + fillMoney
                });
                //userBalance.FillMoneyBalance += fillMoney;
                payDetailList.Add(new PayDetail
                {
                    AccountType = AccountType.FillMoney,
                    PayMoney = fillMoney,
                    PayType = PayType.Payin,
                });

                #endregion

                //balanceManager.UpdateUserBalance(userBalance);
                balanceManager.PayToUserBalance(userId, payDetailList.ToArray());

                biz.CommitTran();
            }

            return orderId;
        }

        public UserFundDetailCollection QueryUserFundDetail(string userId, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, string payTypeList, int pageIndex, int pageSize)
        {
            int totalPayinCount;
            decimal totalPayinMoney;
            int totalPayoutCount;
            decimal totalPayoutMoney;
            var collection = new UserFundDetailCollection();

            List<AccountType> acList = new List<AccountType>();
            if (!string.IsNullOrEmpty(accountTypeList))
            {
                foreach (var item in accountTypeList.Split('|'))
                {
                    acList.Add((AccountType)int.Parse(item));
                }
            }
            List<string> cayList = new List<string>();
            if (!string.IsNullOrEmpty(categoryList))
            {
                foreach (var item in categoryList.Split('|'))
                {
                    cayList.Add(item);
                }
            }
            List<PayType> paytypeList = new List<PayType>();
            if (!string.IsNullOrEmpty(payTypeList))
            {
                foreach (var item in payTypeList.Split('|'))
                {
                    paytypeList.Add((PayType)int.Parse(item));
                }
            }
            collection.FundDetailList = new IntegralManager().QueryUserFundDetail(userId, fromDate, toDate, acList, cayList, paytypeList, pageIndex, pageSize
                , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
            collection.TotalPayinCount = totalPayinCount;
            collection.TotalPayinMoney = totalPayinMoney;
            collection.TotalPayoutCount = totalPayoutCount;
            collection.TotalPayoutMoney = totalPayoutMoney;
            return collection;
        }
        public UserFundDetailCollection QueryUserFundDetail_CPS(string userId, DateTime fromDate, DateTime toDate, string accountTypeList, string payTypeList, int pageIndex, int pageSize)
        {
            int totalPayinCount;
            decimal totalPayinMoney;
            int totalPayoutCount;
            decimal totalPayoutMoney;
            var collection = new UserFundDetailCollection();

            List<AccountType> acList = new List<AccountType>();
            if (!string.IsNullOrEmpty(accountTypeList))
            {
                foreach (var item in accountTypeList.Split('|'))
                {
                    acList.Add((AccountType)int.Parse(item));
                }
            }
            List<PayType> paytypeList = new List<PayType>();
            if (!string.IsNullOrEmpty(payTypeList))
            {
                foreach (var item in payTypeList.Split('|'))
                {
                    paytypeList.Add((PayType)int.Parse(item));
                }
            }
            collection.FundDetailList = new IntegralManager().QueryUserFundDetail_CPS(userId, fromDate, toDate, acList, paytypeList, pageIndex, pageSize
                , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
            collection.TotalPayinCount = totalPayinCount;
            collection.TotalPayinMoney = totalPayinMoney;
            collection.TotalPayoutCount = totalPayoutCount;
            collection.TotalPayoutMoney = totalPayoutMoney;
            return collection;
        }
        /// <summary>
        /// 申请提现
        /// 计算申请金额是否可提现，返回计算结果
        /// </summary>
        public CheckWithdrawResult RequestWithdraw_Step1(string userId, decimal requestMoney)
        {
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            var maxTimes = 3;
            var currentTimes = fundManager.QueryTodayWithdrawTimes(userId);
            if (currentTimes >= maxTimes)
                throw new Exception(string.Format("每日只能提取积分{0}次", maxTimes));
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                throw new Exception("用户不存在 - " + userId);
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null)
                throw new Exception("用户帐户不存在 - " + userId);

            //奖金+佣金+名家
            var can_tx_money = userBalance.CommissionBalance;
            //正常提现
            if (can_tx_money >= requestMoney)
            {
                return new CheckWithdrawResult
                {
                    WithdrawCategory = WithdrawCategory.General,
                    RequestMoney = requestMoney,
                    ResponseMoney = requestMoney,
                    Summary = string.Format("申请提取积分{0:N2}，提取成功{0:N2}。", requestMoney),
                };
            }
            return new CheckWithdrawResult
            {
                WithdrawCategory = WithdrawCategory.Error,
                RequestMoney = requestMoney,
                ResponseMoney = requestMoney,
                Summary = string.Format("积分余额不足,申请提取积分{0:N2}，实际积分{0:N2}。", requestMoney, userBalance.CommissionBalance),
            };
        }
        /// <summary>
        /// 申请提现
        /// 实际添加提现记录，扣除用户资金
        /// </summary>
        public void RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string password, out string OrderId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var resonseMoney = 0M;
                var orderId = BusinessHelper.GetWithdrawId();
                OrderId = orderId;
                var category = Payout_To_Frozen_Withdraw(BusinessHelper.FundCategory_IntegralRequestWithdraw, userId, orderId, info.RequestMoney
                      , string.Format("申请提取积分：{0:N2}", info.RequestMoney), "Withdraw", password, out resonseMoney);

                var fundManager = new FundManager();
                fundManager.AddWithdraw(new Withdraw
                {
                    OrderId = orderId,
                    BankCardNumber = info.BankCardNumber,
                    BankCode = info.BankCode,
                    BankName = info.BankName,
                    BankSubName = info.BankSubName,
                    CityName = info.CityName,
                    RequestTime = DateTime.Now,
                    ProvinceName = info.ProvinceName,
                    UserId = userId,
                    RequestMoney = info.RequestMoney,
                    WithdrawAgent = info.WithdrawAgent,
                    Status = WithdrawStatus.Requesting,

                    WithdrawCategory = category,
                    ResponseMoney = resonseMoney,
                });

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 申请提现-CPS
        /// 计算申请金额是否可提现，返回计算结果
        /// </summary>
        public CheckWithdrawResult RequestWithdrawCPS_Step1(string userId, decimal requestMoney)
        {
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            var maxTimes = 3;
            var currentTimes = fundManager.QueryTodayWithdrawTimes(userId);
            if (currentTimes >= maxTimes)
                throw new Exception(string.Format("每日只能提取积分{0}次", maxTimes));
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                throw new Exception("用户不存在 - " + userId);
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null)
                throw new Exception("用户帐户不存在 - " + userId);

            //奖金+佣金+名家
            var can_tx_money = userBalance.CPSBalance;
            //正常提现
            if (can_tx_money >= requestMoney)
            {
                return new CheckWithdrawResult
                {
                    WithdrawCategory = WithdrawCategory.General,
                    RequestMoney = requestMoney,
                    ResponseMoney = requestMoney,
                    Summary = string.Format("申请提取积分{0:N2}，提取成功{0:N2}。", requestMoney),
                };
            }
            return new CheckWithdrawResult
            {
                WithdrawCategory = WithdrawCategory.Error,
                RequestMoney = requestMoney,
                ResponseMoney = requestMoney,
                Summary = string.Format("积分余额不足,申请提取积分{0:N2}，实际积分{0:N2}。", requestMoney, userBalance.CPSBalance),
            };
        }

        /// <summary>
        /// 申请提现-CPS
        /// 实际添加提现记录，扣除用户资金
        /// </summary>
        public void RequestWithdrawCPS_Step2(Withdraw_RequestInfo info, string userId, string password, out string OrderId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var resonseMoney = 0M;
                var orderId = BusinessHelper.GetWithdrawId();
                OrderId = orderId;
                var category = CPS_Payout_To_Frozen_Withdraw(BusinessHelper.FundCategory_CPSRequestWithdraw, userId, orderId, info.RequestMoney
                      , string.Format("CPS申请提取返点：{0:N2}", info.RequestMoney), "Withdraw", password, out resonseMoney);

                var fundManager = new FundManager();
                fundManager.AddWithdraw(new Withdraw
                {
                    OrderId = orderId,
                    BankCardNumber = info.BankCardNumber,
                    BankCode = info.BankCode,
                    BankName = info.BankName,
                    BankSubName = info.BankSubName,
                    CityName = info.CityName,
                    RequestTime = DateTime.Now,
                    ProvinceName = info.ProvinceName,
                    UserId = userId,
                    RequestMoney = info.RequestMoney,
                    WithdrawAgent = info.WithdrawAgent,
                    Status = WithdrawStatus.Requesting,

                    WithdrawCategory = category,
                    ResponseMoney = resonseMoney,
                });

                biz.CommitTran();
            }
        }

        #region 资金明细分类



        #endregion

        #region 资金账户操作


        /// <summary>
        ///  用户支出，申请提现
        /// </summary>
        public static WithdrawCategory Payout_To_Frozen_Withdraw(string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string password, out  decimal responseMoney)
        {
            var requestMoney = payoutMoney;
            if (payoutMoney <= 0M)
                throw new Exception("提取积分必须大于零");
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
                    if (string.IsNullOrEmpty(password) || !userBalance.Password.Equals(Encipherment.MD5(password)))
                    {
                        throw new Exception("资金密码输入错误");
                    }
                }
            }

            var totalMoney = userBalance.CommissionBalance;
            if (totalMoney < payoutMoney)
                throw new Exception(string.Format("用户积分总额小于 {0:N2}。", payoutMoney));

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

            userBalance.FreezeBalance += payoutMoney;
            //balanceManager.UpdateUserBalance(userBalance);
            var payCategory = WithdrawCategory.Acceptable;
            #region 正常提现

            //奖金+佣金+名家
            var currentPayout = 0M;

            if (userBalance.CommissionBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CommissionBalance >= payoutMoney ? payoutMoney : userBalance.CommissionBalance;
                payoutMoney -= currentPayout;

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
                userBalance.CommissionBalance -= currentPayout;
            }

            #endregion

            responseMoney = requestMoney;


            balanceManager.UpdateUserBalance(userBalance);
            return payCategory;
        }

        /// <summary>
        ///  用户支出，申请提现(CPS)
        /// </summary>
        public static WithdrawCategory CPS_Payout_To_Frozen_Withdraw(string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string password, out  decimal responseMoney)
        {
            var requestMoney = payoutMoney;
            if (payoutMoney <= 0M)
                throw new Exception("CPS提取返点必须大于零");
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
                    if (string.IsNullOrEmpty(password) || !userBalance.Password.Equals(Encipherment.MD5(password)))
                    {
                        throw new Exception("资金密码输入错误");
                    }
                }
            }

            var totalMoney = userBalance.CPSBalance;
            if (totalMoney < payoutMoney)
                throw new Exception(string.Format("用户返点总额小于 {0:N2}。", payoutMoney));

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

            userBalance.FreezeBalance += payoutMoney;
            //balanceManager.UpdateUserBalance(userBalance);
            var payCategory = WithdrawCategory.Acceptable;
            #region 正常提现

            //奖金+佣金+名家
            var currentPayout = 0M;

            if (userBalance.CPSBalance > 0M && payoutMoney > 0M)
            {
                //佣金参与支付
                currentPayout = userBalance.CPSBalance >= payoutMoney ? payoutMoney : userBalance.CPSBalance;
                payoutMoney -= currentPayout;

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
                userBalance.CPSBalance -= currentPayout;
            }

            #endregion

            responseMoney = requestMoney;


            balanceManager.UpdateUserBalance(userBalance);
            return payCategory;
        }

        /// <summary>
        /// 用户收入，转到指定账户
        /// </summary>
        public static void Payin_To_Balance(AccountType accountType, string category, string userId, string orderId, decimal payMoney, string summary)
        {
            if (accountType == AccountType.Freeze)
                throw new Exception("退款账户不能为冻结账户");

            if (payMoney <= 0M)
                throw new Exception("转入积分不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

            var before = 0M;
            var after = 0M;
            switch (accountType)
            {
                case AccountType.Commission:
                    before = userBalance.CommissionBalance;
                    after = userBalance.CommissionBalance + payMoney;
                    userBalance.CommissionBalance = after;
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
                OperatorId = userId,
            });

            balanceManager.UpdateUserBalance(userBalance);
        }
        /// <summary>
        /// 用户支出，冻结到结束，清理冻结
        /// 用于完成提现，追号完成
        /// </summary>
        public static void Payout_Frozen_To_End(string category, string userId, string orderId, string summary, decimal clearMoney)
        {
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();

            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            if (userBalance.FreezeBalance < clearMoney)
                throw new Exception(string.Format("冻结积分不足{0:N2}", clearMoney));

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
                BeforeBalance = userBalance.FreezeBalance,
                AfterBalance = userBalance.FreezeBalance - clearMoney,
                OperatorId = userId,
            });
            userBalance.FreezeBalance -= clearMoney;
            balanceManager.UpdateUserBalance(userBalance);
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

            var payBackMoney = payMoney;
            var currentPayBack = 0M;

            var commisionFund = fundList.Where(p => p.AccountType == AccountType.Commission).ToList();
            if (commisionFund != null && commisionFund.Count > 0 && payBackMoney > 0M)
            {
                //佣金金额参与支付，退款到佣金金额
                currentPayBack = payBackMoney >= commisionFund.Sum(p => p.PayMoney) ? commisionFund.Sum(p => p.PayMoney) : payBackMoney;
                payBackMoney -= currentPayBack;
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
                userBalance.CommissionBalance += currentPayBack;
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
            userBalance.FreezeBalance -= payMoney;
            balanceManager.UpdateUserBalance(userBalance);
        }
        /// <summary>
        /// 用户支出
        /// 指定帐户支出
        /// </summary>
        public static void Payout_To_End(AccountType accountType, string category, string userId, string orderId, decimal payoutMoney, string summary)
        {
            if (payoutMoney <= 0M)
                throw new Exception("消费积分不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }
            var manager = new IntegralManager();
            #region 扣除账户金额

            switch (accountType)
            {
                case AccountType.Freeze:
                    var currFreeMoney = manager.GetFreeBalanceByUserId_Agent(userId);
                    if (currFreeMoney < payoutMoney)
                        throw new Exception("账户积分不足");
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
                        OperatorId = userId,
                    });
                    userBalance.FreezeBalance -= payoutMoney;
                    break;
                case AccountType.Commission:
                    if (userBalance.CommissionBalance < payoutMoney)
                        throw new Exception("账户积分不足");
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
                        OperatorId = userId,
                    });
                    userBalance.CommissionBalance -= payoutMoney;
                    break;

                default:
                    break;
            }

            #endregion

            balanceManager.UpdateUserBalance(userBalance);
        }

        #endregion

        public LoginInfo UserLogin(string loginName, string passWord, string loginIP)
        {
            var loginBiz = new LocalLoginBusiness();
            var userBiz = new UserBusiness();
            var loginEntity = loginBiz.Login(loginName, passWord);
            if (loginEntity == null)
            {
                return new LoginInfo { IsSuccess = false, Message = "登录名或密码错误", LoginFrom = "LOCAL", };
            }
            var ocBiz = new OCAgentBusiness();
            var userRebate = ocBiz.QueryUserRebateList(loginEntity.UserId);
            bool isGeneralUser = false;//是否为普通代理用户
            if (loginEntity.Register == null)
                return new LoginInfo { IsSuccess = false, Message = "登录名或密码错误", LoginFrom = "LOCAL", };
            if (!loginEntity.Register.IsAgent)
            {
                //if (userRebate == null || userRebate.Count <= 0||!string.IsNullOrEmpty(loginEntity.AgentId))//如果当前用户没有返点，或者代理商Id不为空的时候不能登录
                if (userRebate == null || userRebate.Count <= 0)
                    return new LoginInfo { IsSuccess = false, Message = "此帐号角色不允许在此登录", LoginFrom = "LOCAL", };
                isGeneralUser = true;
            }
            if (!loginEntity.Register.IsEnable)
            {
                return new LoginInfo { IsSuccess = false, Message = "用户已被禁用", LoginFrom = "LOCAL", };
            }

            var balanceManager = new FundBusiness();
            var userBalance = balanceManager.QueryUserBalance(loginEntity.UserId);
            if (userBalance == null || string.IsNullOrEmpty(userBalance.UserId))
                throw new Exception("未查询到用户账户信息");

            try
            {
                //记录登录日志
                var blogManager = new BlogManager();
                UserLoginHistoryInfo blogInfo = new UserLoginHistoryInfo();
                blogInfo.IpDisplayName = IpManager.GetIpDisplayname_Sina(loginIP).ToString();
                blogInfo.LoginFrom = "LOCAL";
                blogInfo.LoginIp = loginIP;
                blogInfo.LoginTime = DateTime.Now;
                blogInfo.UserId = loginEntity.UserId;
                Blog_UserLoginHistory entity = new Blog_UserLoginHistory();
                ObjectConvert.ConverInfoToEntity(blogInfo, ref entity);
                blogManager.AddBlog_UserLoginHistory(entity);
            }
            catch
            {
            }
            return new LoginInfo
            {
                IsSuccess = true,
                Message = "登录成功",
                CreateTime = loginEntity.CreateTime,
                LoginFrom = "LOCAL",
                RegType = loginEntity.Register.RegType,
                Referrer = loginEntity.Register.Referrer,
                UserId = loginEntity.UserId,
                VipLevel = loginEntity.Register.VipLevel,
                LoginName = loginEntity.LoginName,
                DisplayName = loginEntity.Register.DisplayName,
                AgentId = loginEntity.Register.AgentId,
                IsAgent = loginEntity.Register.IsAgent,
                HideDisplayNameCount = loginEntity.Register.HideDisplayNameCount,
                CommissionBalance = userBalance.CommissionBalance,
                FreezeBalance = userBalance.FreezeBalance,
                IsGeneralUser = isGeneralUser,
            };
        }

        public LoginInfo QueryUserRegister(string userId)
        {
            var loginBiz = new LocalLoginBusiness();
            var loginEntity = loginBiz.GetLocalLoginByUserId(userId);
            if (loginEntity == null)
            {
                return new LoginInfo();
            }
            var ocBiz = new OCAgentBusiness();
            var userRebate = ocBiz.QueryUserRebateList(loginEntity.UserId);
            bool isGeneralUser = false;//是否为普通代理用户
            if (loginEntity.Register == null)
                return new LoginInfo();
            if (!loginEntity.Register.IsAgent)
            {
                //if (userRebate == null || userRebate.Count <= 0||!string.IsNullOrEmpty(loginEntity.AgentId))//如果当前用户没有返点，或者代理商Id不为空的时候不能登录
                if (userRebate == null || userRebate.Count <= 0)
                    isGeneralUser = false;
                else
                    isGeneralUser = true;
            }
            var balanceManager = new FundBusiness();
            var userBalance = balanceManager.QueryUserBalance(loginEntity.UserId);
            if (userBalance == null || string.IsNullOrEmpty(userBalance.UserId))
                throw new Exception("未查询到用户账户信息");

            return new LoginInfo
            {
                IsSuccess = true,
                Message = "查询用户信息成功",
                CreateTime = loginEntity.CreateTime,
                LoginFrom = "LOCAL",
                RegType = loginEntity.Register.RegType,
                Referrer = loginEntity.Register.Referrer,
                UserId = loginEntity.UserId,
                VipLevel = loginEntity.Register.VipLevel,
                LoginName = loginEntity.LoginName,
                DisplayName = loginEntity.Register.DisplayName,
                AgentId = loginEntity.Register.AgentId,
                IsAgent = loginEntity.Register.IsAgent,
                HideDisplayNameCount = loginEntity.Register.HideDisplayNameCount,
                CommissionBalance = userBalance.CommissionBalance,
                FreezeBalance = userBalance.FreezeBalance,
                IsGeneralUser = isGeneralUser,
            };
        }

        /// <summary>
        /// 完成提现
        /// 修改提现记录，清理冻结资金
        /// </summary>
        public void CompleteWithdraw(string orderId, string responseMsg, string opUserId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                if (entity.Status != WithdrawStatus.Requesting)
                {
                    throw new Exception("该条信息提取状态不能进行完成操作 - " + entity.Status);
                }

                #region 判断充值金额是否在财务员执行范围

                //FinanceSettingsInfo FinanceInfo = fundManager.GetFinanceSettingsInfo(opUserId, "10");
                //if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                //{
                //    throw new Exception("您还未设置财务员提现金额范围！");
                //}
                //if (entity.RequestMoney < FinanceInfo.MinMoney || entity.RequestMoney > FinanceInfo.MaxMoney)
                //{
                //    throw new Exception("当前提现金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                //}

                #endregion

                entity.Status = WithdrawStatus.Success;
                entity.ResponseTime = DateTime.Now;
                //entity.ResponseMoney = entity.RequestMoney;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = responseMsg;
                entity.ResponseUserId = opUserId;
                fundManager.UpdateWithdraw(entity);

                //BusinessHelper.Payout_Freeze2End(BusinessHelper.FundCategory_CompleteWithdraw, orderId, orderId, false, "", "", entity.UserId, entity.RequestMoney
                //    , string.Format("完成提现，资金{0:N2}元：{1}", entity.RequestMoney, responseMsg), userId);
                //清理冻结
                Payout_Frozen_To_End(BusinessHelper.FundCategory_IntegralCompleteWithdraw, entity.UserId, orderId
                    , string.Format("完成积分提取，积分{0:N2}：{1}", entity.RequestMoney, responseMsg), entity.RequestMoney);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 拒绝提现
        /// 修改提现记录，退还冻结资金
        /// </summary>
        public void RefusedWithdraw(string orderId, string refusedMsg, string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var fundManager = new FundManager(); ;
                var entity = fundManager.QueryWithdraw(orderId);
                if (entity.Status != WithdrawStatus.Requesting)
                {
                    throw new Exception("该条信息提现状态不能进行拒绝操作 - " + entity.Status);
                }
                entity.Status = WithdrawStatus.Refused;
                entity.ResponseMoney = 0M;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = refusedMsg;
                entity.ResponseUserId = userId;
                fundManager.UpdateWithdraw(entity);

                // 返还资金
                Payin_FrozenBack(BusinessHelper.FundCategory_IntegralRefusedWithdraw, entity.UserId, orderId, entity.RequestMoney, string.Format("拒绝提取积分，返还积分{0:N2}：{1}", entity.RequestMoney, refusedMsg));
                //BusinessHelper.Payin_Freeze2Balance(BusinessHelper.FundCategory_RefusedWithdraw, orderId, orderId, false, "", "", entity.UserId, entity.AccountType, entity.RequestMoney
                //    , string.Format("拒绝提现，返还资金{0:N2}元：{1}", entity.RequestMoney, refusedMsg));

                biz.CommitTran();
            }
        }
        public void ManualHandleMoney(string keyLine, string orderId, decimal actionMoney, AccountType accountType, PayType payType, string userId, string description, string financeId = "")
        {
            //开启事务
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                if (actionMoney <= 0) throw new Exception("手工处理积分必须大于0");
                //#region 判断充值金额是否在财务员执行范围

                //var manage = new FundManager();
                //FinanceSettingsInfo FinanceInfo = manage.GetFinanceSettingsInfo(financeId, "20");
                //if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                //{
                //    throw new Exception("您还未设置财务员充值金额范围！");
                //}
                //if (actionMoney < FinanceInfo.MinMoney || actionMoney > FinanceInfo.MaxMoney)
                //{
                //    throw new Exception("当前充值金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                //}

                //#endregion

                if (payType == PayType.Payin)
                {
                    BusinessHelper.Payin_To_Balance(accountType, BusinessHelper.FundCategory_IntegralManualRemitMoney, userId, orderId, actionMoney,
                         string.Format("手工增加积分，发生额：{0:N2}元。{1}", actionMoney, description));
                }
                else
                {
                    //BusinessHelper.Payout_To_End(accountType, BusinessHelper.FundCategory_ManualDeductMoney, userId, orderId, actionMoney,
                    //    string.Format("手工扣除积分，发生额：{0:N2}元。{1}", actionMoney, description));
                    Payout_To_End(accountType, BusinessHelper.FundCategory_IntegralManualDeductMoney, userId, orderId, actionMoney,
                        string.Format("手工扣除积分，发生额：{0:N2}元。{1}", actionMoney, description));
                }

                biz.CommitTran();
            }
        }
        public UserQueryInfoCollection QueryAgentLowerUserList(string agentId, string agentName, int pageIndex, int pageSize)
        {
            var manager = new IntegralManager();

            if (!string.IsNullOrEmpty(agentName))
            {
                var userInfo = new LoginLocalManager().GetLoginByName(agentName);
                if (userInfo != null && !string.IsNullOrEmpty(userInfo.UserId))
                    agentId = userInfo.UserId;
            }
            if (string.IsNullOrEmpty(agentId))
                return new UserQueryInfoCollection();
            return manager.QueryAgentLowerUserList(agentId, pageIndex, pageSize);
        }
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userId, WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId = "")
        {
            var statusList = new List<int>();
            if (status.HasValue) statusList.Add((int)status.Value);

            var result = new Withdraw_QueryInfoCollection();
            var totalCount = 0;
            var totalMoney = 0M;
            var totalResponseMoney = 0M;
            var winCount = 0;
            var refusedCount = 0;
            var totalWinMoney = 0M;
            var totalRefusedMoney = 0M;
            result.WithdrawList = new IntegralManager().QueryWithdrawList(userId, status, startTime, endTime, pageIndex, pageSize, orderId,
                out   winCount, out   refusedCount, out   totalWinMoney, out   totalRefusedMoney, out totalResponseMoney, out   totalCount, out totalMoney);
            result.TotalCount = totalCount;
            result.TotalMoney = totalMoney;
            result.WinCount = winCount;
            result.RefusedCount = refusedCount;
            result.TotalWinMoney = totalWinMoney;
            result.TotalRefusedMoney = totalRefusedMoney;
            result.TotalResponseMoney = totalResponseMoney;
            return result;
        }
        public Withdraw_QueryInfoCollection QueryCPSWithdrawList(string userId, WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId = "")
        {
            var statusList = new List<int>();
            if (status.HasValue) statusList.Add((int)status.Value);

            var result = new Withdraw_QueryInfoCollection();
            var totalCount = 0;
            var totalMoney = 0M;
            var totalResponseMoney = 0M;
            var winCount = 0;
            var refusedCount = 0;
            var totalWinMoney = 0M;
            var totalRefusedMoney = 0M;
            result.WithdrawList = new IntegralManager().QueryCPSWithdrawList(userId, status, startTime, endTime, pageIndex, pageSize, orderId,
                out   winCount, out   refusedCount, out   totalWinMoney, out   totalRefusedMoney, out totalResponseMoney, out   totalCount, out totalMoney);
            result.TotalCount = totalCount;
            result.TotalMoney = totalMoney;
            result.WinCount = winCount;
            result.RefusedCount = refusedCount;
            result.TotalWinMoney = totalWinMoney;
            result.TotalRefusedMoney = totalRefusedMoney;
            result.TotalResponseMoney = totalResponseMoney;
            return result;
        }

        public UserFundDetailCollection QueryUserFundDetail_ContainCommission(string userId, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            int totalPayinCount;
            decimal totalPayinMoney;
            int totalPayoutCount;
            decimal totalPayoutMoney;
            var collection = new UserFundDetailCollection();

            List<AccountType> acList = new List<AccountType>();
            if (!string.IsNullOrEmpty(accountTypeList))
            {
                foreach (var item in accountTypeList.Split('|'))
                {
                    acList.Add((AccountType)int.Parse(item));
                }
            }
            List<string> cayList = new List<string>();
            if (!string.IsNullOrEmpty(categoryList))
            {
                foreach (var item in categoryList.Split('|'))
                {
                    cayList.Add(item);
                }
            }
            collection.FundDetailList = new IntegralManager().QueryUserFundDetail_ContainCommission(userId, fromDate, toDate, acList, cayList, pageIndex, pageSize
                , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
            collection.TotalPayinCount = totalPayinCount;
            collection.TotalPayinMoney = totalPayinMoney;
            collection.TotalPayoutCount = totalPayoutCount;
            collection.TotalPayoutMoney = totalPayoutMoney;
            return collection;
        }

        public UserQueryInfo QueryUserByKey(string userId, string agentId)
        {
            var userManager = new UserBalanceManager();
            var reg = userManager.QueryUserRegister(userId);
            if (reg == null)
                throw new Exception("用户不存在");
            if (!string.IsNullOrEmpty(agentId) && reg.AgentId != agentId)
                throw new Exception(string.Format("用户{0}不属于您发展的用户", userId));

            var balance = userManager.QueryUserBalance(userId);
            if (balance == null)
                throw new Exception("用户账户不存在");

            var realNameManager = new UserRealNameManager();
            var real = realNameManager.GetUserRealName(userId);

            var mobileManagr = new UserMobileManager();
            var mobile = mobileManagr.GetUserMobile(userId);

            var fundManger = new FundManager();
            var freezeBalance = fundManger.QueryAgentFreezeBalanceByUserId(userId);

            return new UserQueryInfo
            {
                DisplayName = reg.DisplayName,
                UserId = reg.UserId,
                RealName = real == null ? string.Empty : real.RealName,
                IdCardNumber = real == null ? string.Empty : real.IdCardNumber,
                Mobile = mobile == null ? string.Empty : mobile.Mobile,
                FillMoneyBalance = balance.FillMoneyBalance,
                BonusBalance = balance.BonusBalance,
                CommissionBalance = balance.CommissionBalance,
                ExpertsBalance = balance.ExpertsBalance,
                RedBagBalance = balance.RedBagBalance,
                FreezeBalance = freezeBalance,
                IsEnable = reg.IsEnable,
                AgentId = reg.AgentId,
                IsAgent = reg.IsAgent,
                CardType = "",
                ComeFrom = reg.ComeFrom,
                Email = "",
                IsFillMoney = reg.IsFillMoney,
                IsSettedEmail = true,
                IsSettedMobile = mobile == null ? false : mobile.IsSettedMobile,
                IsSettedRealName = real == null ? false : real.IsSettedRealName,
                RegisterIp = reg.RegisterIp,
                RegTime = reg.CreateTime,
                VipLevel = reg.VipLevel,
                CurrentDouDou = balance.CurrentDouDou,
            };
        }

        public UserQueryInfoCollection QueryUserList_Dl(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isAgent
        , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize)
        {
            using (var manager = new IntegralManager())
            {
                int totalCount;
                decimal totalFillMoneyBalance;
                decimal totalBonusBalance;
                decimal totalCommissionBalance;
                decimal totalExpertsBalance;
                decimal totalFreezeBalance;
                decimal totalRedBagBalance;
                int totalDouDou;
                var list = manager.QueryUserList(regFrom, regTo, keyType, keyValue, isEnable, isFillMoney, isAgent, commonBlance, bonusBlance, freezeBlance, vipRange, comeFrom, agentId, pageIndex, pageSize,
                    out totalCount, out   totalFillMoneyBalance, out   totalBonusBalance, out   totalCommissionBalance, out totalExpertsBalance, out totalFreezeBalance, out totalRedBagBalance, out totalDouDou);
                var collection = new UserQueryInfoCollection
                {
                    TotalCount = totalCount,
                    TotalFillMoneyBalance = totalFillMoneyBalance,
                    TotalBonusBalance = totalBonusBalance,
                    TotalCommissionBalance = totalCommissionBalance,
                    TotalExpertsBalance = totalExpertsBalance,
                    TotalFreezeBalance = totalFreezeBalance,
                    TotalRedBagBalance = totalRedBagBalance,
                    TotalDouDou = totalDouDou,
                };
                collection.LoadList(list);
                return collection;
            }
        }

        #region 分红相关


        #endregion
    }
}
