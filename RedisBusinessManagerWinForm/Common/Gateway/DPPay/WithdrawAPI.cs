using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.DPPay
{
    public class WithdrawAPI
    {
        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="wai">提现申请实体类</param>
        /// <returns></returns>
        public static String Withdrawal(WithdrawApplyInfo wai,String config,String url) {
            //向DP发出请求
            var writer = Common.Log.LogWriterGetter.GetLogWriter();
            url = url + "Withdrawal?format=json";
            String key = wai.getKey(config);
            writer.Write("key值是", "key值是", Common.Log.LogType.Information, "key值是",config+"==============="+key);
            String param = "company_id=" + wai.company_id + "&bank_id=" + wai.bank_id + "&company_order_num=" + wai.company_order_num + "&amount=" + wai.amount + "&card_num=" + wai.card_num
                + "&card_name=" +HttpUtil.UrlEncode(wai.card_name)+ "&company_user=" + wai.company_user + "&issue_bank_name=" +HttpUtil.UrlEncode(wai.issue_bank_name) + "&issue_bank_address=" +HttpUtil.UrlEncode(wai.issue_bank_address) + 
                "&memo=" + wai.memo + "&key=" + key + "";
            writer.Write("提现请求参数", "提现请求参数", Common.Log.LogType.Information, "提现请求参数", param);
            writer.Write("提现请求url", "提现请求url", Common.Log.LogType.Information, "提现请求url", url);
            String result = HttpUtil.Post(url, param);
         
            writer.Write("DP提现申请结果", "DP提现申请结果", Common.Log.LogType.Information, "DP提现申请结果============", result);
            return result;
        }
        
    }
}
