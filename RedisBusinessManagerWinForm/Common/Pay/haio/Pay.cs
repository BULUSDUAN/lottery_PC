using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Pay.haio
{
    public class Pay
    {
        /// <summary>
        /// 海鸥支付
        /// </summary>
        /// <param name="express_return_url">同步url</param>
        /// <param name="express_notify_url">异步url</param>
        /// <param name="userId">商户id</param>
        /// <param name="orderId">订单号</param>
        /// <param name="price">金额</param>
        /// <param name="payvia">支付类型</param>
        /// <param name="key"></param>
        /// <param name="gateway">请求地址</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string PayRequest(string express_return_url, string express_notify_url, string userId, string orderId,
            decimal price, string payvia, string key, string gateway, string format)
        {
            string timespan = DateTime.Now.ToString("yyyyMMddHHmmss"); 
            string param = string.Format("userid={0}&orderid={1}&price={2}&payvia={3}&notify={4}&callback={5}&key={6}",
                userId, orderId, price, payvia, express_notify_url, express_return_url, key);
            string sign = HttpHelp.MD5(HttpHelp.MD5(param, Encoding.UTF8) + key, Encoding.UTF8);  //数据签名
            StringBuilder form = new StringBuilder();
            form.AppendFormat("<form action='{0}' method='post' id='frm'>", gateway);
            form.AppendFormat("<input type='hidden' name='userid' value='{0}'/>", userId);
            form.AppendFormat("<input type='hidden' name='orderid' value='{0}'/>", orderId);
            form.AppendFormat("<input type='hidden' name='price' value='{0}'/>", price);
            form.AppendFormat("<input type='hidden' name='callback' value='{0}'/>", express_return_url);
            form.AppendFormat("<input type='hidden' name='notify' value='{0}'/>", express_notify_url);
            form.AppendFormat("<input type='hidden' name='payvia' value='{0}'/>", payvia);
            form.AppendFormat("<input type='hidden' name='timespan' value='{0}'/>", timespan);
            form.AppendFormat("<input type='hidden' name='custom' value='{0}'/>", "pay");
            form.AppendFormat("<input type='hidden' name='format' value='{0}'/>", format);
            form.AppendFormat("<input type='hidden' name='sign' value='{0}'/>", sign);
            form.Append("</form>");
            form.Append("<script type='text/javascript'>document.getElementById('frm').submit()</script>");
            return form.ToString();
        }
    }
}
