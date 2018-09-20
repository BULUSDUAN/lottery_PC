using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace app.lottery.site.cbbao.Models
{
    /// <summary>
    /// 彩种开奖号对象
    /// </summary>
    public class GameWinNumberInfo
    {
        public string CreateTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public string WinNumber { get; set; }
    }
}