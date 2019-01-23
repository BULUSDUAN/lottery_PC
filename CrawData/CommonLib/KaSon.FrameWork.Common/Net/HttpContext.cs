﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.Net
{

    public static class MyHttpContext
    {
        public static IServiceProvider ServiceProvider;

        static MyHttpContext()
        { }


        public static HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }

}