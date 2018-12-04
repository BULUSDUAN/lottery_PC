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
    [Entity("blast_bet_orderdetail", Type = EntityType.Table)]
    public class blast_bet_orderdetail
    {
        public blast_bet_orderdetail()
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
        [Field("SchemeId")]
        public string SchemeId { get; set; }

        [Field("userId")]
        public string userId { get; set; }


        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("GameType")]
        public string GameType { get; set; }
        /// <summary>
        // �ں�
        ///</summary>
        [ProtoMember(5)]
        [Field("playId")]
        public int playId { get; set; }
        /// <summary>
        // �ں�
        ///</summary>
        [ProtoMember(6)]
        [Field("issueNo")]
        public string issueNo { get; set; }

        [Field("anteCodeNum")]
        public int anteCodeNum { get; set; }
        [Field("BeiSu")]
        public int BeiSu { get; set; } = 1;

        /// <summary>
        // Ͷע��
        ///</summary>
      
        [Field("AnteCodes")]
        public string AnteCodes { get; set; }
        /// <summary>
        // ����
        ///</summary>
        [ProtoMember(8)]
        [Field("Odds")]
        public decimal Odds { get; set; }
        /// <summary>
        // BonusStatus ��Ϊ2�н�,3���н�Ϊ�ںŹ�,4�;�
        ///</summary>
        [ProtoMember(9)]
        [Field("BonusStatus")]
        public int BonusStatus { get; set; }

        /// <summary>
        // �н����
        ///</summary>
        [ProtoMember(11)]
        [Field("BonusAwardsMoney")]
        public decimal BonusAwardsMoney { get; set; }
        /// <summary>
        // ˰ǰ����
        ///</summary>
        [ProtoMember(12)]
        [Field("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }

        /// <summary>
        // ˰�󽱽�
        ///</summary>
        [ProtoMember(14)]
        [Field("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        // ������
        ///</summary>
        [ProtoMember(15)]
        [Field("winNumber")]
        public string winNumber { get; set; }

        [Field("winNumberDesc")]
        public string winNumberDesc { get; set; }
        /// <summary>
        // ����״̬
        ///</summary>
        [ProtoMember(16)]
        [Field("ProgressStatus")]
        public int ProgressStatus { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(17)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }


        [Field("updateTime")]
        public DateTime updateTime { get; set; }
        /// <summary>
        /// Ͷע����
        /// </summary>
        [Field("unitPrice")]
        public decimal unitPrice { get; set; }

        [Field("issueDate")]
        public string issueDate { get; set; }

  

        /// <summary>
        /// ÿ�����Ӧ������
        /// </summary>
        public string OddsArr { get; set; }

    }
}