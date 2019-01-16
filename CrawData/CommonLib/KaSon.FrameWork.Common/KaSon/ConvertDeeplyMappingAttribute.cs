using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common
{
    /// <summary>
    /// 深度转换对象映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConvertDeeplyMappingAttribute : Attribute
    {
        /// <summary>
        /// 映射路径。如"User.Class.Name"表示User属性的Class属性的Name属性
        /// </summary>
        public string MappingName { get; set; }
    }

}
