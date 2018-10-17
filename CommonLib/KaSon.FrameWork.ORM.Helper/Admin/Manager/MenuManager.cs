using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper.Admin
{
    public class MenuManager:DBbase
    {
        public List<E_Menu_List> QueryMenuListByUserId(string userId)
        {
            string sql = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "Admin_QueryMenuListByUserId").SQL;
            return DB.CreateSQLQuery(sql)
                .SetString("UserId", userId)
                .List<E_Menu_List>().ToList();
        }
        public List<E_Menu_List> QueryAllMenuList()
        {
            return DB.CreateQuery<E_Menu_List>().OrderBy(p=>p.MenuId).ToList();
        }


    }
}
