using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
   public class UserManager:DBbase
    {
        public C_Auth_Users LoadUser(string userId)
        {
            return DB.CreateQuery<C_Auth_Users>().Where(p => p.UserId == userId).FirstOrDefault();
        }
        public IList<C_Auth_Roles> GetRoleListByIds(string[] roleIds)
        {
            var list = new List<C_Auth_Roles>();
            foreach (var roleId in roleIds)
            {
                list.Add(LoadRole(roleId));
            }
            return list;
        }

        public C_Auth_Roles LoadRole(string roleId)
        {
            return DB.CreateQuery<C_Auth_Roles>().Where(p => p.RoleId == roleId).FirstOrDefault();
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

        public SysOpratorInfo_Collection GetOpratorCollection(int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            string sql = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "Admin_GetOpratorCollection").SQL;
            List<SysOpratorInfo> list= DB.CreateSQLQuery(sql)
                .SetInt("PageIndex", pageIndex)
                .SetInt("PageSize", pageSize)
                .List<SysOpratorInfo>().ToList();
            var result = new SysOpratorInfo_Collection();
            result.OpratorListInfo = list;
            var count = DB.CreateQuery<C_Auth_Users>().Where(p => p.RegFrom == "INNER").Count();
            result.TotalCount = count;
            return result;
        }

        public List<C_Auth_Roles> QueryRoleCollection()
        {
            var result = DB.CreateQuery<C_Auth_Roles>().ToList();
            return result;
        }


        public List<C_Auth_UserRole> QueryAuthUserRoleByUserId(string UserId)
        {
            var result = DB.CreateQuery<C_Auth_UserRole>().Where(p => p.UserId == UserId).ToList();
            return result;
        }
    }
}
