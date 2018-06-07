using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Cryptography;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class GameBizAuthBusiness:DBbase
    {
        /// <summary>
        /// 获取用户口令
        /// </summary>
        /// <param name="userId">帐号</param>
        /// <returns>口令</returns>
        public string GetUserToken(string userId)
        {
           
                var login = DB.CreateQuery<C_Auth_Users>().Where(p => p.UserId == userId).Select(p => new SystemUser()
                {
                    CreateTime = p.CreateTime,
                    AgentId = p.AgentId,
                    RegFrom = p.RegFrom,
                    UserId = p.UserId
                }).FirstOrDefault();

                CheckUser(login, userId);

                return GetLoginUserToken(login);
            
        }
        //private static string _guestToken = null;
        //public string GetGuestToken()
        //{
        //    if (string.IsNullOrEmpty(_guestToken))
        //    {
        //        IList<AccessControlItem> acl = new List<AccessControlItem>();
        //        var role = GetGuestRole();
        //        MergeRoleAccessControlList(ref acl, role);
        //        _guestToken = GetUserToken(role.RoleId, acl);
        //    }
        //    return _guestToken;
        //}
        //private static SystemRole _guestRole = null;
        //private SystemRole GetGuestRole()
        //{
        //    if (_guestRole == null)
        //    {
        //        string roleId = "Guest";
        //        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GuestRoleId"]))
        //        {
        //            roleId = ConfigurationManager.AppSettings["GuestRoleId"];
        //        }
        //        using (var userManager = new UserManager())
        //        {
        //            _guestRole = userManager.GetRoleById(roleId);
        //            NHibernate.NHibernateUtil.Initialize(_guestRole.FunctionList);
        //        }
        //    }
        //    return _guestRole;
        //}
        //public void RegisterUser(SystemUser user, string[] roleIds)
        //{
        //    if (roleIds.Length == 0)
        //    {
        //        throw new AuthException("必须指定角色");
        //    }
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var userManager = new UserManager())
        //        {
        //            var lastKey = userManager.GetLastUserKey();
        //            IList<string> skipList;
        //            user.UserId = BeautyNumberHelper.GetNextCommonNumber(lastKey, out skipList);

        //            var roleList = userManager.GetRoleListByIds(roleIds);
        //            if (roleList.Count != roleIds.Length)
        //            {
        //                throw new AuthException("指定的角色可能不存在 - " + string.Join(",", roleIds));
        //            }
        //            user.RoleList = roleList;
        //            userManager.AddSystemUser(user);
        //            userManager.UpdateLastUserKey(user.UserId, skipList.Count);
        //            if (skipList.Count > 0)
        //            {
        //                userManager.AddSkipBeautyKey(skipList.ToArray(), lastKey, user.UserId);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public void AddUserRoles(string userId, string[] roleIds)
        //{
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var userManager = new UserManager())
        //        {
        //            var user = userManager.GetUserById(userId);
        //            if (user == null)
        //            {
        //                throw new ArgumentException("指定的用户不存在。");
        //            }
        //            NHibernate.NHibernateUtil.Initialize(user.RoleList);
        //            var roleList = userManager.GetRoleListByIds(roleIds);
        //            foreach (var role in roleList)
        //            {
        //                user.RoleList.Add(role);
        //            }
        //            userManager.UpdateSystemUser(user);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public void RemoveUserRoles(string userId, string[] roleIds)
        //{
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var userManager = new UserManager())
        //        {
        //            var user = userManager.GetUserById(userId);
        //            if (user == null)
        //            {
        //                throw new ArgumentException("指定的用户不存在。");
        //            }
        //            NHibernate.NHibernateUtil.Initialize(user.RoleList);
        //            foreach (var id in roleIds)
        //            {
        //                foreach (var role in user.RoleList)
        //                {
        //                    if (role.RoleId == id)
        //                    {
        //                        user.RoleList.Remove(role);
        //                        break;
        //                    }
        //                }
        //            }
        //            if (user.RoleList.Count == 0)
        //            {
        //                throw new AuthException("用户必须指定至少一个角色");
        //            }
        //            userManager.UpdateSystemUser(user);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public void AddSystemRole(RoleInfo_Add roleInfo)
        //{
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var roleManager = new RoleManager())
        //        {
        //            var role = roleManager.GetRoleById(roleInfo.RoleId);
        //            if (role != null)
        //            {
        //                throw new ArgumentException("指定编号的角色已经存在 - " + role.RoleId);
        //            }
        //            role = new SystemRole
        //            {
        //                RoleId = roleInfo.RoleId,
        //                RoleName = roleInfo.RoleName,
        //                IsAdmin = roleInfo.IsAdmin,
        //                IsInner = false,
        //                RoleType = roleInfo.RoleType,
        //                FunctionList = new List<RoleFunction>(),
        //            };
        //            if (!role.IsAdmin)
        //            {
        //                foreach (var item in roleInfo.FunctionList)
        //                {
        //                    var roleFunction = new RoleFunction
        //                    {
        //                        Role = role,
        //                        FunctionId = item.FunctionId,
        //                        Function = roleManager.LoadFunctionById(item.FunctionId),
        //                        Status = EnableStatus.Enable,
        //                        Mode = item.Mode,
        //                    };
        //                    role.FunctionList.Add(roleFunction);
        //                }
        //                var list = roleManager.QueryFixFunctionList(roleInfo.RoleType);
        //                foreach (var item in list)
        //                {
        //                    var roleFunction = new RoleFunction
        //                    {
        //                        Role = role,
        //                        FunctionId = item.FunctionId,
        //                        Function = item,
        //                        Status = EnableStatus.Enable,
        //                        Mode = "RW",
        //                    };
        //                    role.FunctionList.Add(roleFunction);
        //                }
        //            }
        //            roleManager.AddRole(role);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public void UpdateSystemRole(RoleInfo_Update roleInfo)
        //{
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        using (var roleManager = new RoleManager())
        //        {
        //            var role = roleManager.GetRoleById(roleInfo.RoleId);
        //            if (role == null)
        //            {
        //                throw new ArgumentException("指定编号的角色不存在 - " + roleInfo.RoleId);
        //            }
        //            role.RoleId = roleInfo.RoleId;
        //            role.RoleName = roleInfo.RoleName;
        //            roleManager.UpdateRole(role);

        //            foreach (var item in roleInfo.AddFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction != null)
        //                {
        //                    throw new ArgumentException("添加权限到角色错误 - 已经包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleFunction = new RoleFunction
        //                {
        //                    Role = role,
        //                    FunctionId = item.FunctionId,
        //                    Function = roleManager.LoadFunctionById(item.FunctionId),
        //                    Status = EnableStatus.Enable,
        //                    Mode = item.Mode,
        //                };
        //                roleManager.AddRoleFunction(roleFunction);
        //            }
        //            foreach (var item in roleInfo.ModifyFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction == null)
        //                {
        //                    throw new ArgumentException("修改权限错误 - 此角色尚未包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleFunction.Mode = item.Mode;
        //                roleManager.UpdateRoleFunction(roleFunction);
        //            }
        //            foreach (var item in roleInfo.RemoveFunctionList)
        //            {
        //                var roleFunction = roleManager.GetRoleFunction(role, item.FunctionId);
        //                if (roleFunction == null)
        //                {
        //                    throw new ArgumentException("移除权限错误 - 此角色尚未包含权限\"" + roleFunction.Function.FunctionId + " - " + roleFunction.Function.DisplayName + "\"");
        //                }
        //                roleManager.DeleteRoleFunction(roleFunction);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //public bool IsSystemRoleAdmin(string roleId)
        //{
        //    using (var roleManager = new RoleManager())
        //    {
        //        var role = roleManager.GetRoleById(roleId);
        //        if (role == null)
        //        {
        //            throw new ArgumentException("指定编号的角色不存在 - " + roleId);
        //        }
        //        return role.IsAdmin;
        //    }
        //}
        //public int GetAdminSystemRoleCount(string roleId)
        //{
        //    using (var roleManager = new RoleManager())
        //    {
        //        var roleList = roleManager.GetAdminRoleList(roleId);
        //        return roleList.Count;
        //    }
        //}

        //public RoleInfo_Query GetSystemRoleById(string roleId)
        //{
        //    using (var roleManager = new RoleManager())
        //    {
        //        var role = roleManager.GetRoleById(roleId);

        //        var info = new RoleInfo_Query();
        //        ObjectConvert.ConverEntityToInfo<SystemRole, RoleInfo_Query>(role, ref info);
        //        foreach (var function in role.FunctionList)
        //        {
        //            if (function.Status == EnableStatus.Enable)
        //            {
        //                var item = new RoleFunctionInfo
        //                {
        //                    FunctionId = function.FunctionId,
        //                    Mode = function.Mode,
        //                };
        //                info.FunctionList.Add(item);
        //            }
        //        }
        //        return info;
        //    }
        //}

        //public RoleInfo_QueryCollection GetSystemRoleCollection()
        //{
        //    using (var roleManager = new RoleManager())
        //    {
        //        var list = roleManager.QueryRoleList();

        //        var collection = new RoleInfo_QueryCollection();
        //        ObjectConvert.ConvertEntityListToInfoList<IList<SystemRole>, SystemRole, RoleInfo_QueryCollection, RoleInfo_Query>(list, ref collection, () => new RoleInfo_Query());
        //        return collection;
        //    }
        //}
        //public FunctionCollection QueryConfigFunctionCollection(RoleType roleType)
        //{
        //    using (var roleManager = new RoleManager())
        //    {
        //        var list = roleManager.QueryConfigFunctionList(roleType);

        //        var collection = new FunctionCollection();
        //        ObjectConvert.ConvertEntityListToInfoList<IList<Function>, Function, FunctionCollection, FunctionInfo>(list, ref collection, () => new FunctionInfo());
        //        return collection;
        //    }
        //}
        //public void DeleteSystemRole(string roleId)
        //{
        //    using (var biz = new GameBizAuthBusinessManagement())
        //    {
        //        biz.BeginTran();

        //        var roleManager = new RoleManager();
        //        var userManager = new UserManager();

        //        var role = roleManager.GetRoleById(roleId);
        //        if (role == null)
        //        {
        //            throw new ArgumentException("指定编号的角色不存在 - " + roleId);
        //        }
        //        if (role.IsInner)
        //        {
        //            throw new ArgumentException("不允许删除内置角色 - " + role.RoleName);
        //        }
        //        var childrenCount = userManager.GetRoleChildrenCount(roleId);
        //        if (childrenCount > 0)
        //        {
        //            throw new ArgumentException(string.Format("指定删除的角色\"{0}\"拥有子节点，不允许被删除", role.RoleName));
        //        }
        //        var userCount = userManager.GetRoleContainUserCount(roleId);
        //        if (userCount > 0)
        //        {
        //            throw new ArgumentException(string.Format("指定删除的角色\"{0}\"被分配给 {1}个用户，不允许删除", role.RoleName, userCount));
        //        }
        //        roleManager.DeleteRole(role);

        //        biz.CommitTran();
        //    }
        //}
        private void CheckUser(SystemUser user, string userId)
        {
            if (user == null)
            {
                throw new LogicException("登录名\"" + userId + "\"不存在");
            }
            if (user.RoleList.Count == 0)
            {
                throw new AuthException("系统配置错误，未配置角色信息");
            }
        }
        private string GetLoginUserToken(SystemUser user)
        {
            IList<AccessControlItem> acl = new List<AccessControlItem>();
            foreach (var role in user.RoleList)
            {
                if (role.RoleType == RoleType.BackgroundRole && role.IsAdmin)
                {
                    return GetAdminToken(user.UserId);
                }
                MergeRoleAccessControlList(ref acl, role);
            }
            acl = MergeAccessControlList<AccessControlItem, UserFunction>(acl, user.FunctionList);
            return GetUserToken(user.UserId, acl);
        }
        private void MergeRoleAccessControlList(ref IList<AccessControlItem> acl, SystemRole role)
        {
            if (role.ParentRole != null)
            {
                MergeRoleAccessControlList(ref acl, role.ParentRole);
            }
            acl = MergeAccessControlList<AccessControlItem, RoleFunction>(acl, role.FunctionList);
        }
        ///// <summary>
        ///// 验证用户身份及权限，并返回用户名称
        ///// </summary>
        //public static void ValidateAuthentication(string userToken, string needRight, string functionId, out string userId)
        //{
        //    try
        //    {
        //        var rlt = UserTokenHandler.ValidateAuthentication(userToken, needRight, functionId, "LI");
        //        if (!rlt.ContainsKey("LI"))
        //        {
        //            throw new AuthException("UserToken不完整，缺少UserId信息");
        //        }
        //        userId = rlt["LI"];
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new AuthException("用户身份验证失败，请检查是否已登录", ex);
        //    }
        //}

        //private static List<MethodFunction> _allMethodFunctionList = new List<MethodFunction>();
        ///// <summary>
        ///// 验证用户是否具有该方法的权限
        ///// </summary>
        //public static string ValidateUserAuthentication(string userToken)
        //{
        //    if (_allMethodFunctionList == null || _allMethodFunctionList.Count == 0)
        //    {
        //        var userManager = new UserManager();
        //        _allMethodFunctionList = userManager.LoadAllMethodFunction();
        //    }

        //    var method = new System.Diagnostics.StackFrame(1).GetMethod();
        //    var currentFullName = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
        //    var config = _allMethodFunctionList.FirstOrDefault(p => p.MethodFullName == currentFullName);
        //    if (config == null)
        //        throw new Exception(string.Format("没有配置方法 {0} 的调用权限数据", currentFullName));

        //    var userId = string.Empty;
        //    ValidateAuthentication(userToken, config.Mode, config.FunctionId, out userId);
        //    return userId;
        //}

        //public static bool CheckIsAdmin(string userToken)
        //{
        //    try
        //    {
        //        return UserTokenHandler.IsAdmin(userToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new AuthException("用户身份验证失败，请检查是否已登录", ex);
        //    }
        //}
        public string GetAdminToken(string userId)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("LI", userId);
            dic.Add("IA", "IA");
            return UserTokenHandler.GetUserToken(dic);
        }
        public string GetUserToken(string userId, IList<AccessControlItem> acl)
        {
            var dic = new Dictionary<string, string>(acl.Count);
            foreach (var item in acl)
            {
                if (item.Status != EnableStatus.Enable)
                {
                    throw new AuthException("被禁止的权限控制项不能出现在此");
                }
                if (!dic.ContainsKey(item.FunctionId))
                {
                    dic.Add(item.FunctionId, item.Mode);
                }
                else
                {
                    dic[item.FunctionId] = MergeFunctionMode(dic[item.FunctionId], item.Mode);
                }
            }
            dic.Add("LI", userId);
            return UserTokenHandler.GetUserToken(dic);
        }
        // 合并两个控制访问列表。T2覆盖T1的列表
        public IList<AccessControlItem> MergeAccessControlList<T1, T2>(IList<T1> acl1, IList<T2> acl2)
            where T1 : AccessControlItem
            where T2 : AccessControlItem
        {
            if (acl1.Where(a => a.Status != EnableStatus.Enable).Count() > 0)
            {
                throw new AuthException("被合并的访问控制列表的原始表出现了禁用项");
            }
            if (acl1.Where(a => a.Mode.Length < 1 || a.Mode.Length > 2 || (a.Mode != "R" && a.Mode != "W" && a.Mode != "RW")).Count() > 0)
            {
                throw new AuthException("被合并的访问控制列表的原始表出现了无效的Mode值，只允许R、W或RW");
            }
            if (acl2.Where(a => a.Mode.Length < 1 || a.Mode.Length > 2 || (a.Mode != "R" && a.Mode != "W" && a.Mode != "RW")).Count() > 0)
            {
                throw new AuthException("被合并的访问控制列表的目标表出现了无效的Mode值，只允许R、W或RW");
            }
            var result = new List<AccessControlItem>();
            if (acl2.Count == 0)
            {
                result.AddRange(acl1);
                return result;
            }
            foreach (var t2 in acl2)
            {
                if (t2.Status == EnableStatus.Enable)
                {
                    result.Add(t2);
                }
                else if (t2.Status == EnableStatus.Disable)
                {
                    var tmp = FindItem<T1>(acl1, t2.FunctionId);
                    if (tmp != null)
                    {
                        if (tmp.Mode == t2.Mode || t2.Mode.Contains(tmp.Mode))
                        {
                            acl1.Remove(tmp);
                        }
                        else if (tmp.Mode.Contains(t2.Mode))
                        {
                            tmp.Mode = (t2.Mode == "R" ? "W" : "R");
                        }
                    }
                }
                else
                {
                    throw new AuthException("访问控制项状态为未知");
                }
            }
            foreach (var t1 in acl1)
            {
                if (!IsContains<AccessControlItem>(result, t1))
                {
                    result.Add(t1);
                }
            }
            return result;
        }
        private string MergeFunctionMode(string mode1, string mode2)
        {
            return new string(mode1.Union(mode2).ToArray());
        }
        private T FindItem<T>(IList<T> acl, string functionId)
            where T : AccessControlItem
        {
            foreach (var t in acl)
            {
                if (t.FunctionId == functionId)
                {
                    return t;
                }
            }
            return null;
        }
        private bool IsContains<T>(IList<T> acl, AccessControlItem item)
            where T : AccessControlItem
        {
            foreach (var t in acl)
            {
                if (t.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        //#region 后台系统管理


        //public SysOpratorInfo_Collection GetOpratorCollection(int pageIndex, int pageSize)
        //{
        //    using (var manage = new UserManager())
        //    {
        //        return manage.GetOpratorCollection(pageIndex, pageSize);
        //    }
        //}

        //public RoleInfo_QueryCollection QueryRoleCollection()
        //{
        //    using (var manage = new UserManager())
        //    {
        //        return manage.QueryRoleCollection();
        //    }
        //}
        //public string QueryUserRoleIdsByUserId(string userId)
        //{
        //    using (var manage = new UserManager())
        //    {
        //        return manage.QueryUserRoleIdsByUserId(userId);
        //    }
        //}

        //#endregion

    }
}
