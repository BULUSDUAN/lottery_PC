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
    [Entity("E_A20150919_已绑定身份和手机的用户登录送红包",Type = EntityType.Table)]
    public class E_A20150919_已绑定身份和手机的用户登录送红包
    { 
        public E_A20150919_已绑定身份和手机的用户登录送红包()
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
            // 登录时间
            ////</summary>
            [ProtoMember(3)]
            [Field("LoginDate")]
            public string LoginDate{ get; set; }
            //// <summary>
            // 赠送红包金额
            ////</summary>
            [ProtoMember(4)]
            [Field("GiveRedBagMoney")]
            public decimal? GiveRedBagMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}