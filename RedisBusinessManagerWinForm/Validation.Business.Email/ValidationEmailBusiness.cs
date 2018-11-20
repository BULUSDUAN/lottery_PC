using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Validation.Email.Domain.Managers;
using Validation.Email.Domain.Entities;

namespace Validation.Business.Email
{
    public class ValidationEmailBusiness
    {
        public string SendValidationCode(string email, string category, string validateCode, int delaySeconds, int maxTimesEachDay)
        {
            using (var biz = new ValidationEmailBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new EmailValidationManager())
                {
                    var validation = manager.GetEmailValidation(email, category);
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
                            throw new Exception(string.Format("发送邮件验证码与上次验证码操作至少间隔【{0}】秒，请稍等【{1}】秒后重试。", delaySeconds, span.TotalSeconds));
                        }
                        validation.SendTimes++;
                        validation.RetryTimes = 0;
                        manager.UpdateEmailValidation(validation);

                        validateCode = validation.ValidateCode;
                    }
                    else
                    {
                        validation = new EmailValidation
                        {
                            Email = email,
                            Category = category,
                            SendTimes = 1,
                            RetryTimes = 0,
                            ValidateCode = validateCode,
                        };
                        manager.AddEmailValidation(validation);
                    }
                }
                biz.CommitTran();
            }
            return validateCode;
        }
        public bool CheckValidationCode(string email, string category, string validateCode, int maxRetryTime)
        {
            bool isSuccess;
            using (var biz = new ValidationEmailBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new EmailValidationManager())
                {
                    var validation = manager.GetEmailValidation(email, category);
                    if (validation == null)
                    {
                        throw new Exception("尚未发送验证码");
                    }
                    if (validation.RetryTimes >= maxRetryTime)
                    {
                        throw new Exception(string.Format("重试次数超出最大限制次数【{0}】次，请尝试重新发送验证码。", maxRetryTime));
                    }
                    if (validation.ValidateCode == validateCode)
                    {
                        manager.DeleteEmailValidation(validation);
                        isSuccess = true;
                    }
                    else
                    {
                        validation.RetryTimes++;
                        manager.UpdateEmailValidation(validation);
                        isSuccess = false;
                    }
                }
                biz.CommitTran();
            }
            return isSuccess;
        }
        public void RefreshValidationStatus(string mobile, string category)
        {
            using (var biz = new ValidationEmailBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new EmailValidationManager())
                {
                    var validation = manager.GetEmailValidation(mobile, category);
                    if (validation == null)
                    {
                        throw new Exception("验证码不存在，可能是已经通过验证");
                    }
                    validation.SendTimes = 1;
                    validation.RetryTimes = 0;
                    manager.UpdateEmailValidation(validation);
                }
                biz.CommitTran();
            }
        }
        public void DeleteValidation(string email, string category, int delaySeconds, string delayDescription)
        {
            using (var biz = new ValidationEmailBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new EmailValidationManager())
                {
                    var validation = manager.GetEmailValidation(email, category);
                    if (validation == null)
                    {
                        throw new Exception("验证码不存在，可能是已经通过验证");
                    }
                    var span = validation.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
                    if (span.TotalSeconds > 0)
                    {
                        throw new Exception(string.Format("删除验证码必须距上次验证码操作【{0}】以上，验证码上次操作时间【{1:yyyy-MM-dd HH:mm:ss}】。", delayDescription, validation.UpdateTime));
                    }
                    manager.DeleteEmailValidation(validation);
                }
                biz.CommitTran();
            }
        }
    }
}
