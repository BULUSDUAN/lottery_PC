using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using Common.Cryptography;
using External.Domain.Managers.Login;
using External.Domain.Entities.Login;
using External.Core.Login;
using GameBiz.Domain.Managers;
using GameBiz.Domain.Entities;

namespace External.Business
{
    public class QQLoginBusiness
    {
        public LoginQQ GetQQByOpenId(string openId)
        {
            using (var manager = new LoginQQManager())
            {
                return manager.GetQQByOpenId(openId);
            }
        }
        public LoginQQ GetQQ(string loginName)
        {
            using (var manager = new LoginQQManager())
            {
                return manager.GetQQ(loginName);
            }
        }
        public LoginQQ GetQQByUserId(string userId)
        {
            using (var manager = new LoginQQManager())
            {
                return manager.GetLoginByUserId(userId);
            }
        }

        public LoginQQ QueryQQByUserIdOpenId(string userId, string openId)
        {
            return new LoginQQManager().QueryLoginByUserIdOpenId(userId, openId);
        }
        public void BindExistUser(string userId, string openId)
        {
            //获取用户信息
            var localBiz = new LocalLoginBusiness();
            var local = localBiz.GetLocalLoginByUserId(userId);
            if (local == null)
                throw new ArgumentException("该用户不存在 " + userId);

            LoginQQ login = new LoginQQ()
            {
                DisplayName = local.Register.DisplayName,
                LoginName = local.LoginName,
                OpenId = openId,
                UserId = local.UserId,
                Register = local.Register,
                User = local.User,
                CreateTime = DateTime.Now,
            };
            new LoginQQManager().Register(login);
        }

        public int GetTodayRegisterCount(DateTime date, string ip)
        {
            using (var loginManager = new LoginQQManager())
            {
                return loginManager.GetTodayRegisterCount(date, ip);
            }
        }
        public void Register(LoginQQ loginEntity, string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var loginManager = new LoginQQManager())
                {
                    var tmp = loginManager.GetLoginByName(loginEntity.LoginName);
                    if (tmp != null)
                    {
                        throw new AuthException("登录名已经存在 - " + loginEntity.LoginName);
                    }
                    loginEntity.User = loginManager.LoadUser(userId);
                    loginEntity.Register = loginManager.LoadRegister(userId);
                    loginManager.Register(loginEntity);
                }
                biz.CommitTran();
            }
        }
        public UserRegister GetUserRegister(string userId)
        {
            return new UserBalanceManager().QueryUserRegister(userId);
        }
        public void Update(string userId, RegisterInfo_QQ qqInfo)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var loginManager = new LoginQQManager())
                {
                    var loginEntity = loginManager.GetLoginByUserId(userId);
                    if (loginEntity == null)
                    {
                        throw new ArgumentException("此支付宝帐号不存在");
                    }
                    loginEntity.LoginName = qqInfo.LoginName;
                    loginEntity.DisplayName = qqInfo.DisplayName;
                    loginEntity.OpenId = qqInfo.OpenId;

                    loginManager.UpdateLogin(loginEntity);
                }
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询用户总数
        /// </summary>
        public int QueryUserRegisterCount()
        {
            var loginManager = new LoginQQManager();
            return loginManager.QueryUserRegisterCount();
        }
    }
}
