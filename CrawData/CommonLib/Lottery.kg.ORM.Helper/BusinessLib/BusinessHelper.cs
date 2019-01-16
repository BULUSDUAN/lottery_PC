using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using System.Linq.Expressions;

using EntityModel.Domain.Entities;


using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper.AnalyzerFactory;
using EntityModel.CoreModel.BetingEntities;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;

namespace KaSon.FrameWork.ORM.Helper
{
   

    public class BusinessHelper:DBbase
    {

        private const string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        private static Log4Log writerLog = new Log4Log();

        private static AssemblyRefHelper AssRef = new AssemblyRefHelper();
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
                var json = JsonHelper.Serialize(userBalance);
                db.StringSetAsync(key, json, TimeSpan.FromSeconds(60 * 2));
            }
            catch (Exception ex)
            {
                writerLog.ErrrorLog("BusinessHelper-RefreshRedisUserBalance", ex);
            }
        }
        //待测试
        //kason
         public static void ExecPlugin<T>(object inputParam)
                  where T : class
        {
            if (_enablePluginClass == null || _enablePluginClass.Count == 0)
                _enablePluginClass = new PluginClassManager().QueryPluginClass(true);


            foreach (var plugin in _enablePluginClass)
            {
                try
                {
                    if (typeof(T).FullName != plugin.InterfaceName) continue;

                    if (plugin.StartTime !=null && plugin.StartTime > DateTime.Now) continue;//未开始
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
                    var type = AssemblyRefHelper.GetType(classname, snamespace);
                    if (type == null)
                    {
                        throw new ArgumentNullException("类型在当前域中不存在，或对应组件未加载：" + fullName);
                    }

                    object instance = Activator.CreateInstance(type); //创建实例
                    //var i = Type.GetType(fullName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as T;
                    if (instance == null)
                    {
                        throw new ArgumentNullException("无法实例化对象：" + fullName);
                    }
                    //new Thread(() =>
                    //{
                    try
                    {
                        type.GetMethod("ExecPlugin").Invoke(instance, new object[] { inputParam });
                       // i.ExecPlugin(typeof(T).Name, inputParam);
                    }
                    catch (AggregateException ex)
                    {
                        throw new AggregateException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        writerLog.WriteLog("ERROR_ExecPlugin", "_ExecPlugin", (int)LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                    }
                    //}).Start();
                }
                catch (AggregateException ex)
                {
                    throw new AggregateException(ex.Message);
                }
                catch (Exception ex)
                {
                  //  var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    writerLog.WriteLog("ERROR_ExecPlugin", "_ExecPlugin", (int)LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                }
            }

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
                    PayType =EntityModel.Enum.PayType.Payout
                });
                fundManager.AddFundDetail(new C_Fund_Detail
                {
                    Category = category,
                    CreateTime = DateTime.Now,
                    KeyLine = orderId,
                    OrderId = orderId,
                    AccountType =(int) AccountType.Bonus,
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

        public static string GetSportsBettingSchemeId(string gameCode)
        {
            string prefix = gameCode;
            return prefix + UsefullHelper.UUID();
        }
        private static Dictionary<string, string> _stopTime = new Dictionary<string, string>();
        /// <summary>
        /// 查询指定彩是否可以出票
        /// </summary>
        public static bool CanRequestBet(string gameCode)
        {
            try
            {   

                string value = "";// ConfigurationManager.AppSettings[key];
                var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
                if (!_stopTime.ContainsKey(key))
                {
                    _stopTime.Add(key, value);
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

        #region 普通投注

        /// <summary>
        /// 验证投注号码 返回实际投注注数 by dzq
        /// </summary>
        public static int CheckBetCode(string userId, string gameCode, string gameType, SchemeSource schemeSource, string playType, int amount, decimal schemeTotalMoney, EntityModel.CoreModel.BetingEntities.Sports_AnteCodeInfoCollection anteCodeList)
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
                AnalyzerFactory.AnalyzerFactory.GetSportAnteCodeChecker(gameCode, item.GameType).CheckAntecodeNumber(item, out error);
                if (!string.IsNullOrEmpty(error))
                    throw new Exception(error);

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
        /// 检查彩种是否开启
        /// </summary>
        public static void CheckGameEnable(string gameCode)
        {
            var game = QueryLotteryGame(gameCode);
            if (game == null)
                throw new Exception(string.Format("错误的彩种编码：{0}", gameCode));
            if (game.EnableStatus != EntityModel.Enum.EnableStatus.Enable)
                throw new Exception(string.Format("{0} 暂停销售", game.DisplayName));
        }
        public static void CheckGameCodeAndType(string gameCode, string gameType)
        {
            if (gameCode.ToUpper() == "BJDC")
            {
                if (new string[] { "ZJQ", "SXDS", "BQC", "BF" }.Contains(gameType.ToUpper()))
                    throw new Exception("该玩法暂停销售");
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
                //var manager = new LotteryGameManager();
               var p= SDB.CreateQuery<LotteryGame>().ToList();
                _cacheAllGameList.AddRange(p);
            }
            return _cacheAllGameList.FirstOrDefault(p => p.GameCode == gameCode);
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


        #endregion

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
                            return "14场胜负";
                        case "TR9":
                            return "胜负任9";
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


        private static List<C_Activity_PluginClass> _enablePluginClass = new List<C_Activity_PluginClass>();


     
        public interface IPlugin
        {
            object ExecPlugin(string type, object inputParam);
        }


        public List<C_Activity_PluginClass> QueryPluginClass(bool isEnable)
        {

            return DB.CreateQuery<C_Activity_PluginClass>().Where(p => p.IsEnable == isEnable).OrderBy(p => p.OrderIndex).ToList();
        }

        /// <summary>
        /// 资金收入/支出明细
        /// </summary>
        public class PayDetail
        {
            public AccountType AccountType { get; set; }
            public EntityModel.Enum.PayType PayType { get; set; }
            public decimal PayMoney { get; set; }
        }

        #region 成长值 和 澳彩豆豆

        /// <summary>
        /// 收入 --添加用户成长值
        /// 返回用户vip等级
        /// </summary>
        public int Payin_UserGrowth(string category, string orderId, string userId, int userGrowth, string summary)
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
            DB.GetDal<C_Fund_UserGrowthDetail>().Add(Fund_UserGrowthDetail);

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
                DB.GetDal<C_User_Register>().Update(user);
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
        public void PayToUserBalance(string userId, params PayDetail[] array)
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

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 where userid='{1};select 1'", string.Join(",", setList), userId);
            // DB.CreateSQLExc();
            var result = DB.CreateSQLQuery(sql).First<int>();
            Console.WriteLine("result:" + result);
        }

        private string GetOperFun(EntityModel.Enum.PayType p)
        {
            switch (p)
            {
                case EntityModel.Enum.PayType.Payin:
                    return "+=";
                case EntityModel.Enum.PayType.Payout:
                    return "-=";
            }
            throw new Exception("PayType类型不正确");
        }

        public  void Payin_To_Balance(AccountType accountType, string category, string userId, string orderId, decimal payMoney, string summary, RedBagCategory redBag = RedBagCategory.FillMoney, string operatorId = "")
        {
            //if (accountType == AccountType.Freeze)
            //    throw new Exception("退款账户不能为冻结账户");

            if (payMoney <= 0M)
                return;
            //throw new Exception("转入金额不能小于0.");
            
            var balanceManager = new LocalLoginBusiness();
            var fundManager = new FundManager();
            //查询帐户余额
          
            var userBalance = balanceManager.QueryUserBalanceInfo(userId);
            if (userBalance == null) { throw new Exception("用户帐户不存在 - " + userId); }

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
                    var RedBagDetail=new C_Fund_RedBagDetail
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
                    throw new ArgumentException("不支持的账户类型 - " + accountType);
            }
            var FundDetail=new C_Fund_Detail
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
    }
}
