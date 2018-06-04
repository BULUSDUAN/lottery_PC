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
    [Entity("E_A20121009_传统足球_14场_任九_加奖",Type = EntityType.Table)]
    public class E_A20121009_传统足球_14场_任九_加奖
    { 
        public E_A20121009_传统足球_14场_任九_加奖()
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
            // 方案类型
            ////</summary>
            [ProtoMember(3)]
            [Field("SchemeType")]
            public int? SchemeType{ get; set; }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(4)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 彩种类型
            ////</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 命中数
            ////</summary>
            [ProtoMember(7)]
            [Field("HitMatchCount")]
            public int? HitMatchCount{ get; set; }
            //// <summary>
            // 加奖金额
            ////</summary>
            [ProtoMember(8)]
            [Field("AddMoney")]
            public decimal? AddMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}