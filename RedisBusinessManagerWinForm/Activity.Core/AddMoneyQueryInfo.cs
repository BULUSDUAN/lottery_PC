using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class AddMoneyQueryInfo
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string GameCode { get; set; }
        public decimal BonusMoney { get; set; }
        public decimal AddMoney { get; set; }
        public decimal GainMoney { get; set; }
    }

    [CommunicationObject]
    public class AddMoneyQueryInfoCollection : List<AddMoneyQueryInfo>
    {
    }
}
