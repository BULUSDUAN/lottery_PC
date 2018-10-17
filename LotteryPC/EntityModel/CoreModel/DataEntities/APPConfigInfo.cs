using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    [Serializable]
    public class APPConfigInfo
    {
        /// <summary>
        /// 代理商编码
        /// </summary>
        /// 
        [ProtoMember(1)]
        public string AppAgentId { get; set; }
        /// <summary>
        /// 代理商名称
        /// </summary>
        /// 
        [ProtoMember(2)]
        public string AgentName { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        /// 
        [ProtoMember(3)]
        public string ConfigName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        /// 
        [ProtoMember(4)]
        public string ConfigVersion { get; set; }
        /// <summary>
        /// 升级内容
        /// </summary>
        /// 
        [ProtoMember(5)]
        public string ConfigUpdateContent { get; set; }
        /// <summary>
        /// 升级地址
        /// </summary>
        /// 
        [ProtoMember(6)]
        public string ConfigDownloadUrl { get; set; }
        /// <summary>
        /// 升级编码
        /// </summary>
        /// 
        [ProtoMember(7)]
        public string ConfigCode { get; set; }
        /// <summary>
        /// 是否自动升级
        /// </summary>
        /// 
        [ProtoMember(8)]
        public bool IsForcedUpgrade { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        /// 
        [ProtoMember(9)]
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

    [ProtoContract]
    [Serializable]
    public class NestedUrlConfig_Collection
    {
        public NestedUrlConfig_Collection()
        {
            NestedUrlList = new List<NestedUrlConfigInfo>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<NestedUrlConfigInfo> NestedUrlList { get; set; }
    }
}
