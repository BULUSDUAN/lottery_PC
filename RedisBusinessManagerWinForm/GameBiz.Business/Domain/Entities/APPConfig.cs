using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class APPConfig
    {
        /// <summary>
        /// 代理商编码
        /// </summary>
        public virtual string AppAgentId { get; set; }
        /// <summary>
        /// 代理商名称
        /// </summary>
        public virtual string AgentName { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        public virtual string ConfigName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public virtual string ConfigVersion { get; set; }
        /// <summary>
        /// 升级内容
        /// </summary>
        public virtual string ConfigUpdateContent { get; set; }
        /// <summary>
        /// 升级地址
        /// </summary>
        public virtual string ConfigDownloadUrl { get; set; }
        /// <summary>
        /// 升级编码
        /// </summary>
        public virtual string ConfigCode { get; set; }
        /// <summary>
        /// 是否自动升级
        /// </summary>
        public virtual bool IsForcedUpgrade { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public virtual string ConfigExtended { get; set; }
    }
    /// <summary>
    /// 配置APP嵌套地址
    /// </summary>
    public class NestedUrlConfig
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// Key值
        /// </summary>
        public virtual string ConfigKey { get; set; }
        /// <summary>
        /// 嵌套地址
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public virtual UrlType UrlType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get;set; }
    }
}
