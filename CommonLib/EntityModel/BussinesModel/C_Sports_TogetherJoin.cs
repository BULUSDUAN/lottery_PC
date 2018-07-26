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
    [Entity("C_Sports_TogetherJoin",Type = EntityType.Table)]
    public class C_Sports_TogetherJoin
    { 
        public C_Sports_TogetherJoin()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 方案编号
            ///</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            /// <summary>
            // 参加用户
            ///</summary>
            [ProtoMember(3)]
            [Field("JoinUserId")]
            public string JoinUserId{ get; set; }
            /// <summary>
            // 购买份数
            ///</summary>
            [ProtoMember(4)]
            [Field("BuyCount")]
            public int BuyCount{ get; set; }
            /// <summary>
            // 实际购买份数
            ///</summary>
            [ProtoMember(5)]
            [Field("RealBuyCount")]
            public int RealBuyCount{ get; set; }
            /// <summary>
            // 价格
            ///</summary>
            [ProtoMember(6)]
            [Field("Price")]
            public decimal Price{ get; set; }
            /// <summary>
            // 总价格
            ///</summary>
            [ProtoMember(7)]
            [Field("TotalMoney")]
            public decimal TotalMoney{ get; set; }
            /// <summary>
            // 参与类型
            ///</summary>
            [ProtoMember(8)]
            [Field("JoinType")]
            public int JoinType{ get; set; }
            /// <summary>
            // 是否成功参与
            ///</summary>
            [ProtoMember(9)]
            [Field("JoinSucess")]
            public bool JoinSucess{ get; set; }
            /// <summary>
            // 参与日志
            ///</summary>
            [ProtoMember(10)]
            [Field("JoinLog")]
            public string JoinLog{ get; set; }
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