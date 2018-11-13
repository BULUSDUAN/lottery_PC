using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    /// <summary>
    /// 配置信息
    /// </summary>
    [Serializable]
    public class OrmConfigInfo
    {
        /// <summary>
        /// 命令超时时间
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// DbKey
        /// </summary>
        public string DbKey { get; set; }

        public bool OpenMinLink { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public string FactoryType { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public ProviderInfo Provider { get; set; }
    }
}
