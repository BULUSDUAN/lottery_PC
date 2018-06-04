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
    [Entity("E_A20131101_用户返点",Type = EntityType.Table)]
    public class E_A20131101_用户返点
    { 
        public E_A20131101_用户返点()
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
            // 月份
            ////</summary>
            [ProtoMember(3)]
            [Field("Month")]
            public int? Month{ get; set; }
            //// <summary>
            // 总金额
            ////</summary>
            [ProtoMember(4)]
            [Field("TotalMoney")]
            public decimal? TotalMoney{ get; set; }
            //// <summary>
            // 赠送金额
            ////</summary>
            [ProtoMember(5)]
            [Field("GiveMoney")]
            public decimal? GiveMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}