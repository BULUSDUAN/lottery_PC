using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using Validation.Mobile.Domain.Entities;
using NHibernate.Criterion;
using System.Collections;
using Validation.Business.Mobile;

namespace Validation.Mobile.Domain.Managers
{
    public class MobileValidationManager : ValidationMobileEntityManagement
    {
        public MobileValidation GetMobileValidation(string mobile, string category)
        {
            return Session.CreateCriteria<MobileValidation>()
                .Add(Restrictions.Eq("Mobile", mobile))
                .Add(Restrictions.Eq("Category", category))
                .UniqueResult<MobileValidation>();
        }
        public IList<MobileValidation> QueryMobileValidationList(string mobile)
        {
            return Session.CreateCriteria<MobileValidation>()
                .Add(Restrictions.Eq("Mobile", mobile))
                .List<MobileValidation>();
        }
        public void AddMobileValidation(MobileValidation validation)
        {
            validation.UpdateTime = DateTime.Now;
            Add<MobileValidation>(validation);
        }
        public void UpdateMobileValidation(MobileValidation validation)
        {
            validation.UpdateTime = DateTime.Now;
            Update<MobileValidation>(validation);
        }
        public void DeleteMobileValidation(MobileValidation validation)
        {
            Delete<MobileValidation>(validation);
        }

        public void AddMobileValidationLog(MobileValidationLog entity)
        {
            this.Add<MobileValidationLog>(entity);
        }
    }
}
