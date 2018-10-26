using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using KaSon.FrameWork.Services.ORM;
using EntityModel.ORM;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Services.Enum;
using EntityModel.CoreModel;
using System.Linq.Expressions;
using log4net.Plugin;
using EntityModel.Communication;
using EntityModel.CoreModel.AuthEntities;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KaSon.FrameWork.ORM.Helper
{
    public class LocalLoginBusiness : DBbase
    {
        //改版密码增加的关键字
        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public LoginLocal Login(string loginName, string password)
        {
            password = MD5Helper.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
            LoginLocal loginInfo = UserLogin(loginName, password);
            return loginInfo;
        }
        public LoginLocal AdminLogin(string loginName, string password)
        {
            password = MD5Helper.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
            LoginLocal loginInfo = AdminUserLogin(loginName, password);
            return loginInfo;
        }
        public LoginLocal LoginAPP(string loginName, string password)
        {
            LoginLocal loginInfo = UserLogin(loginName, password);
            return loginInfo;
        }

        #region test
        //private UserAuthentication userAuthentication = new UserAuthentication();
        //public static SystemUser systemUser = new SystemUser();
        //public static SystemRole systemRole = new SystemRole();
        /// <summary>
        /// 用户名(手机)密码登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //        public LoginLocal UserLogin_old(string loginName, string password)
        //        {

        //            var LoginUser = DB.CreateQuery<E_Login_Local>();


        //            String pattern = @"^(0|86|17951)?(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57])[0-9]{8}$";//"^((1[3,5,8][0-9])|(14[5,7])|(17[0,6,7,8])|(19[7]))\\d{8}$";

        //#if LogInfo

        //                    Stopwatch watch = new Stopwatch();
        //              Double opt=0 ,opt1 = 0, opt2 = 0, opt3= 0;
        //            int count=0, count1=0;

        //            watch.Start();
        //#endif
        //            LoginLocal LoginUsers = null;
        //            if (Regex.IsMatch(loginName, pattern))
        //            {
        //                LoginUsers = LoginUser.Where(p => p.mobile == loginName && p.Password == password).Select(p => new LoginLocal
        //                {
        //                    CreateTime = p.CreateTime,
        //                    LoginName = p.LoginName,
        //                    mobile = p.mobile,
        //                    Password = p.Password,
        //                    UserId = p.UserId,

        //                }).FirstOrDefault();
        //            }
        //            else
        //            {
        //                LoginUsers = LoginUser.Where(p => (p.LoginName == loginName) && p.Password == password).Select(p => new LoginLocal
        //                {
        //                    CreateTime = p.CreateTime,
        //                    LoginName = p.LoginName,
        //                    mobile = p.mobile,
        //                    Password = p.Password,
        //                    UserId = p.UserId,

        //                }).FirstOrDefault();
        //            }


        //#if LogInfo
        //            watch.Stop();
        //            opt = watch.Elapsed.TotalMilliseconds;
        //            watch.Reset();
        //            count = DB.CreateQuery<C_Auth_Users>().Count();
        //            count1 = LoginUser.Count();
        //            watch.Stop();
        //#endif
        //            if (LoginUsers != null)
        //            {

        //                LoginUsers.User = (from p in DB.CreateQuery<C_Auth_Users>()
        //                                   where p.UserId == LoginUsers.UserId
        //                                   select new SystemUser()
        //                                   {
        //                                       CreateTime = p.CreateTime,
        //                                       AgentId = p.AgentId,
        //                                       RegFrom = p.RegFrom,
        //                                       UserId = p.UserId,
        //                                   }).FirstOrDefault();


        //                if (LoginUsers.User != null)
        //                {
        //#if LogInfo
        //                    watch.Reset();


        //#endif

        //                    var uQueryRoles = (from p in DB.CreateQuery<C_Auth_Roles>()
        //                                       select p).ToList().Select(p => new SystemRole()
        //                                       {
        //                                           RoleId = p.RoleId,
        //                                           RoleName = p.RoleName,
        //                                           IsInner = p.IsInner,
        //                                           IsAdmin = p.IsAdmin,
        //                                           RoleType = (RoleType)p.RoleType,
        //                                       }).ToList();

        //#if LogInfo


        //                    opt2 = watch.Elapsed.TotalMilliseconds;
        //                    watch.Stop();
        //                    watch.Reset();
        //#endif

        //                    var uQueryUserRole = DB.CreateQuery<C_Auth_UserRole>();
        //                    LoginUsers.User.RoleList = (from b in uQueryRoles
        //                                                join c in uQueryUserRole
        //                                                on b.RoleId equals c.RoleId
        //                                                where c.UserId == LoginUsers.UserId
        //                                                select b).ToList();

        //                    systemUser.RoleList = LoginUsers.User.RoleList;

        //                    var C_Auth_RoleFunction_query = DB.CreateQuery<C_Auth_RoleFunction>();
        //                    // var C_Auth_UserRole_query = DB.CreateQuery<C_Auth_UserRole>();
        //                    var C_Auth_Function_List = DB.CreateQuery<C_Auth_Function_List>();
        //                    //systemRole.FunctionList
        //                    var RoleFunctionList = (from b in C_Auth_RoleFunction_query
        //                                            join d in uQueryUserRole

        //                                            on b.RoleId equals d.RoleId
        //                                            where d.UserId == LoginUsers.UserId
        //                                            select b
        //                                             ).Select(p => new RoleFunction()
        //                                             {
        //                                                 FunctionId = p.FunctionId,
        //                                                 IId = p.IId,
        //                                                 Mode = p.Mode,

        //                                             }).ToList();

        //                    if (RoleFunctionList != null && RoleFunctionList.Count() != 0)
        //                    {

        //                        var Ids = RoleFunctionList.Select(p => p.FunctionId).ToList();
        //                        var Auth_Function_Lists = (from p in DB.CreateQuery<C_Auth_Function_List>()
        //                                                   where Ids.Contains(p.FunctionId)
        //                                                   select new Function()
        //                                                   {
        //                                                       DisplayName = p.DisplayName,
        //                                                       FunctionId = p.FunctionId,
        //                                                       IsBackBasic = p.IsBackBasic,
        //                                                       IsWebBasic = p.IsWebBasic,
        //                                                       ParentId = p.ParentId,
        //                                                       ParentPath = p.ParentPath,
        //                                                   }).ToList();
        //                        systemRole.FunctionList = RoleFunctionList;

        //                    }

        //                    var C_Auth_UserFunction_query = DB.CreateQuery<C_Auth_UserFunction>();
        //                    // var C_Auth_UserRole = DB.CreateQuery<C_Auth_UserRole>();
        //                    var C_Auth_UserFunction_List = DB.CreateQuery<C_Auth_Function_List>();
        //                    //systemRole.FunctionList
        //                    var UserFunctionList = (from b in C_Auth_UserFunction_query
        //                                            join d in uQueryUserRole

        //                                            on b.UserId equals d.UserId
        //                                            where d.UserId == LoginUsers.UserId
        //                                            select b
        //                                             ).ToList().Select(p => new UserFunction()
        //                                             {
        //                                                 FunctionId = p.FunctionId,
        //                                                 IId = p.IId,
        //                                                 Mode = p.Mode,


        //                                             }).ToList();
        //                    systemUser.FunctionList = UserFunctionList;
        //                    if (UserFunctionList != null && UserFunctionList.Count() != 0)
        //                    {

        //                        var Ids = RoleFunctionList.Select(p => p.FunctionId).ToList();
        //                        var Auth_Function_Lists = DB.CreateQuery<C_Auth_Function_List>().Where(p => Ids.Contains(p.FunctionId)).Select(p => new Function()
        //                        {
        //                            DisplayName = p.DisplayName,
        //                            FunctionId = p.FunctionId,
        //                            IsBackBasic = p.IsBackBasic,
        //                            IsWebBasic = p.IsWebBasic,
        //                            ParentId = p.ParentId,
        //                            ParentPath = p.ParentPath,
        //                        }).ToList();
        //                        systemUser.FunctionList = UserFunctionList;

        //                    }

        //                    string userId = LoginUsers.UserId;

        //                    LoginUsers.Register = (from p in DB.CreateQuery<C_User_Register>()
        //                                           where p.UserId == userId
        //                                           select p).ToList().Select(p => new UserRegister()
        //                                           {

        //                                               AgentId = p.AgentId,
        //                                               ComeFrom = p.ComeFrom,
        //                                               CreateTime = p.CreateTime,
        //                                               DisplayName = p.DisplayName,
        //                                               HideDisplayNameCount = p.HideDisplayNameCount,
        //                                               IsAgent = p.IsAgent,
        //                                               IsEnable = p.IsEnable,
        //                                               IsFillMoney = p.IsFillMoney,
        //                                               IsIgnoreReport = p.IsIgnoreReport,
        //                                               ParentPath = p.ParentPath,
        //                                               Referrer = p.Referrer,
        //                                               ReferrerUrl = p.ReferrerUrl,
        //                                               RegisterIp = p.RegisterIp,
        //                                               RegType = p.RegType,
        //                                               UserId = p.UserId,
        //                                               UserType = p.UserType,
        //                                               VipLevel = p.VipLevel
        //                                           }).FirstOrDefault();



        //                    LoginUsers.Register.IsEnable = LoginUsers.Register.IsEnable;

        //#if LogInfo
        //                    watch.Stop();
        //                    opt3 = watch.Elapsed.TotalMilliseconds;
        //                   // watch.Start();
        //#endif

        //                }
        //            }
        //#if LogInfo
        //            Log4Log.LogEX(KLogLevel.SevTimeInfo,  string.Format("查询C_Auth_Users,E_Login_Local 使用时间:{0},COUNT:{1},{2},进入if 开始时间 {3},if 结束时间 {4} \r\n", opt.ToString(),count1.ToString(),
        //                count.ToString(), opt2.ToString(), opt3.ToString()));
        //#endif
        //            return LoginUsers;
        //            //return Session.CreateCriteria<LoginLocal>()
        //            //    .Add(Restrictions.Eq("LoginName", loginName))
        //            //    .Add(Restrictions.Eq("Password", password))
        //            //    .UniqueResult<LoginLocal>();
        //        }

        #endregion

        public LoginLocal UserLogin(string loginName, string password)
        {

            var LoginUser = DB.CreateQuery<E_Login_Local>();
            String pattern = @"^(0|86|17951)?(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57])[0-9]{8}$";//"^((1[3,5,8][0-9])|(14[5,7])|(17[0,6,7,8])|(19[7]))\\d{8}$";
            LoginLocal LoginUsers = null;
            if (Regex.IsMatch(loginName, pattern))
            {
                LoginUsers = (from p in LoginUser
                              where (p.mobile == loginName && p.Password == password)
                              select new LoginLocal()
                              {
                                  CreateTime = p.CreateTime,
                                  LoginName = p.LoginName,
                                  mobile = p.mobile,
                                  Password = p.Password,
                                  UserId = p.UserId,

                              }).FirstOrDefault();
                //LoginUsers = p != null ? new LoginLocal()
                //{
                //    CreateTime = p.CreateTime,
                //    LoginName = p.LoginName,
                //    mobile = p.mobile,
                //    Password = p.Password,
                //    UserId = p.UserId,

                //} : null;
            }
            else
            {
                LoginUsers = (from p in LoginUser
                              where p.LoginName == loginName && p.Password == password
                              select new LoginLocal()
                              {
                                  CreateTime = p.CreateTime,
                                  LoginName = p.LoginName,
                                  mobile = p.mobile,
                                  Password = p.Password,
                                  UserId = p.UserId,

                              }).FirstOrDefault();
                //LoginUsers = p != null ? new LoginLocal()
                //{
                //    CreateTime = p.CreateTime,
                //    LoginName = p.LoginName,
                //    mobile = p.mobile,
                //    Password = p.Password,
                //    UserId = p.UserId,

                //} : null;
            }
            if (LoginUsers != null)
            {
                string userId = LoginUsers.UserId;
                LoginUsers.User = (from d in DB.CreateQuery<C_Auth_Users>()
                                   where d.UserId == userId
                                   select new SystemUser()
                                   {
                                       CreateTime = d.CreateTime,
                                       AgentId = d.AgentId,
                                       RegFrom = d.RegFrom,
                                       UserId = d.UserId,
                                   }).FirstOrDefault();

                //LoginUsers.User = d != null ? new SystemUser()
                //                   {
                //                       CreateTime = d.CreateTime,
                //                       AgentId = d.AgentId,
                //                       RegFrom = d.RegFrom,
                //                       UserId = d.UserId,
                //                   }:null;               
                LoginUsers.Register = (from p in DB.CreateQuery<C_User_Register>()
                                       where p.UserId == userId
                                       select new UserRegister()
                                       {

                                           AgentId = p.AgentId,
                                           ComeFrom = p.ComeFrom,
                                           CreateTime = p.CreateTime,
                                           DisplayName = p.DisplayName,
                                           HideDisplayNameCount = p.HideDisplayNameCount,
                                           IsAgent = p.IsAgent,
                                           IsEnable = p.IsEnable,
                                           IsFillMoney = p.IsFillMoney,
                                           IsIgnoreReport = p.IsIgnoreReport,
                                           ParentPath = p.ParentPath,
                                           Referrer = p.Referrer,
                                           ReferrerUrl = p.ReferrerUrl,
                                           RegisterIp = p.RegisterIp,
                                           RegType = p.RegType,
                                           UserId = p.UserId,
                                           UserType = p.UserType,
                                           VipLevel = p.VipLevel
                                       }).FirstOrDefault();
            }
            return LoginUsers;
        }

        public LoginLocal AdminUserLogin(string loginName, string password)
        {
            var LoginUser = DB.CreateQuery<E_Login_Local>();
            LoginLocal LoginUsers = (from p in LoginUser
                          where p.LoginName == loginName && p.Password == password
                          select new LoginLocal()
                          {
                              CreateTime = p.CreateTime,
                              LoginName = p.LoginName,
                              mobile = p.mobile,
                              Password = p.Password,
                              UserId = p.UserId,

                          }).FirstOrDefault();
            if (LoginUsers != null)
            {
                string userId = LoginUsers.UserId;
                LoginUsers.User = (from d in DB.CreateQuery<C_Auth_Users>()
                                   where d.UserId == userId
                                   select new SystemUser()
                                   {
                                       CreateTime = d.CreateTime,
                                       AgentId = d.AgentId,
                                       RegFrom = d.RegFrom,
                                       UserId = d.UserId,
                                   }).FirstOrDefault();
                if (LoginUsers.User != null)
                {
                    LoginUsers.User.RoleList = (from d in DB.CreateQuery<C_Auth_Roles>()
                                                join c in DB.CreateQuery<C_Auth_UserRole>()
                                                on d.RoleId equals c.RoleId
                                                where c.UserId == userId
                                                select new SystemRole()
                                                {
                                                    IsAdmin = d.IsAdmin,
                                                    IsInner = d.IsInner,
                                                    RoleId = d.RoleId,
                                                    RoleName = d.RoleName,
                                                    RoleType = (RoleType)d.RoleType
                                                }).ToList();
                }
                LoginUsers.Register = (from p in DB.CreateQuery<C_User_Register>()
                                       where p.UserId == userId
                                       select new UserRegister()
                                       {

                                           AgentId = p.AgentId,
                                           ComeFrom = p.ComeFrom,
                                           CreateTime = p.CreateTime,
                                           DisplayName = p.DisplayName,
                                           HideDisplayNameCount = p.HideDisplayNameCount,
                                           IsAgent = p.IsAgent,
                                           IsEnable = p.IsEnable,
                                           IsFillMoney = p.IsFillMoney,
                                           IsIgnoreReport = p.IsIgnoreReport,
                                           ParentPath = p.ParentPath,
                                           Referrer = p.Referrer,
                                           ReferrerUrl = p.ReferrerUrl,
                                           RegisterIp = p.RegisterIp,
                                           RegType = p.RegType,
                                           UserId = p.UserId,
                                           UserType = p.UserType,
                                           VipLevel = p.VipLevel
                                       }).FirstOrDefault();
            }
            return LoginUsers;
        }

        public void GetSystemUser(SystemUser user)
        {
            var uQueryRoles = (from p in DB.CreateQuery<C_Auth_Roles>()
                               select new SystemRole()
                               {
                                   RoleId = p.RoleId,
                                   RoleName = p.RoleName,
                                   IsInner = p.IsInner,
                                   IsAdmin = p.IsAdmin,
                                   RoleType = (RoleType)p.RoleType,
                               }).ToList();

            var uQueryUserRole = DB.CreateQuery<C_Auth_UserRole>();
            var C_Auth_RoleFunction_query = DB.CreateQuery<C_Auth_RoleFunction>();
            var C_Auth_Function_List = DB.CreateQuery<C_Auth_Function_List>();
            var C_Auth_UserFunction_query = DB.CreateQuery<C_Auth_UserFunction>();


            user.RoleList = (from b in uQueryRoles
                             join c in uQueryUserRole
                             on b.RoleId equals c.RoleId
                             where c.UserId == user.UserId
                             select b).ToList();
            var RoleFunctionList = (from b in C_Auth_RoleFunction_query
                                    join d in uQueryUserRole
                                    on b.RoleId equals d.RoleId
                                    where d.UserId == user.UserId
                                    select new RoleFunction()
                                    {
                                        FunctionId = b.FunctionId,
                                        IId = b.IId,
                                        Mode = b.Mode,

                                    }).ToList();
            var UserFunctionList = (from b in C_Auth_UserFunction_query
                                    join d in uQueryUserRole

                                    on b.UserId equals d.UserId
                                    where d.UserId == user.UserId
                                    select new UserFunction()
                                    {
                                        FunctionId = b.FunctionId,
                                        IId = b.IId,
                                        Mode = b.Mode,


                                    }).ToList();
            user.FunctionList = UserFunctionList;
            //if (UserFunctionList != null && UserFunctionList.Count() != 0)
            //{

            //    var Ids = RoleFunctionList.Select(p => p.FunctionId).ToList();
            //    var Auth_Function_Lists = C_Auth_Function_List.Where(p => Ids.Contains(p.FunctionId)).Select(p => new Function()
            //    {
            //        DisplayName = p.DisplayName,
            //        FunctionId = p.FunctionId,
            //        IsBackBasic = p.IsBackBasic,
            //        IsWebBasic = p.IsWebBasic,
            //        ParentId = p.ParentId,
            //        ParentPath = p.ParentPath,
            //    }).ToList();
            //    user.FunctionList = UserFunctionList;
            //}
        }

        public void GetSystemRole(SystemRole role, string userid)
        {
            var uQueryRoles = (from p in DB.CreateQuery<C_Auth_Roles>()
                               select new SystemRole()
                               {
                                   RoleId = p.RoleId,
                                   RoleName = p.RoleName,
                                   IsInner = p.IsInner,
                                   IsAdmin = p.IsAdmin,
                                   RoleType = (RoleType)p.RoleType,
                               }).ToList();
            var uQueryUserRole = DB.CreateQuery<C_Auth_UserRole>();
            var C_Auth_RoleFunction_query = DB.CreateQuery<C_Auth_RoleFunction>();
            var C_Auth_Function_List = DB.CreateQuery<C_Auth_Function_List>();
            var C_Auth_UserFunction_query = DB.CreateQuery<C_Auth_UserFunction>();
            var RoleFunctionList = (from b in C_Auth_RoleFunction_query
                                    join d in uQueryUserRole
                                    on b.RoleId equals d.RoleId
                                    where d.UserId == userid
                                    select b
                                     ).Select(p => new RoleFunction()
                                     {
                                         FunctionId = p.FunctionId,
                                         IId = p.IId,
                                         Mode = p.Mode,

                                     }).ToList();
            role.FunctionList = RoleFunctionList;
            //if (RoleFunctionList != null && RoleFunctionList.Count() != 0)
            //{
            //    var Ids = RoleFunctionList.Select(p => p.FunctionId).ToList();
            //    var Auth_Function_Lists = (from p in C_Auth_Function_List
            //                               where Ids.Contains(p.FunctionId)
            //                               select new Function()
            //                               {
            //                                   DisplayName = p.DisplayName,
            //                                   FunctionId = p.FunctionId,
            //                                   IsBackBasic = p.IsBackBasic,
            //                                   IsWebBasic = p.IsWebBasic,
            //                                   ParentId = p.ParentId,
            //                                   ParentPath = p.ParentPath,
            //                               }).ToList();
            //    role.FunctionList = RoleFunctionList;
            //}
        }


        public E_Blog_ProfileBonusLevel QueryBlog_ProfileBonusLevel(string userId)
        {

            return DB.CreateQuery<E_Blog_ProfileBonusLevel>().Where(p => p.UserId == userId).FirstOrDefault();
        }



        /// <summary>
        /// 查询QQ用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public E_Login_QQ GetQQByUserId(string userId)
        {
            return DB.CreateQuery<E_Login_QQ>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 查询阿里用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public E_Login_Alipay GetAlipayByUserId(string userId)
        {
            return DB.CreateQuery<E_Login_Alipay>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 查询注册用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_User_Register GetRegisterById(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();
        }


        /// <summary>
        /// 查询本地用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public E_Login_Local GetLocalLoginByUserId(string userId)
        {
            return DB.CreateQuery<E_Login_Local>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 根据用户名电话号码查询用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public E_Login_Local GetLoginByName(string loginName)
        {
            return DB.CreateQuery<E_Login_Local>().Where(p => (p.LoginName == loginName || p.mobile == loginName)).FirstOrDefault();
        }



        public UserBalanceInfo QueryUserBalance(string userId)
        {

            var balance = QueryUserBalanceInfo(userId);
            if (balance == null)
            {
                throw new LogicException("用户账户不存在");
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

        /// <summary>
        /// 查询用户余额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_User_Balance QueryUserBalanceInfo(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();

        }

        /// <summary>
        /// 查询银行卡信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_BankCard BankCardById(string userId)
        {
            var entity = new BankCardManager().BankCardById(userId);
            //if (entity == null)
            //    throw new LogicException(string.Format("查不到{0}的银行卡信息", userId));
            entity = new C_BankCard();
            return new C_BankCard()
            {
                UserId = entity.UserId,
                BankCardNumber = entity.BankCardNumber,
                BankCode = entity.BankCode,
                BankName = entity.BankName,
                BankSubName = entity.BankSubName,
                BId = entity.BId,
                CityName = entity.CityName,
                CreateTime = entity.CreateTime,
                ProvinceName = entity.ProvinceName,
                RealName = entity.RealName,
                UpdateTime = entity.UpdateTime
            };
        }

        public int GetUnreadMailCountByUser(string userId)
        {

            var query = DB.CreateQuery<E_SiteMessage_InnerMail_List_new>().Where(s => s.HandleType == 0 && (s.ReceiverId == userId || s.ReceiverId == "U:" + userId));
            if (query != null) return query.Count();
            return 0;
        }



        public void Register(LoginLocal loginEntity, string userId)
        {
            if (loginEntity.Password == null)
            {
                loginEntity.Password = C_DefaultPassword;
            }
            loginEntity.Password = Encipherment.MD5(string.Format("{0}{1}", loginEntity.Password, _gbKey)).ToUpper();
            var tmp = DB.CreateQuery<E_Login_Local>().Where(p => (p.LoginName == loginEntity.LoginName || p.mobile == loginEntity.LoginName)).FirstOrDefault();
            if (tmp != null)
            {
                throw new LogicException("登录名已经存在 - " + loginEntity.LoginName);
            }
            //loginEntity.User = loginManager.LoadUser(userId);
            var Register = GetRegisterById(userId);
            var register = new E_Login_Local
            {
                LoginName = loginEntity.LoginName,
                UserId = userId,
                CreateTime = DateTime.Now,
                mobile = loginEntity.mobile,
                Password = loginEntity.Password,
                RegisterId = Register.UserId
            };
            DB.GetDal<E_Login_Local>().Add(register);
        }

        
        public E_Login_Local GetUserByLoginName(string loginName)
        {

            var user = GetLoginByName(loginName);
            if (user == null)
            {
                throw new LogicException("用户不存在或不是本地注册用户。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
            }
            return user;

        }

        public bool? CheckIsSame2BalancePassword(string userId, string newPassword)
        {

            var balance = SelectUserBalance(userId);
            if (balance == null)
            {
                return null;
            }
            return balance.Password.Equals(Encipherment.MD5(newPassword));
        }

        public C_User_Balance SelectUserBalance(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();
        }



        public bool? CheckIsSame2LoginPassword(string userId, string newPassword)
        {

            var user = GetLocalLoginByUserId(userId);
            if (user == null)
            {
                return null;
            }
            return user.Password.ToUpper().Equals(Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper());

        }
        public void ChangePassword(string userId, string oldPassword, string newPassword)
        {
            oldPassword = Encipherment.MD5(string.Format("{0}{1}", oldPassword, _gbKey)).ToUpper();
            newPassword = Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper();           
            var user = DB.CreateQuery<E_Login_Local>().Where(p => p.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                throw new LogicException("用户不是本地注册用户，不允许修改密码。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
            }
            if (user.Password.ToUpper() != oldPassword)
            {
                throw new LogicException("旧密码输入错误。");
            }
            user.Password = newPassword;
            DB.GetDal<E_Login_Local>().Update(user);

        }

        public string ChangePassword(string userId)
        {
            var r = new Random(DateTime.Now.Millisecond);
            var password = r.Next(100000, 999999).ToString();
            var encodePassword = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
            var password_balance = r.Next(100000, 999999).ToString();
            var encodePassword_balance = Encipherment.MD5(string.Format("{0}{1}", password_balance, _gbKey)).ToUpper();
            try
            {
                DB.Begin();

                var user = GetLocalLoginByUserId(userId);
                if (user == null)
                {
                    throw new LogicException("用户不存在或不是本地注册用户，不能修改密码。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
                }
                user.Password = encodePassword;

                DB.GetDal<E_Login_Local>().Update(user);

                var balanceManage = new UserBalanceManager();
                var b = balanceManage.QueryUserBalance(userId);
                b.Password = encodePassword_balance;
                balanceManage.UpdateUserBalance(b);
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }


            return string.Format("{0}|{1}", password, password_balance);
        }
        private const string C_DefaultPassword = "123456";
       
        /// <summary>
        /// 指定代理注册的用户  路径
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public C_User_Register QueryUserRegisterByUserId(string userid)
        {

            var user = GetRegisterById(userid);
            if (user == null)
                new C_User_Register();
            return user;
        }

       

        /// <summary>
        /// 查询用户绑定信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public UserBindInfos QueryUserBindInfos(string userId)
        {
            //var user = GetRegisterById(userId);
            //if (user == null || user.IsEnable == false)
            //    return new UserBindInfos();
            //var sql = string.Format(@"select r.UserId,r.VipLevel,r.DisplayName,r.ComeFrom,r.IsFillMoney,r.IsEnable,r.IsAgent,r.HideDisplayNameCount,
            //m.Mobile,n.RealName,n.IdCardNumber,n.CardType,m.IsSettedMobile,
            //b.RealName BankCardRealName,b.ProvinceName,b.CityName,b.BankName,b.BankSubName,b.BankCardNumber
            //from C_User_Register r left join E_Authentication_Mobile m on r.userid=m.userid
            //left join C_BankCard b on r.userid=b.userid
            //left join E_Authentication_RealName n on r.userid=n.userid where r.userid='{0}'", userId);
            //var array = DB.CreateSQLQuery(sql).List<UserBindInfos>().FirstOrDefault();
            var sql = $@"select r.UserId,r.VipLevel,r.DisplayName,r.ComeFrom,r.IsFillMoney,r.IsEnable,r.IsAgent,r.HideDisplayNameCount,
            m.Mobile,n.RealName,n.IdCardNumber,n.CardType,m.IsSettedMobile,
            b.RealName BankCardRealName, b.ProvinceName,b.CityName,b.BankName,b.BankSubName,b.BankCardNumber,
            balance.CommissionBalance,balance.ExpertsBalance,balance.BonusBalance,
            balance.FreezeBalance,balance.FillMoneyBalance,balance.RedBagBalance,balance.UserGrowth,
            balance.IsSetPwd,balance.NeedPwdPlace
            from C_User_Register r
            left join E_Authentication_Mobile m on r.userid = m.userid
            left join C_BankCard b on r.userid = b.userid
            LEFT JOIN C_User_Balance balance ON balance.UserId = r.UserId
            left join E_Authentication_RealName n on r.userid = n.userid where r.userid ='{userId}'";
            var array = DB.CreateSQLQuery(sql).First<UserBindInfos>();
            if (array != null)
            {
                array.LoadDateTime = DateTime.Now;
            }
            //没有去解决一个用户 由于不明原因绑定了多个卡 此处根据前台后台当前逻辑 都是默认读取第一条数据 所以读取完了第一条直接break 
            // 这样redis中就会保存用户和后台显示一样的卡号
            return array;
        }

        public string GetLoginNameIsExsite(string loginName)
        {
                var user = DB.CreateQuery<E_Login_Local>().Where(p => (p.LoginName == loginName || p.mobile == loginName)).FirstOrDefault();
                if (user == null)
                {
                    return "";
                }
                else
                {
                    return user.LoginName;
                }
            
        }

        public List<string> QueryFunctionByRole(string[] arrayRole)
        {
            if (arrayRole == null || arrayRole.Length == 0)
            {
                return new List<string>();
            }
            else
            {
                string strSql = "select FunctionId from C_Auth_RoleFunction where RoleId in ({0})";
                var sb = new StringBuilder();
                foreach (var item in arrayRole)
                {
                    sb.Append("'");
                    sb.Append(item);
                    sb.Append("',");
                }
                var result = DB.CreateSQLQuery(string.Format(strSql,sb.ToString().Trim(',')))
                   .List<C_Auth_RoleFunction>();
                List<string> _fun = new List<string>();
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        _fun.Add(item.ToString());
                    }
                }
                return _fun;
            }
            
        }

        public bool User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            var date = DateTime.Today.ToString("yyyyMMdd");
            //注册当前登录不送红包
            var user = new UserBalanceManager().QueryUserRegister(userId);
            if (user.CreateTime.ToString("yyyyMMdd") == date)
                return false;


            var bizRealName = new RealNameAuthenticationBusiness();
            var realName = bizRealName.GetAuthenticatedRealName(userId);
            if (realName == null)
                return false;
            var bizMoible = new MobileAuthenticationBusiness();
            var mobile = bizMoible.GetAuthenticatedMobile(userId);
            if (mobile == null || !mobile.IsSettedMobile)
                return false;

            var manager = new A20150919Manager();
            var record = manager.QueryA20150919_已绑定身份和手机的用户登录送红包(userId, date);
            if (record != null) return false;

            //var old = manager.QueryByUserId(userId);
            //if (old == null) return;
            //if (!old.IsBindRealName) return;
            //if (!old.IsBindMobile) return;

            decimal giveMoney = decimal.Parse(ActivityCache.QueryActivityConfig("ActivityConfig.BindedUserLoginGiveRedBag"));
            if (user.VipLevel >= 0)
            {
                var config = ActivityCache.QueryActivityConfig(string.Format("ActivityConfig.BindedUserLoginGiveRedBagV{0}", user.VipLevel));
                if (config != null)
                {
                    try
                    {
                        giveMoney = decimal.Parse(config);
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
                return true;
            }
            return false;
        }
    }
}
