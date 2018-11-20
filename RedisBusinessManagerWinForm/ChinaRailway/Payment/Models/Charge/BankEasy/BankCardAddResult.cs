using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models.Charge.BankEasy
{
    public class BankCardAddResult
    {
        public ulong Track { get; set; }

        public uint Status { get; set; }
    }
}
