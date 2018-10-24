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
    }
}
