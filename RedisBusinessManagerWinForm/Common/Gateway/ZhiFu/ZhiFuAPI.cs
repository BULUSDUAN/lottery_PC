using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;
using Common.JSON;
using Common.Net;
using System.Xml.Serialization;
using System.IO;
using System.Web.Security;
using System.Web;

namespace Common.Gateway.ZhiFu
{
    public class ZhiFuAPI
    {
        //private string PayUrl = "https://pay.dinpay.com/gateway?input_charset=UTF-8";
        //private string QueryUrl = "https://query.dinpay.com/query";
        //private string ZhiFuKey = "123ChongXuntiKeJi_wwwcaibbcom";
        public string GetZhiFuPayForm(ZhiFuParamInfo info)
        {
            if (info == null)
                throw new Exception("传入参数为空");
            StringBuilder strBud = new StringBuilder();
            if (string.IsNullOrEmpty(info.service_type))
                info.service_type = "direct_pay";
            if (string.IsNullOrEmpty(info.interface_version))
                info.interface_version = "V3.0";
            if (string.IsNullOrEmpty(info.sign_type))
                info.sign_type = "MD5";
            var order_time = info.order_time.ToString("yyyy-MM-dd HH:mm:ss");
            var order_amount = info.order_amount.ToString("N2").Replace(",", "");
            SortedDictionary<string, object> sortDic = new SortedDictionary<string, object>();
            sortDic.Add("merchant_code", info.merchant_code);
            sortDic.Add("service_type", info.service_type);
            sortDic.Add("pay_type", info.pay_type);
            sortDic.Add("input_charset", info.input_charset);
            sortDic.Add("notify_url", info.notify_url);
            sortDic.Add("return_url", info.return_url);
            sortDic.Add("client_ip", info.client_ip);
            sortDic.Add("interface_version", info.interface_version);
            //sortDic.Add("sign_type", info.sign_type = "MD5");
            sortDic.Add("order_no", info.order_no);
            sortDic.Add("order_time", order_time);
            sortDic.Add("order_amount", order_amount);
            sortDic.Add("product_name", info.product_name);
            sortDic.Add("show_url", info.show_url);
            sortDic.Add("product_code", info.product_code);
            sortDic.Add("product_num", info.product_num);
            sortDic.Add("product_desc", info.product_desc);
            sortDic.Add("bank_code", info.bank_code);
            sortDic.Add("extra_return_param", info.extra_return_param);
            sortDic.Add("extend_param", info.extend_param);
            sortDic.Add("redo_flag", info.redo_flag);

            var query = sortDic.Where(s => s.Value != null && s.Value.ToString() != string.Empty);
            StringBuilder strPara = new StringBuilder();
            StringBuilder strExtend_param = new StringBuilder();
            foreach (var item in query)
            {
                strPara.Append(item.Key + "=" + item.Value + "&");
            }
            //var strExtend = string.Empty; 
            //if (strExtend_param != null && !string.IsNullOrEmpty(strExtend_param.ToString()))
            //    strExtend = strExtend_param.ToString().TrimEnd('|');
            //sortDic.Add("extend_param", strExtend);


            //var _query = sortDic.Where(s => s.Value != null && s.Value.ToString() != string.Empty);
            //foreach (var item in query)
            //{
            //    strPara.Append(item.Key + "=" + item.Value + "&");
            //}

            string para = strPara.ToString() + "key=" + info.ZhiFuKey;
            var strSign = FormsAuthentication.HashPasswordForStoringInConfigFile(para, "md5").ToLower();
            //var strSign = Encipherment.MD5(para, Encoding.UTF8);

            strBud.Append("<form name='dinpayForm' method='post' action=" + info.PayUrl + " target='_self'>");
            strBud.Append("<input type='hidden' name='sign' value='" + strSign + "' />");
            strBud.Append("<input type='hidden' name='merchant_code' value='" + info.merchant_code + "' />");
            strBud.Append("<input type='hidden' name='bank_code' value='" + info.bank_code + "'/>");
            strBud.Append("<input type='hidden' name='order_no' value='" + info.order_no + "'/>");
            strBud.Append("<input type='hidden' name='order_amount' value='" + order_amount + "'/>");
            strBud.Append("<input type='hidden' name='service_type' value='" + info.service_type + "'/>");
            strBud.Append("<input type='hidden' name='input_charset' value='" + info.input_charset + "'/>");
            strBud.Append("<input type='hidden' name='notify_url' value='" + info.notify_url + "'>");
            strBud.Append("<input type='hidden' name='interface_version' value='" + info.interface_version + "'/>");
            strBud.Append("<input type='hidden' name='sign_type' value='" + info.sign_type + "'/>");
            strBud.Append("<input type='hidden' name='order_time'  value='" + order_time + "'/>");
            strBud.Append("<input type='hidden' name='product_name' value='" + info.product_name + "'/>");
            strBud.Append("<input Type='hidden' Name='client_ip' value='" + info.client_ip + "'/>");
            strBud.Append("<input Type='hidden' Name='extend_param' value='" + info.extend_param + "'/>");
            strBud.Append("<input Type='hidden' Name='extra_return_param' value='" + info.extra_return_param + "'/>");
            strBud.Append("<input Type='hidden' Name='pay_type' value='" + info.pay_type + "'/>");
            strBud.Append("<input Type='hidden' Name='product_code' value='" + info.product_code + "'/>");
            strBud.Append("<input Type='hidden' Name='product_desc' value='" + info.product_desc + "'/>");
            strBud.Append("<input Type='hidden' Name='product_num' value='" + info.product_num + "'/>");
            strBud.Append("<input Type='hidden' Name='return_url' value='" + info.return_url + "'/>");
            strBud.Append("<input Type='hidden' Name='show_url' value='" + info.show_url + "'/>");
            strBud.Append("<input Type='hidden' Name='redo_flag' value='" + info.redo_flag + "'/>");
            strBud.Append("</form>");
            strBud.Append("<script language=javascript>document.dinpayForm.submit();</script>");

            return strBud.ToString();
        }
        /// <summary>
        /// 单笔交易查询
        /// </summary>
        /// <returns></returns>
        public response ZhiFuOrderQuery(string merchant_code, string order_no, string trade_no, string url, string zhifuKey, string interface_version = "V3.0", string sign_type = "MD5", string service_type = "single_trade_query")
        {
            response info = new response();
            SortedDictionary<string, object> sortDic = new SortedDictionary<string, object>();
            sortDic.Add("merchant_code", merchant_code);
            sortDic.Add("interface_version", interface_version);
            sortDic.Add("service_type", service_type);
            sortDic.Add("order_no", order_no);
            sortDic.Add("trade_no", trade_no);

            var query = sortDic.Where(s => s.Value != null && s.Value.ToString() != string.Empty);
            StringBuilder strPara = new StringBuilder();
            foreach (var item in query)
            {
                strPara.Append(item.Key + "=" + item.Value + "&");
            }
            string para = strPara.ToString() + "key=" + zhifuKey;
            var strSign = FormsAuthentication.HashPasswordForStoringInConfigFile(para, "md5").ToLower();
            sortDic.Add("sign", strSign);

            var postData = JsonSerializer.Serialize(sortDic);
            var result = PostManager.PostCustomerHttps(url, postData, Encoding.UTF8);
            string strResult = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                int index = result.IndexOf("<response>");
                int end = result.LastIndexOf("</dinpay>");
                strResult = result.Substring(index, (end - index));
            }
            if (!string.IsNullOrEmpty(strResult))
            {
                info = XmlDeSerialize<response>(strResult);
                if (info.is_success == "F")//解析错误编码
                {

                }
            }
            return info;
        }
        /// <summary>
        /// 获取回调加签
        /// </summary>
        /// <returns></returns>
        public ZhiFuCallBackParamInfo ZhiFuCallBackSign(ZhiFuCallBackParamInfo info)
        {
            ZhiFuCallBackParamInfo paraminfo = new ZhiFuCallBackParamInfo();
            if (info == null)
                throw new Exception("传入参数为空");
            StringBuilder strBud = new StringBuilder();
            //if (string.IsNullOrEmpty(info.interface_version))
            //    info.interface_version = "V3.0";
            //if (string.IsNullOrEmpty(info.sign_type))
            //    info.sign_type = "MD5";
            var order_time = info.order_time.ToString("yyyy-MM-dd HH:mm:ss");
            //var order_amount = info.order_amount.ToString("N2");
            var trade_time = info.trade_time.ToString("yyyy-MM-dd HH:mm:ss");
            SortedDictionary<string, object> sortDic = new SortedDictionary<string, object>();
            sortDic.Add("merchant_code", info.merchant_code);
            sortDic.Add("notify_type", info.notify_type);
            sortDic.Add("notify_id", info.notify_id);
            sortDic.Add("interface_version", info.interface_version);
            sortDic.Add("order_no", info.order_no);
            sortDic.Add("order_time", order_time);
            sortDic.Add("order_amount", info.order_amount);
            sortDic.Add("extra_return_param", info.extra_return_param);
            sortDic.Add("trade_no", info.trade_no);
            sortDic.Add("trade_time", trade_time);
            sortDic.Add("trade_status", info.trade_status);
            sortDic.Add("bank_code", info.bank_code);
            sortDic.Add("bank_seq_no", info.bank_seq_no);

            var query = sortDic.Where(s => s.Value != null && s.Value.ToString() != string.Empty);
            StringBuilder strPara = new StringBuilder();
            foreach (var item in query)
            {
                strPara.Append(item.Key + "=" + item.Value + "&");
            }
            var para = string.Empty;
            if (strPara != null && !string.IsNullOrEmpty(strPara.ToString()))
                para = strPara.ToString() + "key=" + info.ZhiFuKey;
            paraminfo.signData = para;
            paraminfo.sign = FormsAuthentication.HashPasswordForStoringInConfigFile(para, "md5").ToLower();
            return paraminfo;
        }
        /// <summary>
        /// 反序列化xml
        /// </summary>
        public T XmlDeSerialize<T>(string xml)
        {
            #region

            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                StringReader sr = new StringReader(xml);
                T o = (T)formatter.Deserialize(sr);
                sr.Close();
                return o;
            }
            catch
            {
            }
            return default(T);

            #endregion
        }
    }
}
