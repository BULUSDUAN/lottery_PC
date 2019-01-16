using Autofac;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawHost
{
    public class ServiceBuilder
    {
       public static IContainer Local { get; set; }
        public static ILoggerFactory LoggerFactory { get; set; }
    }
}
