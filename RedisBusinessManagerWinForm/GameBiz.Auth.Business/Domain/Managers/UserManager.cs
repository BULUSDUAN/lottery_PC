using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using GameBiz.Auth.Domain.Entities;
using NHibernate.Linq;
using NHibernate.Criterion;
using Common.Business;
using NHibernate;
using GameBiz.Auth.Business;
using GameBiz.Core;

namespace GameBiz.Auth.Domain.Managers
{
    public class UserManager : GameBizAuthEntityManagement
    {
        public SystemUser GetUserById(string loginId)
        {
            return GetByKey<SystemUser>(loginId);
        }
        public SystemUser LoadUser(string userId)
        {
            return Session.Load<SystemUser>(userId, NHibernate.LockMode.Read);
        }
        public void AddSystemUser(SystemUser user)
        {
            user.CreateTime = DateTime.Now;
            this.Add<SystemUser>(user);
        }
        public void UpdateSystemUser(SystemUser user)
        {
            Session.Clear();
            this.Update<SystemUser>(user);
        }
        public IList<SystemRole> GetRoleListByIds(string[] roleIds)
        {
            var list = new List<SystemRole>();
            foreach (var roleId in roleIds)
            {
                list.Add(LoadByKey<SystemRole>(roleId));
            }
            return list;
        }
        public SystemRole GetRoleById(string roleId)
        {
            return SingleByKey<SystemRole>(roleId);
        }
        public int GetRoleContainUserCount(string roleId)
        {
            return Session.CreateSQLQuery("SELECT COUNT(1) FROM [C_Auth_UserRole] WHERE [RoleId] = :Role")
                .SetString("Role", roleId)
                .UniqueResult<int>();
        }
        public int GetRoleChildrenCount(string roleId)
        {
            return Session.CreateSQLQuery("SELECT COUNT(1) FROM [C_Auth_Roles] WHERE [ParentRoleId] = :Role")
                .SetString("Role", roleId)
                .UniqueResult<int>();
        }
        public string GetLastUserKey()
        {
            Session.Clear();
            var rule = GetByKey<UserKeyRule>("MAXKEY");
            if (rule == null)
            {
                return "10000";
            }
            return rule.RuleValue;
        }
        public List<MethodFunction> LoadAllMethodFunction()
        {
            Session.Clear();
            return this.Session.Query<MethodFunction>().ToList();
        }
        public void UpdateLastUserKey(string userKey, int skipCount)
        {
            var maxRule = GetByKey<UserKeyRule>("MAXKEY");
            maxRule.RuleValue = userKey;
            Update<UserKeyRule>(maxRule);

            var regCountRule = GetByKey<UserKeyRule>("REGCOUNT");
            if (regCountRule == null)
            {
                regCountRule = new UserKeyRule
                {
                    RuleKey = "REGCOUNT",
                    RuleValue = "1",
                };
                Add<UserKeyRule>(regCountRule);
            }
            else
            {
                regCountRule.RuleValue = (int.Parse(regCountRule.RuleValue) + 1).ToString();
                Update<UserKeyRule>(regCountRule);
            }

            var skipCountRule = GetByKey<UserKeyRule>("SKIPCOUNT");
            if (skipCountRule == null)
            {
                skipCountRule = new UserKeyRule
                {
                    RuleKey = "SKIPCOUNT",
                    RuleValue = skipCount.ToString(),
                };
                Add<UserKeyRule>(skipCountRule);
            }
            else
            {
                skipCountRule.RuleValue = (int.Parse(skipCountRule.RuleValue) + skipCount).ToString();
                Update<UserKeyRule>(skipCountRule);
            }
        }
        public void AddSkipBeautyKey(string[] userKeys, string prevKey, string nextKey)
        {
            var list = new List<BeautyUserKey>();
            foreach (var key in userKeys)
            {
                var item = new BeautyUserKey
                {
                    BeautyKey = key,
                    PrevUserKey = prevKey,
                    NextUserKey = nextKey,
                    Status = "SKIPED",
                };
                list.Add(item);
            }
            Add<BeautyUserKey>(list.ToArray());
        }

        public SysOpratorInfo_Collection GetOpratorCollection(int pageIndex, int pageSize)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_User_QueryOpratorList"))
                .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");

            var list = query.List(out outputs);
            SysOpratorInfo_Collection collection = new SysOpratorInfo_Collection();
            collection.TotalCount = (int)outputs["totalCount"];
            if (list != null && list.Count > 0)
            {
                List<string> userIds=new List<string> ();
                foreach (var item in list)
                {
                    var array = item as object[];
                    if(!userIds.Contains(array[2]))
                    {
                    SysOpratorInfo info = new SysOpratorInfo();
                    info.LoginName = Convert.ToString(array[0]);
                    info.RoleName = Convert.ToString(array[1]);
                    info.UserId = Convert.ToString(array[2]);
                    //info.RoleId = Convert.ToString(array[3]);
                    userIds.Add(info.UserId);
                    collection.OpratorListInfo.Add(info);
                    }
                }
            }
            
            return collection;
        }
        public RoleInfo_QueryCollection QueryRoleCollection()
        {
            Session.Clear();
            string strSql = "select isnull(RoleId,'')as roleId,isnull(RoleName,'')as roleName,RoleType from C_Auth_Roles";
            var result = Session.CreateSQLQuery(strSql).List();
            RoleInfo_QueryCollection collection = new RoleInfo_QueryCollection();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    RoleInfo_Query info = new RoleInfo_Query();
                    info.RoleId = Convert.ToString(array[0]);
                    info.RoleName = Convert.ToString(array[1]);
                    info.RoleType = (RoleType)Convert.ToInt32(array[2]);
                    collection.Add(info);
                }
            }
            return collection;
        }
        public string QueryUserRoleIdsByUserId(string userId)
        {
            Session.Clear();
            string strSql = "select RoleId from C_Auth_UserRole where UserId=:UserId";
            var result = Session.CreateSQLQuery(strSql)
                         .SetString("UserId",userId)
                         .List();
            StringBuilder strBud=new StringBuilder ();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    strBud.Append(item);
                    strBud.Append("%item%");
                }
            }
            return strBud.ToString();
        }
    }
}
