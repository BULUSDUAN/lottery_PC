using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models.Charge.BankEasy
{
    public class BankCardQueryResult
    {
        public AccountType AccountType { get; set; }
        public ulong Bank { get; set; }

        public string No { get; set; }
        public string Name { get; set; }

        public CertType CertType { get; set; }
        public string CertNo { get; set; }

        public string Mobile { get; set; }

        public uint Status { get; set; }
    }
}
