namespace Kason.Net.Common
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;

    public sealed class HttpHelper
    {
        private bool _needLogin;
        private string _tokenValue;
        private string _userName;

        public HttpHelper()
        {
            this._needLogin = false;
            this._tokenValue = "";
            this._userName = "";
            this._needLogin = false;
        }

        public HttpHelper(string domain, string username, string password) : this(domain, username, password, "Auth/Login")
        {
        }

        public HttpHelper(string domain, string username, string password, string loginurl)
        {
            this._needLogin = false;
            this._tokenValue = "";
            this._userName = "";
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentException("domain为空");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username为空");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("password为空");
            }
            if (string.IsNullOrWhiteSpace(loginurl))
            {
                throw new ArgumentException("loginurl为空");
            }
            string str = string.Format("{0}?user={1}&password={2}", loginurl, username, password);
            HttpRequestInfo info = new HttpRequestInfo {
                Domain = domain,
                URI = str
            };
            this._needLogin = true;
            this._userName = username;
            this._tokenValue = this.GetToken(info);
            if (ServicePointManager.DefaultConnectionLimit < 0x200)
            {
                ServicePointManager.DefaultConnectionLimit = 0x200;
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private HttpWebRequest CreateGetRequest(HttpRequestInfo ri)
        {
            HttpWebRequest request = WebRequest.Create(ri.Domain + "/" + ri.URI) as HttpWebRequest;
            if (ri.Domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version10;
            }
            request.Method = "GET";
            this.SetRequest(request, ri);
            return request;
        }

        private HttpWebRequest CreatePostRequest(HttpRequestInfo ri)
        {
            HttpWebRequest request = WebRequest.Create(ri.Domain + "/" + ri.URI) as HttpWebRequest;
            if (ri.Domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version10;
            }
            request.Method = "POST";
            this.SetRequest(request, ri);
            byte[] bytes = ri.Encoding.GetBytes(ri.Arguments);
            request.ContentLength = bytes.Length;
            request.Accept = "application/json";
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            return request;
        }

        private HttpWebRequest CreateRequest(HttpRequestInfo ri)
        {
            if (string.IsNullOrWhiteSpace(ri.URI))
            {
                throw new ArgumentException("请求的URI为空");
            }
            if (string.IsNullOrWhiteSpace(ri.Domain))
            {
                throw new ArgumentException("请求的Domain为空");
            }
            string str = ri.Domain.TrimEnd(new char[] { '/' });
            if (!str.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                ri.Domain = "http://" + str;
            }
            return (string.IsNullOrWhiteSpace(ri.Arguments) ? this.CreateGetRequest(ri) : this.CreatePostRequest(ri));
        }

        public HttpResponseInfo GetResponse(HttpRequestInfo ri)
        {
            string str;
            HttpWebResponse response = this.GetResponse(ri, out str);
            return new HttpResponseInfo { Response = response, Result = str };
        }

        private HttpWebResponse GetResponse(HttpRequestInfo ri, out string result)
        {
            HttpWebResponse response;
            StreamReader reader;
            result = null;
            HttpWebRequest request = this.CreateRequest(ri);
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                if (response == null)
                {
                    return null;
                }
                if (!(!this._needLogin || string.IsNullOrWhiteSpace(response.Headers["Token"])))
                {
                    this._tokenValue = response.Headers["Token"];
                }
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return null;
                    }
                    using (reader = new StreamReader(stream, ri.Encoding))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                return response;
            }
            catch (WebException exception)
            {
                response = exception.Response as HttpWebResponse;
                using (reader = new StreamReader(response.GetResponseStream(), ri.Encoding))
                {
                    result = reader.ReadToEnd();
                }
                return response;
            }
        }

        private string GetToken(HttpRequestInfo _loginRI)
        {
            string str;
            this.GetResponse(_loginRI, out str);
            if (string.IsNullOrWhiteSpace(str))
            {
                string str2 = Path.Combine(_loginRI.Domain, _loginRI.URI);
                throw new ApplicationException(string.Format("登录失败,{0}", str2));
            }
            return str;
        }

        private void SetRequest(HttpWebRequest request, HttpRequestInfo ri)
        {
            request.AllowAutoRedirect = false;
            request.UserAgent = ri.UserAgent;
            request.ContentType = "application/json";
            if (!(!this._needLogin || string.IsNullOrWhiteSpace(this._tokenValue)))
            {
                request.Headers["Token"] = this._tokenValue;
                request.Headers["User"] = this._userName;
            }
            if (ri.TimeOut > 0)
            {
                request.Timeout = ri.TimeOut * 0x3e8;
            }
        }
    }
}

