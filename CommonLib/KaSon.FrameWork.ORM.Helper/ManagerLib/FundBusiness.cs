using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Sport;

namespace KaSon.FrameWork.ORM.Helper
{
    public class FundBusiness : DBbase
    {
        public UserBalanceInfo QueryUserBalance(string userId)
        {

            var manager = new UserBalanceManager();
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

        /// <summary>
        /// 申请提现
        /// 实际添加提现记录，扣除用户资金
        /// </summary>
        public void RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string balancepwd)
        {
            var userManager = new UserBalanceManager();
            var user = userManager.QueryUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");
            var orderId = BettingHelper.GetWithdrawId();
           
            DB.Begin();
            try
            {

                var resonseMoney = 0M;
               
                BusinessHelper businessHelper = new BusinessHelper();
                var category = businessHelper.Payout_To_Frozen_Withdraw(BusinessHelper.FundCategory_RequestWithdraw, userId, orderId, info.RequestMoney
                      , string.Format("申请提现：{0:N2}元", info.RequestMoney), "Withdraw", balancepwd, out resonseMoney);

                var fundManager = new FundManager();
                fundManager.AddWithdraw (new C_Withdraw
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
                    WithdrawAgent = (int)info.WithdrawAgent,
                    Status = (int)WithdrawStatus.Requesting,
                     
                    WithdrawCategory = (int)category,
                    ResponseMoney = resonseMoney,
                });

                //查询到账金额
                //var wi = GetWithdrawById(orderId);


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
                        //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        //writer.Write("关于DP日志", "关于DP日志", Common.Log.LogType.Information, "关于DP日志", "withdrawHigthMoney===============" + withdrawHigthMoney);

                        String str = String.Format("BankCardNumber=" + info.BankCardNumber + ",BankCode=" + info.BankCode + ",BankName=" + info.BankName + ",BankSubName=" + info.BankSubName + ",CityName=" + info.CityName + ",ProvinceName=" + info.ProvinceName + ",RequestMoney=" + info.RequestMoney + ",userRealName=" + info.userRealName + ",WithdrawAgent=" + info.WithdrawAgent + "");
                        //writer.Write("输出参数写测试用例", "输出参数写测试用例", Common.Log.LogType.Information, "输出参数写测试用例", "输出参数写测试用例===============" + str);
                        WithdrawApplyInfo wai = new WithdrawApplyInfo();
                        wai.company_order_num = orderId;
                        wai.company_user = userId;
                        wai.card_name = info.userRealName;
                        wai.card_num = info.BankCardNumber;
                        wai.issue_bank_name = info.BankName;
                        wai.issue_bank_address = info.BankSubName;
                        wai.memo = "";
                        String amount = resonseMoney.ToString();
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
                                DB.Commit();
                                //刷新余额
                                BusinessHelper.RefreshRedisUserBalance(userId);
                                return;
                            }
                            dpresult = Withdrawal(wai, ck.ConfigValue, cw.ConfigValue);
                            if (dpresult == null || dpresult == "")
                            {
                                DB.Rollback();
                                throw new Exception("服务繁忙，请稍后重试,如多次尝试失败,请在客服服务时间内联系在线客服咨询");
                            }
                        }


                    }

                }

                #region 发送站内消息：手机短信或站内信
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[OrderId]", orderId));
                pList.Add(string.Format("{0}={1}", "[UserName]", ""));
                pList.Add(string.Format("{0}={1}", "[RequestMoney]", info.RequestMoney));
                pList.Add(string.Format("{0}={1}", "[ResponseMoney]", resonseMoney));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(userId, "", "ON_User_Request_Withdraw", pList.ToArray());

                #endregion

                DB.Commit();
                //刷新余额
                BusinessHelper.RefreshRedisUserBalance(userId);
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
          


          


        }

        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
            return new SqlQueryManager().GetWithdrawById(orderId);
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
            String key = getKey(wai, config);

            String param = "company_id=" + wai.company_id + "&bank_id=" + wai.bank_id + "&company_order_num=" + wai.company_order_num + "&amount=" + wai.amount + "&card_num=" + wai.card_num
                + "&card_name=" + HttpUtil.UrlEncode(wai.card_name.Trim()) + "&company_user=" + wai.company_user + "&issue_bank_name=" + HttpUtil.UrlEncode(wai.issue_bank_name.Trim()) + "&issue_bank_address=" + HttpUtil.UrlEncode(wai.issue_bank_address.Trim()) +
                "&memo=" + wai.memo + "&key=" + key + "";

            try
            {
                String result = HttpUtil.Post(url, param);
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("关于DP日志", "关于DP日志", Common.Log.LogType.Information, "关于DP日志", "申请DP返回参数===============" + result);
                if (result != null)
                {
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //var war = js.Deserialize<WithdrawApplyResult>(result);
                    //if (war.status != 1)
                    //{
                    //    //Common.Log.LogWriterGetter.GetLogWriter().Write("提现请求参数", "提现请求参数", Common.Log.LogType.Information, "提现请求参数", "提现请求参数===============" + param);
                    //    return null;
                    //}
                }
                return result;

            }
            catch (Exception e)
            {

                //Common.Log.LogWriterGetter.GetLogWriter().Write("提现请求参数", param, e);
                return null;
            }

        }

        public static String getKey(WithdrawApplyInfo wai, String config)
        {

            string ubkey = Encipherment.MD5(Encipherment.MD5(config) + wai.company_id + wai.bank_id + wai.company_order_num.Trim() + wai.amount + wai.card_num.Trim() + wai.card_name.Trim() + wai.company_user.Trim() + wai.issue_bank_name.Trim() + wai.issue_bank_address.Trim() + wai.memo);
            return ubkey;
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
                out winCount, out refusedCount, out totalWinMoney, out totalRefusedMoney, out totalResponseMoney, out totalCount, out totalMoney);
            result.TotalCount = totalCount;
            result.TotalMoney = totalMoney;
            result.WinCount = winCount;
            result.RefusedCount = refusedCount;
            result.TotalWinMoney = totalWinMoney;
            result.TotalRefusedMoney = totalRefusedMoney;
            result.TotalResponseMoney = totalResponseMoney;
            return result;
        }

        public C_Bank_Info QueryBankInfo(string bankCode) {

          return DB.CreateQuery<C_Bank_Info>().Where(p => p.BankCode == bankCode && p.Disabled==false).FirstOrDefault();
        }
    }
}
