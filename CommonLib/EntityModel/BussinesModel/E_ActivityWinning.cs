using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("E_ActivityWinning",Type = EntityType.Table)]
    public class E_ActivityWinning
    { 
        public E_ActivityWinning()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("IssueNumber")]
            public string IssueNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("Game")]
            public string Game{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("BetMoney")]
            public decimal BetMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("WinningMoney")]
            public decimal WinningMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("Prize")]
            public decimal Prize{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("ActivityId")]
            public string ActivityId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("Remark")]
            public string Remark{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}