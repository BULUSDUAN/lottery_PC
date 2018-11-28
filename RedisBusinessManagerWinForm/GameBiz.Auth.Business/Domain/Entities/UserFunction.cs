using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace GameBiz.Auth.Domain.Entities
{
    public class UserFunction : AccessControlItem
    {
        public virtual int IId { get; set; }
        public virtual SystemUser User { get; set; }
    }
}
