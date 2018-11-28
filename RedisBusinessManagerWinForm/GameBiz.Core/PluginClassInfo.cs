using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    /// <summary>
    /// 插件对象
    /// </summary>
    [CommunicationObject]
    public class PluginClassInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 接口名
        /// </summary>
        public string InterfaceName { get; set; }
        /// <summary>
        /// 组件文件名
        /// </summary>
        public string AssemblyFileName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 排序索引
        /// </summary>
        public int OrderIndex { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }

    [CommunicationObject]
    public class PluginClassInfo_Collection
    {
        public PluginClassInfo_Collection()
        {
            PluginClassList = new List<PluginClassInfo>();
        }
        public int TotalCount { get; set; }
        public List<PluginClassInfo> PluginClassList { get; set; }
    }
}
