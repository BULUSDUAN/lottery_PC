using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters.MatchBizGetter
{
    /// <summary>
    ///  球采集接口
    /// </summary>
   public interface IBallAutoCollect
    {
         string Category { get; set; }
        string Key { get; set; }
         void Start(string gameCode);
        void Stop();

        void WriteLog(string log);
        void WriteError(string log);
    }
}
