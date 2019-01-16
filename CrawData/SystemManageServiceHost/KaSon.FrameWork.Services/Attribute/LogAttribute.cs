namespace KaSon.FrameWork.Services
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class LogAttribute : System.Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public LogAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 实体对应的表名
        /// </summary>
        public string Name { get; private set; }
    }
}

