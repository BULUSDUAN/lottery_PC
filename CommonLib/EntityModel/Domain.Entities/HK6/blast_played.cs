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
    [Entity("blast_played",Type = EntityType.Table)]
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
            public int id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("name")]
            public string name{ get; set; }

        /// <summary>
        /// ±Í«©
        /// </summary>
        [ProtoMember(2)]
        [Field("Tag")]
        public string Tag { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(3)]
            [Field("enable")]
            public sbyte enable{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("typeid")]
            public sbyte typeid{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("bonusProp")]
            public float bonusProp{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("bonusPropBase")]
            public float bonusPropBase{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("selectNum")]
            public sbyte selectNum{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("groupId")]
            public short groupId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("simpleInfo")]
            public string simpleInfo{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("info")]
            public string info{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("example")]
            public string example{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("ruleFun")]
            public string ruleFun{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("betCountFun")]
            public string betCountFun{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("zjMax")]
            public string zjMax{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("playedTpl")]
            public string playedTpl{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("sort")]
            public int sort{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("minCharge")]
            public int minCharge{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("allCount")]
            public int allCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("maxCount")]
            public int maxCount{ get; set; }
    }
}