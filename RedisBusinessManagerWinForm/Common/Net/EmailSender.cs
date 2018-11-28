using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.ComponentModel;


namespace Common.Net
{
    /// <summary>
    /// 邮件发送
    /// </summary>
    public class EmailSender
    {
        /// <summary>
        /// SMPT服务器地址
        /// </summary>
        public string Smtp { get; set; }
        /// <summary>
        /// 邮件发送账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 邮件发送密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮件显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public void SendEmail(string toAddress, string title, string content, string cc = "")
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = Smtp;
            smtpClient.Credentials = new NetworkCredential(Account, Password);
            MailMessage mm = new MailMessage();
            mm.Priority = MailPriority.High;
            mm.From = new MailAddress(Account, DisplayName, Encoding.UTF8);
            mm.To.Add(toAddress);
            if (!string.IsNullOrEmpty(cc))
                mm.CC.Add(cc);
            mm.Subject = title;
            mm.SubjectEncoding = Encoding.UTF8;
            mm.IsBodyHtml = true;
            mm.BodyEncoding = Encoding.UTF8;
            mm.Body = content;

            //附件信息
            //AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(content, null, "text/html");
            //LinkedResource lrImage = new LinkedResource("a.jpg", "image/gif");
            //lrImage.ContentId = "Email001";
            //htmlBody.LinkedResources.Add(lrImage);
            //mm.AlternateViews.Add(htmlBody);


            smtpClient.Send(mm);


        }
    }
}
