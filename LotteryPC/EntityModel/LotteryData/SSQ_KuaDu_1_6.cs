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
    [Entity("SSQ_KuaDu_1_6",Type = EntityType.Table)]
    public class SSQ_KuaDu_1_6
    { 
        public SSQ_KuaDu_1_6()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("WinNumber")]
            public string WinNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("RedLotteryNumber")]
            public string RedLotteryNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("KuaDu_12")]
            public int KuaDu_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("KuaDu_W_12")]
            public int KuaDu_W_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("KuaDu_23")]
            public int KuaDu_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("KuaDu_W_23")]
            public int KuaDu_W_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("KuaDu_34")]
            public int KuaDu_34{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("KuaDu_W_34")]
            public int KuaDu_W_34{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("KuaDu_45")]
            public int KuaDu_45{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("KuaDu_W_45")]
            public int KuaDu_W_45{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("KuaDu_56")]
            public int KuaDu_56{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("KuaDu_W_56")]
            public int KuaDu_W_56{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("KD_12_1")]
            public int KD_12_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("KD_12_2")]
            public int KD_12_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("KD_12_3")]
            public int KD_12_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("KD_12_4")]
            public int KD_12_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("KD_12_5")]
            public int KD_12_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("KD_12_6")]
            public int KD_12_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("KD_12_7")]
            public int KD_12_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("KD_12_8")]
            public int KD_12_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("KD_12_9")]
            public int KD_12_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("KD_12_10")]
            public int KD_12_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("KD_12_11")]
            public int KD_12_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("KD_12_12")]
            public int KD_12_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("KD_12_13")]
            public int KD_12_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("KD_12_14")]
            public int KD_12_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("KD_12_15")]
            public int KD_12_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("KD_12_16")]
            public int KD_12_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("KD_12_17")]
            public int KD_12_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("KD_12_18")]
            public int KD_12_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("KD_12_19")]
            public int KD_12_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("KD_12_20")]
            public int KD_12_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("KD_12_21")]
            public int KD_12_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("KD_12_22")]
            public int KD_12_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("KD_12_23")]
            public int KD_12_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("KD_12_24")]
            public int KD_12_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("KD_12_25")]
            public int KD_12_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("KD_12_26")]
            public int KD_12_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("KD_12_27")]
            public int KD_12_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("KD_12_28")]
            public int KD_12_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("KDW_12_0")]
            public int KDW_12_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("KDW_12_1")]
            public int KDW_12_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("KDW_12_2")]
            public int KDW_12_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("KDW_12_3")]
            public int KDW_12_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("KDW_12_4")]
            public int KDW_12_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("KDW_12_5")]
            public int KDW_12_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("KDW_12_6")]
            public int KDW_12_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("KDW_12_7")]
            public int KDW_12_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("KDW_12_8")]
            public int KDW_12_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("KDW_12_9")]
            public int KDW_12_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("KD_23_1")]
            public int KD_23_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("KD_23_2")]
            public int KD_23_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(55)]
            [Field("KD_23_3")]
            public int KD_23_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(56)]
            [Field("KD_23_4")]
            public int KD_23_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(57)]
            [Field("KD_23_5")]
            public int KD_23_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(58)]
            [Field("KD_23_6")]
            public int KD_23_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(59)]
            [Field("KD_23_7")]
            public int KD_23_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(60)]
            [Field("KD_23_8")]
            public int KD_23_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(61)]
            [Field("KD_23_9")]
            public int KD_23_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(62)]
            [Field("KD_23_10")]
            public int KD_23_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(63)]
            [Field("KD_23_11")]
            public int KD_23_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(64)]
            [Field("KD_23_12")]
            public int KD_23_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(65)]
            [Field("KD_23_13")]
            public int KD_23_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(66)]
            [Field("KD_23_14")]
            public int KD_23_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(67)]
            [Field("KD_23_15")]
            public int KD_23_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(68)]
            [Field("KD_23_16")]
            public int KD_23_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(69)]
            [Field("KD_23_17")]
            public int KD_23_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(70)]
            [Field("KD_23_18")]
            public int KD_23_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(71)]
            [Field("KD_23_19")]
            public int KD_23_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(72)]
            [Field("KD_23_20")]
            public int KD_23_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(73)]
            [Field("KD_23_21")]
            public int KD_23_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(74)]
            [Field("KD_23_22")]
            public int KD_23_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(75)]
            [Field("KD_23_23")]
            public int KD_23_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(76)]
            [Field("KD_23_24")]
            public int KD_23_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(77)]
            [Field("KD_23_25")]
            public int KD_23_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(78)]
            [Field("KD_23_26")]
            public int KD_23_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(79)]
            [Field("KD_23_27")]
            public int KD_23_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(80)]
            [Field("KD_23_28")]
            public int KD_23_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(81)]
            [Field("KDW_23_0")]
            public int KDW_23_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(82)]
            [Field("KDW_23_1")]
            public int KDW_23_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(83)]
            [Field("KDW_23_2")]
            public int KDW_23_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(84)]
            [Field("KDW_23_3")]
            public int KDW_23_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(85)]
            [Field("KDW_23_4")]
            public int KDW_23_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(86)]
            [Field("KDW_23_5")]
            public int KDW_23_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(87)]
            [Field("KDW_23_6")]
            public int KDW_23_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(88)]
            [Field("KDW_23_7")]
            public int KDW_23_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(89)]
            [Field("KDW_23_8")]
            public int KDW_23_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(90)]
            [Field("KDW_23_9")]
            public int KDW_23_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(91)]
            [Field("KD_34_1")]
            public int KD_34_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(92)]
            [Field("KD_34_2")]
            public int KD_34_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(93)]
            [Field("KD_34_3")]
            public int KD_34_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(94)]
            [Field("KD_34_4")]
            public int KD_34_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(95)]
            [Field("KD_34_5")]
            public int KD_34_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(96)]
            [Field("KD_34_6")]
            public int KD_34_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(97)]
            [Field("KD_34_7")]
            public int KD_34_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(98)]
            [Field("KD_34_8")]
            public int KD_34_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(99)]
            [Field("KD_34_9")]
            public int KD_34_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(100)]
            [Field("KD_34_10")]
            public int KD_34_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(101)]
            [Field("KD_34_11")]
            public int KD_34_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(102)]
            [Field("KD_34_12")]
            public int KD_34_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(103)]
            [Field("KD_34_13")]
            public int KD_34_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(104)]
            [Field("KD_34_14")]
            public int KD_34_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(105)]
            [Field("KD_34_15")]
            public int KD_34_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(106)]
            [Field("KD_34_16")]
            public int KD_34_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(107)]
            [Field("KD_34_17")]
            public int KD_34_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(108)]
            [Field("KD_34_18")]
            public int KD_34_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(109)]
            [Field("KD_34_19")]
            public int KD_34_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(110)]
            [Field("KD_34_20")]
            public int KD_34_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(111)]
            [Field("KD_34_21")]
            public int KD_34_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(112)]
            [Field("KD_34_22")]
            public int KD_34_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(113)]
            [Field("KD_34_23")]
            public int KD_34_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(114)]
            [Field("KD_34_24")]
            public int KD_34_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(115)]
            [Field("KD_34_25")]
            public int KD_34_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(116)]
            [Field("KD_34_26")]
            public int KD_34_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(117)]
            [Field("KD_34_27")]
            public int KD_34_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(118)]
            [Field("KD_34_28")]
            public int KD_34_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(119)]
            [Field("KDW_34_0")]
            public int KDW_34_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(120)]
            [Field("KDW_34_1")]
            public int KDW_34_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(121)]
            [Field("KDW_34_2")]
            public int KDW_34_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(122)]
            [Field("KDW_34_3")]
            public int KDW_34_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(123)]
            [Field("KDW_34_4")]
            public int KDW_34_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(124)]
            [Field("KDW_34_5")]
            public int KDW_34_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(125)]
            [Field("KDW_34_6")]
            public int KDW_34_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(126)]
            [Field("KDW_34_7")]
            public int KDW_34_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(127)]
            [Field("KDW_34_8")]
            public int KDW_34_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(128)]
            [Field("KDW_34_9")]
            public int KDW_34_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(129)]
            [Field("KD_45_1")]
            public int KD_45_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(130)]
            [Field("KD_45_2")]
            public int KD_45_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(131)]
            [Field("KD_45_3")]
            public int KD_45_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(132)]
            [Field("KD_45_4")]
            public int KD_45_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(133)]
            [Field("KD_45_5")]
            public int KD_45_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(134)]
            [Field("KD_45_6")]
            public int KD_45_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(135)]
            [Field("KD_45_7")]
            public int KD_45_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(136)]
            [Field("KD_45_8")]
            public int KD_45_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(137)]
            [Field("KD_45_9")]
            public int KD_45_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(138)]
            [Field("KD_45_10")]
            public int KD_45_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(139)]
            [Field("KD_45_11")]
            public int KD_45_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(140)]
            [Field("KD_45_12")]
            public int KD_45_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(141)]
            [Field("KD_45_13")]
            public int KD_45_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(142)]
            [Field("KD_45_14")]
            public int KD_45_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(143)]
            [Field("KD_45_15")]
            public int KD_45_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(144)]
            [Field("KD_45_16")]
            public int KD_45_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(145)]
            [Field("KD_45_17")]
            public int KD_45_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(146)]
            [Field("KD_45_18")]
            public int KD_45_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(147)]
            [Field("KD_45_19")]
            public int KD_45_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(148)]
            [Field("KD_45_20")]
            public int KD_45_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(149)]
            [Field("KD_45_21")]
            public int KD_45_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(150)]
            [Field("KD_45_22")]
            public int KD_45_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(151)]
            [Field("KD_45_23")]
            public int KD_45_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(152)]
            [Field("KD_45_24")]
            public int KD_45_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(153)]
            [Field("KD_45_25")]
            public int KD_45_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(154)]
            [Field("KD_45_26")]
            public int KD_45_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(155)]
            [Field("KD_45_27")]
            public int KD_45_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(156)]
            [Field("KD_45_28")]
            public int KD_45_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(157)]
            [Field("KDW_45_0")]
            public int KDW_45_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(158)]
            [Field("KDW_45_1")]
            public int KDW_45_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(159)]
            [Field("KDW_45_2")]
            public int KDW_45_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(160)]
            [Field("KDW_45_3")]
            public int KDW_45_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(161)]
            [Field("KDW_45_4")]
            public int KDW_45_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(162)]
            [Field("KDW_45_5")]
            public int KDW_45_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(163)]
            [Field("KDW_45_6")]
            public int KDW_45_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(164)]
            [Field("KDW_45_7")]
            public int KDW_45_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(165)]
            [Field("KDW_45_8")]
            public int KDW_45_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(166)]
            [Field("KDW_45_9")]
            public int KDW_45_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(167)]
            [Field("KD_56_1")]
            public int KD_56_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(168)]
            [Field("KD_56_2")]
            public int KD_56_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(169)]
            [Field("KD_56_3")]
            public int KD_56_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(170)]
            [Field("KD_56_4")]
            public int KD_56_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(171)]
            [Field("KD_56_5")]
            public int KD_56_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(172)]
            [Field("KD_56_6")]
            public int KD_56_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(173)]
            [Field("KD_56_7")]
            public int KD_56_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(174)]
            [Field("KD_56_8")]
            public int KD_56_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(175)]
            [Field("KD_56_9")]
            public int KD_56_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(176)]
            [Field("KD_56_10")]
            public int KD_56_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(177)]
            [Field("KD_56_11")]
            public int KD_56_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(178)]
            [Field("KD_56_12")]
            public int KD_56_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(179)]
            [Field("KD_56_13")]
            public int KD_56_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(180)]
            [Field("KD_56_14")]
            public int KD_56_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(181)]
            [Field("KD_56_15")]
            public int KD_56_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(182)]
            [Field("KD_56_16")]
            public int KD_56_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(183)]
            [Field("KD_56_17")]
            public int KD_56_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(184)]
            [Field("KD_56_18")]
            public int KD_56_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(185)]
            [Field("KD_56_19")]
            public int KD_56_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(186)]
            [Field("KD_56_20")]
            public int KD_56_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(187)]
            [Field("KD_56_21")]
            public int KD_56_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(188)]
            [Field("KD_56_22")]
            public int KD_56_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(189)]
            [Field("KD_56_23")]
            public int KD_56_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(190)]
            [Field("KD_56_24")]
            public int KD_56_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(191)]
            [Field("KD_56_25")]
            public int KD_56_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(192)]
            [Field("KD_56_26")]
            public int KD_56_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(193)]
            [Field("KD_56_27")]
            public int KD_56_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(194)]
            [Field("KD_56_28")]
            public int KD_56_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(195)]
            [Field("KDW_56_0")]
            public int KDW_56_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(196)]
            [Field("KDW_56_1")]
            public int KDW_56_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(197)]
            [Field("KDW_56_2")]
            public int KDW_56_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(198)]
            [Field("KDW_56_3")]
            public int KDW_56_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(199)]
            [Field("KDW_56_4")]
            public int KDW_56_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(200)]
            [Field("KDW_56_5")]
            public int KDW_56_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(201)]
            [Field("KDW_56_6")]
            public int KDW_56_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(202)]
            [Field("KDW_56_7")]
            public int KDW_56_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(203)]
            [Field("KDW_56_8")]
            public int KDW_56_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(204)]
            [Field("KDW_56_9")]
            public int KDW_56_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(205)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}