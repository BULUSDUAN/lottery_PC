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
    [Entity("C_BankCard",Type = EntityType.Table)]
    public class C_BankCard
    { 
        public C_BankCard()
        {
        
        }
            //// <summary>
            // id
            ////</summary>
            [ProtoMember(1)]
            [Field("BId", IsIdenty = true, IsPrimaryKey = true)]
            public int BId{ get; set; }
            //// <summary>
            // 用户ID
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 用户真实姓名
            ////</summary>
            [ProtoMember(3)]
            [Field("RealName")]
            public string RealName{ get; set; }
            //// <summary>
            // 省份名称
            ////</summary>
            [ProtoMember(4)]
            [Field("ProvinceName")]
            public string ProvinceName{ get; set; }
            //// <summary>
            // 城市名称
            ////</summary>
            [ProtoMember(5)]
            [Field("CityName")]
            public string CityName{ get; set; }
            //// <summary>
            // 银行名称
            ////</summary>
            [ProtoMember(6)]
            [Field("BankName")]
            public string BankName{ get; set; }
            //// <summary>
            // 开户支行名称
            ////</summary>
            [ProtoMember(7)]
            [Field("BankSubName")]
            public string BankSubName{ get; set; }
            //// <summary>
            // 银行编号
            ////</summary>
            [ProtoMember(8)]
            [Field("BankCode")]
            public string BankCode{ get; set; }
            //// <summary>
            // 银行卡卡号
            ////</summary>
            [ProtoMember(9)]
            [Field("BankCardNumber")]
            public string BankCardNumber{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(10)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(11)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}