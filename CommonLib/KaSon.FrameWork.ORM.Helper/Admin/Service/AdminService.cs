﻿using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.Communication;

namespace KaSon.FrameWork.ORM.Helper
{
    public partial class AdminService
    {
        public LoginInfo LoginAdmin(string loginName, string password, string loginIp)
        {
            var loginBiz = new LocalLoginBusiness();
            var loginEntity = loginBiz.Login(loginName, password);
            if (loginEntity == null)
            {
                return new LoginInfo { IsSuccess = false, Message = "登录名或密码错误", LoginFrom = "ADMIN", };
            }

            var register = new UserBalanceManager().LoadUserRegister(loginEntity.UserId);
            if (register == null)
            {
                return new LoginInfo { IsSuccess = false, Message = "找不到用户注册信息", LoginFrom = "ADMIN", };
            }
            if (!register.IsEnable)
            {
                return new LoginInfo { IsSuccess = false, Message = "用户已禁用", LoginFrom = "ADMIN", };
            }
            var authBiz = new GameBizAuthBusiness();
            if (!IsRoleType(loginEntity.User, RoleType.BackgroundRole))
            {
                return new LoginInfo { IsSuccess = false, Message = "此帐号角色不允许在此登录", LoginFrom = "ADMIN", };
            }
            var userToken = authBiz.GetUserToken(loginEntity.User.UserId);

            bool isAdmin = false;
            if (loginEntity.User != null)
            {
                var query = loginEntity.User.RoleList.FirstOrDefault(s => s.IsAdmin == true);
                if (query != null && query.IsAdmin)
                    isAdmin = true;
            }

            //获取权限点
            List<string> _FunctionList = new List<string>();
            try
            {
                if (loginEntity.User != null && loginEntity.User.RoleList != null)
                {
                    List<string> _role = new List<string>();
                    foreach (var item in loginEntity.User.RoleList)
                    {
                        _role.Add(item.RoleId);
                    }
                    _FunctionList = loginBiz.QueryFunctionByRole(_role.ToArray());
                }
            }
            catch
            {
            }

            //! 执行扩展功能代码 - 提交事务前
            BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { loginEntity.UserId, "ADMIN", loginIp, DateTime.Now });

            return new LoginInfo { IsSuccess = true, Message = "登录成功", CreateTime = loginEntity.CreateTime, LoginFrom = "ADMIN", RegType = loginEntity.Register.RegType, Referrer = loginEntity.Register.Referrer, UserId = loginEntity.User.UserId, VipLevel = loginEntity.Register.VipLevel, LoginName = loginEntity.LoginName, DisplayName = loginEntity.LoginName, UserToken = userToken, FunctionList = _FunctionList, IsAdmin = isAdmin };
        }

        private bool IsRoleType(SystemUser user, RoleType roleType)
        {
            foreach (var role in user.RoleList)
            {
                if (role.RoleType == roleType)
                {
                    return true;
                }
            }
            return false;
        }

        #region 权限的增删改查
        /// <summary>
        /// 查询后台菜单
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public List<MenuInfo> QueryMyMenuCollection(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication_Admin(userToken);

            var biz = new AdminMenuBusiness();
            List<E_Menu_List> list;
            if (GameBizAuthBusiness.CheckIsAdmin(userToken))
            {
                list = biz.QueryAllMenuList();
            }
            else
            {
                list = biz.QueryMenuListByUserId(userId);
            }
            var result = list.Select(p => new MenuInfo()
            {
                Description = p.Description,
                DisplayName = p.DisplayName,
                IsEnable = p.IsEnable,
                MenuId = p.MenuId,
                MenuType = (MenuType)p.MenuType,
                ParentId = p.ParentMenuId,
                Url = p.Url
            }).ToList();
            return result;
        }

        /// <summary>
        /// 查询角色的信息及所有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public RoleInfo_Query GetSystemRoleById(string roleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var authBiz = new GameBizAuthBusiness();
            return authBiz.GetSystemRoleById(roleId);
        }

        /// <summary>
        /// 查询菜单下级功能权限点
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<C_Auth_Function_List> QueryLowerLevelFuncitonList()
        {
            try
            {
                return new AdminMenuBusiness().QueryLowerLevelFuncitonList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增系统角色
        /// todo:后台权限
        /// </summary>
        //public CommonActionResult AddSystemRole(RoleInfo_Add roleInfo, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    PreconditionAssert.IsNotEmptyString(roleInfo.RoleId, "添加的角色编号不能为空");
        //    PreconditionAssert.IsNotEmptyString(roleInfo.RoleName, "添加的角色名称不能为空");
        //    PreconditionAssert.IsNotNull(roleInfo.FunctionList, "传入角色的权限列表不能为null");

        //    var authBiz = new GameBizAuthBusiness();
        //    var result= authBiz.AddSystemRole(roleInfo);
        //    return new CommonActionResult(true, "添加角色成功");
        //}
        #endregion
    }
}
