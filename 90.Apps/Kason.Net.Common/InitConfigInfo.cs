﻿using log4net.Repository.Hierarchy;

using System;
using System.Collections.Generic;
using System.Text;

namespace Kason.Net.Common
{
    /// <summary>
    /// 务必初始化这个
    /// </summary>
    public class InitConfigInfo
    {

        public static string data_spider_proxy_url { get; set; }
    
        public static ILoggerFactory logFactory { get; set; }
    }
}