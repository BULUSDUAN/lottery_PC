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
        // �ں�
        ///</summary>
        [ProtoMember(5)]
        [Field("issueNo")]
        public string issueNo { get; set; }
        /// <summary>
        // Ͷעʱ��
        ///</summary>
        [ProtoMember(6)]
        [Field("betTime")]
        public DateTime betTime { get; set; }

        /// <summary>
        // Ͷעע��
        ///</summary>
        [ProtoMember(7)]
        [Field("betNum")]
        public int betNum { get; set; }
        /// <summary>
        // ֹͣʱ��
        ///</summary>
        [ProtoMember(8)]
        [Field("betStopTime")]
        public DateTime betStopTime { get; set; }
        /// <summary>
        // ����ʱ��
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
        // ����
        ///</summary>
        [ProtoMember(12)]
        [Field("hmEnable")]
        public int hmEnable { get; set; }

        /// <summary>
        // �н�ֹͣ׷��
        ///</summary>
        [ProtoMember(14)]
        [Field("winAnteCodeStop")]
        public bool winAnteCodeStop { get; set; }
        /// <summary>
        // ������
        ///</summary>
        [ProtoMember(15)]
        [Field("redBagMoney")]
        public decimal redBagMoney { get; set; }
        /// <summary>
        // �����������
        ///</summary>
        [ProtoMember(16)]
        [Field("schemeDeduct")]
        public decimal schemeDeduct { get; set; }
        /// <summary>
        // �Ƿ��ѷ���
        ///</summary>
        [ProtoMember(17)]
        [Field("isPayRebate")]
        public int isPayRebate { get; set; }
        /// <summary>
        // �����̱��
        ///</summary>
        [ProtoMember(18)]
        [Field("agentId")]
        public int agentId { get; set; }
        /// <summary>
        // ʵ�ʼ��㷵��Ľ��
        ///</summary>
        [ProtoMember(19)]
        [Field("realPayRebateMoney")]
        public decimal realPayRebateMoney { get; set; }
        /// <summary>
        // �ܷ�����
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
        // ׷������
        ///</summary>
        [ProtoMember(22)]
        [Field("anteCodeNum")]
        public int anteCodeNum { get; set; }

        [Field("BeiSu")]
        public int BeiSu { get; set; }
        /// <summary>
        // ������
        ///</summary>
        [ProtoMember(23)]
        [Field("SchemeId")]
        public string SchemeId { get; set; }

    }
}