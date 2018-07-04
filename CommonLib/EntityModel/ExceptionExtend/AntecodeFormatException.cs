using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.ExceptionExtend
{
    /// <summary>
    /// 投注号码格式错误异常
    /// </summary>
    public class AntecodeFormatException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="gameType">玩法</param>
        /// <param name="antecode">投注号码</param>
        /// <param name="message">错误消息</param>
        public AntecodeFormatException(string gameCode, string gameType, string antecode, string message)
            : base(message)
        {
            GameCode = gameCode;
            GameType = gameType;
            Antecode = antecode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="gameType">玩法</param>
        /// <param name="antecode">投注号码</param>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">引起此异常的异常</param>
        public AntecodeFormatException(string gameCode, string gameType, string antecode, string message, Exception innerException)
            : base(message, innerException)
        {
            GameCode = gameCode;
            GameType = gameType;
            Antecode = antecode;
        }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public string Antecode { get; set; }
    }
}
