using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{
    /// <summary>
    /// Oracle序列号
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SequenceAttribute : System.Attribute
    {
        public SequenceAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { get; private set; }
    }
}
