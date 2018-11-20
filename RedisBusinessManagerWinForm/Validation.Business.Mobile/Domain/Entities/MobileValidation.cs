using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation.Mobile.Domain.Entities
{
    public class MobileValidation
    {
        public virtual long Id { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Category { get; set; }
        public virtual string ValidateCode { get; set; }
        public virtual int SendTimes { get; set; }
        public virtual int RetryTimes { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

    public class MobileValidationLog
    {
        public virtual long Id { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string DB_ValidateCode { get; set; }
        public virtual string User_ValidateCode { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
