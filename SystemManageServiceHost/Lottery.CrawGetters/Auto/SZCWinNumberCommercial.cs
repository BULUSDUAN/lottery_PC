using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters.Auto
{
    public class SZCWinNumberCommercial : ISZCWinNumberCrawler
    {
        public Dictionary<string, string> Process(string gameCode)
        {
            return new WinNumberGetter_KaiCaiWang().GetWinNumber(gameCode, 0);
            //return WinNumberGetter.CreateInstance("kaicaiwang").GetWinNumber(gameCode, 0);
        }
    }
}
