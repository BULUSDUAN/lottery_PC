using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 14场胜负/胜负任9
    /// </summary>
     
    public class CTZQ_T14C_GameWinNumber_Info : ImportInfoBase
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }

    /// <summary>
    /// 6场半全
    /// </summary>
     
    public class CTZQ_T6BQC_GameWinNumber_Info : ImportInfoBase
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }

    /// <summary>
    /// 4场进球
    /// </summary>
     
    public class CTZQ_T4CJQ_GameWinNumber_Info : ImportInfoBase
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
    }
}
