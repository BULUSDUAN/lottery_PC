using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class RoleManager:DBbase
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
                        where car.RoleId == roleId && car.Status==(int)EnableStatus.Enable
                        select caf;
            return query.ToList();
        }

        /// <summary>
        /// 是否已存在超级管理员角色
        /// </summary>
        /// <returns></returns>
        public bool HasSurperAdmin()
        {
            return DB.CreateQuery<C_Auth_Roles>().Where(p => p.IsAdmin).FirstOrDefault()!=null;
        }
    }
}
