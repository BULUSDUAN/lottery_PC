using EntityModel;
using EntityModel.CoreModel.AuthEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class MobileAuthenticationBusiness : DBbase
    {
        /// <summary>
        /// 手机黑名单
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C_Core_Config BanRegistrMobile(string key)
        {
            return DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
        }

        /// <summary>
        /// 查询手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public UserMobile GetMobileInfoByMobile(string mobile)
        {
            var query = DB.CreateQuery<E_Authentication_Mobile>()
                         .Where(s => s.Mobile == mobile).ToList().Select(p => new UserMobile
                         {
                             AuthFrom = p.AuthFrom,
                             CreateBy = p.CreateBy,
                             CreateTime = p.CreateTime,
                             IsSettedMobile = p.IsSettedMobile,
                             Mobile = p.Mobile

                         }).ToList();

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

        /// <summary>
        /// 根据用户ID查询手机号
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public E_Authentication_Mobile GetUserMobile(string UserId) {

            return DB.CreateQuery<E_Authentication_Mobile>().Where(p => p.UserId == UserId).FirstOrDefault();
        }



    }
}
