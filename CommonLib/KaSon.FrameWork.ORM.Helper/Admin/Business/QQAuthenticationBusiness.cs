using System;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper
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
                manager.AddUserQQ(new E_Authentication_QQ
                {
                    QQ = qq,
                    UserId = userId,
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
