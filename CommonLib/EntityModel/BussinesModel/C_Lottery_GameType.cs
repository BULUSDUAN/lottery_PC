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
    [Entity("C_Lottery_GameType",Type = EntityType.Table)]
    public class C_Lottery_GameType
    { 
        public C_Lottery_GameType()
        {
        
        }
            //// <summary>
            // 玩法类型编号
            ////</summary>
            [ProtoMember(1)]
            [Field("GameTypeId", IsIdenty = false, IsPrimaryKey = true)]
            public string GameTypeId{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法类型
            ////</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 名称
            ////</summary>
            [ProtoMember(4)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            //// <summary>
            // 启用状态
            ////</summary>
            [ProtoMember(5)]
            [Field("EnableStatus")]
            public int? EnableStatus{ get; set; }
            //// <summary>
            // 票数
            ////</summary>
            [ProtoMember(6)]
            [Field("TicketCount")]
            public int? TicketCount{ get; set; }
    }
}