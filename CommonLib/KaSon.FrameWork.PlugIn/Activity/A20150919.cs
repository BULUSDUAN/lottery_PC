using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Redis;
using EntityModel.Redis;

namespace KaSon.FrameWork.PlugIn.Activity
{
    /// <summary>
    /// 注册后，绑定银行卡、身份证、手机号 后赠送红包
    /// </summary>
    public class A20150919 : DBbase,IAddBankCard, IResponseAuthentication_AfterTranCommit
    {
        /// <summary>
        /// 添加银行卡绑定 赠送1.5元彩金
        /// </summary>
        public void AddBankCard(string userId, string bankCardNumber, string bankCode, string bankName, string bankSubName, string cityName, string provinceName, string realName)
        {
            //return;
            if (string.IsNullOrEmpty(userId)) return;
            if (string.IsNullOrEmpty(bankCardNumber)) return;
            if (string.IsNullOrEmpty(bankCode)) return;

            var manager = new A20150919Manager();
            var old = manager.QueryByUserId(userId);
            if (old == null)
            {
                //活动表中没有数据
                manager.AddA20150919_注册绑定送红包(new E_A20150919_注册绑定送红包
                {
                    UserId = userId,
                    CreateTime = DateTime.Now,
                    GiveRedBagMoney = 0M,
                    IsBindBankCard = true,
                    IsBindMobile = false,
                    IsBindRealName = false,
                    IsGiveRedBag = false,
                });
            }
            else
            {
                //活动表中有数据
                old.IsBindBankCard = true;
                if (old.IsBindBankCard && !old.IsBonus)// && old.IsBindMobile && old.IsBindRealName 
                {
                    //三种都已绑定，执行赠送
                    //DoGiveMoney(userId, old, manager);

                    //首次绑定银行卡 赠送1.5元现金
                    DoBindBankCardGiveMoney(userId, old, manager);
                }
                else
                    manager.UpdateA20150919_注册绑定送红包(old);
            }

            //开启事务

            try
            {
                DB.Begin();
                //分享推广送绑定了卡的送红包
                //绑定卡了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
                var entityBankCard = new BankCardManager().BankCardById(userId);
                var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
                if (entityBankCard != null && entityShareSpread != null && !entityShareSpread.isGiveRegisterRedBag)
                {
                    //该用户首次绑定卡 没有给分享者送活动红包 就执行送红包
                    var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FirstBindBankCardGiveRedBagTofxid").ConfigValue);
                    if (giveFillMoney > 0)
                    {
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
                                          , string.Format("{1}用户首次绑定银行卡赠送红包给分享推广用户{0}元", giveFillMoney, userId), RedBagCategory.FxidRegister);

                        entityShareSpread.isGiveRegisterRedBag = true;
                        entityShareSpread.UpdateTime = DateTime.Now;
                        entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
                        new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            


        }

        /// <summary>
        /// 首次绑定银行卡 赠送1.5元现金(奖金账户)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="record"></param>
        /// <param name="manager"></param>
        private void DoBindBankCardGiveMoney(string userId, E_A20150919_注册绑定送红包 record, A20150919Manager manager)
        {
            var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FirstBindBankCardGiveFillMoney").ConfigValue);
            if (giveFillMoney > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Activity, userId, Guid.NewGuid().ToString("N"), giveFillMoney
                    , string.Format("用户首次绑定银行卡赠送彩金{0}元", giveFillMoney), RedBagCategory.Activity);

                record.IsBonus = true;
                record.IsGiveRedBag = true;
                record.GiveBonusMoney = giveFillMoney;
                manager.UpdateA20150919_注册绑定送红包(record);
            }
        }

        /// <summary>
        /// 实名和手机认证
        /// </summary>
        public void ResponseAuthentication_AfterTranCommit(string userId, string authenticationType, string authenticationInfo, SchemeSource source)
        {
            if (string.IsNullOrEmpty(userId)) return;
            if (authenticationType.ToLower() != "realname" && authenticationType.ToLower() != "mobile") return;
            var manager = new A20150919Manager();
            var old = manager.QueryByUserId(userId);
            if (old == null)
            {
                //活动表中没有数据
                manager.AddA20150919_注册绑定送红包(new E_A20150919_注册绑定送红包
                {
                    UserId = userId,
                    CreateTime = DateTime.Now,
                    GiveRedBagMoney = 0M,
                    IsBindBankCard = false,
                    IsBindMobile = authenticationType.ToLower() == "mobile",
                    IsBindRealName = authenticationType.ToLower() == "realname",
                    IsGiveRedBag = false,
                });
            }
            else
            {
                //活动表中有数据
                if (authenticationType.ToLower() == "mobile")
                {
                    old.IsBindMobile = true;
                }
                if (authenticationType.ToLower() == "realname")
                {
                    old.IsBindRealName = true;
                }
                //if (old.IsBindBankCard && old.IsBindMobile && old.IsBindRealName && !old.IsGiveRedBag)
                if (old.IsBindMobile && old.IsBindRealName && !old.IsGiveRedBag)
                {
                    //三种都已绑定，执行赠送
                    DoGiveMoney(userId, old, manager);
                }
                else
                    manager.UpdateA20150919_注册绑定送红包(old);

            }
        }

        private void DoGiveMoney(string userId, E_A20150919_注册绑定送红包 record, A20150919Manager manager)
        {
            var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.RegistAndBindGiveFillMoney").ConfigValue);
            if (giveFillMoney > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, userId, Guid.NewGuid().ToString("N"), giveFillMoney
                    , string.Format("用户绑定手机、身份证赠送彩金{0}元", giveFillMoney), RedBagCategory.Activity);

                record.IsGiveRedBag = true;
                record.GiveRedBagMoney = giveFillMoney;
                manager.UpdateA20150919_注册绑定送红包(record);
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IAddBankCard":
                        AddBankCard((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (string)paraList[5], (string)paraList[6], (string)paraList[7]);
                        break;
                    case "IResponseAuthentication_AfterTranCommit":
                        ResponseAuthentication_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (SchemeSource)paraList[3]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("EXEC_Plugin_A20120925_Authen_Error_", type, ex);
            }

            return null;
        }
    }

    //增加一个IRegister_AfterTranCommit插件 根据yqid每注册好一个用户就自动统计一次，确保及时送红包
    //实名，绑定卡，充钱，购彩这四个条件
    //邀请了10个人领5元红包，（这个5元红包可以在后台配置任意数字），20人就10元红包这样下去，红包直接到账用户的红包账户上去（统计返回的数字是能被10整除的数字就送红包）
    //建一个推广送红包的表 自增id agendId giveRedBagMoney userCount(满足条件的会员个数) redBagCount（领取的红包个数） CreateTime
    //直接用红包明细表C_Fund_RedBagDetail 明细类型加一个推广类型
    //简单思路：根据条件查询到满足的话就送红包
    /// <summary>
    /// 
    /// </summary>
    public class Yqid_Regist_GiveRedBag : IyqidRegister_AfterTranCommit
    {
        public void AfterRegisterTranCommit(string yqid)
        {
            if (string.IsNullOrEmpty(yqid)) return;
            decimal giveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.YqidRegistGiveRedBag").ConfigValue);
            if (giveMoney > 0M)
            {
                var manager = new SqlQueryManager();
                var managerA = new BlogManager();
                var countStr = manager.QueryYqidRegisterByAgentId(yqid);//返回的是满足条件的会员个数
                int count = 0;
                count = Convert.ToInt32(countStr.Split('|')[0]);
                var entity = managerA.QueryBlog_UserSpreadGiveRedBag(yqid);
                if (entity == null)
                {
                    managerA.AddBlog_UserSpreadGiveRedBag(new E_Blog_UserSpreadGiveRedBag
                    {
                        CreateTime = DateTime.Now,
                        giveRedBagMoney = 0,
                        GiveBonusMoney = 0,
                        UserId = yqid,
                        userCount = 0,
                        redBagCount = 0,
                        userGiveCount = 0,
                        UpdateTime = DateTime.Now,
                    });
                    return;
                }
                //int redBagCount = entity.redBagCount;//返回领取了多少个红包（比如邀请了30人,应该领取3个红包） 
                if (count > 0 && count % 10 == 0 && entity.redBagCount < count / 10 && entity.userGiveCount < count)
                {
                    BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, yqid, Guid.NewGuid().ToString("N"), giveMoney
                                       , string.Format("邀请注册的符合领红包规则的会员{1}个,领取第{2}个10人红包{0}元", giveMoney, count, count / 10), RedBagCategory.YqidRegister);

                    managerA.UpdateBlog_UserSpreadGiveRedBag(new E_Blog_UserSpreadGiveRedBag
                    {
                        UpdateTime = DateTime.Now,
                        redBagCount = entity.redBagCount + 1,
                        userGiveCount = count,
                        giveRedBagMoney = entity.giveRedBagMoney + giveMoney,
                    });
                }
                else
                {
                    managerA.UpdateBlog_UserSpreadGiveRedBag(new E_Blog_UserSpreadGiveRedBag
                    {
                        UpdateTime = DateTime.Now,
                        userCount = count,
                    });
                }

            }
        }
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IyqidRegister_AfterTranCommit":
                        AfterRegisterTranCommit((string)paraList[0]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("IyqidRegister_Error_", type, ex);
            }
            return null;
        }
    }

    /// <summary>
    /// 注册就送5元红包
    /// </summary>
    public class A20150919_Regist_GiveRedBag : IRegister_AfterTranCommit
    {
        public void AfterRegisterTranCommit(string regType, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var manager = new A20150919Manager();
            decimal giveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.RegistAndBindGiveRedBag").ConfigValue);
            if (giveMoney > 0M)
            {
                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, userId, Guid.NewGuid().ToString("N"), giveMoney
                    , string.Format("用户注册赠送红包{0}元", giveMoney), RedBagCategory.Activity);

                manager.AddA20150919_注册送红包(new E_A20150919_注册送红包
                {
                    CreateTime = DateTime.Now,
                    GiveRedBagMoney = giveMoney,
                    UserId = userId,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IRegister_AfterTranCommit":
                        AfterRegisterTranCommit((string)paraList[0], (string)paraList[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("A20150919_Regist_GiveRedBag_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 用户绑定身份信息和手机信息，每天登录赠送红包
    /// </summary>
    public class A20150919_BindRealName_Mobile_Login : IUser_AfterLogin
    {
        public void User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime)
        {
            if (string.IsNullOrEmpty(userId)) return;
            var date = DateTime.Today.ToString("yyyyMMdd");
            //注册当前登录不送红包
            var user = new UserBalanceManager().QueryUserRegister(userId);
            if (user.CreateTime.ToString("yyyyMMdd") == date)
                return;


            var bizRealName = new RealNameAuthenticationBusiness();
            var realName = bizRealName.GetAuthenticatedRealName(userId);
            if (realName == null)
                return;
            var bizMoible = new MobileAuthenticationBusiness();
            var mobile = bizMoible.GetAuthenticatedMobile(userId);
            if (mobile == null || !mobile.IsSettedMobile)
                return;

            var manager = new A20150919Manager();
            var record = manager.QueryA20150919_已绑定身份和手机的用户登录送红包(userId, date);
            if (record != null) return;

            //var old = manager.QueryByUserId(userId);
            //if (old == null) return;
            //if (!old.IsBindRealName) return;
            //if (!old.IsBindMobile) return;

            decimal giveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.BindedUserLoginGiveRedBag").ConfigValue);
            if (user.VipLevel >= 0)
            {
                var config = ActivityCache.QueryActivityConfig(string.Format("ActivityConfig.BindedUserLoginGiveRedBagV{0}", user.VipLevel));
                if (config != null)
                {
                    try
                    {
                        giveMoney = decimal.Parse(config.ConfigValue);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (giveMoney > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, userId, Guid.NewGuid().ToString("N"), giveMoney
                  , string.Format("绑定身份和手机后，VIP{1} 每天登录赠送红包{0}元", giveMoney, user.VipLevel), RedBagCategory.Activity);
                manager.AddA20150919_已绑定身份和手机的用户登录送红包(new E_A20150919_已绑定身份和手机的用户登录送红包
                {
                    CreateTime = DateTime.Now,
                    UserId = userId,
                    LoginDate = date,
                    GiveRedBagMoney = giveMoney,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IUser_AfterLogin":
                        User_AfterLogin((string)paraList[0], (string)paraList[1], (string)paraList[2], (DateTime)paraList[3]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
                return null;
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("A20150919_BindRealName_Mobile_Login_Error_", type, ex);
            }

            return null;
        }


        public string MobileLoginRed(string userId)
        {
            var date = DateTime.Today.ToString("yyyyMMdd");
            //注册当前登录不送红包
            var user = new UserBalanceManager().QueryUserRegister(userId);
            if (user.CreateTime.ToString("yyyyMMdd") == date)
            {
                return "注册当天登录不送红包";
            }


            var bizRealName = new RealNameAuthenticationBusiness();
            var realName = bizRealName.GetAuthenticatedRealName(userId);
            if (realName == null)
            {
                return "未实名认证不送登录红包";
            }
            var bizMoible = new MobileAuthenticationBusiness();
            var mobile = bizMoible.GetAuthenticatedMobile(userId);
            if (mobile == null || !mobile.IsSettedMobile)
            {
                return "未绑定手机号不送登录红包";
            }

            var manager = new A20150919Manager();
            var record = manager.QueryA20150919_已绑定身份和手机的用户登录送红包(userId, date);
            if (record != null)
            {
                return "未绑定身份信息不送登录红包";
            }

            var entity = manager.QueryA20150919_已绑定身份和手机的用户登录送红包(userId, date);
            if (entity != null)
            {
                return "今天登录红包已经领取!";
            }

            decimal giveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.BindedUserLoginGiveRedBag").ConfigValue);
            if (user.VipLevel >= 0)
            {
                var config = ActivityCache.QueryActivityConfig(string.Format("ActivityConfig.BindedUserLoginGiveRedBagV{0}", user.VipLevel));
                if (config != null)
                {
                    try
                    {
                        giveMoney = decimal.Parse(config.ConfigValue);
                    }
                    catch (Exception)
                    {
                        return "系统异常，请联系管理员！";
                    }
                }
            }
            if (giveMoney > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, userId, Guid.NewGuid().ToString("N"), giveMoney
                  , string.Format("绑定身份和手机后，VIP{1} 每天登录赠送红包{0}元", giveMoney, user.VipLevel), RedBagCategory.Activity);
                manager.AddA20150919_已绑定身份和手机的用户登录送红包(new E_A20150919_已绑定身份和手机的用户登录送红包
                {
                    CreateTime = DateTime.Now,
                    UserId = userId,
                    LoginDate = date,
                    GiveRedBagMoney = giveMoney,
                });
            }
            return "";
        }
    }


    /// <summary>
    /// 充值赠送红包
    /// </summary>
    public class A20150919_FillMoneyGive :DBbase, ICompleteFillMoney_AfterTranCommit
    {
        public void CompleteFillMoney_AfterTranCommit(string orderId, FillMoneyStatus status, FillMoneyAgentType agentType, decimal fillMoney, string userId, int vipLevel)
        {
            if (string.IsNullOrEmpty(userId)) return;
            if (status != FillMoneyStatus.Success) return;
            if (agentType == FillMoneyAgentType.ManualAdd || agentType == FillMoneyAgentType.ManualDeduct) return;
            var manager = new A20150919Manager();

            //var cdbusiness = new CacheDataBusiness();
            //cdbusiness.FirstLotteryGiveRedBag(userId);
            #region 选择不同充值方式送红包
            var PayRedBagConfig = new CacheDataBusiness().QueryCoreConfigByKey("PayRedBagConfig").ConfigValue;
            if (!string.IsNullOrEmpty(PayRedBagConfig))
            {
                Dictionary<int, decimal> dict = new Dictionary<int, decimal>();
                var configs = PayRedBagConfig.Split(',');
                int i = 0;
                decimal f = 0;
                foreach (var item in configs)
                {
                    var arr = item.Split('|');
                    if (arr.Length == 2)
                    {
                        i = 0;
                        f = 0;
                        int.TryParse(arr[0], out i);
                        decimal.TryParse(arr[1], out f);
                        dict.Add(i, f);
                    }
                }
                int key = (int)agentType;
                if (dict.ContainsKey(key))
                {
                    var ff = dict[key];
                    var money = fillMoney * ff / 100;
                    if (money > 0)
                    {
                        BusinessHelper.Payin_To_Balance(AccountType.RedBag,
                                        BusinessHelper.FundCategory_Activity, userId, orderId,
                                        money, string.Format("选择网银/快捷充值类型，赠送红包{0}元", money.ToString("N2")), RedBagCategory.FillMoney);
                        //添加赠送记录
                        manager.AddA20150919_充值送红包记录(new E_A20150919_充值送红包记录
                        {
                            CreateTime = DateTime.Now,
                            UserId = userId,
                            OrderId = orderId,
                            FillMoney = fillMoney,
                            GiveMoney = money,
                            GiveMonth = DateTime.Now.ToString("yyyyMM"), //GiveMonth = DateTime.Now.Month.ToString(),
                            PayType = 2
                        });
                    }
                }

            }
            #endregion

            //开启事务

            try
            {
                DB.Begin();
                var givedMoney = manager.QueryTotalFillMoneyGiveRedBag(userId, DateTime.Now.ToString("yyyyMM"), 1);
                var maxGiveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FillMoneyMaxGiveRedBagOneMonth").ConfigValue);
                //本月赠送金额已达到最大
                if (givedMoney >= maxGiveMoney)
                    throw new Exception("用户本月已达到最大赠送红包数");
                var configList = manager.QueryA20150919_充值送红包配置();
                E_A20150919_充值送红包配置 rule = null;
                foreach (var config in configList)
                {
                    if (fillMoney >= config.FillMoney)
                    {
                        //找到了对应规则
                        rule = config;
                        break;
                    }
                }
                //未达到任何一条赠送规则
                if (rule == null)
                    throw new Exception("充值金额未查找到赠送规则");
                //赠送红包
                var giveMoney = rule.GiveMoney;//要赠送的红包
                //要赠送的红包+已经获取的红包>当月最大赠送红包
                if ((giveMoney + givedMoney) > maxGiveMoney)
                {
                    giveMoney = maxGiveMoney - givedMoney;//当月最大赠送红包-已经获取的红包
                }
                if (giveMoney <= 0)
                    throw new Exception("用户本月已赠送红包 加 当前赠送红包 已达到最大配置");



                //throw new Exception("用户本月已赠送红包 加 当前赠送红包 已达到最大配置");
                BusinessHelper.Payin_To_Balance(AccountType.RedBag,
                    BusinessHelper.FundCategory_Activity, userId, orderId,
                    giveMoney, string.Format("充值金额大于等于{0}元，赠送红包{1}元", rule.FillMoney, rule.GiveMoney.ToString("N2")), RedBagCategory.FillMoney);
                //添加赠送记录
                manager.AddA20150919_充值送红包记录(new E_A20150919_充值送红包记录
                {
                    CreateTime = DateTime.Now,
                    UserId = userId,
                    OrderId = orderId,
                    FillMoney = fillMoney,
                    GiveMoney = giveMoney,
                    GiveMonth = DateTime.Now.ToString("yyyyMM"), //GiveMonth = DateTime.Now.Month.ToString(),
                    PayType = 1
                });
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
            #region 给分享用户送红包逻辑
            FirstRechargeGiveRedBag(userId, fillMoney);
            #endregion
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "ICompleteFillMoney_AfterTranCommit":
                        CompleteFillMoney_AfterTranCommit(paraList[0].ToString(), (FillMoneyStatus)paraList[1], (FillMoneyAgentType)paraList[2], decimal.Parse(paraList[3].ToString()), paraList[4].ToString(), (int)paraList[5]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
                return null;
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("EXEC_Plugin_A20150919_FillMoneyGive_Error_", type, ex);
            }

            return null;
        }

        /// <summary>
        /// 查询充值赠送红包配置
        /// </summary>
        public FillMoneyGiveRedBagConfigInfoCollection QueryFillMoneyGiveRedBagConfigList()
        {
            var r = new FillMoneyGiveRedBagConfigInfoCollection();
            var manager = new A20150919Manager();
            r.AddRange(manager.QueryFillMoneyGiveRedBagConfigList());
            return r;
        }

        /// <summary>
        /// 增加充值赠送红包配置
        /// </summary>
        public void AddFillMoneyGiveRedBagConfig(decimal fillMoney, decimal giveMoney)
        {
            var manager = new A20150919Manager();
            manager.AddA20150919_充值送红包配置(new E_A20150919_充值送红包配置
            {
                CreateTime = DateTime.Now,
                FillMoney = fillMoney,
                GiveMoney = giveMoney,
            });
        }

        /// <summary>
        /// 修改充值赠送红包配置
        /// </summary>
        public void UpdateFillMoneyGiveRedBagConfig(int id, decimal fillMoney, decimal giveMoney)
        {
            var manager = new A20150919Manager();
            var old = manager.QueryA20150919_充值送红包配置(id);
            if (old == null)
                throw new Exception("找不到相应的配置");
            old.FillMoney = fillMoney;
            old.GiveMoney = giveMoney;
            manager.UpdateA20150919_充值送红包配置(old);
        }

        /// <summary>
        /// 删除充值赠送红包配置
        /// </summary>
        public void DeleteFillMoneyGiveRedBagConfig(int id)
        {
            var manager = new A20150919Manager();
            var old = manager.QueryA20150919_充值送红包配置(id);
            if (old == null)
                throw new Exception("找不到相应的配置");
            manager.DeleteA20150919_充值送红包配置(old);
        }

        #region 首充Z元 分享人可获得X元
        /// <summary>
        /// 分享推广 首充Z元 分享人可获得X元
        /// </summary>
        /// <param name="userId"></param>
        public void FirstRechargeGiveRedBag(string userId, decimal totalMoney)
        {
            
                DB.Commit();
                //分享推广 首充 送红包
                //首充超过一定金额了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
                var fundManager = new FundManager();
                var count = fundManager.QueryFillMoneyCount(userId);
                if (count == 1)
                {
                    var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
                    var satisfyFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.SatisfyRechargeGiveRedBagTofxid").ConfigValue);
                    if (entityShareSpread != null && !entityShareSpread.isGiveRechargeRedBag && totalMoney >= satisfyFillMoney)
                    {
                        //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
                        var giveFillMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.FirstRechargeGiveRedBagTofxid").ConfigValue);
                        if (giveFillMoney > 0)
                        {
                            BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
                                              , string.Format("{1}用户首次充值超过{2}元，赠送红包给分享推广用户{0}元", giveFillMoney, userId, satisfyFillMoney), RedBagCategory.FxidRegister);

                            entityShareSpread.isGiveRechargeRedBag = true;
                            entityShareSpread.UpdateTime = DateTime.Now;
                            entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
                            new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
                        }
                    }
                }
                DB.Commit();
            
        }
        #endregion
    }

    /// <summary>
    /// 彩种加奖相关
    /// </summary>
    public class A20150919_AddBonusMoney :DBbase, IOrderPrize_AfterTranCommit
    {
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            //if (isVirtualOrder) return;
            //if (!isBonus) return;
            //if (afterTaxBonusMoney <= 0) return;
            //开启事务
            DoGiveAddMoney(schemeId);
        }

        public void DoGiveAddMoney(string schemeId)
        {
            //开启事务

            try
            {
                DB.Begin();

                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("加奖，没有找到订单{0}", schemeId));
                if (order.IsVirtualOrder)
                    throw new LogicException("订单为虚拟订单，不执行加奖");

                if (order.TicketStatus != (int)TicketStatus.Ticketed)
                    throw new LogicException("订单出票状态不正确");
                if (order.AfterTaxBonusMoney <= 0)
                    throw new LogicException("订单未中奖");

                var activityManager = new A20150919Manager();
                var configList = activityManager.QueryA20150919_加奖配置(order.GameCode);
                var totalAddMoney = 0M;

                var addMoneyWay = "10";//10是奖金，70是红包
                //按订单计算加奖
                //var rule = FindConfig(configList, order.GameCode, order.GameType, order.PlayType);
                //if (rule != null)
                //{
                //    addMoneyWay = rule.AddMoneyWay;
                //}
                //totalAddMoney += ComputeAddMoney(rule, order.AfterTaxBonusMoney);


                //查找订单用户的所有上级用户编号
                var userManager = new UserBalanceManager();
                var parentUserIdList = new List<string>();
                parentUserIdList.Add(order.UserId);
                FindAllParentUserId(userManager, order.UserId, ref parentUserIdList);

                //查询票数据,并按单票计算加奖金额
                var ticketList = manager.QueryTicketList(schemeId);
                var doTicketId = new List<string>();
                foreach (var ticket in ticketList)
                {
                    if (doTicketId.Contains(ticket.TicketId))
                        continue;

                    doTicketId.Add(ticket.TicketId);

                    #region 用户的代理指定玩法不加奖

                    try
                    {
                        //有玩法的彩种
                        var ticketGameType = string.Empty;
                        //if (new string[] { "CTZQ", "JCZQ", "JCLQ", "BJDC" }.Contains(ticket.GameCode))
                        //    ticketGameType = ticket.GameType;

                        //有过关方式的彩种
                        var ticketPlayType = "";
                        if (new string[] { "JCZQ", "JCLQ", "BJDC" }.Contains(ticket.GameCode))
                            ticketPlayType = ticket.PlayType == "P1_1" ? "P1_1" : "PM_1";
                        var count = activityManager.Query不加奖用户列表(parentUserIdList, ticket.GameCode, ticketGameType, ticketPlayType);
                        if (count > 0)
                            continue;
                    }
                    catch (Exception ex)
                    {
                    }

                    #endregion

                    #region 如果单关的比赛且比赛是取消的，不做加奖

                    if (ticket.PlayType == "P1_1")
                    {
                        //MatchIdList就是一场比赛的编号
                        if (ticket.GameCode == "JCZQ")
                        {
                            var matchResult = new JCZQMatchManager().QueryJCZQ_MatchResult_PrizeByMatchId(ticket.MatchIdList);
                            //SP为1 说明是取消的比赛
                            if (matchResult.SPF_SP == 1M)
                            {
                                continue;
                            }
                        }
                        if (ticket.GameCode == "JCLQ")
                        {
                            var matchResult = new JCLQMatchManager().QueryJCLQ_MatchResult_PrizeByMatchId(ticket.MatchIdList);
                            //SP为1 说明是取消的比赛
                            if (matchResult.SF_SP == 1M)
                            {
                                continue;
                            }
                        }
                        if (ticket.GameCode == "BJDC")
                        {
                            var matchResult = new BJDCMatchManager().QueryBJDC_MatchResult_Prize(ticket.IssuseNumber, int.Parse(ticket.MatchIdList));
                            //SP为1 说明是取消的比赛
                            if (matchResult.SPF_SP == 1M)
                            {
                                continue;
                            }
                        }
                    }

                    #endregion

                    //查换符合条件的规则
                    var rule = FindConfig(configList, ticket);
                    if (rule != null)
                    {
                        //计算加奖金额
                        totalAddMoney += ComputeAddMoney(rule, ticket.AfterTaxBonusMoney);
                        addMoneyWay = rule.AddMoneyWay;
                    }
                }

                order.BonusStatus = (int)BonusStatus.Win;
                order.AddMoney = totalAddMoney;
                order.AddMoneyDescription = addMoneyWay;
                manager.UpdateSports_Order_Complate(order);

                var schemeManager = new SchemeManager();
                var orderDetail = schemeManager.QueryOrderDetail(schemeId);
                if (orderDetail != null)
                {
                    orderDetail.BonusStatus = (int)BonusStatus.Win;
                    orderDetail.AddMoney = totalAddMoney;//之前的加奖字段，现暂时无用
                    if (addMoneyWay == "10")//奖金加奖
                        orderDetail.BonusAwardsMoney += totalAddMoney;
                    else if (addMoneyWay == "70")//红包加奖
                        orderDetail.RedBagAwardsMoney += totalAddMoney;
                    schemeManager.UpdateOrderDetail(orderDetail);
                }

                //加奖记录
                if (totalAddMoney > 0)
                {
                    activityManager.AddA20150919_加奖赠送记录(new E_A20150919_加奖赠送记录
                    {
                        OrderId = schemeId,
                        BonusMoney = order.AfterTaxBonusMoney,
                        AddBonusMoney = totalAddMoney,
                        CreateTime = DateTime.Now,
                        GameCode = order.GameCode,
                        GameType = order.GameType,
                        PlayType = "",
                    });
                }

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
        }

        /// <summary>
        /// 递归查找用户的所有上级
        /// </summary>
        private void FindAllParentUserId(UserBalanceManager manager, string userId, ref List<string> currentParentUserIdList)
        {
            var user = manager.QueryUserRegister(userId);
            if (user == null)
                return;
            if (string.IsNullOrEmpty(user.AgentId))
                return;
            currentParentUserIdList.Add(user.AgentId);
            FindAllParentUserId(manager, user.AgentId, ref currentParentUserIdList);
        }

        private E_A20150919_加奖配置 FindConfig(List<E_A20150919_加奖配置> configList, string gameCode, string gameType, string playType)
        {
            foreach (var config in configList.OrderBy(p => p.OrderIndex).ToArray())
            {
                if (new string[] { "JCZQ", "JCLQ", "BJDC" }.Contains(gameCode.ToUpper()))
                {
                    if (!string.IsNullOrEmpty(playType))
                        playType = string.Format("P{0}", playType);
                    //竞彩
                    if (config.GameType.ToUpper() == gameType.ToUpper())
                    {
                        //指定玩法
                        if (playType.ToUpper() == "P1_1" && playType.ToUpper() == config.PlayType.ToUpper())
                        {
                            //单关
                            return config;
                        }
                        if (playType.ToUpper() != "P1_1" && config.PlayType.ToUpper() == "PM_1")
                        {
                            //过关
                            return config;
                        }
                        if (config.PlayType.ToUpper() == "ALL")
                        {
                            //全部
                            return config;
                        }
                    }
                    if (config.GameType.ToUpper() == "ALL")
                    {
                        //全部玩法
                        if (playType.ToUpper() == "P1_1" && playType.ToUpper() == config.PlayType.ToUpper())
                        {
                            //单关
                            return config;
                        }
                        if (playType.ToUpper() != "P1_1" && config.PlayType.ToUpper() == "PM_1")
                        {
                            //过关
                            return config;
                        }
                        if (config.PlayType.ToUpper() == "ALL")
                        {
                            //全部
                            return config;
                        }
                    }
                }
                else
                {
                    //传统足球和数字彩
                    if (config.GameType.ToUpper() == gameType.ToUpper())
                        return config;
                    if (config.GameType.ToUpper() == "ALL")
                        return config;
                }
            }
            return null;
        }

        private E_A20150919_加奖配置 FindConfig(List<E_A20150919_加奖配置> configList, C_Sports_Ticket ticket)
        {
            foreach (var config in configList.OrderBy(p => p.OrderIndex).ToArray())
            {
                if (new string[] { "JCZQ", "JCLQ", "BJDC" }.Contains(ticket.GameCode.ToUpper()))
                {
                    //竞彩
                    if (config.GameType.ToUpper() == ticket.GameType.ToUpper())
                    {
                        //指定玩法
                        if (ticket.PlayType.ToUpper() == "P1_1" && ticket.PlayType.ToUpper() == config.PlayType.ToUpper())
                        {
                            //单关
                            return config;
                        }
                        if (ticket.PlayType.ToUpper() != "P1_1" && config.PlayType.ToUpper() == "PM_1")
                        {
                            //过关
                            return config;
                        }
                        if (config.PlayType.ToUpper() == "ALL")
                        {
                            //全部
                            return config;
                        }
                    }
                    if (config.GameType.ToUpper() == "ALL")
                    {
                        //全部玩法
                        if (ticket.PlayType.ToUpper() == "P1_1" && ticket.PlayType.ToUpper() == config.PlayType.ToUpper())
                        {
                            //单关
                            return config;
                        }
                        if (ticket.PlayType.ToUpper() != "P1_1" && config.PlayType.ToUpper() == "PM_1")
                        {
                            //过关
                            return config;
                        }
                        if (config.PlayType.ToUpper() == "ALL")
                        {
                            //全部
                            return config;
                        }
                    }
                }
                else
                {
                    //传统足球和数字彩
                    if (config.GameType.ToUpper() == ticket.GameType.ToUpper())
                        return config;
                    if (config.GameType.ToUpper() == "ALL")
                        return config;
                }
            }
            return null;
        }

        /// <summary>
        /// 手工执行加奖
        /// </summary>
        public void ManualAddMoney(string schemeId)
        {
            DoGiveAddMoney(schemeId);

            var manager = new Sports_Manager();
            var order = manager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                throw new LogicException(string.Format("加奖，没有找到订单{0}", schemeId));
            if (order.AddMoney > 0)
                BusinessHelper.Payin_To_Balance(order.AddMoneyDescription == "10" ? AccountType.Bonus : AccountType.RedBag, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AddMoney,
                                            string.Format("订单{0}活动赠送{1:N2}元.", schemeId, order.AddMoney));

            return;



            ////开启事务
            //using (var biz = new GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            //    var manager = new Sports_Manager();
            //    var order = manager.QuerySports_Order_Complate(schemeId);
            //    if (order == null)
            //        throw new LogicException(string.Format("加奖，没有找到订单{0}", schemeId));
            //    if (order.IsVirtualOrder)
            //        throw new LogicException("订单为虚拟订单，不执行加奖");

            //    if (order.TicketStatus != TicketStatus.Ticketed)
            //        throw new LogicException("订单出票状态不正确");
            //    if (order.AfterTaxBonusMoney <= 0)
            //        throw new LogicException("订单未中奖");

            //    var activityManager = new A20150919Manager();
            //    var configList = activityManager.QueryA20150919_加奖配置(order.GameCode);
            //    var totalAddMoney = 0M;

            //    //查询票数据,并按单票计算加奖金额
            //    var ticketList = manager.QueryTicketList(schemeId);
            //    if (ticketList.Count <= 0)
            //        throw new Exception("订单未查找到票数据");
            //    foreach (var ticket in ticketList)
            //    {
            //        //查换符合条件的规则
            //        var rule = FindConfig(configList, ticket);
            //        //计算加奖金额
            //        totalAddMoney += ComputeAddMoney(rule, ticket.AfterTaxBonusMoney);
            //    }
            //    if (totalAddMoney <= 0)
            //        throw new Exception("加奖计算金额为0，请确认订单是否应该加奖");

            //    order.BonusStatus = BonusStatus.Win;
            //    order.AddMoney = totalAddMoney;
            //    order.AddMoneyDescription = string.Format("订单加奖{0}元", totalAddMoney);
            //    manager.UpdateSports_Order_Complate(order);

            //    var schemeManager = new SchemeManager();
            //    var orderDetail = schemeManager.QueryOrderDetail(schemeId);
            //    if (orderDetail != null)
            //    {
            //        orderDetail.BonusStatus = BonusStatus.Win;
            //        orderDetail.AddMoney = totalAddMoney;
            //        schemeManager.UpdateOrderDetail(orderDetail);
            //    }

            //    //加奖记录
            //    if (totalAddMoney > 0)
            //    {
            //        activityManager.AddA20150919_加奖赠送记录(new A20150919_加奖赠送记录
            //        {
            //            OrderId = schemeId,
            //            BonusMoney = order.AfterTaxBonusMoney,
            //            AddBonusMoney = totalAddMoney,
            //            CreateTime = DateTime.Now,
            //            GameCode = order.GameCode,
            //            GameType = order.GameType,
            //            PlayType = "",
            //        });

            //        //加钱
            //        BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Bonus, order.UserId, schemeId, order.AddMoney,
            //                                       string.Format("订单{0}活动赠送{1:N2}元.", schemeId, order.AddMoney));
            //    }

            //    biz.CommitTran();
            //}
        }

        private decimal ComputeAddMoney(E_A20150919_加奖配置 rule, decimal bonusMoney)
        {
            if (rule == null || bonusMoney <= 0M)
                return 0M;

            var addMoney = bonusMoney * rule.AddBonusMoneyPercent / 100;
            if (addMoney > rule.MaxAddBonusMoney)
                addMoney = rule.MaxAddBonusMoney;
            return addMoney;
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("EXEC_Plugin_A20150919_AddBonusMoney_Error_", type, ex);
            }
            return null;
        }

        /// <summary>
        /// 查询加奖配置
        /// </summary>
        public AddBonusMoneyConfigInfoCollection QueryAddBonusMoneyConfig()
        {
            var r = new AddBonusMoneyConfigInfoCollection();
            var manager = new A20150919Manager();
            r.AddRange(manager.QueryAddBonusMoneyConfig());
            return r;
        }

        /// <summary>
        /// 增加加奖配置
        /// </summary>
        public void AddAddBonusMoneyConfig(string gameCode, string gameType, string playType, decimal addPercent, decimal maxAddMoney, int orderIndex, string addMoneyWay)
        {
            var manager = new A20150919Manager();
            manager.AddA20150919_加奖配置(new E_A20150919_加奖配置
            {
                AddBonusMoneyPercent = addPercent,
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                GameType = gameType,
                PlayType = playType,
                MaxAddBonusMoney = maxAddMoney,
                OrderIndex = orderIndex,
                AddMoneyWay = addMoneyWay,
            });
        }

        /// <summary>
        /// 删除加奖配置
        /// </summary>
        public void DeleteAddBonusMoneyConfig(int id)
        {
            var manager = new A20150919Manager();
            var old = manager.QueryAddBonusMoneyConfig(id);
            if (old == null)
                return;
            manager.DeleteA20150919_加奖配置(old);
        }
    }

    /// <summary>
    /// 红包使用配置
    /// </summary>
    public class A20150919_RedBagUseConfig
    {
        public string QueryRedBagUseConfig()
        {
            var manager = new A20150919Manager();
            var list = manager.QueryRedBagUseConfig();
            var query = from l in list
                        select string.Format("{0}_{1}_{2}", l.Id, l.GameCode, l.UsePercent.ToString("N2"));
            return string.Join("|", query.ToArray());
        }

        public decimal QueryRedBagUseConfig(string gameCode)
        {
            var manager = new A20150919Manager();
            var config = manager.QueryRedBagUseConfig(gameCode);
            if (config == null)
                return 0M;
            return config.UsePercent;
        }

        public void AddRedBagUseConfig(string gameCode, decimal percent)
        {
            var manager = new A20150919Manager();
            var config = manager.QueryRedBagUseConfig(gameCode);
            if (config != null)
                throw new Exception("已有彩种使用规则");

            manager.AddA20150919_红包使用配置(new E_A20150919_红包使用配置
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                UsePercent = percent
            });

            //重新加载配置
            LoadRedBagUseConfigToRedis();
        }

        public void DeleteRedBagUseConfig(int id)
        {
            var manager = new A20150919Manager();
            var old = manager.QueryRedBagUseConfig(id);
            if (old == null)
                return;
            manager.DeleteA20150919_红包使用配置(old);

            //重新加载配置
            LoadRedBagUseConfigToRedis();
        }

        /// <summary>
        /// 加载红包使用规则到Redis
        /// </summary>
        public void LoadRedBagUseConfigToRedis()
        {
            var db = RedisHelper.DB_CoreCacheData;
            var key = RedisKeys.Key_RedBagUseConfig;
            db.KeyDeleteAsync(key);

            var manager = new A20150919Manager();
            var list = manager.QueryRedBagUseConfig();
            foreach (var item in list)
            {
                var v = string.Format("{0}_{1}", item.GameCode, item.UsePercent.ToString("N2"));
                db.ListRightPushAsync(key, v);
            }
        }

        /// <summary>
        /// 查询彩种红包使用配置
        /// </summary>
        public decimal QueryRedBagUseConfigFromRedis(string gameCode)
        {
            var db = RedisHelper.DB_CoreCacheData;
            var key = RedisKeys.Key_RedBagUseConfig;
            foreach (var item in db.ListRangeAsync(key).Result)
            {
                if (!item.HasValue) continue;
                var array = item.ToString().Split('_');
                if (array.Length != 2)
                    continue;
                if (array[0] == gameCode)
                    return decimal.Parse(array[1]);
            }
            return 0M;
        }

    }

    /// <summary>
    /// 代理用户对应彩种不支持加奖相关
    /// </summary>
    public class A20150919_UserGameCodeNotAddMoney
    {
        /// <summary>
        /// 查询取消加奖列表
        /// </summary>
        public UserGameCodeNotAddMoneyInfoCollection QueryUserGameCodeNotAddMoneyList(string userId)
        {
            var r = new UserGameCodeNotAddMoneyInfoCollection();
            var manager = new A20150919Manager();
            r.AddRange(manager.QueryUserGameCodeNotAddMoneyList(userId));
            return r;
        }

        /// <summary>
        /// 增加用户的取消加奖
        /// </summary>
        public void AddUserGameCodeNotAddMoney(string userId, string gameCode, string gameType, string playType)
        {
            var manager = new UserBalanceManager();
            var userEntity = manager.QueryUserRegister(userId);
            if (userEntity == null)
                throw new Exception(string.Format("{0}用户不存在。", userId));
            var activityManager = new A20150919Manager();
            var activityEntity = activityManager.QueryA20150919_列表用户不加奖(userId, gameCode, gameType, playType);
            if (activityEntity != null)
                throw new Exception(string.Format("{0}用户下{1}彩种已存在不加奖。", userId, gameCode));
            activityManager.AddA20150919_列表用户不加奖(new E_A20150919_列表用户不加奖
            {
                CreateTime = DateTime.Now,
                GameCode = gameCode,
                UserId = userId,
                GameType = gameType,
                PlayType = playType,
            });
        }

        /// <summary>
        /// 删除用户的取消加奖
        /// </summary>
        public void DeleteUserGameCodeNotAddMoney(int id)
        {
            var manager = new A20150919Manager();
            var old = manager.QueryUserGameCodeNotAddMoney(id);
            if (old == null)
                return;
            manager.DeleteA20150919_列表用户不加奖(old);
        }
    }
}
