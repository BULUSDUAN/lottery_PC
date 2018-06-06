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
using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Lottery.Kg.ORM.Helper.UserHelper;

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

        public Task<string> User_Login(string loginName,string password)
        {
            //QueryUserParam model = new QueryUserParam();
            var loginBiz = new LocalLoginBusiness();
            E_Login_Local loginEntity = new E_Login_Local();
            //if (model.IPAddress == "Client")//移动端登录时，密码已经MD5
            //    loginEntity = loginBiz.LoginAPP(model.loginName, model.password);
            //else
                loginEntity = loginBiz.Login(loginName, password);
            if (loginEntity == null)
            {
               
            }
            //var authBiz = new GameBizAuthBusiness();
            //if (!IsRoleType(loginEntity.User, RoleType.WebRole))
            //{

            //}
            //if (!loginEntity.Register.IsEnable)
            //{

            //}
            //var userToken = authBiz.GetUserToken(loginEntity.User.UserId);
            //var blogManager = new BlogManager();
            //var blogEntity = blogManager.QueryBlog_ProfileBonusLevel(loginEntity.User.UserId);

            ////清理用户绑定数据缓存
            ////ClearUserBindInfoCache(loginEntity.UserId);

            ////! 执行扩展功能代码 - 提交事务前
            //BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { loginEntity.UserId, "LOCAL", loginIp, DateTime.Now });
            ////刷新用户在Redis中的余额
            //BusinessHelper.RefreshRedisUserBalance(loginEntity.UserId);
            //return new LoginInfo
            //{
            //    IsSuccess = true,
            //    Message = "登录成功",
            //    CreateTime = loginEntity.CreateTime,
            //    LoginFrom = "LOCAL",
            //    RegType = loginEntity.Register.RegType,
            //    Referrer = loginEntity.Register.Referrer,
            //    UserId = loginEntity.User.UserId,
            //    VipLevel = loginEntity.Register.VipLevel,
            //    LoginName = loginEntity.LoginName,
            //    DisplayName = loginEntity.Register.DisplayName,
            //    UserToken = userToken,
            //    AgentId = loginEntity.Register.AgentId,
            //    IsAgent = loginEntity.Register.IsAgent,
            //    HideDisplayNameCount = loginEntity.Register.HideDisplayNameCount,
            //    MaxLevelName = string.IsNullOrEmpty(blogEntity.MaxLevelName) ? "" : blogEntity.MaxLevelName,
            //    IsUserType = loginEntity.Register.UserType == 1 ? true : false
            //};
            return Task.FromResult("");
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
