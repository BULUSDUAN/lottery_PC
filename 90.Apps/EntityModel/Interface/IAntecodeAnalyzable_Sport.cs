using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.Interface
{
    /// <summary>
    /// 标识能够分析投注号码的接口
    /// </summary>
    public interface IAntecodeAnalyzable_Sport
    {
        /// <summary>
        /// 过关基数
        /// </summary>
        int BaseCount { get; set; }
        /// <summary>
        /// 检查投注号码格式是否正确
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        bool CheckAntecode(ISportAnteCode[] antecodeList, out string errMsg);
        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        int AnalyzeAnteCode(ISportAnteCode[] antecodeList);
        /// <summary>
        /// 计算投注号码的中奖状态，返回中奖的奖等列表。如果为空，表示未中奖；
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="winNumber">中奖号码</param>
        /// <returns>返回中奖的奖等列表</returns>
        SportBonusResult CaculateBonus(ISportAnteCode[] antecodeList, ISportResult[] winNumberList);
    }
   
    public interface ISportResult
    {
        string GetMatchId(string gameCode);
        string GetMatchResult(string gameCode, string gameType, decimal offset = -1);
        string GetFullMatchScore(string gameCode);
    }
   
}
