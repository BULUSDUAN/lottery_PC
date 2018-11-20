using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using External.Core;

namespace External.Domain.Entities.AdminMenu
{
    public class MenuItem
    {
        public virtual string MenuId { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual MenuItem ParentMenu { get; set; }
        public virtual Function ItsFunction { get; set; }
        public virtual string Url { get; set; }
        public virtual MenuType MenuType { get; set; }
        public virtual bool IsEnable { get; set; }
    }
}
