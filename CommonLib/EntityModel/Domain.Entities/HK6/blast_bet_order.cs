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
    [Entity("blast_bet_order", Type = EntityType.Table)]
    public class blast_bet_order
    {
        public blast_bet_order()
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
        [Field("userid")]
        public string userId { get; set; }


        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("displayName")]
        public string displayName { get; set; }
        /// <summary>
        // 期号
        ///</summary>
        [ProtoMember(5)]
        [Field("issueNo")]
        public string issueNo { get; set; }
        /// <summary>
        // 投注时间
        ///</summary>
        [ProtoMember(6)]
        [Field("betTime")]
        public DateTime betTime { get; set; }

        /// <summary>
        // 投注注数
        ///</summary>
        [ProtoMember(7)]
        [Field("betNum")]
        public int betNum { get; set; }
        /// <summary>
        // 停止时间
        ///</summary>
        [ProtoMember(8)]
        [Field("betStopTime")]
        public DateTime betStopTime { get; set; }
        /// <summary>
        // 开奖时间
        ///</summary>
        [ProtoMember(9)]
        [Field("winTime")]
        public DateTime winTime { get; set; }

        /// <summary>
        // 
        ///</summary>
        [ProtoMember(11)]
        [Field("totalMoney")]
        public decimal totalMoney { get; set; }
        /// <summary>
        // 合买
        ///</summary>
        [ProtoMember(12)]
        [Field("hmEnable")]
        public int hmEnable { get; set; }

        /// <summary>
        // 中奖停止追号
        ///</summary>
        [ProtoMember(14)]
        [Field("winAnteCodeStop")]
        public bool winAnteCodeStop { get; set; }
        /// <summary>
        // 红包金额
        ///</summary>
        [ProtoMember(15)]
        [Field("redBagMoney")]
        public decimal redBagMoney { get; set; }
        /// <summary>
        // 方案返利金额
        ///</summary>
        [ProtoMember(16)]
        [Field("schemeDeduct")]
        public decimal schemeDeduct { get; set; }
        /// <summary>
        // 是否已返点
        ///</summary>
        [ProtoMember(17)]
        [Field("isPayRebate")]
        public int isPayRebate { get; set; }
        /// <summary>
        // 代理商编号
        ///</summary>
        [ProtoMember(18)]
        [Field("agentId")]
        public int agentId { get; set; }
        /// <summary>
        // 实际计算返点的金额
        ///</summary>
        [ProtoMember(19)]
        [Field("realPayRebateMoney")]
        public decimal realPayRebateMoney { get; set; }
        /// <summary>
        // 总返点金额
        ///</summary>
        [ProtoMember(20)]
        [Field("TotalPayRebateMoney")]
        public decimal TotalPayRebateMoney { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(21)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        // 追期期数
        ///</summary>
        [ProtoMember(22)]
        [Field("anteCodeNum")]
        public int anteCodeNum { get; set; }

        [Field("BeiSu")]
        public int BeiSu { get; set; }
        /// <summary>
        // 订单号
        ///</summary>
        [ProtoMember(23)]
        [Field("SchemeId")]
        public string SchemeId { get; set; }

    }
}