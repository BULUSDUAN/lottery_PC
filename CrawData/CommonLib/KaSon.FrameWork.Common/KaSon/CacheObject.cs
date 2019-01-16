namespace KaSon.FrameWork.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class CacheObject
    {
        public object CacheData { get; set; }

        public DateTime CacheTime { get; set; }
    }
}

