using EntityModel;
using System;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserQQManager : DBbase
    {
        public E_Authentication_QQ GetUserQQ(string userId)
        {
            return DB.CreateQuery<E_Authentication_QQ>().Where(x=>x.UserId==userId).FirstOrDefault();
        }
    }
}
