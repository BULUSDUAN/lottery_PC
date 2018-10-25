using System;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper
{
    public class AlipayAuthenticationBusiness
    {
        public void AddUserAlipay(string userId, string alipayAccount)
        {
            var userManager = new BettingPointManager();
            var manager = new UserAlipayManager();
            var old = manager.GetUserAlipayByAccount(alipayAccount);
            if (old != null)
                throw new Exception("您的账号已被其它用户绑定");
            var entity = manager.GetUserAlipay(userId);
            if (entity != null)
            {
                entity.AlipayAccount = alipayAccount;
                manager.UpdateUserAlipay(entity);
            }
            else
            {
                manager.AddUserAlipay(new E_Authentication_Alipay()
                {
                    AlipayAccount = alipayAccount,
                    UserId = userId
                });
            }
        }
        /// <summary>
        /// 后台取消绑定支付宝
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="alipayAccount"></param>
        public void CancelUserAlipay(string userId)
        {
            var userManager = new BettingPointManager();
            var manager = new UserAlipayManager();
            var entity = manager.GetUserAlipay(userId);
            if (entity != null)
            {
                manager.DeleteUserAlipay(entity);
            }
            else
            {
                throw new ArgumentException("此用户没有绑定支付宝");
            }
        }
    }
}
