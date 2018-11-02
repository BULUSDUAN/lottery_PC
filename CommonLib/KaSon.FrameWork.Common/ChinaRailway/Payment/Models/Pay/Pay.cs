using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class PayProduct
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class PayClient
    {
        public string Ip { get; set; }
        public string Ua { get; set; }
    }

    public class Pay
    {
        public ushort Type { get; set; }
        public ulong Channel { get; set; }

        public string Order { get; set; }

        public ulong Currency { get; set; }
        public decimal Amount { get; set; }

        public ulong Time { get; set; }
        public uint Timeout { get; set; }

        public PayProduct Product { get; set; }
        public PayClient Client { get; set; }

        public string Remark { get; set; }
    }

    public class PayBank : Pay
    {
        public PayBank(ulong channel, BankAccountType type, string name, string no)
        {
            Type = 1;
            Channel = channel;
            AccountType = type;
            Name = name;
            No = no;
        }

        public BankAccountType AccountType { get; set; }
        public string Name { get; set; }
        public string No { get; set; }
    }

    public class PayAlipayTransfer : Pay
    {

    }

    public class PayAlipayEnvelope : Pay
    {

    }

    public class PayWepayTransfer : Pay
    {

    }

    public class PayWepayEnvelope : Pay
    {

    }
}
