using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models.Transfer
{
    public class Transfer
    {
        public ushort Type { get; set; }
        public string TransferId { get; set; }

        public ulong Currency { get; set; }
        public decimal Amount { get; set; }

        public ulong Time { get; set; }
        public uint Timeout { get; set; }

        public string Remark { get; set; }
    }
}
