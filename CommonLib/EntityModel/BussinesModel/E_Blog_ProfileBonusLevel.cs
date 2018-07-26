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
    [Entity("E_Blog_ProfileBonusLevel",Type = EntityType.Table)]
    public class E_Blog_ProfileBonusLevel
    { 
        public E_Blog_ProfileBonusLevel()
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
            [Field("MaxLevelValue")]
            public int? MaxLevelValue{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("MaxLevelName")]
            public string MaxLevelName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("WinOneHundredCount")]
            public int? WinOneHundredCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("WinOneThousandCount")]
            public int? WinOneThousandCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("WinTenThousandCount")]
            public int? WinTenThousandCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("WinOneHundredThousandCount")]
            public int? WinOneHundredThousandCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("WinOneMillionCount")]
            public int? WinOneMillionCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("WinTenMillionCount")]
            public int? WinTenMillionCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("WinHundredMillionCount")]
            public int? WinHundredMillionCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("TotalBonusMoney")]
            public decimal? TotalBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}