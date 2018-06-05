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
    // 彩票最新中奖
    ///</summary>
    [ProtoContract]
    [Entity("E_LotteryNewBonus",Type = EntityType.Table)]
    public class E_LotteryNewBonus
    { 
        public E_LotteryNewBonus()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 方案号
            ///</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
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
            // 期号
            ///</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 用户名称
            ///</summary>
            [ProtoMember(7)]
            [Field("UserDisplayName")]
            public string UserDisplayName{ get; set; }
            /// <summary>
            // 隐藏用户个数
            ///</summary>
            [ProtoMember(8)]
            [Field("HideUserDisplayNameCount")]
            public int HideUserDisplayNameCount{ get; set; }
            /// <summary>
            // 倍数
            ///</summary>
            [ProtoMember(9)]
            [Field("Amount")]
            public int Amount{ get; set; }
            /// <summary>
            // 总金额
            ///</summary>
            [ProtoMember(10)]
            [Field("TotalMoney")]
            public decimal TotalMoney{ get; set; }
            /// <summary>
            // 税前奖金
            ///</summary>
            [ProtoMember(11)]
            [Field("PreTaxBonusMoney")]
            public decimal PreTaxBonusMoney{ get; set; }
            /// <summary>
            // 税后奖金
            ///</summary>
            [ProtoMember(12)]
            [Field("AfterTaxBonusMoney")]
            public decimal AfterTaxBonusMoney{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}