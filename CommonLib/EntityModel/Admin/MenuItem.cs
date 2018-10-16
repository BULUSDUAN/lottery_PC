using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    public class MenuItem
    {
        public string MenuId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public MenuItem ParentMenu { get; set; }
        public Function ItsFunction { get; set; }
        public string Url { get; set; }
        public MenuType MenuType { get; set; }
        public bool IsEnable { get; set; }
    }
}
