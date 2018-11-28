using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Auth.Domain.Entities;
using NHibernate.Criterion;
using GameBiz.Core;
using GameBiz.Auth.Business;

namespace GameBiz.Auth.Domain.Managers
{
    public class RoleManager : GameBizAuthEntityManagement
    {
        public void AddRole(SystemRole role)
        {
            Add<SystemRole>(role);
            Add<RoleFunction>(role.FunctionList.ToArray());
        }
        public void UpdateRole(SystemRole role)
        {
            Update<SystemRole>(role);
        }
        public void DeleteRole(SystemRole role)
        {
            Delete<RoleFunction>(role.FunctionList.ToArray());
            Delete<SystemRole>(role);
        }
        public SystemRole GetRoleById(string roleId)
        {
            return GetByKey<SystemRole>(roleId);
        }
        public void AddRoleFunction(RoleFunction roleFunction)
        {
            Add<RoleFunction>(roleFunction);
        }
        public void UpdateRoleFunction(RoleFunction roleFunction)
        {
            Update<RoleFunction>(roleFunction);
        }
        public IList<SystemRole> GetAdminRoleList(string ignoreRoleId)
        {
            return Session.CreateCriteria<SystemRole>()
                .Add(Restrictions.Eq("IsAdmin", true))
                .Add(Restrictions.Not(Restrictions.Eq("RoleId", ignoreRoleId)))
                .List<SystemRole>();
        }
        public RoleFunction GetRoleFunction(SystemRole role, string functionId)
        {
            return Session.CreateCriteria<RoleFunction>()
                .Add(Restrictions.Eq("Role", role))
                .Add(Restrictions.Eq("Function", LoadFunctionById(functionId)))
                .UniqueResult<RoleFunction>();
        }
        public void DeleteRoleFunction(RoleFunction roleFunction)
        {
            Delete<RoleFunction>(roleFunction);
        }
        public IList<Function> QueryFixFunctionList(RoleType roleType)
        {
            var query = Session.CreateCriteria<Function>();
            if (roleType == RoleType.WebRole)
            {
                query = query.Add(Restrictions.Eq("IsWebBasic", true));
            }
            else if (roleType == RoleType.BackgroundRole)
            {
                query = query.Add(Restrictions.Eq("IsBackBasic", true));
            }
            query = query.AddOrder(NHibernate.Criterion.Order.Asc("FunctionId"));
            return query.List<Function>();
        }
        public IList<Function> QueryConfigFunctionList(RoleType roleType)
        {
            var query = Session.CreateCriteria<Function>();
            if (roleType == RoleType.WebRole)
            {
                query = query.Add(Restrictions.Eq("IsWebBasic", false));
            }
            else if (roleType == RoleType.BackgroundRole)
            {
                query = query.Add(Restrictions.Eq("IsBackBasic", false));
                //query = query.Add(Restrictions.Eq("IsBackBasic", false)).Add(Restrictions.Eq("ParentId", "0"));
            }
            query = query.AddOrder(NHibernate.Criterion.Order.Asc("FunctionId"));
            return query.List<Function>();
        }
        public IList<SystemRole> QueryRoleList()
        {
            return Session.CreateCriteria<SystemRole>()
                .AddOrder(NHibernate.Criterion.Order.Asc("IsInner"))
                .AddOrder(NHibernate.Criterion.Order.Asc("IsAdmin"))
                .List<SystemRole>();
        }
        public SystemRole LoadRoleById(string roleId)
        {
            return LoadByKey<SystemRole>(roleId);
        }
        public Function LoadFunctionById(string functionId)
        {
            return LoadByKey<Function>(functionId);
        }
    }
}
