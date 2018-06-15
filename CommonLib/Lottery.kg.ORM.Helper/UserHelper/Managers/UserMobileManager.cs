using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class UserMobileManager:DBbase
    {
        /// <summary>
        /// 查询手机号
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public E_Authentication_Mobile GetUserMobile(string userId)
        {
            return DB.CreateQuery<E_Authentication_Mobile>().FirstOrDefault(p => p.UserId == userId);
        }

        public void UpdateUserMobile(E_Authentication_Mobile entity)
        {
            DB.GetDal<E_Authentication_Mobile>().Update(entity);
        }

        public void AddUserMobile(E_Authentication_Mobile entity)
        {
            DB.GetDal<E_Authentication_Mobile>().Add(entity);
        }
    }
}
