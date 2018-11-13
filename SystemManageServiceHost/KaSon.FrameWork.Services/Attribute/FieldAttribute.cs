using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute : System.Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public FieldAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 是否能空
        /// </summary>
        public bool CanNull { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否默认值
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsIdenty { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 精度，限数值，小数点后面位数
        /// </summary>
        public int Precision { get; set; }
    }
}
