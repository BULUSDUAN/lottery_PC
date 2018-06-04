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
    [Entity("E_A20131105_优惠券",Type = EntityType.Table)]
    public class E_A20131105_优惠券
    { 
        public E_A20131105_优惠券()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 金额
            ////</summary>
            [ProtoMember(2)]
            [Field("Money")]
            public decimal? Money{ get; set; }
            //// <summary>
            // 备注
            ////</summary>
            [ProtoMember(3)]
            [Field("Summary")]
            public string Summary{ get; set; }
            //// <summary>
            // 号码
            ////</summary>
            [ProtoMember(4)]
            [Field("Number")]
            public string Number{ get; set; }
            //// <summary>
            // 是否能使用
            ////</summary>
            [ProtoMember(5)]
            [Field("CanUsable")]
            public bool? CanUsable{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(6)]
            [Field("BelongUserId")]
            public string BelongUserId{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}