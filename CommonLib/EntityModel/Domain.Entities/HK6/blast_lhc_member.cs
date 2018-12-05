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
    //   会员表
    ///</summary>
    [ProtoContract]
    [Entity("blast_member", Type = EntityType.Table)]
    public class blast_member
    { 
        public blast_member()
        {
        
        }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(1)]
        [Field("id", IsIdenty = true, IsPrimaryKey = true)]
        public int id { get; set; }

        [ProtoMember(1)]
        [Field("userId")]
        public string userId { get; set; }



        /// <summary>
        // 
        ///</summary>
        [ProtoMember(2)]
        [Field("loginName")]
        public string loginName { get; set; }

        /// <summary>
        /// 游戏金额
        /// </summary>
        [ProtoMember(2)]
        [Field("gameMoney")]
        public decimal gameMoney { get; set; }

        [Field("createTime")]
        public  DateTime createTime { get; set; }
        [Field("updateTime")]
        public  DateTime updateTime { get; set; }
    }
}