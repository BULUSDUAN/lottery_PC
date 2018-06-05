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
    // 用户获奖记录
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_ProfileBonusLevel",Type = EntityType.Table)]
    public class E_Blog_ProfileBonusLevel
    { 
        public E_Blog_ProfileBonusLevel()
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
            // 最高中奖等级
            ///</summary>
            [ProtoMember(3)]
            [Field("MaxLevelValue")]
            public int MaxLevelValue{ get; set; }
            /// <summary>
            // 最高中奖中文
            ///</summary>
            [ProtoMember(4)]
            [Field("MaxLevelName")]
            public string MaxLevelName{ get; set; }
            /// <summary>
            // 中奖一百元次数
            ///</summary>
            [ProtoMember(5)]
            [Field("WinOneHundredCount")]
            public int WinOneHundredCount{ get; set; }
            /// <summary>
            // 中一千元次数
            ///</summary>
            [ProtoMember(6)]
            [Field("WinOneThousandCount")]
            public int WinOneThousandCount{ get; set; }
            /// <summary>
            // 中一万元次数
            ///</summary>
            [ProtoMember(7)]
            [Field("WinTenThousandCount")]
            public int WinTenThousandCount{ get; set; }
            /// <summary>
            // 中十万元次数
            ///</summary>
            [ProtoMember(8)]
            [Field("WinOneHundredThousandCount")]
            public int WinOneHundredThousandCount{ get; set; }
            /// <summary>
            // 中百万元次数
            ///</summary>
            [ProtoMember(9)]
            [Field("WinOneMillionCount")]
            public int WinOneMillionCount{ get; set; }
            /// <summary>
            // 中一千万次数
            ///</summary>
            [ProtoMember(10)]
            [Field("WinTenMillionCount")]
            public int WinTenMillionCount{ get; set; }
            /// <summary>
            // 中一亿元次数
            ///</summary>
            [ProtoMember(11)]
            [Field("WinHundredMillionCount")]
            public int WinHundredMillionCount{ get; set; }
            /// <summary>
            // 总中奖金额
            ///</summary>
            [ProtoMember(12)]
            [Field("TotalBonusMoney")]
            public decimal TotalBonusMoney{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(13)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}