using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using GameBiz.Domain.Entities;
using Common.Utilities;
using GameBiz.Auth.Domain.Managers;
using Common.Business;
using Common.Communication;
using Common;
using Common.Cryptography;
using Common.XmlAnalyzer;
using Common.Net.SMS;
using Common.JSON;
using System.Threading;
using Common.Gateway.DPPay;
using GameBiz.Business.Domain.Managers;
using System.Web.Script.Serialization;
using GameBiz.Business.Domain.Entities;

namespace GameBiz.Business
{
    public class FundBusiness
    {
        //private static string _mobile_financial = SettingConfigAnalyzer.GetConfigValueByKey("Financial", "Mobile");
        //private static string _mobile_financial = "13645612378";
        public string UserFillMoney(UserFillMoneyAddInfo info, string userId, string requestBy, string agentId)
        {
            var orderId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                orderId = string.IsNullOrEmpty(info.CustomerOrderId) ? BusinessHelper.GetUserFillMoneyId() : info.CustomerOrderId;
                var manager = new FundManager();
                var exist = manager.QueryFillMoney(orderId);// manager.QueryFundDetailByOrderId(orderId);
                if (exist != null)
                    throw new Exception("订单号已存在");
                //if (info.FillMoneyAgent == FillMoneyAgentType.ChinaPay && info.RequestMoney < 125M)
                //    info.RequestMoney--;
                manager.AddFillMoney(new FillMoney
                {
                    FillMoneyAgent = info.FillMoneyAgent,
                    GoodsDescription = info.GoodsDescription,
                    GoodsName = info.GoodsName,
                    GoodsType = info.GoodsType,
                    IsNeedDelivery = info.IsNeedDelivery,
                    NotifyUrl = info.NotifyUrl,
                    OrderId = orderId,
                    RequestBy = requestBy,
                    RequestExtensionInfo = info.RequestExtensionInfo,
                    RequestMoney = info.RequestMoney,
                    RequestTime = DateTime.Now,
                    ReturnUrl = info.ReturnUrl,
                    ShowUrl = info.ShowUrl,
                    Status = FillMoneyStatus.Requesting,
                    UserId = userId,
                    PayMoney = info.PayMoney,
                    SchemeSource = info.SchemeSource,
                    AgentId = agentId,
                });

                biz.CommitTran();
            }
            return orderId;
        }

        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public void SetBalancePassword(string userId, string oldPassword, bool isSetPwd, string newPassword)
        {
            var balanceManager = new UserBalanceManager();
            var entity = balanceManager.QueryUserBalance(userId);
            if (entity.IsSetPwd)
            {
                oldPassword = Encipherment.MD5(string.Format("{0}{1}", oldPassword, _gbKey)).ToUpper();
                if (string.IsNullOrEmpty(oldPassword) || !oldPassword.Equals(entity.Password))
                {
                    throw new Exception("输入资金密码错误");
                }
            }
            entity.IsSetPwd = isSetPwd;
            entity.Password = Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper();
            balanceManager.UpdateUserBalance(entity);
        }
        public void SetBalancePasswordNeedPlace(string userId, string password, string placeList)
        {
            var balanceManager = new UserBalanceManager();
            var entity = balanceManager.QueryUserBalance(userId);
            if (entity.IsSetPwd)
            {
                password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                if (string.IsNullOrEmpty(password) || !password.Equals(entity.Password))
                {
                    throw new Exception("输入资金密码错误");
                }
            }
            else
            {
                throw new Exception("必须先设置资金密码");
            }
            entity.NeedPwdPlace = placeList;
            balanceManager.UpdateUserBalance(entity);
        }

        public string ManualFillMoney(UserFillMoneyAddInfo info, string userId, string requestBy, out int vipLevel)
        {
            var orderId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                if (info.RequestMoney <= 0) throw new Exception("充值金额必须大于0");
                var user = new UserBalanceManager().QueryUserRegister(userId);
                if (user == null) throw new Exception(string.Format("用户账户{0}不存在", userId));

                #region 判断充值金额是否在财务员执行范围

                var manage = new FundManager();
                FinanceSettingsInfo FinanceInfo = manage.GetFinanceSettingsInfo(requestBy, "20");
                if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                {
                    throw new Exception("您还未设置财务员充值金额范围！");
                }
                if (info.RequestMoney < FinanceInfo.MinMoney || info.RequestMoney > FinanceInfo.MaxMoney)
                {
                    throw new Exception("当前充值金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                }

                #endregion

                vipLevel = user.VipLevel;

                orderId = BusinessHelper.GetManualFillMoneyId();
                new FundManager().AddFillMoney(new FillMoney
                {
                    FillMoneyAgent = info.FillMoneyAgent,
                    GoodsDescription = info.GoodsDescription,
                    GoodsName = info.GoodsName,
                    GoodsType = info.GoodsType,
                    IsNeedDelivery = info.IsNeedDelivery,
                    NotifyUrl = info.NotifyUrl,
                    OrderId = orderId,
                    RequestBy = requestBy,
                    RequestMoney = info.RequestMoney,
                    RequestTime = DateTime.Now,
                    ReturnUrl = info.ReturnUrl,
                    ShowUrl = info.ShowUrl,
                    Status = FillMoneyStatus.Success,
                    UserId = user.UserId,
                    PayMoney = info.PayMoney,
                    SchemeSource = info.SchemeSource,
                    ResponseBy = requestBy,
                    ResponseMessage = "手工充值完成",
                    ResponseMoney = info.RequestMoney,
                    ResponseTime = DateTime.Now,
                    ResponseCode = "ManualFillMoney",
                });

                BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_ManualFillMoney, user.UserId, orderId, info.RequestMoney,
                    string.Format("手工充值，发生额：{0:N2}元，{1}", info.RequestMoney, info.GoodsDescription), operatorId: requestBy);


                biz.CommitTran();
            }
            return orderId;
        }
        public void ManualHandleMoney(string keyLine, string orderId, decimal actionMoney, AccountType accountType, PayType payType, string userId, string description, string financeId = "")
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                if (actionMoney <= 0) throw new Exception("手工处理金额必须大于0");

                #region 判断充值金额是否在财务员执行范围

                var manage = new FundManager();
                FinanceSettingsInfo FinanceInfo = manage.GetFinanceSettingsInfo(financeId, "20");
                if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                {
                    throw new Exception("您还未设置财务员充值金额范围！");
                }
                if (actionMoney < FinanceInfo.MinMoney || actionMoney > FinanceInfo.MaxMoney)
                {
                    throw new Exception("当前充值金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                }

                #endregion

                if (payType == PayType.Payin)
                {
                    BusinessHelper.Payin_To_Balance(accountType, BusinessHelper.FundCategory_ManualRemitMoney, userId, orderId, actionMoney,
                        string.Format("手工打款，发生额：{0:N2}元。{1}", actionMoney, description), operatorId: financeId);
                }
                else
                {
                    BusinessHelper.Payout_To_End(accountType, BusinessHelper.FundCategory_ManualDeductMoney, userId, orderId, actionMoney,
                        string.Format("手工扣款，发生额：{0:N2}元。{1}", actionMoney, description), operatorId: financeId);
                }

                biz.CommitTran();
            }
        }

        private static System.Collections.Hashtable dict = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        //static Dictionary<string, decimal> dict = new Dictionary<string, decimal>();
        Mutex m = new Mutex(false);
        public bool CompleteFillMoneyOrder(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string outerFlowId, out FillMoneyAgentType agentType,
            out string userId, out int vipLevel, string operatorId = "")
        {
            try
            {
                m.WaitOne();
                if (dict.ContainsKey(orderId))
                    throw new Exception("重复订单ID");
                dict.Add(orderId, money);
                var fundManager = new FundManager();
                var entity = fundManager.QueryFillMoney(orderId);
                if (entity == null)
                    throw new Exception("订单号错误");
                agentType = entity.FillMoneyAgent;
                userId = entity.UserId;

                if (entity.Status != FillMoneyStatus.Requesting)
                    throw new Exception("订单状态不正确");
                if (entity.Status == status)
                    throw new Exception("重复订单状态");


                //开启事务
                using (var biz = new GameBizBusinessManagement())
                {
                    biz.BeginTran();

                    vipLevel = 0;
                    //return false;
                    entity.Status = status;
                    entity.ResponseMoney = money;
                    entity.ResponseTime = DateTime.Now;
                    entity.ResponseMessage = msg;
                    entity.ResponseCode = code;
                    entity.OuterFlowId = outerFlowId;
                    fundManager.UpdateFillMoney(entity);

                    if (status == FillMoneyStatus.Success)
                    {
                        var userManager = new UserBalanceManager();
                        var user = userManager.GetUserRegister(entity.UserId);
                        user.IsFillMoney = true;
                        vipLevel = user.VipLevel;
                        userManager.UpdateUserRegister(user);

                        //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_UserFillMoney, orderId, orderId, false, "", "", user.UserId, AccountType.Common, money
                        //    , string.Format("用户充值{0:N2}元", money), operatorId);
                        BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_UserFillMoney, user.UserId, orderId, money,
                            string.Format("用户充值{0:N2}元", money));
                    }

                    biz.CommitTran();
                }

                //if (money >= 5000)
                //{
                //    try
                //    {
                //        var _mobile_financial = new CacheDataBusiness().QueryCoreConfigByKey("Site.Financial.Mobile").ConfigValue;
                //        foreach (var item in _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                //        {
                //            BusinessHelper.SendMsg(item, string.Format("财务人员请注意：用户{0}的充值订单{1}已成功充值{2:N}元，请注意出票平台账户余额。", userId, orderId, money), string.Empty, 4, userId, orderId);
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}

                //刷新余额
                BusinessHelper.RefreshRedisUserBalance(userId);

                return true;
            }
            finally
            {
                m.ReleaseMutex();
            }
        }
        /// <summary>
        /// 充值专员完成订单充值
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <param name="money"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="outerFlowId"></param>
        /// <param name="agentType"></param>
        /// <param name="userId"></param>
        /// <param name="vipLevel"></param>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public bool CompleteFillMoneyOrderByCzzy(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string UserId, out FillMoneyAgentType agentType,
        out string userId, out int vipLevel, string type, string operatorId = "")
        {
            try
            {
                m.WaitOne();
                if (dict.ContainsKey(orderId))
                    throw new Exception("重复订单ID");
                dict.Add(orderId, money);
                var fundManager = new FundManager();
                var entity = fundManager.QueryFillMoney(orderId);
                if (entity == null)
                    throw new Exception("订单号错误");
                agentType = entity.FillMoneyAgent;
                userId = entity.UserId;

                if (entity.Status != FillMoneyStatus.Requesting)
                    throw new Exception("订单状态不正确");
                if (entity.Status == status)
                    throw new Exception("重复订单状态");
                var balanceManager = new UserBalanceManager();
                var userBalance = balanceManager.QueryUserBalance(userId);
                if (userBalance == null)
                    throw new Exception("用户帐户不存在 - " + userId);


                //开启事务
                using (var biz = new GameBizBusinessManagement())
                {
                    biz.BeginTran();

                    vipLevel = 0;
                    //return false;
                    entity.Status = status;
                    entity.ResponseMoney = money;
                    entity.ResponseTime = DateTime.Now;
                    entity.ResponseMessage = msg;
                    entity.ResponseCode = code;
                    entity.OuterFlowId = UserId;
                    fundManager.UpdateFillMoney(entity);

                    if (status == FillMoneyStatus.Success)
                    {
                        var userManager = new UserBalanceManager();
                        var user = userManager.GetUserRegister(entity.UserId);
                        user.IsFillMoney = true;
                        vipLevel = user.VipLevel;
                        userManager.UpdateUserRegister(user);

                        BusinessHelper.PayOut_To_BalanceByCzzy(type == "50" ? AccountType.FillMoney : AccountType.RedBag, BusinessHelper.FundCategory_UserFillMoneyByCzzy, UserId, orderId, money,
                           string.Format("帮用户{1}充值{0:N2}元", money, user.UserId));
                        BusinessHelper.Payin_To_Balance(type == "50" ? AccountType.FillMoney : AccountType.RedBag, BusinessHelper.FundCategory_UserFillMoneyByCzzy, user.UserId, orderId, money,
                            string.Format("充值专员{1}充值{0:N2}元", money, UserId));
                    }

                    biz.CommitTran();
                }
                //刷新余额
                BusinessHelper.RefreshRedisUserBalance(userId);

                return true;
            }
            finally
            {
                m.ReleaseMutex();
            }
        }
        /// <summary>
        /// 支付宝充值免手续费
        /// </summary>
        public bool CompleteFillMoneyOrderFeeFree(string orderId, FillMoneyStatus status, decimal money, string code, string msg, string outerFlowId, DateTime alipayTime, out FillMoneyAgentType agentType, out string userId, out int vipLevel, string operatorId = "")
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                vipLevel = 0;
                var fundManager = new FundManager();
                var entity = fundManager.QueryFillMoneyByOrderId(orderId);
                if (entity == null)
                    throw new Exception("订单号错误或者该订单不属于支付宝充值订单！");
                else if (entity.Status == FillMoneyStatus.Success)
                    throw new Exception("当前订单已完成充值");
                agentType = entity.FillMoneyAgent;
                userId = entity.UserId;

                if (entity.Status != FillMoneyStatus.Requesting)
                    return false;

                entity.Status = status;
                entity.ResponseMoney = money;
                entity.RequestMoney = money;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = msg;
                entity.ResponseCode = code;
                entity.OuterFlowId = outerFlowId;
                fundManager.UpdateFillMoney(entity);

                if (status == FillMoneyStatus.Success)
                {
                    var userManager = new UserBalanceManager();
                    var user = userManager.GetUserRegister(entity.UserId);
                    user.IsFillMoney = true;
                    vipLevel = user.VipLevel;
                    userManager.UpdateUserRegister(user);

                    //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_UserFillMoney, orderId, orderId, false, "", "", user.UserId, AccountType.Common, money
                    //    , string.Format("用户充值{0:N2}元", money), operatorId);
                    BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_UserFillMoney, user.UserId, orderId, money,
                        string.Format("用户充值{0:N2}元", money));
                }
                //todo: 发短信

                biz.CommitTran();
            }

            try
            {
                if (money >= 5000)
                {
                    var _mobile_financial = new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Financial.Mobile");
                    foreach (var item in _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        BusinessHelper.SendMsg(item, string.Format("财务人员请注意：用户{0}的充值订单{1}已成功充值{2:N}元，请注意出票平台账户余额。", userId, orderId, money), string.Empty, 4, userId, orderId);
                    }
                }
            }
            catch
            {
            }

            return true;
        }
        public string ManualCompleteFillMoneyOrder(string orderId, FillMoneyStatus status, out FillMoneyAgentType agentType, out decimal money, out int vipLevel, string financeId = "")
        {
            var userId = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();


                vipLevel = 0;
                var fundManager = new FundManager();
                var entity = fundManager.QueryFillMoney(orderId);
                if (entity == null)
                    throw new Exception("订单号错误");

                #region 判断充值金额是否在财务员执行范围

                var manage = new FundManager();
                FinanceSettingsInfo FinanceInfo = fundManager.GetFinanceSettingsInfo(financeId, "20");
                if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                {
                    throw new Exception("您还未设置财务员充值金额范围！");
                }
                if (entity.RequestMoney < FinanceInfo.MinMoney || entity.RequestMoney > FinanceInfo.MaxMoney)
                {
                    throw new Exception("当前充值金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                }

                #endregion

                agentType = entity.FillMoneyAgent;
                money = entity.RequestMoney;
                if (entity.Status != FillMoneyStatus.Requesting)
                    throw new LogicException("充值订单的状态不是请求中 - " + entity.Status);

                entity.Status = status;
                entity.ResponseMoney = entity.RequestMoney;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = "手工完成充值";
                entity.ResponseCode = status.ToString();
                fundManager.UpdateFillMoney(entity);

                if (status == FillMoneyStatus.Success)
                {
                    var userManager = new UserBalanceManager();
                    var user = userManager.GetUserRegister(entity.UserId);
                    user.IsFillMoney = true;
                    vipLevel = user.VipLevel;
                    userManager.UpdateUserRegister(user);

                    //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_ManualFillMoney, orderId, orderId, false, "", "", user.UserId, AccountType.Common, money
                    //    , string.Format("用户充值{0:N2}元", money), financeId);

                    BusinessHelper.Payin_To_Balance(AccountType.FillMoney, BusinessHelper.FundCategory_ManualFillMoney, user.UserId, orderId, money, string.Format("用户充值{0:N2}元", money));
                    if (money >= 5000)
                    {
                        try
                        {
                            var _mobile_financial = new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Financial.Mobile");
                            foreach (var item in _mobile_financial.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                BusinessHelper.SendMsg(item, string.Format("财务人员请注意：用户{0}的充值订单{1}已成功充值{2:N}元，请注意出票平台账户余额。", userId, orderId, money), string.Empty, 4, entity.UserId, orderId);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                userId = entity.UserId;
                biz.CommitTran();
            }
            return userId;

        }

        public FillMoneyQueryInfo QueryFillMoney(string orderId)
        {
            return new FundManager().QueryFillMoneyInfo(orderId);
        }
        public UserBalanceInfo QueryUserBalance(string userId)
        {
            using (var manager = new UserBalanceManager())
            {
                var balance = manager.QueryUserBalance(userId);
                if (balance == null)
                {
                    throw new ArgumentException("用户账户不存在");
                }
                return new UserBalanceInfo
                {
                    UserId = balance.UserId,
                    FillMoneyBalance = balance.FillMoneyBalance,
                    BonusBalance = balance.BonusBalance,
                    CommissionBalance = balance.CommissionBalance,
                    FreezeBalance = balance.FreezeBalance,
                    ExpertsBalance = balance.ExpertsBalance,
                    RedBagBalance = balance.RedBagBalance,
                    IsSetPwd = balance.IsSetPwd,
                    NeedPwdPlace = balance.NeedPwdPlace,
                    CurrentDouDou = balance.CurrentDouDou,
                    UserGrowth = balance.UserGrowth,
                    CPSBalance = balance.CPSBalance,
                    BalancePwd = balance.Password,
                };
            }
        }

        public void GiveRed(string userIdFrom, Dictionary<string, decimal> userIdToList, string password, string message)
        {
            if (userIdToList.Count == 0)
            {
                throw new ArgumentException("没有指定任何收红包的用户");
            }
            var totalMoney = userIdToList.Sum(i => i.Value);
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                var userManager = new UserBalanceManager();
                var userBalance = userManager.QueryUserBalance(userIdFrom);
                //if (userBalance.CommonBalance < totalMoney)
                //{
                //    throw new ArgumentException(string.Format("通用账户余额不足，赠送红包总数：￥{0:N2}，账户余额：￥{1:N2}", totalMoney, userBalance.CommonBalance));
                //}
                var userFrom = userManager.GetUserRegister(userIdFrom);
                foreach (var userToId in userIdToList)
                {
                    if (userToId.Value <= 0M)
                    {
                        throw new ArgumentException("赠送的红包不能是负金额");
                    }
                    var userTo = userManager.GetUserRegister(userToId.Key);

                    var orderId = BusinessHelper.GetTransferId();
                    // 资金转出
                    //BusinessHelper.Payout_2End(BusinessHelper.FundCategory_TransferFrom, orderId, orderId, true, "Transfer", password, userIdFrom, AccountType.Common, userToId.Value
                    //    , string.Format("赠送红包￥{0:N2}，给{1}【{2}】。{3}", userToId.Value, userToId, userTo.DisplayName, message));
                    // 资金转入
                    //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_TransferTo, orderId, orderId, false, "", "", userToId.Key, AccountType.Common, userToId.Value
                    //    , string.Format("收到红包￥{1:N2}，从{0}【{1}】。{3}", userToId.Value, userIdFrom, userFrom.DisplayName, message));
                }
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 申请提现
        /// 计算申请金额是否可提现，返回计算结果
        /// </summary>
        public CheckWithdrawResult RequestWithdraw_Step1(string userId, decimal requestMoney)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var balanceManager = new UserBalanceManager();
                var fundManager = new FundManager();
                var userManager = new UserManager();
                var maxTimes = 3;
                var currentTimes = fundManager.QueryTodayWithdrawTimes(userId);
                if (currentTimes >= maxTimes)
                    throw new Exception(string.Format("每日只能提现{0}次", maxTimes));
                var user = userManager.LoadUser(userId);
                if (user == null)
                    throw new Exception("用户不存在 - " + userId);
                var userBalance = balanceManager.QueryUserBalance(userId);
                if (userBalance == null)
                    throw new Exception("用户帐户不存在 - " + userId);

                //奖金+佣金+名家
                var can_tx_money = userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance;
                //正常提现
                if (can_tx_money >= requestMoney)
                {
                    return new CheckWithdrawResult
                    {
                        WithdrawCategory = WithdrawCategory.General,
                        RequestMoney = requestMoney,
                        ResponseMoney = requestMoney,
                        Summary = string.Format("申请提现{0:N2}元，到账{0:N2}元。", requestMoney),
                    };
                }

                var payoutMoney = requestMoney - can_tx_money;
                if (userBalance.FillMoneyBalance < payoutMoney)
                {
                    return new CheckWithdrawResult
                    {
                        WithdrawCategory = WithdrawCategory.Error,
                        RequestMoney = requestMoney,
                        ResponseMoney = 0,
                        Summary = "可用充值金额不足",
                    };
                }

                #region 不处理充值金额的消费，直接扣5%

                //查找最后N条充值记录，且N条充值记录之和大于 payoutMoney
                //var lastFillMoneyList = new List<FillMoney>();
                ////第一条充值记录
                //FillMoney firstFillMoneyRecord = null;
                //var goFindFillMoneyRecord = true;
                ////总充值金额
                //var totalFillMoney = 0M;
                //var index = 0;
                //while (goFindFillMoneyRecord)
                //{
                //    totalFillMoney = lastFillMoneyList.Count <= 0 ? 0M : lastFillMoneyList.Sum(p => p.ResponseMoney.Value);
                //    var tempList = fundManager.QueryLastFillMoney(userId, index, 10);
                //    //全部充值记录之和 都达不到 payoutMoney
                //    if (tempList.Count <= 0)
                //        throw new Exception("充值记录达不到提款要求");

                //    foreach (var record in tempList)
                //    {
                //        totalFillMoney += record.ResponseMoney.Value;
                //        lastFillMoneyList.Add(record);
                //        if (totalFillMoney >= payoutMoney)
                //        {
                //            firstFillMoneyRecord = record;
                //            goFindFillMoneyRecord = false;
                //            break;
                //        }
                //    }
                //    ++index;
                //}
                //if (firstFillMoneyRecord == null)
                //    throw new Exception("未查找到满足条件的充值记录");

                ////扣除红包金额
                //var redBagList = fundManager.QueryRedBagDetail(lastFillMoneyList.Select(p => p.OrderId).ToArray());
                //var redBagSum = redBagList.Count <= 0 ? 0M : redBagList.Sum(p => p.RedBagMoney);

                ////查询总消费
                //var totalPayOut = fundManager.QueryFillMoneyAccountPayMoney(firstFillMoneyRecord.RequestTime, userId);
                ////总消费 / 总充值 ，如果大于30%，则无手续费，否则收5%手续费
                //if ((totalPayOut / totalFillMoney) >= 0.3M)
                //{
                //    //无手续费提现
                //    //正常提现
                //    return new CheckWithdrawResult
                //    {
                //        RequestMoney = requestMoney,
                //        ResponseMoney = requestMoney,
                //        WithdrawCategory = WithdrawCategory.Acceptable,
                //        Summary = string.Format("申请提现{0:N2}元，到账{0:N2}元.{1} ", requestMoney,
                //                    redBagSum > 0 ? string.Format("但要扣除充值赠送红包{0:N2}元。", redBagSum) : string.Empty),
                //    };
                //}
                //else
                //{
                //    //收取5%手续费
                //    return new CheckWithdrawResult
                //    {
                //        RequestMoney = requestMoney,
                //        ResponseMoney = requestMoney - requestMoney * 5 / 100,
                //        WithdrawCategory = WithdrawCategory.Compulsory,
                //        Summary = string.Format("申请提现{0:N2}元，扣除手续费10%后到账{1:N2}元. {2} <br/>提款到账时间：15个工作日（不含节假日）", requestMoney, requestMoney - (requestMoney * 5 / 100),
                //                    redBagSum > 0 ? string.Format("同时扣除充值赠送红包{0:N2}元。", redBagSum) : string.Empty),

                //    };
                //} 

                #endregion

                var percent = decimal.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("WithdrawAboutFillMoney.CutPercent"));
                //收取5%手续费
                var cutFree = payoutMoney * percent / 100;
                return new CheckWithdrawResult
                {
                    RequestMoney = requestMoney,
                    ResponseMoney = requestMoney - cutFree,
                    WithdrawCategory = WithdrawCategory.Compulsory,
                    Summary = string.Format("提现金额中包含充值金额，需要扣除{0}%手续费{1:N2}元，实际到账{2:N2}元", percent, cutFree, requestMoney - cutFree),

                };

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 申请提现
        /// 实际添加提现记录，扣除用户资金
        /// </summary>
        public void RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string password)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();


                var resonseMoney = 0M;
                var orderId = BusinessHelper.GetWithdrawId();
                var category = BusinessHelper.Payout_To_Frozen_Withdraw(BusinessHelper.FundCategory_RequestWithdraw, userId, orderId, info.RequestMoney
                      , string.Format("申请提现：{0:N2}元", info.RequestMoney), "Withdraw", password, out resonseMoney);


                #region "快捷充值同卡进出判断"

                ////查找c_fillmoney中fillmoneyagent,如果有快捷充值的记录，就返回快捷充值的总金额。
                ////requestMoney申请金额如果超过 快捷充值的总金额 就提示快捷充值的金额 必须同卡进出。不管是奖金账户还是充值账户等。
                ////账户能提现的总额  大于 申请金额 + 快捷充值的总金额 允许随便什么卡提现
                ////若 账户能提现的总额  小于  申请金额 + 快捷充值的总金额 此时要求 必须同卡进出 （一个用户可能有多个银行卡快捷支付了）
                ////若申请金额 如果小于 快捷充值的总金额 同时小于 账户能提现的总额 此时 允许随便什么卡提现
                ////RequestExtensionInfo存放快捷充值时 的卡号 对比提现时候的请求卡号 
                //var fundManager = new FundManager();
                //var balanceManager = new UserBalanceManager();
                ////需要验证同卡进出的快捷接口 有空写到配置表中去
                //string expressInterface = "152,145,141,135,134,125";
                ////先根据用户id判断 该用户是否有快捷充值成功的记录 若有 就判断是否同卡进出
                //var expressCount = fundManager.QueryExpressCount(userId, expressInterface);
                //var userBalance = balanceManager.QueryUserBalance(userId);
                //var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance;
                ////前面已经判断resonseMoney 不会大于totalMoney 
                ////有快捷充值记录
                //if (expressCount > 0)
                //{
                //    //有快捷卡充值记录 查看该用户用 (可能)用多个卡快捷充值后 提现的情况
                //    //通过查询快捷充值记录返回 多个卡快捷充值的卡号列表 若本次提现的卡号在该列表中 循环该列表 去查询该卡提现的金额 
                //    //若该卡提现金额超过该卡充值的金额，则允许用（该卡）或者其他的卡继续提现 
                //    //若本次提现的卡号不在该列表中 循环该列表中的每个卡 若每个开提现都超过充值的金额 则允许用非快捷充值的卡提现

                //    //该用户通过 该快捷卡充值 的总金额  带上卡号作为条件 
                //    var expressTotalMoney = fundManager.QueryExpressTotalMoney(userId, expressInterface, info.BankCardNumber);
                //    if (expressTotalMoney < 0)
                //        throw new LogicException("存在快捷充值，提现卡号必须和充值卡号相同。");
                //    //多个充值方式的暂时不考虑：比如有微信，有支付宝，有网银等。只考虑存在快捷充值就必须同卡进出
                //    else
                //    {

                //    }
                //}
                #endregion

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
                //查询到账金额
                var wi = GetWithdrawById(orderId);

                //判断DP是否可用
                var cacheDataBusiness = new CacheDataBusiness();
                var iscoreConfigInfo = cacheDataBusiness.QueryCoreConfigByKey("DP.Isvailable");
                //获取当前系统时间（4-5点DP关闭）
                var datetime = System.DateTime.Now;
                var nowHourAndM = datetime.ToString("t");
                var datestar = cacheDataBusiness.QueryCoreConfigByKey("DP.StartTime");
                var dateend = cacheDataBusiness.QueryCoreConfigByKey("DP.EndTime");
                DateTime dstar = Convert.ToDateTime(datestar.ConfigValue);
                DateTime dend = Convert.ToDateTime(dateend.ConfigValue);

                if (iscoreConfigInfo.ConfigValue == "1")
                {

                    if (datetime <= dstar || datetime >= dend)
                    {
                        //发送消息到DP
                        //判断是否发送到DP
                        String htmls = info.BankName;
                        //获取Dp提现最大限额
                        var coreConfigInfo = cacheDataBusiness.QueryCoreConfigByKey("DP.WithdrawHigthMoney");
                        //获取Dp给的公司编码
                        var coreConfigInfoC = cacheDataBusiness.QueryCoreConfigByKey("DP.Companyid");
                        //获取DP给的key
                        var ck = cacheDataBusiness.QueryCoreConfigByKey("DP.Key");
                        //获取DP访问路径
                        var cw = cacheDataBusiness.QueryCoreConfigByKey("DP.WebUrl");
                        decimal withdrawHigthMoney = decimal.Parse(coreConfigInfo.ConfigValue);
                        var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        writer.Write("关于DP日志", "关于DP日志", Common.Log.LogType.Information, "关于DP日志", "withdrawHigthMoney===============" + withdrawHigthMoney);

                        String str = String.Format("BankCardNumber=" + info.BankCardNumber + ",BankCode=" + info.BankCode + ",BankName=" + info.BankName + ",BankSubName=" + info.BankSubName + ",CityName=" + info.CityName + ",ProvinceName=" + info.ProvinceName + ",RequestMoney=" + info.RequestMoney + ",userRealName=" + info.userRealName + ",WithdrawAgent=" + info.WithdrawAgent + "");
                        writer.Write("输出参数写测试用例", "输出参数写测试用例", Common.Log.LogType.Information, "输出参数写测试用例", "输出参数写测试用例===============" + str);
                        WithdrawApplyInfo wai = new WithdrawApplyInfo();
                        wai.company_order_num = orderId;
                        wai.company_user = userId;
                        wai.card_name = info.userRealName;
                        wai.card_num = info.BankCardNumber;
                        wai.issue_bank_name = info.BankName;
                        wai.issue_bank_address = info.BankSubName;
                        wai.memo = "";
                        String amount = wi.ResponseMoney.ToString();
                        wai.amount = Math.Round(decimal.Parse(amount), 2);

                        wai.company_id = Int32.Parse(coreConfigInfoC.ConfigValue);
                        String dpresult = null;
                        decimal dpHighmoney = decimal.Parse(coreConfigInfo.ConfigValue);
                        if (info.RequestMoney < withdrawHigthMoney)
                        {
                            if (htmls.Contains("工商银行"))
                            {
                                int bankid = (int)BankCode.ICBC;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("招商银行"))
                            {
                                int bankid = (int)BankCode.CMB;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("建设银行"))
                            {
                                int bankid = (int)BankCode.CCB;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("农业银行"))
                            {
                                int bankid = (int)BankCode.ABC;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("中国银行"))
                            {
                                int bankid = (int)BankCode.BOC;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("交通银行"))
                            {
                                int bankid = (int)BankCode.BCM;
                                wai.bank_id = bankid.ToString();
                            }

                            else if (htmls.Contains("中国民生银行") || htmls.Contains("民生银行"))
                            {
                                int bankid = (int)BankCode.CMBC;
                                wai.bank_id = bankid.ToString();
                            }

                            else if (htmls.Contains("中信银行"))
                            {
                                int bankid = (int)BankCode.ECC;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("浦东发展银行") || htmls.Contains("浦发") || htmls.Contains("浦东"))
                            {
                                int bankid = (int)BankCode.SPDB;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("邮政储蓄") || htmls.Contains("中国邮政"))
                            {
                                int bankid = (int)BankCode.PSBC;
                                wai.bank_id = bankid.ToString();
                            }

                            else if (htmls.Contains("光大银行"))
                            {
                                int bankid = (int)BankCode.CEB;
                                wai.bank_id = bankid.ToString();
                            }

                            else if (htmls.Contains("平安银行"))
                            {
                                int bankid = (int)BankCode.PINGAN;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("广东发展银行") || htmls.Contains("广发银行"))
                            {
                                int bankid = (int)BankCode.CGB;
                                wai.bank_id = bankid.ToString();
                            }
                            else if (htmls.Contains("华夏银行"))
                            {
                                int bankid = (int)BankCode.HXB;
                                wai.bank_id = bankid.ToString();
                            }

                            else if (htmls.Contains("兴业银行"))
                            {
                                int bankid = (int)BankCode.CIB;
                                wai.bank_id = bankid.ToString();
                            }
                            else
                            {
                                var pListe = new List<string>();
                                pListe.Add(string.Format("{0}={1}", "[OrderId]", orderId));
                                pListe.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                                pListe.Add(string.Format("{0}={1}", "[RequestMoney]", info.RequestMoney));
                                pListe.Add(string.Format("{0}={1}", "[ResponseMoney]", resonseMoney));
                                //发送短信
                                new SiteMessageControllBusiness().DoSendSiteMessage(userId, "", "ON_User_Request_Withdraw", pListe.ToArray());
                                biz.CommitTran();
                                //刷新余额
                                BusinessHelper.RefreshRedisUserBalance(userId);
                                return;
                            }
                            dpresult = Withdrawal(wai, ck.ConfigValue, cw.ConfigValue);
                            if (dpresult == null || dpresult == "")
                            {
                                biz.RollbackTran();
                                throw new Exception("服务繁忙，请稍后重试,如多次尝试失败,请在客服服务时间内联系在线客服咨询");
                            }
                        }


                    }

                }

                #region 发送站内消息：手机短信或站内信

                //var userManager = new UserBalanceManager();
                //var user = userManager.QueryUserRegister(userId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[OrderId]", orderId));
                pList.Add(string.Format("{0}={1}", "[UserName]", ""));
                pList.Add(string.Format("{0}={1}", "[RequestMoney]", info.RequestMoney));
                pList.Add(string.Format("{0}={1}", "[ResponseMoney]", resonseMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(userId, "", "ON_User_Request_Withdraw", pList.ToArray());

                #endregion

                biz.CommitTran();
            }

            //刷新余额
            BusinessHelper.RefreshRedisUserBalance(userId);


        }
        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="wai">提现申请实体类</param>
        /// <returns></returns>
        public static String Withdrawal(WithdrawApplyInfo wai, String config, String url)
        {
            //向DP发出请求

            url = url + "Withdrawal?format=json";
            String key = wai.getKey(config);

            String param = "company_id=" + wai.company_id + "&bank_id=" + wai.bank_id + "&company_order_num=" + wai.company_order_num + "&amount=" + wai.amount + "&card_num=" + wai.card_num
                + "&card_name=" + HttpUtil.UrlEncode(wai.card_name.Trim()) + "&company_user=" + wai.company_user + "&issue_bank_name=" + HttpUtil.UrlEncode(wai.issue_bank_name.Trim()) + "&issue_bank_address=" + HttpUtil.UrlEncode(wai.issue_bank_address.Trim()) +
                "&memo=" + wai.memo + "&key=" + key + "";

            try
            {
                String result = HttpUtil.Post(url, param);
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("关于DP日志", "关于DP日志", Common.Log.LogType.Information, "关于DP日志", "申请DP返回参数===============" + result);
                if (result != null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var war = js.Deserialize<WithdrawApplyResult>(result);
                    if (war.status != 1)
                    {
                        Common.Log.LogWriterGetter.GetLogWriter().Write("提现请求参数", "提现请求参数", Common.Log.LogType.Information, "提现请求参数", "提现请求参数===============" + param);
                        return null;
                    }
                }
                return result;

            }
            catch (Exception e)
            {

                Common.Log.LogWriterGetter.GetLogWriter().Write("提现请求参数", param, e);
                return null;
            }

        }

        /// <summary>
        /// 完成提现
        /// 修改提现记录，清理冻结资金
        /// </summary>
        public void CompleteWithdraw(string orderId, string responseMsg, string opUserId)
        {
            var userId = string.Empty;
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                if (entity.Status != WithdrawStatus.Requesting && entity.Status != WithdrawStatus.Request)
                {
                    throw new Exception("该条信息提现状态不能进行完成操作 - " + entity.Status);
                }
                #region 判断充值金额是否在财务员执行范围

                var manage = new FundManager();
                FinanceSettingsInfo FinanceInfo = fundManager.GetFinanceSettingsInfo(opUserId, "10");
                if (FinanceInfo == null || FinanceInfo.FinanceId <= 0)
                {
                    throw new Exception("您还未设置财务员提现金额范围！");
                }
                if (entity.RequestMoney < FinanceInfo.MinMoney || entity.RequestMoney > FinanceInfo.MaxMoney)
                {
                    throw new Exception("当前提现金额必须在" + FinanceInfo.MinMoney.ToString("N2") + "--" + FinanceInfo.MaxMoney.ToString("N2") + "之间");
                }

                #endregion

                entity.Status = WithdrawStatus.Success;
                entity.ResponseTime = DateTime.Now;
                //entity.ResponseMoney = entity.RequestMoney;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = responseMsg;
                entity.ResponseUserId = opUserId;
                fundManager.UpdateWithdraw(entity);
                userId = entity.UserId;

                //清理冻结
                BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_CompleteWithdraw, entity.UserId, orderId
                    , string.Format("完成提现，实际到账资金{0:N2}元：{1}", entity.ResponseMoney, responseMsg), entity.RequestMoney);

                #region 发送站内消息：手机短信或站内信

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(entity.UserId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[RequestTime]", entity.RequestTime));
                pList.Add(string.Format("{0}={1}", "[OrderId]", entity.OrderId));
                pList.Add(string.Format("{0}={1}", "[RequestMoney]", entity.RequestMoney.ToString("f2")));
                pList.Add(string.Format("{0}={1}", "[ResponseMoney]", entity.ResponseMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Withdraw_Success", pList.ToArray());

                #endregion


                biz.CommitTran();
            }
            //刷新余额
            BusinessHelper.RefreshRedisUserBalance(userId);


        }


        /// <summary>
        /// 提交代付改变订单状态
        /// </summary>
        public void ReuqestWithdraw(string orderId, string opUserId, string agendid)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                entity.Status = WithdrawStatus.Request;
                entity.ResponseUserId = opUserId;
                entity.AgentId = agendid;
                fundManager.UpdateWithdraw(entity);
            }
        }

        /// <summary>
        /// 提交代付改变订单状态
        /// </summary>
        public void ReuqestWithdraw2(string orderId, string opUserId, string agendid,WithdrawStatus status)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                entity.Status = status;
                entity.ResponseUserId = opUserId;
                entity.AgentId = agendid;
                fundManager.UpdateWithdraw(entity);
            }
        }




        /// <summary>
        /// 拒绝提现
        /// 修改提现记录，退还冻结资金
        /// </summary>
        public void RefusedWithdraw(string orderId, string refusedMsg, string opUserId)
        {
            var userId = string.Empty;
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var fundManager = new FundManager();
                var entity = fundManager.QueryWithdraw(orderId);
                if (entity.Status != WithdrawStatus.Requesting && entity.Status != WithdrawStatus.Request)
                {
                    throw new Exception("该条信息提现状态不能进行拒绝操作 - " + entity.Status);
                }
                entity.Status = WithdrawStatus.Refused;
                entity.ResponseMoney = 0M;
                entity.ResponseTime = DateTime.Now;
                entity.ResponseMessage = refusedMsg;
                entity.ResponseUserId = opUserId;
                fundManager.UpdateWithdraw(entity);
                userId = entity.UserId;

                // 返还资金
                BusinessHelper.Payin_FrozenBack(BusinessHelper.FundCategory_RefusedWithdraw, entity.UserId, orderId, entity.RequestMoney, string.Format("拒绝提现，返还资金{0:N2}元：{1}", entity.RequestMoney, refusedMsg));


                #region 发送站内消息：手机短信或站内信

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(entity.UserId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[OrderId]", entity.OrderId));
                pList.Add(string.Format("{0}={1}", "[RequestMoney]", entity.RequestMoney));
                pList.Add(string.Format("{0}={1}", "[ResponseMoney]", entity.ResponseMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Withdraw_Fail", pList.ToArray());

                #endregion

                biz.CommitTran();
            }
            //刷新余额
            BusinessHelper.RefreshRedisUserBalance(userId);
        }

        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
            return new SqlQueryManager().GetWithdrawById(orderId);
        }
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string orderId = "")
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
            result.WithdrawList = new SqlQueryManager().QueryWithdrawList(userId, agent, status, minMoney, maxMoney, startTime, endTime, sortType, pageIndex, pageSize, orderId,
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

        public UserBalanceFreezeCollection QueryUserBalanceFreezeListByUser(string userId, int pageIndex, int pageSize)
        {
            var balanceManager = new UserBalanceManager();

            int totalCount;
            decimal totalMoney;
            var list = balanceManager.QueryUserBalanceFreezeListByUser(userId, pageIndex, pageSize, out totalCount, out totalMoney);
            var result = new UserBalanceFreezeCollection
            {
                TotalCount = totalCount,
                TotalMoney = totalMoney,
                FreezeList = list,
            };
            return result;
        }

        /// <summary>
        ///查询某人成长值的赠送记录
        /// </summary>
        public UserGrowthDetailInfoCollection QueryUserGrowthDetailList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new UserGrowthDetailInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new FundManager().QueryUserGrowthDetailList(userId, starTime, endTime, pageIndex, pageSize, out  totalCount));
            result.TotalCount = totalCount;
            return result;
        }
        /// <summary>
        /// 查询用户成长值列表
        /// </summary>
        public UserGrowthDetailInfoCollection QueryUserGrowList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new FundManager())
            {
                return manager.QueryUserGrowList(userId, starTime, endTime, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 查询指定日期的所有资金明细
        /// </summary>
        public UserFundDetailCollection QueryFundDetailByDateTime(DateTime date)
        {
            var collection = new UserFundDetailCollection();
            var list = new SqlQueryManager().QueryFundDetailByDateTime(date);
            collection.FundDetailList = list;
            return collection;
        }
        public UserFundDetailCollection QueryFundDetailByDateTime(DateTime date, string userId)
        {
            var collection = new UserFundDetailCollection();
            var list = new SqlQueryManager().QueryFundDetailByDateTime(date, userId, new int[] { });
            collection.FundDetailList = list;
            return collection;
        }

        /// <summary>
        /// 按日期生成资金明细缓存
        /// </summary>
        public void BuildFundDetailCacheByDateTime(DateTime date)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                DoBuildFundDetailCacheByDateTime(date);
            }, date);
        }

        /// <summary>
        /// 生成指定日期的资金明细缓存
        /// </summary>
        public void DoBuildFundDetailCacheByDateTime(DateTime date)
        {
            var list = new SqlQueryManager().QueryFundDetailByDateTime(date);
            //按用户分组
            var g = list.GroupBy(p => p.UserId);
            var dateStr = date.ToString("yyyyMMdd");
            foreach (var item in g)
            {
                var dayList = list.Where(p => p.UserId == item.Key).OrderByDescending(p => p.CreateTime).ToList();
                BuildFundDetailByDate(dayList, item.Key, dateStr);
            }
        }

        public void BuildFundDetailByDate(List<FundDetailInfo> dayList, string userId, string date)
        {
            try
            {
                var content = JsonSerializer.Serialize<List<FundDetailInfo>>(dayList);
                BuildFundDetailByDate(content, userId, date);
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("BuildFundDetailByDate", userId, ex);
            }
        }

        public void BuildFundDetailByDate(string json, string userId, string date)
        {
            try
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "FundDetail", userId);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", date));
                System.IO.File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("BuildFundDetailByDate", userId, ex);
            }
        }

        /// <summary>
        /// 生成今天的用户资金缓存数据
        /// </summary>
        public void BuildTodayFundDetailCache(string userId)
        {
            //var list = new SqlQueryManager().QueryFundDetailByDateTime(DateTime.Today, userId);
            //BuildFundDetailByDate(list, userId, DateTime.Today.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// 从缓存中查询用户的资金明细数据
        /// </summary>
        public UserFundDetailCollection QueryUserFundDetailFromCache(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "FundDetail", userId);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var list = new List<FundDetailInfo>();
            var accountArray = accountTypeList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToArray();
            //var categoryArray = categoryList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            while (fromDate < toDate)
            {
                try
                {
                    //从缓存中读取指定日期的数据
                    var dateStr = fromDate.ToString("yyyyMMdd");
                    if (dateStr == DateTime.Today.ToString("yyyyMMdd"))
                    {
                        //当天的查数据库
                        var todayList = new SqlQueryManager().QueryFundDetailByDateTime(DateTime.Today, userId, accountArray);
                        list.AddRange(todayList);
                    }
                    else
                    {
                        var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", dateStr));
                        if (System.IO.File.Exists(filePath))
                        {
                            //有缓存文件
                            var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                            if (!string.IsNullOrEmpty(content))
                            {
                                //文件内容不为空
                                var currentList = JsonSerializer.Deserialize<List<FundDetailInfo>>(content);
                                var query = from l in currentList
                                            where (keyLine == string.Empty || l.KeyLine == keyLine)
                                            && (accountArray.Length == 0 || accountArray.Contains((int)l.AccountType))
                                            //&& (categoryArray.Length == 0 || categoryArray.Contains(l.Category))
                                            select l;
                                list.AddRange(query.ToList());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                fromDate = fromDate.AddDays(1);
            }

            var payInList = list.Where(p => p.PayType == PayType.Payin).ToList();
            var payOutList = list.Where(p => p.PayType == PayType.Payout).ToList();
            var collection = new UserFundDetailCollection();
            collection.TotalPayinCount = payInList.Count;
            collection.TotalPayinMoney = payInList.Count <= 0 ? 0 : payInList.Sum(p => p.PayMoney);
            collection.TotalPayoutCount = payOutList.Count;
            collection.TotalPayoutMoney = payOutList.Count <= 0 ? 0 : payOutList.Sum(p => p.PayMoney);
            collection.FundDetailList = list.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return collection;
        }

        public decimal QueryUserTotalFillMoney(string userId)
        {
            return new FundManager().QueryUserTotalFillMoney(userId);
        }

        public decimal QueryUserTotalWithdrawMoney(string userId)
        {
            return new FundManager().QueryUserTotalWithdrawMoney(userId);
        }

    }
}
