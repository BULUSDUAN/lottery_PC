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
    [Entity("E_A20140318_现场送彩票",Type = EntityType.Table)]
    public class E_A20140318_现场送彩票
    { 
        public E_A20140318_现场送彩票()
        {
        
        }
            /// <summary>
            // 主键
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
            // 手机号码
            ///</summary>
            [ProtoMember(3)]
            [Field("MobileNumber")]
            public string MobileNumber{ get; set; }
            /// <summary>
            // 方案编号
            ///</summary>
            [ProtoMember(4)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            /// <summary>
            // 注数
            ///</summary>
            [ProtoMember(5)]
            [Field("BetCount")]
            public int BetCount{ get; set; }
            /// <summary>
            // 投注金额
            ///</summary>
            [ProtoMember(6)]
            [Field("BetMoney")]
            public decimal BetMoney{ get; set; }
            /// <summary>
            // 中奖金额
            ///</summary>
            [ProtoMember(7)]
            [Field("BonusMoney")]
            public decimal BonusMoney{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}