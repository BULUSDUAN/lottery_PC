using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.DrawingCore;

namespace KaSon.FrameWork.Common.Net
{
    /// <summary>
    /// 请求网络接口方式
    /// </summary>
    public static class PostManager
    {
        /// <summary>
        /// 使用 Socket 方式，伪造请求信息，对指定页面发送 Post 请求，并返回响应文本。特定环境下使用
        /// </summary>
        /// <param name="host">服务器。如：www.baidu.com</param>
        /// <param name="port">请求端口</param>
        /// <param name="postPage">请求页面</param>
        /// <param name="arg">请求的参数字符串</param>
        /// <returns>远程响应文本</returns>
        public static string PostBySocket(string host, int port, string postPage, string arg)
        {
            IPAddress[] ips = Dns.GetHostAddresses(host);
            Byte[] bytesReceived = new byte[24000];
            for (int i = 0; i < ips.Length; i++)
            {
                IPEndPoint ipe = new IPEndPoint(ips[i], port);
                using (Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    s.Connect(ipe);
                    if (s.Connected)
                    {
                        StringBuilder buf = new StringBuilder();
                        buf.AppendLine(string.Format("POST {0} HTTP/1.1", postPage));
                        buf.AppendLine("Host:" + host);
                        buf.AppendLine("User-Agent:Mozilla/5.0 (Windows; U; Windows NT 6.0; zh-CN; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12 ( .NET CLR 3.5.30729; .NET4.0E)");
                        buf.AppendLine("Accept:ext/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                        buf.AppendLine("Accept-Language:zh-cn,zh;q=0.5");
                        buf.AppendLine("Accept-Encoding:gzip,deflate");
                        buf.AppendLine("Accept-Charset:GB2312,utf-8;q=0.7,*;q=0.7");
                        buf.AppendLine("Keep-Alive:115");
                        buf.AppendLine("Connection:keep-alive");
                        buf.AppendLine("Content-Type:application/x-www-form-urlencoded; charset=UTF-8");
                        buf.AppendLine("Referer:http://www.shishicai.cn/Lottery/Speed/FCCQSSC/Base.aspx");
                        buf.AppendLine("Content-Length:177");
                        buf.AppendLine("");
                        buf.AppendLine(string.Format("{0}&submit=submit", arg));

                        s.Send(Encoding.ASCII.GetBytes(buf.ToString()));
                        int rint = s.Receive(bytesReceived);
                        string r = Encoding.ASCII.GetString(bytesReceived, 0, rint);

                        Array.Clear(bytesReceived, 0, bytesReceived.Length);
                        s.Disconnect(false);
                        return r;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 通过 HttpRequest 对象，对指定 Url 发起 Post 请求，并返回响应文本
        /// </summary>
        /// <param name="url">请求 Url 地址</param>
        /// <param name="requestString">请求参数字符串</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeoutSeconds">设置请求超时秒数，传入正整数有效，否则为默认值。默认值：0</param>
        /// <returns>远程响应文本</returns>
        public static string Post(string url, string requestString, Encoding encoding, int timeoutSeconds = 0, WebProxy proxy = null, string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (proxy != null) request.Proxy = proxy;
                if (timeoutSeconds > 0)
                {
                    request.Timeout = 0x3e8 * timeoutSeconds;
                }
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.ContentType = contentType;
                request.ServicePoint.Expect100Continue = false;
                requestString = System.Net.WebUtility.HtmlDecode(requestString);
                byte[] bytes = encoding.GetBytes(requestString);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                return reader.ReadToEnd();
            }
            catch (System.Net.WebException ex)
            {
                var exr= (HttpWebResponse)ex.Response;
                StreamReader srr = new StreamReader(exr.GetResponseStream(), encoding);
                throw new Exception(srr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string Post_Head(string url, string requestString, Encoding encoding, int timeoutSeconds = 0, WebProxy proxy = null, string contentType = "application/x-www-form-urlencoded",string refer="", WebHeaderCollection Head=null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(requestString);

                request.Method = "POST";
                request.Accept = "*/*";
                request.Referer = refer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
               //request.Host = "6hch.com";
                request.ContentLength = bytes.Length;
                System.Net.ServicePointManager.Expect100Continue = false;

                //request.Headers.Add("Accept-Language", "zh-cn");
                //request.Headers.Add("Accept-Encoding", "gzip, deflate");
                //request.Headers.Add("Pragma", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);

                return reader.ReadToEnd();
            }
            catch (System.Net.WebException ex)
            {
                return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// post请求到指定地址并获取返回的信息内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求参数</param>
        /// <param name="encodeType">编码类型如：UTF-8</param>
        /// <returns>返回响应内容</returns>
        public static string HttpPost(string URL, string strPostdata, string strEncoding)
        {
            HttpWebResponse response = null;
            try
            {
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "post";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/json"; //application/x-www-form-urlencoded
                                                          //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
                byte[] buffer = encoding.GetBytes(strPostdata);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding));
            return sr.ReadToEnd();
        }


        public static string PostCustomer(string url, string requestString, Encoding encoding, Action<HttpWebRequest> requestHandler = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ServicePoint.Expect100Continue = false;
               
                if (requestHandler != null) requestHandler(request);

                requestString = System.Net.WebUtility.HtmlDecode(requestString);
                byte[] bytes = encoding.GetBytes(requestString);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                return reader.ReadToEnd();
            }
            catch (System.Net.WebException ex)
            {
                return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string Post(string host, string referer, string url, string postData, CookieContainer cc)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.Accept = "*/*";
                request.Referer = referer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Host = host;
                request.ContentLength = bytes.Length;
                System.Net.ServicePointManager.Expect100Continue = false;

                request.Headers.Add("Accept-Language", "zh-cn");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Pragma", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

                return reader.ReadToEnd();
            }
            catch (System.Net.WebException ex)
            {
                return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 通过 HttpRequest 对象，对指定 Url 发起 Get 请求，并返回响应文本
        /// </summary>
        /// <param name="url">请求 Url 地址</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeoutSeconds">设置请求超时秒数，传入正整数有效，否则为默认值。默认值：0</param>
        /// <param name="action">处理Http请求</param>
        /// <returns>远程响应文本</returns>
        public static string Get(string url, Encoding encoding, int timeoutSeconds = 0, Action<HttpWebRequest> requestHandler = null)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (timeoutSeconds > 0)
                {
                    request.Timeout = 0x3e8 * timeoutSeconds;
                }
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                System.Net.CookieContainer c = new System.Net.CookieContainer();
                request.CookieContainer = c;
                if (requestHandler != null) requestHandler(request);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                return reader.ReadToEnd();

            }
            catch (System.Net.WebException ex)
            {
                return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static string GetCaiLeLe(string url, Encoding encoding, int timeoutSeconds = 0, Action<HttpWebRequest> requestHandler = null)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (timeoutSeconds > 0)
                {
                    request.Timeout = 0x3e8 * timeoutSeconds;
                }
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                //request.Host = "kjh.cailele.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:27.0) Gecko/20100101 Firefox/27.0";
                if (requestHandler != null) requestHandler(request);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                return reader.ReadToEnd();

            }
            catch (System.Net.WebException ex)
            {
                return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static CookieContainer GetCookie(string url)
        {
            CookieContainer cc = new CookieContainer();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.AllowAutoRedirect = true;
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

            request.Headers.Add("Accept-Language", "zh-CN");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            request.CookieContainer = new CookieContainer();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

            foreach (Cookie cookie in response.Cookies)
            {
                cc.Add(cookie);
            }

            return cc;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        public static void DownloadImage(string url, string saveFileName, int timeoutSeconds = 0)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (timeoutSeconds > 0)
            {
                request.Timeout = 0x3e8 * timeoutSeconds;
            }
            var response = request.GetResponse();
            long cLength = response.ContentLength;

            if (cLength > 0)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (Image img = Image.FromStream(stream))
                    {
                        img.Save(saveFileName);
                    }
                }
            }
            else
            {
                throw new Exception("文件过大或过小 - " + cLength);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public static string UploadFile(string url, string fileFullPath, Dictionary<string, string> customerHead)
        {
            if (customerHead == null || customerHead.Count == 0)
                throw new Exception("customerHead不能为空");

            // 直接上传，并获取返回的二进制数据.
            WebClient client = new WebClient();
            foreach (var item in customerHead)
            {
                client.Headers.Add(item.Key, item.Value);
            }
            byte[] responseArray = client.UploadFile(url, WebRequestMethods.Http.Post, fileFullPath);
            var r = Encoding.UTF8.GetString(responseArray);
            return r;
        }
        //验证服务器证书
        public static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }
        public static string PostCustomerHttps(string url, string requestString, Encoding encoding, Action<HttpWebRequest> requestHandler = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ServicePoint.Expect100Continue = false;
                if (requestHandler != null) requestHandler(request);

                requestString = System.Net.WebUtility.HtmlDecode(requestString);
                byte[] bytes = encoding.GetBytes(requestString);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                return reader.ReadToEnd();
            }
            catch (System.Net.WebException ex)
            {
                return ex.ToString();
                //return "404";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
