using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using External.Domain.Entities.Authentication;
using External.Domain.Managers.Authentication;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using Common.Cryptography;
using External.Domain.Managers;
using External.Domain.Entities.Task;
using External.Core;
using External.Core.Authentication;

namespace External.Business
{
    public class QQAuthenticationBusiness
    {
        /// <summary>
        /// 用户添加qq
        /// </summary>
        public void AddUserQQ(string userId, string qq)
        {
            var userManager = new BettingPointManager();
            var manager = new UserQQManager();
            var entity = manager.GetUserQQ(userId);
            if (entity != null)
            {
                entity.QQ = qq;
                manager.UpdateUserQQ(entity);
            }
            else
            {
                manager.AddUserQQ(new UserQQ
                {
                    QQ = qq,
                    UserId = userId,
                    User = userManager.LoadUser(userId),
                });
            }
        }
        /// <summary>
        /// 后台取消绑定QQ
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="qq"></param>
        public void CancelUserQQ(string userId)
        {
            var userManager = new BettingPointManager();
            var manager = new UserQQManager();
            var entity = manager.GetUserQQ(userId);
            if (entity != null)
            {
                manager.DeleteUserQQ(entity);
            }
            else {
                throw new ArgumentException("此用户没有绑定QQ号码");
            }
        }

    }
}
