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
    // ºÅÂë±í
    ///</summary>
    [ProtoContract]
    [Entity("blast_lhc_antecode", Type = EntityType.Table)]
    public class blast_lhc_antecode
    {
        public blast_lhc_antecode()
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
        [Field("odds")]
        public decimal odds { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("displayName")]
        public string displayName { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("AnteCode")]
        public string AnteCode { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("playid")]
        public int playid { get; set; }
        [Field("sort")]
        public int sort { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(6)]
        [Field("remark")]
        public string remark { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(7)]
        [Field("enable")]
        public bool enable { get; set; }

        [Field("fag")]
        public string fag { get; set; }
        [Field("createTime")]
        public DateTime createTime { get; set; }

        [Field("updateTime")]
        public DateTime updateTime { get; set; }
        [Field("cateNum")]
        public int cateNum { get; set; }
        [Field("typeid")]
        public int typeid { get; set; }

        public string CodeContent { get; set; }

        //[Field("antuSchemeId")]
        //public string antuSchemeId { get; set; }
    }
}