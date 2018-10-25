using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
   public class UserManager:DBbase
    {
        public C_Auth_Users LoadUser(string userId)
        {
             return DB.CreateQuery<C_Auth_Users>().Where(p => p.UserId == userId).FirstOrDefault();
        }
        public SystemUser LoadSystemUser(string userId)
        {
            var User = (from a in DB.CreateQuery<C_Auth_Users>()
                        where a.UserId == userId
                        select new SystemUser()
                        {
                            UserId=a.UserId,
                            RegFrom=a.RegFrom,
                            CreateTime=a.CreateTime,
                            AgentId=a.AgentId,
                            RoleList=new List<SystemRole>(),
                            FunctionList=new List<UserFunction>()
                        }).FirstOrDefault();
            if (User != null)
            {
                User.RoleList = (from d in DB.CreateQuery<C_Auth_Roles>()
                                 join c in DB.CreateQuery<C_Auth_UserRole>()
                                 on d.RoleId equals c.RoleId
                                 where c.UserId == userId
                                 select new SystemRole()
                                 {
                                     IsAdmin = d.IsAdmin,
                                     IsInner = d.IsInner,
                                     RoleId = d.RoleId,
                                     RoleName = d.RoleName,
                                     RoleType = (RoleType)d.RoleType
                                 }).ToList();
                User.FunctionList= (from b in DB.CreateQuery<C_Auth_UserFunction>()
                                    join d in DB.CreateQuery<C_Auth_UserRole>()

                                    on b.UserId equals d.UserId
                                    where d.UserId == userId
                                    select new UserFunction()
                                    {
                                        FunctionId = b.FunctionId,
                                        IId = b.IId,
                                        Mode = b.Mode,


                                    }).ToList();

            }
            return User;
        }
        public IList<C_Auth_Users> GetRoleListByIds(string[] roleIds)
        {
            var list = new List<C_Auth_Users>();
            foreach (var roleId in roleIds)
            {
                list.Add(LoadUser(roleId));
            }
            return list;
        }

        public void UpdateSystemUser(C_Auth_Users user)
        {
          
            DB.GetDal<C_Auth_Users>().Update(user);
        }
        public string QueryUserRoleIdsByUserId(string userId)
        {
           
            string strSql = "select RoleId from C_Auth_UserRole where UserId=:UserId";
            var result = DB.CreateSQLQuery(strSql)
                         .SetString("UserId", userId)
                         .List<C_Auth_UserRole>();
            StringBuilder strBud = new StringBuilder();
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

        public List<MethodFunction> LoadAllMethodFunction()
        {
            return DB.CreateQuery<MethodFunction>().ToList();
        }
    }
}
