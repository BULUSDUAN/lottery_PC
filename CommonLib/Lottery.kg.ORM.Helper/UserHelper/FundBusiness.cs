using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
   public class FundBusiness:DBbase
    {
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


        /// <summary>
        /// 申请提现
        /// 计算申请金额是否可提现，返回计算结果
        /// </summary>
        public CheckWithdrawResult RequestWithdraw_Step1(string userId, decimal requestMoney)
        {
          
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
                #endregion

            
        }


    }
}
