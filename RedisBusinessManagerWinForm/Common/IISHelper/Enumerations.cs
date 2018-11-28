using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.IISHelper
{
    /// <summary>
    /// 服务器IIS版本
    /// </summary>
    [Serializable]
    public enum IISVersion
    {
        /// <summary>
        /// 未安装
        /// </summary>
        None = 0,
        /// <summary>
        /// 未知
        /// </summary>
        Unkonw = 1,
        /// <summary>
        /// IIS 5.0,5.1
        /// </summary>
        IIS5 = 5,
        /// <summary>
        /// IIS 6.0
        /// </summary>
        IIS6 = 6,
        /// <summary>
        /// IIS 7.0
        /// </summary>
        IIS7 = 7,
        /// <summary>
        /// IIS 8.0
        /// </summary>
        IIS8 = 8,
    }
}
