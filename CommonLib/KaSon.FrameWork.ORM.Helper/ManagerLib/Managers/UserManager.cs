using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
   public class UserManager:DBbase
    {
        public C_Auth_Users LoadUser(string userId)
        {
            return DB.CreateQuery<C_Auth_Users>().FirstOrDefault(p => p.UserId == userId);
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
    }
}
