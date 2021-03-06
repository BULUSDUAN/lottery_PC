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
    [Entity("blast_lhc_time", Type = EntityType.Table)]
    public class blast_lhc_time
    {
        public blast_lhc_time()
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
        public sbyte typeid { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("actionNo")]
        public int actionNo { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("actionTime")]
        public DateTime actionTime { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("stopTime")]
        public DateTime stopTime { get; set; }


        public string winNum { get; set; }

        public blast_data PreData {get;set;}

       

    }



    public class blast_time
    {
        public blast_time()
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
        public sbyte typeid { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("actionNo")]
        public int actionNo { get; set; }

        public int actionNum { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("actionTime")]
        public DateTime actionTime { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("stopTime")]
        public DateTime stopTime { get; set; }


        public string winNum { get; set; }

        public blast_data PreData { get; set; }



    }
}