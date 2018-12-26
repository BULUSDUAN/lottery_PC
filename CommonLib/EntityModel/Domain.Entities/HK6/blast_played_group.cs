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
    [Entity("blast_played_group", Type = EntityType.Table)]
    public class blast_played_group
    {
        public blast_played_group()
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
        [Field("enable")]
        public bool enable { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("typeid")]
        public int typeid { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("groupName")]
        public string groupName { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("sort")]
        public int sort { get; set; }
        [Field("groupId")]
        public int groupId { get; set; }


        public List<blast_played> PlayedList { get; set; }
    }
}