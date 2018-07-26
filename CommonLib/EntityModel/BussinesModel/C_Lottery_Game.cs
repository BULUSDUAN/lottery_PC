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
    [Entity("C_Lottery_Game",Type = EntityType.Table)]
    public class C_Lottery_Game
    { 
        public C_Lottery_Game()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("GameCode", IsIdenty = false, IsPrimaryKey = true)]
            public string GameCode{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("EnableStatus")]
            public int? EnableStatus{ get; set; }
    }
}