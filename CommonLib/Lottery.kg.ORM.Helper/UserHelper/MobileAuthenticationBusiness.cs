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

        #region  检查手机号是否认证过
        public bool IsMobileAuthenticated(string mobile)
        {
           
                var other = GetOtherUserMobile(mobile, "");
                return (other != null);
           
        }

        public E_Authentication_Mobile GetOtherUserMobile(string mobile, string userId)
        {
           
            var list = DB.CreateQuery<E_Authentication_Mobile>()
                .Where(p=>p.Mobile==mobile && p.UserId==userId).ToList();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];

        }
        #endregion

        public bool CheckValidationCode(string mobile, string category, string validateCode, int maxRetryTime)
        {
                   bool isSuccess;

                    DB.Begin();

                    //查询是否发送过验证码
                   var validation = DB.CreateQuery<E_Validation_Mobile>().Where(p => p.Mobile == mobile && p.Category == category).FirstOrDefault();
                    if (validation == null)
                    {
                        throw new Exception("尚未发送验证码");
                    }

                    //新增发送短信的记录
                    var MobileValidationLog=new E_Validation_Mobile_Log
                    {
                        CreateTime = DateTime.Now,
                        DB_ValidateCode = validation.ValidateCode,
                        Mobile = mobile,
                        User_ValidateCode = validateCode,
                    };
                     DB.GetDal<E_Validation_Mobile_Log>().Add(MobileValidationLog);

                    if (validation.RetryTimes >= maxRetryTime)
                    {
                        throw new Exception(string.Format("重试次数超出最大限制次数【{0}】次，请尝试重新发送验证码。", maxRetryTime));
                    }
                    if (validation.ValidateCode == validateCode)
                    {
                     DB.GetDal<E_Validation_Mobile>().Delete(validation);
                        isSuccess = true;
                    }
                    else
                    {
                        validation.RetryTimes++;
                        validation.UpdateTime = DateTime.Now;
                        DB.GetDal<E_Validation_Mobile>().Update(validation);
                        isSuccess = false;
                    }
                
            DB.Commit();
            
            return isSuccess;
        }

    }
}
