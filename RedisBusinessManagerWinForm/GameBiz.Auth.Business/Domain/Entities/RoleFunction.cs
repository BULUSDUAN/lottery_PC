using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace GameBiz.Auth.Domain.Entities
{
    public class RoleFunction : AccessControlItem
    {
        public virtual int IId { get; set; }
        public virtual SystemRole Role { get; set; }
    }
}
