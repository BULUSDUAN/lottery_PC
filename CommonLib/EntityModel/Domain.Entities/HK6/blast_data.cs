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
    [Entity("blast_data", Type = EntityType.Table)]
    public class blast_data
    {
        public blast_data()
        {

        }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(1)]
        [Field("id", IsIdenty = true, IsPrimaryKey = true)]
        public int id { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(2)]
        [Field("typeid")]
        public int typeid { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("kjtime")]
        public string kjtime { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("kjnumber")]
        public string kjnumber { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("kjdata")]
        public string kjdata { get; set; }

        [Field("issueNo")]
        public int issueNo { get; set; }

        [Field("createTime")]
        public DateTime createTime { get; set; }
        [Field("updateTime")]
        public DateTime updateTime { get; set; }

        [Field("mark")]
        public string mark { get; set; }
    }
}