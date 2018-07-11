using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
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
            return DB.CreateQuery<E_Authentication_Mobile>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        public void UpdateUserMobile(E_Authentication_Mobile entity)
        {
            DB.GetDal<E_Authentication_Mobile>().Update(entity);
        }

        public void AddUserMobile(E_Authentication_Mobile entity)
        {
            DB.GetDal<E_Authentication_Mobile>().Add(entity);
        }


        public E_Authentication_Mobile GetMobileInfoByMobile(string mobile)
        {
            var query = DB.CreateQuery<E_Authentication_Mobile>().Where(s => s.Mobile == mobile);
            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedMobile == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedMobile == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }
    }
}
