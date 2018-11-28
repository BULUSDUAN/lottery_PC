using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;

namespace Common.Gateway.BiFuBao
{
   public class BBPayAPI
    {
        /// <summary>
        /// 获取币币支付请求地址
        /// </summary>
        public string GetBBPayUrl(BBPayAPIInfo info)
        {
            string url = string.Empty;
            try
            {
                if (info == null)
                    throw new Exception("传入参数不能为空！");
                if (!ValidData(info))//数据验证
                    throw new Exception("传入参数数据验证失败！");
                if (string.IsNullOrEmpty(info.BBPayUrl) || string.IsNullOrEmpty(info.BBPayKey))
                    throw new Exception("未能找到支付地址或商户唯一编码！");
                info.p6_amount = Convert.ToDecimal(info.p6_amount.ToString("N2"));
                var postData = string.Format("p1_md={0}&p2_xn={1}&p3_bn={2}&p4_pd={3}&p5_name={4}&p6_amount={5}&p7_cr={6}&p8_ex={7}&p9_url={8}&p10_reply={9}&p11_mode={10}&p12_ver={11}", info.p1_md, info.p2_xn, info.p3_bn, GetBnakID(info.p4_pd), info.p5_name, info.p6_amount, info.p7_cr, info.p8_ex, info.p9_url, info.p10_reply, info.p11_mode, info.p12_ver);
                var sign = Encipherment.MD5(postData + info.BBPayKey);
                url = info.BBPayUrl + "?" + postData + "&sign=" + sign + "";
                //resultMsg = PostManager.Post(bbPayUrl, postData, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return url;
        }
        public string CreateBBPayForm(BBPayAPIInfo info)
        {
            string formHtml = string.Empty;
            try
            {
                if (info == null)
                    return formHtml;
                if (!ValidData(info))//数据验证
                    return formHtml;
                if (string.IsNullOrEmpty(info.BBPayUrl) || string.IsNullOrEmpty(info.BBPayKey))
                    return formHtml;
                info.p6_amount = Convert.ToDecimal(info.p6_amount.ToString("N2"));
                var postData = string.Format("p1_md={0}&p2_xn={1}&p3_bn={2}&p4_pd={3}&p5_name={4}&p6_amount={5}&p7_cr={6}&p8_ex={7}&p9_url={8}&p10_reply={9}&p11_mode={10}&p12_ver={11}", info.p1_md, info.p2_xn, info.p3_bn, info.p4_pd, info.p5_name, info.p6_amount, info.p7_cr, info.p8_ex, info.p9_url, info.p10_reply, info.p11_mode, info.p12_ver);
                var sign = string.Empty;
                sign = Encipherment.MD5(postData + info.BBPayKey, Encoding.UTF8);
                StringBuilder strBud = new StringBuilder();
                strBud.Append("<form name='formBBPay'  id='formBBPay' method='post' action='" + info.BBPayUrl + "'/>");
                strBud.Append("<input type='hidden' name='p1_md'  id='p1_md' value='" + info.p1_md + "'/>");
                strBud.Append("<input type='hidden' name='p2_xn'id='p2_xn' value='" + info.p2_xn + "'/>");
                strBud.Append("<input type='hidden' name='p3_bn' id='p3_bn' value='" + info.p3_bn + "'/>");
                strBud.Append("<input type='hidden' name='p4_pd' id='p4_pd' value='" + info.p4_pd + "'/>");
                strBud.Append("<input type='hidden' name='p5_name' id='p5_name' value='" + info.p5_name + "'/>");
                strBud.Append("<input type='hidden' name='p6_amount' id='p6_amount' value='" + info.p6_amount + "'/>");
                strBud.Append("<input type='hidden' name='p7_cr' id='p7_cr' value='" + info.p7_cr + "'/>");
                strBud.Append("<input type='hidden' name='p8_ex'  id='p8_ex' value='" + info.p8_ex + "'/>");
                strBud.Append("<input type='hidden' name='p9_url' id='p9_url' value='" + info.p9_url + "'/>");
                strBud.Append("<input type='hidden' name='p10_reply' id='p10_reply'  value='" + info.p10_reply + "'/>");
                strBud.Append("<input type='hidden' name='p11_mode' id='p11_mode' value='" + info.p11_mode + "'/>");
                strBud.Append("<input type='hidden' name='p12_ver' id='p12_ver' value='" + info.p12_ver + "'/>");
                strBud.Append("<input type='hidden' name='sign' id='sign' value='" + sign + "'/>");
                strBud.Append("</form>");
                strBud.Append("<script language=javascript>document.getElementById('formBBPay').submit();</script>");
                formHtml = strBud.ToString();
            }
            catch (Exception ex)
            {
                return formHtml;
            }
            return formHtml;
        }
        /// <summary>
        /// 币币支付返回参数MD5加密
        /// </summary>
        public string GetBBPayCallBackSign(BBPayCallBackInfo info)
        {
            string resultSign = string.Empty;
            try
            {
                if (info == null || string.IsNullOrEmpty(info.sign))
                    return string.Empty;
                var resultData = string.Format("p1_md={0}&p2_sn={1}&p3_xn={2}&p4_amt={3}&p5_ex={4}&p6_pd={5}&p7_st={6}&p8_reply={7}", info.p1_md, info.p2_sn, info.p3_xn, info.p4_amt, info.p5_ex, info.p6_pd, info.p7_st, info.p8_reply);
                if (string.IsNullOrEmpty(info.BBPayKey))
                    return string.Empty;
                resultData = resultData + info.BBPayKey;
                resultSign = Encipherment.MD5(resultData,Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resultSign;
        }
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool ValidData(BBPayAPIInfo info)
        {
            string[] arrayP1 = new string[] { "1", "2" };
            string[] arrayP10 = new string[] { "0", "1" };
            string[] arrayP11 = new string[] { "0", "1", "2" };

            if (!arrayP1.Contains(info.p1_md.ToString()))
                return false;
            else if (string.IsNullOrEmpty(info.p2_xn))
                return false;
            else if (string.IsNullOrEmpty(info.p3_bn))
                return false;
            else if (string.IsNullOrEmpty(info.p4_pd))
                return false;
            else if (string.IsNullOrEmpty(info.p5_name))
                return false;
            else if (info.p6_amount <= 0)
                return false;
            else if (info.p7_cr != 1)
                return false;
            else if (string.IsNullOrEmpty(info.p9_url))
                return false;
            else if (!arrayP10.Contains(info.p10_reply.ToString()))
                return false;
            else if (!arrayP11.Contains(info.p11_mode.ToString()))
                return false;
            return true;
        }
        /// <summary>
        /// 银行信息列表
        /// </summary>
        public int GetBnakID(string bankName)
        {
            switch (bankName.Trim())
            {
                case "招商银行":
                    return 10001;
                case "兴业银行":
                    return 10002;
                case "中信银行":
                    return 10003;
                case "民生银行":
                    return 10004;
                case "光大银行":
                    return 10005;
                case "华夏银行":
                    return 10006;
                case "北京农村商业银行":
                    return 10007;
                case "深圳发展银行":
                    return 10008;
                case "中国银行":
                    return 10009;
                case "北京银行":
                    return 10010;
                case "邮政储蓄银行":
                    return 10011;
                case "上海浦发银行":
                    return 10012;
                case "东亚银行":
                    return 10013;
                case "广东发展银行":
                    return 10014;
                case "南京银行":
                    return 10015;
                case "上海交通银行":
                    return 10016;
                case "平安银行":
                    return 10017;
                case "中国工商银行":
                    return 10018;
                case "杭州银行":
                    return 10019;
                case "中国建设银行":
                    return 10020;
                case "宁波银行":
                    return 10021;
                case "中国农业银行":
                    return 10022;
                case "浙商银行":
                    return 10023;
            }
            return 0;
        }
    }
}
