using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class PaySucceeded
    {
        public ushort Type { get; set; }
        public ulong Channel { get; set; }

        public string Order { get; set; }
        public string Transaction { get; set; }

        public ulong Currency { get; set; }

        public decimal Amount { get; set; }
        public decimal Fee { get; set; }

        public ulong Time { get; set; }
        public ulong? TimeClose { get; set; }
        public ulong? TimeFinish { get; set; }

        public PayProduct Product { get; set; }

        public uint Status { get; set; }
        public string Remark { get; set; }
    }
}
