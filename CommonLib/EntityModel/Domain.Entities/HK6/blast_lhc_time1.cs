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
    [Entity("blast_lhc_time",Type = EntityType.Table)]
    public class blast_lhc_time1
    { 
        public blast_lhc_time1()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("id", IsIdenty = true, IsPrimaryKey = true)]
            public int id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("typeid")]
            public sbyte typeid{ get; set; }
            /// <summary>
            //  �����ں�
            ///</summary>
            [ProtoMember(3)]
            [Field("actionNo")]
            public uint actionNo{ get; set; }
            /// <summary>
            //  ����ʱ��
            ///</summary>
            [ProtoMember(4)]
            [Field("actionTime")]
            public DateTime actionTime{ get; set; }
    }
}