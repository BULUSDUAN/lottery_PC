using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Drawing;
using Common.Net;

namespace Common.Snapshot
{
   public class SendSchemeSnapshot
    {

        #region 投注成功后发送快照到邮箱,2014.11.10 dj

        /// <summary>
        /// 发送方案快照到邮箱
        /// </summary>
       public bool SendSchemeSnapshotToEmail(string schemeAddress, string toAddress, string emailSmtp, string emailAccount, string emailDisplayName, string emailPassword, string emailTitle)
       {
           try
           {
               if (string.IsNullOrEmpty(schemeAddress) || string.IsNullOrEmpty(toAddress) || string.IsNullOrEmpty(emailSmtp) || string.IsNullOrEmpty(emailAccount) || string.IsNullOrEmpty(emailDisplayName) || string.IsNullOrEmpty(emailPassword) || string.IsNullOrEmpty(emailTitle))
                   return false;
               if (!IsUrlEffective(schemeAddress)) return false;
               WebPageSnapshot wps = new WebPageSnapshot();
               wps.Url = schemeAddress;
               var bmp = wps.TakeSnapshot();
               if (bmp == null)
                   return false;
               string imgUrl = UpLoadImgToServer(bmp);
               if (string.IsNullOrEmpty(imgUrl))
                   return false;
               //发送图片到指定邮箱
               var content = "你在彩宝宝的投注订单为：请及时关注.<img src='" + imgUrl + "'/>";
               var sender = new EmailSender();
               sender.Smtp = emailSmtp;
               sender.Account = emailAccount;
               sender.DisplayName = emailDisplayName;
               sender.Password = emailPassword;
               sender.SendEmail(toAddress, emailTitle, content);
               return true;
           }
           catch
           {
               return false;
           }
       }
       /// <summary>
       /// 判断url是否有效
       /// </summary>
       /// <param name="Url"></param>
       /// <returns></returns>
       private bool IsUrlEffective(string Url)
       {
           WebRequest myWebRequest = WebRequest.Create(Url);
           myWebRequest.Timeout = 1000 * 15;
           try
           {
               WebResponse myWebResponse = myWebRequest.GetResponse();
               Stream resStream = myWebResponse.GetResponseStream();
               StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
               var result = sr.ReadToEnd();
               resStream.Close();
               sr.Close();
               return true;
           }
           catch (System.Net.WebException ex)
           {
               return false;
           }
       }
        /// <summary>
        /// 上传图片到资源站点
        /// </summary>
        private string UpLoadImgToServer(Bitmap btm)
        {
            string uploadfile = "/UpLoad/uploadimages/";
            string nameImg = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".jpg";

            string resourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl"].ToString();

            string upLoadFile = uploadfile;
            string upLoadPostPath = ConfigurationManager.AppSettings["UpLoadPostPath"].ToString();

            string url = string.Format("{0}{1}{2}", resourceSiteUrl, upLoadFile, nameImg);

            upLoadFile = "/" + ConfigurationManager.AppSettings["WebSiteEName"].ToString() + upLoadFile;

            string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSiteUrl, upLoadPostPath, nameImg, upLoadFile);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ContentType = "multipart/form-data";
            MemoryStream ms = new MemoryStream();
            btm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
            return url;
        }

        #endregion
    }
}
