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
    // 用户中奖概率
    ////</summary>
    [ProtoContract]
    [Entity("C_User_BonusPercent",Type = EntityType.Table)]
    public class C_User_BonusPercent
    { 
        public C_User_BonusPercent()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种代码
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
            // 总订单数
            ////</summary>
            [ProtoMember(5)]
            [Field("TotalOrderCount")]
            public int? TotalOrderCount{ get; set; }
            //// <summary>
            // 中奖订单数
            ////</summary>
            [ProtoMember(6)]
            [Field("BonusOrderCount")]
            public int? BonusOrderCount{ get; set; }
            //// <summary>
            // 中奖率
            ////</summary>
            [ProtoMember(7)]
            [Field("BonusPercent")]
            public decimal? BonusPercent{ get; set; }
            //// <summary>
            // 当前日期
            ////</summary>
            [ProtoMember(8)]
            [Field("CurrentDate")]
            public string CurrentDate{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}