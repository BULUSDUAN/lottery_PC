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
    // 
    ///</summary>
    [ProtoContract]
    [Entity("E_A20150919_加奖配置",Type = EntityType.Table)]
    public class E_A20150919_加奖配置
    { 
        public E_A20150919_加奖配置()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 订单标识
            ///</summary>
            [ProtoMember(2)]
            [Field("OrderIndex")]
            public int OrderIndex{ get; set; }
            /// <summary>
            // 彩种代码
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
            // 串关方式
            ///</summary>
            [ProtoMember(5)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 加奖金额百分比
            ///</summary>
            [ProtoMember(6)]
            [Field("AddBonusMoneyPercent")]
            public decimal AddBonusMoneyPercent{ get; set; }
            /// <summary>
            // 最大加奖金额
            ///</summary>
            [ProtoMember(7)]
            [Field("MaxAddBonusMoney")]
            public decimal MaxAddBonusMoney{ get; set; }
            /// <summary>
            // 加奖方式
            ///</summary>
            [ProtoMember(8)]
            [Field("AddMoneyWay")]
            public string AddMoneyWay{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}