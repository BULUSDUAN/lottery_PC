using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters.Auto
{
    public interface ISZCWinNumberCrawler
    {
        Dictionary<string, string> Process(string gameCode);
    }
}
