using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class PayResult
    {
        public ushort Type { get; set; }
        public string Payment { get; set; }
        public string Transaction { get; set; }

        public ulong Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountRefund { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public ulong Time { get; set; }
        public ulong? TimeClose { get; set; }
        public ulong? TimeFinish { get; set; }

        public string Remark { get; set; }

        public uint Status { get; set; }
    }

    public class PayBankResult : PayResult
    {

    }

    public class PayAlipayTransferResult : PayResult
    {

    }

    public class PayAlipayEnvelopeResult : PayResult
    {

    }

    public class PayWepayTransferResult : PayResult
    {

    }

    public class PayWepayEnvelopeResult : PayResult
    {

    }
}
