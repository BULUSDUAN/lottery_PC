using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Validation.Mobile.Domain.Managers;
using Validation.Mobile.Domain.Entities;
using System.Collections;

namespace Validation.Business.Mobile
{
    public class ValidationMobileBusiness
    {
        public string SendValidationCode(string mobile, string category, string validateCode, int delaySeconds, int maxTimesEachDay)
        {
            using (var biz = new ValidationMobileBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new MobileValidationManager())
                {
                    var validation = manager.GetMobileValidation(mobile, category);
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
                        manager.UpdateMobileValidation(validation);

                        validateCode = validation.ValidateCode;
                    }
                    else
                    {
                        validation = new MobileValidation
                        {
                            Mobile = mobile,
                            Category = category,
                            SendTimes = 1,
                            RetryTimes = 0,
                            ValidateCode = validateCode,
                        };
                        manager.AddMobileValidation(validation);
                    }
                }
                biz.CommitTran();
            }
            return validateCode;
        }
        public bool CheckValidationCode(string mobile, string category, string validateCode, int maxRetryTime)
        {
            bool isSuccess;
            using (var biz = new ValidationMobileBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new MobileValidationManager())
                {
                    var validation = manager.GetMobileValidation(mobile, category);
                    if (validation == null)
                    {
                        throw new Exception("尚未发送验证码");
                    }

                    manager.AddMobileValidationLog(new MobileValidationLog
                    {
                        CreateTime = DateTime.Now,
                        DB_ValidateCode = validation.ValidateCode,
                        Mobile = mobile,
                        User_ValidateCode = validateCode,
                    });

                    if (validation.RetryTimes >= maxRetryTime)
                    {
                        throw new Exception(string.Format("重试次数超出最大限制次数【{0}】次，请尝试重新发送验证码。", maxRetryTime));
                    }
                    if (validation.ValidateCode == validateCode)
                    {
                        manager.DeleteMobileValidation(validation);
                        isSuccess = true;
                    }
                    else
                    {
                        validation.RetryTimes++;
                        manager.UpdateMobileValidation(validation);
                        isSuccess = false;
                    }
                }
                biz.CommitTran();
            }
            return isSuccess;
        }
        public void RefreshValidationStatus(string mobile, string category)
        {
            using (var biz = new ValidationMobileBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new MobileValidationManager())
                {
                    var validation = manager.GetMobileValidation(mobile, category);
                    if (validation == null)
                    {
                        throw new Exception("验证码不存在，可能是已经通过验证");
                    }
                    validation.SendTimes = 1;
                    validation.RetryTimes = 0;
                    manager.UpdateMobileValidation(validation);
                }
                biz.CommitTran();
            }
        }
        public void DeleteValidation(string mobile, string category, int delaySeconds, string delayDescription)
        {
            using (var biz = new ValidationMobileBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new MobileValidationManager())
                {
                    var validation = manager.GetMobileValidation(mobile, category);
                    if (validation == null)
                    {
                        throw new Exception("验证码不存在，可能是已经通过验证");
                    }
                    var span = validation.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
                    if (span.TotalSeconds > 0)
                    {
                        throw new Exception(string.Format("删除验证码必须距上次验证码操作【{0}】以上，验证码上次操作时间【{1:yyyy-MM-dd HH:mm:ss}】。", delayDescription, validation.UpdateTime));
                    }
                    manager.DeleteMobileValidation(validation);
                }
                biz.CommitTran();
            }
        }

        public IList<MobileValidation> QueryValidationMobileByMobile(string mobile)
        {
            using (var manager = new MobileValidationManager())
            {
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    throw new Exception("手机号码不能为空");
                }
                return manager.QueryMobileValidationList(mobile);
            }
        }
    }
}
