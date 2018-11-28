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
    [Entity("blast_bets", Type = EntityType.Table)]
    public class blast_bet_order
    {
        public blast_bet_order()
        {

        }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(1)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Field("SchemeId")]
        public string SchemeId { get; set; }


        [Field("userid")]
        public int userid { get; set; }

        [Field("username")]
        public string username { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        [Field("BetTime")]
        public DateTime BetTime { get; set; }

        /// <summary>
        /// 投注注数
        /// </summary>
        [Field("BetNum")]
        public int BetNum { get; set; }
        /// <summary>
        /// 停止时间
        /// </summary>
        [Field("BetStopTime")]
        public DateTime BetStopTime { get; set; }


        [Field("winTime")]
        public DateTime winTime { get; set; }


        [Field("beiShu")]
        public int beiShu { get; set; }


        [Field("totalMoney")]
        public decimal totalMoney { get; set; }


        [Field("hmEnable")]
        public bool hmEnable { get; set; }

        [Field("winAnteCodeStop")]
        public bool winAnteCodeStop { get; set; }


        [Field("AnteCodeNum")]
        public int AnteCodeNum { get; set; }




        [Field("RedBagMoney")]
        public decimal RedBagMoney { get; set; }

        [Field("SchemeDeduct")]
        public decimal SchemeDeduct { get; set; }


        [Field("IsPayRebate")]
        public bool IsPayRebate { get; set; }


        [Field("AgentId")]
        public int AgentId { get; set; }


        [Field("SuccessMoney")]
        public decimal SuccessMoney { get; set; }

        [Field("RealPayRebateMoney")]
        public decimal RealPayRebateMoney { get; set; }

        [Field("TotalPayRebateMoney")]
        public decimal TotalPayRebateMoney { get; set; }


        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

    }
}