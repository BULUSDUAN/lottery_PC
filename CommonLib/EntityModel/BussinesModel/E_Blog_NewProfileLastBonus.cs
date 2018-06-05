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
    // 用户最新中奖
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_NewProfileLastBonus",Type = EntityType.Table)]
    public class E_Blog_NewProfileLastBonus
    { 
        public E_Blog_NewProfileLastBonus()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户Id
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 方案号
            ///</summary>
            [ProtoMember(3)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            /// <summary>
            // 彩种
            ///</summary>
            [ProtoMember(4)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 中奖金额
            ///</summary>
            [ProtoMember(7)]
            [Field("BonusMoney")]
            public decimal BonusMoney{ get; set; }
            /// <summary>
            // 中奖时间
            ///</summary>
            [ProtoMember(8)]
            [Field("BonusTime")]
            public DateTime BonusTime{ get; set; }
    }
}