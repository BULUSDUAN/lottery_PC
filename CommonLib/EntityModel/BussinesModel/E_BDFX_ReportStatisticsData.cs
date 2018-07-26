using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 宝单分享数据统计
    ///</summary>
    [ProtoContract]
    [Entity("E_BDFX_ReportStatisticsData",Type = EntityType.Table)]
    public class E_BDFX_ReportStatisticsData
    { 
        public E_BDFX_ReportStatisticsData()
        {
        
        }
            /// <summary>
            // 主键编号
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 总盈利率(注：目前暂定最近三个月统计数据)
            ///</summary>
            [ProtoMember(3)]
            [Field("TotalProfit")]
            public decimal TotalProfit{ get; set; }
            /// <summary>
            // 月盈利率(注：目前暂定最近一个月统计数据)
            ///</summary>
            [ProtoMember(4)]
            [Field("MonthProfit")]
            public decimal MonthProfit{ get; set; }
            /// <summary>
            // 周盈利率(注：指最近七天统计数据)
            ///</summary>
            [ProtoMember(5)]
            [Field("WeekProfit")]
            public decimal WeekProfit{ get; set; }
            /// <summary>
            // 总粉丝数(注：目前暂定最近三个月统计数据)
            ///</summary>
            [ProtoMember(6)]
            [Field("TotalFansCount")]
            public int TotalFansCount{ get; set; }
            /// <summary>
            // 月粉丝数(注：目前暂定最近一个月统计数据)
            ///</summary>
            [ProtoMember(7)]
            [Field("MonthFansCount")]
            public int MonthFansCount{ get; set; }
            /// <summary>
            // 周粉丝数(注：指最近七天统计数据)
            ///</summary>
            [ProtoMember(8)]
            [Field("WeekFansCount")]
            public int WeekFansCount{ get; set; }
            /// <summary>
            // 被抄单总数
            ///</summary>
            [ProtoMember(9)]
            [Field("TotalSingleCopyCount")]
            public int TotalSingleCopyCount{ get; set; }
            /// <summary>
            // 月-被抄单数
            ///</summary>
            [ProtoMember(10)]
            [Field("MonthSingleCopyCount")]
            public int MonthSingleCopyCount{ get; set; }
            /// <summary>
            // 周-被抄单数
            ///</summary>
            [ProtoMember(11)]
            [Field("WeekSingleCopyCount")]
            public int WeekSingleCopyCount{ get; set; }
            /// <summary>
            // 总战绩(注：目前暂定最近三个月统计数据)
            ///</summary>
            [ProtoMember(12)]
            [Field("TotalRecord")]
            public string TotalRecord{ get; set; }
            /// <summary>
            // 月战绩(注：目前暂定最近一个月统计数据)
            ///</summary>
            [ProtoMember(13)]
            [Field("MonthRecord")]
            public string MonthRecord{ get; set; }
            /// <summary>
            // 周战绩(注：指最近七天统计数据)
            ///</summary>
            [ProtoMember(14)]
            [Field("WeekRecord")]
            public string WeekRecord{ get; set; }
            /// <summary>
            // 总战绩比例(注：目前暂定最近三个月统计数据)
            ///</summary>
            [ProtoMember(15)]
            [Field("TotalRecordRatio")]
            public decimal TotalRecordRatio{ get; set; }
            /// <summary>
            // 月战绩比例(注：目前暂定最近一个月统计数据)
            ///</summary>
            [ProtoMember(16)]
            [Field("MonthRecordRatio")]
            public decimal MonthRecordRatio{ get; set; }
            /// <summary>
            // 周战绩比例(注：指最近七天统计数据)
            ///</summary>
            [ProtoMember(17)]
            [Field("WeekRecordRatio")]
            public decimal WeekRecordRatio{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(18)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(19)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("ContinuityRed")]
            public int ContinuityRed{ get; set; }
    }
}