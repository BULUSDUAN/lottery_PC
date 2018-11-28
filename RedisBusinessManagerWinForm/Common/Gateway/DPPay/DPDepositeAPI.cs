using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
    /// <summary>
    /// DP充值相关
    /// </summary>
   public class DPDepositeAPI
    {
        /// <summary>
        /// 充值申请
        /// </summary>
        /// <param name="company_id">平台id</param>
        /// <param name="bank_id">银行id</param>
        /// <param name="amount">充值金额</param>
        /// <param name="company_order_num">平台订单号</param>
        /// <param name="company_user">平台用户</param>
        /// <param name="estimated_payment_bank">预计付款银行 客户预计使用的银行编码，实际</param>
        /// <param name="deposit_mode">充值渠道 1银行卡2第三方、微信 3移动电子钱包（支付宝二维码）</param>
        /// <param name="web_url">平台访问地址 建议传送平台网站域名</param>
        /// <param name="memo">备用字段</param>
        /// <param name="note">附言 平台附言，如果note_model为1时，则此字段不能为空</param>
        /// <param name="note_model">附言模式 1平台附言 2 DP系统附言，如果deposit_mode为3时，则此字段为1，暂时必须使用平台附言（详见2.4附言格式规范）</param>
        /// <param name="terminal">使用终端 1电脑端 2手机端 3平板 9其他 平台需要传输客户的真实使用终端，以防止客户会无法成功发起充值申请 </param>
        ///  <param name="银行卡充值申请样例">http://52.69.65.224/Mownecum_2_API_Live/Deposit?format=json&company_id=7&bank_id=1&amount=1.00&company_order_num=FDADAL5SHP1UI9TFGWW892zbvR&company_user=2d8&estimated_payment_bank=1&deposit_mode=1&group_id=0&web_url=baidu.com&note_model=1&memo=1&note=73et&terminal=1&key=ead808648cb2703642da69cb8d461e5b</param>
        /// <param name="第三方充值申请样例">http://52.69.65.224/Mownecum_2_API_Live/Deposit?format=json&company_id=3&bank_id=1&amount=4.00&company_order_num=123456789&company_user=hb123&estimated_payment_bank=1&deposit_mode=2&group_id=0&web_url=http://baidu.com/&note_model=2&memo=&note=&terminal=1&key= a5e2cdae69b40b03b4c497975e5a9a66</param>
        /// <param name="支付宝充值申请样例">http://52.69.65.224/Mownecum_2_API_Live/Deposit?format=json&company_id=5&bank_id=30&amount=3.00&company_order_num=123456&company_user=123qwe&estimated_payment_bank=30&deposit_mode=3&group_id=0&web_url= &note_model=1&memo=&note=杰1309048&terminal=1&key= e52f0f859649f4966cbf1d59947969f3</param>
        /// <returns></returns>
       
        public static string Deposit(DepositeInfo di,String config,String url)
        {

            //String config = "123qwe";
            var writer = Common.Log.LogWriterGetter.GetLogWriter();
            //拼接要传递的参数值
            String param = null;
            if (di.deposit_mode != null && "1".Equals(di.deposit_mode))
            {//银行卡
                param = "company_id=" + di.company_id + "&bank_id=" + di.bank_id + "&amount=" + di.amount + "&company_order_num=" + di.company_order_num + "&company_user=" + di.company_user + "&estimated_payment_bank=" + di.estimated_payment_bank + "&deposit_mode=" + di.deposit_mode + "&group_id=0&web_url=" + di.web_url + "&note_model=" + di.note_model + "&memo=" + di.memo + "&note=" + di.note + "&terminal=" + di.terminal + "&key=" + di.getKey(config) + "";
            }
            else if (di.deposit_mode != null && "2".Equals(di.deposit_mode))
            {
                param = "company_id=" + di.company_id + "&bank_id=" + di.bank_id + "&amount=" + di.amount + "&company_order_num=" + di.company_order_num + "&company_user=" + di.company_user + "&estimated_payment_bank=" + di.estimated_payment_bank + "&deposit_mode=" + di.deposit_mode + "&group_id=0&web_url=" + di.web_url + "&note_model=" + di.note_model + "&memo=" + di.memo + "&note=" + di.note + "&terminal=" + di.terminal + "&key=" + di.getKey(config) + "";
            }
            else if (di.deposit_mode != null && "3".Equals(di.deposit_mode))
            {
                param = "company_id=" + di.company_id + "&bank_id=" + di.bank_id + "&amount=" + di.amount + "&company_order_num=" + di.company_order_num + "&company_user=" + di.company_user + "&estimated_payment_bank=" + di.estimated_payment_bank + "&deposit_mode=" + di.deposit_mode + "&group_id=0&web_url=" + di.web_url + "&note_model=" + di.note_model + "&memo=" + di.memo + "&note=" + di.note + "&terminal=" + di.terminal + "&key=" + di.getKey(config) + "";
            }
            else
            {
                param = "company_id=" + di.company_id + "&bank_id=" + di.bank_id + "&amount=" + di.amount + "&company_order_num=" + di.company_order_num + "&company_user=" + di.company_user + "&estimated_payment_bank=" + di.estimated_payment_bank + "&deposit_mode=" + di.deposit_mode + "&group_id=0&web_url=" + di.web_url + "&note_model=" + di.note_model + "&memo=" + di.memo + "&note=" + di.note + "&terminal=" + di.terminal + "&key=" + di.getKey(config) + "";
            }
            writer.Write("充值请求参数", "充值请求参数", Common.Log.LogType.Information, "充值请求参数", param);
            url = url + "Deposit?format=json";
            writer.Write("充值请求url", "充值请求url", Common.Log.LogType.Information, "充值请求url", url);
          
            String result = HttpUtil.Post(url, param);
            writer.Write("充值请求结果", "充值请求结果", Common.Log.LogType.Information, "充值请求结果", result);
            return result;
        }
       
    }
}
