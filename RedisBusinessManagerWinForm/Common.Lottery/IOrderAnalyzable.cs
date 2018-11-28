using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery
{
    /// <summary>
    /// 能进行订单分析的
    /// </summary>
    public interface IOrderAnalyzable
    {
        /// <summary>
        /// 检查一个单独号码的格式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="type">号码类型</param>
        /// <returns>格式是否正确</returns>
        bool CheckOneAntecodeNumber(string antecodeNumber, out string errMsg, string type = null);
        /// <summary>
        /// 检查一个组合号码的格式，如：时时彩中的复式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="spliter">分隔符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        bool CheckComboAntecodeNumber(string antecodeNumber, char? spliter, out string errMsg);
        /// <summary>
        /// 检查占位符的格式
        /// </summary>
        /// <param name="antecodeNumber">占位符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        bool CheckSpaceNumber(string antecodeNumber, out string errMsg);
    }
    public interface IAnteCodeChecker_Sport
    {
        /// <summary>
        /// 检查一个单独号码的格式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="type">号码类型</param>
        /// <returns>格式是否正确</returns>
        bool CheckAntecodeNumber(ISportAnteCode antecode, out string errMsg);
        /// <summary>
        /// 检查中奖号码格式是否正确
        /// </summary>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        bool CheckWinNumber(ISportResult winNumber, string gameType, out string errMsg);
    }
}
