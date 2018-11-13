using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    public class ColumnMapping
    {
        public string DataTableColumnName { get; set; }

        public string DBColumnName { get; set; }
    }
}
