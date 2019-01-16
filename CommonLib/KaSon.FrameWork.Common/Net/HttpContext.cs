using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KaSon.FrameWork.Common.Net
{

    public class MyHttpContext
    {
        public static IServiceProvider ServiceProvider;

        public MyHttpContext()
        {
        }


        public static HttpContext Current(IServiceProvider sp)
        {
            ServiceProvider = sp;
            object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

            HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
            return context;

        }
    }

    public class HttpContextAccessor1 : IHttpContextAccessor
    {

        private AsyncLocal<HttpContext> _httpContextCurrent = new AsyncLocal<HttpContext>();
        public HttpContext HttpContext
        {
            get
            {
                return _httpContextCurrent.Value;
            }
            set
            {
                _httpContextCurrent.Value = value;
            }
        }

    }
}
