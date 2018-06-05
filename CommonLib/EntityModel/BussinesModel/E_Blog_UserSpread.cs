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
    // yqid普通用户推广
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_UserSpread",Type = EntityType.Table)]
    public class E_Blog_UserSpread
    { 
        public E_Blog_UserSpread()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 用户姓名
            ///</summary>
            [ProtoMember(3)]
            [Field("userName")]
            public string userName{ get; set; }
            /// <summary>
            // 代理商编号
            ///</summary>
            [ProtoMember(4)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CrateTime")]
            public DateTime CrateTime{ get; set; }
            /// <summary>
            // 传统足球
            ///</summary>
            [ProtoMember(6)]
            [Field("CTZQ")]
            public decimal CTZQ{ get; set; }
            /// <summary>
            // 北京单场
            ///</summary>
            [ProtoMember(7)]
            [Field("BJDC")]
            public decimal BJDC{ get; set; }
            /// <summary>
            // 竞彩足球
            ///</summary>
            [ProtoMember(8)]
            [Field("JCZQ")]
            public decimal JCZQ{ get; set; }
            /// <summary>
            // 竞彩篮球
            ///</summary>
            [ProtoMember(9)]
            [Field("JCLQ")]
            public decimal JCLQ{ get; set; }
            /// <summary>
            // SZC
            ///</summary>
            [ProtoMember(10)]
            [Field("SZC")]
            public decimal SZC{ get; set; }
            /// <summary>
            // GPC
            ///</summary>
            [ProtoMember(11)]
            [Field("GPC")]
            public decimal GPC{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(12)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}