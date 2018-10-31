using EntityModel;
using System;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserAlipayManager : DBbase
    {
        public E_Authentication_Alipay GetUserAlipay(string userId)
        {
            return DB.CreateQuery<E_Authentication_Alipay>().Where(x=>x.UserId==userId).FirstOrDefault();
        }
        public E_Authentication_Alipay GetUserAlipayByAccount(string alipay)
        {
            return DB.CreateQuery<E_Authentication_Alipay>().Where(p => p.AlipayAccount == alipay).FirstOrDefault();
        }
        public void UpdateUserAlipay(E_Authentication_Alipay entity)
        {
            entity.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_Alipay>().Update(entity);
        }
        public void AddUserAlipay(E_Authentication_Alipay entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_Alipay>().Add(entity);
        }
        public void DeleteUserAlipay(E_Authentication_Alipay entity)
        {
            entity.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_Alipay>().Delete(entity);
        }
    }
}
