using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FancyOne.Payment
{
    public class DataResponse
    {
        public string Order { get; set; }
        public ulong? Bank { get; set; }
        public ushort? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string Identity { get; set; }
        public string Remark { get; set; }
        public ushort? Status { get; set; }
    }

    public class DataResponsePush
    {
        public string PushId { get; set; }
        public string Order { get; set; }
        public ulong? Bank { get; set; }
        public ushort? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string Identity { get; set; }
        public string Remark { get; set; }
        public ushort? Status { get; set; }
    }
}
