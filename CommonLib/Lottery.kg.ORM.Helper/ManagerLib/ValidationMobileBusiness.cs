using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
    public class ValidationMobileBusiness:DBbase
    {
        public string SendValidationCode(string mobile, string category, string validateCode, int delaySeconds, int maxTimesEachDay)
        {

            DB.Begin();
            var validation = DB.CreateQuery<E_Validation_Mobile>().Where(p => p.Mobile == mobile && p.Category == category).FirstOrDefault();
            if (validation != null)
            {
                if (DateTime.Today == validation.UpdateTime.Date)
                {
                    if (validation.SendTimes >= maxTimesEachDay)
                    {
                        throw new Exception(string.Format("今天已发送每天允许的最大限制次数【{0}】次，请明天再试。", maxTimesEachDay));
                    }
                }
                var span = validation.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    throw new Exception(string.Format("发送短信验证码与上次验证码操作至少间隔【{0}】秒，请稍等【{1}】秒后重试。", delaySeconds, (int)span.TotalSeconds + 1));
                }
                validation.SendTimes++;
                validation.RetryTimes = 0;
                validation.UpdateTime = DateTime.Now;
                DB.GetDal<E_Validation_Mobile>().Update(validation);


                validateCode = validation.ValidateCode;
            }
            else
            {
                validation = new E_Validation_Mobile
                {
                    Mobile = mobile,
                    Category = category,
                    SendTimes = 1,
                    RetryTimes = 0,
                    ValidateCode = validateCode,
                    UpdateTime = DateTime.Now
                };
                DB.GetDal<E_Validation_Mobile>().Add(validation);
            }

            DB.Commit();

            return validateCode;
        }

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
            var MobileValidationLog = new E_Validation_Mobile_Log
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
