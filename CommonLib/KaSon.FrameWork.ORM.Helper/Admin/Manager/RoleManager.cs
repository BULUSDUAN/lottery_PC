using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
    public class RoleManager : DBbase
    {
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public C_Auth_Roles GetRoleById(string roleId)
        {
            return DB.CreateQuery<C_Auth_Roles>().Where(p => p.RoleId == roleId).FirstOrDefault();
        }

        /// <summary>
        /// 根据角色获取角色的所有权限列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<C_Auth_Function_List> GetFunctionListByRoleId(string roleId)
        {
            var query = from car in DB.CreateQuery<C_Auth_RoleFunction>()
                        join caf in DB.CreateQuery<C_Auth_Function_List>() on car.FunctionId equals caf.FunctionId
                        where car.RoleId == roleId && car.Status == (int)EnableStatus.Enable
                        select caf;
            return query.ToList();
        }

        /// <summary>
        /// 是否已存在超级管理员角色
        /// </summary>
        /// <returns></returns>
        public bool HasSurperAdmin()
        {
            return DB.CreateQuery<C_Auth_Roles>().Where(p => p.IsAdmin).FirstOrDefault() != null;
        }

        public IList<C_Auth_Function_List> QueryFixFunctionList(RoleType roleType)
        {
            var query = DB.CreateQuery<C_Auth_Function_List>();
            if (roleType == RoleType.WebRole)
            {
                query = query.Where(p => p.IsWebBasic);
            }
            else if (roleType == RoleType.BackgroundRole)
            {
                query = query.Where(p => p.IsBackBasic);
            }
            return query.ToList();
        }
        public void AddRole(SystemRole role)
        {
            var roleModel = new C_Auth_Roles()
            {
                IsAdmin = role.IsAdmin,
                IsInner = role.IsInner,
                ParentRoleId = role.ParentRole == null ? "" : role.ParentRole.RoleId,
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                RoleType = (int)role.RoleType
            };
            DB.GetDal<C_Auth_Roles>().Add(roleModel);
            if (role.FunctionList != null && role.FunctionList.Count > 0)
            {
                var list = new List<C_Auth_RoleFunction>();
                foreach (var item in role.FunctionList)
                {
                    var temp = new C_Auth_RoleFunction()
                    {
                        FunctionId = item.FunctionId,
                        Mode = item.Mode,
                        RoleId = role.RoleId,
                        Status = (int)item.Status
                    };
                    list.Add(temp);
                }
                DB.GetDal<C_Auth_RoleFunction>().BulkAdd(list);
            }
        }

        public void UpdateRole(C_Auth_Roles role)
        {
            DB.GetDal<C_Auth_Roles>().Update(role);
        }

        public RoleFunction GetRoleFunction(string roleId, string functionId)
        {
            return DB.CreateQuery<C_Auth_Function_List>().Join(DB.CreateQuery<C_Auth_RoleFunction>(), p => p.FunctionId, c => c.FunctionId, (p, c) => new { p, c }).Where(a => a.c.FunctionId == functionId && a.c.RoleId == roleId).Select(p => new RoleFunction()
            {
                IId = p.c.IId,
                FunctionId = p.c.FunctionId,
                Mode = p.c.Mode,
                Function = new Function()
                {
                    DisplayName = p.p.DisplayName
                }
            }).FirstOrDefault();
        }

        public void AddRoleFunction(C_Auth_RoleFunction roleFunction)
        {
            DB.GetDal<C_Auth_RoleFunction>().Add(roleFunction);
        }

        public void DeleteRoleFunction(C_Auth_RoleFunction roleFunction)
        {
            DB.GetDal<C_Auth_RoleFunction>().Delete(roleFunction);
        }

        public List<C_Auth_Roles> QueryRoleList()
        {
            return DB.CreateQuery<C_Auth_Roles>().OrderBy(p=>p.IsInner).ThenBy(p=>p.IsAdmin).ToList();
        }
    }
}
