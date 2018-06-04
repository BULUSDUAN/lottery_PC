namespace KaSon.FrameWork.Helper
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class HttpRequestInfo
    {
        public HttpRequestInfo()
        {
            this.Encoding = System.Text.Encoding.UTF8;
            this.TimeOut = 100;
            this.UserAgent = "HttpHelper";
        }

        public string Arguments { get; set; }

        public string Domain { get; set; }

        public System.Text.Encoding Encoding { get; set; }

        public int TimeOut { get; set; }

        public string URI { get; set; }

        public string UserAgent { get; set; }
    }
}

