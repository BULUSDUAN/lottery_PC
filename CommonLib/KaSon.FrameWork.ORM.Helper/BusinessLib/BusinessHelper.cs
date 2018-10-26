using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using System.Linq.Expressions;
using EntityModel.Domain.Entities;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Algorithms;
using EntityModel.PayModel;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using EntityModel.CoreModel;
using System.Threading.Tasks;
using EntityModel.Redis;
using KaSon.FrameWork.Common.SMS;

namespace KaSon.FrameWork.ORM.Helper
{


    public class BusinessHelper : DBbase
    {

        private const string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        private static Log4Log writerLog = new Log4Log();

        //private static AssemblyRefHelper AssRef = new AssemblyRefHelper();

        /// <summary>
        ///  用户支出，申请提现
        /// </summary>
        public WithdrawCategory Payout_To_Frozen_Withdraw(FundManager fundManager, UserBalanceManager balanceManager, C_User_Balance userBalance, string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string balancepwd, out decimal responseMoney)
        {
            var requestMoney = payoutMoney;
            if (payoutMoney <= 0M)
                throw new LogicException("消费金额不能小于0.");
            //查询帐户余额
            //var balanceManager = new UserBalanceManager();
            //var fundManager = new FundManager();
            ////资金密码判断
            //var userBalance = balanceManager.QueryUserBalance(userId);
            //if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }
            //if (userBalance.IsSetPwd && !string.IsNullOrEmpty(userBalance.NeedPwdPlace))
            //{
            //    if (userBalance.NeedPwdPlace == "ALL" || userBalance.NeedPwdPlace.Split('|', ',').Contains(place))
            //    {
            //        balancepwd = Encipherment.MD5(string.Format("{0}{1}", balancepwd, _gbKey)).ToUpper();
            //        if (!userBalance.Password.ToUpper().Equals(balancepwd))
            //        {
            //            throw new LogicException("资金密码输入错误");
            //        }
            //    }
            //}
            //var totalMoney = userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance;
            //if (totalMoney < payoutMoney)
            //    throw new LogicException(string.Format("用户总金额小于 {0:N2}元。", payoutMoney));

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
            });
            //冻结资金明细
            fundManager.AddFundDetail(new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = (int)PayType.Payin,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                    throw new LogicException("可用充值金额不足");

                #region 异常提现

                //收取5%手续费
                var percent = decimal.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("WithdrawAboutFillMoney.CutPercent"));
                var counterFee = payoutMoney * percent / 100;
                //到帐金额
                responseMoney = requestMoney - counterFee;

                //手续费明细
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = BusinessHelper.FundCategory_RequestWithdrawCounterFee,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = counterFee,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = resMoney,
                    PayType = (int)PayType.Payout,
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


        /// <summary>
        /// 刷新Redis中用户余额
        /// </summary>
        public static void RefreshRedisUserBalance(string userId)
        {
            try
            {


                var db = RedisHelperEx.DB_UserBalance;
                string key = string.Format("UserBalance_{0}", userId);
                var fund = new LocalLoginBusiness();
                var userBalance = fund.QueryUserBalance(userId);
                var json = JsonHelper.Serialize(userBalance);
                db.Set(key, json, 60 * 2);
            }
            catch (Exception ex)
            {

                Log4Log.Error("BusinessHelper-RefreshRedisUserBalance", ex);
            }
        }
        //待测试
        //kason
        public static void ExecPlugin<T>(object inputParam)
                 where T : class, IPlugin
        {
            if (_enablePluginClass == null || _enablePluginClass.Count == 0)
                _enablePluginClass = new PluginClassManager().QueryPluginClass(true);

            //启动线程
            Task.Factory.StartNew(() =>
            {

                foreach (var plugin in _enablePluginClass)
                {
                    try
                    {
                        if (typeof(T).FullName != plugin.InterfaceName) continue;




                        if (plugin.StartTime != null && plugin.StartTime > DateTime.Now) continue;//未开始
                        if (plugin.EndTime != null && plugin.EndTime < DateTime.Now) continue;//已结束

                        var fullName = plugin.ClassName + "," + plugin.AssemblyFileName;

                        //kason
                        string snamespace = plugin.AssemblyFileName;
                        string classname = plugin.ClassName;
                        //拆分,确定ClassName
                        if (plugin.ClassName.Contains("."))
                        {
                            var arr = plugin.ClassName.Split('.');
                            classname = arr[arr.Length - 1];
                        }
                        //var type = AssemblyRefHelper.GetType(classname, snamespace);
                        var type = Type.GetType(fullName);
                        if (type == null)
                        {
                            throw new ArgumentNullException("类型在当前域中不存在，或对应组件未加载：" + fullName);
                        }
                        var instance = Type.GetType(fullName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as T;
                        // object instance = Activator.CreateInstance(type); //创建实例
                        //var i = Type.GetType(fullName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as T;
                        if (instance == null)
                        {
                            throw new ArgumentNullException("无法实例化对象：" + fullName);
                        }
                        //new Thread(() =>
                        //{
                        try
                        {
                            //type.GetMethod("ExecPlugin").Invoke(instance, new object[] { inputParam });
                            instance.ExecPlugin(typeof(T).Name, inputParam);
                            // i.ExecPlugin(typeof(T).Name, inputParam);
                        }
                        catch (AggregateException ex)
                        {
                            throw new AggregateException(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            // var writer = Common.Log.LogWriterGetter.GetLogWriter();
                            Log4Log.Error("ERROR_ExecPlugin-_ExecPlugin-执行插件{0}出错" + plugin.ClassName, ex);
                        }
                        //}).Start();
                    }
                    catch (AggregateException ex)
                    {
                        throw new AggregateException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log4Log.Error("ERROR_ExecPlugin-_ExecPlugin-执行插件{0}出错" + plugin.ClassName, ex);
                        //  var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        //  writerLog.WriteLog("ERROR_ExecPlugin", "_ExecPlugin", (int)LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                    }
                }

            });



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
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }

            var old = fundManager.QueryUserClearChaseRecord(orderId, userId);
            if (old != null)
                return;

            var befor = userBalance.FreezeBalance;
            var after = userBalance.FreezeBalance - clearMoney;
            fundManager.AddFundDetail(new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)AccountType.Freeze,
                PayMoney = clearMoney,
                PayType = (int)EntityModel.Enum.PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout,
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout,
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout,
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout,
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.CPS,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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
        /// 用户支出
        /// 指定帐户支出
        /// </summary>
        public static void Payout_To_End(AccountType accountType, string category, string userId, string orderId, decimal payoutMoney, string summary, string operatorId = "")
        {
            if (payoutMoney <= 0M)
                throw new LogicException("消费金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }

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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                        throw new LogicException("账户余额不足");
                    fundManager.AddFundDetail(new C_Fund_Detail
                    {
                        Category = category,
                        CreateTime = DateTime.Now,
                        KeyLine = orderId,
                        OrderId = orderId,
                        AccountType = (int)accountType,
                        PayMoney = payoutMoney,
                        PayType = (int)PayType.Payout,
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
                    PayType = EntityModel.Enum.PayType.Payout,
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.RedBag,
                    PayMoney = currentPayout,
                    PayType = (int)EntityModel.Enum.PayType.Payout,
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





        #region 普通投注

        /// <summary>
        /// 验证投注号码 返回实际投注注数 by dzq
        /// </summary>
        public static int CheckBetCode(string userId, string gameCode, string gameType, SchemeSource schemeSource, string playType, int amount, decimal schemeTotalMoney, List<Sports_AnteCodeInfo> anteCodeList)
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
                BettingHelper.CheckPlayType(gameCode, gameTypeList, m);

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
        /// 检查彩种是否开启
        /// </summary>
        public static void CheckGameEnable(string gameCode)
        {
            var game = QueryLotteryGame(gameCode);
            if (game == null)
                throw new LogicException(string.Format("错误的彩种编码：{0}", gameCode));
            if (game.EnableStatus != EntityModel.Enum.EnableStatus.Enable)
                throw new LogicException(string.Format("{0} 暂停销售", game.DisplayName));
        }

        /// <summary>
        /// 查询彩种
        /// </summary>
        public static LotteryGame QueryLotteryGame(string gameCode)
        {
            //var RedisKeyH = "CoreConfig_";
            //var RedisKey = RedisKeyH + gameCode;
            ////var flag = RedisHelperEx.KeyExists(gameCode);
            ////var v = "";
            //var Game = new LotteryGame();
            //var db = RedisHelperEx.DB_Other;
            ////if (flag)
            ////{
            //Game = db.GetObj<LotteryGame>(RedisKey);
            //    //Game.GameCode = v;
            ////}
            //if (Game==null)
            //{
            var LotteryGame = SDB.CreateQuery<LotteryGame>().Where(p => p.GameCode == gameCode).FirstOrDefault();
            //    Game = LotteryGame;
            //    if (LotteryGame != null)
            //    {
            //        db.SetObj(RedisKey, LotteryGame,TimeSpan.FromMinutes(3));
            //    }
            //}
            //return Game;
            return LotteryGame;
        }

        #endregion
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
        #region 分页常量
        public const int MaxPageSize = 200;
        #endregion
        /// <summary>
        /// 资金分类——购彩
        /// </summary>
        public const string FundCategory_SettlementBonus = "结算分红";
        /// <summary>
        /// 充值到第三方游戏
        /// </summary>
        public const string FundCategory_GameRecharge = "游戏充值";

        /// <summary>
        /// 充值到第三方游戏
        /// </summary>
        public const string FundCategory_GameWithdraw = "游戏提款";

        #endregion




        private static List<C_Activity_PluginClass> _enablePluginClass = new List<C_Activity_PluginClass>();





        public List<C_Activity_PluginClass> QueryPluginClass(bool isEnable)
        {

            return DB.CreateQuery<C_Activity_PluginClass>().Where(p => p.IsEnable == isEnable).OrderBy(p => p.OrderIndex).ToList();
        }



        #region 成长值 和 澳彩豆豆

        /// <summary>
        /// 收入 --添加用户成长值
        /// 返回用户vip等级
        /// </summary>
        public static int Payin_UserGrowth(string category, string orderId, string userId, int userGrowth, string summary)
        {
            if (userGrowth <= 0) return 0;

            var balanceManager = new LocalLoginBusiness();
            //var fundManager = new FundManager();
            var user = balanceManager.GetRegisterById(userId);
            var userBalance = balanceManager.QueryUserBalanceInfo(userId);
            var Fund_UserGrowthDetail = new C_Fund_UserGrowthDetail()
            {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = userBalance.UserGrowth,
                PayMoney = userGrowth,
                PayType = (int)EntityModel.Enum.PayType.Payin,
                Summary = summary,
                AfterBalance = userBalance.UserGrowth + userGrowth,
            };
            SDB.GetDal<C_Fund_UserGrowthDetail>().Add(Fund_UserGrowthDetail);

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.UserGrowth,
                PayMoney = userGrowth,
                PayType = EntityModel.Enum.PayType.Payin,
            });

            var vipLevel = GetUserVipLevel(userBalance.UserGrowth + userGrowth);
            //更新成长值
            //userBalance.UserGrowth += userGrowth;
            //balanceManager.UpdateUserBalance(userBalance);
            PayToUserBalance(userId, payDetailList.ToArray());
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
                SDB.GetDal<C_User_Register>().Update(user);
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

        #endregion


        /// <summary>
        /// 支付到用户余额
        /// </summary>
        public static void PayToUserBalance(string userId, params PayDetail[] array)
        {
            if (array.Length <= 0)
                return;

            var setList = new List<string>();
            Expression<Func<C_User_Balance, bool>> where;
            Expression<Func<C_User_Balance, C_User_Balance>> update;
            where = b => b.UserId == userId;
            foreach (var item in array)
            {
                switch (item.AccountType)
                {
                    case AccountType.Bonus:
                        setList.Add(string.Format(" [BonusBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        //update = b => new C_User_Balance {
                        //    BonusBalance= GetOperFun(item.PayType),                            item.PayMoney)
                        //};

                        break;
                    case AccountType.Freeze:
                        setList.Add(string.Format(" [FreezeBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Commission:
                        setList.Add(string.Format(" [CommissionBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.FillMoney:
                        setList.Add(string.Format(" [FillMoneyBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Experts:
                        setList.Add(string.Format(" [ExpertsBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.RedBag:
                        setList.Add(string.Format(" [RedBagBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.UserGrowth:
                        setList.Add(string.Format(" [UserGrowth]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.DouDou:
                        setList.Add(string.Format(" [CurrentDouDou]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    default:
                        break;
                }
            }

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 where userid='{1}'", string.Join(",", setList), userId);
            // DB.CreateSQLExc();
            var result = SDB.CreateSQLQuery(sql).Excute();
            Console.WriteLine("result:" + result);
        }

        private static string GetOperFun(EntityModel.Enum.PayType p)
        {
            switch (p)
            {
                case EntityModel.Enum.PayType.Payin:
                    return "+=";
                case EntityModel.Enum.PayType.Payout:
                    return "-=";
            }
            throw new LogicException("PayType类型不正确");
        }

        public static void Payin_To_Balance(AccountType accountType, string category, string userId, string orderId, decimal payMoney, string summary, RedBagCategory redBag = RedBagCategory.FillMoney, string operatorId = "")
        {
            //if (accountType == AccountType.Freeze)
            //    throw new LogicException("退款账户不能为冻结账户");

            if (payMoney <= 0M)
                return;
            //throw new LogicException("转入金额不能小于0.");

            var balanceManager = new LocalLoginBusiness();
            var fundManager = new FundManager();
            //查询帐户余额

            var userBalance = balanceManager.QueryUserBalanceInfo(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = accountType,
                PayMoney = payMoney,
                PayType = EntityModel.Enum.PayType.Payin,
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
                    var RedBagDetail = new C_Fund_RedBagDetail
                    {
                        CreateTime = DateTime.Now,
                        OrderId = orderId,
                        RedBagCategory = (int)redBag,
                        RedBagMoney = payMoney,
                        UserId = userId,
                    };
                    fundManager.AddRedBagDetail(RedBagDetail);
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
                    throw new LogicException("不支持的账户类型 - " + accountType);
            }
            var FundDetail = new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)accountType,
                PayMoney = payMoney,
                PayType = (int)EntityModel.Enum.PayType.Payin,
                Summary = summary,
                UserId = userId,
                BeforeBalance = before,
                AfterBalance = after,
                OperatorId = string.IsNullOrEmpty(operatorId) ? userId : operatorId,
            };
            fundManager.AddFundDetail(FundDetail);
            //balanceManager.UpdateUserBalance(userBalance);
            PayToUserBalance(userId, payDetailList.ToArray());
        }

        /// <summary>
        ///  用户支出，申请提现
        /// </summary>
        public WithdrawCategory Payout_To_Frozen_Withdraw1(string category, string userId, string orderId, decimal payoutMoney,
            string summary, string place, string password, out decimal responseMoney)
        {
            var requestMoney = payoutMoney;
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

            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = PayType.Payin,
            });
            //冻结资金明细
            fundManager.AddFundDetail(new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = (int)PayType.Payin,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                    throw new LogicException("可用充值金额不足");

                #region 异常提现

                //收取5%手续费
                var percent = decimal.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("WithdrawAboutFillMoney.CutPercent"));
                var counterFee = payoutMoney * percent / 100;
                //到帐金额
                responseMoney = requestMoney - counterFee;

                //手续费明细
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = BusinessHelper.FundCategory_RequestWithdrawCounterFee,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = counterFee,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = resMoney,
                    PayType = (int)PayType.Payout,
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
            PayToUserBalance(userId, payDetailList.ToArray());

            return payCategory;
        }


        /// <summary>
        /// 用户退款,用于用户支出后，因业务原因退款
        /// 调用前提：必须有已支付过的订单，按订单支付的相应账号退还金额
        /// </summary>
        public static void Payback_To_Balance(string category, string userId, string orderId, decimal payBackMoney, string summary)
        {
            if (payBackMoney <= 0M)
                throw new LogicException("退款金额不能小于0.");
            //查询帐户余额
            var balanceManager = new UserBalanceManager();
            var fundManager = new FundManager();
            //资金密码判断
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null) { throw new LogicException("用户帐户不存在 - " + userId); }

            var fundList = fundManager.QueryFundDetailList(orderId, userId);
            if (fundList == null || fundList.Count == 0)
                throw new LogicException(string.Format("未查询到用户{0}的订单{1}的支付明细", userId, orderId));
            //if (fundList.Sum(p => p.PayMoney) < payBackMoney)
            //    throw new LogicException("退款金额大于订单总支付金额");

            //退款顺序：名家=>佣金=>奖金=>红包=>充值金额
            #region 按顺序退款

            var currentPayBack = 0M;
            var payDetailList = new List<PayDetail>();
            var expertFund = fundList.Where(p => p.AccountType == (int)AccountType.Experts).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance += currentPayBack;
            }

            var commisionFund = fundList.Where(p => p.AccountType == (int)AccountType.Commission).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance += currentPayBack;
            }

            var bonusFund = fundList.Where(p => p.AccountType == (int)AccountType.Bonus).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance += currentPayBack;
            }

            var redBagFund = fundList.Where(p => p.AccountType == (int)AccountType.RedBag).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance += currentPayBack;
            }

            var fillFund = fundList.Where(p => p.AccountType == (int)AccountType.FillMoney).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.FillMoneyBalance,
                    AfterBalance = userBalance.FillMoneyBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.FillMoneyBalance += currentPayBack;
            }
            //if (payBackMoney > 0M)
            //    throw new LogicException("退款金额大于总支付金额");

            #endregion

            //balanceManager.UpdateUserBalance(userBalance);
            balanceManager.PayToUserBalance(userId, payDetailList.ToArray());
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
            fundManager.AddFundDetail(new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)AccountType.Freeze,
                PayMoney = payoutMoney,
                PayType = (int)PayType.Payin,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayout,
                    PayType = (int)PayType.Payout,
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

        public static void CheckDisableGame(string gameCode, string gameType)
        {
            var status = new GameBusiness().LotteryGameToStatus(gameCode);
            if (status != EnableStatus.Enable)
                throw new LogicException("彩种暂时不能投注");
        }

        public static void CheckBalance(string userId, string password = "", string place = "")
        {
            var balanceManager = new UserBalanceManager();
            var userBalance = balanceManager.QueryUserBalance(userId);
            if (userBalance == null)
                throw new LogicException("未查询到您的账户信息");
            else if ((userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.BonusBalance + userBalance.CommissionBalance) <= 0)
                throw new LogicException("对不起！您当前的余额不足");
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
                            throw new LogicException("资金密码输入错误");
                        }
                    }
                }
            }
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
            fundManager.AddOCDouDouDetail(new C_Fund_OCDouDouDetail
            {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = user.CurrentDouDou,
                PayMoney = doudou,
                PayType = (int)PayType.Payin,
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

        public static bool AddToRunningOrder(C_Sports_Order_Running order)
        {
            try
            {
                var manager = new Sports_Manager();
                string schemeId = order.SchemeId;
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
                        BonusStatus = (BonusStatus)ticket.BonusStatus,
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
                    RedisOrderBusiness.AddToRunningOrder_BJDC(schemeId, matchIdArray, redisTicketList);
                }
                else if (new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode))
                {
                    //竞彩
                    RedisOrderBusiness.AddToRunningOrder_JC(order.GameCode, schemeId, matchIdArray, redisTicketList);
                }
                else
                {
                    //传统足球或数字彩
                    var keyLine = schemeId;
                    var stopAfterBonus = true;
                    if ((SchemeType)order.SchemeType == SchemeType.ChaseBetting)
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
                    RedisOrderBusiness.AddToRunningOrder_SZC((SchemeType)order.SchemeType, order.GameCode, order.GameType, schemeId, keyLine, stopAfterBonus, order.IssuseNumber, redisTicketList);
                }
                return true;
            }
            catch
            {

            }
            return false;
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
            var expertFund = fundList.Where(p => p.AccountType == (int)AccountType.Experts).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Experts,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.ExpertsBalance,
                    AfterBalance = userBalance.ExpertsBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.ExpertsBalance += currentPayBack;
            }

            var commisionFund = fundList.Where(p => p.AccountType == (int)AccountType.Commission).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Commission,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.CommissionBalance,
                    AfterBalance = userBalance.CommissionBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.CommissionBalance += currentPayBack;
            }

            var bonusFund = fundList.Where(p => p.AccountType == (int)AccountType.Bonus).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.Bonus,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.BonusBalance,
                    AfterBalance = userBalance.BonusBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.BonusBalance += currentPayBack;
            }

            var redBagFund = fundList.Where(p => p.AccountType == (int)AccountType.RedBag).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.RedBag,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
                    Summary = summary,
                    UserId = userId,
                    BeforeBalance = userBalance.RedBagBalance,
                    AfterBalance = userBalance.RedBagBalance + currentPayBack,
                    OperatorId = userId,
                });
                //userBalance.RedBagBalance += currentPayBack;
            }

            var fillFund = fundList.Where(p => p.AccountType == (int)AccountType.FillMoney).ToList();
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
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType = (int)AccountType.FillMoney,
                    PayMoney = currentPayBack,
                    PayType = (int)PayType.Payin,
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

            fundManager.AddFundDetail(new C_Fund_Detail
            {
                Category = category,
                CreateTime = DateTime.Now,
                KeyLine = orderId,
                OrderId = orderId,
                AccountType = (int)AccountType.Freeze,
                PayMoney = payMoney,
                PayType = (int)PayType.Payout,
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
        /// 手工充值编号
        /// </summary>
        public static string GetManualFillMoneyId()
        {
            string prefix = "MFM";
            return prefix + UsefullHelper.UUID();
        }
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
                var result = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
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
                //SaveSMSHistoryRecord(info);
            }
        }
        //public static void SaveSMSHistoryRecord(SendMsgHistoryRecordInfo info)
        //{
        //    var manager = new UserBalanceManager();
        //    if (info != null)
        //    {
        //        if (info.MsgId > 0)//后台列表手工发送时修改回执状态即可
        //        {
        //            var entity = manager.QueryMsgHistoryRecordByMsgId(info.MsgId);
        //            if (entity != null)
        //            {
        //                entity.SendTime = DateTime.Now;
        //                entity.MsgStatusDesc = info.MsgStatusDesc;
        //                entity.MsgResultStatus = info.MsgResultStatus;
        //                entity.SMSId = info.SMSId;
        //                entity.SendNumber += 1;
        //                manager.UpdateMsgHistoryRecord(entity);
        //            }
        //        }
        //        else
        //        {
        //            info.CreateTime = DateTime.Now;
        //            info.SendTime = DateTime.Now;
        //            info.SendNumber = 0;
        //            SendMsgHistoryRecord entity = new SendMsgHistoryRecord();
        //            ObjectConvert.ConverInfoToEntity(info, ref entity);
        //            manager.AddSendMsgHistoryRecord(entity);
        //        }
        //    }
        //}
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
        #endregion
    }
}
