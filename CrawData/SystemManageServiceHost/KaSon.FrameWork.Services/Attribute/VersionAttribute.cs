using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{

    /// <summary>
    /// 版本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VersionAttribute : System.Attribute
    {
        private int _interval = 1;

        /// <summary>
        /// </summary>
        /// <param name="initialValue"></param>
        public VersionAttribute(int initialValue)
        {
            this.InitialValue = initialValue;
        }

        /// <summary>
        /// 初始值
        /// </summary>
        public int InitialValue { get; private set; }

        /// <summary>
        /// 间隔数不能为0
        /// </summary>
        public int Interval
        {
            get
            {
                return this._interval;
            }
            set
            {
                if (value > 0)
                {
                    this._interval = value;
                }
            }
        }
    }
}
