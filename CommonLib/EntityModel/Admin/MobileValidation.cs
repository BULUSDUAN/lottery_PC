using System;

namespace EntityModel
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
}