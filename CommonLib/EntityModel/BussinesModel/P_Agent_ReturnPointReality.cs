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
    [Entity("P_Agent_ReturnPointReality",Type = EntityType.Table)]
    public class P_Agent_ReturnPointReality
    { 
        public P_Agent_ReturnPointReality()
        {
        
        }
            //// <summary>
            // ID
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            //// <summary>
            // 用户ID
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(4)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 我的返点
            ////</summary>
            [ProtoMember(5)]
            [Field("MyPoint")]
            public decimal? MyPoint{ get; set; }
            //// <summary>
            // 下级的返点
            ////</summary>
            [ProtoMember(6)]
            [Field("LowerPoint")]
            public decimal? LowerPoint{ get; set; }
            //// <summary>
            // 下级的修改时间
            ////</summary>
            [ProtoMember(7)]
            [Field("LowerUpTime")]
            public DateTime? LowerUpTime{ get; set; }
            //// <summary>
            // 我的修改时间
            ////</summary>
            [ProtoMember(8)]
            [Field("MyUpTime")]
            public DateTime? MyUpTime{ get; set; }
    }
}