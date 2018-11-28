using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Net;

namespace Common.Gateway.YinBao
{
    /// <summary>
    /// 银宝urlSign构造类（商户可在此类构造URL Sign）
    /// </summary>
    public class YinBaoSign
    {
        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        public YinBaoSign(string partner, string key)
        {
            Config.Partner = partner;
            Config.Key = key;
        }

        /// <summary>
        /// 构造银宝网银支付请求URL
        /// </summary>
        /// <param name="p2_xn">商户订单号,商户系统生成的订单号</param>
        /// <param name="p4_pd">支付方式ID,用户选择的支付银行ID(银行ID列表）</param>
        /// <param name="p5_name">产品名称,商户自定义产品名称</param>
        /// <param name="p6_amount">支付金额,支付金额(两位小数)</param>
        /// <param name="p8_ex">扩展信息,商户自定义信息，原样返回</param>
        /// <param name="p9_url">通知地址,充值成功后通知和显示的地址</param>
        /// <param name="p1_md">指令类型,网银=1,卡类=2，默认为1</param>
        /// <param name="p7_cr">币种,1=人民币</param>
        /// <param name="p11_mode">调用模式,0 返回充值地址，由商户负责跳转   1 显示银宝充值界面,跳转到充值   2 不显示银宝充值界面 直接跳转到网</param>
        /// <param name="p10_reply">是否通知,不通知=0，通知=1</param>
        /// <param name="p12_ver">版本号,当前为 1 ,如有变动我们回通知您</param>
        /// <returns>生成的URL签名</returns>
        /// <remarks>注意传入的参数不能为空</remarks>
        public string CreateDirectPayUrl(string p2_xn, string p4_pd, string p5_name, string p6_amount, string p8_ex, string p9_url, int p1_md = 1, int p7_cr = 1, int p10_reply = 1, int p11_mode = 2, int p12_ver = 1)
        {
            string query = string.Format("p1_md={0}&p2_xn={1}&p3_bn={2}&p4_pd={3}&p5_name={4}&p6_amount={5}&p7_cr={6}&p8_ex={7}&p9_url={8}&p10_reply={9}&p11_mode={10}&p12_ver={11}",
                p1_md, p2_xn, Config.Partner, p4_pd, p5_name, p6_amount, p7_cr, p8_ex, p9_url, p10_reply, p11_mode, p12_ver
                );

            return YinBao.YinBaoDeGet(query, Config.Key);
        }

    }
}
