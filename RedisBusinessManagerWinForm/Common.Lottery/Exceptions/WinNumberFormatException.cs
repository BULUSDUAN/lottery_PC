using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery
{
    /// <summary>
    /// 中奖号码格式错误异常
    /// </summary>
    public class WinNumberFormatException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="message">错误消息</param>
        public WinNumberFormatException(string gameCode, string winNumber, string message)
            : base(message)
        {
            GameCode = gameCode;
            WinNumber = winNumber;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">引起此异常的异常</param>
        public WinNumberFormatException(string gameCode, string winNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            GameCode = gameCode;
            WinNumber = winNumber;
        }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public string WinNumber { get; set; }
    }
}
