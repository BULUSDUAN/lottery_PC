using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery
{
    /// <summary>
    /// 标识能够分析中奖号码的接口
    /// </summary>
    public interface IWinNumberAnalyzable
    {
        /// <summary>
        /// 检查中奖号码格式是否正确
        /// </summary>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        bool CheckWinNumber(string winNumber, out string errMsg);
        /// <summary>
        /// 检查号码以后，可以获取号码数组
        /// </summary>
        string[] WinNumbers { get; }
    }
}
