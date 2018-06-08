﻿using KaSon.FrameWork.ORM.Provider;

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
        //public void PublicInfo(IntegrationEvent evt)
        //{
        //    Publish(evt);
        //}

        //Task IIntegrationEventHandler<EventModel>.Handle(EventModel @event)
        //{
        //    throw new NotImplementedException();
        //}

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
        LoginLocal loginEntity = new LoginLocal();
        private BusinessHelper businessHelper;
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

  
        #region 修改密码

        private UserAuthentication userAuthentication = new UserAuthentication();
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
          
            var loginBiz = new LocalLoginBusiness();        
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
                var manager = new LocalLoginBusiness();
            
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
                //var entity = manager.GetUserMobile(userId);
                //if (other != null)
                //{
                //    other.IsSettedMobile = false;
                //    other.UpdateBy = createUserId;
                //    other.RequestTimes++;
                //    entity.Mobile = mobile;
                //    manager.UpdateUserMobile(entity);
                //}
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

        public Task<int> GetUserId(string userName)
        {
            //var xid = RpcContext.GetContext().GetAttachment("xid");

            //throw new Exception("错误！");

            //测试容错
            // Thread.Sleep(200000);

            //var T1 = TTest1();
            //var T21 = Test21();
            //var T2 = Test2();
            //var T3 = Test3();
            LoginHelper loginHelper = new LoginHelper();
            //查询用户明
            var list = loginHelper.QueryUserName();
            return Task.FromResult(1);
        }

        public Task<List<User>> GetUserList(string userName)
        {
            //var xid = RpcContext.GetContext().GetAttachment("xid");

            //throw new Exception("错误！");

            //  var T1 = TTest2();
            //var T21 = Test21();
            //var T2 = Test2();
            //var T3 = Test3();
            LoginHelper loginHelper = new LoginHelper();
            //查询用户明
            var list = loginHelper.QueryUserName();

            foreach (var item in list)
            {
                Console.WriteLine(item.Name);
            }

            //  var list = new List<User>();

            return Task.FromResult(list);
        }

        public Task<List<E_Login_Local>> GetLoginUserList(string userName)
        {
            //var xid = RpcContext.GetContext().GetAttachment("xid");

            //throw new Exception("错误！");

            //  var T1 = TTest2();
            //var T21 = Test21();
            //var T2 = Test2();
            //var T3 = Test3();
            LoginHelper loginHelper = new LoginHelper();
            //查询用户明
            var list = loginHelper.QueryloginUserName();

            foreach (var item in list)
            {
                Console.WriteLine(item.LoginName);
            }

            //  var list = new List<User>();

            return Task.FromResult(list);
        }

        //获取用户
        public Task<UserModel> GetUser(UserModel user)
        {
            return Task.FromResult(new UserModel
            {
                Name = "fanly",
                Age = 18
            });
        }


        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            // Publish(evt);
            await Task.CompletedTask;
        }

       



        #endregion Implementation of IUserService
    }
}
