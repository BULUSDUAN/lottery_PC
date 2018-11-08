using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class ThirdPartyGameListParam
    {
        public int GameType { get; set; }
        public int TransferType { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
