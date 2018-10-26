using EntityModel.Communication;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class GameBusinessManagement:DBbase
    {
        public void AddBackgroundAdminUser(RegisterInfo_Admin regInfo)
        {
            DB.Begin();
            try
            {
                #region 注册权限控制帐号
                var userId = Guid.NewGuid().ToString("N");
                var registerBusiness = new RegisterBusiness();
                var userEntity = new SystemUser
                {
                    UserId = userId,
                    RegFrom = "INNER",
                };
                registerBusiness.RegisterUser(userEntity, regInfo.RoleIdList.ToArray());
                #endregion

                #region 注册核心系统显示帐号

                var userRegInfo = new UserRegInfo
                {
                    DisplayName = regInfo.DisplayName,
                    ComeFrom = "INNER",
                    Referrer = regInfo.Referrer,
                    ReferrerUrl = regInfo.ReferrerUrl,
                    RegisterIp = regInfo.RegisterIp,
                    RegType = regInfo.RegType,
                };
                registerBusiness.RegisterUser(userEntity, userRegInfo);

                #endregion

                #region 注册本地登录帐号

                var loginBiz = new LocalLoginBusiness();
                var loginEntity = new LoginLocal
                {
                    LoginName = regInfo.LoginName,
                    Password = null,
                };
                //loginBiz.Register(loginEntity, userId);//原始代码，2014.03.06dj屏蔽
                loginBiz.Register(loginEntity, userEntity.UserId);

                #endregion

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
        }

        public CommonActionResult UpdateBackgroundUserInfo(string userId, string displayName, string addRoleIdList, string removeRoleIdList)
        {
            DB.Begin();
            try
            {
                var userBiz = new RegisterBusiness();
                userBiz.UpdateDisplayName(userId, displayName);

                var authBiz = new GameBizAuthBusiness();
                if (!string.IsNullOrEmpty(addRoleIdList))
                {
                    authBiz.AddUserRoles(userId, addRoleIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }
                if (!string.IsNullOrEmpty(removeRoleIdList))
                {
                    authBiz.RemoveUserRoles(userId, removeRoleIdList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
            return new CommonActionResult(true, "修改后台管理人员信息成功");
        }
    }
}
