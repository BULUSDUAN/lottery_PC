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
    // 代理返点明细
    ////</summary>
    [ProtoContract]
    [Entity("P_OCAgent_PayDetail",Type = EntityType.Table)]
    public class P_OCAgent_PayDetail
    { 
        public P_OCAgent_PayDetail()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 订单号
            ////</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 订单用户（投注订单的用户）
            ////</summary>
            [ProtoMember(3)]
            [Field("OrderUser")]
            public string OrderUser{ get; set; }
            //// <summary>
            // 收入用户编号
            ////</summary>
            [ProtoMember(4)]
            [Field("PayInUserId")]
            public string PayInUserId{ get; set; }
            //// <summary>
            // 方案类型
            ////</summary>
            [ProtoMember(5)]
            [Field("SchemeType")]
            public int? SchemeType{ get; set; }
            //// <summary>
            // 彩种
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
            // 返点
            ////</summary>
            [ProtoMember(8)]
            [Field("Rebate")]
            public decimal? Rebate{ get; set; }
            //// <summary>
            // 订单总金额  或 订单盈利金额
            ////</summary>
            [ProtoMember(9)]
            [Field("OrderTotalMoney")]
            public decimal? OrderTotalMoney{ get; set; }
            //// <summary>
            // 返利金额
            ////</summary>
            [ProtoMember(10)]
            [Field("PayMoney")]
            public decimal? PayMoney{ get; set; }
            //// <summary>
            // CPS模式
            ////</summary>
            [ProtoMember(11)]
            [Field("CPSMode")]
            public int? CPSMode{ get; set; }
            //// <summary>
            // 处理人（用于提现返点，结算分红）
            ////</summary>
            [ProtoMember(12)]
            [Field("HandlPeople")]
            public string HandlPeople{ get; set; }
            //// <summary>
            // 备注
            ////</summary>
            [ProtoMember(13)]
            [Field("Remark")]
            public string Remark{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(14)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}