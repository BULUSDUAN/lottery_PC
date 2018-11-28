using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation.Email.Domain.Entities
{
    public class EmailValidation
    {
        public virtual long Id { get; set; }
        public virtual string Email { get; set; }
        public virtual string Category { get; set; }
        public virtual string ValidateCode { get; set; }
        public virtual int SendTimes { get; set; }
        public virtual int RetryTimes { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
