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
    // 澳彩代理返点
    ///</summary>
    [ProtoContract]
    [Entity("P_OCAgent_Rebate",Type = EntityType.Table)]
    public class P_OCAgent_Rebate
    { 
        public P_OCAgent_Rebate()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 代理编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 彩种
            ///</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(4)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 自身返点
            ///</summary>
            [ProtoMember(5)]
            [Field("Rebate")]
            public decimal Rebate{ get; set; }
            /// <summary>
            // 返点类型:0:串关返点；1:单关返点；
            ///</summary>
            [ProtoMember(6)]
            [Field("SubUserRebate")]
            public decimal SubUserRebate{ get; set; }
            /// <summary>
            // 下级用户默认返点
            ///</summary>
            [ProtoMember(7)]
            [Field("RebateType")]
            public int RebateType{ get; set; }
            /// <summary>
            // CPS模式
            ///</summary>
            [ProtoMember(8)]
            [Field("CPSMode")]
            public int CPSMode{ get; set; }
            /// <summary>
            // 生成时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}