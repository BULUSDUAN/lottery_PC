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
    [Entity("E_SMSLog",Type = EntityType.Table)]
    public class E_SMSLog
    { 
        public E_SMSLog()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("Category")]
            public string Category{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("MoblieNumber")]
            public string MoblieNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("Content")]
            public string Content{ get; set; }
    }
}