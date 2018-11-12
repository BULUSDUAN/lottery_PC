using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public class ConnInfo
    {
        /// <summary>
        /// 命令执行超时时间(秒)
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 构造工厂
        /// </summary>
        public string FactoryType { get; set; }

        /// <summary>
        /// 数据库提供者
        /// </summary>
        public ProviderInfo Provider { get; set; }
    }
}
