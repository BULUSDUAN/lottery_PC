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
    [Entity("C_Sports_AnteCode",Type = EntityType.Table)]
    public class C_Sports_AnteCode
    { 
        public C_Sports_AnteCode()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
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
            // 串关方式
            ////</summary>
            [ProtoMember(5)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 比赛编号
            ////</summary>
            [ProtoMember(7)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            //// <summary>
            // 投注号
            ////</summary>
            [ProtoMember(8)]
            [Field("AnteCode")]
            public string AnteCode{ get; set; }
            //// <summary>
            // 是否包单
            ////</summary>
            [ProtoMember(9)]
            [Field("IsDan")]
            public bool? IsDan{ get; set; }
            //// <summary>
            // 赔率
            ////</summary>
            [ProtoMember(10)]
            [Field("Odds")]
            public string Odds{ get; set; }
            //// <summary>
            // 状态
            ////</summary>
            [ProtoMember(11)]
            [Field("BonusStatus")]
            public int? BonusStatus{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(12)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}