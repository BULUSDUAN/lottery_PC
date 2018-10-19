using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class AdminMenuBusiness
    {
        public List<E_Menu_List> QueryAllMenuList()
        {
            var menuManager = new MenuManager();
            var list= menuManager.QueryAllMenuList().Where(s => (s.MenuType == (int)MenuType.Web_Menu || s.MenuType == (int)MenuType.All) && s.IsEnable == true).ToList();
            return list;
        }
        public List<E_Menu_List> QueryMenuListByUserId(string userId)
        {
            var menuManager = new MenuManager();
            var list= menuManager.QueryMenuListByUserId(userId);
            return list;
        }

        public List<C_Auth_Function_List> QueryLowerLevelFuncitonList()
        {
            var manager = new MenuManager();
            return manager.QueryLowerLevelFuncitonList();   
        }
    }
}
