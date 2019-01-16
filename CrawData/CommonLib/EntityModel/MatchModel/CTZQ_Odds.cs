using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.MatchModel
{
    [Entity("C_CTZQ_Odds", Type = EntityType.Table)]
    public class C_CTZQ_Odds
    {
        /// <summary>
        /// 编号 = GameCode|GameType|IssuseNumber|OrderNumber
        /// </summary>
      //  [EntityMappingField("Id", IsKey = true, NeedUpdate = false)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 离散指数-胜
        /// </summary>
        [Field("LSWin")]
        public decimal LSWin { get; set; }
        /// <summary>
        /// 离散指数-负
        /// </summary>
        [Field("LSLose")]
        public decimal LSLose { get; set; }
        /// <summary>
        /// 离散指数-平
        /// </summary>
        [Field("LSFlat")]
        public decimal LSFlat { get; set; }
        /// <summary>
        /// 凯利指数-胜
        /// </summary>
        [Field("KLWin")]
        public decimal KLWin { get; set; }
        /// <summary>
        /// 凯利指数-负
        /// </summary>
        [Field("KLLose")]
        public decimal KLLose { get; set; }
        /// <summary>
        /// 凯利指数-平
        /// </summary>
        [Field("KLFlat")]
        public decimal KLFlat { get; set; }
        /// <summary>
        /// 亚盘水位
        /// </summary>
        [Field("YPSW")]
        public string YPSW { get; set; }
        /// <summary>
        /// 平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        [Field("AverageOdds")]
        public string AverageOdds { get; set; }
        /// <summary>
        /// 半场平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        [Field("HalfAverageOdds")]
        public string HalfAverageOdds { get; set; }
        /// <summary>
        /// 全场平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        [Field("FullAverageOdds")]
        public string FullAverageOdds { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Field("UpdateTime")]
        public string UpdateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_CTZQ_Odds;
            if (t.Id != this.Id
                || t.LSWin != this.LSWin
                || t.LSLose != this.LSLose
                || t.LSFlat != this.LSFlat
                || t.KLWin != this.KLWin
                || t.KLLose != this.KLLose
                || t.KLFlat != this.KLFlat
                || t.YPSW != this.YPSW
                || t.AverageOdds != this.AverageOdds
                || t.HalfAverageOdds != this.HalfAverageOdds
                || t.FullAverageOdds != this.FullAverageOdds)
                return false;

            return true;
        }

    }
}
