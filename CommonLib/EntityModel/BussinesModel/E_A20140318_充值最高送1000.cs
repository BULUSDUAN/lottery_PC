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
    [Entity("E_A20140318_充值最高送1000",Type = EntityType.Table)]
    public class E_A20140318_充值最高送1000
    { 
        public E_A20140318_充值最高送1000()
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
            // 订单编号
            ////</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            //// <summary>
            // 充值金额
            ////</summary>
            [ProtoMember(4)]
            [Field("FillMoney")]
            public decimal? FillMoney{ get; set; }
            //// <summary>
            // 当前赠送金额
            ////</summary>
            [ProtoMember(5)]
            [Field("CurrentGiveMoney")]
            public decimal? CurrentGiveMoney{ get; set; }
            //// <summary>
            // 下一月份
            ////</summary>
            [ProtoMember(6)]
            [Field("NextMonth")]
            public int? NextMonth{ get; set; }
            //// <summary>
            // 下月赠送金额
            ////</summary>
            [ProtoMember(7)]
            [Field("NextMonthGiveMoney")]
            public decimal? NextMonthGiveMoney{ get; set; }
            //// <summary>
            // 是否完成赠送
            ////</summary>
            [ProtoMember(8)]
            [Field("IsGiveComplete")]
            public bool? IsGiveComplete{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}