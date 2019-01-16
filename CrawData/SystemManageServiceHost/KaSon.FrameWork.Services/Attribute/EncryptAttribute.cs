using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{
    public class MultFieldAttribute : System.Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="property"></param>
        public MultFieldAttribute(Type entityType, string property)
        {
            this.EntityType = entityType;
            this.Name = property;
        }

        /// <summary>
        /// 实体名称
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; private set; }
    }
}
