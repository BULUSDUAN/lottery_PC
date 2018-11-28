using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Common.Gateway.Alipay.WAPPay
{
    /// <summary>
    /// 类名：Service
    /// 功能：支付宝各接口构造类
    /// 详细：构造支付宝各接口请求参数
    /// 版本：3.0
    /// 日期：2012-07-11
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考
    /// </summary>
    public class Service
    {
        /// <summary>
        /// 初始化Service类
        /// </summary>
        public Service(string Partner, string Key, string Seller_account_name)
        {
            Config.Partner = Partner;
            Config.Key = Key;
            Config.Seller_account_name = Seller_account_name;
        }

        /// <summary>
        /// 构造wap交易创建接口
        /// </summary>
        /// <param name="subject">商品名称</param>
        /// <param name="outTradeNo">外部交易号(由商户创建，请不要重复)</param>
        /// <param name="totalFee">商品总价</param>
        /// <param name="notifyUrl">商户接收通知URL（异步返回商户）</param>
        /// <param name="outUser">商户用户唯一ID，外部用户唯一标识</param>
        /// <param name="merchantUrl">返回商户产品URL</param>
        /// <param name="callbackurl">支付成功跳转链接</param>
        /// <param name="reqid">商户请求ID</param>
        /// <returns>返回token</returns>
        public string alipay_wap_trade_create_direct(
            string subject,
            string outTradeNo,
            string totalFee,
            string notifyUrl,
            string outUser,
            string merchantUrl,
            string callbackurl,
            string reqid)
        {
            //临时请求参数数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();

            //构造请求参数数组
            string req_Data = "<direct_trade_create_req><subject>" + subject + "</subject><out_trade_no>" +
                outTradeNo + "</out_trade_no><total_fee>" + totalFee + "</total_fee><seller_account_name>" + Config.Seller_account_name +
                "</seller_account_name><notify_url>" + notifyUrl + "</notify_url><out_user>" + outUser +
                "</out_user><merchant_url>" + merchantUrl + "</merchant_url>" +
                "<call_back_url>" + callbackurl + "</call_back_url></direct_trade_create_req>";

            sParaTemp.Add("req_data", req_Data);
            sParaTemp.Add("service", Config.Service_Create);
            sParaTemp.Add("sec_id", Config.Sec_id);
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("req_id", reqid);
            sParaTemp.Add("format", Config.Format);
            sParaTemp.Add("v", Config.V);

            //构造表单提交HTML数据
            string strResult = Submit.SendPostInfo(sParaTemp, Config.Req_url, Config.Input_charset_UTF8, Config.Key, Config.Sec_id);


            //对返回字符串处理，得到request_token的值
            strResult = HttpUtility.UrlDecode(strResult, Encoding.GetEncoding(Config.Input_charset_UTF8));
            //分解返回数据 用&拆分赋值给result
            string[] result = strResult.Split('&');

            string res_data = string.Empty;

            //-------------------------------此处代码有bug，已注释---------------------------
            ////AlipayService.cs 124行代码修改
            //if (result.Length > 0)
            //    //替换成标准Xml数据
            //    res_data = result[0].Replace("res_data=", string.Empty);
            //---------------------------------------------------------------------------------------

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].IndexOf("res_data=") >= 0)
                {
                    res_data = result[i].Replace("res_data=", string.Empty);
                }
            }

            //得到 request_token 的值
            string token = string.Empty;
            try
            {
                token = Function.GetStrForXmlDoc(res_data, "direct_trade_create_res/request_token");
            }
            catch
            {
                //提示 返回token值无效
                return string.Empty;
            }
            return token;

        }

        /// <summary>
        /// 授权并执行
        /// </summary>
        /// <param name="callbackurl">支付成功跳转链接</param>
        /// <param name="token">返回token</param> 
        /// <returns>直接跳转</returns>
        public string alipay_Wap_Auth_AuthAndExecute(
            string callbackurl,
            string token)
        {
            //临时请求参数数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            //拼接req_data
            string req_Data = "<auth_and_execute_req><request_token>" + token + "</request_token></auth_and_execute_req>";

            sParaTemp.Add("req_data", req_Data);
            sParaTemp.Add("service", Config.Service_Auth);
            sParaTemp.Add("sec_id", Config.Sec_id);
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("format", Config.Format);
            sParaTemp.Add("v", Config.V);

            //返回拼接后的跳转URL
            return Submit.SendPostRedirect(sParaTemp, Config.Req_url, Config.Input_charset_UTF8, Config.Key, Config.Sec_id);
        }
    }
}