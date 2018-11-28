using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.KQ.Pay
{
    /// <summary>
    /// 快钱配置类
    /// </summary>
    class KQConfig
    {
        #region 字段
        //人民币网关账户号
        ///请登录快钱系统获取用户编号，用户编号后加01即为人民币网关账户号。
        private static string merchantAcctId;

        //字符集.固定选择值。可为空。
        ///只能选择1、2、3.
        ///1代表UTF-8; 2代表GBK; 3代表gb2312
        ///默认值为1
        private static string inputCharset;

        //网关版本.固定值
        ///快钱会根据版本号来调用对应的接口处理程序。
        ///本代码版本号固定为v2.0
        private static string version;

        //语言种类.固定选择值。
        ///只能选择1、2、3
        ///1代表中文；2代表英文
        ///默认值为1
        private static string language;

        //签名类型.固定值
        ///1代表MD5签名
        ///当前版本固定为1
        private static string signType;

        //商户私钥密钥
        private static string certificatePW;

        //请求充值网关接口地址
        private static string gatewayUrl_request = "https://www.99bill.com/gateway/recvMerchantInfoAction.htm";
        private static string gatewayUrl_request_test = "https://sandbox2.99bill.com/gateway/recvMerchantInfoAction.htm";

        #endregion

        #region 属性
        /// <summary>
        /// 人民币网关账户号
        /// </summary>
        /// <remarks>获取方式是：请登录快钱系统获取用户编号，用户编号后加01即为人民币网关账户号。</remarks>
        public static string MerchantAcctId
        {
            get { return merchantAcctId; }
            set { merchantAcctId = value; }
        }

        /// <summary>
        /// 字符集.固定选择值。可为空。
        /// </summary>
        /// <remarks>1代表UTF-8; 2代表GBK; 3代表gb2312，默认值为1。</remarks>
        public static string InputCharset
        {
            get { return inputCharset; }
        }

        /// <summary>
        /// 网关版本.固定值
        /// </summary>
        /// <remarks>快钱会根据版本号来调用对应的接口处理程序。本代码版本号固定为v2.0</remarks>
        public static string Version
        {
            get { return version; }
        }

        /// <summary>
        /// 语言种类.固定选择值。
        /// </summary>
        /// <remarks>1代表中文；2代表英文,默认值为1。</remarks>
        public static string Language
        {
            get { return language; }
        }

        /// <summary>
        /// 签名类型.固定值。
        /// </summary>
        /// <remarks>1代表MD5签名，当前版本固定为1</remarks>
        public static string SignType
        {
            get { return signType; }
        }

        /// <summary>
        /// 商户私钥密钥
        /// </summary>
        public static string CertificatePW
        {
            get { return certificatePW; }
            set { certificatePW = value; }
        }

        /// <summary>
        /// 请求充值网关地址
        /// </summary>
        public static string Gateway_Request(bool isTest)
        {
            return isTest ? gatewayUrl_request_test : gatewayUrl_request;
        }
        #endregion

        /// <summary>
        /// 初始化参数
        /// </summary>
        static KQConfig()
        {
            merchantAcctId = "1001181342501"; //abby_7796测试账号 1001181342501
            certificatePW = "abby_7796"; //测试密钥 abby_7796
            inputCharset = "1";
            version = "v2.0";
            language = "1";
            signType = "1";
        }
    }
}
