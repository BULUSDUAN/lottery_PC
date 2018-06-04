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
    // 单场方案投注号码
    ////</summary>
    [ProtoContract]
    [Entity("C_SingleScheme_AnteCode",Type = EntityType.Table)]
    public class C_SingleScheme_AnteCode
    { 
        public C_SingleScheme_AnteCode()
        {
        
        }
            //// <summary>
            // 编号
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
            // 号码完整文件路径
            ////</summary>
            [ProtoMember(7)]
            [Field("AnteCodeFullFileName")]
            public string AnteCodeFullFileName{ get; set; }
            //// <summary>
            // 允许投注的号，如胜平负 只能投 3 1 0
            ////</summary>
            [ProtoMember(8)]
            [Field("AllowCodes")]
            public string AllowCodes{ get; set; }
            //// <summary>
            // 选择的场次编号
            ////</summary>
            [ProtoMember(9)]
            [Field("SelectMatchId")]
            public string SelectMatchId{ get; set; }
            //// <summary>
            // 是否包括场次编号
            ////</summary>
            [ProtoMember(10)]
            [Field("ContainsMatchId")]
            public bool? ContainsMatchId{ get; set; }
            //// <summary>
            // 是否文件流
            ////</summary>
            [ProtoMember(11)]
            [Field("FileBuffer")]
            public byte[] FileBuffer{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(12)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}