using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.YinBao
{
    /// <summary>
    ///配置类
    /// </summary>
    class Config
    {
        #region 字段
        private static int p12_ver = 1; //版本号，默认为1
        private static string p3_bn = "";//商户ID
        private static string key = "";//安全校验码
        #endregion

        /// <summary>
        /// 商户ID，合作身份者ID，合作伙伴ID
        /// </summary>
        /// <remarks>在签约后可以登录网站获取</remarks>
        public static string Partner
        {
            get { return p3_bn; }
            set { p3_bn = value; }
        }

        /// <summary>
        /// 安全校验码，与partner是一组的
        /// </summary>
        /// <remarks>在签约后可以登录网站获取</remarks>
        public static string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 版本号，默认为1
        /// </summary>
        /// <remarks>可传值，也可不传值</remarks>
        public static int Version
        {
            get { return p12_ver; }
            set { p12_ver = value; }
        }

    }
}
