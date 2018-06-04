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
    // 中奖规则
    ////</summary>
    [ProtoContract]
    [Entity("C_Bonus_Rule",Type = EntityType.Table)]
    public class C_Bonus_Rule
    { 
        public C_Bonus_Rule()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 游戏代码
            ////</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 游戏类型
            ////</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 奖金等级
            ////</summary>
            [ProtoMember(4)]
            [Field("BonusGrade")]
            public int BonusGrade{ get; set; }
            //// <summary>
            // 奖金等级名称
            ////</summary>
            [ProtoMember(5)]
            [Field("BonusGradeDisplayName")]
            public string BonusGradeDisplayName{ get; set; }
            //// <summary>
            // 奖金
            ////</summary>
            [ProtoMember(6)]
            [Field("BonusMoney")]
            public decimal? BonusMoney{ get; set; }
    }
}