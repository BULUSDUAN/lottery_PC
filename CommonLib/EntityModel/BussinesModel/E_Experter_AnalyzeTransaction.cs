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
    // 专家分析交易信息
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter_AnalyzeTransaction",Type = EntityType.Table)]
    public class E_Experter_AnalyzeTransaction
    { 
        public E_Experter_AnalyzeTransaction()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 分析Id
            ////</summary>
            [ProtoMember(2)]
            [Field("AnalyzeId")]
            public string AnalyzeId{ get; set; }
            //// <summary>
            // 购买用户Id
            ////</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 专家Id
            ////</summary>
            [ProtoMember(4)]
            [Field("ExperterId")]
            public string ExperterId{ get; set; }
            //// <summary>
            // 分析单价
            ////</summary>
            [ProtoMember(5)]
            [Field("Price")]
            public decimal? Price{ get; set; }
            //// <summary>
            // 当前发布时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}