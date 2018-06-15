using KaSon.FrameWork.ORM.Provider;

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
using Kason.Sg.Core.CPlatform.Transport.Implementation;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using UserLottery.Service.Model;
using KaSon.FrameWork.Services.ORM;
using KaSon.FrameWork.Services.Enum;
using EntityModel.ORM;
using KaSon.FrameWork.Helper;
using System.Threading;
using UserLottery.Service.ModuleBaseServices;
using Lottery.Kg.ORM.Helper;
using UserLottery.Service.IModuleServices;
using EntityModel.RequestModel;
using Lottery.Kg.ORM.Helper.UserHelper;
using EntityModel.Enum;
using EntityModel.CoreModel;
using EntityModel.Communication;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace UserLottery.Service.ModuleServices
{
    [ModuleName("User")]
    public class UserService : KgBaseService, IUserService
    {
        #region Implementation of IUserService
        // private readonly UserRepository _repository;
        //public UserService(UserRepository repository)
        //{
        //    this._repository = repository;
        //}
        IKgLog log = null;
        public UserService()
        {

            log = new Log4Log();

        }
       

        // log demo
        /// <summary>
        /// 日志使用 demo
        /// </summary>
        private void LogTest()
        {

            // 异常日志
            log.Log("调试信息");
            log.Log("标签", new Exception("错误"));
        }
        private UserAuthentication userAuthentication = new UserAuthentication();
        LoginLocal loginEntity = new LoginLocal();
        //private BusinessHelper businessHelper;
        public Task<LoginInfo> User_Login(string loginName, string password,string loginIp)
        {
            //QueryUserParam model = new QueryUserParam();
            string IPAddress = loginIp;
            var loginBiz = new LocalLoginBusiness();
          
            if (IPAddress == "Client")//移动端登录时，密码已经MD5
                loginEntity = loginBiz.LoginAPP(loginName,password);
            else
                loginEntity = loginBiz.Login(loginName, password);
            if (loginEntity == null)
            {
                return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "登录名(手机号)或密码错误", LoginFrom = "LOCAL", });
            }

            ////var authBiz = new GameBizAuthBusiness();
            if (!IsRoleType(loginEntity.User, RoleType.WebRole))
            {
                return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "此帐号角色不允许在此登录", LoginFrom = "LOCAL", });
            }
            if (!loginEntity.Register.IsEnable)
            {
                return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "用户未激活", LoginFrom = "LOCAL", UserId = loginEntity.UserId });
            }

            var authBiz = new GameBizAuthBusiness();
            var userToken = authBiz.GetUserToken(loginEntity.User.UserId);
            
             var blogEntity = loginBiz.QueryBlog_ProfileBonusLevel(loginEntity.User.UserId);

            ////清理用户绑定数据缓存
            ////ClearUserBindInfoCache(loginEntity.UserId);


            //! 执行扩展功能代码 - 提交事务前
            //BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { loginEntity.UserId, "LOCAL", loginIp, DateTime.Now });
            ////刷新用户在Redis中的余额
            //BusinessHelper.RefreshRedisUserBalance(loginEntity.UserId);
            return Task.FromResult(new LoginInfo
            {
                IsSuccess = true,
                Message = "登录成功",
                CreateTime = loginEntity.CreateTime,
                LoginFrom = "LOCAL",
                RegType = loginEntity.Register.RegType,
                Referrer = loginEntity.Register.Referrer,
                UserId = loginEntity.User.UserId,
                VipLevel = loginEntity.Register.VipLevel,
                LoginName = loginEntity.LoginName,
                DisplayName = loginEntity.Register.DisplayName,
                UserToken = userToken,
                AgentId = loginEntity.Register.AgentId,
                IsAgent = loginEntity.Register.IsAgent,
                HideDisplayNameCount = loginEntity.Register.HideDisplayNameCount,
                MaxLevelName = string.IsNullOrEmpty(blogEntity.MaxLevelName) ? "" : blogEntity.MaxLevelName,
                IsUserType = loginEntity.Register.UserType == 1 ? true : false
            });
           
        }

        /// <summary>
        /// 使用token登录
        /// </summary>
        public Task<LoginInfo> LoginByUserToken(string userToken)
        {
            // 验证用户身份及权限
            var userId = userAuthentication.ValidateUserAuthentication(userToken);

            var loginBiz = new LocalLoginBusiness();
            var reg = loginBiz.GetRegisterById(userId);
            if (reg == null)
            {
                return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "不存在该用户", LoginFrom = "LOCAL", });
            }
            string loginFrom = reg.ComeFrom;
            string loginName = "";
            switch (loginFrom.ToLower())
            {
                case "local":
                case "index":
                case "app":
                case "ios":
                case "touch":


                    var loginEntity = GetLocalLoginByUserId(userId);
                    if (loginEntity == null)
                    {
                        return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "不存在该用户", LoginFrom = loginFrom, });
                    }
                    loginName = loginEntity.LoginName;
                    break;
                case "alipay":

                    var alipayEntity = loginBiz.GetAlipayByUserId(userId);
                    if (alipayEntity == null)
                    {
                        return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "不存在该用户", LoginFrom = loginFrom, });
                    }
                    loginName = alipayEntity.LoginName;
                    break;
                case "qq":

                    var qqEntity = loginBiz.GetQQByUserId(userId);
                    if (qqEntity == null)
                    {
                        return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "不存在该用户", LoginFrom = loginFrom, });
                    }
                    loginName = qqEntity.LoginName;
                    break;
                default:
                    throw new ArgumentException("登录不支持的注册类型 - " + loginFrom);
            }

            //! 执行扩展功能代码 - 提交事务前
            //BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { userId, loginFrom, "", DateTime.Now });


            //刷新用户在Redis中的余额
            //BusinessHelper.RefreshRedisUserBalance(userId);

            return Task.FromResult(new LoginInfo
            {
                IsSuccess = true,
                Message = "登录成功",
                CreateTime = reg.CreateTime,
                LoginFrom = "Alipay",
                RegType = reg.RegType,
                Referrer = reg.Referrer,
                UserId = reg.UserId,
                VipLevel = reg.VipLevel,
                LoginName = loginName,
                DisplayName = reg.DisplayName,
                UserToken = userToken,
                AgentId = reg.AgentId,
                IsAgent = reg.IsAgent,
                HideDisplayNameCount = reg.HideDisplayNameCount,
            });
        }

        public bool IsRoleType(SystemUser user, RoleType roleType)
        {
            foreach (var role in user.RoleList)
            {
                if (role.RoleType == roleType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否和资金密码一至
        /// </summary>
        /// <param name="newPassword">新密码</param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult CheckIsSame2BalancePassword(string newPassword, string userId)
        {
            var loginBiz = new LocalLoginBusiness();
            var result = loginBiz.CheckIsSame2BalancePassword(userId, newPassword);
            var flag = "N";
            if (result.HasValue)
            {
                flag = result.Value ? "T" : "F";
            }
            return new CommonActionResult(true, "查询成功") { ReturnValue = flag };
        }

        /// <summary>
        /// 查询用户绑定数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<UserBindInfos> QueryUserBindInfos(string userId) {

            //尝试从缓存中读取数据
            var info = LoadUserBindInfoFromCache(userId);
            if (info != null)
                return Task.FromResult(info);
            //从数据库读取数据
            info = new LocalLoginBusiness().QueryUserBindInfos(userId);
            if (info == null)
                return Task.FromResult(new UserBindInfos());

            //添加缓存到文件
            SaveUserBindInfoCache(userId, info);

            return Task.FromResult(info);
        }

        /// <summary>
        /// 读取缓存的用户绑定数据
        /// </summary>
        private UserBindInfos LoadUserBindInfoFromCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelper.DB_UserBindData;
                var exist = db.KeyExistsAsync(fullKey).Result;
                if (!exist)
                    return null;
                var v = db.StringGetAsync(fullKey).Result;
                if (!v.HasValue)
                    return null;
                var info = JsonHelper.Deserialize<UserBindInfos>(v);
                return null;
            }
            catch (Exception ex)
            {
                //Common.Log.LogWriterGetter.GetLogWriter().Write("QueryUserBindInfos_Read", userId, ex);
                return null;
            }
        }

        /// <summary>
        /// 保存用户绑定数据的缓存
        /// </summary>
        private void SaveUserBindInfoCache(string userId, UserBindInfos info)
        {

            try
            {
                var content = JsonHelper.Serialize<UserBindInfos>(info);
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelper.DB_UserBindData;
                db.StringSetAsync(fullKey, content, TimeSpan.FromDays(1));
            }
            catch (Exception ex)
            {
                //Common.Log.LogWriterGetter.GetLogWriter().Write("QueryUserBindInfos_Write", userId, ex);
            }
        }


        /// <summary>
        /// 查询我的余额
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<UserBalanceInfo> QueryMyBalance(string userToken)
        {
            // 验证用户身份及权限
            var userId = userAuthentication.ValidateUserAuthentication(userToken);
            if (userId == "admin")
                return null;
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var entity = loginBiz.QueryUserBalance(userId);
                //return new FundBusiness().QueryUserBalance(userId);
                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的余额出错 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询银行卡信息
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<C_BankCard> QueryBankCard(string userToken)
        {
            // 验证用户身份及权限
            var userId = userAuthentication.ValidateUserAuthentication(userToken);

            try
            {
                var loginBiz = new LocalLoginBusiness();
                return Task.FromResult(loginBiz.BankCardById(userId));
            }
            catch (LogicException)
            {
                return Task.FromResult(new C_BankCard { });
            }
            catch (Exception ex)
            {
                throw new LogicException(ex.Message, ex);
            }
        }
        

        /// <summary>
        /// 获取我的未读站内信条数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<int> GetMyUnreadInnerMailCount(string userToken)
        {
            var loginBiz = new LocalLoginBusiness();
            // 验证用户身份及权限
            var userId = userAuthentication.ValidateUserAuthentication(userToken);

         
            return Task.FromResult(loginBiz.GetUnreadMailCountByUser(userId));
        }


        #region 修改密码


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="userToken">用户Token</param>
        /// <returns></returns>
        public Task<CommonActionResult> ChangeMyPassword(string oldPassword, string newPassword, string userToken)
        {
            // 验证用户身份及权限
            var userId= userAuthentication.ValidateUserAuthentication(userToken);
            var loginBiz = new LocalLoginBusiness();
            loginBiz.ChangePassword(userId, oldPassword, newPassword);

            return Task.FromResult(new CommonActionResult(true, "修改密码成功"));

        }

        #endregion

        /// <summary>
        /// 根据UserId查询用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public LoginInfo GetLocalLoginByUserId(string userId)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var local = loginBiz.GetLocalLoginByUserId(userId);
                if (local == null)
                    return new LoginInfo();
                return new LoginInfo
                {
                    CreateTime = local.CreateTime,               
                    UserId = local.UserId,                
                    LoginName = local.LoginName,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 手机黑名单
        /// </summary>
        private static C_Core_Config _coreConfigList = new C_Core_Config();
        public C_Core_Config QueryCoreConfigByKey(string key)
        {
          
            var loginBiz = new MobileAuthenticationBusiness();        
                _coreConfigList = loginBiz.BanRegistrMobile(key);

            if (_coreConfigList == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return _coreConfigList;
        }

        /// <summary>
        /// 手机认证，重发验证码或更换号码
        /// </summary>
        public CommonActionResult RepeatRequestMobile(string userId, string mobile, string createUserId)
        {
            try
            {
                var validateCode = GetRandomMobileValidateCode();
                RepeatRequestMobile2(userId, mobile, createUserId);

                //var biz = new ValidationMobileBusiness();
                //validateCode = biz.SendValidationCode(mobile, "MobileAuthentication", validateCode, GetDelay(30), GetMaxTimes(3));
                return new CommonActionResult(true, "已成功提交手机认证申请，请等待") { ReturnValue = validateCode };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <returns></returns>
        private string GetRandomMobileValidateCode()
        {
            var validateCode = "8888";
            //if (!UsefullHelper.IsInTest)
            //{
                // 生成随机密码
                Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
                validateCode = random.Next(100, 999999).ToString().PadLeft(6, '0');
                //return RndNum(6);
            //}
            return validateCode;
        }


        /// <summary>
        /// 手机认证，重发验证码或更换号码
        /// </summary>
        public void RepeatRequestMobile2(string userId, string mobile, string createUserId)
        {
                var manager = new MobileAuthenticationBusiness();
            
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("未查询到用户编号");
                else if (string.IsNullOrEmpty(mobile))
                    throw new Exception("手机号码不能为空");
                //var entity = manager.GetUserMobile(userId);
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile && other.UserId != userId)
                {
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                }
            var entity = manager.GetUserMobile(userId);
            if (entity != null)
            {
                entity.IsSettedMobile = false;
                entity.UpdateBy = createUserId;
                //entity.RequestTimes++;
                entity.Mobile = mobile;
                //manager.UpdateUserMobile(entity);
            }
            //else
            //{
            //    entity = new UserMobile
            //    {
            //        UserId = userId,
            //        User = manager.LoadUser(userId),
            //        AuthFrom = "LOCAL",
            //        Mobile = mobile,
            //        IsSettedMobile = false,
            //        CreateBy = createUserId,
            //        UpdateBy = createUserId,
            //    };
            //    manager.AddUserMobile(entity);
            //}

        }

        /// <summary>
        /// 验证手机
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <param name="mobile">手机号</param>
        /// <param name="source"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public Task<CommonActionResult> RegisterResponseMobile(string validateCode, string mobile, SchemeSource source, RegisterInfo_Local info)
        {
            try
            {
                
                if (string.IsNullOrEmpty(validateCode))
                    throw new Exception("验证码不能为空");
                var isCheckValidateCode = false;
                var authenticationBiz = new MobileAuthenticationBusiness();
                #region "20180522新增:用户名注册修改为手机号注册后,存在老用户绑定了手机号后，可以继续用手机号注册一个新号"
                if (authenticationBiz.IsMobileAuthenticated(mobile))
                    throw new Exception("该手机号已经认证");
                #endregion      

                isCheckValidateCode = authenticationBiz.CheckValidationCode(mobile, "MobileAuthentication", validateCode, GetMaxTimes(3));

                if (!isCheckValidateCode)
                {
                    throw new Exception("验证码输入不正确。");
                }
                info.Referrer = "mobile_regist";
                //注册
                var userResult = RegisterLoacal(info);
                if (userResult == null || string.IsNullOrEmpty(userResult.ReturnValue))
                    throw new Exception("注册失败,请重新注册");
                string mobileNumber;
                mobileNumber = authenticationBiz.RegisterResponseMobile(userResult.ReturnValue, mobile, 1800, "半个小时");

                #region 还没做
                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userResult.ReturnValue, "Mobile", mobileNumber, source });
                #endregion
                return Task.FromResult(new CommonActionResult(true, "恭喜您注册成功！"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new CommonActionResult(false, ex.Message));
            }
        }


        private int GetMaxTimes(int time)
        {
            //if (UsefullHelper.IsInTest)
            //{
          return time;
            //}
            //return time;
        }
        DBbase dBbase = new DBbase();

        /// <summary>
        /// 本地 注册账号
        /// </summary>
        public CommonActionResult RegisterLoacal(RegisterInfo_Local regInfo)
        {
            
            if (string.IsNullOrEmpty(ConfigHelper.ConfigInfo["PageRegisterDefaultRole"].ToString()))
            {
                throw new ArgumentNullException("未配置前台注册用户默认角色的参数 - PageRegisterDefaultRole");
            }
            if (string.IsNullOrEmpty(regInfo.LoginName))
                throw new Exception("登录名不能为空");
            else if (Regex.IsMatch(regInfo.LoginName, "[ ]+"))
                throw new Exception("登录名不能包含空格");
            if (regInfo != null && !string.IsNullOrEmpty(regInfo.AgentId))
            {

                var userEntity2 = new LocalLoginBusiness();
               var userEntity3=  userEntity2.QueryUserRegisterByUserId(regInfo.AgentId);
                if (userEntity3 == null || !userEntity3.IsAgent)
                {
                    regInfo.AgentId = string.Empty;
                }
            }

            var roleIds = ConfigHelper.ConfigInfo["PageRegisterDefaultRole"].ToString().Split(',');

            regInfo.LoginName = regInfo.LoginName.Trim();
            //if (!Common.Utilities.UsefullHelper.IsInTest)
            //{
            //    if (string.IsNullOrEmpty(regInfo.LoginName))
            //        throw new ArgumentException("登录名称不能为空");
            //    var b = System.Text.RegularExpressions.Regex.IsMatch(regInfo.LoginName, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]");
            //    if (b)
            //        throw new ArgumentException("登录名称不能为特殊符号");

            //    byte[] myByte = System.Text.Encoding.Default.GetBytes(regInfo.LoginName);
            //    if (myByte.Length < 4 || regInfo.LoginName.Length > 20)
            //        throw new ArgumentException("登录名长度必须在4位到20位之间");
            //}
     
                 string userId;

                dBbase.DB.Begin();

                #region 注册权限控制帐号

                var authBiz = new GameBizAuthBusiness();
                var regBiz = new RegisterBusiness();
                var userEntity = new SystemUser
                {
                    RegFrom = string.IsNullOrEmpty(regInfo.ComeFrom) ? "LOCAL" : regInfo.ComeFrom,
                    AgentId = regInfo.AgentId,
                };
               regBiz.RegisterUser(userEntity, roleIds);
                userId = userEntity.UserId;

                #endregion

                #region 注册核心系统显示帐号

                var userRegInfo = new UserRegInfo
                {
                    DisplayName = regInfo.LoginName,
                    ComeFrom = string.IsNullOrEmpty(regInfo.ComeFrom) ? "LOCAL" : regInfo.ComeFrom,
                    Referrer = regInfo.Referrer,
                    ReferrerUrl = regInfo.ReferrerUrl,
                    RegisterIp = regInfo.RegisterIp,
                    RegType = regInfo.RegType,
                    AgentId = regInfo.AgentId,
                };

                regBiz.RegisterUser(userEntity, userRegInfo);

                #endregion

                #region 注册本地登录帐号

                var loginBiz = new LocalLoginBusiness();
                var loginEntity = new LoginLocal
                {
                    LoginName = regInfo.LoginName,
                    Password = regInfo.Password,
                    mobile = regInfo.Mobile
                };
                loginBiz.Register(loginEntity, userEntity.UserId);

            #endregion

            #region 如果是通过代理链接注册，则设置用户返点 屏蔽：范  

            if (!string.IsNullOrEmpty(regInfo.AgentId))
            {
                SetUserRebate(userId, regInfo.AgentId);
            }

            #endregion

            #region 初始化用户战绩数据和中奖概率数据

            InitUserBeedingAndBounsPercent(userId);

            #endregion

            #region 初始化其它数据

            InitBlog_ProfileBonusLevel(userId);
            InitUserAttentionSummary(userId);

            #endregion

            dBbase.DB.Commit();
            
            //! 执行扩展功能代码 - 提交事务后
            //BusinessHelper.ExecPlugin<IRegister_AfterTranCommit>(new object[] { regInfo.ComeFrom, userId });

            return new CommonActionResult
            {
                IsSuccess = true,
                Message = "注册成功",
                ReturnValue = userId,
            };

        }
        private void InitUserBeedingAndBounsPercent(string userId)
        {
            var sportsManager = new Sports_Manager();

            var allGameCodeArray = new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
            var lotteryGameCodeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
            foreach (var item in allGameCodeArray)
            {
                if (lotteryGameCodeArray.Contains(item))
                {
                    //数字彩
                    AddUserBeedingAndBonusPercent(sportsManager,userId, item, string.Empty);
                }
                else
                {
                    //足彩
                    var gameTypeArray = GetGameTypeArray(item);
                    foreach (var t in gameTypeArray)
                    {
                        AddUserBeedingAndBonusPercent(sportsManager,userId, item, t);
                    }

                }
            }
        }

        private void AddUserBeedingAndBonusPercent(Sports_Manager sportsManager, string userId, string gameCode, string gameType)
        {
            var beeding = sportsManager.QueryUserBeedings(userId, gameCode, gameType);
            if (beeding == null)
            {
                var UserBeedings=new C_User_Beedings
                {
                    UserId = userId,
                    UpdateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    BeFollowedTotalMoney = 0M,
                    BeFollowerUserCount = 0,
                    GoldCrownCount = 0,
                    GoldCupCount = 0,
                    GoldDiamondsCount = 0,
                    GoldStarCount = 0,
                    SilverCrownCount = 0,
                    SilverCupCount = 0,
                    SilverDiamondsCount = 0,
                    SilverStarCount = 0,
                    TotalBonusMoney = 0M,
                    TotalBonusTimes = 0,
                };
                sportsManager.AddUserBeedings(UserBeedings);

            }
            var bonusPercent = sportsManager.QueryUserBonusPercent(userId, gameCode, gameType);
            if (bonusPercent == null)
            {
                var UserBonusPercent= new C_User_BonusPercent
                {
                    BonusPercent = 0M,
                    CreateTime = DateTime.Now,
                    CurrentDate = DateTime.Now.ToString("yyyyMM"),
                    GameCode = gameCode,
                    GameType = gameType,
                    UserId = userId,
                    BonusOrderCount = 0,
                    TotalOrderCount = 0,
                };
                sportsManager.AddUserBonusPercent(UserBonusPercent);

            }
        }

        private string[] GetGameTypeArray(string gameCode)
        {
            switch (gameCode)
            {
                case "CTZQ":
                    return new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
                case "BJDC":
                    return new string[] { "SPF", "ZJQ", "SXDS", "BF", "BQC" };
                case "JCZQ":
                    return new string[] { "SPF", "BRQSPF", "BF", "ZJQ", "BQC", "HH" };
                case "JCLQ":
                    return new string[] { "SF", "RFSF", "SFC", "DXF", "HH" };
            }
            return new string[] { };
        }


        private void InitBlog_ProfileBonusLevel(string userId)
        {
            var manager = new BlogManager();
            var BlogProfileBonusLevel= new E_Blog_ProfileBonusLevel
            {
                UserId = userId,
                MaxLevelName = "幸运彩民",
                MaxLevelValue = 0,
                TotalBonusMoney = 0,
                UpdateTime = DateTime.Now,
                WinHundredMillionCount = 0,
                WinOneHundredCount = 0,
                WinOneHundredThousandCount = 0,
                WinOneMillionCount = 0,
                WinOneThousandCount = 0,
                WinTenMillionCount = 0,
                WinTenThousandCount = 0,
            };
            manager.AddBlog_ProfileBonusLevel(BlogProfileBonusLevel);

            var BlogDataReport=new E_Blog_DataReport
            {
                CreateSchemeCount = 0,
                JoinSchemeCount = 0,
                TotalBonusCount = 0,
                TotalBonusMoney = 0,
                UpdateTime = DateTime.Now,
                UserId = userId,
            };
            manager.AddBlogDataReport(BlogDataReport);

        }

        private void InitUserAttentionSummary(string userId)
        {
            var sportsManager = new Sports_Manager();
            var UserAttentionSummary= new C_User_Attention_Summary
            {
                UserId = userId,
                UpdateTime = DateTime.Now,
                BeAttentionUserCount = 0,
                FollowerUserCount = 0,
            };
            sportsManager.AddUserAttentionSummary(UserAttentionSummary);
        }

        private void SetUserRebate(string userId, string agentId)
        {
            try
            {
                var agentManager = new OCAgentManager();
                var parentRebateList = agentManager.QueryOCAgentRebateList(agentId);
                var rebateList = new List<string>();
                foreach (var item in parentRebateList)
                {
                    rebateList.Add(string.Format("{0}:{1}:{2}:{3}", item.GameCode, item.GameType, item.SubUserRebate, item.RebateType));
                }
                var setString = string.Join("|", rebateList.ToArray());
                //new OCAgentBusiness().UpdateOCAgentRebate(agentId, userId, setString);
                
                //new OCAgentBusiness().EditOCAgentRebate(agentId, userId, setString);
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("SetUserRebate", "SetUserRebate_设置返点", Common.Log.LogType.Error, "设置返点异常", ex.ToString());
            }
        }
        #endregion Implementation of IUserService
    }
}
