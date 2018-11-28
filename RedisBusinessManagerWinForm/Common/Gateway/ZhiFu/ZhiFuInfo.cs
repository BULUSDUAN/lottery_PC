using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.ZhiFu
{
    public class ZhiFuInfo
    {
    }
    public class ZhiFuParamInfo
    {
        #region 支付请求参数

        /// <summary>
        /// 商家号_必选
        /// </summary>
        public string merchant_code { get; set; }
        /// <summary>
        /// 业务类型_必须_固定值:direct_pay
        /// </summary>
        public string service_type { get; set; }
        /// <summary>
        /// 支付类型_可选_必须小写，多选时请用逗号隔开(b2c,plateform,dcard,express)
        /// </summary>
        public string pay_type { get; set; }
        /// <summary>
        /// 参数编码字符集_必选_取值：UTF-8、GBK(必须大写)
        /// </summary>
        public string input_charset { get; set; }
        /// <summary>
        /// 服务器异步通知地址_必选
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 页面跳转同步通知地址_可选
        /// </summary>
        public string return_url { get; set; }
        /// <summary>
        /// 客户端IP_可选
        /// </summary>
        public string client_ip { get; set; }
        /// <summary>
        /// 接口版本_必选_固定值：V3.0(必须大写)
        /// </summary>
        public string interface_version { get; set; }
        /// <summary>
        /// 签名方式_必选_默认值：MD5，目前仅支持
        /// </summary>
        public string sign_type { get; set; }
        /// <summary>
        /// 商户网站唯一订单号_必选
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 商户订单时间_必选_格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime order_time { get; set; }
        /// <summary>
        /// 商户订单总金额_必选_精确到小数点后两位。举例：12.01
        /// </summary>
        public decimal order_amount { get; set; }
        /// <summary>
        /// 商品名称_必选
        /// </summary>
        public string product_name { get; set; }
        /// <summary>
        /// 商品展示URL_可选
        /// </summary>
        public string show_url { get; set; }
        /// <summary>
        /// 商品编号_可选
        /// </summary>
        public string product_code { get; set; }
        /// <summary>
        /// 商品数量_可选_商品数量，必须是数字
        /// </summary>
        public int product_num { get; set; }
        /// <summary>
        /// 商品描述_可选_商品描述，不超过300个字符
        /// </summary>
        public string product_desc { get; set; }
        /// <summary>
        /// 银行代码_可选
        /// </summary>
        public string bank_code { get; set; }
        /// <summary>
        /// 公用回传参数_可选
        /// </summary>
        public string extra_return_param { get; set; }
        /// <summary>
        /// 公用业务扩展参数_可选_格式： 格式： 格式： 参数名 1^ 参数值 |参数 名 2^ 参数值 2 说明 ：多条数据间用 "|""|""|"间隔 举例： 举例： name^name^name^name^name^张三 |sex^ sex^sex^男
        /// </summary>
        public string extend_param { get; set; }
        /// <summary>
        /// 是否允许重复订单_可选_当值为 1时不允许商 户订单号重复提交 ；当值为 0或空时允许商户订单号重复 提交
        /// </summary>
        public int redo_flag { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string ZhiFuKey { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string PayUrl { get; set; }

        #endregion
    }
    /// <summary>
    /// 支付回调参数;网站、IOS、安卓
    /// </summary>
    public class ZhiFuCallBackParamInfo
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_code { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public string notify_type { get; set; }
        /// <summary>
        /// 通知校验Id
        /// </summary>
        public string notify_id { get; set; }
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string interface_version { get; set; }
        /// <summary>
        /// 签名方式
        /// </summary>
        public string sign_type { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 商户网站唯一订单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 商户订单时间_格式：yyyy-MM-dd HH:mm:ss，举例：2013-11-01 12:34:54。
        /// </summary>
        public DateTime order_time { get; set; }
        /// <summary>
        /// 商户订单总金额_以元为单位，精确到小数点后两位，举例：12.01
        /// </summary>
        public decimal order_amount { get; set; }
        /// <summary>
        /// 公用回传参数
        /// </summary>
        public string extra_return_param { get; set; }
        /// <summary>
        /// 智付交易订单号
        /// </summary>
        public string trade_no { get; set; }
        /// <summary>
        /// 智付交易订单时间_格式为：yyyy-MM-dd HH:mm:ss，举例：2013-12-01 12:23:34
        /// </summary>
        public DateTime trade_time { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        public string trade_status { get; set; }
        /// <summary>
        /// 支付银行代码
        /// </summary>
        public string bank_code { get; set; }
        /// <summary>
        /// 网银交易流水号
        /// </summary>
        public string bank_seq_no { get; set; }
        /// <summary>
        /// 加密参数
        /// </summary>
        public string signData { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string ZhiFuKey { get; set; }
    }

    #region 订单查询

    public class response
    {
        /// <summary>
        /// 查询是否成功_必选_T 代表成功；F 代表失败
        /// </summary>
        public string is_success { get; set; }
        /// <summary>
        /// 签名方式_必选_默认值：MD5，目前仅支持MD5
        /// </summary>
        public string sign_type { get; set; }
        /// <summary>
        /// 签名_必选
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 错误码_可选
        /// </summary>
        public string error_code { get; set; }
        public trade trade { get; set; }
    }


    //public class ZhiFuOrderQueryInfo
    //{
    //    /// <summary>
    //    /// 查询是否成功_必选_T 代表成功；F 代表失败
    //    /// </summary>
    //    public string is_success { get; set; }
    //    /// <summary>
    //    /// 签名方式_必选_默认值：MD5，目前仅支持MD5
    //    /// </summary>
    //    public string sign_type { get; set; }
    //    /// <summary>
    //    /// 签名_必选
    //    /// </summary>
    //    public string sign { get; set; }
    //    /// <summary>
    //    /// 错误码_可选
    //    /// </summary>
    //    public string error_code { get; set; }
    //    public trade trade { get; set; }
    //}
    public class trade
    {
        /// <summary>
        /// 商家号_必选
        /// </summary>
        public string merchant_code { get; set; }
        /// <summary>
        /// 商户网站唯一订单号_必选
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 商户订单时间_必选_格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string order_time { get; set; }
        /// <summary>
        /// 商户订单总金额_必选_精确到小数点后两
        /// </summary>
        public decimal order_amount { get; set; }
        /// <summary>
        /// 智付交易订单号_必选
        /// </summary>
        public string trade_no { get; set; }
        /// <summary>
        /// 智付交易订单时间_必选_格式为：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string trade_time { get; set; }
        /// <summary>
        /// 交易状态_必选_SUCCESS 交易成功；FAILED 交易失败；UNPAY 未支付
        /// </summary>
        public string trade_status { get; set; }
    } 

    #endregion

}
