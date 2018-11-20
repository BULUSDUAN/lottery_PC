using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;

namespace Common.Gateway.HuiChao
{
    public class HuiChaoAPI
    {
        /// <summary>
        /// 返回支付提交Form
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetHuiChaoForm2(HuiChaoInfo info)
        {
            if (string.IsNullOrEmpty(info.MerNo))
                throw new Exception("商户号不能为空");
            else if (string.IsNullOrEmpty(info.BillNo))
                throw new Exception("订单号不能为空");
            else if (string.IsNullOrEmpty(info.Amount))
                throw new Exception("订单金额不能为空");
            else if (string.IsNullOrEmpty(info.ReturnURL))
                throw new Exception("页面跳转地址不能为空");
            else if (string.IsNullOrEmpty(info.AdviceURL))
                throw new Exception("服务器通知地址不能为空");
            else if (string.IsNullOrEmpty(info.orderTime))
                throw new Exception("请求时间不能为空");
            else if (string.IsNullOrEmpty(info.MD5Key))
                throw new Exception("密钥不能为空");
            if (string.IsNullOrEmpty(info.defaultBankNumber))
                info.defaultBankNumber = "UNIONPAY";
            info.Amount = Convert.ToDecimal(info.Amount).ToString("N2").Replace(",","");
            var signMsg = Encipherment.MD5(info.MerNo + "&" + info.BillNo + "&" + info.Amount + "&" + info.ReturnURL + "&" + info.MD5Key).ToUpper();
            StringBuilder strBud = new StringBuilder();

            strBud.Append("<form name='dinpayForm' method='post' action=" + info.PayUrl + " target='_self'>");
            strBud.Append("<input type='hidden' name='MerNo' value='" + info.MerNo + "' />");
            strBud.Append("<input type='hidden' name='BillNo' value='" + info.BillNo + "' />");
            strBud.Append("<input type='hidden' name='Amount' value='" + info.Amount + "'/>");
            strBud.Append("<input type='hidden' name='ReturnURL' value='" + info.ReturnURL + "'/>");
            strBud.Append("<input type='hidden' name='AdviceURL' value='" + info.AdviceURL + "'/>");
            strBud.Append("<input type='hidden' name='SignInfo' value='" + signMsg + "'/>");
            strBud.Append("<input type='hidden' name='orderTime' value='" + info.orderTime + "'/>");
            strBud.Append("<input type='hidden' name='defaultBankNumber' value='" + info.defaultBankNumber + "'>");
            strBud.Append("<input type='hidden' name='Remark' value='" + info.Remark + "'/>");
            strBud.Append("<input type='hidden' name='products' value='" + info.products + "'/>");
            strBud.Append("</form>");
            strBud.Append("<script language=javascript>document.dinpayForm.submit();</script>");
            return strBud.ToString();
        }


        /// <summary>
        /// 返回支付提交Form
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetHuiChaoForm(HuiChaoInfo info)
        {
            if (string.IsNullOrEmpty(info.MerNo))
                throw new Exception("商户号不能为空");
            else if (string.IsNullOrEmpty(info.BillNo))
                throw new Exception("订单号不能为空");
            else if (string.IsNullOrEmpty(info.Amount))
                throw new Exception("订单金额不能为空");
            else if (string.IsNullOrEmpty(info.ReturnURL))
                throw new Exception("页面跳转地址不能为空");
            else if (string.IsNullOrEmpty(info.AdviceURL))
                throw new Exception("服务器通知地址不能为空");
            else if (string.IsNullOrEmpty(info.orderTime))
                throw new Exception("请求时间不能为空");
            else if (string.IsNullOrEmpty(info.MD5Key))
                throw new Exception("密钥不能为空");
            if (string.IsNullOrEmpty(info.defaultBankNumber))
                info.defaultBankNumber = "UNIONPAY";
            info.Amount = Convert.ToDecimal(info.Amount).ToString("N2").Replace(",", "");
            var signMsg = Encipherment.MD5(info.MerNo + "&" + info.BillNo + "&" + info.Amount + "&" + info.ReturnURL + "&" + info.MD5Key).ToUpper();
            string md5src = "MerNo=" + info.MerNo + "&" + "BillNo=" + info.BillNo + "&" + "Amount=" + info.Amount + "&" + "OrderTime=" + info.orderTime + "&" + "ReturnURL=" + info.ReturnURL + "&" + "AdviceURL=" + info.AdviceURL + "&" + info.MD5Key;
            //string SignInfo = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(md5src, "MD5");

            string SignInfo = Encipherment.MD5(md5src).ToUpper();
            StringBuilder strBud = new StringBuilder();

            strBud.Append("<form name='dinpayForm' method='post' action=" + info.PayUrl + " target='_self'>");
            //strBud.Append("<input type='hidden' name='OrderDesc' value='" + info.OrderDesc + "' />");
            strBud.Append("<input type='hidden' name='MerNo' value='" + info.MerNo + "' />");
            strBud.Append("<input type='hidden' name='BillNo' value='" + info.BillNo + "' />");
            strBud.Append("<input type='hidden' name='Amount' value='" + info.Amount + "'/>");
            strBud.Append("<input type='hidden' name='ReturnURL' value='" + info.ReturnURL + "'/>");
            strBud.Append("<input type='hidden' name='AdviceURL' value='" + info.AdviceURL + "'/>");
            strBud.Append("<input type='hidden' name='SignInfo' value='" + SignInfo + "'/>");
            strBud.Append("<input type='hidden' name='OrderTime' value='" + info.orderTime + "'/>");
            strBud.Append("<input type='hidden' name='defaultBankNumber' value='" + info.defaultBankNumber + "'>");
            //strBud.Append("<input type='hidden' name='payType' value='" + info.PayType + "'/>");
            strBud.Append("<input type='hidden' name='Remark' value='" + info.Remark + "'/>");
            strBud.Append("<input type='hidden' name='products' value='" + info.products + "'/>");
            strBud.Append("</form>");
            strBud.Append("<script language=javascript>document.dinpayForm.submit();</script>");
            return strBud.ToString();
        }
        /// <summary>
        /// 检查签名
        /// </summary>
        public string CheckSign_HuiChao(string billNo,string amount,string success,string md5Key)
        {
            var sign = Encipherment.MD5(billNo + "&" + amount + "&" + success + "&" + md5Key).ToUpper();
            return sign;
        }
    }
}
