using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public enum BankAccountType : byte
    {
        DebitCard = 1,
        CreditCard
    }

    public class ChargeProduct
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class ChargeClient
    {
        public string Ip { get; set; }
        public string Ua { get; set; }
    }

    public class Charge
    {
        public ushort Type { get; set; }
        public ulong Channel { get; set; }

        public string Order { get; set; }

        public ulong Currency { get; set; }

        public decimal Amount { get; set; }

        public ulong Time { get; set; }
        public uint Timeout { get; set; }

        public ChargeClient Client { get; set; }

        public ChargeProduct Product { get; set; }

        public string Remark { get; set; }
    }

    public class ChargeBank : Charge
    {
        public ChargeBank()
        {
            Type = 1;
            Channel = 0;
            AccountType = BankAccountType.DebitCard;
        }

        public ChargeBank(ulong channel, BankAccountType type)
        {
            Type = 1;
            Channel = channel;
            AccountType = type;
        }

        public BankAccountType AccountType { get; set; }
        public string ResultUrl { get; set; }
    }

    public class ChargeBankEasy : Charge
    {
        public ChargeBankEasy()
        {
            Type = 2;
            Channel = 0;
            AccountType = BankAccountType.DebitCard;
        }

        public ChargeBankEasy(ulong channel, BankAccountType type, string no)
        {
            Type = 2;
            Channel = channel;
            AccountType = type;
            No = no;
        }

        public BankAccountType AccountType { get; set; }
        public string No { get; set; }
        public string ResultUrl { get; set; }

        public string name { get; set; }

        public string cert_id { get; set; }

        public int cert_type { get; set; }

        public string mobile { get; set; }
    }

    public class ChargeAlipayMicro : Charge
    {
        public ChargeAlipayMicro()
        {
            Type = 3;
            Channel = 1;
        }
    }

    public class ChargeAlipayNative : Charge
    {
        public ChargeAlipayNative()
        {
            Type = 3;
            Channel = 2;
        }
    }

    public class ChargeAlipayApp : Charge
    {
        public ChargeAlipayApp()
        {
            Type = 3;
            Channel = 3;
        }
    }

    public class ChargeWepayMicro : Charge
    {
        public ChargeWepayMicro()
        {
            Type = 4;
            Channel = 1;
        }
    }

    public class ChargeWepayNative : Charge
    {
        public ChargeWepayNative()
        {
            Type = 4;
            Channel = 2;
        }
    }

    public class ChargeWepayApp : Charge
    {
        public ChargeWepayApp()
        {
            Type = 4;
            Channel = 3;
        }
    }

    public class ChargeUnipayMicro : Charge
    {
        public ChargeUnipayMicro()
        {
            Type = 5;
            Channel = 1;
        }
    }

    public class ChargeUnipayNative : Charge
    {
        public ChargeUnipayNative()
        {
            Type = 5;
            Channel = 2;
        }
    }

    public class ChargeUnipayApp : Charge
    {
        public ChargeUnipayApp()
        {
            Type = 5;
            Channel = 3;
        }
    }

    public class ChargeQQpayMicro : Charge
    {
        public ChargeQQpayMicro()
        {
            Type = 6;
            Channel = 1;
        }
    }

    public class ChargeQQpayNative : Charge
    {
        public ChargeQQpayNative()
        {
            Type = 6;
            Channel = 2;
        }
    }

    public class ChargeQQpayApp : Charge
    {
        public ChargeQQpayApp()
        {
            Type = 6;
            Channel = 3;
        }
    }
}
