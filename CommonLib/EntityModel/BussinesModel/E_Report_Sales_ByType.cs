using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 
    ////</summary>
    [ProtoContract]
    [Entity("E_Report_Sales_ByType",Type = EntityType.Table)]
    public class E_Report_Sales_ByType
    { 
        public E_Report_Sales_ByType()
        {
        
        }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(2)]
            [Field("DateOrTime")]
            public string DateOrTime{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(3)]
            [Field("DateType")]
            public string DateType{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(4)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(5)]
            [Field("SchemeType")]
            public string SchemeType{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(6)]
            [Field("OrderCount")]
            public int? OrderCount{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(7)]
            [Field("OrderMoney")]
            public decimal? OrderMoney{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(8)]
            [Field("PayCount")]
            public int? PayCount{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(9)]
            [Field("PayMoney")]
            public decimal? PayMoney{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(10)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}