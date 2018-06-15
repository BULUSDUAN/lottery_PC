using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public class APPConfigInfo
    {
        /// <summary>
        /// 代理商编码
        /// </summary>
        public string AppAgentId { get; set; }
        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string ConfigVersion { get; set; }
        /// <summary>
        /// 升级内容
        /// </summary>
        public string ConfigUpdateContent { get; set; }
        /// <summary>
        /// 升级地址
        /// </summary>
        public string ConfigDownloadUrl { get; set; }
        /// <summary>
        /// 升级编码
        /// </summary>
        public string ConfigCode { get; set; }
        /// <summary>
        /// 是否自动升级
        /// </summary>
        public bool IsForcedUpgrade { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string ConfigExtended { get; set; }
    }
    public class APPConfig_Collection
    {
        public APPConfig_Collection()
        {
            AppConfigList = new List<APPConfigInfo>();
        }
        public int TotalCount { get; set; }
        public List<APPConfigInfo> AppConfigList { get; set; }
    }

    /// <summary>
    /// 配置APP嵌套地址
    /// </summary>
    public class NestedUrlConfigInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Key值
        /// </summary>
        public string ConfigKey { get; set; }
        /// <summary>
        /// 嵌套地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public UrlType UrlType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    public class NestedUrlConfig_Collection
    {
        public NestedUrlConfig_Collection()
        {
            NestedUrlList = new List<NestedUrlConfigInfo>();
        }
        public int TotalCount { get; set; }
        public List<NestedUrlConfigInfo> NestedUrlList { get; set; }
    }
}
