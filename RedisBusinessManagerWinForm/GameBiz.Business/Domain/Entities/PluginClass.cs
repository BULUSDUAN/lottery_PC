using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 插件类
    /// </summary>
    public class PluginClass
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public virtual string ClassName { get; set; }
        /// <summary>
        /// 接口名
        /// </summary>
        public virtual string InterfaceName { get; set; }
        /// <summary>
        /// 组件文件名
        /// </summary>
        public virtual string AssemblyFileName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 排序索引
        /// </summary>
        public virtual int OrderIndex { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual DateTime? EndTime { get; set; }
    }
}
