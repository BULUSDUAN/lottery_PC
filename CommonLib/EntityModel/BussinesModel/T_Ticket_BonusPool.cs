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
    [Entity("T_Ticket_BonusPool",Type = EntityType.Table)]
    public class T_Ticket_BonusPool
    { 
        public T_Ticket_BonusPool()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            /// <summary>
            // 彩种编码
            ///</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(4)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 奖金等级
            ///</summary>
            [ProtoMember(5)]
            [Field("BonusLevel")]
            public string BonusLevel{ get; set; }
            /// <summary>
            // 奖金等级名称
            ///</summary>
            [ProtoMember(6)]
            [Field("BonusLevelDisplayName")]
            public string BonusLevelDisplayName{ get; set; }
            /// <summary>
            // 奖池金额
            ///</summary>
            [ProtoMember(7)]
            [Field("BonusMoney")]
            public decimal BonusMoney{ get; set; }
            /// <summary>
            // 中奖数
            ///</summary>
            [ProtoMember(8)]
            [Field("BonusCount")]
            public int BonusCount{ get; set; }
            /// <summary>
            // 比赛结果
            ///</summary>
            [ProtoMember(9)]
            [Field("WinNumber")]
            public string WinNumber{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(10)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}