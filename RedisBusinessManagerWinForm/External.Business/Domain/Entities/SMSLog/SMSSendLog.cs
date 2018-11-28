using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SMSLog
{
    public class SMSSendLog
    {
        public virtual long Id { get; set; }
        public virtual string KeyLine { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
