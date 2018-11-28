using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Gateway.YiShen
{
    /// <summary>
    /// 银盛支付
    /// </summary>
    public class YiShenAPI
    {
        /// <summary>
        /// 返回支付提交Form
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetHuiChaoForm(YiShenInfo info)
        {
            if (string.IsNullOrEmpty(info.PayeeUserCode))
                throw new Exception("商户号不能为空");
            if (string.IsNullOrEmpty(info.OrderId))
                throw new Exception("订单号不能为空");

            string xmlInfo = GetXml(info);
            string msgId = "IQC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string check = SignData.signData(xmlInfo, info.Pfxpath, info.Pfxpassword);
            string msg = Convert.ToBase64String(Encoding.GetEncoding("GBK").GetBytes(xmlInfo));

            //String path = string.Format("{0}\\test.log", AppDomain.CurrentDomain.BaseDirectory);
            //WriteFile(path, xmlInfo);
            
            StringBuilder postForm_ys_bank = new StringBuilder();
            postForm_ys_bank.AppendFormat("<form name=\"form1\" id=\"form1\" method=\"post\" action=\"{0}\">", info.YSPay_Url);
            postForm_ys_bank.AppendFormat("<input type=\"hidden\" name=\"src\" value=\"{0}\"/>", info.Src);
            postForm_ys_bank.AppendFormat("<input type=\"hidden\" name=\"msgCode\" value=\"{0}\"/>", info.MsgCode);//S3001
            postForm_ys_bank.AppendFormat(" <input type=\"hidden\" name=\"check\" value=\"{0}\"/>", check);
            postForm_ys_bank.AppendFormat("<input type=\"hidden\" name=\"msgId\" value=\"{0}\"/>", msgId);
            postForm_ys_bank.AppendFormat("<input type=\"hidden\" name=\"msg\" value=\"{0}\"/>", msg);
            postForm_ys_bank.Append("</form>");
            postForm_ys_bank.Append("<script>document.getElementById(\"form1\").submit();</script>");
            //postForm_ys_bank.Append("<script type=\"text/javascript\" language=\"javascript\">setTimeout(\"document.getElementById('form1').submit();\",100);</script>");
            return postForm_ys_bank.ToString();
        }


        private string GetXml(YiShenInfo info)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
            xml.Append("<yspay>");
            xml.Append("<head>");
            xml.Append("<Ver>1.0</Ver>");
            xml.AppendFormat("<Src>{0}</Src>", info.Src);
            xml.AppendFormat("<MsgCode>{0}</MsgCode>", info.MsgCode);
            xml.AppendFormat("<Time>{0}</Time>", info.Time);
            xml.Append("</head>");
            xml.Append("<body>");
            xml.Append("<Order>");
            xml.AppendFormat("<OrderId>{0}</OrderId>", info.OrderId);
            xml.AppendFormat("<BusiCode>{0}</BusiCode>", info.BusiCode);
            xml.AppendFormat("<ShopDate>{0}</ShopDate>", info.ShopDate);
            xml.AppendFormat("<Cur>{0}</Cur>", info.Cur);
            xml.AppendFormat("<Amount>{0}</Amount>", info.Amount);
            xml.AppendFormat("<Note>{0}</Note>", info.Note);
            xml.AppendFormat("<ExtraData>{0}</ExtraData>", info.ExtraData);
            xml.AppendFormat("<Remark>{0}</Remark>", info.Remark);
            xml.AppendFormat("<BankType>{0}</BankType>", info.BankType);
            xml.AppendFormat("<BankAccountType>{0}</BankAccountType>", info.BankAccountType);
            xml.AppendFormat("<Timeout>{0}</Timeout>", info.Timeout);
            xml.AppendFormat("<SupportCards>{0}</SupportCards>", info.SupportCards);
            xml.Append("</Order>");
            xml.Append("<Payee>");
            xml.AppendFormat("<UserCode>{0}</UserCode>", info.PayeeUserCode);
            xml.AppendFormat("<Name>{0}</Name>", info.PayeeName);
            xml.AppendFormat("<PhoneNum>{0}</PhoneNum>", info.PhoneNum);
            xml.AppendFormat("<Amount>{0}</Amount>", info.Amount);
            xml.Append("</Payee>");
            xml.Append("<Payer>");
            xml.AppendFormat("<UserCode>{0}</UserCode>", info.PayerUserCode);
            xml.AppendFormat("<Name>{0}</Name>", info.PayerName);
            xml.Append("</Payer>");
            xml.Append("<Notice>");
            xml.AppendFormat("<PgUrl>{0}</PgUrl>", info.noticePgUrl);
            xml.AppendFormat("<BgUrl>{0}</BgUrl>", info.noticeBgUrl);
            xml.Append("</Notice>");
            xml.Append("</body>");
            xml.Append("</yspay>");
            return xml.ToString();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public void WriteFile(string Path, string Strings)
        {

            if (!System.IO.File.Exists(Path))
            {
                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
                f.Dispose();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
            f2.WriteLine(Strings);
            f2.Close();
            f2.Dispose();


        }
    }
}
