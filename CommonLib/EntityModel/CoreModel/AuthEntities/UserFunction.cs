using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel.AuthEntities
{
    public class UserFunction : AccessControlItem
    {
        public virtual int IId { get; set; }
        public virtual SystemUser User { get; set; }
    }
}
