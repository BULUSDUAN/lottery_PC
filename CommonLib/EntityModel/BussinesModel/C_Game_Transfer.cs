using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    [ProtoContract]
    [Entity("C_Game_Transfer", Type = EntityType.Table)]
    public class C_Game_Transfer
    {
        public C_Game_Transfer()
        {

        }
        /// <summary>
        // 订单号
        ///</summary>
        [ProtoMember(1)]
        [Field("OrderId", IsIdenty = false, IsPrimaryKey = true)]
        public string OrderId { get; set; }
        /// <summary>
        // 申请人
        ///</summary>
        [ProtoMember(2)]
        [Field("UserId")]
        public string UserId { get; set; }
        /// <summary>
        // 转账类型
        ///</summary>
        [ProtoMember(3)]
        [Field("TransferType")]
        public int TransferType { get; set; }
        /// <summary>
        // 申请转账金额
        ///</summary>
        [ProtoMember(4)]
        [Field("RequestMoney")]
        public decimal RequestMoney { get; set; }
        /// <summary>
        // 申请时间
        ///</summary>
        [ProtoMember(5)]
        [Field("RequestTime")]
        public DateTime RequestTime { get; set; }
        /// <summary>
        // 提款状态
        ///</summary>
        [ProtoMember(6)]
        [Field("Status")]
        public int Status { get; set; }
        /// <summary>
        // 更新时间
        ///</summary>
        [ProtoMember(7)]
        [Field("UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ProtoMember(8)]
        [Field("UserDisplayName")]
        public string UserDisplayName { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        [ProtoMember(9)]
        [Field("ProviderSerialNo")]
        public string ProviderSerialNo { get; set; }

        /// <summary>
        /// 游戏类型（0普通，1捕鱼捕鸟）
        /// </summary>
        [ProtoMember(10)]
        [Field("GameType")]
        public int GameType { get; set; }
    }
}
