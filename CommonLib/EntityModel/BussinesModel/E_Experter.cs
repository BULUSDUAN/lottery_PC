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
    // 专家
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter",Type = EntityType.Table)]
    public class E_Experter
    { 
        public E_Experter()
        {
        
        }
            //// <summary>
            // 专家编号
            ////</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            //// <summary>
            // 专家描述
            ////</summary>
            [ProtoMember(2)]
            [Field("ExperterSummary")]
            public string ExperterSummary{ get; set; }
            //// <summary>
            // 专家头像
            ////</summary>
            [ProtoMember(3)]
            [Field("ExperterHeadImage")]
            public string ExperterHeadImage{ get; set; }
            //// <summary>
            // 擅长彩种
            ////</summary>
            [ProtoMember(4)]
            [Field("AdeptGameCode")]
            public string AdeptGameCode{ get; set; }
            //// <summary>
            // 最近发单数
            ////</summary>
            [ProtoMember(5)]
            [Field("RecentlyOrderCount")]
            public int? RecentlyOrderCount{ get; set; }
            //// <summary>
            // 专家类别
            ////</summary>
            [ProtoMember(6)]
            [Field("ExperterType")]
            public int? ExperterType{ get; set; }
            //// <summary>
            // 周命中率
            ////</summary>
            [ProtoMember(7)]
            [Field("WeekShooting")]
            public decimal? WeekShooting{ get; set; }
            //// <summary>
            // 月命中率
            ////</summary>
            [ProtoMember(8)]
            [Field("MonthShooting")]
            public decimal? MonthShooting{ get; set; }
            //// <summary>
            // 总和命中率
            ////</summary>
            [ProtoMember(9)]
            [Field("TotalShooting")]
            public decimal? TotalShooting{ get; set; }
            //// <summary>
            // 周回报率
            ////</summary>
            [ProtoMember(10)]
            [Field("WeekRate")]
            public decimal? WeekRate{ get; set; }
            //// <summary>
            // 月回报率
            ////</summary>
            [ProtoMember(11)]
            [Field("MonthRate")]
            public decimal? MonthRate{ get; set; }
            //// <summary>
            // 总回报率
            ////</summary>
            [ProtoMember(12)]
            [Field("TotalRate")]
            public decimal? TotalRate{ get; set; }
            //// <summary>
            // 处理类别
            ////</summary>
            [ProtoMember(13)]
            [Field("IsEnable")]
            public bool? IsEnable{ get; set; }
            //// <summary>
            // 名家启用，禁用
            ////</summary>
            [ProtoMember(14)]
            [Field("DealWithType")]
            public int? DealWithType{ get; set; }
            //// <summary>
            // 处理意见
            ////</summary>
            [ProtoMember(15)]
            [Field("DisposeOpinion")]
            public string DisposeOpinion{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(16)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}