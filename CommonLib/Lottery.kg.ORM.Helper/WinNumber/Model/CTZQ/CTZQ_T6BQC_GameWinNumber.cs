using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    public class CTZQ_T6BQC_GameWinNumber : GameWinNumberBase
    {
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
    }
}
