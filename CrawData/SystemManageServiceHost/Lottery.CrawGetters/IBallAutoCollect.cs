using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///  球采集接口
    /// </summary>
   public interface IAutoCollect
    {
         string Category { get; set; }
        string Key { get; set; }
         void Start();
        void Stop();

    
    }
}
