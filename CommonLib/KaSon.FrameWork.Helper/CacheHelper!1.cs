namespace KaSon.FrameWork.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class CacheHelper<T>
    {
        private readonly double cache_minute;
        private Dictionary<string, CacheObject> cacheDicts;
        private readonly object lockobj;

        public CacheHelper(int minute = 10)
        {
            this.cache_minute = 10.0;
            this.lockobj = new object();
            this.cacheDicts = new Dictionary<string, CacheObject>();
            this.cache_minute = (minute < 0) ? ((double) 0) : ((double) minute);
        }

        public void Clear()
        {
            if (this.cacheDicts != null)
            {
                lock (this.lockobj)
                {
                    if (this.cacheDicts != null)
                    {
                        this.cacheDicts.Clear();
                    }
                }
            }
        }

        public T Get(string key)
        {
            if (this.cache_minute <= 0.0)
            {
                return default(T);
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            CacheObject obj2 = this.cacheDicts.ContainsKey(key) ? this.cacheDicts[key] : null;
            if (obj2 != null)
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - obj2.CacheTime);
                if (span.TotalMinutes > this.cache_minute)
                {
                    lock (this.lockobj)
                    {
                        this.cacheDicts[key] = null;
                        obj2 = null;
                    }
                }
            }
            return ((obj2 == null) ? default(T) : ((T) obj2.CacheData));
        }

        public void Set(string key, T data)
        {
            if (this.cache_minute > 0.0)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException("key");
                }
                if (data != null)
                {
                    lock (this.lockobj)
                    {
                        CacheObject obj2 = new CacheObject {
                            CacheData = data,
                            CacheTime = DateTime.Now
                        };
                        this.cacheDicts[key] = obj2;
                    }
                }
            }
        }

        public Dictionary<string, CacheObject> CacheDicts
        {
            get
            {
                return this.cacheDicts;
            }
        }
    }
}

