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
    [Entity("blast_played", Type = EntityType.Table)]
    public class blast_played
    {
        public blast_played()
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
        [Field("name")]
        public string name { get; set; }

        /// <summary>
        /// ±Í«©
        /// </summary>
        
        public string Tag { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
        [Field("enable")]
        public bool enable { get; set; }
        /// <summary>
        // 
        ///</summary>
      //  [ProtoMember(4)]
        [Field("typeid")]
        public int typeid { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(5)]
        [Field("bonusProp")]
        public decimal bonusProp { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(6)]
        [Field("bonusPropBase")]
        public float bonusPropBase { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(7)]
        [Field("selectNum")]
        public int selectNum { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(8)]
        [Field("groupId")]
        public int groupId { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(9)]
        [Field("simpleInfo")]
        public string simpleInfo { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(10)]
        [Field("info")]
        public string info { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(11)]
        [Field("example")]
        public string example { get; set; }

    
        [Field("sort")]
        public int sort { get; set; }

        [Field("playId")]
        public int playId { get; set; }

        [Field("Odds")]
        public decimal Odds { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(12)]
       
        public string ruleFun { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(13)]
        
        public string betCountFun { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(14)]
     
        public string zjMax { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(15)]
      
        public string playedTpl { get; set; }
        /// <summary>
        // 
        ///</summary>
     
    }
}