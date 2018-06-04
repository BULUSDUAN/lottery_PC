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
    // 订制跟单记录
    ////</summary>
    [ProtoContract]
    [Entity("C_Together_FollowerRecord",Type = EntityType.Table)]
    public class C_Together_FollowerRecord
    { 
        public C_Together_FollowerRecord()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 跟单规则编号
            ////</summary>
            [ProtoMember(2)]
            [Field("RuleId")]
            public int? RuleId{ get; set; }
            //// <summary>
            // 查询关键字
            ////</summary>
            [ProtoMember(3)]
            [Field("RecordKey")]
            public string RecordKey{ get; set; }
            //// <summary>
            // 合买发起人用户编号
            ////</summary>
            [ProtoMember(4)]
            [Field("CreaterUserId")]
            public string CreaterUserId{ get; set; }
            //// <summary>
            // 跟单人用户编号
            ////</summary>
            [ProtoMember(5)]
            [Field("FollowerUserId")]
            public string FollowerUserId{ get; set; }
            //// <summary>
            // 彩种编码
            ////</summary>
            [ProtoMember(6)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(7)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 方案号
            ////</summary>
            [ProtoMember(8)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 每份单价
            ////</summary>
            [ProtoMember(9)]
            [Field("Price")]
            public decimal? Price{ get; set; }
            //// <summary>
            // 购买份数
            ////</summary>
            [ProtoMember(10)]
            [Field("BuyCount")]
            public int? BuyCount{ get; set; }
            //// <summary>
            // 购买金额
            ////</summary>
            [ProtoMember(11)]
            [Field("BuyMoney")]
            public decimal? BuyMoney{ get; set; }
            //// <summary>
            // 中奖金额
            ////</summary>
            [ProtoMember(12)]
            [Field("BonusMoney")]
            public decimal? BonusMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}