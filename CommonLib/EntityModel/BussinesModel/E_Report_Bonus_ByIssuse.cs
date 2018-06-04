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
    [Entity("E_Report_Bonus_ByIssuse",Type = EntityType.Table)]
    public class E_Report_Bonus_ByIssuse
    { 
        public E_Report_Bonus_ByIssuse()
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
            [Field("BonusCount")]
            public int? BonusCount{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(6)]
            [Field("BonusMoney")]
            public decimal? BonusMoney{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}