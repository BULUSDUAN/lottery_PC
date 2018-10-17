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
    // 用户余额
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Balance",Type = EntityType.Table)]
    public class C_User_Balance
    { 
        public C_User_Balance()
        {
        
        }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 版本
            ///</summary>
            [ProtoMember(2)]
            [Field("Version")]
            public int Version{ get; set; }
            /// <summary>
            // 充值账户余额
            ///</summary>
            [ProtoMember(3)]
            [Field("FillMoneyBalance")]
            public decimal FillMoneyBalance{ get; set; }
            /// <summary>
            // 奖金账户，中奖后返到此账户，可提现
            ///</summary>
            [ProtoMember(4)]
            [Field("BonusBalance")]
            public decimal BonusBalance{ get; set; }
            /// <summary>
            // 佣金账户，为代理商计算佣金时，转到此账户
            ///</summary>
            [ProtoMember(5)]
            [Field("CommissionBalance")]
            public decimal CommissionBalance{ get; set; }
            /// <summary>
            // 名家余额
            ///</summary>
            [ProtoMember(6)]
            [Field("ExpertsBalance")]
            public decimal ExpertsBalance{ get; set; }
            /// <summary>
            // 冻结账户，提现、追号、异常手工冻结
            ///</summary>
            [ProtoMember(7)]
            [Field("FreezeBalance")]
            public decimal FreezeBalance{ get; set; }
            /// <summary>
            // 红包余额
            ///</summary>
            [ProtoMember(8)]
            [Field("RedBagBalance")]
            public decimal RedBagBalance{ get; set; }
            /// <summary>
            // CPS余额
            ///</summary>
            [ProtoMember(9)]
            [Field("CPSBalance")]
            public decimal CPSBalance{ get; set; }
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
            // 是否设置密码
            ///</summary>
            [ProtoMember(12)]
            [Field("IsSetPwd")]
            public bool IsSetPwd{ get; set; }
            /// <summary>
            // 需要输入资金密码的地方
            ///</summary>
            [ProtoMember(13)]
            [Field("NeedPwdPlace")]
            public string NeedPwdPlace{ get; set; }
            /// <summary>
            // 资金密码
            ///</summary>
            [ProtoMember(14)]
            [Field("Password")]
            public string Password{ get; set; }
            /// <summary>
            // 代理商
            ///</summary>
            [ProtoMember(15)]
            [Field("AgentId")]
            public string AgentId{ get; set; }

        public virtual decimal GetTotalEnableMoney()
        {
            //return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance + this.ExpertsBalance + this.RedBagBalance;
            //return this.FillMoneyBalance + this.BonusBalance + this.ExpertsBalance + this.RedBagBalance;
            return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance;
        }
    }
}