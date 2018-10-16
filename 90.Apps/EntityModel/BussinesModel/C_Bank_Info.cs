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
    [Entity("C_Bank_Info", Type = EntityType.Table)]
    public class C_Bank_Info
    { 
        public C_Bank_Info()
        {
        
        }
            /// <summary>
            /// id
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            /// 银行代号
            ///</summary>
            [ProtoMember(2)]
            [Field("BankCode")]
            public string BankCode { get; set; }
            /// <summary>
            /// 银行名称
            ///</summary>
            [ProtoMember(3)]
            [Field("BankName")]
            public string BankName { get; set; }

        /// <summary>
        /// 是否不可用
        /// </summary>
        [ProtoMember(3)]
        [Field("Disabled")]
        public bool Disabled { get; set; }
    }
}