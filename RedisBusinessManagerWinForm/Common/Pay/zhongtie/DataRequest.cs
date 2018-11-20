using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FancyOne.Payment
{
    public class DataRequest
    {
        public string Order { get; set; }
        public DateTime? Time { get; set; }
        public ulong? Bank { get; set; }
        public ushort? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string Product { get; set; }
        public string CallbackFront { get; set; }
        public string CallbackPush { get; set; }
        public string Identity { get; set; }
        public string Remark { get; set; }
    }
}
