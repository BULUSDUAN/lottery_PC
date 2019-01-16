using KaSon.FrameWork.ORM.Factory;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class MultFieldInfo
    {
        public string FieldName { get; set; }

        public PropertyInfo Info { get; set; }

        public string TableName { get; set; }
    }
}
