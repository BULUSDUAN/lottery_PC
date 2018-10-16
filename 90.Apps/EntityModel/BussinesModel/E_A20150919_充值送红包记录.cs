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
    [Entity("E_A20150919_充值送红包记录", Type = EntityType.Table)]
    public class E_A20150919_充值送红包记录
    {
        public E_A20150919_充值送红包记录()
        {

        }
        /// <summary>
        // 主键
        ///</summary>
        [ProtoMember(1)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        // 用户编号
        ///</summary>
        [ProtoMember(2)]
        [Field("UserId")]
        public string UserId { get; set; }
        /// <summary>
        // 订单编号
        ///</summary>
        [ProtoMember(3)]
        [Field("OrderId")]
        public string OrderId { get; set; }
        /// <summary>
        // 赠送月份
        ///</summary>
        [ProtoMember(4)]
        [Field("GiveMonth")]
        public string GiveMonth { get; set; }
        /// <summary>
        // 充值金额
        ///</summary>
        [ProtoMember(5)]
        [Field("FillMoney")]
        public decimal FillMoney { get; set; }
        /// <summary>
        // 赠送金额
        ///</summary>
        [ProtoMember(6)]
        [Field("GiveMoney")]
        public decimal GiveMoney { get; set; }
        /// <summary>
        // 创建时间
        ///</summary>
        [ProtoMember(7)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 网银快捷充值送红包不设上限，用来区分每月上限红包
        /// PayType=1 充值赠送  PayType=2 充值类型赠送
        /// </summary>
        [ProtoMember(8)]
        [Field("PayType")]
        public int? PayType { get; set; }
    }
}