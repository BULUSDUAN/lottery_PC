using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserBusiness:DBbase
    {
        public void UpdateUserVipLevel(string userId, int vipLevel)
        {
            var manager = new UserBalanceManager();
            var user = manager.GetUserRegister(userId);
            user.VipLevel = vipLevel;
            manager.UpdateUserRegister(user);
        }
        public void LogOffUserAgent(string userId)
        {
            //开启事务
            DB.Begin();
            var manager = new UserBalanceManager();
            var reg = manager.GetUserRegister(userId);
            if (reg == null)
                throw new Exception("不存在此用户：" + userId);
            reg.IsAgent = false;
            manager.UpdateUserRegister(reg);

            var userManager = new UserManager();
            var user = userManager.LoadSystemUser(userId);
            if (user == null)
                throw new Exception("不存在此用户：" + userId);
            if (user.RoleList == null || !user.RoleList.Any())
                throw new LogicException("查询用户角色时出错,UserID:"+userId);
            var role = user.RoleList.FirstOrDefault(p => p.RoleId == "Agent");
            if (role != null)
                user.RoleList.Remove(role);
            DB.Commit();

        }
    }
}
