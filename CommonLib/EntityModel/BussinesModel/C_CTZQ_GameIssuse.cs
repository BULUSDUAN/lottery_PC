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
    [Entity("C_CTZQ_GameIssuse",Type = EntityType.Table)]
    public class C_CTZQ_GameIssuse
    { 
        public C_CTZQ_GameIssuse()
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
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(4)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(5)]
            [Field("StopBettingTime")]
            public DateTime? StopBettingTime{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(6)]
            [Field("WinNumber")]
            public string WinNumber{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}