using EntityModel.Enum;
using EntityModel.Interface;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    /// <summary>
    /// 竞彩足球 比分SP
    /// </summary>
    [Entity("C_JCZQ_BF_SP", Type = EntityType.Table)]
    public class C_JCZQ_BF_SP :IBallBaseInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [Field("MatchId")]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [Field("MatchData")]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [Field("MatchNumber")]
        public string MatchNumber { get; set; }
        [Field("Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 胜-其它
        /// </summary>
        [Field("S_QT")]
        public decimal S_QT { get; set; }
        /// <summary>
        /// 胜-10
        /// </summary>
        [Field("S_10")]
        public decimal S_10 { get; set; }
        /// <summary>
        /// 胜-20
        /// </summary>
        [Field("S_20")]
        public decimal S_20 { get; set; }
        /// <summary>
        /// 胜-21
        /// </summary>
        [Field("S_21")]
        public decimal S_21 { get; set; }
        /// <summary>
        /// 胜-30
        /// </summary>
        [Field("S_30")]
        public decimal S_30 { get; set; }
        /// <summary>
        /// 胜-31
        /// </summary>
        [Field("S_31")]
        public decimal S_31 { get; set; }
        /// <summary>
        /// 胜-32
        /// </summary>
        [Field("S_32")]
        public decimal S_32 { get; set; }
        /// <summary>
        /// 胜-40
        /// </summary>
        [Field("S_40")]
        public decimal S_40 { get; set; }
        /// <summary>
        /// 胜-41
        /// </summary>
        [Field("S_41")]
        public decimal S_41 { get; set; }
        /// <summary>
        /// 胜-42
        /// </summary>
        [Field("S_42")]
        public decimal S_42 { get; set; }
        /// <summary>
        /// 胜-50
        /// </summary>
        [Field("S_50")]
        public decimal S_50 { get; set; }
        /// <summary>
        /// 胜-51
        /// </summary>
        [Field("S_51")]
        public decimal S_51 { get; set; }
        /// <summary>
        /// 胜-52
        /// </summary>
        [Field("S_52")]
        public decimal S_52 { get; set; }
        /// <summary>
        /// 平-其它
        /// </summary>
        [Field("P_QT")]
        public decimal P_QT { get; set; }
        /// <summary>
        /// 平-00
        /// </summary>
        [Field("P_00")]
        public decimal P_00 { get; set; }
        /// <summary>
        /// 平-11
        /// </summary>
        [Field("P_11")]
        public decimal P_11 { get; set; }
        /// <summary>
        /// 平-22
        /// </summary>
        [Field("P_22")]
        public decimal P_22 { get; set; }
        /// <summary>
        /// 平-33
        /// </summary>
        [Field("P_33")]
        public decimal P_33 { get; set; }
        /// <summary>
        /// 负-其它
        /// </summary>
        [Field("F_QT")]
        public decimal F_QT { get; set; }
        /// <summary>
        /// 负-01
        /// </summary>
        [Field("F_01")]
        public decimal F_01 { get; set; }
        /// <summary>
        /// 负-02
        /// </summary>
        [Field("F_02")]
        public decimal F_02 { get; set; }
        /// <summary>
        /// 负-12
        /// </summary>
        [Field("F_12")]
        public decimal F_12 { get; set; }
        /// <summary>
        /// 负-03
        /// </summary>
        [Field("F_03")]
        public decimal F_03 { get; set; }
        /// <summary>
        /// 负-13
        /// </summary>
        [Field("F_13")]
        public decimal F_13 { get; set; }
        /// <summary>
        /// 负-23
        /// </summary>
        [Field("F_23")]
        public decimal F_23 { get; set; }
        /// <summary>
        /// 负-04
        /// </summary>
        [Field("F_04")]
        public decimal F_04 { get; set; }
        /// <summary>
        /// 负-14
        /// </summary>
        [Field("F_14")]
        public decimal F_14 { get; set; }
        /// <summary>
        /// 负-24
        /// </summary>
        [Field("F_24")]
        public decimal F_24 { get; set; }
        /// <summary>
        /// 负-05
        /// </summary>
        [Field("F_05")]
        public decimal F_05 { get; set; }
        /// <summary>
        /// 负-15
        /// </summary>
        [Field("F_15")]
        public decimal F_15 { get; set; }
        /// <summary>
        /// 负-25
        /// </summary>
        [Field("F_25")]
        public decimal F_25 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }
        public string NoSaleState { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_BF_SP;
            if (t.F_01 != this.F_01
                || t.F_02 != this.F_02
                || t.F_03 != this.F_03
                || t.F_04 != this.F_04
                || t.F_05 != this.F_05
                || t.F_12 != this.F_12
                || t.F_13 != this.F_13
                || t.F_14 != this.F_14
                || t.F_15 != this.F_15
                || t.F_23 != this.F_23
                || t.F_24 != this.F_24
                || t.F_25 != this.F_25
                || t.F_QT != this.F_QT
                || t.P_00 != this.P_00
                || t.P_11 != this.P_11
                || t.P_22 != this.P_22
                || t.P_33 != this.P_33
                || t.P_QT != this.P_QT
                || t.S_10 != this.S_10
                || t.S_20 != this.S_20
                || t.S_21 != this.S_21
                || t.S_30 != this.S_30
                || t.S_31 != this.S_31
                || t.S_32 != this.S_32
                || t.S_40 != this.S_40
                || t.S_41 != this.S_41
                || t.S_42 != this.S_42
                || t.S_50 != this.S_50
                || t.S_51 != this.S_51
                || t.S_52 != this.S_52
                || t.S_QT != this.S_QT
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;

            return true;
        }
    }
}
