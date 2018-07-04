using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 传统足球开奖号
    /// </summary>
    public class CTZQ_T14C_GameWinNumber : GameWinNumberBase
    {
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
    }
}
