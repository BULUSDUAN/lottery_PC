using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public enum AccountType : byte
    {
        DebitCard = 1,
        CreditCard
    }

    public enum CertType : byte
    {
        IdentityCard = 1
    }

    public class BankCardAdd
    {
        public AccountType AccountType { get; set; }
        public ulong Bank { get; set; }

        public string No { get; set; }
        public string Name { get; set; }

        public CertType CertType { get; set; }
        public string CertNo { get; set; }

        public string ExpireDate { get; set; }
        public string Cvn { get; set; }

        public string Mobile { get; set; }
    }
}
