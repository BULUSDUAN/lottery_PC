using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lottery.CrawGetters
{
    public interface IWinNumberGetter
    {
        /// <summary>
        /// 获取Cookies
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
          Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
          string issuseNumber = "");
        WinNumberGetter CreateInstance(string interfaceType);
    }
}
