using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserBusiness
    {
        public void UpdateUserVipLevel(string userId, int vipLevel)
        {
            var manager = new UserBalanceManager();
            var user = manager.GetUserRegister(userId);
            user.VipLevel = vipLevel;
            manager.UpdateUserRegister(user);
        }
    }
}
