namespace KaSon.FrameWork.Common
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public sealed class HttpResponseInfo
    {
        public HttpWebResponse Response { get; set; }

        public string Result { get; set; }
    }
}

