using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 
    ////</summary>
    [ProtoContract]
    [Entity("E_A20150919_注册绑定送红包",Type = EntityType.Table)]
    public class E_A20150919_注册绑定送红包
    { 
        public E_A20150919_注册绑定送红包()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 是否绑定银行卡
            ////</summary>
            [ProtoMember(3)]
            [Field("IsBindBankCard")]
            public bool? IsBindBankCard{ get; set; }
            //// <summary>
            // 是否认证身份证
            ////</summary>
            [ProtoMember(4)]
            [Field("IsBindRealName")]
            public bool? IsBindRealName{ get; set; }
            //// <summary>
            // 是否绑定手机
            ////</summary>
            [ProtoMember(5)]
            [Field("IsBindMobile")]
            public bool? IsBindMobile{ get; set; }
            //// <summary>
            // 是否赠送红包
            ////</summary>
            [ProtoMember(6)]
            [Field("IsGiveRedBag")]
            public bool? IsGiveRedBag{ get; set; }
            //// <summary>
            // 赠送红包金额
            ////</summary>
            [ProtoMember(7)]
            [Field("GiveRedBagMoney")]
            public decimal? GiveRedBagMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 时候中奖
            ////</summary>
            [ProtoMember(9)]
            [Field("IsBonus")]
            public bool? IsBonus{ get; set; }
            //// <summary>
            // 赠送中奖金额
            ////</summary>
            [ProtoMember(10)]
            [Field("GiveBonusMoney")]
            public decimal? GiveBonusMoney{ get; set; }
    }
}