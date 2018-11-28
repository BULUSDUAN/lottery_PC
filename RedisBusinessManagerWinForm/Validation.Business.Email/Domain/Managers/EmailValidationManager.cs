using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using Validation.Email.Domain.Entities;
using NHibernate.Criterion;
using Validation.Business.Email;

namespace Validation.Email.Domain.Managers
{
    public class EmailValidationManager : ValidationEmailEntityManagement
    {
        public EmailValidation GetEmailValidation(string email, string category)
        {
            return Session.CreateCriteria<EmailValidation>()
                .Add(Restrictions.Eq("Email", email))
                .Add(Restrictions.Eq("Category", category))
                .UniqueResult<EmailValidation>();
        }
        public void AddEmailValidation(EmailValidation validation)
        {
            validation.UpdateTime = DateTime.Now;
            Add<EmailValidation>(validation);
        }
        public void UpdateEmailValidation(EmailValidation validation)
        {
            validation.UpdateTime = DateTime.Now;
            Update<EmailValidation>(validation);
        }
        public void DeleteEmailValidation(EmailValidation validation)
        {
            Delete<EmailValidation>(validation);
        }
    }
}
