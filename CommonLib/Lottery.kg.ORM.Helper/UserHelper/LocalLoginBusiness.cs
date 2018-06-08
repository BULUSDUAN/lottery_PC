using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using KaSon.FrameWork.Services.ORM;
using EntityModel.ORM;
using KaSon.FrameWork.Helper;
using KaSon.FrameWork.Services.Enum;
using EntityModel.CoreModel;
using System.Linq.Expressions;
using log4net.Plugin;
using EntityModel.Communication;
using EntityModel.CoreModel.AuthEntities;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class LocalLoginBusiness : DBbase
    {
        //改版密码增加的关键字
        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public LoginLocal Login(string loginName, string password)
        {
            password = MD5Helper.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();

            LoginLocal loginInfo = null;

            loginInfo = UserLogin(loginName, password);


            return loginInfo;
        }
        public LoginLocal LoginAPP(string loginName, string password)
        {
            //password = Encipherment.MD5(password);

            LoginLocal loginInfo = null;

            loginInfo = UserLogin(loginName, password);
            if (loginInfo != null && loginInfo.User != null)
            {
                //NHibernate.NHibernateUtil.Initialize(loginInfo.User.RoleList);
                //NHibernate.NHibernateUtil.Initialize(loginInfo.Register);
            }

            return loginInfo;
        }

        public LoginLocal UserLogin(string loginName, string password)
        {

            var LoginUser = DB.CreateQuery<E_Login_Local>();
            var LoginUsers = LoginUser.Where(p => (p.LoginName == loginName || p.mobile == loginName) && p.Password == password).Select(p => new LoginLocal()
            {
                CreateTime = p.CreateTime,
                LoginName = p.LoginName,
                mobile = p.mobile,
                Password = p.Password,
                UserId = p.UserId,
                Register = null,
                 User=null
            }).FirstOrDefault();

            if (LoginUser != null)
            {
                LoginUsers.User = DB.CreateQuery<C_Auth_Users>().Where(p => p.UserId == LoginUsers.UserId).Select(p=>new SystemUser()
                {
                     CreateTime=p.CreateTime,
                     AgentId=p.AgentId,
                     RegFrom=p.RegFrom,
                     UserId=p.UserId
                }).FirstOrDefault();

                if (LoginUsers.User != null)
                {
                    var uQueryRoles = DB.CreateQuery<C_Auth_Roles>().Select(p=>new SystemRole(){
                         RoleId=p.RoleId,
                        RoleName=p.RoleName,
                        IsInner=p.IsInner,
                        IsAdmin=p.IsAdmin,
                       
                    });
                    var uQueryUserRole = DB.CreateQuery<C_Auth_UserRole>();
                    LoginUsers.User.RoleList = (from b in uQueryRoles
                                                join c in uQueryUserRole
                                                on b.RoleId equals c.RoleId
                                                where c.UserId == LoginUsers.UserId
                                                select b).ToList();

                    var register = DB.CreateQuery<C_User_Register>().Where(p => p.UserId == LoginUsers.UserId).Select(p=>new UserRegister() {

                        AgentId=p.AgentId, ComeFrom=p.ComeFrom, CreateTime=p.CreateTime, DisplayName=p.DisplayName, HideDisplayNameCount=p.HideDisplayNameCount,
                        IsAgent=p.IsAgent, IsEnable=p.IsEnable,  IsFillMoney=p.IsFillMoney, IsIgnoreReport=p.IsIgnoreReport, ParentPath=p.ParentPath, Referrer=p.Referrer,
                        ReferrerUrl=p.ReferrerUrl, RegisterIp=p.RegisterIp, RegType=p.RegType, UserId=p.UserId, UserType=p.UserType, VipLevel=p.VipLevel
                    }).FirstOrDefault();

                    LoginUsers.Register.IsEnable = register.IsEnable;

                }
            }

            return LoginUsers;
            //return Session.CreateCriteria<LoginLocal>()
            //    .Add(Restrictions.Eq("LoginName", loginName))
            //    .Add(Restrictions.Eq("Password", password))
            //    .UniqueResult<LoginLocal>();
        }
        public E_Blog_ProfileBonusLevel QueryBlog_ProfileBonusLevel(string userId)
        {

            return DB.CreateQuery<E_Blog_ProfileBonusLevel>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public E_Login_Local GetLocalLoginByUserId(string userId)
        {
            return DB.CreateQuery<E_Login_Local>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// 手机黑名单
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C_Core_Config BanRegistrMobile(string key)
        {
            return DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
        }

        /// <summary>
        /// 查询手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public UserMobile GetMobileInfoByMobile(string mobile)
        {
            var query = DB.CreateQuery<E_Authentication_Mobile>().Where(s => s.Mobile == mobile).Select(p=>new UserMobile {
                 AuthFrom=p.AuthFrom,
                  CreateBy=p.CreateBy,
                   CreateTime=p.CreateTime,
                     IsSettedMobile=p.IsSettedMobile,
                      Mobile=p.Mobile,
                        //UpdateBy=p.
            });

            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedMobile == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedMobile == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }

        //public void Register(LoginLocal loginEntity, string userId)
        //{
        //    if (loginEntity.Password == null)
        //    {
        //        loginEntity.Password = C_DefaultPassword;
        //    }
        //    loginEntity.Password = Encipherment.MD5(string.Format("{0}{1}", loginEntity.Password, _gbKey)).ToUpper();

        //    using (var biz = new GameBiz.Business.GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var loginManager = new LoginLocalManager())
        //        {
        //            var tmp = loginManager.GetLoginByName(loginEntity.LoginName);
        //            if (tmp != null)
        //            {
        //                throw new AuthException("登录名已经存在 - " + loginEntity.LoginName);
        //            }
        //            loginEntity.User = loginManager.LoadUser(userId);
        //            loginEntity.Register = loginManager.LoadRegister(userId);
        //            loginManager.Register(loginEntity);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public int GetTodayRegisterCount(DateTime date, string ip)
        //{
        //    using (var loginManager = new LoginLocalManager())
        //    {
        //        return loginManager.GetTodayRegisterCount(date, ip);
        //    }
        //}
        //public LoginLocal GetUserByLoginName(string loginName)
        //{
        //    using (var loginManager = new LoginLocalManager())
        //    {
        //        var user = loginManager.GetLoginByName(loginName);
        //        if (user == null)
        //        {
        //            throw new AuthException("用户不存在或不是本地注册用户。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
        //        }
        //        return user;
        //    }
        //}
        //public string GetLoginNameIsExsite(string loginName)
        //{

        //    using (var loginManager = new LoginLocalManager())
        //    {
        //        var user = loginManager.GetLoginByName(loginName);
        //        if (user == null)
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return user.LoginName;
        //        }
        //    }
        //}

        public bool? CheckIsSame2BalancePassword(string userId, string newPassword)
        {

            var balance = QueryUserBalance(userId);
            if (balance == null)
            {
                return null;
            }
            return balance.Password.Equals(Encipherment.MD5(newPassword));
        }

        public C_User_Balance QueryUserBalance(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().FirstOrDefault(p => p.UserId == userId);
        }



        //public bool? CheckIsSame2LoginPassword(string userId, string newPassword)
        //{
        //    using (var loginManager = new LoginLocalManager())
        //    {
        //        var user = loginManager.GetLoginByUserId(userId);
        //        if (user == null)
        //        {
        //            return null;
        //        }
        //        return user.Password.ToUpper().Equals(Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper());
        //    }
        //}
        public Task<string> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            oldPassword = Encipherment.MD5(string.Format("{0}{1}", oldPassword, _gbKey)).ToUpper();
            newPassword = Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper();

            DB.Begin();

            var user = DB.CreateQuery<E_Login_Local>().Where(p => p.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                return Task.FromResult("用户不是本地注册用户，不允许修改密码。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
            }
            if (user.Password.ToUpper() != oldPassword)
            {
                return Task.FromResult("旧密码输入错误。");
            }
            user.Password = newPassword;
            DB.GetDal<E_Login_Local>().Update(user);

            DB.Commit();
            return Task.FromResult("修改密码成功");
        }

        //public string ChangePassword(string userId)
        //{
        //    var r = new Random(DateTime.Now.Millisecond);
        //    var password = r.Next(100000, 999999).ToString();
        //    var encodePassword = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
        //    var password_balance = r.Next(100000, 999999).ToString();
        //    var encodePassword_balance = Encipherment.MD5(string.Format("{0}{1}", password_balance, _gbKey)).ToUpper();
        //    using (var biz = new GameBiz.Business.GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var loginManager = new LoginLocalManager();
        //        var user = loginManager.GetLoginByUserId(userId);
        //        if (user == null)
        //        {
        //            throw new AuthException("用户不存在或不是本地注册用户，不能修改密码。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
        //        }
        //        user.Password = encodePassword;
        //        loginManager.UpdateLogin(user);

        //        var balanceManage = new UserBalanceManager();
        //        var b = balanceManage.QueryUserBalance(userId);
        //        b.Password = encodePassword_balance;
        //        balanceManage.UpdateUserBalance(b);

        //        biz.CommitTran();
        //    }
        //    return string.Format("{0}|{1}", password, password_balance);
        //}
        //private const string C_DefaultPassword = "123456";
        //public void ResetUserPassword(string userId)
        //{
        //    var newPassword = Encipherment.MD5(string.Format("{0}{1}", C_DefaultPassword, _gbKey)).ToUpper();
        //    using (var biz = new GameBiz.Business.GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var loginManager = new LoginLocalManager())
        //        {
        //            var user = loginManager.GetLoginByUserId(userId);
        //            if (user == null)
        //            {
        //                throw new AuthException("用户不是本地注册用户，不允许修改密码。请确定是否是通过支付宝或QQ帐号进行登录，如有疑问，请联系客服。");
        //            }
        //            user.Password = newPassword;
        //            loginManager.UpdateLogin(user);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public UserQueryInfo QueryUserByKey(string userId, string agentId)
        //{
        //    var userManager = new UserBalanceManager();
        //    var reg = userManager.QueryUserRegister(userId);
        //    if (reg == null)
        //        throw new Exception("用户不存在");
        //    if (!string.IsNullOrEmpty(agentId) && reg.AgentId != agentId)
        //        throw new Exception(string.Format("用户{0}不属于您发展的用户", userId));

        //    var balance = userManager.QueryUserBalance(userId);
        //    if (balance == null)
        //        throw new Exception("用户账户不存在");

        //    var realNameManager = new UserRealNameManager();
        //    var real = realNameManager.GetUserRealName(userId);

        //    //var apliy = new AlipayLoginInfo();
        //    //var apliyCount = apliy.ApliyCount;
        //    var mobileManagr = new UserMobileManager();
        //    var mobile = mobileManagr.GetUserMobile(userId);

        //    var fundManger = new FundManager();
        //    var agentFreezeBalance = fundManger.QueryAgentFreezeBalanceByUserId(userId);
        //    var caibbFreezeBalance = 0M;
        //    if (balance.FreezeBalance > 0)
        //        caibbFreezeBalance = balance.FreezeBalance - agentFreezeBalance;//网站冻结资金=总冻结资金-代理冻结资金;

        //    var userAlipayManager = new UserAlipayManager();
        //    var userAlipy = userAlipayManager.GetUserAlipay(userId);


        //    var userQQManager = new UserQQManager();
        //    var userQQ = userQQManager.GetUserQQ(userId);

        //    var ocAgentManager = new OCAgentManager();
        //    var ocAgent = ocAgentManager.QueryOCAgent(userId);

        //    return new UserQueryInfo
        //    {
        //        DisplayName = reg.DisplayName,
        //        UserId = reg.UserId,
        //        RealName = real == null ? string.Empty : real.RealName,
        //        IdCardNumber = real == null ? string.Empty : real.IdCardNumber,
        //        Mobile = mobile == null ? string.Empty : mobile.Mobile,
        //        FillMoneyBalance = balance.FillMoneyBalance,
        //        BonusBalance = balance.BonusBalance,
        //        CommissionBalance = balance.CommissionBalance,
        //        ExpertsBalance = balance.ExpertsBalance,
        //        RedBagBalance = balance.RedBagBalance,
        //        FreezeBalance = caibbFreezeBalance,
        //        IsEnable = reg.IsEnable,
        //        AgentId = reg.AgentId,
        //        IsAgent = reg.IsAgent,
        //        CardType = "",
        //        ComeFrom = reg.ComeFrom,
        //        Email = "",
        //        IsFillMoney = reg.IsFillMoney,
        //        IsSettedEmail = true,
        //        IsSettedMobile = mobile == null ? false : mobile.IsSettedMobile,
        //        IsSettedRealName = real == null ? false : real.IsSettedRealName,
        //        RegisterIp = reg.RegisterIp,
        //        RegTime = reg.CreateTime,
        //        VipLevel = reg.VipLevel,
        //        CurrentDouDou = balance.CurrentDouDou,
        //        ApliyCount = userAlipy == null ? string.Empty : userAlipy.AlipayAccount,
        //        QQNumber = userQQ == null ? string.Empty : userQQ.QQ,
        //        CPSMode = ocAgent == null ? 0 : ocAgent.CPSMode,
        //        UserGrowth = balance.UserGrowth,
        //        CPSBalance = balance.CPSBalance
        //    };
        //}

        //public UserQueryInfo QueryUserByUserName(string userName, string agentId)
        //{
        //    var userManager = new UserBalanceManager();
        //    var reg = userManager.QueryUserRegisterByUserName(userName);
        //    if (reg == null)
        //        throw new Exception("用户不存在");
        //    if (string.IsNullOrEmpty(reg.AgentId) || reg.AgentId != agentId)
        //        throw new Exception(string.Format("用户{0}不属于您发展的用户", userName));

        //    var balance = userManager.QueryUserBalance(reg.UserId);
        //    if (balance == null)
        //        throw new Exception("用户账户不存在");

        //    var realNameManager = new UserRealNameManager();
        //    var real = realNameManager.GetUserRealName(reg.UserId);

        //    var mobileManagr = new UserMobileManager();
        //    var mobile = mobileManagr.GetUserMobile(reg.UserId);

        //    var fundManger = new FundManager();
        //    var agentFreezeBalance = fundManger.QueryAgentFreezeBalanceByUserId(reg.UserId);
        //    var caibbFreezeBalance = 0M;
        //    if (balance.FreezeBalance > 0)
        //        caibbFreezeBalance = balance.FreezeBalance - agentFreezeBalance;//网站冻结资金=总冻结资金-代理冻结资金;

        //    return new UserQueryInfo
        //    {
        //        DisplayName = reg.DisplayName,
        //        UserId = reg.UserId,
        //        RealName = real == null ? string.Empty : real.RealName,
        //        IdCardNumber = real == null ? string.Empty : real.IdCardNumber,
        //        Mobile = mobile == null ? string.Empty : mobile.Mobile,
        //        FillMoneyBalance = balance.FillMoneyBalance,
        //        BonusBalance = balance.BonusBalance,
        //        CommissionBalance = balance.CommissionBalance,
        //        ExpertsBalance = balance.ExpertsBalance,
        //        RedBagBalance = balance.RedBagBalance,
        //        FreezeBalance = caibbFreezeBalance,
        //        IsEnable = reg.IsEnable,
        //        AgentId = reg.AgentId,
        //        IsAgent = reg.IsAgent,
        //        CardType = "",
        //        ComeFrom = reg.ComeFrom,
        //        Email = "",
        //        IsFillMoney = reg.IsFillMoney,
        //        IsSettedEmail = true,
        //        IsSettedMobile = mobile == null ? false : mobile.IsSettedMobile,
        //        IsSettedRealName = real == null ? false : real.IsSettedRealName,
        //        RegisterIp = reg.RegisterIp,
        //        RegTime = reg.CreateTime,
        //        VipLevel = reg.VipLevel,
        //        CurrentDouDou = balance.CurrentDouDou,
        //    };
        //}
        //public ManagerQueryInfo QueryBackgroundManagerByKey(string key)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        var reg = manager.GetRegisterByKey(key);
        //        if (reg == null)
        //        {
        //            return null;
        //        }
        //        var roleIdList = reg.User.RoleList.Select((r) => r.RoleId).ToList();
        //        var info = new ManagerQueryInfo
        //        {
        //            UserId = reg.UserId,
        //            DisplayName = reg.DisplayName,
        //            RoleIdList = string.Join(", ", roleIdList.ToArray()),
        //        };
        //        var user = manager.GetLoginByUserId(reg.UserId);
        //        info.LoginName = user.LoginName;
        //        return info;
        //    }
        //}
        //public ManagerQueryInfoCollection QueryBackgroundManagerList(string key, int pageIndex, int pageSize)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        int totalCount;
        //        var list = manager.QueryBackgroundManagerList(key, pageIndex, pageSize, out totalCount);
        //        var collection = new ManagerQueryInfoCollection
        //        {
        //            TotalCount = totalCount,
        //        };
        //        collection.LoadList(list);
        //        return collection;
        //    }
        //}
        //public UserQueryInfoCollection QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? IsUserType, bool? isAgent
        //    , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize, string strOrderBy)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        int totalCount;
        //        decimal totalFillMoneyBalance;
        //        decimal totalBonusBalance;
        //        decimal totalCommissionBalance;
        //        decimal totalExpertsBalance;
        //        decimal totalFreezeBalance;
        //        decimal totalRedBagBalance;
        //        decimal totalCPSBalance;
        //        int totalDouDou;
        //        var list = manager.QueryUserList(regFrom, regTo, keyType, keyValue, isEnable, isFillMoney, IsUserType, isAgent, commonBlance, bonusBlance, freezeBlance, vipRange, comeFrom, agentId, pageIndex, pageSize,
        //            out totalCount, out totalFillMoneyBalance, out totalBonusBalance, out totalCommissionBalance, out totalExpertsBalance, out totalFreezeBalance, out totalRedBagBalance, out totalDouDou, out totalCPSBalance, strOrderBy);
        //        var collection = new UserQueryInfoCollection
        //        {
        //            TotalCount = totalCount,
        //            TotalFillMoneyBalance = totalFillMoneyBalance,
        //            TotalBonusBalance = totalBonusBalance,
        //            TotalCommissionBalance = totalCommissionBalance,
        //            TotalExpertsBalance = totalExpertsBalance,
        //            TotalFreezeBalance = totalFreezeBalance,
        //            TotalRedBagBalance = totalRedBagBalance,
        //            TotalDouDou = totalDouDou,
        //            TotalCPSBalance = totalCPSBalance
        //        };
        //        collection.LoadList(list);
        //        return collection;
        //    }
        //}

        //public UserQueryInfoCollection QueryUserList_AdminCPS(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isAgent
        //  , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int ocAgentCategory, int pageIndex, int pageSize, string strOrderBy)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        int totalCount;
        //        decimal totalFillMoneyBalance;
        //        decimal totalBonusBalance;
        //        decimal totalCommissionBalance;
        //        decimal totalExpertsBalance;
        //        decimal totalFreezeBalance;
        //        decimal totalRedBagBalance;
        //        decimal totalCPSBalance;
        //        int totalDouDou;
        //        var list = manager.QueryUserList_AdminCPS(regFrom, regTo, keyType, keyValue, isEnable, isFillMoney, isAgent, commonBlance, bonusBlance, freezeBlance, vipRange, comeFrom, agentId, ocAgentCategory, pageIndex, pageSize,
        //            out totalCount, out totalFillMoneyBalance, out totalBonusBalance, out totalCommissionBalance, out totalExpertsBalance, out totalFreezeBalance, out totalRedBagBalance, out totalDouDou, out totalCPSBalance, strOrderBy);
        //        var collection = new UserQueryInfoCollection
        //        {
        //            TotalCount = totalCount,
        //            TotalFillMoneyBalance = totalFillMoneyBalance,
        //            TotalBonusBalance = totalBonusBalance,
        //            TotalCommissionBalance = totalCommissionBalance,
        //            TotalExpertsBalance = totalExpertsBalance,
        //            TotalFreezeBalance = totalFreezeBalance,
        //            TotalRedBagBalance = totalRedBagBalance,
        //            TotalDouDou = totalDouDou,
        //            TotalCPSBalance = totalCPSBalance
        //        };
        //        collection.LoadList(list);
        //        return collection;
        //    }
        //}

        //public LoginLocal LoginByUserId(string userId)
        //{
        //    LoginLocal loginInfo = null;
        //    using (var loginManager = new LoginLocalManager())
        //    {
        //        loginInfo = loginManager.GetLoginByUserId(userId);
        //        if (loginInfo != null && loginInfo.User != null)
        //        {
        //            NHibernate.NHibernateUtil.Initialize(loginInfo.User.RoleList);
        //            NHibernate.NHibernateUtil.Initialize(loginInfo.Register);
        //        }
        //    }
        //    return loginInfo;
        //}
        //public string GetUserId(string loginName)
        //{
        //    var loginManager = new LoginLocalManager();
        //    var info = loginManager.GetLoginByName(loginName);
        //    return info == null ? string.Empty : info.UserId;
        //}
        ///// <summary>
        ///// 指定代理注册的用户  路径
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <returns></returns>
        //public UserRegister QueryByUserIdReferrerUrl(string userid)
        //{
        //    var referUrl = new UserBalanceManager();
        //    var ReferrerUrl = referUrl.QueryUserRegister(userid);
        //    if (ReferrerUrl == null)
        //        throw new Exception("用户不存在");
        //    return ReferrerUrl;
        //}
        ///// <summary>
        ///// 指定代理注册的用户  路径
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <returns></returns>
        //public UserRegister QueryUserRegisterByUserId(string userid)
        //{
        //    var referUrl = new UserBalanceManager();
        //    var user = referUrl.QueryUserRegister(userid);
        //    if (user == null)
        //        new UserRegister();
        //    return user;
        //}

        ///// <summary>
        ///// 启用用户
        ///// </summary>
        //public void EnableUser(string userId)
        //{
        //    //开启事务
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var manager = new UserBalanceManager();
        //        var register = manager.LoadUserRegister(userId);
        //        if (register == null)
        //            throw new Exception("用户不存在");
        //        register.IsEnable = true;
        //        manager.UpdateUserRegister(register);

        //        biz.CommitTran();
        //    }
        //}
        ///// <summary>
        ///// 禁用用户
        ///// </summary>
        //public void DisableUser(string userId)
        //{
        //    //开启事务
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var manager = new UserBalanceManager();
        //        var register = manager.LoadUserRegister(userId);
        //        if (register == null)
        //            throw new Exception("用户不存在");
        //        register.IsEnable = false;
        //        manager.UpdateUserRegister(register);

        //        biz.CommitTran();
        //    }
        //}
        ///// <summary>
        ///// 恢复代理
        ///// </summary>
        //public void RecoveryAgent(string userId)
        //{
        //    //开启事务
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var manager = new UserBalanceManager();
        //        var register = manager.LoadUserRegister(userId);
        //        if (register == null)
        //            throw new Exception("用户不存在");
        //        var agent = new OCAgentManager().QueryOCAgent(userId);
        //        if (agent == null)
        //            throw new Exception("代理关系不存在");

        //        register.IsAgent = true;
        //        manager.UpdateUserRegister(register);

        //        biz.CommitTran();
        //    }
        //}

        //public UserQueryInfoCollection QueryTogetherHotUserList(DateTime createFrom, DateTime createTo, string keyType, string keyValue,
        //    int pageIndex, int pageSize)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        int totalCount;
        //        var list = manager.QueryTogetherHotUserList(createFrom, createTo, keyType, keyValue, pageIndex, pageSize, out totalCount);
        //        var collection = new UserQueryInfoCollection
        //        {
        //            TotalCount = totalCount
        //        };
        //        collection.LoadList(list);
        //        return collection;
        //    }
        //}

        //public bool IsFillMoney(string userId, DateTime time)
        //{
        //    var manager = new LoginLocalManager();
        //    var resManager = new RestrictionsBetMoneyManager();
        //    var isAuthenFillMoney = new CacheDataBusiness().QueryCoreConfigByKey("IsAuthenFillMoney").ConfigValue == "1";
        //    if (isAuthenFillMoney)//判断是否需要实名、手机认证后充值
        //    {
        //        var mobile = new MobileAuthenticationBusiness().QueryMobileByUserId(userId);
        //        var realName = new RealNameAuthenticationBusiness().QueryRealNameByUserId(userId);
        //        if (mobile == null || realName == null || !mobile.IsSettedMobile || string.IsNullOrEmpty(mobile.Mobile) || !realName.IsSettedRealName || string.IsNullOrEmpty(realName.RealName))
        //            return false;
        //        return true;
        //    }
        //    return true;
        //    //var isBet = resManager.IsBet(userId);
        //    //if (isBet)
        //    //    return true;
        //    //else
        //    //    return manager.IsFillMoney(userId, time);

        //}
        //public List<string> QueryFunctionByRole(string[] arrayRole)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        return manager.QueryFunctionByRole(arrayRole);
        //    }
        //}
        ////根据手机号查询用户编号
        //public UserMobile_Collection QueryUserIDByMobile(string arrayMobile)
        //{
        //    using (var manager = new LoginLocalManager())
        //    {
        //        return manager.QueryUserIDByMobile(arrayMobile);
        //    };
        //}

        //public UserBindInfos QueryUserBindInfos(string userId)
        //{
        //    var user = new UserBalanceManager().QueryUserRegister(userId);
        //    if (user == null || user.IsEnable == false)
        //        return new UserBindInfos();

        //    var info = new LoginLocalManager().QueryUserBindInfos(userId);

        //    return info;
        //}
        //public void BatchSetInnerUser(string userIds)
        //{
        //    if (string.IsNullOrEmpty(userIds))
        //        throw new Exception("用户编号不能为空");
        //    var arrUserIds = userIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        var manager = new UserBalanceManager();
        //        biz.BeginTran();
        //        foreach (var item in arrUserIds)
        //        {
        //            var entity = manager.LoadUserRegister(item);
        //            entity.UserType = 1;
        //            manager.UpdateUserRegister(entity);
        //        }
        //        biz.CommitTran();
        //    }
        //}
    }
}
