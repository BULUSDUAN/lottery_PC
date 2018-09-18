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

using System.Threading;
using UserLottery.Service.ModuleBaseServices;

using UserLottery.Service.IModuleServices;
using EntityModel.RequestModel;

using EntityModel.Enum;
using EntityModel.CoreModel;
using EntityModel.Communication;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Redis;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Sport;
using EntityModel.Redis;
using System.Diagnostics;

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
        private readonly UserRepository _repository;

        IKgLog log = null;
        public UserService(UserRepository repository) : base()
        {
            this._repository = repository;
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
        public Task<LoginInfo> User_Login(string loginName, string password, string loginIp)
        {

            try
            {
                string IPAddress = loginIp;
                var loginBiz = new LocalLoginBusiness();
                if (IPAddress == "Client")//移动端登录时，密码已经MD5
                    loginEntity = loginBiz.LoginAPP(loginName, password);
                else
                    loginEntity = loginBiz.Login(loginName, password);
                if (loginEntity == null)
                {
                    return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "登录名(手机号)或密码错误", LoginFrom = "LOCAL", });
                }
                ////var authBiz = new GameBizAuthBusiness();
                //if (!IsRoleType(loginEntity.User, RoleType.WebRole))
                //{
                //    return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "此帐号角色不允许在此登录", LoginFrom = "LOCAL", });
                //}
                if (!loginEntity.Register.IsEnable)
                {
                    return Task.FromResult(new LoginInfo { IsSuccess = false, Message = "用户未激活", LoginFrom = "LOCAL", UserId = loginEntity.UserId });
                }
                //var authBiz = new GameBizAuthBusiness();
                //var userToken = authBiz.GetUserToken(loginEntity.User.UserId);
                string userToken = KaSon.FrameWork.Common.CheckToken.UserAuthentication.GetUserToken(loginEntity.UserId, loginEntity.LoginName);
                var blogEntity = loginBiz.QueryBlog_ProfileBonusLevel(loginEntity.User.UserId);
                ////清理用户绑定数据缓存
                ////ClearUserBindInfoCache(loginEntity.UserId);
                //!执行扩展功能代码 - 提交事务前
                BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { loginEntity.UserId, "LOCAL", loginIp, DateTime.Now });
                //刷新用户在Redis中的余额
                BusinessHelper.RefreshRedisUserBalance(loginEntity.UserId);

                var MaxLevelName = "";
                if (blogEntity != null)
                {
                    MaxLevelName = string.IsNullOrEmpty(blogEntity.MaxLevelName) ? "" : blogEntity.MaxLevelName;
                }
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
                    MaxLevelName = MaxLevelName,
                    IsUserType = loginEntity.Register.UserType == 1 ? true : false
                });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }


        }


        /// <summary>
        /// 使用token登录
        /// </summary>
        public Task<LoginInfo> LoginByUserToken(string userId)
        {
            try
            {
                // 验证用户身份及权限
                //var userId = userAuthentication.ValidateUserAuthentication(userToken);
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
                        loginName = loginEntity.Result.LoginName;
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
                BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { userId, loginFrom, "", DateTime.Now });
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
                    //UserToken = userToken,
                    AgentId = reg.AgentId,
                    IsAgent = reg.IsAgent,
                    HideDisplayNameCount = reg.HideDisplayNameCount,
                });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }

        public bool IsRoleType(SystemUser user, RoleType roleType)
        {
            try
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
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 检查是否和资金密码一至
        /// </summary>
        /// <param name="newPassword">新密码</param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> CheckIsSame2BalancePassword(string newPassword, string userId)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var result = loginBiz.CheckIsSame2BalancePassword(userId, newPassword);
                var flag = "N";
                if (result.HasValue)
                {
                    flag = result.Value ? "T" : "F";
                }
                return Task.FromResult(new CommonActionResult(true, "查询成功") { ReturnValue = flag });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 查询用户绑定数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<UserBindInfos> QueryUserBindInfos(string userId)
        {

            try
            {
                //尝试从缓存中读取数据
                //var info = LoadUserBindInfoFromCache(userId);
                //if (info != null)
                //    return Task.FromResult(info);
                //从数据库读取数据
                var info = new LocalLoginBusiness().QueryUserBindInfos(userId);
                if (info == null)
                    return Task.FromResult(new UserBindInfos());
                //添加缓存到文件
                //SaveUserBindInfoCache(userId, info);
                return Task.FromResult(info);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 读取缓存的用户绑定数据
        /// </summary>
        private UserBindInfos LoadUserBindInfoFromCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelperEx.DB_UserBindData;
                var exist = db.ExistsAsync(fullKey).Result;
                if (!exist)
                    return null;
                var v = db.GetAsync(fullKey).Result;
                if (string.IsNullOrEmpty(v))
                    return null;
                var info = JsonHelper.Deserialize<UserBindInfos>(v);
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

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
                var db = RedisHelperEx.DB_UserBindData;
                db.Set(fullKey, content, 24 * 60 * 60);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        private void ClearUserBindInfoCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelperEx.DB_UserBindData;
                if (!db.Exists(fullKey))
                    db.Del(fullKey);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询我的余额
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<UserBalanceInfo> QueryMyBalance(string userId)
        {
            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);
            //if (userId == "admin")
            //    return null;
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var entity = loginBiz.QueryUserBalance(userId);
                //return new FundBusiness().QueryUserBalance(userId);
                return Task.FromResult(entity);
            }
            catch (LogicException ex)
            {
                throw ex;
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
        public Task<C_BankCard> QueryBankCard(string userId)
        {

            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);
            try
            {
                var loginBiz = new LocalLoginBusiness();
                return Task.FromResult(loginBiz.BankCardById(userId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 获取我的未读站内信条数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<int> GetMyUnreadInnerMailCount(string userId)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                // 验证用户身份及权限
                //var userId = userAuthentication.ValidateUserAuthentication(userToken);


                return Task.FromResult(loginBiz.GetUnreadMailCountByUser(userId));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }


        #region 修改密码


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="userToken">用户Token</param>
        /// <returns></returns>
        public Task<CommonActionResult> ChangeMyPassword(string oldPassword, string newPassword, string userId)
        {
            try
            {
                // 验证用户身份及权限
                //var userId = userAuthentication.ValidateUserAuthentication(userToken);
                var loginBiz = new LocalLoginBusiness();
                loginBiz.ChangePassword(userId, oldPassword, newPassword);

                return Task.FromResult(new CommonActionResult(true, "修改密码成功"));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new Exception("修改密码失败", ex);
            }


        }

        #endregion

        /// <summary>
        /// 根据UserId查询用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<LoginInfo> GetLocalLoginByUserId(string userId)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                //var local = loginBiz.GetLocalLoginByUserId(userId);
                //if (local == null)
                //    return Task.FromResult(new LoginInfo());
                var register = loginBiz.GetRegisterById(userId);
                if (register == null)
                    return null;
                return Task.FromResult(new LoginInfo
                {
                    CreateTime = register.CreateTime,
                    RegType = register.RegType,
                    Referrer = register.Referrer,
                    UserId = register.UserId,
                    VipLevel = register.VipLevel,
                    LoginName = register.DisplayName,
                    DisplayName = register.DisplayName,
                    AgentId = register.AgentId,
                    IsAgent = register.IsAgent,
                    HideDisplayNameCount = register.HideDisplayNameCount,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 配置项
        /// </summary>
        private static C_Core_Config _coreConfigList = new C_Core_Config();
        public Task<C_Core_Config> QueryCoreConfigByKey(string key)
        {
            try
            {
                var loginBiz = new MobileAuthenticationBusiness();
                _coreConfigList = loginBiz.BanRegistrMobile(key);

                return Task.FromResult(_coreConfigList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


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
            catch (LogicException ex)
            {
                throw ex;
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
            try
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
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }


        /// <summary>
        /// 手机认证，重发验证码或更换号码
        /// </summary>
        public void RepeatRequestMobile2(string userId, string mobile, string createUserId)
        {
            try
            {
                var manager = new MobileAuthenticationBusiness();

                if (string.IsNullOrEmpty(userId))
                    throw new LogicException("未查询到用户编号");
                else if (string.IsNullOrEmpty(mobile))
                    throw new LogicException("手机号码不能为空");
                //var entity = manager.GetUserMobile(userId);
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile && other.UserId != userId)
                {
                    throw new LogicException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                }
                var entity = manager.GetAuthenticatedMobile(userId);
                if (entity != null)
                {
                    entity.IsSettedMobile = false;
                    entity.UpdateBy = createUserId;
                    //entity.RequestTimes++;
                    entity.Mobile = mobile;
                    //manager.UpdateUserMobile(entity);
                }
            }

            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
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

        #region 回复手机验证
        /// <summary>
        /// 回复手机认证 
        /// </summary>
        /// <param name="validateCode"></param>
        /// <param name="source"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> ResponseAuthenticationMobile(string validateCode, SchemeSource source, string userId)
        {
            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);

            try
            {
                if (string.IsNullOrEmpty(validateCode))
                    throw new LogicException("验证码不能为空");
                var isCheckValidateCode = false;

                var authenticationBiz = new MobileAuthenticationBusiness();
                var mobile = authenticationBiz.GetAuthenticatedMobile(userId);
                if (mobile == null)
                {
                    throw new LogicException("尚未请求手机认证");
                }

                var mobileBiz = new ValidationMobileBusiness();
                isCheckValidateCode = mobileBiz.CheckValidationCode(mobile.Mobile, "MobileAuthentication", validateCode, GetMaxTimes(3));

                if (!isCheckValidateCode)
                {
                    throw new LogicException("验证码输入不正确。");
                }
                string mobileNumber;
                mobileNumber = authenticationBiz.ResponseAuthenticationMobile(userId, 1800, "半个小时");
                //清理用户绑定数据缓存
                ClearUserBindInfoCache(userId);

                #region 发送站内消息：手机短信或站内信

                var userBusiness = new LocalLoginBusiness();
                var user = userBusiness.GetRegisterById(userId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[MobileNumber]", mobile.Mobile));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Bind_Mobile", pList.ToArray());

                #endregion


                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userId, "Mobile", mobileNumber, source });

                return Task.FromResult(new CommonActionResult(true, "手机认证成功。"));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                return Task.FromResult(new CommonActionResult(false, ex.Message));
            }
        }

        #endregion
        /// <summary>
        /// 验证手机
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <param name="mobile">手机号</param>
        /// <param name="source"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public Task<CommonActionResult> RegisterResponseMobile(string validateCode, string mobile, SchemeSource source, RegisterInfo_Local info, string fxid)
        {
            try
            {

                if (string.IsNullOrEmpty(validateCode))
                    throw new LogicException("验证码不能为空");
                var isCheckValidateCode = false;
                var authenticationBiz = new MobileAuthenticationBusiness();
                var ValidationMobile = new ValidationMobileBusiness();
                #region "20180522新增:用户名注册修改为手机号注册后,存在老用户绑定了手机号后，可以继续用手机号注册一个新号"
                if (authenticationBiz.IsMobileAuthenticated(mobile))
                    throw new LogicException("该手机号已经认证");
                #endregion      

                isCheckValidateCode = ValidationMobile.CheckValidationCode(mobile, "MobileAuthentication", validateCode, GetMaxTimes(3));

                if (!isCheckValidateCode)
                {
                    throw new LogicException("验证码输入不正确。");
                }
                info.Referrer = fxid == "0" ? "mobile_regist" : "fxid_regist";
                //注册
                var userResult = RegisterLoacal(info, fxid);
                if (userResult == null || string.IsNullOrEmpty(userResult.ReturnValue))
                    throw new Exception("注册失败,请重新注册");
                string mobileNumber;
                mobileNumber = authenticationBiz.RegisterResponseMobile(userResult.ReturnValue, mobile, 1800, "半个小时");


                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userResult.ReturnValue, "Mobile", mobileNumber, source });

                return Task.FromResult(new CommonActionResult(true, "恭喜您注册成功！"));
            }
            catch (LogicException ex)
            {
                throw ex;
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


        /// <summary>
        /// 本地 注册账号
        /// </summary>
        public CommonActionResult RegisterLoacal(RegisterInfo_Local regInfo, string fxid)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigHelper.AllConfigInfo["PageRegisterDefaultRole"].ToString()))
                {
                    throw new ArgumentNullException("未配置前台注册用户默认角色的参数 - PageRegisterDefaultRole");
                }
                if (string.IsNullOrEmpty(regInfo.LoginName))
                    throw new LogicException("登录名不能为空");
                else if (Regex.IsMatch(regInfo.LoginName, "[ ]+"))
                    throw new LogicException("登录名不能包含空格");
                if (regInfo != null && !string.IsNullOrEmpty(regInfo.AgentId))
                {

                    var userEntity2 = new LocalLoginBusiness();
                    var userEntity3 = userEntity2.QueryUserRegisterByUserId(regInfo.AgentId);
                    if (userEntity3 == null || !userEntity3.IsAgent)
                    {
                        regInfo.AgentId = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(fxid))
                {
                    var userEntity = new LocalLoginBusiness().QueryUserRegisterByUserId(fxid);
                    if (userEntity == null)
                    {
                        fxid = string.Empty;
                    }
                }


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



                var success = new RegisterBusiness().UserRegister(regInfo, fxid);


                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IRegister_AfterTranCommit>(new object[] { regInfo.ComeFrom, success.ReturnValue });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "注册成功",
                    ReturnValue = success.ReturnValue,
                };

            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }



        /// <summary>
        /// 注册验证手机 
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Task<CommonActionResult> RegisterRequestMobile(string mobile)
        {

            try
            {
                var validateCode = GetRandomMobileValidateCode();



                var authenticationBiz = new MobileAuthenticationBusiness();
                authenticationBiz.RegisterRequestMobile(mobile);

                var biz = new ValidationMobileBusiness();
                validateCode = biz.SendValidationCode(mobile, "MobileAuthentication", validateCode, GetDelay(60), GetMaxTimes(3));

                #region 发送站内消息：手机短信或站内信
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[ValidNumber]", validateCode));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage("", mobile, "ON_User_Bind_Mobile_Before", pList.ToArray());

                #endregion

                return Task.FromResult(new CommonActionResult(true, "已成功提交手机认证申请，请等待。") { ReturnValue = validateCode });
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                return Task.FromResult(new CommonActionResult(false, ex.Message));
            }
        }
        private int GetDelay(int delay)
        {
            //if (UsefullHelper.IsInTest)
            //{
            //    return 0;
            //}
            return delay;
        }

        #region 判断手机号是否已被注册
        public Task<bool> HasMobile(string mobile)
        {
            try
            {
                var validateCode = GetRandomMobileValidateCode();
                var authenticationBiz = new MobileAuthenticationBusiness();
                var flag = authenticationBiz.HasMobile(mobile);
                return Task.FromResult(flag);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }
        #endregion


        #region 判断找回密码验证码是否正确
        public Task<bool> CheckValidateCodeByForgetPWD(string mobile, string validateCode)
        {
            try
            {
                var flag = false;
                var biz = new ValidationMobileBusiness();
                flag = biz.CheckValidationCode(mobile, "SendValidateCodeToUserMobileByForgetPWD", validateCode, GetMaxTimes(5));
                return Task.FromResult(flag);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }
        #endregion

        /// <summary>
        /// 根据登录名查询用户Id
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<string> GetUserIdByLoginName(string loginName)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var user = loginBiz.GetUserByLoginName(loginName);
                return Task.FromResult(user.UserId);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> FindPassword(string userId)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var pwd = loginBiz.ChangePassword(userId);
                return Task.FromResult(new CommonActionResult(true, "修改密码成功") { ReturnValue = pwd });
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }


        /// <summary>
        /// 某场景触发的发送站内消息
        /// </summary>
        public Task<CommonActionResult> DoSendSiteMessage(string userId, string mobile, string sceneKey, string msgTemplateParams)
        {
            try
            {
                var siteBiz = new SiteMessageControllBusiness();
                siteBiz.DoSendSiteMessage(userId, mobile, sceneKey, msgTemplateParams.Split('|'));
                return Task.FromResult(new CommonActionResult(true, "发送完成"));

            }
            catch (Exception ex)
            {
                return Task.FromResult(new CommonActionResult(false, ex.Message));
            }
        }


        #region 找回密码发送验证码
        /// <summary>
        /// 找回密码发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Task<CommonActionResult> SendValidateCodeToUserMobileByForgetPWD(string mobile)
        {
            try
            {
                var validateCode = GetRandomMobileValidateCode();
                var authenticationBiz = new MobileAuthenticationBusiness();
                var flag = authenticationBiz.HasMobile(mobile);
                if (flag) //如果手机号已注册则发送
                {

                    var biz = new ValidationMobileBusiness();
                    //SendValidateCodeToUserMobileByForgetPWD
                    validateCode = biz.SendValidationCode(mobile, "SendValidateCodeToUserMobileByForgetPWD", validateCode, GetDelay(30), GetMaxTimes(3));
                    var pList = new List<string>();
                    pList.Add(string.Format("{0}={1}", "[ValidNumber]", validateCode));
                    //发送短信
                    new SiteMessageControllBusiness().DoSendSiteMessage("", mobile, "ON_User_Bind_Mobile_Before", pList.ToArray());
                    return Task.FromResult(new CommonActionResult
                    {
                        IsSuccess = true,
                        ReturnValue = validateCode,
                    });
                }
                else
                {
                    return Task.FromResult(new CommonActionResult
                    {
                        IsSuccess = false,
                        ReturnValue = "",
                        Message = "该手机号未注册"
                    });
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }
        #endregion


        /// <summary>
        /// 检查是否和登录密码一至
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        //public Task<CommonActionResult> CheckIsSame2LoginPassword(string newPwd, string userId)
        //{
        //    try
        //    {
        //        // 验证用户身份及权限
        //        //var userId = userAuthentication.ValidateUserAuthentication(userToken);

        //        var loginBiz = new LocalLoginBusiness();
        //        var result = loginBiz.CheckIsSame2LoginPassword(userId, newPwd);
        //        var flag = "N";
        //        if (result.HasValue)
        //        {
        //            flag = result.Value ? "T" : "F";
        //        }
        //        return Task.FromResult(new CommonActionResult(true, "查询成功") { ReturnValue = flag });
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.Message, ex);
        //    }

        //}

        /// <summary>
        /// 设置资金密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="isSetPwd"></param>
        /// <param name="newPassword"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> SetBalancePassword(string oldPassword, bool isSetPwd, string newPassword, string userId, string placeList)
        {
            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new FundBusiness();
                var loginBiz = new LocalLoginBusiness();
                var result = loginBiz.CheckIsSame2LoginPassword(userId, newPassword);
                var flag = false;
                if (result.HasValue)
                {
                    flag = result.Value;
                }
                if (flag)
                {
                    throw new Exception("资金密码不能与登录密码相同");
                }
                biz.SetBalancePassword(userId, oldPassword, isSetPwd, newPassword, placeList);

                BusinessHelper.ExecPlugin<IBalancePassword>(new object[] { userId, oldPassword, isSetPwd, newPassword });

                return Task.FromResult(new CommonActionResult { IsSuccess = true, Message = "操作资金密码完成" });
            }
            catch (Exception ex)
            {
                throw new Exception("操作资金密码出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置资金密码类型
        /// </summary>
        /// <param name="password"></param>
        /// <param name="placeList"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> SetBalancePasswordNeedPlace(string password, string placeList, string userId)
        {
            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);

            var innerList = new string[] {
                "Bet"               // 投注
                , "Withdraw"        // 提现
                , "Transfer"        // 转账
                , "Red"             // 送红包
                , "CancelWithdraw"  // 取消提现
                , "CancelChase"     // 取消追号
                ,"BuyExperter"      //购买名家分析
                ,"ExchangeDouDou"   //豆豆兑换
            };
            if (placeList != "ALL")
            {
                var list = placeList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (list.Length > 0)
                {
                    foreach (var item in placeList.Split('|'))
                    {
                        if (!innerList.Contains(item))
                        {
                            throw new Exception("不支持设置资金密码类型 - " + item);
                        }
                    }
                }
            }
            try
            {
                var biz = new FundBusiness();
                biz.SetBalancePasswordNeedPlace(userId, password, placeList);
                return Task.FromResult(new CommonActionResult { IsSuccess = true, Message = "操作完成" });
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("设置资金密码类型出错 - " + ex.Message, ex);
            }
        }



        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            try
            {
                return new SqlQueryBusiness().QueryYqidRegisterByAgentId(AgentId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询某个yqid下面的能满足领红包条件的用户个数出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// QueryYqidRegisterByAgentId方法的手机接口
        /// </summary>
        /// <param name = "userToken" ></ param >
        /// < returns ></ returns >
        public Task<string> QueryYqidRegisterByAgentIdToApp(string userId)
        {
            try
            {
                // 验证用户身份及权限
                //var userId = userAuthentication.ValidateUserAuthentication(userToken);
                return Task.FromResult(QueryYqidRegisterByAgentId(userId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 用户实名认证
        /// </summary>
        /// <param name="realNameInfo"></param>
        /// <param name="source"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> AuthenticateMyRealName(string IdCardNumber, string RealName, SchemeSource source, string userId)
        {
            try
            {
                // 验证用户身份及权限
                //var userId = userAuthentication.ValidateUserAuthentication(userToken);

                var biz = new RealNameAuthenticationBusiness();
                BettingHelper.CheckUserRealName(IdCardNumber);

                if (!BettingHelper.CheckIDCard18(IdCardNumber)) {

                    throw new LogicException("请输入正确的身份证号码");
                }
                var realName = biz.GetAuthenticatedRealName(userId);
                if (realName != null)
                {
                    if (realName.IsSettedRealName)
                    {
                        throw new LogicException("此用户已进行过实名认证，不能重复认证");
                    }
                    biz.UpdateAuthenticationRealName("LOCAL", userId, RealName, "0", IdCardNumber, userId);
                }
                else
                {
                    biz.AddAuthenticationRealName("LOCAL", userId, RealName, "0", IdCardNumber, userId, true);
                }

                #region 发送站内消息：手机短信或站内信

                var userManager = new UserBalanceManager();
                var user = userManager.QueryUserRegister(userId);
                var pList = new List<string>();
                pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                pList.Add(string.Format("{0}={1}", "[UserRealityName]", RealName));
                pList.Add(string.Format("{0}={1}", "[IdCard]", IdCardNumber));
                //发送短信
                new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Bind_RealName", pList.ToArray());

                #endregion

                //清理用户绑定数据缓存
                ClearUserBindInfoCache(userId);


                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userId, "RealName", RealName + "|" + IdCardNumber, source });


                return Task.FromResult(new CommonActionResult(true, "实名认证成功。"));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 增加银行卡信息
        /// </summary>
        /// <param name="bankCard"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> AddBankCard(C_BankCard bankCard, string userId)
        {
            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);

            try
            {
                var entity = new BankCardManager().BankCardByCode(bankCard.BankCardNumber);
                if (entity != null)
                {
                    throw new LogicException("该银行卡号已经被其他用户绑定，请选择其它银行卡号");
                }
                if (string.IsNullOrEmpty(bankCard.UserId) || bankCard.UserId == null || bankCard.UserId.Length == 0)
                    bankCard.UserId = userId;

                var bankcarduser = new BankCardManager().BankCardById(userId);
                if (bankcarduser != null)
                    throw new LogicException("您已绑定了银行卡，请不要重复绑定！");
                new BankCardBusiness().AddBankCard(bankCard);
                new CacheDataBusiness().ClearUserBindInfoCache(userId);
                //绑定银行卡之后实现接口

                BusinessHelper.ExecPlugin<IAddBankCard>(new object[] { bankCard.UserId, bankCard.BankCardNumber, bankCard.BankCode, bankCard.BankName, bankCard.BankSubName, bankCard.CityName, bankCard.ProvinceName, bankCard.RealName });

                return Task.FromResult(new CommonActionResult(true, "添加银行卡信息成功"));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("添加银行卡信息出错 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 提款确认  205
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestMoney"></param>
        /// <returns></returns>
        public Task<CheckWithdrawResult> RequestWithdraw_Step1(string userId, decimal requestMoney)
        {
            try
            {
                return Task.FromResult(new FundBusiness().RequestWithdraw_Step1(userId, requestMoney));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("申请提现出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 提款成功
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<CommonActionResult> RequestWithdraw_Step2(Withdraw_RequestInfo info, string userId, string balancepwd)
        {
            try
            {
                new FundBusiness().RequestWithdraw_Step2(info, userId, balancepwd);
                return Task.FromResult(new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "申请提现成功",
                });
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("申请提现出错 - " + ex.Message, ex);
            }
        }


        public Task<Withdraw_QueryInfoCollection> QueryMyWithdrawList(int status, int pageIndex, int pageSize, string userId)
        {

            // 验证用户身份及权限
            //var userId = userAuthentication.ValidateUserAuthentication(userToken);

            try
            {
                return Task.FromResult(new FundBusiness().QueryWithdrawList(userId, null, status, -1, -1, 1, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的提现记录列表 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询银行卡
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>\
        private static C_Bank_Info _BankCardList = new C_Bank_Info();
        public Task<C_Bank_Info> QueryBankInfo(string bankCode)
        {
            try
            {
                var fub = new FundBusiness();
                _BankCardList = fub.QueryBankInfo(bankCode);
                return Task.FromResult(_BankCardList);
            }
            catch (Exception ex)
            {
                throw new Exception("出错 - " + ex.Message, ex);
            }
        }

        #region 通过分享中奖订单注册后送上线红包
        public Task<CommonActionResult> OrderShareRegisterRedBag(string schemeId)
        {
            new CacheDataBusiness().FirstOrderShareRegisterRedBag(schemeId);
            return Task.FromResult(new CommonActionResult()
            {
                IsSuccess = true
            });
        }

        #endregion

        //public async Task<bool> SetVerifyCodeByGuid(string RedisKey,string RedisValue)
        //{
        //    try
        //    {
        //        var db = RedisHelperEx.DB_UserBindData;
        //        var result= await db.StringSetAsync(RedisKey, RedisValue, TimeSpan.FromMinutes(10));
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("获取图形验证码失败 - " + ex.Message, ex);
        //    }
        //}


        //public async Task<string> GetVerifyCodeByGuid(string RedisKey)
        //{
        //    try
        //    {
        //        var db = RedisHelperEx.DB_UserBindData;
        //        var value= await db.StringGetAsync(RedisKey);
        //        return value;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("存储图形验证码失败 - " + ex.Message, ex);
        //    }
        //}
        #endregion Implementation of IUserService

        public Task<string> ReadSqlTimeLog(string FileName)
        {
            if (string.IsNullOrEmpty(FileName)) FileName = "SQLInfo";//SevTimeIoginfo 服务时间

            return Task.FromResult(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\" + FileName, "LogTime_"));
        }

        /// <summary>
        /// 获取redisvalue
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Task<string> GetRedisByOtherDbKey(string Key)
        {
            try
            {
                var db = RedisHelperEx.DB_Other;
                return Task.FromResult(db.Get(Key));
            }
            catch (Exception ex)
            {   
                throw new Exception("获取失败",ex);
            }
        }

        public Task<bool> SetRedisOtherDbKey(string Key,string RValue,int TotalSeconds)
        {
            try
            {
                var db = RedisHelperEx.DB_Other;
                var flag= db.Set(Key, RValue, TotalSeconds);
                return Task.FromResult(flag);
            }
            catch (Exception ex)
            {
                throw new Exception("设置失败", ex);
            }
        }

        //public Task<string> ReadSevTimeLog(string FileName)
        //{
        //    if (string.IsNullOrEmpty(FileName)) FileName = "SQLInfo";//SevTimeIoginfo 服务时间

        //    return Task.FromResult(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\SQLInfo", "LogTime_"));
        //}
    }
}
