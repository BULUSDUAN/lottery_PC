using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class NearTimeProfitRate_Collection:Page
    {
        public NearTimeProfitRate_Collection()
        {
            NearTimeProfitRateList = new List<NearTimeProfitRateInfo>();
        }
        [ProtoMember(1)]
        public List<NearTimeProfitRateInfo> NearTimeProfitRateList { get; set; }
    }
    [ProtoContract]
    public class NearTimeProfitRateInfo
    {
        public NearTimeProfitRateInfo() { }
        [ProtoMember(1)]
        public int RowNumber { get; set; }
        [ProtoMember(2)]
        public string CurrDate { get; set; }
        [ProtoMember(3)]
        public decimal CurrProfitRate { get; set; }
    }
}
