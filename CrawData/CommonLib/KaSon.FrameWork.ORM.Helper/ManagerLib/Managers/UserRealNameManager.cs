using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class UserRealNameManager:DBbase
    {
        public E_Authentication_RealName GetRealNameInfoByName(string realName, string cardNumber)
        {
            var query = DB.CreateQuery<E_Authentication_RealName>().Where(s => s.RealName == realName && s.IdCardNumber == cardNumber);
            if (query != null && query.Count() > 0)
            {
                var resutl = query.Where(s => s.IsSettedRealName == true).FirstOrDefault();
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.Where(s => s.IsSettedRealName == false).FirstOrDefault();
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }

        public E_Authentication_RealName QueryUserRealName(string idCard)
        {
           
            return DB.CreateQuery<E_Authentication_RealName>().Where(p => p.IdCardNumber == idCard && p.IsSettedRealName == true).FirstOrDefault();
        }

        public void UpdateUserRealName(E_Authentication_RealName realName)
        {
            realName.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_RealName>().Update(realName);
        }

        public void AddUserRealName(E_Authentication_RealName realName)
        {
            realName.CreateTime = DateTime.Now;
            realName.UpdateTime = DateTime.Now;
            DB.GetDal<E_Authentication_RealName>().Add(realName);
        }
    }
}
