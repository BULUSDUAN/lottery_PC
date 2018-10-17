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
    // 用户余额历史
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Balance_History",Type = EntityType.Table)]
    public class C_User_Balance_History
    { 
        public C_User_Balance_History()
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
            // 用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 充值账户余额
            ///</summary>
            [ProtoMember(4)]
            [Field("FillMoneyBalance")]
            public decimal FillMoneyBalance{ get; set; }
            /// <summary>
            // 奖金账户，中奖后返到此账户，可提现
            ///</summary>
            [ProtoMember(5)]
            [Field("BonusBalance")]
            public decimal BonusBalance{ get; set; }
            /// <summary>
            // 佣金账户，为代理商计算佣金时，转到此账户
            ///</summary>
            [ProtoMember(6)]
            [Field("CommissionBalance")]
            public decimal CommissionBalance{ get; set; }
            /// <summary>
            // 名家余额
            ///</summary>
            [ProtoMember(7)]
            [Field("ExpertsBalance")]
            public decimal ExpertsBalance{ get; set; }
            /// <summary>
            // 冻结账户，提现、追号、异常手工冻结
            ///</summary>
            [ProtoMember(8)]
            [Field("FreezeBalance")]
            public decimal FreezeBalance{ get; set; }
            /// <summary>
            // 红包余额
            ///</summary>
            [ProtoMember(9)]
            [Field("RedBagBalance")]
            public decimal RedBagBalance{ get; set; }
            /// <summary>
            // 成长值
            ///</summary>
            [ProtoMember(10)]
            [Field("UserGrowth")]
            public int UserGrowth{ get; set; }
            /// <summary>
            // 当前豆豆值
            ///</summary>
            [ProtoMember(11)]
            [Field("CurrentDouDou")]
            public int CurrentDouDou{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(12)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}