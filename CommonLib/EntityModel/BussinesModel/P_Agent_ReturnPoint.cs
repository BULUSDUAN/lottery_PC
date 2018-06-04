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
    [Entity("P_Agent_ReturnPoint",Type = EntityType.Table)]
    public class P_Agent_ReturnPoint
    { 
        public P_Agent_ReturnPoint()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            //// <summary>
            // 设置的人
            ////</summary>
            [ProtoMember(2)]
            [Field("AgentIdFrom")]
            public string AgentIdFrom{ get; set; }
            //// <summary>
            // 被设置的人
            ////</summary>
            [ProtoMember(3)]
            [Field("AgentIdTo")]
            public string AgentIdTo{ get; set; }
            //// <summary>
            // 设置等级
            ////</summary>
            [ProtoMember(4)]
            [Field("SetLevel")]
            public int? SetLevel{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(5)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(6)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 返点
            ////</summary>
            [ProtoMember(7)]
            [Field("ReturnPoint")]
            public decimal? ReturnPoint{ get; set; }
    }
}