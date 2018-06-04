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
    // 分析推荐
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter_AnalyzeScheme",Type = EntityType.Table)]
    public class E_Experter_AnalyzeScheme
    { 
        public E_Experter_AnalyzeScheme()
        {
        
        }
            //// <summary>
            // 分析推荐编号
            ////</summary>
            [ProtoMember(1)]
            [Field("AnalyzeId", IsIdenty = false, IsPrimaryKey = true)]
            public string AnalyzeId{ get; set; }
            //// <summary>
            // 专家编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 文章标题
            ////</summary>
            [ProtoMember(3)]
            [Field("Title")]
            public string Title{ get; set; }
            //// <summary>
            // 来源
            ////</summary>
            [ProtoMember(4)]
            [Field("Source")]
            public string Source{ get; set; }
            //// <summary>
            // 内容
            ////</summary>
            [ProtoMember(5)]
            [Field("Content")]
            public string Content{ get; set; }
            //// <summary>
            // 分析价格
            ////</summary>
            [ProtoMember(6)]
            [Field("Price")]
            public decimal? Price{ get; set; }
            //// <summary>
            // 售出次数
            ////</summary>
            [ProtoMember(7)]
            [Field("SellCount")]
            public int? SellCount{ get; set; }
            //// <summary>
            // 处理类别
            ////</summary>
            [ProtoMember(8)]
            [Field("DealWithType")]
            public int? DealWithType{ get; set; }
            //// <summary>
            // 处理意见
            ////</summary>
            [ProtoMember(9)]
            [Field("DisposeOpinion")]
            public string DisposeOpinion{ get; set; }
            //// <summary>
            // 购买时间戳
            ////</summary>
            [ProtoMember(10)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}