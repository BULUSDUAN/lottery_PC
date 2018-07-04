using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class UserManager:DBbase
    {
        public C_Auth_Users LoadUser(string userId)
        {
            return DB.CreateQuery<C_Auth_Users>().FirstOrDefault(p => p.UserId == userId);
        }
    }
}
