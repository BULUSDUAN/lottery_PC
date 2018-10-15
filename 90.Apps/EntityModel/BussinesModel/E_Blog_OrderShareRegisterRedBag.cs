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
    // 用户获奖记录
    ///</summary>
    [ProtoContract]
    [Entity("E_Blog_OrderShareRegisterRedBag", Type = EntityType.Table)]
    public class E_Blog_OrderShareRegisterRedBag
    {
        public E_Blog_OrderShareRegisterRedBag()
        {

        }
        /// <summary>
        // 主键
        ///</summary>
        [ProtoMember(1)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(2)]
        [Field("SchemeId")]
        public string SchemeId { get; set; }
        /// <summary>
        // 用户ID
        ///</summary>
        [ProtoMember(3)]
        [Field("UserId")]
        public string UserId { get; set; }
        /// <summary>
        // 是否给红包
        ///</summary>
        [ProtoMember(4)]
        [Field("IsGiveRegisterRedBag")]
        public bool IsGiveRegisterRedBag { get; set; }

        /// <summary>
        // 注册
        ///</summary>
        [ProtoMember(5)]
        [Field("RegisterCount")]
        public int? RegisterCount { get; set; }

        /// <summary>
        // 创建时间
        ///</summary>
        [ProtoMember(6)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        // 更新时间
        ///</summary>
        [ProtoMember(7)]
        [Field("UpdateTime")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        // 红包
        ///</summary>
        [ProtoMember(8)]
        [Field("RedBagPre")]
        public decimal RedBagPre { get; set; }

        /// <summary>
        // 红包
        ///</summary>
        [ProtoMember(9)]
        [Field("RedBagMoney")]
        public decimal RedBagMoney { get; set; }

    }
}
