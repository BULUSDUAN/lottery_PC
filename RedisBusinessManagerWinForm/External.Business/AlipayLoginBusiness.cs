using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using Common.Cryptography;
using External.Domain.Managers.Login;
using External.Domain.Entities.Login;
using External.Core.Login;
using System.Collections;
using GameBiz.Business;

namespace External.Business
{
    public class AlipayLoginBusiness
    {
        public LoginAlipay GetAlipayByOpenId(string openId)
        {
            return new LoginAlipayManager().QueryByOpenId(openId);
        }

        public LoginAlipay GetAlipayByUserId(string userId)
        {
            return new LoginAlipayManager().QueryByUserId(userId);
        }

        public LoginAlipay QueryAlipayByUserIdOpenId(string userId, string openId)
        {
            return new LoginAlipayManager().QueryAlipayByUserIdOpenId(userId, openId);
        }

        public void BindExistUser(string userId, string openId)
        {
            //获取用户信息
            var localBiz = new LocalLoginBusiness();
            var local = localBiz.GetLocalLoginByUserId(userId);
            if (local == null)
                throw new ArgumentException("该用户不存在 " + userId);

            LoginAlipay login = new LoginAlipay()
            {
                LoginName = local.LoginName,
                OpenId = openId,
                UserId = local.UserId,
                Register = local.Register,
                User = local.User,
                CreateTime = DateTime.Now,
            };
            new LoginAlipayManager().Register(login);
        }
    }
}
