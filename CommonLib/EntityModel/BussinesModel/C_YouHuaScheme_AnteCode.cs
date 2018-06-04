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
    // 优化投注方案号码
    ////</summary>
    [ProtoContract]
    [Entity("C_YouHuaScheme_AnteCode",Type = EntityType.Table)]
    public class C_YouHuaScheme_AnteCode
    { 
        public C_YouHuaScheme_AnteCode()
        {
        
        }
            //// <summary>
            //  主键
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
            // 订单签名
            ////</summary>
            [ProtoMember(3)]
            [Field("OrderSign")]
            public string OrderSign{ get; set; }
            //// <summary>
            // 彩种代码
            ////</summary>
            [ProtoMember(4)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 串关方式
            ////</summary>
            [ProtoMember(6)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            //// <summary>
            // 倍数
            ////</summary>
            [ProtoMember(7)]
            [Field("Amount")]
            public int? Amount{ get; set; }
            //// <summary>
            // 比赛编号
            ////</summary>
            [ProtoMember(8)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            //// <summary>
            // 投注号码
            ////</summary>
            [ProtoMember(9)]
            [Field("AnteCode")]
            public string AnteCode{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(10)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}