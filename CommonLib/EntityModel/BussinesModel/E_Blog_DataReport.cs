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
    // 用户数据统计
    ////</summary>
    [ProtoContract]
    [Entity("E_Blog_DataReport",Type = EntityType.Table)]
    public class E_Blog_DataReport
    { 
        public E_Blog_DataReport()
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
            // 发起合买次数
            ////</summary>
            [ProtoMember(3)]
            [Field("CreateSchemeCount")]
            public int? CreateSchemeCount{ get; set; }
            //// <summary>
            // 参与合买次数
            ////</summary>
            [ProtoMember(4)]
            [Field("JoinSchemeCount")]
            public int? JoinSchemeCount{ get; set; }
            //// <summary>
            // 总中奖次数
            ////</summary>
            [ProtoMember(5)]
            [Field("TotalBonusCount")]
            public int? TotalBonusCount{ get; set; }
            //// <summary>
            // 总中奖金额
            ////</summary>
            [ProtoMember(6)]
            [Field("TotalBonusMoney")]
            public decimal? TotalBonusMoney{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}