using Kason.Sg.Core.Caching.HashAlgorithms;
using System;

namespace Lottery.ApiGateway.Model
{
    public class CacheEndpointParam
    {
        public string CacheId { get; set; }

        public string Endpoint { get; set; }

       public ConsistentHashNode CacheEndpoint { get; set; }
    }
}
