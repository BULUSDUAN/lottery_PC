using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel.AuthEntities
{
    public class RoleFunction : AccessControlItem
    {
        public virtual int IId { get; set; }
        public virtual SystemRole Role { get; set; }
    }
}
