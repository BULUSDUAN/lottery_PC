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
        public void UpdateUserQQ(E_Authentication_QQ userQQ)
        {
            userQQ.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_QQ>().Update(userQQ);
        }
        public void AddUserQQ(E_Authentication_QQ userQQ)
        {
            userQQ.CreateTime = DateTime.Now;
            userQQ.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_QQ>().Add(userQQ);
        }
        public void DeleteUserQQ(E_Authentication_QQ userQQ)
        {
            userQQ.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_QQ>().Delete(userQQ);
        }
    }
}
