using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models.Transfer
{
    public class TransferResult
    {
        public ushort Type { get; set; }
        public string Transfer { get; set; }
        public string Transaction { get; set; }

        public ulong Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountRefund { get; set; }

        public ulong Time { get; set; }
        public ulong? TimeClose { get; set; }
        public ulong? TimeFinish { get; set; }

        public string Remark { get; set; }

        public uint Status { get; set; }
    }
}
