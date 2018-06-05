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
    // 用户保存的订单
    ///</summary>
    [ProtoContract]
    [Entity("C_UserSaveOrder",Type = EntityType.Table)]
    public class C_UserSaveOrder
    { 
        public C_UserSaveOrder()
        {
        
        }
            /// <summary>
            // 方案编号
            ///</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
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
            // 停止时间
            ///</summary>
            [ProtoMember(5)]
            [Field("StrStopTime")]
            public string StrStopTime{ get; set; }
            /// <summary>
            // 串关方式
            ///</summary>
            [ProtoMember(6)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 投注方案类别
            ///</summary>
            [ProtoMember(7)]
            [Field("SchemeType")]
            public int SchemeType{ get; set; }
            /// <summary>
            // 方案来源
            ///</summary>
            [ProtoMember(8)]
            [Field("SchemeSource")]
            public int SchemeSource{ get; set; }
            /// <summary>
            // 方案投注类别
            ///</summary>
            [ProtoMember(9)]
            [Field("SchemeBettingCategory")]
            public int SchemeBettingCategory{ get; set; }
            /// <summary>
            // 进度状态
            ///</summary>
            [ProtoMember(10)]
            [Field("ProgressStatus")]
            public int ProgressStatus{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(11)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 倍数
            ///</summary>
            [ProtoMember(12)]
            [Field("Amount")]
            public int Amount{ get; set; }
            /// <summary>
            // 注数
            ///</summary>
            [ProtoMember(13)]
            [Field("BetCount")]
            public int BetCount{ get; set; }
            /// <summary>
            // 投注总金额
            ///</summary>
            [ProtoMember(14)]
            [Field("TotalMoney")]
            public decimal TotalMoney{ get; set; }
            /// <summary>
            // 停止时间
            ///</summary>
            [ProtoMember(15)]
            [Field("StopTime")]
            public DateTime StopTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(16)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 宝单宣言
            ///</summary>
            [ProtoMember(17)]
            [Field("SingleTreasureDeclaration")]
            public string SingleTreasureDeclaration{ get; set; }
            /// <summary>
            // 宝单分享提成
            ///</summary>
            [ProtoMember(18)]
            [Field("BDFXCommission")]
            public decimal BDFXCommission{ get; set; }
    }
}