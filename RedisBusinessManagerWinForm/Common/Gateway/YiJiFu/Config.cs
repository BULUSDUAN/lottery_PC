using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.YiJiFu
{
    /// <summary>
    ///配置类
    /// </summary>
    class Config
    {
        #region 字段
        private static string _partnerId = "";//商户ID，合作身份者ID，合作伙伴ID
        private static string _key = "";//商户密钥
        private static string _signType = "MD5"; //加密方式
        private static string _gateway = "http://openapi.yiji.com/gateway.html?"; //接口地址
        private static string _gateway_test = "http://218.70.82.178:8630/gateway.html?"; //接口地址-测试
        private static string _input_charset = "UTF-8";//编码类型
        private static bool _istest = false; //是否为测试，true表示当前处于测试状态，测试状态下使用测试地址，默认为非测试
        #endregion

        #region 属性
        /// <summary>
        /// 商户ID，合作身份者ID，合作伙伴ID
        /// </summary>
        /// <remarks>由易极付提供</remarks>
        public static string PatnerId
        {
            get { return _partnerId; }
            set { _partnerId = value; }
        }

        /// <summary>
        /// 商户密钥
        /// </summary>
        /// <remarks>由易极付提供的商户密钥</remarks>
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// 是否为测试状态
        /// </summary>
        /// <remarks>由易极付提供的商户密钥</remarks>
        public static bool IsTest
        {
            get { return _istest; }
            set { _istest = value; }
        }

        /// <summary>
        /// 加密方式
        /// </summary>
        /// <remarks>加密方式，默认为MD5</remarks>
        public static string SignType
        {
            get { return _signType; }
            set { _signType = value; }
        }

        /// <summary>
        /// 网关地址
        /// </summary>
        /// <remarks>网关地址，自动根据是否为测试状态返回正式或测试网站</remarks>
        public static string GateWay
        {
            get { return IsTest ? _gateway_test : _gateway; }
        }

        /// <summary>
        /// 编码类型
        /// </summary>
        /// <remarks>字符串编码类型</remarks>
        public static string Input_Charset
        {
            get { return _input_charset; }
            set { _input_charset = value; }
        }
        #endregion

    }
}
