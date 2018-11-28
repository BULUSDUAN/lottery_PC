using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.Alipay.GeneralPoint
{
    /// <summary>
    /// 支付宝urlSign构造类（商户可在此类构造URL Sign）
    /// </summary>
    public class AlipaySign
    {
        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        public AlipaySign(string partner, string key, string seller_email)
        {
            AlipayConfig.Partner = partner;
            AlipayConfig.Key = key;
            AlipayConfig.Seller_Email = seller_email;
        }

        #region 通用积分兑入接口

        /// <summary>
        /// 构造积分兑入接口请求url
        /// </summary>
        /// <param name="logon_id">支付宝会员登陆账号，即需要被兑入集分宝的支付宝登录账号，可以是手机或邮箱</param>
        /// <param name="amount">需要兑入的支付宝积分数</param>
        /// <param name="out_biz_no">外部流水号-即本地系统的订单流水号</param>
        /// <param name="out_user_id">外部会员ID，即为外部系统的会员ID，用以支付宝进行反查询使用</param>
        /// <param name="out_biz_time">外部业务时间</param>
        /// <param name="out_point_amount">外部积分数</param>
        /// <param name="is_need_q">是否需要Q会员</param>
        /// <returns></returns>
        public string CreateDirectPayByUser(string logon_id, string amount, string out_biz_no, string out_user_id, string out_biz_time, string out_point_amount, string is_need_q)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("service", "generalpoint_add_point");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partner", AlipayConfig.Partner);
            dic.Add("_input_charset", Alipay._Input_Charset);
            dic.Add("logon_id", logon_id);
            dic.Add("amount", amount);
            dic.Add("out_biz_no", out_biz_no);
            dic.Add("out_user_id", out_user_id);
            dic.Add("out_biz_time", out_biz_time);
            dic.Add("out_point_amount", out_point_amount);
            dic.Add("is_need_q", is_need_q);
            return Alipay.AlipayDoGetN(dic, AlipayConfig.Key);
        }

        #endregion


    }
}
