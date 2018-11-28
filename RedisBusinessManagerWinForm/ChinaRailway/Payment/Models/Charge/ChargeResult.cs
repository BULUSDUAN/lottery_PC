using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models.Charge
{
    public class ChargeCredential
    {
        public ulong Type { get; set; }
        public string Content { get; set; }
    }

    public class ChargeResult
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

        public ChargeProduct Product { get; set; }
        public ChargeClient Client { get; set; }
        public ChargeCredential Credential { get; set; }

        public uint Status { get; set; }
        public string Remark { get; set; }
    }

    public class ChargeBankResult : ChargeResult
    {

    }

    public class ChargeBankEasyResult : ChargeResult
    {

    }

    public class ChargeAlipayMicroResult : ChargeResult
    {
        public ChargeAlipayMicroResult()
        {
            Type = 3;
            Channel = 1;
        }
    }

    public class ChargeAlipayNativeResult : ChargeResult
    {
        public ChargeAlipayNativeResult()
        {
            Type = 3;
            Channel = 2;
        }
    }

    public class ChargeAlipayAppResult : ChargeResult
    {
        public ChargeAlipayAppResult()
        {
            Type = 3;
            Channel = 3;
        }
    }

    public class ChargeWepayMicroResult : ChargeResult
    {
        public ChargeWepayMicroResult()
        {
            Type = 4;
            Channel = 1;
        }
    }

    public class ChargeWepayNativeResult : ChargeResult
    {
        public ChargeWepayNativeResult()
        {
            Type = 4;
            Channel = 2;
        }
    }

    public class ChargeWepayAppResult : ChargeResult
    {
        public ChargeWepayAppResult()
        {
            Type = 4;
            Channel = 3;
        }
    }

    public class ChargeUnipayMicroResult : ChargeResult
    {
        public ChargeUnipayMicroResult()
        {
            Type = 5;
            Channel = 1;
        }
    }

    public class ChargeUnipayNativeResult : ChargeResult
    {
        public ChargeUnipayNativeResult()
        {
            Type = 5;
            Channel = 2;
        }
    }

    public class ChargeUnipayAppResult : ChargeResult
    {
        public ChargeUnipayAppResult()
        {
            Type = 5;
            Channel = 3;
        }
    }

    public class ChargeQQpayMicroResult : ChargeResult
    {
        public ChargeQQpayMicroResult()
        {
            Type = 6;
            Channel = 1;
        }
    }

    public class ChargeQQpayNativeResult : ChargeResult
    {
        public ChargeQQpayNativeResult()
        {
            Type = 6;
            Channel = 2;
        }
    }

    public class ChargeQQpayAppResult : ChargeResult
    {
        public ChargeQQpayAppResult()
        {
            Type = 6;
            Channel = 3;
        }
    }


}
