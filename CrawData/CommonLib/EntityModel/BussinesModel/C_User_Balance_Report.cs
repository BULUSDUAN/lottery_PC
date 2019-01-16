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
    // 用户余额报表
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Balance_Report",Type = EntityType.Table)]
    public class C_User_Balance_Report
    { 
        public C_User_Balance_Report()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 保存时间
            ///</summary>
            [ProtoMember(2)]
            [Field("SaveDateTime")]
            public string SaveDateTime{ get; set; }
            /// <summary>
            // 总充值余额
            ///</summary>
            [ProtoMember(3)]
            [Field("TotalFillMoneyBalance")]
            public decimal TotalFillMoneyBalance{ get; set; }
            /// <summary>
            // 总奖金余额
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalBonusBalance")]
            public decimal TotalBonusBalance{ get; set; }
            /// <summary>
            // 总佣金余额
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalCommissionBalance")]
            public decimal TotalCommissionBalance{ get; set; }
            /// <summary>
            // 总名家余额
            ///</summary>
            [ProtoMember(6)]
            [Field("TotalExpertsBalance")]
            public decimal TotalExpertsBalance{ get; set; }
            /// <summary>
            // 总冻结余额
            ///</summary>
            [ProtoMember(7)]
            [Field("TotalFreezeBalance")]
            public decimal TotalFreezeBalance{ get; set; }
            /// <summary>
            // 总红包金额
            ///</summary>
            [ProtoMember(8)]
            [Field("TotalRedBagBalance")]
            public decimal TotalRedBagBalance{ get; set; }
            /// <summary>
            // 总成长值
            ///</summary>
            [ProtoMember(9)]
            [Field("TotalUserGrowth")]
            public int TotalUserGrowth{ get; set; }
            /// <summary>
            // 总豆豆数量
            ///</summary>
            [ProtoMember(10)]
            [Field("TotalDouDou")]
            public int TotalDouDou{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}